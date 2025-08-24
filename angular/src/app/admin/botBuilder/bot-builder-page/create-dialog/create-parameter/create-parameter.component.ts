import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@shared/common/app-component-base';
import { BotFlowServiceProxy, BotParameterModel } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'createParameter',
  templateUrl: './create-parameter.component.html',
  styleUrls: ['./create-parameter.component.css']
})
export class CreateParameterComponent extends AppComponentBase  {
  @ViewChild("createParameter", { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  variableForm = new FormGroup({
    name: new FormControl('', Validators.required),
    format: new FormControl('', Validators.required),
    id: new FormControl(''),
    isDeleted: new FormControl(false),
    tenantId: new FormControl(this.appSession.tenant.id),
  });
  submitted = false;
  saving = false
  constructor(
    injector: Injector,
    private _BotFlowServiceProxy: BotFlowServiceProxy,

  ) { 
    super(injector);
  }

  ngOnInit(): void {
  }

  show(){
    this.modal.show();
    this.variableForm.reset();
  }
  close(){
    this.modal.hide();
  }
  save(){
    this.saving=true;
    if(this.variableForm.valid){
      let variable = new BotParameterModel();
      variable = this.variableForm.value as any;
      variable.isDeleted = false;
      variable.tenantId = this.appSession.tenantId;
      variable.id=0;
      this._BotFlowServiceProxy
      .botParameterCreate(variable)
      .subscribe(
          (res) => {
              if (res) {
                  this.notify.success(this.l("SavedSuccessfully"));
                  this.modalSave.emit(null);
                  this.close();
                  this.submitted = false;
                  this.saving=false;
              }else{
                this.notify.error(this.l("Failed"));
                this.close();
                this.submitted = false;      
                this.saving=false;
  
              }
          },
          (error: any) => {
              if (error) {
                this.submitted = false;
                this.saving = false;
              }
          }
      );
    }else{
      this.saving = false;
    }
  }

}
