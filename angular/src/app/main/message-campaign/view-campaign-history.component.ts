import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { ThemeHelper } from '@app/shared/layout/themes/ThemeHelper';
import { AppComponentBase } from '@shared/common/app-component-base';
import { WhatsAppCampaignHistoryModel, WhatsAppMessageTemplateServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Router } from '@angular/router';

@Component({
  selector: 'ViewCampaignHistory',
  templateUrl: './view-campaign-history.component.html',
  styleUrls: ['./view-campaign-history.component.css']
})
export class ViewCampaignHistoryComponent extends AppComponentBase {

  constructor(
    injector: Injector,
    private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
    http: HttpClient,
    public darkModeService: DarkModeService,
    private router: Router,
    ){
    super(injector);
  }
@ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
@Output() modalSave: EventEmitter<any> = new EventEmitter<any>();


  ngOnInit(): void {
    this.theme = ThemeHelper.getTheme();

  }
  campaignHistory: WhatsAppCampaignHistoryModel[] = [new WhatsAppCampaignHistoryModel()];
  totalCampaignHistory : number =0;

  theme: string;
  active = false;
  saving = false;

  show(messageId:number){
    this.campaignHistory=[new WhatsAppCampaignHistoryModel()]

    this._whatsAppMessageTemplateServiceProxy.getWhatsAppCampaignHistory(messageId).subscribe(result => {
      this.totalCampaignHistory = result.lstwhatsAppCampaignHistoryModels.length
      if (result.lstwhatsAppCampaignHistoryModels.length > 0 ) {
        this.campaignHistory = result.lstwhatsAppCampaignHistoryModels;
      }
      this.active = true;
      this.modal.show();   
    });
  }




  close(): void {
    this.active = false;
    this.modal.hide();
  }

  generateReport(campaignId,templateId){
    this.router.navigate(['/app/main/externalContacts'],{queryParams: {campaignId,templateId}});
  }

}
