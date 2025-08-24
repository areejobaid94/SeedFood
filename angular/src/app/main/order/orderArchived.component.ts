import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';

import { OrdersServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

import { ViewOrderModalComponent } from './view-order-modal.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { AppSessionService } from '@shared/common/session/app-session.service';

import { Subscription } from 'rxjs';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';
import * as rtlDetect from "rtl-detect";

@Component({
    templateUrl: './orderArchived.component.html',
    encapsulation: ViewEncapsulation.None,
    styleUrls: ['./orderArchived.component.less'],
})

export class OrdersArchivedComponent extends AppComponentBase {


    theme: string;


    // @ViewChild('createOrEditOrderModal', { static: true }) createOrEditOrderModal: CreateOrEditOrderModalComponent;
    @ViewChild('viewOrderModalComponent', { static: true }) viewOrderModal: ViewOrderModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '-1';
    nameFilter2 = '';
    currency = "";
    orderNameFilter = '';
    orderDescriptionFilter = '';
    maxEffectiveTimeFromFilter: moment.Moment;
    minEffectiveTimeFromFilter: moment.Moment;
    maxEffectiveTimeToFilter: moment.Moment;
    minEffectiveTimeToFilter: moment.Moment;
    maxTaxFilter: number;
    maxTaxFilterEmpty: number;
    minTaxFilter: number;
    minTaxFilterEmpty: number;
    imageUriFilter = '';
    ifSameUser = true;
    appSession: AppSessionService;
    items: any;
    agentOrderSub: Subscription;
    botOrderSub: Subscription;
    change: any;
    differ: any[];
    isArabic = false;


    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private _ordersServiceProxy: OrdersServiceProxy,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        await this.getIsAdmin()

    }




    getOrders(event?: LazyLoadEvent) {
        this.currency = this.appSession.tenant.currencyCode;

        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this._ordersServiceProxy.getAll(this.nameFilter, this.appSession.user.id, null, null, this.nameFilter2, null, null, null, null, null, null, null, null, null, null, null, null, true,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)).subscribe(result => {
                result.items.forEach(element => {


                    element.order.stringTotal = element.order.total.toString();
                    const [day, month, year] = element.creatDate.split('/');
                    element.creatDate = day + "/" + month + "/" + year.slice(2);


                });


                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;



            });
    }


    reloadPage(): void {


        this.paginator.changePage(this.paginator.getPage());
    }

    exportToExcel(): void {

        this._ordersServiceProxy.getOrderToExcel(this.nameFilter, this.appSession.user.id, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true,
            this.primengTableHelper.getSorting(this.dataTable),
            0,
            10000000)
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }



}


