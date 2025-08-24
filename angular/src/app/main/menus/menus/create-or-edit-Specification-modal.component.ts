import {Component, ViewChild, Injector, Output, EventEmitter, OnInit} from '@angular/core';
import {ModalDirective} from 'ngx-bootstrap/modal';
import {finalize} from 'rxjs/operators';
import {
    MenusServiceProxy,
    CreateOrEditMenuDto,
    ItemCategoryServiceProxy,
    CreateOrEditMenuCategoryDto,
    ItemsServiceProxy,
    CreateOrEditItemDto, ItemDto, TeamInboxServiceProxy, ItemAdditionDto, RestaurantsTypeEunm, RType, LanguageBot, ItemAdditionServiceProxy, CreateOrEditItemAdditionCategoryDto, SpecificationsDto, SpecificationChoicesDto
} from '@shared/service-proxies/service-proxies';
import {AppComponentBase} from '@shared/common/app-component-base';
import * as moment from 'moment';
import {UntypedFormArray, UntypedFormBuilder, UntypedFormGroup, Validators} from '@angular/forms';
import { addOnsLookupTableModalComponent } from './addOns-lookup-table-modal.component';


declare var ImageCompressor: any;

const compressor = new ImageCompressor();

@Component({
    selector: 'createOrEditSpecificationModal',
   // styleUrls: ['./model-menus.component.less'],
    templateUrl: './create-or-edit-Specification-modal.component.html'
})

export class CreateOrEditSpecificationModalComponent extends AppComponentBase implements OnInit {


    @ViewChild('createOrEditModal', {static: true}) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    menu: CreateOrEditMenuDto = new CreateOrEditMenuDto();

    menuItemStatusName = '';
    menuCategoryName = '';

    restaurantsTypeEunm = RestaurantsTypeEunm;
    enumKeys = [];

    menuForm: UntypedFormGroup;
    menuId = null;
    langId = 1;
    itemIndex = -1;
    itemAddition = -1;
    categoryIndex = -1;
    fromFileUplode = false;
    arrayAddition: any;

    rType: RType[];

    languageBot: LanguageBot[];

    menuType: number;
    priorityMenu: number;

    isNf: boolean;
    categoryTypess: RestaurantsTypeEunm;
    selectedValue: any;

    constructor(
        injector: Injector,
        private _teamInboxServiceProxy: TeamInboxServiceProxy,
        private _menusServiceProxy: MenusServiceProxy,
        private fb: UntypedFormBuilder,
        private _menuCategoriesServiceProxy: ItemCategoryServiceProxy,
        private _itemAdditionServiceProxy: ItemAdditionServiceProxy,
        private _itemsServiceProxy: ItemsServiceProxy,
        // private _itemAdditionServiceProxy:ItemAdditionServiceProxy,
    ) {
        super(injector);
       // this.ngOnInit();
        this.enumKeys = Object.keys(this.restaurantsTypeEunm);
        // set default value
    }

    public get selectedCategoryType(): RestaurantsTypeEunm {
        return this.selectedValue ? this.selectedValue.value : null;
    }


    show(): void {
     ///   this.ngOnInit();
        this.menuForm = this.fb.group({
            menuName: [''],
            menuNameEnglish: [''],
            menuDescription: [''],
            menuDescriptionEnglish: [''],
            priority: 0,
            restaurantsType: 0,
            languageBotId: 1,
            id: null,
            'category': this.fb.array([
                this.initCategory()
            ]),


        });
            let categoryList = [];
            this._menusServiceProxy.getSpecificationsCategorys().subscribe(res => {

                res.forEach(category => {

                    categoryList.push(this.bindCategory(category.categoryId,category.categoryPriority, category.categoryName, category.categoryNameEnglish, category.isRequired, category.isMultipleSelection ,category.listSpecificationChoices,category.maxSelectNumber));
                });

                                 
                    this.menuForm = this.fb.group({
                        'category': this.fb.array(categoryList)
                    });

                    this.active = true;
                    this.modal.show();
            });



    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }

    ngOnInit() {


        this.isNf = (this.appSession.tenantId === 4)|| (this.appSession.tenantId ===7)|| (this.appSession.tenantId ===14);


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

        this.menuForm = this.fb.group({
            menuName: [''],
            menuNameEnglish: [''],
            menuDescription: [''],
            menuDescriptionEnglish: [''],
            restaurantsType: 0,
            priority: 0,
            languageBotId: 1,
            id: null,
            'category': this.fb.array([
                this.initCategory()
            ])
        });


    }

