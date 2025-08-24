import { Component, Injector, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    GetAllDashboard,
    MessageTemplateModel,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import * as rtlDetect from "rtl-detect";
import { CreateCampaignComponent } from "./create-campaign.component";
import { CampaignStatisticsComponent } from "./campaign-statistics.component";
import moment from "moment";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import { FileDownloadService } from "@shared/utils/file-download.service";
const { toGregorian } = require("hijri-converter");

@Component({
    selector: "app-message-campaign",
    templateUrl: "./message-campaign.component.html",
    styleUrls: ["./message-campaign.component.css"],
})
export class MessageCampaignComponent extends AppComponentBase {
    theme: string;
    isArabic = false;
    first;
    rows;
    type = 1

    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        private router: Router,
        public darkModeService: DarkModeService,
        private _fileDownloadService: FileDownloadService,
    ) {
        super(injector);
    }
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    @ViewChild("createCampaign", { static: true })
    createCampaign: CreateCampaignComponent;
    @ViewChild("campaignStatistics", { static: true })
    campaignStatistics: CampaignStatisticsComponent;

    TotalRemainingAdsConversation: number = 0;

    tabIndex = 0;
    Measurement: GetAllDashboard = new GetAllDashboard();
    templateById: MessageTemplateModel = new MessageTemplateModel();

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        // if (this.primengTableHelper.shouldResetPaging(event)) {
        //     this.paginator.changePage(0);
        //     return;
        // }

        //this.GetStatistics()
        this.primengTableHelper.showLoadingIndicator();
    }
    
    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split("T");
        const [year, month, day] = hijriDateString.split("-").map(Number);
        const gregorianDate = toGregorian(year, month, day);

        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, "0"),
            String(gregorianDate.gd).padStart(2, "0"),
        ].join("-");

        return formattedDate + "T" + time;
    }

    getLocalizedText(key: string): string {
        return this.l(key);
    }

    cancelScheduledCampaign(campaignId) {
        this._whatsAppMessageTemplateServiceProxy
            .updateActivationScheduledCampaign(campaignId)
            .subscribe((result) => {
                this.notify.success(this.l("SuccessfullySaved"));
                this.ngOnInit();
            });
    }

    generateReport(campaignId, templateId) {

        

        this._whatsAppMessageTemplateServiceProxy
        .backUpCampaginForAll(campaignId)
        .subscribe((result) => {
            this._fileDownloadService.downloadTempFile(result);
            this.reloadPage();
            this.notify.success(this.l('SuccessfullyBackUp'));
        });

        // this.router.navigate(["/app/main/externalContacts"], {
        //     queryParams: { campaignId, templateId },
        // });
    }

    getWhatsAppCampaign(event?: LazyLoadEvent) {
        this.primengTableHelper.showLoadingIndicator();
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppCampaign(
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(
                    this.paginator,
                    event
                ),
                this.appSession.tenantId,
                1
            )
            .subscribe((result) => {
                this.primengTableHelper.records =
                    result.lstWhatsAppCampaignModel;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }
    GetStatistics() {
        this.primengTableHelper.showLoadingIndicator();

        this._whatsAppMessageTemplateServiceProxy
            .getStatistics(null)
            .subscribe((result) => {
                this.Measurement = result;

                this.TotalRemainingAdsConversation =
                    this.Measurement.remainingFreeConversation +
                    this.Measurement.remainingBIConversation;
            });
    }
    getTemplates() {
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppTemplateForCampaign(0, 50, this.appSession.tenantId)
            .subscribe((result) => {
                this.primengTableHelper.records =
                    result.lstWhatsAppTemplateModel;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    CreateCampaignPage() {
        this.router.navigate(["/app/main/sendCampaign"]);
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    SendCampaign(id, title, language, templateId) {
        this.templateById = new MessageTemplateModel();
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateById(templateId)
            .subscribe((result) => {
                this.templateById = result;
                if (this.templateById.isDeleted == true) {
                    this.message.error("", this.l("templateHasBeenDeleted"));
                } else {
                    this.router.navigate(["/app/main/sendCampaign"], {
                        queryParams: { id, title, language, templateId },
                    });
                }
            });
    }

    DeleteCampaign(campaign: any) {
        if (campaign.status == 1) {
            this.message.error("", this.l("cantDeletCampaign"));

            return;
        } else {
            this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
                if (isConfirmed) {
                    this._whatsAppMessageTemplateServiceProxy
                        .deleteWhatsAppCampaign(campaign.id)
                        .subscribe((result) => {
                            this.reloadPage();
                            this.notify.success(this.l("successfullyDeleted"));
                        });
                }
            });
        }
    }

    getData(event?: LazyLoadEvent) {
        this.first = event.first;
        this.rows = event.rows;
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppCampaign(
                event?.first,
                event?.rows,
                this.appSession.tenantId,
                this.type 
            )
            .subscribe((result) => {
                if (this.isArabic) {
                    result.lstWhatsAppCampaignModel.forEach((element) => {
                        element.sentTime = moment(
                            this.convertHijriToGregorian(
                                moment(element.sentTime)
                                    .locale("en")
                                    .format("YYYY-MM-DDTHH:mm:ss")
                            )
                        );
                    });
                }
                this.primengTableHelper.records =
                    result.lstWhatsAppCampaignModel;
                    this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
            });

    }

    refreshData(type: number) {
        this.primengTableHelper.showLoadingIndicator();
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppCampaign(
                this.first,
                this.rows,
                this.appSession.tenantId,
                type
            )
            .subscribe((result) => {
                if (this.isArabic) {
                    result.lstWhatsAppCampaignModel.forEach((element) => {
                        element.sentTime = moment(
                            this.convertHijriToGregorian(
                                moment(element.sentTime)
                                    .locale("en")
                                    .format("YYYY-MM-DDTHH:mm:ss")
                            )
                        );
                    });
                }
                this.primengTableHelper.records =
                    result.lstWhatsAppCampaignModel;

                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    switchHeaders(tabNumber: any) {
        debugger;
        if(tabNumber.index == 0){
            this.refreshData(1)
            this.type = 1
        }
        else {
            this.refreshData(2)
            this.type = 2
        }
    }
}
