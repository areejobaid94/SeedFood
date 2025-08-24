import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
;
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AddDeliveryCostComponent } from './add-delivery-cost.component';
import { Paginator } from 'primeng/paginator';
import { DeliveryCostServiceProxy } from '@shared/service-proxies/service-proxies';
import { ThemeHelper } from '../../..../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from '../../services/dark-mode.service';
import * as rtlDetect from 'rtl-detect';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';


@Component({
  templateUrl: './delivery-cost.component.html',
  styleUrls: ['./delivery-cost.component.css'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class DeliveryCostComponent extends AppComponentBase {

    @ViewChild('addDeliveryCost', { static: true }) addDeliveryCostComponent: AddDeliveryCostComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    theme:string;
    currency = '';
    isArabic= false;
    constructor(
        injector: Injector,
        private _deliveryCostServiceProxy: DeliveryCostServiceProxy,
        public darkModeService : DarkModeService,
        )
    {super(injector);}

    ngOnInit(event?: LazyLoadEvent): void {
        debugger ;
        this.theme= ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            //return;
        }
        this.primengTableHelper.showLoadingIndicator();
        this._deliveryCostServiceProxy.getDeliveryCost(this.appSession.tenant.id,0,50).subscribe(result => {
            this.primengTableHelper.getSorting(this.dataTable)
            this.primengTableHelper.hideLoadingIndicator();
            this.primengTableHelper.records  = result.lstDeliveryCostDto;
            this.currency = this.appSession.tenant.currencyCode;
            this.primengTableHelper.totalRecordsCount = result.totalCount;
    });
    }

    DeleteCost(id : number){
                         
        this._deliveryCostServiceProxy.deleteDeliveryCost(id).subscribe((res) => {
            if(res){
            this.reloadPage();
            this.notify.success(this.l('successfullyDeleted'));
            }else{
                this.notify.error('deleteFailed');
            }
           
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }
}
