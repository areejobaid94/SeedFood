
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AreasServiceProxy, CreateOrEditAreaDto, RType, UserListDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, ViewChild, ElementRef, NgZone, EventEmitter, Injector, Output } from '@angular/core';
import { MapsAPILoader } from '@agm/core';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';



@Component({
  selector: 'createOrEditBranchessModal',
  templateUrl: './create-or-edit-branchess-modal.component.html',
  styleUrls: ['./create-or-edit-branchess-modal.component.less']
})
export class CreateOrEditBranchessModalComponent extends AppComponentBase {
  theme: string;

  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving = false;
  area: CreateOrEditAreaDto = new CreateOrEditAreaDto();
  agentsList: UserListDto[];
  agentsList2: UserListDto[];
  rType: RType[];
  isNf: boolean;
  dropdownSettings = {};
  selectedUserIds: Array<any> = [];
  private geoCoder;
  submitted= false;
  @ViewChild("search") public searchElementRef: ElementRef;



  constructor(
    injector: Injector,
    private _userServiceProxy: UserServiceProxy,
    private _areasServiceProxy: AreasServiceProxy,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    public darkModeService: DarkModeService,
  ) {
    super(injector);
  }

  googleMap: google.maps.Map;
  title: string = 'AGM project';

  zoom: number;
  address: string;

  ngOnInit() {
      this.theme = ThemeHelper.getTheme();
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'fullName',
      itemsShowLimit: 3,
      allowSearchFilter: false,
      maxHeight: 200,
      closeDropDownOnSelection: true
    };


    this.mapsAPILoader.load().then(() => {
      this.setCurrentLocation();
      this.geoCoder = new google.maps.Geocoder;
    });
  }
  getSearch(){
    
    if(this.searchElementRef != undefined){
      let autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement);
      autocomplete.addListener("place_changed", () => {
           this.ngZone.run(() => {
             //get the place result
             let place: google.maps.places.PlaceResult = autocomplete.getPlace();
   
             //verify result
             if (place.geometry === undefined || place.geometry === null) {
               return;
             }
   
             //set latitude, longitude and zoom
           
           });
         }
         );
        }
  }


  show(areaId?: number): void {
    if (!areaId) {
        this.selectedUserIds = null;
    } else {
      this.ngOnInit();
    }


    this.isNf = true;

    this._userServiceProxy.getUsers(null, null, null, false, null, 1000, 0).subscribe((result: any) => {
      this.agentsList = result.items;


      if (!areaId) {
        this.area = new CreateOrEditAreaDto();
        this.area.id = areaId;
        this.area.latitude = 31.929183317808885;
        this.area.longitude = 35.8766653636247;
        this.active = true;
        this.modal.show();

      } else {


        this._areasServiceProxy.getAreaById(areaId,this.appSession.tenantId).subscribe(result => {
          this.area = result;

          this.active = true;

          this.selectedUserIds = [];
          if (this.area.userIds != undefined) {
            var array = this.area.userIds.split(',')


            array.forEach(element => {

              var user = this.agentsList.find(x => x.id == parseInt(element));
              if(user){
                this.selectedUserIds.push(user)
              }
            });

          }

          this.modal.show();
        });


      }

    });


  }
  onChangeArea(event): void {
    const agantId = event.target.value;
  }


  save(): void {
    this.saving =true;
    if(this.area.areaName === null || this.area.areaName === undefined ||this.area.areaName === '' ||
    this.area.areaNameEnglish === null || this.area.areaNameEnglish === undefined ||this.area.areaNameEnglish === '' ||
    this.selectedUserIds === null || this.selectedUserIds === undefined || this.selectedUserIds.length === 0 ||
    this.area.latitude === null || this.area.latitude === undefined || 
    this.area.longitude === null || this.area.longitude === undefined 
    ){
      this.submitted=true;
      this.saving =false;
      return
    }
    if (this.area.userId == 0) {

      this.area.isAssginToAllUser = true;
    }
    this.saving = true;
    this.area.userIds = this.selectedUserIds.filter(f => f.id > 0).map(({ id }) => id).toString();

    this._areasServiceProxy.createOrEdit(this.area)
      .pipe(finalize(() => { this.saving = false; }))
      .subscribe(() => {
        this.notify.info(this.l('savedSuccessfully'));
        this.submitted=false;
        this.saving =false;
        this.close();
        this.modalSave.emit(null);
      },(error:any) =>{
        if(error){
            this.saving= false;
            this.submitted=false;
        }
    }
      );
  }


  close(): void {
    this.active = false;
    this.submitted=false;
    this.saving =false;
    this.modal.hide();
  }




  // Get Current Location Coordinates
  private setCurrentLocation() {
    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition((position) => {
        this.area.latitude = position.coords.latitude;
        this.area.longitude = position.coords.longitude;
        this.zoom = 8;
        this.getAddress(this.area.latitude, this.area.longitude);
      });
    }
  }


  markerDragEnd(event) {

    // var marker = new google.maps.Marker({
    //     position: event.latLng,
    //     map: this.googleMap
    //   });

    this.area.latitude = event.latLng.lat();
    this.area.longitude = event.latLng.lng();
    this.getAddress(this.area.latitude, this.area.longitude);
  }

  getAddress(latitude, longitude) {
    this.geoCoder.geocode({ 'location': { lat: latitude, lng: longitude } }, (results, status) => {
      if (status === 'OK') {
        if (results[0]) {
          this.zoom = 12;
          this.address = results[0].formatted_address;
        } else {
          window.alert('No results found');
        }
      } else {
        // window.alert('Geocoder failed due to: ' + status);
      }

    });
  }


}
