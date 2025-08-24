import { debounce } from 'lodash';
import { AppSetup, AppSetupDTO, ButtonAuthontcation, CardsModel, CatalogFormat, CatalogFormat2, CodeDeliverySetupDTO, FacebookTemplateServiceProxy, HeaderContent, MessageValidityPeriod, ParametersModel, QuickReplyButton, QuickReplyType,TemplateContentAuthentication, Text, WhatsAppButtonModel, WhatsAppExampleModel, WhatsAppHeaderUrl, WhatsAppMessageTemplateServiceProxy,  } from './../../../../shared/service-proxies/service-proxies';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild, Type, Injector } from '@angular/core';
import { SafeHtml } from '@node_modules/@angular/platform-browser';
import { ActivatedRoute, Router, Routes } from '@node_modules/@angular/router';
import { ButtonsTemplate, CallToActionButton, FacebookTemplateDto, MessageTemplateModel, TemplateContent, TemplateNameLanguageFacebook, TypeOfAction, URLType, WhatsAppComponentModel } from '@shared/service-proxies/service-proxies';
import { FacebookTemplateComponent } from '../Facebook-Template/Facebook-Template.component';
import { TemplateService } from '../Facebook-Template/Template.service';
import { CountryISO, PhoneNumberFormat } from 'ngx-intl-tel-input';
import { AuthenticationCodeDeliverySetupComponent } from './AuthenticationCodeDeliverySetup/AuthenticationCodeDeliverySetup.component';
import Swal from 'sweetalert2';
import { ButtonsTemplateComponent } from './ButtonsTemplate/ButtonsTemplate.component';
import { MessageService, NotifyService } from '@node_modules/abp-ng2-module/public-api';

@Component({
  selector: 'app-edit-template-facebook',
  templateUrl: './edit-template-facebook.component.html',
  styleUrls: ['./edit-template-facebook.component.css']
})
export class EditTemplateFacebookComponent implements OnInit {
   // Carousel specific properties
   currentIndex: number = 0;
  //  carousel: any;
  //  componentBody: any;
   carousel: WhatsAppComponentModel = new WhatsAppComponentModel();
    CardIsValidTemplate:boolean=false;

  isModalVisible: boolean = false;
  selectedLanguage: string = 'English';
  
  status:string;

  carouselCards: any[] = [];

  templateName: string = '';
  language: string = 'English';
  header: string = 'None';
  variable: string | null = null;
  headerContent: string = ''; 
  headerLocation: string = ''; 

  languages: string[] = ['English', 'Arabic'];
  showEmojiPicker: boolean = false;
  fileToUpload: File | null = null; 
  extFile: string = ''; 
  uploadedImageUrl: string | ArrayBuffer | null = null;
  uploadedVideoUrl: string | ArrayBuffer | null = null; 
  uploadedDocumentUrl: string | ArrayBuffer | null = null; 
  mediaErorr:boolean=false;
  selectedEmoji: string = '';
  variableCounter = 1;
  usedVariables = new Set<number>();
  variableAdded: boolean = false; 
  variables: { number: number }[] = [];
  // componentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
  listOfVariables = [];
  formattedTextBody: SafeHtml;
  formattedText: string;
  sampleContents: { [key: number]: string } = {}; 
   maxCharacters: number = 1024;
   validationMessage: string = '';
   isSampleErorr:boolean=false;

  isTextValid = true;
  sanitizer: any;
  maxVariableRatio: number = 10; 
  showValidationAlert: boolean = false; 
  isSubmitted: boolean = false;
  isUsedTemplateName: boolean = true;
  checkTemplate: MessageTemplateModel[] = [new MessageTemplateModel()];

  componentButtonForView: WhatsAppComponentModel = new WhatsAppComponentModel();

  isNameUsed: boolean = false;
  isRecentlyDeleted: boolean = false;
  totalCharacters: number = 0;

  PhoneNumberFormat = PhoneNumberFormat;
  preferredCountries: CountryISO[] = [
      CountryISO.SaudiArabia,
      CountryISO.Jordan,
  ];
  selectedButtonType: string = '';
  isCallButtonDisabled: boolean = false;
  isCopyOfferCodeDisabled: boolean = false;
  isVisitWebsiteDisabled: boolean = false;
  isMarketinOptOutDisabled: boolean = false;
  isCustomDisabled: boolean = false; 
  isCompleteFlowDisabled: boolean = false;
  urlPattern = /^https?:\/\/.+$/;
  MarketingOptOutCheckboxStates: boolean ;
  visitWebsiteCheckboxStates: boolean ;
  filteredButtonsForPreview = [];


  isDropdownOpen: boolean=true ;
  
  showWarningMessage: boolean = false;

  isUrlInvalid:boolean;
  checked = false;
  indeterminate = false;
  labelPosition: 'before' | 'after' = 'after';
  disabled = false;


  selectedCategory: string;
  selectedMessageType: string;

  isCustomValidity = false;

  dynamicVariables: string[] = [];


