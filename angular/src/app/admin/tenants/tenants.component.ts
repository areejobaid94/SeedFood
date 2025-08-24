import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { CommonLookupModalComponent } from '@app/shared/common/lookup/common-lookup-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CommonLookupServiceProxy, EntityDtoOfInt64, FindUsersInput, NameValueDto, TenantListDto, TenantServiceModalDto, TenantServiceProxy, TenantServicesServiceProxy, ZohoServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { CreateTenantModalComponent } from './create-tenant-modal.component';
import { EditTenantModalComponent } from './edit-tenant-modal.component';
import { TenantFeaturesModalComponent } from './tenant-features-modal.component';
import { TenantServicesModalComponent } from './tenant-services-modal.component';
import { TenantSettingsModalComponent } from './tenant-settings-modal.component';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
import { FormArray, FormControl } from '@angular/forms';
import { viewTenantModalComponent } from './view-tenant-modal.component';
import { BillingHostComponent } from '../billingsHost/billings/billingsHost.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ExportModalComponent } from './export-modal.component';
import { AllSettingsModalComponent } from './all-settings-modal/all-settings-modal.component';

@Component({
    templateUrl: './tenants.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class TenantsComponent extends AppComponentBase implements OnInit {

    @ViewChild('impersonateUserLookupModal', {static: true}) impersonateUserLookupModal: CommonLookupModalComponent;
    @ViewChild('createTenantModal', {static: true}) createTenantModal: CreateTenantModalComponent;
    @ViewChild('exportModal', {static: true}) exportModal: ExportModalComponent;

    @ViewChild('editTenantModal', {static: true}) editTenantModal: EditTenantModalComponent;

    @ViewChild('viewTenantModal', {static: true}) viewTenantModal: viewTenantModalComponent;
    @ViewChild('BillingHostModal', {static: true}) BillingHostModal: BillingHostComponent;

    @ViewChild('tenantFeaturesModal', {static: true}) tenantFeaturesModal: TenantFeaturesModalComponent;
    @ViewChild('tenantServicesModal', {static: true}) tenantServicesModal: TenantServicesModalComponent;
    @ViewChild('AllSettingsModal', {static: true}) AllSettingsModal: AllSettingsModalComponent;

    @ViewChild('dataTable', {static: true}) dataTable: Table;
    @ViewChild('paginator', {static: true}) paginator: Paginator;
    @ViewChild('entityTypeHistoryModal', {static: true}) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    subscriptionDateRange: Date[] = [moment().startOf('day').toDate(), moment().add(30, 'days').endOf('day').toDate()];
    creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];

    _entityTypeFullName = 'Infoseed.MessagingPortal.MultiTenancy.Tenant';
    entityHistoryEnabled = false;


    Newitems: TenantListDto[]
    services: TenantServiceModalDto[];

    servicesName:string;
    totalFees:number;
    isActive: string = "";

    filters: {
        filterText: string;
        creationDateRangeActive: boolean;
        subscriptionEndDateRangeActive: boolean;
        selectedEditionId: number;
    } = <any>{};

    constructor(
        private _tenantServiceService: TenantServicesServiceProxy,
        injector: Injector,
        private _tenantService: TenantServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _commonLookupService: CommonLookupServiceProxy,
        private _impersonationService: ImpersonationService,
        private _zohoServiceProxyProxy: ZohoServiceProxy,
        private _fileDownloadService: FileDownloadService,



    ) {
        super(injector);
        this.setFiltersFromRoute();
    }

    setFiltersFromRoute(): void {
        //this.BillingHostModal.tenentID=27;
        if (this._activatedRoute.snapshot.queryParams['subscriptionEndDateStart'] != null) {
            this.filters.subscriptionEndDateRangeActive = true;
            this.subscriptionDateRange[0] = moment(this._activatedRoute.snapshot.queryParams['subscriptionEndDateStart']).toDate();
        } else {
            this.subscriptionDateRange[0] = moment().startOf('day').toDate();
        }

        if (this._activatedRoute.snapshot.queryParams['subscriptionEndDateEnd'] != null) {
            this.filters.subscriptionEndDateRangeActive = true;
            this.subscriptionDateRange[1] = moment(this._activatedRoute.snapshot.queryParams['subscriptionEndDateEnd']).toDate();
        } else {
            this.subscriptionDateRange[1] = moment().add(30, 'days').endOf('day').toDate();
        }

        if (this._activatedRoute.snapshot.queryParams['creationDateStart'] != null) {
            this.filters.creationDateRangeActive = true;
            this.creationDateRange[0] = moment(this._activatedRoute.snapshot.queryParams['creationDateStart']).toDate();
        } else {
            this.creationDateRange[0] = moment().add(-7, 'days').startOf('day').toDate();
        }

        if (this._activatedRoute.snapshot.queryParams['creationDateEnd'] != null) {
            this.filters.creationDateRangeActive = true;
            this.creationDateRange[1] = moment(this._activatedRoute.snapshot.queryParams['creationDateEnd']).toDate();
        } else {
            this.creationDateRange[1] = moment().endOf('day').toDate();
        }

        if (this._activatedRoute.snapshot.queryParams['editionId'] != null) {
            this.filters.selectedEditionId = parseInt(this._activatedRoute.snapshot.queryParams['editionId']);
        }
    }

    ngOnInit(): void {
        this.filters.filterText = this._activatedRoute.snapshot.queryParams['filterText'] || '';

        this.setIsEntityHistoryEnabled();


        this.impersonateUserLookupModal.configure({
            title: this.l('SelectAUser'),
            dataSource: (skipCount: number, maxResultCount: number, filter: string, tenantId?: number) => {
                let input = new FindUsersInput();
                input.filter = filter;
                input.maxResultCount = maxResultCount;
                input.skipCount = skipCount;
                input.tenantId = tenantId;
                return this._commonLookupService.findUsers(input);
            }
        });
    }

    private setIsEntityHistoryEnabled(): void {
        let customSettings = (abp as any).custom;
        this.entityHistoryEnabled = customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getTenants(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._tenantService.getHostTenants(
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.isActive
        ).pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator())).subscribe(result => {
            debugger;
            this.primengTableHelper.totalRecordsCount = result.totalCount;

            // result.items.forEach(element => {
  
                
            //     this._tenantServiceService.getTenatServices(element.id).subscribe((result) => {
            //         this.totalFees=0;
            //         result.forEach(element2 => {                      
            //             if(element2.isSelected){
                            
            //                 this.totalFees=this.totalFees+element2.fees;
            //             }
                       
                        
            //         });
                
            //         element.totalFeesServices=this.totalFees;
                    
                     
            //     });

                
               
            // });
           
            this.primengTableHelper.records= result.items;



            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    showUserImpersonateLookUpModal(record: any): void {
        
        this.impersonateUserLookupModal.tenantId = record.id;
        this.impersonateUserLookupModal.show();
    }

    unlockUser(record: any): void {
        this._tenantService.unlockTenantAdmin(new EntityDtoOfInt64({ id: record.id })).subscribe(() => {
            this.notify.success(this.l('UnlockedTenandAdmin', record.name));
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createTenant(): void {
        this.createTenantModal.show();
    }


    deleteTenant(tenant: TenantListDto): void {
        this.message.confirm(
            this.l('TenantDeleteWarningMessage', tenant.tenancyName),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    this._tenantService.deleteTenant(tenant.id).subscribe(() => {
                        this.reloadPage();
                        this.notify.success(this.l('SuccessfullyDeleted'));
                    });
                }
            }
        );
    }

    showHistory(tenant: TenantListDto): void {
        this.entityTypeHistoryModal.show({
            entityId: tenant.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: tenant.tenancyName
        });
    }

    impersonateUser(item: NameValueDto): void {
        this._impersonationService
            .impersonate(
                parseInt(item.value),
                this.impersonateUserLookupModal.tenantId
            );
    }

    getTenatServices(tenantId): void {
        //this.activeCheckboxFormArray = <FormArray>this.editTenantServicesForm.controls.activeCheckbox;
        this._tenantServiceService.getTenatServices(tenantId).subscribe((result) => {
            this.services = result;

            this.services.forEach(element => {
              
                if(element.isSelected){
                   // this.activeCheckboxFormArray.push(new FormControl(element));
                }else{
                    // let index = this.activeCheckboxFormArray.controls.findIndex(x => x.value == element)
                    // if(index>=0)
                    // this.activeCheckboxFormArray.removeAt(index);
                }
                
            });

        });
    }

    sync( tenantId) {
        this._zohoServiceProxyProxy
            .syncBilling(
             10,
             100,
             tenantId
            )
            .subscribe((result) => {

            });
       
    }

    handleSearchChange(event): void {
        console.log(event)
        this.isActive = event;
    }
    ExporToExcel(){
        this._tenantService.exportTenantsToExcelHost().subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
        });
    }
}
