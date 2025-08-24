import { CreateOrEditSpecificationPageComponent } from "./menus/menus/create-or-edit-specification-page.component";
//import { TeamInboxComponent } from './teamInbox/teaminbox.component';
import { NgModule } from "@angular/core";
import { BillingsComponent } from "./billings/billings/billings.component";
import { ContactsComponent } from "./contacts/contacts/contacts.component";
import { RouterModule } from "@angular/router";
import { GendersComponent } from "./genders/genders/genders.component";
import { CitiesComponent } from "./cities/cities/cities.component";
import { MenuDetailsComponent } from "./menuDetails/menuDetails/menuDetails.component";
import { MenusComponent } from "./menus/menus/menus.component";
import { CreateOrEditAdvancedMenuPageComponent } from "./menus/menus/create-or-edit-advanced-menu-page.component";
import { CreateOrEditMenuPageComponent } from "./menus/menus/create-or-edit-menu-page.component";
import { CreateOrEditAddOnsComponent } from "./menus/menus/create-or-edit-add-ons.component";

import { MenuItemStatusesComponent } from "./menuItemStatuses/menuItemStatuses/menuItemStatuses.component";
import { ItemsComponent } from "./items/items/items.component";
import { MenuCategoriesComponent } from "./menuCategories/menuCategories/menuCategories.component";
import { PaymentTestComponent } from "./payment-test/payment-test.component";
import { OrdersComponent } from "./order/order.component";
import { BranchesComponent } from "./areaLocation/branches.component";
import { BranchessComponent } from "./branchess/branchess.component";
import { LocationComponent } from "./location/location.component";
import { EvaluationComponent } from "./evaluation/evaluation.component";
import { DeliveryLocationComponent } from "./deliverylocation/location.component";
import { DeliveryOrdersComponent } from "./deliveryorder/order.component";
import { ForcatsComponent } from "./forcats/forcats.component";
import { BookingComponent } from "./booking/booking.component";
import { PaymentComponent } from "./payment/payment.component";
import { ctownComponent } from "./CtownUpdate/ctown.component";
import { MaintenanceComponent } from "./Maintenance/Maintenance.component";
import { OrdersArchivedComponent } from "./order/orderArchived.component";
import { ManageAdvancedMenuComponent } from "./menus/menus/manage-advanced-menu.component";
import { SealingRequestComponent } from "./sealing-request/sealing-request.component";
import { LiveChatComponent } from "./liveChat/liveChat.component";
import { AssetsComponent } from "./assets/assets.component";
import { DepartmentsComponent } from "./departments/departments.component";

import { DeliveryCostComponent } from "./delivery-cost/delivery-cost.component";
import { ChatThem12Component } from "./chat-them12/chat-them12.component";
import { ChatBarComponent } from "@app/shared/layout/chat/chat-bar.component";
import { MessageTemplateComponent } from "./message-template/message-template.component";
import { MessageCampaignComponent } from "./message-campaign/message-campaign.component";
import { SendCampaignComponent } from "./message-campaign/send-campaign.component";
import { ExternalContactsComponent } from "./external-contacts/external-contact.component";
import { MessageConversationComponent } from "./message-conversation/message-conversation.component";
import { CalendarComponent } from "./calendar/calendar/calendar.component";
import { AuthGuardService } from "@app/auth/service/auth-guard.service";
import { ReservedWordsComponent } from "./reserved-words/reserved-words.component";
import { MainDashboardComponent } from "./main-dashboard/main-dashboard.component";
import { ContactGroupComponent } from "./contacts/contactGroup/contactGroup.component";
import { CreateGroupPageComponent } from "./contacts/contactGroup/create-group-page/create-group-page.component";
import { EditGroupComponent } from "./contacts/contactGroup/edit-group/edit-group.component";
import { FacebookTemplateComponent } from "./message-template/Facebook-Template/Facebook-Template.component";
import { EditTemplateFacebookComponent } from "./message-template/Edit-Template-Facebook/Edit-Template-Facebook.component";

// import { SealingRequestComponent } from './sealing-request/sealing-request.component';
// import { SealingRequestArchivedComponent } from './sealing-request/sealing-requestArchived.component';

