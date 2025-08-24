import { SharedService } from "./../../../../../shared/shared-services/shared.service";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import {
    Component,
    Injector,
    Input,
    OnInit,
    ViewChild,
    inject,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ModalDirective } from "ngx-bootstrap/modal";
import { DarkModeService } from "@app/services/dark-mode.service";
import { Router } from "@angular/router";
import { GroupServiceProxy } from "@shared/service-proxies/service-proxies";

@Component({
    selector: "app-contactGroup-modal",
    templateUrl: "./contactGroup-modal.component.html",
    styleUrls: ["./contactGroup-modal.component.css"],
})
export class ContactGroupModalComponent
    extends AppComponentBase
    implements OnInit
{
    @ViewChild("groupcontactModal", { static: true }) modal: ModalDirective;
    @Input() isEdit: boolean;
    selectedId: number;
    oldCotactsChecked: boolean = false;
    newCotactsChecked: boolean = false;
    unsubscribedChecked: boolean = false;
    groupNameModel: string = "";
    isGroupNameValid: boolean = true;
    groupNameStatus: number = 2;
    nextbtn: boolean = false;

    private groupSharedService: SharedService = inject(SharedService);

    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private router: Router,
        private groupService: GroupServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {}

handleChange(id: number) {
    if (id === 1) {
        this.oldCotactsChecked = true;
        this.newCotactsChecked = false;
        this.unsubscribedChecked = false;
    } else if (id === 2){
        this.oldCotactsChecked = false;
        this.newCotactsChecked = true;
        this.unsubscribedChecked = false;
        
    } else if (id === 3) {
        this.oldCotactsChecked = false;
        this.newCotactsChecked = false;
        this.unsubscribedChecked = true;
    }
}

    hasOnlySpaces(inputString: string): boolean {
        return /^\s*$/.test(inputString);
    }

    onSelectionChange(id: number) {
        this.selectedId = id;
    }
    handleNextButton() {
        if (!this.oldCotactsChecked && !this.newCotactsChecked && !this.unsubscribedChecked) {
            return;
        }
        this.hasOnlySpaces(this.groupNameModel);
        this.nextbtn = true;

        this.groupService
            .validGroupName(this.groupNameModel)
            .subscribe((res) => {
                this.nextbtn = false;
                switch (res.state) {
                    case 2:
                        this.groupSharedService.changeGroupName(
                            this.groupNameModel
                        );

                        this.router.navigate(["/app/main/group/creategroup"], {
                            queryParams: {
                                isExternal: this.newCotactsChecked
                                    ? this.newCotactsChecked
                                    : false,
                                isUnsubscribed: this.unsubscribedChecked
                            },
                        });
                        break;
                    case 1 : 
                    this.message.error("", this.l("TheGroupNameNotEmptyAndNotExceed"));
                    break;
                    case 3 : 
                    this.message.error("", this.l("groupNameisUsed"));
                    break;
                    default:
                        this.message.error("", res?.message || ' internal Server Error');
                }
            });
    }

    open(): void {
        this.groupNameModel = "";
        this.modal.show();
        this.nextbtn = false;
    }

    close(): void {
        this.oldCotactsChecked = false;
        this.newCotactsChecked = false;
        this.nextbtn = false;
        this.isGroupNameValid = true;
        this.groupNameStatus = 2;
        this.modal.hide();
    }
}
