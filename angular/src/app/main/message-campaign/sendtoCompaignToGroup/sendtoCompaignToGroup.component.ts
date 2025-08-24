import { DatePipe } from "@angular/common";
import { Component, Injector, Input, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    AssetServiceProxy,
    AreasServiceProxy,
    GroupServiceProxy,
    WhatsAppMessageTemplateServiceProxy,
    MembersDto,
    GroupDtoModel,
    MessageTemplateModel,
    WhatsAppComponentModel,
    TemplateVariables,
    ListContactToCampin,
    SendCampinStatesModel,
    CampinToQueueDto,
    TemplateVariablles,
} from "@shared/service-proxies/service-proxies";
import { PermissionCheckerService } from "abp-ng2-module";
import { MessageCampaignService } from "../message-campaign.service";
import { LazyLoadEvent } from "primeng/api";
import {
    debounceTime,
    distinctUntilChanged,
    finalize,
    switchMap,
} from "rxjs/operators";
import { Subject } from "rxjs";

export interface MemberDtoEdited {
    id?: number;
    phoneNumber?: string;
    displayName?: string;
    isFailed?: boolean;
    variables: {
        VarOne?: string;
        VarTwo?: string;
        VarThree?: string;
        VarFour?: string;
        VarFive?: string;
    };
}
@Component({
    selector: "app-sendtoCompaignToGroup",
    templateUrl: "./sendtoCompaignToGroup.component.html",
    styleUrls: ["./sendtoCompaignToGroup.component.css"],
})
export class SendtoCompaignToGroupComponent
    extends AppComponentBase
    implements OnInit
{
    saving = false;
    dailyLimit: number = 0;
    countallowedperday: number = 0;
    VariableCount: number = 0;
    groupById!: CampinToQueueDto ;
    groups: GroupDtoModel[] = [];
    templateId: any = 0;
    loading = false;
    currentPageNumber: number = 0;
    currentPageSize: number = 20;
    searchUser: string = null;
    customerApiInput$ = new Subject<string>();
    customerApiLoading = false;
    selectedContact: GroupDtoModel;
    listOfVariable: string[] = null;
    imageFlag: boolean = false;
    loadingTable: boolean = false;
    videoFlag: boolean = false;
    documentFlag: boolean = false;
    displayText: string = "";
    isSchedule: boolean = false;
    isScheduleBtn: boolean = false;
    dynamicTextParts: {
        type: "text" | "input";
        content: string;
        index?: number;
    }[] = [];

    handleIsSchedule(newValue: Event) {
        this.minDate = new Date();
        this.minDate.setMinutes(this.minDate.getMinutes() + 2);

        this.dateAndTime = new Date();
        this.dateAndTime.setMinutes(this.dateAndTime.getMinutes() + 2);
        this.isSchedule = !this.isSchedule;
    }

    Component: WhatsAppComponentModel[] = [new WhatsAppComponentModel()];
    ComponentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButton: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButtonText: any;

    inputs: { value: string }[] = [];

    templateById: MessageTemplateModel = new MessageTemplateModel();

    listofvar: any[] = [];
    updateVariablesFlag: boolean = false;
    updatebtn: boolean = false;
    isVariablesValidFlag: boolean = false;
    groupID: number;
    selectAll: boolean = false;
    dateAndTime: Date = new Date();
    minDate: Date = new Date();
    customers: ListContactToCampin[] = [];

    clonedContacts: { [s: string]: ListContactToCampin } = {};
    isSubmitted: boolean = false;
    totalRecords: number = 0;
    confirm: string = "";
    inputCountFromBackend: number = 0;
    totalCount: number = 0;
    selectedCustomers: ListContactToCampin[];

    @Input()
    campaign: any;

    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        private router: Router,
        private datePipe: DatePipe,
        public darkModeService: DarkModeService,
        private groupService: GroupServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.getCampaignDailyLimit();
        this.getGroupAll();
        this.getTemplateById();
        this.updateVariablesFlag = false;
        this.isVariablesValidFlag = false;
        this.updatebtn = false;
        this.customers = [];
        this.minDate = new Date();
        this.minDate.setMinutes(this.minDate.getMinutes() + 2);
        this.dateAndTime = new Date();
        this.templateId = this.campaign.templateId;
    }

    trackByFn(item: any) {
        return item.contactId;
    }

    onKeyPress(event: KeyboardEvent) {
        const forbiddenKey = ["$"];
        const keyPressed = event.key;
        if (forbiddenKey.includes(keyPressed)) {
            event.preventDefault();
        }
    }

    updateVariables() {
        this.updateVariablesFlag = true;
        this.updatebtn = true;

        if (this.VariableCount > 0) {
            for (const item of this.inputs) {
                if (item.value.length === 0) {
                    this.updateVariablesFlag = false;
                    this.isVariablesValidFlag = false;
                    return;
                }
            }
            for (let i = 0; i < this.customers.length; i++) {
                this.customers[i].templateVariables = new TemplateVariablles();

                for (let j = 0; j < this.VariableCount; j++) {
                    switch (j) {
                        case 0:
                            this.customers[i].templateVariables["varOne"] =
                                this.inputs[j].value;
                            break;
                        case 1:
                            this.customers[i].templateVariables["varTwo"] =
                                this.inputs[j].value;
                            break;
                        case 2:
                            this.customers[i].templateVariables["varThree"] =
                                this.inputs[j].value;
                            break;
                        case 3:
                            this.customers[i].templateVariables["varFour"] =
                                this.inputs[j].value;
                            break;
                        case 4:
                            this.customers[i].templateVariables["varFive"] =
                                this.inputs[j].value;
                            break;

                        default:
                            break;
                    }
                }
            }
            this.updateVariablesFlag = false;
            this.isVariablesValidFlag = true;
        }
    }
    updateText() {
        this.displayText = "";
        // Replace placeholders with input values, or revert to placeholders if input is empty
        this.dynamicTextParts.forEach((part) => {
            if (part.type === "input") {
                part.content =
                    this.inputs[part.index].value || `{{${part.index + 1}}}`;
            }
        });

        this.displayText = this.dynamicTextParts
            .map((item) => item.content)
            .join("");
        this.ComponentBody.text = this.displayText;
    }
    onSelectionChange(value = []) {
        this.selectAll = value.length === this.totalRecords;
        this.selectedCustomers = value;
    }

    onRowEditInit(contact: ListContactToCampin) {
        // this.clonedContacts[contact.id] = null
        // this.clonedContacts[contact.id] = { ...contact };
    }

    onRowEditSave(contact: ListContactToCampin) {
        delete this.clonedContacts[contact.id];
        this.notify.success("success");
    }

    onRowEditCancel(contact: ListContactToCampin, index: number) {
        this.customers[index] = this.clonedContacts[contact.id];
        delete this.customers[contact.id];
    }
    

    isValidText(text) {
        // Regular expression to match at least three words or a space followed by two words
        const regex = /^(?=(?:\S*\s\S*\s\S*\s*)|$)(?!\s*$).*$/;

        const trimmedString = text?.replace(/\s+$/, "");

        return regex.test(trimmedString);
    }

    sendCampaign(flag: boolean) {
        this.confirm =
            "Are You Sure To Send Ads To " +
            this.selectedCustomers.length +
            " Contacts ?";

        this.message.confirm("", this.l(this.confirm), (isConfirmed) => {
            if (isConfirmed) {
                const campinToQueueDto = new CampinToQueueDto();
                campinToQueueDto.campaignId = this.campaign.id;
                campinToQueueDto.templateId = this.campaign.templateId;
                campinToQueueDto.campaignName = this.campaign.title;
                campinToQueueDto.templateName = this.templateById.name;
                campinToQueueDto.isExternal = false;
                campinToQueueDto.totalCount = this.selectedCustomers.length;
                campinToQueueDto.totalOptOut = 0;
                campinToQueueDto.contacts = this.selectedCustomers;
                campinToQueueDto.templateVariables = null;

                this.isSubmitted = true;
                this.saving = true;
                if (flag) {
                    this.saving = true;
                    this._whatsAppMessageTemplateServiceProxy
                        .sendCampaignNew(null, campinToQueueDto)
                        .subscribe((result: SendCampinStatesModel) => {
                            if (result.status) {
                                this.isSubmitted = false;
                                this.saving = false;
                                this.notify.success(
                                    this.l("Successfully Sent")
                                );

                                this.router.navigate([
                                    "/app/main/messageCampaign",
                                ]);
                                this.message.success("", result.message);
                            } else {
                                this.isSubmitted = false;
                                this.saving = false;
                                this.notify.error(result.message);
                            }
                        });
                } else {
                    let sendTime: string;
                    sendTime = this.datePipe
                        .transform(this.dateAndTime, "yyyy-MM-dd HH:mm")
                        .toString();
                    this._whatsAppMessageTemplateServiceProxy
                        .sendCampaignNew(sendTime, campinToQueueDto)
                        .subscribe(
                            (result: SendCampinStatesModel) => {
                                if (result.status) {
                                    this.notify.success(result.message);
                                    this.isSubmitted = false;
                                    this.saving = false;

                                    this.router.navigate([
                                        "/app/main/messageCampaign",
                                    ]);
                                } else {
                                    this.notify.error(result.message);
                                    this.isSubmitted = false;
                                    this.saving = false;
                                }
                            },
                            (error: any) => {
                                if (error) {
                                    this.isSubmitted = false;
                                    this.saving = false;
                                }
                            }
                        );
                }
            }
        });
    }

    // handleSend(Schedule: boolean = false) {
    //   this.minDate = new Date();
    //   this.isSubmitted = true;
    //   let templateVariables: TemplateVariables;
    //   let sendTime: string;
    //   this.isScheduleBtn = null;
    //   this.isScheduleBtn = Schedule ? true : false;

    //   if (!this.templateById) {
    //     return;
    //   }

    //   if(this.dailyLimit<this.selectedCustomers.length){
    //     this.message.error(this.l("groupMoreThanDailyLimitError") + ':' + this.dailyLimit , this.l("note"));
    //     return;
    //   }

    //   if (Schedule) {
    //     if (this.dateAndTime < new Date()) {
    //       this.dateAndTime = new Date();
    //       this.dateAndTime.setMinutes(this.dateAndTime.getMinutes() + 2);
    //     }

    //     sendTime = this.datePipe
    //       .transform(this.dateAndTime, "yyyy-MM-dd HH:mm")
    //       .toString();
    //   } else {
    //     sendTime = null;
    //   }

    //   // this._whatsAppMessageTemplateServiceProxy.sendCampignFromGroup()

    //   // contactName!: string | undefined;
    //   // phoneNumber!: string | undefined;
    //   // templateVariables!: string | undefined;

    //   let listContact: ListContactToCampin[] = [];
    //   for (let index = 0; index < this.selectedCustomers.length; index++) {
    //     let contact = new ListContactToCampin();

    //     contact.contactName = this.selectedCustomers[index].displayName,
    //       contact.phoneNumber = this.selectedCustomers[index].phoneNumber,
    //       contact.templateVariables =JSON.stringify(this.selectedCustomers[index].variables),

    //       listContact.push(contact)

    //   }

    //   let sendCampignFromGroupDto = new SendCampignFromGroupDto({
    //     campaignId: this.campaign.id,
    //     groupId: this.groupID,
    //     templateId: this.campaign.templateId,
    //     language: this.campaign.language,
    //     sendTime,
    //     isExternal: true,
    //     campaignStatus: 0,
    //     listContact

    //   });

    //   console.log(sendCampignFromGroupDto)

    //   //..

    //       this.saving  = true;
    //   this.teamInbox.sendCampign(teamInboxDto).subscribe(
    //       (result) => {

    //           if (result?.state === -1)
    //               this.notify.error(this.l("ErrorFromServer"));
    //           if (result?.state === 1)
    //               this.notify.error(this.l("notHaveEnoughFunds"));
    //           if (result?.state === 2)
    //               this.notify.success(this.l("sentSuccefully"));
    //           if (result?.state === 3)
    //               this.notify.warn(this.l("haveActiveCamp"));
    //           if (result?.state === 4)
    //               this.notify.error(this.l("notValidDate"));
    //           if (result?.state === 5)
    //           this.notify.error(this.l("reachlimit"));
    //           if (result?.state === 6)
    //           this.notify.warn(this.l("invalidTenant"));
    //           if (result?.state === 7)
    //               this.notify.error(this.l("contactAlreadyexits"));
    //           if (result?.state === 8)
    //           this.notify.error(this.l("invalidFormat"));

    //           this.saving = false;
    //           this.templateById = null;
    //           this.selectedContact = null;
    //           this.createdOrGotContact = null;
    //           this.isSubmitted = false;
    //           this.minDate = new Date();
    //           this.dateAndTime = new Date();
    //       },
    //       (err) => {
    //           this.notify.error(this.l("ErrorFromServer"));
    //           this.saving = false;
    //           this.templateById = null;
    //           this.selectedContact = null;
    //           this.createdOrGotContact = null;
    //           this.isSubmitted = false;
    //           this.minDate = new Date();
    //           this.dateAndTime = new Date();
    //           this.close();
    //       }
    //   );
    // }

    parseDynamicText(
        text: string
    ): { type: "text" | "input"; content: string; index?: number }[] {
        let newText = "";
        const parts: {
            type: "text" | "input";
            content: string;
            index?: number;
        }[] = [];
        let currentIndex = 0;

        text.replace(/{{(\d+)}}/g, (match, index, offset) => {
            if (offset > currentIndex) {
                parts.push({
                    type: "text",
                    content: text.substring(currentIndex, offset),
                });
            }

            const inputIndex = parseInt(index, 10) - 1;
            parts.push({ type: "input", content: match, index: inputIndex });

            currentIndex = offset + match.length;
            return match;
        });

        if (currentIndex < text.length) {
            parts.push({ type: "text", content: text.substring(currentIndex) });
        }
        for (let index = 0; index < parts.length; index++) {
            if (parts[index].type === "input") {
                if (!this.isValidText(parts[index + 1]?.content)) {
                    break;
                }
            }
        }
        return parts;
    }

    getTemplateById() {
        this.isSubmitted = false;
        this.displayText = "";
        this.dynamicTextParts = [];
        this.VariableCount = 0;
        this.listOfVariable = null;
        this.ComponentBody = null;
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateById(+this.campaign.templateId)
            .subscribe((result) => {
                this.templateById = result;
                if (this.templateById) {
                    this.VariableCount = this.templateById.variableCount;
                    this.listofvar = Array.from(
                        { length: this.templateById.variableCount },
                        (_, index) => {
                            switch (index) {
                                case 0:
                                    return "varOne";
                                case 1:
                                    return "varTwo";
                                case 2:
                                    return "varThree";
                                case 3:
                                    return "varFour";
                                case 4:
                                    return "varFive";
                                default:
                                    return ""; // Handle default case if necessary
                            }
                        }
                    );
                    this.inputCountFromBackend =
                        this.templateById.variableCount;

                    if (this.templateById.mediaType == "image") {
                        this.imageFlag = true;
                    } else {
                        this.imageFlag = false;
                    }
                    if (this.templateById.mediaType == "video") {
                        this.videoFlag = true;
                    } else {
                        this.videoFlag = false;
                    }
                    if (this.templateById.mediaType == "document") {
                        this.documentFlag = true;
                    } else {
                        this.documentFlag = false;
                    }
                    this.Component = this.templateById.components;
                    this.Component.forEach((e) => {
                        if (e.type == "HEADER") {
                            this.ComponentHeader = e;
                        }
                        if (e.type == "BODY") {
                            this.ComponentBody = e;
                        }
                        if (e.type == "FOOTER") {
                            this.ComponentFooter = e;
                        }
                        if (e.type == "BUTTONS") {
                            this.ComponentButton = e;
                        }
                    });
                    if (this.ComponentBody) {
                        this.listOfVariable =
                            this.ComponentBody.example?.body_text[0];
                        this.displayText = this.ComponentBody.text;
                        this.inputs = Array.from(
                            { length: this.inputCountFromBackend },
                            () => ({ value: "" })
                        );
                        this.dynamicTextParts = this.parseDynamicText(
                            this.displayText
                        );
                    }
                }
            });
    }

    handleSelectChange(selectedItmem: any) {
        this.groupID = selectedItmem.id;
        this.getGroupById();
    }

    getGroupById(event?: LazyLoadEvent) {
        if (!this.groupID) return;

        this.loadingTable = true;
        this.isVariablesValidFlag = false;
        this._whatsAppMessageTemplateServiceProxy
            .groupGetByIdForCampign(this.groupID)
            .subscribe(
                (result: CampinToQueueDto) => {
                    this.groupById = result;
                    this.loadingTable = false;
                    this.totalRecords = result.contacts.length;
                    this.customers = result.contacts;

                    this.selectedCustomers = this.customers;
                    window.scrollTo(0, document.body.scrollHeight);
                    if (this.dailyLimit < this.customers.length) {
                        this.message.warn(
                            this.l("groupMoreThanDailyLimit") +
                                ":" +
                                this.dailyLimit,
                            this.l("note")
                        );
                    }
                },
                (error: any) => {
                    if (error) {
                        this.loadingTable = false;
                        this.notify.error(error.error.error.message);
                    }
                }
            );
    }

    loadcustomerApi() {
        this.customerApiInput$
            .pipe(
                debounceTime(800),
                distinctUntilChanged(),
                switchMap((input: string) => {
                    this.searchUser = input?.length == 0 ? null : input;
                    this.customerApiLoading = true;
                    return this.groupService.groupGetAll(input, 0, 10);
                })
            )
            .subscribe(({ groupDtoModel }) => {
                this.groups = groupDtoModel;
                this.customerApiLoading = false;
            });
    }

    onScroll() {
        const nextPageNumber = this.currentPageNumber + 1;
        this.loadMoreItems(nextPageNumber * 10, 10);
        this.currentPageNumber = nextPageNumber;
        this.currentPageSize = 20;
    }

    loadMoreItems(pageNumber: number = 0, pageSize: number = 20) {
        if (!this.loading) {
            this.loading = true;
            this.groupService
                .groupGetAll(null, pageNumber, pageSize)
                .subscribe({
                    next: ({ groupDtoModel }) => {
                        this.groups = [...this.groups, ...groupDtoModel];
                        this.loading = false;
                    },
                    error: (err) => {
                        this.loading = false;
                    },
                });
        }
    }

    getGroupAll() {
        

        this.groupService
            .groupGetAll("", 0, 10)
            .subscribe(({ groupDtoModel }) => {
                this.groups = groupDtoModel;

            });
    }

    getCampaignDailyLimit() {
        this._whatsAppMessageTemplateServiceProxy
            .getDailylimitCount()
            .subscribe((resultCount) => {
                this.dailyLimit = resultCount.dailyLimit;
                this.countallowedperday = resultCount.biDailyLimit;
            });
    }
}
