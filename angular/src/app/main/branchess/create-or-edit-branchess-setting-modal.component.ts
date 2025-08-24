import {Component, ViewChild, Injector, Output, EventEmitter, OnInit} from '@angular/core';
import {ModalDirective} from 'ngx-bootstrap/modal';
import {} from 'rxjs/operators';
import {
    MenusServiceProxy,
     RestaurantsTypeEunm, RType, LanguageBot, AdditionsCategorysListModel, GetSpecificationsCategorysModel, MenuCategoryDto, WorkModel, AreasServiceProxy
} from '@shared/service-proxies/service-proxies';
import {AppComponentBase} from '@shared/common/app-component-base';
import * as moment from 'moment';
import {UntypedFormBuilder, UntypedFormGroup} from '@angular/forms';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '@app/services/dark-mode.service';



declare var ImageCompressor: any;


@Component({
    selector: 'createOrEditBranchSettingModel',
   // styleUrls: ['./model-menus.component.less'],
    templateUrl: './create-or-edit-branchess-setting-modal.component.html',
    styleUrls: ['./create-or-edit-branchess-modal.component.less']

})

export class CreateOrEditBranchessSettingModalComponent extends AppComponentBase implements OnInit {
   @ViewChild('createOrEditMenuSettingModel', {static: true}) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    theme: string;
    active = false;
    saving = false;

    menu: MenuCategoryDto = new MenuCategoryDto();

    menuItemStatusName = '';


    restaurantsTypeEunm = RestaurantsTypeEunm;
    enumKeys = [];
    branchId:number;
    subCategoryForm: UntypedFormGroup;

    langId = 1;
    itemIndex = -1;
    itemAddition = -1;
    categoryIndex = -1;
    fromFileUplode = false;
    arrayAddition: any;

    workTextAR!: string | undefined;
    workTextEN!: string | undefined;

    isWorkActiveSun!: boolean;
    workTextSun!: string | undefined;
    startDateSun!: any | undefined;
    endDateSun!: any | undefined;
    startDateSunSP!: any | undefined;
    endDateSunSP!: any | undefined;

    isWorkActiveMon!: boolean;
    workTextMon!: string | undefined;
    startDateMon!: any | undefined;
    endDateMon!: any | undefined;
    startDateMonSP!: any | undefined;
    endDateMonSP!: any | undefined;

    isWorkActiveTues!: boolean;
    workTextTues!: string | undefined;
    startDateTues!: any | undefined;
    endDateTues!: any | undefined;
    startDateTuesSP!: any | undefined;
    endDateTuesSP!: any | undefined;

    isWorkActiveWed!: boolean;
    workTextWed!: string | undefined;
    startDateWed!: any | undefined;
    endDateWed!: any | undefined;
    startDateWedSP!: any | undefined;
    endDateWedSP!: any | undefined;

    isWorkActiveThurs!: boolean;
    workTextThurs!: string | undefined;
    startDateThurs!: any | undefined;
    endDateThurs!: any | undefined;
    startDateThursSP!: any | undefined;
    endDateThursSP!: any | undefined;

    isWorkActiveFri!: boolean;
    workTextFri!: string | undefined;
    startDateFri!: any | undefined;
    endDateFri!: any | undefined;
    startDateFriSP!: any | undefined;
    endDateFriSP!: any | undefined;

    isWorkActiveSat!: boolean;
    workTextSat!: string | undefined;
    startDateSat!: any | undefined;
    endDateSat!: any | undefined;
    startDateSatSP!: any | undefined;
    endDateSatSP!: any | undefined;


    rType: RType[];

    additionsCategorysListModel: AdditionsCategorysListModel[];

    SpecificationListModel: GetSpecificationsCategorysModel[];

    languageBot: LanguageBot[];

    menuType: number;
    priorityMenu: number;

    isNf: boolean;
    categoryTypess: RestaurantsTypeEunm;
    selectedValue: any;

    subCategoryImage:string;
    imagBGSrc:string;

    addons: AdditionsCategorysListModel = new AdditionsCategorysListModel();

    dealStatusName = '';
    dealTypeName = '';
    cat:any;
     ind:any;
     add:any;
     sub:any;
     isMall: boolean;
     isWorkActive :boolean;
    constructor(
        injector: Injector,
        private _areasServiceProxy: AreasServiceProxy,
        private _menusServiceProxy: MenusServiceProxy,
        private fb: UntypedFormBuilder,
        public darkModeService: DarkModeService,

    ) {
        super(injector);
        this.enumKeys = Object.keys(this.restaurantsTypeEunm);
    }



