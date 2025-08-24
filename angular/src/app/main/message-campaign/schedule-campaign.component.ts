import { DatePipe } from "@angular/common";
import {
    Component,
    EventEmitter,
    Injector,
    OnInit,
    Output,
    ViewChild,
} from "@angular/core";
import { Router } from "@angular/router";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    CampinToQueueDto,
    SendCampinStatesModel,
    WhatsAppContactsDto,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import moment from "moment";
import { ModalDirective } from "ngx-bootstrap/modal";

@Component({
    selector: "scheduleCampaign",
    templateUrl: "./schedule-campaign.component.html",
    styleUrls: ["./schedule-campaign.component.css"],
})
export class ScheduleCampaignComponent extends AppComponentBase {
    @ViewChild("scheduleCampaign", { static: true }) modal: ModalDirective;
    @ViewChild("scheduleCampaign", { static: true })
    scheduleCampaign: ScheduleCampaignComponent;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    saving = false;
    submitted = false;
    campaignId: number = null;
    templateId: number = null;
    templateName: string = null;
    isExternalContact: boolean = false;
    contact: WhatsAppContactsDto = new WhatsAppContactsDto();
    dateAndTime: string = null;
    campinToQueueDto! : CampinToQueueDto

    @ViewChild("startDatePicker") startDatePicker;
    @ViewChild("endDatePicker") endDatePicker;

    public endDateOptions = {
        altInput: true,
        mode: "single",
        minDate: new Date(new Date().getTime() + 2 * 60000),
        altInputClass:
            "form-control flat-picker flatpickr-input invoice-edit-input",
        enableTime: true,
    };
    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        private router: Router,
        private datePipe: DatePipe,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._whatsAppMessageTemplateServiceProxy.getContactsCount().subscribe(
            (result) => {
                this.submitted = false;
                this.saving = false;
            },
            (error: any) => {
                if (error) {
                    this.submitted = false;
                    this.saving = false;
                }
            }
        );
    }

    show(isExternalContact: boolean,campaignId: number,templateId: number, contact: WhatsAppContactsDto , templateName : string , localcampinToQueueDto : CampinToQueueDto) {
        //this.contact = new WhatsAppContactsDto();
        if (contact.orderTimeFrom != null) {
            contact.orderTimeFrom = moment(contact.orderTimeFrom);
        }
        if (contact.orderTimeTo != null) {
            contact.orderTimeTo = moment(contact.orderTimeTo);
        }
        this.campaignId = Number(campaignId);
        this.templateId = Number(templateId);
        this.templateName = templateName;
        this.contact = contact;
        this.campinToQueueDto = localcampinToQueueDto; 
        this.isExternalContact = isExternalContact;
        this.endDatePicker.flatpickr.clear();
        this.modal.show();
    }

    save() {
        this.saving = true;
        if (
            this.endDatePicker.flatpickrElement.nativeElement.children[0]
                .value === ""
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        this.dateAndTime =
            this.endDatePicker.flatpickrElement.nativeElement.children[0].value;
            let sendTime : string;
            sendTime = this.datePipe
            .transform(this.dateAndTime, "yyyy-MM-dd HH:mm")
            .toString();
        this._whatsAppMessageTemplateServiceProxy
            .sendCampaignNew(
              sendTime,
                this.campinToQueueDto
            )
            .subscribe(
                (result : SendCampinStatesModel) => {
                    if(result.status){
                        this.notify.success(result.message);
                        this.submitted = false;
                        this.saving = false;
                        this.close();
                        this.router.navigate(["/app/main/messageCampaign"]);
                    }else{
                        this.notify.error(result.message);
                        this.submitted = false;
                        this.saving = false;
                        this.close();
                    }
                   
                },
                (error: any) => {
                    if (error) {
                        this.submitted = false;
                        this.saving = false;
                    }
                }
            );
    }

    close(): void {
        this.modal.hide();
        this.modalSave.emit(null);
    }
}
