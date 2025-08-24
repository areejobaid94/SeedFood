import { Component, ElementRef, EventEmitter, Injector, Output, ViewChild } from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { AppComponentBase } from "@shared/common/app-component-base";
import { WhatsAppConversationSessionServiceProxy, WhatsAppFreeMessageModel, WhatsAppScheduledCampaign } from "@shared/service-proxies/service-proxies";
import moment from "moment";
import { ModalDirective } from "ngx-bootstrap/modal";



@Component({
    selector: "ScheduledMessageConversation",
    templateUrl: "./scheduled-message-conversation.html",
    styleUrls: ["./scheduled-message-conversation.css"],
})
export class ScheduledMessageConversationComponent extends AppComponentBase {
    min = new Date();
    submitted=false;
 
    constructor(
        injector: Injector,
        private _WhatsAppConversationSessionServiceProxy: WhatsAppConversationSessionServiceProxy,
        public darkModeService: DarkModeService,

    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();

    }
    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("file")
    element: ElementRef;
    scheduledDate : Date
    id : number
    theme: string;
    messageStatus : boolean = false
    scheduledMessage : WhatsAppScheduledCampaign = new WhatsAppScheduledCampaign(); 
    freeMessage:WhatsAppFreeMessageModel = new WhatsAppFreeMessageModel();
    active = false;
    saving = false;

    Save(){
        
        this.saving=true;
        if(this.scheduledDate === null || this.scheduledDate === undefined ){
            this.submitted=true;
            this.saving=false;
            return;
        }
        this.scheduledMessage.campaignId = this.id;

        if (this.scheduledDate != undefined && this.scheduledDate != null) {

            //this.scheduledMessage.sendDateTime = moment(this.scheduledDate).format('YYYY-MM-DD HH:mm:ss');
            this.scheduledMessage.sendTime = moment(this.scheduledDate).locale('en').format('YYYY-MM-DD HH:mm:ss');
        } else {
            this.message.error("",this.l("pleaseEnterMessageDate"));
            this.scheduledMessage.sendDateTime = null;
            return;
        }
        this._WhatsAppConversationSessionServiceProxy.scheduleMessage(this.scheduledMessage)
        .subscribe((result) => {
            if (result != -1) 
            {
                this.notify.success(this.l("savedSuccessfully"));
                this.submitted=false;
                this.saving=false;
                this.scheduledMessage = new WhatsAppScheduledCampaign()
                this.modalSave.emit(null);
                this.modal.hide();
            }else 
            {
                this.modal.hide();
                this.message.error("", this.l("youAlreadyHaveMessageActive"));
            }
        });
        this.primengTableHelper.hideLoadingIndicator();
    }

    show(freeMessage:WhatsAppFreeMessageModel): void {
        this.id = freeMessage.id
        this.scheduledDate = null;
        this.min=new Date();
        this.min= new Date(this.min.setHours(this.min.getHours()+1,0,0));
      
        this._WhatsAppConversationSessionServiceProxy.scheduleValidation(freeMessage.id)
        .subscribe((result) => {
            if (result) {
                this.scheduledMessage = new WhatsAppScheduledCampaign();
                this._WhatsAppConversationSessionServiceProxy.getScheduledCampaignByCampaignId(freeMessage.id)

                .subscribe((result) => {
                    this.scheduledMessage = result;
                    if(result.sendDateTime !=null || result.sendDateTime !=undefined){
                    let date = moment(result.sendDateTime).format('YYYY-MM-DD HH:mm:ss')
                    this.scheduledDate=new Date(date);
                    }else{
                        this.scheduledDate=null;
                    }
                  
                });
                this.active = true;
                this.modal.show();     
            } else {
                this.message.error("", this.l("youAlreadyHaveMessageActive"));
            }
        });

    }
    close(): void {
        this.active = false;
        this.submitted=false;
        this.saving=false;
        this.modal.hide();
        this.modalSave.emit(null);

    }
    transform(value: string): string {
        return moment(value).format('YYYY-MM-DD HH:mm:ss');
      }
    
}
