import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AssetDto } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Paginator } from 'primeng/paginator';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';


@Component({
  selector: 'viewAssetsModal',
  templateUrl: './view-assets.component.html',
})
export class ViewAssetsComponent extends AppComponentBase {
    theme:string;
    @ViewChild('ViewAssetDeatils', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    active = false;
    saving = false;
    userId:any;
    totalAll=0;

    assetDto: AssetDto;
    sunmiInnerPrinter: any;
    constructor(
        injector: Injector,

    )
    {
        super(injector);
    }

  ngOnInit(): void {
    this.theme= ThemeHelper.getTheme();
  }

  viewDetials(objAssetDto: AssetDto): void {
             ;
    this.assetDto=objAssetDto;
    this.modal.show();

 }



 reloadPage(): void {
     this.totalAll=0;
     this.paginator.changePage(this.paginator.getPage());
 }


 close(): void {
     this.totalAll=0;

     this.modal.hide();

 }

}

