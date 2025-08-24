import {
    Component,
    ElementRef,
    HostListener,
    Injector,
    OnInit,
    ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import moment from "moment";
import { MainDashboardServiceService } from "./main-dashboard-service.service";
import { get } from "lodash";
import { CoreConfigService } from "@core/services/config.service";
import { PermissionCheckerService } from "abp-ng2-module";
import { LiveChatServiceProxy } from "@shared/service-proxies/service-proxies";

@Component({
    selector: "app-main-dashboard",
    templateUrl: "./main-dashboard.component.html",
    styleUrls: ["./main-dashboard.component.scss"],
})
export class MainDashboardComponent extends AppComponentBase implements OnInit {
    isHasPermissionLiveChat = false;
    isHasPermissionOrder = false;
    isHasPermissionBooking = false;
    isHasPermissionRequest = false;
    isHasPermissionContacts = false;
    isHasPermissionCampiagn = false;

    lastMonth1: string;
    lastMonth2: string;
    today = new Date();
    yesterday = new Date();
    lastWeek = new Date();
    thisMonth = new Date();
    lastMonth = new Date();
    dates = [];
    seletctedDate = new Date();

    tableColumns: any[] = ["Type of Chat", "Total Chat", "Fees"];

    @ViewChild("scroallableDates") scroallableDates: ElementRef;

    private isDown = false;
    private startX;
    private scrollLeft;
    // private signal$ :

    constructor(
        injector: Injector,
        public dahsboardService: MainDashboardServiceService,
        private _permissionCheckerService: PermissionCheckerService,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.getDatesToStartOfYear();
        this.getToday();
        this.isHasPermissionOrder =
            this._permissionCheckerService.isGranted("Pages.Orders");
        this.isHasPermissionLiveChat =
            this._permissionCheckerService.isGranted("Pages.LiveChat");
        this.isHasPermissionRequest = this._permissionCheckerService.isGranted(
            "Pages.SellingRequests"
        );
        this.isHasPermissionCampiagn = this._permissionCheckerService.isGranted(
            "Pages.MessageCampaign"
        );
        this.isHasPermissionContacts =
            this._permissionCheckerService.isGranted("Pages.Contacts");
        this.isHasPermissionBooking =
            this._permissionCheckerService.isGranted("Pages.Booking");
    }
    @HostListener("mousedown", ["$event"])
    onMouseDown(event: MouseEvent) {
        const element = this.scroallableDates.nativeElement;
        this.isDown = true;
        this.startX = event.pageX - element.offsetLeft;
        this.scrollLeft = element.scrollLeft;
    }

    @HostListener("mousemove", ["$event"])
    onMouseMove(event: MouseEvent) {
        if (!this.isDown) return;
        event.preventDefault();
        const element = this.scroallableDates.nativeElement;
        const x = event.pageX - element.offsetLeft;
        const walk = (x - this.startX) * 3; // Scroll-fastness factor (adjust as necessary)
        element.scrollLeft = this.scrollLeft - walk;
    }

    @HostListener("mouseup")
    @HostListener("mouseleave")
    onEndDrag() {
        this.isDown = false;
    }
    getToday() {
        let to = new Date(moment(this.today).endOf("day").toISOString());
        this.seletctedDate = this.today;
        this.dahsboardService.getDashboardInfo(this.today, to);
        this.dahsboardService.getDashboardCampaignInfo(this.today, );
        this.dahsboardService.getDashboardContactInfo(this.today, to);
        this.dahsboardService.getDashboardTicketsInfo(this.today, to);
        this.dahsboardService.getDashboardOrdersInfo(this.today, to);
        this.dahsboardService.getBestSellingItems(this.today, to);
        this.dahsboardService.getDashboardAppoitmentsInfo(this.today, to);
    }

    getYesterday() {
        let to = new Date(moment().subtract(1, "days").toISOString());
        to.setHours(23, 59, 59, 999);
        this.seletctedDate = this.yesterday;
        this.dahsboardService.getDashboardInfo(this.yesterday, to);
        this.dahsboardService.getDashboardCampaignInfo(this.yesterday, to);
        this.dahsboardService.getDashboardContactInfo(this.yesterday, to);
        this.dahsboardService.getDashboardTicketsInfo(this.yesterday, to);
        this.dahsboardService.getDashboardOrdersInfo(this.yesterday, to);
        this.dahsboardService.getBestSellingItems(this.yesterday, to);
        this.dahsboardService.getDashboardAppoitmentsInfo(this.yesterday, to);
    }

    getLastWeek() {
        let to = new Date(moment().toISOString());
        this.seletctedDate = this.lastWeek;
        this.dahsboardService.getDashboardInfo(this.lastWeek, to);
        this.dahsboardService.getDashboardCampaignInfo(this.lastWeek, to);
        this.dahsboardService.getDashboardContactInfo(this.lastWeek, to);
        this.dahsboardService.getDashboardTicketsInfo(this.lastWeek, to);
        this.dahsboardService.getDashboardOrdersInfo(this.lastWeek, to);
        this.dahsboardService.getBestSellingItems(this.lastWeek, to);
        this.dahsboardService.getDashboardAppoitmentsInfo(this.lastWeek, to);
    }
    getThisMonth() {
        let to = new Date(moment().toISOString());
        this.seletctedDate = this.thisMonth;
        this.dahsboardService.getDashboardInfo(this.thisMonth, to);
        this.dahsboardService.getDashboardCampaignInfo(this.thisMonth, to);
        this.dahsboardService.getDashboardContactInfo(this.thisMonth, to);
        this.dahsboardService.getDashboardTicketsInfo(this.thisMonth, to);
        this.dahsboardService.getDashboardOrdersInfo(this.thisMonth, to);
        this.dahsboardService.getBestSellingItems(this.thisMonth, to);
        this.dahsboardService.getDashboardAppoitmentsInfo(this.thisMonth, to);
    }

    getLastMonth() {
        let to = new Date(moment(this.lastMonth).endOf("month").toISOString());
        this.seletctedDate = this.lastMonth;
        this.dahsboardService.getDashboardInfo(this.lastMonth, to);
        this.dahsboardService.getDashboardCampaignInfo(this.lastMonth, to);
        this.dahsboardService.getDashboardContactInfo(this.lastMonth, to);
        this.dahsboardService.getDashboardTicketsInfo(this.lastMonth, to);
        this.dahsboardService.getDashboardOrdersInfo(this.lastMonth, to);
        this.dahsboardService.getBestSellingItems(this.lastMonth, to);
        this.dahsboardService.getDashboardAppoitmentsInfo(this.lastMonth, to);
    }

    getPreviousMonth(month) {
        let to = new Date(moment(month).endOf("month").toISOString());
        this.seletctedDate = month;
        this.dahsboardService.getDashboardInfo(month, to);
        this.dahsboardService.getDashboardCampaignInfo(month, to);
        this.dahsboardService.getDashboardContactInfo(month, to);
        this.dahsboardService.getDashboardTicketsInfo(month, to);
        this.dahsboardService.getDashboardOrdersInfo(month, to);
        this.dahsboardService.getBestSellingItems(month, to);
        this.dahsboardService.getDashboardAppoitmentsInfo(month, to);
    }

    getDatesToStartOfYear() {
        this.today = new Date(moment().startOf('day').toISOString());
        //get yesterday from 12:00 AM
        this.yesterday = new Date(moment().subtract(1, "days").toISOString());
        this.yesterday.setHours(0, 0, 0, 0);
        this.lastWeek = new Date(moment().subtract(7, "days").toISOString());
        this.lastWeek.setHours(0, 0, 0, 0);
        this.lastMonth = new Date(
            moment().subtract(1, "months").startOf("month").toISOString()
        );
        this.lastMonth.setHours(0, 0, 0, 0);
        this.thisMonth = new Date(moment().startOf("month").toISOString());
        this.lastMonth.setHours(0, 0, 0, 0);

        const dates = [];

        for (let i = 2; i <= moment().month(); i++) {
            dates.push(
                new Date(
                    moment()
                        .subtract(i, "months")
                        .startOf("month")
                        .toISOString()
                )
            );
        }
        this.dates = dates;
    }

    getMonthName(date: Date): string {
        const monthNames = [
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December",
        ];
        return monthNames[date.getMonth()];
    }

    tableDataFree: any[] = [
        {
            id: 1,
            typeofChat: "Instagram, WhatsApp. ",
            totalChat: 1700,
            fees: "Unlimited Entry",
        },
        {
            id: 2,
            typeofChat: "Social media",
            totalChat: 988 / 1000,
            fees: "limited Entry",
        },
    ];
    tableDataPaid: any[] = [
        {
            id: 1,
            typeofChat: "Markting",
            totalChat: 1700,
            fees: "1000$",
        },
        {
            id: 2,
            typeofChat: "Services",
            totalChat: 100,
            fees: "1000$",
        },
        {
            id: 3,
            typeofChat: "Utility",
            totalChat: 600,
            fees: "1000$",
        },
    ];
}
