import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { MainDashboardServiceService } from '../main-dashboard-service.service';
import { LastFourTransactionModel, TenantDashboardServiceProxy, TransactionModel } from '@shared/service-proxies/service-proxies';
import { PermissionCheckerService } from 'abp-ng2-module';

@Component({
  selector: 'transactionsModal',
  templateUrl: './wallet-transactions.component.html',
  styleUrls: ['./wallet-transactions.component.css']
})
export class WalletTransactionsComponent extends AppComponentBase  {
  @ViewChild('transactionsModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  transactions: LastFourTransactionModel[] = [];
  isHasBillingPermission = false;

  constructor(
    injector: Injector,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy,
    private _permissionCheckerService: PermissionCheckerService,

  ) {        
    super(injector);
  }

  ngOnInit(): void {
    this.isHasBillingPermission = this._permissionCheckerService.isGranted("Pages.Billings");
  }
  show(): void {
    this.getLastTransactions();
  }
  close(): void {
    this.modal.hide();
    this.modalSave.emit(null);
}

getLastTransactions() {
  this._tenantDashboardServiceProxy
    .transactionGetLastFour(
      this.appSession.tenantId
    )
    .subscribe((response: LastFourTransactionModel[]) => {
      this.transactions = response;
      this.modal.show();
    });
}

checkIfNgeative(value:string) {
  if (value.includes('-')) {
    return true;
  } else {
    return false;
  }
}
}
