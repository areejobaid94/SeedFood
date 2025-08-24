import { Component, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { ReceiptDetailsServiceProxy, ReceiptDetailDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { MasterDetailChild_Receipt_CreateOrEditReceiptDetailModalComponent } from './masterDetailChild_Receipt_create-or-edit-receiptDetail-modal.component';

import { MasterDetailChild_Receipt_ViewReceiptDetailModalComponent } from './masterDetailChild_Receipt_view-receiptDetail-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';



@Component({
    templateUrl: './masterDetailChild_Receipt_receiptDetails.component.html',
    selector: "masterDetailChild_Receipt_receiptDetails-component",
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class MasterDetailChild_Receipt_ReceiptDetailsComponent extends AppComponentBase {
    @Input("receiptId") receiptId: any;
    
    @ViewChild('createOrEditReceiptDetailModal', { static: true }) createOrEditReceiptDetailModal: MasterDetailChild_Receipt_CreateOrEditReceiptDetailModalComponent;
    @ViewChild('viewReceiptDetailModalComponent', { static: true }) viewReceiptDetailModal: MasterDetailChild_Receipt_ViewReceiptDetailModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    billingNumberFilter = '';
    maxBillDateFromFilter : moment.Moment;
		minBillDateFromFilter : moment.Moment;
    maxBillDateToFilter : moment.Moment;
		minBillDateToFilter : moment.Moment;
    serviceNameFilter = '';
    maxBillAmountFilter : number;
		maxBillAmountFilterEmpty : number;
		minBillAmountFilter : number;
		minBillAmountFilterEmpty : number;
    maxOpenAmountFilter : number;
		maxOpenAmountFilterEmpty : number;
		minOpenAmountFilter : number;
		minOpenAmountFilterEmpty : number;
    currencyNameFilter = '';
        accountBillingBillIDFilter = '';




    constructor(
        injector: Injector,
        private _receiptDetailsServiceProxy: ReceiptDetailsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getReceiptDetails(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._receiptDetailsServiceProxy.getAll(
            this.filterText,
            this.billingNumberFilter,
            this.maxBillDateFromFilter,
            this.minBillDateFromFilter,
            this.maxBillDateToFilter,
            this.minBillDateToFilter,
            this.serviceNameFilter,
            this.maxBillAmountFilter == null ? this.maxBillAmountFilterEmpty: this.maxBillAmountFilter,
            this.minBillAmountFilter == null ? this.minBillAmountFilterEmpty: this.minBillAmountFilter,
            this.maxOpenAmountFilter == null ? this.maxOpenAmountFilterEmpty: this.maxOpenAmountFilter,
            this.minOpenAmountFilter == null ? this.minOpenAmountFilterEmpty: this.minOpenAmountFilter,
            this.currencyNameFilter,
            null,
            this.accountBillingBillIDFilter,
            this.receiptId,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createReceiptDetail(): void {
        this.createOrEditReceiptDetailModal.show(this.receiptId);        
    }


    deleteReceiptDetail(receiptDetail: ReceiptDetailDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._receiptDetailsServiceProxy.delete(receiptDetail.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._receiptDetailsServiceProxy.getReceiptDetailsToExcel(
        this.filterText,
            this.billingNumberFilter,
            this.maxBillDateFromFilter,
            this.minBillDateFromFilter,
            this.maxBillDateToFilter,
            this.minBillDateToFilter,
            this.serviceNameFilter,
            this.maxBillAmountFilter == null ? this.maxBillAmountFilterEmpty: this.maxBillAmountFilter,
            this.minBillAmountFilter == null ? this.minBillAmountFilterEmpty: this.minBillAmountFilter,
            this.maxOpenAmountFilter == null ? this.maxOpenAmountFilterEmpty: this.maxOpenAmountFilter,
            this.minOpenAmountFilter == null ? this.minOpenAmountFilterEmpty: this.minOpenAmountFilter,
            this.currencyNameFilter,
            "",
            this.accountBillingBillIDFilter
            ,this.receiptId,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
