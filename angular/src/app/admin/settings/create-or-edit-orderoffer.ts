import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AreaDto, AreasServiceProxy, CreateOrEditOrderOfferDto, DeliveryLocationInfoModel, LocationModel, LocationServiceProxy, OrderOfferDto, OrderOfferServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { DarkModeService } from '@app/services/dark-mode.service';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';


@Component({
  selector: 'createOrEditorderofferModal',
  templateUrl: './create-or-edit-orderoffer.html',
  styleUrls: ['./create-or-edit-orderoffer.less'],
})
export class CreateOrEditorderofferModelComponent extends AppComponentBase {
  theme: string;
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  submitted = false;
  active = false;
  saving = false;

  cities: LocationModel[];

  fromareas: LocationModel[];
  _orderOfferDto: OrderOfferDto;

  locationId: number;
  fromlocation: DeliveryLocationInfoModel = new DeliveryLocationInfoModel();
  selectedItems = [];
  dropdownSettings = {};
  createOrEditOrderOfferDto = new CreateOrEditOrderOfferDto;
  selectedItems2 = [];
  orderOfferStartS: moment.Moment;
  orderOfferEndS: moment.Moment;



  area: AreaDto = new AreaDto();
  areas: AreaDto[] = [new AreaDto];
  branchId = -1;

  dropdownSettings12 = {};
  dropdownSettings22 = {};
  selectedBranchIds: Array<any> = [];;
  selectedBranches: Array<any> = [];
  dropdownList = [];
  selectedItems1 = [];






  feesStart: number;
  feesEnd: number;
  newFees: number;
  min = new Date();
  dateRange: [Date, Date];

  dateRangePickerOptions = {
    dateInputFormat: "DD/MM/YYYY",
    rangeInputFormat: "DD/MM/YYYY",
    isAnimated: true,
    adaptivePosition: true,
    containerClass: "theme-default",
    selectFromOtherMonth: true,
    showPreviousMonth: true,
    showWeekNumbers: false,
    useUtc: true,
  };

  constructor(

    injector: Injector,
    private _locationServiceProxy: LocationServiceProxy,

    private orderOfferServiceProxy: OrderOfferServiceProxy,
    private _areasServiceProxy: AreasServiceProxy,
    public darkModeService: DarkModeService,
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.theme = ThemeHelper.getTheme();

    this.dropdownSettings12 = {
      singleSelection: false,
      idField: 'id',
      textField: 'areaName',
      itemsShowLimit: 3,
      allowSearchFilter: false,
      maxHeight: 200,
      closeDropDownOnSelection: true
      
    };

  }

  onItemSelect(item: any) {
    this.selectedItems2.push(item);
  }
  onItemDeSelect(item: any) {
    const index = this.selectedItems2.indexOf(item);
    if (index > -1) {
      this.selectedItems2.splice(index, 1);
    }
  }
  onSelectAll(items: any) {
    this.selectedItems2 = items;
  }

  showCreate(): void {
    this.min = new Date();
    this.min = new Date(this.min.setHours(this.min.getHours() + 1, 0, 0));
    this._orderOfferDto = new OrderOfferDto;
    this.fromlocation = new DeliveryLocationInfoModel();
    this.fromareas = [];
    this.selectedItems = []
    this.active = true;
    this.getCities();
    this.getBranches();
    this.initilizeDateRange();
    this.feesStart = null;
    this.dateRange = null;
    this.newFees = null;
    this.selectedBranchIds = [];
    this.feesEnd = null;
    this.selectedItems2 = [];
    this.createOrEditOrderOfferDto = new CreateOrEditOrderOfferDto
    this.modal.show();
  }


  private initilizeDateRange = () => {
    const now = new Date();
    const firstDayOfCurrentMonth = new Date(
      now.getFullYear(),
      now.getMonth(),
      1
    );
    this.dateRange = [firstDayOfCurrentMonth, now];
  };

