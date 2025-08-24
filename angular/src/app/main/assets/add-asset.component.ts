import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AssetAttachmentDto, AssetDto, AssetLevelFourDto, AssetLevelOneDto, AssetLevelThreeDto, AssetLevelTwoDto, AssetServiceProxy, AssetStatus, ItemsServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DomSanitizer } from '@angular/platform-browser';


@Component({
  selector: 'AddAssetModal',
  templateUrl: './add-asset.component.html',
  styleUrls: ['./add-asset.component.css'],

})
export class AddAssetComponent extends AppComponentBase {
    theme:string;
    submitted =false;

    constructor(
        injector: Injector,
        private _assetServiceProxy: AssetServiceProxy,
        private _itemsServiceProxy: ItemsServiceProxy,
        public darkModeService: DarkModeService,
        public sanitizer: DomSanitizer

    )
    {
        super(injector);
        this.AssetAttachmentDto=new AssetAttachmentDto( );
    }
    ngOnInit(): void {
        this.theme= ThemeHelper.getTheme();
      }
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();



    active = false;
    saving = false;

    levelFlag :boolean = false;
    levelFourFlag :boolean = true;


    assetlevelOneFlag : boolean = true;
    assetlevelTwoFlag : boolean = true;
    assetlevelThreeFlag : boolean = true;
    assetlevelFourFlag : boolean = true;

    assetLevelOne:AssetLevelOneDto[];
    assetLevelTwo:AssetLevelTwoDto[];
    assetLevelTwoMain :AssetLevelTwoDto[];

    assetLevelThree:AssetLevelThreeDto[];
    assetLevelThreeMain :AssetLevelThreeDto[];

    assetLevelFour:AssetLevelFourDto[];
    assetLevelFourMain :AssetLevelFourDto[];


    asset: AssetDto=new AssetDto();


    assetNameAr!: string | undefined;
    assetNameEn!: string | undefined;

    assetDescriptionAr!: string | undefined;
    assetDescriptionEn!: string | undefined;

    assetLevelOneId!: number | undefined;
    assetLevelTwoId!: number | undefined;
    assetLevelThreeId!: number | undefined;
    assetLevelFourId!: number | undefined;

    isOffer:boolean = false;
    assetTypeId!: number | undefined;

    assetStatus = AssetStatus.Active;
    assetStatusId!: number;

    lstAssetAttachmentDto:Array<AssetAttachmentDto> = [];
    AssetAttachmentDto!:AssetAttachmentDto| undefined;

    imagSrc:string ='';
    imagType : string = '';
    imagService : boolean = true;

    assetDto: AssetDto;
    isEdit :boolean
    radioButtonFlag :boolean =false;
    typeName1 : string = "Sell"
    typeName2 :string="Rent"


    show(): void {
        this.radioButtonFlag  =false;
        this.isEdit = false;
        this.active = true;

        if(this.appSession.tenantId===52)
        {
            this.radioButtonFlag = true;
            this.typeName1 = "Zero"
            this.typeName2 = "Used"
        }
        if(this.appSession.tenantId===40)
        {
            this.radioButtonFlag = true;
            this.typeName1 = "Sell"
            this.typeName2 = "Rent"
        }
        // if(this.appSession.tenantId===45 || this.appSession.tenantId===56 || this.appSession.tenantId===58|| this.appSession.tenantId===60 || this.appSession.tenantId===59)
        // {
        //     this.radioButtonFlag =false
        // }

        this.assetLevelTwo=[] ;
        this.assetLevelOne=[];
        this.assetLevelThree=[] ;
        this.assetLevelFour=[];
        this.AssetAttachmentDto=new AssetAttachmentDto();
        this.lstAssetAttachmentDto = [];
        this.asset=new AssetDto() ;

        this._assetServiceProxy.loadLevels(null).subscribe((result: any) => {

            this.assetLevelOne=result.lstAssetLevelOneDto;
            this.assetlevelOneFlag =true;

            if (result.lstAssetLevelOneDto== undefined) 
            {
                this.assetlevelOneFlag =false;
                this.assetlevelTwoFlag =false;
                this.assetlevelThreeFlag =false;
                this.assetlevelFourFlag =false;
            }
            else{
                this.assetLevelOne=[ new AssetLevelOneDto()]
                this.assetLevelOne=result.lstAssetLevelOneDto;
                this.assetlevelOneFlag =true;
                if (result.lstAssetLevelTwoDto== undefined) 
                {
                    this.assetlevelTwoFlag =false;
                    this.assetlevelThreeFlag =false;
                    this.assetlevelFourFlag =false;
                }
                else{
                    this.assetlevelTwoFlag =true;
                    this.assetLevelTwoMain=result.lstAssetLevelTwoDto;
                    if (result.lstAssetLevelThreeDto== undefined) 
                    {
                        this.assetlevelThreeFlag =false;
                        this.assetlevelFourFlag =false;

                    }
                    else{
                        this.assetlevelThreeFlag =true;
                        this.assetLevelThreeMain=result.lstAssetLevelThreeDto;

                        if (result.lstAssetLevelFourDto == undefined) 
                        {
                            this.assetlevelFourFlag =false;
                        }
                        else{
                            this.assetlevelFourFlag =true;
                            this.assetLevelFourMain=result.lstAssetLevelFourDto;
                        }
                    }
                }
            }
            this.assetLevelOne=result.lstAssetLevelOneDto;
            this.assetLevelTwoMain=result.lstAssetLevelTwoDto;
            this.assetLevelThreeMain=result.lstAssetLevelThreeDto;
            this.assetLevelFourMain=result.lstAssetLevelFourDto;
        this.modal.show();
        });
    }



