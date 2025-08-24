import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as rtlDetect from 'rtl-detect';
import { TransactionModel } from '@shared/service-proxies/service-proxies';
import { PermissionCheckerService } from 'abp-ng2-module';
import { MainDashboardServiceService } from '../../main-dashboard-service.service';
import { AddFundsComponent } from '../../add-funds/add-funds.component';

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css']
})
export class CardComponent extends AppComponentBase implements OnInit {

  @Input()
    cardType : number;

    @ViewChild('transactionsModalNew', { static: true }) transactionsModal: TransactionModel;
    @ViewChild('addFundNew', { static: true }) addFunds: AddFundsComponent;

  isHasBillingPermission = false;
  isArabic = false;

  constructor(
    injector: Injector,
   public dasboardService: MainDashboardServiceService,
   private _permissionCheckerService: PermissionCheckerService,
  ) {
      super(injector);

   }

  ngOnInit(): void {
    this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
    this.isHasBillingPermission = this._permissionCheckerService.isGranted("Pages.Billings");
  }

}