  save(): void {
    this.saving = true;
    this.createOrEditOrderOfferDto.isBranchDiscount=true;
    if (
      this.feesStart === null || this.feesStart === undefined ||
      this.feesEnd === undefined || this.feesEnd === null ||   
      ( this.selectedBranchIds.length <= 0) ||
      this.dateRange === null || this.dateRange === undefined ||
      this.createOrEditOrderOfferDto.orderOfferStartS === null || this.createOrEditOrderOfferDto.orderOfferStartS === undefined ||
      this.createOrEditOrderOfferDto.orderOfferEndS === null || this.createOrEditOrderOfferDto.orderOfferEndS === undefined ||
      this.newFees === null || this.newFees === undefined 
    ) {
      this.submitted = true;
      this.saving = false;
      return;
    }
    this.fromlocation.tenantId = this.appSession.tenantId;
    // this.cities.forEach(element => {
    //   if (element.locationId == this.locationId) {
    //     this.createOrEditOrderOfferDto.cities = element.locationName;
    //   }
    // });
    this.createOrEditOrderOfferDto.area = '';
    // this.selectedItems2.forEach(element => {
    //   this.createOrEditOrderOfferDto.area = this.createOrEditOrderOfferDto.area + "," + element
    // });
    // if (this.createOrEditOrderOfferDto.cities == null || this.createOrEditOrderOfferDto.cities == "") {
    //   this.createOrEditOrderOfferDto.cities = this.cities[0].locationName;
    // }
    this.createOrEditOrderOfferDto.feesStart = this.feesStart;
    this.createOrEditOrderOfferDto.feesEnd = this.feesEnd;
    this.createOrEditOrderOfferDto.newFees = this.newFees;
    this.createOrEditOrderOfferDto.isAvailable = true;

    //date
    this.createOrEditOrderOfferDto.orderOfferDateStart = moment(this.dateRange[0], 'DD/MM/YYYY');
    this.createOrEditOrderOfferDto.orderOfferDateEnd = moment(this.dateRange[1], 'DD/MM/YYYY');


    //time
    // this.createOrEditOrderOfferDto.orderOfferStart = moment(this.createOrEditOrderOfferDto.orderOfferStartS, 'DD/MM/YYYY HH:mm A');
    // this.createOrEditOrderOfferDto.orderOfferEnd = moment(this.createOrEditOrderOfferDto.orderOfferEndS, 'DD/MM/YYYY HH:mm A');
    
    ///////
    let startTime=  moment(this.createOrEditOrderOfferDto.orderOfferStartS, 'HH:mm A');
    let endTime = moment(this.createOrEditOrderOfferDto.orderOfferEndS, 'HH:mm A');

    this.createOrEditOrderOfferDto.orderOfferStart =  moment(this.createOrEditOrderOfferDto.orderOfferDateStart).set('hour',startTime.hours()).set('minute',startTime.minutes());
    this.createOrEditOrderOfferDto.orderOfferEnd =  moment(this.createOrEditOrderOfferDto.orderOfferDateEnd ).set('hour',endTime.hours()).set('minute',endTime.minutes());
    ////////


    
    this.createOrEditOrderOfferDto.isBranchDiscount=true;

    this.createOrEditOrderOfferDto.branchesIds = this.selectedBranchIds.filter(f => f.id > 0).map(({ id }) => id).toString();
    if (this.createOrEditOrderOfferDto.branchesIds == "") {
      this.createOrEditOrderOfferDto.branchesIds = null;
    }

    this.createOrEditOrderOfferDto.branchesName = this.selectedBranchIds.filter(f => f.id > 0).map(({ areaName }) => areaName).toString();
    this.orderOfferServiceProxy.createOrEdit(this.createOrEditOrderOfferDto)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe(() => {
        this.submitted = false;
        this.saving = false;
        this.notify.info(this.l('savedSuccessfully'));
        this.close();
        this.modalSave.emit(true);
      });
  }


  getCities() {
    this._locationServiceProxy
      .getRootLocations()
      .subscribe((result: any) => {
        this.cities = result;
      });
  }


  getBranches() {
    this._areasServiceProxy.getAvailableAreas(this.appSession.tenantId).subscribe(branchList => {
      this.areas = branchList;
    });
  }

  onChangeFromCity(event): void {
    this.selectedItems2 = [];

    this.locationId = event.target.value;
    this._locationServiceProxy.getLocationsByParentId(this.locationId).subscribe((result: any) => {
      this.fromareas = result;
      this.selectedItems = [];
      result.forEach(element => {
        this.selectedItems.push(element.locationName)
      });
    });
  }


  close(): void {
    this.active = false;
    this.submitted = false;
    this.saving = false;
    this.modal.hide();
  }


  onDateRangeUpdate = () => {

  };
}
