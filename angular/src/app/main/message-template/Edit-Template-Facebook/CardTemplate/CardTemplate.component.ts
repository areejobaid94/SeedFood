import { Button } from '@node_modules/primeng/button';
import { Component, ElementRef, EventEmitter, Injector, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild, Type, ChangeDetectorRef } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@node_modules/@angular/forms';
import { CardsModel, MessageTemplateModel, WhatsAppButtonModel, WhatsAppComponentModel, WhatsAppExampleModel, WhatsAppHeaderUrl, WhatsAppMediaID } from '@shared/service-proxies/service-proxies';
import Swal from 'sweetalert2';
import {
  CountryISO,
  NgxIntlTelInputComponent,
  PhoneNumberFormat,
  SearchCountryField,
} from "ngx-intl-tel-input";
import { SafeHtml } from '@node_modules/@angular/platform-browser';
import { MessageService } from '@node_modules/abp-ng2-module/public-api';
import { TemplateService } from '../../Facebook-Template/Template.service';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { debounce } from '@node_modules/@types/lodash';

@Component({
  selector: 'app-CardTemplate',
  templateUrl: './CardTemplate.component.html',
  styleUrls: ['./CardTemplate.component.css']
})
export class CardTemplateComponent implements OnInit, OnChanges {
//card var
bodyCardComponent:WhatsAppComponentModel;
@Input() carousel:WhatsAppComponentModel
@Output() carouselChange = new EventEmitter<WhatsAppComponentModel>();
trackByCard(index: number, card: any): number {
  return index;
}
    @Input() 
    get carouselCards(): CardsModel[] {
      return this._carouselCards;
    }

    set carouselCards(value: CardsModel[]) {
      this._carouselCards = value || [];
      this.carouselCardsChange.emit(this._carouselCards);
    }
    @Output() carouselCardsChange = new EventEmitter<CardsModel[]>();

    private _carouselCards: CardsModel[] = [];

    carouselIsValidTemplate:boolean=false;
    sampilBodyValid:boolean=false;
    @Input() isValid: boolean = false;
    @Output() isValidChange = new EventEmitter<boolean>();
    tempPhoneNumbers: string[][] = []; // same structure as buttons[i][j]

    example = {
      body_text: [[]]
    };

    onValidityChange() {
      debugger;
      this.isValid = this.carouselIsValidTemplate;
      this.isValidChange.emit(this.carouselIsValidTemplate);
    }
    aaa(){
      this.isValid =!this.isValid
      this.isValidChange.emit(this.isValid );
    
    }

variablePattern = /\{\{\d+\}\}/g;


buttonTextEmpty:boolean[] = [];
buttonLimits: boolean[] = [false];
buttonTextErorr: boolean[] = [];
buttonPhoneNumberErorr: boolean[] = [];
buttonURLError: boolean[] = [];
errorBodyIsEmptyError: boolean[] = [];
mediaPathIsEmptyError: boolean[] = [true];
exmpleURLError: boolean[] = [];
buttonErrors: boolean[][] = [];
phoneErrors: boolean[][] = [];
carouselButtonErrorMessage :string='';
ButtonCountEqual:boolean=true;
//end card var
  variableAdded: boolean = false; 
  header: string = 'None';
  isSampleErorr:boolean=false;
  headerContent:string ='';
  fileToUpload: File | null = null; 
  extFile: string = '';
  uploadedImageUrl: string | ArrayBuffer | null = null; 
  uploadedVideoUrl: string | ArrayBuffer | null = null; 
  uploadedDocumentUrl: string | ArrayBuffer | null = null; 
  errorAddMedia:boolean=false;
  errorAddSampleBody:boolean=false;
  errorAddSampleHeader:boolean=false;
  errorHeadertext:boolean=false;
  errorUnEqualVaribleBody:boolean=false;

  // errorBodyIsEmpty :boolean=false;
  image:string;

  headervariable: number = 0;
  sampleContents: { [key: number]: string } = {};
  mediaErorr:boolean=false;
  isTextValid = true;
  listOfVariables = [];
  formattedTextBody: SafeHtml;
  formattedText: string;
  sanitizer: any;
  showWarningMessage: boolean = false;
  validationMessage: string = '';
  showEmojiPicker: boolean= false;
  body: string = 'Hello';
  variables: { number: number }[] = [];
  maxCharacters: number = 1024;
  totalCharacters: number = 0;
  dynamicVariables: string[][] = [];
  
  @Input() facebookTemplateDto: any ;

  whatsAppHeaderHandle: WhatsAppHeaderUrl = new WhatsAppHeaderUrl();
  whatsAppMediaID: WhatsAppMediaID = new WhatsAppMediaID();

  bodyTextContent: string | undefined;
  private http: HttpClient;

  
  @Input() componentBody:WhatsAppComponentModel
  @Input() componentFooter:WhatsAppComponentModel
  @Input() componentHeader: WhatsAppComponentModel;
  @Output() componentHeaderChange = new EventEmitter<WhatsAppComponentModel>();
  @Output() componentBoodyChange = new EventEmitter<WhatsAppComponentModel>();
  @Output() componentFooterChange = new EventEmitter<WhatsAppComponentModel>();

