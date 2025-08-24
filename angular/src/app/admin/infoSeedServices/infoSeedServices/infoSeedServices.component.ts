import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { InfoSeedServicesServiceProxy, InfoSeedServiceDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditInfoSeedServiceModalComponent } from './create-or-edit-infoSeedService-modal.component';

import { ViewInfoSeedServiceModalComponent } from './view-infoSeedService-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './infoSeedServices.component.html',
    selector: "infoSeedServices-component", 
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class InfoSeedServicesComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditInfoSeedServiceModal', { static: true }) createOrEditInfoSeedServiceModal: CreateOrEditInfoSeedServiceModalComponent;
    @ViewChild('viewInfoSeedServiceModalComponent', { static: true }) viewInfoSeedServiceModal: ViewInfoSeedServiceModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    serviceIDFilter = '';
    maxServiceFeesFilter : number;
		maxServiceFeesFilterEmpty : number;
		minServiceFeesFilter : number;
		minServiceFeesFilterEmpty : number;
    serviceNameFilter = '';
    maxServiceCreationDateFilter : moment.Moment;
		minServiceCreationDateFilter : moment.Moment;
    maxServiceStoppingDateFilter : moment.Moment;
		minServiceStoppingDateFilter : moment.Moment;
        serviceTypeServicetypeNameFilter = '';
        serviceStatusServiceStatusNameFilter = '';
        serviceFrquencyServiceFrequencyNameFilter = '';






    constructor(
        injector: Injector,
        private _infoSeedServicesServiceProxy: InfoSeedServicesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getInfoSeedServices(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._infoSeedServicesServiceProxy.getAll(
            this.filterText,
            this.serviceIDFilter,
            this.maxServiceFeesFilter == null ? this.maxServiceFeesFilterEmpty: this.maxServiceFeesFilter,
            this.minServiceFeesFilter == null ? this.minServiceFeesFilterEmpty: this.minServiceFeesFilter,
            this.serviceNameFilter,
            this.maxServiceCreationDateFilter === undefined ? this.maxServiceCreationDateFilter : moment(this.maxServiceCreationDateFilter).endOf('day'),
            this.minServiceCreationDateFilter === undefined ? this.minServiceCreationDateFilter : moment(this.minServiceCreationDateFilter).startOf('day'),
            this.maxServiceStoppingDateFilter === undefined ? this.maxServiceStoppingDateFilter : moment(this.maxServiceStoppingDateFilter).endOf('day'),
            this.minServiceStoppingDateFilter === undefined ? this.minServiceStoppingDateFilter : moment(this.minServiceStoppingDateFilter).startOf('day'),
            this.serviceTypeServicetypeNameFilter,
            this.serviceStatusServiceStatusNameFilter,
            this.serviceFrquencyServiceFrequencyNameFilter,
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

    createInfoSeedService(): void {
        this.createOrEditInfoSeedServiceModal.show();        
    }


    deleteInfoSeedService(infoSeedService: InfoSeedServiceDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._infoSeedServicesServiceProxy.delete(infoSeedService.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._infoSeedServicesServiceProxy.getInfoSeedServicesToExcel(
        this.filterText,
            this.serviceIDFilter,
            this.maxServiceFeesFilter == null ? this.maxServiceFeesFilterEmpty: this.maxServiceFeesFilter,
            this.minServiceFeesFilter == null ? this.minServiceFeesFilterEmpty: this.minServiceFeesFilter,
            this.serviceNameFilter,
            this.maxServiceCreationDateFilter === undefined ? this.maxServiceCreationDateFilter : moment(this.maxServiceCreationDateFilter).endOf('day'),
            this.minServiceCreationDateFilter === undefined ? this.minServiceCreationDateFilter : moment(this.minServiceCreationDateFilter).startOf('day'),
            this.maxServiceStoppingDateFilter === undefined ? this.maxServiceStoppingDateFilter : moment(this.maxServiceStoppingDateFilter).endOf('day'),
            this.minServiceStoppingDateFilter === undefined ? this.minServiceStoppingDateFilter : moment(this.minServiceStoppingDateFilter).startOf('day'),
            this.serviceTypeServicetypeNameFilter,
            this.serviceStatusServiceStatusNameFilter,
            this.serviceFrquencyServiceFrequencyNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
    
}
