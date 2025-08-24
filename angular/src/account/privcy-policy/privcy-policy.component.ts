import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';


@Component({
    templateUrl: './privcy-policy.component.html',
    
    animations: [accountModuleAnimation()] ,
    styleUrls: ['./privcy-policy.component.css']


})
export class PrivcyPolicyComponent extends AppComponentBase {

    constructor (
        injector: Injector,     
        private _router: Router
        ) {
        super(injector);
    }

    back(): void {

      this._router.navigate(['account/login']);
             
    }
}
