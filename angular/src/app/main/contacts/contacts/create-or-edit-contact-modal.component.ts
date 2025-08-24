import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ContactDto, ContactsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
//import { ContactChatStatuseLookupTableModalComponent } from './contact-chatStatuse-lookup-table-modal.component';
//import { ContactContactStatuseLookupTableModalComponent } from './contact-contactStatuse-lookup-table-modal.component';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '@app/services/dark-mode.service';

@Component({
    selector: 'createOrEditContactModal',
    templateUrl: './create-or-edit-contact-modal.component.html'
})
export class CreateOrEditContactModalComponent extends AppComponentBase {
    theme:string;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    //@ViewChild('contactChatStatuseLookupTableModal', { static: true }) contactChatStatuseLookupTableModal: ContactChatStatuseLookupTableModalComponent;
    //@ViewChild('contactContactStatuseLookupTableModal', { static: true }) contactContactStatuseLookupTableModal: ContactContactStatuseLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    isBlock:boolean;
    unBlock:boolean;
    active = false;
    saving = false;

    contact: ContactDto = new ContactDto();

    // chatStatuseChatStatusName = '';
    // contactStatuseContactStatusName = '';


    constructor(
        public darkModeService: DarkModeService,
        injector: Injector,
        private _contactsServiceProxy: ContactsServiceProxy
    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme= ThemeHelper.getTheme();
    }
    
    show(contactId?: number): void {
        if (!contactId) {
            this.contact = new ContactDto();
            this.contact.id = contactId;
            // this.chatStatuseChatStatusName = '';
            // this.contactStatuseContactStatusName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._contactsServiceProxy.getContactbyId(contactId).subscribe(result => {
                this.contact = result;
                // this.chatStatuseChatStatusName = result.chatStatuseChatStatusName;
                // this.contactStatuseContactStatusName = result.contactStatuseContactStatusName;

                this.active = true;
                this.modal.show();
            });
        }
        
    }
    save(): void {
            this.saving = true;
            this._contactsServiceProxy.createOrEdit(this.contact)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    // openSelectChatStatuseModal() {
    //     this.contactChatStatuseLookupTableModal.id = this.contact.chatStatuseId;
    //     this.contactChatStatuseLookupTableModal.displayName = this.chatStatuseChatStatusName;
    //     this.contactChatStatuseLookupTableModal.show();
    // }
    // openSelectContactStatuseModal() {
    //     this.contactContactStatuseLookupTableModal.id = this.contact.contactStatuseId;
    //     this.contactContactStatuseLookupTableModal.displayName = this.contactStatuseContactStatusName;
    //     this.contactContactStatuseLookupTableModal.show();
    // }


    // setChatStatuseIdNull() {
    //     this.contact.chatStatuseId = null;
    //     this.chatStatuseChatStatusName = '';
    // }
    // setContactStatuseIdNull() {
    //     this.contact.contactStatuseId = null;
    //     this.contactStatuseContactStatusName = '';
    // }


    // getNewChatStatuseId() {
    //     this.contact.chatStatuseId = this.contactChatStatuseLookupTableModal.id;
    //     this.chatStatuseChatStatusName = this.contactChatStatuseLookupTableModal.displayName;
    // }
    // getNewContactStatuseId() {
    //     this.contact.contactStatuseId = this.contactContactStatuseLookupTableModal.id;
    //     this.contactStatuseContactStatusName = this.contactContactStatuseLookupTableModal.displayName;
    // }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
