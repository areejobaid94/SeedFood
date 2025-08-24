import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs/operators";
import { LocationService } from "./location.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { LocationListModel } from "./location-list.model";
import {
    AreasServiceProxy,
    DeliveryLocationInfoModel,
    LocationsServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ThemeHelper } from "../..../../../shared/layout/themes/ThemeHelper";

@Component({
    selector: "createOrEditDeliveryLocationModal",
    templateUrl: "./create-or-edit-location.html",
    styleUrls: ["./create-or-edit-location.less"],
})
export class CreateOrEditDeliveryLocationModelComponent extends AppComponentBase {
    theme: string;

    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    tenantLocations: LocationListModel[];

    fromcities: LocationListModel[];
    selectFromCities: LocationListModel;
    fromareas: LocationListModel[];
    selectFromAreas: LocationListModel;
    fromdistricts: LocationListModel[];
    selectFromDistricts: LocationListModel[];

    tocities: LocationListModel[];
    selectToCities: LocationListModel;
    toareas: LocationListModel[];
    selectToAreas: LocationListModel;
    todistricts: LocationListModel[];
    selectToDistricts: LocationListModel[];

    fromlocation: DeliveryLocationInfoModel = new DeliveryLocationInfoModel();
    tolocation: DeliveryLocationInfoModel = new DeliveryLocationInfoModel();

    constructor(
        injector: Injector,
        private locationsServiceProxy: LocationsServiceProxy,
        private _locationServiceProxy: LocationService,
        private _areasServiceProxy: AreasServiceProxy
    ) {
        super(injector);
        this.getCities();
    }
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
    }

    showCreate(locationModel: DeliveryLocationInfoModel): void {
        this.fromlocation = locationModel;
        this.tolocation = locationModel;

        this.fromareas = [];
        this.fromdistricts = [];
        this.toareas = [];
        this.todistricts = [];
        this.active = true;
        this.modal.show();
    }
    showEdit(locationModel: DeliveryLocationInfoModel): void {
        this.fromlocation = locationModel;
        this.tolocation = locationModel;

        this._locationServiceProxy
            .GetLocationsByParentId(this.fromlocation.fromCityId)
            .subscribe((fromresult: any) => {
                this._locationServiceProxy
                    .GetLocationsByParentId(this.tolocation.toCityId)
                    .subscribe((toresult: any) => {
                        this.fromareas = fromresult.result;
                        this.toareas = toresult.result;

                        this._locationServiceProxy
                            .GetLocationsByParentId(
                                this.fromlocation.fromAreaId
                            )
                            .subscribe((fromresult: any) => {
                                this._locationServiceProxy
                                    .GetLocationsByParentId(
                                        this.tolocation.toAreaId
                                    )
                                    .subscribe((toresult: any) => {
                                        this.fromdistricts = fromresult.result;
                                        this.todistricts = toresult.result;
                                        this.active = true;
                                        this.modal.show();
                                    });
                            });
                    });
            });
    }
    save(): void {
        this.saving = true;
        this.fromlocation.tenantId = this.appSession.tenantId;
        this.locationsServiceProxy
            .addDeliveryLocationCost(this.fromlocation)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
            .subscribe(() => {
                this.notify.info(this.l("savedSuccessfully"));
                this.close();
                this.modalSave.emit(null);
            });
    }
    getCities() {
        this._locationServiceProxy
            .GetRootLocations()
            .subscribe((result: any) => {
                this.fromcities = result.result;
                this.tocities = result.result;
            });
    }

    onChangeFromCity(event): void {
        const locationId = event.target.value;

        this._locationServiceProxy
            .GetLocationsByParentId(this.fromlocation.fromCityId)
            .subscribe((result: any) => {
                this.fromareas = result.result;
            });
    }

    onChangeFromArea(event): void {
        const locationId = event.target.value;
        this._locationServiceProxy
            .GetLocationsByParentId(this.fromlocation.fromAreaId)
            .subscribe((result: any) => {
                this.fromdistricts = result.result;
            });
        //this._locationService.GetLocationsByParentId(locationId).subscribe((result: any) => {
        //this.areas = result.result;
        // });
    }

    onChangeToCity(event): void {
        const locationId = event.target.value;

        this._locationServiceProxy
            .GetLocationsByParentId(this.tolocation.toCityId)
            .subscribe((result: any) => {
                this.toareas = result.result;
            });
    }

    onChangeToArea(event): void {
        const locationId = event.target.value;
        this._locationServiceProxy
            .GetLocationsByParentId(this.tolocation.toAreaId)
            .subscribe((result: any) => {
                this.todistricts = result.result;
            });
    }
    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
