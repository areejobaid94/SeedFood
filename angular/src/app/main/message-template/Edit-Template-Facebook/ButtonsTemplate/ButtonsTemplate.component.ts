import { Text, Language } from './../../../../../shared/service-proxies/service-proxies';
import { UrlHelper } from './../../../../../shared/helpers/UrlHelper';
import { Button } from '@node_modules/primeng/button';
import { Component, EventEmitter, Input, OnInit, Output, Type, ViewChild } from '@angular/core';
import { ButtonsTemplate, CallToActionButton, MessageTemplateModel, QuickReplyButton, QuickReplyType, TypeOfAction, URLType, WhatsAppButtonModel, WhatsAppComponentModel, WhatsAppExampleModel } from '@shared/service-proxies/service-proxies';
import parsePhoneNumberFromString from '@node_modules/libphonenumber-js';
import {
  CountryISO,
  NgxIntlTelInputComponent,
  PhoneNumberFormat,
  SearchCountryField,
} from "ngx-intl-tel-input";
import { FormControl, FormGroup, Validators } from '@node_modules/@angular/forms';
@Component({
  selector: 'app-ButtonsTemplate',
  templateUrl: './ButtonsTemplate.component.html',
  styleUrls: ['./ButtonsTemplate.component.css']
})
export class ButtonsTemplateComponent implements OnInit {
  PhoneNumberFormat = PhoneNumberFormat;
  preferredCountries: CountryISO[] = [
    CountryISO.SaudiArabia,
    CountryISO.Jordan,
];
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
  // isUrlInvalid:boolean;
  isUrlInvalidMap: Map<number, boolean> = new Map();
  isUrlInvalidExampleMap: Map<number, boolean> = new Map();

  isSampleUrlInvalid:boolean=true;
  buttonLength:number;
  maxLength:number=25;
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
  // buttons  =   new WhatsAppButtonModel
  componentButtonForView: any = {};
  
   buttonsArray: WhatsAppButtonModel[] ;

   buttonURL:any;
  // componentButtonForView: WhatsAppComponentModel = new WhatsAppComponentModel();

  // facebookTemplateDto: MessageTemplateModel = new MessageTemplateModel();
  Buttons: WhatsAppComponentModel = new WhatsAppComponentModel();
  Butto: WhatsAppComponentModel = new WhatsAppComponentModel();

  isOptOutResponsibilityAcknowledged:boolean=true;
  countryCode: string = '';
  localNumber: string = '';
  
  duplicateError: boolean[] = [];
  hasDuplicates: boolean = true;
  isValid: boolean = true;
  isFormValidForSubmit:boolean=true;

  countryObj: any = null;

  constructor() { }

  onPhoneNumberChange() {
    debugger
    const phoneNumber = this.phonenumber.e164Number;

    if (  phoneNumber) {
      const fullPhoneNumber = phoneNumber;
      
      this.Button = this.Buttons?.buttons?.find(
        (i: { type: string }) => i.type === "PHONE_NUMBER"
      ) || null;

      if (this.Button) {
        this.Button.phone_number = fullPhoneNumber; 
      }
    }
    this.validateForm();
  }
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

  checkifnotError(){
    if (
      this.isOptOutResponsibilityAcknowledged !== false &&
      this.hasDuplicates === false &&
      this.isValid !== false &&
      !Array.from(this.isUrlInvalidMap.values()).some((value) => value === true) &&
      !Array.from(this.isUrlInvalidExampleMap.values()).some((value) => value === true)
    )
    {      this.buttons;
    }

    this.hasDuplicates = this.duplicateError.some(error => error === true);
    this.isValid = this.Buttons.buttons.every(button => button.text && button.text.trim() !== '');
    // this.validateButtons();
    this.validateFormForSubmission();
    this.isFormValidForSubmit;
  }

  validateButtons(): void {
    
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
    this.validateFormForSubmission();
    this.validateForm()
  }

  validateFormForSubmission() {
    this.isFormValidForSubmit = true;
    this.hasDuplicates = this.duplicateError.some(error => error === true);

    if(this.hasDuplicates==true){
      this.isFormValidForSubmit=false
    }

    if (!this.isOptOutResponsibilityAcknowledged) {
      this.isFormValidForSubmit = false;
    }

    for (let i = 0; i < this.Buttons.buttons.length; i++) {
      const button = this.Buttons.buttons[i];
      if (!button.text || button.text.trim() === '') {
        this.isFormValidForSubmit = false;
      }
    }
    this.isFormValidForSubmit;
    return this.isFormValidForSubmit;
  }

