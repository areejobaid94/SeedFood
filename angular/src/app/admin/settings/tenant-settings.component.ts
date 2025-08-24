import { Status } from './../../../shared/service-proxies/service-proxies';
import {
    IAjaxResponse,
    PermissionCheckerService,
    TokenService,
} from "abp-ng2-module";
import {
    AfterViewInit,
    Component,
    ElementRef,
    Injector,
    OnInit,
    ViewChild,
} from "@angular/core";
import { AppConsts } from "@shared/AppConsts";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    SettingScopes,
    SendTestEmailInput,
    TenantSettingsEditDto,
    TenantSettingsServiceProxy,
    JsonClaimMapDto,
    DeliveryLocationInfoModel,
    TenantServiceProxy,
    DeliveryCostType,
    TenantInformationDto,
    WhatsAppHeaderUrl,
    MenusServiceProxy,
    CaptionDto,
} from "@shared/service-proxies/service-proxies";
import { FileUploader, FileUploaderOptions } from "ng2-file-upload";
import { finalize } from "rxjs/operators";
import { KeyValueListManagerComponent } from "@app/shared/common/key-value-list-manager/key-value-list-manager.component";
import * as moment from "moment";
import { Paginator } from "primeng/paginator";
import { CreateOrEditorderofferModelComponent } from "./create-or-edit-orderoffer";
import { ModalDirective } from "ngx-bootstrap/modal";
import { DarkModeService } from "./../../services/dark-mode.service";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { debounce } from "lodash";
import { FlatpickrOptions } from "ng2-flatpickr";
import {
    DiagramComponent,
    NodeModel,
    HierarchicalTree,
    ConnectorModel,
    StackPanel,
    TextElement,
    Segments,
    ConnectorConstraints,
    NodeConstraints,
    PointPortModel,
    PortVisibility,
    BasicShapeModel,
    LayoutModel,
} from "@syncfusion/ej2-angular-diagrams";
import {
    Diagram,
    SnapConstraints,
    SnapSettingsModel,
    randomId,
} from "@syncfusion/ej2-diagrams";
import { ChangeEventArgs as CheckBoxChangeEventArgs } from "@syncfusion/ej2-buttons";
import { NgxFlowChatOptions, NgxFlowChatData } from "ngx-flowchart";
import { AppSessionService } from "@shared/common/session/app-session.service";
import { NgbModal, NgbNav } from "@ng-bootstrap/ng-bootstrap";
import { ImageCroppedEvent, base64ToFile } from "ngx-image-cropper";
import { HttpClient } from "@angular/common/http";
import { PlyrComponent } from "ngx-plyr";
import { DomSanitizer } from "@angular/platform-browser";
import * as rtlDetect from "rtl-detect";
import { EditCaptionComponent } from "./edit-caption/edit-caption.component";
import { ActivatedRoute } from "@angular/router";
import { Router } from '@angular/router';
const { toGregorian } = require("hijri-converter");
export class Caption {
    id: string;
    name: string;
    groupData: GroupData[];
}

export class GroupData {
    id: string;
    name: string;
}

Diagram.Inject(HierarchicalTree);
@Component({
    templateUrl: "./tenant-settings.component.html",
    styleUrls: ["./tenant-setting.css"],
    animations: [appModuleAnimation()],
})

