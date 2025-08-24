import { Component,Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { LocationService } from './location.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';

import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { LocationListModel } from './location-list.model';
import { CreateOrEditDeliveryLocationModelComponent } from './create-or-edit-location';
import { DeliveryLocationInfoModel, LocationsServiceProxy } from '@shared/service-proxies/service-proxies';
import { ThemeHelper } from '../../..../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '../../services/dark-mode.service';

@Component({
  selector: 'app-location',
  templateUrl: './location.component.html',
  styleUrls: ['./location.component.css'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]

})
export class DeliveryLocationComponent extends AppComponentBase {
  @ViewChild('createOrEditDeliveryLocationModal', { static: true }) createOrEditDeliveryLocationModal: CreateOrEditDeliveryLocationModelComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  theme:string;
    tenantLocations: LocationListModel[];
    cities: LocationListModel[];
    firstCitie: LocationListModel;
    areas: LocationListModel[];
    firstAreas: LocationListModel;
    locationModel:DeliveryLocationInfoModel;
    advancedFiltersAreShown = false;

  constructor(
    injector: Injector,
    
    private locationsServiceProxy: LocationsServiceProxy,
    private _locationService: LocationService,
    public darkModeService : DarkModeService,
) {
    super(injector);
    //this.getLocation();
    //this.getCities();
}

ngOnInit(): void {
  this.theme= ThemeHelper.getTheme();
}

  getCities(event?: LazyLoadEvent){
    
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
  }

  // this.primengTableHelper.showLoadingIndicator();

     this.locationsServiceProxy.getAllDeliveryLocationCost(this.primengTableHelper.getSkipCount(this.paginator, event), this.primengTableHelper.getMaxResultCount(this.paginator, event),this.appSession.tenantId,this.primengTableHelper.getSorting(this.dataTable)).subscribe((result: DeliveryLocationInfoModel[]) => {

      this.primengTableHelper.getSorting(this.dataTable)
      this.primengTableHelper.records= result;
      this.primengTableHelper.totalRecordsCount = result.length;
      this.primengTableHelper.hideLoadingIndicator();
          });
    }

    onChangeCity(event): void 
    {
     
      const locationId = event.target.value;

      if(locationId=="357"){

        this._locationService.GetLocationsByParentId(locationId).subscribe((result: any) => {
          this.areas = result.result.reverse(); 
          this.firstAreas=this.areas[0];

                 this._locationService.GetAllLocations(this.appSession.tenantId).subscribe((result: any) => {
                                   
            this.primengTableHelper.records= result.result;
            this.tenantLocations = result.result;
            this.primengTableHelper.totalRecordsCount = this.tenantLocations.length;
            this.primengTableHelper.hideLoadingIndicator();
          }); 
          // this._locationService.getLocations(this.firstAreas.id).subscribe((result: any) => {
          //                    
          //   this.primengTableHelper.records= result.result;
          //   this.tenantLocations = result.result;
          // });  

        });

      }else{

        this._locationService.GetLocationsByParentId(locationId).subscribe((result: any) => {
          this.areas = result.result.reverse(); 
          this.firstAreas=this.areas[0];
          this._locationService.getLocations(this.firstAreas.id).subscribe((result: any) => {
                             
            this.primengTableHelper.records= result.result;
            this.tenantLocations = result.result;
            this.primengTableHelper.totalRecordsCount = this.tenantLocations.length;
            this.primengTableHelper.hideLoadingIndicator();
          });  
        });

      }
   
    }

    onChangeArea(event): void 
    {
      
      const locationId = event.target.value;
      this._locationService.getLocations(locationId).subscribe((result: any) => {
        
                         
        this.primengTableHelper.records= result.result;
        this.tenantLocations = result.result;
      });
      //this._locationService.GetLocationsByParentId(locationId).subscribe((result: any) => {
        //this.areas = result.result;  
     // });
    }

    createLocation():void
    {
      this.locationModel = new DeliveryLocationInfoModel();
      this.createOrEditDeliveryLocationModal.showCreate(this.locationModel);  
      
    }

    editLocation(locationModel:DeliveryLocationInfoModel):void{
      

      
      this.createOrEditDeliveryLocationModal.showEdit(locationModel);  
      
    }

    deleteLocation(locationModel:DeliveryLocationInfoModel):void{
      this.message.confirm(
        '',
        this.l('AreYouSure'),
        (isConfirmed) => {
            if (isConfirmed) {
              this.locationsServiceProxy.delete(locationModel)
                    .subscribe(() => {
                      this.getCities();
                        this.notify.success(this.l('successfullyDeleted'));
                    });
            }
        }
    );
     
    }

    exportToExcel():void{

    }
}
