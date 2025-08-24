import {
    Component,
    EventEmitter,
    Injector,
    Input,
    OnInit,
    Output,
    ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    CreateTenantInput,
    PasswordComplexitySetting,
    TenantDataFacebook,
    TenantServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs/operators";
import { SignInFacebookService } from "../Services/signinfacebook.service";

@Component({
    selector: "app-embeded-sign-in-modal",
    templateUrl: "./embeded-sign-in-modal.component.html",
    styleUrls: ["./embeded-sign-in-modal.component.css"],
})
export class EmbededSignInModalComponent
    extends AppComponentBase
    implements OnInit
{
    @ViewChild("EmbededSignIn", { static: true }) modal: ModalDirective;
    @Output() tenancyName = new EventEmitter<any>();
    tenant: CreateTenantInput = new CreateTenantInput();
    saving: boolean = false;
    useHostDb: boolean = false;
    passwordComplexitySetting: PasswordComplexitySetting =
        new PasswordComplexitySetting();
    tenantAdminPasswordRepeat = "";

    constructor(
        injector: Injector,
        private _tenantService: TenantServiceProxy,
        private _facbookService: SignInFacebookService
    ) {
        super(injector);
    }

    ngOnInit(): void {}

    show(tenantData) {
        this.readyTenant(tenantData);
        this._facbookService.isLoading = false;
        this.modal.show();
    }
    close() {
        this.modal.hide();
    }

    save() {
        this.saving = true;
        this._tenantService
            .createTenant(this.tenant)
            .pipe(finalize(() => (this.saving = false)))
            .subscribe(() => {
                this.notify.info(this.l("SavedSuccessfully"));
                this.tenancyName.emit(this.tenant.tenancyName);
                this.close();
            });
    }

    readyTenant(tenantParam: TenantDataFacebook) {
        this.tenant.tenancyName = ""; // input by user
        this.tenant.name = tenantParam.name; // auto input
        this.tenant.legalID = "1"; // constant
        this.tenant.connectionString = ""; // constant
        this.tenant.website = "https://www.info-seed.com/"; // constant
        this.tenant.adminEmailAddress = ""; // input by user
        this.tenant.adminPassword = ""; // input by user
        this.tenantAdminPasswordRepeat = ""; // input by user
        this.tenant.phoneNumber = tenantParam.phoneNumber; // auto input
        this.tenant.tenantType = 0; // constant
        this.tenant.d360Key = tenantParam.phoneNumberId; // auto input
        this.tenant.accessToken = tenantParam.accessToken; // auto input
        this.tenant.whatsAppAccountID = tenantParam.whatsAppAccountId; // auto input
        this.tenant.whatsAppAppID = "705499500542058"; // auto input
        this.tenant.dailyLimit = 250; // auto input
        this.tenant.isBotActive = true; // constant
        this.tenant.directLineSecret = ""; //constant
        this.tenant.botId = "FlowsBot"; // constant
        this.tenant.botLocal = 0; // constant
        this.tenant.dueDay = 0; // constant
        this.tenant.shouldChangePasswordOnNextLogin = false; // constant
        this.tenant.sendActivationEmail = false; // constant
        this.tenant.isActive = false; // constant
        this.tenant.isD360Dialog = false; // constant
        this.tenant.editionId = 1; // requerd constant (importnent)
        this.tenant.isInTrialPeriod = false; // requerd constant (importnent)
        this.tenant.botTemplateId = 1; // requerd constant (importnent)
        this.tenant.biDailyLimit = 250; // requerd constant (importnent)
    }
}