import { Component, Injector, ViewChild } from "@angular/core";
import {
    BookingTemplateCaptionEnum,
    EventRef,
} from "../../calendar.model";
import { CalendarService } from "../../calendar.service";
import { DarkModeService } from "@app/services/dark-mode.service";
import {
    AreaDto,
    BookingModel,
    BookingServiceProxy,
    CaptionDto,
    TemplateMessagesServiceProxy,
    UserListDto,
    UserServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ThemeHelper } from "../../../../../shared/layout/themes/ThemeHelper";
import moment from "moment";
import { CountryISO, SearchCountryField } from "ngx-intl-tel-input";
const { toGregorian } = require('hijri-converter');

@Component({
    selector: "app-calendar-event-sidebar",
    templateUrl: "./calendar-event-sidebar.component.html",
    styleUrls: ["./calendar-event-sidebar.component.css"],
})
export class CalendarEventSidebarComponent extends AppComponentBase {
    theme: string;
    //  Decorator

    @ViewChild("startDatePicker") startDatePicker;
    @ViewChild("endDatePicker") endDatePicker;

    public endDateOptions = {
        altInput: true,
        mode: "single",
        altInputClass:
            "form-control flat-picker flatpickr-input invoice-edit-input",
        enableTime: true,
        onClose: (selectedDates: any) => {
         
            this.getDate(selectedDates[0]);
        },
    };
    disabled = false;

    // Public
    public event: EventRef;
    public isDataEmpty = true;
    branches: AreaDto[];
    users: UserListDto[];
    createEvent = new BookingModel();
    eventDate = null;
    name = "";
    date = "";
    time = "";
    showArabic = true;
    submitted = false;
    submitted2 = false;
    saving = false;
    show = false;
    validPhoneNumber = true;
    isLoading = true;
    SearchCountryField = SearchCountryField;
    CountryISO = CountryISO;
    phoneNumber: any;
    BookingTemplateCaptionEnum: BookingTemplateCaptionEnum;
    dialCode: any;
    captinDetails: CaptionDto[];
    captionArabic = new CaptionDto();
    captionEnglish = new CaptionDto();
    templates: any[] = [];
    textArabic = "";
    textEnglish = "";

    preferredCountries: CountryISO[] = [CountryISO.SaudiArabia];
    languageEnum = [
        { id: "2", name: "English" },
        { id: "1", name: "Arabic" },
    ];

    constructor(
        public _calendarService: CalendarService,
        public darkModeService: DarkModeService,
        private _bookingServiceProxy: BookingServiceProxy,
        private templateMessagesServiceProxy: TemplateMessagesServiceProxy,
        private _userServiceProxy: UserServiceProxy,
        injector: Injector
    ) {
        super(injector);
    }

