import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantServicesServiceProxy, TenantServiceModalDto } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FeatureTreeComponent } from '../shared/feature-tree.component';
import { finalize } from 'rxjs/operators';
import { FormGroup, UntypedFormBuilder, UntypedFormArray, UntypedFormControl } from '@angular/forms';
import { Paginator } from 'primeng/paginator';
@Component({
    selector: 'tenantServicesModal',
    templateUrl: './tenant-services-modal.component.html'
})
export class TenantServicesModalComponent extends AppComponentBase {
    @ViewChild('paginator', {static: true}) paginator: Paginator;
    @ViewChild('tenantServicesModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
      test:number;
    tenantId: number;
    tenantName: string;
    services: TenantServiceModalDto[];
    editTenantServicesForm;
    activeCheckboxFormArray;

    constructor(
        injector: Injector,
        private _tenantServiceService: TenantServicesServiceProxy,
        private fb: UntypedFormBuilder
    ) {
        super(injector);
    }

    show(tenantId: number, tenantName: string): void {

        this.test=52020;

        this.editTenantServicesForm = this.fb.group({
            activeCheckbox: this.fb.array([])
        });
        this.tenantId = tenantId;
        this.tenantName = tenantName;
        this.active = true;      
        this.modal.show();
        this.getTenatServices(this.tenantId);
      
    }

    // trackByIdx(index: number, obj: any): any {
    //         
    //     return index;
    //   }
    onFeesChange(data: TenantServiceModalDto[]) {
      
    }

    save(): void {
        this.services;
        this.saving = true;
        

        this._tenantServiceService.updateTenantService(this.services)
            .pipe(finalize(() => this.saving = false))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
               
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }
    getTenatServices(tenantId): void {
        this.activeCheckboxFormArray = <UntypedFormArray>this.editTenantServicesForm.controls.activeCheckbox;
        
        this._tenantServiceService.getTenatServices(tenantId).subscribe((result) => {
                
            this.services = result;

            
            this.services.forEach(element => {
               
                if(element.isSelected){
                    this.activeCheckboxFormArray.push(new UntypedFormControl(element));
                }else{
                    let index = this.activeCheckboxFormArray.controls.findIndex(x => x.value == element)
                    if(index>=0)
                    this.activeCheckboxFormArray.removeAt(index);
                }
                
            });

        });
    }
}
