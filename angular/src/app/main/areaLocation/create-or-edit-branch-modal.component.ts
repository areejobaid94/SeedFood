import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {  CreateOrEditBranchDto, BranchesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';


@Component({
    selector: 'createOrEditBranchModal',
    templateUrl: './create-or-edit-branch-modal.component.html'
})
export class CreateOrEditBranchtModalComponent extends AppComponentBase {
   
     @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
   
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    branch: CreateOrEditBranchDto = new CreateOrEditBranchDto();
    options: string[] = ["Umm Uthaina", "Umm Al Summaq", "Al Jubeiha"];
    citys: string[] = ["عجلون", "عمان", "العقبة" ,"البلقاء", "اربد", "جرش", "الكرك", "معان", "مادبا", "المفرق", "الطفيله", "الزرقاء"];
    selectedOption = "Umm Uthaina";
    selectedCity = "عمان";

    constructor(
        injector: Injector,
        private _branchsServiceProxy: BranchesServiceProxy
    ) {
        super(injector);
    }
    
    show(branchId?: number): void {
    

        if (!branchId) {
            this.branch = new CreateOrEditBranchDto();
            this.branch.id = branchId;

            this.active = true;
            this.modal.show();
        } else {
            this._branchsServiceProxy.getBranchForEdit(branchId).subscribe(result => {
                this.branch = result.branch;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;
            this._branchsServiceProxy.createOrEdit(this.branch)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('savedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
