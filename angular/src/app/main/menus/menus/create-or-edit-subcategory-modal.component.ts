import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
    MenusServiceProxy,
    CreateOrEditMenuCategoryDto,
    ItemsServiceProxy, RestaurantsTypeEunm, RType, LanguageBot, CreateOrEditMenuSubCategoryDto
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '@app/services/dark-mode.service';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { base64ToFile, ImageCroppedEvent } from 'ngx-image-cropper';

@Component({
    selector: 'createOrEditSubCategoryComponent',
    // styleUrls: ['./model-menus.component.less'],
    templateUrl: './create-or-edit-subcategory-modal.component.html'
})

export class CreateOrEditSubCategoryModalComponent extends AppComponentBase implements OnInit {
    theme: string;
    currency = '';
    loadImage = false;
    imageChangedEvent: any = '';
    file:any;
    fromFileUplode: any;
    @ViewChild('createOrEditSubCategoryComponent', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("file") element: ElementRef;

    active = false;
    saving = false;
    submitted = false;
    restaurantsTypeEunm = RestaurantsTypeEunm;
    enumKeys = [];
    menuId: number;
    subCategoryForm: UntypedFormGroup;
    langId = 1;
    rType: RType[];
    languageBot: LanguageBot[];
    isNf: boolean;
    subCategoryImage: string;
    isMall: boolean;
    constructor(
        injector: Injector,
        private _menusServiceProxy: MenusServiceProxy,
        private fb: UntypedFormBuilder,
        private _itemsServiceProxy: ItemsServiceProxy,
        public darkModeService: DarkModeService,
        private modalService: NgbModal,
        config: NgbModalConfig,

    ) {
        super(injector);
        config.backdrop = "static";
        config.keyboard = false;
        this.enumKeys = Object.keys(this.restaurantsTypeEunm);
    }


    show(menuId: number, selectcat: CreateOrEditMenuCategoryDto): void {
        // this.isNf = true;

        // this._menusServiceProxy.getRType(this.appSession.tenantId).subscribe(result => {
        //     if (this.isNf) {
        //         this.rType = result;
        //     } else {
        //         this.rType = result;
        //     }
        // });

        // this._menusServiceProxy.getLanguageBot().subscribe(result => {

        //     this.languageBot = result;
        // });

        this.currency = this.appSession.tenant.currencyCode;
        this.isMall = false;
        this.subCategoryForm = this.fb.group({
            categoryName: selectcat.name,
            name: [''],
            nameEnglish: [''],
            priority: 0,
            languageBotId: 1,
            categoryId: selectcat.id,
            subCategoryId: 0,
            isNew: true,
            price: null
        });
        this.subCategoryImage = null;
        this.menuId = menuId;
        this.active = true;
        this.modal.show();
    }


    editSubCategory(menuId: number, objSubCategory: CreateOrEditMenuSubCategoryDto, selectcat: CreateOrEditMenuCategoryDto): void {
        this.currency = this.appSession.tenant.currencyCode;
        this.isMall = false;
        this.subCategoryForm = this.fb.group({
            categoryName: selectcat.name,
            name: objSubCategory.name,
            nameEnglish: objSubCategory.nameEnglish,
            priority: objSubCategory.priority,
            languageBotId: 1,
            categoryId: objSubCategory.itemCategoryId,
            subCategoryId: objSubCategory.id,
            isNew: objSubCategory.isNew,
            price: objSubCategory.price,
        });
        this.subCategoryImage = objSubCategory.bgImag;
        this.menuId = menuId;
        this.active = true;
        this.modal.show();
    }



    close(): void {
        this.active = false;
        this.submitted = false;
        this.saving = false;
        this.modal.hide();
        this.loadImage = false;
        this.element.nativeElement.value = "";
    }

    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        await this.getIsAdmin()
        this.isNf = true;
        this._menusServiceProxy.getRType(this.appSession.tenantId).subscribe(result => {
            if (this.isNf) {
                this.rType = result;
            } else {
                this.rType = result;
            }
        });
        this._menusServiceProxy.getLanguageBot().subscribe(result => {

            this.languageBot = result;
        });
        this.isMall = false;
        this.subCategoryForm = this.fb.group({
            categoryName: [''],
            name: ['', Validators.required],
            nameEnglish: ['', Validators.required],
            priority: [0, Validators.required],
            languageBotId: 1,
            categoryId: 0,
            subCategoryId: 0,
            isNew: false,
            price: [0, Validators.required]
        });


    }

    addEditSubCategory() {
        this.saving = true
        if (this.subCategoryForm.invalid) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        this.submitted = true;
        this.saving = true;
        let data = this.subCategoryForm.value;
        let subCategory = new CreateOrEditMenuSubCategoryDto();
        subCategory.name = data.name;
        subCategory.nameEnglish = data.nameEnglish;
        subCategory.itemCategoryId = data.categoryId;
        subCategory.isNew = data.isNew;
        subCategory.price = data.price;
        subCategory.priority = parseInt(data.priority);
        subCategory.languageBotId = parseInt(data.languageBotId);
        subCategory.bgImag = this.subCategoryImage;
        subCategory.id = parseInt(data.subCategoryId);
        this.langId = parseInt(data.languageBotId);
        subCategory.menuId = this.menuId;
        if (subCategory.id > 0) {
            this._menusServiceProxy.updateSubCatogeory(subCategory).subscribe(response => {
                this.notify.info(this.l('savedSuccessfully'));
                this.submitted = false;
                this.saving = false;
                this.modal.hide();
                this.modalSave.emit(null);
                this.element.nativeElement.value = "";
            });
        } else {
            this._menusServiceProxy.addSubCatogeory(subCategory).subscribe(response => {
                this.notify.info(this.l('savedSuccessfully'));
                this.submitted = false;
                this.saving = false;
                this.modal.hide();
                this.modalSave.emit(null);
                this.element.nativeElement.value = "";
            },(error:any) =>{
                if(error){
                    this.saving= false;
                    this.submitted=false;
                }
            });
        }
    }



    onFileChange1(event,modalBasic) {
        if ( event.target.files[0]) {
            if(  event.target.files[0].type === 'image/jpeg' || 
            event.target.files[0].type === 'image/png' ||
            event.target.files[0].type === 'image/jpg'){
                this.modalOpen(modalBasic);
                this.imageChangedEvent = event;
            }else{
            this.message.error("",this.l("youCantUploadThisFile"));
            this.element.nativeElement.value = "";
            }
         
        }
    }

    modalOpen(modalBasic) {
        this.modalService.open(modalBasic, {
          windowClass: 'modal'
        });
      }
      
    imageCroppedFile(event: ImageCroppedEvent) {
        // Determine MIME type from base64 string
        const mimeType = this.extractMimeType(event.base64);
        const filename =
            "cropped-image"+ `${new Date().getTime()}` + this.getExtensionFromMimeType(mimeType);

        // Convert base64 string to File object
        const file = this.base64ToFile(event.base64, filename, mimeType);

        // Create FormData object and append the file
        const form = new FormData();
        form.append("FormFile", file);

        // Store the FormData object
        this.file = form;
    }

    saveImage(image){ 
        this.loadImage=true;
        this._itemsServiceProxy.getImageUrl(this.file).subscribe(res => {
            this.subCategoryImage = res['result'];
            this.fromFileUplode = false;
            this.loadImage=false;
        });
}
}
