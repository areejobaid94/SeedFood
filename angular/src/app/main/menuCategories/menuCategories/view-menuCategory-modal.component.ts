import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetMenuCategoryForViewDto, MenuCategoryDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewMenuCategoryModal',
    templateUrl: './view-menuCategory-modal.component.html'
})
export class ViewMenuCategoryModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetMenuCategoryForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetMenuCategoryForViewDto();
        this.item.menuCategory = new MenuCategoryDto();
    }

    show(item: GetMenuCategoryForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
