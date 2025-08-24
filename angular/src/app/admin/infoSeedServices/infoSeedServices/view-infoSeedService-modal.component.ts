import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetInfoSeedServiceForViewDto, InfoSeedServiceDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewInfoSeedServiceModal',
    templateUrl: './view-infoSeedService-modal.component.html'
})
export class ViewInfoSeedServiceModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetInfoSeedServiceForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetInfoSeedServiceForViewDto();
        this.item.infoSeedService = new InfoSeedServiceDto();
    }

    show(item: GetInfoSeedServiceForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
