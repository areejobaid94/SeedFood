import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';

import { OrdersServiceProxy, OrderDto, GetOrderForViewDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditDeliveryOrderModalComponent } from './create-or-edit-order-modal.component';

import { ViewDeliveryOrderModalComponent } from './view-order-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { Subscription } from 'rxjs';
import { SocketioService } from '@app/shared/socketio/socketioservice';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';


@Component({
    templateUrl: './order.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})

export class DeliveryOrdersComponent extends AppComponentBase {
    theme: string;


    @ViewChild('createOrEditDeliveryOrderModal', { static: true }) createOrEditDeliveryOrderModal: CreateOrEditDeliveryOrderModalComponent;
    @ViewChild('viewDeliveryOrderModal', { static: true }) viewDeliveryOrderModal: ViewDeliveryOrderModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '-1';
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
    fromGoogleURL: string;
    //number:any;
    appSession: AppSessionService;
    items: any;
    agentOrderSub: Subscription;
    botOrderSub: Subscription;
    change: any;
    differ: any[];

    // order:OrderDto;
    constructor(
        // differs: IterableDiffers,
        injector: Injector,
        private socketioService: SocketioService,

        private _ordersServiceProxy: OrdersServiceProxy,
        private _fileDownloadService: FileDownloadService,
        public darkModeService: DarkModeService,

    ) {
        super(injector);
    }



    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.fromGoogleURL = "";
        this.subscribeAgentOrder();
        this.subscribeBotOrder();
        await this.getIsAdmin();

    }

    getOrders(event?: LazyLoadEvent) {

        this.fromGoogleURL = "";
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
    
        this._ordersServiceProxy.getAll(this.nameFilter, this.appSession.user.id, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, false,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)).subscribe(result => {


                result.items.forEach(element => {


                    var x = (Math.round(element.order.total * 100) / 100).toFixed(2);

                    element.order.stringTotal = x;

                });

                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;



            });
    }

    subscribeAgentOrder = () => {

        this.agentOrderSub = this.socket.agentOrder.subscribe((data: GetOrderForViewDto) => {


            this.primengTableHelper.records.forEach(element => {

                if (element.order.orderNumber == data.order.orderNumber) {



                    element.order = data.order;
                    element.orderStatusName = data.orderStatusName;
                    element.orderTypeName = data.orderTypeName;
                    element.areahName = data.areahName;
                }

            });
            //    this.getTime(this.primengTableHelper.records);

        });
    };

    subscribeBotOrder = () => {

        this.botOrderSub = this.socketioService.BotOrder.subscribe((data: GetOrderForViewDto) => {
            if(this.appSession.tenantId === data.tenantId){
                this.primengTableHelper.records.push(data);
            this.primengTableHelper.totalRecordsCount = this.primengTableHelper.totalRecordsCount + 1
            //  this.getTime(this.primengTableHelper.records);
            this.reloadPage();
            }
          
          
        });

    };

    reloadPage(): void {


        this.paginator.changePage(this.paginator.getPage());
    }

    // createOrder(): void {
    //     this.createOrEditOrderModal.show();        
    // }


    
    deleteForEver(order: OrderDto): void {

        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {

                    this._ordersServiceProxy.deleteForEver(order.id)
                        .subscribe(() => {

                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }

    deleteAllForEver(): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._ordersServiceProxy.deleteAllForEver()
                        .subscribe(() => {

                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }

    lockOrder(order: OrderDto, stringTotla: string): void {

        this._ordersServiceProxy.lock(this.appSession.user.id, this.appSession.user.userName, stringTotla, order)
            .subscribe(() => {

                // this.reloadPage();
                this.notify.success(this.l('successfullyLocked'));
            });
 
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
                            this.notify.success(this.l('succcessfullyUnlocked'));
                        });
                }
            }
        );
    }

    getTime(Orders: GetOrderForViewDto[]) {

        for (let user of Orders) {


            let time = user.order.creationTime.toString();
            user.creatDate == moment.utc(time).format('hh:mm a');
            user.deliveryChangeDeliveryServiceProvider = moment.utc(time).format('MM/DD/YY'); //    moment.utc(time).format('MM/DD hh:mm a');      
        }
    }

    exportToExcel(): void {
        this._ordersServiceProxy.getOrderToExcel(this.nameFilter, this.appSession.user.id, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            this.primengTableHelper.getSorting(this.dataTable),
            0,
            10)
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }



}