  @Output() componentTextChange = new EventEmitter<WhatsAppComponentModel>();
  imageFlag: boolean = false;
  videoFlag: boolean = false;


      mediaUrl: WhatsAppExampleModel = new WhatsAppExampleModel();
      message: MessageService;
      @Output() errorSampleBodyChange = new EventEmitter<boolean>();

///end content varible


  // -----------------------------------------Button variables
  PhoneNumberFormat =PhoneNumberFormat;
  preferredCountries: string[] = ['jo', 'sa'];

    phoneForm = new FormGroup({
        phone: new FormControl(undefined, [Validators.required]),
    });
    phonenumber:any;
    
    
    @ViewChild('telInput') telInput: NgxIntlTelInputComponent; 

  @Input() template: any = {};
  @Input() selectedMessageType:string;
  @Input() selectedCategory:string;
  @Output() transferData = new EventEmitter<any[]>();

  @Output() isPlayVisibleChange = new EventEmitter<boolean>(); 
  Button:WhatsAppButtonModel=new WhatsAppButtonModel();

  @Output() isFormInvalidChange = new EventEmitter<boolean>();

  isFormInvalid: boolean = true;
  language: string;
  urlPattern = /^https?:\/\/.+$/;
  isUrlInvalidMap: Map<number, boolean> = new Map();
  isUrlInvalidExampleMap: Map<number, boolean> = new Map();

  isSampleUrlInvalid:boolean=true;
  buttonLength:number;

  isCompleteFlowDisabled: boolean = false;
  isCallButtonDisabled: boolean = false;
  isCopyOfferCodeDisabled: boolean = false;
  isVisitWebsiteDisabled: boolean = false;
  isMarketinOptOutDisabled: boolean = false;
  isCustomDisabled: boolean = false; 
  MarketingOptOutCheckboxStates: boolean ;
  visitWebsiteCheckboxStates: boolean ;
  isPlayVisible: boolean = false;
  buttons = new WhatsAppComponentModel();
  componentButtonForView: any = {};
  
   buttonsArray: WhatsAppButtonModel[] ;

   buttonURL:any;

  Buttons: WhatsAppComponentModel = new WhatsAppComponentModel();
  Butto: WhatsAppComponentModel = new WhatsAppComponentModel();

  isOptOutResponsibilityAcknowledged:boolean=true;
  countryCode: string = '';
  localNumber: string = '';
  
  duplicateError: boolean[] = [];
  hasDuplicates: boolean = true;
  // isValid: boolean = true;
  isFormValidForSubmit:boolean=true;

  countryObj: any = null;



  @ViewChild('fileInput') fileInput: ElementRef;

  @Output() errorStatusChanged = new EventEmitter<boolean>();
  @Output() errorsampleHeaderChange = new EventEmitter<boolean>();  
  @Output() errorHeaderChange = new EventEmitter<boolean>();  
  @Output() errorBodyTextIsEmpty = new EventEmitter<boolean>();
  @Output() whatsAppHeaderChanged = new EventEmitter<any>();

  // -----------------------------------------Button variables


  // Template properties
  
  selectedHeaderType: string = 'IMAGE';
  maxCards = 10;

  // File upload properties
  
  // Error flags


  constructor(private element: ElementRef,private templateService:TemplateService ,injector: Injector, http: HttpClient,private cdRef: ChangeDetectorRef ) { 
    this.http = http;

  }

  ngOnInit() {
      this.buttonLimits
    this.carousel = this.template?.components?.find(
      (i: { type: string }) => i.type === "carousel"
    ) || null;
    this.carouselIsValidTemplate=false;
    this.onValidityChange();
    this.checkValidationCard();    
    // this.insertVariable(0);
    this.buttonErrors = this.carouselCards.map(card => 
      card.components[2].buttons.map(() => false)
    );

      if (this.template?.id != null && 
        this.carouselCards[0]?.components[2]?.buttons?.length == 1) {
          for(let i=0;i< this.carouselCards[0]?.components[2]?.buttons?.length;i++){
            this.buttonLimits[i]=false;
          }
      this.buttonLimits[0] = false; 
    }else if(this.template?.id != null && 
        this.carouselCards[0]?.components[2]?.buttons?.length == 2){
        for(let i=0;i< this.carouselCards[0]?.components[2]?.buttons?.length;i++){
            this.buttonLimits[i]=true;
          }
    }

    this.phoneErrors = [];

    this.phoneErrors = this.carouselCards.map(card => 
      card.components[2].buttons.map(() => false)
    );
    this.initializePhoneNumbers();


      this.tempPhoneNumbers = this.carouselCards.map(card =>
      card.components[2].buttons.map(btn =>
        typeof btn.phone_number === 'string' ? btn.phone_number : ''
      )
    );

    this.phoneErrors = this.carouselCards.map(card =>
      card.components[2].buttons.map(() => false)
    );

  }
  test22(){
    debugger;
    this.buttonErrors
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.template && !changes.template.firstChange) {
    }
  }
