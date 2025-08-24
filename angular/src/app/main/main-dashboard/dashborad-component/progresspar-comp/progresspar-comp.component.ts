import { Component, Injector, OnInit } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CampaignDashModel, BranchsModel, UsersDashModel, TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { PermissionCheckerService } from 'abp-ng2-module';
import { MainDashboardServiceService } from '../../main-dashboard-service.service';

@Component({
  selector: 'app-progresspar-comp',
  templateUrl: './progresspar-comp.component.html',
  styleUrls: ['./progresspar-comp.component.css']
})
export class ProgressparCompComponent  extends AppComponentBase
implements OnInit {
isArabic = false;
isHasPermissionLiveChat = false;
isHasPermissionOrder = false;
isHasPermissionBooking = false;
isHasPermissionRequest = false;
isHasPermissionContacts = false;
isHasPermissionCampiagn = false;
allCampaigns: CampaignDashModel[] = [];
allBranches: BranchsModel[] = [];
allUsers: UsersDashModel[] = [];
  public contentHeader: object;
  public progressbarHeight = ".857rem";
  progressValue = 89;

  dynamicColorClass = 'bg-success';



  public selectedRole = [];

  constructor(
    injector: Injector,
    public dasboardService: MainDashboardServiceService,
    private _permissionCheckerService: PermissionCheckerService,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
    public darkModeService: DarkModeService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    // content header
    this.getAllCampaigns();
    this.contentHeader = {
        headerTitle: "Progress",
        actionButton: true,
        breadcrumb: {
            type: "",
            links: [
                {
                    name: "Home",
                    isLink: true,
                    link: "/",
                },
                {
                    name: "Components",
                    isLink: true,
                    link: "/",
                },
                {
                    name: "Progress",
                    isLink: false,
                },
            ],
        },
    };
}

getAllCampaigns() {
  this._tenantDashboardServiceProxy
    .getAllCampaign(this.appSession.tenantId)
    .subscribe((response: CampaignDashModel[]) => {
      this.allCampaigns = response;
    });
}

}
