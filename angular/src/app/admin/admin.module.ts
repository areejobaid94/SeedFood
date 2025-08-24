import { CommonModule } from '@angular/common';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { AccountBillingBillingLookupTableModalComponent } from './accountBillings/accountBillings/accountBilling-billing-lookup-table-modal.component';
import { NgbDropdownModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ChatStatusesComponent } from './chatStatuses/chatStatuses/chatStatuses.component';
import { CreateOrEditChatStatuseModalComponent } from './chatStatuses/chatStatuses/create-or-edit-chatStatuse-modal.component';

import { ContactStatusesComponent } from './contactStatuses/contactStatuses/contactStatuses.component';
import { CreateOrEditContactStatuseModalComponent } from './contactStatuses/contactStatuses/create-or-edit-contactStatuse-modal.component';

import { ReceiptsComponent } from './receipts/receipts/receipts.component';
import { ViewReceiptModalComponent } from './receipts/receipts/view-receipt-modal.component';
import { CreateOrEditReceiptModalComponent } from './receipts/receipts/create-or-edit-receipt-modal.component';
import { ReceiptBankLookupTableModalComponent } from './receipts/receipts/receipt-bank-lookup-table-modal.component';
import { ReceiptPaymentMethodLookupTableModalComponent } from './receipts/receipts/receipt-paymentMethod-lookup-table-modal.component';

import { BanksComponent } from './banks/banks/banks.component';
import { ViewBankModalComponent } from './banks/banks/view-bank-modal.component';
import { CreateOrEditBankModalComponent } from './banks/banks/create-or-edit-bank-modal.component';

import { PaymentMethodsComponent } from './paymentMethods/paymentMethods/paymentMethods.component';
import { ViewPaymentMethodModalComponent } from './paymentMethods/paymentMethods/view-paymentMethod-modal.component';
import { CreateOrEditPaymentMethodModalComponent } from './paymentMethods/paymentMethods/create-or-edit-paymentMethod-modal.component';

import { TenantServicesComponent } from './tenantServices/tenantServices/tenantServices.component';
import { ViewTenantServiceModalComponent } from './tenantServices/tenantServices/view-tenantService-modal.component';
import { CreateOrEditTenantServiceModalComponent } from './tenantServices/tenantServices/create-or-edit-tenantService-modal.component';
import { TenantServiceInfoSeedServiceLookupTableModalComponent } from './tenantServices/tenantServices/tenantService-infoSeedService-lookup-table-modal.component';

import { AccountBillingsComponent } from './accountBillings/accountBillings/accountBillings.component';
import { ViewAccountBillingModalComponent } from './accountBillings/accountBillings/view-accountBilling-modal.component';
import { CreateOrEditAccountBillingModalComponent } from './accountBillings/accountBillings/create-or-edit-accountBilling-modal.component';
import { AccountBillingInfoSeedServiceLookupTableModalComponent } from './accountBillings/accountBillings/accountBilling-infoSeedService-lookup-table-modal.component';
import { AccountBillingServiceTypeLookupTableModalComponent } from './accountBillings/accountBillings/accountBilling-serviceType-lookup-table-modal.component';
import { AccountBillingCurrencyLookupTableModalComponent } from './accountBillings/accountBillings/accountBilling-currency-lookup-table-modal.component';

import { CurrenciesComponent } from './currencies/currencies/currencies.component';
import { ViewCurrencyModalComponent } from './currencies/currencies/view-currency-modal.component';
import { CreateOrEditCurrencyModalComponent } from './currencies/currencies/create-or-edit-currency-modal.component';

import { InfoSeedServicesComponent } from './infoSeedServices/infoSeedServices/infoSeedServices.component';
import { ViewInfoSeedServiceModalComponent } from './infoSeedServices/infoSeedServices/view-infoSeedService-modal.component';
import { CreateOrEditInfoSeedServiceModalComponent } from './infoSeedServices/infoSeedServices/create-or-edit-infoSeedService-modal.component';
import { InfoSeedServiceServiceTypeLookupTableModalComponent } from './infoSeedServices/infoSeedServices/infoSeedService-serviceType-lookup-table-modal.component';
import { InfoSeedServiceServiceStatusLookupTableModalComponent } from './infoSeedServices/infoSeedServices/infoSeedService-serviceStatus-lookup-table-modal.component';
import { InfoSeedServiceServiceFrquencyLookupTableModalComponent } from './infoSeedServices/infoSeedServices/infoSeedService-serviceFrquency-lookup-table-modal.component';

