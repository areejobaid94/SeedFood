import {Component, ViewChild, Injector, Output, EventEmitter, OnInit} from '@angular/core';
import {ModalDirective} from 'ngx-bootstrap/modal';
import {} from 'rxjs/operators';
import {
     RestaurantsTypeEunm, MenuCategoryDto, WorkModel, UserServiceProxy
} from '@shared/service-proxies/service-proxies';
import {AppComponentBase} from '@shared/common/app-component-base';
import * as moment from 'moment';
import { UntypedFormGroup} from '@angular/forms';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '@app/services/dark-mode.service';
import { FlatpickrOptions } from 'ng2-flatpickr';




@Component({
    selector: 'createOrEditUserSettingModel',
   // styleUrls: ['./model-menus.component.less'],
    templateUrl: './create-or-edit-user-setting-modal.component.html',
    styleUrls: ['./create-or-edit-user-modal.component.less']

})

export class CreateOrEditUserSettingModalComponent extends AppComponentBase implements OnInit {
   @ViewChild('createOrEditUserSettingModel', {static: true}) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    theme: string;
    active = false;
    saving = false;

    menu: MenuCategoryDto = new MenuCategoryDto();

    menuItemStatusName = '';
    listUanvailableDates = [];

    restaurantsTypeEunm = RestaurantsTypeEunm;
    enumKeys = [];
    userID:number;
    subCategoryForm: UntypedFormGroup;
    public multipleDateOptions: FlatpickrOptions = {
        altInput: true,
        mode: 'multiple',
        dateFormat: "d.m.Y",
      };

    langId = 1;
    itemIndex = -1;
    itemAddition = -1;
    categoryIndex = -1;
    fromFileUplode = false;
    arrayAddition: any;
    unAvailableBookingDates :any = [];
    workTextAR!: string | undefined;
    workTextEN!: string | undefined;

    isWorkActiveSun!: boolean;
    workTextSun!: string | undefined;
    startDateSun!: any | undefined;
    endDateSun!: any | undefined;
    startDateSunSP!: any | undefined;
    endDateSunSP!: any | undefined;
    hasSPSun!: boolean;


    isWorkActiveMon!: boolean;
    workTextMon!: string | undefined;
    startDateMon!: any | undefined;
    endDateMon!: any | undefined;
    startDateMonSP!: any | undefined;
    endDateMonSP!: any | undefined;
    hasSPMon!: boolean;


    isWorkActiveTues!: boolean;
    workTextTues!: string | undefined;
    startDateTues!: any | undefined;
    endDateTues!: any | undefined;
    startDateTuesSP!: any | undefined;
    endDateTuesSP!: any | undefined;
    hasSPTues!: boolean;


    isWorkActiveWed!: boolean;
    workTextWed!: string | undefined;
    startDateWed!: any | undefined;
    endDateWed!: any | undefined;
    startDateWedSP!: any | undefined;
    endDateWedSP!: any | undefined;
    hasSPWed!: boolean;


    isWorkActiveThurs!: boolean;
    workTextThurs!: string | undefined;
    startDateThurs!: any | undefined;
    endDateThurs!: any | undefined;
    startDateThursSP!: any | undefined;
    endDateThursSP!: any | undefined;
    hasSPThurs!: boolean;


    isWorkActiveFri!: boolean;
    workTextFri!: string | undefined;
    startDateFri!: any | undefined;
    endDateFri!: any | undefined;
    startDateFriSP!: any | undefined;
    endDateFriSP!: any | undefined;
    hasSPFri!: boolean;


    isWorkActiveSat!: boolean;
    workTextSat!: string | undefined;
    startDateSat!: any | undefined;
    endDateSat!: any | undefined;
    startDateSatSP!: any | undefined;
    endDateSatSP!: any | undefined;
    hasSPSat!: boolean;

    showUnavailableDates = false;
     isWorkActive :boolean;
    constructor(
        injector: Injector,
        private _userServiceProxy: UserServiceProxy,
        public darkModeService: DarkModeService,

    ) {
        super(injector);
        this.enumKeys = Object.keys(this.restaurantsTypeEunm);
    }



