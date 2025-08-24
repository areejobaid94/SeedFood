import {
    Component,
    EventEmitter,
    Injector,
    Output,
    ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    BookingServiceProxy,
    ConversationMeasurements,
    CurrenciesServiceProxy,
    CurrencyDto,
    SettingsTenantHostModel,
    TenantDashboardServiceProxy,
    TenantEditDto,
    TenantServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { Paginator } from "primeng/paginator";
import { finalize } from "rxjs/operators";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

@Component({
    selector: "tenantSettingsModal",
    templateUrl: "./tenant-settings-modal.component.html",
})
export class TenantSettingsModalComponent extends AppComponentBase {
    @ViewChild("tenantSettingsModal", { static: true }) modal: ModalDirective;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    form: FormGroup;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    tenantName: string;
    tenant: SettingsTenantHostModel = new SettingsTenantHostModel();
    currency: CurrencyDto[] = [];
    saving = false;
    conversations: ConversationMeasurements = new ConversationMeasurements();
    isButtonDisabled: boolean = false;

    constructor(
        injector: Injector,
        private _bookingService: BookingServiceProxy,
        private _tenantService: TenantServiceProxy,
        private _currenciesServiceProxy: CurrenciesServiceProxy,
        private _tenantDashboardServiceProxy: TenantServiceProxy,
        private fb: FormBuilder
    ) {
        super(injector);
        this.isButtonDisabled = false;

        this.form = this.fb.group({
            timeZoneId: [""],
            tenantId: [0],
            currencyList: [null],
            currencyCode: [""],
            zohoCustomerId: [""],
            cautionDays: [0],
            warningDays: [0],
            totalCustomerWallet: [0],
            clientIpAddress: [""],
            isPreOrder: [""],
            isPickup: [""],
            isDelivery: [""],
        });
    }

    fillFormWithData() {
        if (this.tenant) {
            this.form.patchValue({
                timeZoneId: this.tenant.timeZone,
                tenantId: this.tenant.tenantId,
                currencyList: this.tenant.currencyList,
                currencyCode: this.tenant.currency,
                zohoCustomerId: this.tenant.zohoCustomerId,
                clientIpAddress: this.tenant.clientIpAddress,
                cautionDays: this.tenant.cautionDays,
                warningDays: this.tenant.warningDays,
                totalCustomerWallet: this.tenant.totalCustomerWallet,
                isPreOrder: this.tenant.isPreOrder,
                isPickup: this.tenant.isPickup,
                isDelivery: this.tenant.isDelivery,
            });
        }
    }

    show(tenantId: number, tenantName: string): void {
        this.tenant = new SettingsTenantHostModel();
        this.tenantName = tenantName;
        this.conversations.tenantId = tenantId;
        this._tenantDashboardServiceProxy
            .getSettingsTenantHost(tenantId)
            .subscribe((tenantResult: SettingsTenantHostModel) => {
                this.tenant = tenantResult;
                this.tenant.tenantId = tenantId;
                this.fillFormWithData();
                // this.getConversations(tenantId);
            });
        this.modal.show();
    }

    getConversations(tenantId) {
        this._tenantService
            .getTenantConversation(tenantId)
            .subscribe((result) => {
                this.conversations = result;
            });
    }

    onSubmit() {
        this.isButtonDisabled = true;
 
            this.tenant.cautionDays= this.form.value.cautionDays;
            this.tenant.currency= this.form.value.currencyCode;

            this.tenant.isDelivery= this.form.value.isDelivery;
            this.tenant.isPickup= this.form.value.isPickup;

            this.tenant.isPreOrder= this.form.value.isPreOrder;
            this.tenant.tenantId= this.form.value.tenantId;
            this.tenant.timeZone= this.form.value.timeZoneId;

            this.tenant.totalCustomerWallet= this.form.value.totalCustomerWallet;

            this.tenant.warningDays= this.form.value.warningDays;

            this.tenant.zohoCustomerId= this.form.value.zohoCustomerId;
      
            this.tenant.clientIpAddress= this.form.value.clientIpAddress;


        this._tenantDashboardServiceProxy
            .updateSettingsTenantHost(this.tenant)
            .pipe(finalize(() => (this.saving = false)))
            .subscribe(
                () => {
                    this.notify.info(this.l("SavedSuccessfully"));
                    this.isButtonDisabled = false;
                    this.close();
                    this.modalSave.emit(null);
                    this.reloadPage();

                    this.close();
                    // this.saveConversations();
                },
                (error) => {
                    this.isButtonDisabled = false;
                },
                () => {
                    this.isButtonDisabled = false;
                }
            );
    }

    saveConversations() {
        this._tenantService
            .updateTenantConversation(this.conversations)
            .subscribe(() => {
                this.notify.info(this.l("SavedSuccessfully"));
                this.close();
                this.modalSave.emit(null);
                this.reloadPage();
                this.close();
            });
    }
    generateBookingTemplates() {
        this._bookingService
            .generateBookingTemplates(this.tenant.tenantId)
            .pipe(finalize(() => (this.saving = false)))
            .subscribe(() => {
                this.notify.info(this.l("SavedSuccessfully"));
                this.close();
                this.modalSave.emit(null);
                this.reloadPage();
            });
        this.close();
    }

    ZohoSync() {
        this._tenantService
            .zohoContactSync(this.tenant.tenantId)
            .subscribe((tenantResult) => {
                if (tenantResult.contacts.length > 0) {
                    
                    // this.form['zohoCustomerId'].value = tenantResult.contacts[0].contact_id
                    this.form.patchValue({
                        zohoCustomerId: tenantResult.contacts[0].contact_id,
                    });
                
                    // this.tenant.zohoCustomerId =tenantResult.contacts[0].contact_id;
                    this.notify.info(this.l("SavedSuccessfully"));
                } else {
                    this.notify.info(this.l("Error"));
                }
            });
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
