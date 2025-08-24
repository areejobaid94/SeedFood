import { Component, ViewChild, Injector, Output, EventEmitter, ViewEncapsulation} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {AdditionsCategorysListModel, GetItemAdditionsCategorysModel, MenusServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { LazyLoadEvent } from 'primeng/api';
@Component({
    selector: 'addOnsLookupTableModal',
    styleUrls: ['./addOns-lookup-table-modal.component.less'],
    encapsulation: ViewEncapsulation.None,
    templateUrl: './addOns-lookup-table-modal.component.html'
})
export class addOnsLookupTableModalComponent extends AppComponentBase {
    theme:string;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    filterText = '';
    AdditionsCategorysid: number;
    id: number;
    displayName: string;
    displayNameEnglish: string;
    
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    listaddons:AdditionsCategorysListModel[];

    isCloseClick:boolean;

    constructor(
        injector: Injector,
        private _menusServiceProxy: MenusServiceProxy,

    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme= ThemeHelper.getTheme();
    }

    show(list:AdditionsCategorysListModel[]): void {

        // this.listaddons=list;
                         
        // this.active = true;
        // this.paginator.rows = 5;
        // this.getAll();
        this.modal.show();
    }

    getAdditions(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();
        this._menusServiceProxy.getItemAdditionsCategories(
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(res => {
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event);
            this.primengTableHelper.totalRecordsCount = res.totalCount;
            this.primengTableHelper.records = res.lstItemAdditionsCategory;
            this.primengTableHelper.hideLoadingIndicator();
            this.active = true;
        });
    }
 

    // show(list:AdditionsCategorysListModel[]): void {

    //     this.listaddons=list;
    //                      
    //     this.active = true;
    //     this.paginator.rows = 5;
    //     this.getAll();
    //     this.modal.show();
    // }

    // getAll(event?: LazyLoadEvent) {
    //     if (!this.active) {
    //         return;
    //     }

    //     if (this.primengTableHelper.shouldResetPaging(event)) {
    //         this.paginator.changePage(0);
    //         return;
    //     }

    //     this.primengTableHelper.showLoadingIndicator();

    //     this._dealsServiceProxy.getAllDealStatusForLookupTable(
    //         this.filterText,
    //         this.primengTableHelper.getSorting(this.dataTable),
    //         this.primengTableHelper.getSkipCount(this.paginator, event),
    //         this.primengTableHelper.getMaxResultCount(this.paginator, event)
    //     ).subscribe(result => {
    //         this.primengTableHelper.totalRecordsCount = result.totalCount;
    //         this.primengTableHelper.records = result.items;
    //         this.primengTableHelper.hideLoadingIndicator();
    //     });
    // }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    setAndSave(addons: GetItemAdditionsCategorysModel) {
        this.AdditionsCategorysid=addons.categoryId;
        this.isCloseClick=false;
        this.id = addons.categoryId;
        this.displayName = addons.categoryName;
        this.displayNameEnglish = addons.categoryNameEnglish;
        this.active = false;
        this.modal.hide();
        this.modalSave.emit(null);
    }

    close(): void {
        this.isCloseClick=true;
        this.active = false;
        this.modal.hide();
        this.modalSave.emit(null);
    }
}
