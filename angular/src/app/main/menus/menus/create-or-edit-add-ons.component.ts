import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { finalize } from 'rxjs/operators';
import {
    MenusServiceProxy,
    CreateOrEditMenuDto,
    ItemsServiceProxy, ItemAdditionDto, RestaurantsTypeEunm, RType, LanguageBot, ItemAdditionServiceProxy, CreateOrEditItemAdditionCategoryDto, GetItemAdditionsCategorysModel, AdditionsModelDto, LoyaltyServiceProxy
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UntypedFormArray, UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../../services/dark-mode.service';
import { Router } from '@angular/router';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Paginator } from "primeng/paginator";

import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import * as rtlDetect from 'rtl-detect';


declare var ImageCompressor: any;

const compressor = new ImageCompressor();
@Component({
    selector: 'app-create-or-edit-add-ons',
    templateUrl: './create-or-edit-add-ons.component.html',
})
export class CreateOrEditAddOnsComponent extends AppComponentBase implements OnInit {
    category = new GetItemAdditionsCategorysModel();
    theme: string;
    currency = '';
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('upload', { static: false }) upload: ElementRef;


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    addNewItem = false;
    active = false;
    saving = false;
    submitted = false;
    submitted2 = false;
    savingOnCreate = false;
    savingOnEdit = false;



    menu: CreateOrEditMenuDto = new CreateOrEditMenuDto();

    menuItemStatusName = '';
    menuCategoryName = '';

    restaurantsTypeEunm = RestaurantsTypeEunm;
    enumKeys = [];
    item = [];
    menuForm: UntypedFormGroup;
    menuId = null;
    langId = 1;
    itemIndex = -1;
    itemAddition = -1;
    categoryIndex = -1;
    fromFileUplode = false;
    arrayAddition: any;
    createcategoryArray: ItemAdditionDto = new ItemAdditionDto();


    CreateCategorys: GetItemAdditionsCategorysModel = new GetItemAdditionsCategorysModel();

    CreateItemsategorys: ItemAdditionDto = new ItemAdditionDto();
    // isCondiments  = false;
    // isDeserts= false;
    // isCrispy= false;
    // isNon= true;

    rType: RType[];

    languageBot: LanguageBot[];

    menuType: number;
    priorityMenu: number;

    isNf: boolean;
    categoryTypess: RestaurantsTypeEunm;
    selectedValue: any;
    checkedCategoryList: any;
    isTenantLoyal:boolean;
    isArabic= false;

    constructor(
        injector: Injector,
        private _menusServiceProxy: MenusServiceProxy,
        private fb: UntypedFormBuilder,
        private _itemAdditionServiceProxy: ItemAdditionServiceProxy,
        private _itemsServiceProxy: ItemsServiceProxy,
        public darkModeService: DarkModeService,
        private router: Router,
        private modalService: NgbModal,
        config: NgbModalConfig,
        private _router: Router,
        private _loyaltyServiceProxy: LoyaltyServiceProxy,

    ) {
        super(injector);
        config.backdrop = 'static';
        config.keyboard = false;
        // this.ngOnInit();
        this.enumKeys = Object.keys(this.restaurantsTypeEunm);
    }

    public get selectedCategoryType(): RestaurantsTypeEunm {
        return this.selectedValue ? this.selectedValue.value : null;
    }
    public CreateItemsategorysList: ItemAdditionDto[];
    ngAfterViewInit() {

        if (this.upload !== undefined) {
            this.upload.nativeElement.focus();
        }
    }
    addItem2() {
        if (this.CreateCategorys.categoryName === null || this.CreateCategorys.categoryName === undefined || this.CreateCategorys.categoryName === '' ||
            this.CreateCategorys.categoryNameEnglish === null || this.CreateCategorys.categoryNameEnglish === undefined || this.CreateCategorys.categoryNameEnglish === '') {
            this.submitted = true;
            return;
        }
        this.submitted = false;
        for (let j = 0; j < this.CreateItemsategorysList.length; j++) {
            if (this.CreateItemsategorysList[j].name === '' || this.CreateItemsategorysList[j].price === null ||
                this.CreateItemsategorysList[j].name === null || this.CreateItemsategorysList[j].price === undefined ||
                this.CreateItemsategorysList[j].name === undefined ||
                this.CreateItemsategorysList[j].nameEnglish === '' ||
                this.CreateItemsategorysList[j].nameEnglish === null ||
                this.CreateItemsategorysList[j].nameEnglish === undefined
            ) {
                this.submitted = true;
                return;
            }
        }

        this.submitted = false;
        this.CreateItemsategorysList.push({
            itemAdditionsId: 0,
            name: '',
            nameEnglish: null,
            price: 0,
            itemId: 0,
            sku: '',
            menuType: 0,
            languageBotId: 0,
            itemAdditionsCategoryId: 0,
            isInService: false,
            tenantId: 0,
            imageUri: '',
            loyaltyPoints: 0,
            originalLoyaltyPoints: 0,
            isOverrideLoyaltyPoints: false,
            loyaltyDefinitionId: 0,
            createdBy: 0,
            id: 0,
            init: function (_data?: any): void {
                throw new Error('Function not implemented.');
            },
            toJSON: function (data?: any) {
                throw new Error('Function not implemented.');
            }
        });
    }

