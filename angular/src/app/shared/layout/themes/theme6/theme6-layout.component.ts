import { Injector, ElementRef, Component, OnInit, Inject } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { AppConsts } from '@shared/AppConsts';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { DOCUMENT } from '@angular/common';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { TenantDashboardServiceProxy, TenantServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './theme6-layout.component.html',
    selector: 'theme6-layout',
    animations: [appModuleAnimation()]
})
export class Theme6LayoutComponent extends ThemesLayoutBaseComponent implements OnInit {

    userMenuToggleOptions: ToggleOptions = {
        target: 'kt_aside',
        targetState: 'aside-on',
        toggleState: 'active'
    };

    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;

    appSession: AppSessionService;
    phoneNumber:string;
    online=navigator.onLine;
    constructor(
        private _tenantService: TenantServiceProxy,
        private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
        injector: Injector,
        @Inject(DOCUMENT) private document: Document
    ) {
        super(injector);
        this.appSession = injector.get(AppSessionService);
    }

    ngOnInit() {
        this.installationMode = UrlHelper.isInstallUrl(location.href);
        this._tenantService.getTenantPhoneNumber(this.appSession.tenantId).subscribe((data) => {

            this.phoneNumber=data;
          });
    }
    onLine(): void {
        this.online=navigator.onLine;
    }
}
