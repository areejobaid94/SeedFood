import {Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef} from '@angular/core';
import {ModalDirective} from 'ngx-bootstrap/modal';
import {MenusServiceProxy,CreateOrEditMenuDto,ItemsServiceProxy,  RestaurantsTypeEunm, RType, TenantTypeEunm, MenuTypeEnum} from '@shared/service-proxies/service-proxies';
import {AppComponentBase} from '@shared/common/app-component-base';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { base64ToFile, ImageCroppedEvent } from 'ngx-image-cropper';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';


@Component({
    selector: 'createOrEditAdvancedMenuModal',
    styleUrls:['./create-or-edit-advanced-menu-modal.component.css'],
    //styleUrls: ['./create-or-edit-advanced-menu-modal.component.css'],
    templateUrl: './create-or-edit-advanced-menu-modal.component.html'
})

export class CreateOrEditAdvancedMenuModalComponent extends AppComponentBase implements OnInit {
    @ViewChild('AdvancedMenu', {static: true}) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("filee")
    filee: ElementRef;
    active = false;
    saving = false;
    isEdit :boolean
    menu: CreateOrEditMenuDto = new CreateOrEditMenuDto();
    menuTypeEnum: MenuTypeEnum;

    selectedValue: any;

    rType: RType[];
    menuType: number;
    priorityMenu: number;
    isNf: boolean;
    menuImage:string;
    isMall: boolean;
    dropdownSettings = {};
    selectedAreaIds: Array<any> = [];
    submitted= false;
    imageChangedEvent: any = '';
    file:any;


    constructor(
        injector: Injector,
        private _menusServiceProxy: MenusServiceProxy,
        private _itemsServiceProxy: ItemsServiceProxy,
        private _appSessionService: AppSessionService,
        private modalService: NgbModal,
        config: NgbModalConfig,

    ) {
    super(injector);
    config.backdrop = "static";
    config.keyboard = false;

    }
    public get selectedCategoryType(): RestaurantsTypeEunm {
        return this.selectedValue ? this.selectedValue.value : null;
    }
    async ngOnInit() {
        this.dropdownSettings = {
            singleSelection: false,
            idField: 'id',
            textField: 'name',
            itemsShowLimit: 3,
            allowSearchFilter: false,
            maxHeight: 200,
            closeDropDownOnSelection: true

        };
        await this.getIsAdmin();
        this._menusServiceProxy.getRType(this.appSession.tenantId).subscribe(result => {
            this.rType = result.filter(x => x.id != 0)
        })

    }
    show(): void {
        
        this.saving = false;
        this.isEdit = false;
        this.menu = new CreateOrEditMenuDto();
        this.selectedAreaIds = [];
        if(this._appSessionService.tenant.tenantType ==TenantTypeEunm.Mall){
            this.isMall=true;

        }else{
            this.isMall=false;
        }
        this.menuImage='';
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modalSave.emit(null);
        this.submitted= false;
        this.modal.hide();
    }

    save() {
        this.saving = true;

        if(
            this.menu.menuName === null ||   this.menu.menuName === undefined ||   this.menu.menuName === '' ||
            this.menu.menuNameEnglish === null ||  this.menu.menuNameEnglish === undefined ||   this.menu.menuNameEnglish === '' ||
            this.menuImage === null || this.menuImage === undefined || this.menuImage=== ''
            ){{
                this.submitted=true;
                this.saving=false;
                return;
            }}
        this.saving = true;
        this.menu.areaIds = this.selectedAreaIds.filter(f => f.id > 0).map(({ id }) => id).toString();
        this.menu.imageUri=this.menuImage;
        this.menu.languageBotId =1;
        this.menu.restaurantsType =0;
        this.menu.menuTypeId = MenuTypeEnum.Advance
        
        this._menusServiceProxy.createOrEdit(this.menu).subscribe(response => {
            this.notify.info(this.l('savedSuccessfully'));
            this.saving=false;
            this.modal.hide();
            this.modalSave.emit(null);
        },(error:any) =>{
            if(error){
                this.saving= false;
                this.submitted=false;
            }
        }
        );
    }
    showEdit(item: CreateOrEditMenuDto): void {
        
        this.saving = false;
        this.isEdit = true;

        this.selectedAreaIds = [];
        if (item.areaIds != undefined) {
            
            var array = item.areaIds.split(',')
            array.forEach(element => {
                var branch = this.rType.find(x => x.id == parseInt(element));
                if(branch != undefined){ 
                this.selectedAreaIds.push(branch)
                }
            });
        }
        this.menu = new CreateOrEditMenuDto();
        this.menu = item
        
        this.active = true;
        this.menuImage=item.imageUri
        this.modal.show();

    }
    openFileUploaded() {
        
        
        document.getElementById('upload').click();
    }
    
    onFileChange(event,modalBasic) {
        if ( event.target.files[0]) {
            if( event.target.files[0].type ==='image/svg+xml'){
                this.message.error("",this.l("cantYploadSvgImage"));
                this.filee.nativeElement.value = "";
            }else{
            this.modalOpen(modalBasic);
            this.imageChangedEvent = event;
            }
         
        }
       
    }
    modalOpen(modalBasic) {
        this.modalService.open(modalBasic, {
          windowClass: 'modal'
        });
      }

imageCroppedFile(event: ImageCroppedEvent) {
    const reader = new FileReader();
     const [file] = [<File>base64ToFile(event.base64)];
     reader.readAsDataURL(file);
     let form = new FormData();
     form.append('FormFile', file);
    this.file = form;
}
saveImage(image){ 
        this._itemsServiceProxy.getImageUrl(this.file).subscribe(res => {
            this.menuImage=res['result'];
        });
}

}
