import { Component, OnInit, Injector } from '@angular/core';
import { ChartDateInterval, GetAllUserOutput, GetMeassagesInfoOutput, TenantDashboardServiceProxy, UserModel } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { DashboardChartBase } from '../dashboard-chart-base';
import { WidgetComponentBaseComponent } from '../widget-component-base';


class DashboardTopStats extends DashboardChartBase {

  totalProfit = 0; totalProfitCounter = 0;
  newFeedbacks = 0; newFeedbacksCounter = 0;
  newOrders = 0; newOrdersCounter = 0;
  newUsers = 0; newUsersCounter = 0;

  totalProfitChange = 76; totalProfitChangeCounter = 0;
  newFeedbacksChange = 85; newFeedbacksChangeCounter = 0;
  newOrdersChange = 45; newOrdersChangeCounter = 0;
  newUsersChange = 57; newUsersChangeCounter = 0;

  init(totalProfit, newFeedbacks, newOrders, newUsers) {
    this.totalProfit = totalProfit;
    this.newFeedbacks = newFeedbacks;
    this.newOrders = newOrders;
    this.newUsers = newUsers;
    this.hideLoading();
  }
}

@Component({
  selector: 'app-widget-top-stats',
  templateUrl: './widget-top-stats.component.html',
  styleUrls: ['./widget-top-stats.component.css']
})
export class WidgetTopStatsComponent extends WidgetComponentBaseComponent implements OnInit {

 userID:string;
 tenantId:number;
  TotalOfConvsersation:number;
  SendMessagesCount:number;
  ReceivedMessagesCount:number;

  appIncomeStatisticsDateInterval = ChartDateInterval;
  getMeassagesInfoOutput:GetMeassagesInfoOutput;
  dashboardTopStats: DashboardTopStats;
  getAllUserOutput:GetAllUserOutput;

  selectedIncomeStatisticsDateInterval = ChartDateInterval.Yearly;
  selectedDateRange: moment.Moment[] = [moment().add(-7, 'days').startOf('day'), moment().endOf('day')];
  
  constructor(injector: Injector,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy
  ) {
    super(injector);
    this.dashboardTopStats = new DashboardTopStats();
  }

  ngOnInit() {
    this.subDateRangeFilter();
   // this.getAllUser();
  //  this.getMessagesInfo();
    this.loadTopStatsData();
    // this.runDelayed(this.getMessagesInfo);
  }

  getAllUser() {
    this._tenantDashboardServiceProxy.getUserData(this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {
      this.getAllUserOutput=data;
      this.userID=data.firstUserId;
      this.tenantId=data.firstTenantId;

    });
  }
  // getMessagesInfo() {
  //   this._tenantDashboardServiceProxy.getMessagesInfo("admin",this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {
  //     this.getMeassagesInfoOutput=data;
  //     this.SendMessagesCount=data.sendMessagesCount;
  //   });
  // }
  // selectUser(newSelected: UserModel ) { 

  //   this._tenantDashboardServiceProxy.getMessagesInfo("admin",this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {
    
  //     this.SendMessagesCount=data.sendMessagesCount;
  //   });
  // }
  incomeStatisticsDateIntervalChange(interval: number) {  
    this.selectedIncomeStatisticsDateInterval = interval;
    // this.getMessagesInfo();;
  }
  loadTopStatsData() {
    this._tenantDashboardServiceProxy.getTopStats().subscribe((data) => {
      this.dashboardTopStats.init(data.totalProfit, data.newFeedbacks, data.newOrders, data.newUsers);
    });
  }

  onDateRangeFilterChange = (dateRange) => {
    if (!dateRange || dateRange.length !== 2 || (this.selectedDateRange[0] === dateRange[0] && this.selectedDateRange[1] === dateRange[1])) {
      return;
    }

    this.selectedDateRange[0] = dateRange[0];
    this.selectedDateRange[1] = dateRange[1];
    // this.runDelayed(this.getMessagesInfo);
  }
  subDateRangeFilter() {
    abp.event.on('app.dashboardFilters.dateRangePicker.onDateChange', this.onDateRangeFilterChange);
  }

  unSubDateRangeFilter() {
    abp.event.off('app.dashboardFilters.dateRangePicker.onDateChange', this.onDateRangeFilterChange);
  }

  ngOnDestroy(): void {
    this.unSubDateRangeFilter();
  }

}
