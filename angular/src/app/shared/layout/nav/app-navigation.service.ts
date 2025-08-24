import { PermissionCheckerService } from 'abp-ng2-module';
import { AppSessionService } from '@shared/common/session/app-session.service';

import { Injectable } from '@angular/core';
import { AppMenu } from './app-menu';
import { AppMenuItem } from './app-menu-item';
import { TenantTypeEunm } from '@shared/service-proxies/service-proxies';
import { ThemeHelper } from '../themes/ThemeHelper';

@Injectable()
export class AppNavigationService {
    [x: string]: any;

    listappmenuitem: AppMenuItem[];
    //appSession: AppSessionService;
    constructor(
        // this.appSession = injector.get(AppSessionService);
        private _permissionCheckerService: PermissionCheckerService,
        private _appSessionService: AppSessionService
    ) {

    }

    getMenu(): AppMenu {


        let theme = ThemeHelper.getTheme();


        this.listappmenuitem = [new AppMenuItem('Dashboard', 'Pages.Administration.Host.Dashboard', 'bi bi-house', '/app/admin/hostDashboard')]
        this.listappmenuitem.push(new AppMenuItem('Tenants', 'Pages.Tenants', 'bi bi-house', '/app/admin/tenants'))
        this.listappmenuitem.push(new AppMenuItem('Editions', 'Pages.Editions', 'bi bi-house', '/app/admin/editions'));
        this.listappmenuitem.push(new AppMenuItem('Dashboard', 'Pages.Tenant.Dashboard', 'bi bi-house', '/app/main/dashboard'))

        if (theme === 'theme12') {
            this.listappmenuitem.push(new AppMenuItem('teamInbox', 'Pages.TeamInbox', 'bi bi-chat', '/app/main/teamInbox/teamInbox12'))
        } else {
            this.listappmenuitem.push(new AppMenuItem('teamInbox', 'Pages.TeamInbox', 'bi bi-chat', '/app/main/teamInbox/teamInbox12'))
        }



        this.listappmenuitem.push(new AppMenuItem('tickets', 'Pages.LiveChat', 'bi bi-chat-dots', '/app/main/liveChat'))


        //this.listappmenuitem.push(new AppMenuItem('Contacts', 'Pages.Contacts', 'fas fa-address-book', '/app/main/contacts/contacts'))
        //this.listappmenuitem.push(new AppMenuItem('External Contacts', 'Pages.Contacts', 'fas fa-address-book', '/app/main/externalContacts'))

        this.listappmenuitem.push(
            new AppMenuItem('contacts', 'Pages.Contacts', 'bi bi-person-rolodex', '/app/main/contacts/contacts')
        );
        //if Delivery tenant

        // this.listappmenuitem.push(new AppMenuItem('Delivery Orders', 'Pages.DeliveryOrders', 'fas fa-shopping-cart', '/app/main/deliveryorder/orders'))
        // this.listappmenuitem.push(new AppMenuItem('Delivery Location', 'Pages.DeliveryLocation', 'fas fa-map-marker-alt', '/app/main/deliverylocation'))


        //21
        // this.listappmenuitem.push(new AppMenuItem('Booking', 'Pages.Booking', 'fas fa-bookmark', '/app/main/booking/booking'));

        //34
        if (this._appSessionService.tenantId === 34) {
            this.listappmenuitem.push(new AppMenuItem('Update Item', 'Pages.UpdateItem', 'bi bi-basket', '/app/main/ctown/updateItem'));

        }

        //36
        // this.listappmenuitem.push(new AppMenuItem('Orders Maintenances', 'Pages.OrdersMaintenances', 'fas fa-wrench', '/app/main/maintenances/maintenances'));
        this.listappmenuitem.push(new AppMenuItem('orders', 'Pages.Orders', 'bi bi-basket', '/app/main/orders/orders'));
        this.listappmenuitem.push(new AppMenuItem('archivedOrders', 'Pages.Orders', 'bi bi-archive', '/app/main/orders/ordersArchived'));

        // this.listappmenuitem.push(new AppMenuItem('requestPage', 'Pages.SellingRequests', 'bi bi-bag-check', '/app/main/sellingRequest'));

        this.listappmenuitem.push(new AppMenuItem('deliveryCost', 'Pages.DeliveryCost', 'bi bi-cash-coin', '/app/main/deliveryCost'));

        // this.listappmenuitem.push(new AppMenuItem('Assets', 'Pages.Assets', 'bi bi-images', '/app/main/assets'));
        this.listappmenuitem.push(new AppMenuItem('calendar', 'Pages.Booking', 'bi bi-calendar', '/app/main/calendar'));

        this.listappmenuitem.push(new AppMenuItem('template', 'Pages.MessageTemplate', 'bi bi-chat-text', '/app/main/messageTemplate'));
        this.listappmenuitem.push(new AppMenuItem('campaign', 'Pages.MessageCampaign', 'bi bi-megaphone', '', [], [
            new AppMenuItem('Groups', "Pages.Contacts", 'fa fa-users', '/app/main/groupcontact'),
            new AppMenuItem('campaign', "Pages.MessageCampaign", 'bi bi-megaphone', '/app/main/messageCampaign'),
            new AppMenuItem('campaignContacts', "Pages.Contacts.ExternalContact", 'bi bi-envelope-paper', '/app/main/externalContacts'),
        ]));
        this.listappmenuitem.push(new AppMenuItem('botFlows', 'Pages.Botflow', 'bi bi-robot', '/app/admin/botFlows'));
        // this.listappmenuitem.push(new AppMenuItem('pushMessage', 'Pages.MessageConversation', 'bi bi-send', '/app/main/messageConversation'));


        this.listappmenuitem.push(new AppMenuItem('location', 'Pages.Location', 'bi bi-geo', '/app/main/location'));
        this.listappmenuitem.push(new AppMenuItem('menus', 'Pages.Menus', 'bi bi-receipt', '/app/main/menus/menus'));


        this.listappmenuitem.push(new AppMenuItem('evaluation', 'Pages.Evaluation', 'bi bi-star', '/app/main/evaluation'))
        // this.listappmenuitem.push(new AppMenuItem('departments', 'Pages.Department', 'bi bi-building-add', '/app/main/departments'));

        this.listappmenuitem.push(new AppMenuItem('branches', 'Pages.Branches', 'bi bi-building-add', '/app/main/branchess/branchess'));


        this.listappmenuitem.push(new AppMenuItem('Channels', 'Pages.Channels', 'bi bi-megaphone', '', [], [
            new AppMenuItem('facebook', 'Pages.Facebook', 'bi bi-facebook', '/app/admin/facebook-connect'),
            new AppMenuItem('instagram', 'Pages.Instagram', 'bi bi-instagram', '/app/admin/instagram-connect')

        ]));

        this.listappmenuitem.push(new AppMenuItem('Administration', 'Pages.Administration', 'bi bi-menu-button', '', [], [
            new AppMenuItem('Users', 'Pages.Administration.Users', 'far fa-circle', '/app/admin/users'),

            new AppMenuItem('BillingsHost', '', 'flaticon-interface-8', '', [], [
                new AppMenuItem('InvoiceHost', "Pages.Administration.Host.Dashboard", 'far fa-circle', '/app/admin/billingsHost/billings'),
            ]),





            
            // new AppMenuItem('billings', 'Pages.Billings', 'far fa-circle', '/app/main/billings/billings'),
            new AppMenuItem('ChatStatuses', 'Pages.Administration.ChatStatuses', 'far fa-circle', '/app/admin/chatStatuses/chatStatuses'),
            new AppMenuItem('ContactStatuses', 'Pages.Administration.ContactStatuses', 'far fa-circle', '/app/admin/contactStatuses/contactStatuses'),
            new AppMenuItem('TenantsAccountBillings', 'Pages.Administration.Host.AccountBillings', 'far fa-circle', '/app/admin/tenantsAccountBillings/tenantsAccountBillings'),
            new AppMenuItem('Services', 'Pages.Administration.InfoSeedServices', 'far fa-circle', '/app/admin/services'),
            new AppMenuItem('quickAnswers', 'Pages.Administration.TemplateMessages', 'far fa-circle', '/app/admin/quickAnswerMessages'),
            new AppMenuItem('usageDetails', 'Pages_Administration_Usage_details', 'far fa-circle', '/app/admin/usage-details'),

            new AppMenuItem('Maintenance', 'Pages.Administration.Host.Maintenance', 'far fa-circle', '/app/admin/maintenance'),
            
            new AppMenuItem('Settings', 'Pages.Administration.Host.Settings', 'far fa-circle', '/app/admin/hostSettings'),
            new AppMenuItem('Settings', 'Pages.Administration.Tenant.Settings', 'far fa-circle', '/app/admin/tenantSettings'),
            new AppMenuItem('Receipts', 'Pages.Administration.Receipts', 'far fa-circle', '/app/admin/receipts/receipts'),
            new AppMenuItem('TenantServices', 'Pages.Administration.TenantServices', 'far fa-circle', '/app/admin/tenantServices'),
            new AppMenuItem('AccountBillings', 'Pages.Administration.AccountBillings', 'far fa-circle', '/app/admin/accountBillings/accountBillings'),
            // new AppMenuItem('Languages', 'Pages.Administration.Languages', 'flaticon-tabs', '/app/admin/languages', ['/app/admin/languages/{name}/texts']),
            new AppMenuItem('AuditLogs', 'Pages.Administration.AuditLogs', 'far fa-circle', '/app/admin/auditLogs'),
            new AppMenuItem('Subscription', 'Pages.Administration.Tenant.SubscriptionManagement', 'far fa-circle', '/app/admin/subscription-management'),
            new AppMenuItem('reservedWords', null, 'far fa-circle', '/app/main/reserved-words'),
            new AppMenuItem('VisualSettings', null, 'far fa-circle', '/app/admin/ui-customization'),
            new AppMenuItem('WebhookSubscriptions', 'Pages.Administration.WebhookSubscription', 'far fa-circle', '/app/admin/webhook-subscriptions'),
            new AppMenuItem('DynamicProperties', 'Pages.Administration.DynamicProperties', 'far fa-circle', '/app/admin/dynamic-property'),
            new AppMenuItem('OrganizationUnits', 'Pages.Administration.OrganizationUnits', 'far fa-circle', '/app/admin/organization-units'),
            // new AppMenuItem('Roles', 'Pages.Administration.Roles', 'flaticon-suitcase', '/app/admin/roles'),
        ]),)
        return new AppMenu('MainMenu', 'MainMenu', this.listappmenuitem);




    }

