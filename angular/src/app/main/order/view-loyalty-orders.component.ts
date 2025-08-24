import { Component, Injector, OnInit, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { OrdersServiceProxy } from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import * as rtlDetect from "rtl-detect";
import 'moment-hijri'; // Hijri extension for moment.js
import moment from "moment";
const { toGregorian } = require('hijri-converter');
@Component({
    selector: "viewLoyaltyOrders",
    templateUrl: "./view-loyalty-orders.component.html",
    styleUrls: ["./view-loyalty-orders.component.css"],
})
export class ViewLoyaltyOrdersComponent extends AppComponentBase {
    @ViewChild("ViewLoyaltyOrders", { static: true }) modal: ModalDirective;
    @ViewChild("ViewLoyaltyOrdersComponent", { static: true })
    viewLoyaltyOrders: ViewLoyaltyOrdersComponent;

    pageSize = 10;
    pageNumber = 0;
    loyalityOrders = [];
    isLanguageArabic= false;


    constructor(
        injector: Injector,
        private _ordersServiceProxy: OrdersServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.isLanguageArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name)
    }
    show(contactId): void {
        this._ordersServiceProxy
            .getAllLoyaltyRemainingdays(
                contactId,
                this.appSession.tenantId,
                this.pageNumber,
                this.pageSize
            )
            .subscribe((result) => {
                this.loyalityOrders = result.lstOrder.sort(
                    (a, b) =>
                        b.loyaltyRemainingdays.createdDate.valueOf() -
                        a.loyaltyRemainingdays.createdDate.valueOf()
                );
                if(this.isLanguageArabic){
                    this.loyalityOrders.forEach(element => {
                        element.loyaltyRemainingdays.createdDate = this.convertHijriToGregorian(moment(element.loyaltyRemainingdays.createdDate).locale('en').format('YYYY-MM-DDTHH:mm:ss'));
                    });
                }
                this.modal.show();
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
    close(): void {
        this.modal.hide();
    }
}
