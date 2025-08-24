import {
    Component,
    Injector,
    Input,
    OnDestroy,
    OnInit,
    ViewChild,
} from "@angular/core";
import { PermissionCheckerService } from 'abp-ng2-module';

import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    ChartDateInterval,
    DashbardModel,
    DashboardUIServiceProxy,
    GetAllUserOutput,
    GetMeassagesInfoOutput,
    TenantDashboardServiceProxy,
    UserModel,
    UserPerformanceBookingGenarecModel,
    UserPerformanceOrderGenarecModel,
    UserPerformanceOrderModel,
    UserPerformanceTicketGenarecModel,
    UserPerformanceTicketModel,
} from "@shared/service-proxies/service-proxies";
import { LocalStorageService } from "@shared/utils/local-storage.service";
import * as Chart from "chart.js";
import "chartjs-plugin-labels";
import moment, * as Moment from "moment";
import { curveBasis } from "d3-shape";
import { LocationListModel } from "@app/main/location/location-list.model";
import { LocationModel } from "@app/main/location/location.model";
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../../services/dark-mode.service";
import { ToastrService } from 'ngx-toastr';
import * as rtlDetect from 'rtl-detect';
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";

//colors
export const colors = {
    solid: {
      primary: '#7367F0',
      secondary: '#82868b',
      success: '#28C76F',
      info: '#00cfe8',
      warning: '#FF9F43',
      danger: '#EA5455',
      dark: '#4b4b4b',
      black: '#000',
      white: '#fff',
      body: '#f8f8f8'
    },
    light: {
      primary: '#7367F01a',
      secondary: '#82868b1a',
      success: '#28C76F1a',
      info: '#00cfe81a',
      warning: '#FF9F431a',
      danger: '#EA54551a',
      dark: '#4b4b4b1a'
    }
  };
  
  