getTextDirection(text: string): 'rtl' | 'ltr' {
  const arabicRegex = /[\u0600-\u06FF]/;
  return arabicRegex.test(text || '') ? 'rtl' : 'ltr';
}

  addNewCard(): void {
    if (this.carouselCards.length >= this.maxCards) {
      Swal.fire({
        icon: 'warning',
        title: 'Maximum cards reached',
        text: `WhatsApp carousel templates can have maximum ${this.maxCards} cards.`
      });
      return;
    }
  
    const components = [
      WhatsAppComponentModel.fromJS({
        type: 'HEADER',
        format: 'IMAGE',
        example: { header_handle: [''],
          mediaCard: ['']
         }
      }),
      WhatsAppComponentModel.fromJS({
        type: 'BODY',
        text: 'Welcome to our store!',
        example: {  body_text: [[]] }
      }),
      WhatsAppComponentModel.fromJS({
        type: 'BUTTONS',
        buttons: [
          WhatsAppButtonModel.fromJS({
            type: 'QUICK_REPLY',
            text: 'Quick Reply'
          })
          // ,
          // WhatsAppButtonModel.fromJS({
          //   type: 'URL',
          //   text: 'Visit Shop',
          //   url: 'https://example.com/shop',
          //   // example: ['example']
          // })
        ]
      })
    ];
  
    const newCard = CardsModel.fromJS({ components });
    this.buttonErrors.push(
    newCard.components[2].buttons.map(() => false)
    );

    const buttonComponent = newCard.components[2];
    const buttonArray = buttonComponent?.buttons || [];
    this.buttonErrors.push(buttonArray.map(() => false));

    // Update array immutably
    this.carouselCards = [...this.carouselCards, newCard];
    this.buttonLimits[this.carouselCards.length - 1] = false;
    this.carouselIsValidTemplate = false;
    this.ButtonLimits(this.carouselCards.length - 1);
    this.onValidityChange();
    this.initializePhoneNumbers();
      this.cdRef.detectChanges();
      this.checkValidationCard();
    this.carouselChange.emit(this.carousel);

      this.tempPhoneNumbers = this.carouselCards.map(card =>
      card.components[2].buttons.map(btn =>
        typeof btn.phone_number === 'string' ? btn.phone_number : ''
      )
    ); 
  
  }
  

  removeCard(index: number): void {
    if (this.carouselCards.length > 1) {
      this.carouselCards.splice(index, 1);
      this.carouselChange.emit(this.carousel);
      this.checkValidationCard();
    } else {
      Swal.fire({
        icon: 'warning',
        title: 'Cannot remove card',
        text: 'A carousel template must have at least one card.'
      });
      
    }
    this.checkValidationCard();

  }

  clearUploadedFile(cardIndex: number): void {
    debugger
    const header = this.carouselCards[cardIndex].components.find((c: any) => c.type === 'HEADER');
    if (header) {
      header.example.header_handle = [''];
      header.example.mediaCard = [''];
      this.carouselCards[cardIndex].components.find((c: any) => c.type === 'HEADER').example.header_handle[0]='';
      this.carouselCards[cardIndex].components.find((c: any) => c.type === 'HEADER').example.mediaCard[0]='';
      this.mediaPathIsEmptyError[cardIndex]=true;
        this.carouselIsValidTemplate=false;
        this.onValidityChange();
      
    }
    this.carouselChange.emit(this.carousel);
  }

  private showFileError(message: string): void {
    Swal.fire({
      icon: 'error',
      title: 'Invalid file',
      text: message
    });
    this.clearUploadedFile(0);
  }

  onActionChange(button: WhatsAppButtonModel,cardIndex:number) {
    this.setDefaultProperties(button,cardIndex);
    this.validateButtons(cardIndex);
    this.checkValidationCard();

  }

  //////////////button code 

  checkPhoneNumber() {
    this.Button = this.Buttons?.buttons?.find(
      (i: { type: string }) => i.type === "PHONE_NUMBER"
    )||null;
    if (this.phoneForm.status == "VALID") {
      
        this.countryObj = this.phoneForm.value.phone;
        this.Button.phone_number = this.countryObj.e164Number.replace(
            "+",
            ""
        );
    } else {
        this.Button.phone_number = null;
        this.phoneForm.value.phone = null;
        this.countryObj = null;
        return;
    }
  }


  validateButtons2(cardIndex: number): void {
    debugger
    this.buttonURLError;
    const buttons = this.carouselCards[cardIndex].components[2].buttons;
    if(this.carouselCards[cardIndex].components[2].buttons.some(v => v.text =='' )){
      this.buttonTextEmpty[cardIndex]=true;
    }else{
      this.buttonTextEmpty[cardIndex]=false;
    }
   if(buttons.length == 2 && buttons[0].type==buttons[1].type){
      const hasDuplicate =
      buttons.length === 2 && buttons[0].text === buttons[1].text;
    this.buttonTextErorr[cardIndex] = hasDuplicate;
    }else{
      this.buttonTextErorr[cardIndex] =false;
    }
    this.buttonURLError;

    this.checkValidationCard();
    this.buttonURLError;

  }
  
  validateButtons(cardIndex:number): void {
    this.Buttons= this.carouselCards[cardIndex].components[2];
    this.duplicateError = this.Buttons.buttons.map(() => false);
  
    this.Buttons.buttons.forEach((button, i) => {
      if (button.text) {
        this.Buttons.buttons.forEach((innerButton, j) => {
          if (i !== j && button.text === innerButton.text) {
            this.duplicateError[i] = true;
            this.duplicateError[j] = true;
          }
        });
      }
    });
    
    this.hasDuplicates = this.duplicateError.some(error => error === true);
    this.isValid = this.Buttons.buttons.every(button => button.text && button.text.trim() !== '');
  }

  onURLExampleChange(event: Event, index: number): void {
    this.checkValidationCard()
  }
  isUrlInvalid(index: number): boolean {
    return this.isUrlInvalidMap.get(index) || false; 
  }
  isUrlInvalidExample(index: number  ): boolean {
    return this.isUrlInvalidExampleMap.get(index) || false;
    
  }
  validateURL(button: any): void {
    button.isUrlInvalid = !this.urlPattern.test(button.url || '');
  }
  onSampleURLChange(event: Event): void {
    const input = (event.target as HTMLInputElement).value; 
    const regex = new RegExp(this.urlPattern);
    this.isSampleUrlInvalid = !regex.test(input); 
  }

  validateSampleURL(button: any): void {
    this.isSampleUrlInvalid = !this.urlPattern.test(button|| '');
  }
  initializeExample(button: WhatsAppButtonModel): void {
    if (!button.example || !Array.isArray(button.example)) {
      button.example = [''];
    }
    else{
      
    }
  }

  onButtonTypeChange(buttonType: string,cardIndex :number) {
    debugger;
    this.carouselCards[cardIndex].components[2];
    let buttonComponent = this.carouselCards[cardIndex].components[2];
    this.Buttons = this.carouselCards[cardIndex].components[2];

    if (!this.Buttons && buttonComponent) {
        this.Buttons = new WhatsAppComponentModel();
        this.Buttons.type = "BUTTONS";
        this.Buttons.buttons = [];
        this.template.components.push(this.Buttons);
    }

    let button = new WhatsAppButtonModel();

    if (buttonType === 'visit-website') {
        button.type = "URL";
        button.text = 'Visit Website';
        button.url = "https://example.com/shop";
        // button.example = [""];


        this.Buttons.buttons.push(button);
        this.isCompleteFlowDisabled = true;
        this.buttonLength = this.Buttons.buttons.length;
        this.isUrlInvalidMap.set(this.Buttons.buttons.length - 1, true);
    } else if (buttonType === 'call-phone-number') {
        button.type = "PHONE_NUMBER";
        button.text = 'Call phone number';
        this.Buttons.buttons.push(button);
        if (!this.phoneErrors[cardIndex]) {
          this.phoneErrors[cardIndex] = [];
        }
        const newIndex = this.carouselCards[cardIndex].components[2].buttons.length - 1;
        this.phoneErrors[cardIndex][newIndex] = false;

    }   else if (buttonType === 'QUICK_REPLY') {
        button.type = "QUICK_REPLY";
        button.text = 'Quick Reply';
        this.Buttons.buttons.push(button);
    }
    this.ButtonLimits(cardIndex);
    this.validateButtons2(cardIndex);
    this.validateButtons(cardIndex);
}


