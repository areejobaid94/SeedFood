import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { SellingRequestDto, SellingRequestServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Paginator } from 'primeng/paginator';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';
@Component({
    selector: 'viewSellingRequestModal',
    templateUrl: './view-sealing-request-modal.component.html',
    styleUrls: ['./view-sealing-request-modal.component.css']
})
export class ViewSealingRequestModalComponent extends AppComponentBase {
    theme: string;

    //  @ViewChild('SideBarMenuComponent', { static: true }) sideBarMenuComponent: SideBarMenuComponent;

    @ViewChild('ViewSellingRequestDeatils', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    active = false;
    saving = false;
    userId: any;
    totalAll = 0;

    sellingRequestDto: SellingRequestDto;
    sunmiInnerPrinter: any;
    createdBy: string;
    phoneNumber: string;
    price: any;
    price2: any;
    requestDescription: any;
    contactInfo: any;
    lstSellingRequestDetailsDto: any[];
    printHtnl: any;
    isRequestForm = false;
    sellingRequestStatus = 1;
    Location: any;
    data = [];
    constructor(
        injector: Injector,
        private _sellingRequestServiceProxy: SellingRequestServiceProxy,
        public darkModeService: DarkModeService,
    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
    }

    viewDetails(objSellingRequestDto: SellingRequestDto): void {
        this.data =[];
        this.sellingRequestStatus = objSellingRequestDto.sellingRequestStatus;
        this.isRequestForm = objSellingRequestDto.isRequestForm;
        this.Location = objSellingRequestDto.requestDescription;
        this.createdBy = objSellingRequestDto.createdBy;
        this.phoneNumber = objSellingRequestDto.phoneNumber;
        this.price = objSellingRequestDto.price;
        this.price2 = objSellingRequestDto.contactInfo;
        this.requestDescription = objSellingRequestDto.requestDescription;
        this.contactInfo = objSellingRequestDto.contactInfo;
        this.lstSellingRequestDetailsDto = objSellingRequestDto.lstSellingRequestDetailsDto;

        this.sellingRequestDto = objSellingRequestDto;

        if( this.sellingRequestDto.requestDescription){
            this.sellingRequestDto.requestDescription = objSellingRequestDto.requestDescription.replace('""','');
            let dataSplited = objSellingRequestDto.requestDescription.split(',');
            dataSplited.forEach(data => {
                if(data.includes('http://') || data.includes('https://')){
                    data= data.replace(/\s/g, '%20');
                }
                this.data.push(data);
            });
        }
      
        this.userId = objSellingRequestDto.userId;
        this.modal.show();
    }

    reloadPage(): void {
        this.totalAll = 0;
        this.paginator.changePage(this.paginator.getPage());
    }

    copyInputMessage(inputElement) {
        inputElement.select();
        document.execCommand('copy');
        inputElement.setSelectionRange(0, 0);
        this.notify.success(this.l('successfullyCopied'));
    }

    close(): void {
        this.totalAll = 0;
        this.modal.hide();
    }

    delete(): void {
        this.totalAll = 0;
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._sellingRequestServiceProxy.deleteSellingRequest(this.sellingRequestDto.id).subscribe(result => {
                        this.active = false;
                        this.notify.success(this.l('SuccessfullyDeleted'));
                        this.modal.hide();
                    });
                }
            }
        );
    }
    done(): void {

        this.totalAll = 0;
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._sellingRequestServiceProxy.doneSellingRequest(this.sellingRequestDto.id).subscribe(result => {
                        this.active = false;
                        this.notify.success(this.l('successfullyDone'));
                        this.modal.hide();
                    });
                }
            }
        );
    }

    chat(): void {
        this.totalAll = 0;
        this.active = false;
        this.modal.hide();
    }
    //  printPage() {

    //      const printContent = document.getElementById("SellingRequestDetails");
    //      const WindowPrt = window.open('', '', '');
    //      WindowPrt.document.write(printContent.innerHTML);
    //      window.focus();
    //      WindowPrt.print();
    //      WindowPrt.close()


    //    }

}
