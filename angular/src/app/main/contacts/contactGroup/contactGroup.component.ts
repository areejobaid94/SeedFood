import {
    Component,
    inject,
    Injector,
    OnInit,
    ViewChild,
    ViewEncapsulation,
} from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/common/app-component-base";
// import { LazyLoadEvent, Paginator } from 'primeng';
import * as rtlDetect from "rtl-detect";
import { ContactGroupModalComponent } from "./contactGroup-modal/contactGroup-modal.component";
import { Router } from "@angular/router";
import {
    GroupLog,
    GroupProgressDto,
    GroupServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { finalize, switchMap } from "rxjs/operators";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import { SharedService } from "@shared/shared-services/shared.service";
import { Subscription, timer } from "@node_modules/rxjs";

@Component({
    encapsulation: ViewEncapsulation.None,
    templateUrl: "./contactGroup.component.html",
    styleUrls: ["./contactGroup.component.scss"],
    animations: [appModuleAnimation()],
})
export class ContactGroupComponent extends AppComponentBase implements OnInit {
    @ViewChild("createGroupContact", { static: false })
    modal: ContactGroupModalComponent;
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    products: any[];
    visible: boolean = false;
    theme: string;
    faildloading: boolean;
    selectedGroupId: number;
    totalFaildRecords: number;
    faildContacts!: GroupLog;
    faildNameOrPhoneSearch: string = "";
    isArabic = false;
    filterGroupName: string = "";
    isEdit: boolean = false;

    onHoldValue: number = 1;
    totalNumbersOfGroup: number = 250;

    public progressbarHeight = ".900rem";

    progressPercent = 0;
    insertedCount = 0;
    remainingCount = 0;
    total = 0;
    isProgressComplete = false;
    progressInterval: any;
    progressSubscription: Subscription;
    progressMap: { [groupId: number]: GroupProgressDto } = {};
    displayProgressMap: { [groupId: number]: number } = {};


    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private router: Router,
        private groupService: GroupServiceProxy
    ) {
        super(injector);
    }

    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        await this.getIsAdmin();

        const cachedId = localStorage.getItem('currentGroupId');
        if (cachedId) {
            const groupId = +cachedId;
            this.startPollingGroupProgress(groupId, this.appSession.tenantId);
        }

    }


    startPollingGroupProgress(groupId: number, tenantId: number): void {
        if (this.displayProgressMap[groupId] === undefined) {
            this.displayProgressMap[groupId] = 0;
        }

        this.progressSubscription = timer(0, 1000)
            .pipe(
                switchMap(() => this.groupService.getGroupProgress(groupId, tenantId))
            )
            .subscribe({
                next: (progress) => {
                    this.progressMap[groupId] = progress;

                    this.animateProgress(
                        groupId,
                        this.displayProgressMap[groupId],
                        progress.progressPercent
                    );
                },
                error: () => {
                    this.notify.error(`Failed to fetch progress for group ${groupId}.`);
                    this.stopPolling();
                }
            });
    }

    animateProgress(groupId: number, from: number, to: number) {
        const step = 1;
        const interval = 100;

        const animationKey = `animationTimer_${groupId}`;
        if ((this as any)[animationKey]) {
            clearInterval((this as any)[animationKey]);
        }

        (this as any)[animationKey] = setInterval(() => {
            if (this.displayProgressMap[groupId] < to) {
                this.displayProgressMap[groupId] = Math.min(
                    this.displayProgressMap[groupId] + step,
                    to
                );
            } else {
                clearInterval((this as any)[animationKey]);

                if (to === 100) {
                    this.notify.success('Group creation completed!');
                    this.stopPolling(); 
                    this.router.navigate(['/app/main/groupcontact']);
                    localStorage.removeItem('currentGroupId');
                }
            }
        }, interval);
    }




    stopPolling(): void {
        if (this.progressSubscription) {
            this.progressSubscription.unsubscribe();
        }
    }

    ngOnDestroy(): void {
        this.stopPolling();
    }


    loadFaild(event: LazyLoadEvent) {
        if (!this.selectedGroupId) {
            return;
        }

        if (
            this.faildNameOrPhoneSearch.length > 0 &&
            /^\s*$/.test(this.faildNameOrPhoneSearch)
        ) {
            return;
        }
        this.faildloading = true;
        this.groupService
            .groupLogGetAll(
                this.selectedGroupId,
                this.faildNameOrPhoneSearch,
                event?.first || 0,
                event?.rows || 20
            )
            .subscribe((result) => {
                if (result.totalCount > 0) {
                    this.visible = true;
                    this.faildContacts = result;
                    this.totalFaildRecords = result.totalCount;
                    this.faildloading = false;
                } else {
                    this.message.warn(
                        this.l("faildmessage"),
                        this.l("failedTitle")
                    );
                }
            });
    }


    
    editGroup(id: number, groupName:string) {
        this.router.navigate(["/app/main/group/editgroup"], {
            queryParams: { id, groupName },
        });
    }



    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    handleCloseDialog() {
        this.faildNameOrPhoneSearch = "";
        this.visible = false;
    }

    getFaildNames(id: number) {
        this.selectedGroupId = id;
        this.loadFaild(null);
    }

    refreshGroups() {
        this.getGroupAll();
    }

    deleteGroup(record: any): void {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                let groupId = localStorage.getItem('currentGroupId');
                if(groupId){
                    localStorage.removeItem('currentGroupId');
                    this.stopPolling(); 

                }
                this.primengTableHelper.showLoadingIndicator();
                this.groupService.groupDelete(record.id).subscribe(() => {
                    this.reloadPage();
                    this.primengTableHelper.hideLoadingIndicator();
                    this.notify.success(this.l("SuccessfullyDeleted"));
                });
            }
        });
    }

    Sync(event?: LazyLoadEvent) {
        this.getGroupAll(event);
    }
    getGroupAll(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this.groupService
            .groupGetAll(
                this.filterGroupName,
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => this.primengTableHelper.hideLoadingIndicator())
            )
            .subscribe((result) => {
                this.primengTableHelper.totalRecordsCount = result.total;

                this.primengTableHelper.records = result.groupDtoModel;
            });
    }

    openModal() {
        this.modal.open();
    }
}
