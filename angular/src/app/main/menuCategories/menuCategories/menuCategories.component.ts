import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ItemCategoryServiceProxy, MenuCategoryDto  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditMenuCategoryModalComponent } from './create-or-edit-menuCategory-modal.component';
import { ViewMenuCategoryModalComponent } from './view-menuCategory-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';

@Component({
    templateUrl: './menuCategories.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class MenuCategoriesComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditMenuCategoryModal', { static: true }) createOrEditMenuCategoryModal: CreateOrEditMenuCategoryModalComponent;
    @ViewChild('viewMenuCategoryModalComponent', { static: true }) viewMenuCategoryModal: ViewMenuCategoryModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '';






    constructor(
        injector: Injector,
        private _menuCategoriesServiceProxy: ItemCategoryServiceProxy,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getMenuCategories(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._menuCategoriesServiceProxy.getAll(
            this.filterText,
            this.nameFilter,
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

    createMenuCategory(): void {
        this.createOrEditMenuCategoryModal.show();        
    }


    deleteMenuCategory(menuCategory: MenuCategoryDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._menuCategoriesServiceProxy.delete(menuCategory.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._menuCategoriesServiceProxy.getItemCategoryToExcel(
        this.filterText,
            this.nameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
}
