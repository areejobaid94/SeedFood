import { Injector, ElementRef, Component, OnInit, AfterViewInit, ViewChild, Inject } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { LayoutRefService } from '@metronic/app/core/_base/layout/services/layout-ref.service';
import { AppConsts } from '@shared/AppConsts';
import { DOCUMENT } from '@angular/common';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { OffcanvasOptions } from '@metronic/app/core/_base/layout/directives/offcanvas.directive';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { TenantDashboardServiceProxy, TenantServiceProxy } from '@shared/service-proxies/service-proxies';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
    templateUrl: './theme5-layout.component.html',
    selector: 'theme5-layout',
    animations: [appModuleAnimation()]
})
export class Theme5LayoutComponent extends ThemesLayoutBaseComponent implements OnInit, AfterViewInit {

    @ViewChild('ktHeader', { static: true }) ktHeader: ElementRef;

    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;
    asideToggler;

    userMenuToggleOptions: ToggleOptions = {
        target: this.document.body,
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
        private layoutRefService: LayoutRefService,
        @Inject(DOCUMENT) private document: Document,
        private toastr: ToastrService,
        private router: Router,

    ) {
        super(injector);
        this.appSession = injector.get(AppSessionService);
    }

    ngOnInit() {
        // this.checkPayment();
        this.installationMode = UrlHelper.isInstallUrl(location.href);
        this.phoneNumber=this.appSession.tenant.phoneNumber;
        // this._tenantService.getTenantPhoneNumber(this.appSession.tenantId).subscribe((data) => {

        //     this.phoneNumber=data;
        //   });
    }
    
    checkPayment() {
        if (!this.appSession.tenant.isPaidInvoice) {
            this.toastr
                .error(
                    "you have an unpaid bill, pay it now to avoid disconnecting the service",
                    "WARNING!",
                    {
                        tapToDismiss: false,
                        positionClass: "toast-top-center",
                        timeOut: 0,
                        extendedTimeOut: 0,
                    }
                ) .onTap
                .subscribe(() => this.goToBillings());

        } else if (this.appSession.tenant.isCaution) {
            this.toastr.warning(
                "You have an unpaid bill, pay it before disconnecting the service.",
                "CAUTION!",
                {
                 
                    tapToDismiss: false,
                    positionClass: "toast-top-center",
                    timeOut: 0,
                    extendedTimeOut: 0,
                    // @ts-ignore
                    onclick: function () { 
                        this.goToBillings();
                     }
                    }
            ).onTap
            .subscribe(() => this.goToBillings());
        }
    }

    
    goToBillings(){
        this.router.navigate(['/app/main/billings/billings']);
    }

    ngAfterViewInit(): void {
        this.layoutRefService.addElement('header', this.ktHeader.nativeElement);
        this.asideToggler = new KTOffcanvas(this.document.getElementById('kt_aside'), {
            overlay: true,
            baseClass: 'aside',
            toggleBy: ['kt_aside_toggle', 'kt_aside_tablet_and_mobile_toggle']
        });
    }
    onLine(): void {
        this.online=navigator.onLine;
    }
}
