// ticket-limit-popup.component.ts
import { Component } from '@angular/core';
import { UserTicketsModel } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'appticketlimitpopup',
  templateUrl: './ticket-limit-popup.component.html',
  styleUrls: ['./ticket-limit-popup.component.css']
})
export class TicketLimitPopupComponent {
  showPopup = false;

  mo:UserTicketsModel;

  // Call this when ticket limit is exceeded
  show(model:UserTicketsModel) {
    debugger

    this.mo=model;
    this.showPopup = true;
  }

  closePopup() {
    debugger
    this.showPopup = false;
  }
}