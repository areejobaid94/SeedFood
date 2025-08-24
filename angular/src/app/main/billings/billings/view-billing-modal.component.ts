import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Billing_Address, Invoice } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DarkModeService } from '@app/services/dark-mode.service';
import { ThemeHelper } from '@app/shared/layout/themes/ThemeHelper';
import { NgxQrcodeElementTypes, NgxQrcodeErrorCorrectionLevels } from '@techiediaries/ngx-qrcode';

@Component({
    selector: 'viewBillingModal',
    templateUrl: './view-billing-modal.component.html',
})

export class ViewBillingModalComponent extends AppComponentBase {
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    theme: string;
    invoiceDetails = new Invoice();
    invoiceAddress = new Billing_Address();
    elementType = NgxQrcodeElementTypes.URL;
    correctionLevel = NgxQrcodeErrorCorrectionLevels.HIGH;
    currency = "";

    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
    ) {
        super(injector);
    }

    ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.currency = this.appSession.tenant.currencyCode;
    }


    show(billingInfo: Invoice): void {
        this.invoiceDetails = billingInfo;  
        this.invoiceAddress = billingInfo.billing_address;
        this.modal.show();
    }

    close(): void {
        this.modal.hide();
    }

}