import { ServiceFrquenciesComponent } from './serviceFrequencies/serviceFrquencies/serviceFrquencies.component';
import { ViewServiceFrquencyModalComponent } from './serviceFrequencies/serviceFrquencies/view-serviceFrquency-modal.component';
import { CreateOrEditServiceFrquencyModalComponent } from './serviceFrequencies/serviceFrquencies/create-or-edit-serviceFrquency-modal.component';

import { ServiceStatusesComponent } from './serviceStatuses/serviceStatuses/serviceStatuses.component';
import { ViewServiceStatusModalComponent } from './serviceStatuses/serviceStatuses/view-serviceStatus-modal.component';
import { CreateOrEditServiceStatusModalComponent } from './serviceStatuses/serviceStatuses/create-or-edit-serviceStatus-modal.component';

import { ServiceTypesComponent } from './serviceTypes/serviceTypes/serviceTypes.component';
import { ViewServiceTypeModalComponent } from './serviceTypes/serviceTypes/view-serviceType-modal.component';
import { CreateOrEditServiceTypeModalComponent } from './serviceTypes/serviceTypes/create-or-edit-serviceType-modal.component';

import { TemplateMessagesComponent } from './templateMessages/templateMessages/templateMessages.component';
import { ViewTemplateMessageModalComponent } from './templateMessages/templateMessages/view-templateMessage-modal.component';
import { CreateOrEditTemplateMessageModalComponent } from './templateMessages/templateMessages/create-or-edit-templateMessage-modal.component';

