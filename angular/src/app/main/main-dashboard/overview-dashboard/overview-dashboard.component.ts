import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as rtlDetect from 'rtl-detect';
import { MainDashboardServiceService } from '../main-dashboard-service.service';
import { ContactsServiceProxy, TransactionModel } from '@shared/service-proxies/service-proxies';
import { AddFundsComponent } from '../add-funds/add-funds.component';
import { PermissionCheckerService } from 'abp-ng2-module';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-overview-dashboard',
  templateUrl: './overview-dashboard.component.html',
  styleUrls: ['./overview-dashboard.component.css']
})
export class OverviewDashboardComponent extends AppComponentBase implements OnInit {


  contacts$ : Observable<any>;

  isHasBillingPermission = false;
  isArabic = false;

  constructor(
    injector: Injector,
   public dasboardService: MainDashboardServiceService,
   private _permissionCheckerService: PermissionCheckerService,
   private _contactsServiceProxy: ContactsServiceProxy,
  ) {
      super(injector);

   }

  ngOnInit(): void {
    this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
    this.isHasBillingPermission = this._permissionCheckerService.isGranted("Pages.Billings");
    // this.getContacts();
  }

  // getContacts(){
  //    this.contacts$ =  this._contactsServiceProxy.getContact(0,5,"","");

  // }

}
