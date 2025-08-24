import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AssetServiceProxy } from '@shared/service-proxies/service-proxies';
import { ViewAssetsComponent } from './view-assets.component';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';

import { LazyLoadEvent } from 'primeng/api';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { Subscription } from 'rxjs';
import { AddAssetComponent } from './add-asset.component';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';
import * as rtlDetect from 'rtl-detect';


@Component({
    templateUrl: './assets.component.html',
    styleUrls: ['./assets.component.css'],
    encapsulation: ViewEncapsulation.None,
})
export class AssetsComponent extends AppComponentBase {
    theme: string;
    @ViewChild('ViewAssetsComponent', { static: true }) viewAssetsModal: ViewAssetsComponent;
    @ViewChild('AddAssetModal', { static: true }) AddAssetModal: AddAssetComponent;

    @ViewChild("dataTable", { static: true }) dataTable: Table;
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '';
    nameFilter2 = '';
    orderNameFilter = '';
    orderDescriptionFilter = '';
    maxEffectiveTimeFromFilter: moment.Moment;
    minEffectiveTimeFromFilter: moment.Moment;
    maxEffectiveTimeToFilter: moment.Moment;
    minEffectiveTimeToFilter: moment.Moment;
    maxTaxFilter: number;
    maxTaxFilterEmpty: number;
    minTaxFilter: number;
    minTaxFilterEmpty: number;
    imageUriFilter = '';


    isAdmin = false;

    appSession: AppSessionService;
    items: any;
    agentOrderSub: Subscription;
    botOrderSub: Subscription;
    change: any;
    differ: any[];
    isArabic= false;





    constructor(
        public darkModeService: DarkModeService,
        injector: Injector,
        private _asstsServiceProxy: AssetServiceProxy,
    ) { super(injector); }

    ngOnInit(event?: LazyLoadEvent): void {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
    }

    getAssets(event?: LazyLoadEvent){
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();
        this._asstsServiceProxy.getAsset(
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator,event),
            this.appSession.tenant.id
        ).subscribe(result => {
            this.primengTableHelper.records = result.lstAssetDto;
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }


    AddAsset(): void {
        this.AddAssetModal.show();
    }

    DeleteAsset(id: any) {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._asstsServiceProxy.deleteAsset(id)
                        .subscribe((res) => {
                            if (res) {
                                this.reloadPage();
                                this.notify.success(this.l('successfullyDeleted'));
                            } else {
                                this.notify.error('deleteFailed');

                            }

                        });
                }
            }
        );

    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

}
