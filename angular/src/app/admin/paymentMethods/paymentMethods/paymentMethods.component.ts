import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { PaymentMethodsServiceProxy, PaymentMethodDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditPaymentMethodModalComponent } from './create-or-edit-paymentMethod-modal.component';

import { ViewPaymentMethodModalComponent } from './view-paymentMethod-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './paymentMethods.component.html',
    encapsulation: ViewEncapsulation.None,
    selector: 'paymentMethods-component',
    animations: [appModuleAnimation()]
})
export class PaymentMethodsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditPaymentMethodModal', { static: true }) createOrEditPaymentMethodModal: CreateOrEditPaymentMethodModalComponent;
    @ViewChild('viewPaymentMethodModalComponent', { static: true }) viewPaymentMethodModal: ViewPaymentMethodModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';






    constructor(
        injector: Injector,
        private _paymentMethodsServiceProxy: PaymentMethodsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getPaymentMethods(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._paymentMethodsServiceProxy.getAll(
            this.filterText,
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

    createPaymentMethod(): void {
        this.createOrEditPaymentMethodModal.show();        
    }


    deletePaymentMethod(paymentMethod: PaymentMethodDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._paymentMethodsServiceProxy.delete(paymentMethod.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
    
    
    
    
}
