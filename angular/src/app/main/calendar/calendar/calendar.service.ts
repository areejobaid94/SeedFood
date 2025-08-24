import { Injectable, Injector } from "@angular/core";
import { Resolve } from "@angular/router";
import { AppComponentBase } from "@shared/common/app-component-base";
import { BookingServiceProxy } from "@shared/service-proxies/service-proxies";
import moment, { Moment } from "moment";
import { BehaviorSubject, Observable } from "rxjs";
import { EventRef } from "../calendar/calendar.model";
const { toGregorian } = require('hijri-converter');

@Injectable({
    providedIn: "root",
})
export class CalendarService extends AppComponentBase implements Resolve<any> {
    // Public
    isOpened: boolean = false;
    public events;
    public calendar;
    public currentEvent;
    public tempEvents;
    selectedFilters: Array<any> = [];
    selectedBranchesFilters: Array<any> = [];
    selectedUsersFilters:Array<any> = [];
    totalBooked = 0;
    totalCanceled = 0;
    totalConfirmed = 0;
    totalCount = 0;
    totalPending = 0;
    selectedEvent: any;
    isInitialDate: false;
    from: Moment;
    to: Moment;
    filter: string;
    branchesFilter: string;
    usersFilter;

    public onEventChange: BehaviorSubject<any>;
    public onCurrentEventChange: BehaviorSubject<any>;
    public onCalendarChange: BehaviorSubject<any>;
    dateRange: [Moment, Moment];

    /**
     * Constructor
     *
     */
    constructor(
        private _bookingServiceProxy: BookingServiceProxy,
        injector: Injector
    ) {
        super(injector);
        this.onEventChange = new BehaviorSubject({});
        this.onCurrentEventChange = new BehaviorSubject({});
        this.onCalendarChange = new BehaviorSubject({});
    }

    resolve(): Observable<any> | Promise<any> | any {
        return new Promise((resolve, reject) => {
            Promise.all([this.getEvents(null, null, null, null,null)]).then(
                (res) => {
                    resolve(res);
                },
                reject
            );
        });
    }

    getEvents(bracnhesFilter?, filter?, startPeriod?, endPeriod?,usersIds?) {
        if (startPeriod != null && endPeriod != null) {
            this.from = moment(startPeriod);
            this.to = moment(endPeriod);
        }
        if (filter != null) {
            this.filter = filter;
        }
        if (bracnhesFilter != null) {
            this.branchesFilter = bracnhesFilter;
        }
        if (usersIds != null) {
            this.usersFilter = usersIds;
        }
        let userId = 0;
        if (this.isAdmin) {
            userId = 0;
        } else {
            userId = this.appSession.userId;
        }
        let from = this.from.locale("en");
        let to = this.to.locale("en");
            this._bookingServiceProxy
            .getBooking(
                this.filter,
                from.format("MM/DD/yyyy"),
                to.format("MM/DD/yyyy"),
                this.branchesFilter,
                this.appSession.tenantId,
                this.appSession.userId,
                this.usersFilter
            )
            .subscribe((response: any) => {
                this.totalBooked = response.totalBooked;
                this.totalCanceled = response.totalCanceled;
                this.totalConfirmed = response.totalConfirmed;
                this.totalCount = response.totalCount;
                this.totalPending = response.totalPending;

                this.events = response.lstBookingModel;
                this.events.forEach((item) => {
                    let year = item.bookingDateTime.year();
                    if(year < 2000){
                    item.bookingDateTime = moment(this.convertHijriToGregorian(item.bookingDateTime._i)).locale("en");
                    }
                    item.start = item.bookingDateTime._i;
                    item.end = item.bookingDateTime._i;
                    if (item.bookingTypeId === 2) {
                        // item.title = item.customerName + '' + '🤖';
                        item.title = item.customerName;
                    } else {
                        item.title = item.customerName;
                    }
                });
                this.calendar = this.events;
                this.tempEvents = this.events;
                this.onEventChange.next(this.events);
            });
    }
    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split('T');
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
    // getCalendar(filter?, period?) {
    //     let now = moment(period);
    //     let firstDayOfCurrentMonth = new Date(now.year(), now.month(), 1);
    //     let lastDayOfCurrentMonth = new Date(now.year(), now.month() + 1, 0);
    //     let fisrtDate = moment(firstDayOfCurrentMonth);
    //     let lastDate = moment(lastDayOfCurrentMonth);
    //     this.dateRange = [fisrtDate, lastDate];
    //     this._bookingServiceProxy
    //     .getBooking(this.filter, this.from.format('MM/DD/yyyy'), this.to.format('MM/DD/yyyy'),this.branchesFilter,this.appSession.tenantId,userId)
    //     .subscribe((response: any) => {
    //             this.totalBooked = response.totalBooked;
    //             this.totalCanceled = response.totalCanceled;
    //             this.totalConfirmed = response.totalConfirmed;
    //             this.totalCount = response.totalCount;
    //             this.totalPending = response.totalPending;
    //             this.events = response.lstBookingModel;
    //             this.events.forEach((item) => {
    //                 item.start = item.bookingDateTime._i;
    //                 item.end = item.bookingDateTime._i;
    //                 item.title = item.customerName;
    //             });
    //             this.calendar = this.events;
    //             this.tempEvents = this.events;
    //             this.onEventChange.next(this.events);
    //         });
    // }
    createNewEvent() {
        this.currentEvent = {};
        this.onCurrentEventChange.next(this.currentEvent);
    }