removeButton(cardIndex :number,index: number) {
  if(this.carouselCards[cardIndex].components[2].buttons.length<=1){
    return;
  }
  if (this.Buttons) {
    this.carouselCards[cardIndex].components.find((c: any) => c.type === 'BUTTONS').buttons.splice(index, 1);

      this.ButtonLimits(cardIndex);

      const totalButtons = this.carouselCards[cardIndex].components
          .filter(c => c.type === "BUTTONS")
          .reduce((count, c) => count + (c.buttons ? c.buttons.length : 0), 0);

      if (totalButtons < 2) {
          this.isPlayVisible = false;
          this.isPlayVisibleChange.emit(this.isPlayVisible);
      }
  }
 
  if(this.isUrlInvalidMap.has(index)){
    this.isUrlInvalidMap.set(index, false); 
    this.isUrlInvalidMap.delete(index);
  }

  this.validateButtons(cardIndex);
    this.checkValidationCard();
}

  setDefaultProperties(button: WhatsAppButtonModel,cardIndex:number) {

    // this.carouselCards[cardIndex].components[2].buttons.
    button.url = null;
    button.phone_number = null;
    button.text='';
    if (button.type == 'URL') {
      button.type = "URL";
      button.text = 'Visit website';
      button.url = "https://example.com";
  } else if (button.type == 'PHONE_NUMBER') {
      button.type = 'PHONE_NUMBER';
      button.text = 'Call phone number';
      button.phone_number='';
      button.example=null;
  } 
  else if (button.type == 'QUICK_REPLY') {
      button.type = 'QUICK_REPLY';
      button.text = 'Quick Reply';
      button.example=null;
    }
    this.ButtonLimits(cardIndex);

    if (!this.phoneErrors[this.carouselCards.length - 1]) {
      this.phoneErrors[this.carouselCards.length - 1] = [];
    }
    }


  countButtons(actionType: string,cardIndex:number): number {
    debugger
    const allButtons = this.carouselCards[cardIndex].components ?.flatMap(component => component.buttons || []);

    // const allButtons = this.template.components
    // ?.flatMap(component => component.buttons || []);

    const filteredButtons = allButtons?.filter(button => button.type === actionType) || [];

    return filteredButtons.length;
  }


  get hasQuickReply() {
    return this.template.components.some(component => 
      component.buttons?.some(button => 
          button.type === 'QUICK_REPLY' && button.Text !='Stop promotions'|| 
         ( button.type === 'QUICK_REPLY' && button.Text =='Stop promotions')
      )
  );
    return this.template.components.buttons.quickReplyButtons.some(button => button.replyType == 1 || button.replyType === 0);
  }
  get CallToAction() {
    return this.carouselCards[0].components.some(component => 
        component.buttons?.some(button => 
            button.type === 'URL' || 
            button.type === 'PHONE_NUMBER' || 
            button.type === 'COPY_CODE'
        )
    );
}
get checkboxWebSite() {
  return this.template.components.some(component =>
      component.buttons?.some(button => button.type === 'URL')
  );
}