    showEdit(records : AssetDto ): void {
        this.asset=new AssetDto() ;

        this.active = true;
        this.isEdit = true;
        this.lstAssetAttachmentDto = [];

        if(this.appSession.tenantId===52)
        {
            this.typeName1 = "Zero"
            this.typeName2 = "Used"
        }
        if(this.appSession.tenantId===45|| this.appSession.tenantId===56 || this.appSession.tenantId===58|| this.appSession.tenantId===60|| this.appSession.tenantId===59)
        {
            this.radioButtonFlag =false
        }
        this.asset.lstAssetAttachmentDto=records.lstAssetAttachmentDto;
        this.lstAssetAttachmentDto =records.lstAssetAttachmentDto;


        this.assetLevelOne=[ new AssetLevelOneDto()]
        this.assetLevelTwo=[ new AssetLevelTwoDto()]
        this.assetLevelThree=[new AssetLevelThreeDto()]
        this.assetLevelFour=[new AssetLevelFourDto()]

        
        this._assetServiceProxy.loadLevels(null).subscribe((result: any) => {

        const levelId = records.assetLevelOneId;
        const levelTwoId = records.assetLevelTwoId;
        const levelThreeId = records.assetLevelThreeId;

        
        if (result.lstAssetLevelOneDto== undefined) 
        {
            
            this.assetlevelOneFlag =false;
      
            this.assetlevelTwoFlag =false;

            this.assetlevelThreeFlag =false;

            this.assetlevelFourFlag =false;
        }
        else
        {
            
            this.assetLevelOne=result.lstAssetLevelOneDto;
            if (result.lstAssetLevelTwoDto == undefined || records.assetLevelTwoId == 0) 
            {
                
                this.assetlevelTwoFlag =false;

                this.assetlevelThreeFlag =false;
                
                this.assetlevelFourFlag =false;
            }
            else
            {
                
                this.assetLevelTwoMain=result.lstAssetLevelTwoDto;
                this.assetLevelTwoMain.forEach(element => {
                    if(element.levelOneId==levelId){
                        this.assetLevelTwo.push(element);
                        
                    }
                });
                if (this.assetLevelTwo.length == 1) {
                    
                    
                    this.assetlevelTwoFlag =false;
                }
                if (result.lstAssetLevelThreeDto== undefined || records.assetLevelThreeId == 0) 
                {
                    
                    this.assetlevelThreeFlag =false;
                    
                    this.assetlevelFourFlag =false;
                }
                else
                {
                    
                    this.assetLevelThreeMain=result.lstAssetLevelThreeDto;
                    this.assetLevelThreeMain.forEach(element => {
                        if(element.levelTwoId==levelTwoId)
                        {
                            this.assetLevelThree.push(element);
                            
                        }
                    });
                    if (this.assetLevelThree.length == 1) {
                        
                        
                        this.assetlevelThreeFlag =false;
                    }
                    if (result.lstAssetLevelFourDto == undefined || records.assetLevelFourId == 0) 
                    {
                        this.assetlevelFourFlag =false;
                    }
                    else
                    {
                        
                        this.assetLevelFourMain=result.lstAssetLevelFourDto;
                        this.assetLevelFourMain.forEach(element => {
                        if(element.levelThreeId==levelThreeId)
                        {
                            this.assetLevelFour.push(element);
                        }
                    });
                    if (this.assetLevelFour.length == 1) {
                        
                        
                        this.assetlevelFourFlag =false;
                    }
                    }
                }
            }
        }

            this.asset = records;
            this.asset.assetLevelOneId = Number(records.assetLevelOneId)
            this.asset.assetLevelTwoId = Number(records.assetLevelTwoId)
            this.asset.assetLevelThreeId = Number(records.assetLevelThreeId)
            this.asset.assetLevelFourId = Number(records.assetLevelFourId)
            
            this.active = true;
            this.modal.show();
        });
        };
        save(): void {
            this.saving=true;
            
            if(this.asset.assetNameEn === null || this.asset.assetNameEn === undefined || this.asset.assetNameEn === '' ||
            this.asset.assetNameAr === null || this.asset.assetNameAr === undefined  || this.asset.assetNameAr === '' ||
            this.asset.assetDescriptionAr === null || this.asset.assetDescriptionAr === undefined || this.asset.assetDescriptionAr === ''||
            this.asset.assetDescriptionEn === null || this.asset.assetDescriptionEn === undefined || this.asset.assetDescriptionEn === ''||
            this.asset.lstAssetAttachmentDto == null || this.asset.lstAssetAttachmentDto.length == 0
            ){
                this.submitted= true;
                this.saving=false;
                return;
            }
            if((this.assetlevelOneFlag && (this.asset.assetLevelOneId === null || this.asset.assetLevelOneId === undefined)) || (this.assetlevelTwoFlag && (this.asset.assetLevelTwoId === null || this.asset.assetLevelTwoId === undefined)) || (this.assetlevelThreeFlag && (this.asset.assetLevelThreeId === null || this.asset.assetLevelThreeId === undefined)) || (this.assetlevelFourFlag && (this.asset.assetLevelFourId === null || this.asset.assetLevelFourId === undefined))){
                this.submitted= true;
                this.saving=false;
                return;
            }

            if (this.radioButtonFlag ==false) 
            {
                this.asset.assetTypeId = 0
            }
            if (this.asset.lstAssetAttachmentDto == null || this.asset.lstAssetAttachmentDto.length == 0)
            {
                this.saving=false
                this.message.error('',this.l('pleaseFillAttachment'));
            }
            else
            {
                if(this.asset.assetLevelOneId == null || this.asset.assetLevelOneId == undefined){
                    this.asset.assetLevelOneId = 0;
                }
                if(this.asset.assetLevelTwoId == null || this.asset.assetLevelTwoId == undefined){
                    this.asset.assetLevelTwoId = 0;
                }
                if(this.asset.assetLevelFourId == null || this.asset.assetLevelFourId == undefined){
                    this.asset.assetLevelFourId = 0;
                }
                if(this.asset.assetLevelThreeId == null || this.asset.assetLevelThreeId == undefined){
                    this.asset.assetLevelThreeId = 0;
                }

                this.asset.tenantId=this.appSession.tenant.id;
                if (this.asset.id == 0 || this.asset.id == null) {
                    this.asset.assetStatus = this.assetStatus;
                    this.asset.createdBy=this.appSession.user.name;
                    
                    this._assetServiceProxy.addAsset(this.asset)
                    .pipe(finalize(() => { this.saving = false;}))
                    .subscribe(() => {
                        this.notify.info(this.l('savedSuccessfully'));
                        this.submitted= false;
                        this.saving=false;
                        this.close();
                        this.modalSave.emit(null);
                        
                    });
                }else
                {
                    
                    this.asset.modifiedBy=this.appSession.user.name;
                    this._assetServiceProxy.updateAsset(this.asset)
                    .pipe(finalize(() => { this.saving = false;}))
                    .subscribe(() => {
                        this.notify.info(this.l('savedSuccessfully'));
                        this.close();
                        this.modalSave.emit(null);
                    });
            }
        }
        }
        checkIsOffer(){
            var offer = (document.getElementById("isOffer") as HTMLInputElement).checked ;
            if (offer) {
                
                this.asset.assetLevelOneId =0;
                this.asset.assetLevelTwoId =0;
                this.asset.assetLevelThreeId =0;
                this.asset.assetLevelFourId =0;
            }else{
                this.asset.assetLevelOneId = this.asset.assetLevelOneId || undefined;
                this.asset.assetLevelTwoId =this.asset.assetLevelTwoId || undefined;
                this.asset.assetLevelThreeId =this.asset.assetLevelThreeId || undefined;
                this.asset.assetLevelFourId = this.asset.assetLevelFourId || undefined;
            }
            return offer;
        }

