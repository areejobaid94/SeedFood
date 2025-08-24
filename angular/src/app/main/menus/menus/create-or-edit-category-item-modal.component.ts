import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
    ElementRef,
    Input,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs/operators";
import {
    AdditionsCategorysListModel,
    CreateOrEditItemAndAdditionsCategoryDto,
    CreateOrEditItemDto,
    CreateOrEditMenuCategoryDto,
    CreateOrEditMenuSubCategoryDto,
    GetSpecificationsCategorysModel,
    ItemDto,
    ItemImagesModel,
    ItemSpecification,
    ItemSpecificationsDto,
    ItemsServiceProxy,
    LoyaltyServiceProxy,
    MenuServiceProxy,
    MenusServiceProxy,
    RType,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    UntypedFormArray,
    UntypedFormBuilder,
    Validators,
} from "@angular/forms";
import { specificationLookupTableModalComponent } from "./specification-lookup-table-modal.component";
import { addOnsLookupTableModalComponent } from "./addOns-lookup-table-modal.component";
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "@app/services/dark-mode.service";
import { FileUploader } from "ng2-file-upload";
import { base64ToFile, ImageCroppedEvent } from "ngx-image-cropper";
import { NgbModal, NgbModalConfig } from "@ng-bootstrap/ng-bootstrap";
import { SwiperConfigInterface } from "ngx-swiper-wrapper";

@Component({
    selector: "createOrEditCategoryItemModalComponent",
    templateUrl: "./create-or-edit-category-item-modal.component.html",
})
export class CreateOrEditCategoryItemComponent extends AppComponentBase {
    theme: string;
    currency = "";
    imageChangedEvent: any = "";
    public uploader: FileUploader;
    file: any;
    loadImage = false;
    @ViewChild("filee", { static: false }) filee: ElementRef;
    @ViewChild("createOrEditCategoryItemModalComponent", { static: true })
    modal: ModalDirective;
    @ViewChild("specificationLookupTableModal", { static: true })
    specificationLookupTableModal: specificationLookupTableModalComponent;
    @ViewChild("addOnsLookupTableModal", { static: true })
    addOnsLookupTableModal: addOnsLookupTableModalComponent;
    @Input() isTenantLoyal: boolean;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    isImageUpload = false;
    TypeId: number;
    imagSrc: string;
    cat: any;
    ind: any;
    add: any;
    sub: any;
    subCategoryName: string;
    categoryName: string;
    rType: RType[];
    additionsCategorysListModel: AdditionsCategorysListModel[];
    SpecificationListModel: GetSpecificationsCategorysModel[];
    item: ItemDto = new ItemDto();
    fromFileUplode: any;
    categoryIndex: any;
    itemIndex: any;
    menuForm: any;
    controlSpecification: any[];
    controlAdditions: any[];
    dropdownSettings = {};
    selectedAreaIds: Array<any> = [];
    selectedInServiceIds: Array<any> = [];
    submitted = false;
    imageDto!: ItemImagesModel | undefined;
    lstImagestDto: Array<ItemImagesModel> = [];
    isMainImage = false;
    menuId: number;
    category: CreateOrEditMenuCategoryDto[];
    subcategory: CreateOrEditMenuSubCategoryDto[];

    public swiperResponsive: SwiperConfigInterface = {
        slidesPerView: 3,
        spaceBetween: 50,
        navigation: {
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
        },
        breakpoints: {
            1024: {
                slidesPerView: 3,
                spaceBetween: 40,
            },
            768: {
                slidesPerView: 3,
                spaceBetween: 30,
            },
            640: {
                slidesPerView: 2,
                spaceBetween: 20,
            },
            320: {
                slidesPerView: 1,
                spaceBetween: 10,
            },
        },
    };
    constructor(
        injector: Injector,
        private _itemsServiceProxy: ItemsServiceProxy,
        private _menusServiceProxy: MenusServiceProxy,
        private _loyaltyServiceProxy: LoyaltyServiceProxy,
        private fb: UntypedFormBuilder,
        public darkModeService: DarkModeService,
        private modalService: NgbModal,
        config: NgbModalConfig,
        private _MenuServiceProxy: MenuServiceProxy
    ) {
        super(injector);
        config.backdrop = "static";
        config.keyboard = false;
    }

