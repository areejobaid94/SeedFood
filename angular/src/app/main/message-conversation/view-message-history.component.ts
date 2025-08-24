import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { ThemeHelper } from '@app/shared/layout/themes/ThemeHelper';
import { AppComponentBase } from '@shared/common/app-component-base';
import { WhatsAppCampaignHistoryModel, WhatsAppMessageTemplateServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
const { toGregorian } = require('hijri-converter');
import * as rtlDetect from 'rtl-detect';

@Component({
  selector: 'ViewMessageHistory',
  templateUrl: './view-message-history.component.html',
  styleUrls: ['./view-message-history.component.css']
})
export class ViewMessageHistoryComponent extends AppComponentBase {

  constructor(
    injector: Injector,
    private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
    http: HttpClient,
    public darkModeService: DarkModeService,
    ){
    super(injector);
  }
@ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
@Output() modalSave: EventEmitter<any> = new EventEmitter<any>();


  ngOnInit(): void {
    this.theme = ThemeHelper.getTheme();
    this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);

  }
  campaignHistory: WhatsAppCampaignHistoryModel[] = [new WhatsAppCampaignHistoryModel()];
  totalCampaignHistory : number =0;

  theme: string;
  active = false;
  saving = false;
  isArabic = false;

  show(messageId:number){
    this.campaignHistory=[new WhatsAppCampaignHistoryModel()]

      this._whatsAppMessageTemplateServiceProxy.getWhatsAppCampaignHistory(messageId).subscribe(result => {
      this.totalCampaignHistory = result.lstwhatsAppCampaignHistoryModels.length;
      if (result.lstwhatsAppCampaignHistoryModels.length > 0 ) {
        if(this.isArabic){
          result.lstwhatsAppCampaignHistoryModels[0].sentTime = moment(this.convertHijriToGregorian(moment(result.lstwhatsAppCampaignHistoryModels[0].sentTime).locale('en').format('YYYY-MM-DDTHH:mm:ss')));      
        }
        this.campaignHistory = result.lstwhatsAppCampaignHistoryModels;
      }
      this.active = true;
      this.modal.show();   
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




  close(): void {
    this.active = false;
    this.modal.hide();
  }


}
