import { NgModule } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { ChatStatusesComponent } from './chatStatuses/chatStatuses/chatStatuses.component';
import { ContactStatusesComponent } from './contactStatuses/contactStatuses/contactStatuses.component';
import { ReceiptsComponent } from './receipts/receipts/receipts.component';
import { BanksComponent } from './banks/banks/banks.component';
import { PaymentMethodsComponent } from './paymentMethods/paymentMethods/paymentMethods.component';
import { TenantServicesComponent } from './tenantServices/tenantServices/tenantServices.component';
import { AccountBillingsComponent } from './accountBillings/accountBillings/accountBillings.component';
import { CurrenciesComponent } from './currencies/currencies/currencies.component';
import { InfoSeedServicesComponent } from './infoSeedServices/infoSeedServices/infoSeedServices.component';
import { ServiceFrquenciesComponent } from './serviceFrequencies/serviceFrquencies/serviceFrquencies.component';
import { ServiceStatusesComponent } from './serviceStatuses/serviceStatuses/serviceStatuses.component';
import { ServiceTypesComponent } from './serviceTypes/serviceTypes/serviceTypes.component';
import { TemplateMessagesComponent } from './templateMessages/templateMessages/templateMessages.component';
import { AuditLogsComponent } from './audit-logs/audit-logs.component';
import { HostDashboardComponent } from './dashboard/host-dashboard.component';
import { DemoUiComponentsComponent } from './demo-ui-components/demo-ui-components.component';
import { EditionsComponent } from './editions/editions.component';
import { InstallComponent } from './install/install.component';
import { LanguageTextsComponent } from './languages/language-texts.component';
import { LanguagesComponent } from './languages/languages.component';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { OrganizationUnitsComponent } from './organization-units/organization-units.component';
import { RolesComponent } from './roles/roles.component';
import { HostSettingsComponent } from './settings/host-settings.component';
import { TenantSettingsComponent } from './settings/tenant-settings.component';
import { InvoiceComponent } from './subscription-management/invoice/invoice.component';
import { SubscriptionManagementComponent } from './subscription-management/subscription-management.component';
import { TenantsComponent } from './tenants/tenants.component';
import { UiCustomizationComponent } from './ui-customization/ui-customization.component';
import { UsersComponent } from './users/users.component';

