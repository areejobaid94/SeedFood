import { Component, Injector, ViewChild, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { RoleListDto, RoleServiceProxy, PermissionServiceProxy, FlatPermissionDto, OrderOfferServiceProxy, GetOrderOfferForViewDto } from '@shared/service-proxies/service-proxies';
import { Table } from 'primeng/table';
import { CreateOrEditRoleModalComponent } from './create-or-edit-role-modal.component';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
import { PermissionTreeModalComponent } from '../shared/permission-tree-modal.component';
import { EvaluationSignalRService } from '@app/main/evaluation/evaluation-signalR.service';
import { OrderSignalRService } from '@app/main/order/order-signalR.service';
import { TeamInboxSignalRService } from '@app/main/teamInbox/teaminbox-signalR.service';
import { Paginator } from 'primeng/paginator';
@Component({
    templateUrl: './roles.component.html',
    animations: [appModuleAnimation()]
})
export class OrderOfferComponent extends AppComponentBase implements OnInit {

    @ViewChild('createOrEditRoleModal', { static: true }) createOrEditRoleModal: CreateOrEditRoleModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('permissionFilterTreeModal', { static: true }) permissionFilterTreeModal: PermissionTreeModalComponent;

    @ViewChild('paginator', { static: true }) paginator: Paginator;

    //_entityTypeFullName = 'Infoseed.MessagingPortal.Authorization.Roles.Role';
    entityHistoryEnabled = false;


    orderOffers:GetOrderOfferForViewDto[];
    isOrderOffer:boolean;

    constructor(
        injector: Injector,       
        private orderOfferServiceProxy: OrderOfferServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit(): void {
    this.isOrderOffer=true;
        this.getOrderOffer();
    }

    private setIsEntityHistoryEnabled(): void {
       // let customSettings = (abp as any).custom;
      //  this.entityHistoryEnabled = customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getOrderOffer(): void {
  
        this.orderOfferServiceProxy.getAll(null,null,null,null,null,null,null,null,null,null,null,0,1000)
            .subscribe(result => {
                this.orderOffers = result.items;
            });
    }

    createRole(): void {
        // this.createOrEditRoleModal.show();
    }

    showHistory(role: RoleListDto): void {
        // this.entityTypeHistoryModal.show({
        //     entityId: role.id.toString(),
        //     entityTypeFullName: this._entityTypeFullName,
        //     entityTypeDescription: role.displayName
        // });
    }

    deleteRole(role: RoleListDto): void {
        // let self = this;
        // self.message.confirm(
        //     self.l('RoleDeleteWarningMessage', role.displayName),
        //     this.l('AreYouSure'),
        //     isConfirmed => {
        //         if (isConfirmed) {
        //             this._roleService.deleteRole(role.id).subscribe(() => {
        //                 this.getRoles();
        //                 abp.notify.success(this.l('SuccessfullyDeleted'));
        //             });
        //         }
        //     }
        // );
    }
}
