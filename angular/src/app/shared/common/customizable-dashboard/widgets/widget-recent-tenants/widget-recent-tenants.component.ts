import { Component, ViewChild, Injector } from '@angular/core';
import { Table } from 'primeng/table';
import { HostDashboardServiceProxy, GetRecentTenantsOutput, ChartDateInterval, GetMeassagesInfoHostOutput, RecentTenant, TopStatsData } from '@shared/service-proxies/service-proxies';
import { WidgetComponentBaseComponent } from '../widget-component-base';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  selector: 'app-widget-recent-tenants',
  templateUrl: './widget-recent-tenants.component.html',
  styleUrls: ['./widget-recent-tenants.component.css']
})
export class WidgetRecentTenantsComponent extends WidgetComponentBaseComponent {
  @ViewChild('RecentTenantsTable', { static: true }) recentTenantsTable: Table;
  countSendMessagesCount: number;
  countoReceivedMessagesCount: number;
  constructor(injector: Injector,
    private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
    this.loadRecentTenantsData();
  }

  public TotalOfConvsersations  = 0;
  public TotalNumberOfCustomers = 0;

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

  ngOnInit(): void {
    this.subDateRangeFilter();
    this.getTenantsDataFilter();
    this.getMessagesInfoHost();
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
  
    this._hostDashboardServiceProxy.getTenantsDataFilter(this.selectedIncomeStatisticsDateInterval,this.selectedDateRange[0], this.selectedDateRange[1]).subscribe((data) => {
     
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
