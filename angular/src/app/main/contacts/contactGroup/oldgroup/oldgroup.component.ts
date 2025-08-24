import { Component, ElementRef, Injector, ViewChild, inject } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    AreaDto,
    AreasServiceProxy,
    AssetServiceProxy,
    ContactsEntity,
    GetAllDashboard,
    GroupCreateDto,
    GroupDtoModel,
    GroupMembersDto,
    GroupServiceProxy,
    MembersDto,
    WhatsAppContactsDto,
} from "@shared/service-proxies/service-proxies";
import { DatePipe } from "@angular/common";
import "chartjs-plugin-labels";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import {
    SearchCountryField,
    CountryISO,
    PhoneNumberFormat,
} from "ngx-intl-tel-input";
import moment from "moment";
import { PermissionCheckerService } from "abp-ng2-module";
import * as rtlDetect from 'rtl-detect';
import { MessageCampaignService } from "@app/main/message-campaign/message-campaign.service";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { contactFilterModel } from "../group.model";
import { LazyLoadEvent } from "primeng/api";
import { SharedService } from "@shared/shared-services/shared.service";

@Component({
    selector: 'app-oldgroup',
    templateUrl: './oldgroup.component.html',
    styleUrls: ['./oldgroup.component.css']
})
export class OldgroupComponent extends AppComponentBase {
    theme: string;


    submitted = false;
    filtered = false;
    groupAddResult!: GroupCreateDto;


    
  
  visible: boolean = false;

  showDialog() {
      this.visible = true;
  }

