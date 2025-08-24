import {
    AfterViewInit,
    Component,
    Injector,
    Input,
    OnInit,
} from "@angular/core";
import { MainDashboardServiceService } from "../../main-dashboard-service.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { PermissionCheckerService } from "abp-ng2-module";

@Component({
    selector: "app-payment-detials",
    templateUrl: "./payment-detials.component.html",
    styleUrls: ["./payment-detials.component.css"],
})
export class PaymentDetialsComponent
    extends AppComponentBase
    implements OnInit, AfterViewInit
{
    
    @Input()
    tableData: any[];

    @Input()
    tableColumns: any[];
    @Input()
    tableDataPaid: any[] = [];
    @Input()
    tableDataFree: any[] = [];

    svgDollarType: string = "dollar";
    svgBoltType: string = "bolt";
    paidMarkting!: any;
    paidServices!: any;
    paidUtility!: any;

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
        this.fillData();
    }
    fillData() {
        console.log("ih", this.dasboardService);

        this.tableDataFree = [
            {
                id: 1,
                typeofChat: "Instagram, WhatsApp. ",
                totalChat: 1700,
                fees: "Unlimited Entry",
            },
            {
                id: 2,
                typeofChat: "Social media",
                totalChat: 988 / 1000,
                fees: "limited Entry",
            },
        ];
        this.tableDataPaid = [
            {
                id: 1,
                typeofChat: "Markting",
                totalChat:
                    this.dasboardService.bundleData.totalMarketingConversation,
                fees: this.dasboardService.bundleData.totalMarketingCharge,
            },
            {
                id: 2,
                typeofChat: "Services",
                totalChat:
                    this.dasboardService.bundleData.totalServicesConversation,
                fees: this.dasboardService.bundleData.totalServicesCharge,
            },
            {
                id: 3,
                typeofChat: "Utility",
                totalChat:
                    this.dasboardService.bundleData.totalUtilityConversation,
                fees: this.dasboardService.bundleData.totalUtilityCharge,
            },
        ];
    }
}
