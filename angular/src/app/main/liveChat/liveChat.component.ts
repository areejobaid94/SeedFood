import { element } from 'protractor';
import { DarkModeService } from "./../../services/dark-mode.service";
import {
    Component,
    Injector,
    ViewEncapsulation,
    ViewChild,
    QueryList,
    ViewChildren,
    ChangeDetectorRef,
} from "@angular/core";
import {
    LiveChatServiceProxy,
    CustomerLiveChatModel,
    WhatsAppMessageTemplateServiceProxy,
    MessageTemplateModel,
    TemplateMessagesServiceProxy,
    TeamsServiceProxy,
    UserServiceProxy,
    TeamsDtoModel,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import { FileDownloadService } from "@shared/utils/file-download.service";
import * as _ from "lodash";
import { Subscription } from "rxjs";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { SocketioService } from "@app/shared/socketio/socketioservice";
import { finalize } from "rxjs/operators";
import { ViewTicketModalComponent } from "./view-ticket-modal.component";
import { VideoModelComponent } from "../videoComponent/video-model.component";

import { Channel } from "../teamInbox/channel";
import { ChannelMessage } from "../teamInbox/channelMessage";
import { CustomerListFilter } from "../teamInbox/customer-list-filter.model";
import { TeamInboxService } from "../teamInbox/teaminbox.service";
import { NgxSpinnerService } from "ngx-spinner";
import { AssignToModalComponent } from "../teamInbox/assign-to-modal/assign-to-modal.component";
import moment from "moment";
import { Howl } from "howler";
import "moment-hijri"; // Hijri extension for moment.js
import { PermissionCheckerService } from "abp-ng2-module";
import { ViewRequestModalComponent } from "./view-request-modal/view-request-modal.component";
import { Router } from "@angular/router";
import { ListSettingsModalComponent } from "./list-settings-modal/list-settings-modal.component";
import { RoleService } from "@shared/common/session/role.service";
import { AssignUsersModalComponent } from "./assign-users-modal/assign-users-modal.component";
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal/modal.directive';
import { TeaminboxTemplateModalComponent } from '../chat-them12/teaminbox-template-modal/teaminbox-template-modal.component';
import { ChatThem12Component } from '../chat-them12/chat-them12.component';
import { UntypedFormGroup } from '@node_modules/@angular/forms';
import { TicketLimitPopupComponent } from './TicketLimitPopup/ticket-limit-popup.component';
import { ExportToExcelModelComponent } from './Export-To-Excel-Model/Export-To-Excel-Model.component';
const { toGregorian } = require("hijri-converter");

interface TimeDuration {
    days?: number;
    hours?: number;
    minutes: number;
}
@Component({
    templateUrl: "./liveChat.component.html",
    styleUrls: ["liveChat.component.css"],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class LiveChatComponent extends AppComponentBase {
    // Inner Components
    template: MessageTemplateModel[] = [];
    @ViewChild("teamInboxModal", { static: true }) modal: ModalDirective;
  
    // @ViewChild("appteaminboxtemplatemodal2", { static: false })
    // modal2: TemplateModalComponent;

       @ViewChild("appteaminboxtemplatemodal", { static: false })
        modal2: TeaminboxTemplateModalComponent;
    
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    @ViewChildren("rowRef") rowRef: QueryList<any>;
    @ViewChild("viewVideo", { static: true })
    viewVideo: VideoModelComponent;

    @ViewChild("viewRequestModalComponent", { static: true })
    viewRequestModalComponent: ViewRequestModalComponent;

    @ViewChild("viewTicketModal", { static: true })
    viewTicketModal: ViewTicketModalComponent;
    @ViewChild("assignToModal", { static: true })
    assignToModal: AssignToModalComponent;


        @ViewChild("appticketlimitpopup", { static: true })
    ticketLimitPopupComponent: TicketLimitPopupComponent;



    @ViewChild("viewlistsettingsmodal", { static: true })
    listSettings: ListSettingsModalComponent;

    @ViewChild("assignusersmodal", { static: true })
    assignUsers: AssignUsersModalComponent;

    @ViewChild(ChatThem12Component) ChatThem12Component!: ChatThem12Component;

    // @ViewChild('backupAllConversationModal', { static: true }) BackupAllModal: BackupAllConversationModalComponent;   
        @ViewChild('exportToExcelModel', { static: true }) BackupAllModal: ExportToExcelModelComponent;   

    // Vars
    headersList = [
        { name: "ID", isActive: true },
        { name: "type", isActive: true },
        { name: "agent", isActive: true },
        { name: "department", isActive: true },
        { name: "customer", isActive: true },
        { name: "Time", isActive: true },
        { name: "resolution", isActive: true },
        { name: "Status", isActive: true },
        { name: "TimeToOpen", isActive: true },
        { name: "summary", isActive: true },
         {name: "Contact Creation Date", isActive: true },
    ];

    headersListFromStorage;

    first: number = 0;
    rows: number = 20;
    theme: string;
    advancedFiltersAreShown = false;
    btnClicked = false;
    filterName = "";
    restaurantsFilter = "";
    agentLivchatSub: Subscription;
    botLivchatSub: Subscription;
    totalCount = 0;
    totalClosed = 0;
    totalPending = 0;
    totalOpen = 0;
    totalExpired = 0;

    // filter values
    searchBy = "";
    ticketId = "";
    ticketType = "";
    agent = "";
    searchName = "";
    departemntName ="";
    summary ="";
    byteam="";
    teamName="";
    searchByPhone = "";
    dateRange: [Date, Date];
    dateRangeC: [Date, Date];
    status = "";

    totalResolutionHours: number = 0;
    STORAGEDATE = null;
    isArabic = false;
    UsersChannels: Channel[];
    user: Channel;
    UserMessage: ChannelMessage[] = [];
    customerMessageSub: Subscription;
    anotherAgentMessgeSub: Subscription;
    selectedUser: Channel = new Channel();
    templates: any[] = [];
    isUserSelectedChat = false;
    isOnline = false;
    showChatt = false;
    public selectedIndex = null;

    colors = [];
    statusId;
    numberOfFiliter=0;
    filters: {
        selectedCustomerID: string;
        selectedLiveChatID: number;
    } = <any>{};

    selectedLiveChatId = 0;
    disabledAnotherAgent: boolean;
    customersFilter: CustomerListFilter = {
        pageNumber: 0,
        pageSize: 1000,
        searchTerm: null,
    };
    users: any[] = [];
    selectedContactId = "";
    sound: any;
    videoLink = "https://www.youtube.com/embed/_4FwrlsdaRs";

    addFiliterDisabled:boolean=true;
    

popup: TicketLimitPopupComponent;
    startDate:any=null;
    endDate:any=null;
 public selectMultiTeams: TeamsDtoModel[] = [];
    
    startDateC:any=null;
    endDateC:any=null;

    filters2: { searchBy: string; value: number }[] = [
        { searchBy: 'searchBy', value: 0 }  
      ];
    agentToExportToExecel:any=null;
    statusIdToExportToExecel:number=null;
    userIdfromTemplate:string;
    name:string;
    hasSpecificFilter(): boolean {
        return this.filters2.some(
          (filter) =>
            filter.searchBy === 'searchByName' ||
            filter.searchBy === 'departemntName' ||
             filter.searchBy === 'summary' ||
             filter.searchBy === 'byteam' ||
            filter.searchBy === 'searchByPhone' ||
            filter.searchBy === 'searchByTicketId'
        );
      }

      addFilter(numberOfFiliter: number): void {
        if (this.filters2.length <= 10) {
          this.filters2.push({ searchBy: 'searchBy', value: ++this.numberOfFiliter });
          this.addFiliterDisabled=true;
        }
      }
      
        handleSearchBy(event, numberOfFiliter: number): void {
            debugger;
        this.ticketId = (this.ticketId === null || this.ticketId === undefined || this.ticketId === '') ? "" : this.ticketId;
        this.ticketType = (this.ticketType === null || this.ticketType === undefined || this.ticketType === '') ? "" : this.ticketType;
        this.status =  (this.status === null || this.status === undefined || this.status === '') ? "" : this.status;
        this.agent = (this.agent === null || this.agent === undefined || this.agent === '') ? "" : this.agent;
        this.searchName = (this.searchName === null || this.searchName === undefined || this.searchName === '') ? "" : this.searchName;
        this.departemntName = (this.departemntName === null || this.departemntName === undefined || this.departemntName === '') ? "" : this.departemntName;
        this.summary = (this.summary === null || this.summary === undefined || this.summary === '') ? "" : this.summary;
        this.byteam = (this.byteam === null || this.byteam === undefined || this.byteam === '') ? "" : this.byteam;
        this.searchByPhone = (this.searchByPhone === null || this.searchByPhone === undefined || this.searchByPhone === '') ? "" : this.searchByPhone;
        this.dateRange = (this.dateRange === null || this.dateRange === undefined) ? null : this.dateRange;

    this.filters2.forEach((filter) => {
        if (numberOfFiliter === filter.value) {
            switch (filter.searchBy) {
                
                case 'searchByTicketId':
                    this.ticketId = null;
                    break;
                case 'searchByTicketType':
                    this.ticketType = null;
                    break;
                case 'searchByAgent':
                    this.agent = null;
                    break;
                case 'searchByName':
                    this.searchName = null;
                    break;
                case 'departemntName':
                    this.departemntName = null;
                    break;
                     case 'summary':
                    this.summary = null;
                    break;
                    case 'byteam':
                    this.byteam = null;
                    break;
                case 'searchByPhone':
                    this.searchByPhone = null;
                    break;
                case 'searchByDate':
                    this.dateRange = null;
                    break;
                case 'searchByStatus':
                    this.status = null;
                    break;
                default:
                    break;
            }
        }
    });

    }
      // Remove a filter group
      removeFilter(index: number, event?: LazyLoadEvent) {

            debugger;
        this.addFiliterDisabled=false;
        // if(this.filters2.length==1){
        //     return;
        // }
        const filter = this.filters2[index];
        if (!filter.searchBy &&this.filters2.length!=1) {
            this.filters2.splice(index, 1);
            return; 
        }
        if (filter) {
            switch (this.filters2[index].searchBy) {
                case 'searchByTicketId':
                    this.ticketId = '';
                    break;
                case 'searchByTicketType':
                    this.ticketType = '';
                    break;
                case 'searchByAgent':
                    this.agent = "";
                    break;
                case 'searchByName':
                    this.searchName = "";
                    break;
                case 'departemntName':
                    this.departemntName = "";
                    break;
                case 'summary':
                    this.summary = "";
                    break;
                case 'byteam':
                    this.byteam = "";
                    break;
                case 'searchByPhone':
                    this.searchByPhone = "";
                    break;
                case 'searchByStatus':
                    this.status = "";
                    break;
                case 'searchByDate':
                    this.dateRange = null;
                    this.startDate=null;
                    this.endDate=null;
                    break;
                case 'contactCreateDate':
                    this.dateRangeC = null;
                    this.startDateC=null;
                    this.endDateC=null;
                    break;
                default:
                    break;
            }
            --this.numberOfFiliter 
        }
        if(this.filters2.length!=1){
        this.filters2.splice(index, 1);
        }else{
            this.filters2[index].searchBy='searchBy';
        }
        this.getTicketAndRequest(event);
    }
    
    

    pageChange(event) {
        this.first = event.first;
        this.rows = event.rows;
    }

    convertMinutesToTimeDuration(minutes: number): string {
        if (minutes < 0) {
            throw new Error("Input must be a non-negative number of minutes.");
        }

        const hoursInDay = 24;
        const minutesInHour = 60;

        const days = Math.floor(minutes / (hoursInDay * minutesInHour));
        const remainingHours = Math.floor(
            (minutes % (hoursInDay * minutesInHour)) / minutesInHour
        );
        const remainingMinutes = minutes % minutesInHour;
        let resolution = "";

        const result: TimeDuration = {
            minutes: remainingMinutes,
        };

        if (days > 0) {
            result.days = days;
            resolution += days + "D:";
        }

        if (remainingHours > 0) {
            result.hours = remainingHours;
            resolution += remainingHours + "H:";
        }
        resolution += remainingMinutes + "M";

        return resolution;
    }

    predefinedRanges = [
        {
            value: [
                this.setToStartOfDay(new Date()), 
                this.setToEndOfDay(new Date()),
            ],
            label: this.l('Today'),
        },
        {
            value: [
                this.setToStartOfDay(new Date(new Date().setDate(new Date().getDate() - 1))), 
                this.setToEndOfDay(new Date(new Date().setDate(new Date().getDate() - 1))),   
            ],
            label: this.l('Yesterday'),
        },
        {
            value: [
                this.setToStartOfDay(new Date(new Date().setDate(new Date().getDate() - 7))),
                this.setToEndOfDay(new Date()),
            ],
            label: this.l('Last7Days'),
        },
        {
            value: [
                this.setToStartOfDay(new Date(new Date().setMonth(new Date().getMonth() - 1))),
                this.setToEndOfDay(new Date()),
            ],
            label: this.l('LastMonth'),
        },
        {
            value: [
                this.setToStartOfDay(new Date(new Date().setFullYear(new Date().getFullYear() - 1))),
                this.setToEndOfDay(new Date()),
            ],
            label: this.l('LastYear'),
        },
    
    ];
    dateRangePickerOptions = {
        showCustomRangeLabel: false,
        dateInputFormat: "DD/MM/YYYY",
        rangeInputFormat: "DD/MM/YYYY",
        isAnimated: true,
        adaptivePosition: true,
        containerClass: "theme-default",
        selectFromOtherMonth: true,
        ranges: this.predefinedRanges,
        showPreviousMonth: true,
        showWeekNumbers: false,
        useUtc: true,
    };
    dateRangeCPickerOptions = {
        showCustomRangeLabel: false,
        dateInputFormat: "DD/MM/YYYY",
        rangeInputFormat: "DD/MM/YYYY",
        isAnimated: true,
        adaptivePosition: true,
        containerClass: "theme-default",
        selectFromOtherMonth: true,
        ranges: this.predefinedRanges,
        showPreviousMonth: true,
        showWeekNumbers: false,
        useUtc: true,
    };

    constructor(
        injector: Injector,
        private socketioService: SocketioService,
        private _liveChatServiceProxy: LiveChatServiceProxy,
        private _fileDownloadService: FileDownloadService,
        public darkModeService: DarkModeService,
         private _teamsService: TeamsServiceProxy,
        private _userService: UserServiceProxy,
     
        private teamService: TeamInboxService,
     //  private _teamsService: TeamsServiceProxy,               
        private spinner: NgxSpinnerService,
        private router: Router,
        private _permissionCheckerService: PermissionCheckerService,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
    ) {
        super(injector);
    }

    panelStates: { [panelId: string]: boolean } = {};

    openViewTicketModal({
        idLiveChat,
        userId,
        type,
        data,
        rejectedOrConfirmed,
        name
    }: any) {



this._userService.userTicketsOpenUpdate(this.appSession.userId,true )
                .subscribe(
                    (response) => {

 this.userIdfromTemplate=userId;
        this.name=name;
        this.viewRequestModalComponent.close();
        this.viewTicketModal.show(
            idLiveChat,
            userId,
            type,
            data,
            rejectedOrConfirmed
        );


                    },
                    (error: any) => {
                  
                    }
                );

       
    }

    saveToLocalStorage(data: any, name: string) {
        // Convert object to JSON string
        const jsonString = JSON.stringify(data);

        // Save JSON string to local storage
        localStorage.setItem(name, jsonString);
    }

    ChatBtn(CustomerID, LiveChatId,statusId) {
        let agent = null;
        if (this.agent) {
            agent = this.agent.toString();
        }

        let ticketType = null;
        if (this.ticketType) {
            ticketType = this.ticketType.toString();
        }
        let startDate = null;
        let endDate = null;

        if (this.dateRange != undefined) {
            startDate = moment(this.dateRange[0]);
            endDate = moment(this.dateRange[1]);
        }
        let startDateC = null;
        let endDateC = null;

        if (this.dateRangeC != undefined) {
            startDateC = moment(this.dateRangeC[0]);
            endDateC = moment(this.dateRangeC[1]);
        }
        switch (statusId) {
            case 1:
                this.status = "pending"; // Update status to "pending"
                break;
            case 2:
                this.status = "opened"; // Update status to "opened"
                break;
            case 3:
                this.status = "closed"; // Update status to "closed"
                break;
            case 6:
                this.status = "pending"; // Update status to "expired" //expired
                break;
            case 4:
                this.status = "Confirm"; // Update status to "Confirm"
                break;
            case 5:
                this.status = "Reject"; // Update status to "Reject"
                break; 
            case 6:
                this.status = "Assigned"; // Update status to "Assigned"
                break;
            default:
                this.status = null; // Default case if statusId doesn't match any case
                break;
        }
        let statusName=this.status;

        // Now set statusID based on this.status
        let statusID;
        switch (this.status) {
            case "pending":
                statusID = 1;
                this.status=null;
                break;
            case "opened":
                statusID = 2;
                this.status=null;
                break;
            case "closed":
                statusID = 3;
                this.status=null;
                break;
            case "pending": //expired
                statusID = 6;
                this.status=null;
                break;
            case "Confirm":
                statusID = 4;
                this.status=null;
                break;
            case "Reject":
                statusID = 5;
                this.status=null;
                break;
            case "Assigned":
                statusID = 6;
                this.status=null;
                break;
            default:
                statusID = null;
                break;
        }
        this.statusIdToExportToExecel=statusID ;
       // console.log(statusID); // Outputs the assigned statusID
        
        // let statusID =
        //     this.status == "pending"
        //         ? 1
        //         : this.status == "opened"
        //         ? 2
        //         : this.status == "closed"
        //         ? 3
        //         : this.status == "expired"
        //         ? 6
        //         : this.status == "Confirm"
        //         ? 4
        //         : this.status == "Reject"
        //         ? 5
        //         : null;

        const dataObject = {
            id: LiveChatId,
            row: this.dataTable._rows,
            first: this.dataTable._first,
            CustomerID,
            userId: CustomerID,
            LiveChatId,
            searchName: this.searchName,
            departemntName: this.departemntName,
            summary: this.summary,
             byteam: this.byteam,
            ticketType: this.ticketType,
            search: this.searchByPhone,
            agent,
            startDate,
            endDate,
            statusID:statusID,
            status:statusName,
            dateRange: this.dateRange,
            searchBy:this.searchBy,
            filters :this.filters2,
            startDateC,
            endDateC,
            dateRangeC: this.dateRangeC,

        };

        this.saveToLocalStorage(dataObject, "ticketData");

        this.router.navigate(["/app/main/teamInbox/teamInbox12"], {
            queryParams: { CustomerID, LiveChatId },
        });
    }




    openChat(CustomerID, LiveChatId,statusId) {

this._userService.userTicketsOpenUpdate(this.appSession.userId,true )
                .subscribe(
                    (response) => {
                        if(response.isOpen){
    if (this.appSession.tenantId !== 59) {
            let agent = null;
            if (this.agent) {
                agent = this.agent.toString();
            }

            let ticketType = null;
            if (this.ticketType) {
                ticketType = this.ticketType.toString();
            }
            let startDate = null;
            let endDate = null;

            if (this.dateRange != undefined) {
                startDate = moment(this.dateRange[0]);
                endDate = moment(this.dateRange[1]);
            }
            let startDateC = null;
            let endDateC = null;

            if (this.dateRangeC != undefined) {
                startDateC = moment(this.dateRangeC[0]);
                endDateC = moment(this.dateRangeC[1]);
            }  
            switch (statusId) {
                case 1:
                    this.status = "pending"; // Update status to "pending"
                    break;
                case 2:
                    this.status = "opened"; // Update status to "opened"
                    break;
                case 3:
                    this.status = "closed"; // Update status to "closed"
                    break;
                case 6:
                    this.status = "expired"; // Update status to "expired"
                    break;
                case 4:
                    this.status = "Confirm"; // Update status to "Confirm"
                    break;
                case 5:
                    this.status = "Reject"; // Update status to "Reject"
                    break;
                case 6:
                    this.status = "Assigned"; // Update status to "Assigned"
                    break;

                default:
                    this.status = null; // Default case if statusId doesn't match any case
                    break;
            }
        
            // Now set statusID based on this.status
            let statusID;
            let statusName=this.status;

            switch (this.status) {
                case "pending":
                    statusID = 1;
                    this.status=null;
                    break;
                case "opened":
                    statusID = 2;
                    this.status=null;
                    break;
                case "closed":
                    statusID = 3;
                    this.status=null;
                    break;
                case "expired":
                    statusID = 6;
                    this.status=null;
                    break;
                case "Confirm":
                    statusID = 4;
                    this.status=null;
                    break;
                case "Reject":
                    statusID = 5;
                    this.status=null;
                    break;
                case "Assigned":
                    statusID = 6;
                    this.status=null;
                    break;
                default:
                    statusID = null;
                    break;
            }


            const dataObject = {
                id: LiveChatId,
                row: this.dataTable._rows,
                first: this.dataTable._first,
                CustomerID,
                userId: CustomerID,
                LiveChatId,
                searchName: this.searchName,
                departemntName: this.departemntName,
                summary: this.summary,
                byteam: this.byteam,
                ticketType: this.ticketType,
                search: this.searchByPhone,
                agent,
                startDate,
                endDate,
                statusID:statusID,
                status:statusName,
                dateRange: this.dateRange,
                searchBy:this.searchBy, 
                filters :this.filters2,
                startDateC,
                endDateC,
                dateRangeC: this.dateRangeC,

            };
            
            this.saveToLocalStorage(dataObject, "ticketData");

            this.filters.selectedCustomerID = CustomerID;
            this.filters.selectedLiveChatID = LiveChatId;
            this.selectedUser.userId = CustomerID;
            this.primengTableHelper.showLoadingIndicator();
            this.teamService
                .lockCustomer(
                    this.selectedUser.userId,
                    this.appSession.user.userName,
                    this.appSession.user.id,
                    this.filters.selectedLiveChatID
                )
                .subscribe(
                    (response) => {
                        if (response) {
                            // this.selectedUser.isOpen = true;
                            // this.selectedUser.isLockedByAgent = true;
                            // this.showLoaderOnOpenClose = false;
                            this.primengTableHelper.hideLoadingIndicator();

                            this.router.navigate(
                                ["/app/main/teamInbox/teamInbox12"],
                                {
                                    queryParams: { CustomerID, LiveChatId },
                                }
                            );
                        }
                    },
                    (error: any) => {
                        if (error) {
                            this.primengTableHelper.hideLoadingIndicator();
                        }
                    }
                );
        }

                        }else{


 this.ticketLimitPopupComponent.show(response);

                        }



                    },
                    (error: any) => {
                  
                    }
                );

    
    }

    filterTicket() {
        const storedData = localStorage.getItem('ticketData');
    
        if (storedData) {
            const parsedData = JSON.parse(storedData);
            this.filters2=parsedData.filters;
            for (const filter of this.filters2) {
                if (filter.searchBy === 'searchByTicketId') {
                    if (parsedData.LiveChatId) {
                        this.ticketId = parsedData.LiveChatId || '';
                    }
                } else if (filter.searchBy === 'searchByName') {
                    if (parsedData.searchName) {
                        this.searchName = parsedData.searchName || '';
                    }
                }else if (filter.searchBy === 'departemntName') {
                    if (parsedData.departemntName) {
                        this.departemntName = parsedData.departemntName || '';
                    }
                }else if (filter.searchBy === 'summary') {
                    if (parsedData.summary) {
                        this.summary = parsedData.summary || '';
                    }
                }else if (filter.searchBy === 'byteam') {
                    if (parsedData.byteam) {
                        this.byteam = parsedData.byteam || '';
                    }
                }
                else if (filter.searchBy === 'searchByTicketType') {
                    if (parsedData.ticketType) {
                        this.ticketType = parsedData.ticketType || '';
                    }
                } else if (filter.searchBy === 'searchByAgent') {
                    if (parsedData.agent) {
                        this.agent = parsedData.agent || '';
                    }
                } else if (filter.searchBy === 'searchByPhone') {
                    if (parsedData.search) {
                        this.searchByPhone = parsedData.search || '';
                    }
                } else if (filter.searchBy === 'searchByStatus') {
                    if (parsedData.status) {
                        this.status = parsedData.status || '';
                    }
                } else if (filter.searchBy === 'searchByDate') {
                    if (parsedData.dateRange) {
                        this.dateRange = parsedData.dateRange || null;
                    }
                }else if (filter.searchBy === 'contactCreateDate') {
                    if (parsedData.dateRangeC) {
                        this.dateRangeC = parsedData.dateRangeC || null;
                    }
                }
            }
            if(this.filters2.length>0){
                this.addFiliterDisabled=false;
            }
    
            // if (parsedData.searchBy) {
            //     this.searchBy = parsedData.searchBy || '';
            // }
        }
    }
    async ngOnInit() { 
        
          this.getTenantTeams();  
        this.subscribeBotLiveChat();
        this.subscribeCustomertMessages();
        this.subscribeAnotherAgentMessages();
        this.filterTicket();

        this.getUsers();

        // this.isArabic = rtlDetect.isRtlLang(
        //     abp.localization.currentLanguage.name
        // );


        this.theme = ThemeHelper.getTheme();

        await this.getIsAdmin();


    }
         private doesFailActiveFilters(data: CustomerLiveChatModel): boolean {
          if (this.ticketId && data.idLiveChat !== +this.ticketId) return true;
        
          if (this.ticketType && data.categoryType?.toLowerCase() !== this.ticketType.toLowerCase()) return true;
        
          if (this.agent && data.agentId !== +this.agent) return true;
        
          if (this.searchName && !data.displayName?.toLowerCase().includes(this.searchName.toLowerCase())) return true;
        
          if (this.departemntName && !data.department?.toLowerCase().includes(this.departemntName.toLowerCase())) return true;
        
          if (this.summary && !data.ticketSummary?.toLowerCase().includes(this.summary.toLowerCase())) return true;
          // if (this.byteam && !data.byteam?.toLowerCase().includes(this.byteam.toLowerCase())) return true;
        
          if (this.searchByPhone && !data.phoneNumber?.includes(this.searchByPhone)) return true;
        
        const statusMap: { [key: number]: string } = {
            1: "pending",
            2: "opened",
            3: "closed",
            4: "Confirm",
            5: "Reject",
            6: "Assigned"
        };
        
        if (this.status && statusMap[data.liveChatStatus]?.toLowerCase() !== this.status.toLowerCase()) return true;
        if (
            this.dateRange &&
            this.dateRange.length === 2 &&
            data.requestedLiveChatTime &&
            !data.requestedLiveChatTime.isBetween(moment(this.dateRange[0]), moment(this.dateRange[1]), undefined, '[]')
           ) {
            return true;
          }
    
          return false; 
        }
    subscribeBotLiveChat = () => {
        //live-chat-get // get live chat status
        debugger;
        this.botLivchatSub = this.socketioService.liveChat.subscribe(
            (data: CustomerLiveChatModel) => {
                if (this.doesFailActiveFilters(data)) {
                    debugger;
                    return; 
                    }
                if (this.appSession.tenantId === data.tenantId) {
                    const index = this.primengTableHelper.records.findIndex(
                        (e) => e.idLiveChat === data.idLiveChat
                    );
                    if (index == -1) {

                        debugger

                        if(data.lockedByAgentName==null){

                            data.lockedByAgentName="";
                        }


                         if((data.agentId!=this.appSession.user.id)&&!this.isAdmin && (data.lockedByAgentName!="")){



                            
                         }else{



                                         let ticket = new CustomerLiveChatModel();
 

                             ticket.contactCreationDate = data.contactCreationDate; 

                                      ticket.isNote = data.isNote; 
                                       ticket.numberNote = data.numberNote;
                                       
                                       
                        ticket.requestDescription = data.requestDescription;
                        ticket.liveChatStatus = data.liveChatStatus;
                        ticket.actionTime = data.actionTime;    
                        if(data.categoryType=="Request"){
                            ticket.department = data.requestDescription; //data.requestDescription;
                        }
                        else{
                            ticket.department = data.department; //data.department;
                        }
                        ticket.categoryType = data.categoryType;
                        ticket.ticketSummary = data.ticketSummary;
                        // ticket.byteam = data.byteam;
                        ticket.idLiveChat = data.idLiveChat;
                        ticket.userId = data.userId;
                        ticket.openTimeTicket = data.openTimeTicket;
                        // let date = moment(data.requestedLiveChatTime).utc();
                        ticket.requestedLiveChatTime =
                            data.requestedLiveChatTime;
                        ticket.closeTimeTicket = data.closeTimeTicket;
                        ticket.durationTime = data.durationTime;
                        ticket.agentId = data.agentId;
                        ticket.assignedToUserId = data.assignedToUserId;
                        ticket.lockedByAgentName = data.lockedByAgentName;
                        ticket.isConversationExpired =
                            data.isConversationExpired;
                        ticket.isOpen = data.isOpen;
                        ticket.isConversationExpired =
                            data.isConversationExpired;
                        ticket.displayName = data.displayName;
                        ticket.phoneNumber = data.phoneNumber;
                        ticket.userIds = data.userIds;
                        ticket.assignedToUserName = data.assignedToUserName;

                        ticket.isNote = data.isNote;
                        ticket.numberNote = data.numberNote;


                        if (
                            ticket.userIds != null &&
                            ticket.userIds != undefined &&
                            ticket.userIds != "" &&
                            ticket.userIds != ""
                        ) {
                            if (
                                ticket.userIds.includes(
                                    this.appSession.user.id.toString()
                                )
                            ) {
                                this.primengTableHelper.records.unshift(ticket);
                                this.primengTableHelper.totalRecordsCount =
                                    this.primengTableHelper.totalRecordsCount +
                                    1;
                            } else {
                                // let ishasPermissionToDepartment =
                                // this._permissionCheckerService.isGranted(
                                //     "Pages.Department"
                                // );
                                if (
                                    data.departmentUserIds != null ||
                                    data.departmentUserIds != undefined
                                ) {
                                    if (
                                        data.departmentUserIds.includes(
                                            this.appSession.user.id.toString()
                                        )
                                    ) {
                                        this.primengTableHelper.records.unshift(
                                            ticket
                                        );
                                        this.primengTableHelper.totalRecordsCount =
                                            this.primengTableHelper
                                                .totalRecordsCount + 1;
                                    }
                                } else {
                                    if (
                                        ticket.userIds.includes(
                                            this.appSession.user.id.toString()
                                        )
                                    ) {
                                        this.primengTableHelper.records.unshift(
                                            ticket
                                        );
                                        this.primengTableHelper.totalRecordsCount =
                                            this.primengTableHelper
                                                .totalRecordsCount + 1;
                                    }
                                }
                            }
                        } else {
                            this.primengTableHelper.records.unshift(ticket);
                            this.primengTableHelper.totalRecordsCount =
                                this.primengTableHelper.totalRecordsCount + 1;
                        }
                         }
           
                    } else {
                        try {
debugger
                            if((data.agentId!=this.appSession.user.id)&&!this.isAdmin){

                       this.primengTableHelper.records.splice(index, 1);
                       this.primengTableHelper.totalRecordsCount = Math.max(    0,  this.primengTableHelper.totalRecordsCount - 1   );

                            }else{


                   this.primengTableHelper.records[
                                index
                            ].contactCreationDate = data.contactCreationDate;




                     this.primengTableHelper.records[
                                index
                            ].isNote = data.isNote;

                     this.primengTableHelper.records[
                                index
                            ].numberNote = data.numberNote;




                            
                            this.primengTableHelper.records[
                                index
                            ].requestDescription = data.requestDescription;
                            this.primengTableHelper.records[
                                index
                            ].liveChatStatus = data.liveChatStatus;
                            this.primengTableHelper.records[index].actionTime =
                                data.actionTime;
                            this.primengTableHelper.records[
                                index
                            ].categoryType = data.categoryType;
                            this.primengTableHelper.records[index].department =
                                data.department;
                            this.primengTableHelper.records[
                                index
                            ].openTimeTicket = data.openTimeTicket;
                            this.primengTableHelper.records[
                                index
                            ].requestedLiveChatTime =
                                data.requestedLiveChatTime;
                            this.primengTableHelper.records[
                                index
                            ].closeTimeTicket = data.closeTimeTicket;
                            this.primengTableHelper.records[
                                index
                            ].durationTime = data.durationTime;
                            this.primengTableHelper.records[
                                index
                            ].lockedByAgentName = data.lockedByAgentName;
                            this.primengTableHelper.records[
                                index
                            ].isConversationExpired =
                                data.isConversationExpired;
                            this.primengTableHelper.records[index].isOpen =
                                data.isOpen;
                            this.primengTableHelper.records[index].displayName =
                                data.displayName;
                            this.primengTableHelper.records[index].phoneNumber =
                                data.phoneNumber;
                            this.primengTableHelper.records[
                                index
                            ].assignedToUserName = data.assignedToUserName;
                            this.primengTableHelper.records[
                                index
                            ].assignedToUserId = data.assignedToUserId;
                            this.primengTableHelper.records[index].agentId =
                                data.agentId;
                            this.primengTableHelper.records[index].userId =
                                data.userId;
                            this.primengTableHelper.records[index].userIds =
                                data.userIds;
                            this.primengTableHelper.records[
                                index
                            ].ticketSummary = data.ticketSummary;

                            //  this.primengTableHelper.records[
                            //     index
                            // ].byteam = data.byteam;
                            this.primengTableHelper.records[
                                index
                            ].isCisConversationExpired =
                                data.isConversationExpired;


                            }
                          


                        } catch {
                            this.primengTableHelper.records.forEach(
                                (element) => {
                                    if (element.idLiveChat == data.idLiveChat) {

                                   element.isNote =
                                            data.isNote;
                                                    element.numberNote =
                                            data.numberNote;

                                        element.requestDescription =
                                            data.requestDescription;

                                        element.liveChatStatus =
                                            data.liveChatStatus;
                                        element.actionTime = data.actionTime;
                                        element.categoryType =
                                            data.categoryType;
                                        element.department = data.department;
                                        element.openTimeTicket =
                                            data.openTimeTicket;
                                        element.requestedLiveChatTime =
                                            data.requestedLiveChatTime;
                                        element.closeTimeTicket =
                                            data.closeTimeTicket;
                                        element.durationTime =
                                            data.durationTime;
                                        element.lockedByAgentName =
                                            data.lockedByAgentName;
                                        element.isConversationExpired =
                                            data.isConversationExpired;
                                        element.isOpen = data.isOpen;
                                        element.displayName = data.displayName;
                                        element.phoneNumber = data.phoneNumber;
                                        element.assignedToUserName =
                                            data.assignedToUserName;
                                        element.assignedToUserId =
                                            data.assignedToUserId;
                                        element.agentId = data.agentId;
                                        element.ticketSummary =
                                            data.ticketSummary;
                                        //  element.byteam =
                                        //     data.byteam;
                                        element.userId = data.userId;
                                        element.userIds = data.userIds;
                                        element.userId.isCisConversationExpired =
                                            data.isConversationExpired;
                                   element.contactCreationDate =data.contactCreationDate;
                                    }
                                }
                            );
                        }
                    }

                    this.primengTableHelper.totalRecordsCount =
                        this.primengTableHelper.totalRecordsCount + 1;
                    //  this.getTime(this.primengTableHelper.records);
                }
            }
        );
    };

    getDataFromLocalStorage(name: string) {
        // Retrieve data from local storage
        const jsonString = localStorage.getItem(name);

        // Parse JSON string to JavaScript object
        if (jsonString) {
            const dataObject = JSON.parse(jsonString);
            console.log("Data retrieved from local storage:", dataObject);
            return dataObject;
        } else {
            console.log("No data found in local storage");
            return null;
        }
    }

    shouldAnimate(id: any) {
        if (!this.STORAGEDATE?.id || !id) return;
        return this.STORAGEDATE.id == id ? true : false;
    }

    // restticket(event?: LazyLoadEvent){
    //     this.first=0
    //     this.rows=20;
    //     this.ticketId="";
    //     this.status=null;
    //     this.ticketType=null;
    //     this.ticketId="";
    //    this. getTicketAndRequest(event);

    // }

    getTicketAndRequest(event?: LazyLoadEvent) {
        // if (this.primengTableHelper.shouldResetPaging(event)) {
        //     this.paginator.changePage(0);
        //     return;
        // }
        debugger
this.byteam;
        let agent = null;
        if (this.agent) {
            agent = this.agent.toString();
        
        }

        let ticketType = null;
        if (this.ticketType) {
            ticketType = this.ticketType.toString();
        }
        let startDate = null;
        let endDate = null;

        if (this.dateRange != undefined) {
            startDate = moment(this.dateRange[0]);
            this.startDate=startDate
            endDate = moment(this.dateRange[1]);
            this.endDate=endDate;
        }
        let startDateC = null;
        let endDateC = null;

        if (this.dateRangeC != undefined) {
            startDateC = moment(this.dateRangeC[0]);
            this.startDateC=startDateC
            endDateC = moment(this.dateRangeC[1]);
            this.endDateC=endDateC;
        }
        this.primengTableHelper.showLoadingIndicator();
        // ( name: string, ticketType: string, statusId: number, pageNumber: number, pageSize: number)
        let statusID =
            this.status == "pending"
                ? 1
                : this.status == "opened"
                ? 2
                : this.status == "closed"
                ? 3
                : this.status == "expired"
                ? 6
                : this.status == "Confirm"
                ? 4
                : this.status == "Reject"
                ? 5
                : null;

        this. statusIdToExportToExecel=statusID;

        this.STORAGEDATE = this.getDataFromLocalStorage("ticketData");
        this.headersListFromStorage =
            this.getDataFromLocalStorage("headersList");
            let first = this.first;
            let row = this.rows;

        let id = 0;

        if (this.STORAGEDATE) {
            first = this.STORAGEDATE?.first
                ? this.STORAGEDATE.first
                : event?.first;
            row = this.STORAGEDATE?.row ? this.STORAGEDATE.row : event?.rows;
            id = this.STORAGEDATE?.id ? this.STORAGEDATE?.id : id;
            statusID = this.STORAGEDATE?.statusID
                ? this.STORAGEDATE.statusID
                : statusID;
                this.statusIdToExportToExecel=statusID;
            this.status =
                statusID == 1
                    ? "pending"
                    : statusID == 2
                    ? "opened"
                    : statusID == 3
                    ? "closed"
                    : statusID == 6
                    ? "expired"
                    : statusID == 4
                    ? "Confirm"
                    : statusID == 5
                    ? "Reject"   
                    : statusID == 6
                    ? "Assigned"
                    : "";
                    this.statusIdToExportToExecel=statusID;

            startDate = this.STORAGEDATE?.startDate
                ? moment(new Date(this.STORAGEDATE.startDate))
                : startDate;
                this.startDate=startDate;
            endDate = this.STORAGEDATE?.endDate
                ? moment(new Date(this.STORAGEDATE.endDate))
                : endDate;

            startDateC = this.STORAGEDATE?.startDateC
                ? moment(new Date(this.STORAGEDATE.startDateC))
                : startDateC;
                this.startDateC=startDateC;
            endDateC = this.STORAGEDATE?.endDateC
                ? moment(new Date(this.STORAGEDATE.endDateC))
                : endDateC;
                this.endDateC=endDateC;

            if (this.STORAGEDATE?.dateRange)
                this.dateRange = this.STORAGEDATE?.dateRange
                    ? [
                            new Date(this.STORAGEDATE.dateRange[0]),
                            new Date(this.STORAGEDATE.dateRange[1]),
                      ]
                    : [
                            new Date(this.dateRange[0]),
                            new Date(this.dateRange[1]),
                      ];

            if (this.STORAGEDATE?.dateRangeC)
                this.dateRangeC = this.STORAGEDATE?.dateRangeC
                    ? [
                            new Date(this.STORAGEDATE.dateRangeC[0]),
                            new Date(this.STORAGEDATE.dateRangeC[1]),
                      ]
                    : [
                            new Date(this.dateRangeC[0]),
                            new Date(this.dateRangeC[1]),
                      ];
            agent = this.STORAGEDATE?.agent ? this.STORAGEDATE.agent : agent;
            this.agent = this.STORAGEDATE?.agent ? this.STORAGEDATE.agent : "";

            this.ticketType = this.STORAGEDATE?.ticketType
                ? this.STORAGEDATE.ticketType
                : this.ticketType;
            this.searchByPhone = this.STORAGEDATE?.search
                ? this.STORAGEDATE.search
                : this.searchByPhone;
            this.searchName = this.STORAGEDATE?.searchName
                ? this.STORAGEDATE.searchName
                : this.searchName;
                this.departemntName = this.STORAGEDATE?.departemntName
                ? this.STORAGEDATE.departemntName
                : this.departemntName;
                this.summary = this.STORAGEDATE?.summary
                ? this.STORAGEDATE.summary
                : this.summary;
                // this.ticketId = this.STORAGEDATE?.LiveChatId

                // ? this.STORAGEDATE.LiveChatId

                // : "";
        } else {
            first = event?.first !== undefined ? event.first : first;
            row = event?.rows || row;
        }

        if (this.headersListFromStorage) {
            this.headersList = this.headersListFromStorage;
        }       


        // debugger
        // this.STORAGEDATE

        // if(this.ticketId!=""){
        //     first=0;
        //     row=20;

        // }

        // agent=10430;
        //    this.ticketType= "Ticket";
        // statusID=2;
        // this.statusIdToExportToExecel=ticketId

        let teamid=0;
        teamid=Number(this.byteam);
        this._liveChatServiceProxy
            .getTicket(
                startDate,
                endDate,
                this.searchByPhone,
                agent,
                this.searchName,
                this.departemntName,
                this.ticketType,
                statusID,
                first,
                row,
                this.ticketId,
                this.appSession.user.id.toString(),
                this.summary,startDateC,endDateC,teamid
                // STORAGEDATE? STORAGEDATE.first : event?.first,
                // STORAGEDATE? STORAGEDATE.row :  event?.rows
                // this.primengTableHelper.getSkipCount(this.paginator, event),
                // this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    // this.primengTableHelper.
                    // console.log(this.primengTableHelper.totalRecordsCount);
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe(
                (result) => {
                  debugger;

                    this.totalResolutionHours = result.totalResolutionTime;
                    //  this.convertMinutesToHoursAndMinutes(result.totalResolutionTime)
                    this.primengTableHelper.totalRecordsCount =
                        result.totalCount;
                    this.totalCount = result.totalCount;
                    this.totalClosed = result.totalClosed;
                    this.totalPending = result.totalPending;
                    this.totalExpired = result.totalExpired;
                    this.totalOpen = result.totalOpen;
                    if (this.STORAGEDATE) {
                        id = this.STORAGEDATE?.id;

                        setTimeout(() => {
                            //    const e =  document.querySelector(`${id}`);

                            const rowElement = document.getElementById("" + id);
                            // console.log(this.rowRef);
                            if (rowElement) {
                                rowElement.scrollIntoView({
                                    behavior: "smooth",
                                    block: "center",
                                });
                            }
                        }, 1000);
                    }

                    if (this.isArabic) {
                        result.lstLiveChat.forEach((element) => {
                            element.requestedLiveChatTime = moment(
                                this.convertHijriToGregorian(
                                    moment(element.requestedLiveChatTime)
                                        .locale("en")
                                        .format("YYYY-MM-DDTHH:mm:ss")
                                )
                            );
                            if (
                                element.openTimeTicket != null ||
                                element.closeTimeTicket != undefined
                            ) {
                                element.openTimeTicket = moment(
                                    this.convertHijriToGregorian(
                                        moment(element.openTimeTicket)
                                            .locale("en")
                                            .format("YYYY-MM-DDTHH:mm:ss")
                                    )
                                );
                            }
                            if (
                                element.closeTimeTicket != null ||
                                element.closeTimeTicket != undefined
                            ) {
                                element.closeTimeTicket = moment(
                                    this.convertHijriToGregorian(
                                        moment(element.closeTimeTicket)
                                            .locale("en")
                                            .format("YYYY-MM-DDTHH:mm:ss")
                                    )
                                );
                            }
                        });
                    }
                    this.primengTableHelper.records = result.lstLiveChat;
                },
                () => {
                    setTimeout(() => {
                        if (this.STORAGEDATE) {
                            localStorage.removeItem("ticketData");
                            this.STORAGEDATE = null;
                        }
                    }, 3000);
                },
                () => {
                    setTimeout(() => {
                        if (this.STORAGEDATE) {
                            localStorage.removeItem("ticketData");
                            this.STORAGEDATE = null;
                        }
                    }, 3000);
                }
            );
        this.first = first;
        this.rows = row;

    }

    getLiveChat(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        let agent = null;
        if (this.agent) {
            agent = this.agent.toString();
        }

        let startDate = null;
        let endDate = null;

        if (this.dateRange != undefined) {
            startDate = moment(this.dateRange[0])
                .locale("en")
                .format("MM/DD/yyyy");
            endDate = moment(this.dateRange[1])
                .locale("en")
                .format("MM/DD/yyyy");
        }
              
        let startDateC = null;
        let endDateC = null;

        if (this.dateRangeC != undefined) {
            startDateC = moment(this.dateRangeC[0])
                .locale("en")
                .format("MM/DD/yyyy");
            endDateC = moment(this.dateRangeC[1])
                .locale("en")
                .format("MM/DD/yyyy");
        }
        this.primengTableHelper.showLoadingIndicator();

        this.primengTableHelper.getSkipCount(this.paginator, event);
        this.primengTableHelper.getMaxResultCount(this.paginator, event);
        this._liveChatServiceProxy
            .getLiveChat(
                this.searchByPhone,
                agent,
                this.searchName,
                startDate,
                endDate,
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => this.primengTableHelper.hideLoadingIndicator())
            )
            .subscribe((result) => {
                this.totalResolutionHours = result.totalResolutionTime;
                // this.convertMinutesToHoursAndMinutes(result.totalResolutionTime)
                
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.totalCount = result.totalCount;
                this.totalClosed = result.totalClosed;
                this.totalPending = result.totalPending;
                this.totalOpen = result.totalOpen;


                if (this.isArabic) {
                    result.lstLiveChat.forEach((element) => {
                        element.requestedLiveChatTime = moment(
                            this.convertHijriToGregorian(
                                moment(element.requestedLiveChatTime)
                                    .locale("en")
                                    .format("YYYY-MM-DDTHH:mm:ss")
                            )
                        );
                        if (
                            element.openTimeTicket != null ||
                            element.closeTimeTicket != undefined
                        ) {
                            element.openTimeTicket = moment(
                                this.convertHijriToGregorian(
                                    moment(element.openTimeTicket)
                                        .locale("en")
                                        .format("YYYY-MM-DDTHH:mm:ss")
                                )
                            );
                        }
                        if (
                            element.closeTimeTicket != null ||
                            element.closeTimeTicket != undefined
                        ) {
                            element.closeTimeTicket = moment(
                                this.convertHijriToGregorian(
                                    moment(element.closeTimeTicket)
                                        .locale("en")
                                        .format("YYYY-MM-DDTHH:mm:ss")
                                )
                            );
                        }
                    });
                }
                this.primengTableHelper.records = result.lstLiveChat;
            });
    }

    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split("T");
        const [year, month, day] = hijriDateString.split("-").map(Number);
        const gregorianDate = toGregorian(year, month, day);

        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, "0"),
            String(gregorianDate.gd).padStart(2, "0"),
        ].join("-");

        return formattedDate + "T" + time;
    }


    getTenantTeams() {
        this._teamsService.teamsGetAll(null, 0, 100000).subscribe((result) => {
            this.selectMultiTeams = result.teamsDtoModel;
        });
    }

    onDateRangeUpdate(event?) {
        if (event != undefined) {
            this.dateRange = event;
            this.dateRange[0] = this.setToStartOfDay(this.dateRange[0]);
            this.dateRange[1] = this.setToEndOfDay(this.dateRange[1]);
            this.ensureValidRange();
        }
        this.getTicketAndRequest();
    }
    onDateRangeCUpdate(event?) {
        if (event != undefined) {
            this.dateRangeC = event;
            this.dateRangeC[0] = this.setToStartOfDay(this.dateRangeC[0]);
            this.dateRangeC[1] = this.setToEndOfDay(this.dateRangeC[1]);
            this.ensureValidRangeC();
        }
        this.getTicketAndRequest();
    }
    getUsers() {
        this.teamService.getUsers().subscribe((result: any) => {
            this.users = result.result.items;
        });
    }
    showAssignToModal(liveChatId, contactId): void {
        this.selectedLiveChatId = liveChatId;
        this.selectedContactId = contactId;
        this.assignToModal.show();
    }
    closeChatFromTeamInbox(contactId) {
        this.teamService
            .updateCustomerStatus(
                contactId,
                false,
                this.appSession.user.name,
                0
            )
            .subscribe((res) => {
                if (res) {
                }
            });
    }

    assigned(event?: any) {
        this._liveChatServiceProxy
            .assignLiveChatToUser(
                this.selectedLiveChatId,
                event.id,
                event.name,
                this.appSession.user.userName,
                this.appSession.user.id,""
            )
            .subscribe((res: any) => {
                // this.selectedUser.isOpen= res.result.isOpen
                this.assignToModal.close();
                this.selectedLiveChatId = null;
                this.closeChatFromTeamInbox(this.selectedContactId);
                this.selectedContactId = null;
                this.getTicketAndRequest();
            });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }



convertMinutesToTimeFormat(minutes: number): string {
    const days = Math.floor(minutes / (24 * 60)); 
    const hours = Math.floor((minutes % (24 * 60)) / 60); 
    const remainingMinutes = minutes % 60; 
    let result = "";
    if (days > 0) {
        result += `${days}D:`;
    }
    if (hours > 0) {
        result += `${hours}H:`;
    }
    result += `${remainingMinutes}min`;
    return result;

}

exportToExcel(): void {
    this.spinner.show();

    this._liveChatServiceProxy
        .getTicket(
            this.startDate ?? null,
            this.endDate ?? null,
            this.searchByPhone ?? null,
            this.agent ?? null,
            this.searchName ?? null,
            this.departemntName ?? null,
            this.ticketType ?? null,
            this.statusIdToExportToExecel,
            0,
            100000,
            this.ticketId ?? null,
            this.appSession.user.id.toString(),
             this.summary,this.startDateC,this.endDateC,0
        )
        .subscribe(
            (result) => {
                this.AllTicketRecordAfterFilter = result.lstLiveChat;

                if (!this.AllTicketRecordAfterFilter || this.AllTicketRecordAfterFilter.length === 0) {
                    this.spinner.hide();
                    return;
                }
                debugger

                import("xlsx").then((xlsx) => {
                    const formattedRecords = this.AllTicketRecordAfterFilter.map(record => {
                        return {
                            idLiveChat: record.idLiveChat,
                            categoryType: record.categoryType,
                            lockedByAgentName: record.lockedByAgentName,
                            department: record.department,
                            displayName: record.displayName,
                            phoneNumber: record.phoneNumber,
                            requestedLiveChatTime: record.requestedLiveChatTime
                                ? this.convertToEnglishDate(moment(record.requestedLiveChatTime).format("YYYY-MM-DD HH:mm:ss"))
                                : '',
                                openTime: record.openTime
                                ? this.convertToEnglishDate(
                                    moment(record.openTime)
                                        .locale('en')
                                        .format("hh:mm A")
                                        .replace('ص', 'AM')
                                        .replace('م', 'PM')
                                )
                              : '',
                                closeTimeTicket: record.closeTimeTicket
                                ? this.convertToEnglishDate(
                                    moment(record.closeTimeTicket)
                                        .locale('en')
                                        .format("hh:mm A")
                                        .replace('ص', 'AM')
                                        .replace('م', 'PM')
                                )
                              : '',
                            liveChatStatusName: record.liveChatStatusName == 'Done' ? 'Closed' : record.liveChatStatusName, 
                            durationTime: record.durationTime < 0 ? '': record.durationTime ,                         
                            dateOpen: record.openTimeTicket ? this.convertToEnglishDate(moment(record.openTimeTicket).format("YYYY-MM-DD")) : '', 
                            timeToOpen: record.timeToOpen ? this.convertMinutesToTimeFormat(record.timeToOpen) : '', 

                            ticketSummary: record.ticketSummary,
                              byteam: record.byteam,
                            contactCreationDate: record.contactCreationDate ? this.convertToEnglishDate(moment(record.contactCreationDate).format("YYYY-MM-DD")) : '', 

                        };
                    });

                    const worksheet = xlsx.utils.json_to_sheet(formattedRecords);

                    const headers = [
                        "ID", "Type", "Agent", "Department",
                        "Customer Name", "Phone Number", "Time", "Open Time","Close Time",  "Status", "Resolution",
                        "Date Open ","Time to Open", "Summary","Contact Creation Date"
                    ];
                    xlsx.utils.sheet_add_aoa(worksheet, [headers], { origin: "A1" });

                    const workbook = xlsx.utils.book_new();
                    xlsx.utils.book_append_sheet(workbook, worksheet, "LiveChat Records");

                    const excelBuffer = xlsx.write(workbook, {
                        bookType: "xlsx",
                        type: "array",
                    });

                    const blob = new Blob([excelBuffer], {
                        type: "application/octet-stream",
                    });

                    const link = document.createElement("a");
                    link.href = window.URL.createObjectURL(blob);
                    link.download = `LiveChatRecords_${new Date().toISOString()}.xlsx`;
                    link.click();

                    this.spinner.hide();
                }).catch((error) => {
                    this.spinner.hide();
                    console.error("Error generating Excel:", error);
                });
            },
            (error) => {
                console.error("Error fetching tickets:", error);
                this.spinner.hide();
            }
        );
}


    
    
    convertToEnglishDate(date: string): string {
        const arabicToEnglishMap = {
            '٠': '0',
            '١': '1',
            '٢': '2',
            '٣': '3',
            '٤': '4',
            '٥': '5',
            '٦': '6',
            '٧': '7',
            '٨': '8',
            '٩': '9'
        };
    
        return date.replace(/[٠-٩]/g, char => arabicToEnglishMap[char] || char);
    }
    
    
    



    
    
    
    
    
    

    subscribeCustomertMessages = () => {

        this.customerMessageSub =
            this.socketioService.customerMessage.subscribe((data: Channel) => {

                debugger;//C
                if (data.tenantId == this.appSession.tenantId) {
                    if (this.primengTableHelper.records != null) {
                        this.primengTableHelper.records.forEach((e) => {
                            if (e.phoneNumber === data.phoneNumber) {
                                e.conversationsCount = data.conversationsCount;
                                let ishere =
                                    window.location.href.includes("liveChat");
                                if (
                                    data.isOpen &&
                                    ishere &&
                                    e.liveChatStatus === 2
                                ) {
                                    this.sound = new Howl({
                                        src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/WhatsAppNotification.mp3",
                                        html5: true,
                                        volume: 1.0,
                                    });
                                    this.sound.play();
                                }
                            }
                        });
                    }
                }
            });
    };

subscribeAnotherAgentMessages = () => {
    this.anotherAgentMessgeSub = this.socketioService.agentMessage.subscribe((data: Channel) => {
         // A

        if (data.tenantId == this.appSession.tenantId) {
            if (this.primengTableHelper.records != null) {
                this.primengTableHelper.records.forEach((e) => {
                    if (e.phoneNumber === data.phoneNumber) {
                        e.isOpen = data.isOpen;
                    }
                });

                // if (data.agentId !== this.appSession.userId) {
                //     // Remove the record where idLiveChat matches
                //     this.primengTableHelper.records = this.primengTableHelper.records.filter(
                //         (e) => e.idLiveChat !== data.id
                //     );
                // }
            }
        }
    });
};


    handleNumberSearch(newValue: string): void {
        this.addFiliterDisabled=false;
        this.first = 0;
        this.searchByPhone = newValue;
    }

    handleTicketIdSearch(newValue: string): void {
        this.addFiliterDisabled=false;
        this.first = 0;
        this.ticketId = newValue;
    }
    handleSearchName(elem: Event): void {

        this.addFiliterDisabled=false;
        this.first = 0;
        const inputElement = elem.target as HTMLInputElement;
        this.searchName = inputElement.value;
        // console.log(this.searchName);
    }
    handleSearchDepartemnt(elem: Event): void {
        this.addFiliterDisabled=false;
        this.first = 0;
        const inputElement = elem.target as HTMLInputElement;
        this.departemntName = inputElement.value;
        // console.log(this.searchName);
    }
    handleSearchsummary(elem: Event): void {
        this.addFiliterDisabled=false;
        this.first = 0;
        const inputElement = elem.target as HTMLInputElement;
        this.summary = inputElement.value;
        // console.log(this.searchName);
    }
    handleSearchbyteam(elem: Event): void {
        this.addFiliterDisabled=false;
        this.first = 0;
        const inputElement = elem.target as HTMLInputElement;
        this.byteam = inputElement.value;
        // console.log(this.searchName);
    }

    
    
    
    handleTableSettings(): void {
        this.listSettings.show();
    }

    onModalSave(updatedList) {
        this.headersList = updatedList;
        this.saveToLocalStorage(this.headersList, "headersList");
    }

    handleSearchChange(event): void {

        this.addFiliterDisabled=false;
        this.first = 0;
    }




    assignToTeamsBtn(ticket) {


        debugger
        this.assignUsers.show(ticket,2);
    }



    assignToBtn(ticket) {
        this.assignUsers.show(ticket,1);
    }
    ensureValidRange() {
        if (this.dateRange[0] > this.dateRange[1]) {
            [this.dateRange[0], this.dateRange[1]] = [
                this.dateRange[1],
                this.dateRange[0],
            ];
        }
    }
    ensureValidRangeC() {
        if (this.dateRangeC[0] > this.dateRangeC[1]) {
            [this.dateRangeC[0], this.dateRangeC[1]] = [
                this.dateRangeC[1],
                this.dateRangeC[0],
            ];
        }
    }
    openModal(){
        this.selectedUser.userId=this.userIdfromTemplate
        this.selectedUser.displayName=this.name

        this.modal2.viewDetails(this.template || [], this.selectedUser);
        // this.them12Component.executeFromParent();

        // this.getTemplates();
    }
    
    // openModal(selectedUser: Channel) {
    //     // this.modal2.viewDetails(this.template || [], selectedUser);
    // }
    getTemplates() {
        this.template = [];
        this.appSession.tenantId
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppTemplateForCampaign(0, 50, this.appSession.tenantId)
            .subscribe((result) => {
                this.template = result.lstWhatsAppTemplateModel.filter(
                    (element) =>
                        element.language == "ar" || element.language == "en"
                );
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    handleTemplateList(list): void {
        this.template = list;
        // console.log('List received from child:', list);
    }
    handleUpdateStatus(){
        this.viewTicketModal.updateStatus();
    }




    backUpAll() {
       this.exportToExcel();
    }

      exportToExcelcheck(){
        this.BackupAllModal.show()
    }

    handleModalResult(result: boolean) {
        if (result) {
            this.backUpAll();
        }
        if (result) {
            this.exportToExcelcheck();
        }
    }


}
