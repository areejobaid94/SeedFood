import { AfterViewInit, ChangeDetectorRef, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DarkModeService } from '@app/services/dark-mode.service';
import { ThemeHelper } from '@app/shared/layout/themes/ThemeHelper';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as rtlDetect from 'rtl-detect'
import { GroupServiceProxy } from '@shared/service-proxies/service-proxies';
import { MenuItem } from 'primeng/api';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { DeleteUpdateGroupComponent } from './delete-update-group/delete-update-group.component';
import { ExtenalUpdateGroupComponent } from './extenal-update-group/extenal-update-group.component';


@Component({
  selector: 'app-edit-group',
  templateUrl: './edit-group.component.html',
  styleUrls: ['./edit-group.component.css'],
  animations: [appModuleAnimation()],
  encapsulation: ViewEncapsulation.None
})
export class EditGroupComponent extends AppComponentBase implements OnInit ,AfterViewInit {

  @ViewChild('deleteCompoentRef', {static: true}) deleteCompoentRef: DeleteUpdateGroupComponent;

  @ViewChild('externalUpdateGroup', {static: true}) externalUpdateGroup: ExtenalUpdateGroupComponent;

  // @ViewChild(DeleteUpdateGroupComponent) deleteCompoentRef!  : DeleteUpdateGroupComponent;
  // @ViewChild('externalUpdateGroup') externalUpdateGroup: ExtenalUpdateGroupComponent | undefined;
  

  theme: string;
  isAdmin = false;
  isArabic = false;
  groupName: string = '';
  submitted = false;
  filtered = false;
  groupID : number ;
  totalRecords: number;
  tabIndex: number = 0; // Default tab index
  isGroupUnsubscribed: boolean = false;

  constructor(injector: Injector,
    public darkModeService: DarkModeService,
    private groupService : GroupServiceProxy,
     private router: ActivatedRoute, private _router: Router,
     private cdr: ChangeDetectorRef 
  ) {
    super(injector)
  }

  ngAfterViewInit(): void {
    console.log('inite')
  }

  setTab(index: number) {
    debugger;
    if (index < 2 && this.totalRecords < 50000) {
        this.tabIndex = index; // Set the active tab index
    }
  }
  
  ngOnInit() {
    this.theme = ThemeHelper.getTheme();
    this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
  try {
    if (this.router.snapshot.queryParams['id']) {
      this.groupID =  Number(this.router.snapshot.queryParams["id"]);
      this.groupName = String(this.router.snapshot.queryParams["groupName"]);

      if(Number.isNaN(this.groupID) ){
        this._router.navigate(['/app/main/groupcontact']);
        return;
      }
    } else {
      this._router.navigate(['/app/main/groupcontact']);
    }
  } catch (error) {

    this._router.navigate(['/app/main/groupcontact']);
  }
  }

 
  activeTab = 0;
  
  switchHeaders(tabNumber: any) {
    
    this.activeTab = tabNumber.index;

    if(tabNumber.index === 0 ){
      this.deleteCompoentRef.loadCustomers()
    }

    if(tabNumber.index === 2) {
      this.externalUpdateGroup?.resetData();
    }
  }

  handleValueChange(value: string) {
    // this.groupName = value;
  }

  handleTotalChange(value: number) {
    this.totalRecords = value;
  }
  handleIsGroupUnsubscribedChange(value: boolean) {
    this.isGroupUnsubscribed = value;
  }
}