import { WebhookSubscriptionComponent } from './webhook-subscription/webhook-subscription.component';
import { WebhookSubscriptionDetailComponent } from './webhook-subscription/webhook-subscription-detail.component';
import { WebhookEventDetailComponent } from './webhook-subscription/webhook-event-detail.component';
import { DynamicPropertyComponent } from '@app/admin/dynamic-properties/dynamic-property.component';
import { DynamicEntityPropertyComponent } from '@app/admin/dynamic-properties/dynamic-entity-properties/dynamic-entity-property.component';
import { DynamicEntityPropertyValueComponent } from '@app/admin/dynamic-properties/dynamic-entity-properties/value/dynamic-entity-property-value.component';
import { TenantsAccountBillingsComponent } from './tenants-account-billings/tenants-account-billings.component';
import { ServicesComponent } from './services/services.component';
import { PaymentComponent } from './payment/payment.component';
import { FileUploadComponent } from './file-upload/file-upload.component';
import { OrderOfferComponent } from './OrderOffer/roles.component';
import { BillingHostComponent } from './billingsHost/billings/billingsHost.component';
import { AuthGuardService } from '@app/auth/service/auth-guard.service';
import {BotFlowsComponent} from './botBuilder/bot-flows.component'
import {BotBuilderPageComponent} from './botBuilder/bot-builder-page/bot-builder-page.component'
import { UsageDetailsComponent } from './usage-details/usage-details.component';
import { FacebookConnectComponent } from '@app/shared/layout/themes/facebook-connect/facebook-connect.component';
import { InstagramConnectComponent } from '@app/shared/layout/themes/instagram-connect/instagram-connect.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'fileUpload', component: FileUploadComponent },
                    { path: 'chatStatuses/chatStatuses', component: ChatStatusesComponent, data: { permission: 'Pages.Administration.ChatStatuses' }  },
                    { path: 'contactStatuses/contactStatuses', component: ContactStatusesComponent, data: { permission: 'Pages.Administration.ContactStatuses' }  },
                    { path: 'receipts/receipts', component: ReceiptsComponent, data: { permission: 'Pages.Administration.Receipts' }  },

                    { path: 'banks/banks', component: BanksComponent, data: { permission: 'Pages.Administration.Banks' }  },
                    
                    { path: 'billingsHost/billings', component: BillingHostComponent, data: { permission: 'Pages.Administration.Host.Dashboard' }  },
         { path: 'facebook-connect', component: FacebookConnectComponent },
         { path: 'instagram-connect', component: InstagramConnectComponent },
                    { path: 'paymentMethods/paymentMethods', component: PaymentMethodsComponent, data: { permission: 'Pages.Administration.PaymentMethods' }  },
                    { path: 'tenantServices', component: TenantServicesComponent, data: { permission: 'Pages.Administration.TenantServices' }  },
                    { path: 'accountBillings/accountBillings', component: AccountBillingsComponent, data: { permission: 'Pages.Administration.AccountBillings' }  },
                    { path: 'tenantsAccountBillings/tenantsAccountBillings', component: TenantsAccountBillingsComponent  },
                    { path: 'currencies/currencies', component: CurrenciesComponent, data: { permission: 'Pages.Administration.Currencies' }  },
                    { path: 'services', component: ServicesComponent },
                    { path: 'payment/payment', component: PaymentComponent },
                    { path: 'infoSeedServices/infoSeedServices', component: InfoSeedServicesComponent, data: { permission: 'Pages.Administration.InfoSeedServices' }  },
                    { path: 'serviceFrequencies/serviceFrquencies', component: ServiceFrquenciesComponent, data: { permission: 'Pages.Administration.ServiceFrquencies' }  },
                    { path: 'serviceStatuses/serviceStatuses', component: ServiceStatusesComponent, data: { permission: 'Pages.Administration.ServiceStatuses' }  },
                    { path: 'serviceTypes/serviceTypes', component: ServiceTypesComponent, data: { permission: 'Pages.Administration.ServiceTypes' }  },
                    { path: 'quickAnswerMessages', component: TemplateMessagesComponent, data: { permission: 'Pages.Administration.TemplateMessages' } , canActivate : [AuthGuardService] },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.Administration.Users' } , canActivate : [AuthGuardService]},
                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Administration.Roles' } },
                    { path: 'OrderOffer', component: OrderOfferComponent},
                    { path: 'auditLogs', component: AuditLogsComponent, data: { permission: 'Pages.Administration.AuditLogs' } },
                    { path: 'maintenance', component: MaintenanceComponent, data: { permission: 'Pages.Administration.Host.Maintenance' } },
                    { path: 'hostSettings', component: HostSettingsComponent, data: { permission: 'Pages.Administration.Host.Settings' } },
                    { path: 'editions', component: EditionsComponent, data: { permission: 'Pages.Editions' } },
                    { path: 'languages', component: LanguagesComponent, data: { permission: 'Pages.Administration.Languages' } },
                    { path: 'languages/:name/texts', component: LanguageTextsComponent, data: { permission: 'Pages.Administration.Languages.ChangeTexts' } },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Tenants' } },
                    { path: 'organization-units', component: OrganizationUnitsComponent, data: { permission: 'Pages.Administration.OrganizationUnits' } },
                    { path: 'subscription-management', component: SubscriptionManagementComponent, data: { permission: 'Pages.Administration.Tenant.SubscriptionManagement' } },
                    { path: 'invoice/:paymentId', component: InvoiceComponent, data: { permission: 'Pages.Administration.Tenant.SubscriptionManagement' } },
                    { path: 'tenantSettings', component: TenantSettingsComponent, data: { permission: 'Pages.Administration.Tenant.Settings' }, canActivate : [AuthGuardService] },
                    { path: 'hostDashboard', component: HostDashboardComponent, data: { permission: 'Pages.Administration.Host.Dashboard' } },
                    { path: 'demo-ui-components', component: DemoUiComponentsComponent, data: { permission: 'Pages.DemoUiComponents' } },
                    { path: 'install', component: InstallComponent },
                    { path: 'ui-customization', component: UiCustomizationComponent , canActivate : [AuthGuardService]},
                    { path: 'webhook-subscriptions', component: WebhookSubscriptionComponent, data: { permission: 'Pages.Administration.WebhookSubscription' } },
                    { path: 'webhook-subscriptions-detail', component: WebhookSubscriptionDetailComponent, data: { permission: 'Pages.Administration.WebhookSubscription.Detail' } },
                    { path: 'webhook-event-detail', component: WebhookEventDetailComponent, data: { permission: 'Pages.Administration.WebhookSubscription.Detail' } },
                    { path: 'dynamic-property', component: DynamicPropertyComponent, data: { permission: 'Pages.Administration.DynamicProperties' } },
                    { path: 'dynamic-entity-property/:entityFullName', component: DynamicEntityPropertyComponent, data: { permission: 'Pages.Administration.DynamicEntityProperties' } },
                    { path: 'dynamic-entity-property-value/manage-all/:entityFullName/:rowId', component: DynamicEntityPropertyValueComponent, data: { permission: 'Pages.Administration.DynamicEntityProperties' } },
                    { path: 'botFlows', component: BotFlowsComponent,canActivate : [AuthGuardService]},
                    { path: 'botBuilder/:id', component: BotBuilderPageComponent,canActivate : [AuthGuardService]},
                    { path: 'usage-details', component: UsageDetailsComponent,canActivate : [AuthGuardService]},
                    { path: '', redirectTo: 'hostDashboard', pathMatch: 'full' },
                    { path: '**', redirectTo: 'hostDashboard' }
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class AdminRoutingModule {

    constructor(
        private router: Router
    ) {
        router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                window.scroll(0, 0);
            }
        });
    }
}