ButtonLimits(cardIndex: number) {
  debugger
    let cardButtonsCount = this.carouselCards[cardIndex].components[2].buttons.length;
    this.buttonLimits[cardIndex] = cardButtonsCount == 2;
}



OnURLTypeChange(button: WhatsAppButtonModel, cardIndex: number, buttonIndex: number): void {
  const currentButton = this.carouselCards[cardIndex].components[2].buttons[buttonIndex];

  if (currentButton.example) {
    currentButton.example = null;
  } 
  else{
    currentButton.example = [""];
  }

  this.checkValidationCard();
}

checkErrorForSubmit(){
  if(!this.isUrlInvalid && !this.isSampleUrlInvalid ){
  }
}

    isValidText(text) {
      const regex = /^(?=(?:\S*\s\S*\s\S*\s*)|$)(?!\s*$).*$/;
    
      const trimmedString = text?.replace(/\s+$/, "");
    
      return regex.test(trimmedString);
    }

    onHeaderChange(typeMedia: any,cardIndex:number): void {
      const header = this.carouselCards[cardIndex].components.find((c: any) => c.type === 'HEADER');

      header.format= typeMedia;
      if(header.format=='IMAGE'||header.format=='VIDEO'){
        this.errorAddMedia=true;
        this.errorStatusChanged.emit(this.errorAddMedia);
      }
      this.clearUploadedFile(cardIndex); 
      this.carouselChange.emit(this.carousel);
    }
  
  
    getSampleContent(variableIndex: number): string {
      return this.sampleContents[variableIndex] || ''; 
    }
  
    checkHeaderTextForEmpty(){
      if(!this.componentHeader.example.header_text[0]){
      const example = new WhatsAppExampleModel();
      example.header_text = [""];
      this.componentHeader.example = example;
      }
      if (this.componentHeader.example.header_text[0].length == 0 || this.componentHeader.example.header_text[0] == '') {
        this.errorAddSampleHeader=true;
      }
      else{
        this.errorAddSampleHeader=false;
      }
      this.errorsampleHeaderChange.emit(this.errorAddSampleHeader);
    }


  getSelection() {
    return window.getSelection()?.toString() || '';
  }
  
  
   insertAtCursor(text: string): void {
     const textarea = document.getElementById('body') as HTMLTextAreaElement;
     if (textarea) {
       const start = textarea.selectionStart;
       const end = textarea.selectionEnd;
       this.body = (this.body || '').slice(0, start) + text + (this.body || '').slice(end);
  
       setTimeout(() => {
        textarea.selectionStart = textarea.selectionEnd = start + text.length;
        textarea.focus();
      });
    } 
   }
  
  
  toggleEmojiPicker(): void {
    this.showEmojiPicker = !this.showEmojiPicker;
  }
  
  addEmoji(cardIndex,event: any,): void {
    this.carouselCards[cardIndex].components.find((c: any) => c.type === 'BODY').text += event.emoji.native;
  }
  

  
  updateValidationMessage() {
    this.totalCharacters = this.body.length;
    if (this.totalCharacters > this.maxCharacters) {
      this.validationMessage = `Message exceeds ${this.maxCharacters} characters limit!`;
    } else {
      this.validationMessage = '';
    }
  }

