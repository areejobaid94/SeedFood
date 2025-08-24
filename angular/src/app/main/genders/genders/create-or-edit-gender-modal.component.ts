import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { GendersServiceProxy, CreateOrEditGenderDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'createOrEditGenderModal',
    templateUrl: './create-or-edit-gender-modal.component.html'
})
export class CreateOrEditGenderModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    gender: CreateOrEditGenderDto = new CreateOrEditGenderDto();



    constructor(
        injector: Injector,
        private _gendersServiceProxy: GendersServiceProxy
    ) {
        super(injector);
    }
    
    show(genderId?: number): void {
    

        if (!genderId) {
            this.gender = new CreateOrEditGenderDto();
            this.gender.id = genderId;

            this.active = true;
            this.modal.show();
        } else {
            this._gendersServiceProxy.getGenderForEdit(genderId).subscribe(result => {
                this.gender = result.gender;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;
            this._gendersServiceProxy.createOrEdit(this.gender)
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
