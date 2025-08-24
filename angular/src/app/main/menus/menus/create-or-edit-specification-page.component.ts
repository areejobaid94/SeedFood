import {
  Component,
  Injector,
  Output,
  EventEmitter,
  OnInit,
  ViewChild,
} from "@angular/core";
import { finalize } from "rxjs/operators";
import {
  MenusServiceProxy,
  CreateOrEditMenuDto,
  RestaurantsTypeEunm,
  RType,
  LanguageBot,
  ItemAdditionServiceProxy,
  SpecificationsDto,
  SpecificationChoicesDto,
  ItemSpecification,
  SpecificationChoice,
  SpecificationsModelDto,
  GetSpecificationsCategorysModel,
  LoyaltyServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";

import { UntypedFormGroup } from "@angular/forms";
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../../services/dark-mode.service";

import { NgbModal, NgbModalConfig } from "@ng-bootstrap/ng-bootstrap";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { Router } from "@angular/router";
import * as rtlDetect from 'rtl-detect';
import { LazyLoadEvent } from "primeng/api";

@Component({
  selector: "app-create-or-edit-specification-page",
  templateUrl: "./create-or-edit-specification-page.component.html",
})
export class CreateOrEditSpecificationPageComponent extends AppComponentBase
  implements OnInit {
  theme: string;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  active = false;
  saving = false;
  currency = '';

  menu: CreateOrEditMenuDto = new CreateOrEditMenuDto();

  menuItemStatusName = "";
  menuCategoryName = "";

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
  //
  public CreateItemsategorysList: SpecificationChoice[];

  category = new GetSpecificationsCategorysModel();
  addNewItem = false;
  CreateCategorys: ItemSpecification = new ItemSpecification();
  CreateItemsategorys: SpecificationChoice = new SpecificationChoice();
  submitted = false;
  submitted2 = false;
  savingOnCreate = false;
  savingOnEdit = false;
  isTenantLoyal:boolean;
  isArabic = false;


  constructor(
    injector: Injector,
    private _menusServiceProxy: MenusServiceProxy,
    private _itemAdditionServiceProxy: ItemAdditionServiceProxy,
    public darkModeService: DarkModeService,
    private modalService: NgbModal,
    private _router: Router,
    config: NgbModalConfig,
    private _loyaltyServiceProxy: LoyaltyServiceProxy,

  ) {
    super(injector);
    config.backdrop = "static";
    config.keyboard = false;
    this.enumKeys = Object.keys(this.restaurantsTypeEunm);
  }
  deleteItem(id) {
    for (let i = 0; i < this.CreateItemsategorysList.length; i++) {
      if (
        this.CreateItemsategorysList.indexOf(
          this.CreateItemsategorysList[i]
        ) === id
      ) {
        this.CreateItemsategorysList.splice(i, 1);
        break;
      }
    }
  }
  deleteItem2(item, id) {
    if (item.id) {
      this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
        if (isConfirmed) {
          this._itemAdditionServiceProxy
            .deleteSpecificationChoices(item.id)
            .subscribe(() => {
              this.notify.success(this.l("successfullyDeleted"));
            });
          this._itemAdditionServiceProxy
            .deleteSpecification(item.id)
            .subscribe(() => {
              this.notify.success(this.l("successfullyDeleted"));
              for (
                let i = 0;
                i < this.category.listSpecificationChoices.length;
                i++
              ) {
                if (
                  this.category.listSpecificationChoices.indexOf(
                    this.category.listSpecificationChoices[i]
                  ) === id
                ) {
                  this.category.listSpecificationChoices.splice(i, 1);
                  break;
                }
              }
              this.show();
            });
        }
      });
    } else {
      let lastElement = this.category.listSpecificationChoices.splice(-1, 1);
    }
  }

  addItem2() {
    if (this.CreateCategorys.specificationDescription === null || this.CreateCategorys.specificationDescription === undefined || this.CreateCategorys.specificationDescription === '' ||
      this.CreateCategorys.specificationDescriptionEnglish === null || this.CreateCategorys.specificationDescriptionEnglish === undefined || this.CreateCategorys.specificationDescriptionEnglish === '') {
      this.submitted = true;
      return;
    }
    this.submitted = false;
    if (this.CreateItemsategorysList.length > 0) {
      for (let j = 0; j < this.CreateItemsategorysList.length; j++) {
        if (this.CreateItemsategorysList[j].specificationChoiceDescription === '' || this.CreateItemsategorysList[j].price === null ||
          this.CreateItemsategorysList[j].specificationChoiceDescription === null || this.CreateItemsategorysList[j].price === undefined ||
          this.CreateItemsategorysList[j].specificationChoiceDescription === undefined ||
          this.CreateItemsategorysList[j].specificationChoiceDescriptionEnglish === '' ||
          this.CreateItemsategorysList[j].specificationChoiceDescriptionEnglish === null ||
          this.CreateItemsategorysList[j].specificationChoiceDescriptionEnglish === undefined
        ) {
          this.submitted = true;
          return;
        }
      }
    }

    this.submitted = false;
    this.CreateItemsategorysList.push({
      id: 0,
      specificationChoiceDescription: '',
      specificationChoiceDescriptionEnglish: '',
      sku: '',
      isInService: false,
      languageBotId: 0,
      specificationId: 0,
      tenantId: 0,

      price: 0,
      uniqueId: 0,
      specificationUniqueId: 0,
      loyaltyPoints: 0,
      originalLoyaltyPoints: 0,
      isOverrideLoyaltyPoints: false,
      loyaltyDefinitionId:0,
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
    if (this.CreateItemsategorysList.length > 0) {
      for (let j = 0; j < this.category.listSpecificationChoices.length; j++) {
        if (this.category.listSpecificationChoices[j].specificationChoiceDescription === '' || this.category.listSpecificationChoices[j].price === null ||
          this.category.listSpecificationChoices[j].specificationChoiceDescription === null || this.category.listSpecificationChoices[j].price === undefined ||
          this.category.listSpecificationChoices[j].specificationChoiceDescription === undefined ||
          this.category.listSpecificationChoices[j].specificationChoiceDescriptionEnglish === '' ||
          this.category.listSpecificationChoices[j].specificationChoiceDescriptionEnglish === null ||
          this.category.listSpecificationChoices[j].specificationChoiceDescriptionEnglish === undefined
        ) {
          this.submitted2 = true;
          return;
        }
      }
    }
    this.submitted2 = false;
    this.category.listSpecificationChoices.push({
      id: 0,
      tenantId: 0,
      specificationChoiceDescription:'',
      specificationChoiceDescriptionEnglish :'',
      sku: '',
      languageBotId: 0,
      specificationId: 0,
      price: 0,
      loyaltyPoints: 0,
      originalLoyaltyPoints: 0,
      isOverrideLoyaltyPoints: false,
      loyaltyDefinitionId: 0,
      createdBy: 0,
      init: function (_data?: any): void {
        throw new Error('Function not implemented.');
      },
      toJSON: function (data?: any) {
        throw new Error('Function not implemented.');
      }
    });
  }
  checkIsLoyality(){
    this._loyaltyServiceProxy.isLoyalTenant().subscribe(result => {
        this.isTenantLoyal = result;
    })
}


  show(event?: LazyLoadEvent): void {
    this.submitted = false;
    this.submitted2 = false;
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
  }
    this.primengTableHelper.showLoadingIndicator();
    this._menusServiceProxy.getSpecificationsCategory(
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator,event)
    ).subscribe(res => {
    this.primengTableHelper.getSkipCount(this.paginator, event),
    this.primengTableHelper.getMaxResultCount(this.paginator, event);
    this.primengTableHelper.totalRecordsCount = res.totalCount;
    this.primengTableHelper.records = res.lstSpecificationsCategory;
    this.primengTableHelper.hideLoadingIndicator();

    });
  }

  ngOnInit() {
    this.checkIsLoyality();
    this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
    this.currency = this.appSession.tenant.currencyCode;
    this.CreateItemsategorysList = [];

    this.theme = ThemeHelper.getTheme();

    this.isNf =
      this.appSession.tenantId === 4 ||
      this.appSession.tenantId === 7 ||
      this.appSession.tenantId === 14;

    this._menusServiceProxy
      .getRType(this.appSession.tenantId)
      .subscribe((result) => {
        if (this.isNf) {
          this.rType = result;
          this.rType.shift();
        } else {
          this.rType = result;
        }
      });

    this._menusServiceProxy.getLanguageBot().subscribe((result) => {
      this.languageBot = result;
    });
  }

  deleteCategoryItem(id) {
    if (id) {
      this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
        if (isConfirmed) {
          this._itemAdditionServiceProxy
            .deleteSpecification(id)
            .subscribe((result) => {
              if(result){
                this.notify.success(this.l("successfullyDeleted"));
                this.reloadPage();
              }else{
                this.message.error("", this.l("cantDeletOption"));

              }
        
            });
        }
      });
    } else {
    }
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
}



  createNewCategory() {
    this.savingOnCreate = true;
    if (this.CreateCategorys.specificationDescription === null || this.CreateCategorys.specificationDescription === '' || this.CreateCategorys.specificationDescription === undefined ||
      this.CreateCategorys.specificationDescriptionEnglish === null || this.CreateCategorys.specificationDescriptionEnglish === '' || this.CreateCategorys.specificationDescriptionEnglish === undefined || this.CreateItemsategorysList.length < 1) {
      this.submitted = true;
      this.savingOnCreate = false;
      return;
    }
    this.CreateCategorys.specificationChoices = this.CreateItemsategorysList;
    for (let j = 0; j < this.CreateCategorys.specificationChoices.length; j++) {
      if (this.CreateCategorys.specificationChoices[j].specificationChoiceDescription === '' || this.CreateCategorys.specificationChoices[j].price === null ||
        this.CreateCategorys.specificationChoices[j].specificationChoiceDescription === null || this.CreateCategorys.specificationChoices[j].price === undefined ||
        this.CreateCategorys.specificationChoices[j].specificationChoiceDescription === undefined ||
        this.CreateCategorys.specificationChoices[j].specificationChoiceDescriptionEnglish === '' ||
        this.CreateCategorys.specificationChoices[j].specificationChoiceDescriptionEnglish === null ||
        this.CreateCategorys.specificationChoices[j].specificationChoiceDescriptionEnglish === undefined
      ) {
        this.submitted = true;
        this.savingOnCreate = false;
        return;
      }
    }
    this.submitted = false;


    this.CreateCategorys.specificationChoices = this.CreateItemsategorysList;
    let data = this.CreateCategorys;
    let category: SpecificationsDto = new SpecificationsDto();
    let empList = Array<SpecificationChoicesDto>();
    let Model = new SpecificationsModelDto();


    if (this.CreateCategorys) {
      category.id = data.id;
      category.isMultipleSelection = data.isMultipleSelection;
      category.specificationDescription = data.specificationDescription;
      category.specificationDescriptionEnglish =
        data.specificationDescriptionEnglish;
      category.priority = data.priority;
      category.languageBotId = this.langId;
      category.maxSelectNumber = data.maxSelectNumber;

      if (category.maxSelectNumber === null || category.maxSelectNumber === undefined) {

        category.maxSelectNumber = 0;

      }

      if (category.priority === null || category.priority === undefined) {
        category.priority = 1;
      }
      for (let j = 0; j < data.specificationChoices.length; j++) {
        let obj = data.specificationChoices[j];

        let test = new SpecificationChoicesDto();

        test.id = obj.id;

        test.specificationChoiceDescription =
          obj.specificationChoiceDescription;
        test.specificationChoiceDescriptionEnglish =
          obj.specificationChoiceDescriptionEnglish;
        test.sku = obj.sku;
        test.price = obj.price;
        test.loyaltyPoints = obj.loyaltyPoints;

        test.languageBotId = this.langId;
        empList.push(test);
      }

      Model.specificationsDto = category;
      Model.specificationChoicesDtos = empList;

      this._itemAdditionServiceProxy
        .createOrEditSpecificationsModel(Model)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe((categoryResponse) => {
          this.notify.info(this.l("savedSuccessfully"));
          this.savingOnCreate = false;
          this.submitted = false
          this.modalService.dismissAll();
          this.modalSave.emit(null);
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
  editMenu() {
    this.savingOnEdit = true;
    if (this.category) {
      let data = this.category;
      if (this.category.categoryName === null || this.category.categoryName === '' || this.category.categoryName === undefined ||
        this.category.categoryNameEnglish === null || this.category.categoryNameEnglish === '' || this.category.categoryNameEnglish === undefined || this.category.listSpecificationChoices.length < 1) {
        this.submitted2 = true;
        this.savingOnEdit = false;
        return;
      }
      let categoryItemsList = data.listSpecificationChoices;
      for (let j = 0; j < categoryItemsList.length; j++) {
        if (categoryItemsList[j].specificationChoiceDescription === '' || categoryItemsList[j].price === null ||
          categoryItemsList[j].specificationChoiceDescription === null ||
          categoryItemsList[j].specificationChoiceDescription === undefined || categoryItemsList[j].price === undefined ||
          categoryItemsList[j].specificationChoiceDescriptionEnglish === '' ||
          categoryItemsList[j].specificationChoiceDescriptionEnglish === undefined ||
          categoryItemsList[j].specificationChoiceDescriptionEnglish === null
        ) {
          this.submitted2 = true;
          this.savingOnEdit = false;
          return;
        }
      }

      let category: SpecificationsDto = new SpecificationsDto();
      let empList = Array<SpecificationChoicesDto>();
      let Model = new SpecificationsModelDto();


      category.id = data.categoryId;
      category.isMultipleSelection = data.isMultipleSelection;
      category.maxSelectNumber = data.maxSelectNumber;
      category.specificationDescription = data.categoryName;
      category.priority = data.categoryPriority;
      category.languageBotId = this.langId;
      category.specificationDescriptionEnglish = data.categoryNameEnglish;
      if (category.priority === null || category.priority === undefined) {
        category.priority = 1;
      }

      if (category.maxSelectNumber === null || category.maxSelectNumber === undefined) {

        category.maxSelectNumber = 0;

      }
      for (let j = 0; j < data.listSpecificationChoices.length; j++) {
        let obj = data.listSpecificationChoices[j];

        let test = new SpecificationChoicesDto();

        test.id = obj.id;
        test.specificationId = category.id;
        test.specificationChoiceDescription =
          obj.specificationChoiceDescription;
        test.specificationChoiceDescriptionEnglish =
          obj.specificationChoiceDescriptionEnglish;
        test.sku = obj.sku;
        test.price = obj.price;
        test.loyaltyPoints = obj.loyaltyPoints;

        test.languageBotId = this.langId;

        empList.push(test);
      }

      Model.specificationsDto = category;
      Model.specificationChoicesDtos = empList;

      this._itemAdditionServiceProxy
        .createOrEditSpecificationsModel(Model)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe((categoryResponse) => {
          this.notify.info(this.l("savedSuccessfully"));
          this.submitted2 = false;
          this.savingOnCreate = false;
          this.modalService.dismissAll();
          this.modalSave.emit(null);
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

  CreateSpecifications(createSpecifications) {
    this.modalService.open(createSpecifications, {
      scrollable: true,
      centered: true,
      size: "lg",
    });
    this.CreateCategorys = new ItemSpecification();
    this.CreateCategorys.specificationChoices = [];
    this.CreateItemsategorysList = [];
    this.CreateItemsategorys = new SpecificationChoice();
    this.addNewItem = false;
  }
  viewItems(modalSLCIM, items) {
    this.modalService.open(modalSLCIM, {
      scrollable: true,
      centered: true,
      size: "lg",
    });
    this._menusServiceProxy
      .getSpecificationChoicesModel(items.categoryId)
      .subscribe((res) => {
        items.listSpecificationChoices = res;
        this.category = items;
      });
  }

  goToDashboard() {
    this._router.navigate(['/app/main/dashboard']);
  }
  goToMenu() {
    this._router.navigate(['/app/main/menus/menus']);

  }

}