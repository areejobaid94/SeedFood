import { Component, ViewChild, Injector, Output, EventEmitter, Renderer2, Inject } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetBillingForViewDto, BillingDto, AccountBillingsServiceProxy, GetAccountBillingForViewDto, TenantServicesServiceProxy, GetTenantServiceForViewDto, HostedCheckoutServiceProxy, CheckoutModel } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { DOCUMENT } from '@angular/common';
import { AppConsts } from '@shared/AppConsts';
declare function setCard(data:any) :any;
declare function setTotal(data:number) :any;


declare function errorCallback(data:any) :any;
declare function cancelCallback() :any;
declare function beforeRedirect() :any;
declare function afterRedirect(data:any) :any;
declare function configureF() :any;

declare function setBaseUrl(data:any) :any;
declare function setFrontEndUrl(data:any) :any;


declare function setBillingId(data:any) :any;

declare function completeCallback(data:any) :any;


declare function setorderDescription(data:any) :any;

@Component({
    selector: 'viewBillingHostModal',
    templateUrl: './view-billingsHost-modal.component.html',
    
              
})
              
              

export class ViewBillingsHostModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    checkoutModel:CheckoutModel;
    item: GetBillingForViewDto;
    itemDeitil: GetAccountBillingForViewDto[];
    phoneNumber:string;
    nameTenant:string;
    imageSrc:string;
    //  item2:GetTenantServiceForViewDto;

    constructor(
        injector: Injector,
        private _renderer2: Renderer2, 
        @Inject(DOCUMENT) private _document: Document,
        private hostedCheckoutServiceProxy: HostedCheckoutServiceProxy,
        private _appSessionService: AppSessionService,
        private _accountBillingsServiceProxy: AccountBillingsServiceProxy,
        private _tenantServicesServiceProxy: TenantServicesServiceProxy,
    ) {
        super(injector);
        this.item = new GetBillingForViewDto();
        this.item.billing = new BillingDto();
    }


    show(item: GetBillingForViewDto): void {
        setBillingId(item.billing.id);
        setBaseUrl(AppConsts.remoteServiceBaseUrl);
        setFrontEndUrl(AppConsts.appBaseUrl);

        this.hostedCheckoutServiceProxy.hostedCheckout2().subscribe(result => { 
            this.checkoutModel = result;
            this.phoneNumber="34565432345654";

            this.nameTenant="";//this._appSessionService.tenant.name;
            this.imageSrc="";//this._appSessionService.tenant.image;

            setCard(result);
        
            setTotal(item.billing.totalAmount);

            setorderDescription(this.nameTenant+"/ "+this.phoneNumber);
           // completeCallback(true);
            //beforeRedirect(result);
           // afterRedirect(result);
           
            configureF();


           
           
   
            this.item = item;
            this._accountBillingsServiceProxy.getAccountBillingForView(item.billing.id).subscribe(result => { 
                this.itemDeitil = result;
                
                this.active = true;
                this.modal.show();
            });
          
            
        });
        

    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
    printPage() {

        const printContent = document.getElementById("PrintBillingID");
        const WindowPrt = window.open('', '', '');
        WindowPrt.document.write(printContent.innerHTML);
        WindowPrt.document.close();
        WindowPrt.focus();
        WindowPrt.print();
        WindowPrt.close();

        // window.print();
      }
}
