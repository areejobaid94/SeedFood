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
import { TeamInboxService } from "../teaminbox.service";

@Component({
    selector: "assignToModal",
    templateUrl: "./assign-to-modal.component.html",
    styleUrls: ["./assign-to-modal.component.less"],
})
export class AssignToModalComponent extends AppComponentBase {
    theme: string;

    @ViewChild("assignModal") modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Input() agentsList = [];
    agentData;

    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private teamService: TeamInboxService
    ) {
        super(injector);
    }
    ngOnInit(): void {
        this;
        this.theme = ThemeHelper.getTheme();
    }

    show(): void {
        if (this.agentsList.length <= 0) {
            this.getUsers();
        }
        this.modal.show();
    }

    selectedAgent(agent) {
        this.agentData = agent;
    }

    save(): void {
        this.modalSave.emit(this.agentData);
    }

    close(): void {
        this.modal.hide();
    }

    getUsers() {
        this.teamService.getUsers().subscribe((result: any) => {
            this.agentsList = result.result.items;
        });
    }
}
