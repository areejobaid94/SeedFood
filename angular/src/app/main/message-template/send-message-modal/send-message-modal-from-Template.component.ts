import { HttpClient } from "@angular/common/http";
import {
    Component,
    ElementRef,
    Renderer2,
    EventEmitter,
    Injector,
    Output,
    ViewChild,
    Input,
    HostListener,
} from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/common/app-component-base";

import { Observable, Subject } from "rxjs";
import {
    debounceTime,
    distinctUntilChanged,
    switchMap,
} from "rxjs/operators";

import {
    ContactDto,
    ContactsServiceProxy,
    MessageTemplateModel,
    TeamInboxDto,
    TeamInboxServiceProxy,
    TemplateVariables,
    WhatsAppButtonModel,
    WhatsAppComponentModel,
    WhatsAppContactsDto,
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
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import {
    EditorChangeContent,
    EditorChangeSelection,
} from "ngx-quill";
import "quill-emoji/dist/quill-emoji.js";
const { compile } = require("html-to-text");
const { convert } = require("html-to-text");
import { DatePipe } from "@angular/common";

import { NgSelectComponent } from "@ng-select/ng-select";
import { ContactSelectDTO } from "@app/main/teamInbox/contactSelect";
import { LazyLoadEvent } from "primeng/api";

const options = {
    wordwrap: 130,
    // ...
};

@Component({
    selector: "send-message-modal-from-Template",
    templateUrl: "./send-message-modal-from-Template.component.html",
    styleUrls: ["./send-message-modal-from-Template.component.css"],
})
export class SendMessageModalFromTeamplatComponent extends AppComponentBase {
    @ViewChild(NgSelectComponent, { static: false })
    ngSelect: NgSelectComponent;

    localCust: ContactSelectDTO[] = [];
    searchUser: string = null;
    whatsAppName: string = "";
    contactsLocal$: any;

    items: any[] = [];
    isLoading = true;
    currentPage = 0;
    itemsPerPage = 10;

    @Input() template: MessageTemplateModel[];
    listOfVariable: string[] = null;
    isSubmitted: boolean = false;
    savingNext: boolean = false;

    combinedValue: string = "";
    templateById: MessageTemplateModel = null;

    templatePrevious: MessageTemplateModel;
    VariableCount: number = 0;
    contacts: ContactDto[];
    private http: HttpClient;
    theme: string;
    private verticalWizardStepper: Stepper;
    countryObj: any = null;
    separateDialCode = false;
    SearchCountryField = SearchCountryField;
    CountryISO = CountryISO;
    contact: WhatsAppContactsDto = new WhatsAppContactsDto();

    TemplateId: string;
    PhoneNumberFormat = PhoneNumberFormat;
    searchContact: string = "";
    nextPage: boolean ;

    inputs: { value: string }[] = [];
    dynamicTextParts: {
        type: "text" | "input";
        content: string;
        index?: number;
    }[] = [];
    preferredCountries: CountryISO[] = [
        CountryISO.SaudiArabia,
        CountryISO.Jordan,
    ];
    showSecondButton = false;
    showFirstButton = true;
    submitted = false;
    content = "";
    inputData = "";
    searchPhone = "";
    hideEmoji = true;
    imageFlag: boolean = false;
    videoFlag: boolean = false;
    documentFlag: boolean = false;
    displayText: string = "";
    Component: WhatsAppComponentModel[] = [new WhatsAppComponentModel()];
    ComponentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButton: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButtonText: any;
    isValidMobileNumber: boolean = null;
    mobileNumber: string = "";
    currentPageNumber: number = 0;
    currentPageSize: number = 20;
    customerApi$: Observable<any>;
    customerApiLoading = false;
    customerApiInput$ = new Subject<string>();
    selectedCustomer: any;
    minLengthTerm = 3;
    loading = false;
    selectedPersonId: number;
    isModalOpen = false;
    @ViewChild("sendMessageTeamInbox3", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("file")
    element: ElementRef;
    templateId: number = 9;
    @ViewChild("name")
    name: ElementRef;
    active = false;
    inputCountFromBackend: number = 0;
    saving = false;
    nextbtn = false;
    customeexampleBody: string[] = [];
    isEdit: boolean;
    isUsed: boolean = false;
    dateAndTime: Date = new Date();
    minDate: Date = new Date();
    selectedContact: ContactSelectDTO;
    createdOrGotContact: ContactSelectDTO;
    isSchedule: boolean = false;
    isExternal: boolean = false;
    isScheduleBtn: boolean = false;
    checkTemplate: MessageTemplateModel[] = [new MessageTemplateModel()];
    componentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
    componentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
    componentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
    footer: string = "";
    mediaUrl: WhatsAppExampleModel = new WhatsAppExampleModel();
    exampleBody: WhatsAppExampleModel = new WhatsAppExampleModel();
    button: WhatsAppButtonModel = new WhatsAppButtonModel();
    buttonTwo: WhatsAppButtonModel = new WhatsAppButtonModel();
    componentButton: WhatsAppComponentModel = new WhatsAppComponentModel();
    whatsAppHeaderHandle: WhatsAppHeaderUrl = new WhatsAppHeaderUrl();
    fileToUpload: any;
    codeAndMobile: string = "";
    tempFile: any;
    selectedUser: any;
    textBodyMesage = false;
    countryCode: string;
    languageEnum = [
        { id: "en", name: "English" },
        { id: "ar", name: "Arabic" },
    ];

    htmls = [];

    regexp = new RegExp("^[a-zA-Z0-9_.-]*$");
    isValidName = true;
    blured = false;
    focused = false;
    isArabic = false;
    @Output() getTemplateList: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('modal', { static: false }) modalElement!: ElementRef;

      @Input() sendMessage: (model: MessageTemplateModel) => void;

    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        http: HttpClient,
        public darkModeService: DarkModeService,
        private renderer: Renderer2,
        private _contactsServiceProxy: ContactsServiceProxy,
        private datePipe: DatePipe,
        private teamInbox: TeamInboxServiceProxy,
        private eRef: ElementRef
    ) {
        super(injector);
        this.http = http;
    }


    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.templateById = null;
        // this.isArabic = rtlDetect.isRtlLang(
        //     abp.localization.currentLanguage.name
        // );

        this.verticalWizardStepper = new Stepper(
            document.querySelector("#stepper3"),
            {
                linear: false,
                animation: true,
            }
        );
    }

    checkPattern(text: string): boolean {
        const pattern = /\{\{\d+\}\}/g;
        return pattern.test(text);
    }

    getExampleBody(example: string[]) {
        this.customeexampleBody = example;
    }

    loadcustomerApi() {
        this.customerApiInput$
            .pipe(
                debounceTime(800),
                distinctUntilChanged(),
                switchMap((input: string) => {
                    this.searchUser = input?.length == 0 ? null : input;
                    this.customerApiLoading = true;
                    return this.teamInbox.contactsGetAll(input, 0, 10);
                })
            )
            .subscribe(({ message }) => {
                this.localCust = message;
                this.customerApiLoading = false;
            });
    }

    trackByFn(item: any) {
        return item.contactId;
    }

    preventNonNumeric(event: KeyboardEvent): void {
        if (
            !(
                (event.key >= "0" && event.key <= "9") ||
                event.key === "Backspace" ||
                event.key === "Delete"
            )
        ) {
            event.preventDefault();
        }
    }

    handleIsSchedule(newValue: Event) {
        this.minDate = new Date();
        this.minDate.setMinutes(this.minDate.getMinutes() + 2);

        this.dateAndTime = new Date();
        this.dateAndTime.setMinutes(this.dateAndTime.getMinutes() + 2);
        this.isSchedule = !this.isSchedule;
    }

    handleIsExternal(newValue: Event) {
        this.isExternal = !this.isExternal;
        this.mobileNumber = "";
        this.whatsAppName = "";
    }

    // onContactSelect(user: any): void {

    //   this.verticalWizardNext()
    // }

    onKeyPress(event: any, value: any) {
        const forbiddenKey = ["$"];
        const keyPressed = event.key;
        if (forbiddenKey.includes(keyPressed)) {
            event.preventDefault();
        }
    }

    updateText() {
        this.displayText = "";
        // Replace placeholders with input values, or revert to placeholders if input is empty
        this.dynamicTextParts.forEach((part) => {
            if (part.type === "input") {
                part.content =
                    this.inputs[part.index].value || `{{${part.index + 1}}}`;
            }
        });
        this.displayText = this.dynamicTextParts
            .map((item) => item.content)
            .join("");
        this.ComponentBody.text = this.displayText;
    }

    getContacts(search: string, pageNumber: number = 0, pageSize: number = 20) {
        this.teamInbox
            .contactsGetAll(search, pageNumber, pageSize)
            .subscribe((res) => {
                this.localCust = res.message;
            });
    }

    loadMoreItems(pageNumber: number = 0, pageSize: number = 20) {
        if (!this.loading) {
            this.loading = true;
            this.teamInbox
                .contactsGetAll(this.searchUser, pageNumber, pageSize)
                .subscribe({
                    next: ({ message }) => {
                        this.localCust = [...this.localCust, ...message];
                        this.loading = false;
                    },
                    error: (err) => {
                        this.loading = false;
                    },
                });
        }
    }

    onScroll() {
        const nextPageNumber = this.currentPageNumber + 1;
        this.loadMoreItems(nextPageNumber * 10, 10);
        this.currentPageNumber = nextPageNumber;
        this.currentPageSize = 20;
    }

    show(template: MessageTemplateModel): void {
        this.templatePrevious=template;
              if(this.nextPage){
            this.nextPage=false;
            this.verticalWizardPrevious();
            this.sendMessage(template); 
        }
        if(template!=null){

            //this.templateById=model;
            this.getTemplateById(template);
        }

        if (this.localCust.length <= 0) {
            
            this.getContacts(this.searchUser, 0, 10);
            this.loadcustomerApi();
        }
        this.selectedContact = null;
        this.modal.show();
        this.isModalOpen = true;
    }

    createButtonTwo() {
        this.showSecondButton = true;
    }

    deleteButtonOne() {
        this.showFirstButton = false;
    }

    deleteButtonTwo() {
        this.showSecondButton = false;
        this.buttonTwo.text = "";
    }

    getMessageTemplate() {
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppMessageTemplate()
            .subscribe((result) => {

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
        this.checkTemplate.forEach((element) => {
            if (element.name == name.toLowerCase().split(" ").join("_")) {
                this.message.error("", this.l("thisNameAlreadyUsed"));
                this.isUsed = true;
                return;
            }
        });

        this.isUsed = false;
        return this.isUsed;
    }
    checkTemplateButtonUrl() {
        if (!this.button.url.startsWith("https://")) {
            this.message.error("", this.l("urlNotValid"));
            this.button.url = null;
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
                this.whatsAppHeaderHandle = result;
            });
    }

    uploadFileToActivity(event?: LazyLoadEvent) {
        if (this.fileToUpload == null || this.fileToUpload == undefined) {
            this.message.error("", this.l("pleaseChooseFile"));
        } else {
            let formDataFile = new FormData();
            formDataFile.append("formFile", this.fileToUpload);
        }
    }

    textChanged(event) {
        const text = convert(event.html, options);
        if (text.length > 1024) {
            this.textBodyMesage = true;
        } else {
            this.textBodyMesage = false;
        }
    }

    handleSend(Schedule: boolean = false) {
        debugger
        this.minDate = new Date();
        this.isSubmitted = true;
        let templateVariables: TemplateVariables;
        let sendTime: string;
        this.isScheduleBtn = null;
        this.isScheduleBtn = Schedule ? true : false;

        if (this.VariableCount > 0) {
            for (const item of this.inputs) {
                if (item.value.length === 0) return;
            }
            let arr = this.dynamicTextParts.filter(
                (item) => item.type == "input"
            );

            templateVariables = new TemplateVariables({
                varOne: arr[0]?.content || null,
                varTwo: arr[1]?.content || null,
                varThree: arr[2]?.content || null,
                varFour: arr[3]?.content || null,
                varFive: arr[4]?.content || null,
            });
        } else {
            templateVariables = null;
        }

        if (!this.templateById) {
            return;
        }
        if (Schedule) {
            if (this.dateAndTime < new Date()) {
                this.dateAndTime = new Date();
                this.dateAndTime.setMinutes(this.dateAndTime.getMinutes() + 2);
            }

            sendTime = this.datePipe
                .transform(this.dateAndTime, "yyyy-MM-dd HH:mm")
                .toString();
        } else {
            sendTime = null;
        }

        // HAssan Abed

        // let teamInboxDto = new TeamInboxDto({
        //     templateId: this.templateById.localTemplateId,
        //     contactName: this.isExternal
        //         ? this.createdOrGotContact.displayName
        //         : this.whatsAppName,
        //     phoneNumber: this.isExternal
        //         ? this.createdOrGotContact.userId.split("_")[1]
        //         : this.contact.countryCode + this.mobileNumber,
        //     campaignStatus: Schedule ? 2 : 1,
        //     language: this.templateById.language,
        //     customerOPT: 0,
        //     sendTime,
        //     templateVariables,
        //     isExternal: !this.isExternal,
        //      headerVariabllesTemplate:templateHeaderVariables ,
        //     firstButtonURLVariabllesTemplate:this.firstButtonURLVariabllesTemplate,
        //     secondButtonURLVariabllesTemplate:this.secondButtonURLVariabllesTemplate,
        //     carouselTemplate: carouselTemplate ,
        //     buttonCopyCodeVariabllesTemplate:null
        // });
        this.saving = true;
        // this.teamInbox.sendCampign(teamInboxDto).subscribe(
        //     (result) => {
        //         if (result?.state === -1)
        //             this.notify.error(this.l("ErrorFromServer"));
        //         if (result?.state === 1)
        //             this.notify.error(this.l("notHaveEnoughFunds"));
        //         if (result?.state === 2)
        //             this.notify.success(this.l("sentSuccefully"));
        //         if (result?.state === 3)
        //             this.notify.warn(this.l("haveActiveCamp"));
        //         if (result?.state === 4)
        //             this.notify.error(this.l("notValidDate"));
        //         if (result?.state === 5)
        //             this.notify.error(this.l("reachlimit"));
        //         if (result?.state === 6)
        //             this.notify.warn(this.l("invalidTenant"));
        //         if (result?.state === 7)
        //             this.notify.error(this.l("contactAlreadyexits"));
        //         if (result?.state === 8)
        //             this.notify.error(this.l("invalidFormat"));
        //         if (result?.state === 9)
        //             this.notify.error(this.l("templateNotApporved"));
        //         if (result?.state === 10)
        //             this.notify.error(this.l("theContactisOptOut"));

        //         this.saving = false;
        //         this.templateById = null;
        //         this.selectedContact = null;
        //         this.createdOrGotContact = null;
        //         this.isSubmitted = false;
        //         this.minDate = new Date();
        //         this.dateAndTime = new Date();
        //         this.close();
        //         this.isModalOpen = false;
        //     },
        //     (err) => {
        //         this.notify.error(this.l("ErrorFromServer"));
        //         this.saving = false;
        //         this.templateById = null;
        //         this.selectedContact = null;
        //         this.createdOrGotContact = null;
        //         this.isSubmitted = false;
        //         this.minDate = new Date();
        //         this.dateAndTime = new Date();
        //         this.close();
        //     }
        // );
    }

    close(): void {
        this.minDate = new Date();
        this.dateAndTime = new Date();
        this.listOfVariable = [];
        this.templateById = null;
        this.whatsAppName = "";
        this.mobileNumber = "";
        this.nextbtn = false;
        this.selectedContact = null;
        this.isExternal = false;
        this.isSubmitted = false;
        this.verticalWizardStepper.previous();
        this.modal.hide();

        // this.template = new MessageTemplateModel();

        // this.active = false;
        // this.submitted = false;
        // this.saving = false;
        // this.modal.hide();

        // this.modalSave.emit(null);
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
        let newText = "";
        const parts: {
            type: "text" | "input";
            content: string;
            index?: number;
        }[] = [];
        let currentIndex = 0;

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
                    break;
                }
            }
        }
        return parts;
    }

    getCountryCode(event: any) {
        this.countryObj = event;
        this.contact.countryCode = Number(event.dialCode).toString();
    }

    onCountryChange(event: any) {
        this.contact.countryCode = Number(event.dialCode).toString();
    }

    getTemplateById(event: any) {
        debugger;
        this.isSubmitted = false;
        this.displayText = "";
        this.dynamicTextParts = [];
        this.VariableCount = 0;
        this.listOfVariable = null;
        this.ComponentBody = null;
        this.ComponentButton = null;
        this.ComponentFooter = null;
        this.ComponentHeader = null;
        this.ComponentButtonText = null;
        this.primengTableHelper.showLoadingIndicator();
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateByWhatsAppId(event.id)
            .subscribe((result) => {
                this.primengTableHelper.hideLoadingIndicator();
                this.templateById = result;
                if (this.templateById) {
                    this.VariableCount = this.templateById.variableCount;
                    this.inputCountFromBackend =
                        this.templateById.variableCount;

                    if (this.templateById.mediaType == "image") {
                        this.imageFlag = true;
                    } else {
                        this.imageFlag = false;
                    }
                    if (this.templateById.mediaType == "video") {
                        this.videoFlag = true;
                    } else {
                        this.videoFlag = false;
                    }
                    if (this.templateById.mediaType == "document") {
                        this.documentFlag = true;
                    } else {
                        this.documentFlag = false;
                    }
                    this.Component = this.templateById.components;
                    this.Component.forEach((e) => {
                        if (e.type == "HEADER") {
                            this.ComponentHeader = e;
                        }
                        if (e.type == "BODY") {
                            this.ComponentBody = e;
                        }
                        if (e.type == "FOOTER") {
                            this.ComponentFooter = e;
                        }
                        if (e.type == "BUTTONS") {
                            this.ComponentButton = e;
                        }
                    });
                    if (this.ComponentBody) {
                        this.listOfVariable =
                            this.ComponentBody.example?.body_text[0];
                        this.displayText = this.ComponentBody.text;
                        this.inputs = Array.from(
                            { length: this.inputCountFromBackend },
                            () => ({ value: "" })
                        );
                        if (this.VariableCount > 0) {
                            this.getExampleBody(
                                this.ComponentBody.example?.body_text[0]
                            );
                        }
                        this.dynamicTextParts = this.parseDynamicText(
                            this.displayText
                        );
                    }
                }
            });
    }

    changeFooter(lang: any) {
        if (lang == "en") {
            this.footer = "To Stop Receiving Notification Please Send (STOP).";
        }
        if (lang == "ar") {
            this.footer = "لايقاف استقبال النتبيهات يرجى ارسال *STOP*";
        }
    }

    // isSameNumber phoneAndCode=>phoneAndCode===
    isSamePhone = (phoneAndCode) => phoneAndCode === this.codeAndMobile;

    handleCheckIsValid() {
        if (!this.mobileNumber && !this.whatsAppName) return;

        this.savingNext = true;

        const countryCode = this.contact.countryCode;
        const phoneNumber = this.mobileNumber;

        this.teamInbox
            .isValidContact(phoneNumber, countryCode)
            .subscribe((result) => {
                this.savingNext = false;
                switch (result.state) {
                    case 1:
                        this.message.error(this.l("invalidTenant"));
                        this.savingNext = false;
                        break;
                    case 2:
                        this.combinedValue =
                            this.contact.countryCode +
                            this.mobileNumber +
                            " " +
                            "(" +
                            this.whatsAppName +
                            ")";
                        this.savingNext = false;
                        this.verticalWizardStepper.next();
                        break;
                    case 3:
                        this.message.error(this.l("contactAlreadyexits"));
                        this.savingNext = false;
                        break;
                    case 4:
                        this.message.error(this.l("invalidFormat"));
                        this.savingNext = false;
                        break;
                    case -1:
                        this.message.error(this.l("Error"));
                        this.submitted = false;
                        this.saving = false;
                        break;
                    default:
                        this.message.error(this.l("Error"));
                        break;
                }
            });
    }

    getTemplates() {
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppTemplateForCampaign(0, 50, this.appSession.tenantId)
            .subscribe((result) => {
                this.template = result.lstWhatsAppTemplateModel.filter(
                    (element) =>
                        element.language == "ar" || element.language == "en"
                );
                this.isLoading = false;
                // console.log(this.template);
                this.getTemplateList.emit(this.template);
            });
    }

    verticalWizardNext() {
        this.nextPage=true;

        this.nextbtn = true;
        if (this.template.length <= 0) {
            this.getTemplates();
        }
        if (
            !this.isExternal &&
            this.whatsAppName.length !== 0 &&
            this.mobileNumber.length !== 0
        ) {
            this.handleCheckIsValid();
            return;
        }
        else{
            this.isLoading = false;
        }

        if (this.isExternal && this.selectedContact) {
            this.createdOrGotContact = this.selectedContact;
            this.verticalWizardStepper.next();
            return;
        }
    }

    verticalWizardPrevious() {
        this.templateById = null;

        this.isSubmitted = false;

        this.nextbtn = false;
        this.sendMessage(this.templatePrevious); 

        this.verticalWizardStepper.previous();
    }

    incrementedIndex(index: number): number {
        return index + 1;
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

    created(event) {}

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

    getItems() {
        if (this.isLoading) {
          return [{ id: null, name: 'Loading...', disabled: true }];
        }
        return this.template;
      }
}