    async ngOnInit() {
        this.checkIsLoyality();
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
        await this.getIsAdmin();
        this.menuForm = this.fb.group({
            items: this.fb.array([this.initItem()]),
        });
    }

    checkIsLoyality() {
        this._loyaltyServiceProxy.isLoyalTenant().subscribe((result) => {
            this.isTenantLoyal = result;
        });
    }
    
    show(item: ItemDto): void {
        this.currency = this.appSession.tenant.currencyCode;
        this.menuId = item.menuId;
        this.filee.nativeElement.value = "";
        this._menusServiceProxy
            .getRType(this.appSession.tenantId)
            .subscribe((result) => {
                this.rType = result.filter((x) => x.id != 0);
                this.ngOnInit();
                // this.imagSrc = item.imageUri;
                this.item = item;
                this.active = true;
                this.modal.show();
                this.getCategories();
            });

        this._menusServiceProxy
            .getAdditionsCategorysList()
            .subscribe((result) => {
                this.additionsCategorysListModel = result;
            });
        this._menusServiceProxy
            .getSpecificationsCategorys()
            .subscribe((result) => {
                this.SpecificationListModel = result;
                // this.languageBot.shift();
            });
    }

    addNewItem(
        menuid: number,
        selectcatgory: CreateOrEditMenuCategoryDto,
        selectsubcatgory: CreateOrEditMenuSubCategoryDto
    ): void {
        this.currency = this.appSession.tenant.currencyCode;
        //this.imagSrc=item.imageUri;
        this.ngOnInit();
        this.filee.nativeElement.value = "";
        this._menusServiceProxy
            .getRType(this.appSession.tenantId)
            .subscribe((result) => {
                this.rType = result.filter((x) => x.id != 0);
            });
        this.filee.nativeElement.value = "";
        this.item = new ItemDto();
        // this.imagSrc = null;
        this.imageDto = new ItemImagesModel();
        this.active = true;
        this.item.menuId = menuid;
        this.modal.show();
        this.menuId = menuid;
        this.category = [new CreateOrEditMenuCategoryDto()];
        this.subcategory = [new CreateOrEditMenuSubCategoryDto()];
        this.getCategories();
        this.subCategoryName = selectsubcatgory.name;
        this.categoryName = selectcatgory.name;
        this.controlSpecification = new Array();
        this.controlAdditions = new Array();
        this.item.itemCategoryId = selectcatgory.id;
        this.item.itemSubCategoryId = selectsubcatgory.id;
        this.selectedAreaIds = [];
        this.selectedInServiceIds = [];
        this.lstImagestDto = [];
        this.item.isLoyal = false;
    }

