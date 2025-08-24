import { Injectable } from "@angular/core";
import { TeamInboxService } from "@app/main/teamInbox/teaminbox.service";
import {
    GetCurrentLoginInformationsOutput,
    SessionServiceProxy,
    UserLoginInfoDto,
} from "@shared/service-proxies/service-proxies";

@Injectable({
    providedIn: "root",
})
export class RoleService {
    private _roles: string[] | null = null;  // Initialize roles to null to check if they are set
    private rolesPromise: Promise<void> | null = null;  // To track if the roles are being fetched

    constructor(
        private _teamInboxService: TeamInboxService,
        private _sessionService: SessionServiceProxy
    ) {}

    // Make sure to call this method when the service is initialized or needed
    async setRoles(): Promise<void> {
        // console.log(this._roles)
        if(!this._roles){
            if (this.rolesPromise) {
                // If roles are already being fetched, wait for the previous promise
                await this.rolesPromise;
                return;
            }
            
            this.rolesPromise = (async () => {
                try {
                    const usersResponse = await this._teamInboxService.getUsers().toPromise();
                    const result: GetCurrentLoginInformationsOutput =
                        await this._sessionService.getCurrentLoginInformations().toPromise();
                    this._roles = this.getUserRole(usersResponse, result.user);
                } catch (error) {
                    // Handle the error appropriately
                    console.error('Error fetching roles:', error);
                    this._roles = [];  // Or handle error as appropriate
                } finally {
                    this.rolesPromise = null;  // Reset the rolesPromise
                }
            })();
            
            await this.rolesPromise;
        }
    }

    getRoles(): string[] {
        if (this._roles === null) {
            // Handle the case where roles are not yet set
            throw new Error('Roles have not been set. Call setRoles() first.');
        }
        return this._roles;
    }

    private getUserRole(usersResponse, user: UserLoginInfoDto): string[] {
        if (!usersResponse) return [];
        const userRoles = usersResponse.result.items.find(
            (userR) => userR.id === user.id
        )?.roles.map(role => role.roleName);
        return userRoles || [];
    }

    async isAdmin(): Promise<boolean> {
        // Ensure roles are set before checking
        await this.setRoles();
        return this._roles?.some(role => role.toLowerCase() === 'admin') || false;
    }
}
