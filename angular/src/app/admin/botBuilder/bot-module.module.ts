import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {BotServiceService  } from './bot-service.service';
import { BotFlowsComponent } from './bot-flows.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { UtilsModule } from "../../../shared/utils/utils.module";
import { AppCommonModule } from "../../shared/common/app-common.module";
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { DropdownModule } from 'primeng/dropdown';
import { CreateFlowComponent } from './create-flow/create-flow.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppBsModalModule } from '@shared/common/appBsModal/app-bs-modal.module';
import { BotBuilderPageComponent } from './bot-builder-page/bot-builder-page.component';
import { DraggableDirective } from './draggable.directive';
import { CreateDialogComponent } from './bot-builder-page/create-dialog/create-dialog.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BotFlowServiceProxy, ItemsServiceProxy, WhatsAppMessageTemplateServiceProxy,} from '@shared/service-proxies/service-proxies';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import { EmojiModule } from '@ctrl/ngx-emoji-mart/ngx-emoji';
import { TestBotComponent } from './bot-builder-page/test-bot/test-bot.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { CreateParameterComponent } from './bot-builder-page/create-dialog/create-parameter/create-parameter.component';
import { StretchNodeDirective } from './stretch.directive';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { FilterPipe } from './filter.pipe';
import {DragDropModule} from '@angular/cdk/drag-drop';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { TabViewModule } from 'primeng/tabview';
import { SelectButtonModule } from 'primeng/selectbutton';
import { CalendarModule } from 'primeng/calendar';
import { Ng2FlatpickrModule } from 'ng2-flatpickr';
import { NodeSearchModalComponent } from './bot-builder-page/node-search-modal/node-search-modal.component';

@NgModule({
    declarations: [BotFlowsComponent, CreateFlowComponent, BotBuilderPageComponent, DraggableDirective, StretchNodeDirective, CreateDialogComponent, TestBotComponent, CreateParameterComponent,FilterPipe, NodeSearchModalComponent],
    providers: [BotServiceService,BotFlowServiceProxy,ItemsServiceProxy,WhatsAppMessageTemplateServiceProxy
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        UtilsModule,
        AppCommonModule,
        PaginatorModule,
        TableModule,
        DropdownModule,
        ModalModule.forRoot(),
        BsDropdownModule.forRoot(),
        PickerModule,
        CalendarModule,
        Ng2FlatpickrModule,
        NgbModule,
        AppBsModalModule,
        TabViewModule,
        SelectButtonModule,
        EmojiModule,
        NgSelectModule,
        NgMultiSelectDropDownModule,
        DragDropModule,
        NgbDropdownModule

    ]
})
export class BotModuleModule { }
