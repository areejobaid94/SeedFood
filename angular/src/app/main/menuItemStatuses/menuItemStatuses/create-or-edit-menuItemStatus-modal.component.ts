import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { MenuItemStatusesServiceProxy, CreateOrEditMenuItemStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditMenuItemStatusModal',
    templateUrl: './create-or-edit-menuItemStatus-modal.component.html'
})
export class CreateOrEditMenuItemStatusModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    menuItemStatus: CreateOrEditMenuItemStatusDto = new CreateOrEditMenuItemStatusDto();



    constructor(
        injector: Injector,
        private _menuItemStatusesServiceProxy: MenuItemStatusesServiceProxy
    ) {
        super(injector);
    }
    
    show(menuItemStatusId?: number): void {
    

        if (!menuItemStatusId) {
            this.menuItemStatus = new CreateOrEditMenuItemStatusDto();
            this.menuItemStatus.id = menuItemStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._menuItemStatusesServiceProxy.getMenuItemStatusForEdit(menuItemStatusId).subscribe(result => {
                this.menuItemStatus = result.menuItemStatus;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._menuItemStatusesServiceProxy.createOrEdit(this.menuItemStatus)
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
