import {
    Component,
    ElementRef,
    Injector,
    ViewChild,
    ViewEncapsulation,
} from "@angular/core";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/common/app-component-base";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";
import { AddMessageTemplateComponent } from "./add-message-template.component";
import {
    MessageTemplateModel,
    WhatsAppComponentModel,
    WhatsAppMessageTemplateModel,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ViewMessageTemplateComponent } from "./view-message-template.component";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import * as rtlDetect from "rtl-detect";
import { viewResons } from "./view-reasons.component";
import { LazyLoadEvent } from "primeng/api";
import { SendMessageModalComponent } from "../chat-them12/send-message-modal/send-message-modal.component";
import { SendMessageModalFromTeamplatComponent } from "./send-message-modal-from-Template/send-message-modal-from-Template.component";
import { FacebookTemplateComponent } from "./Facebook-Template/Facebook-Template.component";
import { Router } from "@node_modules/@angular/router";

@Component({
    templateUrl: "./message-template.component.html",
    styleUrls: ["./message-template.component.css"],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class MessageTemplateComponent extends AppComponentBase {
    [x: string]: any;
    theme: string;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    @ViewChild("AddAddMessageTemplate", { static: true }) AddMessageTemplate: AddMessageTemplateComponent;
    @ViewChild("FirstTemplate", { static: true }) FirstTemplate: FacebookTemplateComponent;


    @ViewChild("ViewMessageTemplate", { static: true })
    ViewMessageTemplate: ViewMessageTemplateComponent;

    @ViewChild("viewResons", { static: true })
    viewResons: viewResons;

    @ViewChild("sendTestMessageModal", { static: false }) 
    sendTestMessageModal: SendMessageModalFromTeamplatComponent;
    templateId: string;
  
    modal:MessageTemplateModel[]
    isModalOpen: boolean;

    constructor(
        private router: Router,
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        public darkModeService: DarkModeService
    ) {
        super(injector);
    }
    filterText = "";
    Template: MessageTemplateModel[] = [new MessageTemplateModel()];
    Component: WhatsAppComponentModel[] = [new WhatsAppComponentModel()];
    ComponentModel: WhatsAppComponentModel[] = [new WhatsAppComponentModel()];
    isArabic = false;
    templates: any[] = [];
    isVisible:boolean;

    addTemplate(): void {
        
        this.router.navigate(['/app/main/start']);
    }
    Edit(mdoel:MessageTemplateModel): void {
        debugger
        this.router.navigate(['/app/main/EditTemplateFacebook'], {
            queryParams: {
              templateId: mdoel.id,
            },
          });
     // this.FirstTemplate.edit(mdoel);
    }
    ngOnInit(event?: LazyLoadEvent): void {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
    }
    checkTemplateName(templateName) {
        if (
            templateName === "reminder_booking_19" ||
            templateName === "booking_template_ar_19" ||
            templateName === "reminder_booking_ar_19" ||
            templateName === "booking_template_19"
        ) {
            return true;
        }
    }



    getWhatsAppTemplates(event?: LazyLoadEvent) {
        this.primengTableHelper.showLoadingIndicator();
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppMessageTemplate()
            .subscribe((result) => {

                
                this.primengTableHelper.records = result.data;
                this.Template = result.data;
                this.primengTableHelper.totalRecordsCount = result.data.filter(
                    (i) => i.language === "en" || i.language === "ar"
                ).length;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }
    // getStatistics(start: Moment,end:Moment){
    //   this._whatsAppMessageTemplateServiceProxy.getWhatsAppAnalytic(start,end).subscribe(result => {

    //     console.log(result);

    //     });
    // }
    Sync(event?: LazyLoadEvent) {
        this._whatsAppMessageTemplateServiceProxy
            .syncTemplate()
            .subscribe((result) => {});
        this.getWhatsAppTemplates(event);
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    UpdateTemplate(template: any) {
        this._whatsAppMessageTemplateServiceProxy
            .updateTemplate(this.appSession.tenantId, template)
            .subscribe((result) => {
                if (result.success == true) {
                    this.notify.success(this.l("succcessfullyUpdated"));
                }
                if (result.error != null) {
                    this.message.error(
                        result.error.error_user_title,
                        result.error.error_user_msg
                    );
                }
            });
    }

    DeleteTemplate(templateName: string, event?: LazyLoadEvent) {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._whatsAppMessageTemplateServiceProxy
                    .deleteWhatsAppMessageTemplate(templateName)
                    .subscribe((result) => {
                        if (result.success == true) {
                            this.notify.success(this.l("successfullyDeleted"));
                            this.getWhatsAppTemplates(event);
                        }
                        if (result.error != null) {
                            this.message.error(
                                result.error.error_user_title,
                                result.error.error_user_msg
                            );
                        }
                    });
            }
        });
    }


    send(model:MessageTemplateModel){
        this.isModalOpen = true;
        this.sendTestMessageModal.show(model);
    }

 



}