import { UtilsModule } from '@shared/utils/utils.module';
import { AddMemberModalComponent } from 'app/admin/organization-units/add-member-modal.component';
import { AddRoleModalComponent } from 'app/admin/organization-units/add-role-modal.component';
import { FileUploadModule } from 'ng2-file-upload';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { EditorModule } from 'primeng/editor';
import { FileUploadModule as PrimeNgFileUploadModule } from 'primeng/fileupload';
import { InputMaskModule } from 'primeng/inputmask';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TreeModule } from 'primeng/tree';
import { DragDropModule } from 'primeng/dragdrop';
import { TreeDragDropService } from 'primeng/api';
import { ContextMenuModule } from 'primeng/contextmenu';
import { AdminRoutingModule } from './admin-routing.module';
import { AuditLogDetailModalComponent } from './audit-logs/audit-log-detail-modal.component';
import { AuditLogsComponent } from './audit-logs/audit-logs.component';
import { HostDashboardComponent } from './dashboard/host-dashboard.component';
import { DemoUiComponentsComponent } from './demo-ui-components/demo-ui-components.component';
import { DemoUiDateTimeComponent } from './demo-ui-components/demo-ui-date-time.component';
import { DemoUiEditorComponent } from './demo-ui-components/demo-ui-editor.component';
import { DemoUiFileUploadComponent } from './demo-ui-components/demo-ui-file-upload.component';
import { DemoUiInputMaskComponent } from './demo-ui-components/demo-ui-input-mask.component';
import { DemoUiSelectionComponent } from './demo-ui-components/demo-ui-selection.component';
import { CreateEditionModalComponent } from './editions/create-edition-modal.component';
import { EditEditionModalComponent } from './editions/edit-edition-modal.component';
import { MoveTenantsToAnotherEditionModalComponent } from './editions/move-tenants-to-another-edition-modal.component';
import { EditionsComponent } from './editions/editions.component';
import { InstallComponent } from './install/install.component';
import { CreateOrEditLanguageModalComponent } from './languages/create-or-edit-language-modal.component';
import { EditTextModalComponent } from './languages/edit-text-modal.component';
import { LanguageTextsComponent } from './languages/language-texts.component';
import { LanguagesComponent } from './languages/languages.component';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { CreateOrEditUnitModalComponent } from './organization-units/create-or-edit-unit-modal.component';
import { OrganizationTreeComponent } from './organization-units/organization-tree.component';
import { OrganizationUnitMembersComponent } from './organization-units/organization-unit-members.component';
import { OrganizationUnitRolesComponent } from './organization-units/organization-unit-roles.component';
import { OrganizationUnitsComponent } from './organization-units/organization-units.component';
import { CreateOrEditRoleModalComponent } from './roles/create-or-edit-role-modal.component';
import { RolesComponent } from './roles/roles.component';
import { HostSettingsComponent } from './settings/host-settings.component';
import { TenantSettingsComponent } from './settings/tenant-settings.component';
import { EditionComboComponent } from './shared/edition-combo.component';
import { FeatureTreeComponent } from './shared/feature-tree.component';
import { OrganizationUnitsTreeComponent } from './shared/organization-unit-tree.component';
import { PermissionComboComponent } from './shared/permission-combo.component';
import { PermissionTreeComponent } from './shared/permission-tree.component';
import { RoleComboComponent } from './shared/role-combo.component';
import { InvoiceComponent } from './subscription-management/invoice/invoice.component';
import { SubscriptionManagementComponent } from './subscription-management/subscription-management.component';
import { CreateTenantModalComponent } from './tenants/create-tenant-modal.component';
import { EditTenantModalComponent } from './tenants/edit-tenant-modal.component';
import { TenantFeaturesModalComponent } from './tenants/tenant-features-modal.component';
import { TenantServicesModalComponent } from './tenants/tenant-services-modal.component';
import { TenantSettingsModalComponent } from './tenants/tenant-settings-modal.component';
import { TenantsComponent } from './tenants/tenants.component';
import { UiCustomizationComponent } from './ui-customization/ui-customization.component';
import { DefaultThemeUiSettingsComponent } from './ui-customization/default-theme-ui-settings.component';
import { Theme2ThemeUiSettingsComponent } from './ui-customization/theme2-theme-ui-settings.component';
import { Theme3ThemeUiSettingsComponent } from './ui-customization/theme3-theme-ui-settings.component';
import { Theme4ThemeUiSettingsComponent } from './ui-customization/theme4-theme-ui-settings.component';
import { Theme5ThemeUiSettingsComponent } from './ui-customization/theme5-theme-ui-settings.component';
import { Theme6ThemeUiSettingsComponent } from './ui-customization/theme6-theme-ui-settings.component';
import { Theme7ThemeUiSettingsComponent } from './ui-customization/theme7-theme-ui-settings.component';
import { Theme8ThemeUiSettingsComponent } from './ui-customization/theme8-theme-ui-settings.component';
import { Theme9ThemeUiSettingsComponent } from './ui-customization/theme9-theme-ui-settings.component';
import { Theme10ThemeUiSettingsComponent } from './ui-customization/theme10-theme-ui-settings.component';
import { Theme11ThemeUiSettingsComponent } from './ui-customization/theme11-theme-ui-settings.component';

import { Theme12ThemeUiSettingsComponent } from './ui-customization/theme12-theme-ui-settings.component';




import { CreateOrEditUserModalComponent } from './users/create-or-edit-user-modal.component';
import { EditUserPermissionsModalComponent } from './users/edit-user-permissions-modal.component';
import { ImpersonationService } from './users/impersonation.service';
import { UsersComponent } from './users/users.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { CountoModule } from 'angular2-counto';
import { TextMaskModule } from 'angular2-text-mask';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { DropdownModule } from 'primeng/dropdown';