applyFormat(cardIndex: number, startTag: string, endTag: string): void {
  const textarea = document.getElementById('bodyText-' + cardIndex) as HTMLTextAreaElement;
  if (!textarea) return;

  const selectionStart = textarea.selectionStart;
  const selectionEnd = textarea.selectionEnd;

  const bodyCardComponent = this.carouselCards[cardIndex].components[1];
  const selectedText = bodyCardComponent.text.slice(selectionStart, selectionEnd);

  bodyCardComponent.text =
    bodyCardComponent.text.slice(0, selectionStart) +
    startTag + selectedText + endTag +
    bodyCardComponent.text.slice(selectionEnd);

  // Update textarea content manually if needed
  setTimeout(() => {
    textarea.focus();
    if (selectedText.length === 0) {
      textarea.selectionStart = selectionStart + startTag.length;
      textarea.selectionEnd = selectionStart + startTag.length;
    } else {
      textarea.selectionStart = selectionStart + startTag.length + selectedText.length + endTag.length;
      textarea.selectionEnd = textarea.selectionStart;
    }
  }, 0);
}

  
  trackByGroup(index: number, group: any): number {
    return index;
  }
  
  trackByInput(index: number, input: any): number {
    return index;
  }

  test(cardIndex){
    debugger
    this.carouselCards[cardIndex].components[1];
  }

  
  incrementedIndex(index: number): number {
    return index + 1;
  }
  
  handleFileInput(event: { target: { files: any[] } },cardIndex :number) {
    debugger
    const file = event.target.files[0];
    if (!file) return;

    const header = this.carouselCards[cardIndex].components.find((c: any) => c.type === 'HEADER');
    if (!header) return;

    this.fileToUpload = event.target.files[0];
    let formDataFile = new FormData();
  
    this.extFile = this.fileToUpload.name.substring(
      this.fileToUpload.name.lastIndexOf(".") + 1
    );
  
    if (header.format == "IMAGE") {
      if (this.extFile == "jpg" || this.extFile == "jpeg" || this.extFile == "png" ) {
        if (this.fileToUpload.size > 5 * 1024 * 1024) {
          Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'The uploaded image exceeds 5 MB. Please select a smaller image.',
          });
          this.element.nativeElement.value = "";
          event.target.files[0] = null;
          this.fileToUpload = null;
        } else {
          this.template.mediaLink = "";
          this.imageFlag = false;
          formDataFile.append("formFile", this.fileToUpload);
          this.GetHeaderHandle(formDataFile,cardIndex);
          this.template.mediaType = "image";
        }
      } else {
        this.element.nativeElement.value = "";
        event.target.files[0] = null;
        this.fileToUpload = null;
      }
    }
  
    if (header.format == "VIDEO") {
      if (this.extFile == "mp4") {
        if (this.fileToUpload.size > 16 * 1024 * 1024) {
          Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'The uploaded video exceeds 16 MB. Please select a smaller video.',
          });
          this.element.nativeElement.value = "";
          this.fileToUpload = null;
        } else if (this.fileToUpload.size == 0) {
          this.element.nativeElement.value = "";
          this.fileToUpload = null;
        } else {
          formDataFile.append("formFile", this.fileToUpload);
          this.GetHeaderHandle(formDataFile,cardIndex);
          this.template.mediaLink = "";
          this.videoFlag = false;
        }
      } else {
        this.element.nativeElement.value = "";
        event.target.files[0] = null;
        this.fileToUpload = null;
      }
    }
    this.carouselChange.emit(this.carousel);

  }
  
  GetHeaderHandle(file: FormData,cardIndex:number) {
    const component = this.carouselCards[cardIndex]?.components?.[0];
    if (component?.example?.header_handle) {
      component.example.header_handle[0] ="";
      component.example.mediaCard[0] ="";
    }




    this.http
        .post<WhatsAppHeaderUrl>(
            AppConsts.remoteServiceBaseUrl +
                "/api/services/app/WhatsAppMessageTemplate/GetWhatsAppMediaLink",
            file
        )
        .subscribe((result) => {
          debugger
            this.whatsAppHeaderHandle = new WhatsAppHeaderUrl();
            this.carouselCards[cardIndex].components[0].example.header_handle[0] = result.h;
            this.carouselCards[cardIndex].components[0].example.mediaCard[0] = result.infoSeedUrl;
            this.template.mediaLink = result.infoSeedUrl;
            this.whatsAppHeaderHandle = result;
            this.mediaPathIsEmptyError[cardIndex]=false;
            if(!this.carouselIsValidTemplate){
              this.checkValidationCard();
            }
            this.carouselChange.emit(this.carousel);
        });
              this.http
        .post<WhatsAppMediaID>(
            AppConsts.remoteServiceBaseUrl +
                "/api/services/app/WhatsAppMessageTemplate/GetWhatsAppMediaID",
            file
        )
        .subscribe((result) => {
          debugger
            var id = result.id;
            this.whatsAppMediaID = new WhatsAppMediaID();
            this.whatsAppMediaID =result;
            this.carouselCards[cardIndex].components[0].example.mediaID=this.whatsAppMediaID.id
        });
  
        this.componentHeader.type="HEADER";
        this.template.mediaLink;
        this.errorAddMedia=false;

      }
  
      senddata(){
        debugger;
        
        this.carouselCards;
        this.carousel;
        this.template;
        this.carouselChange.emit(this.carousel);

        this.template.components[2]= this.carousel;
        this.carouselChange.emit(this.carousel);

      }

    checkValidationSampilBody(){
      // this.carouselIsValidTemplate = true;
      debugger;
      let expectedBodyVarCount: number | null = null;
      
      const cards = this.template.components[1].cards;
      expectedBodyVarCount=cards[0].components[1].example?.body_text?.[0]?.length;
      let expectedButtonCount = null;
      let expectedButtonTypes = [];
      for (let cardIndex = 0; cardIndex < cards.length; cardIndex++) {
        const card = cards[cardIndex];
        const body = card.components[1];
        if (!body.text || body.text.trim() === '') {
            this.carouselIsValidTemplate = false;
            this.sampilBodyValid=false;
              this.onValidityChange();
            break;
          }
          else{
            this.sampilBodyValid=true;
          }
        if (!body.example?.body_text?.[0] || body.example.body_text[0].length === 0 || 
              body.example.body_text[0].some(item => !item || item.trim() === '')) {
          this.carouselIsValidTemplate = false;
          this.sampilBodyValid=false;
            this.onValidityChange();
          break;
          }
          else{
            this.sampilBodyValid=true;
          }
          const currentVarCount = body.example?.body_text?.[0]?.length || 0;
          if (expectedBodyVarCount !== null && currentVarCount !== expectedBodyVarCount) {
            this.errorUnEqualVaribleBody = true;
            this.carouselIsValidTemplate = false;
            this.sampilBodyValid = false;
            this.onValidityChange();
            return;
          }
            this.carouselIsValidTemplate = true;
            this.sampilBodyValid = true;
            this.errorUnEqualVaribleBody=false;
      }
      this.checkValidationCard() 
    }

