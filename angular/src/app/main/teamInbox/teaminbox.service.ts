import {Inject, Injectable, DEFAULT_CURRENCY_CODE} from '@angular/core';
import {Observable} from 'rxjs';
import {ChannelsMessage} from './channelsMessage';
import {Channels} from './channels';
import {HttpClient, HttpParams} from '@angular/common/http';
import {MessageType} from './messageType-enum';
import {CustomerListFilter} from './customer-list-filter.model';
import {UpdateCustomerModel} from './customer-update.model';
import {AppConsts} from '@shared/AppConsts';
import { CustomerChat } from '@shared/service-proxies/service-proxies';

@Injectable({
    providedIn: 'root'
})
export class TeamInboxService {
    private http: HttpClient;

    constructor(@Inject(HttpClient) http: HttpClient) {
        this.http = http;
    }


    getCustomer(filter: CustomerListFilter): Observable<Channels[]> {
        return this.http.post<Channels[]>(AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/GetCustomers', filter);
    }


    GetNoteChat(
        userId,
        pageSize,
        pageNumber
    ): Observable<CustomerChat[]> {
        return this.http.get<CustomerChat[]>(
            AppConsts.remoteServiceBaseUrl +
            `/api/TeamInbox/GetNoteChat?userId=${userId}&pageNumber=${pageNumber}&pageSize=${pageSize}`
        );
    }


    getChannelMessage(
        userId,
        pageSize,
        pageNumber
    ): Observable<ChannelsMessage[]> {
        return this.http.get<ChannelsMessage[]>(
            AppConsts.remoteServiceBaseUrl +
            `/api/TeamInbox/GetCustomersChat?userId=${userId}&pageNumber=${pageNumber}&pageSize=${pageSize}`
        );
    }

    // getCustomer(): Observable<MessageTypeSender[]> {
    //     return this.http.get<MessageTypeSender[]>(
    //         environment.teamInboxUrl + "/api/TeamInbox/GetCustomers"
    //     );
    // }

    postMessage(data): Observable<MessageType[]> {
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/PostMessage', data
        );
    }
    postMessageD360(data): Observable<MessageType[]> {
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/PostMessageD360', data
        );
    }


    postAttachment(data): Observable<MessageType[]> {
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/PostAttachment',
            data
        );
    }
    postD360Attachment(data): Observable<MessageType[]> {
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/PostD360Attachment',
            data
        );
    }

    lockCustomer(contactId, agentName, agentId,selectedLiveChatID=0) {
        return this.http.post(
            AppConsts.remoteServiceBaseUrl +
            '/api/TeamInbox/LockedByAgent?contactId=' +
            contactId + '&agentName=' + agentName + '&agentId=' + agentId + '&selectedLiveChatID=' + selectedLiveChatID, null);
    }

    
    updatenote(contactId, agentName, agentId,isNote) {
        return this.http.post(
            AppConsts.remoteServiceBaseUrl +
            '/api/TeamInbox/UpdateNote?contactId=' +
            contactId + '&agentName=' + agentName + '&agentId=' + agentId + '&IsNote=' + isNote, null);
    }

    unlockCustomer(contactId, agentName, agentId) {
        return this.http.post(
            AppConsts.remoteServiceBaseUrl +
            '/api/TeamInbox/UnlockedByAgent?contactId=' +
            contactId + '&agentName=' + agentName + '&agentId=' + agentId, null);
    }

    updateCustomer(model: UpdateCustomerModel) {
        return this.http.post(
            AppConsts.remoteServiceBaseUrl +
            '/api/TeamInbox/UpdateCustomerInfo',
            model
        );
    }

    updateCustomerStatus(userId, IsOpen, lockedByAgentName,selectedLiveChatID=0) {
        return this.http.post(AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/UpdateCustomerStatus?userId=' + userId + '&IsOpen=' + IsOpen + '&lockedByAgentName=' + lockedByAgentName+ '&selectedLiveChatID=' + selectedLiveChatID, null);
    }

    getUsers() {
        return this.http.get(AppConsts.remoteServiceBaseUrl + '/api/services/app/User/GetUsers');
    }

    assignTo(contactId, agentName, agentId) {
        return this.http.post(
            AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/AssignTo?contactId=' +
            contactId + '&agentName=' + agentName + '&agentId=' + agentId, null);
    }

    getLastMessage(contactId) {
        return this.http.post(
            AppConsts.remoteServiceBaseUrl + AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/GetCustomersLastMessage?contactId=' +
            contactId, null);
    }
    checkToken() {
        return this.http.get(
            AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/CheckToken'
           );
    }

    blockContact(contactId, agentId, agentName, isBlock) {
        return this.http.post(
            AppConsts.remoteServiceBaseUrl + '/api/TeamInbox/BlockCustomer?contactId=' +
            contactId + '&agentId=' + agentId + '&agentName=' + agentName + '&isBlock=' + isBlock, null);
    }


    uploadExcelFile(data): Observable<MessageType[]> {
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/services/app/Menus/UploadExcelFile',
            data
        );
    }

    uploadExcelFileT(data): Observable<MessageType[]> {
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/services/app/Tenant/GetFileURLT',
            data
        );
    }

    ctownUploadExcelFile(data,subcatogryName,replece): Observable<MessageType[]> {
                         
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/CTownApi/CtownUploadExcelFile?ItemSubCategoryName='+subcatogryName+ '&replece='+replece,
            data
        );
    }
    UploadExcelFile(data,campaignId): Observable<MessageType[]> {
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/services/app/WhatsAppMessageTemplate/ReadFromExcel?campaignId='+campaignId,data
        );
    }
    uploadImageCtown(data): Observable<MessageType[]> {
                         
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/api/CTownApi/UploadImageCtown',
            data
        );
    }
    

    ItemsUploadExcelFile(data,tenantId): Observable<MessageType[]> {
                         
        return this.http.post<MessageType[]>(
            AppConsts.remoteServiceBaseUrl + '/iItemUploadExcelFileAsync?TenantId='+tenantId,
            data
        );
    }
}
