import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';

import { OrdersServiceProxy, OrderDto, GetOrderForViewDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

import { ViewOrderModalComponent } from './view-order-modal.component';
import { Table } from 'primeng/table';
import { MultiSelect } from 'primeng/multiselect';

import { Paginator } from 'primeng/paginator';
import { DatePipe } from '@angular/common'

import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { Subscription } from 'rxjs';
import { SocketioService } from '@app/shared/socketio/socketioservice';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';
import { ViewLoyaltyOrdersComponent } from './view-loyalty-orders.component';
import * as rtlDetect from "rtl-detect";

@Component({
    templateUrl: './order.component.html',
    encapsulation: ViewEncapsulation.None,
    styleUrls: ['./order.component.less'],
})

export class OrdersComponent extends AppComponentBase {
    theme: string;

    @ViewChild('viewOrderModalComponent', { static: true }) viewOrderModal: ViewOrderModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('multiSelect', { static: true }) multiSelect: MultiSelect;

    currency = "";
    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '-1';
    nameFilter2 = '';
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
    cols: any[];
    _selectedColumns: any[];
    showAgent = true;
    showOrder = true;
    showCustomer = true;
    showDate = true;
    showTime = true;
    showStatus = true;
    showTotal = true;
    showActionTime = true;
    showType = true;
    showBranch = true;
    isArabic = false;



    // order:OrderDto;
    constructor(
        injector: Injector,
        private socketioService: SocketioService,
        public datepipe: DatePipe,
        private _ordersServiceProxy: OrdersServiceProxy,
        private _fileDownloadService: FileDownloadService,
        public darkModeService: DarkModeService,

    ) {
        super(injector);
    }



    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );

        // this.order=null;
        // this.socketioService.setupSocketConnection();
        this.subscribeAgentOrder();
        this.subscribeBotOrder();
        await this.getIsAdmin()
    }




    getOrders(event?: LazyLoadEvent) {

        this.currency = this.appSession.tenant.currencyCode;

        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        //  this.paginator.rows = 5;
        //this.number=this.primengTableHelper.getMaxResultCount(this.paginator, event);
        //this.appSession.user.id
        this._ordersServiceProxy.getAll(this.nameFilter, this.appSession.user.id, null, null, this.nameFilter2, null, null, null, null, null, null, null, null, null, null, null, null, false,
            this.primengTableHelper.getSorting(this.dataTable),
            event?.first,
            event?.rows).subscribe(result => {
                result.items.forEach(element => {

                    //var x= (Math.round(element.order.total * 100) / 100).toFixed(2);

                    element.order.stringTotal = element.order.total.toString();
                    const [day, month, year] = element.creatDate.split('/');
                    element.creatDate = day + "/" + month + "/" + year.slice(2);

                    // let n = 3;

                    // element.customerMobile =   element.customerMobile.substring(n);
                    //  this.items.push();
                });

                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;

            });
    }

    subscribeAgentOrder = () => {


        this.agentOrderSub = this.socketioService.BotOrder.subscribe((data: GetOrderForViewDto) => {

            if (data.tenantId == this.appSession.tenantId) {
                try {
                    const index = this.primengTableHelper.records.findIndex(e => e.order.orderNumber === data.order.orderNumber)

                    this.primengTableHelper.records[index].order = data.order;
                    this.primengTableHelper.records[index].orderStatusName = data.orderStatusName;
                    this.primengTableHelper.records[index].orderTypeName = data.orderTypeName;
                    this.primengTableHelper.records[index].areahName = data.areahName;
                    this.primengTableHelper.records[index].actionTime = data.actionTime;

                } catch {


                    this.primengTableHelper.records.forEach(element => {

                        if (element.order.orderNumber == data.order.orderNumber) {



                            element.order = data.order;
                            element.orderStatusName = data.orderStatusName;
                            element.orderTypeName = data.orderTypeName;
                            element.areahName = data.areahName;
                        }

                    });


                }

            }

        });
    };

    subscribeBotOrder = () => {

        this.botOrderSub = this.socketioService.BotOrder.subscribe((data: GetOrderForViewDto) => {
            if (this.appSession.tenantId === data.tenantId) {

                const index = this.primengTableHelper.records.findIndex(e => e.order.id === data.order.id)


                if (index == -1) {

                    var xx = JSON.stringify({ data })
                    if (data.tenantId == this.appSession.tenantId) {
                        this.primengTableHelper.records.unshift(data);
                        this.primengTableHelper.totalRecordsCount = this.primengTableHelper.totalRecordsCount + 1
                        //  this.getTime(this.primengTableHelper.records);
                        this.reloadPage();
                    }

                } else {


                    if (data.tenantId == this.appSession.tenantId) {
                        try {


                            this.primengTableHelper.records[index].order = data.order;
                            this.primengTableHelper.records[index].orderStatusName = data.orderStatusName;
                            this.primengTableHelper.records[index].orderTypeName = data.orderTypeName;
                            this.primengTableHelper.records[index].areahName = data.areahName;
                            this.primengTableHelper.records[index].actionTime = data.actionTime;

                        } catch {


                            this.primengTableHelper.records.forEach(element => {

                                if (element.order.orderNumber == data.order.orderNumber) {



                                    element.order = data.order;
                                    element.orderStatusName = data.orderStatusName;
                                    element.orderTypeName = data.orderTypeName;
                                    element.areahName = data.areahName;

                                    element.actionTime = data.actionTime;
                                }

                            });


                        }
                        this.reloadPage();
                    }




                }


            }
        });

    };

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());

    }


    lockOrder(order: OrderDto, stringTotla: string): void {

        this._ordersServiceProxy.lock(this.appSession.user.id, this.appSession.user.userName, stringTotla, order)
            .subscribe(() => {

                // this.reloadPage();
                this.notify.success(this.l('Successfully locked'));
            });
        // this.message.confirm(
        //     '',
        //     this.l('AreYouSure'),
        //     (isConfirmed) => {
        //         if (isConfirmed) {
        //             this._ordersServiceProxy.lock(this.appSession.user.id,this.appSession.user.userName,order)
        //                 .subscribe(() => {

        //                     // this.reloadPage();
        //                     this.notify.success(this.l('Successfully locked'));
        //                 });
        //         }
        //     }
        // );
    }
    UnlockOrderS(order: OrderDto, stringTotla: string): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._ordersServiceProxy.unLock(stringTotla, order)
                        .subscribe(() => {

                            // this.reloadPage();
                            this.notify.success(this.l('Successfully Unlocked'));
                        });
                }
            }
        );
    }


    exportToExcel(): void {
        ;
        this._ordersServiceProxy.getOrderToExcel(this.nameFilter, this.appSession.user.id, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, false,
            this.primengTableHelper.getSorting(this.dataTable),
            0,
            10000000)
            .subscribe(result => {
                ;
                this._fileDownloadService.downloadTempFile(result);
            });
    }



}


