//import { ReferralMassages } from "@shared/service-proxies/service-proxies";

export class ChannelMessage {
    id:string;
    result: any;
    agentId: string;
    agentName: string;
    createDate: Date;
    itemType: number;
    mediaUrl: string;
    messageId: string;
    sender: number;
    status: number;
    sunshineConversationId: string;
    text: string;
    type: string;
    userId: string;
    unreadMessagesCount: number;
    notificationsText: string;
    isArabic:boolean;

    //referral: ReferralMassages;
    source_url: string;
    source_id: string;
    source_type: string;
    headline: string;
    media_type: string;
    image_url: string;
    video_url: string;
    thumbnail_url: string;

}
