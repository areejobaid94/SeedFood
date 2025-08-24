import { Component, ViewChild, Injector, Output, EventEmitter, Injectable} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { OrdersServiceProxy, CreateOrEditOrderDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'createOrEditDeliveryOrderModal',
    templateUrl: './create-or-edit-order-modal.component.html'
})
export class CreateOrEditDeliveryOrderModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    order: CreateOrEditOrderDto = new CreateOrEditOrderDto();

    orderStatussName = '';
    customersName = '';
    deliveryChangeName = '';
    branchName = '';

					
    constructor(
        injector: Injector,
        private _ordersServiceProxy: OrdersServiceProxy
    ) {
        super(injector);
    }
    
    show(orderId?: number): void {
    

        if (!orderId) {
            this.order = new CreateOrEditOrderDto();
            this.order.id = orderId;
            // this.order.effectiveTimeFrom = moment().startOf('day');
            // this.order.effectiveTimeTo = moment().startOf('day');
            // this.orderItemStatusName = '';
            // this.orderCategoryName = '';
            this.orderStatussName = '';
            this.customersName = '';
            this.deliveryChangeName = '';
            this.branchName = '';
            this.active = true;
            this.modal.show();
        } else {
            this._ordersServiceProxy.getOrderForEdit(orderId).subscribe(result => {
                this.order = result.order;

                // this.menuItemStatusName = result.menuItemStatusName;
                // this.menuCategoryName = result.menuCategoryName;
                this.orderStatussName = result.orderStatusName;
               // this.customersName = result.customerCustomerName;
                this.deliveryChangeName = result.deliveryChangeDeliveryServiceProvider;
                this.branchName = result.branchName;
                this.active = true;
                this.modal.show();
            });
        }
					
    }

    save(): void {
            this.saving = true;			
			
            this._ordersServiceProxy.createOrEdit(this.order)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
