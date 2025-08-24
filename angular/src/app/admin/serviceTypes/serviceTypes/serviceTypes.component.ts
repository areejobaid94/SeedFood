import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { ServiceTypesServiceProxy, ServiceTypeDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditServiceTypeModalComponent } from './create-or-edit-serviceType-modal.component';

import { ViewServiceTypeModalComponent } from './view-serviceType-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './serviceTypes.component.html',
    selector: 'serviceTypes-component',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ServiceTypesComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditServiceTypeModal', { static: true }) createOrEditServiceTypeModal: CreateOrEditServiceTypeModalComponent;
    @ViewChild('viewServiceTypeModalComponent', { static: true }) viewServiceTypeModal: ViewServiceTypeModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    servicetypeNameFilter = '';
    isEnabledFilter = -1;
    maxCreationDateFilter : moment.Moment;
		minCreationDateFilter : moment.Moment;






    constructor(
        injector: Injector,
        private _serviceTypesServiceProxy: ServiceTypesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getServiceTypes(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._serviceTypesServiceProxy.getAll(
            this.filterText,
            this.servicetypeNameFilter,
            this.isEnabledFilter,
            this.maxCreationDateFilter === undefined ? this.maxCreationDateFilter : moment(this.maxCreationDateFilter).endOf('day'),
            this.minCreationDateFilter === undefined ? this.minCreationDateFilter : moment(this.minCreationDateFilter).startOf('day'),
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

    createServiceType(): void {
        this.createOrEditServiceTypeModal.show();        
    }


    deleteServiceType(serviceType: ServiceTypeDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._serviceTypesServiceProxy.delete(serviceType.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
    
    
    
}