  selectedDeliveryOption: string = '';
  zeroTapConsent: boolean = false;
  showAppSetup: boolean = true;
  packageName: string = '';
  appSignature: string = '';
  apps: { packageName: string; appSignature: string }[] = [{ packageName: '', appSignature: '' }];
  isInvalid: boolean = false;
  isSignatureInvalid: boolean = false;
  private androidAppIdRegex = /^[a-zA-Z][a-zA-Z0-9_]*\.[a-zA-Z0-9_]+(\.[a-zA-Z0-9_]+)*$/;
  private signatureRegex = /^[a-zA-Z0-9/+=]{11}$/;
  isDuplicateError: boolean = false;

  maxApps = 5;
  templateId:string;

  selectedOption: string = 'zeroTap'; // Default selection
  zeroTapAgreement: boolean = false;  
  
  isPlayVisible: boolean = false;

  selectedCatalogFormat: string = 'CatalogMessage'; 
  CatalogButton:string="View items";

  componentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
  componentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
  componentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
  componentButton:WhatsAppComponentModel = new WhatsAppComponentModel();
  parameters:ParametersModel=new ParametersModel();
  Button:WhatsAppButtonModel=new WhatsAppButtonModel();
  Buttons:WhatsAppComponentModel = new WhatsAppComponentModel();
  // carousel:WhatsAppComponentModel = new WhatsAppComponentModel();

  componentCardBody: WhatsAppComponentModel = new WhatsAppComponentModel();
  componentCardHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
  componentCardButton:WhatsAppComponentModel = new WhatsAppComponentModel();

      notify: NotifyService;


   whatsAppHeaderHandle: WhatsAppHeaderUrl;

    // whatsAppHeaderHandle: WhatsAppHeaderUrl = new WhatsAppHeaderUrl();
    mediaUrl: WhatsAppExampleModel = new WhatsAppExampleModel();
    message: MessageService;

  @ViewChild(AuthenticationCodeDeliverySetupComponent)  AuthComponent!: AuthenticationCodeDeliverySetupComponent;
  @ViewChild(ButtonsTemplateComponent) buttonsTemplateComponent: ButtonsTemplateComponent | undefined;

  packageNameArray:string[];
  signatureHashArray:string[];

  packageNameAndSignatureHashObj:any;
  codeExpirationMinutes: string;
  otpCode: string;
  ctaDisplayName: string='';
  otpType:string;
  code: string;

    template: MessageTemplateModel = new MessageTemplateModel();
    cards:WhatsAppComponentModel = new WhatsAppComponentModel();
    templateLastEdit: MessageTemplateModel = new MessageTemplateModel();
    template2: MessageTemplateModel = new MessageTemplateModel();



    showChildComponent = false;

  // selectedCategory: string = 'Marketing';
  // selectedMessageType: string = 'Custom'

  components = new WhatsAppComponentModel();

  selectedFormat: string | null = "CatalogMessage";
  isLoading: boolean = false; 
  submit:boolean=false;
  errorAddSampleBody: boolean=false;
  errorAddSampleHeader: boolean=false;
  errorBodyTextIsEmpty:boolean=false;
  validSubmit: boolean=true;
  isModalVisibleEdit: boolean=false;
  isFormInvalidFromBottonChild:boolean=false;

  autofillText: string = 'Autofill';  
  copyCodeText: string = 'Copycode';  

  @ViewChild ('SubmitForReview') SubmitForReview !:TemplateRef<any>;


  checkifCanSubmitToCreateTemplate(){

    if(this.template.category =='MARKETING' || this.template.category =='UTILITY'){
    // !this.isUsedTemplateName && 
    if(this.template.id!=null){
      this.isUsedTemplateName=false;
      //  this.errorAddSampleBody=false;
    }
      if(!this.errorBodyTextIsEmpty && !this.isFormInvalidFromBottonChild && !this.isUsedTemplateName && !this.mediaErorr && !this.errorAddSampleBody && !this.errorAddSampleHeader){
        this.validSubmit=true;
    }
    else{
      this.validSubmit=false;
    }
  }
  else if(this.template.category =='AUTHENTICATION'){
    if(this.template.id!=null){
      this.isUsedTemplateName=false;
    }
    this.validSubmit=this.submit;
    if(this.isUsedTemplateName ){
      this.validSubmit=false;
     }

    }
    else if(this.template.sub_category =='carousel' ){
      if(this.CardIsValidTemplate){
        this.validSubmit=false;
      }
    }
  }
  CardComponent(value: boolean) {
    this.CardIsValidTemplate = value;
  }

  handleErrorBodyIsEmpty(value: boolean) {

    this.errorBodyTextIsEmpty = value;
    this.checkifCanSubmitToCreateTemplate();
  }

  handleSubmit(submitValue: boolean) {
    this.submit = submitValue; 
    this.checkifCanSubmitToCreateTemplate();
  }

  onErrorBodyChange(value: boolean) {
    this.errorAddSampleBody = value;
    this.checkifCanSubmitToCreateTemplate();
  }

  onErrorHEaderTextChange(value: boolean) {
    this.errorAddSampleBody = value;
    this.checkifCanSubmitToCreateTemplate();
  }
  
  
  onErrorHeaderChange(value: boolean) {
    this.errorAddSampleHeader = value;
    this.checkifCanSubmitToCreateTemplate();
  }
  onErrorStatusChanged(isError: boolean) {
    this.mediaErorr = isError; 
    this.checkifCanSubmitToCreateTemplate();
  }
  handleifNameTemplteUsedChange(isUsed: boolean) {
    this.isUsedTemplateName = isUsed;  
    this.checkifCanSubmitToCreateTemplate();
  }
  onIsFormInvalidChange(status: boolean) {
    this.isFormInvalidFromBottonChild = status;
    this.checkifCanSubmitToCreateTemplate();
  }
  handleLanguageChange(language: string) {
    this.template.language=language;
    this.language = language; 
    if (this.buttonsTemplateComponent) {
      this.buttonsTemplateComponent.updateButtonTextBasedOnLanguage(language);
    }
  }