  // validateFormForSubmission() {
  //   // Track whether all required fields are filled
  //   let isAllFieldsFilled = true;
  
  //   for (let button of this.Buttons.buttons) {
  //     if (button.type === 'URL' && (!button.url || this.isUrlInvalid)) {
  //       isAllFieldsFilled = false;
  //     }
  //     if (button.type === 'PHONE_NUMBER' && (!this.localNumber || !this.countryCode)) {
  //       isAllFieldsFilled = false;
  //     }
  //     if (button.type === 'COPY_CODE' && (!button.text || button.text.trim() === '')) {
  //       isAllFieldsFilled = false;
  //     }
  //     // Add additional checks for other fields as needed
  //   }
  
  //   // Update the form submission status based on whether all fields are filled
  //   this.isFormValidForSubmit = isAllFieldsFilled;
  // }
  getapi(){
    this.Buttons = this.template.components.find(
      (component: WhatsAppComponentModel) => component.type === "BUTTONS"
    )||null;
  }
  ngOnInit() {
      this.Buttons = this.template.components.find(
      (component: WhatsAppComponentModel) => component.type === "BUTTONS"
      || null);

      this.Button = this.Buttons?.buttons?.find(
        (i: { type: string }) => i.type === "PHONE_NUMBER"
      ) || null;
  
      if (this.Button && this.Button.phone_number) {
        const phoneNumber = this.Button.phone_number;
  
        const regex = /^\+(\d{1,4})(\d{1,})$/; 
        const match = phoneNumber.match(regex);
  
        if (match) {
          const countryDialCode = match[1]; 
          const nationalNumber = match[2]; 
  
          this.countryCode = countryDialCode;
          this.phonenumber = nationalNumber; 
        }
      }
  
if (this.template && this.template.components ) {
  if (!this.Buttons) {
          this.Buttons = new WhatsAppComponentModel();
          this.Buttons.type = "BUTTONS";
          this.Buttons.buttons = [];
          this.template.components.push(this.Buttons);

          this.Buttons = this.template.components.find(
            (component: WhatsAppComponentModel) => component.type === "BUTTONS"
            || null);

            this.Button = this.Buttons?.buttons?.find(
              (i: { type: string }) => i.type === "phone_number"
            )||null;
        
  }

  if (this.Buttons.buttons && this.Buttons.buttons.length > 0) {
    this.Buttons.buttons.forEach((button) => {
      if (button.type === "PHONE_NUMBER" && button.phone_number) {
        const match = button.phone_number.match(/^(\+\d{1,3})(\d+)$/);
        if (match) {
          this.countryCode = match[1];  
          this.localNumber = match[2];  
        }
      }
    });
  } else {
    console.log("No buttons found. You can add buttons here if needed.");
  }
} else {
  this.template = new MessageTemplateModel();
  this.template.components = [];
  this.Buttons = new WhatsAppComponentModel();
  this.Buttons.type = "BUTTONS";
  this.Buttons.buttons = [];
  this.template.components.push(this.Buttons);
}

    this.Buttons = this.template.components.find(
      (component: WhatsAppComponentModel) => component.type === "BUTTONS"
    );
  }

  onURLChange(event: Event, index: number): void {
    const input = (event.target as HTMLInputElement).value;
    const regex = new RegExp(this.urlPattern);
    this.isUrlInvalidMap.set(index, !regex.test(input)); 
    this.validateForm(); 
  }

onURLExampleChange(event: Event, index: number): void {
  const input = (event.target as HTMLInputElement).value;

  const isEmpty =
    input != null && input.trim().length === 0;

  this.isUrlInvalidExampleMap.set(index, isEmpty);

  this.validateForm();
}


  isUrlInvalid(index: number): boolean {
    return this.isUrlInvalidMap.get(index) || false; 
  }
  isUrlInvalidExample(index: number): boolean {
    
    return this.isUrlInvalidExampleMap.get(index) || false;
    
  }

  validateURL(button: any): void {
    button.isUrlInvalid = !this.urlPattern.test(button.url || '');
    this.validateForm();
  }
  onSampleURLChange(event: Event): void {
    const input = (event.target as HTMLInputElement).value; 
    const regex = new RegExp(this.urlPattern);
    this.isSampleUrlInvalid = !regex.test(input); 
  }