checkValidationCard() {
  debugger;

  const cards = this.template?.components?.[1]?.cards || [];
  this.carouselIsValidTemplate = true;
  this.errorUnEqualVaribleBody = false;
  this.ButtonCountEqual = true;
  this.sampilBodyValid = true;

  let expectedButtonCount: number | null = null;
  let expectedButtonTypes: string[] = [];
  let expectedBodyVarCount: number | null = null;

  for (let cardIndex = 0; cardIndex < cards.length; cardIndex++) {
    const card = cards[cardIndex];
    const header = card.components?.[0];
    const body = card.components?.[1];
    const buttons = card.components?.[2]?.buttons || [];

    // Check body variables count
    const currentVarCount = body?.example?.body_text?.[0]?.length || 0;
    if (expectedBodyVarCount === null) {
      expectedBodyVarCount = currentVarCount;
    } else if (currentVarCount !== expectedBodyVarCount) {
      this.carouselIsValidTemplate = false;
      this.errorUnEqualVaribleBody = true;
      this.sampilBodyValid = false;
      break;
    }

    // Validate header example
    if (!header?.example?.header_handle?.[0] || header.example.header_handle[0].length === 0) {
      this.carouselIsValidTemplate = false;
      break;
    }

    if (!body?.text || body.text.trim() === '') {
      this.carouselIsValidTemplate = false;
      break;
    }

    const bodyVars = body.example?.body_text?.[0];
    if (!bodyVars || bodyVars.some(item => !item || item.trim() === '')) {
      this.carouselIsValidTemplate = false;
      this.sampilBodyValid = false;
      break;
    }

    if (cardIndex === 0) {
      expectedButtonCount = buttons.length;
      expectedButtonTypes = buttons.map(btn => btn.type);
    } else {
      if (buttons.length !== expectedButtonCount) {
        this.carouselIsValidTemplate = false;
        this.ButtonCountEqual = false;
        break;
      }

      for (let i = 0; i < buttons.length; i++) {
        if (buttons[i].type !== expectedButtonTypes[i]) {
          this.carouselIsValidTemplate = false;
          this.ButtonCountEqual = false;
          break;
        }
      }

      if (!this.carouselIsValidTemplate) break;
    }

    for (let buttonIndex = 0; buttonIndex < buttons.length; buttonIndex++) {
      const button = buttons[buttonIndex];

      if (!button.text || button.text.trim() === '') {
        this.carouselIsValidTemplate = false;
        break;
      }

      switch (button.type) {
        case 'URL':
          if (!button.url || !new RegExp(this.urlPattern).test(button.url)) {
            this.carouselIsValidTemplate = false;
            this.buttonURLError[cardIndex] = true;
            break;
          }
          if (button.example && (!button.example[0] || button.example[0].length === 0)) {
          // if (!button.example?.[0] || button.example[0].length === 0) {
            this.carouselIsValidTemplate = false;
            break;
          }
          break;

        case 'PHONE_NUMBER':
          if (
            !button.phone_number ||
            button.phone_number.length < 12 ||
            !this.isPhoneNumberValid(button.phone_number)
          ) {
            this.carouselIsValidTemplate = false;
            break;
          }
          break;

        case 'QUICK_REPLY':
          if (!button.text || button.text.length === 0) {
            this.carouselIsValidTemplate = false;
            break;
          }
          break;
      }

      if (!this.carouselIsValidTemplate) break;
    }

    if (!this.carouselIsValidTemplate) break;
  }

  // Call once at the end
  this.onValidityChange();
}

    validateUrl(url: string, cardIndex: number, buttonIndex: number): boolean {
      debugger
      if (!url) {
        this.buttonErrors[cardIndex][buttonIndex] = true;
        return false;
      }
      // Use either regex or URL constructor validation
      const pattern = new RegExp(
        '^(https?:\\/\\/)?' + // protocol
        '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain
        '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip
        '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port/path
        '(\\?[;&a-z\\d%_.~+=-]*)?' + // query
        '(\\#[-a-z\\d_]*)?$', 'i'
      );
    
      const isValid = !!pattern.test(url);
      this.buttonErrors[cardIndex][buttonIndex] = !isValid;
      
      return isValid;
    }
    
    onURLChange(event: any, cardIndex: number, buttonIndex: number) {
      debugger
      this.validateUrl(event.target.value, cardIndex, buttonIndex);
      this.checkValidationCard();
    }



    