  senddata(){
    debugger
    this.template;
    this.carousel;
  }

  getapi(){

    this.componentHeader = this.template.components.find(
      (i: { type: string }) => i.type === "HEADER"
    )||null;

    this.componentBody = this.template.components.find(
      (i: { type: string }) => i.type === "BODY"
    )||null;
    this.componentFooter = this.template.components.find(
     (i: { type: string }) => i.type === "FOOTER"
    )||null;
    this.Buttons = this.template.components.find(
     (component: WhatsAppComponentModel) => component.type === "BUTTONS"
    )||null;

    this.Button = this.Buttons?.buttons?.find(
     (i: { type: string }) => i.type === "URL"
    )||null;

  if(this.selectedCategory=='AUTHENTICATION'){
    this.Button = this.Buttons?.buttons?.find(
      (i: { type: string }) => i.type === "URL"
    )||null;
  }

  const uri = new URL(this.Button.url);

 const queryParams = uri.searchParams;
 
 const packageName = queryParams.get("package_name");
 const signatureHash = queryParams.get("signature_hash");
 
  this.packageNameArray = packageName ? packageName.split(",") : [];
 
  this.signatureHashArray = signatureHash ? signatureHash.split(",") : [];

  const combinedArray = this.packageNameArray.map((name, index) => ({
    packageName: name,
    signatureHash: this.signatureHashArray[index] || null, // Handle mismatch in array lengths
  }));
  this.packageNameAndSignatureHashObj=combinedArray;

  const parsedUrl = new URL(this.Button.url);

  const queryParam = new URLSearchParams(parsedUrl.search);

  const data: Record<string, string> = {};
  queryParam.forEach((value, key) => {
    data[key] = value;
  });
   this.ctaDisplayName = queryParams.get("cta_display_name");
   this.codeExpirationMinutes = queryParams.get("code_expiration_minutes");
   this.code = queryParams.get("code");
   this.otpType = queryParams.get("otp_type") || "ZERO_TAP"||"COPY_CODE";
  if(this.codeExpirationMinutes!=null){
    this.isCustomValidity=true;
  }

  }

  handleFormatChange(format: string): void {
    this.selectedFormat = format;
    this.Button = this.Buttons ?.buttons?.find(
      (i: { type: string }) => i.type === "CATALOG"
      )||null;
      if(this.selectedFormat=='MultiProductMessage'){
        this.Button.text="View catalog";
      }
      else{
        this.Button.text="View items";
      }
  }

  next() {
    this.router.navigate(['/app/main/EditTemplateFacebook'], {
      queryParams: {
        selectedCategory: this.selectedCategory,
        selectedMessageType: this.selectedMessageType,
      },
    });
  }

  Discard(){
    this.router.navigate(['/app/main/messageTemplate']);
    
  }
  Previous(){
    localStorage.setItem('category', JSON.stringify(this.template.category));
    localStorage.setItem('sub_category', JSON.stringify(this.template.sub_category));
    this.router.navigate(['/app/main/start']);
  }

