// import { TeamInboxComponent } from './teamInbox/teaminbox.component';
import { CommonModule, DatePipe } from '@angular/common';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA, LOCALE_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BillingsComponent } from './billings/billings/billings.component';
import { ViewBillingModalComponent } from './billings/billings/view-billing-modal.component';
import { CreateOrEditBillingModalComponent } from './billings/billings/create-or-edit-billing-modal.component';
import { BillingCurrencyLookupTableModalComponent } from './billings/billings/billing-currency-lookup-table-modal.component';
import { ContactsComponent } from './contacts/contacts/contacts.component';
import { ViewContactModalComponent } from './contacts/contacts/view-contact-modal.component';
import { CreateOrEditContactModalComponent } from './contacts/contacts/create-or-edit-contact-modal.component';
import { ContactChatStatuseLookupTableModalComponent } from './contacts/contacts/contact-chatStatuse-lookup-table-modal.component';
import { ContactContactStatuseLookupTableModalComponent } from './contacts/contacts/contact-contactStatuse-lookup-table-modal.component';
import { MasterDetailChild_Receipt_ReceiptDetailsComponent } from './receiptDetails/receiptDetails/masterDetailChild_Receipt_receiptDetails.component';
import { MasterDetailChild_Receipt_ViewReceiptDetailModalComponent } from './receiptDetails/receiptDetails/masterDetailChild_Receipt_view-receiptDetail-modal.component';
import { MasterDetailChild_Receipt_CreateOrEditReceiptDetailModalComponent } from './receiptDetails/receiptDetails/masterDetailChild_Receipt_create-or-edit-receiptDetail-modal.component';
import { MasterDetailChild_Receipt_ReceiptDetailAccountBillingLookupTableModalComponent } from './receiptDetails/receiptDetails/masterDetailChild_Receipt_receiptDetail-accountBilling-lookup-table-modal.component';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { GendersComponent } from './genders/genders/genders.component';
import { ViewGenderModalComponent } from './genders/genders/view-gender-modal.component';
import { CreateOrEditGenderModalComponent } from './genders/genders/create-or-edit-gender-modal.component';
import { CitiesComponent } from './cities/cities/cities.component';
import { ViewCityModalComponent } from './cities/cities/view-city-modal.component';
import { CreateOrEditCityModalComponent } from './cities/cities/create-or-edit-city-modal.component';
import { MenuDetailsComponent } from './menuDetails/menuDetails/menuDetails.component';
import { ViewMenuDetailModalComponent } from './menuDetails/menuDetails/view-menuDetail-modal.component';
import { CreateOrEditMenuDetailModalComponent } from './menuDetails/menuDetails/create-or-edit-menuDetail-modal.component';
import { MenuDetailMenuLookupTableModalComponent } from './menuDetails/menuDetails/menuDetail-menu-lookup-table-modal.component';
import { MenusComponent } from './menus/menus/menus.component';
import { ViewMenuModalComponent } from './menus/menus/view-menu-modal.component';
import { CreateOrEditMenuModalComponent } from './menus/menus/create-or-edit-menu-modal.component';
import { OrdersComponent } from './order/order.component';
import { ViewOrderModalComponent } from './order/view-order-modal.component';
import { MenuItemStatusesComponent } from './menuItemStatuses/menuItemStatuses/menuItemStatuses.component';
import { ViewMenuItemStatusModalComponent } from './menuItemStatuses/menuItemStatuses/view-menuItemStatus-modal.component';
import { CreateOrEditMenuItemStatusModalComponent } from './menuItemStatuses/menuItemStatuses/create-or-edit-menuItemStatus-modal.component';
import { ItemsComponent } from './items/items/items.component';
import { ViewItemModalComponent } from './items/items/view-item-modal.component';
import { CreateOrEditItemModalComponent } from './items/items/create-or-edit-item-modal.component';
import { MenuCategoriesComponent } from './menuCategories/menuCategories/menuCategories.component';
import { ViewMenuCategoryModalComponent } from './menuCategories/menuCategories/view-menuCategory-modal.component';
import { CreateOrEditMenuCategoryModalComponent } from './menuCategories/menuCategories/create-or-edit-menuCategory-modal.component';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PaginatorModule } from 'primeng/paginator';
import { EditorModule } from 'primeng/editor';
import { InputMaskModule } from 'primeng/inputmask';
import { FileUploadModule } from 'primeng/fileupload';
import { MultiSelectModule } from 'primeng/multiselect';
import { TableModule } from 'primeng/table';
import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { MainRoutingModule } from './main-routing.module';
import {  NgxChartsModule } from '@swimlane/ngx-charts';
import { BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { CountdownModule } from 'ngx-countdown';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import { EmojiModule } from '@ctrl/ngx-emoji-mart/ngx-emoji';
import {
    AreasServiceProxy,
    AssetServiceProxy,
    BillingsServiceProxy,
    BookingServiceProxy,
    BranchesServiceProxy,
    CTownApiServiceProxy,
    DeliveryCostServiceProxy,
    DeliveryOrderDetailsServiceProxy,
    EvaluationsServiceProxy,
    ForcatsesServiceProxy,
    HostedCheckoutServiceProxy,
    HyperPayPaymentServiceServiceProxy,
    ItemAdditionServiceProxy,
    LiveChatServiceProxy,
    LocationServiceProxy,
    LocationsServiceProxy,
    LoyaltyServiceProxy,
    MaintenancesServiceProxy,
    MenuServiceProxy,
    MenusServiceProxy,
    MenuSystemServiceProxy,
    OrderOfferServiceProxy,
    OrdersServiceProxy,
    SellingRequestServiceProxy,
    TeamInboxServiceProxy,
    TenantServicesServiceProxy,
    UserServiceProxy,
    WhatsAppConversationSessionServiceProxy,
    WhatsAppMessageTemplateServiceProxy,
    DepartmentServiceProxy,
    ZohoServiceProxy,
    TenantServicesInfoServiceProxy,
    BotFlowServiceProxy,
    GroupServiceProxy,
    TeamsServiceProxy,
    FacebookTemplateServiceProxy
} from '@shared/service-proxies/service-proxies';
import { AssignToModalComponent } from './teamInbox/assign-to-modal/assign-to-modal.component';
import { TeamInboxService } from './teamInbox/teaminbox.service';
import { RecordRTCService } from './teamInbox/record-rtc.service';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { CrystalLightboxModule } from '@crystalui/angular-lightbox';
import { PaymentTestComponent } from './payment-test/payment-test.component';
import { DragDropDirective } from './teamInbox/drag-drop.directive';
import { BranchesComponent } from './areaLocation/branches.component';
import { CreateOrEditBranchtModalComponent } from './areaLocation/create-or-edit-branch-modal.component';
import { ViewBranchModalComponent } from './areaLocation/view-branch-modal.component';
import { BranchessComponent } from './branchess/branchess.component';
import { CreateOrEditBranchessModalComponent } from './branchess/create-or-edit-branchess-modal.component';
import { ViewBranchessModalComponent } from './branchess/view-branchess-modal.component';
import { LocationComponent } from './location/location.component'
import { CreateOrEditLocationModelComponent } from './location/create-or-edit-location';
import { EvaluationComponent } from './evaluation/evaluation.component';
import { CreateOrEditDeliveryLocationModelComponent } from './deliverylocation/create-or-edit-location';
import { DeliveryLocationComponent } from './deliverylocation/location.component';
import { CreateOrEditDeliveryOrderModalComponent } from './deliveryorder/create-or-edit-order-modal.component';
import { DeliveryOrdersComponent } from './deliveryorder/order.component';
import { ViewDeliveryOrderModalComponent } from './deliveryorder/view-order-modal.component';
import { ForcatsComponent } from './forcats/forcats.component';
import { CreateOrEditForcatsModalComponent } from './forcats/create-or-edit-forcats-modal.component';
import { ViewForcatsModalComponent } from './forcats/view-forcats-modal.component';
import { BookingComponent } from './booking/booking.component';
import { CreateOrEditBookingModalComponent } from './booking/create-or-edit-booking-modal.component';
import { ViewBookingModalComponent } from './booking/view-booking-modal.component';
import { PaymentComponent } from './payment/payment.component';
import { addOnsLookupTableModalComponent } from './menus/menus/addOns-lookup-table-modal.component';
import { specificationLookupTableModalComponent } from './menus/menus/specification-lookup-table-modal.component';
import { ViewItemSpecificationsModalComponent } from './menus/menus/view-ItemSpecifications-modal.component';
import { ctownComponent } from './CtownUpdate/ctown.component';
import { CreateOrEditctownModalComponent } from './CtownUpdate/create-or-edit-ctown-modal.component';
import { ViewctownModalComponent } from './CtownUpdate/view-ctown-modal.component';
import { MaintenanceComponent } from './Maintenance/Maintenance.component';
import { ViewMaintenanceModalComponent } from './Maintenance/view-Maintenance-modal.component';
import { OrdersArchivedComponent } from './order/orderArchived.component';
import { AgmCoreModule } from '@agm/core';
import { CreateOrEditAdvancedMenuModalComponent } from './menus/menus/create-or-edit-advanced-menu-modal.component';
import { ManageAdvancedMenuComponent } from './menus/menus/manage-advanced-menu.component';
import { CreateOrEditCategoryModalComponent } from './menus/menus/create-or-edit-category-modal.component';
import { CreateOrEditSubCategoryModalComponent } from './menus/menus/create-or-edit-subcategory-modal.component';
import { CreateOrEditCategoryItemComponent } from './menus/menus/create-or-edit-category-item-modal.component';
import { CreateOrEditMenuSettingModalComponent } from './menus/menus/create-or-edit-menu-setting-modal.component';
import { CreateOrEditBranchessSettingModalComponent } from './branchess/create-or-edit-branchess-setting-modal.component';
import { SealingRequestComponent } from './sealing-request/sealing-request.component';
import { ViewSealingRequestModalComponent } from './sealing-request/view-sealing-Request-modal.component'
import { LiveChatComponent } from './liveChat/liveChat.component';
import { AssetsComponent } from './assets/assets.component'
import { ViewAssetsComponent } from './assets/view-assets.component'
import { AddAssetComponent } from './assets/add-asset.component'
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { DeliveryCostComponent } from './delivery-cost/delivery-cost.component';
import { AddDeliveryCostComponent } from './delivery-cost/add-delivery-cost.component';
import { ChatThem12Component } from './chat-them12/chat-them12.component';
import { AddMessageTemplateComponent } from './message-template/add-message-template.component';

import { MessageTemplateComponent } from './message-template/message-template.component';
import { MessageCampaignComponent } from './message-campaign/message-campaign.component';
import { SendCampaignComponent } from './message-campaign/send-campaign.component';
import { NgxIntlTelInputModule } from 'ngx-intl-tel-input';
import { ViewMessageTemplateComponent } from './message-template/view-message-template.component';
import { TabViewModule } from 'primeng/tabview';
import { CreateOrEditAdvancedMenuPageComponent } from './menus/menus/create-or-edit-advanced-menu-page.component';
import { CreateOrEditMenuPageComponent } from './menus/menus/create-or-edit-menu-page.component';
import { CreateOrEditAddOnsComponent } from './menus/menus/create-or-edit-add-ons.component';
import { CustomPipe } from '@shared/pipes/custom.pipe';
import { CustomCountDownPipe } from '@shared/pipes/custom-count-down.pipe';
import { CreateOrEditSpecificationPageComponent } from './menus/menus/create-or-edit-specification-page.component'
import { NgbModule, NgbProgressbarModule } from '@ng-bootstrap/ng-bootstrap';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { ExternalContactsComponent } from './external-contacts/external-contact.component';
import { MessageConversationComponent } from './message-conversation/message-conversation.component';
import { AddMessageConversationComponent } from './message-conversation/add-message-conversation.component';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { ImageCropperModule } from 'ngx-image-cropper';
import { ImageCrppersComponent } from './modals/imageCroppers.component';
import { DragScrollModule } from 'ngx-drag-scroll';
import { ScheduledMessageConversationComponent } from './message-conversation/scheduled-message-conversation';
import { ViewMessageHistoryComponent } from './message-conversation/view-message-history.component';
import { ViewCampaignHistoryComponent } from './message-campaign/view-campaign-history.component';
NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();
import { CreateOrEditAddModalComponent } from './menus/menus/create-or-edit-Add-modal.component';
import { CreateOrEditSpecificationModalComponent } from './menus/menus/create-or-edit-Specification-modal.component';
import { CalendarModule } from 'primeng/calendar';
import { CalendarrModule } from './calendar/calendar/calendarr.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgxQRCodeModule } from '@techiediaries/ngx-qrcode';
import { TabMenuModule } from 'primeng/tabmenu';


import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { DepartmentsComponent } from './departments/departments.component';
import { editDepartment } from './departments/edit-department.component';
import { CalendarService } from './calendar/calendar/calendar.service';
import { ExportAsModule } from 'ngx-export-as';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SwiperConfigInterface, SwiperModule, SWIPER_CONFIG } from 'ngx-swiper-wrapper';
const DEFAULT_SWIPER_CONFIG: SwiperConfigInterface = {
    direction: 'horizontal',
    observer: true
};

