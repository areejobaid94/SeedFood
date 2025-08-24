import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import {
    DeliveryOrderDetailsDto,
    DeliveryOrderDetailsServiceProxy,
    GetOrderDetailForViewDto,
    GetOrderForViewDto,
    OrderDto,
    OrdersServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { Paginator } from "primeng/paginator";
import { AppSessionService } from "@shared/common/session/app-session.service";

@Component({
    selector: "viewDeliveryOrderModal",
    templateUrl: "./view-order-modal.component.html",
})
export class ViewDeliveryOrderModalComponent extends AppComponentBase {
    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("paginator", { static: true }) paginator: Paginator;
    active = false;
    saving = false;
    //userId:any;
    //totalAll=0;

    item: GetOrderForViewDto;
    // item2: GetOrderForViewDto;
    orderModel: GetOrderForViewDto;
    orderDetailList: GetOrderDetailForViewDto[];
    deliveryDetail: DeliveryOrderDetailsDto;

    Name: string;
    Mobile: string;
    Address: string;
    OrderNumber: Number;
    Time: any;
    Status: string;
    Type: string;
    fromGoogleURL: string;
    isMall: boolean;
    afterDeliveryCost: string;
    isafterDeliveryCost: boolean;
    phoneNumber: string;
    nameTenant: string;
    imageSrc: string;
    sunmiInnerPrinter: any;
    sound: any;
    printHtnl: any;

    constructor(
        injector: Injector,
        private _deliveryOrderDetailsServiceProxy: DeliveryOrderDetailsServiceProxy,

        private _appSessionService: AppSessionService,
        private _ordersServiceProxy: OrdersServiceProxy
    ) {
        super(injector);
        this.item = new GetOrderForViewDto();
        //this.item.order = new OrderDto();
    }

    show(item: GetOrderForViewDto): void {
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
                this.item = item;

                this._deliveryOrderDetailsServiceProxy
                    .getDeliveryOrderDetails(item.order.id)
                    .subscribe((result) => {
                        this.deliveryDetail = result;

                        this.active = true;
                        this.modal.show();
                    });
            }
        } else {
            this.item = item;

            this._deliveryOrderDetailsServiceProxy
                .getDeliveryOrderDetails(item.order.id)
                .subscribe((result) => {
                    this.deliveryDetail = result;
                    // this.deliveryDetail.fromGoogleURL=result.fromGoogleURL;
                    this.active = true;
                    this.modal.show();
                });
        }
    }

    reloadPage(): void {
        // this.totalAll=0;

        this.paginator.changePage(this.paginator.getPage());
    }

    closee(stringTotla: string): void {
        //this.totalAll=0;
        this._ordersServiceProxy
            .closeOrder(stringTotla, this.item.order)
            .subscribe((result) => {
                this.active = false;
                this.notify.success(this.l("succcessfullyClose"));
                this.modal.hide();
            });
    }
    delete(stringTotla: string): void {
        //this.totalAll=0;
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
                        this.active = false;
                        this.notify.success(this.l("successfullyDeleted"));
                        this.modal.hide();
                    });
            }
        });
    }
    copyInputMessage(inputElement) {
        inputElement.select();
        document.execCommand("copy");
        inputElement.setSelectionRange(0, 0);

        this.notify.success(this.l("succcessfullyCopied"));
    }
    done(stringTotla: string): void {
        //  this.totalAll=0;
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
                        this.active = false;
                        this.notify.success(this.l("succcessfullyDone"));
                        this.modal.hide();
                    });
            }
        });
    }
    chat(): void {
        // this.totalAll=0;
        this.active = false;
        this.modal.hide();
    }
}
