import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import {  ForcatsesServiceProxy, ForcatsDto  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import {   CreateOrEditForcatsModalComponent } from './create-or-edit-forcats-modal.component';

import { ViewForcatsModalComponent } from './view-forcats-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';

@Component({
    templateUrl: './forcats.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ForcatsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditForcatsModal', { static: true }) createOrEditForcatsModal: CreateOrEditForcatsModalComponent;
    @ViewChild('viewForcatsModalComponent', { static: true }) viewForcatsModal: ViewForcatsModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '';






    constructor(
        injector: Injector,
        private _forcatsServiceProxy: ForcatsesServiceProxy,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getForcats(event?: LazyLoadEvent) {
                         
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._forcatsServiceProxy.getAll(
            this.filterText,
            this.appSession.user.userName,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
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

    createForcats(): void {
        this.createOrEditForcatsModal.show();        
    }


    deleteForcats(forcats: ForcatsDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._forcatsServiceProxy.delete(forcats.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._forcatsServiceProxy.getForcatsesToExcel(
        this.filterText,
            this.nameFilter,
            null,
            null,
            null,
            null
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
}