// class ITenantInformationDto2 {
//     startDate:any;
//     endDate: any;
// }
export class TenantSettingsComponent
    extends AppComponentBase
    implements OnInit, AfterViewInit
{
    @ViewChild("navVertical", { static: false }) navVertical: NgbNav;
    page: string | null = null;
    submitted = false;
    openSat = true;
    openSun = true;
    openMon = true;
    openTus = true;
    openWed = true;
    openThur = true;
    openFri = true;
    isArabic = false;
    theme: string;
    times = [];
    saving = false;
    currency = "";
    public plyr: PlyrComponent;
    public player: Plyr;
    public plyrOptions = { tooltips: { controls: true } };
    minDate: any;
    imagSrc: string;
    imagBGSrc: string;
    imageChangedEvent: any = "";
    file: any;
    private http: HttpClient;
    fromFileUplode: any;
    loadLogoImage = false;

    @ViewChild("createOrEditorderofferModal")
    public createOrEditorderofferModal: CreateOrEditorderofferModelComponent;

    @ViewChild("editCaption") public editCaption: EditCaptionComponent;

    @ViewChild("wsFederationClaimsMappingManager")
    wsFederationClaimsMappingManager: KeyValueListManagerComponent;
    @ViewChild("openIdConnectClaimsMappingManager")
    openIdConnectClaimsMappingManager: KeyValueListManagerComponent;

    @ViewChild("paginator", { static: true }) paginator: Paginator;
    usingDefaultTimeZone = false;
    initialTimeZone: string = null;
    testEmailAddress: string = undefined;
    setRandomPassword: boolean;

    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;

    isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;
    showTimezoneSelection: boolean =
        abp.clock.provider.supportsMultipleTimezone;
    activeTabIndex: number = abp.clock.provider.supportsMultipleTimezone
        ? 0
        : 1;
    loading = false;
    public settings: TenantSettingsEditDto = undefined;
    min = new Date();
    minDateOffer = new Date();
    bookingCapacity;

    public DeliveryCost: DeliveryCostType = undefined;
    settings2: any;
    public multipleDateOptions: FlatpickrOptions = {
        altInput: true,
        mode: "multiple",
        dateFormat: "d.m.Y",
    };

    options = [
        "France",
        "United Kingdom",
        "Germany",
        "Belgium",
        "Netherlands",
        "Spain",
        "Italy",
        "Poland",
        "Austria",
    ];
    startTime: any;
    endTime: any;
    cancelTime: any;
    TenantInformationDto: TenantInformationDto;
    isOrderOffer: boolean;
    isWorkActive: boolean;
    isLiveChatWorkActive: boolean;
    isReplyAfterHumanHandOver: boolean;
    isCancelOrder: boolean;
        isTaxOrder: boolean;


    colorTheme: string;
    isBotActive: boolean;
    workText: string;

    MassageIfBotNotActive: string;

    logoUploader: FileUploader;
    customCssUploader: FileUploader;

    remoteServiceBaseUrl = AppConsts.remoteServiceBaseUrl;

    defaultTimezoneScope: SettingScopes = SettingScopes.Tenant;

    enabledSocialLoginSettings: string[];
    useFacebookHostSettings: boolean;
    useGoogleHostSettings: boolean;
    useMicrosoftHostSettings: boolean;
    useWsFederationHostSettings: boolean;
    useOpenIdConnectHostSettings: boolean;
    numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9];
    videoSource: any;
    myDropdownModel: number;
    taxValue: number;

    evaluationTime: number;
    points: number;
    isEvaluation: boolean;
    isLoyalityPoint: boolean;
    evaluationText: string;

    isBellOn: boolean;
    isBellContinues: boolean;
    loyalityStartDate: string;
    loyalityEndDate: string;
    @ViewChild("file")
    element: ElementRef;

    isWorkActiveSun!: boolean;
    workTextSun!: string | undefined;
    startDateSun!: any | undefined;
    endDateSun!: any | undefined;
    startDateSunSP!: any | undefined;
    endDateSunSP!: any | undefined;
    hasSPSun!: boolean;

    isWorkActiveMon!: boolean;
    workTextMon!: string | undefined;
    startDateMon!: any | undefined;
    endDateMon!: any | undefined;
    startDateMonSP!: any | undefined;
    endDateMonSP!: any | undefined;
    hasSPMon!: boolean;

    isWorkActiveTues!: boolean;
    workTextTues!: string | undefined;
    startDateTues!: any | undefined;
    endDateTues!: any | undefined;
    startDateTuesSP!: any | undefined;
    endDateTuesSP!: any | undefined;
    hasSPTues!: boolean;

    isWorkActiveWed!: boolean;
    workTextWed!: string | undefined;
    startDateWed!: any | undefined;
    endDateWed!: any | undefined;
    startDateWedSP!: any | undefined;
    endDateWedSP!: any | undefined;
    hasSPWed!: boolean;

    isWorkActiveThurs!: boolean;
    workTextThurs!: string | undefined;
    startDateThurs!: any | undefined;
    endDateThurs!: any | undefined;
    startDateThursSP!: any | undefined;
    endDateThursSP!: any | undefined;
    hasSPThurs!: boolean;

    isWorkActiveFri!: boolean;
    workTextFri!: string | undefined;
    startDateFri!: any | undefined;
    endDateFri!: any | undefined;
    startDateFriSP!: any | undefined;
    endDateFriSP!: any | undefined;
    hasSPFri!: boolean;

    isWorkActiveSat!: boolean;
    workTextSat!: string | undefined;
    startDateSat!: any | undefined;
    endDateSat!: any | undefined;
    startDateSatSP!: any | undefined;
    endDateSatSP!: any | undefined;
    hasSPSat!: boolean;

    isWorkActiveLV!: boolean;
    isWorkActiveSunLV!: boolean;
    workTextSunLV!: string | undefined;
    startDateSunLV!: any | undefined;
    endDateSunLV!: any | undefined;
    isWorkActiveMonLV!: boolean;
    workTextMonLV!: string | undefined;
    startDateMonLV!: any | undefined;
    endDateMonLV!: any | undefined;
    isWorkActiveTuesLV!: boolean;
    workTextTuesLV!: string | undefined;
    startDateTuesLV!: any | undefined;
    endDateTuesLV!: any | undefined;
    isWorkActiveWedLV!: boolean;
    workTextWedLV!: string | undefined;
    startDateWedLV!: any | undefined;
    endDateWedLV!: any | undefined;
    isWorkActiveThursLV!: boolean;
    workTextThursLV!: string | undefined;
    startDateThursLV!: any | undefined;
    endDateThursLV!: any | undefined;
    isWorkActiveFriLV!: boolean;
    workTextFriLV!: string | undefined;
    startDateFriLV!: string | undefined;
    endDateFriLV!: any | undefined;
    isWorkActiveSatLV!: boolean;
    workTextSatLV!: string | undefined;
    startDateSatLV!: any | undefined;
    endDateSatLV!: any | undefined;

    deliveryCost: any;

    orderOfferStart!: any | undefined;
    orderOfferEnd!: any | undefined;
    isHasPermissionLivChat: boolean;
    isHasPermissionOrder: boolean;
    isHasPermissionEvaluation: boolean;
    isHasPermissionCondition: boolean;
    isHasPermissionBooking: boolean;
    isHasPermissionRequest: boolean;
    unAvailableBookingDates: any = [];
    arabicCaptions = [];
    englishCaptions = [];

    startDate!: string | undefined;
    listUanvailableDates = [];
    endDate!: string | undefined;
    selectedOrderTypes: Array<any> = [];
    orderTypes = [
        { id: "0", name: "TakeAway" },
        { id: "1", name: "Delivery" },
    ];
    dropdownSettings2 = {};

    dateRange: [Date, Date];
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
        // useUtc: true,
    };

    isUstazi = false;
    ss: string;

    ee: string;
    // appSession: AppSessionService;

    wsFederationClaimMappings: { key: string; value: string }[];
    openIdConnectClaimMappings: { key: string; value: string }[];
    locationModel: DeliveryLocationInfoModel;

    @ViewChild("diagram")
    public diagram: DiagramComponent;

    public layout: LayoutModel = {
        type: "HierarchicalTree",
        verticalAlignment: "Top",

        verticalSpacing: 75,
        margin: { left: 0, right: 0, top: 0, bottom: 0 },
    };

    public shape: BasicShapeModel = {
        type: "Basic",
        shape: "Rectangle",
        cornerRadius: 10,
    };
    public snapSettings: SnapSettingsModel = {
        constraints: SnapConstraints.None,
    };

    public getNodeDefaults: Function = this.nodeDefaults.bind(this);
    public getConnectorDefaults: Function = this.connectorDefaults.bind(this);

    public setNodeTemplate: Function = this.nodeTemplate.bind(this);
    captions: NgxFlowChatData[] = [];
    caption: Caption;
    flowData: NgxFlowChatData[] = [
        {
            id: "1",
            name: "Group1",
            groupData: [
                {
                    id: "2",
                    name: "Flow1",
                },
            ],
        },

        {
            id: "4",
            name: "Group2",
            groupData: [
                {
                    id: "5",
                    name: "Flow3",
                },
                {
                    id: "6",
                    name: "Flow4",
                },
            ],
        },
    ];
    constructor(
        public darkModeService: DarkModeService,
        injector: Injector,
        private _tenantService: TenantServiceProxy,
        //private locationsServiceProxy: LocationsServiceProxy,
        private _tenantSettingsService: TenantSettingsServiceProxy,
        private _tokenService: TokenService,
        private _permissionCheckerService: PermissionCheckerService,
        private _appSessionService: AppSessionService,
        private modalService: NgbModal,
        http: HttpClient,
        private _menusServiceProxy: MenusServiceProxy,
        private _sanitizer: DomSanitizer,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
        this.http = http;
    }

    ngAfterViewInit(): void {
        setTimeout(() => {
            this.route.queryParamMap.subscribe((params) => {
                this.page = params.get("Page");
                if (this.page === "TS" && this.navVertical) {
                    this.selectTab();
                }
            });
        }, 1000);
    }

    dropdownList = [];
    selectedItems = [];
    dropdownSettings = {};
    async ngOnInit() {
        //related to google sheet integration
        window.addEventListener("message", this.handleOAuthMessage);

        let tenantId = this.appSession.tenantId;
        this._tenantSettingsService
            .googleSheetConfigGet(tenantId)
            .subscribe((response) => {
                if ( response.isConnected == true) {
                    let savedEmail = response.googleEmail;
                    this.email = savedEmail;
                    this.isConnected = true;
                }
            });

        //end

        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.minDate = new Date();
        this.minDateOffer = new Date();
        this.minDateOffer = new Date(
            this.minDateOffer.setHours(this.minDateOffer.getHours() + 1, 0, 0)
        );
        this.dropdownSettings2 = {
            singleSelection: false,
            idField: "id",
            textField: "name",
            itemsShowLimit: 2,
            allowSearchFilter: false,
            maxHeight: 200,
            closeDropDownOnSelection: true,
        };
        this.min = new Date();
        this.min = new Date(this.min.setHours(this.min.getHours() + 1, 0, 0));
        this.currency = this.appSession.tenant.currencyCode;
        this.theme = ThemeHelper.getTheme();
        this.isUstazi =
            this.appSession.tenantId == 54 ||
            this.appSession.tenantId == 62 ||
            this.appSession.tenantId == 65;
        this.isHasPermissionLivChat =
            this._permissionCheckerService.isGranted("Pages.LiveChat");
        this.isHasPermissionOrder =
            this._permissionCheckerService.isGranted("Pages.Orders");
        this.isHasPermissionEvaluation =
            this._permissionCheckerService.isGranted("Pages.Evaluation");
        this.isHasPermissionCondition =
            this._permissionCheckerService.isGranted("Pages_Offers");
        this.isHasPermissionBooking =
            this._permissionCheckerService.isGranted("Pages.Booking");
        this.isHasPermissionRequest = this._permissionCheckerService.isGranted(
            "Pages.SellingRequests"
        );
        this.imagSrc = this.appSession.tenant.image;
        this.imagBGSrc = this.appSession.tenant.imageBg;

        // alert(this.isHasPermissionLivChat);
        this.dropdownSettings = {
            singleSelection: false,
            idField: "item_id",
            itemsShowLimit: 2,
            allowSearchFilter: false,
            maxHeight: 165,
            closeDropDownOnSelection: true,
        };

        this.testEmailAddress = this.appSession.user.emailAddress;
        this.getSettings(false);
        this.initUploaders();
        this.loadSocialLoginSettings();
        // document.getElementById('appearance').onclick = this.documentClick.bind(this);
        await this.getIsAdmin();
        // Subscribe to queryParamMap to react to changes in the parameters
    }

    ngOnDestroy() {
        window.removeEventListener("message", this.handleOAuthMessage);
    }
    pushData(item: any) {
        this.settings.tenantInformation.orderOffers.push(item);
    }

    onItemSelect(item: any) {}
    onSelectAll(items: any) {}

    getCities() {}

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

        return formattedDate;
    }

    convertHijriToGregorianFullDateLoyality(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString._i.split("T");
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
    convertHijriToGregorianFullDate(hijriDateTimeString) {
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

    getSettings(isOrderOffer): void {
        this.loading = true;
        this.arabicCaptions = [];
        this.englishCaptions = [];
        this.selectedOrderTypes = [];
        this._tenantSettingsService
            .getAllSettings()
            .pipe(
                finalize(() => {
                    this.loading = false;
                })
            )
            .subscribe((result: TenantSettingsEditDto) => {
                debugger;
                this.settings = result;
                this.settings.tenantInformation.captions.forEach((caption) => {
                    if (caption.languageBotId === 1) {
                        //arabic
                        this.arabicCaptions.push(caption);
                    } else if (caption.languageBotId === 2) {
                        //english
                        this.englishCaptions.push(caption);
                    }
                });
                let year = moment(
                    this.settings.tenantInformation.loyaltyModel.startDate
                ).year();
                if (year < 2000) {
                    this.loyalityStartDate = moment(
                        this.convertHijriToGregorianFullDateLoyality(
                            moment(
                                this.settings.tenantInformation.loyaltyModel
                                    .startDate
                            )
                        )
                    )
                        .locale("en")
                        .format("YYYY-MM-DD");
                } else {
                    this.loyalityStartDate = moment(
                        this.settings.tenantInformation.loyaltyModel.startDate
                    )
                        .locale("en")
                        .format("YYYY-MM-DD");
                }

                let year2 = moment(
                    this.settings.tenantInformation.loyaltyModel.endDate
                ).year();
                if (year2 < 2000) {
                    this.loyalityEndDate = moment(
                        this.convertHijriToGregorianFullDateLoyality(
                            moment(
                                this.settings.tenantInformation.loyaltyModel
                                    .endDate
                            )
                        )
                    )
                        .locale("en")
                        .format("YYYY-MM-DD");
                } else {
                    this.loyalityEndDate = moment(
                        this.settings.tenantInformation.loyaltyModel.endDate
                    )
                        .locale("en")
                        .format("YYYY-MM-DD");
                }

                //     if(this.isArabic){
                //         if(this.loyalityStartDate && this.loyalityEndDate){
                //         this.loyalityStartDate = this.convertHijriToGregorian(moment(this.loyalityStartDate).locale('en').format('YYYY-MM-DDTHH:mm:ss'));
                //         this.loyalityEndDate = this.convertHijriToGregorian(moment(this.loyalityEndDate).locale('en').format('YYYY-MM-DDTHH:mm:ss'));
                //     }
                // }
                this.orderTypes = [
                    { id: "0", name: "TakeAway" },
                    { id: "1", name: "Delivery" },
                ];
                let selectedOrderTypes = [];
                if (
                    this.settings.tenantInformation.loyaltyModel.orderType !=
                        "" &&
                    this.settings.tenantInformation.loyaltyModel.orderType !=
                        null
                ) {
                    var array =
                        this.settings.tenantInformation.loyaltyModel.orderType.split(
                            ","
                        );
                    array.forEach((element) => {
                        var user = this.orderTypes.find((x) => x.id == element);
                        selectedOrderTypes.push(user);
                    });
                } else {
                    this.selectedOrderTypes = [];
                }
                this.selectedOrderTypes = selectedOrderTypes;
                // if (this.settings.tenantInformation.unAvailableBookingDates != "" && this.settings.tenantInformation.unAvailableBookingDates != null) {
                //     var array2 = this.settings.tenantInformation.unAvailableBookingDates.split(',')
                //     array2.forEach(element => {
                //         let x = new Date(element);
                //         this.unAvailableBookingDates.push(x);
                //     });

                // } else {
                //     this.unAvailableBookingDates = [];
                // }
                result.tenantInformation.loyaltyModel.isOverrideUpdatedPrice;

                this.deliveryCost =
                    this.settings.tenantInformation.deliveryCostTypeId.toString();
                this.isWorkActiveSun =
                    this.settings.tenantInformation.workModel.isWorkActiveSun;
                this.workTextSun =
                    this.settings.tenantInformation.workModel.workTextSun;
                this.startDateSun =
                    this.settings.tenantInformation.workModel.startDateSun;
                this.endDateSun =
                    this.settings.tenantInformation.workModel.endDateSun;
                this.hasSPSun =
                    this.settings.tenantInformation.workModel.hasSPSun;
                this.startDateSunSP =
                    this.settings.tenantInformation.workModel.startDateSunSP;
                this.endDateSunSP =
                    this.settings.tenantInformation.workModel.endDateSunSP;

                this.isWorkActiveMon =
                    this.settings.tenantInformation.workModel.isWorkActiveMon;
                this.workTextMon =
                    this.settings.tenantInformation.workModel.workTextMon;
                this.startDateMon =
                    this.settings.tenantInformation.workModel.startDateMon;
                this.endDateMon =
                    this.settings.tenantInformation.workModel.endDateMon;
                this.hasSPMon =
                    this.settings.tenantInformation.workModel.hasSPMon;
                this.startDateMonSP =
                    this.settings.tenantInformation.workModel.startDateMonSP;
                this.endDateMonSP =
                    this.settings.tenantInformation.workModel.endDateMonSP;

                this.isWorkActiveTues =
                    this.settings.tenantInformation.workModel.isWorkActiveTues;
                this.workTextTues =
                    this.settings.tenantInformation.workModel.workTextTues;
                this.startDateTues =
                    this.settings.tenantInformation.workModel.startDateTues;
                this.endDateTues =
                    this.settings.tenantInformation.workModel.endDateTues;
                this.hasSPTues =
                    this.settings.tenantInformation.workModel.hasSPTues;
                this.startDateTuesSP =
                    this.settings.tenantInformation.workModel.startDateTuesSP;
                this.endDateTuesSP =
                    this.settings.tenantInformation.workModel.endDateTuesSP;

                this.isWorkActiveWed =
                    this.settings.tenantInformation.workModel.isWorkActiveWed;
                this.workTextWed =
                    this.settings.tenantInformation.workModel.workTextWed;
                this.startDateWed =
                    this.settings.tenantInformation.workModel.startDateWed;
                this.endDateWed =
                    this.settings.tenantInformation.workModel.endDateWed;
                this.hasSPWed =
                    this.settings.tenantInformation.workModel.hasSPWed;
                this.startDateWedSP =
                    this.settings.tenantInformation.workModel.startDateWedSP;
                this.endDateWedSP =
                    this.settings.tenantInformation.workModel.endDateWedSP;

                this.isWorkActiveThurs =
                    this.settings.tenantInformation.workModel.isWorkActiveThurs;
                this.workTextThurs =
                    this.settings.tenantInformation.workModel.workTextThurs;
                this.startDateThurs =
                    this.settings.tenantInformation.workModel.startDateThurs;
                this.endDateThurs =
                    this.settings.tenantInformation.workModel.endDateThurs;
                this.hasSPThurs =
                    this.settings.tenantInformation.workModel.hasSPThurs;
                this.startDateThursSP =
                    this.settings.tenantInformation.workModel.startDateThursSP;
                this.endDateThursSP =
                    this.settings.tenantInformation.workModel.endDateThursSP;

                this.isWorkActiveFri =
                    this.settings.tenantInformation.workModel.isWorkActiveFri;
                this.workTextFri =
                    this.settings.tenantInformation.workModel.workTextFri;
                this.startDateFri =
                    this.settings.tenantInformation.workModel.startDateFri;
                this.endDateFri =
                    this.settings.tenantInformation.workModel.endDateFri;
                this.hasSPFri =
                    this.settings.tenantInformation.workModel.hasSPFri;
                this.startDateFriSP =
                    this.settings.tenantInformation.workModel.startDateFriSP;
                this.endDateFriSP =
                    this.settings.tenantInformation.workModel.endDateFriSP;

                this.isWorkActiveSat =
                    this.settings.tenantInformation.workModel.isWorkActiveSat;
                this.workTextSat =
                    this.settings.tenantInformation.workModel.workTextSat;
                this.startDateSat =
                    this.settings.tenantInformation.workModel.startDateSat;
                this.endDateSat =
                    this.settings.tenantInformation.workModel.endDateSat;
                this.hasSPSat =
                    this.settings.tenantInformation.workModel.hasSPSat;
                this.startDateSatSP =
                    this.settings.tenantInformation.workModel.startDateSatSP;
                this.endDateSatSP =
                    this.settings.tenantInformation.workModel.endDateSatSP;

                this.isLiveChatWorkActive =
                    this.settings.tenantInformation.isLiveChatWorkActive; //.isWorkActiveSun;
                this.isReplyAfterHumanHandOver =
                    this.settings.tenantInformation.isReplyAfterHumanHandOver; //.isWorkActiveSun;
                this.isWorkActiveSunLV =
                    this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveSun;
                this.workTextSunLV =
                    this.settings.tenantInformation.liveChatWorkingHours.workTextSun;
                this.startDateSunLV =
                    this.settings.tenantInformation.liveChatWorkingHours.startDateSun;
                this.endDateSunLV =
                    this.settings.tenantInformation.liveChatWorkingHours.endDateSun;

                this.isWorkActiveMonLV =
                    this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveMon;
                this.workTextMonLV =
                    this.settings.tenantInformation.liveChatWorkingHours.workTextMon;
                this.startDateMonLV =
                    this.settings.tenantInformation.liveChatWorkingHours.startDateMon;
                this.endDateMonLV =
                    this.settings.tenantInformation.liveChatWorkingHours.endDateMon;

                this.isWorkActiveTuesLV =
                    this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveTues;
                this.workTextTuesLV =
                    this.settings.tenantInformation.liveChatWorkingHours.workTextTues;
                this.startDateTuesLV =
                    this.settings.tenantInformation.liveChatWorkingHours.startDateTues;
                this.endDateTuesLV =
                    this.settings.tenantInformation.liveChatWorkingHours.endDateTues;

                this.isWorkActiveWedLV =
                    this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveWed;
                this.workTextWedLV =
                    this.settings.tenantInformation.liveChatWorkingHours.workTextWed;
                this.startDateWedLV =
                    this.settings.tenantInformation.liveChatWorkingHours.startDateWed;
                this.endDateWedLV =
                    this.settings.tenantInformation.liveChatWorkingHours.endDateWed;

                this.isWorkActiveThursLV =
                    this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveThurs;
                this.workTextThursLV =
                    this.settings.tenantInformation.liveChatWorkingHours.workTextThurs;
                this.startDateThursLV =
                    this.settings.tenantInformation.liveChatWorkingHours.startDateThurs;
                this.endDateThursLV =
                    this.settings.tenantInformation.liveChatWorkingHours.endDateThurs;

                this.isWorkActiveFriLV =
                    this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveFri;
                this.workTextFriLV =
                    this.settings.tenantInformation.liveChatWorkingHours.workTextFri;
                this.startDateFriLV =
                    this.settings.tenantInformation.liveChatWorkingHours.startDateFri;
                this.endDateFriLV =
                    this.settings.tenantInformation.liveChatWorkingHours.endDateFri;

                this.isWorkActiveSatLV =
                    this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveSat;
                this.workTextSatLV =
                    this.settings.tenantInformation.liveChatWorkingHours.workTextSat;
                this.startDateSatLV =
                    this.settings.tenantInformation.liveChatWorkingHours.startDateSat;
                this.endDateSatLV =
                    this.settings.tenantInformation.liveChatWorkingHours.endDateSat;

                this.isLiveChatWorkActive =
                    this.settings.tenantInformation.isLiveChatWorkActive;
                this.isReplyAfterHumanHandOver =
                    this.settings.tenantInformation.isReplyAfterHumanHandOver;

                this.isWorkActive =
                    this.settings.tenantInformation.isWorkActive; //.format('HH:mm')
                this.isBotActive = this.settings.tenantInformation.isBotActive; //.format('HH:mm')
                this.MassageIfBotNotActive =
                    this.settings.tenantInformation.massageIfBotNotActive;

                this.isTaxOrder =
                    this.settings.tenantInformation.isTaxOrder;

                                    this.isCancelOrder =
                    this.settings.tenantInformation.isCancelOrder;


                this.cancelTime = this.settings.tenantInformation.cancelTime;
                this.myDropdownModel =
                    this.settings.tenantInformation.cancelTime;
            this.taxValue =
                    this.settings.tenantInformation.taxValue;
                    
                this.evaluationTime =
                    this.settings.tenantInformation.evaluationTime;
                this.isEvaluation =
                    this.settings.tenantInformation.isEvaluation;
                this.isLoyalityPoint =
                    this.settings.tenantInformation.isLoyalityPoint;
                this.points = this.settings.tenantInformation.points;
                this.evaluationText =
                    this.settings.tenantInformation.evaluationText;
                this.isBellOn = this.settings.tenantInformation.isBellOn;
                this.isBellContinues =
                    this.settings.tenantInformation.isBellContinues;
                if (isOrderOffer) {
                    this.isOrderOffer = true;
                } else {
                    this.isOrderOffer =
                        this.settings.tenantInformation.isOrderOffer; //.format('HH:mm')
                }
                this.dropdownList =
                    this.settings.tenantInformation.orderOffers[0]?.area.split(
                        ","
                    );

                // this.settings.tenantInformation.orderOffers[0].selectetDate;

                this.settings.tenantInformation.orderOffers.forEach(
                    (element) => {
                        this.dateRange = [new Date(), new Date()];
                        let year1 = moment(element.orderOfferDateStart).year();
                        if (year1 < 2000) {
                            element.orderOfferDateStart = moment(
                                this.convertHijriToGregorianFullDateLoyality(
                                    moment(element.orderOfferDateStart).locale(
                                        "en"
                                    )
                                )
                            );
                        }

                        let year2 = moment(element.orderOfferDateEnd).year();
                        if (year2 < 2000) {
                            element.orderOfferDateEnd = moment(
                                this.convertHijriToGregorianFullDateLoyality(
                                    moment(element.orderOfferDateEnd).locale(
                                        "en"
                                    )
                                )
                            );
                        }
                        this.dateRange = [
                            element.orderOfferDateStart.toDate(),
                            element.orderOfferDateEnd.toDate(),
                        ];
                        element.selectetDate = this.dateRange;
                    }
                );

                if (this.settings.general) {
                    this.initialTimeZone = this.settings.general.timezone;
                    this.usingDefaultTimeZone =
                        this.settings.general.timezoneForComparison ===
                        abp.setting.values["Abp.Timing.TimeZone"];
                }
                // this.settings.tenantInformation.captions.forEach(caption =>{
                //     this.caption = new Caption();
                //     this.caption.id = caption.id.toString();
                //     if(caption.headerText != null || caption.headerText != '' || caption.headerText != undefined){
                //         this.caption.name = caption.headerText;
                //     }
                //     let text = {id: caption.textResourceId.toString() , name: caption.text};
                //     this.caption.groupData =  [new GroupData()] ;
                //     this.caption.groupData.push(text);
                //     this.captions.push(this.caption);

                // })
            });
    }

    initUploaders(): void {
        this.logoUploader = this.createUploader(
            "/TenantCustomization/UploadLogo",
            (result) => {
                this.appSession.tenant.logoFileType = result.fileType;
                this.appSession.tenant.logoId = result.id;
            }
        );

        this.customCssUploader = this.createUploader(
            "/TenantCustomization/UploadCustomCss",
            (result) => {
                this.appSession.tenant.customCssId = result.id;

                let oldTenantCustomCss =
                    document.getElementById("TenantCustomCss");
                if (oldTenantCustomCss) {
                    oldTenantCustomCss.remove();
                }

                let tenantCustomCss = document.createElement("link");
                tenantCustomCss.setAttribute("id", "TenantCustomCss");
                tenantCustomCss.setAttribute("rel", "stylesheet");
                tenantCustomCss.setAttribute(
                    "href",
                    AppConsts.remoteServiceBaseUrl +
                        "/TenantCustomization/GetCustomCss?tenantId=" +
                        this.appSession.tenant.id
                );
                document.head.appendChild(tenantCustomCss);
            }
        );
    }

    createUploader(url: string, success?: (result: any) => void): FileUploader {
        const uploader = new FileUploader({
            url: AppConsts.remoteServiceBaseUrl + url,
        });

        uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        uploader.onSuccessItem = (item, response, status) => {
            const ajaxResponse = <IAjaxResponse>JSON.parse(response);
            if (ajaxResponse.success) {
                this.notify.info(this.l("savedSuccessfully"));
                if (success) {
                    success(ajaxResponse.result);
                }
            } else {
                this.message.error(ajaxResponse.error.message);
            }
        };

        const uploaderOptions: FileUploaderOptions = {};
        uploaderOptions.authToken = "Bearer " + this._tokenService.getToken();
        uploaderOptions.removeAfterUpload = true;
        uploader.setOptions(uploaderOptions);
        return uploader;
    }

    uploadLogo(): void {
        this.logoUploader.uploadAll();
    }

    uploadCustomCss(): void {
        this.customCssUploader.uploadAll();
    }

    clearLogo(): void {
        this._tenantSettingsService.clearLogo().subscribe(() => {
            this.appSession.tenant.logoFileType = null;
            this.appSession.tenant.logoId = null;
            this.notify.info(this.l("ClearedSuccessfully"));
        });
    }

    clearCustomCss(): void {
        this._tenantSettingsService.clearCustomCss().subscribe(() => {
            this.appSession.tenant.customCssId = null;

            let oldTenantCustomCss = document.getElementById("TenantCustomCss");
            if (oldTenantCustomCss) {
                oldTenantCustomCss.remove();
            }

            this.notify.info(this.l("ClearedSuccessfully"));
        });
    }

    createRule(): void {
        this.createOrEditorderofferModal.showCreate();
    }

    mapClaims(): void {
        if (this.wsFederationClaimsMappingManager) {
            this.settings.externalLoginProviderSettings.wsFederationClaimsMapping =
                this.wsFederationClaimsMappingManager.getItems().map(
                    (item) =>
                        new JsonClaimMapDto({
                            key: item.key,
                            claim: item.value,
                        })
                );
        }

        if (this.openIdConnectClaimsMappingManager) {
            this.settings.externalLoginProviderSettings.openIdConnectClaimsMapping =
                this.openIdConnectClaimsMappingManager.getItems().map(
                    (item) =>
                        new JsonClaimMapDto({
                            key: item.key,
                            claim: item.value,
                        })
                );
        }
    }
    saveLoyality(): void {
        if (this.isLoyalityPoint) {
            if (
                this.settings.tenantInformation.loyaltyModel
                    .customerCurrencyValue === null ||
                this.settings.tenantInformation.loyaltyModel
                    .customerCurrencyValue === undefined ||
                this.settings.tenantInformation.loyaltyModel.customerPoints ===
                    null ||
                this.settings.tenantInformation.loyaltyModel.customerPoints ===
                    undefined ||
                this.settings.tenantInformation.loyaltyModel
                    .itemsCurrencyValue === null ||
                this.settings.tenantInformation.loyaltyModel
                    .itemsCurrencyValue === undefined ||
                this.settings.tenantInformation.loyaltyModel.itemsPoints ===
                    null ||
                this.settings.tenantInformation.loyaltyModel.itemsPoints ===
                    undefined ||
                this.loyalityStartDate === null ||
                this.loyalityStartDate === undefined ||
                this.loyalityEndDate === null ||
                this.loyalityEndDate === undefined ||
                this.selectedOrderTypes.length <= 0
            ) {
                this.submitted = true;
                return;
            }
        }
        this.message.confirm(
            "",
            this.l("loyaltyPointchangeNote"),
            (isConfirmed) => {
                if (isConfirmed) {
                    this.settings.tenantInformation.loyaltyModel.isLoyalityPoint =
                        this.isLoyalityPoint;
                    this.settings.tenantInformation.loyaltyModel.startDate =
                        moment(this.loyalityStartDate, "YYYY-MM-DD");
                    this.settings.tenantInformation.loyaltyModel.endDate =
                        moment(this.loyalityEndDate, "YYYY-MM-DD");
                    this.settings.tenantInformation.loyaltyModel.orderType =
                        this.selectedOrderTypes
                            .filter((f) => f.id)
                            .map(({ id }) => id)
                            .toString();
                    this._tenantSettingsService
                        .updateLoyalty(
                            this.settings.tenantInformation.loyaltyModel
                        )
                        .subscribe(
                            () => {
                                this.notify.info(this.l("savedSuccessfully"));
                            },
                            (error: any) => {
                                if (error) {
                                    this.saving = false;
                                    this.submitted = false;
                                }
                            }
                        );
                } else {
                    this.getSettings(false);
                }
            }
        );
    }
    saveAll(): void {
        this.saving = true;
        this.settings.tenantInformation.workModel.isWorkActiveSun =
            this.isWorkActiveSun;
        this.settings.tenantInformation.workModel.workTextSun =
            this.workTextSun;
        this.settings.tenantInformation.workModel.hasSPSun = this.hasSPSun;
        this.settings.tenantInformation.workModel.startDateSun = moment(
            this.startDateSun,
            "HH:mm A"
        );
        this.settings.tenantInformation.workModel.endDateSun = moment(
            this.endDateSun,
            "HH:mm A"
        );

        if (this.hasSPSun) {
            this.settings.tenantInformation.workModel.startDateSunSP = moment(
                this.startDateSunSP,
                "HH:mm A"
            );
            this.settings.tenantInformation.workModel.endDateSunSP = moment(
                this.endDateSunSP,
                "HH:mm A"
            );
        } else {
            this.settings.tenantInformation.workModel.startDateSunSP =
                this.settings.tenantInformation.workModel.startDateSun;
            this.settings.tenantInformation.workModel.endDateSunSP =
                this.settings.tenantInformation.workModel.endDateSun;
        }

        this.settings.tenantInformation.workModel.isWorkActiveMon =
            this.isWorkActiveMon;
        this.settings.tenantInformation.workModel.workTextMon =
            this.workTextMon;
        this.settings.tenantInformation.workModel.hasSPMon = this.hasSPMon;
        this.settings.tenantInformation.workModel.startDateMon = moment(
            this.startDateMon,
            "HH:mm A"
        );
        this.settings.tenantInformation.workModel.endDateMon = moment(
            this.endDateMon,
            "HH:mm A"
        );
        if (this.hasSPMon) {
            this.settings.tenantInformation.workModel.startDateMonSP = moment(
                this.startDateMonSP,
                "HH:mm A"
            );
            this.settings.tenantInformation.workModel.endDateMonSP = moment(
                this.endDateMonSP,
                "HH:mm A"
            );
        } else {
            this.settings.tenantInformation.workModel.startDateMonSP =
                this.settings.tenantInformation.workModel.startDateMon;
            this.settings.tenantInformation.workModel.endDateMonSP =
                this.settings.tenantInformation.workModel.endDateMon;
        }

        this.settings.tenantInformation.workModel.isWorkActiveTues =
            this.isWorkActiveTues;
        this.settings.tenantInformation.workModel.workTextTues =
            this.workTextTues;
        this.settings.tenantInformation.workModel.hasSPTues = this.hasSPTues;
        this.settings.tenantInformation.workModel.startDateTues = moment(
            this.startDateTues,
            "HH:mm A"
        );
        this.settings.tenantInformation.workModel.endDateTues = moment(
            this.endDateTues,
            "HH:mm A"
        );
        if (this.hasSPTues) {
            this.settings.tenantInformation.workModel.startDateTuesSP = moment(
                this.startDateTuesSP,
                "HH:mm A"
            );
            this.settings.tenantInformation.workModel.endDateTuesSP = moment(
                this.endDateTuesSP,
                "HH:mm A"
            );
        } else {
            this.settings.tenantInformation.workModel.startDateTuesSP =
                this.settings.tenantInformation.workModel.startDateTues;
            this.settings.tenantInformation.workModel.endDateTuesSP =
                this.settings.tenantInformation.workModel.endDateTues;
        }

        this.settings.tenantInformation.workModel.isWorkActiveWed =
            this.isWorkActiveWed;
        this.settings.tenantInformation.workModel.workTextWed =
            this.workTextWed;
        this.settings.tenantInformation.workModel.hasSPWed = this.hasSPWed;
        this.settings.tenantInformation.workModel.startDateWed = moment(
            this.startDateWed,
            "HH:mm A"
        );
        this.settings.tenantInformation.workModel.endDateWed = moment(
            this.endDateWed,
            "HH:mm A"
        );
        if (this.hasSPWed) {
            this.settings.tenantInformation.workModel.startDateWedSP = moment(
                this.startDateWedSP,
                "HH:mm A"
            );
            this.settings.tenantInformation.workModel.endDateWedSP = moment(
                this.endDateWedSP,
                "HH:mm A"
            );
        } else {
            this.settings.tenantInformation.workModel.startDateWedSP =
                this.settings.tenantInformation.workModel.startDateWed;
            this.settings.tenantInformation.workModel.endDateWedSP =
                this.settings.tenantInformation.workModel.endDateWed;
        }

        this.settings.tenantInformation.workModel.isWorkActiveThurs =
            this.isWorkActiveThurs;
        this.settings.tenantInformation.workModel.workTextThurs =
            this.workTextThurs;
        this.settings.tenantInformation.workModel.hasSPThurs = this.hasSPThurs;
        this.settings.tenantInformation.workModel.startDateThurs = moment(
            this.startDateThurs,
            "HH:mm A"
        );
        this.settings.tenantInformation.workModel.endDateThurs = moment(
            this.endDateThurs,
            "HH:mm A"
        );
        if (this.hasSPThurs) {
            this.settings.tenantInformation.workModel.startDateThursSP = moment(
                this.startDateThursSP,
                "HH:mm A"
            );
            this.settings.tenantInformation.workModel.endDateThursSP = moment(
                this.endDateThursSP,
                "HH:mm A"
            );
        } else {
            this.settings.tenantInformation.workModel.startDateThursSP =
                this.settings.tenantInformation.workModel.startDateThurs;
            this.settings.tenantInformation.workModel.endDateThursSP =
                this.settings.tenantInformation.workModel.endDateThurs;
        }

        this.settings.tenantInformation.workModel.isWorkActiveFri =
            this.isWorkActiveFri;
        this.settings.tenantInformation.workModel.workTextFri =
            this.workTextFri;
        this.settings.tenantInformation.workModel.hasSPFri = this.hasSPFri;
        this.settings.tenantInformation.workModel.startDateFri = moment(
            this.startDateFri,
            "HH:mm A"
        );
        this.settings.tenantInformation.workModel.endDateFri = moment(
            this.endDateFri,
            "HH:mm A"
        );
        if (this.hasSPFri) {
            this.settings.tenantInformation.workModel.startDateFriSP = moment(
                this.startDateFriSP,
                "HH:mm A"
            );
            this.settings.tenantInformation.workModel.endDateFriSP = moment(
                this.endDateFriSP,
                "HH:mm A"
            );
        } else {
            this.settings.tenantInformation.workModel.startDateFriSP =
                this.settings.tenantInformation.workModel.startDateFri;
            this.settings.tenantInformation.workModel.endDateFriSP =
                this.settings.tenantInformation.workModel.endDateFri;
        }

        this.settings.tenantInformation.workModel.isWorkActiveSat =
            this.isWorkActiveSat;
        this.settings.tenantInformation.workModel.workTextSat =
            this.workTextSat;
        this.settings.tenantInformation.workModel.hasSPSat = this.hasSPSat;
        this.settings.tenantInformation.workModel.startDateSat = moment(
            this.startDateSat,
            "HH:mm A"
        );
        this.settings.tenantInformation.workModel.endDateSat = moment(
            this.endDateSat,
            "HH:mm A"
        );
        if (this.hasSPSat) {
            this.settings.tenantInformation.workModel.startDateSatSP = moment(
                this.startDateSatSP,
                "HH:mm A"
            );
            this.settings.tenantInformation.workModel.endDateSatSP = moment(
                this.endDateSatSP,
                "HH:mm A"
            );
        } else {
            this.settings.tenantInformation.workModel.startDateSatSP =
                this.settings.tenantInformation.workModel.startDateSat;
            this.settings.tenantInformation.workModel.endDateSatSP =
                this.settings.tenantInformation.workModel.endDateSat;
        }

        // this.settings.tenantInformation.isLiveChatWorkActive=this.isWorkActiveLV;
        this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveSun =
            this.isWorkActiveSunLV;
        this.settings.tenantInformation.liveChatWorkingHours.workTextSun =
            this.workTextSunLV;
        this.settings.tenantInformation.liveChatWorkingHours.startDateSun =
            moment(this.startDateSunLV, "HH:mm A");
        this.settings.tenantInformation.liveChatWorkingHours.endDateSun =
            moment(this.endDateSunLV, "HH:mm A");

        this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveMon =
            this.isWorkActiveMonLV;
        this.settings.tenantInformation.liveChatWorkingHours.workTextMon =
            this.workTextMonLV;

        this.settings.tenantInformation.liveChatWorkingHours.startDateMon =
            moment(this.startDateMonLV, "HH:mm A");

        this.settings.tenantInformation.liveChatWorkingHours.endDateMon =
            moment(this.endDateMonLV, "HH:mm A");

        this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveTues =
            this.isWorkActiveTuesLV;
        this.settings.tenantInformation.liveChatWorkingHours.workTextTues =
            this.workTextTuesLV;
        this.settings.tenantInformation.liveChatWorkingHours.startDateTues =
            moment(this.startDateTuesLV, "HH:mm A");
        this.settings.tenantInformation.liveChatWorkingHours.endDateTues =
            moment(this.endDateTuesLV, "HH:mm A");

        this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveWed =
            this.isWorkActiveWedLV;
        this.settings.tenantInformation.liveChatWorkingHours.workTextWed =
            this.workTextWedLV;
        this.settings.tenantInformation.liveChatWorkingHours.startDateWed =
            moment(this.startDateWedLV, "HH:mm A");
        this.settings.tenantInformation.liveChatWorkingHours.endDateWed =
            moment(this.endDateWedLV, "HH:mm A");

        this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveThurs =
            this.isWorkActiveThursLV;
        this.settings.tenantInformation.liveChatWorkingHours.workTextThurs =
            this.workTextThursLV;
        this.settings.tenantInformation.liveChatWorkingHours.startDateThurs =
            moment(this.startDateThursLV, "HH:mm A");
        this.settings.tenantInformation.liveChatWorkingHours.endDateThurs =
            moment(this.endDateThursLV, "HH:mm A");

        this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveFri =
            this.isWorkActiveFriLV;
        this.settings.tenantInformation.liveChatWorkingHours.workTextFri =
            this.workTextFriLV;
        this.settings.tenantInformation.liveChatWorkingHours.startDateFri =
            moment(this.startDateFriLV, "HH:mm A");
        this.settings.tenantInformation.liveChatWorkingHours.endDateFri =
            moment(this.endDateFriLV, "HH:mm A");

        this.settings.tenantInformation.liveChatWorkingHours.isWorkActiveSat =
            this.isWorkActiveSatLV;
        this.settings.tenantInformation.liveChatWorkingHours.workTextSat =
            this.workTextSatLV;

        this.settings.tenantInformation.liveChatWorkingHours.startDateSat =
            moment(this.startDateSatLV, "HH:mm A");
        this.settings.tenantInformation.liveChatWorkingHours.endDateSat =
            moment(this.endDateSatLV, "HH:mm A");

        this.settings.tenantInformation.cancelTime = this.myDropdownModel;
        this.settings.tenantInformation.taxValue = this.taxValue;

        this.settings.tenantInformation.evaluationTime = this.evaluationTime;
        this.settings.tenantInformation.isEvaluation = this.isEvaluation;
        this.settings.tenantInformation.isLoyalityPoint = this.isLoyalityPoint;
        this.settings.tenantInformation.points = this.points;
        this.settings.tenantInformation.evaluationText = this.evaluationText;

        this.settings.tenantInformation.isCancelOrder = this.isCancelOrder;
        this.settings.tenantInformation.isTaxOrder = this.isTaxOrder;


        this.settings.tenantInformation.isBotActive = this.isBotActive;
        this.settings.tenantInformation.massageIfBotNotActive =
            this.MassageIfBotNotActive;
        this.settings.tenantInformation.isWorkActive = this.isWorkActive;
        this.settings.tenantInformation.isBellOn = this.isBellOn;
        this.settings.tenantInformation.isBellContinues = this.isBellContinues;
        this.settings.tenantInformation.isLiveChatWorkActive =
            this.isLiveChatWorkActive;
        this.settings.tenantInformation.isReplyAfterHumanHandOver =
            this.isReplyAfterHumanHandOver;

        this.settings.tenantInformation.deliveryCostTypeId = this.deliveryCost;
        this.mapClaims();

        if (this.isWorkActive) {
            if (this.isWorkActiveSat) {
                if (
                    this.startDateSat >= this.endDateSat ||
                    this.endDateSat <= this.startDateSat
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInSaturday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveSat && this.hasSPSat) {
                if (
                    (this.startDateSatSP >= this.startDateSat &&
                        this.startDateSatSP) <= this.endDateSat ||
                    (this.endDateSatSP <= this.endDateSat &&
                        this.endDateSatSP) >= this.startDateSat ||
                    this.startDateSatSP >= this.endDateSatSP ||
                    this.endDateSatSP <= this.startDateSatSP ||
                    (this.startDateSat <= this.endDateSatSP &&
                        this.endDateSat >= this.startDateSatSP)
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInSaturday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveSun) {
                if (
                    this.startDateSun >= this.endDateSun ||
                    this.endDateSun <= this.startDateSun
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInSunday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveSun && this.hasSPSun) {
                if (
                    (this.startDateSunSP >= this.startDateSun &&
                        this.startDateSunSP) <= this.endDateSun ||
                    (this.endDateSunSP <= this.endDateSun &&
                        this.endDateSunSP) >= this.startDateSun ||
                    this.startDateSunSP >= this.endDateSunSP ||
                    this.endDateSunSP <= this.startDateSunSP ||
                    (this.startDateSun <= this.endDateSunSP &&
                        this.endDateSun >= this.startDateSunSP)
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInSunday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveMon) {
                if (
                    this.startDateMon >= this.endDateMon ||
                    this.endDateMon <= this.startDateMon
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInMonday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveMon && this.hasSPMon) {
                if (
                    (this.startDateMonSP >= this.startDateMon &&
                        this.startDateMonSP) <= this.endDateMon ||
                    (this.endDateMonSP <= this.endDateMon &&
                        this.endDateMonSP) >= this.startDateMon ||
                    this.startDateMonSP >= this.endDateMonSP ||
                    this.endDateMonSP <= this.startDateMonSP ||
                    (this.startDateMon <= this.endDateMonSP &&
                        this.endDateMon >= this.startDateMonSP)
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInMonday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveTues) {
                if (
                    this.startDateTues >= this.endDateTues ||
                    this.endDateTues <= this.startDateTues
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInTuesday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveTues && this.hasSPTues) {
                if (
                    (this.startDateTuesSP >= this.startDateTues &&
                        this.startDateTuesSP) <= this.endDateTues ||
                    (this.endDateTuesSP <= this.endDateTues &&
                        this.endDateTuesSP) >= this.startDateTues ||
                    this.startDateTuesSP >= this.endDateTuesSP ||
                    this.endDateTuesSP <= this.startDateTuesSP ||
                    (this.startDateTues <= this.endDateTuesSP &&
                        this.endDateTues >= this.startDateTuesSP)
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInTuesday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveWed) {
                if (
                    this.startDateWed >= this.endDateWed ||
                    this.endDateWed <= this.startDateWed
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInWednesday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveWed && this.hasSPWed) {
                if (
                    (this.startDateWedSP >= this.startDateWed &&
                        this.startDateWedSP) <= this.endDateWed ||
                    (this.endDateWedSP <= this.endDateWed &&
                        this.endDateWedSP) >= this.startDateWed ||
                    this.startDateWedSP >= this.endDateWedSP ||
                    this.endDateWedSP <= this.startDateWedSP ||
                    (this.startDateWed <= this.endDateWedSP &&
                        this.endDateWed >= this.startDateWedSP)
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInWednesday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveThurs) {
                if (
                    this.startDateThurs >= this.endDateThurs ||
                    this.endDateThurs <= this.startDateThurs
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInThursady")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveThurs && this.hasSPThurs) {
                if (
                    (this.startDateThursSP >= this.startDateThurs &&
                        this.startDateThursSP) <= this.endDateThurs ||
                    (this.endDateThursSP <= this.endDateThurs &&
                        this.endDateThursSP) >= this.startDateThurs ||
                    this.startDateThursSP >= this.endDateThursSP ||
                    this.endDateThursSP <= this.startDateThursSP ||
                    (this.startDateThurs <= this.endDateThursSP &&
                        this.endDateThurs >= this.startDateThursSP)
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInThursady")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveFri) {
                if (
                    this.startDateFri >= this.endDateFri ||
                    this.endDateFri <= this.startDateFri
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInFriday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveFri && this.hasSPFri) {
                if (
                    (this.startDateFriSP >= this.startDateFri &&
                        this.startDateFriSP) <= this.endDateFri ||
                    (this.endDateFriSP <= this.endDateFri &&
                        this.endDateFriSP) >= this.startDateFri ||
                    this.startDateFriSP >= this.endDateFriSP ||
                    this.endDateFriSP <= this.startDateFriSP ||
                    (this.startDateFri <= this.endDateFriSP &&
                        this.endDateFri >= this.startDateFriSP)
                ) {
                    this.message.error(
                        this.l("workingHoursError"),
                        this.l("invalidDateInFriday")
                    );
                    this.saving = false;
                    return;
                }
            }
        }

        //Live Chat Working Hours

        if (this.isLiveChatWorkActive) {
            if (this.isWorkActiveSatLV) {
                if (this.startDateSatLV >= this.endDateSatLV) {
                    this.message.error(
                        this.l("liveChatHoursError"),
                        this.l("invalidDateInSaturday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveSunLV) {
                if (this.startDateSunLV >= this.endDateSunLV) {
                    this.message.error(
                        this.l("liveChatHoursError"),
                        this.l("invalidDateInSunday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveMonLV) {
                if (this.startDateMonLV >= this.endDateMonLV) {
                    this.message.error(
                        this.l("liveChatHoursError"),
                        this.l("invalidDateInMonday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveTuesLV) {
                if (this.startDateTuesLV >= this.endDateTuesLV) {
                    this.message.error(
                        this.l("liveChatHoursError"),
                        this.l("invalidDateInTuesday")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveWedLV) {
                if (this.startDateWedLV >= this.endDateWedLV) {
                    this.message.error(
                        this.l("liveChatHoursError"),
                        this.l("invalidDateInWednesday")
                    );
                    this.saving = false;
                    return;
                }
            }
            if (this.isWorkActiveThursLV) {
                if (this.startDateThursLV >= this.endDateThursLV) {
                    this.message.error(
                        this.l("liveChatHoursError"),
                        this.l("invalidDateInThursady")
                    );
                    this.saving = false;
                    return;
                }
            }

            if (this.isWorkActiveFriLV) {
                if (this.startDateFriLV >= this.endDateFriLV) {
                    this.message.error(
                        this.l("liveChatHoursError"),
                        this.l("invalidDateInFriday")
                    );
                    this.saving = false;
                    return;
                }
            }
        }

        //Condition
        this.settings.tenantInformation.isOrderOffer = this.isOrderOffer;
        this.settings.tenantInformation.orderOffers.forEach((element) => {
            let year = moment(element.selectetDate[0]).year();
            if (year < 2000) {
                element.orderOfferDateStart = moment(
                    this.convertHijriToGregorianFullDate(
                        moment(element.selectetDate[0])
                            .locale("en")
                            .format("YYYY-MM-DDTHH:mm:ss")
                    )
                );
            } else {
                element.orderOfferDateStart = moment(
                    element.selectetDate[0],
                    "DD/MM/YYYY"
                );
            }
            let year2 = moment(element.selectetDate[1]).year();
            if (year2 < 2000) {
                element.orderOfferDateEnd = moment(
                    this.convertHijriToGregorianFullDate(
                        moment(element.selectetDate[1])
                            .locale("en")
                            .format("YYYY-MM-DDTHH:mm:ss")
                    )
                );
            } else {
                element.orderOfferDateEnd = moment(
                    element.selectetDate[1],
                    "DD/MM/YYYY"
                );
            }
            element.selectetDate = null;
        });
        this.settings.tenantInformation.orderOffers.forEach((element) => {
            let startTime = moment(element.orderOfferStartS, "HH:mm A").locale(
                "en"
            );
            let endTime = moment(element.orderOfferEndS, "HH:mm A").locale(
                "en"
            );
            element.orderOfferStart = moment(element.orderOfferDateStart)
                .locale("en")
                .set("hour", startTime.hours())
                .set("minute", startTime.minutes());
            element.orderOfferEnd = moment(element.orderOfferDateEnd)
                .locale("en")
                .set("hour", endTime.hours())
                .set("minute", endTime.minutes());
        });
        // if(this.unAvailableBookingDates.length >= 1){
        //     this.unAvailableBookingDates.forEach(unAvailableBookingDate => {
        //     let date = moment(unAvailableBookingDate).format("MM/DD/yyyy");
        //     this.listUanvailableDates.push(date);
        //     })
        // }

        let year3 = moment(
            this.settings.tenantInformation.loyaltyModel.createdDate
        ).year();
        if (year3 < 2000) {
            this.settings.tenantInformation.loyaltyModel.createdDate = moment(
                this.convertHijriToGregorianFullDateLoyality(
                    moment(
                        this.settings.tenantInformation.loyaltyModel.createdDate
                    )
                )
            ).locale("en");
        } else {
            this.settings.tenantInformation.loyaltyModel.createdDate = moment(
                this.settings.tenantInformation.loyaltyModel.createdDate
            ).locale("en");
        }

        let year = moment(
            this.settings.tenantInformation.loyaltyModel.startDate
        ).year();
        if (year < 2000) {
            this.settings.tenantInformation.loyaltyModel.startDate = moment(
                this.convertHijriToGregorianFullDateLoyality(
                    moment(
                        this.settings.tenantInformation.loyaltyModel.startDate
                    )
                )
            ).locale("en");
        } else {
            this.settings.tenantInformation.loyaltyModel.startDate = moment(
                this.settings.tenantInformation.loyaltyModel.startDate
            ).locale("en");
        }

        let year2 = moment(
            this.settings.tenantInformation.loyaltyModel.endDate
        ).year();
        if (year2 < 2000) {
            this.settings.tenantInformation.loyaltyModel.endDate = moment(
                this.convertHijriToGregorianFullDateLoyality(
                    moment(this.settings.tenantInformation.loyaltyModel.endDate)
                )
            ).locale("en");
        } else {
            this.settings.tenantInformation.loyaltyModel.endDate = moment(
                this.settings.tenantInformation.loyaltyModel.endDate
            ).locale("en");
        }
        if (
            this.settings.tenantInformation.timeReminder === null ||
            this.settings.tenantInformation.timeReminder === undefined
        ) {
            this.settings.tenantInformation.timeReminder = 0;
        }
        if (this.isHasPermissionOrder && this.isBotActive) {
            if (
                !this.settings.tenantInformation.isBotLanguageAr &&
                !this.settings.tenantInformation.isBotLanguageEn
            ) {
                this.settings.tenantInformation.isBotLanguageAr = true;
            }

            if (
                !this.settings.tenantInformation.isMenuLinkFirst &&
                !this.settings.tenantInformation.isDelivery &&
                !this.settings.tenantInformation.isPickup &&
                !this.settings.tenantInformation.isPreOrder &&
                !this.settings.tenantInformation.isInquiry
            ) {
                this.settings.tenantInformation.isDelivery = true;
                this.settings.tenantInformation.isPickup = true;
            }
        }

        // this.settings.tenantInformation.unAvailableBookingDates = this.listUanvailableDates.join();
        this._tenantSettingsService.updateAllSettings(this.settings).subscribe(
            () => {
                // this._tenantService.getTenantForEdit(this.appSession.tenant.id).subscribe((tenantResult) => {
                //     tenantResult.isBellOn = this.isBellOn;
                //     tenantResult.isBellContinues = this.isBellContinues;
                //     this._tenantService.updateTenant(tenantResult)
                //         .subscribe(() => {
                this.getSettings(false);

                this.settings.tenantInformation.orderOffers.forEach(
                    (element) => {
                        this.dateRange = [
                            element.orderOfferDateStart.toDate(),
                            element.orderOfferDateEnd.toDate(),
                        ];
                        element.selectetDate = this.dateRange;
                    }
                );
                this.notify.info(this.l("savedSuccessfully"));
                this.saving = false;

                if (
                    abp.clock.provider.supportsMultipleTimezone &&
                    this.usingDefaultTimeZone &&
                    this.initialTimeZone !== this.settings.general.timezone
                ) {
                    this.message
                        .info(
                            this.l(
                                "TimeZoneSettingChangedRefreshPageNotification"
                            )
                        )
                        .then(() => {
                            window.location.reload();
                        });
                }
            },
            (error: any) => {
                if (error) {
                    this.saving = false;
                }
            }
        );
        //}
        //);
        //});
    }

    saveCaption() {
        this.settings.tenantInformation.captions = this.arabicCaptions.concat(
            this.englishCaptions
        );
        this._tenantSettingsService
            .updateCaption(this.settings.tenantInformation.captions)
            .subscribe(
                () => {
                    this.notify.info(this.l("savedSuccessfully"));
                    this.saving = false;
                },
                (error: any) => {
                    if (error) {
                        this.saving = false;
                    }
                }
            );
    }

    viewloyaltyVideo(modalBasic, video) {
        this.modalOpen2(modalBasic);
        this.videoSource =
            this._sanitizer.bypassSecurityTrustResourceUrl(video);
    }
    modalOpen2(modalBasic) {
        this.modalService.open(modalBasic, {
            windowClass: "modal",
            centered: true,
            size: "lg",
        });
    }
    // saveOrderOffers() {
    //     this.settings.tenantInformation.isOrderOffer= this.isOrderOffer;
    //     this.settings.tenantInformation.orderOffers.forEach(element => {
    //         element.orderOfferDateStart = moment(element.selectetDate[0], 'DD/MM/YYYY');
    //         element.orderOfferDateEnd = moment(element.selectetDate[1], 'DD/MM/YYYY');

    //         element.selectetDate = null;
    //     });
    //     this._tenantSettingsService.updateOrderOffers(this.settings.tenantInformation).subscribe(() => {

    //         this.notify.info(this.l('savedSuccessfully'));
    //         this.saving = false;
    //         this.getSettings(false);
    //     },
    //     (error: any) => {
    //         if (error) {
    //             this.saving = false;
    //         }
    //     });
    // }
    onDateRangeUpdate = () => {};

    sendTestEmail(): void {
        const input = new SendTestEmailInput();
        input.emailAddress = this.testEmailAddress;
        this._tenantSettingsService.sendTestEmail(input).subscribe((result) => {
            this.notify.info(this.l("TestEmailSentSuccessfully"));
        });
    }

    loadSocialLoginSettings(): void {
        const self = this;
        this._tenantSettingsService
            .getEnabledSocialLoginSettings()
            .subscribe((setting) => {
                self.enabledSocialLoginSettings =
                    setting.enabledSocialLoginSettings;
            });
    }

    clearFacebookSettings(): void {}

    clearGoogleSettings(): void {
        this.settings.externalLoginProviderSettings.google.clientId = "";
        this.settings.externalLoginProviderSettings.google.clientSecret = "";
        this.settings.externalLoginProviderSettings.google.userInfoEndpoint =
            "";
    }

    clearMicrosoftSettings(): void {
        this.settings.externalLoginProviderSettings.microsoft.clientId = "";
        this.settings.externalLoginProviderSettings.microsoft.clientSecret = "";
    }

    clearWsFederationSettings(): void {
        this.settings.externalLoginProviderSettings.wsFederation.clientId = "";
        this.settings.externalLoginProviderSettings.wsFederation.authority = "";
        this.settings.externalLoginProviderSettings.wsFederation.wtrealm = "";
        this.settings.externalLoginProviderSettings.wsFederation.metaDataAddress =
            "";
        this.settings.externalLoginProviderSettings.wsFederation.tenant = "";
        this.settings.externalLoginProviderSettings.wsFederationClaimsMapping =
            [];
    }

    clearOpenIdSettings(): void {
        this.settings.externalLoginProviderSettings.openIdConnect.clientId = "";
        this.settings.externalLoginProviderSettings.openIdConnect.clientSecret =
            "";
        this.settings.externalLoginProviderSettings.openIdConnect.authority =
            "";
        this.settings.externalLoginProviderSettings.openIdConnect.loginUrl = "";
        this.settings.externalLoginProviderSettings.openIdConnectClaimsMapping =
            [];
    }

    onRemove(orderoffer: any): void {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._tenantSettingsService
                    .deleteOrderOffer(orderoffer.id)
                    .subscribe(() => {
                        const index =
                            this.settings.tenantInformation.orderOffers.indexOf(
                                orderoffer
                            );

                        if (index > -1) {
                            this.settings.tenantInformation.orderOffers.splice(
                                index,
                                1
                            );
                        }
                        this.notify.success(this.l("SuccessfullyDeleted"));
                    });
            }
        });
    }

    private nodeDefaults(node: NodeModel, diagram: Diagram): NodeModel {
        let obj: NodeModel = {};
        if (node.id !== "node1") {
            //Set ports
            obj.ports = this.getPorts(node);
        }
        if (node.id !== "node6") {
            obj.width = 80;
            obj.style = { strokeWidth: 2, strokeColor: "#6F409F" };
            obj.height = 35;
        }
        return obj;
    }

    private connectorDefaults(obj: ConnectorModel): void {
        obj.type = "Bezier";
        obj.style.strokeColor = "#6f409f";
        obj.style.strokeWidth = 2;
        obj.targetDecorator = {
            style: { strokeColor: "#6f409f", fill: "#6f409f" },
        };
    }

    public created(): void {
        this.diagram.fitToPage();
    }

    private nodeTemplate(node: NodeModel): StackPanel {
        if (node.id === "node6") {
            let canvas: StackPanel = new StackPanel();
            canvas.id = randomId();
            canvas.children = [
                this.getTextElement("Events", "#a6a1e0"),
                this.getTextElement("Emails", "#db8ec9"),
                this.getTextElement("Calls", "#db8ec9"),
                this.getTextElement("Smart Contents", "#db8ec9"),
            ];
            canvas.style.strokeWidth = 0;
            canvas.style.fill = "#e6e0eb";
            return canvas;
        }
        return null;
    }

    private getPorts(obj: NodeModel): PointPortModel[] {
        if (obj.id === "node2") {
            let node2Ports: PointPortModel[] = [
                {
                    id: "port1",
                    offset: { x: 1, y: 0.25 },
                    visibility: PortVisibility.Hidden,
                },
                {
                    id: "port2",
                    offset: { x: 1, y: 0.5 },
                    visibility: PortVisibility.Hidden,
                },
                {
                    id: "port3",
                    offset: { x: 1, y: 0.75 },
                    visibility: PortVisibility.Hidden,
                },
            ];
            return node2Ports;
        } else if (obj.id === "node6") {
            let node6Ports: PointPortModel[] = [
                {
                    id: "port4",
                    offset: { x: 0, y: 0.46 },
                    visibility: PortVisibility.Hidden,
                },
                {
                    id: "port5",
                    offset: { x: 0, y: 0.5 },
                    visibility: PortVisibility.Hidden,
                },
                {
                    id: "port6",
                    offset: { x: 0, y: 0.54 },
                    visibility: PortVisibility.Hidden,
                },
            ];
            return node6Ports;
        } else {
            let ports: PointPortModel[] = [
                {
                    id: "portIn",
                    offset: { x: 0, y: 0.5 },
                    visibility: PortVisibility.Hidden,
                },
                {
                    id: "portOut",
                    offset: { x: 1, y: 0.5 },
                    visibility: PortVisibility.Hidden,
                },
            ];
            return ports;
        }
    }

    private getTextElement(text: string, color: string): TextElement {
        let textElement: TextElement = new TextElement();
        textElement.id = randomId();
        textElement.width = 80;
        textElement.height = 35;
        textElement.content = text;
        textElement.style.fill = "#6f409f";
        textElement.style.color = "white";
        textElement.style.strokeColor = "#6f409f";
        textElement.cornerRadius = 5;
        textElement.margin = { top: 10, bottom: 10, left: 10, right: 10 };
        textElement.relativeMode = "Object";
        return textElement;
    }

    public onChangeLock(args: CheckBoxChangeEventArgs): void {
        for (let i: number = 0; i < this.diagram.nodes.length; i++) {
            let node: NodeModel = this.diagram.nodes[i];
            if (args.checked) {
                node.constraints =
                    NodeConstraints.PointerEvents | NodeConstraints.Select;
            } else {
                node.constraints =
                    NodeConstraints.Default & ~NodeConstraints.ReadOnly;
            }
            this.diagram.dataBind();
        }
        for (let i: number = 0; i < this.diagram.connectors.length; i++) {
            let connector: ConnectorModel = this.diagram.connectors[i];
            if (args.checked) {
                connector.constraints =
                    ConnectorConstraints.PointerEvents |
                    ConnectorConstraints.Select;
            } else {
                connector.constraints =
                    ConnectorConstraints.Default &
                    ~ConnectorConstraints.ReadOnly;
            }
            this.diagram.dataBind();
        }
    }
    private documentClick(args: MouseEvent): void {
        let target: HTMLElement = args.target as HTMLElement;
        // custom code start
        let selectedElement: HTMLCollection =
            document.getElementsByClassName("e-selected-style");
        if (selectedElement.length) {
            selectedElement[0].classList.remove("e-selected-style");
        }
        // custom code end
        if (target.className === "image-pattern-style") {
            switch (target.id) {
                case "straightConnector":
                    this.applyConnectorStyle(
                        false,
                        false,
                        false,
                        "Straight",
                        1
                    );
                    break;
                case "orthogonalConnector":
                    this.applyConnectorStyle(
                        false,
                        false,
                        false,
                        "Orthogonal",
                        1
                    );
                    break;
                case "bezierConnector":
                    this.applyConnectorStyle(false, false, false, "Bezier", 1);
                    break;
                case "straightConnectorWithStroke":
                    this.applyConnectorStyle(false, false, false, "Straight");
                    break;
                case "orthogonalConnectorWithStroke":
                    this.applyConnectorStyle(false, false, false, "Orthogonal");
                    break;
                case "bezierConnectorWithStroke":
                    this.applyConnectorStyle(false, false, false, "Bezier");
                    break;
                case "straightConnectorWithDasharray":
                    this.applyConnectorStyle(true, false, false, "Straight");
                    break;
                case "orthogonalConnectorWithDasharray":
                    this.applyConnectorStyle(true, false, false, "Orthogonal");
                    break;
                case "bezierConnectorWithDasharray":
                    this.applyConnectorStyle(true, false, false, "Bezier");
                    break;
                case "cornerRadious":
                    this.applyConnectorStyle(false, false, true, "Orthogonal");
                    break;
                case "sourceDecorator":
                    this.applyConnectorStyle(false, true, false, "Straight");
                    break;
                case "sourceDecoratorWithDasharray":
                    this.applyConnectorStyle(true, true, false, "Straight");
                    break;
            }
            // custom code start
            target.classList.add("e-selected-style");
            // custom code end
        }
    }
    private applyConnectorStyle(
        dashedLine: boolean,
        sourceDec: boolean,
        isRounded: boolean,
        type: Segments,
        strokeWidth?: number
    ): void {
        for (let i: number = 0; i < this.diagram.connectors.length; i++) {
            this.diagram.connectors[i].style.strokeWidth = strokeWidth
                ? strokeWidth
                : 2;
            this.diagram.connectors[i].type = type;
            this.diagram.connectors[i].cornerRadius = isRounded ? 5 : 0;
            this.diagram.connectors[i].style.strokeDashArray = dashedLine
                ? "5,5"
                : "";
            if (sourceDec) {
                this.diagram.connectors[i].sourceDecorator = {
                    style: {
                        strokeColor: "#6f409f",
                        fill: "#6f409f",
                        strokeWidth: 2,
                    },
                    shape: "Circle",
                };
            } else {
                this.diagram.connectors[i].sourceDecorator = { shape: "None" };
            }
            this.diagram.connectors[i].targetDecorator = {
                style: {
                    strokeColor: "#6f409f",
                    fill: "#6f409f",
                    strokeWidth: 2,
                },
                shape: "Arrow",
            };
            this.diagram.dataBind();
        }
    }

    flowOptions: NgxFlowChatOptions = {
        groupBorderRadius: "3px",
        groupTextColor: "#000",
        background: "#009789",
        shadow: "0 2px 4px 0 #333",
        borderRadius: "5px",
        textColor: "#fff",
        width: "200px",
    };

    clickN(event) {
        // console.log(event);
    }

    onFileChange1(event, modalBasic) {
        if (event.target.files[0]) {
            if (
                event.target.files[0].type === "image/jpeg" ||
                event.target.files[0].type === "image/png" ||
                event.target.files[0].type === "image/jpg"
            ) {
                this.modalOpen(modalBasic);
                this.imageChangedEvent = event;
            } else {
                this.message.error("", this.l("You cant upload this file"));
                this.element.nativeElement.value = "";
            }
        }
    }

    imageCroppedFile(event: ImageCroppedEvent) {
        // Determine MIME type from base64 string
        const mimeType = this.extractMimeType(event.base64);
        const filename =
            "cropped-image" +
            `${new Date().getTime()}` +
            this.getExtensionFromMimeType(mimeType);

        // Convert base64 string to File object
        const file = this.base64ToFile(event.base64, filename, mimeType);

        // Create FormData object and append the file
        const form = new FormData();
        form.append("FormFile", file);

        // Store the FormData object
        this.file = form;
    }

    modalOpen(modalBasic) {
        this.modalService.open(modalBasic, {
            windowClass: "modal",
        });
    }

    saveImage() {
        this.loadLogoImage = true;
        this.http
            .post<WhatsAppHeaderUrl>(
                AppConsts.remoteServiceBaseUrl +
                    "/api/services/app/General/GetInfoSeedUrlFile",
                this.file
            )
            .subscribe(
                (result) => {
                    this.imagSrc = result.infoSeedUrl;
                    this._menusServiceProxy
                        .updateMenuImages(this.imagSrc, this.imagBGSrc)
                        .subscribe((res2) => {
                            this.loadLogoImage = false;
                            this.fromFileUplode = false;
                        });
                },
                (error: any) => {
                    if (error) {
                        this.loadLogoImage = false;
                        this.element.nativeElement.value = "";
                    }
                }
            );
    }

    cancel() {
        this.element.nativeElement.value = "";
    }

    selectTab(): void {
        if (this.navVertical) {
            this.navVertical.select(this.navVertical.items.get(8).domId);
        } else {
            console.log(this.navVertical);
        }
    }

    //Google sheets integration

    isConnected: boolean = false; //check with get
    googleAuthUrl: string = "";
    onConnect() {
        let tenantId = this.appSession.tenantId;
        this._tenantSettingsService
            .getGoogleAuthUrl(tenantId)
            .subscribe((response) => {
                    this.googleAuthUrl = response;
                    window.open(
                        this.googleAuthUrl,
                        "_blank",
                        "width=600,height=700"
                    );
            });
    }

    onDisconnect() {
        let tenantId = this.appSession.tenantId;
        this._tenantSettingsService
            .revokeGoogleAccess(tenantId)
            .subscribe((response) => {
                this.isConnected = false;
                this.email = null;
                this.notify.success(this.l("Disconnected Successfully"));
            });

    }

    email: string | null = null;

    handleOAuthMessage = (event: MessageEvent) => {
        let tenantId = this.appSession.tenantId;
        if (event.data === "oauth-success") {
            this.isConnected = true;
            this.notify.success(this.l("Connected Successfully"));

            this._tenantSettingsService
                .googleSheetConfigGet(tenantId)
                .subscribe((response) => {
                    if (
                        response.isConnected == true
                    ) {
                        let savedEmail = response.googleEmail;
                        this.email = savedEmail;
                    }
                });
        }
    };



}