// Initialize phone numbers
initializePhoneNumbers() {
  debugger
  this.carouselCards.forEach(card => {
    if (card.components[2]?.buttons) {
      card.components[2].buttons.forEach((button: any) => {
        if (button.type === 'PHONE_NUMBER') {
          // Initialize phoneObject if it doesn't exist
          if (!button.phoneObject && button.phone_number) {
            button.phoneObject = {
              number: button.phone_number.replace(/\D/g, '').slice(-9), // Last 9 digits
              internationalNumber: button.phone_number,
              nationalNumber: button.phone_number.replace(/^\+?\d+/, ''),
              countryCode: button.phone_number.startsWith('+966') ? 'sa' : 'jo',
              dialCode: button.phone_number.startsWith('+966') ? '+966' : '+962'
            };
          } else if (!button.phoneObject) {
            button.phoneObject = {
              number: '',
              internationalNumber: '',
              nationalNumber: '',
              countryCode: 'jo', // Default to Jordan
              dialCode: '+962'
            };
          }
        }
      });
    }
  });
}

onPhoneNumberChange(cardIndex: number, buttonIndex: number, event: any) {
  let phoneNumber = '';

  if (event?.number) {
    phoneNumber = event.number; 
  } else if (event?.e164Number) {
    phoneNumber = event.e164Number;
  } else if (event?.target?.value) {
    phoneNumber = event.target.value;
  } else if (typeof event === 'string') {
    phoneNumber = event;
  }
  if (!phoneNumber.startsWith('+')) {
    const defaultCountryCode = '+962'; 
    phoneNumber = defaultCountryCode + phoneNumber;
  }
  this.carouselCards[cardIndex].components[2].buttons[buttonIndex].phone_number = phoneNumber;

  const isValid = this.formatAndValidatePhoneNumber(phoneNumber, cardIndex, buttonIndex);
  if (!this.phoneErrors[cardIndex]) this.phoneErrors[cardIndex] = [];
  this.phoneErrors[cardIndex][buttonIndex] = !isValid;

  this.checkValidationCard();
}


  formatAndValidatePhoneNumber(phone: string, cardIndex: number, buttonIndex: number): boolean {
    if (!this.phoneErrors[cardIndex]) {
      this.phoneErrors[cardIndex] = [];
    }

    if (!phone) {
      this.phoneErrors[cardIndex][buttonIndex] = false;
      return false;
    }

    const cleaned = phone.replace(/\D/g, '');
    let formattedNumber = '';
    let isValid = false;

    if (cleaned.startsWith('962') && cleaned.length === 12) {
      formattedNumber = `+${cleaned}`;
      isValid = true;
    } else if (cleaned.startsWith('966') && cleaned.length === 12) {
      formattedNumber = `+${cleaned}`;
      isValid = true;
    } else if (cleaned.startsWith('07') && cleaned.length === 10) {
      formattedNumber = `+962${cleaned.substring(1)}`;
      isValid = true;
    } else if (cleaned.startsWith('05') && cleaned.length === 10) {
      formattedNumber = `+966${cleaned.substring(1)}`;
      isValid = true;
    } else if (cleaned.startsWith('7') && cleaned.length === 9) {
      formattedNumber = `+962${cleaned}`;
      isValid = true;
    } else if (cleaned.startsWith('5') && cleaned.length === 9) {
      formattedNumber = `+966${cleaned}`;
      isValid = true;
    }

    this.carouselCards[cardIndex].components[2].buttons[buttonIndex].phone_number =
      formattedNumber || phone;

    this.phoneErrors[cardIndex][buttonIndex] = !isValid;
    return isValid;
  }

  isPhoneNumberValid(phone: string): boolean {
    if (!phone) return true;
    return phone.startsWith('+962') || phone.startsWith('+966');
  }


insertVariable(index: number) {
  const currentText = this.carouselCards[index].components[1].text;
  const variableCount = (currentText.match(/{{\d+}}/g) || []).length;
  if(variableCount>15){
    return
  }
  const variableTag = `{{${variableCount + 1}}}`;

  this.carouselCards[index].components[1].text += ` ${variableTag}`;

  const example = this.carouselCards[index].components[1].example;
  if (!example.body_text) {
    example.body_text = [];
  }

  if (!example.body_text[0]) {
    example.body_text[0] = [];
  }

  example.body_text[0].push('');

  this.carouselCards[index].components[1].example.body_text = example.body_text;

  this.checkValidationSampilBody();
  this.checkValidationCard();
}

deleteLastVariable(index: number) {
  const card = this.carouselCards[index].components[1];
  let text = card.text;

  const matches = text.match(/{{\d+}}/g) || [];

  if (matches.length === 0) {
    return; 
  }

  const lastVariable = matches[matches.length - 1];

  const lastIndex = text.lastIndexOf(lastVariable);
  if (lastIndex !== -1) {
    text = text.slice(0, lastIndex) + text.slice(lastIndex + lastVariable.length);
    text = text.replace(/\s{2,}/g, ' ').trim(); 
  }

  card.text = text;

  const example = card.example;
  if (example.body_text && example.body_text[0]) {
    example.body_text[0].pop();
  }

  this.checkValidationSampilBody();
  this.checkValidationCard();

}


  countVariables(text: string): number {
    const matches = text.match(/\{\{\d+\}\}/g);
    return matches ? matches.length : 0;
  }


  onBodyTextChange(text: string, index: number) {
    const variableCount = this.countVariables(text);
    const cleanLength = text.replace(this.variablePattern, '').length;

    this.isTextValid = variableCount === 0 || cleanLength / variableCount >= 10;

  }

trackByIndex2(index: number): number {
  return index;
}

}