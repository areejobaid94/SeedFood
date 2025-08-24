import { Component, Injector, OnInit } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { PermissionCheckerService } from 'abp-ng2-module';
import { MainDashboardServiceService } from '../../main-dashboard-service.service';

@Component({
  selector: 'app-user-performance-table-appointment',
  templateUrl: './user-performance-table-appointment.component.html',
  styleUrls: ['./user-performance-table-appointment.component.css']
})
export class UserPerformanceTableAppointmentComponent extends AppComponentBase implements OnInit {

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