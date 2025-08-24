import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';


@Component({
    templateUrl: './cancellation-policy.component.html',
    animations: [accountModuleAnimation()]
})
export class CancellationPolicyComponent extends AppComponentBase {

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
