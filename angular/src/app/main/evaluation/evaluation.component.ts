import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { EvaluationsServiceProxy, EvaluationDto, GetEvaluationForViewDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import { Subscription } from 'rxjs';
import { SocketioService } from '@app/shared/socketio/socketioservice';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';
import * as rtlDetect from 'rtl-detect';
import 'moment-hijri'; // Hijri extension for moment.js
import moment from 'moment';
const { toGregorian } = require('hijri-converter');
@Component({
    templateUrl: './evaluation.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class EvaluationComponent extends AppComponentBase {

    theme: string;


    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    showDropDownList = false;
    advancedFiltersAreShown = false;
    filterName = '';
    restaurantsFilter = '';
    agentevaluationSub: Subscription;
    botevaluationSub: Subscription;

    isArabic = false;

    constructor(
        injector: Injector,
        private socketioService: SocketioService,
        private _evaluationsServiceProxy: EvaluationsServiceProxy,
        private _fileDownloadService: FileDownloadService,
        public darkModeService: DarkModeService,
    ) {
        super(injector);
    }


    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        await this.getIsAdmin();
        this.subscribeBotEvaluation();


    }
    subscribeAgentEvaluation = () => {

        this.agentevaluationSub = this.socketioService.evaluationBot.subscribe((data: GetEvaluationForViewDto) => {

            this.primengTableHelper.records.forEach(element => {

                if (element.evaluation.orderNumber == data.evaluation.orderNumber) {

                    element.evaluation.contactName = data.evaluation.contactName;
                    element.evaluation.creationTime = data.evaluation.creationTime;
                    element.evaluation.evaluationsText = data.evaluation.evaluationsText;
                    element.evaluation.orderNumber = data.evaluation.orderNumber;
                }

            });

        });
    };

    subscribeBotEvaluation = () => {
        this.botevaluationSub = this.socketioService.evaluationBot.subscribe((data: any) => {




            this.primengTableHelper.records.push(data);
            this.primengTableHelper.totalRecordsCount = this.primengTableHelper.totalRecordsCount + 1
            //  this.getTime(this.primengTableHelper.records);
            this.reloadPage();

        });

    };

    getEvaluation(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._evaluationsServiceProxy.getAll(

            this.filterName,
            this.restaurantsFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {

            this.primengTableHelper.totalRecordsCount = result.totalCount;
            if(this.isArabic){
                result.items.forEach(element => {
                    element.creatDate =this.convertHijriToGregorian(moment(element.creatDate).locale('en').format('YYYY-MM-DDTHH:mm:ss'));
                });
            }
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }
    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split('T');
        const [year, month, day] = hijriDateString.split('-').map(Number);
        const gregorianDate = toGregorian(year, month, day);
    
        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, '0'),
            String(gregorianDate.gd).padStart(2, '0'),
        ].join('-');
    
        return formattedDate + 'T' + time;
    }
    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createEvaluation(): void {
        //    this.createOrEditEvaluationModal.show();        
    }


    deleteEvaluation(evaluationDto: EvaluationDto): void {

        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._evaluationsServiceProxy.delete(evaluationDto.id)
                        .subscribe(() => {

                            this.reloadPage();
                            this.notify.success(this.l('successfullyDeleted'));
                        });
                }
            }
        );
    }
    deleteAllEvaluation(): void {

        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._evaluationsServiceProxy.deleteAll()
                        .subscribe(() => {

                            this.reloadPage();
                            this.notify.success(this.l('successfullysDeleted'));
                        });
                }
            }
        );
    }
    exportToExcel(): void {
        this._evaluationsServiceProxy.getEvaluationsToExcel(null, null,
            this.primengTableHelper.getSorting(this.dataTable),
            0,
            10)
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }


}
