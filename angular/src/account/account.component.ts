import { SignInFacebookService } from './login/Services/signinfacebook.service';
import {
    Component,
    Injector,
    OnInit,
    ViewContainerRef,
    ViewEncapsulation,
} from "@angular/core";
import { Router } from "@angular/router";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/common/app-component-base";
import { AppUiCustomizationService } from "@shared/common/ui/app-ui-customization.service";
import * as _ from "lodash";
import * as moment from "moment";
import { LoginService } from "./login/Services/login.service";
import * as rtlDetect from "rtl-detect";
import {
    AccountServiceProxy,
    IsTenantAvailableInput,
    IsTenantAvailableOutput,
    TenantAvailabilityState,
} from "@shared/service-proxies/service-proxies";

@Component({
    templateUrl: "./account.component.html",
    styleUrls: ["./account.component.less"],
    encapsulation: ViewEncapsulation.None,
})
export class AccountComponent extends AppComponentBase implements OnInit {
    private viewContainerRef: ViewContainerRef;
    tenancyName: string;

    currentYear: number = moment().year();
    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;
    tenantChangeDisabledRoutes: string[] = [
        "select-edition",
        "buy",
        "upgrade",
        "extend",
        "register-tenant",
        "cancellation",
        "stripe-purchase",
        "stripe-subscribe",
        "stripe-update-subscription",
        "paypal-purchase",
        "stripe-payment-result",
        "payment-completed",
        "stripe-cancel-payment",
        "session-locked",
    ];
    isArabic = false;

    public constructor(
        injector: Injector,
        private _router: Router,
        private _loginService: LoginService,
        private _uiCustomizationService: AppUiCustomizationService,
        viewContainerRef: ViewContainerRef,
        private _accountService: AccountServiceProxy,
        public facebookservice: SignInFacebookService
    ) {
        super(injector);

        // We need this small hack in order to catch application root view container ref for modals
        this.viewContainerRef = viewContainerRef;
    }

    showTenantChange(): boolean {
        if (!this._router.url) {
            return false;
        }

        if (
            _.filter(
                this.tenantChangeDisabledRoutes,
                (route) => this._router.url.indexOf("/account/" + route) >= 0
            ).length
        ) {
            return false;
        }
        if (this._router.url.indexOf("/account/signUp") >= 0) {
            return false;
        }
        return abp.multiTenancy.isEnabled && !this.supportsTenancyNameInUrl();
    }

    showinfoSeddImage() {
        if (this._router.url.indexOf("/account/signUp") >= 0) {
            return false;
        }
    }

    useFullWidthLayout(): boolean {
        return (
            this._router.url.indexOf("/account/select-edition") >= 0 ||
            this._router.url.indexOf("/account/signUp") >= 0
        );
    }

    ngOnInit(): void {
        this._loginService.init();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        document.body.className =
            this._uiCustomizationService.getAccountModuleBodyClass();
    }

    goToHome(): void {
        (window as any).location.href = "/";
    }

    getBgUrl(): string {
        return (
            "url(./assets/metronic/themes/" +
            this.currentTheme.baseSettings.theme +
            "/images/bg/bg-4.jpg)"
        );
    }

    private supportsTenancyNameInUrl() {
        return (
            AppConsts.appBaseUrlFormat &&
            AppConsts.appBaseUrlFormat.indexOf(
                AppConsts.tenancyNamePlaceHolderInUrl
            ) >= 0
        );
    }

}
