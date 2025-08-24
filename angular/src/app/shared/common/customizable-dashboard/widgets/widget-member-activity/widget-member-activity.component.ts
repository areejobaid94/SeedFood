import { Component, OnInit, Injector } from '@angular/core';
import { DashboardChartBase } from '../dashboard-chart-base';
import { ChartDateInterval, GetAllDashboard, GetAllUserOutput, GetMeassagesInfoOutput, GetRecentTenantsOutput, HostDashboardServiceProxy, TenantDashboardServiceProxy, UserModel } from '@shared/service-proxies/service-proxies';
import { WidgetComponentBaseComponent } from '../widget-component-base';
import * as moment from 'moment';
import { Table } from 'primeng/table';
import * as _ from 'lodash';
import { ViewChild } from '@angular/core';
class MemberActivityTable extends DashboardChartBase {

  memberActivities: Array<any>;
  constructor(private _dashboardService: TenantDashboardServiceProxy) {
    super();
    
  }

  init() {
    this.reload();
  }

  reload() {
    this.showLoading();
    this._dashboardService
      .getMemberActivity()
      .subscribe(result => {
        this.memberActivities = result.memberActivities;
        this.hideLoading();
      });
  }
}

@Component({
  selector: 'app-widget-member-activity',
  templateUrl: './widget-member-activity.component.html',
  styleUrls: ['./widget-member-activity.component.css']
})
export class WidgetMemberActivityComponent extends WidgetComponentBaseComponent{
  @ViewChild('RecentTenantsTable', { static: true }) recentTenantsTable: Table;
  loading = true;
  userIdSelected="1";
  getAllDashboard :GetAllDashboard;
  getAllUserOutput:GetAllUserOutput;
  memberActivityTable: MemberActivityTable;
  appIncomeStatisticsDateInterval = ChartDateInterval;
  getMeassagesInfoOutput:GetMeassagesInfoOutput;
  userID:string;
  tenantId:number;
  TotalOfAllContact:number;
   SendMessagesCount:number;
   ReceivedMessagesCount:number;
   totalOfClose:number;
   selectedIncomeStatisticsDateInterval = ChartDateInterval.Yearly;
   selectedDateRange: moment.Moment[] = [moment().add(-7, 'days').startOf('day'), moment().endOf('day')];
  constructor(
    injector: Injector,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy) {

    super(injector);
  }

  ngOnInit() {
    
    this.getAllInfo();
    this.subDateRangeFilter();
    this.getAllUser();
    //this.getMessagesInfo();
    this.getAllInfo();
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
  //     this.totalOfClose=data.closeCount;
  //   });
  // }

  getAllInfo() {
    
    this._tenantDashboardServiceProxy.getAllInfo(this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {

      
      this.getAllDashboard=data;
      this.TotalOfAllContact=data.totalOfAllContact;
      this.SendMessagesCount=data.totalOfSendMessages;
      this.totalOfClose=data.totalOfClose;
    });
  }

  selectUser(newSelected: UserModel ) { 

    this.userID=newSelected.id;
    // this._tenantDashboardServiceProxy.getMessagesInfo("admin",this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {

    //   this.getMeassagesInfoOutput=data;
    //   this.SendMessagesCount=data.sendMessagesCount;
    //   this.totalOfClose=data.closeCount;
    // });
  }
  incomeStatisticsDateIntervalChange(interval: number) {  
    this.selectedIncomeStatisticsDateInterval = interval;   
    this.getAllUser();
    // this.getMessagesInfo()
  }


  onDateRangeFilterChange = (dateRange) => {
    if (!dateRange || dateRange.length !== 2 || (this.selectedDateRange[0] === dateRange[0] && this.selectedDateRange[1] === dateRange[1])) {
      return;
    }
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
