import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { PermissionCheckerService } from 'abp-ng2-module';
import { MainDashboardServiceService } from '../../main-dashboard-service.service';

@Component({
  selector: 'app-user-peroformance-table-ticket',
  templateUrl: './user-peroformance-table-ticket.component.html',
  styleUrls: ['./user-peroformance-table-ticket.component.css'],
  encapsulation: ViewEncapsulation.None 
})
export class UserPeroformanceTableTicketComponent  extends AppComponentBase implements OnInit {

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