  onDataChanged(data: { category: string; messageType: string }) {
    this.selectedCategory = data.category;
    this.selectedMessageType = data.messageType;
  }

  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private router: Router,
    private templateService:TemplateService,
    private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy ,
  ) {
    this.whatsAppHeaderHandle = new WhatsAppHeaderUrl(); 
            // this.notify = injector.get(NotifyService);
  }

  handleWhatsAppHeader(newHeaderHandle: WhatsAppHeaderUrl) {
    this.whatsAppHeaderHandle = newHeaderHandle;
  }

  toggleIcons(): void {
    this.isPlayVisible = !this.isPlayVisible;
  }

  MarkCustom(){
    debugger;
  const Body =new WhatsAppComponentModel()
  Body.type = "BODY" ; 
  Body.text = "hello" ; 
  this.template.components.push(Body);

  const FOOTER =new WhatsAppComponentModel()
  FOOTER.type = "FOOTER" ; 
  this.template.components.push(FOOTER);

  const HEADER = new WhatsAppComponentModel();
  HEADER.type = "HEADER";
  HEADER.format = "None";
  // const example = new WhatsAppExampleModel();
  // example.header_handle = [""];

  // HEADER.example = example;
  this.template.components.push(HEADER);


  this.Buttons = new WhatsAppComponentModel();
  this.Buttons.type = "BUTTONS";
  this.Buttons.buttons = [];
  this.template.components.push(this.Buttons);

  this.componentFooter = this.template.components.find(
    (i: { type: string }) => i.type === "FOOTER"
   )||null;
  this.componentBody = this.template.components.find(
   (i: { type: string }) => i.type === "BODY"
  )||null;
 
  this.Buttons = this.template.components.find(
   (i: { type: string }) => i.type === "BUTTONS"
  )||null;

  this.CardIsValidTemplate=true;
  }

    MarkCarousel(){
        const Body =new WhatsAppComponentModel()
        Body.type = "BODY" ; 
        Body.text = "Hello" ; 
        this.template.components.push(Body);
                
        // Now create the carousel
        const carousel = new WhatsAppComponentModel();
        carousel.type = "carousel";
                
        // Create cards for the carousel
        const cards: CardsModel[] = [];
                
        // Create first card
        const firstCard = new CardsModel();
        firstCard.components = [];
                
        // Add header to first cardsasanmsa
        const cardHeader = new WhatsAppComponentModel();
        cardHeader.type = "HEADER";
        cardHeader.format = "IMAGE"; 
        const cardHeaderExample = new WhatsAppExampleModel();
        cardHeaderExample.header_handle = [""];
        cardHeaderExample.mediaCard = [""];
        cardHeader.example = cardHeaderExample;
        firstCard.components.push(cardHeader);
                
        // Add body to first card
        const cardBody = new WhatsAppComponentModel();
        cardBody.type = "BODY";
        cardBody.text = "hello";
        
        const cardBodyExample = new WhatsAppExampleModel();
        cardBodyExample.body_text = [[]];
        cardBody.example = cardBodyExample;
        firstCard.components.push(cardBody);
                
        // Add buttons to first card
        const cardButtons = new WhatsAppComponentModel();
        cardButtons.type = "BUTTONS";
        cardButtons.buttons = [];
                
        // Add quick reply button
        const quickReplyButton = new WhatsAppButtonModel();
        quickReplyButton.type = "QUICK_REPLY";
        quickReplyButton.text = "Quick Reply";
        cardButtons.buttons.push(quickReplyButton);
                
        // Add URL button
        // const urlButton = new WhatsAppButtonModel();
        // urlButton.type = "URL";
        // urlButton.text = "visit Website";
        // urlButton.url = "https://www.Example.com/";
        // // urlButton.example = ["https://www.youtube.com/"];
        // cardButtons.buttons.push(urlButton);
                
                
        firstCard.components.push(cardButtons);
                
          cards.push(firstCard);
       
          carousel.cards = cards;
      
          this.template.components.push(carousel);
      
          this.componentBody = this.template.components.find(
            (i: { type: string }) => i.type === "BODY"
          )||null;
        
        this.carousel = this.template.components.find(
          (component: WhatsAppComponentModel) => component.type === "carousel"
        )||null;
      

        // if (this.template?.components) {
        //   // Find the body component
        //   this.componentBody = this.template.components.find((c: any) => c.type === 'BODY');
          
        //   // Find the carousel component
        //   const carouselComponent = this.template.components.find((c: any) => c.type === 'carousel');
        //   if (carouselComponent) {
        //     this.carousel = carouselComponent;
        //     // this.carousel.variables = {
        //     //   '{{1}}': 'Pablo',
        //     //   '{{2}}': '20%',
        //     //   '{{3}}': '20OFF'
        //     // };
        //   }
        // }
      }
    updateOtpType(newOtpType: string): void {
      this.otpType = newOtpType;
    }
   Authentication(){    
  this.template.message_send_ttl_seconds=null;
  const Body = new WhatsAppComponentModel();
    Body.type = "BODY"; 
    Body.text = "{{1}} is your verification code."; 
    Body.add_security_recommendation=true;
    this.template.components.push(Body);


  const FOOTER =new WhatsAppComponentModel()
  FOOTER.type = "FOOTER" ;
  FOOTER.code_expiration_minutes = null ; 

  this.template.components.push(FOOTER);
  
  this.Buttons = new WhatsAppComponentModel();
  this.Buttons.type = "BUTTONS";
  this.Buttons.buttons = [];
  this.template.components.push(this.Buttons);

  const button = new WhatsAppButtonModel();
  button.type = "URL";

  this.otpType = "ZERO_TAP";
  this.ctaDisplayName = "Autofill";
  this.codeExpirationMinutes = null; 
  const queryParams: { [key: string]: string } = {
    otp_type: this.otpType,
    cta_display_name: this.ctaDisplayName,
    code_expiration_minutes: this.codeExpirationMinutes,
    code: "otp{{1}}",
  };
    const url = new URLSearchParams(queryParams).toString();
  
    button.url = `https://www.whatsapp.com/otp/code/?${url}`;
  
  button.text = "Copy code";

  this.Buttons.buttons.push(button);

  this.componentFooter = this.template.components.find(
    (i: { type: string }) => i.type === "FOOTER"
  )||null;
  this.componentBody = this.template.components.find(
  (i: { type: string }) => i.type === "BODY"
  )||null;

  this.componentButton = this.template.components.find(
    (i: { type: string }) => i.type === "BUTTONS"
  )||null;

    this.Button = this.componentButton?.buttons?.find(
    (i: { type: string }) => i.type === "URL"
  )||null;

    }
    Utility(){

    this.template.message_send_ttl_seconds=null;
    const Body = new WhatsAppComponentModel();
     Body.type = "BODY"; 
     Body.text = "Hello"; 
     Body.add_security_recommendation=true;
     this.template.components.push(Body);
 
 
   const FOOTER =new WhatsAppComponentModel()
   FOOTER.type = "FOOTER" ;
   FOOTER.code_expiration_minutes = null ; 
   this.template.components.push(FOOTER);

    const HEADER = new WhatsAppComponentModel();
    HEADER.type = "HEADER";
    HEADER.format = "None";
    const example = new WhatsAppExampleModel();
    example.header_text = [""];
    HEADER.example = example;
    this.template.components.push(HEADER);
    this.Buttons = new WhatsAppComponentModel();
    this.Buttons.type = "BUTTONS";
    this.Buttons.buttons = [];
    this.template.components.push(this.Buttons);

    this.Buttons = this.template.components.find(
      (component: WhatsAppComponentModel) => component.type === "BUTTONS"
    )||null;
    
    this.componentFooter = this.template.components.find(
      (i: { type: string }) => i.type === "FOOTER"
     )||null;
    this.componentBody = this.template.components.find(
     (i: { type: string }) => i.type === "BODY"
    )||null;
   
    this.Buttons = this.template.components.find(
     (i: { type: string }) => i.type === "BUTTONS"
    )||null;

    }

    MarketingCatalog(){
    const Body = new WhatsAppComponentModel();
    Body.type = "BODY"; 
    Body.text = "Hello"; 
    Body.add_security_recommendation=true;
    this.template.components.push(Body);


   const FOOTER =new WhatsAppComponentModel()
   FOOTER.type = "FOOTER" ;
   FOOTER.code_expiration_minutes = null ; 
   this.template.components.push(FOOTER);

   const HEADER = new WhatsAppComponentModel();
   HEADER.type = "HEADER";
   HEADER.format = "None";
   const example = new WhatsAppExampleModel();
   example.header_text = [""];
   HEADER.example = example;
   this.template.components.push(HEADER);

    this.Buttons = new WhatsAppComponentModel();
    this.Buttons.type = "BUTTONS";
    this.Buttons.buttons = [];
    this.template.components.push(this.Buttons);

    this.Buttons = this.template.components.find(
      (component: WhatsAppComponentModel) => component.type === "BUTTONS"
    )||null;
    
    const button = new WhatsAppButtonModel();
    button.type = "CATALOG";
    this.Buttons.buttons.push(button);
    
   this.Button = this.Buttons ?.buttons?.find(
    (i: { type: string }) => i.type === "CATALOG"
    )||null;

    if(this.selectedFormat=='MultiProductMessage'){
      this.Button.text="View catalog";
    }
    else{
      this.Button.text="View items";
    }

    this.Buttons = this.template.components.find(
      (component: WhatsAppComponentModel) => component.type === "BUTTONS"
    )||null;

    this.Button = this.Buttons ?.buttons?.find(
      (i: { type: string }) => i.type === "CATALOG"
      )||null;
  
      this.componentFooter = this.template.components.find(
        (i: { type: string }) => i.type === "FOOTER"
       )||null;
      this.componentBody = this.template.components.find(
       (i: { type: string }) => i.type === "BODY"
      )||null;

   this.template.sub_category ='PRODUCT_MESSAGE'

    }

  ngOnInit(): void {
    debugger;
      this.route.queryParams.subscribe((params) => {
      this.templateId = params['templateId'];

      this.selectedCategory = params['selectedCategory'];  
      this.selectedMessageType = params['selectedMessageType'];
      
      if(this.templateId!=null){
        this._whatsAppMessageTemplateServiceProxy
        .getTemplateByWhatsAppId(this.templateId )
        .subscribe((result) => {
            this.template=result;
            debugger;
              this.CardIsValidTemplate=true;

            this.selectedCategory = this.template.category;
            this.selectedMessageType = this.template.sub_category;
  
            this.componentHeader = this.template.components.find(
              (i: { type: string }) => i.type === "HEADER"
            )||null;
        
            this.componentBody = this.template.components.find(
              (i: { type: string }) => i.type === "BODY"
          )||null;
                      
          this.carousel = this.template.components.find(
            (i: { type: string }) => i.type === "carousel"
          )||null;

            if(this.carousel!=null){
              this.template.sub_category='carousel';
              this.selectedMessageType = this.template.sub_category;
              return;
            }
          this.componentFooter = this.template.components.find(
            (i: { type: string }) => i.type === "FOOTER"
          )||null;
          
          this.Buttons = this.template.components.find(
            (component: WhatsAppComponentModel) => component.type === "BUTTONS"
          )||null;

          debugger;
          
          if(this.Buttons){
          this.Button = this.Buttons.buttons.find(
            (button: WhatsAppButtonModel) => button.type === "CATALOG"
          )||null;
          if(this.Button){
            this.template.sub_category="PRODUCT_MESSAGE";
          }
        }

          if(!this.componentFooter){
            const FOOTER =new WhatsAppComponentModel()
            FOOTER.type = "FOOTER" ; 
            this.template.components.push(FOOTER);
            this.componentFooter = this.template.components.find(
              (i: { type: string }) => i.type === "FOOTER"
            )||null;
          }

          if(!this.Buttons){
            this.Buttons = new WhatsAppComponentModel();
            this.Buttons.type = "BUTTONS";
            this.Buttons.buttons = [];
            this.template.components.push(this.Buttons);   

            this.Buttons = this.template.components.find(
              (component: WhatsAppComponentModel) => component.type === "BUTTONS"
            )||null;       
          }
          if(!this.componentHeader){
            const HEADER = new WhatsAppComponentModel();
            HEADER.type = "HEADER";
            HEADER.format = "None";
            const example = new WhatsAppExampleModel();
            example.header_text = [""];
            HEADER.example = example;
            this.template.components.push(HEADER);

            this.componentHeader = this.template.components.find(
              (i: { type: string }) => i.type === "HEADER"
            )||null;
          }
          if(!this.componentBody){
            const Body =new WhatsAppComponentModel()
            Body.type = "BODY" ; 
            this.template.components.push(Body);
          }
          this.componentBody = this.template.components.find(
            (i: { type: string }) => i.type === "BODY"
        )||null;
        });

        if(this.selectedCategory=='AUTHENTICATION'){

          this.Button = this.Buttons?.buttons?.find(
            (i: { type: string }) => i.type === "URL"
          )||null;

          const uri = new URL(this.Button.url);
        
          const queryParams = uri.searchParams;
          
          const packageName = queryParams.get("package_name");
          const signatureHash = queryParams.get("signature_hash");
          
          this.packageNameArray = packageName ? packageName.split(",") : [];
          this.signatureHashArray = signatureHash ? signatureHash.split(",") : [];
          
          const combinedArray = this.packageNameArray.map((name, index) => ({
            packageName: name,
            signatureHash: this.signatureHashArray[index] || null, 
          }));
          
          if (combinedArray.length === 0) {
            combinedArray.push({
              packageName: "",
              signatureHash: "",
            });
            
          }
          
          this.packageNameAndSignatureHashObj = combinedArray;
          
          const parsedUrl = new URL(this.Button.url);
          // Get query parameters
          const queryParam = new URLSearchParams(parsedUrl.search);
        
          // Extract query parameters into an object
          const data: Record<string, string> = {};
          queryParam.forEach((value, key) => {
            data[key] = value;
          });
           this.ctaDisplayName = queryParams.get("cta_display_name");
           this.codeExpirationMinutes = queryParams.get("code_expiration_minutes");
           this.code = queryParams.get("code");
           this.otpType = queryParams.get("otp_type") || "ZERO_TAP";          
        }
        this.validSubmit=true;
      }
      else{
        debugger
        this.template = new MessageTemplateModel();
        this.template.components=[];
       //  this.template.components.buttons=[];template.selectedCategory.toString()
        this.template.category = this.selectedCategory.toString();
        this.template.sub_category =this.selectedMessageType.toString();
        

        if(this.selectedCategory=='MARKETING'&&  this.selectedMessageType=='carousel'){
          this.MarkCarousel();
        }

        else if(this.selectedCategory=='MARKETING' && this.selectedMessageType=='CUSTOM'){
          this.MarkCustom();
            this.CardIsValidTemplate=true;
        }
        else if(this.selectedCategory=='AUTHENTICATION'){
          const HEADER = new WhatsAppComponentModel();
          HEADER.type = "HEADER";
          HEADER.format = "None";
          const example = new WhatsAppExampleModel();
          example.header_text = [""];
          HEADER.example = example;
          this.componentHeader=HEADER;
          this.Authentication();
          this.CardIsValidTemplate=true;
        }
        else if(this.selectedCategory=='UTILITY'){
          this.Utility();
            this.CardIsValidTemplate=true;
        }
        else if(this.selectedMessageType=='PRODUCT_MESSAGE'){
          this.MarketingCatalog();
            this.CardIsValidTemplate=true;
        }

        this.checkifCanSubmitToCreateTemplate();
      }
    });

  }






 
