import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { MenuDetailsServiceProxy, MenuDetailDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditMenuDetailModalComponent } from './create-or-edit-menuDetail-modal.component';

import { ViewMenuDetailModalComponent } from './view-menuDetail-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './menuDetails.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class MenuDetailsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditMenuDetailModal', { static: true }) createOrEditMenuDetailModal: CreateOrEditMenuDetailModalComponent;
    @ViewChild('viewMenuDetailModalComponent', { static: true }) viewMenuDetailModal: ViewMenuDetailModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    descriptionFilter = '';
    isStandAloneFilter = -1;
    maxPriceFilter : number;
		maxPriceFilterEmpty : number;
		minPriceFilter : number;
		minPriceFilterEmpty : number;
        itemItemNameFilter = '';
        menuMenuNameFilter = '';
        menuItemStatusNameFilter = '';






    constructor(
        injector: Injector,
        private _menuDetailsServiceProxy: MenuDetailsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getMenuDetails(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._menuDetailsServiceProxy.getAll(
            this.filterText,
            this.descriptionFilter,
            this.isStandAloneFilter,
            this.maxPriceFilter == null ? this.maxPriceFilterEmpty: this.maxPriceFilter,
            this.minPriceFilter == null ? this.minPriceFilterEmpty: this.minPriceFilter,
            this.itemItemNameFilter,
            this.menuMenuNameFilter,
            this.menuItemStatusNameFilter,
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

    createMenuDetail(): void {
        this.createOrEditMenuDetailModal.show();        
    }


    deleteMenuDetail(menuDetail: MenuDetailDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._menuDetailsServiceProxy.delete(menuDetail.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._menuDetailsServiceProxy.getMenuDetailsToExcel(
        this.filterText,
            this.descriptionFilter,
            this.isStandAloneFilter,
            this.maxPriceFilter == null ? this.maxPriceFilterEmpty: this.maxPriceFilter,
            this.minPriceFilter == null ? this.minPriceFilterEmpty: this.minPriceFilter,
            this.itemItemNameFilter,
            this.menuMenuNameFilter,
            this.menuItemStatusNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
}
