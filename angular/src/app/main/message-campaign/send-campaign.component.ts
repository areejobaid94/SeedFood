import {
    ChangeDetectorRef,
    Component,
    ElementRef,
    Injector,
    ViewChild,
    ViewEncapsulation,
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    AreaDto,
    AreasServiceProxy,
    AssetLevelOneDto,
    AssetLevelThreeDto,
    AssetLevelTwoDto,
    AssetServiceProxy,
    ButtonCopyCodeVariabllesTemplate,
    CampinToQueueDto,
    Card,
    CardsModel,
    CardVariabllesTemplate,
    CarouselVariabllesTemplate,
    ContactsEntity,
    FileParameter,
    FirstButtonURLVariabllesTemplate,
    GetAllDashboard,
    GroupDtoModel,
    GroupServiceProxy,
    HeaderVariablesTemplate,
    ListContactToCampin,
    MessageTemplateModel,
    SecondButtonURLVariabllesTemplate,
    SendCampinStatesModel,
    TemplateVariables,
    TemplateVariablles,
    TenantDashboardServiceProxy,
    WalletModel,
    WhatsAppButtonModel,
    WhatsAppCampaignModel,
    WhatsAppComponentModel,
    WhatsAppContactsDto,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { MessageCampaignService } from "./message-campaign.service";
import { DatePipe, DOCUMENT } from "@angular/common";
import "chartjs-plugin-labels";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { FormsModule } from '@angular/forms'; // ✅ Import FormsModule

import { ConfirmationService, LazyLoadEvent, MenuItem } from "primeng/api";
import {
    SearchCountryField,
    CountryISO,
    PhoneNumberFormat,
} from "ngx-intl-tel-input";
import moment from "moment";
import { PermissionCheckerService } from "abp-ng2-module";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import { ScheduleCampaignComponent } from "./schedule-campaign.component";
import * as rtlDetect from "rtl-detect";
import { MemberDtoEdited } from "./sendtoCompaignToGroup/sendtoCompaignToGroup.component";
import Stepper from "bs-stepper";
import { Observable, Subject } from "rxjs";
import { NgForm } from "@angular/forms";
import { FlatpickrOptions } from "ng2-flatpickr";
import { MainDashboardServiceService } from "../main-dashboard/main-dashboard-service.service";
import { catchError, map } from "rxjs/operators";
import Swal, { SweetAlertOptions, SweetAlertResult } from "sweetalert2";

export interface MemberDtoEditedSend {
    id?: number;
    phoneNumber?: string;
    contactName?: string;
    customerOPT?: string;
    templateVariables: {
        VarOne?: string;
        VarTwo?: string;
        VarThree?: string;
        VarFour?: string;
        VarFive?: string;
        varSix?: string;
        varSeven?: string;
        varEight?: string;
        varNine?: string;
        varTen?: string;
        varEleven?: string;
        varTwelve?: string;
        varThirteen?: string;
        varFourteen?: string;
        varFifteen?: string;

    };
}

// export interface MemberDtoEdited {
//     id?: number;
//     phoneNumber?: string;
//     displayName?: string;
//     isFailed?: boolean;
//     variables: {
//       VarOne?: string;
//       VarTwo?: string;
//       VarThree?: string;
//       VarFour?: string;
//       VarFive?: string;
//     }
//   }

@Component({
    selector: "app-send-campaign",
    templateUrl: "./send-campaign.component.html",
    styleUrls: ["./send-campaign.component.css"],
})
export class SendCampaignComponent extends AppComponentBase {
    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        private route: ActivatedRoute,
        private groupService: GroupServiceProxy,
        private messageCampaignService: MessageCampaignService,
        private router: Router,
        private confirmationService: ConfirmationService,
        private datePipe: DatePipe,
        private _assetServiceProxy: AssetServiceProxy,
        public dasboardService: TenantDashboardServiceProxy,
        private _areasServiceProxy: AreasServiceProxy,
        private _permissionCheckerService: PermissionCheckerService,
        public darkModeService: DarkModeService,
        private _router: Router,
        // private cd: ChangeDetectorRef
    ) {
        super(injector);
        //   this.cdr.detectChanges();

    }



    listOfHeaderVariable: string[] = null;
    VariableCountHeader: number = 0;
    // VariableURL1: number = 0;
    // VariableURL2: number = 0;

    variableURL1: boolean = false;
    variableURL2: boolean = false;
    copyCodeEsxist:     boolean = false;
    firstButtonURLVariabllesTemplate: FirstButtonURLVariabllesTemplate;
    secondButtonURLVariabllesTemplate: SecondButtonURLVariabllesTemplate;
    buttonCopyCodeVariabllesTemplate:ButtonCopyCodeVariabllesTemplate;
    urlExample1: string;
    urlExample2: string;
    copyCodeExample1: string;

    urlLink1:string;
    urlLink2:string;
    copyCode:string;

    URLBtton1:WhatsAppButtonModel;
    URLBtton2:WhatsAppButtonModel;
    URL1isValid:boolean=true;
    URL2isValid:boolean=true;
    CopyCodeisValid:boolean=true;
    headerVaribleValid:boolean=true;

    copyCodeButton:WhatsAppButtonModel;
urlVariables: { 
  [cardIndex: number]: { 
    [buttonIndex: number]: { value: string }[] 
  } 
} = {};

    inputsHeaders: { value: string }[] = [];

     dynamicTextPartsHeader: {
        type: "text" | "input";
        content: string;
        index?: number;
    }[] = [];
        displayHeaderText: string = "";
    inputCountFromBackendHeader: number = 0;
    customeexampleHeader: string[] = [];
    ah:string="";
    category:string="";


   componentCarousel: WhatsAppComponentModel = new WhatsAppComponentModel();
    cards:CardsModel[]=[]
    // cards: Card[] = [];
    carouselVariables: { [cardIndex: number]: { inputs: { value: string }[] } } = {};
    currentCardIndex: number = 0;

    VariableCountCarousel: number[] = []; 
    inputsCarousel: { value: string }[][] = []; 
    dynamicTextPartsCarousel: { type: "text" | "input"; content: string; index?: number; }[][] = []; 
    displayCarouselTexts: string[] = []; 

    Buttons: WhatsAppComponentModel = new WhatsAppComponentModel();





    theme: string;
    imageFlag: boolean = false;
    loading = true;
    currentPageNumber: number = 0;
    currentPageSize: number = 20;
    videoFlag: boolean = false;
    documentFlag: boolean = false;
    Component: WhatsAppComponentModel[] = [new WhatsAppComponentModel()];
    ComponentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButton: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButtonText: any;
    isCampignSending: boolean = false;
    submitted = false;
    isVariablesValidFlag: boolean = false;
    selectAll: boolean = false;
    loadingTable: boolean = false;
    totalRecords: number = 0;
    dateAndTime: Date = new Date();
    isFromPage: boolean = false;
    sentCapgin: boolean = false;
    currentTime = new Date();
    nextBtnClicked: boolean = false;
    isCampignTitleValid: boolean = false;
    hour: number = 1;
    language: string;
    templateId: string;
    templateName: string;
    selectOnce: boolean = false;
    headerValidVariable: boolean = true;

    public timeOptions: FlatpickrOptions = {
        // defaultDate: new Date(this.currentTime.getTime() + 2 * 60000),
        enableTime: true,
        noCalendar: true,
        altInput: true,
        minTime: new Date(Date.now()),
    };

    listOfOpt: any[] = [
        {
            id: 1,
            name: "hassan",
            phoneNumber: "0789123",
        },

        {
            id: 2,
            name: "Mousa",
            phoneNumber: "123213",
        },

        {
            id: 3,
            name: "Ahmad",
            phoneNumber: "08123",
        },
    ];

    public DateOptions: FlatpickrOptions = {
        // defaultDate: new Date(Date.now()),
        altInput: true,
        minDate: new Date(Date.now()),
    };

    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("titleCampign", { static: true }) title: any;
    @ViewChild("paginator", { static: false }) paginator: Paginator;
    @ViewChild("scheduleCampaign", { static: true })
    scheduleCampaign: ScheduleCampaignComponent;

    @ViewChild("templateForm") campaignForm!: NgForm;

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
    customerApiInput$ = new Subject<string>();
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

    customerApiLoading = false;
    dynamicTextParts: {
        type: "text" | "input";
        content: string;
        index?: number;
    }[] = [];
    showMessageLoader: boolean = false;
    saving = false;
    contactFlag: boolean = false;
    filterContactFlag: boolean = false;
    externalContactFlag: boolean = false;
    sendToGroupFlag: boolean = true;
    VariableCount: number = 0;
    fileToUpload: any;
    listofvar: any[] = [];
    tempFile: any;
    extFile: string;
    sendTime: string;
    totalContact: number;
    contact: WhatsAppContactsDto = new WhatsAppContactsDto();
    lstContact: ListContactToCampin[] = [new ListContactToCampin()];
    lstContactWithoutOptOut: string[] = [];
    ContactsEntity: ContactsEntity = new ContactsEntity();
    private horizontalWizardStepper: Stepper;
    private verticalWizardStepper: Stepper;
    private modernWizardStepper: Stepper;
    private modernVerticalWizardStepper: Stepper;
    private bsStepper;

    totalOptOut: number = 0;
    totalaccepted: number = 0;
    campaign: WhatsAppCampaignModel = new WhatsAppCampaignModel();
    template = [];
    templateById: MessageTemplateModel = new MessageTemplateModel();
    languageEnum = [
        { id: "en", name: "English" },
        { id: "ar", name: "Arabic" },
    ];
    dropdownList = [];
    dropdownSettings = {};
    selectedItems = [];

    isCount = false;
    zoomLevel: number = 1;

    customeexampleBody: string[] = [];
    isLoading: boolean = false;
    selectedCustomers: ListContactToCampin[];
    groupById: CampinToQueueDto = new CampinToQueueDto();
    countallowedperday = 0;
    assetLevelOne: AssetLevelOneDto[];
    assetLevelTwo: AssetLevelTwoDto[];
    assetLevelTwoMain: AssetLevelTwoDto[];
    assetLevelThree: AssetLevelThreeDto[];
    assetLevelThreeMain: AssetLevelThreeDto[];
    displayText: string = "";
    displayGlobal: string = "";
    assetLevelOneFlag: boolean = false;
    assetLevelTwoFlag: boolean = false;
    assetLevelThreeFlag: boolean = false;
    listOfVariable: string[] = null;
    area: AreaDto = new AreaDto();
    stepOneNextBtn: boolean = false;
    stepThreeNextBtn: boolean = false;
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
    file: any;
    clonedContacts: { [s: string]: ListContactToCampin } = {};
    isHasPermissionOrder: boolean;
    isHasPermissionAssets: boolean;
    inputs: { value: string }[] = [];
    activeIndex: number = 0;
    items: MenuItem[] | undefined;
    newTemplateId: string;
    newGroupId: string;
    CampignType: string;

    iconTemplateCheck: boolean = false;
    iconFilterationCheck: boolean = false;
    iconDateCheck: boolean = false;
    iconSentCheck: boolean = false;
    timeToSelect: Date;

    campignCost: number = 0.0;
    TotalRemainingAdsConversation: number = 0;
    //totalRate: number = 0;
    updatebtn: boolean = false;
    Measurement: GetAllDashboard = new GetAllDashboard();
    confirm: string = "";
    sendCampaignValidation: boolean = true;
    filteredContacts: CampinToQueueDto;
    customers: ListContactToCampin[] = [];
    orderTime1: moment.Moment | undefined;
    orderTime2: moment.Moment | undefined;
    @ViewChild("file")
    element: ElementRef;
    isArabic = false;
    inputCountFromBackend: number = 0;
    groups = [];
    updateVariablesFlag: boolean = false;
    groupID: number;
    minDate: Date = new Date();
    selectedDateInfo: string;
    dateDropDownSetting = [];
    public alertClose: boolean = false;
    walletAmount$: Observable<WalletModel>;
    isCostValid: boolean = false;

    checkPattern(text: string): boolean {
        const pattern = /\{\{\d+\}\}/g;
        return pattern.test(text);
    }

    getExampleBody(example: string[]) {
        this.customeexampleBody = example;
    }

    initStepper() {
        this.modernWizardStepper = new Stepper(
            document.querySelector("#stepper3Campaign"),
            {
                linear: true,
            }
        );
        this.bsStepper = document.querySelectorAll(".bs-stepper");
    }

    visible: boolean = false;

    showDialog() {
        // console.log(this.groupById?.contacts.length === 0);
        if (this.groupById?.contacts.length === 0) return;
        this.visible = true;
    }

        isValidUrl(urlString: string): boolean {
    try {
            const url = new URL(urlString);

            return /^[\w\-~.%]+$/.test(urlString) && 
                        !/\s/.test(urlString) &&
                    url.protocol === 'http:' || url.protocol === 'https:';
        } catch (e) {
            return false;
        }
    }

    modernHorizontalNext(index: number) {
        debugger;
        this.isFromPage = false;
        let templateHeaderVariables: HeaderVariablesTemplate;

        this.dateAndTime = new Date();
        this.dateAndTime.setMinutes(this.dateAndTime.getMinutes() + 2);

        this.minDate = new Date();
        this.minDate.setMinutes(this.minDate.getMinutes() + 2);

        if (index === 1) {
            if (this.totalaccepted >= 5000) {
                this.hour = Math.ceil(this.totalaccepted / 5000);
            } else {
                this.hour = 1;
            }
            this.isCampignTitleValid = true;
            if (this.groups.length === 0) {
                this.message.error("thereIsNoGroupsFound");
                return;
            }
            if (this.template.length === 0) {
                this.message.error("thereIsNoTemplateFound");
                return;
            }
            this.stepOneNextBtn = true;
            if (this.campaignForm.form.controls["stepOneCampign"].invalid) {
                this.iconTemplateCheck = false;
                return;
            }
            if(this.firstButtonURLVariabllesTemplate){
                if (
                    !this.firstButtonURLVariabllesTemplate?.varOne ||                 
                    this.firstButtonURLVariabllesTemplate.varOne.trim() === '' ||    
                    !this.isValidUrl(this.buildURLButton1())
                ) {
                    this.URL1isValid = false;
                    return;
                }
        }
        if(this.secondButtonURLVariabllesTemplate){
            if (
                this.secondButtonURLVariabllesTemplate.varOne.trim() === '' ||    
                !this.isValidUrl(this.buildURLButton2())
            ) {
                this.URL2isValid = false;
                return;
            }
        }
        if(!this.headerValidVariable){
            return;
        }

        if(this.buttonCopyCodeVariabllesTemplate){
              if (
                this.buttonCopyCodeVariabllesTemplate.varOne.trim() === ''||
                /\s/.test(this.buttonCopyCodeVariabllesTemplate.varOne)  
            ) {
                this.CopyCodeisValid = false;
                return;
            }
        }

        if (this.VariableCountHeader > 0) {
            for (const item of this.inputsHeaders) {
                if (item.value.length === 0){
                    this.headerVaribleValid=false;
                    return;
                }
            }
            let arr = this.dynamicTextPartsHeader.filter(
                (item) => item.type == "input"
            );
    
            templateHeaderVariables = new HeaderVariablesTemplate({
                varOne: arr[0]?.content || null,
            });
        } else {
            templateHeaderVariables = null;
        }

            if (this.componentCarousel) {
                const cards: Card[] = [];
                let hasCarouselVariables = false;
                let carouselTemplate: CarouselVariabllesTemplate | null = null;

                for (let i = 0; i < this.cards.length; i++) {
                    const card = this.cards[i];
                
                    const buttonsComponent = card.components.find(c => c.type === "BUTTONS");
                    const buttons = buttonsComponent?.buttons || [];
                    const buttonVariables: any = {};
                
                    for (let buttonIndex = 0; buttonIndex < buttons.length; buttonIndex++) {
                        const button = buttons[buttonIndex];
                    
                        if (button.type === 'URL' && this.urlVariables[i]?.[buttonIndex]) {
                            const inputs = this.urlVariables[i][buttonIndex];
                        
                            for (const input of inputs) {
                                if (!input.value) {
                                    this.notify.warn("Please fill all URL variables");
                                    return;
                                }
                            }
                        
                            buttonVariables[`button${buttonIndex + 1}`] = {
                                varOne: inputs[0]?.value || null,
                            };
                        
                            hasCarouselVariables = true;
                        }
                    }
                
                    const bodyComponent = card.components.find(c => c.type === "BODY");
                    let cardVariables: CardVariabllesTemplate | null = null;
                
                    if (this.VariableCountCarousel[i] > 0) {
                        for (const item of this.inputsCarousel[i]) {
                            if (!item.value) {
                                this.notify.warn("Please fill all body variables");
                                return;
                            }
                        }
                    
                        const arr = this.dynamicTextPartsCarousel[i].filter(item => item.type === "input");
                    
                        cardVariables = new CardVariabllesTemplate({
                            varOne: arr[0]?.content || null,
                            varTwo: arr[1]?.content || null,
                            varThree: arr[2]?.content || null,
                            varFour: arr[3]?.content || null,
                            varFive: arr[4]?.content || null,
                            varSix: arr[5]?.content || null,
                            varSeven: arr[6]?.content || null,
                            varEight: arr[7]?.content || null,
                            varNine: arr[8]?.content || null,
                            varTen: arr[9]?.content || null,
                            varEleven: arr[10]?.content || null,
                            varTwelve: arr[11]?.content || null,
                            varThirteen: arr[12]?.content || null,
                            varFourteen: arr[13]?.content || null,
                            varFifteen: arr[14]?.content || null,
                        });
                    
                        hasCarouselVariables = true;
                    }
                
                    let firstButtonVars: FirstButtonURLVariabllesTemplate | null = null;
                    let secondButtonVars: SecondButtonURLVariabllesTemplate | null = null;
                
                    const urlButtons = buttons.filter(b => b.type === "URL");
                
                    if (urlButtons[0]?.example) {
                        firstButtonVars = new FirstButtonURLVariabllesTemplate({
                            varOne: this.urlVariables[i]?.[0]?.[0]?.value ?? null,
                        });
                    }
                
                    if (urlButtons[1]?.example) {
                        secondButtonVars = new SecondButtonURLVariabllesTemplate({
                            varOne: this.urlVariables[i]?.[1]?.[0]?.value ?? null,
                        });
                    }
                
                    cards.push(new Card({
                        variables: cardVariables,
                        firstButtonURLVariabllesTemplate: firstButtonVars,
                        secondButtonURLVariabllesTemplate: secondButtonVars,
                        cardIndex: i,
                        variableCount: this.VariableCountCarousel[i]
                    }));
                }
            
                if (hasCarouselVariables) {
                    carouselTemplate = new CarouselVariabllesTemplate({
                        cards: cards
                    });
                }
            }
            this.nextBtnClicked = true;
            this._whatsAppMessageTemplateServiceProxy
                .titleCompaignCheck(
                    this.campaignForm.form.value.stepOneCampign.titleCampign
                )
                .subscribe({
                    next: (result) => {
                        this.nextBtnClicked = false;
                        if (!result.isSuccess) {
                            this.iconTemplateCheck = false;
                            this.isCampignTitleValid = false;
                            this.message.error(this.l("campignTitleIsUsed"));
                            return;
                        } else {
                            this.iconTemplateCheck = true;
                            this.modernWizardStepper.next();
                        }
                    },
                    error: () => {
                        this.nextBtnClicked = false;
                        this.message.error(
                            this.l("ErrorHappenedOnSavingModal")
                        );
                    },
                });
            this.getCampaignDailyLimit();
        }
        if (index == 2) {
            // get req here!
            this.iconFilterationCheck = true;
            this.modernWizardStepper.next();
        }

        if (index === 3) {
            this.stepThreeNextBtn = true;
            if (this.campaignForm.form.controls["stepthreeCampign"].invalid) {
                this.iconDateCheck = false;
                return;
            } else {
                this.iconDateCheck = true;
                this.modernWizardStepper.next();
            }
        }
    }

    handleSelectChange(selectedItmem: any) {
        if (selectedItmem && selectedItmem.isStatic) {
            this.router.navigate(["app/main/groupcontact"]);
            return;
        }

        this.groupID = selectedItmem.id;
        this.getGroupById();
    }

    onNextTemplate(form: NgForm) {
        if (form.valid) {
        }
    }

    zoomIn() {
        if (!this.templateById.localTemplateId) return;

        if (this.zoomLevel < 1.3) {
            // Limit minimum zoom level
            this.zoomLevel += 0.1; // Increase zoom level
        }
    }

    zoomOut() {
        if (!this.templateById.localTemplateId) return;
        if (this.zoomLevel > 0.2) {
            // Limit minimum zoom level
            this.zoomLevel -= 0.1; // Decrease zoom level
        }
    }

    getGroupById(event?: LazyLoadEvent) {
        if (!this.groupID) return;
        this.loadingTable = true;
        this.isVariablesValidFlag = false;
        this.walletAmount$ = this.dasboardService.walletGetByTenantId(
            this.appSession.tenantId
        );
        this._whatsAppMessageTemplateServiceProxy
            .groupGetByIdForCampign(this.groupID)
            .subscribe(
                (result: CampinToQueueDto) => {
                    this.groupById = result;
                    this.totalaccepted = result.totalCount - result.totalOptOut;
                    this.totalOptOut = result.totalOptOut;
                    this.loadingTable = false;
                    this.totalRecords = result.contacts.length;
                    this.customers = result.contacts;
                    this.campignCost =
                        (result.totalCount - result.totalOptOut) * 0.014;
                    this.walletAmount$.subscribe((res) => {
                        this.isCostValid =
                            this.campignCost < res.totalAmountSAR;
                    });
                    this.selectedCustomers = this.customers;
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

    /**
     * Modern Horizontal Wizard Stepper Previous
     */

    /**
     * Modern Vertical Wizard Stepper Previous
     */
    modernVerticalPrevious() {
        this.modernVerticalWizardStepper.previous();
    }

    horizontalWizardStepperPrevious() {
        this.horizontalWizardStepper.previous();
    }

    modernHorizontalPrevious() {
        this.isFromPage = false;
        this.modernWizardStepper.previous();
    }
    /**
     * Modern Vertical Wizard Stepper Next
     */
    modernVerticalNext() {
        this.modernVerticalWizardStepper.next();
    }

    trackByFn(item: any) {
        return item.contactId;
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
                .pipe(
                    map(({ groupDtoModel }) =>
                        groupDtoModel.filter(
                            (group) =>
                                group.totalNumber > 0 && group.onHoldCount === 0
                        )
                    ),
                    catchError((err) => {
                        this.message.error("Error loading more items:", err);
                        return [];
                    })
                )
                .subscribe((filteredGroups) => {
                    this.groups = [...this.groups, ...filteredGroups];
                    this.loading = false;
                });
        }
    }

    getGroupAll() {
        this.groupService
            .groupGetAll("", 0, 1000)
            .pipe(
                map(({ groupDtoModel }) =>
                    groupDtoModel.filter(
                        (group) =>
                            group.totalNumber > 0 && group.onHoldCount === 0
                    )
                )
            )
            .subscribe((filteredGroups) => {
                this.groups = filteredGroups;
                this.groups.push({
                    id: "static",
                    groupName: "+Create Group",
                    totalNumber: null,
                    isStatic: true,
                });
            });
    }

    onKeyPress(event: KeyboardEvent) {
        const forbiddenKey = ["$"];
        const keyPressed = event.key;
        if (forbiddenKey.includes(keyPressed)) {
            event.preventDefault();
        }
    }

    onRowEditInit(contact: ListContactToCampin) {
        this.clonedContacts[contact.id] = null;
        // this.clonedContacts[contact.id] = { ...contact };
    }

    /**
     * format bytes
     * @param bytes (File size in bytes)
     * @param decimals (Decimals point)
     */
    formatBytes(bytes, decimals) {
        if (bytes === 0) {
            return "0 Bytes";
        }
        const k = 1024;
        const dm = decimals <= 0 ? 0 : decimals || 2;
        const sizes = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return (
            parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i]
        );
    }

    onActiveIndexChange(event: number) {
        this.activeIndex = event;
    }

    onRowEditSave(contact: MemberDtoEdited) {
        delete this.clonedContacts[contact.id];
        this.notify.success("sucess");
    }

    onRowEditCancel(contact: MemberDtoEdited, index: number) {
        this.customers[index] = this.clonedContacts[contact.id];
        delete this.customers[contact.id];
    }

    initDropDownListForDate() {
        this.dateDropDownSetting = [
            {
                id: 1,
                name: "Send Now",
                color: "#28c76f",
                background: "#28c76f1e",
            },
            // {
            //     id: 2,
            //     name: "Schedule",
            //     color: "#8b5cf6",
            //     background: "#ede9fe",
            // },
        ];
    }

    preventDefalut(event) {
        event.preventDefalut();
    }
    ngOnInit(): void {
        // this.inputsCarousel = [...this.inputsCarousel];
        this.route.queryParams.subscribe((params) => {
            this.language = params["language"];
            this.templateId = params["templateId"];
            this.templateName = params["templateName"];
            // Now you can use these variables as needed
        });
  this.inputsCarousel = [
      [{ value: '' }, { value: '' }],
      [{ value: '' }, { value: '' }]
    ];
        this.assetLevelTwoFlag = false;
        this.assetLevelThreeFlag = false;
        this.isFromPage = false;
        this.initStepper();
        this.groupById.contacts = [];
        this.updatebtn = false;
        this.initializeDateRange();
        this.initializeDropdownSettings();

        // wallet not in the first page
        // deleted
        // this.getBranches();
        // this.GetStatistics();

        // this.getCampaignDailyLimit();
        // this.getTemplateById();
        // this.walletAmount$ = this.dasboardService.walletGetByTenantId(
        //     this.appSession.tenantId
        // );

        // if (this.isHasPermissionAssets) {
        //     this.loadLevels();
        // }

        this.initDropDownListForDate();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.theme = ThemeHelper.getTheme();
        this.isHasPermissionOrder =
            this._permissionCheckerService.isGranted("Pages.Orders");
        this.isHasPermissionAssets =
            this._permissionCheckerService.isGranted("Pages.Assets");
        if (this.route.snapshot.queryParams["id"] != null) {
            this.campaign.id = this.route.snapshot.queryParams["id"];
            this.contactFlag = true;
        }
        if (this.route.snapshot.queryParams["title"] != null) {
            this.campaign.title = this.route.snapshot.queryParams["title"];
        }
        if (this.route.snapshot.queryParams["language"] != null) {
            this.campaign.language =
                this.route.snapshot.queryParams["language"];
        }
        if (this.route.snapshot.queryParams["templateId"] != null) {
            this.campaign.templateId =
                this.route.snapshot.queryParams["templateId"];
        }

        //needed!
        this.getTemplates();
        this.getGroupAll();

        this.items = [
            {
                label: "Personal",
                routerLink: "personal",
            },
            {
                label: "Seat",
                routerLink: "seat",
            },
            {
                label: "Payment",
                routerLink: "payment",
            },
            {
                label: "Confirmation",
                routerLink: "confirmation",
            },
        ];
    }

    GetStatistics() {
        this.primengTableHelper.showLoadingIndicator();

        this._whatsAppMessageTemplateServiceProxy
            .getStatistics(null)
            .subscribe((result) => {
                this.Measurement = result;

                this.TotalRemainingAdsConversation =
                    //this.Measurement.remainingFreeConversation +
                    this.Measurement.remainingBIConversation;
            });
    }
    getSendCampaignValidation() {
        this._whatsAppMessageTemplateServiceProxy
            .sendCampaignValidation()
            .subscribe((result) => {
                this.sendCampaignValidation = result;
            });
    }
    initializeDateRange() {
        this.dateRange = [null, null];
        this.orderDateRange = [null, null];
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
                        case 5:
                            this.customers[i].templateVariables["varSix"] =
                                this.inputs[j].value;
                            break;
                        case 6:
                            this.customers[i].templateVariables["varSeven"] =
                                this.inputs[j].value;
                            break;
                        case 7:
                            this.customers[i].templateVariables["varEight"] =
                                this.inputs[j].value;
                            break;
                        case 8:
                            this.customers[i].templateVariables["varNine"] =
                                this.inputs[j].value;
                            break;
                        case 9:
                            this.customers[i].templateVariables["varTen"] =
                                this.inputs[j].value;
                            break;
                        case 10:
                            this.customers[i].templateVariables["varEleven"] =
                                this.inputs[j].value;
                            break;
                        case 11:
                            this.customers[i].templateVariables["varTwelve"] =
                                this.inputs[j].value;
                            break;
                        case 12:
                            this.customers[i].templateVariables["varThirteen"] =
                                this.inputs[j].value;
                            break;
                        case 13:
                            this.customers[i].templateVariables["varFourteen"] =
                                this.inputs[j].value;
                            break;
                        case 14:
                            this.customers[i].templateVariables["varFifteen"] =
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
    initializeDropdownSettings() {
        this.selectedItems = [];

        this.dropdownList = [
            { id: 2, item_text: "Opt In" },
            { id: 1, item_text: "Opt Out" },
            { id: 0, item_text: "Neutral" },
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

    createCampaign() {
        this.saving = true;
        if (
            (this.campaign.title === null &&
                this.campaign.title === undefined) ||
            this.campaign.title === "" ||
            this.campaign.language === null ||
            this.campaign.language === undefined ||
            this.campaign.language === "" ||
            this.campaign.templateId === null ||
            this.campaign.templateId === undefined
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        this.showMessageLoader = true;
        debugger;
        this._whatsAppMessageTemplateServiceProxy
            .addWhatsAppCampaign(this.campaign)
            .subscribe((result) => {
                this.notify.success(this.l("Successfully Created"));
                if (result.valueOf()) {
                    this.contactFlag = true;
                    this.campaign.id = result;
                    this.submitted = true;
                    this.saving = false;
                }
                // this.getTemplateById();
            });
        this.showMessageLoader = false;
    }

    PaginatorContacts(event: LazyLoadEvent) {
        if (this.filterContactFlag) {
            this.filterContacts(event);
        }
        if (this.externalContactFlag) {
            // this.showExternalContacts(event);
        }
    }

    goTowalletPage() {
        this.isFromPage = true;
        this.router.navigate(["/app/main/dashboard"]);
    }
    goToCampignPage() {
        this.isFromPage = true;
        this.router.navigate(["/app/main/messageCampaign"]);
    }

    sendCampaign(form: NgForm = null) {
        
        let templateVariables: TemplateVariablles;
        let templateHeaderVariables: HeaderVariablesTemplate;
        let carouselTemplate: CarouselVariabllesTemplate | null = null;


        if (this.componentCarousel) {
            const cards: Card[] = [];
            let hasCarouselVariables = false;
        
            for (let i = 0; i < this.cards.length; i++) {
                const card = this.cards[i];
        
                const buttonsComponent = card.components.find(c => c.type === "BUTTONS");
                const buttons = buttonsComponent?.buttons || [];
                const buttonVariables: any = {};
        
                for (let buttonIndex = 0; buttonIndex < buttons.length; buttonIndex++) {
                    const button = buttons[buttonIndex];
        
                    if (button.type === 'URL' && this.urlVariables[i]?.[buttonIndex]) {
                        const inputs = this.urlVariables[i][buttonIndex];
        
                        for (const input of inputs) {
                            if (!input.value) {
                                this.notify.warn("Please fill all URL variables");
                                return;
                            }
                        }
        
                        buttonVariables[`button${buttonIndex + 1}`] = {
                            varOne: inputs[0]?.value || null,
                        };
        
                        hasCarouselVariables = true;
                    }
                }
        
                const bodyComponent = card.components.find(c => c.type === "BODY");
                let cardVariables: CardVariabllesTemplate | null = null;
        
                if (this.VariableCountCarousel[i] > 0) {
                    for (const item of this.inputsCarousel[i]) {
                        if (!item.value) {
                            this.notify.warn("Please fill all body variables");
                            return;
                        }
                    }
        
                    const arr = this.dynamicTextPartsCarousel[i].filter(item => item.type === "input");
        
                    cardVariables = new CardVariabllesTemplate({
                        varOne: arr[0]?.content || null,
                        varTwo: arr[1]?.content || null,
                        varThree: arr[2]?.content || null,
                        varFour: arr[3]?.content || null,
                        varFive: arr[4]?.content || null,
                        varSix: arr[5]?.content || null,
                        varSeven: arr[6]?.content || null,
                        varEight: arr[7]?.content || null,
                        varNine: arr[8]?.content || null,
                        varTen: arr[9]?.content || null,
                        varEleven: arr[10]?.content || null,
                        varTwelve: arr[11]?.content || null,
                        varThirteen: arr[12]?.content || null,
                        varFourteen: arr[13]?.content || null,
                        varFifteen: arr[14]?.content || null,
                    });
        
                    hasCarouselVariables = true;
                }
        
                let firstButtonVars: FirstButtonURLVariabllesTemplate | null = null;
                let secondButtonVars: SecondButtonURLVariabllesTemplate | null = null;
        
                const urlButtons = buttons.filter(b => b.type === "URL");
        
                if (urlButtons[0]?.example) {
                    firstButtonVars = new FirstButtonURLVariabllesTemplate({
                        varOne: this.urlVariables[i]?.[0]?.[0]?.value ?? null,
                    });
                }
        
                if (urlButtons[1]?.example) {
                    secondButtonVars = new SecondButtonURLVariabllesTemplate({
                        varOne: this.urlVariables[i]?.[1]?.[0]?.value ?? null,
                    });
                }
        
                cards.push(new Card({
                    variables: cardVariables,
                    firstButtonURLVariabllesTemplate: firstButtonVars,
                    secondButtonURLVariabllesTemplate: secondButtonVars,
                    cardIndex: i,
                    variableCount: this.VariableCountCarousel[i]
                }));
            }
        
            if (hasCarouselVariables) {
                carouselTemplate = new CarouselVariabllesTemplate({
                    cards: cards
                });
            }
        }
        

            if (this.copyCodeEsxist && this.buttonCopyCodeVariabllesTemplate.varOne.length==0) {
                    return;
            }

            if(this.ComponentHeader?.example?.header_text){
                this.VariableCountHeader= this.ComponentHeader.example.header_text?.length
                this.inputCountFromBackendHeader = this.VariableCountHeader;
            }else{
                this.VariableCountHeader=0;
                this.inputCountFromBackendHeader = 0;
            }
        if (this.VariableCountHeader > 0) {
            for (const item of this.inputsHeaders) {
                if (item.value.length === 0) return;
            }
            let arr = this.dynamicTextPartsHeader.filter(
                (item) => item.type == "input"
            );

            templateHeaderVariables = new HeaderVariablesTemplate({
                varOne: arr[0]?.content || null,
            });
        } else {
            templateHeaderVariables = null;
        }

                this.buttonCopyCodeVariabllesTemplate;

        // if (this.VariableCount > 0) {
        //     for (const item of this.inputs) {
        //         if (item.value.length === 0) return;
        //     }
        //     let arr = this.dynamicTextParts.filter(
        //         (item) => item.type == "input"
        //     );

        //     templateVariables = new TemplateVariablles({
        //         varOne: arr[0]?.content || null,
        //         varTwo: arr[1]?.content || null,
        //         varThree: arr[2]?.content || null,
        //         varFour: arr[3]?.content || null,
        //         varFive: arr[4]?.content || null,
        //         varSix: arr[5]?.content || null,
        //         varSeven: arr[6]?.content || null,
        //         varEight: arr[7]?.content || null,
        //         varNine: arr[8]?.content || null,
        //         varTen: arr[9]?.content || null,
        //         varEleven: arr[10]?.content || null,
        //         varTwelve: arr[11]?.content || null,
        //         varThirteen: arr[12]?.content || null,
        //         varFourteen: arr[13]?.content || null,
        //         varFifteen: arr[14]?.content || null, 
        //     });
        // } else {
        //     templateVariables = null;
        // }


        debugger;
        this.sentCapgin = true;
        let sendTime: string = null;

        const campinToQueueDto = new CampinToQueueDto();
        campinToQueueDto.templateLanguage = this.templateById.language;
        campinToQueueDto.campaignId = 0;
        campinToQueueDto.templateId =
            this.campaignForm.form.value.stepOneCampign.selectedTemplate.localTemplateId;
        campinToQueueDto.campaignName =
            this.campaignForm.form.value.stepOneCampign.titleCampign;
        campinToQueueDto.templateName =
            this.campaignForm.form.value.stepOneCampign.selectedTemplate.name;
        campinToQueueDto.isExternal = false;
        campinToQueueDto.totalCount =
            this.groupById.totalCount - this.groupById.totalOptOut;
        campinToQueueDto.groupId =
            this.campaignForm.form.value.stepOneCampign.selectedGroup.id;
        campinToQueueDto.totalOptOut = this.groupById.totalOptOut;
        campinToQueueDto.contacts = this.groupById.contacts;
        campinToQueueDto.carouselTemplate=carouselTemplate;
        campinToQueueDto.firstButtonURLVariabllesTemplate=this.firstButtonURLVariabllesTemplate;
        campinToQueueDto.secondButtonURLVariabllesTemplate=this.secondButtonURLVariabllesTemplate;
        campinToQueueDto.headerVariabllesTemplate=templateHeaderVariables;
        campinToQueueDto.buttonCopyCodeVariabllesTemplate=this.buttonCopyCodeVariabllesTemplate;

        campinToQueueDto.templateVariables = new TemplateVariablles();

        if (this.templateById.variableCount > 0) {
            campinToQueueDto.templateVariables = new TemplateVariablles();
            campinToQueueDto.templateVariables["varOne"] =
                this.campaignForm.form.value.stepOneCampign["var1"] || null;
            campinToQueueDto.templateVariables["varTwo"] =
                this.campaignForm.form.value.stepOneCampign["var2"] || null;
            campinToQueueDto.templateVariables["varThree"] =
                this.campaignForm.form.value.stepOneCampign["var3"] || null;
            campinToQueueDto.templateVariables["varFour"] =
                this.campaignForm.form.value.stepOneCampign["var4"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["var5"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["varSix"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["varEight"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["varNine"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["varEleven"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["varEleven"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["varThirteen"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["varFourteen"] || null;
            campinToQueueDto.templateVariables["varFive"] =
                this.campaignForm.form.value.stepOneCampign["varFifteen"] || null;
        } else {
            campinToQueueDto.templateVariables = null;
        }
        if (this.VariableCountHeader > 0) {
            campinToQueueDto.headerVariabllesTemplate = new HeaderVariablesTemplate();
            campinToQueueDto.headerVariabllesTemplate =templateHeaderVariables;
        } else {
            campinToQueueDto.headerVariabllesTemplate = null;
        }
        if (
            this.campaignForm.form.value.stepthreeCampign.selectedDateInfo == 2
        ) {
            sendTime = this.datePipe
                .transform(
                    this.campaignForm.form.value.stepthreeCampign.dateAndTime,
                    "yyyy-MM-dd HH:mm"
                )
                .toString();
        }

        if (!this.isCostValid) {
            this.message.confirm(
                this.l("noenoughFundsinWallet"),
                "",
                (isConfirmed) => {
                    if (isConfirmed) {
                        this.isFromPage = true;
                        this.router.navigate(["/app/main/dashboard"]);
                    } else {
                        this.isFromPage = false;
                    }
                },
                { confirmButtonText: "goToMyWallet" }
            );
            return;
        }

        this._whatsAppMessageTemplateServiceProxy
            .sendCampaignValidation()
            .subscribe((validation) => {
                debugger;
                if (validation) {
                    if (this.totalContact - this.totalOptOut == 0) {
                        this.message.error(this.l("cantSendCampaignToOptOut"));
                        this.primengTableHelper.hideLoadingIndicator();
                        return;
                    }
                    // if (
                    //     this.totalContact >
                    //     Math.ceil(this.TotalRemainingAdsConversation) ||
                    //     Math.ceil(this.TotalRemainingAdsConversation) == 0
                    // ) {
                    //     this.message.error(
                    //          this.l('dontHaveEnoughBunddle')
                    //     );
                    //     this.primengTableHelper.hideLoadingIndicator();
                    //     return;
                    // }

                    // if (this.Measurement.remainingBIConversation == 0) {
                    //     this.confirm =
                    //         "Your Marketing/Utility Bundle Is ZERO ,Are You Sure To Deducted " +
                    //         this.totalContact +
                    //         " From Conversation Bundle ?";
                    // } else {
                    //     this.confirm =
                    //         "Are You Sure To Send Ads To " +
                    //         this.totalContact +
                    //         " Contacts ?";
                    // }
                    debugger;
                    this.confirm =
                        "Are You Sure To Send Ads To " +
                        (this.groupById.totalCount -
                            this.groupById.totalOptOut) +
                        " Contacts ?";

                    this.message.confirm(
                        "",
                        this.l(this.confirm),
                        (isConfirmed) => {
                            if (isConfirmed) {
                                this.isCampignSending = true;
                                this._whatsAppMessageTemplateServiceProxy
                                    .sendCampaignNew(sendTime, campinToQueueDto)
                                    .subscribe(
                                        (resultt: SendCampinStatesModel) => {
                                            let timerInterval: number;

                                            const options: SweetAlertOptions = {
                                                title: "Auto close alert!",
                                                html: "I will close in <b></b> milliseconds.",
                                                timer: 10000,
                                                timerProgressBar: true,
                                                didOpen: () => {
                                                    Swal.showLoading();
                                                    const timer =
                                                        Swal.getHtmlContainer()?.querySelector(
                                                            "b"
                                                        );
                                                    timerInterval =
                                                        window.setInterval(
                                                            () => {
                                                                if (timer) {
                                                                    timer.textContent = `${Swal.getTimerLeft()}`;
                                                                }
                                                            },
                                                            100
                                                        );
                                                },
                                                willClose: () => {
                                                    clearInterval(
                                                        timerInterval
                                                    );
                                                },
                                            };

                                            Swal.fire(options).then(
                                                (result: SweetAlertResult) => {
                                                    if (
                                                        result.dismiss ===
                                                        Swal.DismissReason.timer
                                                    ) {
                                                        this.isCampignSending =
                                                            false;
                                                        if (resultt.status) {
                                                            this.iconSentCheck =
                                                                true;
                                                            this.modernWizardStepper.next();
                                                        } else {
                                                            this.notify.error(
                                                                resultt.message
                                                            );
                                                        }
                                                    }
                                                }
                                            );
                                        }
                                    );
                            }
                        }
                    );
                } else {
                    this.message.error(
                        "",
                        this.l("You have campaign in under process")
                    );
                    this.primengTableHelper.hideLoadingIndicator();
                }
            });
    }
    // sendCampaign() {
    //     // this._whatsAppMessageTemplateServiceProxy
    //     //     .getContactRate(this.lstContactWithoutOptOut)
    //     //     .subscribe((result) => {
    //     //         this.totalRate = result;
    //     //     });
    //     this._whatsAppMessageTemplateServiceProxy
    //         .sendCampaignValidation()
    //         .subscribe((validation) => {
    //             if (validation) {
    //                 if (
    //                     this.primengTableHelper.totalRecordsCount -
    //                     this.totalOptOut ==
    //                     0
    //                 ) {
    //                     this.message.error(
    //                          this.l('cantSendCampaignToOptOut')
    //                     );
    //                     this.primengTableHelper.hideLoadingIndicator();
    //                     return;
    //                 }
    //                 if (
    //                     this.totalContact >
    //                     Math.ceil(this.TotalRemainingAdsConversation) ||
    //                     Math.ceil(this.TotalRemainingAdsConversation) == 0
    //                 ) {
    //                     this.message.error(
    //                          this.l('dontHaveEnoughBunddle')
    //                     );
    //                     this.primengTableHelper.hideLoadingIndicator();
    //                     return;
    //                 }

    //                 if (this.Measurement.remainingBIConversation == 0) {
    //                     this.confirm =
    //                         "Your Marketing/Utility Bundle Is ZERO ,Are You Sure To Deducted " +
    //                         this.totalContact +
    //                         " From Conversation Bundle ?";
    //                 } else {
    //                     this.confirm =
    //                         "Are You Sure To Send Ads To " +
    //                         this.totalContact +
    //                         " Contacts ?";
    //                 }
    //                 this.message.confirm(
    //                     "",
    //                     this.l(this.confirm),
    //                     (isConfirmed) => {
    //                         if (isConfirmed) {
    //                             document
    //                                 .getElementById("btnSend")
    //                                 .setAttribute("disabled", "");
    //                                 if(this.contact.orderTimeFrom != null){
    //                                                                         let orderStart= moment(this.contact.orderTimeFrom);
    //                                                                         this.contact.orderTimeFrom = orderStart;
    //                                                                     }
    //                                                                     if(this.contact.orderTimeTo != null){
    //                                                                         let orderEnd= moment(this.contact.orderTimeTo);
    //                                                                         this.contact.orderTimeTo = orderEnd;
    //                                                                     }
    //                             this._whatsAppMessageTemplateServiceProxy
    //                                 .sendMessageTemplate(
    //                                     this.campaign.templateId,
    //                                     this.campaign.id,
    //                                     this.filterContactFlag,
    //                                     this.contact
    //                                 )
    //                                 .subscribe((result) => {
    //                                     if (result == 1) {
    //                                         this.message.success(
    //                                             "",
    //                                             this.l(
    //                                                 "campaignProceedShortly"
    //                                             )
    //                                         );
    //                                         this.router.navigate(["/app/main/messageCampaign"]);
    //                                     } else {
    //                                         this.notify.error(
    //                                             this.l("sendFailed")
    //                                         );
    //                                     }
    //                                 });
    //                         }
    //                     }
    //                 );
    //             } else {
    //                 this.message.error(
    //                     "",
    //                     this.l("You have campaign in under process")
    //                 );
    //                 this.primengTableHelper.hideLoadingIndicator();
    //             }
    //         });
    // }
    // addScheduledCampaign() {
    //     this.message.confirm(
    //         "",
    //         this.l(
    //             "Are You Sure To Scheduled Campaign At" + this.sendTime + "?"
    //         ),
    //         (isConfirmed) => {
    //             if (isConfirmed) {
    //                 this._whatsAppMessageTemplateServiceProxy
    //                     .addScheduledCampaign(
    //                         this.sendTime,
    //                         this.campaign.id,
    //                         this.campaign.templateId,
    //                         this.externalContactFlag,
    //                         this.contact
    //                     )
    //                     .subscribe((result) => {
    //                         if (typeof (result == 1)) {
    //                             this.notify.success(
    //                                 this.l("Saved Successfully")
    //                             );
    //                             this.router.navigate([
    //                                 "/app/main/messageCampaign",
    //                             ]);
    //                         } else {
    //                             this.notify.error(this.l("savedFailed"));
    //                         }
    //                     });
    //             }
    //         }
    //     );
    // }
    getTemplates() {
        this.template = [];
        this.isLoading = true;
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppTemplateForCampaign(0, 50, this.appSession.tenantId)
            .subscribe((result) => {
                this.template = result.lstWhatsAppTemplateModel.filter(
                    (element) =>
                        element.language == "ar" || element.language == "en"
                );
                this.template.push({
                    id: "static",
                    name: "+Create Template",
                    isStatic: true,
                });
                // this.primengTableHelper.hideLoadingIndicator();
                this.isLoading = false;
            });
    }
    onSelectionChange(value = []) {
        this.selectAll = value.length === this.totalContact;
        this.selectedCustomers = value;
    }

    getTemplateById(event) {
        if (event && event.isStatic) {
            this.router.navigate(["app/main/messageTemplate"]);
            return;
        }

        this.VariableCount = 0;
        this.VariableCountHeader = 0;
        this.dynamicTextPartsHeader = [];
        this.displayHeaderText = "";
        this.listOfHeaderVariable = null;


        this.buttonCopyCodeVariabllesTemplate=null;
        this.secondButtonURLVariabllesTemplate=null
        this.firstButtonURLVariabllesTemplate=null;
        this.variableURL2=false;
        this.variableURL1=false;
        this.copyCodeEsxist=false;


        
        this.displayText = "";
        this.dynamicTextParts = [];
        this.listOfVariable = null;
        this.VariableCount = 0;
        this.listOfVariable = null;

    
        this.Component = [new WhatsAppComponentModel()];
        this.ComponentHeader = new WhatsAppComponentModel();
        this.ComponentBody = new WhatsAppComponentModel();
        this.ComponentFooter = new WhatsAppComponentModel();
        this.ComponentButton = new WhatsAppComponentModel();
        this.ComponentButtonText = null;
        this.templateById = new MessageTemplateModel();
        this.ComponentBody = null;
        this.ComponentButton = null;
        this.ComponentFooter = null;
        this.ComponentHeader = null;
        this.ComponentButtonText = null;
        this.componentCarousel=null;
        let id = event?.localTemplateId || null;
        if (!id) return;
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateById(id)
            .subscribe((result) => {
                debugger;
                this.templateById = result;

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
                    if (e.type == "componentCarousel") {
                        this.componentCarousel = e;
                    }
                    
                });
                    this.ComponentBody = this.templateById.components.find(
                        (i: { type: string }) => i.type === "BODY"
                    )||null;
                    this.ComponentHeader = this.templateById.components.find(
                            (i: { type: string }) => i.type === "HEADER"
                        )||null;
                            //this.componentBody
                    if (this.ComponentBody?.example?.body_text?.[0]) {
                        const firstArray = this.ComponentBody.example.body_text[0];
                        this.VariableCount = Array.isArray(firstArray) ? firstArray.length : 0;
                    } else {
                        this.VariableCount = 0;
                    }
                if (this.VariableCount > 0) {
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
                }
                this.inputCountFromBackend = this.VariableCount;
                // this.listOfVariable = this.templateById.variableCount;
                debugger
                if(this.ComponentHeader){
                if (this.ComponentHeader?.format == "IMAGE") {
                    this.imageFlag = true;
                } else {
                    this.imageFlag = false;
                }
                if (this.ComponentHeader?.format == "VIDEO") {
                    this.videoFlag = true;
                } else {
                    this.videoFlag = false;
                }
                if (this.ComponentHeader?.format == "DOCUMENT") {
                    this.documentFlag = true;
                } else {
                    this.documentFlag = false;
                }
                }
       
                debugger
                if(this.ComponentHeader?.example?.header_text){
                        this.VariableCountHeader= this.ComponentHeader.example.header_text?.length
                        this.inputCountFromBackendHeader = this.VariableCountHeader;
                    }else{
                        this.VariableCountHeader=0;
                        this.inputCountFromBackendHeader = 0;
                    }

                    this.Buttons = this.templateById.components.find(
                        (i: { type: string }) => i.type === "BUTTONS"
                    ) || null;
                    
                    const urlButtons = this.Buttons?.buttons?.filter(
                        (i: { type: string; example?: string[] }) =>
                        i.type === "URL" && i.example != null
                    ) || [];

                    
                    const copyCodeButtons = this.Buttons?.buttons?.filter(
                        (i: { type: string }) => i.type === "COPY_CODE"
                    ) || [];

                    const urlButton1 = urlButtons[0] || null;
                    const urlButton2 = urlButtons[1] || null;
                    const copyCodeButton = copyCodeButtons[0] || null;

                    if(urlButton1){
                        if(urlButton1?.example){
                            this.URLBtton1=urlButton1;
                            this.variableURL1=true;
                            this.urlLink1=urlButton1.url;
                            this.urlExample1=urlButton1.example[0];
                            this.firstButtonURLVariabllesTemplate = new FirstButtonURLVariabllesTemplate({
                            varOne: "",
                            });
                        }
                    }

                    if(urlButton2){
                        if(urlButton2?.example){
                            this.variableURL2=true;
                            this.URLBtton2=urlButton2;
                            this.urlLink2=urlButton2.url
                            this.urlExample2=urlButton2.example[0];
                            this.secondButtonURLVariabllesTemplate = new SecondButtonURLVariabllesTemplate({
                            varOne: "",
                            });
                        }
                    }

                    if(copyCodeButton){
                        if(copyCodeButton?.example){
                            this.copyCodeEsxist=true;
                            this.copyCodeButton=copyCodeButton;
                            this.buttonCopyCodeVariabllesTemplate = new ButtonCopyCodeVariabllesTemplate({
                            varOne: "",
                            });
                        }
                    }

                    if (this.ComponentHeader) {
                        if(this.ComponentHeader.example?.header_text?.[0]){
                                    const firstHeader = this.ComponentHeader.example?.header_text?.[0];
                                if (firstHeader) {
                                    this.listOfHeaderVariable = [firstHeader];
                                }else{
                                    this.listOfHeaderVariable=null;
                                }
                                this.displayHeaderText = this.ComponentHeader.text;
                                this.inputsHeaders = Array.from(
                                    { length: this.VariableCountHeader },
                                    () => ({ value: "" })
                                );
                                if (this.VariableCountHeader > 0 && firstHeader) {
                                    this.getExampleHeader(
                                        [this.ComponentHeader.example?.header_text[0]]
                                    );
                                }
                                this.dynamicTextPartsHeader = this.parseDynamicTextHeader(
                                    this.displayHeaderText
                                );
                        }
                    }

        
                if (this.ComponentBody) {
                    this.listOfVariable =
                        this.ComponentBody.example?.body_text[0];
                    this.displayText = this.ComponentBody.text;
                    this.displayGlobal = this.ComponentBody.text;
                    this.inputs = Array.from(
                        { length: this.inputCountFromBackend },
                        () => ({ value: "" })
                    );
                    this.dynamicTextParts = this.parseDynamicText(
                        this.displayText
                    );
                }
                if (this.VariableCount > 0) {
                    this.getExampleBody(
                        this.ComponentBody.example?.body_text[0]
                    );
                }

                if(result.category=="AUTHENTICATION"){
                    this.category="AUTHENTICATION"
                    if (this.ComponentBody) {
                        debugger

                        this.listOfVariable =
                            this.ComponentBody.example?.body_text[0];
                        this.displayText = this.ComponentBody.text;
                        this.inputs = Array.from(
                            { length: this.VariableCount },
                            () => ({ value: "" })
                        );
                        if (this.VariableCount > 0) {
                            this.getExampleBody(
                                this.ComponentBody.example?.body_text[0]
                            );
                        }          
                        debugger

                        this.dynamicTextParts = this.parseDynamicText(
                            this.displayText
                        );
                        debugger
                    }
                }
                this.componentCarousel = this.templateById.components.find(
                        (i: { type: string }) => i.type === "carousel"
                    )||null;

                if (this.componentCarousel) {
                    this.cards = this.componentCarousel.cards;
                    this.VariableCountCarousel = [];
                    this.inputsCarousel = [];
                    this.dynamicTextPartsCarousel = [];
                    this.displayCarouselTexts = [];


                    this.cards.forEach((card, cardIndex) => {
                        const buttons = card.components.find(c => c.type === "BUTTONS")?.buttons || [];
                        debugger;
                        buttons.forEach((button, buttonIndex) => {
                        if (button.type === 'URL' && button.example) {
                            debugger;
                              // Initialize variables for this URL button
                            this.urlVariables[cardIndex] = this.urlVariables[cardIndex] || {};
                            this.urlVariables[cardIndex][buttonIndex] = 
                                button.example.map(() => ({ value: '' }));
                            }
                        });
                        });

                        this.cards.forEach((card, cardIndex) => {
                            const cardBody = card.components.find((c: { type: string }) => c.type === "BODY");
                            if (cardBody) {
                                debugger;
                                // Get variable count for this card's body
                                const varCount = cardBody.example?.body_text?.[0]?.length || 0;
                                this.VariableCountCarousel[cardIndex] = varCount;
                                
                                // Initialize inputs for this card
                                this.inputsCarousel[cardIndex] = Array.from(
                                    { length: varCount },
                                    () => ({ value: "" })
                                );
                                
                                // Parse dynamic text for this card's body
                                if (cardBody.text) {
                                    debugger;
                                    this.dynamicTextPartsCarousel[cardIndex] = this.parseDynamicText(cardBody.text);
                                    this.displayCarouselTexts[cardIndex] = cardBody.text;
                                }
                            }
                        });
                        
                        this.componentCarousel.cards   = this.cards;
                    }















                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    /**
     * Convert Files list to normal array list
     * @param files (Files List)
     */
    prepareFilesList(files: any) {
        const allowedExtensions = [".xlsx", ".xls"];
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const fileName = file.name;
            const fileExtension = fileName
                .substring(fileName.lastIndexOf("."))
                .toLowerCase();
            if (allowedExtensions.includes(fileExtension)) {
                // Process the file since it's an Excel file
                this.file = file;
                this.uploadFileToActivityUpdated();
            } else {
                this.message.error(this.l("Please Select Only Excel File"));
            }
        }
    }

    uploadFileToActivityUpdated() {
        this.isVariablesValidFlag = false;

        if (this.VariableCount > 0) {
            this.isVariablesValidFlag = true;
        }

        const fileParameter: FileParameter = {
            data: this.file,
            fileName: this.file.name,
        };

        let formDataFile = new FormData();
        formDataFile.append("formFile", this.file);

        this.primengTableHelper.showLoadingIndicator();

        this._whatsAppMessageTemplateServiceProxy
            .readFromExcelNew(
                this.campaign.id,
                this.campaign.templateId,
                fileParameter,
                []
            )
            .subscribe(
                (result) => {
                    this.customers = result.contacts;
                    this.selectedCustomers = result.contacts;
                    this.totalContact = result.contacts.length;

                    if (this.totalContact > this.dailyLimit) {
                        this.message.warn(
                            "",
                            this.l(
                                "Total Contact Must Be Less Than Or Equal : " +
                                    this.dailyLimit
                            )
                        );
                    }

                    // this.campinToQueueDto.totalCount = this.campinToQueueDto.contacts.length;
                    // this.campinToQueueDto.totalOptOut = 0;
                    //             this.lstContact = result.contacts;
                    //             this.totalContact = result.totalCount;
                    // if (result1.result == "exeededlimit") {
                    //     this.message.error(
                    //         "",
                    //         this.l(
                    //             "uploadMoreThanDailyLimit"
                    //         )
                    //     );
                    //     this.primengTableHelper.hideLoadingIndicator();
                    //     return;
                    // }
                    // if (result1.result == "exeededrate") {
                    //     this.message.error(
                    //         "",
                    //         this.l(
                    //             "Sorry ! You have been upload greater than remaining Marketing/Utility bundle"
                    //         )
                    //     );
                    //     this.primengTableHelper.hideLoadingIndicator();
                    //     return;
                    // }
                    // if (result1.result == "notfound") {
                    //     this.message.error(
                    //         "",
                    //         this.l("Sorry ! This file is empty")
                    //     );
                    //     this.primengTableHelper.hideLoadingIndicator();
                    //     return;
                    // }
                    this.primengTableHelper.hideLoadingIndicator();
                    this.notify.success(this.l("Successfully Uploaded"));
                    // this.showExternalContacts(event);
                    // this.reloadPage();
                },
                (error: any) => {
                    if (error) {
                        this.primengTableHelper.hideLoadingIndicator();
                        this.notify.error(error.error.error.message);
                    }
                }
            );
    }

    onFileDropped($event) {
        this.prepareFilesList($event);
    }

    /**
     * handle file from browsing
     */
    fileBrowseHandler(files) {
        this.prepareFilesList(files);
    }

    isValidText(text) {
        // Regular expression to match at least three words or a space followed by two words
        const regex = /^(?=(?:\S*\s\S*\s\S*\s*)|$)(?!\s*$).*$/;

        const trimmedString = text?.replace(/\s+$/, "");

        return regex.test(trimmedString);
    }
    getCountryCode(event: any) {
        this.countryObj = event;
        this.countryObj2 = event;
        this.contact.countryCode = Number(event.dialCode).toString();
    }

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

    filterContacts(event?: LazyLoadEvent) {
        this.filteredContacts = new CampinToQueueDto();
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
            this.submitted = true;
            this.primengTableHelper.hideLoadingIndicator();
            return;
        } else {
            if (this.dateRange[0] == null || this.dateRange[1] == null) {
                this.contact.joiningFrom = null;
                this.contact.joiningTo = null;
            } else {
                this.contact.joiningFrom = this.datePipe.transform(
                    moment(this.dateRange[0]).toDate(),
                    "MM-dd-yyyy"
                );
                this.contact.joiningTo = this.datePipe.transform(
                    moment(this.dateRange[1]).toDate(),
                    "MM-dd-yyyy"
                );
            }
            if (
                this.contact.orderTimeFrom != undefined &&
                this.contact.orderTimeFrom != null
            ) {
                this.orderTime1 = moment(this.contact.orderTimeFrom);
            } else {
                this.orderTime1 = null;
            }
            if (
                this.contact.orderTimeTo != undefined &&
                this.contact.orderTimeTo != null
            ) {
                this.orderTime2 = moment(this.contact.orderTimeTo);
            } else {
                this.orderTime2 = null;
            }
            if (this.contact.interestedOfOne == undefined) {
                this.contact.interestedOfOne = null;
                this.contact.interestedOfTwo = null;
                this.contact.interestedOfThree = null;
            }
            if (this.contact.interestedOfTwo == undefined) {
                this.contact.interestedOfTwo = null;
                this.contact.interestedOfThree = null;
            }
            if (this.contact.interestedOfThree == undefined) {
                this.contact.interestedOfThree = null;
            }

            if (this.contact.totalOrderMin < 0) {
                this.contact.totalOrderMin = null;
            }
            if (this.contact.totalOrderMax < 0) {
                this.contact.totalOrderMax = null;
            }
            if (this.contact.interestedOfOne == undefined) {
                this.contact.interestedOfOne = null;
            }
            if (this.contact.interestedOfTwo == undefined) {
                this.contact.interestedOfTwo = null;
            }
            if (this.contact.interestedOfThree == undefined) {
                this.contact.interestedOfThree = null;
            }
            this.lstContact = [new ListContactToCampin()];
            this.totalContact = 0;
            this.primengTableHelper.showLoadingIndicator();
            this._whatsAppMessageTemplateServiceProxy
                .getDailylimitCount()
                .subscribe((resultCount) => {
                    this._whatsAppMessageTemplateServiceProxy
                        .getNewFilterContacts(
                            0,
                            this.contact.phoneNumber,
                            this.contact.countryCode,
                            this.contact.city,
                            this.contact.branch,
                            this.contact.contactName,
                            this.contact.joiningFrom,
                            this.contact.joiningTo,
                            this.orderTime1,
                            this.orderTime2,
                            this.contact.totalOrderMin,
                            this.contact.totalOrderMax,
                            this.contact.totalSessions,

                            this.contact.interestedOfOne,
                            this.contact.interestedOfTwo,
                            this.contact.interestedOfThree,
                            this.contact.isOpt,
                            null, // templateVariables_VarOne
                            null, // templateVariables_VarTwo
                            null, // templateVariables_VarThree
                            null, // templateVariables_VarFour
                            null, // templateVariables_VarFive
                            null, // templateVariables_VarSix
                            null, // templateVariables_VarSeven
                            null, // templateVariables_VarEight
                            null, // templateVariables_VarNine
                            null, // templateVariables_VarTen
                            null, // templateVariables_VarEleven
                            null, // templateVariables_VarTwelve
                            null, // templateVariables_VarThirteen
                            null, // templateVariables_VarFourteen
                            null, // templateVariables_VarFifteen
                            null, // templateVariables_VarSixteen
                            null, // customerOPT
                            this.campaign.templateId,
                            this.campaign.id,
                            this.appSession.tenantId,
                            0,
                            0
                        )
                        .subscribe((result) => {
                            this.isCount = true;
                            this.filteredContacts = result;
                            this.customers = this.filteredContacts.contacts;
                            this.selectedCustomers =
                                this.filteredContacts.contacts;
                            this.totalContact =
                                this.filteredContacts.totalCount;

                            this.countallowedperday = resultCount.biDailyLimit;
                            if (this.totalContact > resultCount.dailyLimit) {
                                this.message.error(
                                    "",
                                    this.l("numberGreaterThanAllowed")
                                );
                                this.primengTableHelper.hideLoadingIndicator();
                                return;
                            }

                            this.contact.orderTimeFrom;
                            this.contact.orderTimeTo;
                            this.primengTableHelper.hideLoadingIndicator();
                            this.primengTableHelper.totalRecordsCount =
                                result.totalCount;
                            this.primengTableHelper.records = result.contacts;
                            window.scrollTo(0, document.body.scrollHeight);
                            this.lstContact = result.contacts;
                            this.lstContactWithoutOptOut = [];

                            result.contacts.forEach((element) => {
                                if (element.customerOPT != 1) {
                                    this.lstContactWithoutOptOut.push(
                                        element.phoneNumber
                                    );
                                }
                            });
                            this.totalOptOut = result.totalOptOut;
                        });
                });
        }
    }

    goToSchedulePage() {
        debugger

        let templateVariables: TemplateVariablles;
        let templateHeaderVariables: HeaderVariablesTemplate;
        let carouselTemplate: CarouselVariabllesTemplate | null = null;

    if (this.componentCarousel) {
            const cards: Card[] = [];
            let hasCarouselVariables = false;
            this.cards.forEach((card, cardIndex) => {
            const buttons = card.components.find(c => c.type === "BUTTONS")?.buttons || [];
            const buttonVariables: any = {};
            buttons.forEach((button, buttonIndex) => {
            if (button.type === 'URL' && this.urlVariables[cardIndex]?.[buttonIndex]) {
                // Validate URL variables
                for (const input of this.urlVariables[cardIndex][buttonIndex]) {
                    if (!input.value) {
                        this.notify.warn("Please fill all URL variables");
                        return;
                    }
                };
              }
            });
          });

        for (let i = 0; i < this.cards.length; i++) {
            const card = this.cards[i];
            const bodyComponent = card.components.find((c: any) => c.type === "BODY");
            const buttonsComponent = card.components.find((c: any) => c.type === "BUTTONS");
            debugger

            let cardVariables: CardVariabllesTemplate | null = null;
            if (this.VariableCountCarousel[i] > 0) {
                for (const item of this.inputsCarousel[i]) {
                    if (item.value.length === 0) return;
                }

                const arr = this.dynamicTextPartsCarousel[i].filter(
                    (item) => item.type == "input"
                );

                cardVariables = new CardVariabllesTemplate({
                    varOne: arr[0]?.content || null,
                    varTwo: arr[1]?.content || null,
                    varThree: arr[2]?.content || null,
                    varFour: arr[3]?.content || null,
                    varFive: arr[4]?.content || null,
                    varSix: arr[5]?.content || null,
                    varSeven: arr[6]?.content || null,
                    varEight: arr[7]?.content || null,
                    varNine: arr[8]?.content || null,
                    varTen: arr[9]?.content || null,
                    varEleven: arr[10]?.content || null,
                    varTwelve: arr[11]?.content || null,
                    varThirteen: arr[12]?.content || null,
                    varFourteen: arr[13]?.content || null,
                    varFifteen: arr[14]?.content || null, 
                    });
                hasCarouselVariables = true;
            }
            debugger

            let firstButtonVars: FirstButtonURLVariabllesTemplate | null = null;
            let secondButtonVars: SecondButtonURLVariabllesTemplate | null = null;

            
            if (buttonsComponent) {
                const urlButtons = buttonsComponent.buttons?.filter(
                    (b: any) => b.type === "URL"
                ) || [];

                if (urlButtons[0]?.example) {
                    firstButtonVars = new FirstButtonURLVariabllesTemplate({
                        varOne: this.urlVariables[i]?.[0]?.[0]?.value ?? null
                    });
                }

                
                if (urlButtons[1]?.example) {
                    secondButtonVars = new SecondButtonURLVariabllesTemplate({
                        varOne: this.urlVariables[i]?.[1]?.[0]?.value ?? null
                    });
                }
            }


            cards.push(new Card({
                variables: cardVariables,
                firstButtonURLVariabllesTemplate: firstButtonVars,
                secondButtonURLVariabllesTemplate: secondButtonVars,
                cardIndex: i,
                variableCount: this.VariableCountCarousel[i]
            }));
        }

        if (hasCarouselVariables) {
            carouselTemplate = new CarouselVariabllesTemplate({
                cards: cards
            });
        }
    }

            if (this.copyCodeEsxist && this.buttonCopyCodeVariabllesTemplate.varOne.length==0) {
                    return;
            }
        if (this.VariableCountHeader > 0) {

            if (this.inputsHeaders[0].value.length === 0) return;
            
            let arr = this.dynamicTextPartsHeader.filter(
                (item) => item.type == "input"
            );

            templateHeaderVariables = new HeaderVariablesTemplate({
                varOne: arr[0]?.content || null,
            });
        } else {
            templateHeaderVariables = null;
        }




        if (this.VariableCount > 0) {
            for (const item of this.inputs) {
                if (item.value.length === 0) return;
            }
            let arr = this.dynamicTextParts.filter(
                (item) => item.type == "input"
            );

            templateVariables = new TemplateVariablles({
                varOne: arr[0]?.content || null,
                varTwo: arr[1]?.content || null,
                varThree: arr[2]?.content || null,
                varFour: arr[3]?.content || null,
                varFive: arr[4]?.content || null,
                varSix: arr[5]?.content || null,
                varSeven: arr[6]?.content || null,
                varEight: arr[7]?.content || null,
                varNine: arr[8]?.content || null,
                varTen: arr[9]?.content || null,
                varEleven: arr[10]?.content || null,
                varTwelve: arr[11]?.content || null,
                varThirteen: arr[12]?.content || null,
                varFourteen: arr[13]?.content || null,
                varFifteen: arr[14]?.content || null, 
                vaSixteen: arr[15]?.content || null,
                varSixteen: arr[15]?.content || null, 
            });
        } else {
            templateVariables = null;
        }





        
        const campinToQueueDto = new CampinToQueueDto();
        campinToQueueDto.campaignId = this.campaign.id;
        campinToQueueDto.templateId = this.campaign.templateId;
        campinToQueueDto.campaignName = this.campaign.title;
        campinToQueueDto.templateName = this.templateById.name;
        campinToQueueDto.isExternal = this.externalContactFlag;
        campinToQueueDto.totalCount = this.selectedCustomers.length;
        campinToQueueDto.totalOptOut = 0;
        campinToQueueDto.contacts = this.selectedCustomers;
        campinToQueueDto.templateVariables = null;
        campinToQueueDto.headerVariabllesTemplate = null;
        campinToQueueDto.firstButtonURLVariabllesTemplate = null;
        campinToQueueDto.secondButtonURLVariabllesTemplate = null;
        campinToQueueDto.carouselTemplate = null;
        campinToQueueDto.buttonCopyCodeVariabllesTemplate = null;

        this._whatsAppMessageTemplateServiceProxy
            .sendCampaignValidation()
            .subscribe((validation) => {
                debugger;
                if (validation) {
                    if (this.totalContact - this.totalOptOut == 0) {
                        this.message.error(this.l("cantSendCampaignToOptOut"));
                        this.primengTableHelper.hideLoadingIndicator();
                        return;
                    }
                    // if (
                    //     this.totalContact >
                    //     Math.ceil(this.TotalRemainingAdsConversation) ||
                    //     Math.ceil(this.TotalRemainingAdsConversation) == 0
                    // ) {
                    //     this.message.error(
                    //          this.l('dontHaveEnoughBunddle')
                    //     );
                    //     this.primengTableHelper.hideLoadingIndicator();
                    //     return;
                    // }

                    // if (this.Measurement.remainingBIConversation == 0) {
                    //     this.confirm =
                    //         "Your Marketing/Utility Bundle Is ZERO ,Are You Sure To Deducted " +
                    //         this.totalContact +
                    //         " From Conversation Bundle ?";
                    // } else {
                    //     this.confirm =
                    //         "Are You Sure To Send Ads To " +
                    //         this.totalContact +
                    //         " Contacts ?";
                    // }

                    this.confirm =
                        "Are You Sure To Send Ads To " +
                        this.selectedCustomers.length +
                        " Contacts ?";
                    this.message.confirm(
                        "",
                        this.l(this.confirm),
                        (isConfirmed) => {
                            if (isConfirmed) {
                                this.scheduleCampaign.show(
                                    this.externalContactFlag,
                                    this.campaign.id,
                                    this.campaign.templateId,
                                    this.contact,
                                    this.templateById.name,
                                    campinToQueueDto
                                );
                            }
                        }
                    );
                } else {
                    this.message.error(
                        "",
                        this.l("You have campaign in under process")
                    );
                    this.primengTableHelper.hideLoadingIndicator();
                }
            });
    }
    getCampaignDailyLimit() {
        debugger;
        this._whatsAppMessageTemplateServiceProxy
            .getDailylimitCount()
            .subscribe((resultCount) => {
                this.dailyLimit = resultCount.dailyLimit;
                this.countallowedperday = resultCount.biDailyLimit;
            });
    }
    showExternalContacts(event?: LazyLoadEvent) {
        if (!this.paginator) return;
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        if (this.dailyLimit > 0) {
            this.contact = new WhatsAppContactsDto();
            this.totalContact = 0;
            this.primengTableHelper.showLoadingIndicator();
            this._whatsAppMessageTemplateServiceProxy
                .getNewExternalContacts(
                    this.campaign.templateId,
                    this.campaign.id,
                    this.primengTableHelper.getSkipCount(this.paginator, event),
                    this.primengTableHelper.getMaxResultCount(
                        this.paginator,
                        event
                    ),
                    this.appSession.tenantId
                )
                .subscribe(
                    (result) => {
                        this.primengTableHelper.hideLoadingIndicator();
                        this.primengTableHelper.totalRecordsCount =
                            result.totalCount;
                        this.primengTableHelper.records = result.contacts;
                        this.lstContact = result.contacts;
                        this.totalContact = result.totalCount;
                        // if (this.totalContact > 1000) {
                        //   this.message.error(
                        //     "",
                        //     this.l("Total Contact Must Be Less Than Or Equal 1000")
                        //   );
                        //   return;
                        // }
                    },
                    (error: any) => {
                        if (error) {
                            this.primengTableHelper.hideLoadingIndicator();
                        }
                    }
                );
        }
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }
    canLeavePage(): Promise<boolean> {
        if (this.isFromPage) {
            return Promise.resolve(true);
        } else {
            return new Promise((resolve) => {
                this.confirmationService.confirm({
                    target: event.target as EventTarget,
                    message: "Are you sure that you want to Leave?",
                    header: "Confirmation",
                    icon: "pi pi-exclamation-triangle",
                    acceptIcon: "none",
                    rejectIcon: "none",
                    rejectButtonStyleClass: "p-button-text",
                    accept: () => {
                        return resolve(true);
                    },
                    reject: () => {
                        return resolve(false);
                    },
                });
            });
        }
    }

    handleFileInput(event) {
        this.tempFile = event.target.files[0];
        this.extFile = this.tempFile.name.substring(
            this.tempFile.name.lastIndexOf(".") + 1
        );

        if (this.extFile == "xlsx") {
            this.fileToUpload = event.target.files[0];
        } else {
            this.element.nativeElement.value = "";
            this.message.error("", this.l("pleaseChooseExcelFile"));
            event.target.files[0] = null;
            return;
        }
    }

    // uploadFileToActivity(event?: LazyLoadEvent) {
    //     if (this.fileToUpload == null || this.fileToUpload == undefined) {
    //         this.message.error("", this.l("pleaseChooseExcelFile"));
    //         return;
    //     } else {
    //         let formDataFile = new FormData();
    //         formDataFile.append("formFile", this.fileToUpload);
    //         this.primengTableHelper.showLoadingIndicator();
    //         this.messageCampaignService
    //             .UploadExcelFileNew(
    //                 formDataFile,
    //                 this.campaign.id,
    //                 this.campaign.templateId
    //             )
    //             .subscribe(

    //                 ( {result} ) => {

    //                     this.primengTableHelper.totalRecordsCount =
    //                         result.contacts.length;

    //                     this.primengTableHelper.records = result.contacts;
    //                     this.primengTableHelper.hideLoadingIndicator();
    //                          if (this.primengTableHelper.records.length > this.dailyLimit) {
    //                       this.message.error(
    //                         "",
    //                         this.l("Total Contact Must Be Less Than Or Equal : "  + this.dailyLimit)
    //                       );
    //                       return;
    //                     }

    //                     this.campinToQueueDto = new CampinToQueueDto();
    //     this.campinToQueueDto.campaignId = this.campaign.id;
    //     this.campinToQueueDto.templateId = this.campaign.templateId;
    //     this.campinToQueueDto.contacts = result.contacts;;
    //     let templateVariables1 = new TemplateVariables();

    //     // this.primengTableHelper.showLoadingIndicator();

    //     let templateVariables = new TemplateVariables();

    //     if (this.externalContactFlag) {
    //         this.campinToQueueDto.isExternal = true;
    //         for (let index = 0; index < this.campinToQueueDto.contacts.length; index++) {

    //             templateVariables1 = new TemplateVariables();

    //             let contact1 =   new ListContactToCampin()

    //             contact1.contactName =  this.campinToQueueDto.contacts[index].contactName
    //             contact1.phoneNumber =  this.campinToQueueDto.contacts[index].phoneNumber
    //             contact1.customerOPT =  this.campinToQueueDto.contacts[index].customerOPT
    //             contact1.id =  this.campinToQueueDto.contacts[index].id
    //             contact1.templateVariables =  this.campinToQueueDto.contacts[index].templateVariables

    //             this.campinToQueueDto.contacts[index] = contact1;

    //             const contact = this.campinToQueueDto.contacts[index];
    //             templateVariables1.varOne = contact.templateVariables.varOne ? contact.templateVariables.varOne : '';
    //             templateVariables1.varTwo = contact.templateVariables.varTwo ? contact.templateVariables.varTwo : '';
    //             templateVariables1.varThree = contact.templateVariables.varThree ? contact.templateVariables.varThree : '';
    //             templateVariables1.varFour = contact.templateVariables.varFour ? contact.templateVariables.varFour : '';
    //             templateVariables1.varFive = contact.templateVariables.varFive ? contact.templateVariables.varFive : '';

    //             this.campinToQueueDto.contacts[index].templateVariables = templateVariables1
    //         }

    //     } else {
    //         this.campinToQueueDto.isExternal = false;
    //     }
    //     // templateVariables.varOne = this.listOfVariable >= 1 ? "hasanabedalqader" : null;
    //     // templateVariables.varTwo = this.listOfVariable >= 2 ? "hasanabedalqader" : null;
    //     // templateVariables.varThree = this.listOfVariable >= 3 ? "hasanabedalqader" : null;
    //     // templateVariables.varFour = this.listOfVariable >= 4 ? "hasanabedalqader" : null;
    //     // templateVariables.varFive = this.listOfVariable >= 5 ? "hasanabedalqader" : null;
    //     this.campinToQueueDto.templateVariables = templateVariables;

    //     this.campinToQueueDto.totalCount = this.campinToQueueDto.contacts.length;
    //     this.campinToQueueDto.totalOptOut = 0;
    //                     this.lstContact = result.contacts;
    //                     this.totalContact = result.totalCount;
    //                     // if (result1.result == "exeededlimit") {
    //                     //     this.message.error(
    //                     //         "",
    //                     //         this.l(
    //                     //             "uploadMoreThanDailyLimit"
    //                     //         )
    //                     //     );
    //                     //     this.primengTableHelper.hideLoadingIndicator();
    //                     //     return;
    //                     // }
    //                     // if (result1.result == "exeededrate") {
    //                     //     this.message.error(
    //                     //         "",
    //                     //         this.l(
    //                     //             "Sorry ! You have been upload greater than remaining Marketing/Utility bundle"
    //                     //         )
    //                     //     );
    //                     //     this.primengTableHelper.hideLoadingIndicator();
    //                     //     return;
    //                     // }
    //                     // if (result1.result == "notfound") {
    //                     //     this.message.error(
    //                     //         "",
    //                     //         this.l("Sorry ! This file is empty")
    //                     //     );
    //                     //     this.primengTableHelper.hideLoadingIndicator();
    //                     //     return;
    //                     // }
    //                     this.primengTableHelper.hideLoadingIndicator();
    //                     this.notify.success(this.l("Successfully Uploaded"));
    //                     // this.showExternalContacts(event);
    //                     this.reloadPage();
    //                 },
    //                 (error: any) => {
    //                     if (error) {
    //                         this.primengTableHelper.hideLoadingIndicator();
    //                         this.notify.error(error.error.error.message);
    //                     }
    //                 }
    //             );
    //     }
    // }

    handleSendToGroup() {
        this.sendToGroupFlag = true;
        // Reset other flags
        this.filterContactFlag = false;
        this.externalContactFlag = false;
    }

    filterContact() {
        //this.countryObj=this.countryObj2;
        //this.countryObj2;
        this.primengTableHelper.records = [];
        this.primengTableHelper.totalRecordsCount = 0;
        this.customers = [];
        this.selectedCustomers = [];
        this.totalContact = 0;
        this.isVariablesValidFlag = false;
        this.updatebtn = false;
        this.file = null;
        // this.getTemplateById();

        this.filterContactFlag = true;
        this.sendToGroupFlag = false;
        this.externalContactFlag = false;
    }
    externalContact(event?: LazyLoadEvent) {
        this.showExternalContacts(event);

        this.externalContactFlag = true;
        this.isVariablesValidFlag = false;
        this.updatebtn = false;
        this.file = null;
        //   this.getTemplateById();
        // Reset other flags
        this.sendToGroupFlag = false;
        this.filterContactFlag = false;
        // this.showExternalContacts(event);
        //this.countryObj;
        //this.countryObj=this.countryObj2;
        this.primengTableHelper.records = [];
        this.customers = [];
        this.selectedCustomers = [];
        this.displayText = "";
        this.totalContact = 0;
        this.primengTableHelper.totalRecordsCount = 0;
        this.totalOptOut = 0;
        this.selectedItems = [];
        this.contact.branch = null;
        this.contact.city = null;
        this.contact.contactName = null;
        this.contact.countryCode = null;
        this.contact.customerOPT = null;
        this.contact.interestedOfOne = null;
        this.contact.interestedOfTwo = null;
        this.contact.interestedOfThree = null;
        this.dateRange = [null, null];
        this.contact.joiningFrom = null;
        this.contact.joiningTo = null;
        this.contact.totalOrderMax = null;
        this.contact.totalOrderMin = null;
        this.contact.totalSessions = null;
        this.contact.orderTimeFrom = null;
        this.contact.orderTimeTo = null;
        this.filterContactFlag = false;
        this.externalContactFlag = true;
    }

    resetFilters() {
        this.selectedItems = [];
        this.contact.branch = null;
        this.contact.city = null;
        this.contact.contactName = null;
        //this.contact.countryCode= null
        this.contact.customerOPT = null;
        this.contact.interestedOfOne = null;
        this.contact.interestedOfTwo = null;
        this.contact.interestedOfThree = null;
        this.dateRange = [null, null];
        this.contact.joiningFrom = null;
        this.contact.joiningTo = null;
        this.contact.totalOrderMax = null;
        this.contact.totalOrderMin = null;
        this.contact.totalSessions = null;
        this.contact.orderTimeFrom = null;
        this.contact.orderTimeTo = null;
    }

    loadLevels() {
        this._assetServiceProxy.loadLevels(null).subscribe((result: any) => {
            if (result.lstAssetLevelOneDto == undefined) {
                this.assetLevelOneFlag = false;
                this.assetLevelTwoFlag = false;
                this.assetLevelThreeFlag = false;
            } else {
                this.assetLevelOne = [new AssetLevelOneDto()];
                this.assetLevelOne = result.lstAssetLevelOneDto;
                this.assetLevelOneFlag = true;
                if (result.lstAssetLevelTwoDto == undefined) {
                    this.assetLevelTwoFlag = false;
                    this.assetLevelThreeFlag = false;
                } else {
                    //this.assetLevelTwoFlag = true;
                    this.assetLevelTwoMain = result.lstAssetLevelTwoDto;
                    if (result.lstAssetLevelThreeDto == undefined) {
                        this.assetLevelThreeFlag = false;
                    } else {
                        //this.assetLevelThreeFlag = true;
                        this.assetLevelThreeMain = result.lstAssetLevelThreeDto;
                    }
                }
            }
            this.assetLevelOne = result.lstAssetLevelOneDto;
            this.assetLevelTwoMain = result.lstAssetLevelTwoDto;
            this.assetLevelThreeMain = result.lstAssetLevelThreeDto;
        });
    }

    onChangeLevelOne(event): void {
        this.contact.interestedOfTwo = null;

        const level1Id = Number(event.target.value);
        this.assetLevelTwo = [new AssetLevelTwoDto()];
        this.assetLevelThree = [new AssetLevelThreeDto()];

        this._assetServiceProxy.loadLevels(null).subscribe((result: any) => {
            this.assetLevelTwoMain = result.lstAssetLevelTwoDto;
            this.assetLevelTwoMain.forEach((element) => {
                this.assetLevelTwoFlag = true;
                this.assetLevelThreeFlag = false;

                if (element.levelOneId == level1Id) {
                    this.assetLevelTwo.push(element);
                }
            });
            if (this.assetLevelTwo.length == 1) {
                this.assetLevelTwoFlag = false;
                this.assetLevelThreeFlag = false;
            }
        });
        if (event.target.value == "undefined") {
            this.contact.interestedOfOne = null;
        } else {
            this.contact.interestedOfOne = event.target.value;
        }
    }

    onChangeLevelTwo(event): void {
        const level2Id = Number(event.target.value);
        this.contact.interestedOfThree = null;

        this.assetLevelThree = [new AssetLevelThreeDto()];
        this._assetServiceProxy.loadLevels(null).subscribe((result: any) => {
            this.assetLevelThreeMain = result.lstAssetLevelThreeDto;
            this.assetLevelThreeMain.forEach((element) => {
                this.assetLevelThreeFlag = true;
                if (element.levelTwoId == level2Id) {
                    this.assetLevelThree.push(element);
                }
            });
            if (this.assetLevelThree.length <= 1) {
                this.assetLevelThreeFlag = false;
            }
        });

        if (event.target.value == "undefined") {
            this.contact.interestedOfTwo = null;
        } else {
            this.contact.interestedOfTwo = event.target.value;
        }
    }

    getBranches() {
        this.area = new AreaDto();
        this._areasServiceProxy
            .getAllAreas(this.appSession.tenantId, null)
            .subscribe((result: any) => {
                this.area = result;
            });
    }

    goToDashboard() {
        this._router.navigate(["/app/main/dashboard"]);
    }

    goToCampaign() {
        this._router.navigate(["/app/main/messageCampaign"]);
    }

    getItems() {
        if (this.isLoading) {
            return [{ id: null, name: "Loading...", disabled: true }];
        } else {
            if (
                this.language != null &&
                this.templateId != null &&
                this.templateName != null &&
                !this.selectOnce
            ) {
                this.selectOnce = true;
                let tempTemplate = this.template.find(x => x.id == this.templateId)
                this.newTemplateId = this.templateName
                this.getTemplateById(tempTemplate)
            }
            return this.template;
        }
    }

    test22(){
        debugger
        this.ah;
        this.inputsCarousel;
        this.componentCarousel;
        this.dynamicTextPartsHeader;
        this.dynamicTextParts;
        this.inputsHeaders;
        this.inputs;
        this.dynamicTextPartsHeader;
        this.ComponentButton;
        this.buttonCopyCodeVariabllesTemplate;
        this.ComponentBody;
        this.displayCarouselTexts;this.dynamicTextPartsCarousel;
        this.urlVariables;
        this.campaignForm.form.value.stepOneCampign["var1"] || null;
                        this.campaignForm.form.value.stepOneCampign["var1"] || null;

        // this.cd.detectChanges();
    }

    
     parseDynamicTextHeader(
        text: string
    ): { type: "text" | "input"; content: string; index?: number }[] {
        debugger;
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
    
    updateTextHeader() {
            debugger;
        this.displayHeaderText = "";
        // Replace placeholders with input values, or revert to placeholders if input is empty
        this.dynamicTextPartsHeader.forEach((part) => {
            if (part.type === "input") {
                part.content =
                    this.inputsHeaders[part.index].value || `{{${part.index + 1}}}`;
            }
        });
        this.displayHeaderText = this.dynamicTextPartsHeader
            .map((item) => item.content)
            .join("");
        this.ComponentHeader.text = this.displayHeaderText;
    }

    getExampleHeader(example: string[]) {
        debugger
        this.customeexampleHeader = example;
    }

        updateCarouselText(cardIndex: number) {
            debugger;
                    this.inputsCarousel;

    this.displayCarouselTexts[cardIndex] = "";

    this.dynamicTextPartsCarousel[cardIndex].forEach((part) => {
        if (part.type === "input") {
            part.content = this.inputsCarousel[cardIndex][part.index].value  || `{{${part.index + 1}}}`;
        }
    });

    this.inputsCarousel;
    this.displayCarouselTexts[cardIndex] = this.dynamicTextPartsCarousel[cardIndex]
        .map((item) => item.content)
        .join("");

    // Update the card's body text in the carousel data
    if (this.cards[cardIndex]) {
        const cardBody = this.cards[cardIndex].components.find((c: { type: string }) => c.type === "BODY");
        if (cardBody) {
            cardBody.text = this.displayCarouselTexts[cardIndex];
        }
    }
}


// Method to parse URL for variables
parseUrlVariables(url: string): { type: "text" | "input"; content: string; index?: number }[] {
    const parts: { type: "text" | "input"; content: string; index?: number }[] = [];
    const regex = /{{(\d+)}}/g;
    let lastIndex = 0;
    let match;
    
    while ((match = regex.exec(url)) !== null) {
        if (match.index > lastIndex) {
        parts.push({
            type: "text",
            content: url.substring(lastIndex, match.index)
        });
      }
      
      parts.push({
        type: "input",
        content: match[0],
        index: parseInt(match[1]) - 1
        });
      
        lastIndex = match.index + match[0].length;
    }
    
    if (lastIndex < url.length) {
      parts.push({
        type: "text",
        content: url.substring(lastIndex)
      });
    }
    
    return parts;
  }
onInputChange(cardIndex: number, buttonIndex: number, inputIndex: number) {
  console.log('Changed value:', this.urlVariables[cardIndex][buttonIndex][inputIndex].value);
}

// Method to update URL with variables
        updateUrlWithVariables(cardIndex: number, buttonIndex: number, url: string): string {
            this.urlVariables;
            if (!this.urlVariables[cardIndex]?.[buttonIndex]) return url;
        
            return url.replace(/{{(\d+)}}/g, (match, index) => {
            const varIndex = parseInt(index) - 1;
            return this.urlVariables[cardIndex][buttonIndex][varIndex]?.value || match;
            });
        }

       buildCopyCodeButton(): string {
        
                const placeholderValue = this.buttonCopyCodeVariabllesTemplate.varOne || '';
            return  placeholderValue;
        }

        buildURLButton1(): string {
                const placeholderValue = this.firstButtonURLVariabllesTemplate.varOne || '';
            return this.URLBtton1.url.replace('{{1}}', placeholderValue);
        }
        
        buildURLButton2(): string {
                const placeholderValue = this.secondButtonURLVariabllesTemplate.varOne || '';
            return this.URLBtton2.url.replace('{{1}}', placeholderValue);
        }
        
        URLValidVarible1() {
            if (this.URLBtton1) {
                const varOne = this.firstButtonURLVariabllesTemplate.varOne;
                this.URL1isValid = typeof varOne === 'string' && /^[a-zA-Z0-9-_]+$/.test(varOne);
            }
        }
        URLValidVarible2() {
            if (this.URLBtton2) {
                const varOne = this.secondButtonURLVariabllesTemplate.varOne;
                this.URL2isValid = typeof varOne === 'string' && /^[a-zA-Z0-9-_]+$/.test(varOne);
            }
        }

     onKeyPress2(event: any, value: any) {
        const forbiddenKey = ["$"];
        const keyPressed = event.key;
        if (forbiddenKey.includes(keyPressed)) {
            event.preventDefault();
        }
    }

    getDisplayText(cardIndex: number): string {
    return this.inputsCarousel[cardIndex]
      .map((input, i) => `Input ${i + 1}: ${input.value}`)
      .join(', ');
  }

  CheckHeaderIfValid(): void {
    debugger;
    this.headerValidVariable = this.inputsHeaders[0]?.value.trim() !== '';
    this.headerValidVariable ;
}


        
}