import { viewResons } from './message-template/view-reasons.component'
import { QuillModule } from 'ngx-quill'
import { ViewTicketModalComponent } from './liveChat/view-ticket-modal.component';
import { ViewLoyaltyOrdersComponent } from './order/view-loyalty-orders.component';
import { CreateCampaignComponent } from './message-campaign/create-campaign.component';
import { ScheduleCampaignComponent } from './message-campaign/schedule-campaign.component';
import { Ng2FlatpickrModule } from 'ng2-flatpickr';
import { PlyrModule } from 'ngx-plyr';
import { NgxLinkifyjsModule } from 'ngx-linkifyjs';
import { VgCoreModule } from '@videogular/ngx-videogular/core';
import { VgControlsModule } from '@videogular/ngx-videogular/controls';
import { VgOverlayPlayModule } from '@videogular/ngx-videogular/overlay-play';
import { VgBufferingModule } from '@videogular/ngx-videogular/buffering';
import { VideoModelComponent } from './videoComponent/video-model.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CampaignStatisticsComponent } from './message-campaign/campaign-statistics.component';
import { ReservedWordsComponent } from './reserved-words/reserved-words.component';
import { CreateReservedWordsComponent } from './reserved-words/create-reserved-words.component';
import { MainDashboardComponent } from './main-dashboard/main-dashboard.component';
import { DashboardStatisticsComponent } from './main-dashboard/dashboard-statistics/dashboard-statistics.component';
import { DashboardChartsComponent } from './main-dashboard/dashboard-charts/dashboard-charts.component';
import { WalletTransactionsComponent } from './main-dashboard/wallet-transactions/wallet-transactions.component';
import { AddFundsComponent } from './main-dashboard/add-funds/add-funds.component';
import { LazyLoadImageModule } from 'ng-lazyload-image';
import { ViewRequestModalComponent } from './liveChat/view-request-modal/view-request-modal.component';
import { TeaminboxTemplateModalComponent } from './chat-them12/teaminbox-template-modal/teaminbox-template-modal.component';
import { NoSpecialCharecterDirective } from './message-template/noSpecialCharecter.directive';
import { SendMessageModalComponent } from './chat-them12/send-message-modal/send-message-modal.component';
import { ScrollListComponent } from './chat-them12/scroll-list/scroll-list.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { SkeleltonComponent } from './chat-them12/skelelton/skelelton.component';
import { ContactGroupComponent } from './contacts/contactGroup/contactGroup.component';
import { ContactGroupModalComponent } from './contacts/contactGroup/contactGroup-modal/contactGroup-modal.component';
import { CreateGroupPageComponent } from './contacts/contactGroup/create-group-page/create-group-page.component';
import { OldgroupComponent } from './contacts/contactGroup/oldgroup/oldgroup.component';
import { NewContactComponent } from './contacts/contactGroup/newContact/newContact.component';
import { UploadDirective } from './contacts/contactGroup/newContact/upload.directive';
import { EditGroupComponent } from './contacts/contactGroup/edit-group/edit-group.component';
import { DeleteUpdateGroupComponent } from './contacts/contactGroup/edit-group/delete-update-group/delete-update-group.component';
import { ExtenalUpdateGroupComponent } from './contacts/contactGroup/edit-group/extenal-update-group/extenal-update-group.component';
import { InternalUpdateGroupComponent } from './contacts/contactGroup/edit-group/internal-update-group/internal-update-group.component';
import { SendtoCompaignToGroupComponent } from './message-campaign/sendtoCompaignToGroup/sendtoCompaignToGroup.component';
import { DialogModule } from 'primeng/dialog';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { StepsModule } from 'primeng/steps';
import { CoreDirectivesModule } from '@core/directives/directives';
import { NumberAbbreviationPipe } from './message-campaign/numberAbbreviation.pipe';
import { ConfirmationService } from 'primeng/api';

