import { Component, Injector, ViewChild } from "@angular/core";
import {
    CalendarOptions,
    EventClickArg,
    FullCalendarComponent,
} from "@fullcalendar/angular";
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../../services/dark-mode.service";
import { CalendarService } from "../calendar/calendar.service";
import { EventRef } from "../calendar/calendar.model";
import { AppComponentBase } from "@shared/common/app-component-base";
import { CalendarEventSidebarComponent } from "./calendar-sidebar/calendar-event-sidebar/calendar-event-sidebar.component";
import moment, { Moment } from "moment";
import { Subject, Subscription } from "rxjs";
import { SocketioService } from "@app/shared/socketio/socketioservice";
import { BookingModel } from "@shared/service-proxies/service-proxies";
import { VideoModelComponent } from "@app/main/videoComponent/video-model.component";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { PlyrComponent } from "ngx-plyr";
import * as rtlDetect from 'rtl-detect';
import allLocales from '@fullcalendar/core/locales-all';

@Component({
    templateUrl: "./calendar.component.html",
    styleUrls: ["./calendar.component.css"],
})
export class CalendarComponent extends AppComponentBase  {
    @ViewChild("calendarEvent", { static: true })
    calendarEvent: CalendarEventSidebarComponent;
    @ViewChild("calendar", { static: true })
    calendarComponent: FullCalendarComponent;

    @ViewChild("viewVideo", { static: true })
    viewVideo: VideoModelComponent;
    public plyr: PlyrComponent;
    public player: Plyr;
    public plyrOptions = { tooltips: { controls: true } };
    theme: string;
    public slideoutShow = false;
    public events = [];
    public event;
    dateRange: [Moment, Moment];
    calendarApi: any;
    calendarService2: CalendarService;
    botBooking: Subscription;
    start = "";
    end = "";

    videoLink =  '../../../assets/Calendar.mp4'
    // video Sources
    public videoSources: Plyr.Source[] = [
        {
          src:
            'https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-576p.mp4',
          type: 'video/mp4',
          size: 576
        },
        {
          src:
            'https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-720p.mp4',
          type: 'video/mp4',
          size: 720
        },
        {
          src:
            'https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-1080p.mp4',
          type: 'video/mp4',
          size: 1080
        },
        {
          src:
            'https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-1440p.mp4',
          type: 'video/mp4',
          size: 1440
        }
      ];
  
    public calendarOptions: CalendarOptions = {
        headerToolbar: {
            left: "prev,next today,",
            center: "title",
            right: "dayGridMonth,timeGridWeek,timeGridDay",
        },
        initialView: "dayGridMonth",
        initialEvents: this.events,
        weekends: true,
        editable: false,
        locale: abp.localization.currentLanguage.name,
        locales: allLocales,
        eventResizableFromStart: false,
        selectable: true,
        fixedWeekCount: false,
        allDaySlot: false,
        slotDuration: "01:00:00",
        selectMirror: true,
        eventTimeFormat: {
            // like '14:30'
            hour: "numeric",
            minute: "2-digit",
            meridiem: "short",
        },
        aspectRatio: 3,
        longPressDelay: 1,
        dayMaxEvents: 3,
        height: 850,
        datesSet: this.handleMonthChange.bind(this),
        navLinks: true,
        eventClick: this.handleUpdateEventClick.bind(this),
        eventClassNames: this.icons.bind(this),
        select: this.handleDateSelect.bind(this),
    };
    private _unsubscribeAll: Subject<any>;
    isArabic= false;


    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        public _calendarService: CalendarService,
        private socketioService: SocketioService,
        private modalService: NgbModal,

    ) {
        super(injector);
        this._unsubscribeAll = new Subject();
    }

    async handleMonthChange(payload) {
        if (this.start != payload.startStr || this.end != payload.endStr) {
            this.start = payload.startStr;
            this.end = payload.endStr;
            await this._calendarService.getEvents(
                null,
                null,
                payload.startStr,
                payload.endStr
            );
        }
    }

    // prevMonth(eventRef) {
    //     const calendarApi = this.calendarComponent.getApi();
    //     calendarApi.prev();
    //     let prevDate = calendarApi.currentData.currentDate;
    //     this._calendarService.getEvents("", prevDate);
    // }

    // nextMonth(eventRef) {
    //     const calendarApi = this.calendarComponent.getApi();
    //     calendarApi.next();
    //     let nextDate = calendarApi.currentData.currentDate;
    //     this._calendarService.getEvents("", nextDate);
    // }

    icons(s) {
        const calendarIsNew = {
            true: "blob-danger",
        };
        const calendarsColor = {
            1: "danger",
            2: "success",
            3: "info",
            4: "warning",
            5: "secondary",
        };

        const isNew = calendarIsNew[s.event._def.extendedProps.isNew];
        const colorName = calendarsColor[s.event._def.extendedProps.statusId];
        return ` ${isNew} ${colorName}`;
        // return ` blob ${colorName}`;
    }
    subscribeBooking = () => {
        this.botBooking = this.socketioService.Booking.subscribe(
            (data: BookingModel) => {
                if (data.tenantId == this.appSession.tenantId) {
                    this._calendarService.getEvents(null, null, null, null);
                    this._calendarService.onEventChange.subscribe((res) => {
                        this.events = res;
                        this.calendarOptions.events = res;
                    });
                }
            }
        );
    };

    handleDateSelect(eventRef) {
        const newEvent = new EventRef();
        newEvent.bookingDateTime = eventRef.start;
        this._calendarService.addEventt(eventRef);
        this._calendarService.onCurrentEventChange.next(newEvent);
    }
    handleUpdateEventClick(eventRef: EventClickArg) {
        this._calendarService.addEventt(eventRef);
    }
    openVideo(modalBasic){
        this.modalOpen2(modalBasic);
        this.videoSources = [
            {
              src: this.videoLink,
              type: 'video/mp4',
              size: 576
            },
            {
              src:
              this.videoLink,
              type: 'video/mp4',
              size: 720
            },
            {
              src:
              this.videoLink,
              type: 'video/mp4',
              size: 1080
            },
            {
              src:
              this.videoLink,
              type: 'video/mp4',
              size: 1440
            }
          ];
    }
    modalOpen2(modalBasic) {
        this.modalService.open(modalBasic, {
          windowClass: 'modal',
          centered: true,
          size: 'lg'
        });
      }
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
   
        this.subscribeBooking();
        // setTimeout(() => {
        //     this.calendarComponent.getApi().render();
        // });
        this._calendarService.onEventChange.subscribe((res) => {
            this.events = res;
            this.calendarOptions.events = res;
        });

        this._calendarService.onEventChange.subscribe((res) => {
            this.events = res;
            this.calendarOptions.events = res;
        });
        setTimeout(() => {
            if (this._calendarService.calendar) {
                this.events = this._calendarService.calendar;
                this.calendarOptions.events = this._calendarService.calendar;
            }
        }, 2000);
    }
}