    onChangeLevelOne(event): void
    {
        this.asset.assetLevelTwoId = null;

        const level1Id = event.target.value;
        this.assetLevelTwo=[ new AssetLevelTwoDto()]
        this.assetLevelThree=[new AssetLevelThreeDto()]
        this.assetLevelFour=[new AssetLevelFourDto()]
        

        this._assetServiceProxy.loadLevels(null).subscribe((result: any) => {
            
                this.assetLevelTwoMain=result.lstAssetLevelTwoDto;
                this.assetLevelTwoMain.forEach(element => {
                    this.assetlevelTwoFlag =true;
                    if(element.levelOneId==level1Id){
                        this.assetLevelTwo.push(element);
                    }
                    
                });
                if (this.assetLevelTwo.length == 1) {
                    
                    this.assetlevelTwoFlag =false;
                    
                    this.assetlevelThreeFlag =false;
                    
                    this.assetlevelFourFlag =false;
                }
                
                this.modalSave.emit(null);
        });
        this.asset.assetLevelOneId = event.target.value;
    }

    onChangeLevelTwo(event): void
    {
        const level2Id = event.target.value;
        this.asset.assetLevelThreeId = null;

        this.assetLevelThree=[new AssetLevelThreeDto()]
        this.assetLevelFour=[new AssetLevelFourDto()]
        this._assetServiceProxy.loadLevels(null).subscribe((result: any) => {
            
                this.assetLevelThreeMain=result.lstAssetLevelThreeDto;
                this.assetLevelThreeMain.forEach(element => {
                    
                    this.assetlevelThreeFlag =true;
                    if(element.levelTwoId==level2Id){
                        this.assetLevelThree.push(element);
                    }
                    
                });
                if (this.assetLevelThree.length <= 1) {
                    this.assetlevelThreeFlag =false;
                    this.assetlevelFourFlag =false;
                }
            
                this.modalSave.emit(null);
        });
        this.asset.assetLevelTwoId = event.target.value;

    }


