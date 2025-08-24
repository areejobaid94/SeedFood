import {Component, ViewChild, Injector, Output, EventEmitter, OnInit} from '@angular/core';
import {ModalDirective} from 'ngx-bootstrap/modal';
import {finalize} from 'rxjs/operators';
import {
    MenusServiceProxy,
    CreateOrEditMenuDto,
    ItemCategoryServiceProxy,
    CreateOrEditMenuCategoryDto,
    ItemsServiceProxy,
    CreateOrEditItemDto, ItemDto, TeamInboxServiceProxy, ItemAdditionDto, RestaurantsTypeEunm, RType, LanguageBot, ItemAdditionServiceProxy, CreateOrEditItemAdditionCategoryDto, CreateOrEditItemAdditionDto
} from '@shared/service-proxies/service-proxies';
import {AppComponentBase} from '@shared/common/app-component-base';
import * as moment from 'moment';
import {UntypedFormArray, UntypedFormBuilder, UntypedFormGroup, Validators} from '@angular/forms';
import { addOnsLookupTableModalComponent } from './addOns-lookup-table-modal.component';


declare var ImageCompressor: any;

const compressor = new ImageCompressor();

@Component({
    selector: 'createOrEditAddModal',
   // styleUrls: ['./model-menus.component.less'],
    templateUrl: './create-or-edit-Add-modal.component.html'
})

export class CreateOrEditAddModalComponent extends AppComponentBase implements OnInit {

