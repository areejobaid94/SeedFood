import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import {
    CustomerLiveChatModel,
    LiveChatServiceProxy,
    SellingRequestServiceProxy,   
    TemplateMessagesServiceProxy,
    UserServiceProxy,

} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import { TeamInboxService } from "../teamInbox/teaminbox.service";
import { catchError } from "rxjs/operators";
import { of } from "rxjs";
import Swal from "sweetalert2";
import { UntypedFormGroup } from "@node_modules/@angular/forms";
declare var bootstrap: any;

@Component({
    selector: "viewTicketModal",
    templateUrl: "./view-ticket-modal.component.html",
})
export class ViewTicketModalComponent extends AppComponentBase {
    
    theme: string;
    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Output() reSearch: EventEmitter<any> = new EventEmitter<any>();
    @Output() openModalEvent = new EventEmitter<void>();



    chatForm: UntypedFormGroup;
    titleMessage: string;
    templates: any[] = [];

    ConversationisExpired:boolean=false
    active = false;
    saving = false;
    totalAll = 0;
    submitted = false;
    summary = "";
    ticketId = 0;
    contactId: number;
    type: string = "ticket";
    data: CustomerLiveChatModel;
    ConfirmedOrReject: string;

    constructor(
        injector: Injector,
        private _liveChatServiceProxy: LiveChatServiceProxy,
        private teamService: TeamInboxService,
          private _userService: UserServiceProxy,
        public darkModeService: DarkModeService,
        private _sellingRequestServiceProxy: SellingRequestServiceProxy,
        private templateMessagesServiceProxy: TemplateMessagesServiceProxy

    ) {
        super(injector);
    }

    ngOnInit(): void {

        this.theme = ThemeHelper.getTheme();
    }

    show(
        ticketId: number,
        contactId: number,
        type: string = "ticket",
        data?: CustomerLiveChatModel,
        ConfirmedOrReject?: string
    ): void {

let userId=76;


debugger

this._userService.userTicketsOpenUpdate(this.appSession.userId,false )
                .subscribe(
                    (response) => {

debugger
        this.type = type;
        this.ticketId = ticketId;
        this.contactId = contactId;
        this.active = true;
        this.summary = null;
        this.data = data;
        this.ConfirmedOrReject = ConfirmedOrReject;

        this.modal.show();



                    },
                    (error: any) => {
                  
                    }
                );


       
    }

    CloseTicket() {
        debugger;

        this.saving = true;

        if (!this.summary) {
            this.submitted = true;
            this.saving = false;
            return;
        }

        this._liveChatServiceProxy
            .updateTicket(
                this.appSession.user.id,
                this.appSession.user.userName,
                this.ticketId,
                3,
                false,
                this.summary
            )
            .pipe(
                catchError((error) => {
                    console.log(error);
                    this.notify.error(this.l("ErrorSaving"));
                    this.active = false;
                    this.saving = false;
                    return of(null); // Return an observable that emits null
                })
            )
            .subscribe((result) => {
                debugger;
                if (result) {
                    // Proceed only if the result is not null
                    this.closeChatFromTeamInbox(this.contactId);
                    this.notify.success(this.l("SavedSuccessfully"));
                    this.active = false;
                    this.saving = false;
                    this.modal.hide();
                }
            });
    }

    CloseRequest() {
        this.saving = true;
    
        if (!this.summary) {
            this.submitted = true;
            this.saving = false;
            return;
        }
    
        this._sellingRequestServiceProxy
            .ticketUpdateStatus(
                this.data.idLiveChat,
                this.ConfirmedOrReject == "reject" ? 5 : 4,
                this.summary,
                2
            )
            .subscribe(
                (result) => {
                    this.active = false;
    
                    if (result === "the Conversationis Expired") {
                        this.modal.hide();
                        // this.notify.error(result);
    
                        // Delay to ensure modal is properly displayed
                        setTimeout(() => {
                            this.changeStatus();
                        }, 500); // Wait 500ms before showing the SweetAlert
                    } else {
                        this.notify.success(
                            this.ConfirmedOrReject == "reject"
                                ? this.l("SuccessfullyDeleted")
                                : this.l("successfullyDone")
                        );
                        this.modal.hide();
                    }
    
                    this.saving = false;
                },
                (error) => {
                    this.saving = false;
                    this.notify.error(error);
                }
            );
    }
    
    changeStatus() {
    
        Swal.fire({
            title: 'Ticket Expired!',
            text: 'This ticket has been pending for more than 24 hours. If you wish to reply, you can activate the chat.',
            icon: 'info',
            confirmButtonText: 'Activate Chat',
            showCancelButton: true, // Optional: Add a cancel button
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                // User clicked "Activate Chat"
                this.openModalEvent.emit();
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                // console.log("Chat activation canceled");
            }
        });
    }
    
    done(): void {
        if (this.type === "ticket") {
            this.CloseTicket();
        } else {
            this.CloseRequest();
        }
    }

    closeChatFromTeamInbox(contactId): void {
        this.teamService
            .updateCustomerStatus(
                contactId,
                false,
                this.appSession.user.userName,
                0
            )
            .subscribe();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    updateStatus(){
        debugger
        this._sellingRequestServiceProxy
        .ticketUpdateStatus(
            this.data.idLiveChat,
            this.ConfirmedOrReject == "reject" ? 5 : 4,
            this.summary,
            1
        )
        .subscribe(
            (result) => {
                debugger;
                this.active = false;
                if (result == "the Conversationis Expired") {
                    // this.notify.error(result);
                } else {
                    this.notify.success(
                        this.ConfirmedOrReject == "reject"
                            ? this.l("SuccessfullyDeleted")
                            : this.l("successfullyDone")
                    );
                }

                this.saving = false;
                this.modal.hide();
            },
            (error) => {
                debugger;
                this.notify.error(error);
            }
        );
    }

        getTemplateMessage(templateMessageId: number) {
            this.templateMessagesServiceProxy
                .getTemplateMessageForEdit(templateMessageId)
                .subscribe((result) => {
                    if (this.summary) {
                        this.summary += result.templateMessage.messageText;
                    } else {
                        this.summary = result.templateMessage.messageText;
                    }
                    const modalEl = document.getElementById('quickAnswersModal');
                    const modalInstance = bootstrap.Modal.getInstance(modalEl);
                    if (modalInstance) {
                        modalInstance.hide();
                    }
                });
        }


    
    getAllTemplates() {
        
        if((!this.templates)||this.templates.length==0){
        this.templateMessagesServiceProxy.getAllNoFilter().subscribe(
            (result) => {
                this.templates = result.items;
            },
            (error) => this.notify.error("Error Happened")
        );
    }
}

}
