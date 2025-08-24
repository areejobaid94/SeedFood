import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ContactDto, ContactsInterestedOfModel, ContactsServiceProxy,GetOrderForViewDto,LoyaltyServiceProxy,OrderDetailsServiceProxy,OrderEntity,OrdersServiceProxy,TenantServicesInfoServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '@app/services/dark-mode.service';
import { finalize } from 'rxjs/operators';
import moment from 'moment';
import 'moment-hijri'; // Hijri extension for moment.js
const { toGregorian } = require('hijri-converter');
import * as rtlDetect from "rtl-detect";

@Component({
    selector: 'viewContactModal',
    templateUrl: './view-contact-modal.component.html'
})
export class ViewContactModalComponent extends AppComponentBase {
    theme: string;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    selected = '';
    loyalityOrders  = [];
    submitted = false;
    totalInterestedOf: number = 0;
    interestedOf: ContactsInterestedOfModel[] = [new ContactsInterestedOfModel()];
    item: ContactDto = new ContactDto();;
    creationDate : any;
    isTenantLoyal = false;
    currentPosition: number;
    prvHeight: number;
    pageNumberC = 0;
    pageSize = 10;
    pageNumber = 0;
    isOneTime: boolean;
    topnumber: any;
    heiget: any;
    contactId :number;
    isLanguageArabic= false;



    constructor(
        public darkModeService: DarkModeService,
        injector: Injector,
        private _contactsServiceProxy: ContactsServiceProxy,
        public tenantService: TenantServicesInfoServiceProxy,
        private _loyaltyServiceProxy: LoyaltyServiceProxy,
        private _ordersServiceProxy: OrdersServiceProxy,
    ) {
        super(injector);
        this.item = new ContactDto();
    }
    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isLanguageArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name)
        this.checkIsLoyality();
        await this.getIsAdmin();
    }

    show(item: ContactDto): void {
        if (this.isAdmin) {
            this.selected = 'editable';
        } else {
            this.selected = 'uneditable';
        }
        this.getContactById(item.id);
        this.viewInterestedOf(item);
        this.getLoyaltyOrders(item.id);
    }

    getContactById(contactId) {
        this._contactsServiceProxy.getContactbyId(contactId).subscribe(result => {
            this.item = result;
            if(this.item.creationTime != null || this.item.creationTime != undefined ){
              this.creationDate = moment(this.item.creationTime).locale('en').format('YYYY-MM-DDTHH:mm:ss');
              if(this.isLanguageArabic){
                this.creationDate = this.convertHijriToGregorian(moment(this.creationDate).locale('en').format('YYYY-MM-DDTHH:mm:ss'));
              }
            }
            this.active = true;
            this.getContactLoyalityPoints(this.appSession.tenantId,contactId)
        });

    }

    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split('T');
        const [year, month, day] = hijriDateString.split('-').map(Number);
        const gregorianDate = toGregorian(year, month, day);
    
        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, '0'),
            String(gregorianDate.gd).padStart(2, '0'),
        ].join('-');
    
        return formattedDate + 'T' + time;
    }

    getContactLoyalityPoints(tenantId,contactId){
        this.tenantService.getTenantsById(tenantId,contactId).subscribe(result => {
           this.item.loyalityPoint= result.originalLoyaltyPoints;
        });
    }

    viewInterestedOf(item: ContactDto) {
        this.interestedOf = [new ContactsInterestedOfModel()]
        this._contactsServiceProxy.getContactsInterested(this.appSession.tenantId, item.id).subscribe(result => {
            this.totalInterestedOf = result.length
            if (result.length > 0) {
                this.interestedOf = result;
            }
            this.modal.show();
        });
    }

    close(): void {
        this.active = false;
        this.selected = '';
        this.modal.hide();
        this.modalSave.emit(null);

    }
    save(): void {
        this.saving = true;
        if (
            this.item.displayName === null || this.item.displayName === undefined || this.item.displayName === ''
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }
        this._contactsServiceProxy.createOrEdit(this.item)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.submitted = false;
                this.saving = false;
                this.notify.info(this.l('savedSuccessfully'));
                this.close();

            });
    }

    checkIsLoyality() {
        this._loyaltyServiceProxy.isLoyalTenant().subscribe((result) => {
            this.isTenantLoyal = result;
        });
    }

    getLoyaltyOrders(contactId){
        this._ordersServiceProxy.getAllLoyaltyRemainingdays(contactId,this.appSession.tenantId,this.pageNumber,this.pageSize).subscribe((result) =>  {
            this.loyalityOrders = result.lstOrder.sort((a,b) => b.loyaltyRemainingdays.createdDate.valueOf() - a.loyaltyRemainingdays.createdDate.valueOf());
            this.contactId=contactId;
            if(this.isLanguageArabic){
                this.loyalityOrders.forEach(element => {
                    element.loyaltyRemainingdays.createdDate = this.convertHijriToGregorian(moment(element.loyaltyRemainingdays.createdDate).locale('en').format('YYYY-MM-DDTHH:mm:ss'));
                });
            }
        });
    }




}
