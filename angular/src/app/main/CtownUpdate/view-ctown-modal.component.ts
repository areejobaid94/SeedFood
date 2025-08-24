import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {  CTownApiServiceProxy} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { id } from '@swimlane/ngx-charts';

@Component({
    selector: 'viewctownModal',
    templateUrl: './view-ctown-modal.component.html'
})
export class ViewctownModalComponent extends AppComponentBase {
    theme:string;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;



    constructor(
        injector: Injector,
        private _CTownApiServiceProxy: CTownApiServiceProxy,
    ) {
        super(injector);
    }



    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
