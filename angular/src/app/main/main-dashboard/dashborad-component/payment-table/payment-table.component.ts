import { Component, Injector, Input, OnInit } from '@angular/core';
import { MainDashboardServiceService } from '../../main-dashboard-service.service';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-payment-table',
  templateUrl: './payment-table.component.html',
  styleUrls: ['./payment-table.component.css']
})
export class PaymentTableComponent extends AppComponentBase implements OnInit {
 @Input() 
 dahsboardService : MainDashboardServiceService;

 @Input()
 paid : boolean;



  constructor(
    injector: Injector,
  ) {
    super(injector);

 }


  ngOnInit() {
    
  }

}
