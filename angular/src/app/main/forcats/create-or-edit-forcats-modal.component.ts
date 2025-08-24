import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {CreateOrEditForcatsDto, ForcatsesServiceProxy} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'createOrEditForcatsModal',
    templateUrl: './create-or-edit-forcats-modal.component.html'
})
export class CreateOrEditForcatsModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    forcats: CreateOrEditForcatsDto = new CreateOrEditForcatsDto();



    constructor(
        injector: Injector,
        private _forcatsServiceProxy: ForcatsesServiceProxy
    ) {
        super(injector);
    }
    
    show(forcatsId?: number): void {
    

        if (!forcatsId) {
            this.forcats = new CreateOrEditForcatsDto();
            this.forcats.id = forcatsId;

            this.active = true;
            this.modal.show();
        } else {
            this._forcatsServiceProxy.getForcatsForEdit(forcatsId).subscribe(result => {
                this.forcats = result.forcats;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;
            this._forcatsServiceProxy.createOrEdit(this.forcats)
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
