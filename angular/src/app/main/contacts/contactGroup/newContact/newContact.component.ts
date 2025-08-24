import {
    Component,
    ElementRef,
    Injector,
    Input,
    ViewChild,
    ViewEncapsulation,
    inject,
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    AreaDto,
    AreasServiceProxy,
    AssetServiceProxy,
    ContactsEntity,
    FileParameter,
    GetAllDashboard,
    GroupCreateDto,
    GroupDtoModel,
    GroupMembersDto,
    GroupServiceProxy,
    MembersDto,
    MembersList,
    WhatsAppContactsDto,
} from "@shared/service-proxies/service-proxies";
import { DatePipe } from "@angular/common";
import "chartjs-plugin-labels";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import {
    SearchCountryField,
    CountryISO,
    PhoneNumberFormat,
} from "ngx-intl-tel-input";
import moment from "moment";
import { PermissionCheckerService } from "abp-ng2-module";
import * as rtlDetect from "rtl-detect";
import { MessageCampaignService } from "@app/main/message-campaign/message-campaign.service";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { LazyLoadEvent } from "primeng/api";
import { SharedService } from "@shared/shared-services/shared.service";
import * as XLSX from "xlsx";
import { NgxSpinnerService } from "@node_modules/ngx-spinner/ngx-spinner";

