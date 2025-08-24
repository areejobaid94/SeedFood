import { error } from 'console';
import { DatePipe } from "@angular/common";
import {
    Component,
    Injector,
    Input,
    OnInit,
    Output,
    ViewChild,
    EventEmitter,
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { MessageCampaignService } from "@app/main/message-campaign/message-campaign.service";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    AssetServiceProxy,
    AreasServiceProxy,
    GroupServiceProxy,
    MembersDto,
    GroupMembersDto,
    GroupDtoModel,
    MoveMembersDto,
} from "@shared/service-proxies/service-proxies";
import { PermissionCheckerService } from "abp-ng2-module";
import { LazyLoadEvent } from "primeng/api";
import * as rtlDetect from "rtl-detect";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";
import {
    debounceTime,
    distinctUntilChanged,
    finalize,
    switchMap,
} from "rxjs/operators";
import { Subject } from "rxjs";
@Component({
    selector: "app-delete-update-group",
    templateUrl: "./delete-update-group.component.html",
    styleUrls: ["./delete-update-group.component.css"],
})
export class DeleteUpdateGroupComponent
    extends AppComponentBase
    implements OnInit
{
    theme: string;
    isArabic = false;

    submitted = false;

    moveMembersDto!: MoveMembersDto;
    filtered = false;

    @Input()
    groupName: string;

    @Output() valueChange = new EventEmitter<string>();
    @Output() totalRecordsToPerant = new EventEmitter<number>();
    @Output() isGroupUnsubscribedChange = new EventEmitter<boolean>();
    @Input()
    groupID: number;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator1", { static: false }) paginator: Paginator;

    customers: MembersDto[];

    totalRecords: number;
    groups: GroupDtoModel[] = [];
    customerApiLoading = false;

    cols: any[];

    loading: boolean;
    loadingGroup: boolean;

    searchUser: string = null;
    currentPageNumber: number = 0;
    currentPageSize: number = 20;
    selectAll: boolean = false;
    customerApiInput$ = new Subject<string>();

    selectedNewGroup: GroupDtoModel | undefined;

    selectedCustomers: MembersDto[] = [];
    selectedCustomersToBeMoved: MembersDto[] = [];

    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private groupService: GroupServiceProxy
    ) {
        super(injector);
    }

    visible: boolean = false;

    showDialog() {
        this.visible = true;
    }

    handleChangeGroup(selectedCustomer: any = null) {
        if (selectedCustomer) {
            this.selectedCustomersToBeMoved = [];
            this.selectedCustomersToBeMoved.push(selectedCustomer);
        } else {
            this.selectedCustomersToBeMoved = this.selectedCustomers;
        }
        this.showDialog();
    }

    handleSelectGroup() {
        this.submitted = true;

        const moveMembersDto = new MoveMembersDto();

        moveMembersDto.membersDto = this.selectedCustomersToBeMoved;
        moveMembersDto.oldGroupId = this.groupID;
        moveMembersDto.newGroupId = this.selectedNewGroup.id;


        this.primengTableHelper.showLoadingIndicator();

        this.groupService.movingGroup(moveMembersDto).subscribe((res) => {
            this.submitted = false;
            this.selectedCustomers = [];
            this.selectedCustomersToBeMoved = [];
            this.selectedNewGroup = null;
            this.primengTableHelper.hideLoadingIndicator();
            this.visible = false;
            this.loadCustomers();
            if(res.message.includes("Members already exist in the group") ){
                this.message.error("", `There are ${res.failedCount} members already exist in the group`);
            }
            else{
                this.notify.success(this.l("successful"));
            }
        });
    }

    handleSelectChange(selectedItmem: any) {
        this.groupID = selectedItmem.id;
        // this.getGroupById();
    }

    trackByFn(item: any) {
        return item.contactId;
    }

    handleClose() {
        this.visible = false;
        this.selectedNewGroup = null;
        this.selectedCustomersToBeMoved = [];
    }
    getGroupAll() {

        this.groupService
            .groupGetAll("", 0, 2147483640)
            .subscribe(({ groupDtoModel }) => {
                this.groups = groupDtoModel.filter(group=>group.id !== this.groupID);
            });
    }

    ngOnInit() {
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.theme = ThemeHelper.getTheme();
        this.getGroupAll();
    }
    async loadCustomers(event?: LazyLoadEvent) {
        this.loading = true;

        try {
            const res = await this.groupService
                .groupGetById(
                    this.groupID,
                    "",
                    event?.first || 0,
                    event?.rows || 10
                )
                .toPromise(); // <-- convert Observable to Promise

            this.sendValue(res.groupDtoModel.groupName);
            this.customers = res.membersDto;
            this.totalRecords = res.groupDtoModel.totalNumber;
            this.totalRecordsToPerant.emit(this.totalRecords);

            const unsubscribed = this.customers.every(c => c.customeropt === 1);
            this.isGroupUnsubscribedChange.emit(unsubscribed);

        } catch (error) {
            console.error("Error loading customers:", error);
        } finally {
            this.loading = false;
        }
    }


    sendValue(groupName: string) {
        this.valueChange.emit(groupName);
    }

    onSelectionChange(value = []) {
        this.selectAll = value.length === this.totalRecords;
        this.selectedCustomers = value;
    }

    hasOnlySpaces(inputString: string): boolean {
        return /^\s*$/.test(inputString);
    }

    handleUpdate(selectedCustomer: any = null) {
    

        if (this.hasOnlySpaces(this.groupName)) {
            this.message.error("", this.l("canotHaveOnlySpace"));
            this.submitted = false;
            return;
        }
        this.message.confirm(
            "",
            selectedCustomer ? this.l("Are You sure you want to delete")+" " + selectedCustomer.phoneNumber:
            this.l("Are You sure you want to delete")+ ' ' + this.selectedCustomers .length +'Contacts',
            (isConfirmed) => {
                if (isConfirmed) {
                  this.submitted = true;
                    this.primengTableHelper.showLoadingIndicator();
                    const groupMembersDto = new GroupMembersDto();
                    groupMembersDto.groupDtoModel = new GroupDtoModel();
                    groupMembersDto.groupDtoModel.id = this.groupID;
                    groupMembersDto.groupDtoModel.groupName = this.groupName;
                    if (selectedCustomer) {
                        groupMembersDto.membersDto = [selectedCustomer];
                    } else {
                        groupMembersDto.membersDto =
                            this.selectedCustomers || [];
                    }
                    groupMembersDto.totalCount = 0;
                    this.primengTableHelper.showLoadingIndicator();

                    this.groupService
                        .groupUpdate(false, 3, groupMembersDto)
                        .subscribe((res) => {
                            this.submitted = false;
                            this.selectedCustomers = [];
                            this.notify.success(this.l("succeesfull"));
                            this.primengTableHelper.hideLoadingIndicator();

                            this.loadCustomers();
                        });
                }
            }
        );
    }
}