  validateSampleURL(button: any): void {
    this.isSampleUrlInvalid = !this.urlPattern.test(button|| '');
    this.validateForm();
  }
  initializeExample(button: WhatsAppButtonModel): void {
    if (!button.example || !Array.isArray(button.example)) {
      button.example = [''];
    }
    else{
      
    }
    this.validateForm();

  }

  onButtonTypeChange(buttonType: string) {
    let buttonComponent = this.template.components.find(
        (comp: WhatsAppComponentModel) => comp.type === "BUTTONS"
    );
    debugger;

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
        button.url = "";
        button.example = null;

        this.Buttons.buttons.push(button);
        this.isCompleteFlowDisabled = true;
        this.buttonLength = this.Buttons.buttons.length;
        this.isUrlInvalidMap.set(this.Buttons.buttons.length - 1, true);
        
        // this.isUrlInvalid=true;
        if (this.countButtons("URL") >= 2) {
            this.isVisitWebsiteDisabled = true;
        }
    } else if (buttonType === 'call-phone-number') {
        button.type = "PHONE_NUMBER";
        button.text = 'Call phone number';
        this.Buttons.buttons.push(button);
        this.isCallButtonDisabled = true;
        this.isCompleteFlowDisabled = true;
    } else if (buttonType === 'copy-offer-code' && this.countButtons("COPY_CODE") < 1) {
        button.type = "COPY_CODE";
        button.text = 'Copy offer code';
        button.example = []; 
        button.example.push("");
        this.Buttons.buttons.push(button);
        this.isCopyOfferCodeDisabled = true;
        this.isCompleteFlowDisabled = true;
    } else if (buttonType === 'complete-flow') {
        button.type = "COPY_CODE";
        button.text = 'complete-flow';
        this.Buttons.buttons.push(button);
        this.checkButtonLimits();
    } else if (buttonType === 'custom') {
        button.type = "QUICK_REPLY";
        button.text = 'Quick Reply';
        this.Buttons.buttons.push(button);
        if(this.isMarketinOptOutDisabled){
          this.isMarketinOptOutDisabled=true;
        }
        this.checkButtonLimits();
    }
    else if (buttonType === 'Marketing-opt-out') {
      debugger
      if(this.template.language=='en'){
        button.type = "QUICK_REPLY";
        button.text = 'Stop promotions';
      }
      else{
        button.type = "QUICK_REPLY";
        button.text = 'إيقاف عمليات الترويج';
      }
      this.Buttons.buttons.push(button);
      this.isMarketinOptOutDisabled=true;
      this.checkButtonLimits();

  }

  this.validateButtons();
  this.validateFormForSubmission();
  // this.template.components.push(this.Buttons);

    // Update the buttons array
    this.buttonsArray = this.Buttons.buttons;
    // this.buttons = buttonComponent;
    // else if (buttonType === 'Marketing-opt-out'
    //    comment && this.countButtonsByQuickReply(QuickReplyType.MarketingOptOut) < 1
    //    ) 
    //    {
    //   const button = new QuickReplyButton();
    //   if (this.facebookTemplateDto.language.language == 0) {
        
    //     button.replyType=QuickReplyType.MarketingOptOut;
    //       button.buttonText='Stop Promotions'
    //       button.footerText='Not interested? Tap Stop promotions'
    //       this.facebookTemplateDto.buttons.quickReplyButtons.push(button);
    //       debugger
    //       this.isMarketinOptOutDisabled=true;
    //     } else if (this.facebookTemplateDto.language.language == 1) {

    //       button.replyType=QuickReplyType.MarketingOptOut;
    //       button.buttonText='إيقاف عمليات الترويج'
    //       button.footerText='ألست مهتمًا؟ اضغط على عمليات الترويج'
    //       this.facebookTemplateDto.buttons.quickReplyButtons.push(button);
    //       this.isMarketinOptOutDisabled=true;

    //     }
    // }
    // this.combineAndFilterButtons();


    this.validateForm();
}


