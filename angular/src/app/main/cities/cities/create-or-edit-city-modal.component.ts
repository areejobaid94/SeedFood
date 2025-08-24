import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CitiesServiceProxy, CreateOrEditCityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'createOrEditCityModal',
    templateUrl: './create-or-edit-city-modal.component.html'
})
export class CreateOrEditCityModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    city: CreateOrEditCityDto = new CreateOrEditCityDto();



    constructor(
        injector: Injector,
        private _citiesServiceProxy: CitiesServiceProxy
    ) {
        super(injector);
    }
    
    show(cityId?: number): void {
    

        if (!cityId) {
            this.city = new CreateOrEditCityDto();
            this.city.id = cityId;

            this.active = true;
            this.modal.show();
        } else {
            this._citiesServiceProxy.getCityForEdit(cityId).subscribe(result => {
                this.city = result.city;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._citiesServiceProxy.createOrEdit(this.city)
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
