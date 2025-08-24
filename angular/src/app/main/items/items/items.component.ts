import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { ItemsServiceProxy, ItemDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditItemModalComponent } from './create-or-edit-item-modal.component';

import { ViewItemModalComponent } from './view-item-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
// import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/api';

@Component({
    templateUrl: './items.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ItemsComponent extends AppComponentBase {


    @ViewChild('createOrEditItemModal', { static: true }) createOrEditItemModal: CreateOrEditItemModalComponent;
    @ViewChild('viewItemModalComponent', { static: true }) viewItemModal: ViewItemModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    itemDescriptionFilter = '';
    ingredientsFilter = '';
    itemNameFilter = '';
    isInServiceFilter = -1;
    categoryNamesFilter = '';
    maxCreationTimeFilter : moment.Moment;
		minCreationTimeFilter : moment.Moment;
    maxDeletionTimeFilter : moment.Moment;
		minDeletionTimeFilter : moment.Moment;
    maxLastModificationTimeFilter : moment.Moment;
		minLastModificationTimeFilter : moment.Moment;


        minPriceFilter = 0;
        imageUriFilter = '';
        priorityFilter = 0;
        menuNameFilter = '';
        categoryNameFilter = '';



    constructor(
        injector: Injector,
        private _itemsServiceProxy: ItemsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getItems(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._itemsServiceProxy.getAll(
            this.filterText,
            this.itemDescriptionFilter,
            this.ingredientsFilter,
            this.itemNameFilter,
            this.isInServiceFilter,
            this.categoryNamesFilter,
            this.maxCreationTimeFilter === undefined ? this.maxCreationTimeFilter : moment(this.maxCreationTimeFilter).endOf('day'),
            this.minCreationTimeFilter === undefined ? this.minCreationTimeFilter : moment(this.minCreationTimeFilter).startOf('day'),
            this.maxDeletionTimeFilter === undefined ? this.maxDeletionTimeFilter : moment(this.maxDeletionTimeFilter).endOf('day'),
            this.minDeletionTimeFilter === undefined ? this.minDeletionTimeFilter : moment(this.minDeletionTimeFilter).startOf('day'),
            this.maxLastModificationTimeFilter === undefined ? this.maxLastModificationTimeFilter : moment(this.maxLastModificationTimeFilter).endOf('day'),
            this.minLastModificationTimeFilter === undefined ? this.minLastModificationTimeFilter : moment(this.minLastModificationTimeFilter).startOf('day'),
            this.minPriceFilter,
            this.imageUriFilter,
            this.priorityFilter,
            this.menuNameFilter,
            this.categoryNameFilter,
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

    createItem(): void {
        this.createOrEditItemModal.show();
    }


    deleteItem(item: ItemDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._itemsServiceProxy.deleteItem(item.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._itemsServiceProxy.getItemsToExcel(
        this.filterText,
            this.itemDescriptionFilter,
            this.ingredientsFilter,
            this.itemNameFilter,
            this.isInServiceFilter,
            this.categoryNamesFilter,
            this.maxCreationTimeFilter === undefined ? this.maxCreationTimeFilter : moment(this.maxCreationTimeFilter).endOf('day'),
            this.minCreationTimeFilter === undefined ? this.minCreationTimeFilter : moment(this.minCreationTimeFilter).startOf('day'),
            this.maxDeletionTimeFilter === undefined ? this.maxDeletionTimeFilter : moment(this.maxDeletionTimeFilter).endOf('day'),
            this.minDeletionTimeFilter === undefined ? this.minDeletionTimeFilter : moment(this.minDeletionTimeFilter).startOf('day'),
            this.maxLastModificationTimeFilter === undefined ? this.maxLastModificationTimeFilter : moment(this.maxLastModificationTimeFilter).endOf('day'),
            this.minLastModificationTimeFilter === undefined ? this.minLastModificationTimeFilter : moment(this.minLastModificationTimeFilter).startOf('day'),
            this.minPriceFilter,
            this.imageUriFilter,
            this.priorityFilter,
            this.menuNameFilter,
            this.categoryNameFilter
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }



}
