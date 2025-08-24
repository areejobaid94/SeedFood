import {
    ChangeDetectorRef,
    Component,
    Injector,
    OnInit,
    ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import * as rtlDetect from "rtl-detect";
import { DarkModeService } from "./../../services/dark-mode.service";
import { LazyLoadEvent } from "primeng/api";
import { CreateFlowComponent } from "./create-flow/create-flow.component";
import { BotFlowServiceProxy } from "@shared/service-proxies/service-proxies";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";
import { Router } from "@angular/router";
import Swal from "sweetalert2";

@Component({
    templateUrl: "./bot-flows.component.html",
    styleUrls: ["./bot-flows.component.css"],
})
export class BotFlowsComponent extends AppComponentBase {
    theme: string;
    isArabic = false;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    @ViewChild("createBotFlow", { static: true })
    createBotFlow: CreateFlowComponent;

    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private _BotFlowServiceProxy: BotFlowServiceProxy,
        private router: Router,
        private cdr: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
    }

    getFlows(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();
        this._BotFlowServiceProxy
            .getAllBotFlows(
                this.appSession.tenantId,
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .subscribe((result) => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }
    edit(id, record) {
        if (record.statusId === 4 && record.isPublished) {
            this.notify.error(this.l("CannotEditPublishedFlow"));
            return;
        } else {
            this.router.navigate(["/app/admin/botBuilder", id]);
        }
    }

    deleteBotFlows(id, flow) {
        if (flow.isPublished) {
            this.notify.error(this.l("CannotDeletePublishedFlow"));
            return;
        }
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                // Second confirmation using SweetAlert2
                Swal.fire({
                    title: 'Are you sure you want to delete this bot?',
                    input: 'text',
                    inputPlaceholder: 'Type REMOVE to confirm',
                    showCancelButton: true,
                    confirmButtonText: 'Confirm Delete',
                    cancelButtonText: 'Cancel',
                    inputValidator: (value) => {
                        if (value?.trim().toLowerCase() !== 'remove') {
                            return 'You must type REMOVE to confirm';
                        }
                        return null;
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Actual delete logic
                        this._BotFlowServiceProxy.deleteBotFlows(id).subscribe(
                            (res) => {
                                if (res > 0) {
                                    this.notify.success(this.l("SuccessfullyDeleted"));
                                    this.getFlows();
                                } else {
                                    this.notify.error(this.l("Failed"));
                                }
                            },
                            (error: any) => {
                                this.notify.error(this.l("Failed"));
                            }
                        );
                    }
                });
            }
        });
    }

    askForDeploy(bot) {
        debugger;
        bot.isPublished = !bot.isPublished;
        if (bot.getBotFlowForViewDto != undefined) {
                bot.isPublished = !bot.isPublished;
                //Check if the flow is outbound
                if (bot.statusId == 4) {
                    if (bot.isPublished) {
                        this.message.confirm(
                            "Are You Sure You Want To Deactivate This Flow?",
                            this.l("Important Notice"),
                            (isConfirmed) => {
                                if (isConfirmed) {
                                    this.deploy(bot);
                                } else {
                                    bot.isPublished = !bot.isPublished;
                                    this.cdr.detectChanges();
                                }
                            }
                        );
                    } else {
                        if (bot.getBotFlowForViewDto.length === 0) {
                            this.notify.error(this.l("Please Add Nodes"));
                            this.reloadPage();
                            return;
                        }
                        this.message.confirm(
                            "When the flow is deployed, it CANNOT be deactivated or terminated After 24 HOURS. Please proceed with deployment carefully.",
                            this.l("Important Notice"),
                            (isConfirmed) => {
                                if (isConfirmed) {
                                    this.deploy(bot);
                                } else {
                                    bot.isPublished = !bot.isPublished;
                                    this.cdr.detectChanges();
                                }
                            }
                        );
                    }
                } else {
                    if (bot.isPublished) {
                        this.message.confirm(
                            this.l(
                                "Are You Sure You Want To Deactivate This Flow?"
                            ),
                            this.l("Important Notice"),
                            (isConfirmed) => {
                                if (isConfirmed) {
                                    this.deploy(bot);
                                } else {
                                    bot.isPublished = !bot.isPublished;
                                    this.cdr.detectChanges();
                                }
                            }
                        );
                    } else {
                        if (bot.getBotFlowForViewDto.length === 0) {
                            this.notify.error(this.l("Please Add Nodes"));
                            this.reloadPage();
                            return;
                        }
                        this.message.confirm(
                            "Please Note: when you deploy this flow it will effect on the bot directly, please proceed with deployment carefully!",
                            this.l("Important Notice"),
                            (isConfirmed) => {
                                if (isConfirmed) {
                                    this.deploy(bot);
                                } else {
                                    bot.isPublished = !bot.isPublished;
                                    this.cdr.detectChanges();
                                }
                            }
                        );
                    }
                }
        } else {
            this.notify.error(this.l("Please Add Nodes"));
            this.reloadPage();
        }
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }


    deploy(bot) {
        this._BotFlowServiceProxy
            .updateBotFlowsIsPublished(
                bot.id,
                this.appSession.userId,
                this.appSession.user.name,
                bot.isPublished,
                bot.botChannel
            )
            .subscribe((result: any) => {
                if (result > 0) {
                    this.notify.success(this.l("savedSuccessfully"));
                    this.reloadPage();
                }
            });
    }
}
