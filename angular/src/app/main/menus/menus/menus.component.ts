import {
    Component,
    Injector,
    ViewEncapsulation,
    ViewChild,
    ElementRef,
} from "@angular/core";
import {
    MenusServiceProxy,
    MenuDto,
    ItemsServiceProxy,
    MenuTypeEnum,
    WhatsAppHeaderUrl,
    RType,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { CreateOrEditMenuModalComponent } from "./create-or-edit-menu-modal.component";
import { ViewMenuModalComponent } from "./view-menu-modal.component";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import { FileDownloadService } from "@shared/utils/file-download.service";
import * as _ from "lodash";
import * as moment from "moment";
import {
    FormControl,
    FormGroup,
    UntypedFormGroup,
    Validators,
} from "@angular/forms";
import { TeamInboxService } from "@app/main/teamInbox/teaminbox.service";
import { AppSessionService } from "@shared/common/session/app-session.service";
import { ViewItemSpecificationsModalComponent } from "./view-ItemSpecifications-modal.component";
import { CreateOrEditAdvancedMenuModalComponent } from "./create-or-edit-advanced-menu-modal.component";
import { CreateOrEditMenuSettingModalComponent } from "./create-or-edit-menu-setting-modal.component";
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../../services/dark-mode.service";
import { Event, Router } from "@angular/router";
import { FileUploader } from "ng2-file-upload";
import { base64ToFile, ImageCroppedEvent } from "ngx-image-cropper";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { HttpClient } from "@angular/common/http";
import { AppConsts } from "@shared/AppConsts";
import * as rtlDetect from "rtl-detect";
import { VideoModelComponent } from "@app/main/videoComponent/video-model.component";

@Component({
    templateUrl: "./menus.component.html",

    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class MenusComponent extends AppComponentBase {
    theme: string;
    showDropDownList = false;
    file: any;
    file2: any;

    imageChangedEvent: any = "";
    public uploader: FileUploader;
    private http: HttpClient;
    isArabic = false;
    loadLogoImage = false;
    loadBgImage = false;

    // Done Fixing..

    // Edit Add On's
    @ViewChild("viewMenuModalComponent", { static: true })
    viewMenuModal: ViewMenuModalComponent;

    // Edit Options
    @ViewChild("viewItemSpecificationsModal", { static: true })
    viewItemSpecificationsModal: ViewItemSpecificationsModalComponent;

    // NOT READ MODALS
    @ViewChild("createOrEditMenuModal", { static: true })
    createOrEditMenuModal: CreateOrEditMenuModalComponent;
    @ViewChild("createOrEditAdvancedMenuModal", { static: true })
    createOrEditAdvancedMenuModal: CreateOrEditAdvancedMenuModalComponent;

    @ViewChild("createOrEditMenuSettingModel", { static: true })
    createOrEditMenuSettingModel: CreateOrEditMenuSettingModalComponent;

    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    @ViewChild("viewVideo", { static: true })
    viewVideo: VideoModelComponent;

    uploadButton: boolean = false;
    files: any;
    chatForm: UntypedFormGroup;
    advancedFiltersAreShown = false;
    filterText = "";
    menuNameFilter = "";
    menuDescriptionFilter = "";
    maxEffectiveTimeFromFilter: moment.Moment;
    minEffectiveTimeFromFilter: moment.Moment;
    maxEffectiveTimeToFilter: moment.Moment;
    minEffectiveTimeToFilter: moment.Moment;
    maxTaxFilter: number;
    maxTaxFilterEmpty: number;
    minTaxFilter: number;
    minTaxFilterEmpty: number;
    imageUriFilter = "";
    priorityFilter = 0;
    Rtype: RType[] = null;

    formDataFile: any;
    fileToUpload: any;
    url: string | ArrayBuffer;
    ff: any;
    fromFileUplode: any;
    categoryIndex: any;
    itemIndex: any;
    menuForm: any;

    imagSrc: string;
    imagBGSrc: string;
    menu: MenuDto = new MenuDto();

    regularMenu: MenuTypeEnum = MenuTypeEnum.Regular;
    advanceMenu: MenuTypeEnum = MenuTypeEnum.Advance;

    showLogo = false;
    showBg = false;
    containWithinAspectRatio = false;
    whatsAppHeaderHandle: WhatsAppHeaderUrl = new WhatsAppHeaderUrl();
    videoLink = "../../../assets/Loyality.mp4";
    @ViewChild("file")
    element: ElementRef;

    @ViewChild("file2")
    element2: ElementRef;

    constructor(
        injector: Injector,
        private _itemsServiceProxy: ItemsServiceProxy,
        private _appSessionService: AppSessionService,
        private teamService: TeamInboxService,
        private _menusServiceProxy: MenusServiceProxy,
        private _fileDownloadService: FileDownloadService,
        public darkModeService: DarkModeService,
        private router: Router,
        private modalService: NgbModal,
        http: HttpClient
    ) {
        super(injector);
        this.http = http;
    }
    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.imagSrc = this._appSessionService.tenant.image;
        this.imagBGSrc = this._appSessionService.tenant.imageBg;
        console.log(this._appSessionService.tenant.imageBg);
        await this.getIsAdmin();
        // this.initForm();
    }
    // initForm() {
    //     this.chatForm = new FormGroup({
    //         text: new FormControl("", [Validators.required]),
    //         formFile: new FormControl("", [Validators.required]),
    //     });
    // }
    getMenus(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._menusServiceProxy
            .getMenus(
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(
                    this.paginator,
                    event
                ),
                null
            )
            .subscribe((result) => {
                //this.menu= result.lstMenu;

                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.lstMenu;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createMenu(): void {
        this.createOrEditMenuModal.show();
    }
    createAdvancedMenu(): void {
        this.createOrEditAdvancedMenuModal.show();
    }

    deleteMenu(menu: MenuDto): void {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._menusServiceProxy.delete(menu.id).subscribe((result) => {
                    if (result) {
                        this.reloadPage();
                        this.notify.success(this.l("successfullyDeleted"));
                    } else {
                        this.notify.error(this.l("menuDeleteRejection"));
                    }
                });
            }
        });
    }

    exportToExcel(): void {
        this._menusServiceProxy
            .getMenusToExcel(
                this.filterText,
                this.menuNameFilter,
                this.menuDescriptionFilter,
                this.maxEffectiveTimeFromFilter === undefined
                    ? this.maxEffectiveTimeFromFilter
                    : moment(this.maxEffectiveTimeFromFilter).endOf("day"),
                this.minEffectiveTimeFromFilter === undefined
                    ? this.minEffectiveTimeFromFilter
                    : moment(this.minEffectiveTimeFromFilter).startOf("day"),
                this.maxEffectiveTimeToFilter === undefined
                    ? this.maxEffectiveTimeToFilter
                    : moment(this.maxEffectiveTimeToFilter).endOf("day"),
                this.minEffectiveTimeToFilter === undefined
                    ? this.minEffectiveTimeToFilter
                    : moment(this.minEffectiveTimeToFilter).startOf("day"),
                this.maxTaxFilter == null
                    ? this.maxTaxFilterEmpty
                    : this.maxTaxFilter,
                this.minTaxFilter == null
                    ? this.minTaxFilterEmpty
                    : this.minTaxFilter,
                this.imageUriFilter,
                this.priorityFilter
            )
            .subscribe((result) => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }
    handleFileInput(event) {
        this.fileToUpload = event.target.files[0];
    }

    uploadFileToActivity() {
        let FormFile = this.chatForm.get("formFile").value;

        let formDataFile = new FormData();

        formDataFile.append("formFile", this.fileToUpload);

        this.teamService.uploadExcelFile(formDataFile).subscribe((result) => {
            this.reloadPage();
            this.notify.success(this.l("successfullyUpload"));
        });
    }
    initFileUploader() {
        throw new Error("Method not implemented.");
    }

    onFileChange1(event, modalBasic) {
        if (event.target.files[0]) {
            if (
                event.target.files[0].type === "image/jpeg" ||
                event.target.files[0].type === "image/png" ||
                event.target.files[0].type === "image/jpg"
            ) {
                this.modalOpen(modalBasic);
                this.imageChangedEvent = event;
            } else {
                this.message.error("", this.l("youCantUploadThisFile"));
                this.element.nativeElement.value = "";
            }
        }
    }

    onFileChange2(event, modalBasic) {
        if (event.target.files[0]) {
            if (
                event.target.files[0].type === "image/jpeg" ||
                event.target.files[0].type === "image/png" ||
                event.target.files[0].type === "image/jpg"
            ) {
                this.modalOpen2(modalBasic);
                this.imageChangedEvent = event;
            } else {
                this.message.error("", this.l("youCantUploadThisFile"));
                this.element2.nativeElement.value = "";
            }
        }
    }

    openFileUploder1() {
        //this.fromFileUplode = true;
        document.getElementById("uplode1").click();
    }

    openFileUploder2() {
        // this.categoryIndex = category;
        // this.itemIndex = index;
        // this.fromFileUplode = true;
        document.getElementById("uplode2").click();
    }

    copyMenu(menu: MenuDto) {
        this._menusServiceProxy.copyeMenu(menu.id).subscribe((res2) => {
            this.reloadPage();
        });
    }
    showDropDown() {
        if (this.showDropDownList) {
            this.showDropDownList = false;
        } else {
            this.showDropDownList = true;
        }
    }

    goToAdvancedMenu() {
        this.router.navigate(["/app/main/CreateOrEditAdvancedMenu"]);
    }
    goToCreateMenu(id: string) {
        this.router.navigate(["/app/main/CreateOrEditMenu" + id]);
    }

    createMenuu() {
        this.router.navigate(["/app/main/CreateOrEditMenu"]);
    }
    editMenu(id: string) {
        this.router.navigate(["/app/main/CreateOrEditMenu"], {
            queryParams: { id },
        });
    }

    createAdvancedMenuu() {
        this.router.navigate(["/app/main/CreateOrEditAdvancedMenu"]);
    }
    editAdvancedMenu(menu: any) {
        this.router.navigate(["/app/main/CreateOrEditAdvancedMenu"], {
            queryParams: menu,
        });
    }

    imageCroppedFile(event: ImageCroppedEvent) {
        // Determine MIME type from base64 string
        const mimeType = this.extractMimeType(event.base64);
        const filename =
            "cropped-image"+ `${new Date().getTime()}` + this.getExtensionFromMimeType(mimeType);

        // Convert base64 string to File object
        const file = this.base64ToFile(event.base64, filename, mimeType);

        // Create FormData object and append the file
        const form = new FormData();
        form.append("FormFile", file);

        // Store the FormData object
        this.file = form;
    }

    imageCroppedFile2(event: ImageCroppedEvent) {
        // Validate image dimensions
        if (event.height < 300 || event.width < 987) {
            this.message.error("", this.l("menuImageRestriction"));
            this.modalClose2();
            return;
        }

        // Determine MIME type from base64 string
        const mimeType = this.extractMimeType(event.base64);
        const filename =
            "cropped-image"+ `${new Date().getTime()}` + this.getExtensionFromMimeType(mimeType);

        // Convert base64 string to File object
        const file = this.base64ToFile(event.base64, filename, mimeType);

        // Create FormData object and append the file
        const form = new FormData();
        form.append("FormFile", file);

        // Store the FormData object
        this.file2 = form;
    }

    saveImage() {
        this.loadLogoImage = true;
        console.log(this.file);
        this.http
            .post<WhatsAppHeaderUrl>(
                AppConsts.remoteServiceBaseUrl +
                    "/api/services/app/General/GetInfoSeedUrlFile",
                this.file
            )
            .subscribe((result) => {
                this.imagSrc = `${
                    result.infoSeedUrl
                }?v=${new Date().getTime()}`;
                this._menusServiceProxy
                    .updateMenuImages(this.imagSrc, this.imagBGSrc)
                    .subscribe(
                        (res2) => {
                            this.loadLogoImage = false;
                            this.fromFileUplode = false;
                            this.categoryIndex = -1;
                            this.itemIndex = -1;
                        },
                        (error: any) => {
                            if (error) {
                                this.loadBgImage = false;
                            }
                        }
                    );
            });
    }

    saveImage2() {
        this.loadBgImage = true;
        console.log(this.file2.file);
        this.http
            .post<WhatsAppHeaderUrl>(
                AppConsts.remoteServiceBaseUrl +
                    "/api/services/app/General/GetInfoSeedUrlFile",
                this.file2
            )
            .subscribe((result) => {
                this.imagBGSrc = `${
                    result.infoSeedUrl
                }?v=${new Date().getTime()}`;
                console.log(result.infoSeedUrl);
                this._menusServiceProxy
                    .updateMenuImages(this.imagSrc, this.imagBGSrc)
                    .subscribe(
                        (res2) => {
                            this.fromFileUplode = false;
                            this.loadBgImage = false;
                            this.categoryIndex = -1;
                            this.itemIndex = -1;
                        },
                        (error: any) => {
                            if (error) {
                                this.loadBgImage = false;
                            }
                        }
                    );
            });
    }

    GetMenuImageUrl(file: any) {
        this.http
            .post<WhatsAppHeaderUrl>(
                AppConsts.remoteServiceBaseUrl +
                    "/api/services/app/General/GetInfoSeedUrlFile",
                file
            )
            .subscribe((result) => {
                this.whatsAppHeaderHandle = new WhatsAppHeaderUrl();
                this.whatsAppHeaderHandle = result;
                this.imagSrc = `${
                    result.infoSeedUrl
                }?v=${new Date().getTime()}`;
            });
    }

    GetMenuImageBGUrl(file: any) {
        this.http
            .post<WhatsAppHeaderUrl>(
                AppConsts.remoteServiceBaseUrl +
                    "/api/services/app/General/GetInfoSeedUrlFile",
                file
            )
            .subscribe((result) => {
                this.whatsAppHeaderHandle = new WhatsAppHeaderUrl();
                this.whatsAppHeaderHandle = result;
                this.imagBGSrc = `${
                    result.infoSeedUrl
                }?v=${new Date().getTime()}`;
            });
    }

    modalOpen(modalBasic) {
        this.modalService.open(modalBasic, {
            windowClass: "modal",
        });
    }

    modalOpen2(modalBasic) {
        this.modalService.open(modalBasic, {
            windowClass: "modal",
        });
    }

    modalClose2() {
        this.modalService.dismissAll();
    }

    handleRtypeFromchild(value: RType[]) {
        this.Rtype = value;
    }
}
