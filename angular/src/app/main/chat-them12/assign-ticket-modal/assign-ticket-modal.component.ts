import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from "@angular/core";
import { TeamInboxService } from "@app/main/teamInbox/teaminbox.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { AppComponentBase } from "@shared/common/app-component-base";
import { LiveChatServiceProxy } from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { ToastrService } from "ngx-toastr";
import { ChatThem12Component } from "../chat-them12.component";

interface assignedUser {
    id: number;
    fullName: string;
    isActive: boolean;
}

@Component({
    selector: "app-assign-ticket-modal",
    templateUrl: "./assign-ticket-modal.component.html",
    styleUrls: ["./assign-ticket-modal.component.css"],
})
export class AssignTicketModalComponent
    extends AppComponentBase
    implements OnInit
{
    @ViewChild("assignusersmodal") modal: ModalDirective;
    @Output() submitClicked: EventEmitter<void> = new EventEmitter<void>();

  //   @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    // @Input() agentsList = [];
     agentData;
  //   @ViewChild('assignTicketModal', { static: false }) assignTicketModal: ChatThem12Component;

    theme;
    @Input() users;
    selectedUsersToAssign: assignedUser[] = [];
    unChangedUsersToAssign: number[] = [];
    ticketId: number = 0;
    isModified: boolean = false;

    constructor(
        injector: Injector,
        private _liveChatServiceProxy: LiveChatServiceProxy,
        private teamService: TeamInboxService,
        private toastr: ToastrService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
    }

    show(ticket): void {
        this.ticketId = ticket.idLiveChat;
        this.isModified = false;
        let tempSelectedUsers;
        tempSelectedUsers = ticket.userIds.split(",");
        this.selectedUsersToAssign = [];
        this.unChangedUsersToAssign = [];

        this.users.forEach((user) => {
            let temp: assignedUser = {
                id: user.id,
                fullName: user.fullName,
                isActive: false,
            };
            if (
                tempSelectedUsers.findIndex(
                    (id) => parseInt(id, 10) === user.id
                ) != -1
            ) {
                temp.isActive = true;
                this.unChangedUsersToAssign.push(temp.id);
            } else {
                temp.isActive = false;
            }
            this.selectedUsersToAssign.push(temp);
        });
        this.modal.show();
    }

    showInbox(ticketId: number) {
        this.ticketId = ticketId;
        this.selectedUsersToAssign = [];
        this.unChangedUsersToAssign = [];
        this.isModified = false;

        if (this.users.length <= 0) {
            this.teamService.getUsers().subscribe((result: any) => {
                this.users = result.result.items;
                this.users.forEach((user) => {
                    let temp: assignedUser = {
                        id: user.id,
                        fullName: user.fullName,
                        isActive: false,
                    };
                    this.selectedUsersToAssign.push(temp);
                    this.modal.show();
                });
            });
        } else {
            this.users.forEach((user) => {
                let temp: assignedUser = {
                    id: user.id,
                    fullName: user.fullName,
                    isActive: false,
                };
                this.selectedUsersToAssign.push(temp);
            });
            this.modal.show();
        }
    }

    close(): void {
        this.modal.hide();
    }

    onCheckboxChange(user) {
        let tempUserIndex = this.selectedUsersToAssign.findIndex(
            (assignedUser) => assignedUser.id === user.id
        );
        this.selectedUsersToAssign[tempUserIndex].isActive =
            !this.selectedUsersToAssign[tempUserIndex].isActive;

        if (
            this.listsAreDifferent(
                this.selectedUsersToAssign,
                this.unChangedUsersToAssign
            )
        ) {
            this.isModified = true;
        } else {
            this.isModified = false;
        }
    }

    listsAreDifferent(list1, list2) {
        let tempTrue = list1
            .filter((x) => x.isActive === true)
            .map((l) => l.id);

        // Check if arrays have the same length
        if (tempTrue.length !== list2.length) {
            return true;
        }

        // Sort both arrays to make comparison easier
        tempTrue.sort();
        list2.sort();

        // Check if all elements in both arrays are the same
        for (let i = 0; i < tempTrue.length; i++) {
            if (tempTrue[i] !== list2[i]) {
                return true;
            }
        }

        // If none of the above conditions are met, the lists are the same
        return false;
    }

    assignUsers() {
        let userIds: string = this.selectedUsersToAssign
            .filter((item) => item.isActive) // Filter out items where `active` is true
            .map((item) => item.id) // Map to the `id` property
            .join(",");

        this._liveChatServiceProxy
            .assignLiveChatToUser(
                this.ticketId,
                userIds,
                undefined,
                this.appSession.user.userName,
                this.appSession.user.id,""
            )
            .subscribe(() => {
                this.toastr.info(
                    "Operation is successful",
                    "Operation status",
                    {
                        positionClass: "toast-bottom-right",
                        timeOut: 3000,
                        progressBar: true,
                    }
                );
            });
            debugger;
            this.submitClicked.emit();
            this.close()

        }
    
      
    
}
