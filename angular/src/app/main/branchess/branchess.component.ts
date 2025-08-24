import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { AreasServiceProxy, AreaDto  } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import { CreateOrEditBranchessModalComponent } from './create-or-edit-branchess-modal.component';
import { ViewBranchessModalComponent } from './view-branchess-modal.component';
import { CreateOrEditBranchessSettingModalComponent } from './create-or-edit-branchess-setting-modal.component';
import { DarkModeService } from './../../services/dark-mode.service';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import * as rtlDetect from 'rtl-detect';


@Component({
    templateUrl: './branchess.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class BranchessComponent extends AppComponentBase {
    
    theme:string;
    @ViewChild('createOrEditBranchessModal', { static: true }) createOrEditBranchessModal: CreateOrEditBranchessModalComponent;
    @ViewChild('createOrEditBranchSettingModel', { static: true }) createOrEditBranchSettingModel: CreateOrEditBranchessSettingModalComponent;
    @ViewChild('viewBranchessModal', { static: true }) viewBranchessModal: ViewBranchessModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterName = '';
    restaurantsFilter = '';
    isNf:boolean;
    isArabic = false;
    constructor(
        injector: Injector,
        private _areasServiceProxy: AreasServiceProxy,
        public darkModeService : DarkModeService,
    ) {
        super(injector);
    }
    
    ngOnInit(): void {
    this.theme= ThemeHelper.getTheme();
    this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
    this.isNf=true;
    }
    getBranches(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();
        
        this._areasServiceProxy.getAreas(
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            
                        
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.lstAreas;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createBranch(): void {
        this.createOrEditBranchessModal.show();        
    }


    deleteBranches(areaId: number): void {
        
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._areasServiceProxy.deleteArea(areaId)
                        .subscribe((result) => {
                            if(result=="DELETED")
                            {
                                this.reloadPage();
                                this.notify.success(this.l('successfullyDeleted'));
                            }
                            if (result=="MENU") {
                                this.message.error("", this.l("branchRelatedWithMenu"));
                            }
                            if (result=="ITEM") {
                                this.message.error("", this.l("branchRelatedWithItem"));
                            }
                            if (result=="DELIVERYCOST") {
                                this.message.error("", this.l("branchRelatedWithdeliveryCost"));
                            }
                            if (result=="LOCATION") {
                                this.message.error("", this.l("branchRelatedWithLocation"));
                            }
                            if (result=="ORDER") {
                                this.message.error("", this.l("branchRelatedWithoRDER"));
                            }
                        });
                }
            }
        );
    }


}