// Metronic
import { PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { PermissionTreeModalComponent } from './shared/permission-tree-modal.component';
import { WebhookSubscriptionComponent } from './webhook-subscription/webhook-subscription.component';
import { CreateOrEditWebhookSubscriptionModalComponent } from './webhook-subscription/create-or-edit-webhook-subscription-modal.component';
import { WebhookSubscriptionDetailComponent } from './webhook-subscription/webhook-subscription-detail.component';
import { WebhookEventDetailComponent } from './webhook-subscription/webhook-event-detail.component';
import { AppBsModalModule } from '@shared/common/appBsModal/app-bs-modal.module';
import {DynamicPropertyComponent} from '@app/admin/dynamic-properties/dynamic-property.component';
import {CreateOrEditDynamicPropertyModalComponent} from '@app/admin/dynamic-properties/create-or-edit-dynamic-property-modal.component';
import {DynamicEntityPropertyComponent} from '@app/admin/dynamic-properties/dynamic-entity-properties/dynamic-entity-property.component';
import {DynamicPropertyValueModalComponent} from '@app/admin/dynamic-properties/dynamic-property-value/dynamic-property-value-modal.component';
import {CreateDynamicEntityPropertyModalComponent} from '@app/admin/dynamic-properties/dynamic-entity-properties/create-dynamic-entity-property-modal.component';
import {DynamicEntityPropertyValueComponent} from '@app/admin/dynamic-properties/dynamic-entity-properties/value/dynamic-entity-property-value.component';
import {DynamicEntityPropertyListComponent} from '@app/admin/dynamic-properties/dynamic-entity-properties/dynamic-entity-property-list.component';
import {ManageValuesModalComponent} from '@app/admin/dynamic-properties/dynamic-entity-properties/value/manage-values-modal.component';
import {ManagerComponent} from '@app/admin/dynamic-properties/dynamic-entity-properties/value/manager.component';
import {SelectAnEntityModalComponent} from '@app/admin/dynamic-properties/select-an-entity-modal.component';;
import { TenantsAccountBillingsComponent } from './tenants-account-billings/tenants-account-billings.component';
import { ServicesComponent } from './services/services.component';;
import { FileUploadComponent } from './file-upload/file-upload.component'
;
import { PaymentComponent } from './payment/payment.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { CreateOrEditorderofferModelComponent } from './settings/create-or-edit-orderoffer';
import { OrderOfferComponent } from './OrderOffer/roles.component';
import { BillingsServiceProxy, BookingServiceProxy, HostedCheckoutServiceProxy, LocationServiceProxy, OrderOfferServiceProxy, TeamsServiceProxy, ZohoServiceProxy } from '@shared/service-proxies/service-proxies';
import { viewTenantModalComponent } from './tenants/view-tenant-modal.component';
import { BillingHostCurrencyLookupTableModalComponent } from './billingsHost/billings/billingsHost-currency-lookup-table-modal.component';
import { BillingHostComponent } from './billingsHost/billings/billingsHost.component';
import { CreateOrEditBillingHostModalComponent } from './billingsHost/billings/create-or-edit-billingsHost-modal.component';
import { ViewBillingsHostModalComponent } from './billingsHost/billings/view-billingsHost-modal.component';
import { ExportModalComponent } from './tenants/export-modal.component';
import { Ng2FlatpickrModule } from 'ng2-flatpickr';
import { CreateOrEditUserSettingModalComponent } from './users/create-or-edit-user-setting-modal.component';
import { DiagramModule } from '@syncfusion/ej2-angular-diagrams';
import { NgxFlowchartModule } from 'ngx-flowchart';
import { CreateOrEditBookingSettingsComponent } from './users/create-or-edit-booking-settings.component';
import { PlyrModule } from 'ngx-plyr';
import {BotModuleModule} from './botBuilder/bot-module.module';
import {BotServiceService} from './botBuilder/bot-service.service';
import { EditCaptionComponent } from './settings/edit-caption/edit-caption.component';
import { LazyLoadImageModule } from 'ng-lazyload-image';
import { UsageDetailsComponent } from './usage-details/usage-details.component';
import { TabViewModule } from 'primeng/tabview';
import { TabMenuModule } from 'primeng/tabmenu';
import { AllSettingsModalComponent } from './tenants/all-settings-modal/all-settings-modal.component';
import { CreateOrEditTeamsModalComponent } from './users/create-or-edit-teams-modal.component';
import { FacebookConnectComponent } from '@app/shared/layout/themes/facebook-connect/facebook-connect.component';
import { InstagramConnectComponent } from '@app/shared/layout/themes/instagram-connect/instagram-connect.component';
import { UserTicketsModalComponent } from './users/user-tickets-modal.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    // suppressScrollX: true
};