    addItem3() {
        if (this.category.categoryName === null || this.category.categoryName === undefined || this.category.categoryName === '' ||
            this.category.categoryNameEnglish === null || this.category.categoryNameEnglish === undefined || this.category.categoryNameEnglish === '') {
            this.submitted2 = true;
            return;
        }
        this.submitted2 = false;
        for (let j = 0; j < this.category.listItemAdditionsCategories.length; j++) {
            if (this.category.listItemAdditionsCategories[j].name === '' || this.category.listItemAdditionsCategories[j].price === null ||
                this.category.listItemAdditionsCategories[j].name === null || this.category.listItemAdditionsCategories[j].price === undefined ||
                this.category.listItemAdditionsCategories[j].name === undefined ||
                this.category.listItemAdditionsCategories[j].nameEnglish === '' ||
                this.category.listItemAdditionsCategories[j].nameEnglish === null ||
                this.category.listItemAdditionsCategories[j].nameEnglish === undefined
            ) {
                this.submitted2 = true;
                return;
            }
        }
        this.submitted2 = false;

        this.category.listItemAdditionsCategories.push({
            itemAdditionsId: 0,
            name: '',
            nameEnglish: null,
            price: 0,
            itemId: 0,
            sku: '',
            menuType: 0,
            languageBotId: 0,
            itemAdditionsCategoryId: 0,
            isInService: false,
            tenantId: 0,
            imageUri: '',
            loyaltyPoints: 0,
            originalLoyaltyPoints: 0,
            isOverrideLoyaltyPoints: false,
            loyaltyDefinitionId: 0,
            createdBy: 0,
            id: 0,
            init: function (_data?: any): void {
                throw new Error('Function not implemented.');
            },
            toJSON: function (data?: any) {
                throw new Error('Function not implemented.');
            }
        });
    }

