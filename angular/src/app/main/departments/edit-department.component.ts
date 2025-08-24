import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { DarkModeService } from '@app/services/dark-mode.service';
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DepartmentModel, DepartmentServiceProxy, UserListDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'editDepartment',
  templateUrl: './edit-department.component.html',
  styleUrls: ['./edit-department.component.css']
})
export class editDepartment extends AppComponentBase {
  theme: string;
  department: DepartmentModel = new DepartmentModel();
  dropdownSettings = {};
  selectedUserIds: Array<UserListDto> = [];
  agentsList: UserListDto[];
  submitted = false;
  saving = false;

  @ViewChild('editDepartment', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  constructor(
    injector: Injector,
    public darkModeService: DarkModeService,
    private _userServiceProxy: UserServiceProxy,
    private _departmentServiceProxy: DepartmentServiceProxy,

  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.theme = ThemeHelper.getTheme();

    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'fullName',
      itemsShowLimit: 3,
      allowSearchFilter: false,
      maxHeight: 200,
      closeDropDownOnSelection: true
    };
  }

  show(department: DepartmentModel): void {
    this.selectedUserIds = [];
    this._userServiceProxy.getUsers(null, null, null, false, null, 1000, 0).subscribe((result: any) => {
      this.agentsList = result.items;
      this.selectedUserIds = [];
      if (department != null) {
        this.department = department;
        if (this.department.userIds != undefined) {
          var array = this.department.userIds.split(',');
          array.forEach(element => {
            let user = this.agentsList.find(x => x.id == parseInt(element));
            this.selectedUserIds.push(user);
          });
        }
        this.modal.show();
      }
    });

  }

  save() {
    this.saving = true;
    if (this.selectedUserIds === null || this.selectedUserIds === undefined || this.selectedUserIds.length === 0) {
      this.submitted = true;
      this.saving = false;
      return
    }
    this.saving = true;
    this.department.userIds = this.selectedUserIds.filter(f => f.id > 0).map(({ id }) => id).toString();
    this._departmentServiceProxy.updateDepartment(this.department)
      .subscribe(() => {
        this.notify.info(this.l('savedSuccessfully'));
        this.submitted = false;
        this.saving = false;
        this.close();
      }, (error: any) => {
        if (error) {
          this.saving = false;
          this.submitted = false;
        }
      }
      );

  }
  close(): void {
    this.modal.hide();
    this.modalSave.emit(null);
  }

}
