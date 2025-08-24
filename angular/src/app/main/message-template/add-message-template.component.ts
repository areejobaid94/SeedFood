import { HttpClient } from "@angular/common/http";
import {
    Component,
    ElementRef,
    Renderer2,
    EventEmitter,
    Injector,
    Output,
    ViewChild,
    HostListener,
} from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    MessageTemplateModel,
    WhatsAppButtonModel,
    WhatsAppComponentModel,
    WhatsAppExampleModel,
    WhatsAppHeaderUrl,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import Stepper from "bs-stepper";
import { ModalDirective } from "ngx-bootstrap/modal";
import {
    CountryISO,
    PhoneNumberFormat,
    SearchCountryField,
} from "ngx-intl-tel-input";
import { finalize } from "rxjs/operators";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import {
    EditorChangeContent,
    EditorChangeSelection,
    QuillEditorComponent,
} from "ngx-quill";
import "quill-emoji/dist/quill-emoji.js";
const { compile } = require("html-to-text");
const { convert } = require("html-to-text");
import { AsYouType } from "libphonenumber-js";
import { Quill } from "assets/vendors/js/editors/quill/quill";
import * as rtlDetect from "rtl-detect";
import { LazyLoadEvent } from "primeng/api";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { ActivatedRoute } from "@node_modules/@angular/router";

const options = {
    wordwrap: 130,
    // ...
};
const compiledConvert = compile(options); // options passed here

