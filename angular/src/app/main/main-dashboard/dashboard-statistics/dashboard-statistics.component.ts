import { Component, Injector, OnInit, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import * as rtlDetect from "rtl-detect";
import { MainDashboardServiceService } from "../main-dashboard-service.service";
import { TransactionModel } from "@shared/service-proxies/service-proxies";
import { PermissionCheckerService } from "abp-ng2-module";
import { AddFundsComponent } from "../add-funds/add-funds.component";

@Component({
    selector: "app-dashboard-statistics",
    templateUrl: "./dashboard-statistics.component.html",
    styleUrls: ["./dashboard-statistics.component.css"],
})
export class DashboardStatisticsComponent
    extends AppComponentBase
    implements OnInit
{
    isArabic = false;
    @ViewChild("transactionsModal", { static: true })
    transactionsModal: TransactionModel;
    @ViewChild("addFunds", { static: true }) addFunds: AddFundsComponent;
    isHasBillingPermission = false;

    constructor(
        injector: Injector,
        public dasboardService: MainDashboardServiceService,
        private _permissionCheckerService: PermissionCheckerService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.isHasBillingPermission =
            this._permissionCheckerService.isGranted("Pages.Billings");
        console.log(this.appSession.tenant.currencyCode);
    }

    isJOD(): boolean {
        if (this.appSession.tenant.currencyCode == "JOD") {
            return true;
        } else {
            return false;
        }
    }
        isPLS(): boolean {
        if (this.appSession.tenant.currencyCode == "PIS") {
            return true;
        } else {
            return false;
        }
    }

    getJOD(value) {
        if (value) {
            return (value * 0.71).toFixed(2);
        }
        else {
            return 0;
        }
    }
    getShekel(value) {
        if (value) {
            return (value * 3.6).toFixed(2); 
        } else {
            return 0;
        }
    }

    getTotalMessages(value) {
        if (value) {
            return Math.floor(value / 0.014);
        }
        else {
            return 0;
        }
    }
}
