import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-CatalogSetupTemplate',
  templateUrl: './CatalogSetupTemplate.component.html',
  styleUrls: ['./CatalogSetupTemplate.component.css']
})
export class CatalogSetupTemplateComponent implements OnInit {

  @Input() template: any ;

  constructor() { }

  ngOnInit() {
  }

}