//import * as CanvasJS from 'canvasjs';
@Component({
    selector: "customizable-dashboard",
    templateUrl: "./customizable-dashboard.component.html",
    styleUrls: ["./customizable-dashboard.component.css"],
    animations: [appModuleAnimation()],
})
export class CustomizableDashboardComponent
    extends AppComponentBase
    implements OnInit, OnDestroy {

    public selectedIndex = 0;

    theme: string;
    tenancyName = "";
    userName = "";
    @Input() dashboardName: string;

    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    tenantLocations: LocationListModel[];
    cities: LocationListModel[];
    firstCitie: LocationListModel;
    areas: LocationListModel[];
    firstAreas: LocationListModel;
    locationModel: LocationModel;
    advancedFiltersAreShown = false;

    buildChart33: Chart;

    stats: Array<any>;
    colors = ["#00c5dc", "#f4516c", "#34bfa3", "#ffb822"];
    customColors = [
        { name: "1", value: "#00c5dc" },
        { name: "2", value: "#f4516c" },
        { name: "3", value: "#34bfa3" },
        { name: "4", value: "#ffb822" },
        { name: "5", value: "#00c5dc" },
    ];

    imgProf:string;
    dashbardModel:DashbardModel;
    curve: any = curveBasis;
    Linechart: any;
    isFirstBundlle = true;
    users: UserModel[] = [];
    selectedUser: UserModel;
    selectedUserMessagesInfo: GetMeassagesInfoOutput;
    totalClosedConversations: number;
    totalContacts: number;
    totalOrders: number;
    totalRating: number;
    totalSentMessages: number;
    errorMsg1: string;
    errorMsg2: string;
    isDelivery: boolean;

    remainingConversation: number;
    bandel: number;

    remainingConversationWA: number;
    TotalFreeConversationWA: number;
    TotalUsageFreeConversationWA: number;
    TotalUsageFreeUI_WA: number;
    TotalUsageFreeBI_WA: number;
    TotalUsageFreeEntry: number;
    TotalUsageFreeConversation: number;
    TotalUIConversation: number;
    TotalUsageUIConversation: number;
    TotalBIConversation: number;
    TotalUsageBIConversation: number;

    remainingFreeConversation: number;
    remainingBIConversation!: number;
    remainingUIConversation!: number;
    isArabic = false;

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
    dateRange: [Date, Date];
    predefinedRanges = [
        {
            value: [
                new Date(),
                new Date(new Date().setDate(new Date().getDate() + 1)),
            ],
            label: this.l('Today'),
        },
        {
            value: [
                new Date(new Date().setDate(new Date().getDate() - 1)),
                new Date(),
            ],
            label: this.l('Yesterday'),
        },
        {
            value: [
                new Date(new Date().setDate(new Date().getDate() - 7)),
                new Date(),
            ],
            label: this.l('Last7Days'),
        },
        {
            value: [
                new Date(new Date().setMonth(new Date().getMonth() - 1)),
                new Date(),
            ],
            label: this.l('LastMonth'),
        },
        {
            value: [
                new Date(new Date().setFullYear(new Date().getFullYear() - 1)),
                new Date(),
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

    isHasPermissionLiveChat = false;
    isHasPermissionOrder = false;
    isHasPermissionBooking = false;
    isHasPermissionRequest = false;
    isHasPermissionContacts = false;;
    isHasPermissionCampiagn = false;
    closeOrders = 0;
    pendingOrders=0;
    deletedOrders= 0;
    canceledOrders= 0;
    preOrders= 0 ;
    totalTicket = 0;
    pendingTickets = 0;
    openedTickets= 0 ;
    closedTickets = 0;
    totalRequests = 0;
    pendingRequests = 0;
    openedRequests = 0;
    closedRequests = 0;
    totalCampaign = 0;
    deliverdCampaign = 0;
    readCampaign = 0;
    scheduledCampaign=0;
    sentCampaign = 0;
    optInContacts= 0 ;
    optOutContacts= 0 ;
    neturalContacts= 0 ;    
    totalSessions = 0;
    totalUsedOfMarketing = 0;
    totalUsedOfServices = 0;


    //charts
    public supportChartoptions;   
    public requestChartoptions;
    public earningChartoptions;
    public sessionsChartoptions;
    public facebookChartoptions;


    @ViewChild('supportChartRef') supportChartRef: any;
    @ViewChild('requestChartRef') requestChartRef: any;
    @ViewChild('earningChartRef') earningChartRef: any;
    @ViewChild('totalSessionsRef') totalSessionsRef: any;
    @ViewChild('facebookChartRef') facebookChartRef: any;


    closedTicketsChart = 0;
    closeRequestsChart = 0;

  // Private charts colors
  private $white = '#fff';
  private $textHeadingColor = '#5e5873';
  private $goalStrokeColor2 = '#51e5a8';
  private $strokeColor = '#ebe9f1';
  private $earningsStrokeColor2 = '#28c76f66';
  private $earningsStrokeColor3 = '#28c76f33';
  private $primary = '#7367F0';
  private $warning = '#FF9F43';

  //performance
  ticketPerformence = new UserPerformanceTicketGenarecModel();
  bookingPerformence = new UserPerformanceBookingGenarecModel();
  orderPerformence = new UserPerformanceOrderGenarecModel();

    constructor(
        injector: Injector,
        private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
        private _localStorageService: LocalStorageService,
        public darkModeService: DarkModeService,
        private _permissionCheckerService: PermissionCheckerService,
        private toastr: ToastrService,
        private _dashbaordService : DashboardUIServiceProxy
    ) {
        super(injector);

        // LiveChat  Chart
        this.supportChartoptions = {
            series: [this.closedTickets],
            chart: {
              height: 290,
              type: 'radialBar',
              sparkline: {
                enabled: false
              }
            },
            plotOptions: {
              radialBar: {
                offsetY: 20,
                startAngle: -150,
                endAngle: 150,
                hollow: {
                  size: '65%'
                },
                track: {
                  background: this.$white,
                  strokeWidth: '100%'
                },
                dataLabels: {
                  name: {
                    offsetY: -5,
                    color: this.$textHeadingColor,
                    fontSize: '1rem'
                  },
                  value: {
                    offsetY: 15,
                    color: this.$textHeadingColor,
                    fontSize: '1.714rem'
                  }
                }
              }
            },
            colors: [colors.solid.danger],
            fill: {
              type: 'gradient',
              gradient: {
                shade: 'dark',
                type: 'horizontal',
                shadeIntensity: 0.5,
                gradientToColors: [colors.solid.primary],
                inverseColors: true,
                opacityFrom: 1,
                opacityTo: 1,
                stops: [0, 100]
              }
            },
            stroke: {
              dashArray: 8
            },
            labels: [this.localization.localize('closed', this.localizationSourceName)]
          };

        // RequestChart Chart
    this.requestChartoptions = {
        chart: {
          height: 245,
          type: 'radialBar',
          sparkline: {
            enabled: true
          },
          dropShadow: {
            enabled: true,
            blur: 3,
            left: 1,
            top: 1,
            opacity: 0.1
          }
        },
        colors: [this.$goalStrokeColor2],
        plotOptions: {
          radialBar: {
            offsetY: -10,
            startAngle: -150,
            endAngle: 150,
            hollow: {
              size: '77%'
            },
            track: {
              background: this.$strokeColor,
              strokeWidth: '50%'
            },
            dataLabels: {
              name: {
                show: false
              },
              value: {
                color: this.$textHeadingColor,
                fontSize: '2.86rem',
                fontWeight: '600'
              }
            }
          }
        },
        fill: {
          type: 'gradient',
          gradient: {
            shade: 'dark',
            type: 'horizontal',
            shadeIntensity: 0.5,
            gradientToColors: [colors.solid.success],
            inverseColors: true,
            opacityFrom: 1,
            opacityTo: 1,
            stops: [0, 100]
          }
        },
        stroke: {
          lineCap: 'round'
        },
        grid: {
          padding: {
            bottom: 30
          }
        }
      };

      this.earningChartoptions = {
        chart: {
          type: 'donut',
          height: 120,
          toolbar: {
            show: false
          }
        },
        dataLabels: {
          enabled: false
        },
        series: [0, 0,0],
        legend: { show: false },
        comparedResult: [2, -3, 8],
        labels: [this.localization.localize('closed', this.localizationSourceName), this.localization.localize('pending', this.localizationSourceName), this.localization.localize('canceled', this.localizationSourceName)],
        stroke: { width: 0 },
        colors: [ colors.solid.success,this.$earningsStrokeColor2, this.$earningsStrokeColor3],
        grid: {
          padding: {
            right: -20,
            bottom: -8,
            left: -20
          }
        },
        plotOptions: {
          pie: {
            startAngle: -10,
            donut: {
              labels: {
                show: true,
                name: {
                  offsetY: 15
                },
                value: {
                  offsetY: -15,
                  formatter: function (val) {
                    return parseInt(val) + '%';
                  }
                },
                total: {
                  show: true,
                  offsetY: 15,
                  label: [this.localization.localize('closed', this.localizationSourceName)],
                  formatter: function (w) {
                    return "0%";
                  }
                }
              }
            }
          }
        },
        responsive: [
          {
            breakpoint: 1325,
            options: {
              chart: {
                height: 100
              }
            }
          },
          {
            breakpoint: 1200,
            options: {
              chart: {
                height: 120
              }
            }
          },
          {
            breakpoint: 1065,
            options: {
              chart: {
                height: 100
              }
            }
          },
          {
            breakpoint: 992,
            options: {
              chart: {
                height: 120
              }
            }
          }
        ]
      };

      this.sessionsChartoptions = {
        chart: {
          height: 100,
          type: 'area',
          toolbar: {
            show: false
          },
          sparkline: {
            enabled: true
          }
        },
        colors: [this.$primary],
        dataLabels: {
          enabled: false
        },
        stroke: {
          curve: 'smooth',
          width: 2.5
        },
        fill: {
          type: 'gradient',
          gradient: {
            shadeIntensity: 0.9,
            opacityFrom: 0.7,
            opacityTo: 0.5,
            stops: [0, 80, 100]
          }
        },
        tooltip: {
          x: { show: false }
        }
      };

      this.facebookChartoptions = {
        chart: {
          height: 100,
          type: 'area',
          toolbar: {
            show: false
          },
          sparkline: {
            enabled: true
          }
        },
        colors: [this.$warning],
        dataLabels: {
          enabled: false
        },
        stroke: {
          curve: 'smooth',
          width: 2.5
        },
        fill: {
          type: 'gradient',
          gradient: {
            shadeIntensity: 0.9,
            opacityFrom: 0.7,
            opacityTo: 0.5,
            stops: [0, 80, 100]
          }
        },
        series: [
          {
            name: 'Orders',
            data: [0, 0, 0, 0, 0, 0, 0]
          }
        ],
        tooltip: {
          x: { show: false }
        }
      };

    }

    ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name)

        this.isHasPermissionOrder = this._permissionCheckerService.isGranted("Pages.Orders");
        this.isHasPermissionLiveChat = this._permissionCheckerService.isGranted("Pages.LiveChat");
        this.isHasPermissionRequest = this._permissionCheckerService.isGranted("Pages.SellingRequests");
        this.isHasPermissionCampiagn = this._permissionCheckerService.isGranted("Pages.MessageCampaign");
        this.isHasPermissionContacts= this._permissionCheckerService.isGranted("Pages.Contacts");
        this.isHasPermissionBooking= this._permissionCheckerService.isGranted("Pages.Booking");
        let sd = new UserModel();
        sd.profilePictureUrl = null;
        this.selectedUser = sd;
        this.initializeDateRange();
        this.getUserperformance();
        this.getCardsData();
        this.getDashboardNumbers();
    }
    ngAfterViewInit() {
        // Subscribe to core config changes
          // If Menu Collapsed Changes
            setTimeout(() => {
              // Get Dynamic Width for Charts
                   this.supportChartoptions.chart.width = this.supportChartRef?.nativeElement.offsetWidth;
                   this.requestChartoptions.chart.width = this.requestChartRef?.nativeElement.offsetWidth;
                   this.earningChartoptions.chart.width = this.earningChartRef?.nativeElement.offsetWidth;
                   this.sessionsChartoptions.chart.width = this.totalSessionsRef?.nativeElement.offsetWidth;
                   this.facebookChartoptions.chart.width = this.facebookChartRef?.nativeElement.offsetWidth;

                }, 1000);
 
      }


    // getLocation(event?: LazyLoadEvent) {
    //     if (this.appSession.tenant.tenantType == TenantTypeEunm.Delivery) {
    //         this.isDelivery = true;
    //         this.primengTableHelper.defaultRecordsCountPerPage = 10;
    //         this.deliveryGetLocation(event);
    //     } else {
    //         this.isDelivery = false;

    //         if (this.primengTableHelper.shouldResetPaging(event)) {
    //             this.paginator.changePage(0);
    //             return;
    //         }
    //         this.primengTableHelper.showLoadingIndicator();

    //         this.primengTableHelper.predefinedRecordsCountPerPage = [
    //             10, 50, 150, 200, 400, 800,
    //         ];

    //         this.primengTableHelper.defaultRecordsCountPerPage = 10;

    //         this.primengTableHelper.getSorting(this.dataTable),
    //             this.primengTableHelper.getSkipCount(this.paginator, event),
    //             this.primengTableHelper.getMaxResultCount(
    //                 this.paginator,
    //                 event
    //             );

    //         let max = this.primengTableHelper.getMaxResultCount(
    //             this.paginator,
    //             event
    //         );
    //         if (max == 100) max = 10;

    //         this._locationsServiceProxy
    //             .getAllLocationList(
    //                 this.primengTableHelper.getSkipCount(this.paginator, event),
    //                 max,
    //                 this.appSession.tenantId,
    //                 this.primengTableHelper.getSorting(this.dataTable)
    //             )
    //             .subscribe((result: any) => {
    //                 this.primengTableHelper.records = result.locationInfoModel;

    //                 this.primengTableHelper.hideLoadingIndicator();
    //             });
    //     }
    // }

    // deliveryGetLocation(event?: LazyLoadEvent) {
    //     if (this.primengTableHelper.shouldResetPaging(event)) {
    //         this.paginator.changePage(0);
    //         return;
    //     }
    //     this.primengTableHelper.showLoadingIndicator();

    //     this.primengTableHelper.predefinedRecordsCountPerPage = [
    //         10, 50, 150, 200, 400, 800,
    //     ];

    //     this.primengTableHelper.defaultRecordsCountPerPage = 10;

    //     this.primengTableHelper.getSorting(this.dataTable),
    //         this.primengTableHelper.getSkipCount(this.paginator, event),
    //         this.primengTableHelper.getMaxResultCount(this.paginator, event);

    //     let max = this.primengTableHelper.getMaxResultCount(
    //         this.paginator,
    //         event
    //     );
    //     if (max == 100) max = 10;

    //     this.locationsServiceProxy
    //         .getAllDeliveryLocationList(
    //             this.primengTableHelper.getSkipCount(this.paginator, event),
    //             max,
    //             this.appSession.tenantId,
    //             this.primengTableHelper.getSorting(this.dataTable)
    //         )
    //         .subscribe((result: DeliveryLocationInfoModel[]) => {
    //             this.primengTableHelper.getSorting(this.dataTable);
    //             this.primengTableHelper.records = result;
    //             this.primengTableHelper.totalRecordsCount = result.length;
    //             this.primengTableHelper.hideLoadingIndicator();
    //         });
    // }

    ngOnDestroy() { }

     initializeDateRange() {
        const now = new Date();
        let firstDayOfCurrentMonth = new Date(
            now.getFullYear(),
            now.getMonth(),
            2
        );
        firstDayOfCurrentMonth.setMinutes( firstDayOfCurrentMonth.getMinutes() + firstDayOfCurrentMonth.getTimezoneOffset() );
        this.dateRange = [firstDayOfCurrentMonth, now];
    }

    onDateRangeUpdate(event?) {
      if(event != undefined){
        this.dateRange = event;
      }
        this.getCardsData();
        this.getDashboardNumbers();
        this.getUserperformance();
        // this.getUsers();
    };

    setUsersProfilePictureUrl(users: UserModel[]): void {
        for (let i = 0; i < users.length; i++) {
            let user = users[i];

            this._localStorageService.getItem(
                AppConsts.authorization.encrptedAuthTokenName,
                function (err, value) {
                    let profilePictureUrl =
                        AppConsts.remoteServiceBaseUrl +
                        "/Profile/GetProfilePictureByUser?userId=" +
                        user.id +
                        "&" +
                        AppConsts.authorization.encrptedAuthTokenName +
                        "=" +
                        encodeURIComponent(value.token);
                    (user as any).profilePictureUrl = profilePictureUrl;
                }
            );
        }
    }
    setUsersProfilePictureUrlOne(users: UserModel): void {
        let user = users;
        this._localStorageService.getItem(
            AppConsts.authorization.encrptedAuthTokenName,
            function (err, value) {
                let profilePictureUrl =
                    AppConsts.remoteServiceBaseUrl +
                    "/Profile/GetProfilePictureByUser?userId=" +
                    user.id +
                    "&" +
                    AppConsts.authorization.encrptedAuthTokenName +
                    "=" +
                    encodeURIComponent(value.token);
                (user as any).profilePictureUrl = profilePictureUrl;
            }
        );
    }

    private getCardsData = () => {
        this._tenantDashboardServiceProxy
            .getAllInfo(
                ChartDateInterval.None,
                Moment(this.dateRange[0]),
                Moment(this.dateRange[1])
            )
            .subscribe((response: any) => {
                const {
                    totalOfAllContact,
                    totalOfSendMessages,
                    totalOfClose,
                    totalOfOrders,
                    errorMsg,
                    totalOfRating,
                    bandel,
                    remainingConversation,
                    totalFreeConversationWA,
                    totalUsageFreeConversationWA,
                    totalUsageFreeUIWA,
                    totalUsageFreeBIWA,
                    totalUsageFreeEntry,
                    totalUsageFreeConversation,
                    totalUIConversation,
                    totalUsageUIConversation,
                    totalBIConversation,
                    totalUsageBIConversation,
                    remainingBIConversation,
                    remainingFreeConversation,
                    remainingUIConversation,
                } = response;

                
                this.bandel = bandel;
                this.remainingConversation = remainingConversation;
                this.TotalFreeConversationWA = Math.ceil(totalFreeConversationWA);
                this.TotalUsageFreeConversationWA = totalUsageFreeConversationWA;
                this.TotalUsageFreeConversation = Math.ceil(totalUsageFreeConversation);
                this.TotalUsageFreeUI_WA = totalUsageFreeUIWA;
                this.TotalUsageFreeBI_WA = totalUsageFreeBIWA;
                this.TotalUsageFreeEntry = totalUsageFreeEntry;
                this.TotalUIConversation = totalUIConversation;
                this.TotalUsageUIConversation = Math.ceil(totalUsageUIConversation);
                this.TotalBIConversation = totalBIConversation;
                this.TotalUsageBIConversation = Math.ceil(totalUsageBIConversation);
                this.remainingBIConversation = Math.ceil(remainingBIConversation);
                this.remainingFreeConversation = Math.ceil(remainingFreeConversation);
                this.remainingUIConversation = Math.ceil(remainingUIConversation);
                this.errorMsg1 = errorMsg;
                this.totalSessions= this.TotalUsageFreeConversationWA + (this.TotalBIConversation-this.remainingBIConversation) + (this.TotalUIConversation - this.remainingUIConversation);
                this.totalUsedOfMarketing = this.TotalBIConversation - this.remainingBIConversation;
                if(this.totalUsedOfMarketing <=0 ){
                  this.totalUsedOfMarketing = 0;
                }
                this.totalUsedOfServices = this.TotalUIConversation - this.remainingUIConversation;
                if(this.totalUsedOfServices <= 0){
                  this.totalUsedOfServices = 0;
                }
                if(this.totalSessions < 0){
                  this.totalSessions = 0;
                }
                // this.totalOrders = totalOfOrders;
                this.totalRating = totalOfRating;
                // this.totalContacts = totalOfAllContact;
                this.totalSentMessages = totalOfSendMessages;

                this.sessionsChartoptions = {
                  chart: {
                    height: 100,
                    type: 'area',
                    toolbar: {
                      show: false
                    },
                    sparkline: {
                      enabled: true
                    }
                  },
                  colors: [this.$primary],
                  dataLabels: {
                    enabled: false
                  },
                  stroke: {
                    curve: 'smooth',
                    width: 2.5
                  },
                  fill: {
                    type: 'gradient',
                    gradient: {
                      shadeIntensity: 0.9,
                      opacityFrom: 0.7,
                      opacityTo: 0.5,
                      stops: [0, 80, 100]
                    }
                  },
                  tooltip: {
                    x: { show: false }
                  }
                };
          
                this.facebookChartoptions = {
                  chart: {
                    height: 100,
                    type: 'area',
                    toolbar: {
                      show: false
                    },
                    sparkline: {
                      enabled: true
                    }
                  },
                  colors: [this.$warning],
                  dataLabels: {
                    enabled: false
                  },
                  stroke: {
                    curve: 'smooth',
                    width: 2.5
                  },
                  fill: {
                    type: 'gradient',
                    gradient: {
                      shadeIntensity: 0.9,
                      opacityFrom: 0.7,
                      opacityTo: 0.5,
                      stops: [0, 80, 100]
                    }
                  },
                  series: [
                    {
                      name: 'Orders',
                      data: [this.TotalUsageFreeEntry]
                    }
                  ],
                  tooltip: {
                    x: { show: false }
                  }
                };
                if ((this.remainingFreeConversation < 100 && this.remainingUIConversation < 100) && this.isFirstBundlle) {
                    this.isFirstBundlle = false;
                    this.toastr.error('The remaining conversations less than 100, add payment method to your business manager, contact Info Seed Support Team if you need a help', 'WARNING!',
                        {
                            timeOut: 500000000,
                            closeButton: true,
                            positionClass: 'toast-top-center'
                        },
                    );
                } else if ((this.remainingFreeConversation < 500 && this.remainingUIConversation < 500) && this.isFirstBundlle) {
                    this.isFirstBundlle = false;
                    this.toastr.warning('The remaining conversations less than 500, add payment method to your business manager, contact InfoSeed Support Team if you need a help', 'CAUTION!',
                        {
                            timeOut: 500000000,
                            closeButton: true,
                            positionClass: 'toast-top-center'
                        },
                    );
                }
                this.getUsers();
                this.saveRemainingFreeConversation(this.remainingFreeConversation);
                // this.buildChart2();
            });
    };
    private getDashboardNumbers() {
        let from= moment(this.dateRange[0]).locale('en').format("MM/DD/yyyy");
        let to = moment(this.dateRange[1]).locale('en').format("MM/DD/yyyy");
        this._dashbaordService.getDashboardNumbers(this.appSession.tenantId,from,to)
        .subscribe((result: any) => {
           //Orders
           this.totalOrders= result.order.totalOrder;
           this.closeOrders = result.order.totalOrderDone;
           this.pendingOrders = result.order.totalOrderPending;
           this.deletedOrders= result.order.totalOrderDelete;
           this.canceledOrders = result.order.totalOrderCancel;
           this.preOrders = result.order.totalOrderPreOrder;
           let ClosedOrders = 0;
           let stringClosedOrders ="0%";
           let pendingOrders= 0 ;
           let canceledOrders = 0;
           if(this.totalOrders != 0){
            ClosedOrders= Math.round((this.closeOrders/this.totalOrders)*100);
            stringClosedOrders = ClosedOrders.toString()+'%';
            pendingOrders= Math.round((this.pendingOrders/this.totalOrders)*100);
            canceledOrders= Math.round((this.canceledOrders/this.totalOrders)*100);
           }else{
            stringClosedOrders = "0%";
           }
           //orders Chart
           this.earningChartoptions = {
            chart: {
              type: 'donut',
              height: 120,
              toolbar: {
                show: false
              }
            },
            dataLabels: {
              enabled: false
            },
            series: [ClosedOrders, pendingOrders,canceledOrders],
            legend: { show: false },
            comparedResult: [2, -3, 8],
            labels: [this.localization.localize('closed', this.localizationSourceName), this.localization.localize('pending', this.localizationSourceName), this.localization.localize('canceled', this.localizationSourceName)],
            stroke: { width: 0 },
            colors: [ colors.solid.success,this.$earningsStrokeColor2, this.$earningsStrokeColor3],
            grid: {
              padding: {
                right: -20,
                bottom: -8,
                left: -20
              }
            },
            plotOptions: {
              pie: {
                startAngle: -10,
                donut: {
                  labels: {
                    show: true,
                    name: {
                      offsetY: 15
                    },
                    value: {
                      offsetY: -15,
                      formatter: function (val) {
                        return parseInt(val) + '%';
                      }
                    },
                    total: {
                      show: true,
                      offsetY: 15,
                      label: [this.localization.localize('closed', this.localizationSourceName)],
                      formatter: function (w) {
                        return stringClosedOrders;
                      }
                    }
                  }
                }
              }
            },
            responsive: [
              {
                breakpoint: 1325,
                options: {
                  chart: {
                    height: 100
                  }
                }
              },
              {
                breakpoint: 1200,
                options: {
                  chart: {
                    height: 120
                  }
                }
              },
              {
                breakpoint: 1065,
                options: {
                  chart: {
                    height: 100
                  }
                }
              },
              {
                breakpoint: 992,
                options: {
                  chart: {
                    height: 120
                  }
                }
              }
            ]
          };

           //LiveChat
            this.totalTicket= result.liveChat.totalLiveChat;
            this.pendingTickets= result.liveChat.totalLiveChatPending;
            this.openedTickets = result.liveChat.totalLiveChatOpen;
            this.closedTickets = result.liveChat.totalLiveChatClose;
            if(this.totalTicket != 0){
                this.closedTicketsChart= Math.round((this.closedTickets/this.totalTicket)*100);
            }else{
              this.closedTicketsChart = 0;
            }
            //RequestPage
            this.totalRequests = result.request.totalRequest;
            this.pendingRequests = result.request.totalRequestPending;
            this.openedRequests = result.request.totalRequestOpen;
            this.closedRequests = result.request.totalRequestClose;
            if(this.totalRequests != 0){
                this.closeRequestsChart = Math.round((this.closedRequests / this.totalRequests)*100);
            }

            //campaign
            this.totalCampaign = result.campaign.totalCampaign;
            this.deliverdCampaign=result.campaign.totalCampaignDelivered;
            this.readCampaign = result.campaign.totalCampaignRead;
            this.sentCampaign = result.campaign.totalCampaignSent;
            
            //contacts 
            this.totalContacts = result.contact.totalContact;
            this.optInContacts = result.contact.totalContactOptIn;
            this.optOutContacts = result.contact.totalContactOptOut;
            this.neturalContacts = result.contact.totalContactNeutral;
            

        });
            
    }
    private getUserperformance() {
      let from= moment(this.dateRange[0]).locale('en').format("MM/DD/yyyy");
      let to = moment(this.dateRange[1]).locale('en').format("MM/DD/yyyy");
      this._tenantDashboardServiceProxy
      .getDashoardInfo(
      Moment(from),
       Moment(to),
        this.appSession.tenantId
      )
      .subscribe((response : DashbardModel) => {
          this.dashbardModel = response;
          this.ticketPerformence = response.userPerformanceTicketModel;
          this.orderPerformence = response.userPerformanceOrderModel;
          this.bookingPerformence = response.userPerformanceBookingModel;
      });
          
  }
    saveRemainingFreeConversation(remainingFreeConversation) {
        this.darkModeService.saveRemainingFreeConversation(remainingFreeConversation);
    }

    private getUsers = () => {
        this._tenantDashboardServiceProxy
            .getUserData(
                ChartDateInterval.None,
                Moment(this.dateRange[0]),
                Moment(this.dateRange[1])
            )
            .subscribe((response: GetAllUserOutput) => {
          
                this.users = response.userModel;

                this.setUsersProfilePictureUrl(response.userModel);
                this.updateSelectedUser(response.userModel?.[0]);
                // this.buildChart();
            });

    };

    getUserMessagesInfo = (
        startDate: Moment.Moment,
        endDate: Moment.Moment
    ) => {
        this._tenantDashboardServiceProxy
            .getMessagesInfo(
                Number.parseInt(this.selectedUser.id),
                this.selectedUser.userName,
                ChartDateInterval.None,
                startDate,
                endDate
            )
            .subscribe((response: GetMeassagesInfoOutput) => {
                this.selectedUserMessagesInfo = response;
            });
    };

    private buildChart() {
        const data = {
            labels: ["Orders", "Total Orders"],
            datasets: [
                {
                    data: [
                        ((this.selectedUser.totalOfOrder / this.totalOrders) %
                            100) *
                        100,
                        100 -
                        ((this.selectedUser.totalOfOrder /
                            this.totalOrders) %
                            100) *
                        100,
                    ],
                    backgroundColor: ["#6d78b7", "#f0f2f8"],
                    borderWidth: 0,
                },
            ],
        };

        const canvas = document.getElementById("chart") as HTMLCanvasElement;
        const ctx = canvas.getContext("2d");

        const myDoughnutChart = new Chart(ctx, {
            type: "doughnut",
            data: data,
            options: {
                cutoutPercentage: 60,
                plugins: {
                    labels: {
                        render: "percentage",
                        fontColor: ["#000", "#000"],
                        position: "outside",
                        outsidePadding: 4,
                        textMargin: 8,
                    },
                },
                rotation: Math.PI,
                circumference: Math.PI,
                animation: {
                    animateScale: false,
                },
                legend: {
                    display: false,
                },
            },
        });
    }

    // private buildChart2() {
    //     this._tenantDashboardServiceProxy
    //         .getBarChartInfo()
    //         .subscribe((response: GetBarChartInfoOutput) => {
    //             const max = response.barChartInfoModel.data.reduce((a, b) =>
    //                 Math.max(a, b)
    //             );
    //             const myDoughnutChart = new Chart("canvas1", {
    //                 type: "line",

    //                 data: {
    //                     labels: response.barChartInfoModel.labels,
    //                     datasets: [
    //                         {
    //                             label: "Orders",
    //                             //  barPercentage: 0,
    //                             // barThickness: 1000,
    //                             // maxBarThickness: 1000,
    //                             // minBarLength: 0,

    //                             data: response.barChartInfoModel.data,
    //                             borderColor: "#3cba9f",
    //                             backgroundColor: [
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                                 "#6d78b7",
    //                             ],
    //                             fill: false,
    //                         },
    //                     ],
    //                 },
    //                 options: {
    //                     responsive: true,
    //                     title: {
    //                         text: "Orders Per Hour",
    //                         display: true,
    //                     },
    //                     legend: {
    //                         display: true,
    //                     },
    //                     scales: {
    //                         xAxes: [
    //                             {
    //                                 display: true,
    //                                 scaleLabel: {
    //                                     display: true,
    //                                     labelString: "Hours",
    //                                 },
    //                                 ticks: {
    //                                     beginAtZero: true,
    //                                     stepSize: 1,
    //                                 },
    //                             },
    //                         ],
    //                         yAxes: [
    //                             {
    //                                 scaleLabel: {
    //                                     display: true,
    //                                     labelString: "Orders",
    //                                 },
    //                                 display: true,
    //                                 ticks: {
    //                                     beginAtZero: true,
    //                                     // stepSize: 1,
    //                                     max: max + 10,
    //                                 },
    //                             },
    //                         ],
    //                     },
    //                 },
    //             });
    //         });
    // }

    updateSelectedUser = (user: UserModel) => {
        this._localStorageService.getItem(
            AppConsts.authorization.encrptedAuthTokenName,
            function (err, value) {
                let profilePictureUrl =
                    AppConsts.remoteServiceBaseUrl +
                    "/Profile/GetProfilePictureByUser?userId=" +
                    user.id +
                    "&" +
                    AppConsts.authorization.encrptedAuthTokenName +
                    "=" +
                    encodeURIComponent(value.token);
                (user as any).profilePictureUrl = profilePictureUrl;
            }
        );

        this.selectedUser = user;
        // this.getUserMessagesInfo(
        //     Moment(this.dateRange[0]),
        //     Moment(this.dateRange[1])
        // );
        // this.buildChart();
    };

    formatData(): any {
        for (let j = 0; j < this.stats.length; j++) {
            let stat = this.stats[j];

            let series = [];
            for (let i = 0; i < stat.change.length; i++) {
                series.push({
                    name: i + 1,
                    value: stat.change[i],
                });
            }

            stat.changeData = [
                {
                    name: j + 1,
                    series: series,
                },
            ];
        }
    }

    reload() {
        this._tenantDashboardServiceProxy
            .getRegionalStats()
            .subscribe((result) => {
                this.stats = result.stats;

                this.formatData();
            });
    }
    setIndex(index: number) {
        this.selectedIndex = index;
    }

    refresh(){
        this._tenantDashboardServiceProxy
        .statisticsWAUpdateSync()
        .subscribe((response: any) => {
            this.getCardsData();
        })
         

    }
}
