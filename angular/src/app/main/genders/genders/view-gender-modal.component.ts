import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetGenderForViewDto, GenderDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewGenderModal',
    templateUrl: './view-gender-modal.component.html'
})
export class ViewGenderModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetGenderForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetGenderForViewDto();
        this.item.gender = new GenderDto();
    }

    show(item: GetGenderForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
