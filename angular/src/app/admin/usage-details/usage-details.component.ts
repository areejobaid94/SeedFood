import {
    Component,
    Injector,
    ViewEncapsulation,
    ViewChild,
} from "@angular/core";
import {
    CampaignDashModel,
    FileDto,
    TenantDashboardServiceProxy,
    UsageDetailsModel,
    UsageStatisticsModel,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import * as _ from "lodash";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import moment from "moment";
import { FileDownloadService } from "@shared/utils/file-download.service";

@Component({
    selector: "app-usage-details",
    templateUrl: "./usage-details.component.html",
    styleUrls: ["./usage-details.component.scss"],
})
export class UsageDetailsComponent extends AppComponentBase {
    theme: string;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    groupedBy = "";
    campaignId = 0;
    allCampaigns: CampaignDashModel[] = [];
    usageDetails: UsageStatisticsModel = new UsageStatisticsModel();
    dateRange: [Date, Date];
    predefinedRanges = [
        {
            value: [
                this.setToStartOfDay(new Date()), 
                this.setToEndOfDay(new Date()),  
            ],
            label: this.l("Today"),
        },
        {
            value: [
                this.setToStartOfDay(
                    new Date(new Date().setDate(new Date().getDate() - 1)) 
                ),
                this.setToEndOfDay(
                    new Date(new Date().setDate(new Date().getDate() - 1))
                ),
            ],
            label: this.l("Yesterday"),
        },
        {
            value: [
                this.setToStartOfDay(
                    new Date(new Date().setDate(new Date().getDate() - 7))
                ),
                this.setToEndOfDay(new Date()),
            ],
            label: this.l("Last7Days"),
        },
        {
            value: [
                this.setToStartOfDay(
                    new Date(new Date().setMonth(new Date().getMonth() - 1))
                ),
                this.setToEndOfDay(new Date()),
            ],
            label: this.l("LastMonth"),
        },
        {
            value: [
                this.setToStartOfDay(
                    new Date(
                        new Date().setFullYear(new Date().getFullYear() - 1)
                    )
                ),
                this.setToEndOfDay(new Date()),
            ],
            label: this.l("LastYear"),
        },
    ];
    dateRangePickerOptions = {
        showCustomRangeLabel: false,
        dateInputFormat: "DD/MM/YYYY",
        rangeInputFormat: "DD/MM/YYYY",
        isAnimated: true,
        adaptivePosition: true,
        containerClass: "theme-default",
        selectFromOtherMonth: true,
        ranges: this.predefinedRanges,
        showPreviousMonth: true,
        showWeekNumbers: false,
        useUtc: true,
    };

    constructor(
        public darkModeService: DarkModeService,
        injector: Injector,
        private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.getAllCampaigns();
    }
    clearDateRange() {
        this.dateRange = null;
        this.getUsageDetails();
    }
    onDateRangeUpdate(event?) {
        if (event[0] == "Invalid Date" && event[1] == "Invalid Date") {
            this.clearDateRange();
        } else if (event !== undefined) {
            this.dateRange = event;
            this.dateRange[0] = this.setToStartOfDay(this.dateRange[0]);
            this.dateRange[1] = this.setToEndOfDay(this.dateRange[1]);
            this.ensureValidRange();
            this.getUsageDetails();
        }
    }

    getUsageDetails(event?: LazyLoadEvent) {
        this.usageDetails = new UsageStatisticsModel();
        let startDate;
        let endDate;
        if (this.dateRange === undefined || this.dateRange === null) {
            startDate = undefined;
            endDate = undefined;
        } else {
            startDate = moment(this.dateRange[0]);
            endDate = moment(this.dateRange[1]);
        }
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();
        this._tenantDashboardServiceProxy
            .getUsageDetails(
                this.appSession.tenantId,
                this.campaignId,
                this.groupedBy,
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(
                    this.paginator,
                    event
                ),
                startDate,
                endDate
            )
            .subscribe((result) => {
                this.primengTableHelper.hideLoadingIndicator();
                this.primengTableHelper.totalRecordsCount = result.total;
                this.primengTableHelper.records = result.usageDetails;
            });
    }

    getAllCampaigns() {
        this._tenantDashboardServiceProxy
            .getAllCampaign(this.appSession.tenantId)
            .subscribe((response: CampaignDashModel[]) => {
                this.allCampaigns = response;
            });
    }

    exportToExcel() {
        let startDate;
        let endDate;
        if (this.dateRange === undefined) {
            startDate = undefined;
            endDate = undefined;
        } else {
            startDate = moment(this.dateRange[0]);
            endDate = moment(this.dateRange[1]);
        }
        this._tenantDashboardServiceProxy
            .getUsageDetailsToExcel(
                this.appSession.tenantId,
                this.campaignId,
                this.groupedBy,
                startDate,
                endDate
            )
            .subscribe((response: FileDto) => {
                this._fileDownloadService.downloadTempFile(response);
            });
    }

    clickOnMArketingOrUtility(record: UsageDetailsModel) {
        this.usageDetails = new UsageStatisticsModel();
        if (
            record.categoryType === "Utility Conversations" ||
            record.categoryType === "Marketing Conversations"
        ) {
            this._tenantDashboardServiceProxy
                .getUsageStatistics(this.appSession.tenantId, record.campaignId)
                .subscribe((response: UsageStatisticsModel) => {
                    this.usageDetails = response;
                });
        }
    }

    ensureValidRange() {
        if (this.dateRange[0] > this.dateRange[1]) {
            [this.dateRange[0], this.dateRange[1]] = [
                this.dateRange[1],
                this.dateRange[0],
            ];
        }
    }
}
