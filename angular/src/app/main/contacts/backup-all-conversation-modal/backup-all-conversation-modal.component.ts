import {
    Component,
    EventEmitter,
    Injector,
    OnInit,
    Output,
    ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ModalDirective } from "ngx-bootstrap/modal";
import { ToastrService } from "ngx-toastr";

@Component({
    selector: "app-backup-all-conversation-modal",
    templateUrl: "./backup-all-conversation-modal.component.html",
    styleUrls: ["./backup-all-conversation-modal.component.css"],
})
export class BackupAllConversationModalComponent
    extends AppComponentBase
    implements OnInit
{
    @ViewChild("backupAllConversationModal", { static: true })
    modal: ModalDirective;

    @Output() modalResult = new EventEmitter<boolean>();

    username: string;
    password: string;
    Date: string;

    constructor(injector: Injector, private toastr: ToastrService) {
        super(injector);
    }

    ngOnInit(): void {}

    close() {
        this.modalResult.emit(false);
        this.modal.hide();
    }

    show() {
      this.username = "";
      this.password = "";
      this.Date = "";
      this.modal.show();
    }

    getTodayDate(): string {
        const today = new Date();
        const day = String(today.getDate());
        const month = String(today.getMonth() + 1);
        const year = today.getFullYear();

        return `${day}/${month}/${year}`;
    }

    checkLogin() {
        let userName = localStorage.getItem("userNameOrEmailAddress");
        let password = localStorage.getItem("password");
        let dateOfToday = this.getTodayDate();

        if (
            userName == this.username &&
            password == this.password &&
            dateOfToday == this.Date
        ) {
            return true;
        } else {
            return false;
        }
    }

    backup() {
        if (this.checkLogin()) {
            this.modalResult.emit(true);
            this.toastr.info(
                "Operation is successful request sent",
                "Operation status",
                {
                    positionClass: "toast-bottom-right",
                    timeOut: 3000,
                    progressBar: true,
                }
            );
            this.modal.hide();
        } else {
            this.modalResult.emit(false);
            this.toastr.error(
                "Operation Failed Wrong Input",
                "Operation status",
                {
                    positionClass: "toast-bottom-right",
                    timeOut: 3000,
                    progressBar: true,
                }
            );
        }
    }
}
