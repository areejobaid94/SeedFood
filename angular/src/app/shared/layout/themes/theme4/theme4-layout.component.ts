import { Injector, ElementRef, Component, OnInit, AfterViewInit, ViewChild, Inject } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { LayoutRefService } from '@metronic/app/core/_base/layout/services/layout-ref.service';
import { AppConsts } from '@shared/AppConsts';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { TenantDashboardServiceProxy, TenantServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './theme4-layout.component.html',
    selector: 'theme4-layout',
    animations: [appModuleAnimation()]
})
export class Theme4LayoutComponent extends ThemesLayoutBaseComponent implements OnInit, AfterViewInit {

    @ViewChild('ktHeader', { static: true }) ktHeader: ElementRef;

    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;

    asideToggleOptions: ToggleOptions = {
        target: 'kt_aside',
        targetState: 'aside-on',
        toggleState: 'active'
    };

    userMenuToggleOptions: ToggleOptions = {
        target: 'kt_header',
        targetState: 'topbar-mobile-on',
        toggleState: 'active'
    };
    appSession: AppSessionService;
    phoneNumber:string;
    online=navigator.onLine;
    constructor(
        private _tenantService: TenantServiceProxy,
        private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
        injector: Injector,
        private layoutRefService: LayoutRefService
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

    ngAfterViewInit(): void {
        this.layoutRefService.addElement('header', this.ktHeader.nativeElement);
    }
    onLine(): void {
        this.online=navigator.onLine;
    }
}