   show(branchId: number): void {



    this._areasServiceProxy.getMenuSetting(branchId).subscribe((res) => {

       let workModel=res;
       this.workTextAR= workModel.workTextAR;
       this.workTextEN= workModel.workTextEN;
       this.isWorkActiveSun= workModel.isWorkActiveSun;
       this.workTextSun= workModel.workTextSun;
       this.startDateSun= workModel.startDateSun;
       this.endDateSun= workModel.endDateSun;

       this.isWorkActiveMon= workModel.isWorkActiveMon;
       this.workTextMon= workModel.workTextMon;
       this.startDateMon= workModel.startDateMon;
       this.endDateMon= workModel.endDateMon;

       this.isWorkActiveTues= workModel.isWorkActiveTues;
       this.workTextTues= workModel.workTextTues;
       this.startDateTues= workModel.startDateTues;
       this.endDateTues= workModel.endDateTues;

       this.isWorkActiveWed= workModel.isWorkActiveWed;
       this.workTextWed= workModel.workTextWed;
       this.startDateWed= workModel.startDateWed;
       this.endDateWed= workModel.endDateWed;

       this.isWorkActiveThurs= workModel.isWorkActiveThurs;
       this.workTextThurs= workModel.workTextThurs;
       this.startDateThurs= workModel.startDateThurs;
       this.endDateThurs= workModel.endDateThurs;

       this.isWorkActiveFri= workModel.isWorkActiveFri;
       this.workTextFri= workModel.workTextFri;
       this.startDateFri= workModel.startDateFri;
       this.endDateFri= workModel.endDateFri;

       this.isWorkActiveSat= workModel.isWorkActiveSat;
       this.workTextSat= workModel.workTextSat;
       this.startDateSat= workModel.startDateSat;
       this.endDateSat= workModel.endDateSat;






       this.startDateSunSP= workModel.startDateSunSP;
       this.endDateSunSP= workModel.endDateSunSP;

       this.startDateMonSP= workModel.startDateMonSP;
       this.endDateMonSP= workModel.endDateMonSP;


       this.startDateTuesSP= workModel.startDateTuesSP;
       this.endDateTuesSP= workModel.endDateTuesSP;


       this.startDateWedSP= workModel.startDateWedSP;
       this.endDateWedSP= workModel.endDateWedSP;


       this.startDateThursSP= workModel.startDateThursSP;
       this.endDateThursSP= workModel.endDateThursSP;


       this.startDateFriSP= workModel.startDateFriSP;
       this.endDateFriSP= workModel.endDateFriSP;


       this.startDateSatSP= workModel.startDateSatSP;
       this.endDateSatSP= workModel.endDateSatSP;



      });

    this.isWorkActive= true;
    this.branchId = branchId;
    this.active = true;
    this.modal.show();
}







    close(): void {
        this.active = false;
       this.modal.hide();
    }

    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isNf =true;


        this._menusServiceProxy.getRType(this.appSession.tenantId).subscribe(result => {
            if (this.isNf) {

                this.rType = result;
                // this.rType.shift();

            } else {

                this.rType = result;
            }

        });

        this._menusServiceProxy.getLanguageBot().subscribe(result => {

                this.languageBot = result;
               // this.languageBot.shift();



        });

   
        


        this.isMall=false;
        this.subCategoryForm = this.fb.group({
            categoryName:[''],
            name: [''],
            nameEnglish: [''],
            priority: 0,
            languageBotId: 1,
            categoryId:0,
            subCategoryId:0,
        });

