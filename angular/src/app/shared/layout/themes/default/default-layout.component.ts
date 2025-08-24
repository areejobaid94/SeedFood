import { Injector, Component, OnInit, Inject } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { DOCUMENT } from '@angular/common';
import { OffcanvasOptions } from '@metronic/app/core/_base/layout/directives/offcanvas.directive';
import { AppConsts } from '@shared/AppConsts';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { TenantDashboardServiceProxy, TenantServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './default-layout.component.html',
    selector: 'default-layout',
    animations: [appModuleAnimation()]
})
export class DefaultLayoutComponent extends ThemesLayoutBaseComponent implements OnInit {

    menuCanvasOptions: OffcanvasOptions = {
        baseClass: 'aside',
        overlay: true,
        closeBy: 'kt_aside_close_btn',
        toggleBy: 'kt_aside_mobile_toggle'
    };

    userMenuToggleOptions: ToggleOptions = {
        target: this.document.body,
        targetState: 'topbar-mobile-on',
        toggleState: 'active'
    };
    appSession: AppSessionService;
    phoneNumber:string;
    online=navigator.onLine;
    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;
  
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
                         
        this.phoneNumber=this.appSession.tenant.phoneNumber;
        // this._tenantService.getTenantPhoneNumber(this.appSession.tenantId).subscribe((data) => {

            
        //     this.phoneNumber=data;
        //   });
    }
    onLine(): void {
        this.online=navigator.onLine;
    }
}
