import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { BranchsModel, TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { PermissionCheckerService } from 'abp-ng2-module';
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
  ChartComponent,
  ApexOptions
} from 'ng-apexcharts';
import { MainDashboardServiceService } from '../../main-dashboard-service.service';

export interface ChartOptions {
  series?: ApexAxisChartSeries;
  chart?: ApexChart;
  xaxis?: ApexXAxis;
  apexOptions? : ApexOptions,
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

@Component({
  selector: 'app-progress-order',
  templateUrl: './progress-order.component.html',
  styleUrls: ['./progress-order.component.css']
})
export class ProgressOrderComponent  extends AppComponentBase implements OnInit {

  @ViewChild("chart") apexBarChartRef!: any;
  public chartOptions!: Partial<ChartOptions>;
  allBranches: BranchsModel[] = [];
  public isMenuToggled = false;

  // @ViewChild('apexBarChartRef') apexBarChartRef: any;
  // public isMenuToggled = false;

  // public apexBarChart: Partial<ChartOptions>;


  constructor(
    injector: Injector,
    public dasboardService: MainDashboardServiceService,
    private _permissionCheckerService: PermissionCheckerService,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
    public darkModeService: DarkModeService
  ) {
    super(injector);
  }

  getBranches() {
    this._tenantDashboardServiceProxy
      .branchsGetAll(this.appSession.tenantId)
      .subscribe((response: BranchsModel[]) => {
        this.allBranches = response;
      });
  }

  ngOnInit() {
    this.getBranches();
    this.chartOptions = {
      fill:{
        colors : [
          "#00DBC5",
          "#CBCBCB",
          "#FE9A95",
          "#99FFF5",
          "#FDC668",
        ],
        opacity : 1
      },
      series: [
        {
          data: [90, 80, 81,100, 35],

        },
      ],
      legend : {
        show:false
      },
      chart: {
        type: "bar",
        height: 220,
        toolbar: {
          show: false
        },
      },
      plotOptions: {
        bar: {
          endingShape : "rounded",
     
          barHeight: "80%",
          distributed: true,
          horizontal: true,
          dataLabels: {
            position: "start",
          },
        },
      },
      colors: [
        "#00DBC5",
        "#CBCBCB",
        "#FE9A95",
        "#99FFF5",
        "#FDC668",
      ],
      dataLabels: {
        enabled: true,
        textAnchor: "start",
        style: {
          colors: ["#000"],
        },
      
        offsetX: 0,
        
        dropShadow: {
          enabled: true,
        },
      },
      stroke: {
        width: 1,
        colors: ["#fff"],
      },
   
      xaxis: {
        labels: {
          show: false
        },
        axisBorder: {
          show: false
        },
        axisTicks: {
          show: false
        }
      ,
        categories: [
          "Pic Apple",
          "Water",
          "Sweet potato",
          "Shawarma",
          "Min Cheese",
        ],
      },
      yaxis: {
        // min:0,
        // max: 100,
        // opposite : false,
        // reversed : false,
        // floating : false,
        labels: {
          show: true,
          align: "left",
          offsetX : 0,
          style: {
            colors: "#000",
            fontWeight: "bold",
            cssClass : "y-axis-class"

          },
        },
      },
      title: {
        text: "Best Order",
        align: "left",
        floating: false,
      },
      // subtitle: {
      //   text: "Category Names as DataLabels inside bars",
      //   align: "left",
      // },
      grid: {
        show: false
    },

      tooltip: {
        theme: "dark",
        x: {
          show: false,
        },
      },
    };
  }

  ngAfterViewInit() {
      //   setTimeout(() => {
      //     this.isMenuToggled = true;
      //     this.apexBarChart.chart.width = this.apexBarChartRef?.nativeElement.offsetWidth;
      //   }, 900);
      }

}
