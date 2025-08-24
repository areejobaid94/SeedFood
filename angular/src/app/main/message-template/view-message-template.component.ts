import {
    Component,
    EventEmitter,
    Injector,
    Output,
    ViewChild,
} from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    MessageTemplateModel,
    WhatsAppCampaignModel,
    WhatsAppComponentModel,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { CreateCampaignComponent } from "../message-campaign/create-campaign.component";
import { Router } from "@angular/router";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
@Component({
    selector: "ViewMessageTemplate",
    templateUrl: "./view-message-template.component.html",
    styleUrls: ["./view-message-template.component.css"],
})
export class ViewMessageTemplateComponent extends AppComponentBase {
    theme: string;

    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        public darkModeService: DarkModeService,
        private router: Router,
        private sanitizer: DomSanitizer
    ) {
        super(injector);
    }
    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    MessageTemplate: MessageTemplateModel[];
    safeFormattedText: SafeHtml = '';

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
    const text = this.ComponentBody.text || '';
    this.safeFormattedText = this.sanitizer.bypassSecurityTrustHtml(
                text.replace(/\n/g, '<br>')
        );
    }
    Template: MessageTemplateModel = new MessageTemplateModel();
    TemplateId: string;
    Component: WhatsAppComponentModel[] = [new WhatsAppComponentModel()];
    ComponentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButton: WhatsAppComponentModel = new WhatsAppComponentModel();
    Buttons: WhatsAppComponentModel = new WhatsAppComponentModel();
    selectedCategory:string;
    isPlayVisible
    sub_category:string;
    imageFlag: boolean = false;
    videoFlag: boolean = false;
    documentFlag: boolean = false;
    createCampaign = false;
    campaign: WhatsAppCampaignModel = new WhatsAppCampaignModel();
    submitted = false;
    saving = false;
    templateById: MessageTemplateModel = new MessageTemplateModel();
    TemplateFromView;
    carousel: WhatsAppComponentModel = new WhatsAppComponentModel();
    CardIsValidTemplate:boolean=false;

    updateComponentBody(newBody: WhatsAppComponentModel): void {
      this.ComponentBody = newBody;
      const text = this.ComponentBody.text || '';
      this.safeFormattedText = this.sanitizer.bypassSecurityTrustHtml(
        text.replace(/\n/g, '<br>')
      );
    }
    
    viewTemplate(record: any, createCampain) {
        this.TemplateFromView = record;
        this.createCampaign = false;
        this.isPlayVisible=false;
        this.Template = new MessageTemplateModel();
        this.Component = [new WhatsAppComponentModel()];
        this.ComponentHeader = new WhatsAppComponentModel();
        this.ComponentBody = new WhatsAppComponentModel();
        this.ComponentFooter = new WhatsAppComponentModel();
        this.ComponentButton = new WhatsAppComponentModel();
        this.Buttons = new WhatsAppComponentModel();
        this.carousel = new WhatsAppComponentModel();
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateByWhatsAppId(record.id)
            .subscribe((result) => {
                //this.templateById = result;
                this.Template = result;
                this.Template.status = record.status;
                this.sub_category= this.Template.sub_category;
                this.selectedCategory=this.Template.category;

                if (this.Template.mediaType == "image") {
                    this.imageFlag = true;
                } else {
                    this.imageFlag = false;
                }
                if (this.Template.mediaType == "video") {
                    this.videoFlag = true;
                } else {
                    this.videoFlag = false;
                }
                if (this.Template.mediaType == "document") {
                    this.documentFlag = true;
                } else {
                    this.documentFlag = false;
                }
                debugger;
                this.Component = this.Template.components;
                this.Component.forEach((e) => {
                    if (e.type == "HEADER") {
                        this.ComponentHeader = e;
                    }
                    if (e.type == "BODY") {
                        this.ComponentBody = e;
                    }
                    if (e.type == "FOOTER") {
                        this.ComponentFooter = e;
                    }
                    if (e.type == "BUTTONS") {
                        this.ComponentButton = e;
                    }
                    if (e.type == "BUTTONS") {
                        this.Buttons = e;
                    }
                    if (e.type == "carousel") {
                        this.carousel = e;
                    }
                });
                this.active = true;
                this.modal.show();
            });

        if (createCampain) {
            this.createCampaign = true;
        }
    }
    // viewTemplate(record:any){
    //   this.Template=record
    //   this.TemplateId = this.Template.id
    //   this.Component = this.Template.components
    //   this.Component.forEach(e => {
    //     if (e.type == "HEADER") {
    //       this.ComponentHeader = e
    //     }
    //     if (e.type == "BODY") {
    //       this.ComponentBody = e
    //     }
    //     if (e.type == "FOOTER") {
    //       this.ComponentFooter = e
    //     }
    //     if (e.type == "BUTTON") {
    //       this.ComponentButton = e
    //     }
    //   });
    //   this.active = true;
    //   this.modal.show();

    // }
    close(): void {
        this.active = false;
        this.modal.hide();
    }

    save() {
        this.saving = true;
        this.campaign.language = "en";
        this.campaign.templateId = this.Template.localTemplateId;
        if (
            this.campaign.title === null ||
            this.campaign.title === undefined ||
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
            .subscribe(
                (result) => {
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
                },
                (error: any) => {
                    if (error) {
                        this.saving = false;
                        this.submitted = false;
                    }
                }
            );
    }
    SendCampaign(id, title, language, templateId) {
        this.templateById = new MessageTemplateModel();
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateById(templateId)
            .subscribe((result) => {
                this.templateById = result;
                this.router.navigate(["/app/main/sendCampaign"], {
                    queryParams: { id, title, language, templateId },
                });
            });
    }
    createCampaignFromModal() {
        let templateName = this.TemplateFromView.name;
        let language = this.TemplateFromView.language;
        let templateId = this.TemplateFromView.id;
        this.router.navigate(["/app/main/sendCampaign"], {
            queryParams: { language, templateId, templateName },
        });
    }

      toggleIcons(): void {
    this.isPlayVisible = !this.isPlayVisible;
  }

}
