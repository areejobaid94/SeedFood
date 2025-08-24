import { Component, Input, OnInit } from '@angular/core';
import { forEach } from '@node_modules/@types/lodash';
import { Button } from '@node_modules/primeng/button';
import { ButtonsTemplate, CallToActionButton,WhatsAppButtonModel, MessageTemplateModel, TypeOfAction, WhatsAppComponentModel } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-ButtonInCatalog',
  templateUrl: './ButtonInCatalog.component.html',
  styleUrls: ['./ButtonInCatalog.component.css']
})
export class ButtonInCatalogComponent implements OnInit {

  @Input() template: any = {};
  @Input() selectedMessageType:string='';
  @Input() selectedCategory:string='';
  @Input() selectedFormat: string | null = null;
  Buttons: WhatsAppComponentModel = new WhatsAppComponentModel();

  constructor() { }

  
  getapi(){
    this.Buttons = this.template.components.find(
      (component: WhatsAppComponentModel) => component.type === "BUTTONS"
    )||null;
    debugger;
    this.Buttons.buttons[0].text = "View catalog";

  if(this.Buttons.buttons[0].type=='CATALOG'){

  
      this.Buttons.buttons[0].text = "View catalog";
    // else{
    //   this.Buttons.buttons[0].text='View items';
    // }

    }
  }
  ngOnInit() {
 debugger
    this.Buttons = this.template.components.find(
      (component: WhatsAppComponentModel) => component.type === "BUTTONS"
    )||null;
    this.Buttons.buttons[0].text;

  if (this.Buttons.buttons[0] && this.Buttons.buttons[0].type === 'CATALOG') {
    if (this.Buttons.buttons[0].text && this.Buttons.buttons[0].text.trim() !== "") {
    } else {
        this.Buttons.buttons[0].text = "View catalog";
    }
  }
}

}
