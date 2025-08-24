import {Inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {LocationModel} from './location.model';
import {AppConsts} from '@shared/AppConsts';
import { LocationListModel } from './location-list.model';
import { PagedResultDtoOfLocationInfoModel } from '@shared/service-proxies/service-proxies';

@Injectable({
    providedIn: 'root'
})

export class LocationService {
    private http: HttpClient;
    constructor(@Inject(HttpClient) http: HttpClient) {
        this.http = http;
}

getLocations(locationid?:number):Observable<LocationListModel[]> {
    
    
    return this.http.get<LocationListModel[]>(AppConsts.remoteServiceBaseUrl + '/api/Locations/GetLocations?locationid='+locationid);
    
}

GetRootLocations():Observable<LocationListModel[]> {
    return this.http.get<LocationListModel[]>(AppConsts.remoteServiceBaseUrl + '/api/Locations/GetRootLocations');
}

GetLocationsByParentId(parentId:number):Observable<LocationListModel[]> {
    return this.http.get<LocationListModel[]>(AppConsts.remoteServiceBaseUrl + '/api/Locations/GetLocationsByParentId?parentId='+parentId);
}

GetAllLocations(TenantId:number,SkipCountt:number,MaxResultCountt:number,Sorting:string): Observable<PagedResultDtoOfLocationInfoModel> {
    
    return this.http.get<PagedResultDtoOfLocationInfoModel>(AppConsts.remoteServiceBaseUrl + '/api/Locations/GetAllLocationsList?TenantId='+TenantId+' &SkipCountt='+SkipCountt+' &MaxResultCountt='+MaxResultCountt+' &Sorting='+Sorting);
}

DeleteLocation(locationId:number) {
    return this.http.post(AppConsts.remoteServiceBaseUrl + '/api/Locations/DeleteLocation?locationId='+locationId,null);
}

EditLocation(model: LocationModel) {
    return this.http.post(
        AppConsts.remoteServiceBaseUrl +
        '/api/Locations/EditLocation',
        model);
}

AddLocation(model: LocationModel) {
    return this.http.post(
        AppConsts.remoteServiceBaseUrl +
        '/api/Locations/AddLocation',
        model);
}


}