    onChangeLevelThree(event): void
    {
        this.asset.assetLevelFourId = null;

        const level3Id = event.target.value;
        this.assetLevelFour=[new AssetLevelFourDto()]
        this._assetServiceProxy.loadLevels(null).subscribe((result: any) => {
            
            if (result.lstAssetLevelFourDto  == undefined) {
                this.assetlevelFourFlag =false;
                this.asset.assetLevelFourId =0;
            }else{

                this.assetLevelFourMain=result.lstAssetLevelFourDto;
                this.assetLevelFourMain.forEach(element => {
                    this.assetlevelFourFlag =true;
                    if(element.levelThreeId==level3Id){
                        this.assetLevelFour.push(element);
                    }
                    
                });
                if (this.assetLevelFour.length == 1) {
                    
                    
                    this.assetlevelFourFlag =false;
                }
                
            }
            this.modalSave.emit(null);
        });

    }


    onFileChange(event) {
        
        const reader = new FileReader();
        if (event.target.files && event.target.files.length)
        {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            let form = new FormData();
            form.append('FormFile', file);
            this.AssetAttachmentDto=new AssetAttachmentDto( );

        this._itemsServiceProxy.getImageUrl(form).subscribe(res => {
            
            this.AssetAttachmentDto.attachmentUrl=res['result'];
            this.AssetAttachmentDto.attachmentName= event.target.files[0].name;

            if (event.target.files[0].type.match(/image/)) {
                this.AssetAttachmentDto.attachmentType = "image";
            }
            else if(event.target.files[0].type.match(/video/)){
                this.AssetAttachmentDto.attachmentType = "video";
            }
            else if(event.target.files[0].type.match(/application/)){
                this.AssetAttachmentDto.attachmentType = "file";
            }
            
            if(this.lstAssetAttachmentDto == null){
                this.lstAssetAttachmentDto = [];
            }
            this.lstAssetAttachmentDto.push(this.AssetAttachmentDto);
            this.asset.lstAssetAttachmentDto=this.lstAssetAttachmentDto;
            this.asset.lstAssetAttachmentDto.forEach(asset => {
                if(asset.attachmentType== 'file'){
                  var splits = asset.attachmentName.split(".");
                  let pdf=splits[1];
                  if(pdf == 'xlsx'){
      
                  asset.attachmentUrl= 'https://view.officeapps.live.com/op/embed.aspx?src='+ asset.attachmentUrl;
               
                  }}
            
              });
            
            (<HTMLInputElement>document.getElementById("file")).value = null
      
        });
        }
    }
    DeleteItem(idx){
        this.asset.lstAssetAttachmentDto.splice(idx,1);
        
    }
    close(): void {
        this.assetlevelTwoFlag =true;        
        this.assetlevelThreeFlag =true;        
        this.assetlevelFourFlag =true;
        this.active = false;
         this.submitted= false;
         this.saving=false;
        this.modal.hide();
        this.modalSave.emit(null);
    }
}
