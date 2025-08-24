import {
    Component,
    Injector,
    ViewEncapsulation,
    ViewChild,
} from "@angular/core";
import {
    SellingRequestDto,
    SellingRequestServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import * as _ from "lodash";
import * as moment from "moment";
import { AppSessionService } from "@shared/common/session/app-session.service";
import { Subscription } from "rxjs";
//import { sealingRequestSignalRService } from './sealing-request-signalR.service';
import { SocketioService } from "@app/shared/socketio/socketioservice";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import * as rtlDetect from "rtl-detect";
import "moment-hijri"; // Hijri extension for moment.js
import { ViewSealingRequestModalComponent } from "./view-sealing-Request-modal.component";
const { toGregorian } = require("hijri-converter");
@Component({
    templateUrl: "./sealing-request.component.html",
    encapsulation: ViewEncapsulation.None,
})
export class SealingRequestComponent extends AppComponentBase {
    theme: string;

    @ViewChild("viewSellingRequestModalComponent", { static: true })
    viewSellingRequestModal: ViewSealingRequestModalComponent;

    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = "";
    nameFilter = "";
    nameFilter2 = "";
    orderNameFilter = "";
    orderDescriptionFilter = "";
    maxEffectiveTimeFromFilter: moment.Moment;
    minEffectiveTimeFromFilter: moment.Moment;
    maxEffectiveTimeToFilter: moment.Moment;
    minEffectiveTimeToFilter: moment.Moment;
    maxTaxFilter: number;
    maxTaxFilterEmpty: number;
    minTaxFilter: number;
    minTaxFilterEmpty: number;
    imageUriFilter = "";

    appSession: AppSessionService;
    items: any;
    agentOrderSub: Subscription;
    botOrderSub: Subscription;
    change: any;
    differ: any[];
    pharmaCureFlag = false;
    isArabic = false;

    constructor(
        injector: Injector,
        //private sealingRequestSignalR: sealingRequestSignalRService,
        private socketioService: SocketioService,

        private _sellingRequestServiceProxy: SellingRequestServiceProxy,
        public darkModeService: DarkModeService
    ) {
        super(injector);
    }

    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        //this.getSealingRequests();
        //this.subscribeAgentSellingRequest();
        this.subscribeBotsellingRequest();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        if (this.appSession.tenantId == 50) {
            this.pharmaCureFlag = true;
        } else {
            this.pharmaCureFlag = false;
        }
        await this.getIsAdmin()

    }

    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split("T");
        const [year, month, day] = hijriDateString.split("-").map(Number);
        const gregorianDate = toGregorian(year, month, day);

        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, "0"),
            String(gregorianDate.gd).padStart(2, "0"),
        ].join("-");

        return formattedDate + "T" + time;
    }

    getSellingRequests(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this._sellingRequestServiceProxy
            .getSellingRequest(
                null,
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .subscribe((result) => {
                if (this.isArabic) {
                    result.lstSellingRequestDto.forEach((element) => {
                        element.createdOn = moment(
                            this.convertHijriToGregorian(
                                moment(element.createdOn)
                                    .locale("en")
                                    .format("YYYY-MM-DDTHH:mm:ss")
                            )
                        );
                    });
                }
                this.primengTableHelper.records = result.lstSellingRequestDto;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
            });
    }

    subscribeAgentSellingRequest = () => {
        this.agentOrderSub = this.socketioService.sellingRequest.subscribe(
            (data: SellingRequestDto) => {
                const index = this.primengTableHelper.records.findIndex(
                    (e) => e.id === data.id
                );

                this.primengTableHelper.records[index].sellingStatusId =
                    data.sellingStatusId;
                this.reloadPage();
            }
        );
    };

    subscribeBotsellingRequest = () => {
        this.botOrderSub = this.socketioService.sellingRequest.subscribe(
            (data: SellingRequestDto) => {
                var xx = JSON.stringify({ data });

                if (data.sellingStatusId != 1) {
                    const index = this.primengTableHelper.records.findIndex(
                        (e) => e.id === data.id
                    );

                    this.primengTableHelper.records[index].sellingStatusId =
                        data.sellingStatusId;
                    this.reloadPage();
                } else if (data.tenantId == this.appSession.tenantId) {
                    this.primengTableHelper.records.push(data);
                    this.primengTableHelper.totalRecordsCount =
                        this.primengTableHelper.totalRecordsCount + 1;
                    this.reloadPage();
                }
            }
        );
    };

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    deleteAllForEver(): void {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
            }
        });
    }
}
