import { Component, Injector, ViewEncapsulation, ViewChild, IterableDiffers, ElementRef } from '@angular/core';
import {  TenantEditDto, CTownApiServiceProxy, CategorysInItemModel, SubCategorysInItemModel, ItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditctownModalComponent } from './create-or-edit-ctown-modal.component';
import { ViewctownModalComponent } from './view-ctown-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import * as _ from 'lodash';
import * as moment from 'moment';
import { TeamInboxService } from '../teamInbox/teaminbox.service';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';


@Component({
  templateUrl: './ctown.component.html',
  styleUrls: ['./ctown.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class ctownComponent extends AppComponentBase {
  theme: string;
  @ViewChild('createOrEditctownModal', { static: true }) createOrEditctownModal: CreateOrEditctownModalComponent;
  @ViewChild('viewctownModalComponent', { static: true }) viewctownModal: ViewctownModalComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('scrollItems', { static: false }) private scrollItems: ElementRef;

  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';
  searchItem :string = '';
  dateFilter: moment.Moment;
  activeButton = true;
  bookingList: any[];
  InsideNumber: number;
  OutsideNumber: number;
  catgory: CategorysInItemModel[];
  subcatgory: SubCategorysInItemModel[]
  isReplec = false;
  items: ItemDto[];
  selectcatgory: CategorysInItemModel;
  selectsubcatgory: SubCategorysInItemModel
  showMessageLoader = false;
  formDataFile: any;
  fileToUpload: any;
  fileToUpload2: any;
  uploader: any;
  url: string | ArrayBuffer;
  differ: any;
  tenantEditDto: TenantEditDto;
  chatForm: any;
  currentPosition: number;
  prvHeight: number;
  isOneTime: boolean;
  isRepleceImage = false;
  topnumber: any;
  heiget: any;
  pageSizeC =20;
  pageNumberC = 0;
  pageSize = 10;
  pageNumber = 0;
  constructor(
    differs: IterableDiffers,
    injector: Injector,
    private teamService: TeamInboxService,
    private _CTownApiServiceProxy: CTownApiServiceProxy,
    public darkModeService: DarkModeService,

  ) {
    super(injector);
    this.differ = differs.find([]).create(null);
  }

  ngOnInit(): void {
    this.theme = ThemeHelper.getTheme();
    this.getCtownCatogeory();

  }

  UpdateItems() {

    this.getItemtSubCategory();
  }

  getCtownCatogeory(): void {

    this._CTownApiServiceProxy.getCtownCatogeory(this.appSession.tenantId,).subscribe((res) => {
      this.catgory = res;
      this.subcatgory = res[0].subCategorysInItemModels;
      this.selectcatgory = res[0];
      this.selectsubcatgory = this.selectcatgory.subCategorysInItemModels[0];
      this.getItemtSubCategory();
    });

  }

  selectCategory(cat: CategorysInItemModel): void {
    this.searchItem='';
    this.subcatgory = cat.subCategorysInItemModels;
    this.selectcatgory = cat;
    this.selectsubcatgory = this.selectcatgory.subCategorysInItemModels[0];
    this.getItemtSubCategory();

  }

  selectSubCategory(subcat: SubCategorysInItemModel): void {
    this.selectsubcatgory = subcat;
    this.searchItem ='';
    this.getItemtSubCategory();
  }

  getItemtSubCategory(): void {
    this.isOneTime = true;
    this.showMessageLoader = true;
    this.pageNumberC = 0;
    this.pageNumber = 0;
    this._CTownApiServiceProxy.getCtownItem(this.appSession.tenantId,this.selectsubcatgory.subcategoryId, this.pageSizeC, this.pageNumberC,this.searchItem).subscribe((res) => {
      this.showMessageLoader = false;
      this.items = res.itemDtos;
      this.scrollItems.nativeElement.scrollIntoView({behavior: 'smooth'}); 
  
    });
  }

  syncImage(): void {
    this.isOneTime = true;
    this.showMessageLoader = true;
    this.pageNumberC = 0;
    this._CTownApiServiceProxy.syncImage(this.selectsubcatgory.subcategoryId, this.isRepleceImage).subscribe((res) => {

      this.showMessageLoader = false;
      this.reloadPage();

    });

  }

  handleFileInput(event) {
    this.fileToUpload = event.target.files[0];
  }

  uploadFileToActivity() {
    let formDataFile = new FormData();
    formDataFile.append('formFile', this.fileToUpload);
    this.showMessageLoader = true;
    this.teamService.ctownUploadExcelFile(formDataFile, this.selectsubcatgory.categoryNameEnglish, this.isReplec)
      .subscribe(result => {
        this.showMessageLoader = false;
        this.notify.success(this.l('SuccessfullyUpload'));
        this.reloadPage();
      });
  }
  reloadPage() {
    location.reload();
    throw new Error('Method not implemented.');
  }
  initFileUploader() {
    throw new Error('Method not implemented.');
  }


  uploadFileToActivity2() {
    for (let i = 0; i < this.fileToUpload2.length; i++) {

      let formDataFile = new FormData();
      formDataFile.append('formFileList', this.fileToUpload2[i]);
      this.showMessageLoader = true;
      this.teamService.uploadImageCtown(formDataFile)
        .subscribe(result => {
          this.showMessageLoader = false;
          this.notify.success(this.l('SuccessfullyUpload'));
          this.reloadPage();
        });
    }
  }

  handleFileInput2(event) {
    this.fileToUpload2 = event.target.files;
  }

  // loadMoreitem() {
  //   this.showMessageLoader = true;
  //   this._CTownApiServiceProxy.getCtownItem(this.appSession.tenantId,this.selectsubcatgory.subcategoryId, this.pageSizeC, this.pageNumberC, "").subscribe((res) => {

  //     this.showMessageLoader = false;
  //     this.items = res;
  //     this.scrollItems.nativeElement.scrollTop = this.scrollItems.nativeElement.scrollHeight;//this.prvHeight;
  //     this.isOneTime = true;

  //   });
  // }

  onScroll2() {
    let scroll = window.pageYOffset;
    if (scroll >= this.currentPosition || this.currentPosition  === undefined ) {
      var d = document.getElementById("ItemsDiv");
      var height = d.offsetHeight;
      if (this.prvHeight > height) {
      } else {
          var h = this.scrollItems.nativeElement.scrollHeight - 30;//-20;//-height;
          var t = this.scrollItems.nativeElement.scrollTop + height;
          if (t >= h && this.isOneTime) {
              this.prvHeight = height;
              this.topnumber = this.scrollItems.nativeElement.scrollTop;
              this.heiget = this.scrollItems.nativeElement.scrollHeight;
              this.pageNumberC = this.pageNumberC + 1;
              this.loadMoreItems();
          }
        }
      
    } else {

    }
    this.currentPosition = scroll;
}

loadMoreItems() {
  //this.showMessageLoader = true;
  this._CTownApiServiceProxy.getCtownItem(this.appSession.tenantId,this.selectsubcatgory.subcategoryId, this.pageSizeC, this.pageNumberC, "").subscribe((res) => {
    this.showMessageLoader = false;
    res.itemDtos.forEach(element => {
      this.items.push(element);
    });
    // this.scrollItems.nativeElement.scrollTop = this.scrollItems.nativeElement.scrollHeight;//this.prvHeight;

  });
}

}