@NgModule({
    imports: [
      NgMultiSelectDropDownModule.forRoot(),
        FormsModule,
        NgbModule,
        ReactiveFormsModule,
        CommonModule,
        FileUploadModule,
        
        PlyrModule,
        ModalModule.forRoot(),
        TabsModule.forRoot(),
        TooltipModule.forRoot(),
        PopoverModule.forRoot(),
        BsDropdownModule.forRoot(),
        BsDatepickerModule.forRoot(),
        AdminRoutingModule,
        UtilsModule,
        AppCommonModule,
        TabMenuModule,
        TableModule,
        TreeModule,
        DragDropModule,
        ContextMenuModule,
        PaginatorModule,
        PrimeNgFileUploadModule,
        AutoCompleteModule,
        EditorModule,
        InputMaskModule,
        NgxChartsModule,
        CountoModule,
        TextMaskModule,
        ImageCropperModule,
        PerfectScrollbarModule,
        TabViewModule,
        DropdownModule,
        AppBsModalModule,
        Ng2FlatpickrModule,
        DiagramModule,
        NgxFlowchartModule,
        BotModuleModule,
        LazyLoadImageModule,
        NgbDropdownModule
    ],
    declarations: [
    AccountBillingBillingLookupTableModalComponent,
		ChatStatusesComponent,

		CreateOrEditChatStatuseModalComponent,
		ContactStatusesComponent,

		CreateOrEditContactStatuseModalComponent,
		ReceiptsComponent,
   
    
		ViewReceiptModalComponent,
		CreateOrEditReceiptModalComponent,
    ReceiptBankLookupTableModalComponent,
    ReceiptPaymentMethodLookupTableModalComponent,
		BanksComponent,

    BillingHostComponent,
    BillingHostCurrencyLookupTableModalComponent,
    CreateOrEditBillingHostModalComponent,
    ViewBillingsHostModalComponent,

		ViewBankModalComponent,
		CreateOrEditBankModalComponent,
		PaymentMethodsComponent,

		ViewPaymentMethodModalComponent,
		CreateOrEditPaymentMethodModalComponent,
		TenantServicesComponent,

		ViewTenantServiceModalComponent,
		CreateOrEditTenantServiceModalComponent,
    TenantServiceInfoSeedServiceLookupTableModalComponent,
		AccountBillingsComponent,

		ViewAccountBillingModalComponent,
		CreateOrEditAccountBillingModalComponent,
    AccountBillingInfoSeedServiceLookupTableModalComponent,
    AccountBillingServiceTypeLookupTableModalComponent,
    AccountBillingCurrencyLookupTableModalComponent,
		CurrenciesComponent,

		ViewCurrencyModalComponent,
		CreateOrEditCurrencyModalComponent,
		InfoSeedServicesComponent,

		ViewInfoSeedServiceModalComponent,
		CreateOrEditInfoSeedServiceModalComponent,
    InfoSeedServiceServiceTypeLookupTableModalComponent,
    InfoSeedServiceServiceStatusLookupTableModalComponent,
    InfoSeedServiceServiceFrquencyLookupTableModalComponent,
		ServiceFrquenciesComponent,

		ViewServiceFrquencyModalComponent,
		CreateOrEditServiceFrquencyModalComponent,
		ServiceStatusesComponent,

		ViewServiceStatusModalComponent,
		CreateOrEditServiceStatusModalComponent,
		ServiceTypesComponent,

		ViewServiceTypeModalComponent,
		CreateOrEditServiceTypeModalComponent,
		TemplateMessagesComponent,

		ViewTemplateMessageModalComponent,
		CreateOrEditTemplateMessageModalComponent,
        UsersComponent,
        PermissionComboComponent,
        RoleComboComponent,
        CreateOrEditUserModalComponent,
        UserTicketsModalComponent,

        CreateOrEditTeamsModalComponent,
        
        EditUserPermissionsModalComponent,
        PermissionTreeComponent,
        FeatureTreeComponent,
        OrganizationUnitsTreeComponent,
        RolesComponent,
        OrderOfferComponent,
        CreateOrEditRoleModalComponent,
        AuditLogsComponent,
        AuditLogDetailModalComponent,
        HostSettingsComponent,
        InstallComponent,
        MaintenanceComponent,
        EditionsComponent,
        CreateEditionModalComponent,
        EditEditionModalComponent,
        MoveTenantsToAnotherEditionModalComponent,
        LanguagesComponent,
        LanguageTextsComponent,
        CreateOrEditLanguageModalComponent,
        TenantsComponent,
        CreateTenantModalComponent,
        EditTenantModalComponent,
        viewTenantModalComponent,
        TenantFeaturesModalComponent,
        TenantServicesModalComponent,
        TenantSettingsModalComponent,
        CreateOrEditLanguageModalComponent,
        EditTextModalComponent,
        OrganizationUnitsComponent,
        OrganizationTreeComponent,
        OrganizationUnitMembersComponent,
        OrganizationUnitRolesComponent,
        CreateOrEditUnitModalComponent,
        TenantSettingsComponent,
        HostDashboardComponent,
        EditionComboComponent,
        InvoiceComponent,
        SubscriptionManagementComponent,
        AddMemberModalComponent,
        AddRoleModalComponent,
        DemoUiComponentsComponent,
        DemoUiDateTimeComponent,
        DemoUiSelectionComponent,
        DemoUiFileUploadComponent,
        DemoUiInputMaskComponent,
        DemoUiEditorComponent,
        UiCustomizationComponent,
        DefaultThemeUiSettingsComponent,
        Theme2ThemeUiSettingsComponent,
        Theme3ThemeUiSettingsComponent,
        Theme4ThemeUiSettingsComponent,
        Theme5ThemeUiSettingsComponent,
        Theme6ThemeUiSettingsComponent,
        Theme7ThemeUiSettingsComponent,
        Theme8ThemeUiSettingsComponent,
        Theme9ThemeUiSettingsComponent,
        Theme10ThemeUiSettingsComponent,
        Theme11ThemeUiSettingsComponent,
        Theme12ThemeUiSettingsComponent,
        PermissionTreeModalComponent,
        WebhookSubscriptionComponent,
        CreateOrEditWebhookSubscriptionModalComponent,
        WebhookSubscriptionDetailComponent,
        WebhookEventDetailComponent,
        DynamicPropertyComponent,
        CreateOrEditDynamicPropertyModalComponent,
        DynamicPropertyValueModalComponent,
        DynamicEntityPropertyComponent,
        CreateDynamicEntityPropertyModalComponent,
        DynamicEntityPropertyValueComponent,
        ManageValuesModalComponent,
        ManagerComponent,
        DynamicEntityPropertyListComponent,
        SelectAnEntityModalComponent,
        TenantsAccountBillingsComponent,
        ServicesComponent,
        FileUploadComponent,
        PaymentComponent ,
          CreateOrEditorderofferModelComponent,
          ExportModalComponent ,
          CreateOrEditUserSettingModalComponent,
          CreateOrEditBookingSettingsComponent,
          EditCaptionComponent,
          UsageDetailsComponent,
          AllSettingsModalComponent,
          FacebookConnectComponent,
          InstagramConnectComponent
      ],
    exports: [
       CreateOrEditorderofferModelComponent ,
        AddMemberModalComponent,
        AddRoleModalComponent
    ],
    providers: [
      TeamsServiceProxy,
        ImpersonationService,
        TreeDragDropService,
        BillingsServiceProxy,
        HostedCheckoutServiceProxy,
        LocationServiceProxy,
        BookingServiceProxy,
        ZohoServiceProxy,
        BotServiceService,
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
        { provide: PERFECT_SCROLLBAR_CONFIG, useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG }
    ],
    schemas: [
      CUSTOM_ELEMENTS_SCHEMA,
      NO_ERRORS_SCHEMA
      ],
})
export class AdminModule { }
