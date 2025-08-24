import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ItemsServiceProxy, CreateOrEditItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditItemModal',
    templateUrl: './create-or-edit-item-modal.component.html'
})
export class CreateOrEditItemModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: CreateOrEditItemDto = new CreateOrEditItemDto();



    constructor(
        injector: Injector,
        private _itemsServiceProxy: ItemsServiceProxy
    ) {
        super(injector);
    }
    
    show(itemId?: number): void {
    

        if (!itemId) {
            this.item = new CreateOrEditItemDto();
            this.item.id = itemId;
            this.item.creationTime = moment().startOf('day');
            this.item.deletionTime = moment().startOf('day');
            this.item.lastModificationTime = moment().startOf('day');

            this.active = true;
            this.modal.show();
        } else {
            this._itemsServiceProxy.getItemForEdit(itemId).subscribe(result => {
                this.item = result.item;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._itemsServiceProxy.createOrEdit(this.item)
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