//  updateButtonTextBasedOnLanguage() {
//   setTimeout(() => {
//     for (const button of this.facebookTemplateDto.buttons.quickReplyButtons) {
//       if (button.replyType == 0) {
//         if (this.language === 'English') {
//           button.buttonText = 'Stop promotions';
//           button.footerText = 'Not interested? Tap Stop promotions';
//         } else if (this.language === 'Arabic') {
//           button.buttonText = 'إيقاف عمليات الترويج';
//           button.footerText = 'ألست مهتمًا؟ اضغط على  إيقاف عمليات الترويج';
//         }
//       }
//     }
//   }, 0); 
// }

// onLanguageChange() {
//   this.updateButtonTextBasedOnLanguage();
// }



// Function to check for duplicate (packageName, appSignature) pairs


  addSecurityRecommendation = false; // To bind the checkbox for security recommendation
  addExpirationTime = false;         // To bind the checkbox for expiration time
  minutes = 1;                       // Initial value for the minutes input
  minutesError = false;              // Error flag for minutes validation

  // Toggle visibility of the minutes input field based on checkbox state
  // toggleMinutesInput() {
  //   if (!this.addExpirationTime) {
  //     this.minutes = 1;              // Reset minutes if checkbox is unchecked
  //     this.minutesError = false;     // Reset error if unchecked
  //   }
  // }

  validateMinutes() {
    if (this.minutes < 1 || this.minutes > 90) {
      this.minutesError = true;
    } else {
      this.minutesError = false;
    }
  }

  showModal(): void {
    this.isModalVisible = true;
  }
  
  cancelModal(): void {
    this.isModalVisible = false;
  }
  showModalforEdit(): void {
    this.isModalVisibleEdit= true;
  }
  cancelModalforEdit(): void {
    this.isModalVisibleEdit = false;
  }
  
  confirmSubmission(): void {
    debugger
    this.componentHeader = this.template.components.find(
      (i: { type: string }) => i.type === "HEADER"
     )||null;

    this.componentFooter = this.template.components.find(
      (i: { type: string }) => i.type === "FOOTER"
     )||null;
    this.componentBody = this.template.components.find(
     (i: { type: string }) => i.type === "BODY"
    )||null;
   
    this.Buttons = this.template.components.find(
     (i: { type: string }) => i.type === "BUTTONS"
    )||null;

    if(this.template.category=="AUTHENTICATION"){
      this.AuthComponent.constructWhatsAppUrl();

      this.template.components = this.template.components.filter(
          (i: { type: string }) => i.type !== "HEADER"
        );
  
      this.componentFooter.text=null;
      if (this.componentBody) {
        if (!this.componentBody.example) {
          this.componentBody.example = new WhatsAppExampleModel();
        }
      if (!Array.isArray(this.componentBody.example.body_text)) {
        this.componentBody.example.body_text = [];
      }
      this.componentBody.example.body_text.push(["12345"]);
    }
      // this.componentBody.text = null;
      // this.componentBody.example=null;
      // this.template.sub_category=null;
    
    }
// edit to car
else if (this.template.category === "MARKETING" && this.template.sub_category === 'carousel') {
  debugger;

  for (let cardIndex = 0; cardIndex < this.template.components[1].cards.length; cardIndex++) {
    const card = this.template.components[1].cards[cardIndex];


    const buttons = card.components[2]?.buttons;
    if (buttons && Array.isArray(buttons)) {
      for (let buttonIndex = 0; buttonIndex < buttons.length; buttonIndex++) {
        const button = buttons[buttonIndex];
        if (
          button.type === 'URL' &&
          button.example != null &&
          !button.url.endsWith('/{{1}}')
        ) {
          button.url += '/{{1}}';
        }
      }
    }
  }
}

    //end  edit to car
    else if((this.template.category=="MARKETING" && this.template.sub_category=="CUSTOM")||(this.template.category=="UTILITY")||this.template.category=="MARKETING" &&this.template.id!=null){

    //         if (this.componentHeader.example?.mediaCard) {
    //   delete this.componentHeader.example.mediaCard;
    // }
       // this.template.components = this.template.components.filter(
       //   (i: { type: string }) => i.type !== "HEADER" 
       // );
      
    if ((this.componentFooter?.text || '').trim() === '') {
          this.componentFooter = null;
    }

        if (this.Buttons?.buttons?.length != 0) {
        const urlButtons = this.Buttons?.buttons?.filter(
          (i: { type: string }) => i.type === "URL"
        ) || [];
      

        if (urlButtons.length > 0) {
          urlButtons.forEach((button) => {

            if(button.example){
              if (
                button.url &&
                (button.example !== null ) && button.example &&
                !button.url.endsWith('/{{1}}')
              ) {
                button.url += '/{{1}}';
              }
            }

          });
        }
      }
       if (this.componentHeader?.format == 'None'  ||this.componentHeader?.format === null || this.componentHeader?.format === undefined) {
         this.template.components = this.template.components.filter(
           (i: { type: string }) => i.type !== "HEADER"
         );
       }
       else{
       if(this.componentHeader?.format){
         if (this.componentHeader?.example?.header_text?.toString().length == 0) {
           delete this.componentHeader.example.header_text;
           if(this.template.mediaLink){
             delete this.template.mediaLink;
           }
        }

        if (this.componentHeader?.example?.header_handle?.toString().length == 0) {
          delete this.componentHeader.example.header_handle;
          delete this.template.mediaLink;
          }
          else
          {
            if (this.componentHeader != null) {
              this.componentHeader.type = "HEADER";
                if(this.componentHeader.format=="IMAGE"||this.componentHeader.format=="VIDEO"||this.componentHeader.format=="DOCUMENT"){
                 if(this.whatsAppHeaderHandle.h){
                 this.componentHeader.example.header_handle[0] = this.whatsAppHeaderHandle.h;
                 this.template.mediaLink = this.whatsAppHeaderHandle.infoSeedUrl;
                 this.componentHeader.format.toLowerCase();
                 }
                }
              }
          }
          if (this.componentHeader?.example?.header_handle == null && this.componentHeader?.example?.header_text == null ) {
             this.componentHeader.example =null;
          }
       }
      }

    if (this.componentFooter?.text === null || this.componentFooter?.text === undefined) {
      this.template.components = this.template.components.filter(
        (i: { type: string }) => i.type !== "FOOTER"
      );
    }
    if (this.componentBody?.text === null || this.componentBody?.text === undefined) {
      this.template.components = this.template.components.filter(
        (i: { type: string }) => i.type !== "BOODY"
      );
    }
    if (this.Buttons?.buttons?.length==0) {
      this.template.components = this.template.components.filter(
        (i: { type: string }) => i.type !== "BUTTONS"
      );
    }
    
    this.componentBody.add_security_recommendation=null;
    if(this.componentFooter){
     this.componentFooter.code_expiration_minutes=null;
    }
    // if(this.componentHeader?.example.header_text[0]==''||this.componentHeader?.example.header_text[0]==null||this.componentHeader?.example.header_text[0]==undefined){
    //   this.componentHeader.example=null;
    // }
    }
    else if(this.template.category=="MARKETING" &&this.template.sub_category=="PRODUCT_MESSAGE"){
   
    const typesToRemove = ["HEADER", "FOOTER", "BODY", "BUTTONS"];

    debugger
    this.template.components = this.template.components.filter(
        (component: { type: string }) => !typesToRemove.includes(component.type)
      );

    this.template.components = this.template.components.filter(
       (i: { type: string }) => i.type !== "HEADER" 
     );

     if (this.componentFooter?.text === null || this.componentFooter?.text === undefined) {
      this.template.components = this.template.components.filter(
        (i: { type: string }) => i.type !== "FOOTER"
      );
    }
    if (this.componentBody?.text === null || this.componentBody?.text === undefined) {
      this.template.components = this.template.components.filter(
        (i: { type: string }) => i.type !== "BOODY"
      );
    }
    this.Buttons.buttons.length==0;
    if (this.Buttons.buttons.length==0) {
      this.template.components = this.template.components.filter(
        (i: { type: string }) => i.type !== "BUTTONS"
      );
    }

    // this.template.sub_category=null;
    this.componentBody.add_security_recommendation=null;
    if(this.componentFooter){
     this.componentFooter.code_expiration_minutes=null;
    }
    
    } 

  if (this.componentHeader?.format == 'None'  ||this.componentHeader?.format === null || this.componentHeader?.format === undefined) {
    this.template.components = this.template.components.filter(
      (i: { type: string }) => i.type !== "HEADER"
    );
  }
  
  this.template.components = this.template.components.filter(
    (item, index, self) =>
      item.type === "HEADER" || 
      self.findIndex((t) => t.type === item.type) === index
  );

  this.template.components = this.template.components.filter(
    (item, index, self) => 
      item.type !== "HEADER" || index === self.findIndex((t) => t.type === "HEADER")
  );
  this.template;
  if(this.templateId ==null){
    this.isLoading = true;  
    debugger
    // this.template.variableCount=
    this.template;
  
    this._whatsAppMessageTemplateServiceProxy
    .addWhatsAppMessageTemplate(null, this.template)
    .subscribe({
      next: (response) => {
        debugger
        if (!response.error&&response.id!=null ) {
          this.isLoading = false;  

            localStorage.setItem('notificationStatus', 'successful'); 
            this.router.navigate(['/app/main/messageTemplate']);
        } else {
          this.isLoading = false; 
          localStorage.setItem('notificationStatus', 'failed');
          const errorMessage = response.error?.error_user_msg || response.error?.message || 'An unknown error occurred.';
          localStorage.setItem('errorMessage', errorMessage|| 'Unknown error');
          // this.notify.error(`Failed: ${errorMessage}`);
            localStorage.setItem('notificationStatus', 'failed');
            this.router.navigate(['/app/main/messageTemplate']);
        }
        this.isModalVisible = false;
      },
      error: (error) => {
        this.isLoading = false;  
        localStorage.setItem('notificationStatus', 'failed');
        localStorage.setItem('errorMessage', error.message || 'Unknown error');
        this.router.navigate(['/app/main/messageTemplate']);
        this.isModalVisible = false;
      }
    });
    }
    if (this.templateId != null) {
      this.template.sub_category=null;
      this.isLoading = true;  
      this._whatsAppMessageTemplateServiceProxy
      .updateTemplate(null, this.template).subscribe({
         next: (response) => {
          this.isLoading = false;  
           if (response.success === true) {
               localStorage.setItem('notificationStatus', 'successful'); 
               this.router.navigate(['/app/main/messageTemplate']);
           } else {
            localStorage.setItem('notificationStatus', 'failed');
            const errorMessage = response.error?.error_user_msg || response.error?.message || 'An unknown error occurred.';
            localStorage.setItem('errorMessage', errorMessage|| 'Unknown error');
                localStorage.setItem('notificationStatus', 'failed');
                    this.router.navigate(['/app/main/messageTemplate']);
           }
              this.isModalVisibleEdit=false;
         },
         error: (error) => {
          this.isLoading = false;
           localStorage.setItem('notificationStatus', 'failed');
           const errorMessage = error.error?.error_user_msg || error.error?.message || 'An unknown error occurred.';
           localStorage.setItem('errorMessage', errorMessage|| 'Unknown error');
           this.router.navigate(['/app/main/messageTemplate']);
           this.isModalVisibleEdit=false;
         }
       });
    } 
}
  


 getImageUrl(card: any): string {
  const handle = card.components[0]?.example?.header_handle?.[0];
  return `https://example.com/images/${handle}`;
}

handleButtonClick(button: any) {
  if (button.type === 'url') {
    const url = button.url.replace('{{1}}', button.example?.[0] || '');
    window.open(url, '_blank');
  } else {
    alert(`Quick reply selected: ${button.text}`);
  }
}

car(){
    debugger
    this.carousel;
  }
  checktemplate(){
    debugger;
    this.template;
  }
}
// src/app/utils/sanitize-object.ts


