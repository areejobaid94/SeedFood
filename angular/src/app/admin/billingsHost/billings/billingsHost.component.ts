import { Component, Injector, ViewEncapsulation, ViewChild, ChangeDetectorRef } from '@angular/core';
// import { ActivatedRoute , Router} from '@angular/router';
import { BillingsServiceProxy, BillingDto  } from '@shared/service-proxies/service-proxies';
// import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
// import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
// import { CreateOrEditBillingModalComponent } from './create-or-edit-billing-modal.component';

import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { PaymentTestComponent } from '@app/main/payment-test/payment-test.component';
import { ViewBillingsHostModalComponent } from './view-billingsHost-modal.component';
import { ActivatedRoute } from '@angular/router';
import { CreateOrEditBillingHostModalComponent } from './create-or-edit-billingsHost-modal.component';

declare function removescript() :any;
@Component({
    templateUrl: './billingsHost.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class BillingHostComponent extends AppComponentBase {
    loadScript() {
        let node = document.createElement('script'); // creates the script tag
        node.src = 'https://mepspay.gateway.mastercard.com/checkout/version/45/checkout.js'; // sets the source (insert url in between quotes)
        node.id = 'ScriptMastercard'; // set the script type
        node.dataset.error = 'errorCallback'; // set the script type
        node.dataset.cancel = 'cancelCallback'; // set the script type
        node.dataset.beforeredirect = 'beforeRedirect'; // set the script type
        node.dataset.afterredirect = 'afterRedirect'; // set the script type
        node.dataset.complete = 'completeCallback'; // set the script type
       // node.async = true; // makes script run asynchronously
        //node.charset = 'utf-8';
        // append to head of document
        document.getElementsByTagName('head')[0].appendChild(node);
     }

    appBaseUrl = '';
    @ViewChild('paymentTestComponent', { static: true }) paymentTestComponent: PaymentTestComponent;
     @ViewChild('createOrEditBillingHostModal', { static: true }) createOrEditBillingHostModal: CreateOrEditBillingHostModalComponent;
    @ViewChild('viewBillingHostModalComponent', { static: true }) viewBillingHostModal: ViewBillingsHostModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    billingIDFilter = '';
    maxBillingDateFilter : moment.Moment;
		minBillingDateFilter : moment.Moment;
    maxTotalAmountFilter : number;
		maxTotalAmountFilterEmpty : number;
		minTotalAmountFilter : number;
		minTotalAmountFilterEmpty : number;
    maxBillPeriodToFilter : moment.Moment;
		minBillPeriodToFilter : moment.Moment;
    maxBillPeriodFromFilter : moment.Moment;
		minBillPeriodFromFilter : moment.Moment;
    maxDueDateFilter : moment.Moment;
		minDueDateFilter : moment.Moment;
        currencyCurrencyNameFilter = '';



    tenentID:number;


    filters: {
        selectedCustomerID: number;
    } = <any>{};

    constructor(
        injector: Injector,
    
        private _billingsServiceProxy: BillingsServiceProxy,
        // private _notifyService: NotifyService,
        // private _tokenAuth: TokenAuthServiceProxy,
        // private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService,
        
            private _activatedRoute: ActivatedRoute,
          private cd: ChangeDetectorRef
    ) {
        super(injector);
       // this.ngOnInit();
    }
    setFiltersFromRoute(): void {
        if (this._activatedRoute.snapshot.queryParams['tenentID'] != null) {
            this.filters.selectedCustomerID = this._activatedRoute.snapshot.queryParams['tenentID'];
        }
    }

    ngOnInit() {

        this.setFiltersFromRoute();
        // try{
        //     removescript();
        // }catch{


        // }

        var isFoundCard=document.getElementById("ScriptMastercard")

       if(isFoundCard==null){

           this.loadScript();
       }
     }
    getBillings(event?: LazyLoadEvent) {

        // this.appBaseUrl = AppConsts.appBaseUrl;

        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._billingsServiceProxy.hostGetAll(this.filters.selectedCustomerID).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    // createBilling(): void {
    //     this.createOrEditBillingModal.show();        
    // }


    deleteBilling(billing: BillingDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._billingsServiceProxy.delete(billing.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._billingsServiceProxy.getBillingsToExcel(
        this.filterText,
            this.billingIDFilter,
            this.maxBillingDateFilter === undefined ? this.maxBillingDateFilter : moment(this.maxBillingDateFilter).endOf('day'),
            this.minBillingDateFilter === undefined ? this.minBillingDateFilter : moment(this.minBillingDateFilter).startOf('day'),
            this.maxTotalAmountFilter == null ? this.maxTotalAmountFilterEmpty: this.maxTotalAmountFilter,
            this.minTotalAmountFilter == null ? this.minTotalAmountFilterEmpty: this.minTotalAmountFilter,
            this.maxBillPeriodToFilter === undefined ? this.maxBillPeriodToFilter : moment(this.maxBillPeriodToFilter).endOf('day'),
            this.minBillPeriodToFilter === undefined ? this.minBillPeriodToFilter : moment(this.minBillPeriodToFilter).startOf('day'),
            this.maxBillPeriodFromFilter === undefined ? this.maxBillPeriodFromFilter : moment(this.maxBillPeriodFromFilter).endOf('day'),
            this.minBillPeriodFromFilter === undefined ? this.minBillPeriodFromFilter : moment(this.minBillPeriodFromFilter).startOf('day'),
            this.maxDueDateFilter === undefined ? this.maxDueDateFilter : moment(this.maxDueDateFilter).endOf('day'),
            this.minDueDateFilter === undefined ? this.minDueDateFilter : moment(this.minDueDateFilter).startOf('day'),
            this.currencyCurrencyNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    createBilling(): void {
        this.createOrEditBillingHostModal.show(null,this.filters.selectedCustomerID);
    }
    
    
}