        await this.getIsAdmin();
    }







    saveSetting() {
        let objWorkModel = new  WorkModel();
        objWorkModel.workTextAR=this.workTextAR;
        objWorkModel.workTextEN=this.workTextEN;

        objWorkModel.isWorkActiveSun=this.isWorkActiveSun;
        objWorkModel.workTextSun=this.workTextSun;
        objWorkModel.startDateSun=moment(this.startDateSun, 'HH:mm A');
        objWorkModel.endDateSun= moment(this.endDateSun, 'HH:mm A');
        objWorkModel.startDateSunSP=moment(this.startDateSunSP, 'HH:mm A');
        objWorkModel.endDateSunSP= moment(this.endDateSunSP, 'HH:mm A');

        objWorkModel.isWorkActiveMon=this.isWorkActiveMon;
        objWorkModel.workTextMon=this.workTextMon;
        objWorkModel.startDateMon=moment(this.startDateMon, 'HH:mm A');
        objWorkModel.endDateMon=moment(this.endDateMon, 'HH:mm A');
        objWorkModel.startDateMonSP=moment(this.startDateMonSP, 'HH:mm A');
        objWorkModel.endDateMonSP=moment(this.endDateMonSP, 'HH:mm A');

        objWorkModel.isWorkActiveTues=this.isWorkActiveTues;
        objWorkModel.workTextTues=this.workTextTues;
        objWorkModel.startDateTues=moment(this.startDateTues, 'HH:mm A');
        objWorkModel.endDateTues=moment(this.endDateTues, 'HH:mm A');
        objWorkModel.startDateTuesSP=moment(this.startDateTuesSP, 'HH:mm A');
        objWorkModel.endDateTuesSP=moment(this.endDateTuesSP, 'HH:mm A');

        objWorkModel.isWorkActiveWed=this.isWorkActiveWed;
        objWorkModel.workTextWed=this.workTextWed;
        objWorkModel.startDateWed=moment(this.startDateWed, 'HH:mm A');
        objWorkModel.endDateWed= moment(this.endDateWed, 'HH:mm A');
        objWorkModel.startDateWedSP=moment(this.startDateWedSP, 'HH:mm A');
        objWorkModel.endDateWedSP= moment(this.endDateWedSP, 'HH:mm A');

        objWorkModel.isWorkActiveThurs=this.isWorkActiveThurs;
        objWorkModel.workTextThurs=this.workTextThurs;
        objWorkModel.startDateThurs=moment(this.startDateThurs, 'HH:mm A');
        objWorkModel.endDateThurs=moment(this.endDateThurs, 'HH:mm A');
        objWorkModel.startDateThursSP=moment(this.startDateThursSP, 'HH:mm A');
        objWorkModel.endDateThursSP=moment(this.endDateThursSP, 'HH:mm A');

        objWorkModel.isWorkActiveFri=this.isWorkActiveFri;
        objWorkModel.workTextFri=this.workTextFri;
        objWorkModel.startDateFri=moment(this.startDateFri, 'HH:mm A');
        objWorkModel.endDateFri=moment(this.endDateFri, 'HH:mm A');
        objWorkModel.startDateFriSP=moment(this.startDateFriSP, 'HH:mm A');
        objWorkModel.endDateFriSP=moment(this.endDateFriSP, 'HH:mm A');

        objWorkModel.isWorkActiveSat=this.isWorkActiveSat;
        objWorkModel.workTextSat=this.workTextSat;
        objWorkModel.startDateSat=moment(this.startDateSat, 'HH:mm A');
        objWorkModel.endDateSat=moment(this.endDateSat, 'HH:mm A');
        objWorkModel.startDateSatSP=moment(this.startDateSatSP, 'HH:mm A');
        objWorkModel.endDateSatSP=moment(this.endDateSatSP, 'HH:mm A');




        if(this.isWorkActiveSat){
            if(this.startDateSat >= this.endDateSat || this.endDateSat <= this.startDateSat){
           this.message.error( this.l('workingHoursError'), this.l('invalidDateInSaturday'));
           return
       }
       }
      
       if(this.isWorkActiveSat){
           if((this.startDateSatSP >= this.startDateSat && this.startDateSatSP) <= this.endDateSat ||
           
          (this.endDateSatSP <= this.endDateSat && this.endDateSatSP) >= this.startDateSat ||
          this.startDateSatSP >= this.endDateSatSP || this.endDateSatSP <= this.startDateSatSP || 
          (this.startDateSat <= this.endDateSatSP && this.endDateSat >= this.startDateSatSP )
           ){
               this.message.error( this.l('workingHoursError'), this.l('invalidDateInSaturday'));
               return
           }
       }
       



           if(this.isWorkActiveSun){
if(this.startDateSun >= this.endDateSun || this.endDateSun <= this.startDateSun){
           this.message.error( this.l('workingHoursError'), this.l('invalidDateInSunday'));
           return
       }
           }

           if(this.isWorkActiveSun){ 
               if((this.startDateSunSP >= this.startDateSun && this.startDateSunSP) <= this.endDateSun ||
           
          (this.endDateSunSP <= this.endDateSun && this.endDateSunSP) >= this.startDateSun ||
          this.startDateSunSP >= this.endDateSunSP || this.endDateSunSP <= this.startDateSunSP || 
          (this.startDateSun <= this.endDateSunSP && this.endDateSun >= this.startDateSunSP )
           ){
               this.message.error( this.l('workingHoursError'), this.l('invalidDateInSunday'));
               return
           }
           }
      



           if(this.isWorkActiveMon){
                if(this.startDateMon >= this.endDateMon || this.endDateMon <= this.startDateMon){
           this.message.error( this.l('workingHoursError'), this.l('invalidDateInMonday'));
           return
       }
           }

    
           if(this.isWorkActiveMon){
                if((this.startDateMonSP >= this.startDateMon && this.startDateMonSP) <= this.endDateMon ||
           
          (this.endDateMonSP <= this.endDateMon && this.endDateMonSP) >= this.startDateMon ||
          this.startDateMonSP >= this.endDateMonSP || this.endDateMonSP <= this.startDateMonSP || 
          (this.startDateMon <= this.endDateMonSP && this.endDateMon >= this.startDateMonSP )
           ){
               this.message.error( this.l('workingHoursError'), this.l('invalidDateInMonday'));
               return
           }

           }

      

           if(this.isWorkActiveTues){
               if(this.startDateTues >= this.endDateTues || this.endDateTues <= this.startDateTues){
           this.message.error( this.l('workingHoursError'), this.l('invalidDateInTuesday'));
           return
       } 
           }
     
           if(this.isWorkActiveTues){
              if((this.startDateTuesSP >= this.startDateTues && this.startDateTuesSP) <= this.endDateTues ||
           
          (this.endDateTuesSP <= this.endDateTues && this.endDateTuesSP) >= this.startDateTues ||
          this.startDateTuesSP >= this.endDateTuesSP || this.endDateTuesSP <= this.startDateTuesSP || 
          (this.startDateTues <= this.endDateTuesSP && this.endDateTues >= this.startDateTuesSP )
           ){
               this.message.error( this.l('workingHoursError'), this.l('invalidDateInTuesday'));
               return
           }
           }
    

           if(this.isWorkActiveWed){
                   if(this.startDateWed >= this.endDateWed || this.endDateWed <= this.startDateWed){
               this.message.error( this.l('workingHoursError'), this.l('invalidDateInWednesday'));
               return
           }
           }


           if(this.isWorkActiveWed){
            if((this.startDateWedSP >= this.startDateWed && this.startDateWedSP) <= this.endDateWed ||
               
              (this.endDateWedSP <= this.endDateWed && this.endDateWedSP) >= this.startDateWed ||
              this.startDateWedSP >= this.endDateWedSP || this.endDateWedSP <= this.startDateWedSP || 
              (this.startDateWed <= this.endDateWedSP && this.endDateWed >= this.startDateWedSP )
               ){
                   this.message.error( this.l('workingHoursError'), this.l('invalidDateInWednesday'));
                   return
               }
           }
          


           if(this.isWorkActiveThurs){
                if(this.startDateThurs >= this.endDateThurs || this.endDateThurs <= this.startDateThurs){
                   this.message.error( this.l('workingHoursError'), this.l('invalidDateInThursady'));
                   return
               }
           }

           if(this.isWorkActiveThurs){
                if((this.startDateThursSP >= this.startDateThurs && this.startDateThursSP) <= this.endDateThurs ||
                   
                  (this.endDateThursSP <= this.endDateThurs && this.endDateThursSP) >= this.startDateThurs ||
                  this.startDateThursSP >= this.endDateThursSP || this.endDateThursSP <= this.startDateThursSP || 
                  (this.startDateThurs <= this.endDateThursSP && this.endDateThurs >= this.startDateThursSP )
                   ){
                       this.message.error( this.l('workingHoursError'), this.l('invalidDateInThursady'));
                       return
                   }
           }

              

           if(this.isWorkActiveFri){
                 if(this.startDateFri >= this.endDateFri || this.endDateFri <= this.startDateFri){
                       this.message.error( this.l('workingHoursError'), this.l('invalidDateInFriday'));
                       return
                   }
           }

           if(this.isWorkActiveFri){
             if((this.startDateFriSP >= this.startDateFri && this.startDateFriSP) <= this.endDateFri ||
                       
                      (this.endDateFriSP <= this.endDateFri && this.endDateFriSP) >= this.startDateFri ||
                      this.startDateFriSP >= this.endDateFriSP || this.endDateFriSP <= this.startDateFriSP || 
                      (this.startDateFri <= this.endDateFriSP && this.endDateFri >= this.startDateFriSP )
                       ){
                           this.message.error( this.l('workingHoursError'), this.l('invalidDateInFriday'));
                           return
                       }

           }
                 



        this._areasServiceProxy.saveSetting(this.branchId,objWorkModel).subscribe(response => {
            this.notify.info(this.l('savedSuccessfully'));

            this.modal.hide();

            this.modalSave.emit(null);
        });
    }










}
