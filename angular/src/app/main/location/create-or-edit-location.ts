import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    AreaDto,
    AreasServiceProxy,
    LocationModel,
    LocationServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";

@Component({
    selector: "createOrEditLocationModal",
    templateUrl: "./create-or-edit-location.html",
    styleUrls: ["./create-or-edit-location.less"],
})
export class CreateOrEditLocationModelComponent extends AppComponentBase {

    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    
    theme: string;
    submitted = false;
    active = false;
    saving = false;

    location: LocationModel = new LocationModel();

    country: LocationModel[];
    cities: LocationModel[];
    areas: LocationModel[];
    districts: LocationModel[];
    branchs: AreaDto[];
    currency = "";
    isCreate= false;

    constructor(
        injector: Injector,
        private _areasServiceProxy: AreasServiceProxy,
        public darkModeService: DarkModeService,
        private _locationServiceProxy : LocationServiceProxy

    ) {
        super(injector);
    }
    
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.getAreas();
        this.getCities();
        this.getCountrys();
        this.getBranch();
    }

    showCreate(): void {
        this.currency = this.appSession.tenant.currencyCode;
        this.location = new LocationModel();
        this.areas = [];
        this.districts = [];
        this.location.branchAreaId = null;
        this.isCreate = true;
        this.active = true;
        this.modal.show();
    }

    showEdit(locationModel: LocationModel): void {
        this.isCreate = false;
        this.currency = this.appSession.tenant.currencyCode;
        this.location = locationModel;
        this._locationServiceProxy
            .getLocationsByParentId(this.location.cityId)
            .subscribe((result: any) => {
                this.areas = result;
                this._locationServiceProxy
                    .getLocationsByParentId(this.location.areaId)
                    .subscribe((result: any) => {
                        this.districts = result;
                        this.active = true;
                    });
            });
        this.modal.show();
    }

    save(): void {
        this.saving = true;
        if (         
            this.location.countryId === null ||
            this.location.countryId === undefined ||
            this.location.cityId === null ||
            this.location.cityId === undefined ||
            this.location.areaId === null ||
            this.location.areaId === undefined ||
            this.location.locationId === null ||
            this.location.locationId === undefined ||
            this.location.deliveryCost === null ||
            this.location.deliveryCost === undefined ||
            this.location.branchAreaId === null ||
            this.location.branchAreaId === undefined
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }

        this._locationServiceProxy
            .createOrUpdateLocation(this.location).pipe(finalize(() => {
                    this.saving = false;
                })
                ).subscribe(() => {
                this.notify.info(this.l("savedSuccessfully"));
                    this.submitted = false;
                    this.saving = false;
                    this.modal.hide();
                    this.modalSave.emit(null);
            });
    }
    getBranch(){
        this._areasServiceProxy.getAllAreas(this.appSession.tenantId,null).subscribe(result => {
            this.branchs=result;
        });
    }
    getCountrys() {
        this._locationServiceProxy
            .getCountryLocation()
            .subscribe((result: any) => {
                this.country = result;
            });
    }
    getCities() {
        this._locationServiceProxy
            .getRootLocations()
            .subscribe((result: any) => {
                this.cities = result;
            });
    }

    getAreas() {
        this._areasServiceProxy
            .getAllAreas(this.appSession.tenantId, null)
            .subscribe((areas) => {
                this.branchs = areas;
            });
    }
    onChangeCountry(event): void {
        this._locationServiceProxy
            .getLocationsByParentId(this.location.countryId)
            .subscribe((result: any) => {
                this.cities = result;
                this.areas = null;
                this.districts = null;
                this.location.areaId = null;
                this.location.cityId=event.target.value;

            });
    }
    onChangeCity(event): void {
        this._locationServiceProxy
            .getLocationsByParentId(this.location.cityId)
            .subscribe((result: any) => {
                this.areas = result;
                this.districts = null;
                this.location.areaId = null;
                this.location.cityId=event.target.value;

            });
    }

    onChangeArea(event): void {
        const locationId = event.target.value;
        this._locationServiceProxy
            .getLocationsByParentId(this.location.areaId)
            .subscribe((result: any) => {
                this.districts = result;
            });
            this.location.areaId=event.target.value;
    }
    
    close(): void {
        this.active = false;
        this.modal.hide();
        this.submitted = false;
        this.saving = false;
        this.modalSave.emit(null);
    }
}
