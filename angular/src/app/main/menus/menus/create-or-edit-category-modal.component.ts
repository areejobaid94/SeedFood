import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
    MenusServiceProxy,
    ItemCategoryServiceProxy,
    CreateOrEditMenuCategoryDto,
    ItemsServiceProxy, RestaurantsTypeEunm, RType, LanguageBot
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '@app/services/dark-mode.service';
import { base64ToFile, ImageCroppedEvent } from 'ngx-image-cropper';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'createOrEditCategoryComponent',
    // styleUrls: ['./model-menus.component.less'],
    templateUrl: './create-or-edit-category-modal.component.html'
})

export class CreateOrEditCategoryModalComponent extends AppComponentBase implements OnInit {
    theme: string;
    loadImage = false;
    imageChangedEvent: any = '';
    file:any;
    fromFileUplode: any;
    @ViewChild('createOrEditCategoryComponent', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("file", {static: true }) element: ElementRef;
    active = false;
    saving = false;
    restaurantsTypeEunm = RestaurantsTypeEunm;
    enumKeys = [];
    menuId: number;
    menuForm: UntypedFormGroup;
    langId = 1;
    rType: RType[];
    languageBot: LanguageBot[];
    isNf: boolean;
    categoryImage: string;
    isMall: boolean;
    submitted = false;

    constructor(
        injector: Injector,
        private _menusServiceProxy: MenusServiceProxy,
        private fb: UntypedFormBuilder,
        private _menuCategoriesServiceProxy: ItemCategoryServiceProxy,
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
    ngAfterViewInit(): void {
        if (this.file !== undefined) {
            this.file.nativeElement.focus();
          }    }

    show(menuId?: number): void {
        this.isMall = false;
        this.menuForm = this.fb.group({
            name: ['', Validators.required],
            nameEnglish: ['', Validators.required],
            priority: [0, Validators.required],
            languageBotId: 1,
            categoryId: 0,
        });
        this.categoryImage = null;
        this.menuId = menuId;
        this.active = true;
        this.modal.show();
    }


    editCategory(menuId: number, objCategory: CreateOrEditMenuCategoryDto): void {
        this.isMall = false;
        this.menuForm = this.fb.group({
            name: objCategory.name,
            nameEnglish: objCategory.nameEnglish,
            priority: objCategory.priority,
            languageBotId: 1,
            categoryId: objCategory.id
        });
        this.categoryImage = objCategory.logoImag;
        this.menuId = menuId;
        this.active = true;
        this.modal.show();
    }


    close(): void {
        this.active = false;
        this.saving = false;
        this.submitted = false;
        this.modal.hide();
        this.element.nativeElement.value = "";
        this.loadImage=false;
    }

    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        await this.getIsAdmin();
        this.isNf = true;
        this._menusServiceProxy.getRType(this.appSession.tenantId).subscribe(result => {
            if (this.isNf) {
                this.rType = result;
            } else {
                this.rType = result;
            }
        });

        // this._menusServiceProxy.getLanguageBot().subscribe(result => {
        //     this.languageBot = result;
        // });


        this.isMall = false;
        this.menuForm = this.fb.group({
            name: [''],
            nameEnglish: [''],
            priority: 0,
            languageBotId: 1,
            categoryId: 0,
        });


    }

    addCategory() {
        this.saving = true;
        if (this.menuForm.invalid) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        this.submitted = false;
        let data = this.menuForm.value;
        let category = new CreateOrEditMenuCategoryDto();
        category.name = data.name;
        category.nameEnglish = data.nameEnglish;
        category.id = data.categoryId;
        category.priority = parseInt(data.priority);
        category.languageBotId = parseInt(data.languageBotId);
        category.logoImag = this.categoryImage;
        this.langId = parseInt(data.languageBotId);
        category.menuId = this.menuId;
        this._menuCategoriesServiceProxy.createOrEdit(category).subscribe(response => {
            this.notify.info(this.l('savedSuccessfully'));
            this.saving = false;
            this.submitted = false;
            this.modal.hide();
            this.modalSave.emit(null);
            this.element.nativeElement.value = "";
        },(error:any) =>{
            if(error){
                this.saving= false;
                this.submitted=false;
            }
        }
        
        
        );
    }

    onFileChange1(event,modalBasic) {
        if ( event.target.files[0]) {
            if( event.target.files[0].type === 'image/jpeg' || 
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
            this.categoryImage = res['result'];
            this.fromFileUplode = false;
            this.loadImage=false;
        });
         
}
}
