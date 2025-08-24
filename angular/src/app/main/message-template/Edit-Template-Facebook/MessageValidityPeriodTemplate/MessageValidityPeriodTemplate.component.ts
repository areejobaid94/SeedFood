import { Component, Input, OnInit } from '@angular/core';
import { MessageTemplateModel } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-MessageValidityPeriodTemplate',
  templateUrl: './MessageValidityPeriodTemplate.component.html',
  styleUrls: ['./MessageValidityPeriodTemplate.component.css']
})
export class MessageValidityPeriodTemplateComponent implements OnInit {
  @Input() template: any = {};
  isCustomValidity:boolean=false;
  private firstToggle: boolean = true; 

  constructor() { }

  ngOnInit() {

    if(this.template.message_send_ttl_seconds!=null){
      this.isCustomValidity=true;
    }
    if(this.template.message_send_ttl_seconds){
      this.template.message_send_ttl_seconds=null;
    }

  }
  
  onToggleValidity(isChecked: boolean): void {
    this.isCustomValidity = isChecked;
    if (isChecked && this.firstToggle) {
      this.template.message_send_ttl_seconds = 600; 
      this.firstToggle = false; }

    }
  
}