removeButton(index: number) {
  const component = this.template.components.find(c => c.type === "BUTTONS");
  if (this.Buttons) {
    this.Buttons.buttons.splice(index, 1);

      // if (component.buttons.length === 0) {
      //     const componentIndex = this.template.components.indexOf(component);
      //     if (componentIndex !== -1) {
      //         this.template.components.splice(componentIndex, 1);
      //     }
      // }

      this.checkButtonLimits();

      const totalButtons = this.template.components
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
  if(this.isUrlInvalidExampleMap.has(index)){
     this.isUrlInvalidExampleMap.set(index, false); 
    this.isUrlInvalidExampleMap.delete(index);
  }

  this.validateButtons();

  
}

  // removeButtonQuickReply(index: number) {
  //   this.facebookTemplateDto.buttons.quickReplyButtons.splice(index, 1); 
  //   this.combineAndFilterButtons();
  //   this.checkButtonLimits(); 
  //   debugger
  //   if((this.facebookTemplateDto.buttons.callToActionButtons.length+this.facebookTemplateDto.buttons.quickReplyButtons.length)<2){
  //     this.isPlayVisible=false;
  //     this.isPlayVisibleChange.emit(this.isPlayVisible); 
  //   }
  // }

  onActionChange(button: WhatsAppButtonModel) {

    this.setDefaultProperties(button);
    this.checkButtonLimits(); 
    this.validateButtons();
    this.validateFormForSubmission();
    this.validateForm();

  }
  // onURLTypeChange(button:Button){
  //   button.url = button.url;
  //   button.urlType = '';
  // }
  onActionChangeInQuickReply(button: WhatsAppButtonModel) {
    this.setDefaultPropertiesInquickReply(button);
    this.checkButtonLimits(); 
    this.validateButtons();
    this.validateFormForSubmission();


  }

  setDefaultProperties(button: WhatsAppButtonModel) {
    button.url = null;
    button.phone_number = '';
    // button.offerCode = '';
    button.text='';
    if (button.type == 'URL') {
      button.type = "URL";
      button.text = 'Visit website';
      button.url = "";
  } else if (button.type == 'PHONE_NUMBER') {
      button.type = 'PHONE_NUMBER';
      button.text = 'Call phone number';
      button.phone_number='';
      // button.country = '';
      // button.phoneNumber = '';
  } else if (button.type == 'COPY_CODE') {
      button.type = 'COPY_CODE';
      button.text = 'Copy offer code';
      button.example = []; 
      button.example.push("");
      this.initializeExample(button);

  } else if (button.type == 'complete-flow') {
      button.type = 'complete-flow';
      button.text = 'complete-flow';
      this.checkButtonLimits();
  }
  else if (button.type == 'marketing-opt-out') {
    button.type = 'QUICK_REPLY';
    button.text = 'Stop promotions';
    this.checkButtonLimits();
}
else if (button.type == 'QUICK_REPLY') {
  button.type = 'QUICK_REPLY';
  button.text = 'Quick Reply';
  this.checkButtonLimits();
}
this.checkButtonLimits();
this.validateFormForSubmission();

}
setDefaultPropertiesInquickReply(button: WhatsAppButtonModel) {

  if (button.text == 'Stop promotions') {
    button.type = "QUICK_REPLY";
    button.text = 'Quick Reply';
 } else if (button.text != "Stop promotions") {
    button.type = "QUICK_REPLY";
    button.text = "Stop promotions";
   this.isOptOutResponsibilityAcknowledged=true;
 } 
}
  countButtons(actionType: string): number {
    const allButtons = this.template.components
    ?.flatMap(component => component.buttons || []);

    const filteredButtons = allButtons?.filter(button => button.type === actionType) || [];
    debugger;
    return filteredButtons.length;
  }
countQuickReplyButtons(textButton: string): number {
  const allButtons = this.template.components
  ?.flatMap(component => component.buttons || []);
  const filteredButtons = allButtons?.filter(button => button.text === textButton ||button.text === "إيقاف عمليات الترويج") || [];
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
    return this.template.components.some(component => 
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
get checkCompleteFlow() {
  return this.template.components.some(component =>
      component.buttons?.some(button => button.type === 'Complex')
  );
}

get hasButtons() {
  return this.template.components.some(component =>
      component.buttons?.length > 0
  );
}


 checkButtonLimits() {
         const buttonComponent = this.template.components.find(
          (comp: WhatsAppComponentModel) => comp.type === "BUTTONS"
      );
      
      const totalButtons = buttonComponent?.buttons?.length || 0;
      

  if (totalButtons >= 10 || this.countButtons("CompleteFlow") >=1) {
    this.isVisitWebsiteDisabled = true;
    this.isCallButtonDisabled = true;
    this.isCopyOfferCodeDisabled = true;
    this.isMarketinOptOutDisabled = true;
    this.isCustomDisabled = true;
    this.isCompleteFlowDisabled = true;
    return;
  }

  if (totalButtons >= 1) {
    this.isCompleteFlowDisabled = true;
  }  if (totalButtons == 0) {
    this.isCompleteFlowDisabled = false;
  }
  if(this.countButtons("CompleteFlow") ==0){
    debugger;
    this.isVisitWebsiteDisabled = this.countButtons("URL") >= 2;
    this.isCallButtonDisabled = this.countButtons("PHONE_NUMBER") >= 1;
    this.isCopyOfferCodeDisabled = this.countButtons("COPY_CODE") >= 1;
    this.isMarketinOptOutDisabled = this.countQuickReplyButtons("Stop promotions") >= 1;
    this.isCustomDisabled = this.countButtons("QUICK_REPLY") >= 10;
  }
 }

//  get CallToAction() {

//   return this.template.components.some(component => 
//       component.buttons?.some(button => 
//           button.type === 'URL' || 
//           button.type === 'PHONE_NUMBER' || 
//           button.type === 'COPY_CODE'
//       )
//   );
// }
 updateButtonTextBasedOnLanguage(language) {
  setTimeout(() => {
    
    for (const component of this.template.components) {
      if (component.type === 'BUTTONS' && Array.isArray(component.buttons)) {
        for (const button of component.buttons) {
          if (button.type === 'QUICK_REPLY') {
             if(button.text =='Stop promotions'||button.text =='إيقاف عمليات الترويج'){
              if (language === 'en') {
                button.text = 'Stop promotions';
              }
              else if (language === 'ar') {
                button.text = 'إيقاف عمليات الترويج';
                // button.footerText = 'ألست مهتمًا؟ اضغط على إيقاف عمليات الترويج';
              }
            } 
          }
        }
      }
    }
  }, 0);
}

OnURLTypeChange(button: WhatsAppButtonModel, index:number) {
  if (button.example) {
    button.example = null;
  } else if(!button.example) {
    button.example = [];
    button.example.push("");
      this.isUrlInvalidExampleMap.set(index, true);
  }
  this.validateForm()
}

checkErrorForSubmit(){
  if(!this.isUrlInvalid && !this.isSampleUrlInvalid ){

  }
}

validateForm(): void {
  debugger
  this.isFormInvalid = this.Buttons.buttons.some((button) => {
    debugger;
    if (button.type === 'URL') {
  
      return !button.url || button.url.trim() === ''; 

    } else if (button.type === 'PHONE_NUMBER') {
      if (button.phone_number && button.phone_number.length >= 9) {
        return false; 
      }
      return !button.phone_number || button.phone_number.trim() === '';
    } else if (button.type === 'COPY_CODE') {
      return !button.example || !button.example[0]?.trim(); 
    } else if (button.text === 'COPY_CODE') {
      return !button.example || !button.example[0]?.trim(); 
    } else if (!button.text || button.text.trim() === '') {
      return true;
    }
    return false; 
  });

  if (
    this.Buttons.buttons.some(
      (button) => button.type === 'QUICK_REPLY'
    ) && !this.isOptOutResponsibilityAcknowledged
      && !this.isUrlInvalid
      && !this.isUrlInvalidExample
      && !this.isFormInvalid
  ) {
    this.isFormInvalid = true; 
  }
  if (Array.from(this.isUrlInvalidMap.values()).includes(true)) {
    this.isFormInvalid = true;
    this.isFormInvalidChange.emit(this.isFormInvalid);
    return; 
  }
    if (Array.from(this.isUrlInvalidExampleMap.values()).includes(true)) {
    this.isFormInvalid = true;
    this.isFormInvalidChange.emit(this.isFormInvalid);
    return; 
  }

  this.isFormValidForSubmit = true;
  this.hasDuplicates = this.duplicateError.some(error => error === true);

  if(this.hasDuplicates==true){
    this.isFormInvalid=true;
    this.isFormInvalidChange.emit(this.isFormInvalid);
    return;
  }

  if (!this.isOptOutResponsibilityAcknowledged) {
    this.isFormValidForSubmit = false;
    this.isFormInvalidChange.emit(this.isFormInvalid);
    return;
  }

  for (let i = 0; i < this.Buttons.buttons.length; i++) {
    const button = this.Buttons.buttons[i];
    if (button.text || button.text.trim() === '') {
      this.isFormValidForSubmit = true;
    }
  }
  this.isFormInvalidChange.emit(this.isFormInvalid);
}

isOptOutResponsibilityAcknowledgedChange(){
  if(this.isOptOutResponsibilityAcknowledged){
    this.isFormInvalidChange.emit(this.isFormInvalid);
    return
  }
  else{
    this.isFormInvalidChange.emit(true);
    return
  }
}




}
