import { AbpSessionService } from "abp-ng2-module";
import { AfterViewInit, Component, Injector, OnInit, ViewChild } from "@angular/core";
import { accountModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    AccountServiceProxy,
    IsTenantAvailableInput,
    IsTenantAvailableOutput,
    SessionServiceProxy,
    TenantAvailabilityState,
    UpdateUserSignInTokenOutput,
} from "@shared/service-proxies/service-proxies";
import { UrlHelper } from "shared/helpers/UrlHelper";
import { ExternalLoginProvider, LoginService } from "./Services/login.service";
import { ReCaptchaV3Service } from "ngx-captcha";
import { AppConsts } from "@shared/AppConsts";
import { SignInFacebookService } from "./Services/signinfacebook.service";
import { EmbededSignInModalComponent } from "./embeded-sign-in-modal/embeded-sign-in-modal.component";
import { ActivatedRoute } from "@angular/router";

@Component({
    templateUrl: "./login.component.html",
    animations: [accountModuleAnimation()],
    styleUrls: ["./login.component.less"],
})
export class LoginComponent extends AppComponentBase implements OnInit, AfterViewInit {
    @ViewChild("embededsigninmodal", { static: true })
    SignInModal: EmbededSignInModalComponent;
    submitting = false;
    saving = false;
    isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;
    recaptchaSiteKey: string = AppConsts.recaptchaSiteKey;
    public passwordTextType = false;
    tenancyName: string = "";
    isValid: boolean = false;
    rememberMe: boolean = false;

    tenantDataFacebook = {
        name: "",
        phoneNumber: "",
        phoneNumberId: "",
        accessToken: "",
        whatsAppAccountId: "",
    };

    constructor(
        injector: Injector,
        public loginService: LoginService,
        private _sessionService: AbpSessionService,
        private _sessionAppService: SessionServiceProxy,
        private _reCaptchaV3Service: ReCaptchaV3Service,
        private _accountService: AccountServiceProxy,
        private _facbookService: SignInFacebookService,
        private route: ActivatedRoute
    ) {
        super(injector);
    }

    ngAfterViewInit(): void {
        // console.log(this.SignInModal);
        // console.log("done the modal init");
        this._facbookService.setSignInModal(this.SignInModal)
    }

    get multiTenancySideIsTeanant(): boolean {
        return this._sessionService.tenantId > 0;
    }

    get isTenantSelfRegistrationAllowed(): boolean {
        return this.setting.getBoolean(
            "App.TenantManagement.AllowSelfRegistration"
        );
    }

    get isSelfRegistrationAllowed(): boolean {
        if (!this._sessionService.tenantId) {
            return false;
        }

        return this.setting.getBoolean(
            "App.UserManagement.AllowSelfRegistration"
        );
    }

    ngOnInit(): void {

         const savedtenancyName = localStorage.getItem('tenancyName');
        const savedUsername = localStorage.getItem('savedUsername');
        const savedPassword = localStorage.getItem('savedPassword');
   
        if (savedUsername && savedPassword) {
          this.tenancyName=savedtenancyName;
          this.loginService.authenticateModel.userNameOrEmailAddress = savedUsername;
          this.loginService.authenticateModel.password = savedPassword;
          this.rememberMe = true;
        }    
      
        if (this.tenancyName.length > 0) {
            
            this.isValid = true;
        } else {
            this.isValid = false;
        }

        const code = this.route.snapshot.queryParamMap.get("code");
        // console.log("Query parameter code:", code);

        if (code) {
            // console.log("opration successful");
            return;
        }

        this.AutoLogin();
        this.loginService.init();
        if (
            this._sessionService.userId > 0 &&
            UrlHelper.getReturnUrl() &&
            UrlHelper.getSingleSignIn()
        ) {
            this._sessionAppService
                .updateUserSignInToken()
                .subscribe((result: UpdateUserSignInTokenOutput) => {
                    const initialReturnUrl = UrlHelper.getReturnUrl();
                    const returnUrl =
                        initialReturnUrl +
                        (initialReturnUrl.indexOf("?") >= 0 ? "&" : "?") +
                        "accessToken=" +
                        result.signInToken +
                        "&userId=" +
                        result.encodedUserId +
                        "&tenantId=" +
                        result.encodedTenantId;

                    location.href = returnUrl;
                });
        }

        let state = UrlHelper.getQueryParametersUsingHash().state;
        if (state && state.indexOf("openIdConnect") >= 0) {
            this.loginService.openIdConnectLoginCallback({});
        }

        // this._facbookService.setSignInModal(this.SignInModal);
    }

