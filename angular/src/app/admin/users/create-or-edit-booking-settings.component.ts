import {
    Component,
    EventEmitter,
    Injector,
    Output,
    ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ModalDirective } from "ngx-bootstrap/modal";
import * as _ from "lodash";

import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "@app/services/dark-mode.service";
import moment from "moment";
import { FlatpickrOptions } from "ng2-flatpickr";
import {
    BookingOffDays,
    BookingOffDaysEntity,
    BookingServiceProxy,
} from "@shared/service-proxies/service-proxies";

export class BookingOff {
    id: number;
    day: string | undefined;
    startTime: string | undefined;
    endTime: string | undefined;
    tenantId: number;
    userId: number;
}
@Component({
    selector: "createOrEditBookingSettings",
    templateUrl: "./create-or-edit-booking-settings.component.html",
    styleUrls: ["./create-or-edit-booking-settings.component.css"],
})
export class CreateOrEditBookingSettingsComponent extends AppComponentBase {
    theme: string;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("createOrEditBookingSettings", { static: true })
    modal: ModalDirective;
    public dates: BookingOffDaysEntity;
    date: BookingOffDays;
    unAvailableBookingDates: any = [];
    listUanvailableDates = [];
    userId: any;

    listOfDates = [];
    submitted = false;
    saving = false;
    public multipleDateOptions: FlatpickrOptions = {
        altInput: true,
        mode: "multiple",
        dateFormat: "d.m.Y",
    };
    public timeOptions: FlatpickrOptions = {
        enableTime: true,
        noCalendar: true,
        altInput: true,
    };

    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private _bookingServiceProxy: BookingServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
    }

    show(userId?: number): void {
        this.userId = userId;
        this._bookingServiceProxy
            .getBookingOffDays(this.userId)
            .subscribe((response: any) => {
                this.listOfDates = response;
                if (this.listOfDates) {
                    this.listOfDates.forEach((item) => {
                        if (item.day != "" && item.day != null) {
                            var array2 = item.day.split(",");
                            item.day = [];
                            array2.forEach((element) => {
                                let x = new Date(element);
                                item.day.push(x);
                            });

                            item.startTime = moment(
                                "2014-02-27T" + item.startTime
                            ).locale('en').format("HH:mm");
                            item.endTime = moment(
                                "2014-02-27T" + item.endTime
                            ).locale('en').format("HH:mm");

                            // item.endTime = moment(end).format("HH:mm:ss");
                        } else {
                            item.day = [];
                        }
                    });
                }
                this.dates = response;
                this.modal.show();
            });
    }

    deleteItem(id) {
        for (let i = 0; i < this.listOfDates.length; i++) {
            if (this.listOfDates.indexOf(this.dates[i]) === id) {
                this.listOfDates.splice(i, 1);
                break;
            }
        }
    }

    addItem() {
        if (this.listOfDates.length > 0) {
            let lastItem = this.listOfDates.slice(-1);
            if (
                (!lastItem[0].isOffDayBooking &&
                    (lastItem[0].startTime === null ||
                        lastItem[0].startTime === undefined ||
                        lastItem[0].startTime === "" ||
                        lastItem[0].endTime === null ||
                        lastItem[0].endTime === undefined ||
                        lastItem[0].endTime === "" ||
                        lastItem[0].startTime >= lastItem[0].endTime)) ||
                lastItem[0].day === null ||
                lastItem[0].day === undefined
            ) {
                this.submitted = true;
                return;
            }
        }
       

        this.listOfDates.push({
            id: 0,
            day: "",
            startTime: "",
            endTime: "",
            tenantId: this.appSession.tenantId,
            userId: this.userId,
            isOffDayBooking: false,
        });  
     
      
    }

    close(): void {
        this.modal.hide();
        this.saving = false;
        this.submitted = false;
        this.modalSave.emit(null);
    }

    save() {
        if (this.listOfDates.length > 0) {
            let lastItem = this.listOfDates.slice(-1);
            if (
                (!lastItem[0].isOffDayBooking &&
                    (lastItem[0].startTime === null ||
                        lastItem[0].startTime === undefined ||
                        lastItem[0].startTime === "" ||
                        lastItem[0].endTime === null ||
                        lastItem[0].endTime === undefined ||
                        lastItem[0].endTime === "" ||
                        lastItem[0].startTime >= lastItem[0].endTime)) ||
                lastItem[0].day === null ||
                lastItem[0].day === undefined
            ) {
                if (lastItem[0].isOffDay) {
                } else {
                    this.submitted = true;
                    return;
                }
            }
        }
        this.saving = true;
        this.dates = new BookingOffDaysEntity();
        this.dates.bookingOffDays = [new BookingOffDays()];
        this.listOfDates.forEach((datee) => {
            this.listUanvailableDates = [];
            this.date = new BookingOffDays();
            if(datee.startTime){
                if (datee.startTime.length === 1) {
                    this.date.startTime = moment(datee.startTime[0]).locale('en').format(
                        "HH:mm"
                    );
                } else{
                    this.date.startTime = datee.startTime;

                }
            }
            else {
                this.date.startTime = moment("2023/10/04 1:00").locale('en').format("HH:mm");
            }
         
            if(datee.endTime){
                if (datee.endTime.length === 1) {
                    this.date.endTime = moment(datee.endTime[0]).locale('en').format("HH:mm");
            }else{
                this.date.endTime = datee.endTime;
            }
        } else {
            this.date.endTime = moment("2023/10/04 1:20").locale('en').format("HH:mm");
        }
           
            datee.day.forEach((unAvailableBookingDate) => {
                let date = moment(unAvailableBookingDate).locale('en').format("MM/DD/yyyy");
                this.listUanvailableDates.push(date);
            });
            this.date.day = this.listUanvailableDates.join();
            this.date.tenantId = this.appSession.tenantId;
            this.date.userId = this.userId;
            this.date.isOffDayBooking = datee.isOffDayBooking;
            this.dates.bookingOffDays.push(this.date);
        });
        this.dates.tenantId = this.appSession.tenantId;
        this.dates.userId = this.userId;
        this.dates.bookingOffDays.shift();
        this._bookingServiceProxy.createBookingOffDays(this.dates).subscribe(
            (res) => {
                this.notify.info(this.l("savedSuccessfully"));
                this.close();
            },
            (error: any) => {
                if (error) {
                    this.saving = false;
                    this.submitted = false;
                    this.notify.error(error);
                }
            }
        );
    }
}
