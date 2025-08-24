import { Component, Injector, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ChartDateInterval, GetMeassagesInfoHostOutput, GetRecentTenantsOutput, HostDashboardServiceProxy, RecentTenant, TopStatsData } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { WidgetComponentBaseComponent } from '../widget-component-base';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-widget-host-top-stats',
  templateUrl: './widget-host-top-stats.component.html',
  styleUrls: ['./widget-host-top-stats.component.css']
})
export class WidgetHostTopStatsComponent extends WidgetComponentBaseComponent implements OnInit, OnDestroy {

  @ViewChild('RecentTenantsTable', { static: true }) recentTenantsTable: Table;
  // public countoNewSubscriptionAmount = 0;
  // public countoNewTenantsCount = 0;
  // public countoDashboardPlaceholder1 = 0;
  // public countoDashboardPlaceholder2 = 0;

  public TotalOfConvsersations  = 0;
  public TotalNumberOfCustomers = 0;
  public countSendMessagesCount=0;
  public countoReceivedMessagesCount=0;


  @ViewChild('filterModal', { static: true }) modal: ModalDirective;

  getMeassagesInfoHostOutput:GetMeassagesInfoHostOutput;

  ecentTenant : RecentTenant;
  userIdSelected=1;
  appIncomeStatisticsDateInterval = ChartDateInterval;
  incomeStatisticsDateInterval: ChartDateInterval;
  startDate: moment.Moment
  endDate: moment.Moment
  selectedIncomeStatisticsDateInterval = ChartDateInterval.Yearly;
  loadingIncomeStatistics = true;
  incomeStatisticsData: any = [];
 
  recentTenantsData: GetRecentTenantsOutput;
  incomeStatisticsHasData = false;
  selectedDateRange: moment.Moment[] = [moment().add(-7, 'days').startOf('day'), moment().endOf('day')];
  loading = true;
  topStatsData: TopStatsData;
  constructor(
    injector: Injector,
    private _hostDashboardServiceProxy: HostDashboardServiceProxy) {

    super(injector);
  }

  ngOnInit(): void {
    this.subDateRangeFilter();
   // this.getTenantsDataFilter();
   // this.getMessagesInfoHost();
    this.runDelayed(this.loadHostTopStatsData);
    
  }

  getMessagesInfoHost = () => {
    this._hostDashboardServiceProxy.getMessagesInfoHost(this.userIdSelected,this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {
     this.getMeassagesInfoHostOutput=data;
      this.TotalOfConvsersations=data.totalOfConvsersations;
      this.TotalNumberOfCustomers=data.totalNumberOfCustomers;
      this.countSendMessagesCount=data.sendMessagesCount;
      this.countoReceivedMessagesCount=data.receivedMessagesCount;

      this.loading = false;
    });
  }
  loadHostTopStatsData = () => {
   
    this._hostDashboardServiceProxy.getTopStatsData(this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {
      this.topStatsData = data;
      this.loading = false;
    });
  }

  onDateRangeFilterChange = (dateRange) => {
    if (!dateRange || dateRange.length !== 2 || (this.selectedDateRange[0] === dateRange[0] && this.selectedDateRange[1] === dateRange[1])) {
      return;
    }

    this.selectedDateRange[0] = dateRange[0];
    this.selectedDateRange[1] = dateRange[1];
    this.runDelayed(this.loadHostTopStatsData);
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

  getTenantsDataFilter() {
   
    this._hostDashboardServiceProxy.getTenantsDataFilter(this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {
     
      this.recentTenantsData = data;    
      this.loading = false;
    });
  }
  loadRecentTenantsData() {
  
    this._hostDashboardServiceProxy.getRecentTenantsData().subscribe((data) => {
      this.recentTenantsData = data;
      this.loading = false;
    });
  }

  gotoAllRecentTenants(): void {
    window.open(abp.appPath + 'app/admin/tenants?' +
      'creationDateStart=' + encodeURIComponent(this.recentTenantsData.tenantCreationStartDate.format()));
  }
  selectUser(newSelected: any) { 
    this.userIdSelected=newSelected.id;
    this._hostDashboardServiceProxy.getMessagesInfoHost(newSelected.id,this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {
    this.TotalOfConvsersations=data.totalOfConvsersations;
    this.TotalNumberOfCustomers=data.totalNumberOfCustomers;
    this.countoReceivedMessagesCount=data.receivedMessagesCount;
    this.countSendMessagesCount=data.sendMessagesCount;
    
    });

    }

    incomeStatisticsDateIntervalChange(interval: number) {  
      this.selectedIncomeStatisticsDateInterval = interval;
      this.getMessagesInfoHost();
      this.getTenantsDataFilter();
    }
  
    loadIncomeStatisticsData = () => {
      this.loadingIncomeStatistics = true;
      this._hostDashboardServiceProxy.getIncomeStatistics(
        this.selectedIncomeStatisticsDateInterval,
        moment(this.selectedDateRange[0]),
        moment(this.selectedDateRange[1]))
        .subscribe(result => {
          this.incomeStatisticsData = this.normalizeIncomeStatisticsData(result.incomeStatistics);
          this.incomeStatisticsHasData = _.filter(this.incomeStatisticsData[0].series, data => data.value > 0).length > 0;
          this.loadingIncomeStatistics = false;
        });
    }
  
    normalizeIncomeStatisticsData(data): any {
      const chartData = [];
      for (let i = 0; i < data.length; i++) {
        chartData.push({
          'name': moment(moment(data[i].date).utc().valueOf()).format('L'),
          'value': data[i].amount
        });
      }
  
      return [{
        name: '',
        series: chartData
      }];
    }
}
