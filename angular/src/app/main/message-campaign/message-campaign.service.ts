import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { AppConsts } from "@shared/AppConsts";
import { MessageType } from "../teamInbox/messageType-enum";
import {
    CampinToQueueDto,
    WhatsAppHeaderUrl,
} from "@shared/service-proxies/service-proxies";

@Injectable({
    providedIn: "root",
})
export class MessageCampaignService {
    private http: HttpClient;

    constructor(@Inject(HttpClient) http: HttpClient) {
        this.http = http;
    }
    
    whatsAppMediaResult: WhatsAppHeaderUrl = new WhatsAppHeaderUrl();
    UploadExcelFile(data, campaignId,templateId): Observable<any> {
        return this.http.post<any>(
            AppConsts.remoteServiceBaseUrl +"/api/services/app/WhatsAppMessageTemplate/ReadFromExcel?campaignId=" +campaignId+"&templateId="+templateId,data
        );
    }

    UploadExcelFileNew(data, campaignId,templateId): Observable<any> {
        return this.http.post<any>(
            AppConsts.remoteServiceBaseUrl +"/api/services/app/WhatsAppMessageTemplate/ReadFromExcelNew?campaignId=" +campaignId+"&templateId="+templateId,data
        );
    }
}
