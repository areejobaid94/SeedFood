import { CustomerChat, FacebookUserInfoModel, InstagramUserInfoModel } from "@shared/service-proxies/service-proxies";
import { ChannelMessage } from "./channelMessage";
import { CustomerChatStatus, CustomerStatus } from "./customer-status.enum";

export class Channel {
    id: number;
    agentId: number;
    userId: string;
    avatarUrl: string;
    displayName: string;
    type: string;
    microsoftBotId: string;
    sunshineConversationId: string;
    sunshineAppID: string;
    createDate: any;
    modifyDate?: Date;
    isSelected: boolean;
    phoneNumber: string;
    nickName: string = "";
    lastMessageText: string;
    lastMessageData: any;
    isLockedByAgent: boolean;
    lockedByAgentName: string;
    customerStatus: CustomerStatus;
    customerChatStatus: CustomerChatStatus;
    isConversationExpired: boolean;
    isBlock:boolean;
    searchId:number;
    isNew: boolean;
    isOpen: boolean;
    website: string;
    emailAddress: string;
    description: string;
    notificationsText: string;
    unreadMessagesCount: number;
    isBlockCustomer: boolean;
    lastConversationStartDateTime: any;
    isNewContact:boolean;
    customerChat:ChannelMessage;
    isBotChat :boolean;
    isBotCloseChat: boolean;
    contactID : string;
    isComplaint : boolean;
    groupId: number;
    groupName: string;
    isSupport : boolean;
    
    IsSelectedPage: boolean;
    tenantId:number;
    color: string;

    expiration_timestamp:number;
    creation_timestamp:number;
    conversationsCount:number;
    fullCretaedDate : Date;
    customerOPT : number;
    listCustomerChat : CustomerChat[]

    channel: string;
    gender: string;

    instagramUserInfoModel:InstagramUserInfoModel;
 facebookUserInfoModel:FacebookUserInfoModel;
    isNote:boolean;

    numberNote:number;

}
