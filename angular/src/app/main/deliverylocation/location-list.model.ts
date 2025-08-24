export class LocationListModel {
    id:number;
    cityName:string;
    areaName:string;
    locationName:string;
    deliveryCost:number;

    cityId :number;
    areaId :number;
    districtId :number;
    parentId?:number;
    levelId :number;

    districtName :string;
    locationNameEn :string;
    googleURL :string;

    branchAreaId:number;

    branchAreaRes:string;
    branchAreaCor:string;

}