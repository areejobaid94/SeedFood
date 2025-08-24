import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
    Input,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import { AppComponentBase } from "@shared/common/app-component-base";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import {
    CdkDragDrop,
    CdkDragEnd,
    CdkDragMove,
    CdkDragStart,
} from "@angular/cdk/drag-drop";

@Component({
    selector: "app-list-settings-modal",
    templateUrl: "./list-settings-modal.component.html",
    styleUrls: ["./list-settings-modal.component.css"],
})
export class ListSettingsModalComponent extends AppComponentBase {
    @ViewChild("viewlistsettingsmodal") modal: ModalDirective;
    @Output() modalSave: EventEmitter<any[]> = new EventEmitter<any[]>();
    @Input() settingList;
    tempList = [];
    activeCount = 0;

    theme: string;

    constructor(injector: Injector, public darkModeService: DarkModeService) {
        super(injector);
    }

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.tempList = [...this.settingList];
    }

    onCheckboxChange(item, i) {
        if (this.activeCount < 5 && this.tempList[i].isActive) {
            this.tempList[i].isActive = false;
        } else {
            this.tempList[i].isActive = true;
        }
        this.activeCount = this.tempList.filter(
            (val) => val.isActive === false
        ).length;
        // Handle checkbox change if needed
    }

    onDrop(event: CdkDragDrop<any[]>) {
        let tempItem = this.tempList[event.previousIndex];
        this.tempList.splice(event.previousIndex, 1);
        this.tempList.splice(event.currentIndex, 0, tempItem);
    }

    savePositions() {
        // Save the current state of 'items' to local storage, backend, etc.
        this.settingList = [...this.tempList];
        this.modalSave.emit(this.settingList);
        this.close();
    }

    show(): void {
        this.modal.show();
    }

    close(): void {
        this.modal.hide();
    }
}
