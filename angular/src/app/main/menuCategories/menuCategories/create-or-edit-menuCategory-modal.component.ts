import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ItemCategoryServiceProxy, CreateOrEditMenuCategoryDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'createOrEditMenuCategoryModal',
    templateUrl: './create-or-edit-menuCategory-modal.component.html'
})
export class CreateOrEditMenuCategoryModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    menuCategory: CreateOrEditMenuCategoryDto = new CreateOrEditMenuCategoryDto();



    constructor(
        injector: Injector,
        private _menuCategoriesServiceProxy: ItemCategoryServiceProxy
    ) {
        super(injector);
    }
    
    show(menuCategoryId?: number): void {
    

        if (!menuCategoryId) {
            this.menuCategory = new CreateOrEditMenuCategoryDto();
            this.menuCategory.id = menuCategoryId;

            this.active = true;
            this.modal.show();
        } else {
            this._menuCategoriesServiceProxy.getItemCategoryForEdit(menuCategoryId).subscribe(result => {
                this.menuCategory = result.menuCategory;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._menuCategoriesServiceProxy.createOrEdit(this.menuCategory)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