    initCategory() {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            'SpecificationDescription': ['', [Validators.required]],
            'SpecificationDescriptionEnglish': ['', [Validators.required]],
            'priority': [0, [Validators.required]],
            'IsMultipleSelection': [true, [Validators.required]],
            'IsRequired': [true, [Validators.required]],
            // ---------------------------------------------------------------------
            'items': this.fb.array([
                this.initItem()
            ])
        });
    }
    initCategorySpecification() {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            'name': ['', [Validators.required]],
            'nameEnglish': ['', [Validators.required]],
            'priority': [0, [Validators.required]],
            // ---------------------------------------------------------------------
            'items': this.fb.array([
                this.initItemSpecification()
            ])
        });
    }


    bindCategory(id,priority, name, nameEnglish,isRequired,isMultipleSelection, itemAdditionDtos,maxSelectNumber) {

                         
        let data = [];
          data.push(this.bindItem(itemAdditionDtos,id));
        return this.fb.group({
            'name': [name, [Validators.required]],
            'nameEnglish': [nameEnglish, [Validators.required]],
            'priority': [priority, [Validators.required]],
            'isRequired': [isRequired, [Validators.required]],
            'isMultipleSelection': [isMultipleSelection, [Validators.required]],
            'items': this.fb.array(data),
            'maxSelectNumber': [maxSelectNumber, [Validators.required]],
            id: id
        });
    }

    initItemAddition() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'name': ['', [Validators.required]],
            'nameEnglish': ['', [Validators.required]],
            'price': [0, [Validators.required]],
            'sku': '',
            'isInService': [true, [Validators.required]],
            itemId: 0
        });
    }

    initItemSpecificationChoices() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'SpecificationChoiceDescription': ['', [Validators.required]],
            'SpecificationChoiceDescriptionEnglish': ['', [Validators.required]],
            'Price': [0, [Validators.required]],
            'SKU': '',

            itemId: 0
        });
    }

    initAddition() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'name': ['', [Validators.required]],
            'nameEnglish': ['', [Validators.required]],
            'price': [0, [Validators.required]],
            'sku': '',
            'isInService': [true, [Validators.required]],
            itemId: 0
        });
    }

    bindinitSpecificationChoice(id, name, nameEnglish, price, itemId, sKU,specificationId) {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'id': [id, [Validators.required]],
            'specificationId': [specificationId, [Validators.required]],
            'SpecificationChoiceDescription': [name, [Validators.required]],
            'SpecificationChoiceDescriptionEnglish': [nameEnglish, [Validators.required]],
            'Price': [price, [Validators.required]],
            'SKU': sKU,
            itemId: itemId
        });
    }

    bindinitAddition(id, name, nameEnglish, price, itemId, sKU, isInService) {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'id': [id, [Validators.required]],
            'name': [name, [Validators.required]],
            'nameEnglish': [nameEnglish, [Validators.required]],
            'price': [price, [Validators.required]],
            'sku': sKU,
            'isInService': isInService,
            itemId: itemId
        });
    }


    initItem() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'itemName': ['', [Validators.required]],
            'itemNameEnglish': ['', [Validators.required]],
            'price': [0, [Validators.required]],
            'priority': [0, [Validators.required]],
            'itemDescription': ['', [Validators.required]],
            'itemDescriptionEnglish': ['', [Validators.required]],
            'imageUri': ['', [Validators.required]],
            'sku': '',
            'isInService': [true, [Validators.required]],
            'itemAdditionDtos': this.fb.array([
                // this.initAddition()
            ]),
            'itemSpecificationDtos': this.fb.array([
                // this.initAddition()
            ]),
            menuId: '',
            itemCategoryId: ''
        });
    }

    initItemSpecification() {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'itemName': ['', [Validators.required]],
            'itemNameEnglish': ['', [Validators.required]],
            'price': [0, [Validators.required]],
            'priority': [0, [Validators.required]],
            'itemDescription': ['', [Validators.required]],
            'itemDescriptionEnglish': ['', [Validators.required]],
            'imageUri': ['', [Validators.required]],
            'sku': '',
            'isInService': [true, [Validators.required]],
            'itemAdditionDtos': this.fb.array([
                // this.initAddition()
            ]),
            'itemSpecificationDtos': this.fb.array([
                // this.initAddition()
            ]),
            menuId: '',
            itemCategoryId: ''
        });
    }

    bindItem(itemAddition,categoryId) {
        let data = [];
                         
        itemAddition.forEach(itemAddition => {

            data.push(this.bindinitSpecificationChoice(itemAddition.id, itemAddition.specificationChoiceDescription, itemAddition.specificationChoiceDescriptionEnglish, itemAddition.price, null, itemAddition.sku,itemAddition.specificationId));
        });
                         
        this.arrayAddition = itemAddition;
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'itemName': ['', [Validators.required]],
            'itemNameEnglish': ['', [Validators.required]],
            'price': [0, [Validators.required]],
            'priority': [0, [Validators.required]],
            'itemDescription': ['', [Validators.required]],
            'itemDescriptionEnglish': ['', [Validators.required]],
            'imageUri': ['', [Validators.required]],
            'sku': '',
            'isInService': [true, [Validators.required]],
            //'itemAdditionDtos': this.fb.array(data),
            'itemSpecificationDtos': this.fb.array(data),
            itemCategoryId: categoryId,
        });
    }


    initZ() {
        return this.fb.group({
            //  ---------------------forms fields on z level ------------------------
            'Z': ['Z', [Validators.required, Validators.pattern('[0-9]{3}')]],
            // ---------------------------------------------------------------------
        });
    }

    addCategory() {

        const control = <UntypedFormArray>this.menuForm.controls['category'];
        control.push(this.initCategory());
    }

    addCategorySpecification() {

        const control = <UntypedFormArray>this.menuForm.controls['category'];
        control.push(this.initCategorySpecification());
    }



    addItemAddition(category, index) {

        const control = ((<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray).at(index).get('itemSpecificationDtos') as UntypedFormArray;
        control.push(this.initItemSpecificationChoices());
    }


    deleteitemAddition(category, index, add) {
        const control = ((<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray).at(index).get('itemSpecificationDtos') as UntypedFormArray;

        let item = control.at(add).value;
        if (item.id) {
            this.message.confirm(
                '',
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._itemAdditionServiceProxy.deleteSpecificationChoices(item.id)
                            .subscribe(() => {
                                this.notify.success(this.l('successfullyDeleted'));
                                control.removeAt(add);
                            });
                    }
                }
            );
        } else {
            control.removeAt(add);
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
                        this._itemAdditionServiceProxy.deleteSpecification(item.id)
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



    deleteItemCategoryItem(category, index, add) {
        const control1 = (<UntypedFormArray>this.menuForm.controls['category']) as UntypedFormArray;
        let categ = control1.at(index).value;
        const control = ((<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray).at(index).get('itemSpecificationDtos') as UntypedFormArray;
        let item = control.at(add).value;
        if (item.id) {
            this.message.confirm(
                '',
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._itemAdditionServiceProxy.deleteSpecificationChoices(item.id)
                            .subscribe(() => {
                                this.notify.success(this.l('successfullyDeleted'));
                                control.removeAt(add);
                            });
                            this._itemAdditionServiceProxy.deleteSpecification(categ.id)
                            .subscribe(() => {
                                this.notify.success(this.l('successfullyDeleted'));
                                control1.removeAt(index);
                            });
                    }
                }
            );
        } else {
            control.removeAt(add);
        }
    }
    addMenu() {
                         
        let data = this.menuForm.value;

        for (let i = 0; i < data.category.length; i++) {

            let category: SpecificationsDto = new SpecificationsDto();

                             
            category.id = data.category[i].id;
            category.isMultipleSelection = data.category[i].isMultipleSelection;
            category.maxSelectNumber = data.category[i].maxSelectNumber;
            category.specificationDescription = data.category[i].name;
            category.priority = data.category[i].priority;
            category.languageBotId = this.langId;
            category.specificationDescriptionEnglish = data.category[i].nameEnglish;

            this._itemAdditionServiceProxy.createOrEditSpecifications(category)
            .pipe(finalize(() => {
                this.saving = false;
            }))
            .subscribe((categoryResponse) => {
                                 
                                let test = new SpecificationChoicesDto();
                for (let j = 0; j < data.category[i].items.length; j++) {
                    let obj = data.category[i].items[j];

                    let empList = Array<SpecificationChoicesDto>();


                            obj.itemSpecificationDtos.forEach(itemAddition => {


                                test.specificationId=categoryResponse;
                                test.id = itemAddition.id;
                                test.specificationChoiceDescription = itemAddition.SpecificationChoiceDescription;
                                test.specificationChoiceDescriptionEnglish = itemAddition.SpecificationChoiceDescriptionEnglish;
                                test.sku = itemAddition.SKU;
                                test.price = itemAddition.Price;
                                //test.sku = itemAddition.sku;
                                //test.menuType = this.menuType;
                                test.languageBotId=this.langId;
                                                 
                                empList.push(test);
                                

                            });


                            this._itemAdditionServiceProxy.createOrEditSpecificationChoices(empList).subscribe(response => {

                                this._itemAdditionServiceProxy.createpecificationsDet(response).subscribe(response2 => {



                                });

                            });





                }



            });

        }

        this.notify.info(this.l('savedSuccessfully'));
        this.close();
        this.modalSave.emit(null);

    }

    addOrSaveCategory(menuId) {
        let data = this.menuForm.value;


        for (let i = 0; i < data.category.length; i++) {
            let category: CreateOrEditMenuCategoryDto = new CreateOrEditMenuCategoryDto();

            category.name = data.category[i].name;
            category.nameEnglish = data.category[i].nameEnglish;
            category.menuType = this.menuType;
            category.priority = this.priorityMenu;
            category.languageBotId= this.langId
            if (data.category[i].id) {
                category.id = data.category[i].id;
            }
            this._menuCategoriesServiceProxy.createOrEdit(category)
                .pipe(finalize(() => {
                    this.saving = false;
                }))
                .subscribe((categoryResponse) => {
                    for (let j = 0; j < data.category[i].items.length; j++) {


                        let item = new CreateOrEditItemDto();
                        let obj = data.category[i].items[j];


                        item.sku = obj.sku;
                        item.isInService = obj.isInService;

                        item.categoryNames = data.category[i].categoryNames;
                        item.categoryNamesEnglish = data.category[i].categoryNamesEnglish;
                        item.itemDescription = obj.itemDescription;
                        item.itemDescriptionEnglish = obj.itemDescriptionEnglish;
                        item.itemName = obj.itemName;
                        item.itemNameEnglish = obj.itemNameEnglish;

                        item.itemCategoryId = category.id ? category.id : categoryResponse;
                        item.menuId = this.menuId ? this.menuId : menuId;
                        // item.isInService = false;
                        item.imageUri = obj.imageUri;
                        item.priority = parseInt(obj.priority);
                        item.creationTime = moment();
                        item.deletionTime = moment();

                        item.menuType = this.menuType;
                        item.languageBotId=this.langId;
                        // let data2 = [];


                        let empList = Array<ItemAdditionDto>();


                        obj.itemAdditionDtos.forEach(itemAddition => {

                            let test = new ItemAdditionDto();

                            test.id = itemAddition.id;
                            test.itemId = itemAddition.itemId;
                            test.name = itemAddition.name;
                            test.nameEnglish = itemAddition.nameEnglish;
                            test.price = itemAddition.price;
                            test.sku = itemAddition.sku;
                            test.menuType = this.menuType;
                            test.languageBotId=this.langId;
                            empList.push(test);

                        });


                        item.itemAdditionDtos = empList;
                        item.price = parseFloat(obj.price);
                        if (obj.id) {
                            item.id = obj.id;
                        }
                        item.lastModificationTime = moment();

                        this._itemsServiceProxy.createOrEdit(item)
                            .pipe(finalize(() => {
                                this.saving = false;
                            }))
                            .subscribe(() => {

                                this.notify.info(this.l('savedSuccessfully'));
                                this.close();
                                this.modalSave.emit(null);
                            });

                        this.notify.info(this.l('savedSuccessfully'));


                        // this.close();
                    }
                    // this.modalSave.emit(null);
                });
        }

    }

    onFileChange(event, category, index) {
        const reader = new FileReader();
        if (this.fromFileUplode) {
            category = this.categoryIndex;
            index = this.itemIndex;
        }
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);

            const control = (<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray;

            compressor.compress(file, {quality: .5}).then(cf => {
                let form = new FormData();
                form.append('FormFile', cf);
                this._itemsServiceProxy.getImageUrl(form).subscribe(res => {
                    control.at(index).get('imageUri').setValue(res['result']);
                    this.fromFileUplode = false;
                    this.categoryIndex = -1;
                    this.itemIndex = -1;
                });
            });
        }
    }

    getImage(category, index) {

        const control = (<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray;

        let item = control.at(index).value;
        return item.imageUri;
    }

    openFileUploder(category, index) {
        this.categoryIndex = category;
        this.itemIndex = index;
        this.fromFileUplode = true;
        document.getElementById('uplode').click();
    }


}