// import { AreasComponent } from './branch/branches.component';
// import { BranchesComponent } from './branches/branches.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: "",
                children: [
                    // {
                    //     path: "billings/billings",
                    //     component: BillingsComponent,
                    //     data: { permission: "Pages.Billings" },
                    // },

                    {
                        path: "contacts/contacts",
                        component: ContactsComponent,
                        data: { permission: "Pages.Contacts" },
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "externalContacts",
                        component: ExternalContactsComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "groupcontact",
                        component: ContactGroupComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "group/creategroup",
                        component: CreateGroupPageComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "group/editgroup",
                        component: EditGroupComponent,
                        canActivate: [AuthGuardService],
                    },
                    //{ path: 'teamInbox/teamInbox', component:TeamInboxComponent, canActivate : [AuthGuardService]   },
                    {
                        path: "teamInbox/teamInbox12",
                        component: ChatThem12Component,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "location",
                        component: LocationComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "deliverylocation",
                        component: DeliveryLocationComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "evaluation",
                        component: EvaluationComponent,
                        canActivate: [AuthGuardService],
                    },
                    { path: "forcats/forcats", component: ForcatsComponent },
                    { path: "booking/booking", component: BookingComponent },
                    {
                        path: "ctown/updateItem",
                        component: ctownComponent,
                        canActivate: [AuthGuardService],
                    },

                    {
                        path: "liveChat",
                        component: LiveChatComponent,
                        canActivate: [AuthGuardService],
                    },
                    // {
                    //     path: "departments",
                    //     component: DepartmentsComponent,
                    //     canActivate: [AuthGuardService],
                    // },

                    {
                        path: "maintenances/maintenances",
                        component: MaintenanceComponent,
                        canActivate: [AuthGuardService],
                    },

                    {
                        path: "genders/genders",
                        component: GendersComponent,
                        data: { permission: "Pages.Genders" },
                    },
                    {
                        path: "cities/cities",
                        component: CitiesComponent,
                        data: { permission: "Pages.Cities" },
                    },
                    {
                        path: "branchess/branchess",
                        component: BranchessComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "menuDetails/menuDetails",
                        component: MenuDetailsComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "menus/menus",
                        component: MenusComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "CreateOrEditAdvancedMenu",
                        component: CreateOrEditAdvancedMenuPageComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "CreateAddOns",
                        component: CreateOrEditAddOnsComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "CreateOptions",
                        component: CreateOrEditSpecificationPageComponent,
                        canActivate: [AuthGuardService],
                    },

                    {
                        path: "CreateOrEditMenu",
                        component: CreateOrEditMenuPageComponent,
                        canActivate: [AuthGuardService],
                    },

                    {
                        path: "ManageAdvancedMenu/ManageAdvancedMenu",
                        component: ManageAdvancedMenuComponent,
                        canActivate: [AuthGuardService],
                    },

                    {
                        path: "orders/orders",
                        component: OrdersComponent,
                        canActivate: [AuthGuardService],
                    },
                    // {
                    //     path: "sellingRequest",
                    //     component: SealingRequestComponent,
                    //     canActivate: [AuthGuardService],
                    // },

                    // {
                    //     path: "assets",
                    //     component: AssetsComponent,
                    //     canActivate: [AuthGuardService],
                    // },
                    {
                        path: "calendar",
                        component: CalendarComponent,
                        canActivate: [AuthGuardService],
                    },

                    {
                        path: "deliveryCost",
                        component: DeliveryCostComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "messageTemplate",
                        component: MessageTemplateComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "start",
                        component: FacebookTemplateComponent,
                        canActivate: [AuthGuardService],
            
                    },
                    {
                        path:"EditTemplateFacebook",
                        component:EditTemplateFacebookComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "messageCampaign",
                        component: MessageCampaignComponent,
                        canActivate: [AuthGuardService],
                    },
                    // {
                    //     path: "messageConversation",
                    //     component: MessageConversationComponent,
                    //     canActivate: [AuthGuardService],
                    // },
                    {
                        path: "sendCampaign",
                        component: SendCampaignComponent,
                        canActivate: [AuthGuardService],
                        canDeactivate: [
                            (sendCampaignComponent: SendCampaignComponent) =>
                                sendCampaignComponent
                                    .canLeavePage()
                                    .then((canLeave) => {
                                        if (canLeave) return true;
                                        else return false;
                                    }),
                        ],
                    },
                 
                    {
                        path: "reserved-words",
                        component: ReservedWordsComponent,
                        canActivate: [AuthGuardService],
                    },

                    {
                        path: "orders/ordersArchived",
                        component: OrdersArchivedComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "deliveryorder/orders",
                        component: DeliveryOrdersComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "areaLocation/areaLocation",
                        component: BranchesComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "menuItemStatuses/menuItemStatuses",
                        component: MenuItemStatusesComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "items/items",
                        component: ItemsComponent,
                        canActivate: [AuthGuardService],
                    },
                    {
                        path: "menuCategories/menuCategories",
                        component: MenuCategoriesComponent,
                        canActivate: [AuthGuardService],
                    },
                    // { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Tenant.Dashboard' }, canActivate : [AuthGuardService]  },
                    {
                        path: "dashboard",
                        component: MainDashboardComponent,
                        data: { permission: "Pages.Tenant.Dashboard" },
                        canActivate: [AuthGuardService],
                    },
                    { path: "payment", component: PaymentComponent },
                    //{ path: 'payment', component: PaymentTestComponent },
                    { path: "paymenttest", component: PaymentTestComponent },
                    { path: "", redirectTo: "dashboard", pathMatch: "full" },
                    { path: "**", redirectTo: "dashboard" },
                ],
            },
        ]),
    ],
    exports: [RouterModule],
})
export class MainRoutingModule {}