    addEvent() {
        this.saving = true;
        this.createEvent.tenantId = this.appSession.tenantId;
        if(!this.isAdmin){
            this.createEvent.userId= this.appSession.userId;
            this.createEvent.userName = this.appSession.user.name;
        }

        if (
            this.createEvent.userId === null ||
            this.createEvent.userId === undefined ||
            this.createEvent.areaId === null || 
            this.createEvent.areaId === undefined ||
            this.phoneNumber === null ||
            this.phoneNumber === undefined ||
            this.phoneNumber === "" ||
            // this.eventDate === null ||
            // this.eventDate === undefined ||
            // this.eventDate === "" ||
            this.endDatePicker.flatpickrElement.nativeElement.children[0]
                .value === "" ||
            this.createEvent.languageId === null ||
            this.createEvent.languageId === undefined
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        this.createEvent.phoneNumber = this.dialCode + this.phoneNumber.number;
        this.createEvent.bookingDateTime = moment(
            this.endDatePicker.flatpickrElement.nativeElement.children[0].value
        );
        this.createEvent.bookingDateTimeString =
            this.endDatePicker.flatpickrElement.nativeElement.children[0].value;
        this.createEvent.createdBy = this.appSession.userId;
        this.createEvent.statusId = 2;
        this.createEvent.bookingTypeId = 1;
        this.createEvent.isNew = false;
        this._bookingServiceProxy.createBooking(this.createEvent).subscribe(
            (res) => {
                if (res.toString() === "phonenumber_failed") {
                    this.notify.error(this.l("invalidPhone"));
                    this.saving = false;
                } else if (res.toString() === "booking_failed") {
                    this.notify.error(this.l("bookingFailed"));
                    this.saving = false;
                } else if (res.toString() === "send_failed") {
                    this.notify.error(this.l("sendFailed"));
                    this.saving = false;
                } else if (res.toString() === "template_reject") {
                    this.notify.error(this.l("templateReject"));
                    this.saving = false;
                } else if (res.toString() === "template_caption_failed") {
                    this.notify.error(this.l("captionFailed"));
                    this.saving = false;
                } else if (res.toString() === "time_failed") {
                    this.notify.error(this.l("dateFailed"));
                    this.saving = false;
                } else if (res.toString() === "bundle_failed") {
                    this.notify.error(this.l("bundleFailed"));
                    this.saving = false;
                } else if (res.toString() === "capacity_failed") {
                    this.notify.error(this.l("capacityFailed"));
                    this.saving = false;
                } else if (
                    res.toString() === "booking_success" ||
                    res.toString() === "send_success"
                ) {
                    // this._calendarService.getEvents();
                    this.notify.info(this.l("savedSuccessfully"));
                    this._calendarService.close();
                    this.saving = false;
                    this._calendarService.getEvents(null, null, null);
                }
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

    getCountryCode(event: any) {
        this.dialCode = event.dialCode;
    }
    getAllTemplates() {
        this.templateMessagesServiceProxy
            .getAllNoFilter()
            .subscribe((result) => {
                this.templates = result.items;
            });
    }

    /**
     * Update Event
     */
    updateEvent() {
        this.UpdatedEvent(this.createEvent);
    }

    deleteEvent() {
        if(this.createEvent.deletionReasonId === null || this.createEvent.deletionReasonId === undefined){
            this.submitted2 = true;
            this.saving = false;
            return;
        }else{
            this.message.confirm(
                "",
                this.l("areYouSureYouWannaDeleteThisEvent"),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this.createEvent.statusId = 5;
                        this.UpdatedEvent(this.createEvent);
                    }
                }
            );
        }
  
    }

    showDeleteMessage() {
        this.show = true;
        // this.createEvsent.deletionReasonId =;
        // this.createEvent.statusId = 5;
    }

    getTemplateMessageById(templateId) {
        // this.templateMessagesServiceProxy
        //     .getTemplateMessageForEdit(templateId.target.value)
        //     .subscribe((result) => {
        //         this.createEvent.deletionReason =
        //             result.templateMessage.messageText;
        //     });
    }

    getBranch() {
        this.branches = [];
        this._bookingServiceProxy
            .getBranchesByUserId(this.appSession.userId,this.isAdmin)
            .subscribe((result) => {
                this.branches = result;
            });
    }
    getUsers() {
        this.users = [];
        this._userServiceProxy
            .getBookingUsers(this.appSession.tenantId, null)
            .subscribe((result) => {
                
                this.users = result;
            });
    }

    changeBranch(event){
        if(this.isAdmin){
            this.users = [];
            let branch = new AreaDto;
             branch=   this.branches.find(x => x.id == event );
           this._bookingServiceProxy
            .getUserListByUserIds(branch.userIds)
            .subscribe((result) => {
                this.users = result; 
            });
        }
       
    }

  

    async ngOnInit() {
        await this.getIsAdmin();
        this.theme = ThemeHelper.getTheme();
        this.getBranch();
        if(this.isAdmin){
          this.getUsers();  
        }
        this.getAllTemplates();
        this.phoneNumber = null;
        this.createEvent = new BookingModel();
        this.isDataEmpty = true;
        this.eventDate = "";
        // this._calendarService.onCurrentEventChange.subscribe((response) => {
        //     if (this._calendarService.selectedEvent) {
        //         this.isDataEmpty = false;
        //         if (this._calendarService.selectedEvent === "") {
        //             this.isDataEmpty = true;
        //             this.eventDate = new Date();
        //             this.createEvent = new BookingModel();
        //         } else {
        //             this.getEventById(this._calendarService.selectedEvent);
        //         }
        //     } else {
        //         this.createEvent.bookingDateTime =
        //             this._calendarService.selectedEvent;
        //         this.createEvent = new BookingModel();
        //         this.isDataEmpty = true;
        //         this.eventDate = new Date();
        //     }
        // });
        this._calendarService.onCurrentEventChange.subscribe((response) => {
            this.isLoading = true;
            this.show = false;
            this.submitted = false;
            this.phoneNumber = null;

            if (
                this._calendarService.selectedEvent === null ||
                this._calendarService.selectedEvent === undefined
            ) {
                this.createEvent = response;
                // If Event is available
                if (Object.keys(response).length > 0) {
                    this.getBookingCaption();
                    this.showArabic = true;
                    this.createEvent = response;
                    let momentDate = moment(response.bookingDateTime).locale('en');
                    this.eventDate = momentDate.format("yyyy-MM-DDTHH:mm");
                    this.date = momentDate.format("yyyy-MM-DD");
                    this.time = momentDate.format("HH:mm");

                    this.isDataEmpty = true;
                    this.isLoading = false;
                } else {
                    this.eventDate = new Date();
                    this.isDataEmpty = true;
                    this.isLoading = false;
                }
                // else Create New Event
            } else {
                this.createEvent = new BookingModel();
                this.isLoading = false;
                if (this._calendarService.selectedEvent != null) {
                    this.isLoading = true;
                    this.getEventById(
                        this._calendarService.selectedEvent._def.publicId
                    );
                    setTimeout(() => {
                        // this.eventDate = new Date();
                        this.endDatePicker.flatpickr.clear();
                    });
                    this.isDataEmpty = false;
                }
            }
        });
    }

    getEventById(id) {
        this.eventDate = new Date();
        if (id) {
            this._bookingServiceProxy.getBookingById(id).subscribe((result) => {
                this.createEvent = result;
                this.createEvent.statusId = result.statusId;

                let year= result.bookingDateTime.year();
                if(year < 2000){
                    result.bookingDateTime= moment(this.convertHijriToGregorian(result.bookingDateTime)).locale('en');
                    this.eventDate =
                    result.bookingDateTime.format("yyyy-MM-DDTHH:mm");
                }else{
                    this.eventDate = moment(
                    result.bookingDateTime).locale('en').format("yyyy-MM-DDTHH:mm");
                }

                this.phoneNumber = this.createEvent.phoneNumber;
                this.isLoading = false;
                if (this.createEvent.statusId === 5) {
                    this.disabled = true;
                    this.show = true;
                }
                this.createEvent.customerId = result.customerId.replace(
                    /\s/g,
                    ""
                );
                if (this.createEvent.isNew) {
                    this.eventHasViewd(this.createEvent.bookingNumber);
                }
            });
        }
    }
    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString._i.split('T');
        const [year, month, day] = hijriDateString.split('-').map(Number);
        const gregorianDate = toGregorian(year, month, day);
    
        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, '0'),
            String(gregorianDate.gd).padStart(2, '0'),
        ].join('-');
    
