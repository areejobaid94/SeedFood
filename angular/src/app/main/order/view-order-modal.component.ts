import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import {
    GetOrderDetailForViewDto,
    GetOrderForViewDto,
    LoyaltyServiceProxy,
    OrderDto,
    OrdersServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { Paginator } from "primeng/paginator";
import { AppSessionService } from "@shared/common/session/app-session.service";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import * as rtlDetect from "rtl-detect";
import { ViewLoyaltyOrdersComponent } from "./view-loyalty-orders.component";

@Component({
    selector: "viewOrderModal",
    templateUrl: "./view-order-modal.component.html",
    styleUrls: ["./view-order-modal.component.less"],
})
export class ViewOrderModalComponent extends AppComponentBase {
    theme: string;

    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    @ViewChild("viewOrderModalComponent", { static: true })
    viewOrderModal: ViewOrderModalComponent;
    @ViewChild("viewLoyaltyOrders", { static: true })
    viewLoyaltyOrders: ViewLoyaltyOrdersComponent;

    active = false;
    saving = false;
    userId: any;
    totalAll = 0;
    SupTotal: number;
    SubTotalString: string;
    isMall: boolean;
    item: GetOrderForViewDto;
    // item2: GetOrderForViewDto;
    orderModel: GetOrderForViewDto;
    orderDetailList: GetOrderDetailForViewDto[];
    afterDeliveryCost: string;
    isafterDeliveryCost: boolean;
    Name: string;
    Mobile: string;
    Address: string;
    OrderNumber: Number;
    Time: any;
    Status: string;
    Type: string;
    currency = "";
    phoneNumber: string;
    nameTenant: string;
    imageSrc: string;
    sunmiInnerPrinter: any;
    sound: any;
    printHtnl: any;
    isView = false;
    isArabic = false;
    isTenantLoyal: boolean;

    constructor(
        injector: Injector,
        private _appSessionService: AppSessionService,
        private _ordersServiceProxy: OrdersServiceProxy,
        public darkModeService: DarkModeService,
        private _loyaltyServiceProxy: LoyaltyServiceProxy
    ) {
        super(injector);

        this.item = new GetOrderForViewDto();
        this.item.order = new OrderDto();
    }
    async ngOnInit() {
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.theme = ThemeHelper.getTheme();
        this.checkIsLoyality();
        await this.getIsAdmin()

    }

    checkIsLoyality() {
        this._loyaltyServiceProxy.isLoyalTenant().subscribe((result) => {
            this.isTenantLoyal = result;
        });
    }

    show(item: GetOrderForViewDto, modetype: boolean): void {
        this.currency = this.appSession.tenant.currencyCode;
        this.isView = modetype;

        if (this.appSession.tenantId == 34) this.isMall = true;
        else this.isMall = false;

        this.phoneNumber = this._appSessionService.tenant.phoneNumber;
        this.nameTenant = this._appSessionService.tenant.name;
        this.imageSrc = this._appSessionService.tenant.image;

        if (item.order.isLockByAgent) {
            if (
                item.order.agentId === this.appSession.user.id ||
                this.isAdmin
            ) {
                this.totalAll = 0;
                var xx = "";
                if (item.deliveryCost != null) {
                    if (this.appSession.tenantId == 46) {
                        xx = item.deliveryCost.toString();
                    } else {
                        xx = (
                            Math.round(item.deliveryCost * 100) / 100
                        ).toFixed(2);
                    }
                } else {
                    xx = "0.00";
                }
                item.stringdeliveryCost = xx;

                this.item = item;
                // this.item2 = item;
                var xxx = "";
                if (item.deliveryCost != null) {
                    if (this.appSession.tenantId == 46) {
                        xxx = item.order.afterDeliveryCost.toString();
                    } else {
                        xxx = (
                            Math.round(item.order.afterDeliveryCost * 100) / 100
                        ).toFixed(2);
                    }
                } else {
                    xxx = "0.00";
                }

                this.afterDeliveryCost = xxx + this.currency;

                if (xxx == "0.00") {
                    this.isafterDeliveryCost = false;
                } else {
                    this.isafterDeliveryCost = true;
                }

                this._ordersServiceProxy
                    .getAllDetail(item.order.id)
                    .subscribe((result) => {
                        debugger;
                        this.userId = item.customerID;
                        result.forEach((element) => {
                            var x = "";
                            var y = "";
                            if (this.appSession.tenantId == 46) {
                                x = (
                                    element.orderDetail.quantity *
                                    element.orderDetail.unitPrice
                                ).toString();
                                y = element.orderDetail.unitPrice.toString();
                            } else {
                                x = (
                                    Math.round(
                                        element.orderDetail.quantity *
                                            element.orderDetail.unitPrice *
                                            100
                                    ) / 100
                                ).toFixed(2);
                                y = (
                                    Math.round(
                                        element.orderDetail.unitPrice * 100
                                    ) / 100
                                ).toFixed(2);
                            }

                            element.orderDetail.stringTotal = x;
                            element.orderDetail.stringUnitPrice = y;

                            element.orderDetail.lstCategoryExtraOrderDetailsDto.forEach(
                                (elemenCat) => {
                                    elemenCat.lstExtraOrderDetailsDto.forEach(
                                        (element2) => {
                                            var xx = "";
                                            var yy = "";
                                            if (
                                                this.appSession.tenantId == 46
                                            ) {
                                                xx = element2.total.toString();
                                                yy =
                                                    element2.unitPrice.toString();
                                            } else {
                                                xx = (
                                                    Math.round(
                                                        element2.total * 100
                                                    ) / 100
                                                ).toFixed(2);
                                                yy = (
                                                    Math.round(
                                                        element2.unitPrice * 100
                                                    ) / 100
                                                ).toFixed(2);
                                            }

                                            element2.stringTotal = xx;
                                            element2.stringUnitPrice = yy;
                                        }
                                    );
                                }
                            );

                            //  this.items.push();
                        });
                        this.orderDetailList = result;
                        this.SupTotal =
                            item.order.total - item.order.deliveryCost;
                        if (this.appSession.tenantId == 46) {
                            this.SubTotalString = this.SupTotal.toString();
                        } else {
                            this.SubTotalString = (
                                Math.round(this.SupTotal * 100) / 100
                            ).toFixed(2);
                        }

                        this.active = true;
                        this.modal.show();
                    });
            }
        } else {
            this.totalAll = 0;
            var xx = "";
            if (item.deliveryCost != null) {
                if (this.appSession.tenantId == 46) {
                    xx = item.deliveryCost.toString();
                } else {
                    xx = (Math.round(item.deliveryCost * 100) / 100).toFixed(2);
                }
            } else {
                xx = "0.00";
            }

            item.stringdeliveryCost = xx;
            this.item = item;
            //   this.item2 = item;

            var xxx = "";
            if (item.deliveryCost != null) {
                if (this.appSession.tenantId == 46) {
                    xxx = item.order.afterDeliveryCost.toString();
                } else {
                    xxx = (
                        Math.round(item.order.afterDeliveryCost * 100) / 100
                    ).toFixed(2);
                }
            } else {
                xxx = "0.00";
            }

            this.afterDeliveryCost = xxx + this.currency;

            if (xxx == "0.00") {
                this.isafterDeliveryCost = false;
            } else {
                this.isafterDeliveryCost = true;
            }

            this._ordersServiceProxy
                .getAllDetail(item.order.id)
                .subscribe((result) => {
                    this.userId = item.customerID;
                    result.forEach((element) => {
                        var x = "";
                        var y = "";
                        if (this.appSession.tenantId == 46) {
                            x = (
                                element.orderDetail.quantity *
                                element.orderDetail.unitPrice
                            ).toString();
                            y = element.orderDetail.unitPrice.toString();
                        } else {
                            x = (
                                Math.round(
                                    element.orderDetail.quantity *
                                        element.orderDetail.unitPrice *
                                        100
                                ) / 100
                            ).toFixed(2);
                            y = (
                                Math.round(
                                    element.orderDetail.unitPrice * 100
                                ) / 100
                            ).toFixed(2);
                        }

                        element.orderDetail.stringTotal = x;
                        element.orderDetail.stringUnitPrice = y;

                        element.orderDetail.lstCategoryExtraOrderDetailsDto.forEach(
                            (elemenCat) => {
                                elemenCat.lstExtraOrderDetailsDto.forEach(
                                    (element2) => {
                                        var xx = "";
                                        var yy = "";
                                        if (this.appSession.tenantId == 46) {
                                            xx = element2.total.toString();
                                            yy = element2.unitPrice.toString();
                                        } else {
                                            xx = (
                                                Math.round(
                                                    element2.total * 100
                                                ) / 100
                                            ).toFixed(2);
                                            yy = (
                                                Math.round(
                                                    element2.unitPrice * 100
                                                ) / 100
                                            ).toFixed(2);
                                        }
                                        element2.stringTotal = xx;
                                        element2.stringUnitPrice = yy;
                                        //element2.stringTotalLoyaltyPoints = xx;
                                    }
                                );
                            }
                        );

                        //  this.items.push();
                    });
                    this.orderDetailList = result;

                    this.orderDetailList = result;
                    this.SupTotal = item.order.total - item.order.deliveryCost;

                    if (this.appSession.tenantId == 46) {
                        this.SubTotalString = this.SupTotal.toString();
                    } else {
                        this.SubTotalString = (
                            Math.round(this.SupTotal * 100) / 100
                        ).toFixed(2);
                    }

                    this.active = true;
                    this.modal.show();
                });
        }
    }

    copyInputMessage(inputElement) {
        inputElement.select();
        document.execCommand("copy");
        inputElement.setSelectionRange(0, 0);

        this.notify.success(this.l("successfullyCopied"));
    }
    closee(stringTotla: string): void {
        this.totalAll = 0;

        this.active = false;
        // this.notify.success(this.l('Successfully close'));
        this.modal.hide();
    }
    delete(stringTotla: string): void {
        this.totalAll = 0;
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._ordersServiceProxy
                    .deleteOrder(
                        this.item.order.id,
                        stringTotla,
                        this.appSession.user.id,
                        this.appSession.user.userName
                    )
                    .subscribe((result) => {
                        if (result) {
                            this.active = false;
                            this.notify.success(this.l("SuccessfullyDeleted"));
                            this.modal.hide();
                        } else {
                            this.notify.error(this.l("orderDeleteRejection"));
                            this.modal.hide();
                        }
                    });
            }
        });
    }
    done(stringTotla: string): void {
        this.totalAll = 0;
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._ordersServiceProxy
                    .doneOrder(
                        stringTotla,
                        this.appSession.user.id,
                        this.appSession.user.userName,
                        this.item.order
                    )
                    .subscribe((result) => {
                        if (result) {
                            this.active = false;
                            this.notify.success(this.l("successfullyDone"));
                            this.modal.hide();
                        } else {
                            this.notify.error(this.l("orderDoneRejection"));
                            this.modal.hide();
                        }
                    });
            }
        });
    }

    printPage() {
        const printContents = document.getElementById("componentID").innerHTML;
        document.body.innerHTML = printContents;
        window.print();
        location.reload();
    }
    close(): void {
        this.active = false;
        this.modal.hide();
    }

        getTotalWithTax(total: string, tax: number): number {
          const numericTotal = parseFloat(total);
          if (isNaN(numericTotal)) {
            return 0;
          }
          const totalWithTax = numericTotal + numericTotal * tax;
          return +totalWithTax.toFixed(2); 
        }

selectedExtraOrderDetailsDtos: any[] = [];



calculateTotalSum(): number {
    let sum = 0;
    if (this.orderDetailList && this.orderDetailList.length > 0) {
        this.orderDetailList.forEach(record => {
            if (record.orderDetail && record.orderDetail.total) {
                sum += record.orderDetail.total;
            }
            
            if (record.orderDetail.extraOrderDetailsDtos) {
                record.orderDetail.extraOrderDetailsDtos.forEach(extra => {
                    if (extra.total) {
                        sum += extra.total;
                    }
                });
            }
            
            if (record.orderDetail.lstCategoryExtraOrderDetailsDto) {
                record.orderDetail.lstCategoryExtraOrderDetailsDto.forEach(cat => {
                    if (cat.lstExtraOrderDetailsDto) {
                        cat.lstExtraOrderDetailsDto.forEach(extra => {
                            if (extra.total) {
                                sum += extra.total;
                            }
                        });
                    }
                });
            }
        });
    }
    return sum;
}

getTotalSumString(): string {
    return this.calculateTotalSum().toFixed(2); // Adjust decimal places as needed
}
}
