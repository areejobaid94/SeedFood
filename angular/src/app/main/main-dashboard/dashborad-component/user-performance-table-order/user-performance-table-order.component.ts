import { Component, Injector, OnInit } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { PermissionCheckerService } from 'abp-ng2-module';
import { MainDashboardServiceService } from '../../main-dashboard-service.service';

@Component({
  selector: 'app-user-performance-table-order',
  templateUrl: './user-performance-table-order.component.html',
  styleUrls: ['./user-performance-table-order.component.css']
})
export class UserPerformanceTableOrderComponent extends AppComponentBase implements OnInit {

  constructor(
    injector: Injector,
    public dasboardService: MainDashboardServiceService,
    private _permissionCheckerService: PermissionCheckerService,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
    public darkModeService: DarkModeService  
  ) {
    super(injector);
  }

  ngOnInit() {
  }

}