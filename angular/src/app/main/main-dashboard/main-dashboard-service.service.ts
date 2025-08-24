import { Injectable, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    BestSellingModel,
    BookingStatisticsModel,
    BundleModel,
    CampaignStatisticsModel,
    ContactStatisticsModel,
    DashbardModel,
    LiveChatServiceProxy,
    OrderStatisticsModel,
    TenantDashboardServiceProxy,
    TicketsStatisticsModel,
    UserPerformanceBookingGenarecModel,
    UserPerformanceOrderGenarecModel,
    UserPerformanceTicketGenarecModel,
    WalletModel,
} from "@shared/service-proxies/service-proxies";
import moment from "moment";

@Injectable({
    providedIn: "root",
})
export class MainDashboardServiceService extends AppComponentBase {
    bundleData = new BundleModel();
    //performance
    ticketPerformence = new UserPerformanceTicketGenarecModel();
    bookingPerformence = new UserPerformanceBookingGenarecModel();
    orderPerformence = new UserPerformanceOrderGenarecModel();

    //campaign
    campaignData = new CampaignStatisticsModel();
    campaignId = undefined;
    from = new Date();
    to = new Date();

    //Contact
    contactData = new ContactStatisticsModel();

    //tickets
    ticketsData = new TicketsStatisticsModel();

    //orders
    ordersData = new OrderStatisticsModel();
    branchId = undefined;
    compeletedOrderPercentageString = "0";
    //Best Selling Items
    bestSellingItems: BestSellingModel[] = [];

    //appoitments
    selectedUser = undefined;
    selectedUserForBarChart = undefined;
    appoitmentsData = new BookingStatisticsModel();
    bookedAppointmentsPercentage = "0";

    //walletModel
    walletModel = new WalletModel();

    constructor(
        injector: Injector,
        private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
    ) {
        super(injector);
    }

    getDashboardInfo(from, to) {
        this._tenantDashboardServiceProxy
            .getDashoardInfo(from, to, this.appSession.tenantId)
            .subscribe((response: DashbardModel) => {
                this.bundleData = response.bundleModel;
                this.ticketPerformence = response.userPerformanceTicketModel;
                this.orderPerformence = response.userPerformanceOrderModel;
                this.bookingPerformence = response.userPerformanceBookingModel;
                this.walletModel = response.walletModel;
            });
    }

    getDashboardCampaignInfo(from?, to?) {
        console.log(this.campaignId);
        if (!this.campaignId || this.campaignId == "undefined") {
            this.campaignId = undefined;
            if (from === undefined || from === null || from === "") {
            } else {
                this.from = from;
                this.to = to;
            }

            this._tenantDashboardServiceProxy
                .campaignStatisticsGet(
                    moment(this.from),
                    moment(this.to),
                    this.appSession.tenantId,
                    this.campaignId
                )
                .subscribe((response: CampaignStatisticsModel) => {
                    this.campaignData = response;
                });
        } else {
            this._tenantDashboardServiceProxy
                .campaignStatisticsGet(
                    moment(new Date(2020, 1, 1)),
                    moment(new Date()).endOf("day"),
                    this.appSession.tenantId,
                    this.campaignId
                )
                .subscribe((response: CampaignStatisticsModel) => {
                    this.campaignData = response;
                });
        }
    }

    getDashboardContactInfo(from?, to?) {
        this._tenantDashboardServiceProxy
            .contactStatisticsGet(from, to, this.appSession.tenantId)
            .subscribe((response: ContactStatisticsModel) => {
                this.contactData = response;
            });
    }

    getDashboardTicketsInfo(from?, to?) {
        this._tenantDashboardServiceProxy
            .ticketsStatisticsGet(from, to, this.appSession.tenantId)
            .subscribe((response: TicketsStatisticsModel) => {
                this.ticketsData = response;
            });
    }

    getDashboardOrdersInfo(from?, to?) {
        if (from === undefined || from === null || from === "") {
        } else {
            this.from = from;
            this.to = to;
        }
        if (this.branchId == "undefined") {
            this.branchId = undefined;
        }
        this._tenantDashboardServiceProxy
            .ordersStatisticsGet(
                moment(this.from),
                moment(this.to),
                this.appSession.tenantId,
                this.branchId
            )
            .subscribe((response: OrderStatisticsModel) => {
                this.ordersData = response;
                this.compeletedOrderPercentageString =
                    this.ordersData.percentageCompleted.toString();
            });
    }

    getBestSellingItems(from?, to?) {
        if (from === undefined || from === null || from === "") {
        } else {
            this.from = from;
            this.to = to;
        }
        this._tenantDashboardServiceProxy
            .getBestSellingItems(
                moment(this.from),
                moment(this.to),
                this.appSession.tenantId
            )
            .subscribe((response: BestSellingModel[]) => {
                this.bestSellingItems = response;
            });
    }

    getDashboardAppoitmentsInfo(from?, to?) {
        if (from === undefined || from === null || from === "") {
        } else {
            this.from = from;
            this.to = to;
        }
        if (this.selectedUser == "undefined") {
            this.selectedUser = undefined;
        }
        this._tenantDashboardServiceProxy
            .bookingStatisticsGet(
                moment(this.from),
                moment(this.to),
                this.appSession.tenantId,
                this.selectedUser
            )
            .subscribe((response: BookingStatisticsModel) => {
                this.appoitmentsData = response;
                this.bookedAppointmentsPercentage =
                    this.appoitmentsData.percentageBooked.toString();
            });
    }
}
