import { id } from '@swimlane/ngx-charts';
import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { ThemeHelper } from '@app/shared/layout/themes/ThemeHelper';
import { AppComponentBase } from '@shared/common/app-component-base';
import {GroupServiceProxy, MembersDto, GroupMembersDto, GroupDtoModel, GroupCreateDto } from '@shared/service-proxies/service-proxies';
import { CountryISO, PhoneNumberFormat, SearchCountryField } from 'ngx-intl-tel-input';
import * as rtlDetect from 'rtl-detect';
import { contactFilterModel } from '../../group.model';
import { LazyLoadEvent } from 'primeng/api';
import moment from 'moment';
import { Paginator } from "primeng/paginator";
import { Table } from 'primeng/table';
import { Router } from '@angular/router';


@Component({
    selector: 'app-internal-update-group',
    templateUrl: './internal-update-group.component.html',
    styleUrls: ['./internal-update-group.component.css']
})
export class InternalUpdateGroupComponent extends AppComponentBase implements OnInit {

    @Input()
    groupName: string;

    @Input() totalRecords: number;

    @Input()
    groupID: number;
    @Input() isGroupUnsubscribed: boolean = false;

    orderTime1: moment.Moment | undefined;
    orderTime2: moment.Moment | undefined;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: false }) paginator: Paginator;
    isArabic = false;
    theme: string;
    dropdownList = [];
    dropdownSettings = {};
    selectedItems = [];
    selectedContact: MembersDto[] = [];
    contactFlag: boolean = false;
    filterContactFlag: boolean = true;
    externalContactFlag: boolean = false;
    isCount = false;
    dateRange: [Date, Date] = [new Date(), new Date()];
    orderDateRange: [Date, Date] = [new Date(), new Date()];
    submitted = false;
    filtered = false;
    totalContact: number;
    dailyLimit: number = 0;
    countryObj: any = null;
    countryObj2: any = null;
    separateDialCode = false;
    SearchCountryField = SearchCountryField;
    CountryISO = CountryISO;
    PhoneNumberFormat = PhoneNumberFormat;
    groupAddResult: GroupCreateDto = null;
    preferredCountries: CountryISO[] = [
        CountryISO.SaudiArabia,
        CountryISO.Jordan,
    ];
    contact: contactFilterModel = new contactFilterModel();

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




    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private groupService: GroupServiceProxy,
        private router : Router
    ) {
        super(injector);
    }

    visible: boolean = false;

    showDialog() {
        this.visible = true;
    }

    
    ngOnInit() {
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        this.theme = ThemeHelper.getTheme();

        this.initializeDateRange();
        this.initializeDropdownSettings();
        console.log("init")
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
            this.submitted = true;
            this.message.error("", this.l("groupNamecantbeEmpty"));
            return;
        }
      

        // if (this.selectedContact.length <= 1) {
        //     this.message.error("sadadssa", this.l("selectvontact"));
        //     return;
        // }
        this.message.confirm(
            "",
            this.l("areyousureyouwanttoadd") +
                this.selectedContact.length +
                " Contacts ? ",
            (isConfirmed) => {
                if (isConfirmed) {
                    this.submitted = true;
                    this.primengTableHelper.showLoadingIndicator();
            if (this.isGroupUnsubscribed) {
                this.selectedContact = this.selectedContact.map(c =>
                    new MembersDto({
                        id: c.id,
                        phoneNumber: c.phoneNumber,
                        displayName: c.displayName,
                        failedId: c.failedId,
                        isFailed: c.isFailed,
                        variables: c.variables,
                        customeropt: 1
                    })
                );
            }
            const groupMembersDto = new GroupMembersDto();
            groupMembersDto.groupDtoModel = new GroupDtoModel();
            groupMembersDto.groupDtoModel.groupName = this.groupName;
            groupMembersDto.membersDto = this.selectedContact;
            groupMembersDto.totalCount = 0;
            groupMembersDto.groupDtoModel.id =this.groupID;
                    this.groupService
                        .groupUpdate(false,2, groupMembersDto)
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
                                    this.notify.success("Group update started.");
                                    this.router.navigate(['/app/main/groupcontact'])
                                } else {
                                    // if (res.failedCount === 0) {
                                    //     let str =
                                    //         res.successCount + " "+
                                    //         this.l("succeesfull");
                                    //     this.notify.success(str);
                                    // } else {
                                    //     this.showDialog();
                                    // }
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
                } else {
                    this.submitted = false;
                }
            }
        );




    }
    onContactSelect(event: any) {
        if (this.isGroupUnsubscribed) {
            // event.value is the new selectedContact array
            this.selectedContact = event.value.map(c => {
                return new MembersDto({
                    id: c.id,
                    phoneNumber: c.phoneNumber,
                    displayName: c.displayName,
                    failedId: c.failedId,
                    isFailed: c.isFailed,
                    variables: c.variables,
                    customeropt: 1
                });
            });
        }
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
                    this.contact.joiningFrom, this.contact.joiningTo, this.contact.isOpt,
                    this.groupID

                )
                .subscribe((result) => {
                    this.primengTableHelper.hideLoadingIndicator();
                    this.primengTableHelper.totalRecordsCount =
                        result.totalCount;
                    this.primengTableHelper.records = result.items;
                });
        }
    }

    getCountryCode(event: any) {
        this.countryObj = event;
        this.countryObj2 = event;
        this.contact.countryCode = Number(event.dialCode).toString();
    }


    PaginatorContacts(event: LazyLoadEvent) {
        if (this.filterContactFlag) {
            this.filterContacts(event);
        }

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



    initializeDateRange() {
        this.dateRange = [null, null];
        this.orderDateRange = [null, null];
    }




}
