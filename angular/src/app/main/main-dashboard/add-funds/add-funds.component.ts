import { ModalDirective } from 'ngx-bootstrap/modal';
import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ConversationPriceModel, InvoicesWalletModel, TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { MainDashboardServiceService } from '../main-dashboard-service.service';
import { DarkModeService } from '@app/services/dark-mode.service';

@Component({
  selector: 'addFunds',
  templateUrl: './add-funds.component.html',
  styleUrls: ['./add-funds.component.css']
})
export class AddFundsComponent extends AppComponentBase {
  @ViewChild('addFunds', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  countries = [];
  selectedCountry = 'Jordan';
  totalAmount: number;
  totalAmountInSAR: number;
  conversationPrice: ConversationPriceModel = new ConversationPriceModel();
  invoiceWalletModel = new InvoicesWalletModel();
  url = '';
  saving = false;

  constructor(
    injector: Injector,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
    public dasboardService: MainDashboardServiceService,
    public darkModeService : DarkModeService
  ) {
    super(injector);
  }

  ngOnInit(): void {
  }


  show(): void {
    this.selectedCountry = 'Jordan';
    this.totalAmount = null;
    this.totalAmountInSAR = null;
    this.saving = false;
    this.getAllCountries();
  }

  validate(event : any)  : void{
    let input = event.key;
    let numberRegex =  /^\d*\.?\d*$/;
    let isNumber  = numberRegex.test(input);
    if(!isNumber  || event.target.value >= 999999){
      event.preventDefault()
    }
  }
  close(): void {
    this.modal.hide();
    this.modalSave.emit(null);
  }
  getAllCountries() {
    this._tenantDashboardServiceProxy
      .countryGetAll(
        this.appSession.tenantId
      )
      .subscribe((response) => {
        this.countries = response;
        this.modal.show();
      });
  }
  getEstimateCost(event) {
    if (event.target.value != null) {
      this.totalAmountInSAR = this.totalAmount * 3.75;
      this._tenantDashboardServiceProxy
        .getConvarsationPrice(
          this.selectedCountry,
          this.totalAmount,
          undefined, undefined, undefined,
          this.appSession.tenantId
        )
        .subscribe((response: ConversationPriceModel) => {
          this.conversationPrice = response;
        });
    }
  }

  proceedEstimateCost() {
    this.saving = true;
    this.invoiceWalletModel.country = this.selectedCountry;
    this.invoiceWalletModel.totalAmount = this.totalAmount;
    this.invoiceWalletModel.tenantId = this.appSession.tenantId;
    this.invoiceWalletModel.userId = this.appSession.userId;
    this.invoiceWalletModel.depositDate = moment();
    this.invoiceWalletModel.walletId = this.dasboardService.walletModel.walletId;
    this._tenantDashboardServiceProxy
      .walletDeposit(
        this.invoiceWalletModel
      )
      .subscribe((response) => {
        if (response) {
          this.url = response;
          this.saving = false;
          window.open(this.url, '_blank');
        } else {
          this.saving = false;
        }

      }, (error: any) => {
        if (error) {
          this.saving = false;
        }
      });
  }
}
