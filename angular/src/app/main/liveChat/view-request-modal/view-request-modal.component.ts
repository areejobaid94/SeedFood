import {
    Component,
    EventEmitter,
    Injector,
    OnInit,
    Output,
    ViewChild,
} from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    CustomerLiveChatModel,
    LiveChatServiceProxy,
    SellingRequestDto,
    SellingRequestServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { Paginator } from "primeng/paginator";
import { ViewTicketModalComponent } from "../view-ticket-modal.component";

@Component({
    selector: "app-view-request-modal",
    templateUrl: "./view-request-modal.component.html",
    styleUrls: ["./view-request-modal.component.css"],
})
export class ViewRequestModalComponent
    extends AppComponentBase
    implements OnInit
{
    theme: string;

    //  @ViewChild('SideBarMenuComponent', { static: true }) sideBarMenuComponent: SideBarMenuComponent;

    @ViewChild("viewRequestModalComponent", { static: true })
    modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("paginator", { static: true }) paginator: Paginator;

    @ViewChild("viewTicketModalRequest", { static: true })
    viewTicketModalRequest: ViewTicketModalComponent;

    active = false;
    saving = false;
    userId: any;
    totalAll = 0;

    sellingRequestDto: CustomerLiveChatModel;
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
    isConversationExpired = false;
    isSecondModalOpen = false;

    Location: any;
    data = [];
    constructor(
        injector: Injector,
        public darkModeService: DarkModeService
    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
    }

    @Output() openViewTicketModalEvent = new EventEmitter<void>();

    openViewTicketModal(idLiveChat, userId, type, data, rejectedOrConfirmed,name) {
        const modalData: any = {
            idLiveChat,
            userId,
            type,
            data,
            rejectedOrConfirmed,
            name
        };
        this.openViewTicketModalEvent.emit(modalData);
    }

    viewDetails(objSellingRequestDto: CustomerLiveChatModel): void {
        this.data = [];
        this.sellingRequestStatus = objSellingRequestDto.liveChatStatus;
        this.isConversationExpired =objSellingRequestDto.isConversationExpired;
        this.isRequestForm = objSellingRequestDto.isRequestForm;
        this.Location = objSellingRequestDto.requestDescription;
        this.createdBy = objSellingRequestDto.displayName; // displayName by hassan
        this.phoneNumber = objSellingRequestDto.phoneNumber;
        // this.price = objSellingRequestDto.price;
        // this.price2 = objSellingRequestDto.contactInfo;
        this.requestDescription = objSellingRequestDto.requestDescription;
        this.contactInfo = objSellingRequestDto.contactInfo;
        this.lstSellingRequestDetailsDto =
            objSellingRequestDto.lstSellingRequestDetailsDto;

        this.sellingRequestDto = objSellingRequestDto;
        debugger
        if (this.sellingRequestDto.requestDescription) {
           debugger

            this.sellingRequestDto.requestDescription =
                objSellingRequestDto.requestDescription.replace('""', "");
            let dataSplited =
                objSellingRequestDto.requestDescription.split(",");
            dataSplited.forEach((data) => {
                // if (data.includes("http://") || data.includes("https://")) {
                     data = data.replace('%20 ', ",");
                // }
                // data = "<span style='color: red;'>Your Red Text Here: </span>";
                debugger
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
        document.execCommand("copy");
        inputElement.setSelectionRange(0, 0);
        this.notify.success(this.l("successfullyCopied"));
    }

    close(): void {
        this.totalAll = 0;
        this.modal.hide();
    }

    chat(): void {
        this.totalAll = 0;
        this.active = false;
        this.modal.hide();
    }
    SaveInLocalStorge(){
        localStorage.setItem('ticketType','Request') 
    }
}
