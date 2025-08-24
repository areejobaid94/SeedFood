import { Injector, ElementRef, Component, ViewChild, OnInit, AfterViewInit, Inject } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { LayoutRefService } from '@metronic/app/core/_base/layout/services/layout-ref.service';
import { AppConsts } from '@shared/AppConsts';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { TenantDashboardServiceProxy, TenantServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './theme10-layout.component.html',
    selector: 'theme10-layout',
    animations: [appModuleAnimation()]
})
export class Theme10LayoutComponent extends ThemesLayoutBaseComponent implements OnInit, AfterViewInit {

    @ViewChild('ktHeader', { static: true }) ktHeader: ElementRef;

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