    item= [];
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
    checkedCategoryList:any;
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
        this.ngOnInit();
        this.enumKeys = Object.keys(this.restaurantsTypeEunm);
        // set default value
    }

    public get selectedCategoryType(): RestaurantsTypeEunm {
        return this.selectedValue ? this.selectedValue.value : null;
    }


    show(): void {


        this.ngOnInit();
        this.menuForm = this.fb.group({
            menuName: [''],
            menuNameEnglish: [''],
            menuDescription: [''],
            menuDescriptionEnglish: [''],
            priority: 0,
            restaurantsType: 0,
            languageBotId: 1,
            id: null,

            'category': this.fb.array([this.initCategory()]),


        });
            let categoryList = [];
            this._menusServiceProxy.getAddOnsCategorys().subscribe(res => {

                res.forEach(category => {

                    categoryList.push(this.bindCategory(category.categoryId,category.categoryPriority, category.categoryName, category.categoryNameEnglish, category.listItemAdditionsCategories,category.isCondiments,category.isCrispy,category.isDeserts,category.isNon));
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
            'category': this.fb.array([this.initCategory()])
        });


    }

    initCategory() {
        return this.fb.group({
            //  ---------------------forms fields on x level ------------------------
            'name': ['', [Validators.required]],
            'nameEnglish': ['', [Validators.required]],
            'priority': [0, [Validators.required]],
            'isCondiments':[false],
            'isCrispy':[false],
            'isDeserts':[false],
            'isNon':[true],
            // ---------------------------------------------------------------------
            'items': this.fb.array([
                this.initItem()
            ])
        });
    }


    bindCategory(id,priority, name, nameEnglish, itemAdditionDtos,isCondiments,isCrispy,isDeserts,isNon) {

        // this.isCondiments  = isCondiments;
        // this.isDeserts= isDeserts;
        // this.isCrispy= isCrispy;
        // this.isNon= isNon;


        let data = [];
          data.push(this.bindItem(itemAdditionDtos,id));
        return this.fb.group({
            'name': [name, [Validators.required]],
            'nameEnglish': [nameEnglish, [Validators.required]],
            'priority': [priority, [Validators.required]],
            'isCondiments':[isCondiments],
            'isCrispy':[isCrispy],
            'isDeserts':[isDeserts],
            'isNon':[isNon],
            'items': this.fb.array(data),
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
            'imageUri': [true, [Validators.required]],
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
            'imageUri': [true, [Validators.required]],
            itemId: 0
        });
    }


    bindinitAddition(id, name, nameEnglish, price, itemId, sKU, isInService,imageUri) {
        return this.fb.group({
            //  ---------------------forms fields on y level ------------------------
            'id': [id, [Validators.required]],
            'name': [name, [Validators.required]],
            'nameEnglish': [nameEnglish, [Validators.required]],
            'price': [price, [Validators.required]],
            'sku': sKU,
            'isInService': isInService,
            'imageUri': [imageUri],
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
            menuId: '',
            itemCategoryId: ''
        });
    }

    bindItem(itemAddition,categoryId) {
        let data = [];
        itemAddition.forEach(itemAddition => {

            data.push(this.bindinitAddition(itemAddition.id, itemAddition.name, itemAddition.nameEnglish, itemAddition.price, null, itemAddition.sku, itemAddition.isInService,itemAddition.imageUri));
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
            'itemAdditionDtos': this.fb.array(data),
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


    addItemAddition(category, index) {

        const control = ((<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray).at(index).get('itemAdditionDtos') as UntypedFormArray;
        control.push(this.initItemAddition());
    }


    deleteitemAddition(category, index, add) {
        
        const control = ((<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray).at(index).get('itemAdditionDtos') as UntypedFormArray;

        let item = control.at(add).value;
        if (item.id) {
            this.message.confirm(
                '',
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._itemAdditionServiceProxy.deleteItemAddition(item.id)
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

    addMenu() {
        let data = this.menuForm.value;

        for (let i = 0; i < data.category.length; i++) {

            let category: CreateOrEditItemAdditionCategoryDto = new CreateOrEditItemAdditionCategoryDto();

                             
            category.id = data.category[i].id;
            category.name = data.category[i].name;
            category.nameEnglish = data.category[i].nameEnglish;
            category.priority = data.category[i].priority;
            category.languageBotId = this.langId;
            category.menuType = this.menuType;


                // category.isCondiments = data.category[i].isCondiments;
                // category.isDeserts = data.category[i].isDeserts;
                // category.isCrispy = data.category[i].isCrispy;

            category.isCondiments = data.category[i].isCondiments;
            category.isDeserts = data.category[i].isDeserts;
            category.isCrispy = data.category[i].isCrispy;
            category.isNon = data.category[i].isNon;


            // this._itemAdditionServiceProxy.createItemAdditionCategory(category)
            // .pipe(finalize(() => {
            //     this.saving = false;
            // }))
            // .subscribe((categoryResponse) => {

            //     for (let j = 0; j < data.category[i].items.length; j++) {
            //         let obj = data.category[i].items[j];

            //         let empList = Array<CreateOrEditItemAdditionDto>();


            //                 obj.itemAdditionDtos.forEach(itemAddition => {

            //                     let test = new CreateOrEditItemAdditionDto();

            //                     test.itemAdditionsCategoryId=categoryResponse;
            //                     test.id = itemAddition.id;
            //                     test.itemId = itemAddition.itemId;
            //                     test.name = itemAddition.name;
            //                     test.nameEnglish = itemAddition.nameEnglish;
            //                     test.price = itemAddition.price;
            //                     test.sku = itemAddition.sku;
            //                     test.menuType = this.menuType;
            //                     test.languageBotId=this.langId;
            //                     test.imageUri=itemAddition.imageUri;

            //                     empList.push(test);
            //                     this._itemAdditionServiceProxy.createOrEdit(test).subscribe(response => {

            //                         this._itemAdditionServiceProxy.createDet(response).subscribe(response2 => {



            //                         });

            //                     });

            //                 });








            //     }



            // });

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

    onFileChange(event, category, index, add) {
                         
        const reader = new FileReader();
        if (this.fromFileUplode) {

            index = this.itemIndex;
        }
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
        //const control =  ((<FormArray>this.menuForm.controls['category']).at(category).get('items') as FormArray).at(index).get('itemAdditionDtos') as FormArray;

            const control = ((<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray).at(index).get('itemAdditionDtos') as UntypedFormArray;


            compressor.compress(file, {quality: .5}).then(cf => {
                let form = new FormData();
                form.append('FormFile', cf);
                this._itemsServiceProxy.getImageUrl(form).subscribe(res => {
                    control.at(add).get('imageUri').setValue(res['result']);
                    this.getImage(category,index,add)
                    this.fromFileUplode = false;
                    this.categoryIndex = -1;
                    this.itemIndex = -1;
                });
            });
        }
    }

    getImage(category, index, add) {


        try{
        //const control =  ((<FormArray>this.menuForm.controls['category']).at(category).get('items') as FormArray).at(index).get('itemAdditionDtos') as FormArray;
        const control = ((<UntypedFormArray>this.menuForm.controls['category']).at(category).get('items') as UntypedFormArray).at(index).get('itemAdditionDtos') as UntypedFormArray;

        let item = control.at(add).value;
        //let item = control.at(index).value;
        return item.imageUri;
        }catch{

            return "";
        }

    }


    openFileUploder(category, index, add,item) {
        
        this.categoryIndex = category;
        this.itemIndex = add;
        this.fromFileUplode = true;
        document.getElementById('uplode').click();
    }


    fieldsChange(values:any,category):void {
                         
        const control = (<UntypedFormArray>this.menuForm.controls['category']) as UntypedFormArray;
        let item = control.at(category).value;

        if(values.target.id=="isNon"+category){
             (document.getElementById("isNon"+category) as HTMLInputElement).checked = true;
            (document.getElementById("isCondiments"+category) as HTMLInputElement).checked = false;
            (document.getElementById("isDeserts"+category) as HTMLInputElement).checked = false;
             (document.getElementById("isCrispy"+category) as HTMLInputElement).checked = false;
            control.at(category).value.isCondiments=false;
            control.at(category).value.isCrispy=false;
            control.at(category).value.isDeserts=false;
        }
       else if(values.target.id=="isCondiments"+category){

        control.at(category).value.isNon=false;
        control.at(category).value.isCrispy=false;
        control.at(category).value.isDeserts=false;
             (document.getElementById("isNon"+category) as HTMLInputElement).checked = false;
             (document.getElementById("isCondiments"+category) as HTMLInputElement).checked = true;
             (document.getElementById("isDeserts"+category) as HTMLInputElement).checked = false;
             (document.getElementById("isCrispy"+category) as HTMLInputElement).checked = false;
        }
       else if(values.target.id=="isDeserts"+category){

        control.at(category).value.isNon=false;
        control.at(category).value.isCondiments=false;
        control.at(category).value.isCrispy=false;

             (document.getElementById("isNon"+category) as HTMLInputElement).checked = false;
             (document.getElementById("isCondiments"+category) as HTMLInputElement).checked = false;
            (document.getElementById("isDeserts"+category) as HTMLInputElement).checked = true;
             (document.getElementById("isCrispy"+category) as HTMLInputElement).checked = false;

        }
      else  if(values.target.id=="isCrispy"+category){

             (document.getElementById("isNon"+category) as HTMLInputElement).checked = false;
             (document.getElementById("isCondiments"+category) as HTMLInputElement).checked = false;
             (document.getElementById("isDeserts"+category) as HTMLInputElement).checked = false;
             (document.getElementById("isCrispy"+category) as HTMLInputElement).checked = true;
            control.at(category).value.isNon=false;
            control.at(category).value.isCondiments=false;
            control.at(category).value.isDeserts=false;
        }

      }

}
