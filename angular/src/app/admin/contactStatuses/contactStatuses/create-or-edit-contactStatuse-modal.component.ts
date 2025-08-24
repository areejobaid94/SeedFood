import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ContactStatusesServiceProxy, CreateOrEditContactStatuseDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditContactStatuseModal',
    templateUrl: './create-or-edit-contactStatuse-modal.component.html'
})
export class CreateOrEditContactStatuseModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    contactStatuse: CreateOrEditContactStatuseDto = new CreateOrEditContactStatuseDto();



    constructor(
        injector: Injector,
        private _contactStatusesServiceProxy: ContactStatusesServiceProxy
    ) {
        super(injector);
    }
    
    show(contactStatuseId?: number): void {
    

        if (!contactStatuseId) {
            this.contactStatuse = new CreateOrEditContactStatuseDto();
            this.contactStatuse.id = contactStatuseId;

            this.active = true;
            this.modal.show();
        } else {
            this._contactStatusesServiceProxy.getContactStatuseForEdit(contactStatuseId).subscribe(result => {
                this.contactStatuse = result.contactStatuse;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._contactStatusesServiceProxy.createOrEdit(this.contactStatuse)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
