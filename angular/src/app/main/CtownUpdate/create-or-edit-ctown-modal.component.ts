import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CTownApiServiceProxy, ItemDto, ItemsServiceProxy, LoyaltyServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeHelper } from '@app/shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';

@Component({
    selector: 'createOrEditctownModal',
    templateUrl: './create-or-edit-ctown-modal.component.html'
})
export class CreateOrEditctownModalComponent extends AppComponentBase {
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    theme: string;
    isImageUpload = false;
    TypeId: number;
    imagSrc: string;
    item: ItemDto;
    fromFileUplode: any;
    categoryIndex: any;
    itemIndex: any;
    menuForm: any;
    isTenantLoyal: boolean;
    submitted = false;

    constructor(
        injector: Injector,
        private _itemsServiceProxy: ItemsServiceProxy,
        private _CTownApiServiceProxy: CTownApiServiceProxy,
        public darkModeService: DarkModeService,
        private _loyaltyServiceProxy: LoyaltyServiceProxy,

    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.checkIsLoyality();
    }
    show(item: ItemDto): void {

        this._CTownApiServiceProxy.getCtownItemForEdit(item.id).subscribe((res) => {

            this.theme = ThemeHelper.getTheme();
            this.imagSrc = item.imageUri;
            this.item = res;
            this.active = true;
            this.modal.show();
        });


    }
    checkIsLoyality() {
        this._loyaltyServiceProxy.isLoyalTenant().subscribe(result => {
            this.isTenantLoyal = result;
        })
    }

    save(): void {
        this.saving = true;
        if (this.item.itemName === null || this.item.itemName === undefined || this.item.itemName === '' ||
            this.item.itemNameEnglish === null || this.item.itemNameEnglish === undefined || this.item.itemNameEnglish === '' ||
            this.item.price === null || this.item.price === undefined ||
            this.item.priority === undefined || this.item.priority === null ||
            (this.isTenantLoyal && (this.item.isLoyal && (this.item.loyaltyPoints === null || this.item.loyaltyPoints === undefined)))
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        if (this.item.status_Code === null || this.item.status_Code === undefined) {
            this.item.status_Code = 2;
        }
        if (this.item.loyaltyPoints === null || this.item.loyaltyPoints === undefined) {
            this.item.loyaltyPoints = 0;
        }
        if (!this.isImageUpload) {
            // this.saving = true;
            if (this.imagSrc != null && this.imagSrc != '')
                this.item.imageUri = this.imagSrc;
            this._CTownApiServiceProxy.updateCtownItem(this.item)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('savedSuccessfully'));
                    this.close();
                    this.saving = false;
                    this.submitted = false;
                    this.modalSave.emit(null);
                });
        } else {

            this.isImageUpload = false;
        }
    }

    close(): void {
        this.active = false;
        this.saving = false;
        this.submitted = false;
        this.modal.hide();
    }

    onFileChange(event, item: ItemDto) {
        const reader = new FileReader();
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            let form = new FormData();
            form.append('FormFile', file);
            this._itemsServiceProxy.getImageUrl(form).subscribe(res => {
                this.imagSrc = res['result'];
                this.fromFileUplode = false;
            });
        }
    }

    getImage(item: ItemDto) {
        return item.imageUri;
    }

    openFileUploder(item: ItemDto) {
        this.isImageUpload = true;
        document.getElementById('uplodeImage').click();
    }
}
