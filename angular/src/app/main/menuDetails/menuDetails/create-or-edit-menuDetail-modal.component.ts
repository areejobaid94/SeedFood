import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { MenuDetailsServiceProxy, CreateOrEditMenuDetailDto ,MenuDetailItemLookupTableDto
					,MenuDetailMenuItemStatusLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { MenuDetailMenuLookupTableModalComponent } from './menuDetail-menu-lookup-table-modal.component';

@Component({
    selector: 'createOrEditMenuDetailModal',
    templateUrl: './create-or-edit-menuDetail-modal.component.html'
})
export class CreateOrEditMenuDetailModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('menuDetailMenuLookupTableModal', { static: true }) menuDetailMenuLookupTableModal: MenuDetailMenuLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    menuDetail: CreateOrEditMenuDetailDto = new CreateOrEditMenuDetailDto();

    itemItemName = '';
    menuMenuName = '';
    menuItemStatusName = '';

	allItems: MenuDetailItemLookupTableDto[];
						allMenuItemStatuss: MenuDetailMenuItemStatusLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _menuDetailsServiceProxy: MenuDetailsServiceProxy
    ) {
        super(injector);
    }
    
    show(menuDetailId?: number): void {
    

        if (!menuDetailId) {
            this.menuDetail = new CreateOrEditMenuDetailDto();
            this.menuDetail.id = menuDetailId;
            this.itemItemName = '';
            this.menuMenuName = '';
            this.menuItemStatusName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._menuDetailsServiceProxy.getMenuDetailForEdit(menuDetailId).subscribe(result => {
                this.menuDetail = result.menuDetail;

               // this.itemItemName = result.itemItemName;
                //this.menuMenuName = result.menuMenuName;
                this.menuItemStatusName = result.menuItemStatusName;

                this.active = true;
                this.modal.show();
            });
        }
        this._menuDetailsServiceProxy.getAllItemForTableDropdown().subscribe(result => {						
						this.allItems = result;
					});
					this._menuDetailsServiceProxy.getAllMenuItemStatusForTableDropdown().subscribe(result => {						
						this.allMenuItemStatuss = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
			
            this._menuDetailsServiceProxy.createOrEdit(this.menuDetail)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectMenuModal() {
        this.menuDetailMenuLookupTableModal.id = this.menuDetail.menuId;
        this.menuDetailMenuLookupTableModal.displayName = this.menuMenuName;
        this.menuDetailMenuLookupTableModal.show();
    }


    setMenuIdNull() {
        this.menuDetail.menuId = null;
        this.menuMenuName = '';
    }


    getNewMenuId() {
        this.menuDetail.menuId = this.menuDetailMenuLookupTableModal.id;
        this.menuMenuName = this.menuDetailMenuLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
