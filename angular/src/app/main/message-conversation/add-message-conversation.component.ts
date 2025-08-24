import { HttpClient } from "@angular/common/http";
import {
    Component,
    ElementRef,
    EventEmitter,
    Injector,
    Output,
    ViewChild,
} from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/common/app-component-base";
import { WhatsAppConversationSessionServiceProxy, WhatsAppFreeMessageModel, WhatsAppHeaderUrl } from "@shared/service-proxies/service-proxies";

import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs/operators";
import * as rtlDetect from 'rtl-detect';


@Component({
    selector: "AddMessageConversation",
    templateUrl: "./add-message-conversation.component.html",
    styleUrls: ["./add-message-conversation.component.css"],
})
export class AddMessageConversationComponent extends AppComponentBase {
    submitted= false;
    isArabic = false;

    private http: HttpClient;
    constructor(
        injector: Injector,
        private _WhatsAppConversationSessionServiceProxy: WhatsAppConversationSessionServiceProxy,
        http: HttpClient,
        public darkModeService: DarkModeService,

    ) {
        super(injector);
        this.http = http;
    }
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);


    }
    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("file")
    element: ElementRef;

    theme: string;

    active = false;
    saving = false;

    fileToUpload: any;
    fileExtension: string;
    disableSave= false;

    mediaLinks: WhatsAppHeaderUrl = new WhatsAppHeaderUrl();
    freeMessageModel: WhatsAppFreeMessageModel = new WhatsAppFreeMessageModel()
    format: string
    messageText: string
    mediaUrl: string

    messageBody: string
    messageType: string
    headerTextFlag: boolean;
    headerImageFlag: boolean;
    headerVideoFlag: boolean;
    headerDocumentFlag: boolean;
    headerFormatEnum = [
        { id: "TEXT", nameEn: "Text", nameAr: "نص" },
        { id: "DOCUMENT", nameEn: "Document", nameAr: "ملف" },
        { id: "IMAGE", nameEn: "Image", nameAr: "صورة" },
        { id: "VIDEO", nameEn: "Video", nameAr: "فيديو" },
    ];


    show(): void {
        this.headerDocumentFlag = false;
        this.headerVideoFlag = false;
        this.headerImageFlag = false;
        this.headerTextFlag = false;
        this.mediaLinks = new WhatsAppHeaderUrl();
        this.format = null
        this.active = true;
        this.fileToUpload = null;
        this.modal.show();
    }


    save(): void {
        this.saving=true;
        if(this.format === null || this.format === undefined || this.format === '' ||
        (this.headerImageFlag && (this.fileToUpload === null || this.fileToUpload === undefined || this.fileToUpload ==='') ) ||
        (this.headerVideoFlag && (this.fileToUpload === null || this.fileToUpload === undefined || this.fileToUpload ==='')  ) ||
        (this.headerDocumentFlag && (this.fileToUpload === null || this.fileToUpload === undefined || this.fileToUpload ==='')) ||
        (this.headerTextFlag && (this.messageText === null || this.messageText === undefined || this.messageText ==='')) 
        ){
            this.submitted= true;
            this.saving=false;
            return
        }
        if (this.format == "IMAGE" || this.format == "VIDEO" || this.format == "DOCUMENT") {
            if (this.fileToUpload == null || this.fileToUpload == undefined || this.fileToUpload == '') {
                this.message.error("", this.l("pleaseSelectMediaFile"));
                return;
            }
            if(this.mediaLinks.infoSeedUrl == null){

                this.message.error("", this.l("Please Wait Until File Uploaded !"));
                return;
            }else{
                this.freeMessageModel.freeMessage = this.mediaLinks.infoSeedUrl;
            }
        }

        //---Header Text 
        if (this.format == "TEXT") {
            if (this.messageText == null) {
                this.message.error("", this.l("pleaseenterMessageText"));
                return;
            }
            this.freeMessageModel.freeMessage = this.messageText
        }

        this.freeMessageModel.freeMessageType = this.format.toLowerCase()
        this.freeMessageModel.tenantId = this.appSession.tenantId
        document.getElementById("btn").setAttribute("disabled", "")
        this._WhatsAppConversationSessionServiceProxy.addFreeMessage(this.freeMessageModel).pipe(finalize(() => { this.saving = false; })).subscribe(
            (result) => {
                if (isNaN(result)|| result == -1) {
                    this.notify.error(this.l("savedFailed"));
                    this.submitted=false;
                    this.saving = false;
                    document.getElementById("btn").removeAttribute("disabled")
                    this.close();
                    this.modalSave.emit(null);
                    this.fileToUpload = null;

                } else {
                    this.notify.info(this.l("SavedSuccessfully"));
                    document.getElementById("btn").removeAttribute("disabled");
                    this.fileToUpload = null;
                    this.modalSave.emit(null);
                    this.close();
                }
            });
    }

    
    getHeaderHandle(file) {

        (document.getElementById("btn") as HTMLInputElement).disabled = true;
        this.saving=true;
        // this.message.error("", this.l("Please Wait Until File Uploaded !"));
        // setTimeout(function(){(document.getElementById("btn") as HTMLInputElement).disabled = false;},9000);
        this.http.post<WhatsAppHeaderUrl>(AppConsts.remoteServiceBaseUrl + "/api/services/app/WhatsAppConversationSession/GetInfoSeedUrlFile", file)
            .subscribe((result) => {
                this.mediaLinks = new WhatsAppHeaderUrl();
                this.mediaLinks = result;
                if(this.mediaLinks.infoSeedUrl != null){
                    (document.getElementById("btn") as HTMLInputElement).disabled = false;
                    this.saving=false;
                }else{
                    this.message.error("", this.l("invalidFile"));
                    this.saving=false;

                }
            });
    }
    handleFileInput(event) {
        this.mediaLinks = new WhatsAppHeaderUrl();
        this.fileToUpload = event.target.files[0];
        let formDataFile = new FormData();
        this.fileExtension = this.fileToUpload.name.substring(this.fileToUpload.name.lastIndexOf(".") + 1);

        if (this.format == "IMAGE") {
            if (this.fileExtension == "jpg" || this.fileExtension == "jpeg" || this.fileExtension == "png") {

                //this.fileToUpload = event.target.files[0];
                formDataFile.append("formFile", this.fileToUpload);
                this.getHeaderHandle(formDataFile);
            } 
            else {
                this.element.nativeElement.value = "";
                this.message.error("", this.l("pleaseChooseImage"));
                this.fileToUpload =null;
                this.mediaLinks = new WhatsAppHeaderUrl();
                this.freeMessageModel.freeMessage = null;
            }
        }
        if (this.format == "VIDEO") {
            if (this.fileExtension == "mp4") {

                if (this.fileToUpload.size > 30000000 ) {
                    this.element.nativeElement.value = "";
                    this.freeMessageModel.freeMessage = null;
                    this.fileToUpload = null;
                    this.message.error("", this.l("videoTooLarge"));

                }else 
                if(this.fileToUpload.size ==0){

                    this.element.nativeElement.value = "";
                    this.mediaLinks = new WhatsAppHeaderUrl();
                    this.fileToUpload = null; 
                    this.freeMessageModel.freeMessage = null;
                    this.message.error("", this.l("videoTooSmall"));

                }else{
                    formDataFile.append("formFile", this.fileToUpload);
                    this.getHeaderHandle(formDataFile);
                }
            } 
            else {
                this.element.nativeElement.value = "";
                this.message.error("", this.l("pleaseChooseVideo"));
                this.mediaLinks = new WhatsAppHeaderUrl();
                this.fileToUpload = null; 
                this.freeMessageModel.freeMessage = null;
            }
        }

        if (this.format == "DOCUMENT") {
            if (this.fileExtension == "pdf") {
                //this.fileToUpload = event.target.files[0];
                formDataFile.append("formFile", this.fileToUpload);
                this.getHeaderHandle(formDataFile);
            } else {
                this.element.nativeElement.value = "";
                this.mediaLinks = new WhatsAppHeaderUrl();
                this.fileToUpload =null;
                this.freeMessageModel.freeMessage = null;
                this.message.error("", this.l("pleaseChoosePdf"));
            }
        }
    }
    close(): void {
        this.headerDocumentFlag = false;
        this.headerVideoFlag = false;
        this.headerImageFlag = false;
        this.headerTextFlag = false;
        this.submitted=false;
        this.saving = false;
        this.modalSave.emit(null);

        this.active = false;
        this.modal.hide();
    }
    checkFormat(format: any) {
        if (format == "NONE") {
            this.headerDocumentFlag = false;
            this.headerVideoFlag = false;
            this.headerImageFlag = false;
            this.headerTextFlag = false;
        }
        if (format == "TEXT") {
            this.fileToUpload = null;
            this.headerDocumentFlag = false;
            this.headerVideoFlag = false;
            this.headerImageFlag = false;
            this.headerTextFlag = true;
        }
        if (format == "IMAGE") {
            this.mediaLinks = new WhatsAppHeaderUrl();
            this.fileToUpload = null;
            this.headerDocumentFlag = false;
            this.headerVideoFlag = false;
            this.headerImageFlag = true;
            this.headerTextFlag = false;
        }
        if (format == "VIDEO") {
            this.mediaLinks = new WhatsAppHeaderUrl();
            this.fileToUpload = null;
            this.headerDocumentFlag = false;
            this.headerVideoFlag = true;
            this.headerImageFlag = false;
            this.headerTextFlag = false;
        }
        if (format == "DOCUMENT") {
            this.mediaLinks = new WhatsAppHeaderUrl();
            this.fileToUpload = null;
            this.headerDocumentFlag = true;
            this.headerVideoFlag = false;
            this.headerImageFlag = false;
            this.headerTextFlag = false;
        }
    }
}
