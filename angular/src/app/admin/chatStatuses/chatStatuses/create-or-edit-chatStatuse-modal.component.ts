import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ChatStatusesServiceProxy, CreateOrEditChatStatuseDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditChatStatuseModal',
    templateUrl: './create-or-edit-chatStatuse-modal.component.html'
})
export class CreateOrEditChatStatuseModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    chatStatuse: CreateOrEditChatStatuseDto = new CreateOrEditChatStatuseDto();



    constructor(
        injector: Injector,
        private _chatStatusesServiceProxy: ChatStatusesServiceProxy
    ) {
        super(injector);
    }
    
    show(chatStatuseId?: number): void {
    

        if (!chatStatuseId) {
            this.chatStatuse = new CreateOrEditChatStatuseDto();
            this.chatStatuse.id = chatStatuseId;

            this.active = true;
            this.modal.show();
        } else {
            this._chatStatusesServiceProxy.getChatStatuseForEdit(chatStatuseId).subscribe(result => {
                this.chatStatuse = result.chatStatuse;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._chatStatusesServiceProxy.createOrEdit(this.chatStatuse)
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