    /**
     * DeleteItem
     *
     * @param id
     */
    deleteItem(id) {
        for (let i = 0; i < this.CreateItemsategorysList.length; i++) {
            if (this.CreateItemsategorysList.indexOf(this.CreateItemsategorysList[i]) === id) {
                this.CreateItemsategorysList.splice(i, 1);
                break;
            }
        }
    }
    deleteItem2(item, id) {
        if (item.id) {
            this.message.confirm(
                '',
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._itemAdditionServiceProxy.deleteItemAddition(item.id)
                            .subscribe(() => {
                                this.notify.success(this.l('successfullyDeleted'));
                                for (let i = 0; i < this.category.listItemAdditionsCategories.length; i++) {
                                    if (this.category.listItemAdditionsCategories.indexOf(this.category.listItemAdditionsCategories[i]) === id) {
                                        this.category.listItemAdditionsCategories.splice(i, 1);
                                        break;
                                    }
                                }
                                this.show();

                            });
                    }
                }
            );
        } else {
            let lastElement = this.category.listItemAdditionsCategories.splice(-1, 1);

        }

    }
    show(event?: LazyLoadEvent): void {
        this.submitted = false;
        this.submitted2 = false;
        this.menuForm = this.fb.group({
            menuName: [''],
            menuNameEnglish: [''],
            menuDescription: [''],
            menuDescriptionEnglish: [''],
            priority: 0,
            restaurantsType: 0,
            languageBotId: 1,
            id: null,
        });
 
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();
        this._menusServiceProxy.getItemAdditionsCategories(
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(res => {
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event);
            this.primengTableHelper.totalRecordsCount = res.totalCount;
            this.primengTableHelper.records = res.lstItemAdditionsCategory;
            this.primengTableHelper.hideLoadingIndicator();
            this.active = true;
        });
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }


    close(): void {
        this.active = false;
    }

    ngOnInit() {
        this.checkIsLoyality();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        this.currency = this.appSession.tenant.currencyCode;

        this.CreateItemsategorysList = [];

        this.theme = ThemeHelper.getTheme();

        this.isNf = (this.appSession.tenantId === 4) || (this.appSession.tenantId === 7) || (this.appSession.tenantId === 14);

        this._menusServiceProxy.getRType(this.appSession.tenantId).subscribe(result => {
            if (this.isNf) {

                this.rType = result;
                this.rType.shift();

            } else {

                this.rType = result;
            }

        });

        this._menusServiceProxy.getLanguageBot().subscribe(result => {

            this.languageBot = result;
            // this.languageBot.shift();



        });




    }



    deleteitemAddition(item) {
        if (item.id) {
            this.message.confirm(
                '',
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._itemAdditionServiceProxy.deleteItemAddition(item.id)
                            .subscribe(() => {
                                this.notify.success(this.l('successfullyDeleted'));
                                this.show();
                            });
                    }
                }
            );
        } else {
        }
    }


    deleteCategoryItem(index) {
        const control = (<UntypedFormArray>this.menuForm.controls['category']) as UntypedFormArray;
        let item = control.at(index).value;

        if (item.id) {
            this.message.confirm(
                '',
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._itemAdditionServiceProxy.deleteItemAdditionCategory(item.id)
                            .subscribe(() => {
                                this.notify.success(this.l('successfullyDeleted'));
                                control.removeAt(index);

                            });
                    }
                }
            );
        } else {
            control.removeAt(index);
        }
    }
    checkIsLoyality(){
        this._loyaltyServiceProxy.isLoyalTenant().subscribe(result => {
            this.isTenantLoyal = result;
        })
    }
    deleteCategoryItemFormTable(id) {
        const control = (<UntypedFormArray>this.menuForm.controls['category']) as UntypedFormArray;
        if (id) {
            this.message.confirm(
                '',
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._itemAdditionServiceProxy.deleteItemAdditionCategory(id)
                            .subscribe((result) => {
                                if (result == 1) {
                                    this.notify.success(this.l('successfullyDeleted'));
                                    this.reloadPage();
                                } else {
                                    this.message.error("", this.l("cantDeletAddition"));
                                }

                            });
                    }
                }
            );
        } else {
            control.removeAt(id);
        }
    }
    createNewCategory() {
        this.savingOnCreate = true;
        if (this.CreateCategorys.categoryName === null || this.CreateCategorys.categoryName === '' || this.CreateCategorys.categoryName === undefined ||
            this.CreateCategorys.categoryNameEnglish === null || this.CreateCategorys.categoryNameEnglish === '' || this.CreateCategorys.categoryNameEnglish === undefined || this.CreateItemsategorysList.length < 1) {
            this.submitted = true;
            this.savingOnCreate = false;
            return;
        }
        this.CreateCategorys.listItemAdditionsCategories = this.CreateItemsategorysList;
        for (let j = 0; j < this.CreateCategorys.listItemAdditionsCategories.length; j++) {
            if (this.CreateCategorys.listItemAdditionsCategories[j].name === '' || this.CreateCategorys.listItemAdditionsCategories[j].price === null ||
                this.CreateCategorys.listItemAdditionsCategories[j].name === null || this.CreateCategorys.listItemAdditionsCategories[j].price === undefined ||
                this.CreateCategorys.listItemAdditionsCategories[j].name === undefined ||
                this.CreateCategorys.listItemAdditionsCategories[j].nameEnglish === '' ||
                this.CreateCategorys.listItemAdditionsCategories[j].nameEnglish === null ||
                this.CreateCategorys.listItemAdditionsCategories[j].nameEnglish === undefined
            ) {
                this.submitted = true;
                this.savingOnCreate = false;
                return;
            }
        }
        let data = this.CreateCategorys;
        let category: CreateOrEditItemAdditionCategoryDto = new CreateOrEditItemAdditionCategoryDto();
        let empList = Array<ItemAdditionDto>();
        let Model = new AdditionsModelDto();
        category.id = data.categoryId;
        category.name = data.categoryName;
        category.nameEnglish = data.categoryNameEnglish;
        category.priority = data.categoryPriority;
        category.languageBotId = this.langId;
        category.menuType = this.menuType;

        category.isCondiments = data.isCondiments;
        category.isDeserts = data.isDeserts;
        category.isCrispy = data.isCrispy;
        category.isNon = data.isNon;

        if (category.priority === null || category.priority === undefined) {
            category.priority = 1;
        }

        for (let j = 0; j < data.listItemAdditionsCategories.length; j++) {
            let obj = data.listItemAdditionsCategories[j];


            let test = new ItemAdditionDto();

            //test.itemAdditionsCategoryId = categoryResponse;
            test.id = obj.id;
            test.itemId = obj.itemId;
            test.name = obj.name;
            test.nameEnglish = obj.nameEnglish;
            test.price = obj.price;
            test.sku = obj.sku;
            test.menuType = this.menuType;
            test.languageBotId = this.langId;
            test.imageUri = obj.imageUri;
            test.loyaltyPoints = obj.loyaltyPoints;

            empList.push(test);

        }

        Model.createOrEditItemAdditionCategoryDto = category
        Model.itemAdditions = empList

        this._itemAdditionServiceProxy.createItemAdditionCategory(Model)
            .pipe(finalize(() => {
                this.saving = false;
            }))
            .subscribe((categoryResponse) => {
                this.notify.info(this.l('savedSuccessfully'));
                this.modalService.dismissAll();
                this.reloadPage();
            },(error:any) =>{
                if(error){
                    this.saving= false;
                    this.submitted=false;
                }
            }
            );

    }

    editMenu() {
        this.savingOnEdit = true;
        if (this.category) {
            let data = this.category;
            let category: CreateOrEditItemAdditionCategoryDto = new CreateOrEditItemAdditionCategoryDto();
            if (this.category.categoryName === null || this.category.categoryName === '' || this.category.categoryName === undefined ||
                this.category.categoryNameEnglish === null || this.category.categoryNameEnglish === '' || this.category.categoryNameEnglish === undefined || this.category.listItemAdditionsCategories.length < 1) {
                this.submitted2 = true;
                this.savingOnEdit = false;
                return;
            }
            let empList = Array<ItemAdditionDto>();
            let Model = new AdditionsModelDto();
            let categoryItemsList = data.listItemAdditionsCategories;
            for (let j = 0; j < categoryItemsList.length; j++) {
                if (categoryItemsList[j].name === '' || categoryItemsList[j].price === null ||
                    categoryItemsList[j].name === null ||
                    categoryItemsList[j].name === undefined || categoryItemsList[j].price === undefined ||
                    categoryItemsList[j].nameEnglish === '' ||
                    categoryItemsList[j].nameEnglish === undefined ||
                    categoryItemsList[j].nameEnglish === null
                ) {
                    this.submitted2 = true;
                    this.savingOnEdit = false;
                    return;
                }
            }
            category.id = data.categoryId;
            category.name = data.categoryName;
            category.nameEnglish = data.categoryNameEnglish;
            category.priority = data.categoryPriority;
            category.languageBotId = this.langId;
            category.menuType = this.menuType;

            if (category.priority === null || category.priority === undefined) {
                category.priority = 1;
            }


            category.isCondiments = data.isCondiments;
            category.isDeserts = data.isDeserts;
            category.isCrispy = data.isCrispy;
            category.isNon = data.isNon;


            for (let j = 0; j < data.listItemAdditionsCategories.length; j++) {
                let obj = data.listItemAdditionsCategories[j];

                let test = new ItemAdditionDto();

                //test.itemAdditionsCategoryId = categoryResponse;
                test.id = obj.id;
                test.itemId = obj.itemId;
                test.name = obj.name;
                test.nameEnglish = obj.nameEnglish;
                test.price = obj.price;
                test.sku = obj.sku;
                test.menuType = this.menuType;
                test.languageBotId = this.langId;
                test.imageUri = obj.imageUri;
                test.loyaltyPoints = obj.loyaltyPoints;

                empList.push(test);

            }


            Model.createOrEditItemAdditionCategoryDto = category
            Model.itemAdditions = empList

            this._itemAdditionServiceProxy.createItemAdditionCategory(Model)
                .pipe(finalize(() => {
                    this.saving = false;
                }))
                .subscribe((categoryResponse) => {
                    this.notify.info(this.l('savedSuccessfully'));
                    this.modalService.dismissAll();
                    this.reloadPage();
                },(error:any) =>{
                    if(error){
                        this.saving= false;
                        this.submitted=false;
                    }
                }
                
                );
        }

    }



    onFileChange(event, category, item, index) {
        const reader = new FileReader();
        if (this.fromFileUplode) {

            index = this.itemIndex;
            item = this.item;
        }
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            compressor.compress(file, { quality: .5 }).then(cf => {
                let form = new FormData();
                form.append('FormFile', cf);
                this._itemsServiceProxy.getImageUrl(form).subscribe(res => {
                    item.imageUri = res['result'];
                    this.fromFileUplode = false;
                    this.categoryIndex = -1;
                    this.itemIndex = -1;
                });
            });
        }
    }

    getImage(category, index, add) {


        try {
            //const control =  ((<FormArray>this.menuForm.controls['category']).at(category).get('items') as FormArray).at(index).get('itemAdditionDtos') as FormArray;
            const control = ((<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray).at(index).get('itemAdditionDtos') as UntypedFormArray;

            let item = control.at(add).value;
            //let item = control.at(index).value;
            return item.imageUri;
        } catch {

            return "";
        }

    }


    openFileUploder(category, index, add) {

        this.categoryIndex = category;
        this.itemIndex = index;
        this.fromFileUplode = true;
        document.getElementById('uplode').click();
    }
    openFileUploder2(id, i) {
        this.fromFileUplode = true;
        const upload = document.getElementById("upload");
        upload.click();
        this.item = id;
        this.itemIndex = i;
    }


    fieldsChange(values: any, category): void {

        if (values.target.id == "isNon") {
            (document.getElementById("isNon") as HTMLInputElement).checked = true;
            (document.getElementById("isCondiments") as HTMLInputElement).checked = false;
            (document.getElementById("isDeserts") as HTMLInputElement).checked = false;
            (document.getElementById("isCrispy") as HTMLInputElement).checked = false;
            category.isCondiments = false;
            category.isCrispy = false;
            category.isDeserts = false;
        }
        else if (values.target.id == "isCondiments") {

            category.isNon = false;
            category.isCrispy = false;
            category.isDeserts = false;
            (document.getElementById("isNon") as HTMLInputElement).checked = false;
            (document.getElementById("isCondiments") as HTMLInputElement).checked = true;
            (document.getElementById("isDeserts") as HTMLInputElement).checked = false;
            (document.getElementById("isCrispy") as HTMLInputElement).checked = false;
        }
        else if (values.target.id == "isDeserts") {

            category.isNon = false;
            category.isCondiments = false;
            category.isCrispy = false;

            (document.getElementById("isNon") as HTMLInputElement).checked = false;
            (document.getElementById("isCondiments") as HTMLInputElement).checked = false;
            (document.getElementById("isDeserts") as HTMLInputElement).checked = true;
            (document.getElementById("isCrispy") as HTMLInputElement).checked = false;

        }
        else if (values.target.id == "isCrispy") {

            (document.getElementById("isNon") as HTMLInputElement).checked = false;
            (document.getElementById("isCondiments") as HTMLInputElement).checked = false;
            (document.getElementById("isDeserts") as HTMLInputElement).checked = false;
            (document.getElementById("isCrispy") as HTMLInputElement).checked = true;
            category.isNon = false;
            category.isCondiments = false;
            category.isDeserts = false;
        }

    }

    back() {
        this.router.navigate(['/app/main/menus/menus']);
    }
    // view items
    viewItems(modalSLCIM, items) {
        this.modalService.open(modalSLCIM, {
            scrollable: true,
            centered: true,
            size: 'lg'
        });

        this._menusServiceProxy
            .getItemAdditionChoicesModel(items.categoryId)
            .subscribe((res) => {
                items.listItemAdditionsCategories = res;
                this.category = items;
            });



    }
    CreateAddOns(createAddOns) {
        this.modalService.open(createAddOns, {
            scrollable: true,
            centered: true,
            size: 'lg'
        });
        //this.category= new ItemAdditionDto;//CreateOrEditItemAdditionDto();
        this.CreateCategorys = new GetItemAdditionsCategorysModel();
        this.CreateCategorys.listItemAdditionsCategories = [];
        this.CreateItemsategorysList = [];
        this.CreateItemsategorys = new ItemAdditionDto();
        this.addNewItem = false;
    }
    addItem() {
        this.addNewItem = true;
    }
    save(item) {

        this.category.listItemAdditionsCategories.push(item);
        this.addNewItem = false;
    }
    save2(item: ItemAdditionDto) {

        this.CreateItemsategorysList.push(item);

        this.CreateItemsategorys = new ItemAdditionDto();
        this.addNewItem = false;
    }
    goToDashboard() {
        this._router.navigate(['/app/main/dashboard']);
    }
    goToMenu() {
        this._router.navigate(['/app/main/menus/menus']);

    }

}