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
    UserServiceProxy,
    UserTicketsModel,
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
    selector: "userTicketsModal",
    templateUrl: "./user-tickets-modal.component.html",
    styleUrls: ["user-tickets-modal.component.less"],
})
export class UserTicketsModalComponent extends AppComponentBase {
    theme: string;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("userTicketsModal", { static: true })
    modal: ModalDirective;
    public dates: BookingOffDaysEntity;
    date: BookingOffDays;
    unAvailableBookingDates: any = [];
    listUanvailableDates = [];
    userId: number;

    listOfDates = [];
    submitted = false;
    saving = false;
maxOpenedTickets:number;
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
        private _userService: UserServiceProxy,
        private _bookingServiceProxy: BookingServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
    }

    show(userId?: number): void {

        this.userId=userId;
        this._userService
            .userTicketsGet(userId)
            .subscribe((response:UserTicketsModel) => {
               
         
                this.maxOpenedTickets= response.maximumTickets;
                this.modal.show();
            });
    }





    close(): void {
        this.modal.hide();
        this.saving = false;
        this.submitted = false;
        this.modalSave.emit(null);
    }

save() {
  this.submitted = true;

  if (this.maxOpenedTickets === null || this.maxOpenedTickets === undefined || this.maxOpenedTickets < 0) {
    return; 
  }

  this.saving = true;
  this._userService.userTicketsUpdate(this.userId, this.maxOpenedTickets).subscribe(
    (res) => {
      this.notify.info(this.l("savedSuccessfully"));
      this.close();
    },
    (error: any) => {
      this.saving = false;
      this.submitted = false;
      this.notify.error(error);
    }
  );
}
}
