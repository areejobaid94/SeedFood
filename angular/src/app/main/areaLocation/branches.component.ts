import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { BranchesServiceProxy, BranchDto, AreasServiceProxy  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import { ViewBranchModalComponent } from './view-branch-modal.component';
import { CreateOrEditBranchtModalComponent } from './create-or-edit-branch-modal.component';


@Component({
    templateUrl: './branches.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class BranchesComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditBranchModal', { static: true }) createOrEditBranchModal: CreateOrEditBranchtModalComponent;
    @ViewChild('viewBranchModal', { static: true }) viewBranchModal: ViewBranchModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterName = '';
    restaurantsFilter = '';

    constructor(
        injector: Injector,
        private _areasServiceProxy: AreasServiceProxy,
        private _branchesServiceProxy: BranchesServiceProxy,

    ) {
        super(injector);
    }

    getBranches(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();
        
        this._areasServiceProxy.getAreas(
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.lstAreas;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createBranch(): void {
        this.createOrEditBranchModal.show();        
    }


    deleteBranches(branch: BranchDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._branchesServiceProxy.delete(branch.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }


}