@Component({
    selector: "app-newContact",
    templateUrl: "./newContact.component.html",
    styleUrls: ["./newContact.component.scss"],
})
export class NewContactComponent extends AppComponentBase {
    @Input() isUnsubscribed: boolean = false;
    @ViewChild("fileDropRef", { static: false }) fileDropEl: ElementRef;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: false }) paginator: Paginator;
    theme: string;
    file: any;
    submitted = false;

    groupName: string = "";
    createGroup!: GroupCreateDto;

    showMessageLoader: boolean = false;
    saving = false;
    contactFlag: boolean = false;
    filterContactFlag: boolean = true;
    externalContactFlag: boolean = false;
    fileToUpload: any;
    tempFile: any;
    extFile: string;
    sendTime: string;
    totalContact: number = 0;
    totalFailed: number = 0;
    totalSucceeded: number = 0;
    contact: WhatsAppContactsDto = new WhatsAppContactsDto();
    lstContact: WhatsAppContactsDto[] = [new WhatsAppContactsDto()];
    lstContactWithoutOptOut: string[] = [];
    ContactsEntity: ContactsEntity = new ContactsEntity();

    totalOptOut: number = 0;

    dropdownList = [];
    dropdownSettings = {};
    selectedItems = [];
    faildContact = [];
    succeededContact: MembersDto[] = [];
    selectedContact: MembersDto[] = [];
    customers: MembersList[] = [];

    isCount = false;
    //isExceededLimit = false;
    contactLength = 0;

    area: AreaDto = new AreaDto();

    dailyLimit: number = 0;
    countryObj: any = null;
    countryObj2: any = null;
    separateDialCode = false;
    SearchCountryField = SearchCountryField;
    CountryISO = CountryISO;
    PhoneNumberFormat = PhoneNumberFormat;
    preferredCountries: CountryISO[] = [
        CountryISO.SaudiArabia,
        CountryISO.Jordan,
    ];
    isHasPermissionOrder: boolean;

    Measurement: GetAllDashboard = new GetAllDashboard();
    confirm: string = "";
    sendCampaignValidation: boolean = true;
    

    orderTime1: moment.Moment | undefined;
    orderTime2: moment.Moment | undefined;

    groupAddResult!: GroupCreateDto;
    EXCEL_TYPE =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8";
    EXCEL_EXTENSION = ".xlsx";
    isArabic = false;
    uploadedFiles: any[] = [];
    duplicateCount: number = 0;

    private groupSharedService: SharedService = inject(SharedService);

    constructor(
        injector: Injector,
        private route: ActivatedRoute,
        private messageCampaignService: MessageCampaignService,
        private router: Router,
        private datePipe: DatePipe,
        private _assetServiceProxy: AssetServiceProxy,
        private _areasServiceProxy: AreasServiceProxy,
        private _permissionCheckerService: PermissionCheckerService,
        public darkModeService: DarkModeService,
        private _router: Router,
        private groupService: GroupServiceProxy,

    ) {
        super(injector);
        this.isRowSelectable = this.isRowSelectable.bind(this);
    }

    ngOnInit(): void {
        this.checkName();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.theme = ThemeHelper.getTheme();
        this.isHasPermissionOrder =
            this._permissionCheckerService.isGranted("Pages.Orders");
    }

    visible: boolean = false;

    showDialog() {
        this.visible = true;
    }

    checkName() {
        this.groupSharedService.currentGroupName.subscribe((name) => {
            if (name) {
                this.groupName = name;
            } else {
                this._router.navigate(["/app/main/dashboard"]);
            }
        });
    }

    /**
     * on file drop handler
     */
    onFileDropped($event) {
        //this.isExceededLimit = false;
        this.prepareFilesList($event);
    }

    /**
     * handle file from browsing
     */
    fileBrowseHandler(files) {
        //this.isExceededLimit = false;
        this.prepareFilesList(files);
    }

    uploadFileToActivity() {
        this.selectedContact = [];
        this.customers = [];
        let formDataFile = new FormData();

        formDataFile.append("formFile", this.file);

        const fileParameter: FileParameter = {
            data: this.file,
            fileName: this.file.name,
        };

        this.primengTableHelper.showLoadingIndicator();
        this.spinnerService.show();
        this.groupService.readFromExcel(fileParameter, []).subscribe(
            (result) => {
                this.spinnerService.hide();
                this.totalContact = result.list.length;
                this.faildContact = result.list.filter((x) => x.isFailed);
                this.totalFailed = this.faildContact.length;
                //total succeeded phone numbers
                this.duplicateCount = result.duplicateCount;

                this.succeededContact = result.list
                    .filter((x) => !x.isFailed)
                    .map(
                        (item) =>
                            new MembersDto({
                                id: item.id,
                                phoneNumber: item.phoneNumber,
                                displayName: item.displayName,
                                failedId: 0,
                                isFailed: item.isFailed,
                                variables: item.variables,
                                customeropt: item.customerOPT,
                            })
                    );

                this.totalSucceeded = this.succeededContact.length;

                // if (this.totalSucceeded > 50000) {
                //     this.isExceededLimit = true;
                // }
                this.contactLength = result.list.length;
                // this.primengTableHelper.hideLoadingIndicator();
                // this.primengTableHelper.totalRecordsCount = result.length;
                // this.primengTableHelper.records = result;
                this.customers = result.list;
                if (this.duplicateCount > 0) {
                    this.message.error("", this.l(`${this.duplicateCount} duplicate contacts detected in uploaded file`));
                }

            },
            (error: any) => {
                if (error) {
                    // this.primengTableHelper.hideLoadingIndicator();
                    this.spinnerService.hide();
                    this.notify.error(error.error.error.message);
                }
            }
        );
    }

    isRowSelectable(event) {
        return !this.isInvalidContact(event.data);
    }

    isInvalidContact(data) {
        return data.isFailed;
    }

    hasOnlySpaces(inputString: string): boolean {
        return /^\s*$/.test(inputString);
    }

    onRowEditInit(contact: MembersDto) {
        // this.clonedContacts[contact.id] = null
        // this.clonedContacts[contact.id] = { ...contact };
    }

    onRowEditSave(memeber: MembersDto, index: number) {
        let result = this.isValidNumber(Number(memeber.phoneNumber));
        if (result) {
            this.customers[index].isFailed = false;
            this.notify.success("success");
        } else {
            this.selectedContact = this.selectedContact.filter(
                (item) => item.phoneNumber !== memeber.phoneNumber
            );
            this.customers[index].isFailed = true;
            this.notify.error("invalid Formate");
        }
    }

    handleCloseDialog() {
        this.visible = false;
        this.router.navigate(["/app/main/groupcontact"]);
    }

    isValidNumber(number): boolean {
        const regex = /^\d{11,15}$/g;

        if (!regex.test(number)) {
            return false;
        }
        return true;
    }

    handleGroup() {
        // console.log(group)
        this.submitted = true;
        this.groupAddResult = null;

        if (this.hasOnlySpaces(this.groupName)) {
            this.message.error("", this.l("canotHaveOnlySpace"));
            this.submitted = false;
            return;
        }

        if (this.groupName.length === 0) {
            this.submitted = false;
            this.message.error("", this.l("groupNamecantbeEmpty"));
            return;
        }
        this.message.confirm(
            "",
            this.l("areyousureyouwanttoadd") +
            this.succeededContact.length +
            "Memebers ? ",
            (isConfirmed) => {
                if (isConfirmed) {
                    this.primengTableHelper.showLoadingIndicator();

                    const groupMembersDto = new GroupMembersDto();
                    groupMembersDto.groupDtoModel = new GroupDtoModel();
                    groupMembersDto.groupDtoModel.groupName = this.groupName;
                    groupMembersDto.membersDto = this.succeededContact;
                    groupMembersDto.totalCount = 0;

                    this.groupService
                        .groupCreateMembers(true, this.isUnsubscribed, groupMembersDto)
                        .subscribe(
                            (res) => {
                                this.submitted = false;

                                this.groupAddResult = res;
                                this.primengTableHelper.hideLoadingIndicator();
                                if (res.state === 1) {
                                    this.message.error(
                                        "",
                                        this.l("groupNameisUsed")
                                    );
                                    return;
                                } else if (res.state === 4) {
                                    this.message.error(
                                        "",
                                        this.l("groupNamecantbeempty")
                                    );
                                    return;
                                } else if (res.state === 3) {
                                    this.message.error(
                                        "",
                                        this.l("invalidTenant")
                                    );
                                    return;
                                } else if (res.state === 2) {
                                    localStorage.setItem('currentGroupId', res.id.toString());
                                    this.notify.success("Group creation started.");
                                    this.router.navigate([
                                        "/app/main/groupcontact",
                                    ]);
                                } else {
                                    // if (res.failedCount === 0) {
                                    //     let str =
                                    //         res.successCount +
                                    //         this.l("succeesfull");
                                    //     this.notify.success(str);
                                    //     this.router.navigate(['/app/main/groupcontact'])
                                    // } else {
                                    //     this.showDialog();
                                    // }
                                }

                                this.router.navigate([
                                    "/app/main/groupcontact",
                                ]);
                            },
                            (error: any) => {
                                if (error) {
                                    this.submitted = false;
                                    this.primengTableHelper.hideLoadingIndicator();
                                    this.notify.error(
                                        error.error.error.message
                                    );
                                    this.router.navigate([
                                        "/app/main/groupcontact",
                                    ]);
                                }
                            }
                        );
                } else {
                    this.submitted = false;
                }
            }
        );
    }

    /**
     * Simulate the upload process
     */
    uploadFilesSimulator(index: number) {
        this.uploadFileToActivity();
    }

    /**
     * Convert Files list to normal array list
     * @param files (Files List)
     */
    prepareFilesList(files: any) {
        const allowedExtensions = [".xlsx", ".xls"];
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const fileName = file.name;
            const fileExtension = fileName
                .substring(fileName.lastIndexOf("."))
                .toLowerCase();
            if (allowedExtensions.includes(fileExtension)) {
                // Process the file since it's an Excel file
                this.file = file;
                this.uploadFileToActivity();
            } else {
                this.message.error(this.l("Please Select Only Excel File"));
            }
        }
    }

    /**
     * format bytes
     * @param bytes (File size in bytes)
     * @param decimals (Decimals point)
     */
    formatBytes(bytes, decimals) {
        if (bytes === 0) {
            return "0 Bytes";
        }
        const k = 1024;
        const dm = decimals <= 0 ? 0 : decimals || 2;
        const sizes = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return (
            parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i]
        );
    }

    PaginatorContacts(event: LazyLoadEvent) {
        if (this.filterContactFlag) {
            // this.filterContacts(event);
        }
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    goToDashboard() {
        this._router.navigate(["/app/main/dashboard"]);
    }

    goToCampaign() {
        this._router.navigate(["/app/main/messageCampaign"]);
    }

    exportToExcel(): void {
        // Create a new workbook and add a worksheet
        const workbook = XLSX.utils.book_new();
        const worksheet = XLSX.utils.json_to_sheet(
            this.faildContact.map((x) => ({
                ...x,
                id: x.id + 1,
            }))
        );

        // Append the worksheet to the workbook
        XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

        // Generate a binary string of the workbook and create a Blob object
        const excelBuffer: any = XLSX.write(workbook, {
            bookType: "xlsx",
            type: "array",
        });
        const dataBlob: Blob = new Blob([excelBuffer], {
            type: this.EXCEL_TYPE,
        });

        // Create a download link and trigger the download
        const url = window.URL.createObjectURL(dataBlob);
        const link = document.createElement("a");
        link.href = url;
        link.download = "export.xlsx";
        link.click();
        window.URL.revokeObjectURL(url);
    }
}
