import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { AccountBillingsServiceProxy, AccountBillingDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAccountBillingModalComponent } from './create-or-edit-accountBilling-modal.component';

import { ViewAccountBillingModalComponent } from './view-accountBilling-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './accountBillings.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class AccountBillingsComponent extends AppComponentBase {
    
    z
    @ViewChild('createOrEditAccountBillingModal', { static: true }) createOrEditAccountBillingModal: CreateOrEditAccountBillingModalComponent;
    @ViewChild('viewAccountBillingModalComponent', { static: true }) viewAccountBillingModal: ViewAccountBillingModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    billIDFilter = '';
    maxBillDateFromFilter : moment.Moment;
		minBillDateFromFilter : moment.Moment;
    maxBillDateToFilter : moment.Moment;
		minBillDateToFilter : moment.Moment;
    maxOpenAmountFilter : number;
		maxOpenAmountFilterEmpty : number;
		minOpenAmountFilter : number;
		minOpenAmountFilterEmpty : number;
    maxBillAmountFilter : number;
		maxBillAmountFilterEmpty : number;
		minBillAmountFilter : number;
		minBillAmountFilterEmpty : number;
        infoSeedServiceServiceNameFilter = '';
        serviceTypeServicetypeNameFilter = '';
        currencyCurrencyNameFilter = '';
        billingBillingIDFilter = '';

        tenetIDFilterEmpty : number;






    constructor(
        injector: Injector,
        private _accountBillingsServiceProxy: AccountBillingsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getAccountBillings(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._accountBillingsServiceProxy.getAll(
            this.filterText,
            this.billIDFilter,
            this.maxBillDateFromFilter === undefined ? this.maxBillDateFromFilter : moment(this.maxBillDateFromFilter).endOf('day'),
            this.minBillDateFromFilter === undefined ? this.minBillDateFromFilter : moment(this.minBillDateFromFilter).startOf('day'),
            this.maxBillDateToFilter === undefined ? this.maxBillDateToFilter : moment(this.maxBillDateToFilter).endOf('day'),
            this.minBillDateToFilter === undefined ? this.minBillDateToFilter : moment(this.minBillDateToFilter).startOf('day'),
            this.maxOpenAmountFilter == null ? this.maxOpenAmountFilterEmpty: this.maxOpenAmountFilter,
            this.minOpenAmountFilter == null ? this.minOpenAmountFilterEmpty: this.minOpenAmountFilter,
            this.maxBillAmountFilter == null ? this.maxBillAmountFilterEmpty: this.maxBillAmountFilter,
            this.minBillAmountFilter == null ? this.minBillAmountFilterEmpty: this.minBillAmountFilter,
            this.infoSeedServiceServiceNameFilter,
            this.serviceTypeServicetypeNameFilter,
            this.currencyCurrencyNameFilter,
            this.billingBillingIDFilter,
            this.tenetIDFilterEmpty,
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

    createAccountBilling(): void {
        this.createOrEditAccountBillingModal.show();        
    }


    deleteAccountBilling(accountBilling: AccountBillingDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._accountBillingsServiceProxy.delete(accountBilling.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._accountBillingsServiceProxy.getAccountBillingsToExcel(
        this.filterText,
            this.billIDFilter,
            this.maxBillDateFromFilter === undefined ? this.maxBillDateFromFilter : moment(this.maxBillDateFromFilter).endOf('day'),
            this.minBillDateFromFilter === undefined ? this.minBillDateFromFilter : moment(this.minBillDateFromFilter).startOf('day'),
            this.maxBillDateToFilter === undefined ? this.maxBillDateToFilter : moment(this.maxBillDateToFilter).endOf('day'),
            this.minBillDateToFilter === undefined ? this.minBillDateToFilter : moment(this.minBillDateToFilter).startOf('day'),
            this.maxOpenAmountFilter == null ? this.maxOpenAmountFilterEmpty: this.maxOpenAmountFilter,
            this.minOpenAmountFilter == null ? this.minOpenAmountFilterEmpty: this.minOpenAmountFilter,
            this.maxBillAmountFilter == null ? this.maxBillAmountFilterEmpty: this.maxBillAmountFilter,
            this.minBillAmountFilter == null ? this.minBillAmountFilterEmpty: this.minBillAmountFilter,
            this.infoSeedServiceServiceNameFilter,
            this.serviceTypeServicetypeNameFilter,
            this.currencyCurrencyNameFilter,
            this.billingBillingIDFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
    
}
