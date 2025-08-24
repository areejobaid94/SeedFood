import { Component, Injector, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SharedService } from '@shared/shared-services/shared.service';

@Component({
  selector: 'app-create-group-page',
  templateUrl: './create-group-page.component.html',
  styleUrls: ['./create-group-page.component.css'],
  animations: [appModuleAnimation()],
})
export class CreateGroupPageComponent extends AppComponentBase implements OnInit {

  isExternal : boolean = false;
  isUnsubscribed : boolean = false;

  private groupSharedService: SharedService = inject(SharedService);
  groupName! : string ; 

  constructor(injector: Injector, private router: ActivatedRoute, private _router: Router) {
    super(injector)
  }

  ngOnInit() {
    try {
      if (this.router.snapshot.queryParams["isExternal"] === 'false' || this.router.snapshot.queryParams["isExternal"] === 'true') {
        this.checkName()
        this.isExternal =  this.router.snapshot.queryParams["isExternal"] === 'true' ? true : false ;
        this.isUnsubscribed = this.router.snapshot.queryParams["isUnsubscribed"] === "true";

      } else {
        this._router.navigate(['/app/main/dashboard']);
      }
    } catch (error) {
      this._router.navigate(['/app/main/dashboard']);
    }

  }


  
  checkName() {
    this.groupSharedService.currentGroupName.subscribe((name) => {
        if (name) {
            this.groupName = name;
        } else {
            this._router.navigate(["/app/main/dashboard"]);
        }
    });
}

}
