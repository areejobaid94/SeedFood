import {
    Component,
    Injector,
    Input,
    OnInit,
    ViewChild,
    ViewEncapsulation,
} from "@angular/core";
import { ChartOptions2 } from "../statisticsChart/statisticsChart.component";
import { CoreConfigService } from "@core/services/config.service";
import { MainDashboardServiceService } from "../../main-dashboard-service.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { DarkModeService } from "@app/services/dark-mode.service";
import * as rtlDetect from "rtl-detect";
import { TenantDashboardServiceProxy } from "@shared/service-proxies/service-proxies";
import { PermissionCheckerService } from "abp-ng2-module";

@Component({
    selector: "app-expenseRatioChart",
    templateUrl: "./expenseRatioChart.component.html",
    styleUrls: ["./expenseRatioChart.component.css"],
    encapsulation: ViewEncapsulation.None,
})
export class ExpenseRatioChartComponent
    extends AppComponentBase
    implements OnInit
{

    isArabic = false;
 

    @ViewChild("apexDonutChartRef") apexDonutChartRef: any;
    // _coreConfigService : CoreConfigService ;
    public apexDonutChart: any;
    public isMenuToggled = false;
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
            series1: "#ffe700",
            series2: "#00d4bd",
            series3: "#826bf8",
            series4: "#2b9bf4",
            series5: "#FFA1A1",
        },
        area: {
            series3: "#a4f8cd",
            series2: "#60f2ca",
            series1: "#2bdac7",
        },
    };

    constructor(
        injector: Injector,
        public dasboardService: MainDashboardServiceService,
        private _permissionCheckerService: PermissionCheckerService,
        private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
        public darkModeService: DarkModeService
    ) {
        super(injector);
        console.log("hasan", this.dasboardService);
    }

    ngOnInit(): void {
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
          );
      
        this.apexDonutChart = {
        
            chart: {
                height: 400,
                type: "donut",
        
            },
            colors: [
                this.chartColors.donut.series1,
                this.chartColors.donut.series2,
                this.chartColors.donut.series3,
                this.chartColors.donut.series5,
            ],
            plotOptions: {
                pie: {
                    donut: {
                        labels: {
                            show: true,
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
                                fontSize: "1.5rem",
                                label: "Total Tickets",
                            
                            },
                        },
                    },
                },
            },
            legend: {
                show: true,
                position: "bottom",
            },
            labels: ["Open", "Pending", "Closes", "Expired"],
            // responsive: [
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
        };
    }
    ngAfterViewInit() {
        setTimeout(() => {
            // Get Dynamic Width for Charts
            this.apexDonutChart.chart.width =
                this.apexDonutChartRef?.nativeElement.offsetWidth;
        }, 900);
    }
}
