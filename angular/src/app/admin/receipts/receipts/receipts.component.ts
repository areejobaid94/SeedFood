import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { ReceiptsServiceProxy, ReceiptDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditReceiptModalComponent } from './create-or-edit-receipt-modal.component';

import { ViewReceiptModalComponent } from './view-receipt-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './receipts.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ReceiptsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditReceiptModal', { static: true }) createOrEditReceiptModal: CreateOrEditReceiptModalComponent;
    @ViewChild('viewReceiptModalComponent', { static: true }) viewReceiptModal: ViewReceiptModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    receiptNumberFilter = '';
    maxReceiptDateFilter : moment.Moment;
		minReceiptDateFilter : moment.Moment;
    paymentReferenceNumberFilter = '';
        bankBankNameFilter = '';
        paymentMethodPaymnetMethodFilter = '';




            receiptDetailRowSelection: boolean[] = [];
            

                   childEntitySelection: {} = {};
            

    constructor(
        injector: Injector,
        private _receiptsServiceProxy: ReceiptsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getReceipts(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._receiptsServiceProxy.getAll(
            this.filterText,
            this.receiptNumberFilter,
            this.maxReceiptDateFilter === undefined ? this.maxReceiptDateFilter : moment(this.maxReceiptDateFilter).endOf('day'),
            this.minReceiptDateFilter === undefined ? this.minReceiptDateFilter : moment(this.minReceiptDateFilter).startOf('day'),
            this.paymentReferenceNumberFilter,
            this.bankBankNameFilter,
            this.paymentMethodPaymnetMethodFilter,
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

    createReceipt(): void {
        this.createOrEditReceiptModal.show();        
    }


    deleteReceipt(receipt: ReceiptDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._receiptsServiceProxy.delete(receipt.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._receiptsServiceProxy.getReceiptsToExcel(
        this.filterText,
            this.receiptNumberFilter,
            this.maxReceiptDateFilter === undefined ? this.maxReceiptDateFilter : moment(this.maxReceiptDateFilter).endOf('day'),
            this.minReceiptDateFilter === undefined ? this.minReceiptDateFilter : moment(this.minReceiptDateFilter).startOf('day'),
            this.paymentReferenceNumberFilter,
            this.bankBankNameFilter,
            this.paymentMethodPaymnetMethodFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
                  selectEditTable(table){
                      this.childEntitySelection = {};
                      this.childEntitySelection[table] = true;
                  }
            
    
               openChildRowForReceiptDetail(index, table) {
                   let isAlreadyOpened = this.receiptDetailRowSelection[index];                   
                   this.closeAllChildRows();                   
                   this.receiptDetailRowSelection = [];
                   if (!isAlreadyOpened) {
                       this.receiptDetailRowSelection[index] = true;
                   }
                   this.selectEditTable(table);
               }
            
    
                  closeAllChildRows() : void{
                     
                this.receiptDetailRowSelection = [];
            
                  }
}
