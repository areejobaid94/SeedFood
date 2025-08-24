  import { AfterViewInit, Component, Injector, Input, OnInit } from "@angular/core";
import { MainDashboardServiceService } from "@app/main/main-dashboard/main-dashboard-service.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { PermissionCheckerService } from "abp-ng2-module";
import * as rtlDetect from 'rtl-detect';

@Component({
    selector: "app-payment-detial",
    templateUrl: "./payment-detial.component.html",
    styleUrls: ["./payment-detial.component.css"],
})
export class PaymentDetialComponent extends AppComponentBase  implements OnInit ,AfterViewInit {

    isHasBillingPermission = false;
    isArabic = false;
    data: any = {};
    color: string = "red";
    @Input()
    svgType: string = "dollar";

    @Input()
    objData: any;

    title!: string;

    constructor(
        injector: Injector,
       public dasboardService: MainDashboardServiceService,
       private _permissionCheckerService: PermissionCheckerService,
      ) {
          super(injector);
    
       }
    ngAfterViewInit(): void {
    }

    ngOnInit() {
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        this.isHasBillingPermission = this._permissionCheckerService.isGranted("Pages.Billings");
        this.data =
            this.objData.typeofChat == "Markting"
                ? { title: "Paid Marketing", color: "#FE716A" }
                : this.objData.typeofChat == "Services"
                ? { title: "Paid Services", color: "#0D8939" }
                : this.objData.typeofChat == "Utility"
                ? { title: "Paid Utility", color: "#FCA004" }
                : { title: "Free Chat Paid", color: "#009789" };

    }
    

}