    calendarUpdate(calendars) {
        const calendarsChecked = calendars.filter((calendar) => {
            return calendar.checked === true;
        });

        if(calendarsChecked.length === 0 ){
            this.getEvents(null,"0",null,null,null);
            return;
        }
        this.selectedFilters = calendarsChecked
            .filter((f) => f.id > 0)
            .map(({ id }) => id)
            .toString();
        this.getEvents(null, this.selectedFilters, null, null,null);
    }

    calendarUpdateBranches(calendars) {
        const calendarsChecked = calendars.filter((calendar) => {
            return calendar.checked === true;
        });
        if(calendarsChecked.length === 0 ){
            this.getEvents("0",null,null,null,null);
            return;
        }
        this.selectedBranchesFilters = calendarsChecked
            .filter((f) => f.id > 0)
            .map(({ id }) => id)
            .toString();

        this.getEvents(this.selectedBranchesFilters, null, null, null,null);
    }

    calendarUpdateUsers(calendars) {
        const calendarsChecked = calendars.filter((calendar) => {
            return calendar.checked === true;
        });
        if(calendarsChecked.length === 0 ){
            this.getEvents(null,null,null,null,"0");
            return;
        }
        this.selectedUsersFilters = calendarsChecked
            .filter((f) => f.id > 0)
            .map(({ id }) => id)
            .toString();

        this.getEvents(null, null, null, null,this.selectedUsersFilters);
    }

    addEvent(eventForm) {
        const newEvent = new EventRef();
        newEvent.customerName = eventForm.customerName;
        newEvent.phoneNumber = eventForm.phoneNumber;
        newEvent.bookingDateTime = eventForm.bookingDateTime;
        newEvent.bookingStatus = eventForm.bookingStatus;
        this.currentEvent = newEvent;
        this.onCurrentEventChange.next(this.currentEvent);
    }

    updateCurrentEvent(eventRef) {
        const newEvent = new EventRef();
        newEvent.customerName = eventRef.customerName;
        newEvent.phoneNumber = eventRef.phoneNumber;
        newEvent.bookingDateTime = eventRef.bookingDateTime;
        newEvent.bookingStatus = eventRef.bookingStatus;
        this.currentEvent = newEvent;
        this.onCurrentEventChange.next(this.currentEvent);
        this.onCurrentEventChange.next(this.currentEvent);
    }

    addEventt(eventRef?) {
        this.toggleOpen(eventRef);
        this.createNewEvent();
    }

    close(): void {
        // If sidebar is not open or collapsible, then return
        if (this.isOpened) {
            this.isOpened = false;
            this.selectedEvent = null;
        }
    }
    open(event): void {
        // If sidebar already open or collapsible, then return
        if (!this.isOpened) {
            if (event) {
                if (event.event) {
                    if (event.event._def.publicId) {
                        this.selectedEvent = event.event;
                    }
                }
            }
            this.isOpened = true;
        }
    }

    toggleOpen(event?): void {
        if (this.isOpened) {
            this.close();
        } else {
            this.open(event);
        }
    }
}
