// export class DialogModel {
//     id ?: number ;
//     title: string;
//     footerText? : string;
//     message : string;
//     parentIndex?:number;
//     childIndex?:number ;
//     isRoot : boolean ;
//     type : string;
//     parameter? : Parameter[];
//     urlImgae ?: string ;
//     dtoContent? : Content[];
// }

import moment from "moment";

// export class Parameter {
//     key : number ;
//     value: string;
// }

// export class Content {
//     parentIndex : number ;
//     childIndex: number;
//     value: string;
//     key: number;
// }

export class DialogModel {
    id: number
    isPublished: boolean
    tenantId: number
    createdDate?: moment.Moment
    createdUserId: number
    createdUserName: string
    modifiedDate?: moment.Moment
    modifiedUserId?: number
    modifiedUserName?: string
    flowName?: string ;
    statusId?: number;
    getBotFlowForViewDto?: GetBotFlowForView[]
  }
  export class GetBotFlowForView {
    id: number
    captionAr?: string
    captionEn?: string
    footerTextAr?: string
    footerTextEn?: string
    childIndex: number
    parentIndex: number
    isRoot : boolean
    top?: number
    bottom?: number
    left?: number
    rigth?: number
    type?: string
    parameter?: Parameter[]
    urlImage?: any
    content?: ContentInfo
    footerText?: string
  }

export class Parameter {
    key: string;
    value: string;
}

export class ContentInfo {
    txt?: string;
    dtoContent: DtoContent[];

    constructor(dtoContent: DtoContent[] = [new DtoContent], text: string | undefined = undefined) {
        this.dtoContent =dtoContent;
        if (text) {
          this.txt = text;
        }
      }
    }
export class DtoContent {
    childIndex: number;
    parentIndex: number;
    valueAr: string;
    valueEn: string;
    key: number;
}
export class AreaDto {
    areaName!: string | undefined;
    areaCoordinate!: string | undefined;
    areaNameEnglish!: string | undefined;
    areaCoordinateEnglish!: string | undefined;
    branchID!: string | undefined;
    userId!: number | undefined;
    restaurantsType!: number;
    isAssginToAllUser!: boolean;
    isAvailableBranch!: boolean;
    isRestaurantsTypeAll!: boolean;
    latitude!: number | undefined;
    longitude!: number | undefined;
    settingJson!: string | undefined;
    userIds!: string | undefined;
    url!: string | undefined;
    id!: number;
    checked!:boolean;
}


