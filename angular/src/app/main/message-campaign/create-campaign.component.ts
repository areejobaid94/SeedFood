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
    MessageTemplateModel,
    WhatsAppCampaignModel,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";

@Component({
    selector: "createCampaign",
    templateUrl: "./create-campaign.component.html",
    styleUrls: ["./create-campaign.component.css"],
})
export class CreateCampaignComponent extends AppComponentBase {
    @ViewChild("createCampaign", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("createCampaign", { static: true })
    createCampaign: CreateCampaignComponent;
    campaign: WhatsAppCampaignModel = new WhatsAppCampaignModel();
    submitted = false;
    saving = false;
    languageEnum = [
        { id: "en", name: "English" },
        { id: "ar", name: "Arabic" },
    ];
    template: MessageTemplateModel[];
    templateById: MessageTemplateModel = new MessageTemplateModel();
    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit(): void {
      
    }

    show() {
        if(this.template.length <= 0)
        {
            this.getTemplates();
        }
        this.modal.show();
    }
    close(): void {
        this.modal.hide();
        this.modalSave.emit(null);
    }
    save() {
        this.saving = true;
        this.campaign.language = "en";
        if (
            (this.campaign.title === null &&
                this.campaign.title === undefined) ||
            this.campaign.title === "" ||
            this.campaign.language === null ||
            this.campaign.language === undefined ||
            this.campaign.language === "" ||
            this.campaign.templateId === null ||
            this.campaign.templateId === undefined
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }

        this._whatsAppMessageTemplateServiceProxy
            .addWhatsAppCampaign(this.campaign)
            .subscribe((result) => {
                this.notify.success(this.l("Successfully Created"));
                if (result.valueOf()) {
                    this.campaign.id = result;
                    this.submitted = true;
                    this.saving = false;
                    this.close();
                    this.SendCampaign(
                        this.campaign.id,
                        this.campaign.title,
                        this.campaign.language,
                        this.campaign.templateId
                    );
                }
            });
    }

    getTemplates() {
        this.template = [];
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppTemplateForCampaign(0, 50, this.appSession.tenantId)
            .subscribe((result) => {
                this.template = result.lstWhatsAppTemplateModel.filter(
                    (element) =>
                        element.language == "ar" || element.language == "en"
                );
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    SendCampaign(id, title, language, templateId) {
       
        this.templateById = new MessageTemplateModel();
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateById(templateId)
            .subscribe((result) => {
                this.templateById = result;
                this.router.navigate(["/app/main/sendCampaign"]);
            });
    }
}
