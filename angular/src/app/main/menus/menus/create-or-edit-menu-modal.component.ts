import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
    OnInit,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs/operators";
import {
    MenusServiceProxy,
    CreateOrEditMenuDto,
    ItemCategoryServiceProxy,
    CreateOrEditMenuCategoryDto,
    ItemsServiceProxy,
    CreateOrEditItemDto,
    RestaurantsTypeEunm,
    RType,
    LanguageBot,
    AdditionsCategorysListModel,
    CreateOrEditItemAndAdditionsCategoryDto,
    GetSpecificationsCategorysModel,
    ItemAdditionServiceProxy,
    ItemSpecificationsDto,
    TenantTypeEunm,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import * as moment from "moment";
import {
    UntypedFormArray,
    UntypedFormBuilder,
    UntypedFormGroup,
    Validators,
} from "@angular/forms";
import { addOnsLookupTableModalComponent } from "./addOns-lookup-table-modal.component";
import { specificationLookupTableModalComponent } from "./specification-lookup-table-modal.component";
import { AppSessionService } from "@shared/common/session/app-session.service";

declare var ImageCompressor: any;

const compressor = new ImageCompressor();

@Component({
    selector: "createOrEditMenuModal",
    templateUrl: "./create-or-edit-menu-modal.component.html",
})
export class CreateOrEditMenuModalComponent
    extends AppComponentBase
    implements OnInit
{
    @ViewChild("specificationLookupTableModal", { static: true }) specificationLookupTableModal: specificationLookupTableModalComponent;
    @ViewChild("addOnsLookupTableModal", { static: true }) addOnsLookupTableModal: addOnsLookupTableModalComponent;
    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    menu: CreateOrEditMenuDto = new CreateOrEditMenuDto();
    menuItemStatusName = "";
    menuCategoryName = "";
    isReplec = false;
    restaurantsTypeEunm = RestaurantsTypeEunm;
    enumKeys = [];
    menuImage: string;
    menuForm: UntypedFormGroup;
    menuId = null;
    langId = 1;
    itemIndex = -1;
    itemAddition = -1;
    categoryIndex = -1;
    fromFileUplode = false;
    arrayAddition: any;
    rType: RType[];
    additionsCategorysListModel: AdditionsCategorysListModel[];
    SpecificationListModel: GetSpecificationsCategorysModel[];
    languageBot: LanguageBot[];
    menuType: number;
    priorityMenu: number;
    isNf: boolean;
    categoryTypess: RestaurantsTypeEunm;
    selectedValue: any;
    addons: AdditionsCategorysListModel = new AdditionsCategorysListModel();
    dealStatusName = "";
    dealTypeName = "";
    cat: any;
    ind: any;
    add: any;
    sub: any;
    isMall: boolean;
    submitted = false;
    constructor(
        injector: Injector,
        private _itemAdditionServiceProxy: ItemAdditionServiceProxy,
        private _menusServiceProxy: MenusServiceProxy,
        private fb: UntypedFormBuilder,
        private _menuCategoriesServiceProxy: ItemCategoryServiceProxy,
        private _itemsServiceProxy: ItemsServiceProxy,
        private _appSessionService: AppSessionService
    ) 
    {
        super(injector);
        this.enumKeys = Object.keys(this.restaurantsTypeEunm);
    }

    public get selectedCategoryType(): RestaurantsTypeEunm {
        return this.selectedValue ? this.selectedValue.value : null;
    }

    openSelectAdditionsCategoryModal(cat, sub, ind, add) {
        this.cat = cat;
        this.ind = ind;
        this.sub = sub;
        this.add = add;
        this.addOnsLookupTableModal.show(this.additionsCategorysListModel);
    }

    openSelectSpecificationModal(cat, sub, ind, add) {
        this.cat = cat;
        this.ind = ind;
        this.sub = sub;
        this.add = add;
        this.specificationLookupTableModal.show(this.SpecificationListModel);
    }

    setdditionsCategoryNull() {
        if (!this.isMall) {
            if (!this.addOnsLookupTableModal.isCloseClick) {
                const control = (
                    (<UntypedFormArray>this.menuForm.controls["category"])
                        .at(this.cat)
                        .get("items") as UntypedFormArray
                )
                    .at(this.ind)
                    .get("additionsCategorysListModels") as UntypedFormArray;

                let item = control.at(this.add).value;

                item.id = 0;
                item.id = 0;
                item.name = "";
                item.nameEnglish = "";
                control.push(this.initAdditionsCategoryListSet(item));

                control.removeAt(this.add);
            }
        } else {
            if (!this.addOnsLookupTableModal.isCloseClick) {
                const control = (
                    (
                        (<UntypedFormArray>this.menuForm.controls["category"])
                            .at(this.cat)
                            .get("subcategory") as UntypedFormArray
                    )
                        .at(this.sub)
                        .get("items") as UntypedFormArray
                )
                    .at(this.ind)
                    .get("additionsCategorysListModels") as UntypedFormArray;

                let item = control.at(this.add).value;

                item.id = 0;
                item.id = 0;
                item.name = "";
                item.nameEnglish = "";
                control.push(this.initAdditionsCategoryListSet(item));

                control.removeAt(this.add);
            }
        }
    }

    UpdateAdditionsCategoryId() {
        if (!this.isMall) {
            if (!this.addOnsLookupTableModal.isCloseClick) {
                const control = (
                    (<UntypedFormArray>this.menuForm.controls["category"])
                        .at(this.cat)
                        .get("items") as UntypedFormArray
                )
                    .at(this.ind)
                    .get("additionsCategorysListModels") as UntypedFormArray;

                let item = control.at(this.add).value;
                item.AdditionsAndItemId =
                    this.addOnsLookupTableModal.AdditionsCategorysid;
                item.id = this.addOnsLookupTableModal.id;
                item.name = this.addOnsLookupTableModal.displayName;
                item.nameEnglish =
                    this.addOnsLookupTableModal.displayNameEnglish;
                control.push(this.initAdditionsCategoryListSet(item));

                control.removeAt(this.add);
            }
        } else {
            if (!this.addOnsLookupTableModal.isCloseClick) {
                const control = (
                    (
                        (<UntypedFormArray>this.menuForm.controls["category"])
                            .at(this.cat)
                            .get("subcategory") as UntypedFormArray
                    )
                        .at(this.sub)
                        .get("items") as UntypedFormArray
                )
                    .at(this.ind)
                    .get("additionsCategorysListModels") as UntypedFormArray;

                let item = control.at(this.add).value;
                item.AdditionsAndItemId =
                    this.addOnsLookupTableModal.AdditionsCategorysid;
                item.id = this.addOnsLookupTableModal.id;
                item.name = this.addOnsLookupTableModal.displayName;
                item.nameEnglish =
                    this.addOnsLookupTableModal.displayNameEnglish;
                control.push(this.initAdditionsCategoryListSet(item));

                control.removeAt(this.add);
            }
        }
    }

    UpdateSpecificationId() {
        if (!this.isMall) {
            if (!this.specificationLookupTableModal.isCloseClick) {
                const control = (
                    (<UntypedFormArray>this.menuForm.controls["category"])
                        .at(this.cat)
                        .get("items") as UntypedFormArray
                )
                    .at(this.ind)
                    .get("SpecificationModels") as UntypedFormArray;

                let item = control.at(this.add).value;
                item.id = this.specificationLookupTableModal.id;
                item.name = this.specificationLookupTableModal.displayName;
                item.nameEnglish = this.specificationLookupTableModal.displayNameEnglish;
                item.isRequired = this.specificationLookupTableModal.isRequired;

                control.push(this.initSpecificationListSet(item));

                control.removeAt(this.add);
            }
        } else {
            if (!this.specificationLookupTableModal.isCloseClick) {
                const control = (
                    (
                        (<UntypedFormArray>this.menuForm.controls["category"])
                            .at(this.cat)
                            .get("subcategory") as UntypedFormArray
                    )
                        .at(this.sub)
                        .get("items") as UntypedFormArray
                )
                    .at(this.ind)
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
    }

    show(menuId?: number): void {
        if (this._appSessionService.tenant.tenantType == TenantTypeEunm.Mall) {
            this.isMall = true;
            this.menuForm = this.fb.group({
                menuName: [""],
                menuNameEnglish: [""],
                menuDescription: [""],
                menuDescriptionEnglish: [""],
                priority: 0,
                restaurantsType: 0,
                languageBotId: 1,
                id: null,
                category: this.fb.array([this.initCategorySub()]),
            });
        } else {
            this.isMall = false;
            this.menuForm = this.fb.group({
                menuName: [""],
                menuNameEnglish: [""],
                menuDescription: [""],
                menuDescriptionEnglish: [""],
                priority: 0,
                restaurantsType: 0,
                languageBotId: 1,
                id: null,
                category: this.fb.array([this.initCategory()]),
            });
        }

        this._menusServiceProxy
            .getAdditionsCategorysList()
            .subscribe((result) => {
                this.additionsCategorysListModel = result;
                // this.languageBot.shift();
            });

        this._menusServiceProxy
            .getSpecificationsCategorys()
            .subscribe((result) => {
                this.SpecificationListModel = result;
            });
        this.menuId = menuId;

        if (!menuId) {
            this.menu = new CreateOrEditMenuDto();
            this.menu.id = menuId;
            this.menu.effectiveTimeFrom = moment().startOf("day");
            this.menu.effectiveTimeTo = moment().startOf("day");
            this.menuItemStatusName = "";
            this.menuCategoryName = "";

            this.active = true;
            this.modal.show();
        } else {
            let categoryList = [];
            this._menusServiceProxy.getCategoryMenu(menuId).subscribe((res) => {
                res.result.forEach((category) => {
                    categoryList.push(
                        this.bindCategory(
                            category.categoryId,
                            category.categoryName,
                            category.categoryNameEnglish,
                            category.listItemInCategories
                        )
                    );
                });

                this._menusServiceProxy
                    .getMenuForEdit(menuId)
                    .subscribe((result) => {
                        this.menu = result.menu;
                        this.menuForm = this.fb.group({
                            menuName: this.menu.menuName,
                            menuNameEnglish: this.menu.menuNameEnglish,
                            menuDescription: this.menu.menuDescription,
                            menuDescriptionEnglish: this.menu.menuDescriptionEnglish,
                            restaurantsType: this.menu.restaurantsType,
                            languageBotId: this.menu.languageBotId,
                            id: menuId,
                            priority: this.menu.priority,
                            category: this.fb.array(categoryList),
                        });
                        (this.menuImage = this.menu.imageUri),
                            (this.active = true);
                        this.modal.show();
                    });
            });
        }
    }
    close(): void {
        this.active = false;
        this.submitted = false;
        this.saving = false;
        this.modalSave.emit(null);
        this.modal.hide();
    }

    async ngOnInit() {
        await this.getIsAdmin();
        this.isNf = true;

        this._menusServiceProxy
            .getRType(this.appSession.tenantId)
            .subscribe((result) => {
                if (this.isNf) {
                    this.rType = result;
                } else {
                    this.rType = result;
                }
            });

        this._menusServiceProxy.getLanguageBot().subscribe((result) => {
            this.languageBot = result;
        });

        this._menusServiceProxy
            .getAdditionsCategorysList()
            .subscribe((result) => {
                this.additionsCategorysListModel = result;
            });

        if (this._appSessionService.tenant.tenantType == TenantTypeEunm.Mall) {
            this.isMall = true;
            this.menuForm = this.fb.group({
                menuName: [""],
                menuNameEnglish: [""],
                menuDescription: [""],
                menuDescriptionEnglish: [""],
                priority: 0,
                restaurantsType: 0,
                languageBotId: 1,
                id: null,
                category: this.fb.array([this.initCategorySub()]),
            });
        } else {
            this.isMall = false;
            this.menuForm = this.fb.group({
                menuName: [""],
                menuNameEnglish: [""],
                menuDescription: [""],
                menuDescriptionEnglish: [""],
                priority: 0,
                restaurantsType: 0,
                languageBotId: 1,
                id: null,
                category: this.fb.array([this.initCategory()]),
            });
        }
    }

    initCategory() {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            name: ["", [Validators.required]],
            nameEnglish: ["", [Validators.required]],
            // ---------------------------------------------------------------------
            items: this.fb.array([this.initItem()]),
        });
    }
    initCategorySub() {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            name: ["", [Validators.required]],
            nameEnglish: ["", [Validators.required]],
            // ---------------------------------------------------------------------
            subcategory: this.fb.array([this.initSubCategory()]),
        });
    }

    initSubCategory() {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            namesub: ["", [Validators.required]],
            nameEnglishsub: ["", [Validators.required]],
            // ---------------------------------------------------------------------
            items: this.fb.array([this.initItem()]),
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

    bindCategory(id, name, nameEnglish, items) {
        let data = [];
        if (items != null || items != undefined) {
            items.forEach((item) => {
                data.push(
                    this.bindItem(
                        item.itemName,
                        item.itemNameEnglish,
                        item.price,
                        item.priority,
                        item.languageBotId,
                        item.itemDescription,
                        item.itemDescriptionEnglish,
                        item.additionsCategorysListModels,
                        item.itemSpecifications,
                        item.imageUri,
                        this.menuId,
                        id,
                        item.id,
                        item.sku,
                        item.isInService,
                        item.oldPrice,
                        item.ingredients
                    )
                );
            });
        }
        return this.fb.group({
            name: [name, [Validators.required]],
            nameEnglish: [nameEnglish, [Validators.required]],
            items: this.fb.array(data),
            id: id,
        });
    }

    initItemAddition() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            name: ["", [Validators.required]],
            nameEnglish: ["", [Validators.required]],
            price: [0, [Validators.required]],
            sku: "",
            isInService: [true, [Validators.required]],
            IsLoyal: false,
            LoyaltyPoints: 0,
            OriginalLoyaltyPoints: 0,
            IsOverrideLoyaltyPoints: false,
            itemId: 0,
        });
    }

    initAddition() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            name: ["", [Validators.required]],
            nameEnglish: ["", [Validators.required]],
            price: [0, [Validators.required]],
            sku: "",
            isInService: [true, [Validators.required]],
            AdditionsCategorys: this.initAdditionsCategoryList,
            IsLoyal: false,
            LoyaltyPoints: 0,
            OriginalLoyaltyPoints: 0,
            IsOverrideLoyaltyPoints: false,
            itemId: 0,
        });
    }

    bindinitAddition(Id, name, nameEnglish, AdditionsAndItemId) {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            id: Id,
            name: [name, [Validators.required]],
            nameEnglish: [nameEnglish, [Validators.required]],
            additionsAndItemId: [AdditionsAndItemId, [Validators.required]],
        });
    }

    bindinitSpecification(
        id,
        isMultipleSelection,
        isRequired,
        priority,
        specificationDescription,
        specificationDescriptionEnglish,
        itemSpecificationId
    ) {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            id: id,
            itemSpecificationId: [itemSpecificationId, [Validators.required]],
            isMultipleSelection: [isMultipleSelection, [Validators.required]],
            isRequired: [isRequired, [Validators.required]],
            priority: [priority, [Validators.required]],
            specificationDescription: [
                specificationDescription,
                [Validators.required],
            ],
            specificationDescriptionEnglish: [
                specificationDescriptionEnglish,
                [Validators.required],
            ],
        });
    }

    initItem() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            itemName: ["", [Validators.required]],
            itemNameEnglish: ["", [Validators.required]],
            price: [0, [Validators.required]],
            priority: [0, [Validators.required]],
            itemDescription: [""],
            itemDescriptionEnglish: [""],
            imageUri: ["", [Validators.required]],
            sku: "",
            IsLoyal: false,
            LoyaltyPoints: 0,
            OriginalLoyaltyPoints: 0,
            IsOverrideLoyaltyPoints: false,
            isInService: [true],
            oldPrice: [0],
            ingredients: [0, [Validators.required]],
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

    onOptionsSelected(value: any) {
        alert("the selected value is " + value);
    }

    bindItem(
        name,
        nameEnglish,
        price,
        priority,
        languageBotId,
        description,
        descriptionEnglish,
        itemAddition,
        specification,
        image,
        menuId,
        categoryId,
        itemId,
        sKU,
        isInService,
        oldPrice,
        ingredients
    ) {
        let data = [];
        let data2 = [];
        itemAddition.forEach((item) => {
            data.push(
                this.bindinitAddition(
                    item.id,
                    item.name,
                    item.nameEnglish,
                    item.additionsAndItemId
                )
            );
        });

        specification.forEach((item) => {
            data2.push(
                this.bindinitSpecification(
                    item.id,
                    item.isMultipleSelection,
                    item.isRequired,
                    item.priority,
                    item.specificationDescription,
                    item.specificationDescriptionEnglish,
                    item.itemSpecificationId
                )
            );
        });

        this.arrayAddition = itemAddition;
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            itemName: [name, [Validators.required]],
            itemNameEnglish: [nameEnglish, [Validators.required]],
            price: [price, [Validators.required]],
            priority: [priority, [Validators.required]],
            languageBotId: [languageBotId, [Validators.required]],
            itemDescription: [description],
            itemDescriptionEnglish: [descriptionEnglish],
            //'itemAdditionDtos': this.fb.array(data),
            additionsCategorysListModels: this.fb.array(data),
            SpecificationModels: this.fb.array(data2),
            imageUri: [image, [Validators.required]],
            sku: sKU,
            isInService: isInService,
            IsLoyal: false,
            LoyaltyPoints: 0,
            OriginalLoyaltyPoints: 0,
            IsOverrideLoyaltyPoints: false,
            oldPrice: oldPrice,
            ingredients: ingredients,
            menuId: menuId,
            itemCategoryId: categoryId,
            id: itemId,
        });
    }

    initZ() {
        return this.fb.group({
            //  ---------------------forms fields on z level ------------------------
            Z: ["Z", [Validators.required, Validators.pattern("[0-9]{3}")]],
            // ---------------------------------------------------------------------
        });
    }

    addCategory() {
        const control = <UntypedFormArray>this.menuForm.controls["category"];
        control.push(this.initCategorySub());
    }

    addSubCategory(category) {
        const control = (
            (<UntypedFormArray>(
                this.menuForm.controls["category"]
            )) as UntypedFormArray
        )
            .at(category)
            .get("subcategory") as UntypedFormArray;

        const control2 = <UntypedFormArray>this.menuForm.controls["category"];

        control.push(this.initSubCategory());
    }

    addItem(category, ix) {
        if (!this.isMall) {
            const control = (<UntypedFormArray>(
                this.menuForm.controls["category"]
            ))
                .at(ix)
                .get("items") as UntypedFormArray;
            control.push(this.initItem());
        } else {
            const control = (
                (<UntypedFormArray>this.menuForm.controls["category"])
                    .at(category)
                    .get("subcategory") as UntypedFormArray
            )
                .at(ix)
                .get("items") as UntypedFormArray;
            control.push(this.initItem());
        }
    }

    addItemAddition(category, subcategory, index) {
        if (!this.isMall) {
            const control = (
                (<UntypedFormArray>this.menuForm.controls["category"])
                    .at(subcategory)
                    .get("items") as UntypedFormArray
            )
                .at(index)
                .get("additionsCategorysListModels") as UntypedFormArray;
            control.push(this.initAdditionsCategoryList());
        } else {
            const control = (
                (
                    (<UntypedFormArray>this.menuForm.controls["category"])
                        .at(category)
                        .get("subcategory") as UntypedFormArray
                )
                    .at(subcategory)
                    .get("items") as UntypedFormArray
            )
                .at(index)
                .get("additionsCategorysListModels") as UntypedFormArray;
            control.push(this.initAdditionsCategoryList());
        }
    }

    addSpecification(category, subcategory, index) {
        if (!this.isMall) {
            const control = (
                (<UntypedFormArray>this.menuForm.controls["category"])
                    .at(subcategory)
                    .get("items") as UntypedFormArray
            )
                .at(index)
                .get("SpecificationModels") as UntypedFormArray;

            control.push(this.initSpecification());
        } else {
            const control = (
                (
                    (<UntypedFormArray>this.menuForm.controls["category"])
                        .at(category)
                        .get("subcategory") as UntypedFormArray
                )
                    .at(subcategory)
                    .get("items") as UntypedFormArray
            )
                .at(index)
                .get("SpecificationModels") as UntypedFormArray;

            control.push(this.initSpecification());
        }
    }

    deleteItem(category, subcategory, index) {
        if (!this.isMall) {
            const control = (<UntypedFormArray>(
                this.menuForm.controls["category"]
            ))
                .at(category)
                .get("items") as UntypedFormArray;

            let item = control.at(index).value;
            if (item.id) {
                this.message.confirm(
                    "",
                    this.l("AreYouSure"),
                    (isConfirmed) => {
                        if (isConfirmed) {
                            this._itemsServiceProxy
                                .deleteItem(item.id)
                                .subscribe(() => {
                                    this.notify.success(
                                        this.l("successfullyDeleted")
                                    );
                                    control.removeAt(index);
                                });
                        }
                    }
                );
            } else {
                control.removeAt(index);
            }
        } else {
            const control = (
                (<UntypedFormArray>this.menuForm.controls["category"])
                    .at(category)
                    .get("subcategory") as UntypedFormArray
            )
                .at(subcategory)
                .get("items") as UntypedFormArray;

            let item = control.at(index).value;
            if (item.id) {
                this.message.confirm(
                    "",
                    this.l("AreYouSure"),
                    (isConfirmed) => {
                        if (isConfirmed) {
                            this._itemsServiceProxy
                                .deleteItem(item.id)
                                .subscribe(() => {
                                    this.notify.success(
                                        this.l("successfullyDeleted")
                                    );
                                    control.removeAt(index);
                                });
                        }
                    }
                );
            } else {
                control.removeAt(index);
            }
        }
    }

    deleteitemAddition(category, subcategory, index, add) {
        if (!this.isMall) {
            const control = (
                (<UntypedFormArray>this.menuForm.controls["category"])
                    .at(category)
                    .get("items") as UntypedFormArray
            )
                .at(index)
                .get("additionsCategorysListModels") as UntypedFormArray;

            let item = control.at(add).value;
            if (item.id) {
                this.message.confirm(
                    "",
                    this.l("AreYouSure"),
                    (isConfirmed) => {
                        if (isConfirmed) {
                            this._itemsServiceProxy
                                .deleteItemAndAdditionsCategorys(
                                    item.additionsAndItemId
                                )
                                .subscribe(() => {
                                    this.notify.success(
                                        this.l("successfullyDeleted")
                                    );
                                    control.removeAt(add);
                                });
                        }
                    }
                );
            } else {
                control.removeAt(add);
            }
        } else {
            const control = (
                (
                    (<UntypedFormArray>this.menuForm.controls["category"])
                        .at(category)
                        .get("subcategory") as UntypedFormArray
                )
                    .at(subcategory)
                    .get("items") as UntypedFormArray
            )
                .at(index)
                .get("additionsCategorysListModels") as UntypedFormArray;

            let item = control.at(add).value;
            if (item.id) {
                this.message.confirm(
                    "",
                    this.l("AreYouSure"),
                    (isConfirmed) => {
                        if (isConfirmed) {
                            this._itemsServiceProxy
                                .deleteItemAndAdditionsCategorys(
                                    item.additionsAndItemId
                                )
                                .subscribe(() => {
                                    this.notify.success(
                                        this.l("successfullyDeleted")
                                    );
                                    control.removeAt(add);
                                });
                        }
                    }
                );
            } else {
                control.removeAt(add);
            }
        }
    }

    deleteSpecification(category, subcategory, index, add) {
        if (!this.isMall) {
            const control = (
                (<UntypedFormArray>this.menuForm.controls["category"])
                    .at(category)
                    .get("items") as UntypedFormArray
            )
                .at(index)
                .get("SpecificationModels") as UntypedFormArray;

            let item = control.at(add).value;

            if (item.itemSpecificationId) {
                this.message.confirm(
                    "",
                    this.l("AreYouSure"),
                    (isConfirmed) => {
                        if (isConfirmed) {
                            this._itemAdditionServiceProxy
                                .deleteItemSpecifications(
                                    item.itemSpecificationId
                                )
                                .subscribe(() => {
                                    this.notify.success(
                                        this.l("successfullyDeleted")
                                    );
                                    control.removeAt(add);
                                });
                        }
                    }
                );
            } else {
                control.removeAt(add);
            }
        } else {
            const control = (
                (
                    (<UntypedFormArray>this.menuForm.controls["category"])
                        .at(category)
                        .get("subcategory") as UntypedFormArray
                )
                    .at(subcategory)
                    .get("items") as UntypedFormArray
            )
                .at(index)
                .get("SpecificationModels") as UntypedFormArray;

            let item = control.at(add).value;

            if (item.itemSpecificationId) {
                this.message.confirm(
                    "",
                    this.l("AreYouSure"),
                    (isConfirmed) => {
                        if (isConfirmed) {
                            this._itemAdditionServiceProxy
                                .deleteItemSpecifications(
                                    item.itemSpecificationId
                                )
                                .subscribe(() => {
                                    this.notify.success(
                                        this.l("successfullyDeleted")
                                    );
                                    control.removeAt(add);
                                });
                        }
                    }
                );
            } else {
                control.removeAt(add);
            }
        }
    }

    deleteCategoryItem(index) {
        const control = (<UntypedFormArray>(
            this.menuForm.controls["category"]
        )) as UntypedFormArray;
        let item = control.at(index).value;

        if (item.id) {
            this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
                if (isConfirmed) {
                    this._menuCategoriesServiceProxy
                        .delete(item.id)
                        .subscribe(() => {
                            this.notify.success(this.l("successfullyDeleted"));
                            control.removeAt(index);
                        });
                }
            });
        } else {
            control.removeAt(index);
        }
    }

    deleteCategorySubItem(index, Subcatg) {
        const control = (<UntypedFormArray>this.menuForm.controls["category"])
            .at(index)
            .get("subcategory") as UntypedFormArray;
        let item = control.at(Subcatg).value;

        control.removeAt(Subcatg);
    }

    addMenu() {
        this.saving = true;
        if (
            this.menuForm["controls"].category["controls"][0]["controls"].items
                .length === 0
        ) {
            this.message.error("", this.l("menuMustHaveAtLeastOneItem"));
            this.submitted = true;
            this.saving = false;
            return;
        }
        if (this.menuForm.invalid) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        let data = this.menuForm.value;

        let menu = new CreateOrEditMenuDto();

        menu.menuName = data.menuName;
        menu.menuNameEnglish = data.menuNameEnglish;
        menu.menuDescription = data.menuDescription;

        menu.menuDescriptionEnglish = data.menuDescriptionEnglish;
        menu.restaurantsType = data.restaurantsType;

        this.menuType = data.restaurantsType;
        this.priorityMenu = parseInt(data.priority);
        menu.priority = parseInt(data.priority);
        menu.languageBotId = parseInt(data.languageBotId);
        menu.imageUri = this.menuImage;
        this.langId = parseInt(data.languageBotId);

        if (this.menuId) {
            menu.id = this.menuId;
        }
        this._menusServiceProxy.createOrEdit(menu).subscribe(
            (response) => {
                // this.close();
                this.addOrSaveCategory(response);
                this.notify.info(this.l("savedSuccessfully"));
                // this.modalSave.emit(null);
            },
            (error: any) => {
                if (error) {
                    this.saving = false;
                    this.submitted = false;
                }
            }
        );
    }

    addOrSaveCategory(menuId) {
        if (menuId == 0) {
            menuId = this.menuId;
        }
        let data = this.menuForm.value;
        for (let i = 0; i < data.category.length; i++) {
            let category: CreateOrEditMenuCategoryDto =
                new CreateOrEditMenuCategoryDto();

            category.name = data.category[i].name;
            category.nameEnglish = data.category[i].nameEnglish;
            category.menuType = this.menuType;
            category.priority = this.priorityMenu;
            category.menuId = menuId;

            category.languageBotId = this.langId;
            if (data.category[i].id) {
                category.id = data.category[i].id;
            }
            this._menuCategoriesServiceProxy
                .createOrEdit(category)
                .pipe(
                    finalize(() => {
                        this.saving = false;
                    })
                )
                .subscribe((categoryResponse) => {
                    for (let j = 0; j < data.category[i].items.length; j++) {
                        let item = new CreateOrEditItemDto();
                        let obj = data.category[i].items[j];

                        item.sku = obj.sku;
                        item.isInService = obj.isInService;

                        item.categoryNames = data.category[i].categoryNames;
                        item.categoryNamesEnglish =
                            data.category[i].categoryNamesEnglish;
                        item.itemDescription = obj.itemDescription;
                        item.itemDescriptionEnglish =
                            obj.itemDescriptionEnglish;
                        item.itemName = obj.itemName;
                        item.loyaltyPoints = 0;
                        item.isLoyal = false;
                        item.originalLoyaltyPoints = 0;
                        // item.IsOverrideLoyaltyPoints=false;
                        item.itemNameEnglish = obj.itemNameEnglish;

                        if (obj.ingredients != null) {
                            item.ingredients = obj.ingredients;
                            item.oldPrice = obj.oldPrice;
                        }

                        item.itemCategoryId = category.id
                            ? category.id
                            : categoryResponse;
                        item.menuId = this.menuId ? this.menuId : menuId;
                        // item.isInService = false;
                        item.imageUri = obj.imageUri;
                        item.priority = parseInt(obj.priority);
                        item.creationTime = moment();
                        item.deletionTime = moment();

                        item.menuType = this.menuType;
                        item.languageBotId = this.langId;
                        let addAnditem =
                            new CreateOrEditItemAndAdditionsCategoryDto();
                        item.price = parseFloat(obj.price);
                        if (obj.id) {
                            item.id = obj.id;
                        }
                        item.lastModificationTime = moment();

                        this._itemsServiceProxy
                            .createOrEdit(item)
                            .pipe(
                                finalize(() => {
                                    this.saving = false;
                                })
                            )
                            .subscribe(
                                (res) => {
                                    if (item.id) {
                                    } else {
                                        item.id = res;
                                    }

                                    obj.additionsCategorysListModels.forEach(
                                        (element) => {
                                            addAnditem.id = 0;
                                            addAnditem.itemId = item.id;
                                            addAnditem.menuType = this.menuType;
                                            addAnditem.specificationId = 0;
                                            addAnditem.additionsCategorysId =
                                                element.id;
                                            addAnditem.additionsAndItemId =
                                                element.additionsAndItemId;

                                            this._menusServiceProxy
                                                .createOrEditItemAndAdditionsCategorys(
                                                    addAnditem
                                                )
                                                .subscribe((response) => {});
                                        }
                                    );

                                    let addAnditem2 =
                                        new ItemSpecificationsDto();
                                    obj.SpecificationModels.forEach(
                                        (element) => {
                                            addAnditem2.id =
                                                element.itemSpecificationId;
                                            addAnditem2.itemId = item.id;
                                            addAnditem2.specificationId =
                                                element.id;
                                            addAnditem2.isRequired =
                                                element.isRequired;
                                            this._itemAdditionServiceProxy
                                                .createOrEditItemSpecifications(
                                                    addAnditem2
                                                )
                                                .subscribe((response) => {});
                                        }
                                    );
                                    this.notify.info(
                                        this.l("savedSuccessfully")
                                    );
                                    this.close();
                                    this.submitted = false;
                                    this.saving = false;
                                },
                                (error: any) => {
                                    if (error) {
                                        this.saving = false;
                                        this.submitted = false;
                                    }
                                }
                            );
                    }
                    // this.modalSave.emit(null);
                });
        }
    }

    onFileChange(event, category, subcategory, index) {
        if (!this.isMall) {
            const reader = new FileReader();
            if (this.fromFileUplode) {
                category = this.categoryIndex;
                index = this.itemIndex;
            }
            if (event.target.files && event.target.files.length) {
                const [file] = event.target.files;
                reader.readAsDataURL(file);

                const control = (<UntypedFormArray>(
                    this.menuForm.controls["category"]
                ))
                    .at(category)
                    .get("items") as UntypedFormArray;

                compressor.compress(file, { quality: 0.5 }).then((cf) => {
                    let form = new FormData();
                    form.append("FormFile", cf);
                    this._itemsServiceProxy
                        .getImageUrl(form)
                        .subscribe((res) => {
                            control
                                .at(index)
                                .get("imageUri")
                                .setValue(res["result"]);
                            this.fromFileUplode = false;
                            this.categoryIndex = -1;
                            this.itemIndex = -1;
                        });
                });
            }
        } else {
            const reader = new FileReader();
            if (this.fromFileUplode) {
                category = this.categoryIndex;
                index = this.itemIndex;
            }
            if (event.target.files && event.target.files.length) {
                const [file] = event.target.files;
                reader.readAsDataURL(file);

                const control = (
                    (<UntypedFormArray>this.menuForm.controls["category"])
                        .at(category)
                        .get("subcategory") as UntypedFormArray
                )
                    .at(subcategory)
                    .get("items") as UntypedFormArray;

                compressor.compress(file, { quality: 0.5 }).then((cf) => {
                    let form = new FormData();
                    form.append("FormFile", cf);
                    this._itemsServiceProxy
                        .getImageUrl(form)
                        .subscribe((res) => {
                            control
                                .at(index)
                                .get("imageUri")
                                .setValue(res["result"]);
                            this.fromFileUplode = false;
                            this.categoryIndex = -1;
                            this.itemIndex = -1;
                        });
                });
            }
        }
    }

    getImage(category, subcategory, index) {
        if (!this.isMall) {
            const control = (<UntypedFormArray>(
                this.menuForm.controls["category"]
            ))
                .at(category)
                .get("items") as UntypedFormArray;

            let item = control.at(index).value;
            return item.imageUri;
        } else {
            const control = (
                (<UntypedFormArray>this.menuForm.controls["category"])
                    .at(category)
                    .get("subcategory") as UntypedFormArray
            )
                .at(subcategory)
                .get("items") as UntypedFormArray;

            let item = control.at(index).value;
            return item.imageUri;
        }
    }

    openFileUploder(category, subcategory, index) {
        this.categoryIndex = category;
        this.itemIndex = index;
        this.fromFileUplode = true;
        document.getElementById("uplode").click();
    }

    openFileUploder1() {
        //this.fromFileUplode = true;
        document.getElementById("uplode1").click();
    }

    onFileChange1(event) {
        const reader = new FileReader();

        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            let form = new FormData();
            form.append("FormFile", file);
            this._itemsServiceProxy.getImageUrl(form).subscribe((res) => {
                this.menuImage = res["result"];
            });
        }
    }
}