    login(): void {
        if (this.rememberMe) {
           localStorage.setItem('tenancyName', this.tenancyName);
           localStorage.setItem('savedUsername', this.loginService.authenticateModel.userNameOrEmailAddress);
           localStorage.setItem('savedPassword', this.loginService.authenticateModel.password);
          } else {
            localStorage.removeItem('tenancyName');
            localStorage.removeItem('savedUsername');
            localStorage.removeItem('savedPassword');
          }

          
        abp.multiTenancy.setTenantIdCookie(undefined);

        if (this.tenancyName.toLowerCase() == "host") {
            this.doLogin();
            return;
        }

        let input = new IsTenantAvailableInput();
        input.tenancyName = this.tenancyName;

        this._accountService
            .isTenantAvailable(input)
            .subscribe((result: IsTenantAvailableOutput) => {
                switch (result.state) {
                    case TenantAvailabilityState.Available:
                        abp.multiTenancy.setTenantIdCookie(result.tenantId);
                        this.doLogin();
                        break;
                    case TenantAvailabilityState.InActive:
                        this.message.warn(
                            this.l("TenantIsNotActive", this.tenancyName)
                        );
                        break;
                    case TenantAvailabilityState.NotFound: //NotFound
                        this.message.warn(
                            this.l(
                                "ThereIsNoTenantDefinedWithName{0}",
                                this.tenancyName
                            )
                        );
                        break;
                }
            });
    }

    doLogin() {
        let recaptchaCallback = (token: string) => {
            this.showMainSpinner();
            this.saving = true;

            this.submitting = true;
            this.loginService.authenticate(
                () => {
                    this.submitting = false;
                    this.saving = false;

                    this.hideMainSpinner();
                },
                null,
                token
            );
        };

        if (this.useCaptcha) {
            this._reCaptchaV3Service.execute(
                this.recaptchaSiteKey,
                "login",
                (token) => {
                    recaptchaCallback(token);
                }
            );
        } else {
            recaptchaCallback(null);
        }
    }

    AutoLogin() {
        // Retrieve rememberMe value from local storage
        let rememberMe = localStorage.getItem("rememberClient");
        let userName = localStorage.getItem("userNameOrEmailAddress");
        userName = atob(userName);
        let password = localStorage.getItem("password");
        password = atob(password);
        if (userName && rememberMe == "true") {
            this.loginService.authenticateModel.password = password;
            this.loginService.authenticateModel.userNameOrEmailAddress =
                userName;
            this.loginService.authenticateModel.rememberClient = true;
        } else {
            // console.log("You need to login");
        }
    }

    externalLogin(provider: ExternalLoginProvider) {
        this.loginService.externalAuthenticate(provider);
    }

    get useCaptcha(): boolean {
        return this.setting.getBoolean("App.UserManagement.UseCaptchaOnLogin");
    }
    togglePasswordTextType() {
        if (this.loginService.authenticateModel.password) {
            this.passwordTextType = !this.passwordTextType;
        }
    }

    handleTenantName(event: Event) {
        const inputElement = event.target as HTMLInputElement;
        this.tenancyName = inputElement.value;

        if (this.tenancyName.length > 0) {
            debugger
            this.isValid = true;
        } else {
            this.isValid = false;
        }
    }

    handleFaceBookSignIn() {
        this._facbookService.launchWhatsAppSignup();
    }

    receiveMessage(message: string) {
        this.tenancyName = message;
        if (this.tenancyName.length > 0) {
            this.isValid = true;
        } else {
            this.isValid = false;
        }
    }
}