   show(userID: number): void {



    this._userServiceProxy.getUserSetting(userID).subscribe((res) => {
        let workModel=new WorkModel();
         workModel=res;



         this.workTextAR= workModel.workTextAR;
         this.isWorkActiveSun =  workModel.isWorkActiveSun;
         this.workTextSun =  workModel.workTextSun;
         this.startDateSun =  workModel.startDateSun;
         this.endDateSun =  workModel.endDateSun;
         this.hasSPSun =  workModel.hasSPSun;
         this.startDateSunSP =  workModel.startDateSunSP;
         this.endDateSunSP =  workModel.endDateSunSP;


         this.isWorkActiveMon = workModel.isWorkActiveMon;
         this.workTextMon = workModel.workTextMon;
         this.startDateMon = workModel.startDateMon;
         this.endDateMon = workModel.endDateMon;
         this.hasSPMon = workModel.hasSPMon;
         this.startDateMonSP = workModel.startDateMonSP;
         this.endDateMonSP = workModel.endDateMonSP;

         this.isWorkActiveTues = workModel.isWorkActiveTues;
         this.workTextTues = workModel.workTextTues;
         this.startDateTues = workModel.startDateTues;
         this.endDateTues = workModel.endDateTues;
         this.hasSPTues = workModel.hasSPTues;
         this.startDateTuesSP = workModel.startDateTuesSP;
         this.endDateTuesSP = workModel.endDateTuesSP;


         this.isWorkActiveWed = workModel.isWorkActiveWed;
         this.workTextWed = workModel.workTextWed;
         this.startDateWed = workModel.startDateWed;
         this.endDateWed = workModel.endDateWed;
         this.hasSPWed = workModel.hasSPWed;
         this.startDateWedSP = workModel.startDateWedSP;
         this.endDateWedSP = workModel.endDateWedSP;


         this.isWorkActiveThurs = workModel.isWorkActiveThurs;
         this.workTextThurs = workModel.workTextThurs;
         this.startDateThurs = workModel.startDateThurs;
         this.endDateThurs = workModel.endDateThurs;
         this.hasSPThurs = workModel.hasSPThurs;
         this.startDateThursSP = workModel.startDateThursSP;
         this.endDateThursSP = workModel.endDateThursSP;



         this.isWorkActiveFri = workModel.isWorkActiveFri;
         this.workTextFri = workModel.workTextFri;
         this.startDateFri = workModel.startDateFri;
         this.endDateFri = workModel.endDateFri;
         this.hasSPFri = workModel.hasSPFri;
         this.startDateFriSP = workModel.startDateFriSP;
         this.endDateFriSP = workModel.endDateFriSP;


         this.isWorkActiveSat = workModel.isWorkActiveSat;
         this.workTextSat = workModel.workTextSat;
         this.startDateSat = workModel.startDateSat;
         this.endDateSat = workModel.endDateSat;
         this.hasSPSat = workModel.hasSPSat;
         this.startDateSatSP = workModel.startDateSatSP;
         this.endDateSatSP = workModel.endDateSatSP;
















         /***************************** */
    //    this.workTextAR= workModel.workTextAR;
    //    //this.workTextEN= workModel.workTextEN;
    //    this.isWorkActiveSun= workModel.isWorkActiveSun;
    //    this.workTextSun= workModel.workTextSun;
    //    this.startDateSun= workModel.startDateSun;
    //    this.endDateSun= workModel.endDateSun;

    //    this.isWorkActiveMon= workModel.isWorkActiveMon;
    //    this.workTextMon= workModel.workTextMon;
    //    this.startDateMon= workModel.startDateMon;
    //    this.endDateMon= workModel.endDateMon;

    //    this.isWorkActiveTues= workModel.isWorkActiveTues;
    //    this.workTextTues= workModel.workTextTues;
    //    this.startDateTues= workModel.startDateTues;
    //    this.endDateTues= workModel.endDateTues;

    //    this.isWorkActiveWed= workModel.isWorkActiveWed;
    //    this.workTextWed= workModel.workTextWed;
    //    this.startDateWed= workModel.startDateWed;
    //    this.endDateWed= workModel.endDateWed;

    //    this.isWorkActiveThurs= workModel.isWorkActiveThurs;
    //    this.workTextThurs= workModel.workTextThurs;
    //    this.startDateThurs= workModel.startDateThurs;
    //    this.endDateThurs= workModel.endDateThurs;

    //    this.isWorkActiveFri= workModel.isWorkActiveFri;
    //    this.workTextFri= workModel.workTextFri;
    //    this.startDateFri= workModel.startDateFri;
    //    this.endDateFri= workModel.endDateFri;

    //    this.isWorkActiveSat= workModel.isWorkActiveSat;
    //    this.workTextSat= workModel.workTextSat;
    //    this.startDateSat= workModel.startDateSat;
    //    this.endDateSat= workModel.endDateSat;






    //    this.startDateSunSP= workModel.startDateSunSP;
    //    this.endDateSunSP= workModel.endDateSunSP;

    //    this.startDateMonSP= workModel.startDateMonSP;
    //    this.endDateMonSP= workModel.endDateMonSP;


    //    this.startDateTuesSP= workModel.startDateTuesSP;
    //    this.endDateTuesSP= workModel.endDateTuesSP;


    //    this.startDateWedSP= workModel.startDateWedSP;
    //    this.endDateWedSP= workModel.endDateWedSP;


    //    this.startDateThursSP= workModel.startDateThursSP;
    //    this.endDateThursSP= workModel.endDateThursSP;


    //    this.startDateFriSP= workModel.startDateFriSP;
    //    this.endDateFriSP= workModel.endDateFriSP;


    //    this.startDateSatSP= workModel.startDateSatSP;
    //    this.endDateSatSP= workModel.endDateSatSP;

       this.isWorkActive= true;
       this.userID = userID;
       this.active = true;
    //    if (workModel.workTextEN != "" && workModel.workTextEN != null) {
    //     var array2 = workModel.workTextEN.split(',')
    //     array2.forEach(element => {
    //         let x = new Date(element);
    //         this.unAvailableBookingDates.push(x);
    //         this.showUnavailableDates = true;
    //     });

    // } else {
    //     this.unAvailableBookingDates = [];
    //     this.showUnavailableDates = true;
    // }
  

       this.modal.show();

      });

 
}


close(): void {
        this.active = false;
       this.modal.hide();
       this.modalSave.emit(null);

    }

