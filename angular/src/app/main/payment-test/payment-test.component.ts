import { Component,Injector, OnInit } from '@angular/core';
import {appModuleAnimation} from "@shared/animations/routerTransition";
import {AppComponentBase} from "@shared/common/app-component-base";
import { HyperPayPaymentServiceServiceProxy, CheckoutPaymentRequestDto, BillingsServiceProxy  } from '@shared/service-proxies/service-proxies';

import {AppConsts} from "@shared/AppConsts";
import { ActivatedRoute } from '@angular/router';
import { CreateOrEditBillingModalComponent } from '../billings/billings/create-or-edit-billing-modal.component';
import { ViewChild } from '@angular/core';


@Component({
  selector: 'app-payment-test',
  templateUrl: './payment-test.component.html',
  styleUrls: ['./payment-test.component.css'],
  animations: [appModuleAnimation()]

})
export class PaymentTestComponent extends AppComponentBase implements OnInit {

  @ViewChild('createOrEditBillingModal', { static: true }) createOrEditBillingModal: CreateOrEditBillingModalComponent;

  public shopperResultUrl:string = 'www.google.com';
  public checkoutId : string;
  //public paymentUrl : string = 'https://test.oppwa.com/v1/paymentWidgets.js?checkoutId='+ this.checkoutId;
  _requestData:CheckoutPaymentRequestDto = new CheckoutPaymentRequestDto() ;

  id:any;
  BillingModal:any;
  filters: {
    selectedIdBilling: number;
} = <any>{};


  constructor( injector: Injector,
    private _billingsServiceProxy: BillingsServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _payemntProxy:HyperPayPaymentServiceServiceProxy) { super(injector);
      this.setFiltersFromRoute();
  }
  setFiltersFromRoute(): void {
    
    if (this._activatedRoute.snapshot.queryParams['IdBilling'] != null) {
        this.filters.selectedIdBilling = parseInt(this._activatedRoute.snapshot.queryParams['IdBilling']);
    }
}
  ngOnInit(): void {
  
    this._billingsServiceProxy.getBillingForEdit(this.filters.selectedIdBilling).subscribe(result => {  
         
      this.BillingModal = result.billing;
      this._requestData.amount = this.BillingModal.totalAmount;
      this._requestData.currency="JOD";
      this._requestData.paymentBrand = "";
      this._requestData.paymentType="DB";
      this._requestData.taxAmount=2;
      var checkoutId : string;
  
       this._payemntProxy.checkout(this._requestData).subscribe(result=>{  
          
         let el = document.createElement('script');
         el.setAttribute('src', result.checkoutUrl);
         document.body.appendChild(el);
       });
    });


  }

}
