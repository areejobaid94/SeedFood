import {
    Component,
    Injector,
    ViewEncapsulation,
    ViewChild,
} from "@angular/core";

import {
    OrderDto,
    MaintenancesServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";

import { ViewMaintenanceModalComponent } from "./view-Maintenance-modal.component";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import * as _ from "lodash";
import * as moment from "moment";
import { AppSessionService } from "@shared/common/session/app-session.service";
import { Subscription } from "rxjs";

@Component({
    templateUrl: "./Maintenance.component.html",
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class MaintenanceComponent extends AppComponentBase {
    @ViewChild("viewMaintenanceModalComponent", { static: true })
    viewMaintenanceModal: ViewMaintenanceModalComponent;

    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = "";
    nameFilter = "";
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
    ifSameUser = true;
    appSession: AppSessionService;
    items: any;
    agentOrderSub: Subscription;
    botOrderSub: Subscription;
    change: any;
    differ: any[];

    constructor(
        injector: Injector,
        private _maintenancesServiceProxy: MaintenancesServiceProxy
    ) {
        super(injector);
    }

    async ngOnInit() {
        await this.getIsAdmin();
    }

    getMaintenance(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this._maintenancesServiceProxy
            .getAll(
                this.nameFilter,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .subscribe((result) => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
            });
    }

    subscribeAgentOrder = () => {};

    subscribeBotOrder = () => {};

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    lockOrder(order: OrderDto, stringTotla: string): void {
        this._maintenancesServiceProxy
            .lock(
                this.appSession.user.id,
                this.appSession.user.userName,
                stringTotla,
                order
            )
            .subscribe(() => {
                this.notify.success(this.l("successfullyLocked"));
            });
    }
    UnlockOrderS(order: OrderDto, stringTotla: string): void {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._maintenancesServiceProxy
                    .unLock(stringTotla, order)
                    .subscribe(() => {
                        // this.reloadPage();
                        this.notify.success(this.l("successfullyUnlocked"));
                    });
            }
        });
    }
}