    ngOnInit() {
            this.theme = ThemeHelper.getTheme();
    }







    saveSetting() {
        let objWorkModel = new  WorkModel();
        objWorkModel.workTextAR=this.workTextAR;
        //objWorkModel.workTextEN=this.workTextEN;

        objWorkModel.isWorkActiveSun=this.isWorkActiveSun;
        objWorkModel.workTextSun=this.workTextSun;
        objWorkModel.startDateSun=moment(this.startDateSun, 'HH:mm A');
        objWorkModel.endDateSun= moment(this.endDateSun, 'HH:mm A');
        objWorkModel.startDateSunSP=moment(this.startDateSunSP, 'HH:mm A');
        objWorkModel.endDateSunSP= moment(this.endDateSunSP, 'HH:mm A');



        objWorkModel.hasSPSun = this.hasSPSun;
        if (this.hasSPSun) {
            objWorkModel.startDateSunSP = moment(this.startDateSunSP, 'HH:mm A');
            objWorkModel.endDateSunSP = moment(this.endDateSunSP, 'HH:mm A');
        } else {
            objWorkModel.startDateSunSP = objWorkModel.startDateSun;
            objWorkModel.endDateSunSP = objWorkModel.endDateSun;
        }


        objWorkModel.isWorkActiveMon=this.isWorkActiveMon;
        objWorkModel.workTextMon=this.workTextMon;
        objWorkModel.startDateMon=moment(this.startDateMon, 'HH:mm A');
        objWorkModel.endDateMon=moment(this.endDateMon, 'HH:mm A');
        objWorkModel.startDateMonSP=moment(this.startDateMonSP, 'HH:mm A');
        objWorkModel.endDateMonSP=moment(this.endDateMonSP, 'HH:mm A');
        objWorkModel.hasSPMon = this.hasSPMon;
        if (this.hasSPMon) {
            objWorkModel.startDateMonSP = moment(this.startDateMonSP, 'HH:mm A');
            objWorkModel.endDateMonSP = moment(this.endDateMonSP, 'HH:mm A');
        } else {
            objWorkModel.startDateMonSP = objWorkModel.startDateMon;
            objWorkModel.endDateMonSP = objWorkModel.endDateMon;
        }



        objWorkModel.isWorkActiveTues=this.isWorkActiveTues;
        objWorkModel.workTextTues=this.workTextTues;
        objWorkModel.startDateTues=moment(this.startDateTues, 'HH:mm A');
        objWorkModel.endDateTues=moment(this.endDateTues, 'HH:mm A');
        objWorkModel.startDateTuesSP=moment(this.startDateTuesSP, 'HH:mm A');
        objWorkModel.endDateTuesSP=moment(this.endDateTuesSP, 'HH:mm A');
        objWorkModel.hasSPTues = this.hasSPTues;
        if (this.hasSPTues) {
            objWorkModel.startDateTuesSP = moment(this.startDateTuesSP, 'HH:mm A');
            objWorkModel.endDateTuesSP = moment(this.endDateTuesSP, 'HH:mm A');
        }
        else {
            objWorkModel.startDateTuesSP = objWorkModel.startDateTues;
            objWorkModel.endDateTuesSP = objWorkModel.endDateTues;
        }




        objWorkModel.isWorkActiveWed=this.isWorkActiveWed;
        objWorkModel.workTextWed=this.workTextWed;
        objWorkModel.startDateWed=moment(this.startDateWed, 'HH:mm A');
        objWorkModel.endDateWed= moment(this.endDateWed, 'HH:mm A');
        objWorkModel.startDateWedSP=moment(this.startDateWedSP, 'HH:mm A');
        objWorkModel.endDateWedSP= moment(this.endDateWedSP, 'HH:mm A');
        objWorkModel.hasSPWed = this.hasSPWed;
        if (this.hasSPWed) {
            objWorkModel.startDateWedSP = moment(this.startDateWedSP, 'HH:mm A');
            objWorkModel.endDateWedSP = moment(this.endDateWedSP, 'HH:mm A');
        }
        else {
            objWorkModel.startDateWedSP = objWorkModel.startDateWed;
            objWorkModel.endDateWedSP = objWorkModel.endDateWed;
        }



        objWorkModel.isWorkActiveThurs=this.isWorkActiveThurs;
        objWorkModel.workTextThurs=this.workTextThurs;
        objWorkModel.startDateThurs=moment(this.startDateThurs, 'HH:mm A');
        objWorkModel.endDateThurs=moment(this.endDateThurs, 'HH:mm A');
        objWorkModel.startDateThursSP=moment(this.startDateThursSP, 'HH:mm A');
        objWorkModel.endDateThursSP=moment(this.endDateThursSP, 'HH:mm A');
        objWorkModel.hasSPThurs = this.hasSPThurs;
        if (this.hasSPThurs) {
            objWorkModel.startDateThursSP = moment(this.startDateThursSP, 'HH:mm A');
            objWorkModel.endDateThursSP = moment(this.endDateThursSP, 'HH:mm A');
        }
        else {
            objWorkModel.startDateThursSP = objWorkModel.startDateThurs;
            objWorkModel.endDateThursSP = objWorkModel.endDateThurs;
        }




        objWorkModel.isWorkActiveFri=this.isWorkActiveFri;
        objWorkModel.workTextFri=this.workTextFri;
        objWorkModel.startDateFri=moment(this.startDateFri, 'HH:mm A');
        objWorkModel.endDateFri=moment(this.endDateFri, 'HH:mm A');
        objWorkModel.startDateFriSP=moment(this.startDateFriSP, 'HH:mm A');
        objWorkModel.endDateFriSP=moment(this.endDateFriSP, 'HH:mm A');
        objWorkModel.hasSPFri = this.hasSPFri;
        if (this.hasSPFri) {
            objWorkModel.startDateFriSP = moment(this.startDateFriSP, 'HH:mm A');
            objWorkModel.endDateFriSP = moment(this.endDateFriSP, 'HH:mm A');
        }
        else {
            objWorkModel.startDateFriSP = objWorkModel.startDateFri;
            objWorkModel.endDateFriSP = objWorkModel.endDateFri;
        }


        objWorkModel.isWorkActiveSat=this.isWorkActiveSat;
        objWorkModel.workTextSat=this.workTextSat;
        objWorkModel.startDateSat=moment(this.startDateSat, 'HH:mm A');
        objWorkModel.endDateSat=moment(this.endDateSat, 'HH:mm A');
        objWorkModel.startDateSatSP=moment(this.startDateSatSP, 'HH:mm A');
        objWorkModel.endDateSatSP=moment(this.endDateSatSP, 'HH:mm A');
        objWorkModel.hasSPSat = this.hasSPSat;
        if (this.hasSPSat) {
            objWorkModel.startDateSatSP = moment(this.startDateSatSP, 'HH:mm A');
            objWorkModel.endDateSatSP = moment(this.endDateSatSP, 'HH:mm A');
        } else {
            objWorkModel.startDateSatSP = objWorkModel.startDateSat;
            objWorkModel.endDateSatSP = objWorkModel.endDateSat;

        }


        if(this.isWorkActiveSat){
            if(this.startDateSat >= this.endDateSat || this.endDateSat <= this.startDateSat){
           this.message.error( this.l('workingHoursError'), this.l('invalidDateInSaturday'));
           return
       }
       }
      
       if(this.isWorkActiveSat && this.hasSPSat){
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

           if(this.isWorkActiveSun && this.hasSPSun){ 
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

    
           if(this.isWorkActiveMon && this.hasSPMon){
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
     
           if(this.isWorkActiveTues && this.hasSPTues){
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


           if(this.isWorkActiveWed && this.hasSPWed){
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
                   this.message.error( this.l('workingHoursError'), this.l('invalidDateInThursday'));
                   return
               }
           }

           if(this.isWorkActiveThurs && this.hasSPThurs){
                if((this.startDateThursSP >= this.startDateThurs && this.startDateThursSP) <= this.endDateThurs ||
                   
                  (this.endDateThursSP <= this.endDateThurs && this.endDateThursSP) >= this.startDateThurs ||
                  this.startDateThursSP >= this.endDateThursSP || this.endDateThursSP <= this.startDateThursSP || 
                  (this.startDateThurs <= this.endDateThursSP && this.endDateThurs >= this.startDateThursSP )
                   ){
                       this.message.error( this.l('workingHoursError'), this.l('invalidDateInThursday'));
                       return
                   }
           }

              

           if(this.isWorkActiveFri){
                 if(this.startDateFri >= this.endDateFri || this.endDateFri <= this.startDateFri){
                       this.message.error( this.l('workingHoursError'), this.l('invalidDateInFriday'));
                       return
                   }
           }

           if(this.isWorkActiveFri && this.hasSPFri){
             if((this.startDateFriSP >= this.startDateFri && this.startDateFriSP) <= this.endDateFri ||
                       
                      (this.endDateFriSP <= this.endDateFri && this.endDateFriSP) >= this.startDateFri ||
                      this.startDateFriSP >= this.endDateFriSP || this.endDateFriSP <= this.startDateFriSP || 
                      (this.startDateFri <= this.endDateFriSP && this.endDateFri >= this.startDateFriSP )
                       ){
                           this.message.error( this.l('workingHoursError'), this.l('invalidDateInFriday'));
                           return
                       }

           }
                 

        //    if(this.unAvailableBookingDates.length >= 1){
        //    this.unAvailableBookingDates.forEach(unAvailableBookingDate => {
        //      let date = moment(unAvailableBookingDate).format("MM/DD/yyyy");
        //     this.listUanvailableDates.push(date);
        //     })
        // }

          
        // objWorkModel.workTextEN = this.listUanvailableDates.join();

        this._userServiceProxy.saveSetting(this.userID,objWorkModel).subscribe(response => {
            this.notify.info(this.l('savedSuccessfully'));

            this.modal.hide();

            this.modalSave.emit(null);
        });
    }










}
