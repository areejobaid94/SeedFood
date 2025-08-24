import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { editDepartment } from './edit-department.component';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';

@Component({
  templateUrl: './departments.component.html',
  animations: [appModuleAnimation()],

})
export class DepartmentsComponent extends AppComponentBase {
  theme: string;
  @ViewChild("dataTable", { static: true }) dataTable: Table;
  @ViewChild("paginator", { static: true }) paginator: Paginator;
  @ViewChild('editDepartment', { static: true }) editDepartment: editDepartment;

  constructor(
    injector: Injector,
    public darkModeService: DarkModeService,
    private _DepartmentsProxy: DepartmentServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.theme = ThemeHelper.getTheme();
  }

  getDepartments(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    this._DepartmentsProxy
      .getDepartments(
        this.appSession.tenantId,
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.lstDepartments;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

}
