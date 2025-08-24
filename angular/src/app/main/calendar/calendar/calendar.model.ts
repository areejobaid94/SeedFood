export class EventRef {
    id!: number;
    bookingNumber!: number;
    customerName!: string | undefined;
    phoneNumber!: string | undefined;
    bookingDateTime!: moment.Moment;
    bookingDateTimeString!: string | undefined;
    bookingDate!: string | undefined;
    bookingTime!: string | undefined;
    contactBookingTime!: string | undefined;
    bookingStatus!: BookingStatusEnum;
    bookingType!: BookingTypeEnum;
    statusId!: number;
    bookingTypeId!: number;
    areaId!: number | undefined;
    areaName!: string | undefined;
    tenantId!: number;
    createdBy!: number;
    createdOn!: moment.Moment;
    contactId!: number;
    templateId!: number;
    languageId!: number;
    customerId!: string | undefined;
    deletionReason!: string | undefined;
    note!: string | undefined;
    isNew!: boolean;
    userName!: string | undefined;
}

export enum BookingStatusEnum {
    Pending = 1,
    Confirmed = 2,
    Booked = 3,
    Canceled = 4,
    Deleted = 5,
}

export enum BookingTemplateCaptionEnum {
    Confirmation = 1253,
    Reminder = 1254,
    Delete = 1256,
}

export enum BookingTypeEnum {
    Manual = 1,
    WhatsApp = 2,
}

export class   Branches {
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
    checked : boolean;
}

export class   Users {
    name!: string | undefined;
    surname!: string | undefined;
    userName!: string | undefined;
    emailAddress!: string | undefined;
    phoneNumber!: string | undefined;
    profilePictureId!: string | undefined;
    isEmailConfirmed!: boolean;
    roles!: UserListRoleDto[] | undefined;
    isActive!: boolean;
    creationTime!: moment.Moment;
    fullName!: string | undefined;
    id!: number;
    checked : boolean;
}

export interface UserListRoleDto {
    roleId: number;
    roleName: string | undefined;
}

