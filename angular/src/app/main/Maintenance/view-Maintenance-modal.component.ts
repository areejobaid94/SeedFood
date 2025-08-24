import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetMaintenancesForViewDto, GetOrderDetailForViewDto, GetOrderForViewDto, MaintenancesServiceProxy, OrderDto, OrdersServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Paginator } from 'primeng/paginator';
@Component({
    selector: 'viewMaintenanceModal',
    templateUrl: './view-Maintenance-modal.component.html',
    styleUrls: ['./view-Maintenance-modal.component.less']
})
export class ViewMaintenanceModalComponent extends AppComponentBase {

  //  @ViewChild('SideBarMenuComponent', { static: true }) sideBarMenuComponent: SideBarMenuComponent;  

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    active = false;
    saving = false;
    userId:any;
    totalAll=0;

    isMall:boolean;
    item: GetMaintenancesForViewDto;
    orderModel: GetOrderForViewDto
    afterDeliveryCost:string;
    isafterDeliveryCost:boolean
    Name:string;
    Mobile:string;
    Address:string;
    OrderNumber:Number;
    Time:any;
    Status:string;
    Type:string;
    phoneNumber:string;
    nameTenant:string;
    imageSrc:string;
    isAdmin=false;
     sunmiInnerPrinter: any;
     sound:any;
     printHtnl:any;
    constructor(
        injector: Injector,
        private _maintenancesServiceProxy: MaintenancesServiceProxy,
        //private _ordersServiceProxy: OrdersServiceProxy,
       
    ) {
        super(injector);
      
        this.item = new GetMaintenancesForViewDto();
     

    }

   
   

    show(item: GetMaintenancesForViewDto): void {

        this.item=item;
        this.active = true;
        this.modal.show();
      
    }


    reloadPage(): void {
        this.totalAll=0;
        
       
        this.paginator.changePage(this.paginator.getPage());
    }
    
    copyInputMessage(inputElement){
                         
        inputElement.select();
        document.execCommand('copy');
        inputElement.setSelectionRange(0, 0);

        this.notify.success(this.l('successfullyCopied'));
      }
    closee(stringTotla:string): void {
        this.totalAll=0;
        this._maintenancesServiceProxy.closeOrder(stringTotla,this.item).subscribe(result => {  
                             
                            
            this.active = false;                        
            this.notify.success(this.l('successfullyClose'));
            this.modal.hide();
          
        });
    }
    delete(stringTotla:string): void {
        this.totalAll=0;
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {

                        this._maintenancesServiceProxy.deleteOrder(this.item.id,stringTotla,this.appSession.user.id,this.appSession.user.userName).subscribe(result => {  
                             
                            this.active = false;                        
                            this.notify.success(this.l('successfullyDeleted'));
                            this.modal.hide();
                          
                        });
                }
            }
        );


    
    }
    done(stringTotla:string): void {

        this.totalAll=0;
        this._maintenancesServiceProxy.doneOrder(stringTotla,this.appSession.user.id,this.appSession.user.userName,this.item).subscribe(result => {   
                        
            this.active = false;                          
            this.notify.success(this.l('successfullyDone'));
            this.modal.hide();
          
        });

       

    }
    chat(): void {
        this.totalAll=0;
        this.active = false;
        this.modal.hide();
    }
    printPage() {


        const printContent = document.getElementById("componentID");
        const WindowPrt = window.open('', '', '');
                         
        WindowPrt.document.write(printContent.innerHTML);
        window.focus();
        WindowPrt.print();
        WindowPrt.close()


      }

    

}