    editNewItem(itemId: number): void {
        this.currency = this.appSession.tenant.currencyCode;
        this.deleteitemAddition;
        this.filee.nativeElement.value = "";
        this.ngOnInit();
        this._menusServiceProxy
            .getRType(this.appSession.tenantId)
            .subscribe((result) => {
                this.rType = result.filter((x) => x.id != 0);
                this._itemsServiceProxy
                    .getItemById(itemId, null, true)
                    .subscribe((res) => {
                        this.item = new ItemDto();
                        this.lstImagestDto = [];
                        // this.imagSrc = '';
                        this.item = res;
                        this.lstImagestDto = res.lstItemImages;
                        this.menuId = res.menuId;
                        this.category = [new CreateOrEditMenuCategoryDto()];
                        this.subcategory = [
                            new CreateOrEditMenuSubCategoryDto(),
                        ];
                        this.getCategories();
                        // this.imagSrc = res.imageUri;
                        this.subCategoryName = this.item.subCategoryName;
                        this.selectedAreaIds = [];
                        this.selectedInServiceIds = [];
                        if (this.item.areaIds != undefined) {
                            var array = this.item.areaIds.split(",");
                            array.forEach((element) => {
                                var branch = this.rType.find(
                                    (x) => x.id == parseInt(element)
                                );
                                if (branch != undefined) {
                                    this.selectedAreaIds.push(branch);
                                }
                            });
                        }
                        if (this.item.inServiceIds != undefined) {
                            var array = this.item.inServiceIds.split(",");
                            array.forEach((element) => {
                                var branch = this.rType.find(
                                    (x) => x.id == parseInt(element)
                                );
                                if (branch != undefined) {
                                    this.selectedInServiceIds.push(branch);
                                }
                            });
                        }
                        // this.imagSrc = this.item.imageUri;
                        this.categoryName = this.subCategoryName =
                            this.item.categoryNames;
                        this.subCategoryName = "";
                        this.subCategoryName = this.item.subCategoryName;
                        this.active = true;
                        this.controlSpecification =
                            new Array<ItemSpecification>();
                        if (this.item.itemSpecifications != undefined) {
                            this.controlSpecification = (
                                (<UntypedFormArray>(
                                    this.menuForm.controls["items"]
                                ))
                                    .at(0)
                                    .get(
                                        "SpecificationModels"
                                    ) as UntypedFormArray
                            ).controls;
                            this.item.itemSpecifications.forEach((element) => {
                                this.controlSpecification.push(
                                    this.bindSpecification(element)
                                );
                            });
                        }
                        this.controlAdditions =
                            new Array<AdditionsCategorysListModel>();
                        if (this.item.itemAdditionDtos != undefined) {
                            this.controlAdditions = (
                                (<UntypedFormArray>(
                                    this.menuForm.controls["items"]
                                ))
                                    .at(0)
                                    .get(
                                        "additionsCategorysListModels"
                                    ) as UntypedFormArray
                            ).controls;
                            this.item.additionsCategorysListModels.forEach(
                                (element) => {
                                    this.controlAdditions.push(
                                        this.bindAdditionsCategoryList(element)
                                    );
                                }
                            );
                        }
                        //         if(this.item.lstItemImages){
                        //             for(let i = 0 ; i <= this.item.lstItemImages.length ; i++){
                        //    if(this.item.lstItemImages[i].isMainImage){
                        //        const radio=<HTMLInputElement | null> document.getElementById("Select As Main Image" + (i));
                        //      radio.checked= true;
                        //      }else {
                        //          const radio=<HTMLInputElement | null> document.getElementById("Select As Main Image" + (i));
                        //          radio.checked= false;
                        //      }
                        //  }
                        //  }
                    });
                this.filee.nativeElement.value = "";
                this.modal.show();
            });
    }

