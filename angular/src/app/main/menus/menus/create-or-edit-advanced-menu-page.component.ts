import {
    Component,
    Injector,
    Output,
    EventEmitter,
    OnInit,
    ElementRef,
    ViewChild,
} from "@angular/core";
import {
    MenusServiceProxy,
    CreateOrEditMenuDto,
    ItemsServiceProxy,
    RestaurantsTypeEunm,
    RType,
    TenantTypeEunm,
    MenuTypeEnum,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import * as moment from "moment";
import { AppSessionService } from "@shared/common/session/app-session.service";
import { DarkModeService } from "./../../../services/dark-mode.service";
import { ActivatedRoute, Router } from "@angular/router";
import { base64ToFile, ImageCroppedEvent } from "ngx-image-cropper";
import { NgbModal, NgbModalConfig } from "@ng-bootstrap/ng-bootstrap";
import { FileUploader } from "ng2-file-upload";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { catchError } from "rxjs/operators";
declare var ImageCompressor: any;

@Component({
    selector: "app-create-or-edit-advanced-menu-page",
    templateUrl: "./create-or-edit-advanced-menu-page.component.html",
})
export class CreateOrEditAdvancedMenuPageComponent
    extends AppComponentBase
    implements OnInit
{
    theme: string;
    menuu: any;
    public useGravatarProfilePicture = false;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild("filee")
    filee: ElementRef;

    active = false;
    saving = false;
    isEdit: boolean;
    menu1: CreateOrEditMenuDto = new CreateOrEditMenuDto();
    menuTypeEnum: MenuTypeEnum;

    selectedValue: any;
    file: any;
    rType: RType[];
    menuType: number;
    priorityMenu: number;
    isNf: boolean;
    menuImage:string;
    isMall: boolean;
    dropdownSettings = {};
    selectedAreaIds: Array<any> = [];

    menuDescription: string;
    menuName: string;
    menuNameEnglish: string;
    menuDescriptionEnglish: string;
    effectiveTimeFrom: moment.Moment;
    effectiveTimeTo: moment.Moment;
    tax!: number;
    imageUri: string;
    priority: number;
    restaurantsType: RestaurantsTypeEunm;
    languageBotId: number;
    imageBgUri: string;
    menuTypeId: MenuTypeEnum;
    areaIds: string;
    id: number;
    imageChangedEvent: any = "";
    public uploader: FileUploader;
    submitted = false;

    constructor(
        private activeRoute: ActivatedRoute,
        public darkModeService: DarkModeService,
        injector: Injector,
        private _menusServiceProxy: MenusServiceProxy,
        private _itemsServiceProxy: ItemsServiceProxy,
        private _appSessionService: AppSessionService,
        private modalService: NgbModal,
        config: NgbModalConfig,
        private router: Router
    ) // private _itemAdditionServiceProxy:ItemAdditionServiceProxy,
    {
        super(injector);
        config.backdrop = "static";
        config.keyboard = false;
        // set default value
    }

    public get selectedCategoryType(): RestaurantsTypeEunm {
        return this.selectedValue ? this.selectedValue.value : null;
    }
    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();

        this.dropdownSettings = {
            singleSelection: false,
            idField: "id",
            textField: "name",
            itemsShowLimit: 3,
            allowSearchFilter: false,
            maxHeight: 200,
            closeDropDownOnSelection: true,
        };

        await this.getIsAdmin()
        this._menusServiceProxy
            .getRType(this.appSession.tenantId)
            .subscribe((result) => {
                this.rType = result.filter((x) => x.id != 0);
            });

        // this.isAdmin=this.appSession.user.role === "admin";
        // this._menusServiceProxy.getRType(this.appSession.tenantId).subscribe(result => {
        //     this.rType = result.filter(x => x.id != 0)
        // })

        if (this.activeRoute.snapshot.queryParams.id != null) {
            this.isAdmin = this.appSession.user.role === "admin";
            this._menusServiceProxy
                .getRType(this.appSession.tenantId)
                .subscribe((result) => {
                    this.rType = result.filter((x) => x.id != 0);

                    this.menuu = this.activeRoute.snapshot.queryParams;
                    this.showEdit(this.menuu);
                });
        }
    }

    show(): void {
        this.saving = false;
        this.isEdit = false;
        this.menu1 = new CreateOrEditMenuDto();
        this.selectedAreaIds = [];
        if (this._appSessionService.tenant.tenantType == TenantTypeEunm.Mall) {
            this.isMall = true;
        } else {
            this.isMall = false;
        }
        this.menuImage = "";
        this.active = true;
    }

    close(): void {
        this.active = false;
    }

    save() {
        this.saving = true;
        if (
            this.menuName === null ||
            this.menuName === undefined ||
            this.menuName === "" ||
            this.menuNameEnglish === null ||
            this.menuNameEnglish === undefined ||
            this.menuNameEnglish === ""
        ) {
            {
                this.submitted = true;
                this.saving = false;
                return;
            }
        }
        var Menu2 = new CreateOrEditMenuDto();

        Menu2.areaIds = this.selectedAreaIds
            .filter((f) => f.id > 0)
            .map(({ id }) => id)
            .toString();

        Menu2.menuName = this.menuName;
        Menu2.menuDescription = this.menuDescription; //this.menu1.menuDescription;
        Menu2.menuNameEnglish = this.menuNameEnglish;
        Menu2.menuDescriptionEnglish = this.menuDescriptionEnglish;
        Menu2.effectiveTimeFrom = this.effectiveTimeFrom;
        Menu2.effectiveTimeTo = this.effectiveTimeTo;
        Menu2.tax = this.tax;
        Menu2.imageUri = this.menuImage;
        Menu2.priority = this.priority;
        Menu2.restaurantsType = this.restaurantsType;
        Menu2.languageBotId = 1;
        Menu2.imageBgUri = this.imageBgUri;
        Menu2.menuTypeId = MenuTypeEnum.Advance;
        Menu2.restaurantsType = 0;

        Menu2.id = this.id;

        this._menusServiceProxy.createOrEdit(Menu2).subscribe(
            (response) => {
                this.notify.info(this.l("savedSuccessfully"));
                this.saving = false;
                this.router.navigate(["/app/main/menus/menus"]);
            },
            (error: any) => {
                if (error) {
                    this.saving = false;
                    this.submitted = false;
                }
            }
        );
    }
    showEdit(item: CreateOrEditMenuDto): void {
        this.saving = false;
        this.isEdit = true;

        this.selectedAreaIds = [];
        if (item.areaIds != undefined) {
            var array = item.areaIds.split(",");
            array.forEach((element) => {
                var branch = this.rType.find((x) => x.id == parseInt(element));
                if (branch != undefined) {
                    this.selectedAreaIds.push(branch);
                }
            });
        }
        this.menu1 = new CreateOrEditMenuDto();
        this.menu1 = item;

        this.menuDescription = this.menu1.menuDescription;
        this.menuDescriptionEnglish = this.menu1.menuDescriptionEnglish;
        this.menuName = this.menu1.menuName;
        this.menuNameEnglish = this.menu1.menuNameEnglish;
        this.priority = this.menu1.priority;
        this.restaurantsType = this.menu1.restaurantsType;
        this.imageBgUri = this.menu1.imageBgUri;
        this.id = this.menu1.id;

        this.active = true;
        this.menuImage = item.imageUri;
    }

    openFileUploaded() {
        document.getElementById("upload").click();
    }

    onFileChange(event, modalBasic) {
        if (event.target.files[0]) {
            if (event.target.files[0].type === "image/svg+xml") {
                this.message.error("", this.l("cantYploadSvgImage"));
                this.filee.nativeElement.value = "";
            } else {
                this.modalOpen(modalBasic);
                this.imageChangedEvent = event;
            }
        }
    }

    imageCroppedFile(event: ImageCroppedEvent) {
        const reader = new FileReader();
        const [file] = [<File>base64ToFile(event.base64)];
        reader.readAsDataURL(file);
        let form = new FormData();
        form.append("FormFile", file);
        this.file = form;
    }

    saveImage(image) {
        this._itemsServiceProxy.getImageUrl(this.file).subscribe(
            (res) => {
                this.menuImage = res["result"];
            },
            (error: any) => {
                if (error) {
                    this.saving = false;
                    this.submitted = false;
                }
            }
        );
    }

    back() {
        this.router.navigate(["/app/main/menus/menus"]);
    }
    goToDashboard() {
        this.router.navigate(["/app/main/dashboard"]);
    }

    modalOpen(modalBasic) {
        this.modalService.open(modalBasic, {
            windowClass: "modal",
        });
    }
}
