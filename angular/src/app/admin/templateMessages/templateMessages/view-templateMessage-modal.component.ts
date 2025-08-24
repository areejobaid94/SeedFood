import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTemplateMessageForViewDto, TemplateMessageDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';

@Component({
    selector: 'viewTemplateMessageModal',
    templateUrl: './view-templateMessage-modal.component.html'
})
export class ViewTemplateMessageModalComponent extends AppComponentBase {
    theme:string;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    

    active = false;
    saving = false;

    item: GetTemplateMessageForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetTemplateMessageForViewDto();
        this.item.templateMessage = new TemplateMessageDto();
    }
    ngOnInit(): void {
        this.theme= ThemeHelper.getTheme();
    }

    show(item: GetTemplateMessageForViewDto): void {
        
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