    save(): void {
        this.saving = true;
        if (
            this.selectedAreaIds === null ||
            this.selectedAreaIds === undefined ||
            this.selectedAreaIds.length === 0 ||
            this.item.itemName === null ||
            this.item.itemName === undefined ||
            this.item.itemName === "" ||
            this.item.itemNameEnglish === null ||
            this.item.itemNameEnglish === undefined ||
            this.item.itemNameEnglish === "" ||
            this.item.price === null ||
            this.item.price === undefined ||
            !this.lstImagestDto ||
            this.item.itemCategoryId === null ||
            this.item.itemCategoryId === undefined ||
            this.item.itemSubCategoryId === null ||
            this.item.itemSubCategoryId === undefined
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        if (this.item.isLoyal) {
            if (
                this.item.loyaltyPoints === null ||
                this.item.loyaltyPoints === undefined
            ) {
                this.submitted = true;
                this.saving = false;
                return;
            }
        }

        this.submitted = false;
        if (!this.isImageUpload) {
            this.saving = true;

            // this.item.imageUri = this.imagSrc;
            this.fillItemAdditionDto();
            this.fillItemSpecificationsDto();
            this.item.areaIds = this.selectedAreaIds
                .filter((f) => f.id > 0)
                .map(({ id }) => id)
                .toString();
            this.item.inServiceIds = this.selectedInServiceIds
                .filter((f) => f.id > 0)
                .map(({ id }) => id)
                .toString();
            let ss = new CreateOrEditItemDto();
            ss.areaIds = this.item.areaIds;
            ss.inServiceIds = this.item.inServiceIds;
            //ss.creatorUserId=this.item.creatorUserId;
            ss.ingredients = this.item.ingredients;
            if (this.item.ingredients == null) {
                ss.itemDiscount = 0;
            } else {
                ss.itemDiscount = Number(this.item.ingredients);
            }

            ss.itemName = this.item.itemName;
            ss.itemDescription = this.item.itemDescription;
            ss.itemNameEnglish = this.item.itemNameEnglish;
            ss.itemDescriptionEnglish = this.item.itemDescriptionEnglish;
            ss.categoryNames = this.item.categoryNames;
            ss.categoryNamesEnglish = this.item.categoryNamesEnglish;
            ss.isInService = this.item.isInService;
            ss.creationTime = this.item.creationTime;
            ss.deletionTime = this.item.deletionTime;
            ss.lastModificationTime = this.item.lastModificationTime;
            ss.price = this.item.price;
            ss.oldPrice = this.item.oldPrice;
            ss.itemCategoryId = this.item.itemCategoryId;
            ss.itemSubCategoryId = this.item.itemSubCategoryId;
            ss.imageUri = this.item.imageUri;
            ss.priority = this.item.priority;
            ss.menuId = this.item.menuId;
            ss.itemCategoryId = this.item.itemCategoryId;
            ss.itemSubCategoryId = this.item.itemSubCategoryId;
            ss.sku = this.item.sku;
            ss.itemAdditionDtos = this.item.itemAdditionDtos;
            ss.menuType = this.item.menuType;
            ss.languageBotId = this.item.languageBotId;
            ss.size = this.item.size;
            ss.barcode = this.item.barcode;
            ss.tenantId = this.item.tenantId;
            ss.lstItemAndAdditionsCategoryDto =
                this.item.lstItemAndAdditionsCategoryDto;
            ss.lstItemSpecificationsDto = this.item.lstItemSpecificationsDto;
            ss.areaIds = this.item.areaIds;
            ss.inServiceIds = this.item.inServiceIds;
            ss.isQuantitative = this.item.isQuantitative;
            ss.isLoyal = this.item.isLoyal;
            ss.loyaltyPoints = this.item.loyaltyPoints;
            ss.originalLoyaltyPoints = this.item.originalLoyaltyPoints;
            ss.isOverrideLoyaltyPoints = this.item.isOverrideLoyaltyPoints;
            ss.loyaltyDefinitionId = this.item.loyaltyDefinitionId;
            ss.id = this.item.id;
            ss.lstItemImages = this.item.lstItemImages;
            var found = false;
            for (var i = 0; i < ss.lstItemImages.length; i++) {
                if (ss.lstItemImages[i].isMainImage == true) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                this.message.error("", this.l("youHaveToChooseTheMainImage"));
                this.saving = false;
                this.submitted = false;
                return;
            }
            this._itemsServiceProxy
                .createOrEditItems(ss)
                .pipe(
                    finalize(() => {
                        this.saving = false;
                    })
                )
                .subscribe(
                    () => {
                        this.notify.info(this.l("savedSuccessfully"));
                        this.saving = false;
                        this.close();
                        this.modalSave.emit(null);
                    },
                    (error: any) => {
                        if (error) {
                            this.saving = false;
                            this.submitted = false;
                        }
                    }
                );
        }
    }

    save2(): void {
        this.saving = true;
        if (
            this.selectedAreaIds === null ||
            this.selectedAreaIds === undefined ||
            this.selectedAreaIds.length === 0 ||
            this.item.itemName === null ||
            this.item.itemName === undefined ||
            this.item.itemName === "" ||
            this.item.itemNameEnglish === null ||
            this.item.itemNameEnglish === undefined ||
            this.item.itemNameEnglish === "" ||
            this.item.price === null ||
            this.item.price === undefined ||
            this.lstImagestDto.length === 0 ||
            this.item.itemCategoryId === null ||
            this.item.itemCategoryId === undefined ||
            this.item.itemSubCategoryId === null ||
            this.item.itemSubCategoryId === undefined
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }

        if (this.item.isLoyal) {
            if (
                this.item.loyaltyPoints === null ||
                this.item.loyaltyPoints === undefined
            ) {
                this.submitted = true;
                this.saving = false;
                return;
            }
        }
        this.submitted = false;

        // this.item.imageUri = this.imagSrc;
        this.fillItemAdditionDto();
        this.fillItemSpecificationsDto();
        this.item.areaIds = this.selectedAreaIds
            .filter((f) => f.id > 0)
            .map(({ id }) => id)
            .toString();
        this.item.inServiceIds = this.selectedInServiceIds
            .filter((f) => f.id > 0)
            .map(({ id }) => id)
            .toString();
        let ss = new CreateOrEditItemDto();
        ss.areaIds = this.item.areaIds;
        ss.inServiceIds = this.item.inServiceIds;
        ss.ingredients = this.item.ingredients;
        if (this.item.ingredients == null) {
            ss.itemDiscount = 0;
        } else {
            ss.itemDiscount = Number(this.item.ingredients);
        }
        ss.itemName = this.item.itemName;
        ss.itemDescription = this.item.itemDescription;
        ss.itemNameEnglish = this.item.itemNameEnglish;
        ss.itemDescriptionEnglish = this.item.itemDescriptionEnglish;
        ss.categoryNames = this.item.categoryNames;
        ss.categoryNamesEnglish = this.item.categoryNamesEnglish;
        ss.isInService = this.item.isInService;
        ss.creationTime = this.item.creationTime;
        ss.deletionTime = this.item.deletionTime;
        ss.lastModificationTime = this.item.lastModificationTime;
        ss.price = this.item.price;
        ss.oldPrice = this.item.oldPrice;
        ss.imageUri = this.item.imageUri;
        ss.priority = this.item.priority;
        ss.menuId = this.item.menuId;
        ss.itemCategoryId = this.item.itemCategoryId;
        ss.itemSubCategoryId = this.item.itemSubCategoryId;
        ss.sku = this.item.sku;
        ss.itemAdditionDtos = this.item.itemAdditionDtos;
        ss.menuType = this.item.menuType;
        ss.itemCategoryId = this.item.itemCategoryId;
        ss.itemSubCategoryId = this.item.itemSubCategoryId;
        ss.languageBotId = this.item.languageBotId;
        ss.size = this.item.size;
        ss.barcode = this.item.barcode;
        ss.tenantId = this.item.tenantId;
        ss.lstItemAndAdditionsCategoryDto =
            this.item.lstItemAndAdditionsCategoryDto;
        ss.lstItemSpecificationsDto = this.item.lstItemSpecificationsDto;
        ss.areaIds = this.item.areaIds;
        ss.inServiceIds = this.item.inServiceIds;
        ss.isQuantitative = this.item.isQuantitative;
        ss.isLoyal = this.item.isLoyal;
        ss.loyaltyPoints = this.item.loyaltyPoints;
        ss.originalLoyaltyPoints = this.item.originalLoyaltyPoints;
        ss.isOverrideLoyaltyPoints = this.item.isOverrideLoyaltyPoints;
        ss.loyaltyDefinitionId = this.item.loyaltyDefinitionId;
        ss.id = this.item.id;
        ss.lstItemImages = this.item.lstItemImages;

        var found = false;
        for (var i = 0; i < ss.lstItemImages.length; i++) {
            if (ss.lstItemImages[i].isMainImage == true) {
                found = true;
                break;
            }
        }
        if (!found) {
            this.message.error("", this.l("youHaveToChooseTheMainImage"));
            this.saving = false;
            this.submitted = false;
            return;
        }
        this._itemsServiceProxy
            .createOrEditItems(ss)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
            .subscribe(
                () => {
                    this.notify.info(this.l("savedSuccessfully"));
                    this.saving = false;
                    this.close();
                },
                (error: any) => {
                    if (error) {
                        this.saving = false;
                        this.submitted = false;
                    }
                }
            );
    }
    ngAfterViewInit(): void {
        if (this.filee !== undefined) {
            this.filee.nativeElement.focus();
        }
    }

    close(): void {
        this.active = false;
        this.submitted = false;
        this.saving = false;
        // (<HTMLInputElement>document.getElementById("filee")).value = null;
        this.modal.hide();
        this.filee.nativeElement.value = "";
        this.modalSave.emit(null);
        this.loadImage = false;
    }

    DeleteItem(idx) {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this.lstImagestDto.splice(idx, 1);
                this.item.lstItemImages = this.lstImagestDto;
            }
        });
    }

    fillItemAdditionDto() {
        if (
            this.controlAdditions != undefined &&
            this.controlAdditions.length > 0
        ) {
            this.item.lstItemAndAdditionsCategoryDto =
                Array<CreateOrEditItemAndAdditionsCategoryDto>();
            this.controlAdditions.forEach((element) => {
                let addAnditem = new CreateOrEditItemAndAdditionsCategoryDto();
                if (element.controls.id.value != 0) {
                    addAnditem.id = 0;
                    addAnditem.itemId = 0;
                    addAnditem.menuType = 0;
                    addAnditem.tenantId = this.appSession.tenantId;
                    addAnditem.specificationId = 0;
                    addAnditem.additionsCategorysId = element.controls.id.value;
                    addAnditem.additionsAndItemId = element.controls.id.value;
                    this.item.lstItemAndAdditionsCategoryDto.push(addAnditem);
                }
            });
        }
    }

    fillItemSpecificationsDto() {
        if (
            this.controlSpecification != undefined &&
            this.controlSpecification.length > 0
        ) {
            this.item.lstItemSpecificationsDto = Array<ItemSpecificationsDto>();
            this.controlSpecification.forEach((element) => {
                let specifications = new ItemSpecificationsDto();
                if (element.controls.id.value != 0) {
                    specifications.id = 0;
                    specifications.itemId = 0;
                    specifications.tenantId = this.appSession.tenantId;
                    specifications.specificationId = element.controls.id.value;
                    specifications.isRequired =
                        element.controls.isRequired.value;
                    this.item.lstItemSpecificationsDto.push(specifications);
                }
            });
        }
    }

    initItem() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            itemAdditionDtos: this.fb.array([
                // this.initAddition()
            ]),
            additionsCategorysListModels: this.fb.array([
                //  this.initAdditionsCategoryList()
            ]),
            SpecificationModels: this.fb.array([
                // this.initSpecification()
            ]),
            menuId: "",
            itemCategoryId: "",
        });
    }

    // onFileChange(event, modalBasic) {
    //     if ( event.target.files[0]) {
    //         if( event.target.files[0].type ==='image/svg+xml'){
    //             this.message.error("",this.l("You cant upload SVG image!"));
    //             this.filee.nativeElement.value = "";
    //         }else{
    //         this.modalOpen(modalBasic);
    //         this.imageChangedEvent = event;
    //         }

    //     }
    // }

    modalOpen(modalBasic) {
        this.modalService.open(modalBasic, {
            windowClass: "modal",
        });
    }

    openFileUploder(item: ItemDto) {
        this.isImageUpload = true;
        document.getElementById("uplodeImage").click();
    }

    addItemAddition() {
        this.controlAdditions = (
            (<UntypedFormArray>this.menuForm.controls["items"])
                .at(0)
                .get("additionsCategorysListModels") as UntypedFormArray
        ).controls;
        this.controlAdditions.push(this.initAdditionsCategoryList());
    }

    addSpecification() {
        this.controlSpecification = (
            (<UntypedFormArray>this.menuForm.controls["items"])
                .at(0)
                .get("SpecificationModels") as UntypedFormArray
        ).controls;
        this.controlSpecification.push(this.initSpecification());
    }

    initAdditionsCategoryListSet(item) {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            id: item.id,
            name: [item.name, [Validators.required]],
            nameEnglish: [item.nameEnglish, [Validators.required]],
            additionsAndItemId: item.additionsAndItemId,
            // ---------------------------------------------------------------------
        });
    }
    initAdditionsCategoryList() {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            id: 0,
            name: ["", [Validators.required]],
            nameEnglish: ["", [Validators.required]],
            additionsAndItemId: 0,
            // ---------------------------------------------------------------------
        });
    }

    bindAdditionsCategoryList(objItemAdd: AdditionsCategorysListModel) {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            id: objItemAdd.id,
            name: [objItemAdd.name, [Validators.required]],
            nameEnglish: [objItemAdd.nameEnglish, [Validators.required]],
            additionsAndItemId: objItemAdd.additionsAndItemId,
            // ---------------------------------------------------------------------
        });
    }

    initSpecification() {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            id: 0,
            itemSpecificationId: [0, [Validators.required]],
            isMultipleSelection: [true, [Validators.required]],
            isRequired: [true, [Validators.required]],
            priority: [0, [Validators.required]],
            specificationDescription: ["", [Validators.required]],
            specificationDescriptionEnglish: ["", [Validators.required]],
            // ---------------------------------------------------------------------
        });
    }

    bindSpecification(obj: ItemSpecification) {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            id: obj.id,
            itemSpecificationId: [
                obj.itemSpecificationId,
                [Validators.required],
            ],
            isMultipleSelection: [
                obj.isMultipleSelection,
                [Validators.required],
            ],
            isRequired: [obj.isRequired, [Validators.required]],
            priority: [obj.priority, [Validators.required]],
            specificationDescription: [
                obj.specificationDescription,
                [Validators.required],
            ],
            specificationDescriptionEnglish: [
                obj.specificationDescriptionEnglish,
                [Validators.required],
            ],
            // ---------------------------------------------------------------------
        });
    }

    openSelectAdditionsCategoryModal(add) {
        this.cat = 1;
        this.ind = 1;
        this.sub = 1;
        this.add = add;
        this.addOnsLookupTableModal.show(this.additionsCategorysListModel);
    }

    openSelectSpecificationModal(add) {
        this.cat = 1;
        this.ind = 2;
        this.sub = 3;
        this.add = add;
        this.specificationLookupTableModal.show(this.SpecificationListModel);
    }

    deleteitemAddition(add) {
        const control = (<UntypedFormArray>this.menuForm.controls["items"])
            .at(0)
            .get("additionsCategorysListModels") as UntypedFormArray;
        let item = control.at(add).value;
        if (item.id) {
            this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
                if (isConfirmed) {
                    this._itemsServiceProxy
                        .deleteItemAndAdditionsCategorys(
                            item.additionsAndItemId
                        )
                        .subscribe(() => {
                            control.removeAt(add);
                        });
                }
            });
        } else {
            control.removeAt(add);
        }
    }

    deleteSpecification(add) {
        const control = (<UntypedFormArray>this.menuForm.controls["items"])
            .at(0)
            .get("SpecificationModels") as UntypedFormArray;
        let item = control.at(add).value;
        this.message.confirm("", this.l("AreYouSure"), (isConfirme) => {
            if (isConfirme) {
                control.removeAt(add);
            }
        });
    }

    UpdateAdditionsCategoryId() {
        if (!this.addOnsLookupTableModal.isCloseClick) {
            const control = (<UntypedFormArray>this.menuForm.controls["items"])
                .at(0)
                .get("additionsCategorysListModels") as UntypedFormArray;
            let item = control.at(this.add).value;
            item.AdditionsAndItemId =
                this.addOnsLookupTableModal.AdditionsCategorysid;
            item.id = this.addOnsLookupTableModal.id;
            item.name = this.addOnsLookupTableModal.displayName;
            item.nameEnglish = this.addOnsLookupTableModal.displayNameEnglish;
            control.push(this.initAdditionsCategoryListSet(item));
            control.removeAt(this.add);
        }
    }

    UpdateSpecificationId() {
        if (!this.specificationLookupTableModal.isCloseClick) {
            const control = (<UntypedFormArray>this.menuForm.controls["items"])
                .at(0)
                .get("SpecificationModels") as UntypedFormArray;
            let item = control.at(this.add).value;
            item.id = this.specificationLookupTableModal.id;
            item.name = this.specificationLookupTableModal.displayName;
            item.nameEnglish =
                this.specificationLookupTableModal.displayNameEnglish;
            item.isRequired = this.specificationLookupTableModal.isRequired;
            control.push(this.initSpecificationListSet(item));
            control.removeAt(this.add);
        }
    }
    initSpecificationListSet(item) {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            id: item.id,
            // 'itemSpecificationId':  [item.itemSpecificationId, [Validators.required]],
            isMultipleSelection: [true, [Validators.required]],
            isRequired: [item.isRequired, [Validators.required]],
            priority: [0, [Validators.required]],
            specificationDescription: [item.name, [Validators.required]],
            specificationDescriptionEnglish: [
                item.nameEnglish,
                [Validators.required],
            ],
            // ---------------------------------------------------------------------
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

    saveImage(image) {
        this.loadImage = true;
        this.imageDto = new ItemImagesModel();
        this._itemsServiceProxy.getImageUrl(this.file).subscribe((res) => {
            // this.imagSrc = res['result'];
            this.fromFileUplode = false;
            this.loadImage = false;
            this.imageDto.imageUrl = res["result"];

            this.imageDto.isMainImage = false;

            this.imageDto.tenantId = this.appSession.tenantId;
            if (!this.item.id) {
                this.imageDto.itemId = 0;
            } else {
                this.imageDto.itemId = this.item.id;
            }

            if (this.lstImagestDto == null) {
                this.lstImagestDto = [];
            }
            this.lstImagestDto.push(this.imageDto);
            this.item.lstItemImages = this.lstImagestDto;
            const index = this.item.lstItemImages.findIndex((r) => {
                if (r.id === this.imageDto.id) {
                    return this.imageDto.id;
                }
            });

            // const radio = <HTMLInputElement | null>(
            //     document.getElementById("Select As Main Image" + index)
            // );
            // radio.checked = false;

            // (<HTMLInputElement>document.getElementById("file")).value = null
        });
    }

    onFileChange(event, modalBasic) {
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
                this.filee.nativeElement.value = "";
            }
        }
        // const reader = new FileReader();
        // if (event.target.files && event.target.files.length)
        // {
        //     const [file] = event.target.files;
        //     reader.readAsDataURL(file);
        //     let form = new FormData();
        //     form.append('FormFile', file);
        //     this.imageDto=new ItemImagesModel( );

        // this._itemsServiceProxy.getImageUrl(form).subscribe(res => {
        //     this.imageDto.imageUrl=res['result'];

        //     this.imageDto.isMainImage = false;
        //     this.imageDto.tenantId = this.appSession.tenantId;
        //     this.imageDto.itemId= this.item.id;

        //     if(this.lstImagestDto == null){
        //         this.lstImagestDto = [];
        //     }
        //     this.lstImagestDto.push(this.imageDto);
        //     this.item.lstItemImages=this.lstImagestDto;

        //     (<HTMLInputElement>document.getElementById("file")).value = null

        // });
        // }
    }

    changeMainImage(itemImages) {
        const index = this.item.lstItemImages.findIndex((r) => {
            if (r.imageUrl === itemImages.imageUrl) {
                return itemImages.imageUrl;
            }
        });

        for (let i = 0; i < this.item.lstItemImages.length; i++) {
            if (i === index) {
                this.item.lstItemImages[i].isMainImage = true;
                const radio = <HTMLInputElement | null>(
                    document.getElementById("Select As Main Image" + i)
                );
                radio.checked = true;
            } else {
                this.item.lstItemImages[i].isMainImage = false;
                const radio = <HTMLInputElement | null>(
                    document.getElementById("Select As Main Image" + i)
                );
                radio.checked = false;
            }
        }
    }

    getCategories() {
        this._MenuServiceProxy.getCatogeory(this.menuId).subscribe((res) => {
            this.category = res;
            if (this.item.itemSubCategoryId) {
                let index = this.category.findIndex(
                    (e) => e.id === this.item.itemCategoryId
                );
                if (index != -1) {
                    this.subcategory =
                        this.category[index].lstMenuSubCategoryDto;
                } else {
                    this.subcategory = [];
                }
            }
        });
    }

    getSubCatogeory(): void {
        this._MenuServiceProxy.getCatogeory(this.menuId).subscribe((res) => {
            this.category = res;
        });
    }

    selectCategory(event): void {
        // this.item.itemSubCategoryId = null;
        this.subcategory = [];
        let categoryId = event;
        let index = this.category.findIndex(
            (e) => e.id.toString() === categoryId
        );
        if (index != -1) {
            this.subcategory = this.category[index].lstMenuSubCategoryDto;
            if (this.subcategory === undefined) {
                this.item.itemSubCategoryId = null;
            } else {
                this.item.itemSubCategoryId = this.subcategory[0].id;
            }
        } else {
            this.subcategory = [];
        }
        // this.item.itemSubCategoryId = null;
    }
}
