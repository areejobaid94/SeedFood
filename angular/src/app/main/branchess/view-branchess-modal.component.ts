import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AreaDto} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '@app/services/dark-mode.service';

@Component({
    selector: 'viewBranchessModal',
    templateUrl: './view-branchess-modal.component.html'
})
export class ViewBranchessModalComponent extends AppComponentBase {
    theme: string;
    zoom=15;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    area: AreaDto = new AreaDto();


    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,

    ) {
        super(injector);
        this.area = new AreaDto();
    }
    ngOnInit() {
        this.theme = ThemeHelper.getTheme();
    }

    show(item: AreaDto): void {
        this.area = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
