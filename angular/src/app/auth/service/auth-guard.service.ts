import { Injectable, inject } from "@angular/core";
import {
    Router,
    CanActivate,
    ActivatedRouteSnapshot,
    RouterStateSnapshot,
    CanDeactivate,
} from "@angular/router";
import { TeamInboxService } from "@app/main/teamInbox/teaminbox.service";
import { AppSessionService } from "@shared/common/session/app-session.service";
import { AbpSessionService } from "abp-ng2-module";
import { error } from "console";
import { ToastrService } from "ngx-toastr";
import { CookieService } from "ngx-cookie-service";
import { TemplateMessagesServiceProxy } from "@shared/service-proxies/service-proxies";

@Injectable({
    providedIn: "root",
})
export class AuthGuardService implements CanActivate{
    cookieService = inject(CookieService);

    constructor(
        private _router: Router,
        private _appSessionService: AppSessionService,
        private router: Router,
        private toastr: ToastrService,
        private teamService: TeamInboxService,
        private test:TemplateMessagesServiceProxy
    ) {}

    canDeacti(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean {
        // this._appSessionService.tenant.isPaidInvoice = false;
        // this.checkToken();
        if(this._appSessionService.tenant === undefined){
            return;
        }
        if (!this._appSessionService.tenant.isPaidInvoice) {
            // this.toastr.error(
            //     "you have an unpaid bill, pay it now to avoid disconnecting the service",
            //     "WARNING!",
            //     {
            //         tapToDismiss: false,
            //         positionClass: "toast-top-right",
            //         timeOut: 0,
            //         extendedTimeOut: 0,
            //     }
            // );
            this.router.navigate(["/app/main/billings/billings"]);
            //redirect to login/home page etc
            //return false to cancel the navigation
            return false;
        }
        return true;
    }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean {
        // this._appSessionService.tenant.isPaidInvoice = false;
        // this.checkToken();
        if(this._appSessionService.tenant === undefined){
            return;
        }
        if (!this._appSessionService.tenant.isPaidInvoice) {
            // this.toastr.error(
            //     "you have an unpaid bill, pay it now to avoid disconnecting the service",
            //     "WARNING!",
            //     {
            //         tapToDismiss: false,
            //         positionClass: "toast-top-right",
            //         timeOut: 0,
            //         extendedTimeOut: 0,
            //     }
            // );
            this.router.navigate(["/app/main/billings/billings"]);
            //redirect to login/home page etc
            //return false to cancel the navigation
            return false;
        }
        return true;
    }
    checkToken(){
        this._appSessionService.tenantId
        // this.test.getAllNoFilter().subscribe((result: any) => {
        // },
        // (error) => {
        //     if(error.status === 401){
        //         this.cookieService.delete("Abp.AuthToken", " / ");
              
        //         if (window.confirm("The remaining session will get logged out soon, Please login again!")) {
        //             window.location.reload();
        //         } else {
        //             window.location.reload();
        //         }
        //     }
        // })
    }
}
