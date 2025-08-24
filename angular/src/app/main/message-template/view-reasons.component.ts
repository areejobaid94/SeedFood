import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'viewResons',
  templateUrl: './view-reasons.component.html',
  styleUrls: ['./view-reasons.component.css']
})
export class viewResons extends AppComponentBase {
  @ViewChild("viewResons", { static: true }) modal: ModalDirective;

  constructor(
    injector: Injector,
  ) {
    super(injector);
   }

  ngOnInit(): void {
  }

  show(){
    this.modal.show();
 
  }

  close(){
    this.modal.hide();
  }

}
