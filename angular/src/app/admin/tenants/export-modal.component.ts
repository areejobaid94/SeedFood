import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'exportModal',
  templateUrl: './export-modal.component.html'
})
export class ExportModalComponent extends AppComponentBase {
  month: any;
  @ViewChild('exportModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _tenantService: TenantServiceProxy,
    private _fileDownloadService: FileDownloadService,

  ) {
    super(injector);
  }

  ngOnInit(): void {
  }

  show() {
    this.month = null;
    this.modal.show();
  }
  close(): void {
    this.modal.hide();
  }
  export() {
    if (this.month === null || this.month === undefined) {
      return;
    }
    this.month = this.month.toString().split('-');
    this.month = this.month.map(Number);
    this.month = this.month[1];

    this._tenantService.exportTenantsToExcel(this.month).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.close();
    });
  }

}