    checkChildMenuItemPermission(menuItem): boolean {

        for (let i = 0; i < menuItem.items.length; i++) {
            let subMenuItem = menuItem.items[i];

            if (subMenuItem.permissionName === '' || subMenuItem.permissionName === null) {

                if (subMenuItem.route) {
                    return true;
                }
            } else if (this._permissionCheckerService.isGranted(subMenuItem.permissionName)) {

                return true;
            }

            if (subMenuItem.items && subMenuItem.items.length) {

                let isAnyChildItemActive = this.checkChildMenuItemPermission(subMenuItem);
                if (isAnyChildItemActive) {
                    return true;
                }
            }
        }
        return false;
    }

    showMenuItem(menuItem: AppMenuItem): boolean {
        if (menuItem.permissionName === 'Pages.Administration.Tenant.SubscriptionManagement' && this._appSessionService.tenant && !this._appSessionService.tenant.edition) {
            return false;
        }

        let hideMenuItem = false;

        if (menuItem.requiresAuthentication && !this._appSessionService.user) {
            hideMenuItem = true;
        }

        if (menuItem.permissionName && !this._permissionCheckerService.isGranted(menuItem.permissionName)) {
            hideMenuItem = true;
        }

        if (this._appSessionService.tenant || !abp.multiTenancy.ignoreFeatureCheckForHostUsers) {
            if (menuItem.hasFeatureDependency() && !menuItem.featureDependencySatisfied()) {
                hideMenuItem = true;
            }
        }

        if (!hideMenuItem && menuItem.items && menuItem.items.length) {
            return this.checkChildMenuItemPermission(menuItem);
        }

        return !hideMenuItem;
    }

    /**
     * Returns all menu items recursively
     */
    getAllMenuItems(): AppMenuItem[] {
        let menu = this.getMenu();
        let allMenuItems: AppMenuItem[] = [];
        menu.items.forEach(menuItem => {
            allMenuItems = allMenuItems.concat(this.getAllMenuItemsRecursive(menuItem));
        });

        return allMenuItems;
    }

    private getAllMenuItemsRecursive(menuItem: AppMenuItem): AppMenuItem[] {
        if (!menuItem.items) {
            return [menuItem];
        }

        let menuItems = [menuItem];
        menuItem.items.forEach(subMenu => {
            menuItems = menuItems.concat(this.getAllMenuItemsRecursive(subMenu));
        });

        return menuItems;
    }
}