import { OverviewDashboardComponent } from './main-dashboard/overview-dashboard/overview-dashboard.component';
import { CardComponent } from './main-dashboard/dashborad-component/card/card.component';
import { AbbreviateNumberPipe } from '@shared/common/pipes/AbbreviateNumberPipe.pipe';
import { PaymentTableComponent } from './main-dashboard/dashborad-component/payment-table/payment-table.component';
import { PaymentDetialsComponent } from './main-dashboard/dashborad-component/payment-detials/payment-detials.component';
import { PaymentDetialComponent } from './main-dashboard/dashborad-component/payment-detials/payment-detial/payment-detial.component';
import { ExpenseRatioChartComponent } from './main-dashboard/dashborad-component/expenseRatioChart/expenseRatioChart.component';
import { StatisticsChartComponent } from './main-dashboard/dashborad-component/statisticsChart/statisticsChart.component';
import { CompaignProgressComponent } from './main-dashboard/dashborad-component/compaignProgress/compaignProgress.component';
import { ProgressparCompComponent } from './main-dashboard/dashborad-component/progresspar-comp/progresspar-comp.component';
import { ProgressOrderComponent } from './main-dashboard/dashborad-component/progress-order/progress-order.component';
import { CustomeProgressParComponent } from './message-campaign/custome-progress-par/custome-progress-par.component';
import { CustomCurrencyPipePipe } from './main-dashboard/overview-dashboard/CustomCurrencyPipe.pipe';
import { UserPeroformanceTableTicketComponent } from './main-dashboard/dashborad-component/user-peroformance-table-ticket/user-peroformance-table-ticket.component';
import { UserPerformanceTableAppointmentComponent } from './main-dashboard/dashborad-component/user-performance-table-appointment/user-performance-table-appointment.component';
import { UserPerformanceTableOrderComponent } from './main-dashboard/dashborad-component/user-performance-table-order/user-performance-table-order.component';
import { numberinput  } from './number-input/number-input.component';
import { SummaryViewComponent } from './liveChat/summary-view/summary-view.component';
import { ListSettingsModalComponent } from './liveChat/list-settings-modal/list-settings-modal.component';
import { MoveableHeaderComponent } from './liveChat/moveable-header/moveable-header.component';
import { MoveableRowComponent } from './liveChat/moveable-row/moveable-row.component';
import { AssignUsersModalComponent } from './liveChat/assign-users-modal/assign-users-modal.component';
import { AssignTicketModalComponent } from './chat-them12/assign-ticket-modal/assign-ticket-modal.component';
import { BackupAllConversationModalComponent } from './contacts/backup-all-conversation-modal/backup-all-conversation-modal.component';
import { BarChartComponent } from './main-dashboard/bar-chart/bar-chart.component';
import { SendMessageModalFromTeamplatComponent } from './message-template/send-message-modal-from-Template/send-message-modal-from-Template.component';
import { FacebookTemplateComponent } from './message-template/Facebook-Template/Facebook-Template.component';
import { EditTemplateFacebookComponent } from './message-template/Edit-Template-Facebook/Edit-Template-Facebook.component';
import { TemplateNameAndLanguageComponent } from './message-template/Edit-Template-Facebook/TemplateNameAndLanguage/TemplateNameAndLanguage.component';
import { ContentTemplateComponent } from './message-template/Edit-Template-Facebook/ContentTemplate/ContentTemplate.component';
import { CatalogFormatComponent } from './message-template/Edit-Template-Facebook/CatalogFormat/CatalogFormat.component';
import { ButtonsTemplateComponent } from './message-template/Edit-Template-Facebook/ButtonsTemplate/ButtonsTemplate.component';
import { MessageValidityPeriodTemplateComponent } from './message-template/Edit-Template-Facebook/MessageValidityPeriodTemplate/MessageValidityPeriodTemplate.component';
import { AuthenticationCodeDeliverySetupComponent } from './message-template/Edit-Template-Facebook/AuthenticationCodeDeliverySetup/AuthenticationCodeDeliverySetup.component';
import { CatalogSetupTemplateComponent } from './message-template/Edit-Template-Facebook/CatalogSetupTemplate/CatalogSetupTemplate.component';
import { ButtonInCatalogComponent } from './message-template/Edit-Template-Facebook/ButtonInCatalog/ButtonInCatalog.component';
import { CardTemplateComponent } from './message-template/Edit-Template-Facebook/CardTemplate/CardTemplate.component';
import { RouteReuseStrategy } from '@node_modules/@angular/router';
import { CustomReuseStrategy } from './chat-them12/teaminbox-template-modal/custom-reuse-strategy';
import { TicketLimitPopupComponent } from './liveChat/TicketLimitPopup/ticket-limit-popup.component';
import { ExportToExcelModelComponent } from './liveChat/Export-To-Excel-Model/Export-To-Excel-Model.component';
import { DropdownModule } from 'primeng/dropdown';