  closeDialog(){
     this.router.navigate(['/app/main/groupcontact'])
    this.visible = false;

  }


    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: false }) paginator: Paginator;


    tabs = [
        {
            customClass: "duration-tab",
            heading: "Today",
        },
        {
            customClass: "duration-tab",
            heading: "Yesterday",
        },
        {
            customClass: "duration-tab",
            heading: "Last Week",
        },
        {
            customClass: "duration-tab",
            heading: "Last Month",
        },
    ];
    products: any[];

    cols: any[];



    dateRange: [Date, Date] = [new Date(), new Date()];
    orderDateRange: [Date, Date] = [new Date(), new Date()];
    predefinedRanges = [
        {
            value: [
                new Date(),
                new Date(new Date().setDate(new Date().getDate() + 1)),
            ],
            label: "Today",
        },
        {
            value: [
                new Date(new Date().setDate(new Date().getDate() - 1)),
                new Date(),
            ],
            label: "Yesterday",
        },
        {
            value: [
                new Date(new Date().setDate(new Date().getDate() - 7)),
                new Date(),
            ],
            label: "Last 7 Days",
        },
        {
            value: [
                new Date(new Date().setMonth(new Date().getMonth() - 1)),
                new Date(),
            ],
            label: "Last Month",
        },
        {
            value: [
                new Date(new Date().setFullYear(new Date().getFullYear() - 1)),
                new Date(),
            ],
            label: "Last Year",
        },
    ];
    dateRangePickerOptions = {
        dateInputFormat: "DD/MM/YYYY",
        rangeInputFormat: "DD/MM/YYYY",
        selectFromOtherMonth: true,
        ranges: this.predefinedRanges,
        showPreviousMonth: true,
        showWeekNumbers: false,
        useUtc: true,
    };

    showMessageLoader: boolean = false;
    saving = false;
    contactFlag: boolean = false;
    filterContactFlag: boolean = true;
    externalContactFlag: boolean = false;
    fileToUpload: any;
    tempFile: any;
    extFile: string;
    sendTime: string;
    totalContact: number;
    groupName: string = '';

    contact: contactFilterModel = new contactFilterModel();


    lstContact: MembersDto[] = [new MembersDto()];

    lstContactWithoutOptOut: string[] = [];
    ContactsEntity: ContactsEntity = new ContactsEntity();

    totalOptOut: number = 0;

    dropdownList = [];
    dropdownSettings = {};
    selectedItems = [];
    selectedContact: MembersDto[] = [];

    isCount = false;

    area: AreaDto = new AreaDto();

    dailyLimit: number = 0;
    countryObj: any = null;
    countryObj2: any = null;
    separateDialCode = false;
    SearchCountryField = SearchCountryField;
    CountryISO = CountryISO;
    PhoneNumberFormat = PhoneNumberFormat;
    preferredCountries: CountryISO[] = [
        CountryISO.SaudiArabia,
        CountryISO.Jordan,
    ];
    isHasPermissionOrder: boolean;

    Measurement: GetAllDashboard = new GetAllDashboard();
    confirm: string = "";
    sendCampaignValidation: boolean = true;
    // groupCreateDto: GroupMembersDto;
    orderTime1: moment.Moment | undefined;
    orderTime2: moment.Moment | undefined;

    isArabic = false;

    private groupSharedService : SharedService = inject(SharedService);


    constructor(
        injector: Injector,
        private route: ActivatedRoute,
        private messageCampaignService: MessageCampaignService,
        private router: Router,
        private datePipe: DatePipe,
        private _assetServiceProxy: AssetServiceProxy,
        private _areasServiceProxy: AreasServiceProxy,
        private _permissionCheckerService: PermissionCheckerService,
        public darkModeService: DarkModeService,
        private _router: Router,
        private groupService: GroupServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.checkName();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        this.theme = ThemeHelper.getTheme();
        this.isHasPermissionOrder =
            this._permissionCheckerService.isGranted("Pages.Contacts");

        this.initializeDateRange();
        this.initializeDropdownSettings();

    }

    checkName(){
        this.groupSharedService.currentGroupName.subscribe(name=>{
            if(name){
                this.groupName = name;
            }else{
                this._router.navigate(['/app/main/dashboard']);
            }
        })
    }

    initializeDateRange() {
        this.dateRange = [null, null];
        this.orderDateRange = [null, null];
    }

    initializeDropdownSettings() {
        this.selectedItems = [];

        this.dropdownList = [
            { id: 2, item_text: "Opt In" },
            { id: 0, item_text: "Neutral" },
            { id: 1, item_text: "Opt Out" },
        ];
        this.dropdownSettings = {
            singleSelection: false,
            idField: "id",
            textField: "item_text",

            selectAllText: "Select All",
            unSelectAllText: "UnSelect All",
            itemsShowLimit: 3,
            allowSearchFilter: true,
        };
    }


    PaginatorContacts(event: LazyLoadEvent) {
        if (this.filterContactFlag) {
            this.filterContacts(event);
        }

    }
    getCountryCode(event: any) {
        this.countryObj = event;
        this.countryObj2 = event;
        this.contact.countryCode = Number(event.dialCode).toString();
    }

    hasOnlySpaces(inputString: string): boolean {
        return /^\s*$/.test(inputString);
    }

    handleSubmit(event?: LazyLoadEvent) {
        
        if(this.hasOnlySpaces(this.groupName)){
            this.submitted = false;
        
            this.message.error("", this.l("canotHaveOnlySpace"));
            return
        }  
        if (this.groupName.length === 0) {
            this.submitted = false;
            this.message.error("", this.l("groupNamecantbeEmpty"));
            return;
        }

        this.message.confirm('',this.l('AreYouSure'),
        (isConfirmed) => 
        {
          if (isConfirmed) 
          {
            const groupMembersDto = new GroupMembersDto();
            groupMembersDto.groupDtoModel = new GroupDtoModel();
            groupMembersDto.groupDtoModel.groupName = this.groupName
            groupMembersDto.membersDto = this.selectedContact;
            groupMembersDto.totalCount = 0;

            this.submitted = true;
            this.groupService
            .groupCreateMembers(false, false, groupMembersDto)
            .subscribe(
                (res) => {
                    this.submitted = false;

                    this.groupAddResult = res;
                    this.primengTableHelper.hideLoadingIndicator();
                    if (res.state === 1) {
                        this.message.error(
                            "",
                            this.l("groupNameisUsed")
                        );
                        return;
                    } else if (res.state === 4) {
                        this.message.error(
                            "",
                            this.l("groupNamecantbeempty")
                        );
                        return;
                    } else if (res.state === 3) {
                        this.message.error(
                            "",
                            this.l("invalidTenant")
                        );
                        return;
                    } else if (res.state === 2) {
                        localStorage.setItem('currentGroupId', res.id.toString());
                        this.notify.success("Group creation started.");
                        this.router.navigate(['/app/main/groupcontact'])
                    } else {
                    //     if (res.failedCount === 0) {
                    //         let str =
                    //             res.successCount +
                    //             this.l("succeesfull");

                                
                    // this.router.navigate(['/app/main/groupcontact'])
                    //         this.notify.success(str);
                    //     } else {
                    //         this.showDialog();
                    //     }
                    }

                },
                (error: any) => {
                    if (error) {
                        this.submitted = false;
                        this.primengTableHelper.hideLoadingIndicator();
                        this.notify.error(
                            error.error.error.message
                        );
                    }
                }
            );
    
       
      
          }
        }
      );
     
 

        }


    



    filterContacts(event?: LazyLoadEvent) {

        this.contact.isOpt = this.selectedItems
            .filter((f) => f.id >= 0)
            .map(({ id }) => id)
            .toString();
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }


        if (this.contact.countryCode == null) {
            this.message.error("", this.l("selectCountryCode"));
            this.primengTableHelper.hideLoadingIndicator();
            return;
        } else if (this.selectedItems.length == 0) {
            this.filtered = true;
            this.primengTableHelper.hideLoadingIndicator();
            return;
        } else {
            if (this.dateRange[0] == null || this.dateRange[1] == null) {
                this.contact.joiningFrom = null;
                this.contact.joiningTo = null;
            } else {
                this.contact.joiningFrom = moment(this.dateRange[0]);

                this.contact.joiningTo = moment(this.dateRange[1]);
            }
            this.totalContact = 0;
            this.primengTableHelper.showLoadingIndicator();
            this.groupService
                .membersFilter(
                    this.primengTableHelper.getSkipCount(this.paginator, event),
                    this.primengTableHelper.getMaxResultCount(this.paginator, event),
                    this.contact.contactName, this.contact.countryCode,
                    this.contact.joiningFrom, this.contact.joiningTo, this.contact.isOpt, 0

                )
                .subscribe((result) => {
                    this.primengTableHelper.hideLoadingIndicator();
                    this.primengTableHelper.totalRecordsCount =
                        result.totalCount;
                    this.primengTableHelper.records = result.items;
                });
        }
    }


    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }






    goToDashboard() {
        this._router.navigate(["/app/main/dashboard"]);
    }

    goToCampaign() {
        this._router.navigate(["/app/main/messageCampaign"]);
    }
}
