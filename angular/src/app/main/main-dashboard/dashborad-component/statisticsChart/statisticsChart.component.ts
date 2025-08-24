import {
    Component,
    Injector,
    OnInit,
    ViewChild,
    ViewEncapsulation,
} from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { CoreConfigService } from "@core/services/config.service";
import { TenantDashboardServiceProxy, UsersDashModel } from "@shared/service-proxies/service-proxies";
import { PermissionCheckerService } from "abp-ng2-module";
import {
    ApexAxisChartSeries,
    ApexChart,
    ApexStroke,
    ApexDataLabels,
    ApexXAxis,
    ApexGrid,
    ApexTitleSubtitle,
    ApexTooltip,
    ApexPlotOptions,
    ApexYAxis,
    ApexFill,
    ApexMarkers,
    ApexTheme,
    ApexNonAxisChartSeries,
    ApexLegend,
    ApexResponsive,
    ApexStates,
} from "ng-apexcharts";
import { MainDashboardServiceService } from "../../main-dashboard-service.service";
import { AppComponentBase } from "@shared/common/app-component-base";

// interface ChartOptions
export interface ChartOptions {
    series?: ApexAxisChartSeries;
    chart?: ApexChart;
    xaxis?: ApexXAxis;
    dataLabels?: ApexDataLabels;
    grid?: ApexGrid;
    stroke?: ApexStroke;
    legend?: ApexLegend;
    title?: ApexTitleSubtitle;
    colors?: string[];
    tooltip?: ApexTooltip;
    plotOptions?: ApexPlotOptions;
    yaxis?: ApexYAxis;
    fill?: ApexFill;
    labels?: string[];
    markers: ApexMarkers;
    theme: ApexTheme;
}

export interface ChartOptions2 {
    // Apex-non-axis-chart-series
    series?: ApexNonAxisChartSeries;
    chart?: ApexChart;
    stroke?: ApexStroke;
    tooltip?: ApexTooltip;
    dataLabels?: ApexDataLabels;
    fill?: ApexFill;
    colors?: string[];
    legend?: ApexLegend;
    labels?: any;
    plotOptions?: ApexPlotOptions;
    responsive?: ApexResponsive[];
    markers?: ApexMarkers[];
    xaxis?: ApexXAxis;
    yaxis?: ApexYAxis;
    states?: ApexStates;
}

@Component({
    selector: "app-statisticsChart",
    templateUrl: "./statisticsChart.component.html",
    styleUrls: ["./statisticsChart.component.css"],
    encapsulation: ViewEncapsulation.None,
})
export class StatisticsChartComponent
    extends AppComponentBase
    implements OnInit
{

    allUsers: UsersDashModel[] = [];
    public isMenuToggled = false;
    /**
     *
     */
    constructor(
        injector: Injector,
        public dasboardService: MainDashboardServiceService,
        private _permissionCheckerService: PermissionCheckerService,
        private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
        public darkModeService: DarkModeService
    ) {
        super(injector);
    }

    @ViewChild("apexRadialChartRef") apexRadialChartRef: any;
    public apexRadialChart: Partial<ChartOptions2>;

    // Color Variables
    chartColors = {
        column: {
            series1: "#826af9",
            series2: "#d2b0ff",
            bg: "#f8d3ff",
        },
        success: {
            shade_100: "#7eefc7",
            shade_200: "#06774f",
        },
        donut: {
            series1: "#00BDAA",
            series2: "#CBCBCB",
            series3: "#FDC668",
            series4: "#FE9A95",
        },
        area: {
            series3: "#a4f8cd",
            series2: "#60f2ca",
            series1: "#2bdac7",
        },
    };

    getUsers() {
        this._tenantDashboardServiceProxy
          .getAllUser(this.appSession.tenantId)
          .subscribe((response: UsersDashModel[]) => {
            this.allUsers = response;
          });
      }

    ngOnInit() {
        this.getUsers();
        this.apexRadialChart = {
            
            labels: ["Booked", "Pending", "Confirmed", "Canceled"],
            chart: {
                height: 400,
                type: "radialBar",
            },
            colors: [
                this.chartColors.donut.series1,
                this.chartColors.donut.series2,
                this.chartColors.donut.series3,
                this.chartColors.donut.series4,
            ],
            plotOptions: {
                radialBar: {
                    // size: 185,
                    hollow: {
                        size: "25%",
                    },
                    track: {
                        margin: 15,
                    },
                    dataLabels: {
                        name: {
                            fontSize: "2rem",
                            fontFamily: "Verdana",
                        },
                        value: {
                            fontSize: "1rem",
                            fontFamily: "Verdana",
                        },
                        total: {
                            show: true,
                            fontSize: "1rem",
                            label: "Total Booked",
                        },
                    },
                },
            },
            legend: {
                show: true,
                position: "bottom",
            },
            //   responsive: [
            //     {
            //         breakpoint: 480,
            //         options: {
            //             chart: {
            //                 height: 300,
            //             },
            //             legend: {
            //                 position: "bottom",
            //             },
            //         },
            //     },
            // ],
            stroke: {
                lineCap: "round",
            },
        };
    }

    ngAfterViewInit() {
        setTimeout(() => {
            this.apexRadialChart.chart.width =
                this.apexRadialChartRef?.nativeElement.offsetWidth;
        }, 900);
    }
}
