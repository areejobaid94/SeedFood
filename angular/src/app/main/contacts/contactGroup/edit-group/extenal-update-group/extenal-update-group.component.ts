import { DatePipe } from "@angular/common";
import {
    Component,
    ElementRef,
    EventEmitter,
    Injector,
    Input,
    OnInit,
    Output,
    ViewChild,
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { MessageCampaignService } from "@app/main/message-campaign/message-campaign.service";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    AssetServiceProxy,
    AreasServiceProxy,
    GroupServiceProxy,
    FileParameter,
    GroupMembersDto,
    GroupDtoModel,
    MembersDto,
    GroupCreateDto,
    MembersList,
} from "@shared/service-proxies/service-proxies";
import { PermissionCheckerService } from "abp-ng2-module";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";
import * as XLSX from "xlsx";

@Component({
    selector: "app-extenal-update-group",
    templateUrl: "./extenal-update-group.component.html",
    styleUrls: ["./extenal-update-group.component.css"],
})
export class ExtenalUpdateGroupComponent
    extends AppComponentBase
    implements OnInit
{
    @ViewChild("fileDropRef", { static: false }) fileDropEl: ElementRef;
    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: false }) paginator: Paginator;
    @Input()
    groupName: string;
    @Input() totalRecords: number;
    @Input() isGroupUnsubscribed: boolean = false;

    contactLength = 0;
    theme: string;
    file: any;
    submitted = false;
    totalContact: number = 0;
    totalFailed: number = 0;
    totalSucceeded: number = 0;
    faildContact = [];
    succeededContact: MembersDto[] = [];
    selectedItems = [];
    selectedContact: MembersDto[] = [];
    @Input()
    groupID: number;
    customers: MembersList[] = [];
    groupAddResult!: GroupCreateDto;
    EXCEL_TYPE =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8";
    EXCEL_EXTENSION = ".xlsx";

    constructor(
        injector: Injector,
        private router: Router,
        public darkModeService: DarkModeService,
        private groupService: GroupServiceProxy
    ) {
        super(injector);
        this.isRowSelectable = this.isRowSelectable.bind(this);
    }

    ngOnInit() {}

    visible: boolean = false;

    showDialog() {
        this.visible = true;
    }

    onFileDropped($event) {
        this.prepareFilesList($event);
    }
    fileBrowseHandler(files) {
        this.file = null;
        this.prepareFilesList(files);
    }
    //  numberBeforeEdit : string ;

    onRowEditInit(contact: MembersDto) {
        // this.numberBeforeEdit = contact.phoneNumber;
        // this.clonedContacts[contact.id] = null
        // this.clonedContacts[contact.id] = { ...contact };
    }

    onRowEditSave(memeber: MembersDto, index: number) {
        let result = this.isValidNumber(Number(memeber.phoneNumber));
        if (result) {
            this.primengTableHelper.records[index].isFailed = false;
            this.notify.success("success");
        } else {
            this.selectedContact = this.selectedContact.filter(
                (item) => item.phoneNumber !== memeber.phoneNumber
            );
            this.primengTableHelper.records[index].isFailed = true;
            this.notify.error("invalid Formate");
        }
    }

    isValidNumber(number): boolean {
        const regex = /^\d{11,15}$/g;

        if (!regex.test(number)) {
            return false;
        }
        return true;
    }

    uploadFileToActivity() {
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
                                customeropt: this.isGroupUnsubscribed ? 1 : item.customerOPT
                            })
                    );
                this.totalSucceeded = this.succeededContact.length;

                this.contactLength = result.list.length;
                // this.primengTableHelper.hideLoadingIndicator();
                // this.primengTableHelper.totalRecordsCount = result.length;
                // this.primengTableHelper.records = result;
                this.customers = result.list;
            },
            (error: any) => {
                if (error) {
                    this.primengTableHelper.hideLoadingIndicator();
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

    handleGroup() {
        this.submitted = true;
        if (this.hasOnlySpaces(this.groupName)) {
            this.submitted = false;
            this.message.error("", this.l("canotHaveOnlySpace"));
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
                "Contacts ? ",
            (isConfirmed) => {
                if (isConfirmed) {
                    this.primengTableHelper.showLoadingIndicator();

                    const groupMembersDto = new GroupMembersDto();
                    groupMembersDto.groupDtoModel = new GroupDtoModel();
                    groupMembersDto.groupDtoModel.groupName = this.groupName;
                    groupMembersDto.membersDto = this.succeededContact;
                    groupMembersDto.groupDtoModel.id = this.groupID;
                    groupMembersDto.totalCount = 0;

                    this.groupService
                        .groupUpdate(true, 2, groupMembersDto)
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
                                    this.notify.success("Group update started.");
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

                                // this.router.navigate(['/app/main/groupcontact'])
                            },
                            (error: any) => {
                                if (error) {
                                    this.submitted = false;
                                    this.primengTableHelper.hideLoadingIndicator();
                                    this.notify.error(
                                        error.error.error.message
                                    );
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

                console.log("File '" + fileName + "' is not an Excel file.");
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

    resetData(){
        this.contactLength = 0;
        this.file = null
        this.submitted = false;
        this.totalContact = 0;
        this.totalFailed = 0;
        this.totalSucceeded = 0;
        this.faildContact = [];
        this.succeededContact = [];
        this.selectedItems = [];
        this.selectedContact = [];
        this.customers = [];
    }
}
