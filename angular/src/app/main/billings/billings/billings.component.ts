import {
    Component,
    Injector,
    ViewEncapsulation,
    ViewChild,
    ElementRef,
} from "@angular/core";
import {
    BillingDto,
    ZohoServiceProxy,
    InvoicesModel,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";

import * as _ from "lodash";
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../../services/dark-mode.service";
import { ViewBillingModalComponent } from "./view-billing-modal.component";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import { SocketioService } from "@app/shared/socketio/socketioservice";

import { ExportAsService, ExportAsConfig } from "ngx-export-as";

declare var require: any;


@Component({
    templateUrl: "./billings.component.html",
    encapsulation: ViewEncapsulation.None,
    styleUrls: ["./billings.component.css"],
    animations: [appModuleAnimation()],
})
export class BillingsComponent extends AppComponentBase {
    StatementsUrl = "";
    theme: string;

    @ViewChild("viewBillingModal", { static: true })
    viewBillingModal: ViewBillingModalComponent;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    FilterStatements = null;
    printData: any;
    @ViewChild("pdfTable", { static: false }) pdfTable: ElementRef;
    exportAsConfig: ExportAsConfig = {
        type: "pdf", // the type you want to download
        elementIdOrContent: "sampleTable", // the id of html/table element,
        options: {
            // html-docx-js document options
            unit: "in",
            format: "letter",
        },
    };
    constructor(
        injector: Injector,
        private socketioService: SocketioService,
        public darkModeService: DarkModeService,
        private _zohoServiceProxyProxy: ZohoServiceProxy,
        private exportAsService: ExportAsService
    ) {
        super(injector);
    }

    ngOnInit() {
        this.subscribeInvoicesRequest();
        this.FilterStatements = "";
        this.theme = ThemeHelper.getTheme();
        this.FilterStatements = null;
    }

    subscribeInvoicesRequest = () => {
        this.socketioService.Invoices.subscribe((data: InvoicesModel) => {
            if (data.tenantId == this.appSession.tenantId) {
                const index = this.primengTableHelper.records.findIndex(
                    (e) => e.invoice_id === data.invoices[0].invoice_id
                );

                if (index != -1) {
                    this.primengTableHelper.records[index].status =
                        data.invoices[0].status;
                }
            }
        });
    };

    getBillings(event?: LazyLoadEvent) {
        this.StatementsUrl =
            "https://invoice.zoho.com/api/v3/contacts/" +
            this.appSession.tenant.zohoCustomerId +
            "/statements";
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        // var pagenumber =
        //     this.primengTableHelper.getSkipCount(this.paginator, event) /
        //     this.primengTableHelper.getMaxResultCount(this.paginator, event);

        this._zohoServiceProxyProxy
            .invoicesGet(
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator,event)
            )
            .subscribe((result) => {
                this.primengTableHelper.totalRecordsCount =
                    result.page_context.total;
                this.primengTableHelper.records = result.invoices;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }
    onChangeStatements(event): void {
        this.FilterStatements = event.target.value;
        this._zohoServiceProxyProxy
            .getStatementsFillter(this.FilterStatements)
            .subscribe(
                (result) => {
                    this.printData = result.data.html_string;
                },
                (error: any) => {
                    if (error) {
                        this.notify.error(this.l(error));
                    }
                }
            );
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    Sync(event?: LazyLoadEvent) {
        this._zohoServiceProxyProxy
            .syncBilling(
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event),
                null
            )
            .subscribe((result) => {
                // console.log(result);

                this.getBillings(event);
            });
       
    }

    deleteBilling(billing: BillingDto): void {
        // this.message.confirm(
        //     '',
        //     this.l('AreYouSure'),
        //     (isConfirmed) => {
        //         if (isConfirmed) {
        //             this._billingsServiceProxy.delete(billing.id)
        //                 .subscribe(() => {
        //                     this.reloadPage();
        //                     this.notify.success(this.l('SuccessfullyDeleted'));
        //                 });
        //         }
        //     }
        // );
    }

    DownLoadStatements() {
        if (this.printData) {
            this.exportAsConfig.type = "pdf";
            const printContents =
                document.getElementById("sampleTable").innerHTML;
            this.exportAsConfig.elementIdOrContent = printContents;
            this.exportAsService
                .save(this.exportAsConfig, "Statements")
                .subscribe(() => {});
            // get the data as base64 or json object for json type - this will be helpful in ionic or SSR
        } else {
            this.notify.error(this.l("pleaseChoosePeriod"));
        }
    }
}
