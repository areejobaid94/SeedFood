import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CaptionDto, TenantSettingsServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { DarkModeService } from '@app/services/dark-mode.service';

@Component({
  selector: 'editCaption',
  templateUrl: './edit-caption.component.html',
  styleUrls: ['./edit-caption.component.css']
})
export class EditCaptionComponent extends AppComponentBase {
  @ViewChild('editCaption', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  caption = new CaptionDto();
  constructor(
    injector: Injector,
    private _tenantSettingsService: TenantSettingsServiceProxy,
    public darkModeService: DarkModeService,

  ) {
    super(injector);
   }

  ngOnInit(): void {
  }
  show(caption){
    this._tenantSettingsService.getCaptionById(caption.id).subscribe(result => {
      this.caption = result;
      this.modal.show();
    });
  }
  close(){
    this.modal.hide();
  }
  save(){
    this._tenantSettingsService.updateCaptionById(this.caption).subscribe(result => {
      if(result){
        this.notify.success(this.l('SavedSuccessfully'));
        this.modalSave.emit();
        this.close();
      }else{
        this.notify.error(this.l('Failed'));
      }
    
    });
  }

}
