import { Component, Injector, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { WhatsAppCampaignStatusEnum, WhatsAppConversationSessionServiceProxy, WhatsAppScheduledCampaign } from "@shared/service-proxies/service-proxies";

import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import { AddMessageConversationComponent } from "./add-message-conversation.component";
import { ViewMessageHistoryComponent } from "./view-message-history.component";
import * as rtlDetect from 'rtl-detect';
import { VideoModelComponent } from "../videoComponent/video-model.component";
import 'moment-hijri'; // Hijri extension for moment.js
import moment from "moment";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
const { toGregorian } = require('hijri-converter');
@Component({
    selector: "app-message-conversation",
    templateUrl: "./message-conversation.component.html",
    styleUrls: ["./message-conversation.component.css"],
})
export class MessageConversationComponent extends AppComponentBase {
    theme: string;
    messageStatus : boolean = true
    scheduledMessage : WhatsAppScheduledCampaign = new WhatsAppScheduledCampaign(); 
    statusEnum : WhatsAppCampaignStatusEnum ;
    isArabic = false;
    videoLink =  'https://www.youtube.com/embed/sEu16MeDkg4'
    constructor(
        injector: Injector,
        private _WhatsAppConversationSessionServiceProxy: WhatsAppConversationSessionServiceProxy,
        public darkModeService: DarkModeService) {
        super(injector);
    }
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("AddMessageConversation", { static: true })  AddMessageConversation: AddMessageConversationComponent;
    @ViewChild("ViewMessageHistory", { static: true })  ViewMessageHistory: ViewMessageHistoryComponent;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    @ViewChild("viewVideo", { static: true })
    viewVideo: VideoModelComponent;
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);

    }

    getMessageConversation(event?: LazyLoadEvent) {
        this.primengTableHelper.showLoadingIndicator();
        this._WhatsAppConversationSessionServiceProxy.getFreeMessage(
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            )
            .subscribe((result) => {
                if(this.isArabic){
                    result.lstWhatsAppFreeMessageModel.forEach(element => {
                        if(element.sentTime != null || element.sentTime != undefined) {
                            element.sentTime = moment(this.convertHijriToGregorian(moment(element.sentTime).locale('en').format('YYYY-MM-DDTHH:mm:ss')));
                        }
                    });
                }
                this.primengTableHelper.records = result.lstWhatsAppFreeMessageModel;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }
    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split('T');
        const [year, month, day] = hijriDateString.split('-').map(Number);
        const gregorianDate = toGregorian(year, month, day);
    
        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, '0'),
            String(gregorianDate.gd).padStart(2, '0'),
        ].join('-');
    
        return formattedDate + 'T' + time;
    }
    

    getMessageId(campaignId){
        this._WhatsAppConversationSessionServiceProxy.scheduleValidation(campaignId)
        .subscribe((result) => {
            if (result) {
                this.scheduledMessage.campaignId = campaignId;
            } else {
                this.message.error("", this.l("youAlreadyHaveMessageActive"));
            }
        });
    }
    
    DeleteMessage(id: number, event?: LazyLoadEvent) {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._WhatsAppConversationSessionServiceProxy
                    .deleteFreeMessage(id)
                    .subscribe((res) => {
                        if(res){
                        this.notify.success(this.l("successfullyDeleted"));
                        this.getMessageConversation(event);
                        }else{
                            this.notify.error('deleteFailed');

                        }
                     
                    });
            }
        });
    }
    DeactivateMessage(campaignId, event?: LazyLoadEvent){
        this.message.confirm("", this.l("areYouSureToDeactivate"), (isConfirmed) => {
            if (isConfirmed) {
                this._WhatsAppConversationSessionServiceProxy
                    .updateActivationCampaign(campaignId)
                    .subscribe((res) => {
                        this.notify.success(this.l("successfullyDeleted"));
                        this.getMessageConversation(event);
                    });
            }
        });
    }

}
