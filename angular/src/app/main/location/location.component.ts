import {
  Component,
  Injector,
  ViewEncapsulation,
  ViewChild,
} from "@angular/core";
import { LocationService } from "./location.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { Table } from "primeng/table";
import { Paginator } from "primeng/paginator";
import { LazyLoadEvent } from "primeng/api";
import { CreateOrEditLocationModelComponent } from "./create-or-edit-location";
import {
  LocationInfoModel,
  LocationModel,
  LocationServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../services/dark-mode.service";
import * as rtlDetect from "rtl-detect";

@Component({
  selector: "app-location",
  templateUrl: "./location.component.html",
  styleUrls: ["./location.component.css"],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class LocationComponent extends AppComponentBase {
  theme: string;
  currency = "";
  @ViewChild("createOrEditLocationModal", { static: true })
  createOrEditLocationModal: CreateOrEditLocationModelComponent;

  @ViewChild("dataTable", { static: true }) dataTable: Table;
  @ViewChild("paginator", { static: true }) paginator: Paginator;


  country: LocationModel[];
  cities: LocationModel[];
  firstCitie: LocationModel;
  areas: LocationModel[];
  firstAreas: LocationModel;
  locationModel: LocationModel;
  advancedFiltersAreShown = false;


  countryId = null;
  cityId = null;
  areaId = null;
  isOK: boolean;
  isArabic = false;
  skipCount: any;
  maxResultCount: any;

  constructor(
      public darkModeService: DarkModeService,
      injector: Injector,
      private _locationService: LocationService,
      private _locationServiceProxy: LocationServiceProxy
  ) {
      super(injector);
  }

  ngOnInit(): void {
      this.theme = ThemeHelper.getTheme();
      this.isArabic = rtlDetect.isRtlLang(
          abp.localization.currentLanguage.name
      );
      this.isOK = true;
      this.cityId = null;
      this.areaId = null;
      this.getCountry();
      //this.getCities();
      //this.getSearch()
  }
  // getLocation(event?: LazyLoadEvent) {

  //   if (this.primengTableHelper.shouldResetPaging(event)) {
  //     this.paginator.changePage(0);
  //     return;
  //   }

  //   this.primengTableHelper.showLoadingIndicator();

  //   this._locationServiceProxy.getRootLocations().subscribe((result: any) => {
  //     // this.primengTableHelper.getSorting(this.dataTable)
  //     this.cities = result.reverse();
  //     this.firstCitie = this.cities[0];

  //     this._locationServiceProxy.getLocationsByParentId(this.firstCitie.locationId).subscribe((result: any) => {

  //       // this.primengTableHelper.getSorting(this.dataTable)
  //       this.areas = result.reverse();
  //       this.firstAreas = this.areas[0];

  //       this._locationServiceProxy.getAllLocations(
  //         this.primengTableHelper.getSkipCount(this.paginator, event),
  //         this.primengTableHelper.getMaxResultCount(this.paginator, event),
  //         this.primengTableHelper.getSorting(this.dataTable),
  //         this.appSession.tenantId

  //         ).subscribe((result: any) => {
  //           this.primengTableHelper.hideLoadingIndicator();
  //           this.primengTableHelper.totalRecordsCount = result.totalCount;
  //           this.primengTableHelper.records = result.items;
  //           //this.tenantLocations = result.items;
  //         });
  //     });
  //   });

  // }

  // getSearch() {
  //   this._locationServiceProxy.getRootLocations().subscribe((result: any) => {
  //     this.cities = result.reverse();

  //     this.firstCitie = this.cities[0];

  //     this._locationServiceProxy.getLocationsByParentId(this.firstCitie.locationId).subscribe((result: any) => {
  //       this.areas = result.reverse();
  //       this.firstAreas = this.areas[0];
  //       this.isOK = true;
  //     });

  //   });

  // }

  getLocations(event?: LazyLoadEvent, savePage?: boolean) {
      this.currency = this.appSession.tenant.currencyCode;
      let skipCount = 0;
      if (
          (this.cityId != null && this.areaId != null) ||
          (this.cityId != undefined && this.areaId != undefined)
      ) {
          skipCount = 0;
      } else {
          skipCount = this.primengTableHelper.getSkipCount(
              this.paginator,
              event
          );
      }

      this._locationServiceProxy
          .getAllLocations(
              skipCount,
              this.primengTableHelper.getMaxResultCount(
                  this.paginator,
                  event
              ),
              this.primengTableHelper.getSorting(this.dataTable),
              this.appSession.tenantId,
              this.cityId,
              this.areaId
          )
          .subscribe((result: any) => {
              this.primengTableHelper.hideLoadingIndicator();
              this.primengTableHelper.totalRecordsCount = result.totalCount;
              this.primengTableHelper.records = result.items;
              //this.tenantLocations = result.items;
          });
  }

  deleteFilters() {
      this.cityId = null;
      this.areaId = null;
  }
  getCountry() {
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
  onChangeCountry(event) {
    debugger
    this.countryId = event.target.value;
    this.cityId= null;
    this.areaId = null;
    this._locationServiceProxy
        .getLocationsByParentId(this.countryId)
        .subscribe((result: any) => {
            this.cities = result;
            // this.districts = null;
            // this.location.areaId = null;
            // this.location.cityId=event.target.value;
        });
}
  onChangeCity(event) {
      this.cityId = event.target.value;
      this.areaId = null;
      this._locationServiceProxy
          .getLocationsByParentId(this.cityId)
          .subscribe((result: any) => {
              this.areas = result;
              // this.districts = null;
              // this.location.areaId = null;
              // this.location.cityId=event.target.value;
          });
  }

  // searchLocation(parentId : number){
  //   this._locationServiceProxy
  //   .getLocationsByParentId(parentId)
  //   .subscribe((result: any) => {
  //       this.areas = result;
  //       // this.districts = null;
  //       // this.location.areaId = null;
  //       // this.location.cityId=event.target.value;

  // });
  // }
  // onChangeCity(event): void {

  //   this.isOK = false;
  //   const locationId = event.target.value;

  //   if (locationId == "357") {
  //     //this.isOK=true;
  //     this._locationServiceProxy.getLocationsByParentId(locationId).subscribe((result: any) => {
  //       this.areas = result.reverse();
  //       this.firstAreas = this.areas[0];
  //       this._locationServiceProxy.getAllLocations(
  //         this.primengTableHelper.getSkipCount(this.paginator, event),
  //         this.primengTableHelper.getMaxResultCount(this.paginator, event),
  //         this.primengTableHelper.getSorting(this.dataTable),
  //         this.appSession.tenantId
  //       ).subscribe((result: any) => {

  //         this.primengTableHelper.hideLoadingIndicator();
  //         this.primengTableHelper.totalRecordsCount = result.totalCount;
  //         this.primengTableHelper.records = result.items;
  //         //this.tenantLocations = result.items;

  //       });
  //     });

  //   } else {
  //     this.isOK = false;
  //     this._locationService.GetLocationsByParentId(locationId).subscribe((result: any) => {
  //       this.areas = result.reverse();
  //       this.firstAreas = this.areas[0];
  //       // this._locationService.getLocations(this.firstAreas.id).subscribe((result: any) => {

  //       //   this.primengTableHelper.records = result.result;
  //       //   this.tenantLocations = result.result;
  //       //   this.primengTableHelper.totalRecordsCount = this.tenantLocations.length;
  //       //   this.primengTableHelper.hideLoadingIndicator();
  //       // });
  //     });

  //   }

  // }

  onChangeArea(event): void {
      this.areaId = event.target.value;
  }

  createLocation(): void {
      this.createOrEditLocationModal.showCreate();
  }

  editLocation(locationModel: LocationModel): void {
      this.createOrEditLocationModal.showEdit(locationModel);
  }

  deleteLocation(locationModel: LocationInfoModel): void {
      this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
          if (isConfirmed) {
              this._locationService
                  .DeleteLocation(locationModel.id)
                  .subscribe(() => {
                      this.getLocations();
                      this.notify.success(this.l("successfullyDeleted"));
                      //this.getSearch();
                  });
          }
      });
  }
  reset() {}
}
