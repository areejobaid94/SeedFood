import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CatalogFormat2, WhatsAppComponentModel } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-CatalogFormat',
  templateUrl: './CatalogFormat.component.html',
  styleUrls: ['./CatalogFormat.component.css']
})
export class CatalogFormatComponent implements OnInit {
  @Input() template: any = {};

  @Output() formatSelected = new EventEmitter<string>();
  Buttons: WhatsAppComponentModel = new WhatsAppComponentModel();

  CatalogFormat:string="CatalogMessage";

  onCatalogFormatChange(format: string): void {
    this.formatSelected.emit(format);
  }

  constructor() { }

  ngOnInit() {
      this.Buttons = this.template.components.find(
        (component: WhatsAppComponentModel) => component.type === "BUTTONS"
      )||null;
      this.Buttons.buttons[0].text;
  
    if (this.Buttons.buttons[0] && this.Buttons.buttons[0].type === 'CATALOG') {
      if (this.Buttons.buttons[0].text == "View catalog") {
        this.CatalogFormat="MultiProductMessage";
      } else {
        this.CatalogFormat="CatalogMessage";
      }
    }
    
  }

}
