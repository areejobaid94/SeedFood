import { AfterViewInit, Component, ElementRef, Injector, OnInit, ViewChild } from "@angular/core";
import { MainDashboardServiceService } from "../main-dashboard-service.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import * as rtlDetect from "rtl-detect";
import { PermissionCheckerService } from "abp-ng2-module";
import {
    BranchsModel,
    CampaignDashModel,
    LiveChatServiceProxy,
    TenantDashboardServiceProxy,
    UsersDashModel,
} from "@shared/service-proxies/service-proxies";
import { DarkModeService } from "@app/services/dark-mode.service";
import moment from "moment";
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

//colors
export const colors = {
    solid: {
        primary: "#7367F0",
        secondary: "#82868b",
        success: "#28C76F",
        info: "#00cfe8",
        warning: "#FF9F43",
        danger: "#EA5455",
        dark: "#4b4b4b",
        black: "#000",
        white: "#fff",
        body: "#f8f8f8",
    },
    light: {
        primary: "#7367F01a",
        secondary: "#82868b1a",
        success: "#28C76F1a",
        info: "#00cfe81a",
        warning: "#FF9F431a",
        danger: "#EA54551a",
        dark: "#4b4b4b1a",
    },
};

@Component({
    selector: "app-dashboard-charts",
    templateUrl: "./dashboard-charts.component.html",
    styleUrls: ["./dashboard-charts.component.css"],
})
export class DashboardChartsComponent
    extends AppComponentBase
    implements OnInit, AfterViewInit
{
    isArabic = false;
    isHasPermissionLiveChat = false;
    isHasPermissionOrder = false;
    isHasPermissionBooking = false;
    isHasPermissionRequest = false;
    isHasPermissionContacts = false;
    isHasPermissionCampiagn = false;
    allCampaigns: CampaignDashModel[] = [];
    allBranches: BranchsModel[] = [];
    allUsers: UsersDashModel[] = [];
    dataForChart: number[] = [];
  
    //charts
    @ViewChild("supportChartRef") supportChartRef: any;
    @ViewChild("earningChartRef") earningChartRef: any;
    @ViewChild("appoitmentsChartRef") appoitmentsChartRef: any;

    public supportChartoptions;
    public earningChartoptions;
    public appoitmentChartoptions;

    // Private charts colors
    private $white = "#fff";
    private $textHeadingColor = "#5e5873";
    private $goalStrokeColor2 = "#51e5a8";
    private $strokeColor = "#ebe9f1";
    private $earningsStrokeColor2 = "#28c76f66";
    private $earningsStrokeColor3 = "#28c76f33";
    private $primary = "#7367F0";
    private $warning = "#FF9F43";

    constructor(
        injector: Injector,
        public dasboardService: MainDashboardServiceService,
        private _permissionCheckerService: PermissionCheckerService,
        private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
        public darkModeService: DarkModeService,
        private el: ElementRef,
        private _liveChatServiceProxy: LiveChatServiceProxy,
        private router: Router,

    ) {
        super(injector);
    }

    ngOnInit(): void {


        this.dasboardService.selectedUserForBarChart = undefined;

        // Optionally, listen for route changes to reset the selected user
        this.router.events
            .pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(() => {
                this.dasboardService.selectedUserForBarChart = undefined;
            });
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.isHasPermissionOrder =
            this._permissionCheckerService.isGranted("Pages.Orders");
        this.isHasPermissionLiveChat =
            this._permissionCheckerService.isGranted("Pages.LiveChat");
        this.isHasPermissionRequest = this._permissionCheckerService.isGranted(
            "Pages.SellingRequests"
        );
        this.isHasPermissionCampiagn = this._permissionCheckerService.isGranted(
            "Pages.MessageCampaign"
        );
        this.isHasPermissionContacts =
            this._permissionCheckerService.isGranted("Pages.Contacts");
        this.isHasPermissionBooking =
            this._permissionCheckerService.isGranted("Pages.Booking");
        // LiveChat  Chart
        this.supportChartoptions = {
            series: [this.dasboardService.ticketsData.totalClosed],
            chart: {
                height: 290,
                type: "radialBar",
                sparkline: {
                    enabled: false,
                },
            },
            plotOptions: {
                radialBar: {
                    offsetY: 20,
                    startAngle: -150,
                    endAngle: 150,
                    hollow: {
                        size: "65%",
                    },
                    track: {
                        background: this.$white,
                        strokeWidth: "100%",
                    },
                    dataLabels: {
                        name: {
                            offsetY: -5,
                            color: this.$textHeadingColor,
                            fontSize: "1rem",
                        },
                        value: {
                            offsetY: 15,
                            color: this.$textHeadingColor,
                            fontSize: "1.714rem",
                        },
                    },
                },
            },
            colors: [colors.solid.danger],
            fill: {
                type: "gradient",
                gradient: {
                    shade: "dark",
                    type: "horizontal",
                    shadeIntensity: 0.5,
                    gradientToColors: [colors.solid.primary],
                    inverseColors: true,
                    opacityFrom: 1,
                    opacityTo: 1,
                    stops: [0, 100],
                },
            },
            stroke: {
                dashArray: 8,
            },
            labels: [
                this.localization.localize(
                    "closed",
                    this.localizationSourceName
                ),
            ],
        };

        //Orders chart
        const self = this;
        this.earningChartoptions = {
            chart: {
                type: "donut",
                height: 300,
                toolbar: {
                    show: false,
                },
            },
            dataLabels: {
                enabled: false,
            },
            series: [0, 0, 0],
            legend: { show: false },
            comparedResult: [2, -3, 8],
            labels: [
                this.localization.localize(
                    "completed",
                    this.localizationSourceName
                ),
                this.localization.localize(
                    "pending",
                    this.localizationSourceName
                ),
                this.localization.localize(
                    "canceled",
                    this.localizationSourceName
                ),
            ],
            stroke: { width: 0 },
            colors: [
                colors.solid.success,
                this.$earningsStrokeColor2,
                this.$earningsStrokeColor3,
            ],
            grid: {
                padding: {
                    right: -20,
                    bottom: -8,
                    left: -20,
                },
            },
            // plotOptions: {
            //   pie: {
            //     startAngle: -10,
            //     donut: {
            //       labels: {
            //         show: true,
            //         name: {
            //           offsetY: 15,
            //         },
            //         value: {
            //           offsetY: -15,
            //           formatter: function (val) {
            //             return parseInt(val) + "%";
            //           },
            //         },
            //         total: {
            //           show: true,
            //           offsetY: 15,
            //           label: [
            //             this.localization.localize(
            //               "completed",
            //               this.localizationSourceName
            //             ),
            //           ],
            //           formatter: function (w) {
            //             return ` ${self.dasboardService.compeletedOrderPercentageString} % `;
            //           },
            //         },
            //       },
            //     },
            //   },
            // },
            responsive: [
                {
                    breakpoint: 1325,
                    options: {
                        chart: {
                            height: 100,
                        },
                    },
                },
                {
                    breakpoint: 1200,
                    options: {
                        chart: {
                            height: 120,
                        },
                    },
                },
                {
                    breakpoint: 1065,
                    options: {
                        chart: {
                            height: 100,
                        },
                    },
                },
                {
                    breakpoint: 992,
                    options: {
                        chart: {
                            height: 120,
                        },
                    },
                },
            ],
        };

        //Appoitments
        this.appoitmentChartoptions = {
            chart: {
                type: "donut",
                height: 300,
                toolbar: {
                    show: false,
                },
            },
            dataLabels: {
                enabled: false,
            },
            series: [0, 0, 0],
            legend: { show: false },
            comparedResult: [2, -3, 8],
            labels: [
                this.localization.localize(
                    "booked",
                    this.localizationSourceName
                ),
                this.localization.localize(
                    "pending",
                    this.localizationSourceName
                ),
                this.localization.localize(
                    "canceled",
                    this.localizationSourceName
                ),
            ],
            stroke: { width: 0 },
            colors: [
                colors.solid.success,
                this.$earningsStrokeColor2,
                this.$earningsStrokeColor3,
            ],
            grid: {
                padding: {
                    right: -20,
                    bottom: -8,
                    left: -20,
                },
            },
            // plotOptions: {
            //   pie: {
            //     startAngle: -10,
            //     donut: {
            //       labels: {
            //         show: true,
            //         name: {
            //           offsetY: 15,
            //         },
            //         value: {
            //           offsetY: -15,
            //           formatter: function (val) {
            //             return parseInt(val) + "%";
            //           },
            //         },
            //         total: {
            //           show: true,
            //           offsetY: 15,
            //           label: [
            //             this.localization.localize(
            //               "booked",
            //               this.localizationSourceName
            //             ),
            //           ],
            //           formatter: function (w) {
            //             return ` ${self.dasboardService.bookedAppointmentsPercentage} % `;
            //           },
            //         },
            //       },
            //     },
            //   },
            // },
            responsive: [
                {
                    breakpoint: 1325,
                    options: {
                        chart: {
                            height: 100,
                        },
                    },
                },
                {
                    breakpoint: 1200,
                    options: {
                        chart: {
                            height: 120,
                        },
                    },
                },
                {
                    breakpoint: 1065,
                    options: {
                        chart: {
                            height: 100,
                        },
                    },
                },
                {
                    breakpoint: 992,
                    options: {
                        chart: {
                            height: 120,
                        },
                    },
                },
            ],
        };
        if (this.isHasPermissionOrder) this.getBranches();
        this.getUsers();
    }

    ngAfterViewInit() {
        setTimeout(() => {
            this.supportChartoptions.chart.width =
                this.supportChartRef?.nativeElement.offsetWidth;
            this.earningChartoptions.chart.width =
                this.earningChartRef?.nativeElement.offsetWidth;
            this.appoitmentChartoptions.chart.width =
                this.appoitmentsChartRef?.nativeElement.offsetWidth;
        }, 1000);
        const container = this.el.nativeElement.querySelector('.grid-container');
        const items = container.children.length;
    
        // Add a class if the number of items is odd
        if (items % 2 !== 0) {
          container.classList.add('odd');
        }
    }

    getAllCampaigns() { 
      this._tenantDashboardServiceProxy
            .getAllCampaign(this.appSession.tenantId)
            .subscribe((response: CampaignDashModel[]) => {
                this.allCampaigns = response;
            });
    }

    getBranches() {
        this._tenantDashboardServiceProxy
            .branchsGetAll(this.appSession.tenantId)
            .subscribe((response: BranchsModel[]) => {
                this.allBranches = response;
            });
    }

    getUsers() {
        this._tenantDashboardServiceProxy
            .getAllUser(this.appSession.tenantId)
            .subscribe((response: UsersDashModel[]) => {
                this.allUsers = response;
            });
    }

    onClickCampaignHandle() {
        if (this.allCampaigns?.length <= 0) {
            this.getAllCampaigns();
        }
    }
    
    getAllCampaignValue(){
      if (this.allCampaigns?.length <= 0) {
        return [{id: 0, title: 'loading...'}]
      }
      else{
        return this.allCampaigns;
      }
    }

    getDashboardUserPreformanceForBarChartInfo() {
        if (this.dasboardService.selectedUserForBarChart == "undefined" || this.dasboardService.selectedUserForBarChart == undefined) {
            this.dasboardService.selectedUserForBarChart = undefined;
        } else {
            this._liveChatServiceProxy.getTicket(
                moment(this.dasboardService.from),
                moment(this.dasboardService.to),
                "",
                this.dasboardService.selectedUserForBarChart,
                "",
                "",
                "",
                null,
                0,
                1,
                "",
                this.appSession.user.id.toString(),
                null,null,null,0
            ).subscribe(result => {
                this.dataForChart = [result.totalPending, result.totalOpen, result.totalClosed];
                console.log(this.dataForChart);
            });

            // this._liveChatServiceProxy.getTicket(
            //     moment(this.dasboardService.from),
            //     moment(this.dasboardService.to),
            //     "",
            //     this.dasboardService.selectedUserForBarChart,
            //     "",
            //     "",
            //     null,
            //     0,
            //     1,
            //     "",
            //     this.appSession.user.id.toString()
            // ).subscribe(result => {
            //     this.dataForChart = [result.totalPending, this.dataForChart[1], this.dataForChart[2]];
            //     console.log(this.dataForChart);
            // });
        }
}   }
