import { Injectable } from '@angular/core';
import { TeamInboxService } from '@app/main/teamInbox/teaminbox.service';
import { ApplicationInfoDto, GetCurrentLoginInformationsOutput, SessionServiceProxy, TenantLoginInfoDto, UiCustomizationSettingsDto, UserLoginInfoDto } from '@shared/service-proxies/service-proxies';
import { AbpMultiTenancyService } from 'abp-ng2-module';
import { forkJoin } from 'rxjs';

type UserWithRole = UserLoginInfoDto & {
    role?: string;
};

@Injectable()
export class AppSessionService {

    private _user: UserWithRole;
    private _impersonatorUser: UserLoginInfoDto;
    private _tenant: TenantLoginInfoDto;
    private _impersonatorTenant: TenantLoginInfoDto;
    private _application: ApplicationInfoDto;
    private _theme: UiCustomizationSettingsDto;

    constructor(
        private _sessionService: SessionServiceProxy,
        private _teamInboxService: TeamInboxService,
        private _abpMultiTenancyService: AbpMultiTenancyService) {
    }

    get application(): ApplicationInfoDto {
        return this._application;
    }

    set application(val: ApplicationInfoDto) {
        this._application = val;
    }

    get user(): UserWithRole {
        return this._user;
    }

    get userId(): number {
        return this.user ? this.user.id : null;
    }

    get tenant(): TenantLoginInfoDto {
        return this._tenant;
    }

    get tenancyName(): string {
        return this._tenant ? this.tenant.tenancyName : '';
    }

    get tenantId(): number {
        return this.tenant ? this.tenant.id : null;
    }

    getShownLoginName(): string {
                         
        const userName = this._user.userName;
        if (!this._abpMultiTenancyService.isEnabled) {
            return userName;
        }

        return (this._tenant ? this._tenant.tenancyName : '.') + '\\' + userName;
    }

    get theme(): UiCustomizationSettingsDto {
        return this._theme;
    }

    set theme(val: UiCustomizationSettingsDto) {
        this._theme = val;
    }

    async init(): Promise<UiCustomizationSettingsDto> {
        //return Here!!!!
        const result: GetCurrentLoginInformationsOutput =
            await this._sessionService.getCurrentLoginInformations().toPromise();
        this._application = result.application;
        this._user = result.user as UserWithRole;
        this._tenant = result.tenant;
        this._theme = result.theme;
        this._impersonatorTenant = result.impersonatorTenant;
        this._impersonatorUser = result.impersonatorUser;
        return result.theme;
    }

    changeTenantIfNeeded(tenantId?: number): boolean {
                         
        if (this.isCurrentTenant(tenantId)) {
            return false;
        }

        abp.multiTenancy.setTenantIdCookie(tenantId);
        location.reload();
        return true;
    }

    private isCurrentTenant(tenantId?: number) {
                         
        let isTenant = tenantId > 0;

        if (!isTenant && !this.tenant) { // this is host
            return true;
        }

        if (!tenantId && this.tenant) {
            return false;
        } else if (tenantId && (!this.tenant || this.tenant.id !== tenantId)) {
            return false;
        }

        return true;
    }

    private getUserRole = (usersResponse, user: UserLoginInfoDto): string => {
        if (!usersResponse) return null;
        return usersResponse.result.items
            .find(userWithRole => userWithRole.id === user.id)?.roles?.[0].roleName?.toLowerCase();
    };

    get impersonatorUser(): UserLoginInfoDto {
        return this._impersonatorUser;
    }
    get impersonatorUserId(): number {
        return this.impersonatorUser ? this.impersonatorUser.id : null;
    }
    get impersonatorTenant(): TenantLoginInfoDto {
        return this._impersonatorTenant;
    }
    get impersonatorTenancyName(): string {
        return this.impersonatorTenant ? this.impersonatorTenant.tenancyName : '';
    }
    get impersonatorTenantId(): number {
        return this.impersonatorTenant ? this.impersonatorTenant.id : null;
    }
}
