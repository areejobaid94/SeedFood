import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { DarkModeService } from '@app/services/dark-mode.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import {WhatsAppMessageTemplateServiceProxy } from '@shared/service-proxies/service-proxies';
import { ThemeHelper } from '@app/shared/layout/themes/ThemeHelper';
import { CreateReservedWordsComponent } from './create-reserved-words.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';


@Component({
  templateUrl: './reserved-words.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: ['./reserved-words.component.css']
})
export class ReservedWordsComponent  extends AppComponentBase {
  theme: string;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild("viewVideo", { static: true })
  viewVideo: CreateReservedWordsComponent;
  search = null;
  constructor( 
    injector: Injector,
    public darkModeService: DarkModeService,
    private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,

    ) {
    super(injector);
   }

  ngOnInit(): void {
    this.theme = ThemeHelper.getTheme();
  }

  getReservedWords(event?: LazyLoadEvent) {
      if (this.primengTableHelper.shouldResetPaging(event)) {
          this.paginator.changePage(0);
          return;
      }
      this.primengTableHelper.showLoadingIndicator();
      this._whatsAppMessageTemplateServiceProxy.keyWordGetByAll
      (
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event),
      )
      .subscribe(result => {
              this.primengTableHelper.totalRecordsCount = result.totalCount;
              this.primengTableHelper.records = result.items;
              this.primengTableHelper.hideLoadingIndicator();
          },(error: any) => {
            if (error) {
              this.primengTableHelper.hideLoadingIndicator();
            }
        });
  }

  deleteword(id){
    this._whatsAppMessageTemplateServiceProxy.keyWordDelete(id)
    .subscribe(result => {
      this.notify.success(this.l('successfullyDeleted'));
      this.reloadPage();
        },(error: any) => {
          if (error) {
            this.notify.error(error);
          }
      });
  }
  
  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
}

}
