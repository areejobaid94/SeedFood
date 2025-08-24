import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
    OnInit,
    Input,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import {
    ButtonCopyCodeVariabllesTemplate,
    Card,
    CardsModel,
    CardVariabllesTemplate,
    CarouselVariabllesTemplate,
    FirstButtonURLVariabllesTemplate,
    HeaderVariablesTemplate,
    MessageTemplateModel,
    SecondButtonURLVariabllesTemplate,
    TeamInboxDto,
    TeamInboxServiceProxy,
    TemplateVariables,
    TemplateVariablles,
    WhatsAppButtonModel,
    WhatsAppComponentModel,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { DarkModeService } from "@app/services/dark-mode.service";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { Channel } from "@app/main/teamInbox/channel";
import { DatePipe } from "@angular/common";
import { NotifyService } from "abp-ng2-module";
import { update } from "@node_modules/@types/lodash";
import { NewCustomerListFilter } from "@app/main/teamInbox/new-customer-list-filter.model";
import moment from "moment";
import { DomSanitizer, SafeResourceUrl } from "@node_modules/@angular/platform-browser";
const { toGregorian } = require("hijri-converter");

@Component({
    selector: "appteaminboxtemplatemodal",
    templateUrl: "./teaminbox-template-modal.component.html",
    styleUrls: ["./teaminbox-template-modal.component.css"],
})
export class TeaminboxTemplateModalComponent
    extends AppComponentBase
    implements OnInit
{
    theme: string;
    exampleBody: string[] = [];
    customeexampleBody: string[] = [];
    template: MessageTemplateModel[] = [];
    templateId: number = 9;
    templateName: string;
    imageFlag: boolean = false;
    VariableCount: number = 0;
    videoFlag: boolean = false;
    documentFlag: boolean = false;
    listOfVariable: string[] = null;
    values: any[] = [];
    displayText: string = "";
    testText: string = "     hassan   kmk    ";
    isSubmitted: boolean = false;
    dateAndTime: Date = new Date();
    minDate: Date = new Date();
    isSchedule: boolean = false;
    isScheduleBtn: boolean = false;

    selectedUser: Channel;

    inputs: { value: string }[] = [];
    dynamicTextParts: {
        type: "text" | "input";
        content: string;
        index?: number;
    }[] = [];


    listOfHeaderVariable: string[] = null;
    VariableCountHeader: number = 0;
    // VariableURL1: number = 0;
    // VariableURL2: number = 0;

    variableURL1: boolean = false;
    variableURL2: boolean = false;
    copyCodeEsxist:     boolean = false;
    firstButtonURLVariabllesTemplate: FirstButtonURLVariabllesTemplate;
    secondButtonURLVariabllesTemplate: SecondButtonURLVariabllesTemplate;
    buttonCopyCodeVariabllesTemplate:ButtonCopyCodeVariabllesTemplate;
    urlExample1: string;
    urlExample2: string;
    copyCodeExample1: string;

    urlLink1:string;
    urlLink2:string;
    copyCode:string;

    URLBtton1:WhatsAppButtonModel;
    URLBtton2:WhatsAppButtonModel;
    URL1isValid:boolean=true;
    URL2isValid:boolean=true;
    CopyCodeisValid:boolean=true;
    copyCodeButton:WhatsAppButtonModel;
urlVariables: { 
  [cardIndex: number]: { 
    [buttonIndex: number]: { value: string }[] 
  } 
} = {};

    inputsHeaders: { value: string }[] = [];

     dynamicTextPartsHeader: {
        type: "text" | "input";
        content: string;
        index?: number;
    }[] = [];
        displayHeaderText: string = "";
    inputCountFromBackendHeader: number = 0;
    customeexampleHeader: string[] = [];

    category:string="";
    
    
    
    componentCarousel: WhatsAppComponentModel = new WhatsAppComponentModel();
    cards:CardsModel[]=[]
    // cards: Card[] = [];
    carouselVariables: { [cardIndex: number]: { inputs: { value: string }[] } } = {};
    currentCardIndex: number = 0;

    VariableCountCarousel: number[] = []; 
    inputsCarousel: { value: string }[][] = []; 
    dynamicTextPartsCarousel: { type: "text" | "input"; content: string; index?: number; }[][] = []; 
    displayCarouselTexts: string[] = []; 

    Buttons: WhatsAppComponentModel = new WhatsAppComponentModel();

    isLoading = true;
    Component: WhatsAppComponentModel[] = [new WhatsAppComponentModel()];
    ComponentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButton: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButtonText: any;
    templateById: MessageTemplateModel = null;
    inputCountFromBackend: number = 0;
    @Output() updateStatus = new EventEmitter<void>();

    //  @ViewChild('SideBarMenuComponent', { static: true }) sideBarMenuComponent: SideBarMenuComponent;

    @ViewChild("teamInboxModal", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Output() getTemplateList: EventEmitter<any> = new EventEmitter<any>();

    @Input() perantTemplateList: MessageTemplateModel[];

    saving = false;

    constructor(
        private sanitizer: DomSanitizer,
        injector: Injector,
        public darkModeService: DarkModeService,
        private teamInbox: TeamInboxServiceProxy,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        private datePipe: DatePipe,
        private _notifyService: NotifyService,
        private teamInboxServiceProxy: TeamInboxServiceProxy,

    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.minDate = new Date();
        this.minDate.setMinutes(this.minDate.getMinutes() + 2);

        this.dateAndTime = new Date();

        this.templateName = "Select Template";
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
                this.getTemplateList.emit(this.template)
                // console.log(this.template);
                // this.templateListChange.emit(this.template);
            });
    }

    handleIsSchedule(newValue: Event) {
        this.minDate = new Date();
        this.minDate.setMinutes(this.minDate.getMinutes() + 2);

        this.dateAndTime = new Date();
        this.dateAndTime.setMinutes(this.dateAndTime.getMinutes() + 2);
        this.isSchedule = !this.isSchedule;
    }

    checkPattern(text: string): boolean {
        const pattern = /\{\{\d+\}\}/g;
        return pattern.test(text);
    }

    getExampleBody(example: string[]) {
        this.customeexampleBody = example;
    }

    isValidInput(index: number) {
        return this.dynamicTextParts[index].content != "";
    }

    onKeyPress(event: any, value: any) {
        const forbiddenKey = ["$"];
        const keyPressed = event.key;
        if (forbiddenKey.includes(keyPressed)) {
            event.preventDefault();
        }
    }
    getTemplateById(event: any) {

        debugger;
        this.isSubmitted = false;
        this.displayText = "";
        this.dynamicTextParts = [];
        this.VariableCount = 0;
        this.listOfVariable = null;
        this.displayHeaderText = "";
        this.dynamicTextPartsHeader = [];
        this.VariableCountHeader = 0;

        this.buttonCopyCodeVariabllesTemplate=null;
        this.secondButtonURLVariabllesTemplate=null
        this.firstButtonURLVariabllesTemplate=null;
        this.variableURL2=false;
        this.variableURL1=false;
        this.copyCodeEsxist=false;

        this.listOfHeaderVariable = null;
        this.ComponentBody = null;
        this.ComponentButton = null;
        this.ComponentFooter = null;
        this.ComponentHeader = null;
        this.ComponentButtonText = null;

        let id = event?.id || -1;
        this.primengTableHelper.showLoadingIndicator();
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateByWhatsAppId(id)
            .subscribe((result) => {

                debugger
                this.primengTableHelper.hideLoadingIndicator();
                this.templateById = result;
                if (this.templateById) {
                    this.templateId = this.templateById.localTemplateId;
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
                    

                    this.componentCarousel = this.templateById.components.find(
                        (i: { type: string }) => i.type === "carousel"
                    )||null;


                    if (this.ComponentBody?.example?.body_text?.[0]) {
                        const firstArray = this.ComponentBody.example.body_text[0];
                        this.VariableCount = Array.isArray(firstArray) ? firstArray.length : 0;
                    } else {
                        this.VariableCount = 0;
                    }
                debugger;
                    if (this.componentCarousel) {
                        this.cards = this.componentCarousel.cards;
                        this.VariableCountCarousel = [];
                        this.inputsCarousel = [];
                        this.dynamicTextPartsCarousel = [];
                        this.displayCarouselTexts = [];


                        debugger
                        this.cards.forEach((card, cardIndex) => {
                        const buttons = card.components.find(c => c.type === "BUTTONS")?.buttons || [];
                                        
                        buttons.forEach((button, buttonIndex) => {
                            if (button.type === 'URL' && button.example) {
                              // Initialize variables for this URL button
                            this.urlVariables[cardIndex] = this.urlVariables[cardIndex] || {};
                            this.urlVariables[cardIndex][buttonIndex] = 
                                button.example.map(() => ({ value: '' }));
                            }
                        });
                    });

                    debugger;
                    this.cards.forEach((card, cardIndex) => {
                        const cardBody = card.components.find((c: { type: string }) => c.type === "BODY");
                        if (cardBody) {
                            const varCount = cardBody.example?.body_text?.[0]?.length || 0;
                            this.VariableCountCarousel[cardIndex] = varCount;
                            
                            this.inputsCarousel[cardIndex] = Array.from(
                                { length: varCount },
                                () => ({ value: "" })
                            );
                            
                            if (cardBody.text) {
                                this.dynamicTextPartsCarousel[cardIndex] = this.parseDynamicText(cardBody.text);
                                this.displayCarouselTexts[cardIndex] = cardBody.text;
                            }
                        }
                    });
                    }

                      if(this.ComponentHeader?.example?.header_text){
                        this.VariableCountHeader= this.ComponentHeader.example.header_text?.length
                        this.inputCountFromBackendHeader = this.VariableCountHeader;
                    }else{
                        this.VariableCountHeader=0;
                        this.inputCountFromBackendHeader = 0;
                    }
                    this.Buttons = this.templateById.components.find(
                        (i: { type: string }) => i.type === "BUTTONS"
                    ) || null;
                    
                    const urlButtons = this.Buttons?.buttons?.filter(
                            (i: { type: string; example?: string[] }) =>
                            i.type === "URL" && i.example != null
                        ) || [];

                    const copyCodeButtons = this.Buttons?.buttons?.filter(
                        (i: { type: string }) => i.type === "COPY_CODE"
                    ) || [];

                    const urlButton1 = urlButtons[0] || null;
                    const urlButton2 = urlButtons[1] || null;
                    const copyCodeButton = copyCodeButtons[0] || null;

                    if(urlButton1){
                        if(urlButton1?.example){
                            this.URLBtton1=urlButton1;
                            this.variableURL1=true;
                            this.urlLink1=urlButton1.url;
                            this.urlExample1=urlButton1.example[0];
                            this.firstButtonURLVariabllesTemplate = new FirstButtonURLVariabllesTemplate({
                            varOne: "",
                            });
                        }
                        
                    }
                    if(urlButton2){
                        if(urlButton2?.example){
                            this.variableURL2=true;
                            this.URLBtton2=urlButton2;
                            this.urlLink2=urlButton2.url
                            this.urlExample2=urlButton2.example[0];
                            this.secondButtonURLVariabllesTemplate = new SecondButtonURLVariabllesTemplate({
                            varOne: "",
                            });
                        }
                    }
                    if(copyCodeButton){
                        if(copyCodeButton?.example){
                            this.copyCodeEsxist=true;
                            this.copyCodeButton=copyCodeButton;
                            this.buttonCopyCodeVariabllesTemplate = new ButtonCopyCodeVariabllesTemplate({
                            varOne: "",
                            });
                        }
                    }

                    if (this.ComponentHeader) {
                        if(this.ComponentHeader.example?.header_text?.[0]){
                            
                                    const firstHeader = this.ComponentHeader.example?.header_text?.[0];
                                if (firstHeader) {
                                    this.listOfHeaderVariable = [firstHeader];
                                }else{
                                    this.listOfHeaderVariable=null;
                                }
                                this.displayHeaderText = this.ComponentHeader.text;
                                this.inputsHeaders = Array.from(
                                    { length: this.VariableCountHeader },
                                    () => ({ value: "" })
                                );
                                if (this.VariableCountHeader > 0 && firstHeader) {
                                    this.getExampleHeader(
                                        [this.ComponentHeader.example?.header_text[0]]
                                    );
                                }
                                this.dynamicTextPartsHeader = this.parseDynamicTextHeader(
                                    this.displayHeaderText
                                );
                        }
                    }

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

    updateText() {
        this.displayText = "";
        this.dynamicTextParts.forEach((part) => {
            if (part.type === "input") {
                part.content =
                    this.inputs[part.index].value.trimLeft() ||
                    `{{${part.index + 1}}}`;
            }
        });
        this.displayText = this.dynamicTextParts
            .map((item) => item.content)
            .join("");
        this.displayText = this.displayText.replace(/ /g, "&nbsp;");
        this.ComponentBody.text = this.displayText.replace(/ /g, "&nbsp;");
    }

    parseDynamicText(
        text: string
    ): { type: "text" | "input"; content: string; index?: number }[] {
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

        return parts;
    }

    trackByFn(index: number, item: any): number {
        return item.localTemplateId;
    }

    viewDetails(template: MessageTemplateModel[], selectedUser: Channel): void {
        if (this.perantTemplateList.length <= 0) {
            this.getTemplates();
        } else {
            this.template = this.perantTemplateList;
            this.isLoading = false;
        }
        this.templateId = 0;
        this.templateName = "";
        this.templateById = null;
        this.selectedUser = selectedUser;
        this.modal.show();
    }
    isValidUrl(urlString: string): boolean {
    try {
            const url = new URL(urlString);
        debugger;
            return /^[\w\-~.%]+$/.test(urlString) && 
                        !/\s/.test(urlString) &&
                    url.protocol === 'http:' || url.protocol === 'https:';
        } catch (e) {
            return false;
        }
    }

    handleSend(Schedule: boolean = false) {
        this.minDate = new Date();
        this.isSubmitted = true;
        let templateVariables: TemplateVariablles;
        let templateHeaderVariables: HeaderVariablesTemplate;
        let sendTime: string;
        this.isScheduleBtn = null;
        this.isScheduleBtn = Schedule ? true : false;
        let carouselTemplate: CarouselVariabllesTemplate | null = null;

        if (this.VariableCountHeader > 0) {
            for (const item of this.inputsHeaders) {
                if (item.value.length === 0) return;
            }
            let arr = this.dynamicTextPartsHeader.filter(
                (item) => item.type == "input"
            );

            templateHeaderVariables = new HeaderVariablesTemplate({
                varOne: arr[0]?.content || null,
            });
        } else {
            templateHeaderVariables = null;
        }
        if(this.firstButtonURLVariabllesTemplate  && this.templateById.category!='AUTHENTICATION'){
            if (
                !this.firstButtonURLVariabllesTemplate?.varOne ||                 
                this.firstButtonURLVariabllesTemplate.varOne.trim() === '' ||    
                !this.isValidUrl(this.buildURLButton1())
            ) {
                this.URL1isValid = false;
                return;
            }
        }
        if(this.secondButtonURLVariabllesTemplate){
            if (
                this.secondButtonURLVariabllesTemplate.varOne.trim() === '' ||    
                !this.isValidUrl(this.buildURLButton2())
            ) {
                this.URL2isValid = false;
                return;
            }
        }
        if(this.buttonCopyCodeVariabllesTemplate?.varOne){
            
            if (
                this.buttonCopyCodeVariabllesTemplate.varOne.trim() === ''||
                !/\s/.test(this.firstButtonURLVariabllesTemplate.varOne)  
            ) {
                this.CopyCodeisValid = false;
                return;
            }
        }
        if (this.VariableCount > 0) {
            for (const item of this.inputs) {
                if (item.value.length === 0) return;
            }
            let arr = this.dynamicTextParts.filter(
                (item) => item.type == "input"
            );

               templateVariables = new TemplateVariablles({
                varOne: arr[0]?.content || null,
                varTwo: arr[1]?.content || null,
                varThree: arr[2]?.content || null,
                varFour: arr[3]?.content || null,
                varFive: arr[4]?.content || null,
                varSix: arr[5]?.content || null,
                varSeven: arr[6]?.content || null,
                varEight: arr[7]?.content || null,
                varNine: arr[8]?.content || null,
                varTen: arr[9]?.content || null,
                varEleven: arr[10]?.content || null,
                varTwelve: arr[11]?.content || null,
                varThirteen: arr[12]?.content || null,
                varFourteen: arr[13]?.content || null,
                varFifteen: arr[14]?.content || null, 
                vaSixteen: arr[15]?.content || null, 
                varSixteen: arr[15]?.content || null, 

            });
        } else {
            templateVariables = null;
        }

          
        if (this.componentCarousel) {
                const cards: Card[] = [];
                let hasCarouselVariables = false;
                                
                for (let i = 0; i < this.cards.length; i++) {
                    const card = this.cards[i];
                
                    const buttonsComponent = card.components.find(c => c.type === "BUTTONS");
                    const buttons = buttonsComponent?.buttons || [];
                    const buttonVariables: any = {};
                
                    for (let buttonIndex = 0; buttonIndex < buttons.length; buttonIndex++) {
                        const button = buttons[buttonIndex];
                    
                        if (button.type === 'URL' && this.urlVariables[i]?.[buttonIndex]) {
                            const inputs = this.urlVariables[i][buttonIndex];
                        
                            for (const input of inputs) {
                                if (!input.value) {
                                    this.notify.warn("Please fill all URL variables");
                                    return;
                                }
                            }
                        
                            buttonVariables[`button${buttonIndex + 1}`] = {
                                varOne: inputs[0]?.value || null,
                            };
                        
                            hasCarouselVariables = true;
                        }
                    }
                
                    const bodyComponent = card.components.find(c => c.type === "BODY");
                    let cardVariables: CardVariabllesTemplate | null = null;
                
                    if (this.VariableCountCarousel[i] > 0) {
                        for (const item of this.inputsCarousel[i]) {
                            if (!item.value) {
                                this.notify.warn("Please fill all body variables");
                                return;
                            }
                        }
                    
                        const arr = this.dynamicTextPartsCarousel[i].filter(item => item.type === "input");
                    
                        cardVariables = new CardVariabllesTemplate({
                            varOne: arr[0]?.content || null,
                            varTwo: arr[1]?.content || null,
                            varThree: arr[2]?.content || null,
                            varFour: arr[3]?.content || null,
                            varFive: arr[4]?.content || null,
                            varSix: arr[5]?.content || null,
                            varSeven: arr[6]?.content || null,
                            varEight: arr[7]?.content || null,
                            varNine: arr[8]?.content || null,
                            varTen: arr[9]?.content || null,
                            varEleven: arr[10]?.content || null,
                            varTwelve: arr[11]?.content || null,
                            varThirteen: arr[12]?.content || null,
                            varFourteen: arr[13]?.content || null,
                            varFifteen: arr[14]?.content || null,
                        });
                    
                        hasCarouselVariables = true;
                    }
                
                    let firstButtonVars: FirstButtonURLVariabllesTemplate | null = null;
                    let secondButtonVars: SecondButtonURLVariabllesTemplate | null = null;
                
                    const urlButtons = buttons.filter(b => b.type === "URL");
                
                    if (urlButtons[0]?.example) {
                        firstButtonVars = new FirstButtonURLVariabllesTemplate({
                            varOne: this.urlVariables[i]?.[0]?.[0]?.value ?? null,
                        });
                    }
                
                    if (urlButtons[1]?.example) {
                        secondButtonVars = new SecondButtonURLVariabllesTemplate({
                            varOne: this.urlVariables[i]?.[1]?.[0]?.value ?? null,
                        });
                    }
                
                    cards.push(new Card({
                        variables: cardVariables,
                        firstButtonURLVariabllesTemplate: firstButtonVars,
                        secondButtonURLVariabllesTemplate: secondButtonVars,
                        cardIndex: i,
                        variableCount: this.VariableCountCarousel[i]
                    }));
                }
            
                if (hasCarouselVariables) {
                    carouselTemplate = new CarouselVariabllesTemplate({
                        cards: cards
                    });
                }
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

        let teamInboxDto = new TeamInboxDto({
            templateId: this.templateId,
            contactName: this.selectedUser.displayName,
            phoneNumber: this.selectedUser.userId.split("_")[1],
            campaignStatus: Schedule ? 2 : 1,
            customerOPT: this.selectedUser.customerOPT !== undefined ? this.selectedUser.customerOPT : 0,
            language: this.templateById.language,
            sendTime,
            templateVariables,
            isExternal: false,
            headerVariabllesTemplate:templateHeaderVariables ,
            firstButtonURLVariabllesTemplate:this.firstButtonURLVariabllesTemplate,
            secondButtonURLVariabllesTemplate:this.secondButtonURLVariabllesTemplate,
            carouselTemplate: carouselTemplate ,
            buttonCopyCodeVariabllesTemplate:this.buttonCopyCodeVariabllesTemplate
            });
        this.saving = true;

        this.teamInbox.sendCampign(teamInboxDto).subscribe(
            (result) => {
                if (result?.state === -1)
                    this.notify.error(this.l("ErrorFromServer"));
                if (result?.state === 1)
                    this.notify.error(this.l("notHaveEnoughFunds"));
                if (result?.state === 2)
                    this.notify.success(this.l("sentSuccefully"));
                if (result?.state === 3)
                    this.notify.warn(this.l("haveActiveCamp"));
                if (result?.state === 4)
                    this.notify.error(this.l("notValidDate"));
                if (result?.state === 5)
                    this.notify.error(this.l("reachlimit"));
                if (result?.state === 6)
                    this.notify.warn(this.l("invalidTenant"));
                if (result?.state === 7)
                    this.notify.error(this.l("contactAlreadyexits"));
                if (result?.state === 8)
                    this.notify.error(this.l("invalidFormat"));
                if (result?.state === 9)
                    this.notify.error(this.l("templateNotApporved"));
                if (result?.state === 10)
                    this.notify.error(this.l("theContactisOptOut"));

                this.saving = false;
                this.isSubmitted = false;
                this.minDate = new Date();
                this.dateAndTime = new Date();
                this.updateStatus.emit();
                this.modal.hide();
            },
            (err) => {
                this.notify.error(this.l("ErrorFromServer"));
                this.saving = false;
                this.isSubmitted = false;
                this.minDate = new Date();
                this.dateAndTime = new Date();
                this.modal.hide();
            }
        );
    }



        

    close(): void {
        this.buttonCopyCodeVariabllesTemplate=null;
        this.secondButtonURLVariabllesTemplate=null
        this.firstButtonURLVariabllesTemplate=null;
        this.variableURL2=false;
        this.variableURL1=false;
                this.copyCodeEsxist=false;

        this.minDate = new Date();
        this.dateAndTime = new Date();
        this.listOfVariable = [];
        this.templateById = null;
        this.isSubmitted = false;
        this.templateById = null;
        this.minDate = new Date();
        this.dateAndTime = new Date();
        this.isSchedule = false;
        this.modal.hide();
    }

    getItems() {
        if (this.isLoading) {
            return [{ id: null, name: "Loading...", disabled: true }];
        }
        return this.template;
    }


         getExampleHeader(example: string[]) {
        debugger
        this.customeexampleHeader = example;
    }

       updateCarouselText(cardIndex: number) {
    this.displayCarouselTexts[cardIndex] = "";
    
    this.dynamicTextPartsCarousel[cardIndex].forEach((part) => {
        if (part.type === "input") {
            part.content = this.inputsCarousel[cardIndex][part.index].value || `{{${part.index + 1}}}`;
        }
    });

    this.displayCarouselTexts[cardIndex] = this.dynamicTextPartsCarousel[cardIndex]
        .map((item) => item.content)
        .join("");

    // Update the card's body text in the carousel data
    if (this.cards[cardIndex]) {
        const cardBody = this.cards[cardIndex].components.find((c: { type: string }) => c.type === "BODY");
        if (cardBody) {
            cardBody.text = this.displayCarouselTexts[cardIndex];
        }
    }
}

// Method to parse URL for variables
parseUrlVariables(url: string): { type: "text" | "input"; content: string; index?: number }[] {
  const parts: { type: "text" | "input"; content: string; index?: number }[] = [];
  const regex = /{{(\d+)}}/g;
  let lastIndex = 0;
  let match;
  
  while ((match = regex.exec(url)) !== null) {
    if (match.index > lastIndex) {
      parts.push({
        type: "text",
        content: url.substring(lastIndex, match.index)
      });
    }
    
    parts.push({
      type: "input",
      content: match[0],
      index: parseInt(match[1]) - 1
    });
    
    lastIndex = match.index + match[0].length;
  }
  
  if (lastIndex < url.length) {
    parts.push({
      type: "text",
      content: url.substring(lastIndex)
    });
  }
  
  return parts;
}

// Method to update URL with variables
updateUrlWithVariables(cardIndex: number, buttonIndex: number, url: string): string {
  if (!this.urlVariables[cardIndex]?.[buttonIndex]) return url;
  
  return url.replace(/{{(\d+)}}/g, (match, index) => {
    const varIndex = parseInt(index) - 1;
    return this.urlVariables[cardIndex][buttonIndex][varIndex]?.value || match;
  });
}

  parseDynamicTextHeader(
        text: string
    ): { type: "text" | "input"; content: string; index?: number }[] {
        debugger;
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


    updateTextHeader() {
            debugger;
        this.displayHeaderText = "";
        // Replace placeholders with input values, or revert to placeholders if input is empty
        this.dynamicTextPartsHeader.forEach((part) => {
            if (part.type === "input") {
                part.content =
                    this.inputsHeaders[part.index].value || `{{${part.index + 1}}}`;
            }
        });
        this.displayHeaderText = this.dynamicTextPartsHeader
            .map((item) => item.content)
            .join("");
        this.ComponentHeader.text = this.displayHeaderText;
    }

        buildCopyCodeButton(): string {
                const placeholderValue = this.buttonCopyCodeVariabllesTemplate.varOne || '';
            return  placeholderValue;
        }

        buildURLButton1(): string {
            debugger;
                const placeholderValue = this.firstButtonURLVariabllesTemplate.varOne || '';
                let x=this.URLBtton1.url.replace('{{1}}', placeholderValue);
            return this.URLBtton1.url.replace('{{1}}', placeholderValue);
        }
         buildURLButton2(): string {
                const placeholderValue = this.secondButtonURLVariabllesTemplate.varOne || '';
            return this.URLBtton2.url.replace('{{1}}', placeholderValue);
        }

        URLValidVarible1() {
            debugger
            if (this.URLBtton1) {
                const varOne = this.firstButtonURLVariabllesTemplate.varOne;
                this.URL1isValid = typeof varOne === 'string' && /^[a-zA-Z0-9-_]+$/.test(varOne);
            }
        }
        URLValidVarible2() {
            if (this.URLBtton2) {
                const varOne = this.secondButtonURLVariabllesTemplate.varOne;
                this.URL2isValid = typeof varOne === 'string' && /^[a-zA-Z0-9-_]+$/.test(varOne);
            }
        }

        isValidText(text) {
        const regex = /^(?=(?:\S*\s\S*\s\S*\s*)|$)(?!\s*$).*$/;

        const trimmedString = text?.replace(/\s+$/, "");

        return regex.test(trimmedString);
    }

 get safeMediaLink(): SafeResourceUrl {
   if (!this.templateById?.mediaLink) return '';
   return this.sanitizer.bypassSecurityTrustResourceUrl(this.templateById.mediaLink);
 }
}