@Component({
    selector: "AddMessageTemplate",
    templateUrl: "./add-message-template.component.html",
    styleUrls: ["./add-message-template.component.css"],
})
export class AddMessageTemplateComponent extends AppComponentBase {
    phoneForm = new FormGroup({
        phone: new FormControl(undefined, [Validators.required]),
    });
    private http: HttpClient;
    theme: string;
    private verticalWizardStepper: Stepper;
    countryObj: any = null;
    separateDialCode = false;
    SearchCountryField = SearchCountryField;
    CountryISO = CountryISO;
    Template: MessageTemplateModel = new MessageTemplateModel();
    TemplateId: string;
    PhoneNumberFormat = PhoneNumberFormat;
    preferredCountries: CountryISO[] = [
        CountryISO.SaudiArabia,
        CountryISO.Jordan,
    ];
    variableCount = 0;
    showSecondButton = false;
    showFirstButton = true;
    listOfVariables = [];
    dynamicVariables: string[] = [];
    firstSaveFlag: boolean = false;
    var0: string = "";
    var1: string = "";
    var2: string = "";
    var3: string = "";
    var4: string = "";
    isTextValid = true;
    submitted = false;
    isOptOutMessage: boolean = false
    modules = {};
    content = "";
    imageFlag: boolean = false;
    videoFlag: boolean = false;
    documentFlag: boolean = false;
    quillEditor: any;
    tempButton = new WhatsAppButtonModel()
    tempButton2 = new WhatsAppButtonModel()
    IsStep2: boolean = false;
    isOptOut: boolean = false;
    
    
    @ViewChild(QuillEditorComponent, { static: false })
    editor: QuillEditorComponent;
    @ViewChild("emojiMart", { static: false }) emojiMart: ElementRef;
    hideEmoji = true;
    @ViewChild("myTextarea") myTextarea: ElementRef;
    // Close Emoji-Mart when clicking outside
    @HostListener("document:click", ["$event"])
    onClick(event: MouseEvent) {
        const target = event.target as HTMLElement;
        if (
            !this.emojiMart.nativeElement.contains(target) &&
            !target.classList.contains("bi-emoji-smile")
        ) {
            this.hideEmoji = true; // Close Emoji-Mart
        }
    }
    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        http: HttpClient,
        public darkModeService: DarkModeService,
        private renderer: Renderer2,
        private sanitizer: DomSanitizer,
        private route: ActivatedRoute
    ) {
        super(injector);
        this.http = http;
        this.modules = {
            "emoji-shortname": true,
            "emoji-textarea": true,
            "emoji-toolbar": true,
            toolbar: [["bold", "italic"]],
        };
    }

    ngOnInit(): void { 
        debugger
        const notificationStatus = localStorage.getItem('notificationStatus');
        const errorMessage = localStorage.getItem('errorMessage');
    
        localStorage.removeItem('notificationStatus');
        localStorage.removeItem('errorMessage');
    
        if (notificationStatus) {
            if (notificationStatus === 'successful') {
                this.notify.info(this.l("SavedSuccessfully"));
            } else if (notificationStatus === 'failed') {
                if (errorMessage) {
                    this.notify.error(`Failed: ${errorMessage}`);
                } else {
                    this.notify.error('An unknown error occurred.');
                }
            }
        }
        this.theme = ThemeHelper.getTheme();
        this.dynamicVariables = [];
        this.componentButtonForView = new WhatsAppComponentModel();
        this.componentButtonForView.buttons = [];
        this.componentButtonForView.buttons.push(this.tempButton);
        this.componentButtonForView.buttons.push(this.tempButton2);
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.verticalWizardStepper = new Stepper(
            document.querySelector("#stepper2"),
            {
                linear: false,
                animation: true,
            }
        );
    }
    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("file")
    element: ElementRef;

    @ViewChild("name")
    name: ElementRef;
    extFile: string;

    active = false;
    saving = false;
    isEdit: boolean;
    isUsed: boolean = false;
    headerTextFlag: boolean;
    headerImageFlag: boolean;
    headerVideoFlag: boolean;
    headerDocumentFlag: boolean;
    buttonFlag: boolean;
    phoneButtonFlag: boolean;
    urlButtonFlag: boolean;
    textButtonFlag: boolean;
    textButtonTwoFlag: boolean;
    optOutButtonFlag: boolean;
    predefinedStop = "Stop";
    checkTemplate: MessageTemplateModel[] = [new MessageTemplateModel()];
    template: MessageTemplateModel = new MessageTemplateModel();
    componentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
    componentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
    componentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
    footer: string = "";
    mediaUrl: WhatsAppExampleModel = new WhatsAppExampleModel();
    exampleBody: WhatsAppExampleModel = new WhatsAppExampleModel();
    button: WhatsAppButtonModel = new WhatsAppButtonModel();
    buttonTwo: WhatsAppButtonModel = new WhatsAppButtonModel();
    componentButton: WhatsAppComponentModel = new WhatsAppComponentModel();
    componentButtonForView: WhatsAppComponentModel = new WhatsAppComponentModel();
    whatsAppHeaderHandle: WhatsAppHeaderUrl = new WhatsAppHeaderUrl();
    fileToUpload: any;
    tempFile: any;
    textBodyMesage = false;
    countryCode: string;
    formattedTextBody: SafeHtml;
    formattedText: string;

    languageEnum = [
        { id: "en", name: "English" },
        { id: "ar", name: "Arabic" },
    ];
    categoryEnum = [
        {
            id: "UTILITY",
            nameEn: "Utility",
            nameAr: "خدمات",
            placeholderEn: "Send message about an existing order or account.",
            placeholderAr: "إرسال رسالة حول أمر موجود أو حساب.",
        },
        {
            id: "MARKETING",
            nameEn: "Marketing",
            nameAr: "تسويق",
            placeholderEn:
                "Send promotions or information about your product, services or buisness.",
            placeholderAr:
                "إرسال العروض الترويجية أو المعلومات حول منتجك أو خدماتك أو عملك.",
        }
        // {
        //     id: "AUTHENTICATION",
        //     name: "Authentication",
        //     placeholder:
        //         "Send codes to verify a transaction or login.",
        // }
    ];
    headerFormatEnum = [
        { id: "NONE", nameEn: "None", nameAr: "لا شيء" },
        { id: "TEXT", nameEn: "Text", nameAr: "نص" },
        { id: "DOCUMENT", nameEn: "Document", nameAr: "مستند" },
        { id: "IMAGE", nameEn: "Image", nameAr: "صورة" },
        { id: "VIDEO", nameEn: "Video", nameAr: "فيديو" },
    ];
    componentTypeEnum = [
        { id: "HEADER", name: "Header" },
        { id: "BODY", name: "Body" },
        { id: "FOOTER", name: "Footer" },
        { id: "BUTTONS", name: "Buttons" },
    ];
    buttonTypeEnum = [
        { id: "NONE", nameEn: "None", nameAr: "لا شيء" },
        { id: "QUICK_REPLY", nameEn: "Quick Reply", nameAr: "رد سريع" },
        { id: "PHONE_NUMBER", nameEn: "Phone Number", nameAr: "رقم الهاتف" },
        { id: "URL", nameEn: "URL", nameAr: "رابط" },
        { id: "Opt-out", nameEn: "Opt-out", nameAr: "انسحاب" },
    ];

    htmls = [];

    regexp = new RegExp("^[a-zA-Z0-9_.-]*$");
    isValidName = true;
    blured = false;
    focused = false;
    isArabic = false;

    show(): void {
        this.variableCount = 0;
        this.template = null;
        this.componentHeader = null;
        this.componentBody = null;
        this.componentFooter = null;
        this.componentButton = null;
        this.button = null;
        this.footer = ""

        this.template = new MessageTemplateModel();
        this.componentHeader = new WhatsAppComponentModel();
        this.componentBody = new WhatsAppComponentModel();
        this.componentFooter = new WhatsAppComponentModel();
        this.componentButton = new WhatsAppComponentModel();
        this.button = new WhatsAppButtonModel();
        this.componentButtonForView.buttons[0].text = "";
        this.componentButtonForView.buttons[1].text = "";

        this.componentHeader.format = "NONE";
        this.showSecondButton = false;
        this.countryObj = null;
        this.phoneForm.reset();
        this.isEdit = false;
        this.active = true;
        this.getMessageTemplate();
        this.formatText();
        this.modal.show();
    }

    edit(template: any): void {
        debugger;
        this.template = null;
        this.componentHeader = null;
        this.componentBody = null;
        this.componentFooter = null;
        this.componentButton = null;
        this.button = null;
        this.buttonTwo = null;

        // this.buttonCopyCodeVariabllesTemplate=null;
        // this.secondButtonURLVariabllesTemplate=null
        // this.firstButtonURLVariabllesTemplate=null;
        // this.variableURL2=false;
        // this.variableURL1=false;

        this.componentHeader = new WhatsAppComponentModel();
        this.componentBody = new WhatsAppComponentModel();
        this.componentFooter = new WhatsAppComponentModel();
        this.componentButton = new WhatsAppComponentModel();
        this.button = new WhatsAppButtonModel();
        this.componentButtonForView.buttons[0].text = "";
        this.componentButtonForView.buttons[1].text = "";

        this.TemplateId = template.id;
        this.Template = new MessageTemplateModel();
        this.isEdit = true;
        this.active = true;
        this.template = new MessageTemplateModel();
        this.template = template;

        this._whatsAppMessageTemplateServiceProxy
            .getTemplateByWhatsAppId(this.TemplateId)
            .subscribe((result) => {
                // this.variableCount = result.variableCount;
                this.Template = result;

                this.template.localTemplateId = result.localTemplateId;
                if (this.Template.mediaType == "image") {
                    this.imageFlag = true;
                } else {
                    this.imageFlag = false;
                }
                if (this.Template.mediaType == "video") {
                    this.videoFlag = true;
                } else {
                    this.videoFlag = false;
                }
                if (this.Template.mediaType == "document") {
                    this.documentFlag = true;
                } else {
                    this.documentFlag = false;
                }

                this.componentHeader = template.components.find(
                    (i: { type: string }) => i.type === "HEADER"
                );
                if (
                    this.componentHeader != null ||
                    this.componentHeader != undefined
                ) {
                    if (this.componentHeader.format == "TEXT") {
                        this.headerTextFlag = true;
                    }
                    if (this.componentHeader.format == "IMAGE") {
                        this.headerImageFlag = true;
                    }
                    if (this.componentHeader.format == "VIDEO") {
                        this.headerVideoFlag = true;
                    }
                    if (this.componentHeader.format == "DOCUMENT") {
                        this.headerDocumentFlag = true;
                    }
                } else {
                    this.componentHeader = new WhatsAppComponentModel();
                    this.componentHeader.format = "NONE";
                }
                // this.componentBody = template.components.find((i: { type: string; }) => i.type === 'BODY');
                this.componentBody = this.Template.components.find(
                    (i: { type: string }) => i.type === "BODY"
                );
                console.log(this.componentBody);

                if (this.componentBody.example) {
                    if (this.componentBody.example.body_text) {
                        this.listOfVariables.length =
                            this.componentBody.example.body_text[0].length;
                        this.dynamicVariables =
                            this.componentBody.example.body_text[0];
                    }
                }

                this.componentFooter = this.Template.components.find(
                    (i: { type: string }) => i.type === "FOOTER"
                );

                this.footer = this.componentFooter?.text;

                this.componentButton = template.components.find(
                    (i: { type: string }) => i.type === "BUTTONS"
                );
                if (
                    this.componentButton != null ||
                    this.componentButton != undefined
                ) {
                    this.componentButton.buttons.forEach((element) => {
                        this.checkButtonType(element.type);
                        if (element.type == "QUICK_REPLY") {
                            if (this.componentButton.buttons[0] == element) {
                                this.button = element;
                            }
                            if (this.componentButton.buttons.length == 2) {
                                this.createButtonTwo();
                                if (
                                    this.componentButton.buttons[1] == element
                                ) {
                                    this.buttonTwo = element;
                                }
                            }
                        }
                        // opt-Out return (this is not woking for now!)
                        if (element.type == "Opt-out") {
                            if (this.componentButton.buttons[0] == element) {
                                this.button = element;
                            }
                            if (this.componentButton.buttons.length == 2) {
                                this.createButtonTwo();
                                if (
                                    this.componentButton.buttons[1] == element
                                ) {
                                    this.buttonTwo = element;
                                }
                            }
                        }
                        if (element.type == "PHONE_NUMBER") {
                            //    element.phone_number = element.phone_number.replace("+","");
                            let phoneNumber = new AsYouType().input(
                                element.phone_number
                            );
                            let split = phoneNumber.split(" ");
                            let countryCode = phoneNumber.substring(
                                0,
                                phoneNumber.indexOf(" ")
                            );
                            if (countryCode.includes("+")) {
                                this.countryCode = countryCode.slice(1);
                            }
                            let number = phoneNumber.substring(
                                phoneNumber.indexOf(" ") + 1
                            );
                            this.phoneForm.controls["phone"].setValue(
                                element.phone_number
                            );
                            this.button = element;
                        }
                        if (element.type == "URL") {
                            this.checkButtonType(this.componentButton.format);
                            this.button = element;
                        }
                    });
                } else {
                    this.componentButton = new WhatsAppComponentModel();
                    this.button.type = "NONE";
                }
                if(this.button?.text == "Stop promotions" || this.button?.text == "ايقاف عمليات الترويج" || this.buttonTwo?.text == "Stop promotions"|| this.buttonTwo?.text == "ايقاف عمليات الترويج"){
                    this.isOptOut = true;
                    this.isOptOutMessage = true;
                }
                this.formatText();
                this.modal.show();
            });
    }

    createButtonTwo() {
        this.showSecondButton = true;
        this.componentButtonForView.buttons[1].text = "";        
    }

    deleteButtonOne() {
        this.showFirstButton = false;
    }

    deleteButtonTwo() {
        this.showSecondButton = false;
        this.buttonTwo.text = "";
        this.componentButtonForView.buttons[1].text = ""; 
    }

    getMessageTemplate() {
        debugger
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppMessageTemplate()
            .subscribe((result) => {
                debugger
                this.checkTemplate = result.data;
            });
    }
    checkTemplateNameLanguage(name: string) {
        this.isUsed = false;
        this.isValidName = true;
        name;
        this.isValidName = this.regexp.test(name);
        if (!this.isValidName) {
            return;
        }

        this.checkTemplate.forEach((element) => {
            if (element.name == name.toLowerCase().split(" ").join("_")) {
                this.isUsed = true;
                return;
            }
        });
    }
    checkTemplateName(name: string): boolean {
        debugger
        this.isUsed = false;
        this.checkTemplate.forEach((element) => {
            debugger
            if (element.name == name.toLowerCase().split(" ").join("_")) {
                // this.message.error("", this.l("thisNameAlreadyUsed"));
                this.isUsed = true;
                return;
            }
        });
        return this.isUsed;
    }
    checkTemplateButtonUrl() {
        if (!this.button.url.startsWith("https://")) {
            this.message.error("", this.l("urlNotValid"));
            this.button.url = null;
            return;
        }
    }

    checkPhoneNumber() {
        if (this.phoneForm.status == "VALID") {
            this.countryObj = this.phoneForm.value.phone;
            this.button.phone_number = this.countryObj.e164Number.replace(
                "+",
                ""
            );
        } else {
            this.button.phone_number = null;
            this.phoneForm.value.phone = null;
            this.countryObj = null;
            return;
        }
    }
    GetHeaderHandle(file: FormData) {
        this.http
            .post<WhatsAppHeaderUrl>(
                AppConsts.remoteServiceBaseUrl +
                    "/api/services/app/WhatsAppMessageTemplate/GetWhatsAppMediaLink",
                file
            )
            .subscribe((result) => {
                this.whatsAppHeaderHandle = new WhatsAppHeaderUrl();
                this.Template.mediaLink = result.infoSeedUrl;
                this.whatsAppHeaderHandle = result;
            });
    }

    handleFileInput(event: { target: { files: any[] } }) {
        this.fileToUpload = event.target.files[0];
        let formDataFile = new FormData();

        this.extFile = this.fileToUpload.name.substring(
            this.fileToUpload.name.lastIndexOf(".") + 1
        );

        if (this.componentHeader.format == "IMAGE") {
            if (
                this.extFile == "jpg" ||
                this.extFile == "jpeg" ||
                this.extFile == "png"
            ) {
                this.fileToUpload = event.target.files[0];
                this.Template.mediaLink = "";
                this.imageFlag = false;
                formDataFile.append("formFile", this.fileToUpload);
                this.GetHeaderHandle(formDataFile);
            } else {
                this.element.nativeElement.value = "";
                this.message.error("", this.l("pleaseChooseImage"));
                event.target.files[0] = null;
                this.fileToUpload = null;
            }
        }

        if (this.componentHeader.format == "VIDEO") {
            if (this.extFile == "mp4") {
                this.fileToUpload = event.target.files[0];
                if (this.fileToUpload.size > 30000000) {
                    this.element.nativeElement.value = "";
                    this.message.error("", this.l("videoTooLarge"));
                    this.fileToUpload = null;
                } else if (this.fileToUpload.size == 0) {
                    this.element.nativeElement.value = "";
                    this.message.error("", this.l("videoTooSmall"));
                    this.fileToUpload = null;
                } else {
                    formDataFile.append("formFile", this.fileToUpload);
                    this.GetHeaderHandle(formDataFile);
                    this.Template.mediaLink = "";
                    this.videoFlag = false;
                }
            } else {
                this.element.nativeElement.value = "";
                this.message.error("", this.l("pleaseChooseVideo"));
                event.target.files[0] = null;
                this.fileToUpload = null;
            }
        }

        if (this.componentHeader.format == "DOCUMENT") {
            if (this.extFile == "pdf") {
                this.fileToUpload = event.target.files[0];
                formDataFile.append("formFile", this.fileToUpload);
                this.GetHeaderHandle(formDataFile);
                this.Template.mediaLink = "";
            } else {
                this.element.nativeElement.value = "";
                this.message.error("", this.l("pleaseChoosePdf"));
                event.target.files[0] = null;
                this.fileToUpload = null;
                this.documentFlag = false;
            }
        }
    }

    uploadFileToActivity(event?: LazyLoadEvent) {
        if (this.fileToUpload == null || this.fileToUpload == undefined) {
            this.message.error("", this.l("pleaseChooseFile"));
        } else {
            let formDataFile = new FormData();
            formDataFile.append("formFile", this.fileToUpload);
        }
    }

    onChange(event) {
        // let text =convert(this.htmls,options);
        // this.componentBody.text = text.toString();
    }

    textChanged(event) {
        const text = convert(event.html, options);
        if (text.length > 1024) {
            this.textBodyMesage = true;
        } else {
            this.textBodyMesage = false;
        }
    }

    onOk(){
        this.isOptOutMessage =true;
        this.firstSaveFlag = false;
    }

    save(): void {
        if(this.isOptOutMessage == false && this.template.category == 'MARKETING' && !this.optOutButtonFlag){
            this.firstSaveFlag = true;
        }
        else{
            this.saveToApi()
        }
    }

    saveToApi(): void {
        let checkBodyToEachOther = this.check_If_next_To_Each_Other();
        let checkBodyEndOrBegin = this.check_End_Or_beginnig();
        let isValid = this.checkValidText(this.componentBody.text);
        this.saving = true;
        this.submitted = true;
        if (!isValid) {
            this.message.error(this.l("errorInVariableSequrntail"));
            this.saving = false;

            return;
        }

        for (let index = 0; index < this.listOfVariables.length; index++) {
            if (!this.dynamicVariables[index]) {
                this.message.error(this.l("errorInVariableExample"));
                this.saving = false;
                return;
            }
        }

        if (!this.isEdit) {
            debugger;
            if (
                this.textBodyMesage ||
                this.template.language === undefined ||
                this.template.name === undefined ||
                this.template.category === undefined ||
                this.template.language === null ||
                this.template.name === null ||
                this.template.category === null ||
                this.template.language === "" ||
                this.template.name === "" ||
                this.template.category === "" ||
                !this.isValidName ||
                this.isUsed ||
                !this.isTextValid
            ) {
                this.saving = false;
                this.submitted = true;
                return;
            }

            if (
                this.componentHeader.format === null ||
                this.componentHeader.format === undefined ||
                this.componentHeader.format === "" ||
                this.componentBody.text === null ||
                this.componentBody.text === undefined ||
                this.componentBody.text === "" ||
                this.button.type === null ||
                this.button.type === undefined ||
                this.button.type === "" ||
                checkBodyToEachOther ||
                checkBodyEndOrBegin ||
                (this.headerImageFlag &&
                    (this.fileToUpload === null ||
                        this.fileToUpload === undefined)) ||
                (this.headerVideoFlag &&
                    (this.fileToUpload === null ||
                        this.fileToUpload === undefined)) ||
                (this.headerDocumentFlag &&
                    (this.fileToUpload === null ||
                        this.fileToUpload === undefined)) ||
                (this.headerTextFlag &&
                    (this.whatsAppHeaderHandle === null ||
                        this.whatsAppHeaderHandle === undefined)) ||
                (this.textButtonFlag &&
                    (this.button.text === null ||
                        this.button.text === undefined ||
                        this.button.text === "")) ||
                (this.phoneButtonFlag && this.phoneForm.invalid) ||
                (this.urlButtonFlag &&
                    (this.button.url === null ||
                        this.button.url === undefined ||
                        this.button.url === "")) ||
                (this.textButtonFlag &&
                    this.showSecondButton &&
                    (this.buttonTwo.text === null ||
                        this.buttonTwo.text === undefined ||
                        this.buttonTwo.text === ""))
            ) {
                this.saving = false;
                this.submitted = true;
                return;
            }
            if (this.listOfVariables.length > 0) {
                this.variableCount = 0;
                for (var i = 0; i <= 5; i++) {
                    var param = "{{" + i + "}}";
                    if (this.componentBody.text.includes(param)) {
                        this.variableCount++;
                    }
                }
                // this.variableCount = this.listOfVariables.length;
            }
            //Components Validation
            this.template.components = [new WhatsAppComponentModel()];

            //Header Validation
            if (this.componentHeader != null) {
                this.componentHeader.type = "HEADER";

                //---Header Media Validation
                if (
                    this.componentHeader.format == "IMAGE" ||
                    this.componentHeader.format == "VIDEO" ||
                    this.componentHeader.format == "DOCUMENT"
                ) {
                    if (
                        this.fileToUpload == null ||
                        this.fileToUpload == undefined
                    ) {
                        this.message.error(
                            "",
                            this.l("Please Select Media File !")
                        );
                        return;
                    }
                    if (this.componentHeader.format == "IMAGE") {
                        if (
                            this.extFile == "jpg" ||
                            this.extFile == "jpeg" ||
                            this.extFile == "png"
                        ) {
                        } else {
                            this.element.nativeElement.value = "";
                            this.message.error("", this.l("pleaseChooseImage"));
                            this.fileToUpload = null;
                            return;
                        }
                    }

                    if (this.componentHeader.format == "VIDEO") {
                        if (this.extFile != "mp4") {
                            this.element.nativeElement.value = "";
                            this.message.error("", this.l("pleaseChooseVideo"));
                            this.fileToUpload = null;
                            return;
                        }
                    }

                    if (this.componentHeader.format == "DOCUMENT") {
                        if (this.extFile != "pdf") {
                            this.element.nativeElement.value = "";
                            this.message.error("", this.l("pleaseChoosePdf"));
                            this.fileToUpload = null;
                            return;
                        }
                    }
                    this.componentHeader.example = new WhatsAppExampleModel();
                    this.mediaUrl.header_text = [this.whatsAppHeaderHandle.h];
                    this.componentHeader.example = this.mediaUrl;
                    this.template.mediaLink =
                        this.whatsAppHeaderHandle.infoSeedUrl;
                    this.template.mediaType =
                        this.componentHeader.format.toLowerCase();
                    this.template.components.push(this.componentHeader);
                }

                //---Header Text
                if (this.componentHeader.format == "TEXT") {
                    this.template.components.push(this.componentHeader);
                }
            }
        } else {
            if (
                this.componentHeader.format === null ||
                this.componentHeader.format === undefined ||
                this.componentHeader.format === "" ||
                this.componentBody.text === null ||
                this.componentBody.text === undefined ||
                this.componentBody.text === "" ||
                this.button.type === null ||
                this.button.type === undefined ||
                this.button.type === "" ||
                checkBodyToEachOther ||
                checkBodyEndOrBegin ||
                (this.textButtonFlag &&
                    (this.button.text === null ||
                        this.button.text === undefined ||
                        this.button.text === "")) ||
                (this.phoneButtonFlag && this.phoneForm.invalid) ||
                (this.urlButtonFlag &&
                    (this.button.url === null ||
                        this.button.url === undefined ||
                        this.button.url === "")) ||
                (this.textButtonFlag &&
                    this.showSecondButton &&
                    (this.buttonTwo.text === null ||
                        this.buttonTwo.text === undefined ||
                        this.buttonTwo.text === "")) ||
                !this.isTextValid
            ) {
                this.saving = false;
                this.submitted = true;
                return;
            }
        }
        if (this.listOfVariables.length > 0) {
            this.variableCount = 0;
            for (var i = 0; i <= 5; i++) {
                var param = "{{" + i + "}}";
                if (this.componentBody.text.includes(param)) {
                    this.variableCount++;
                }
            }
            // this.variableCount = this.listOfVariables.length;
        }

        //Components Validation
        this.template.components = [new WhatsAppComponentModel()];

        //header
        if (this.componentHeader != null) {
            this.componentHeader.type = "HEADER";

            //---Header Media Validation
            this.componentHeader.example = new WhatsAppExampleModel();
            if (
                this.componentHeader.format == "IMAGE" ||
                this.componentHeader.format == "VIDEO" ||
                this.componentHeader.format == "DOCUMENT"
            ) {
                if (!this.Template.mediaLink) {
                    if (
                        this.fileToUpload == null ||
                        this.fileToUpload == undefined
                    ) {
                        this.message.error(
                            "",
                            this.l("Please Select Media File !")
                        );
                        this.saving = false;
                        return;
                    }

                    if (this.componentHeader.format == "IMAGE") {
                        if (
                            this.extFile == "jpg" ||
                            this.extFile == "jpeg" ||
                            this.extFile == "png"
                        ) {
                        } else {
                            this.element.nativeElement.value = "";
                            this.message.error("", this.l("pleaseChooseImage"));
                            this.fileToUpload = null;
                            this.saving = false;
                            return;
                        }
                    }

                    if (this.componentHeader.format == "VIDEO") {
                        if (this.extFile != "mp4") {
                            this.element.nativeElement.value = "";
                            this.message.error("", this.l("pleaseChooseVideo"));
                            this.fileToUpload = null;
                            this.saving = false;
                            return;
                        }
                    }

                    if (this.componentHeader.format == "DOCUMENT") {
                        if (this.extFile != "pdf") {
                            this.element.nativeElement.value = "";
                            this.message.error("", this.l("pleaseChoosePdf"));
                            this.fileToUpload = null;
                            this.saving = false;
                            return;
                        }
                    }
                }

                this.componentHeader.example = new WhatsAppExampleModel();
                const header_handle = this.whatsAppHeaderHandle.h
                    ? this.whatsAppHeaderHandle.h
                    : this.Template.components[0]?.example?.header_text[0];
                this.mediaUrl.header_text = [header_handle];

                this.componentHeader.example = this.mediaUrl;
                this.template.mediaLink = this.whatsAppHeaderHandle.infoSeedUrl
                    ? this.whatsAppHeaderHandle.infoSeedUrl
                    : this.Template.mediaLink;
                this.template.mediaType =
                    this.componentHeader.format.toLowerCase();
                this.template.components.push(this.componentHeader);
            }

            //---Header Text
            if (this.componentHeader.format == "TEXT") {
                this.componentHeader.example = null;
                this.template.components.push(this.componentHeader);
            }
        }

        //--- Body
        this.componentBody.type = "BODY";
        if (this.listOfVariables.length > 0) {
            this.exampleBody = new WhatsAppExampleModel();
            this.exampleBody.body_text = [[]];
            for (let index = 0; index < this.listOfVariables.length; index++) {
                this.exampleBody.body_text[0].push(
                    this.dynamicVariables[index].toString().trim()
                    // "Variable" + index.toString()
                );
            }
            this.componentBody.example = this.exampleBody;
        } else {
            delete this.componentBody?.example;
        }

        this.template.components.push(this.componentBody);
        this.template.components.splice(0, 1);

        //--- Footer
        if (
            this.footer === null ||
            this.footer === undefined ||
            this.footer === ""
        ) {
            this.footer = "";
        } else {
            this.componentFooter.text = this.footer;
            this.componentFooter.type = "FOOTER";
            this.template.components.push(this.componentFooter);
        }

        //--- Button Validation
        if (this.componentButton != null) {
            this.componentButton.type = "BUTTONS";
            this.componentButton.buttons = [new WhatsAppButtonModel()];
            if (this.button.type === "NONE") {
                this.button.text = null;
            }
            //--- Button Text
            if (
                this.button.text != null &&
                this.button.phone_number == null &&
                this.button.url == null
            ) {
                this.button.type = "QUICK_REPLY"
                this.componentButton.buttons.push(this.button);
                if (this.showSecondButton && this.buttonTwo.text != null) {
                    this.buttonTwo.type = "QUICK_REPLY";
                    this.componentButton.buttons.push(this.buttonTwo);
                }
                this.componentButton.buttons.splice(0, 1);
                this.template.components.push(this.componentButton);
            }
            //--- Button Phone Number Validation
            if (this.phoneButtonFlag) {
                if (
                    this.button.phone_number == null ||
                    this.button.phone_number == undefined
                ) {
                    this.message.error("", this.l("Please Enter Phone Number"));
                    return;
                }
                if (this.phoneForm.status != "VALID") {
                    this.button.phone_number = null;
                    this.phoneForm.value.phone = null;
                    return;
                }
            }
            if (
                this.button.text != null &&
                this.button.phone_number != null &&
                this.button.url == null
            ) {
                this.componentButton.buttons.push(this.button);
                this.componentButton.buttons.splice(0, 1);
                this.template.components.push(this.componentButton);
            }
            //--- Button URL
            if (
                this.button.text != null &&
                this.button.phone_number == null &&
                this.button.url != null &&
                this.button.url.startsWith("https://")
            ) {
                this.componentButton.buttons.push(this.button);
                this.componentButton.buttons.splice(0, 1);
                this.template.components.push(this.componentButton);
            }
        }
        this.template.variableCount = this.variableCount;
        // this.renderQuillContent(this.componentBody.text);

        if (this.isEdit) {
            this._whatsAppMessageTemplateServiceProxy
                .updateTemplate(this.appSession.tenantId, this.template)
                .pipe(
                    finalize(() => {
                        this.saving = false;
                    })
                )
                .subscribe((result) => {
                    if (result.success) {
                        this.notify.info(this.l("SavedSuccessfully"));
                        this.saving = false;
                        this.submitted = false;
                        this.close();
                    } else {
                        this.notify.error(result.error.error_user_msg);
                        this.saving = false;
                        this.submitted = false;
                    }
                });
        } else {
            this._whatsAppMessageTemplateServiceProxy
                .addWhatsAppMessageTemplate(null, this.template)
                .pipe(
                    finalize(() => {
                        this.saving = false;
                    })
                )
                .subscribe((result) => {
                    if (result.id != null) {
                        this.notify.info(this.l("SavedSuccessfully"));
                        //this.message.success("",this.l("Please Enter Phone Number"));
                        //this.message.success
                        this.saving = false;
                        this.submitted = false;
                        this.close();

                        this.modalSave.emit(null);
                    } else {
                        this.message.error(
                            result.error.error_user_title,
                            result.error.error_user_msg
                        );
                        document
                            .getElementById("btn")
                            .removeAttribute("disabled");
                        //this.close();
                        //this.modalSave.emit(null);
                    }
                });
        }
    }

    close(): void {
        this.dynamicVariables = [];
        this.listOfVariables = [];
        this.headerDocumentFlag = false;
        this.headerVideoFlag = false;
        this.headerImageFlag = false;
        this.headerTextFlag = false;

        this.buttonFlag = false;
        this.textButtonFlag = false;
        this.phoneButtonFlag = false;
        this.urlButtonFlag = false;
        this.optOutButtonFlag = false;
        this.template = new MessageTemplateModel();

        this.active = false;
        this.submitted = false;
        this.saving = false;
        this.modal.hide();
        this.variableCount = 0;
        this.modalSave.emit(null);
        this.verticalWizardStepper.previous();
    }

    // private renderQuillContent(content: string): string {
    //     const quillContent = new Quill(document.createElement("div"));
    //     quillContent.clipboard.dangerouslyPasteHTML(content);

    //     const delta = quillContent.getContents();
    //     const ops = delta.ops.map((op) => {
    //         if (op.insert && typeof op.insert === "string") {
    //             op.insert = op.insert.replace(
    //                 /<i class="quill-icon">(.+?)<\/i>/g,
    //                 "[$1]"
    //             );
    //         }
    //         return op;
    //     });

    //     const newDelta = new Quill.QuillDelta(ops);
    //     const renderer = new Quill.Quill("").renderer;
    //     const quillHtml = renderer.render(newDelta);

    //     return quillHtml;
    // }
    checkHeaderFormat(format: any) {
        if (format == "NONE") {
            this.fileToUpload = null;
            this.headerDocumentFlag = false;
            this.headerVideoFlag = false;
            this.headerImageFlag = false;
            this.headerTextFlag = false;
            this.componentHeader.text = "";

            this.Template.mediaLink = "";
        }
        if (format == "TEXT") {
            this.componentHeader.example = null;
            this.fileToUpload = null;
            this.headerDocumentFlag = false;
            this.headerVideoFlag = false;
            this.headerImageFlag = false;
            this.headerTextFlag = true;
            this.Template.mediaLink = "";
        }
        if (format == "IMAGE") {
            this.fileToUpload = null;
            this.headerDocumentFlag = false;
            this.headerVideoFlag = false;
            this.headerImageFlag = true;
            this.headerTextFlag = false;
            this.componentHeader.text = "";
            this.Template.mediaLink = "";
        }
        if (format == "VIDEO") {
            this.fileToUpload = null;
            this.headerDocumentFlag = false;
            this.headerVideoFlag = true;
            this.headerImageFlag = false;
            this.headerTextFlag = false;
            this.componentHeader.text = "";
            this.Template.mediaLink = "";
        }
        if (format == "DOCUMENT") {
            this.fileToUpload = null;
            this.headerDocumentFlag = true;
            this.headerVideoFlag = false;
            this.headerImageFlag = false;
            this.headerTextFlag = false;
            this.componentHeader.text = "";
            this.Template.mediaLink = "";
        }
    }
    checkSelection() {
        const textarea = this.myTextarea.nativeElement;
        const selectedText = textarea.value.substring(
            textarea.selectionStart,
            textarea.selectionEnd
        );

        if (selectedText.length > 0) {
            return true;
            // Add your logic here for when text is selected in the textarea.
        } else {
            return false;
            // Add your logic here for when no text is selected in the textarea.
        }
    }

    boldText() {
        if (this.componentBody.text && this.checkSelection()) {
            const textarea = document.getElementById(
                "bodyText"
            ) as HTMLTextAreaElement;
            const startPosition = textarea.selectionStart;
            const endPosition = textarea.selectionEnd;
            const textBefore = textarea.value.substring(0, startPosition);
            const selectedText = textarea.value.substring(
                startPosition,
                endPosition
            );
            const textAfter = textarea.value.substring(
                endPosition,
                textarea.value.length
            );
            const modifiedText = `${textBefore}*${selectedText}*${textAfter}`;
            this.componentBody.text = modifiedText;
        } else {
            const textarea = this.myTextarea.nativeElement;
            const stars = "**";
            const cursorPosition = textarea.selectionStart || 0;
            const text = textarea.value;
            const newText =
                text.slice(0, cursorPosition) +
                stars +
                text.slice(cursorPosition);
            textarea.value = newText;
            textarea.setSelectionRange(cursorPosition + 1, cursorPosition + 1);
            textarea.focus();
        }
        this.formatText();
    }

    italicText() {
        if (this.componentBody.text && this.checkSelection()) {
            const textarea = document.getElementById(
                "bodyText"
            ) as HTMLTextAreaElement;
            const startPosition = textarea.selectionStart;
            const endPosition = textarea.selectionEnd;
            const textBefore = textarea.value.substring(0, startPosition);
            const selectedText = textarea.value.substring(
                startPosition,
                endPosition
            );
            const textAfter = textarea.value.substring(
                endPosition,
                textarea.value.length
            );
            const modifiedText = `${textBefore}_${selectedText}_${textAfter}`;
            this.componentBody.text = modifiedText;
        } else {
            const textarea = this.myTextarea.nativeElement;
            const stars = "__";
            const cursorPosition = textarea.selectionStart || 0;
            const text = textarea.value;
            const newText =
                text.slice(0, cursorPosition) +
                stars +
                text.slice(cursorPosition);
            textarea.value = newText;
            textarea.setSelectionRange(cursorPosition + 1, cursorPosition + 1);
            textarea.focus();
        }
        this.formatText();
    }
    checkButtonType(type: any) {
        if (type == "NONE") {
            this.buttonFlag = false;
            this.textButtonFlag = false;
            this.phoneButtonFlag = false;
            this.urlButtonFlag = false;
            this.phoneForm.value.phone = null;
            this.phoneForm.reset();
            this.button.phone_number = null;
            this.button.text = null;
            this.button.text = ""
            this.componentButtonForView.buttons[0].text = "";
            this.componentButtonForView.buttons[1].text = "";
        }
        if (type == "QUICK_REPLY") {
            this.buttonFlag = true;
            this.textButtonFlag = true;
            this.phoneButtonFlag = false;
            this.urlButtonFlag = false;
            this.phoneForm.value.phone = null;
            this.phoneForm.reset();
            this.button.phone_number = null;
            // this.button.text= null;
            this.optOutButtonFlag = false;
            // this.button.text = ""
            this.componentButtonForView.buttons[0].text = "";
            this.componentButtonForView.buttons[1].text = "";
        }
        if (type == "PHONE_NUMBER") {
            this.buttonFlag = true;
            this.textButtonFlag = true;
            this.phoneButtonFlag = true;
            this.urlButtonFlag = false;
            this.showSecondButton = false;
            this.button.text = null;
            this.optOutButtonFlag = false;
            this.button.text = ""
            this.componentButtonForView.buttons[0].text = "";
            this.componentButtonForView.buttons[1].text = "";
        }
        if (type == "URL") {
            this.buttonFlag = true;
            this.textButtonFlag = true;
            this.phoneButtonFlag = false;
            this.urlButtonFlag = true;
            this.showSecondButton = false;
            this.phoneForm.value.phone = null;
            this.phoneForm.reset();
            this.button.phone_number = null;
            this.button.text = null;
            this.optOutButtonFlag = false;
            this.button.text = ""
            this.componentButtonForView.buttons[0].text = "";
            this.componentButtonForView.buttons[1].text = "";
        }
        if (type == "Opt-out") {
            this.buttonFlag = false;
            this.textButtonFlag = true;
            this.phoneButtonFlag = false;
            this.urlButtonFlag = false;
            this.phoneForm.value.phone = null;
            this.phoneForm.reset();
            this.button.phone_number = null;
            this.optOutButtonFlag = true;
            
            //button Stop Init
            if(this.template.language == "en"){
                this.button.text = "Stop promotions"
                this.componentButtonForView.buttons[0].text = "Stop promotions";
                this.componentButtonForView.buttons[1].text = "";
            }
            else{
                this.button.text = "ايقاف عمليات الترويج"
                this.componentButtonForView.buttons[0].text = "ايقاف عمليات الترويج";
                this.componentButtonForView.buttons[1].text = "";
            }
        }
    }
    isValidText(text) {
        // Regular expression to match at least three words or a space followed by two words
        const regex = /^(?=(?:\S*\s\S*\s\S*\s*)|$)(?!\s*$).*$/;

        const trimmedString = text?.replace(/\s+$/, "");

        return regex.test(trimmedString);
    }

    parseDynamicText(
        text: string
    ): { type: "text" | "input"; content: string; index?: number }[] {
        debugger
    
        let newText = "";
        const parts: {
            type: "text" | "input";
            content: string;
            index?: number;
        }[] = [];
        let currentIndex = 0;
        this.isTextValid = true;
        text.replace(/{{(\d+)}}/g, (match, index, offset) => {
            if (offset > currentIndex) {
                parts.push({
                    type: "text",
                    content: text.substring(currentIndex, offset),
                });
            }

            const inputIndex = parseInt(index, 10) - 1;
            parts.push({ type: "input", content: match, index: inputIndex });

            currentIndex = offset + match.length;
            return match;
        });

        if (currentIndex < text.length) {
            parts.push({ type: "text", content: text.substring(currentIndex) });
        }
        for (let index = 0; index < parts.length; index++) {
            if (parts[index].type === "input") {
                if (!this.isValidText(parts[index + 1]?.content)) {
                    this.isTextValid = false;
                    break;
                }
            }
        }
        setTimeout(() => {
            this.checkVariable(parts);
        }, 2000);
        this.formatText();
        return parts;
    }

    checkValidText(text: string): boolean {
        let parts = this.parseDynamicText(text);
        let lengthOfVariables = parts.filter((part) => part.type == "input");

        if (this.listOfVariables.length !== lengthOfVariables.length)
            return false;

        for (let i = 0; i < this.listOfVariables.length; i++)
            if (lengthOfVariables[i].content !== `{{${i + 1}}}`) return false;

        return true;
    }

    changeFooter(lang: any) {
        if (lang == "en") {
            this.footer = "";
            this.predefinedStop = "Stop promotions"
            this.componentFooter.text = this.footer;
        }
        if (lang == "ar") {
            this.footer = "";
            this.componentFooter.text = this.footer;
            this.predefinedStop = "ايقاف عمليات الترويج"
        }
    }

    // Hassan

    checkVariable(parts: any) {
        // this.listOfVariables = [];
        const regex = /\{\{(\d*)\}\}/g;

        let holderText = this.componentBody.text;
        let matches = this.componentBody.text.match(regex);
        if (!matches) return;

        // Initialize the counter for ascending variables

        matches.forEach((match, index) => {
            const randomNumber = Math.floor(Math.random() * (1000 - 5 + 1)) + 5;
            holderText = holderText.replace(match, `{{${randomNumber}}}`);
        });

        matches = holderText.match(regex);

        matches.forEach((match, index) => {
            if (index > 4) {
                holderText = holderText.replace(match, "");
            } else {
                holderText = holderText.replace(match, `{{${++index}}}`);
            }
        });

        this.listOfVariables.length = holderText.match(regex).length;

        this.componentBody.text = holderText;
    }

    verticalWizardNext() {
        if (
            this.template.language === undefined ||
            this.template.name === undefined ||
            this.template.category === undefined ||
            this.template.language === null ||
            this.template.name === null ||
            this.template.category === null ||
            this.template.language === "" ||
            this.template.name === "" ||
            this.template.category === "" ||
            !this.isValidName ||
            this.isUsed
        ) {
            this.submitted = true;
            return;
        }
        this.submitted = false;
        this.IsStep2 = true;
        this.verticalWizardStepper.next();
    }
    verticalWizardPrevious() {
        this.verticalWizardStepper.previous();
    }
    // onEditorCreated(editor: any) {
    //     this.quillEditor = editor;
    // }

    addVariableOne() {
        debugger
        if (this.componentBody.text) {
            if (this.listOfVariables.length < 5) {
                this.listOfVariables.length += 1;
                this.componentBody.text +=
                    " {{" + this.listOfVariables.length + "}} . .";
                this.parseDynamicText(this.componentBody.text);
            } else {
                return;
            }
        }
    }

    incrementedIndex(index: number): number {
        return index + 1;
    }

    deleteVaribale() {
        if (this.componentBody.text) {
            if (this.listOfVariables.length > 0) {
                const pattern = new RegExp(
                    "\\{\\{" + this.listOfVariables.length + "\\}} \\. \\.",
                    "g"
                );

                const isPatternPresent = pattern.test(this.componentBody.text);

                if (isPatternPresent) {
                    this.componentBody.text = this.componentBody.text.replace(
                        pattern,
                        ""
                    );
                } else {
                    this.componentBody.text = this.componentBody.text.replace(
                        " {{" + this.listOfVariables.length + "}}",
                        ""
                    );
                }
                this.listOfVariables.pop();
                this.parseDynamicText(this.componentBody.text);
            } else {
                return;
            }
        }
    }

    addEmoji(event) {
        this.componentBody.text += event.emoji.native;
        // this.hideEmoji = true;
    }

    showEmoji() {
        this.hideEmoji = !this.hideEmoji;
    }

    check_If_next_To_Each_Other() {
        let text = convert(this.componentBody.text, options);
        let stringText = text.toString();

        if (stringText) {
            return stringText.includes("}}{{") || stringText.includes("}} {{");
        } else {
            return false;
        }
    }

    check_End_Or_beginnig() {
        let text = convert(this.componentBody.text, options);
        let stringText = text.toString();
        if (stringText) {
            return (
                stringText.endsWith("}}") ||
                stringText.startsWith("{{") ||
                stringText.endsWith("}} ") ||
                stringText.startsWith(" {{")
            );
        } else {
            return false;
        }
    }
    // handleKeyDown(event: KeyboardEvent): void {
    //     // Check if the pressed key is the delete key (46) or backspace key (8)
    //     if (event.keyCode === 46 || event.keyCode === 8) {
    //       // Call your function or perform the desired action
    //       console.log('Delete or Backspace key pressed');
    //     }
    //   }

    created(event) {
        // tslint:disable-next-line:no-console
    }

    changedEditor(event: EditorChangeContent | EditorChangeSelection) {
        // tslint:disable-next-line:no-console
    }

    focus($event) {
        // tslint:disable-next-line:no-console
        this.focused = true;
        this.blured = false;
    }

    blur($event) {
        // tslint:disable-next-line:no-console
        this.focused = false;
        this.blured = true;
    }

    handleOnChangeText(event: Event) {
        const inputElement = event.target as HTMLInputElement;
        this.footer = inputElement.value;
        this.componentFooter.text = this.footer;
    }

    formatText() {
        // Step 1: Normalize bold markers
        if (this.componentBody?.text) {
            let textWithBold = this.componentBody.text.replace(
                /\*{1,}(.*?)\*{1,}/g,
                "<strong>$1</strong>"
            );

            // Step 2: Normalize underline markers
            let textWithUnderline = textWithBold.replace(
                /_{1,}(.*?)_{1,}/g,
                "<u>$1</u>"
            );

            // Step 3: Apply the formatting to the text
            this.formattedText = textWithUnderline;
        } else {
            this.formattedText = "";
        }

        this.formattedTextBody = this.sanitizer.bypassSecurityTrustHtml(
            this.formattedText
        );
    }

    handleButtonChange(event: Event) {
        const input = event.target as HTMLInputElement;
        this.componentButtonForView.buttons[0].text = input.value;
    }

    handleButton2Change(event: Event) {
        const input = event.target as HTMLInputElement;
        this.componentButtonForView.buttons[1].text = input.value;
    }

    onTemplateNameChange(event: Event){
        const input = event.target as HTMLInputElement;
        this.template.name = input.value.replace(/\s+/g, '_').toLowerCase();
        if(input.value.length <= 0){
            this.IsStep2 = false;
        }
    }
}
