import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { FullCalendarModule } from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';
import listPlugin from '@fullcalendar/list';
import timeGridPlugin from '@fullcalendar/timegrid';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { CalendarEventSidebarComponent } from './calendar-sidebar/calendar-event-sidebar/calendar-event-sidebar.component';
import { CalendarMainSidebarComponent } from './calendar-sidebar/calendar-main-sidebar/calendar-main-sidebar.component';

import { CalendarComponent } from './calendar.component';
import { CalendarService } from './calendar.service';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { NgxIntlTelInputModule } from 'ngx-intl-tel-input'
import { Ng2FlatpickrModule } from 'ng2-flatpickr';
import { PlyrModule } from 'ngx-plyr';
import { UtilsModule } from "../../../../shared/utils/utils.module";

FullCalendarModule.registerPlugins([dayGridPlugin, timeGridPlugin, listPlugin, interactionPlugin]);

// routing


@NgModule({
    declarations: [CalendarComponent, CalendarEventSidebarComponent, CalendarMainSidebarComponent],
    providers: [CalendarService],
    imports: [
        CommonModule,
        RouterModule,
        FullCalendarModule,
        CommonModule,
        NgMultiSelectDropDownModule,
        FormsModule,
        ReactiveFormsModule,
        NgxIntlTelInputModule,
        NgSelectModule,
        NgbModule,
        Ng2FlatpickrModule,
        PlyrModule,
        UtilsModule
    ]
})
export class CalendarrModule {}
