import { Component, Injector, ViewEncapsulation, ViewChild, IterableDiffers } from '@angular/core';
import {   BookingServiceProxy, TenantServiceProxy, TenantEditDto  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import {  CreateOrEditBookingModalComponent } from './create-or-edit-booking-modal.component';
import {  ViewBookingModalComponent } from './view-booking-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import * as moment from 'moment';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';

@Component({
    templateUrl: './booking.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class BookingComponent extends AppComponentBase {
    theme:string;
    showDropDownList= false;
    @ViewChild('createOrEditBookingModal', { static: true }) createOrEditBookingModal: CreateOrEditBookingModalComponent;
    @ViewChild('viewBookingModalComponent', { static: true }) viewBookingModal: ViewBookingModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    tenantEditDto:TenantEditDto;
    constructor(
        differs: IterableDiffers,
        injector: Injector,
        public darkModeService : DarkModeService,

    ) {
        super(injector);
    }





    ngOnInit(): void {
       
    }


    
    
}
