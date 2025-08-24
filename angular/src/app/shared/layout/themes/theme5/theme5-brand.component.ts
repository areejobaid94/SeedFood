import { Injector, Component, ViewEncapsulation, Inject } from '@angular/core';

import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

import { DOCUMENT } from '@angular/common';

@Component({
    templateUrl: './theme5-brand.component.html',
    selector: 'theme5-brand',
    encapsulation: ViewEncapsulation.None
})
export class Theme5BrandComponent extends AppComponentBase {

    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;

    isNf:boolean;
    isFrut:boolean;
    isHarNar:boolean;
    isArabieFranji:boolean;
    ShawermaSaj:boolean;
    ShawermaJuicy:boolean;
    ChiliHouse:boolean;

    scrr:string;
    constructor(
        injector: Injector,
        @Inject(DOCUMENT) private document: Document
    ) {
        super(injector);
    }
    ngOnInit() {
        

        if(this.appSession.tenant.image!= null)
        {
            this.scrr=this.appSession.tenant.image;

        }else{

            this.scrr="/assets/common/images/login-logo.svg";
        }
       
        // this.isNf=this.appSession.tenantId===4;
        // this.isFrut=this.appSession.tenantId===5;
        // this.isHarNar=this.appSession.tenantId===6;
        // this.isArabieFranji=this.appSession.tenantId===7;
        // this.ShawermaSaj=this.appSession.tenantId===8;
        // this.ShawermaJuicy=this.appSession.tenantId===9;
        // this.ChiliHouse=this.appSession.tenantId===10;

        
        // if(this.isNf){
        //     this.scrr="/assets/common/images/nafeeseh.jfif";
        // }
        // if(this.isFrut){
        //     this.scrr="/assets/common/images/Fruits.bmp";
        // }

        // if(this.isHarNar){
        //     this.scrr="/assets/common/images/HarNar.jpg";
        // }
        // if(this.isArabieFranji){
        //     this.scrr="/assets/common/images/ArabieFranji.jpg";
        // }
        // if(this.ShawermaSaj){
        //     this.scrr="/assets/common/images/ShawermaSaj.gif";
        // }
        // if(this.ShawermaJuicy){
        //     this.scrr="/assets/common/images/ShawermaJuicy.jpg";
        // }
        // if(this.ChiliHouse){
        //     this.scrr="/assets/common/images/ChiliHouse.jpg";
        // }

    }
    clickTopbarToggle(): void {
        this.document.body.classList.toggle('m-topbar--on');
    }
}