// import { CoreConfigService } from '@core/services/config.service';
// import { CoreConfigService, CORE_CUSTOM_CONFIG } from '@core/services/config.service';

@NgModule({
    imports: [
        NgMultiSelectDropDownModule.forRoot(),
        FormsModule,
                //GoogleMapsModule,
        AgmCoreModule.forRoot({
            apiKey: 'AIzaSyBwqetOtPfVauA4f8jniGzmRDYzQ_G5CfQ',
            libraries: ['places']
        }),
        DropdownModule,
        ExportAsModule,
        NgbModule,
        NgSelectModule, 
        NgApexchartsModule,
        StepsModule,
        CarouselModule,
        DragScrollModule,
        DragDropModule,
        CalendarrModule,
        MultiSelectModule,
        ImageCropperModule,
        FileUploadModule,
        AutoCompleteModule,
        PaginatorModule,
        EditorModule,
        CalendarModule,
        InputMaskModule,
        TableModule,
        CommonModule,
        DialogModule,
        ReactiveFormsModule,
        ModalModule,
        TabsModule,
        TooltipModule,
        AppCommonModule,
        UtilsModule,
        ProgressSpinnerModule, 
        MainRoutingModule,
        CountoModule,
        NgxChartsModule,
        InfiniteScrollModule,
        BsDatepickerModule.forRoot(),
        BsDropdownModule.forRoot(),
        PopoverModule.forRoot(),
        QuillModule.forRoot(),
        NgxLinkifyjsModule.forRoot(),
        CountdownModule,
        PickerModule,
        EmojiModule,
        NgxDropzoneModule,
        CrystalLightboxModule,
        NgxIntlTelInputModule,
        NgxQRCodeModule,
        SwiperModule,
        Ng2FlatpickrModule,
        VgCoreModule,
        ConfirmDialogModule,
        VgControlsModule,
        VgOverlayPlayModule,
        VgBufferingModule,
        TabMenuModule,
        PlyrModule,
        LazyLoadImageModule,
        NgxSkeletonLoaderModule,
        TabViewModule,
        CoreDirectivesModule,
        NgbProgressbarModule
    ],
    declarations: [
        CardTemplateComponent,
        ContentTemplateComponent,
        TemplateNameAndLanguageComponent,
        ButtonInCatalogComponent,
        CatalogSetupTemplateComponent,
        AuthenticationCodeDeliverySetupComponent,
        MessageValidityPeriodTemplateComponent,
        ButtonsTemplateComponent,
        CatalogFormatComponent,
        ScrollListComponent,
        CustomCurrencyPipePipe,
        CardComponent,
        OverviewDashboardComponent,
        BillingsComponent,
        PaymentTableComponent,
        CustomeProgressParComponent,
        UserPeroformanceTableTicketComponent,
        UserPerformanceTableOrderComponent,
        UserPerformanceTableAppointmentComponent,
        PaymentDetialsComponent,
        PaymentDetialComponent,
        CreateGroupPageComponent,
        ExtenalUpdateGroupComponent,
        OldgroupComponent,
        InternalUpdateGroupComponent,
        NewContactComponent,
        ContactGroupModalComponent,
        SendtoCompaignToGroupComponent,
        ContactGroupComponent,
        SkeleltonComponent,
        SendMessageModalComponent,
        NumberAbbreviationPipe,
        CustomPipe,
        ProgressOrderComponent,
        CustomCountDownPipe,
        ViewBillingModalComponent,
        CreateOrEditBillingModalComponent,
        BillingCurrencyLookupTableModalComponent,
        ContactsComponent,
        ProgressparCompComponent,
        CreateOrEditAddModalComponent,
        ExternalContactsComponent,
        DeleteUpdateGroupComponent,
        ImageCrppersComponent,
        ViewContactModalComponent,
        CreateOrEditContactModalComponent,
        CreateOrEditSpecificationModalComponent,
        ContactChatStatuseLookupTableModalComponent,
        ContactContactStatuseLookupTableModalComponent,
        MasterDetailChild_Receipt_ReceiptDetailsComponent,
        MasterDetailChild_Receipt_ViewReceiptDetailModalComponent,
        MasterDetailChild_Receipt_CreateOrEditReceiptDetailModalComponent,
        MasterDetailChild_Receipt_ReceiptDetailAccountBillingLookupTableModalComponent,
        GendersComponent,
        ExpenseRatioChartComponent,
        StatisticsChartComponent,
        ChatThem12Component,
        NoSpecialCharecterDirective,
        AbbreviateNumberPipe,
        ViewGenderModalComponent,
        CreateOrEditGenderModalComponent,
        CitiesComponent,
        ViewCityModalComponent,
        CreateOrEditCityModalComponent,
        MenuDetailsComponent,
        ViewMenuDetailModalComponent,
        CreateOrEditMenuDetailModalComponent,
        MenuDetailMenuLookupTableModalComponent,
        MenusComponent,
        OrdersComponent,
        DepartmentsComponent,
        OrdersArchivedComponent,
        ViewOrderModalComponent,
        DeliveryOrdersComponent,
        ViewDeliveryOrderModalComponent,
        CreateOrEditDeliveryOrderModalComponent,
        BranchesComponent,
        CreateOrEditBranchtModalComponent,
        ViewBranchModalComponent, 
        BranchessComponent,
        CreateOrEditBranchessSettingModalComponent,
        CreateOrEditBranchessModalComponent,
        ViewBranchessModalComponent,
        EvaluationComponent,
        ViewMenuModalComponent,
        CreateOrEditMenuModalComponent,
        CreateOrEditAdvancedMenuModalComponent,
        ManageAdvancedMenuComponent,
        CreateOrEditCategoryModalComponent,
        MenuItemStatusesComponent,
        specificationLookupTableModalComponent,
        ViewItemSpecificationsModalComponent,
        CreateOrEditMenuSettingModalComponent,
        ViewMenuItemStatusModalComponent,
        CreateOrEditMenuItemStatusModalComponent,
        ItemsComponent,
        ViewItemModalComponent,
        CreateOrEditItemModalComponent,
        MenuCategoriesComponent,
        ViewMenuCategoryModalComponent,
        CreateOrEditMenuCategoryModalComponent,
        CreateOrEditSubCategoryModalComponent,
        CreateOrEditCategoryItemComponent,
        AssignToModalComponent,
        PaymentTestComponent,
        DragDropDirective,
        LocationComponent,
        CreateOrEditLocationModelComponent,
        DeliveryLocationComponent,
        CreateOrEditDeliveryLocationModelComponent,
        ForcatsComponent,
        CreateOrEditForcatsModalComponent,
        ViewForcatsModalComponent,
        BookingComponent,
        CreateOrEditBookingModalComponent,
        ViewBookingModalComponent,
        PaymentComponent,
        addOnsLookupTableModalComponent,
        ctownComponent,
        CreateOrEditctownModalComponent,
        ViewctownModalComponent,
        MaintenanceComponent,
        ViewMaintenanceModalComponent,
        SealingRequestComponent,
        ViewSealingRequestModalComponent,
        ViewRequestModalComponent,
        LiveChatComponent,
        AssetsComponent,
        ViewAssetsComponent,
        AddAssetComponent,
        DeliveryCostComponent,
        AddDeliveryCostComponent,
        MessageTemplateComponent,
        AddMessageTemplateComponent,
        MessageCampaignComponent,
        MessageConversationComponent,
        SendCampaignComponent,
        AddMessageConversationComponent,
        ScheduledMessageConversationComponent,
        ViewMessageTemplateComponent,
        CreateOrEditAdvancedMenuPageComponent,
        CreateOrEditMenuPageComponent,
        CreateOrEditAddOnsComponent,
        CreateOrEditSpecificationPageComponent,
        ViewMessageHistoryComponent,
        ViewCampaignHistoryComponent,
        PageNotFoundComponent,
        editDepartment,
        ViewTicketModalComponent,
        UploadDirective,
        viewResons,
        ViewLoyaltyOrdersComponent,
        CreateCampaignComponent,
        ScheduleCampaignComponent,
        VideoModelComponent,
        CampaignStatisticsComponent,
        ReservedWordsComponent,
        CreateReservedWordsComponent,
        MainDashboardComponent,
        DashboardStatisticsComponent,
        DashboardChartsComponent,
        WalletTransactionsComponent,
        AddFundsComponent,
        TeaminboxTemplateModalComponent,
        EditGroupComponent,
        numberinput,
        SummaryViewComponent,
        TicketLimitPopupComponent,
        ListSettingsModalComponent,
        MoveableHeaderComponent,
        MoveableRowComponent,
        AssignUsersModalComponent,
        AssignTicketModalComponent,
        BackupAllConversationModalComponent,
        BarChartComponent,
        SendMessageModalFromTeamplatComponent,
        FacebookTemplateComponent,
        EditTemplateFacebookComponent,
        ExportToExcelModelComponent
    ],
    providers: [
        { provide: RouteReuseStrategy, useClass: CustomReuseStrategy },
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
        {
            provide: SWIPER_CONFIG,
            useValue: DEFAULT_SWIPER_CONFIG
        },
        { provide: LOCALE_ID, useValue: 'en-US' },
        TeamInboxServiceProxy,
        TeamInboxService,
        ConfirmationService,
        RecordRTCService,
        GroupServiceProxy,
        OrdersServiceProxy,
        BotFlowServiceProxy,
        BranchesServiceProxy,
        AreasServiceProxy,
        BillingsServiceProxy,
        HyperPayPaymentServiceServiceProxy,
        TenantServicesServiceProxy,
        UserServiceProxy,
        EvaluationsServiceProxy,
        LocationsServiceProxy,
        DeliveryOrderDetailsServiceProxy,
        OrderOfferServiceProxy,
        ForcatsesServiceProxy,
        BookingServiceProxy,
        HostedCheckoutServiceProxy,
        ItemAdditionServiceProxy,
        CTownApiServiceProxy,
        MaintenancesServiceProxy,
        MenuServiceProxy,
        MenusServiceProxy,
        LiveChatServiceProxy,
        DepartmentServiceProxy,
        SellingRequestServiceProxy,
        AssetServiceProxy,
        DeliveryCostServiceProxy,
        WhatsAppMessageTemplateServiceProxy,
        WhatsAppConversationSessionServiceProxy,
        DatePipe,
        LoyaltyServiceProxy,
        LocationServiceProxy,
        CalendarService,
        ZohoServiceProxy,
        TenantServicesInfoServiceProxy,
        FacebookTemplateServiceProxy,
        TeamsServiceProxy
    ],
    bootstrap: [
        CreateOrEditBranchessModalComponent
    ],
    schemas: [
        CUSTOM_ELEMENTS_SCHEMA,
        NO_ERRORS_SCHEMA
    ]
})
export class MainModule {
}