        return formattedDate + 'T' + time;
    }
    eventHasViewd(eventNumber) {
        this._bookingServiceProxy
            .updateBookingIsNew(eventNumber)
            .subscribe((result) => {
                if (result === "Done") {
                    this._calendarService.getEvents(null, null, null);
                }
            });
    }

    confirm() {
        return new Promise((resolve, reject) => {
            this.saving = true;
            this.createEvent.statusId = 2;
            this.createEvent.isNew= false;
            this.createEvent.bookingDateTime = moment(
                this.endDatePicker.flatpickrElement.nativeElement.children[0].value
            );
            this.createEvent.bookingDateTimeString =
                this.endDatePicker.flatpickrElement.nativeElement.children[0].value;
                if(this.createEvent.createdOn.year() < 2000){
                    this.createEvent.createdOn= moment(this.convertHijriToGregorian(this.createEvent.createdOn)).locale('en');
                }
            this._bookingServiceProxy.updateBooking(this.createEvent).subscribe(
                (response) => {
                    if (
                        response.toString() === "update_success" ||
                        response.toString() === "send_success"
                    ) {
                        this._calendarService.getEvents(null, null, null);
                        this.notify.info(this.l("savedSuccessfully"));
                        this._calendarService.close();
                        this.saving = false;
                    } else if (
                        response.toString() === "update_failed" ||
                        response.toString() === "send_failed" ||
                        response.toString() === "template_reject" ||
                        response.toString() === "template_caption_failed"
                    ) {
                        this.notify.error(this.l("bookingFailed"));
                        this.saving = false;
                        this.submitted = false;
                    } else if (response.toString() === "time_failed") {
                        this.notify.error(this.l("dateFailed"));
                        this.saving = false;
                        this.submitted = false;
                    } else if (response.toString() === "capacity_failed") {
                        this.notify.error(
                            this.l("capacityFailed")
                        );
                        this.saving = false;
                        this.submitted = false;
                    } else if (response.toString() === "bundle_failed") {
                        this.notify.error(
                            this.l("bundleFailed")
                        );
                        this.saving = false;
                        this.submitted = false;
                    }
                },
                (error: any) => {
                    if (error) {
                        this.saving = false;
                        this.submitted = false;
                        this.notify.error(error);
                    }
                },
                reject
            );
        });
    }

    UpdatedEvent(event) {
        // this.createEvent.bookingDateTimeString = event.bookingDateTime._i.toString();
        this.createEvent.bookingDateTime = moment(
            this.endDatePicker.flatpickrElement.nativeElement.children[0].value
        );
        this.createEvent.bookingDateTimeString =
            this.endDatePicker.flatpickrElement.nativeElement.children[0].value;
        return new Promise((resolve, reject) => {
            this.saving = true;
            this.createEvent.tenantId = this.appSession.tenantId;
            this.createEvent.isNew = false;
            event.isNew = false;
            if(this.createEvent.createdOn.year() < 2000){
                this.createEvent.createdOn= moment(this.convertHijriToGregorian(this.createEvent.createdOn)).locale('en');
            }
            this._bookingServiceProxy.updateBooking(event).subscribe(
                (response) => {
                    if (
                        response.toString() === "update_success" ||
                        response.toString() === "send_success"
                    ) {
                        this._calendarService.getEvents(null, null, null);
                        this.notify.info(this.l("savedSuccessfully"));
                        this._calendarService.close();
                        this.saving = false;
                        this.submitted = false;
                        resolve(response);
                    } else if (response.toString() === "update_failed") {
                        this.notify.error(this.l("bookingFailed"));
                        this.saving = false;
                        this.submitted = false;
                    } else if (response.toString() === "time_failed") {
                        this.notify.error(this.l("timeFailed"));
                        this.saving = false;
                        this.submitted = false;
                    } else if (response.toString() === "send_failed") {
                        this.notify.error(this.l("sendFailed"));
                        this.saving = false;
                        this.submitted = false;
                    } else if (response.toString() === "template_reject") {
                        this.notify.error(this.l("templateReject"));
                        this.saving = false;
                        this.submitted = false;
                    } else if (
                        response.toString() === "template_caption_failed"
                    ) {
                        this.notify.error(this.l("captionFailed"));
                        this.saving = false;
                        this.submitted = false;
                    } else if (response.toString() === "capacity_failed") {
                        this.notify.error(
                            this.l("capacityFailed")
                        );
                        this.saving = false;
                        this.submitted = false;
                    } else if (response.toString() === "bundle_failed") {
                        this.notify.error(
                            this.l("bundleFailed")
                        );
                        this.saving = false;
                        this.submitted = false;
                    }
                },
                (error: any) => {
                    if (error) {
                        this.saving = false;
                        this.submitted = false;
                        this.getEventById(this.createEvent.id);
                        this.notify.error(error);
                    }
                },
                reject
            );
        });
    }

    getBookingCaption() {
        this._bookingServiceProxy
            .getBookingCaption(1253, this.appSession.tenantId)
            .subscribe((result) => {
                this.captinDetails = result;
                this.captinDetails.forEach((caption) => {
                    if (caption.languageBotId === 1) {
                        this.captionArabic = caption;
                    } else {
                        this.captionEnglish = caption;
                    }
                });
                this.textArabic = this.captionArabic.text.replace(
                    "{1}",
                    this.date
                );
                this.textArabic = this.textArabic.replace("{2}", this.time);
                this.textEnglish = this.captionEnglish.text.replace(
                    "{1}",
                    this.date
                );
                this.textEnglish = this.textEnglish.replace("{2}", this.time);
            });
    }

    getName(name) {
        if (name.target.value != null) {
            this.textArabic = this.captionArabic.text.replace(
                "{0}",
                this.createEvent.customerName
            );
            this.textArabic = this.textArabic.replace("{1}", this.date);
            this.textArabic = this.textArabic.replace("{2}", this.time);
            this.textEnglish = this.captionEnglish.text.replace(
                "{0}",
                this.createEvent.customerName
            );
            this.textEnglish = this.textEnglish.replace("{1}", this.date);
            this.textEnglish = this.textEnglish.replace("{2}", this.time);
        }
    }

    getDate(date) {
        if (date != null) {
            let momentDate = moment(date);
            this.date = momentDate.format("yyyy-MM-DD");
            this.time = momentDate.format("HH:mm");
            this.textArabic = this.captionArabic.text.replace("{1}", this.date);
            this.textArabic = this.textArabic.replace("{2}", this.time);
            this.textArabic = this.textArabic.replace(
                "{0}",
                this.createEvent.customerName
            );
            this.textEnglish = this.captionEnglish.text.replace(
                "{1}",
                this.date
            );
            this.textEnglish = this.textEnglish.replace("{2}", this.time);
            this.textEnglish = this.textEnglish.replace(
                "{0}",
                this.createEvent.customerName
            );
        }
    }

    changeLanguage(language) {
        if (language === "1") {
            this.showArabic = true;
        } else {
            this.showArabic = false;
        }
    }
}
