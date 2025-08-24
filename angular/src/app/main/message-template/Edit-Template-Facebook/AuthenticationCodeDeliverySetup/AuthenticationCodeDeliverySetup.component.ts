import { Button } from '@node_modules/primeng/button';
import { FooterComponent } from './../../../../shared/layout/footer.component';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageTemplateModel, TemplateContentAuthentication, WhatsAppButtonModel, WhatsAppComponentModel, WhatsAppExampleModel } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-AuthenticationCodeDeliverySetup',
  templateUrl: './AuthenticationCodeDeliverySetup.component.html',
  styleUrls: ['./AuthenticationCodeDeliverySetup.component.css']
})
export class AuthenticationCodeDeliverySetupComponent implements OnInit {
  @Input() template: any = {};
  @Input() selectedCategory: string = '';
  @Input() selectedMessageType: string = '';

  showAppSetup: boolean = true;
  selectedDeliveryOption: string = '';
  zeroTapConsent: boolean = false;
  packageName: string = '';
  appSignature: string = '';
  apps: { packageName: string; appSignature: string }[] = [{ packageName: '', appSignature: '' }];
  isInvalidAppPackageName: boolean = false;
  isSignatureInvalid: boolean = false;
  isDuplicateError: boolean = false;
  maxApps = 5;
  addSecurityRecommendation = false;
  addExpirationTime = false;        
  minutes = 1;                       
  minutesError = false;              
  selectedOption: string = 'zeroTap'; 
  zeroTapAgreement: boolean = true;  

  isCustomValidity:boolean=false;
  private firstToggle: boolean = true; 


  componentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
  componentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
  componentButton: WhatsAppComponentModel = new WhatsAppComponentModel();

  @Output() otpTypeChange = new EventEmitter<string>();

  Button:WhatsAppButtonModel=new WhatsAppButtonModel();
  packageNameArray:string[];
  signatureHashArray:string[];

  packageNameAndSignatureHashObj:any;
  codeExpirationMinutes: string;
  otpCode: string;
  ctaDisplayName: string;
  otpType:string;
  code: string;

  @Output() ctaDisplayNameChange: EventEmitter<string> = new EventEmitter<string>(); 
  @Output() ButtonChange: EventEmitter<{ text: string }> = new EventEmitter();  

  @Output() submitEvent = new EventEmitter<boolean>();

  generatedUrl:any;
  Submit: boolean=false;

  sendDataForSubmit() {
    this.submitEvent.emit(this.Submit); 
  }
  onInputChange() {
    this.ctaDisplayNameChange.emit(this.ctaDisplayName);
  }
  onInputChangeButtonText() {
    this.ButtonChange.emit(this.Button); 
  }
  constructor() { 
    
  }

  

  ngOnInit() {
         
      this.componentFooter = this.template.components.find(
          (i: { type: string }) => i.type === "FOOTER"
        )||null;
        this.componentFooter
      
        if(this.componentFooter.code_expiration_minutes!=null){
          this.addExpirationTime=true;
        }
      
        this.componentBody = this.template.components.find(
          (i: { type: string }) => i.type === "BODY"
        )||null;
      
        this.componentButton = this.template.components.find(
          (i: { type: string }) => i.type === "BUTTONS"
        )||null;
      
      
        this.Button = this.componentButton?.buttons?.find(
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
  const queryParam = new URLSearchParams(parsedUrl.search);

  const data: Record<string, string> = {};
  queryParam.forEach((value, key) => {
    data[key] = value;
  });
    this.ctaDisplayName = queryParams.get("cta_display_name");
    this.codeExpirationMinutes = queryParams.get("code_expiration_minutes");
    debugger
    this.code = queryParams.get("code");
    this.otpType = queryParams.get("otp_type") || "ZERO_TAP";
    debugger;
  

  if(this.template.message_send_ttl_seconds!=null){
    this.isCustomValidity=true;
  }
  this.validateAppInputs();
  this.onInputChange();
  this.onInputChangeButtonText();
  this.onCheckboxChange();
}

  constructWhatsAppUrl() {
    debugger;
    this.packageNameAndSignatureHashObj;

    const packageNames =  this.packageNameAndSignatureHashObj.map(app => app.packageName);
    const signatureHashes = this.packageNameAndSignatureHashObj.map(app => app.signatureHash);

    const baseUrl = "https://www.whatsapp.com/otp/code/";

  if(this.otpType!='COPY_CODE'){
    if(this.codeExpirationMinutes==null ){
      const queryParams: { [key: string]: string } = {
        otp_type: this.otpType,
        cta_display_name: this.ctaDisplayName,
        package_name: packageNames, 
        signature_hash: signatureHashes, 
        // ...(this.codeExpirationMinutes !== null && this.codeExpirationMinutes !== undefined ? { code_expiration_minutes: this.codeExpirationMinutes } : {}),
        // code: this.code, // Uncomment if needed
      };
      const queryParamsFromExample: { [key: string]: string } = {
        otp_type: this.otpType,
        cta_display_name: this.ctaDisplayName,
        package_name: packageNames, 
        signature_hash: signatureHashes,
        code:"otp123456",
      };
      const constructedUrl = `${baseUrl}?${new URLSearchParams(queryParams).toString()}&code=otp{{1}}`;
  
      this.Button.url = constructedUrl;
    
      this.Button.example = [];
      this.Button.example.push(`${baseUrl}?${new URLSearchParams(queryParamsFromExample).toString()}`);
  
      this.Button.url;
      this.Button.example[0];
      
    }
    else{

      const queryParams: { [key: string]: string } = {
        otp_type: this.otpType,
        cta_display_name: this.ctaDisplayName,
        package_name: packageNames, 
        signature_hash: signatureHashes, 
        code_expiration_minutes: this.codeExpirationMinutes
        // ...(this.codeExpirationMinutes !== null && this.codeExpirationMinutes !== undefined ? { code_expiration_minutes: this.codeExpirationMinutes } : {}),
        // code: this.code, // Uncomment if needed
      };
      const queryParamsFromExample: { [key: string]: string } = {
        otp_type: this.otpType,
        cta_display_name: this.ctaDisplayName,
        package_name: packageNames, 
        signature_hash: signatureHashes,
        code_expiration_minutes: this.codeExpirationMinutes,
        code:"otp123456",
      };
      const constructedUrl = `${baseUrl}?${new URLSearchParams(queryParams).toString()}&code=otp{{1}}`;
  
      this.Button.url = constructedUrl;
    
      this.Button.example = [];
      this.Button.example.push(`${baseUrl}?${new URLSearchParams(queryParamsFromExample).toString()}`);
  
      this.Button.url;
      this.Button.example[0];
    }
  }
  else{

    if(this.codeExpirationMinutes==null ){
      const queryParams: { [key: string]: string } = {
        otp_type: this.otpType,
     
        // ...(this.codeExpirationMinutes !== null && this.codeExpirationMinutes !== undefined ? { code_expiration_minutes: this.codeExpirationMinutes } : {}),
        // code: this.code, // Uncomment if needed
      };
      const queryParamsFromExample: { [key: string]: string } = {
        otp_type: this.otpType,
     
        code:"otp123456",
      };
      const constructedUrl = `${baseUrl}?${new URLSearchParams(queryParams).toString()}&code=otp{{1}}`;
  
      this.Button.url = constructedUrl;
    
      this.Button.example = [];
      this.Button.example.push(`${baseUrl}?${new URLSearchParams(queryParamsFromExample).toString()}`);
  
      this.Button.url;
      this.Button.example[0];
      
    }
    else{

      const queryParams: { [key: string]: string } = {
        otp_type: this.otpType,
        cta_display_name: this.ctaDisplayName,
        code_expiration_minutes: this.codeExpirationMinutes
        // ...(this.codeExpirationMinutes !== null && this.codeExpirationMinutes !== undefined ? { code_expiration_minutes: this.codeExpirationMinutes } : {}),
        // code: this.code, // Uncomment if needed
      };
      const queryParamsFromExample: { [key: string]: string } = {
        otp_type: this.otpType,
        cta_display_name: this.ctaDisplayName,
        code_expiration_minutes: this.codeExpirationMinutes,
        code:"otp123456",
      };
      const constructedUrl = `${baseUrl}?${new URLSearchParams(queryParams).toString()}&code=otp{{1}}`;
  
      this.Button.url = constructedUrl;
    
      this.Button.example = [];
      this.Button.example.push(`${baseUrl}?${new URLSearchParams(queryParamsFromExample).toString()}`);
  
      this.Button.url;
      this.Button.example[0];
    }

  }
    
    debugger

    // url: "https://www.whatsapp.com/otp/code/?otp_type=ZERO_TAP&cta_display_name=Autofill12&package_name=aa.zz&signature_hash=12121212121&code=otp{{1}}"
    // url: "https://www.whatsapp.com/otp/code/?otp_type=ZERO_TAP&cta_display_name=Autofill12&package_name=aa.zz&signature_hash=12121212121&code=otp{{1}}"


  }


  getapi(){
        debugger
    this.componentBody = this.template.components.find(
      (i: { type: string }) => i.type === "BODY"
     )||null;

     this.componentButton = this.template.components.find(
      (i: { type: string }) => i.type === "BUTTONS"
     )||null;

    this.componentFooter = this.template.components.find(
      (i: { type: string }) => i.type === "FOOTER"
     );

     this.Button = this.componentButton?.buttons?.find(
      (i: { type: string }) => i.type === "URL"
    )||null;
    this.template

    this.ctaDisplayName ;
    this.codeExpirationMinutes ;
    this.code ;
    debugger;
    this.otpType;
    this.template
     const uri = new URL(this.Button.url);

     const queryParams = uri.searchParams;
     
     const packageName = queryParams.get("package_name");
     const signatureHash = queryParams.get("signature_hash");
     
      this.packageNameArray = packageName ? packageName.split(",") : [];
     
      this.signatureHashArray = signatureHash ? signatureHash.split(",") : [];

      debugger;
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
       this.otpType = queryParams.get("otp_type") || "ZERO_TAP" ||"COPY_CODE";
      if(this.codeExpirationMinutes!=null){
        this.isCustomValidity=true;
      }
      
      this.onInputChangeButtonText();
     
  }
  onDeliveryMethodChange(CodeDeliverySetup: string): void {
    // const selectedValue = (event.target as HTMLInputElement).id;
    switch (CodeDeliverySetup) {
      case "ZERO_TAP":
        this.otpType = "ZERO_TAP";
        this.otpTypeChange.emit(this.otpType);
        // this.ctaDisplayName="Autofill";
      this.validateAppInputs();
      this.constructWhatsAppUrl();
      break;
      case "ONE_TAP":
        this.otpType = "ONE_TAP";
        this.otpTypeChange.emit(this.otpType);
        // this.ctaDisplayName="Autofill";
      this.validateAppInputs();
      this.constructWhatsAppUrl();

        break;
      case "COPY_CODE":
        this.otpType = "COPY_CODE";
        this.otpTypeChange.emit(this.otpType);
        // this.ctaDisplayName=null;
        this.validateAppInputs();
        this.checkErrorForSubmit();
        this.constructWhatsAppUrl();
        this.submitEvent.emit(this.Submit); 
        break;
    }
    this.otpTypeChange.emit(this.otpType);
    debugger;
  }
  
  validateAppInputs() {
    debugger
    if(this.otpType=='COPY_CODE'){
      this.isInvalidAppPackageName=false;
      this.isSignatureInvalid=false;
      this.isDuplicateError=false;
    }
    else{
      this.isInvalidAppPackageName = this.packageNameAndSignatureHashObj.some(app => !this.isValidPackageName(app.packageName));
      this.isSignatureInvalid = this.packageNameAndSignatureHashObj.some(app => !this.isValidSignatureHash(app.signatureHash));
      
     //code for check if have dubplicates
      const seenPairs = new Set();
      this.isDuplicateError = this.packageNameAndSignatureHashObj.some(app => {
      const pair = `${app.packageName}-${app.signatureHash}`;
      if (seenPairs.has(pair)) {
        return true; // Duplicate found
      }
      seenPairs.add(pair);
      return false;
    });
   //end code for check dubplicates
    }
    this.checkErrorForSubmit();

   }
  
   isValidPackageName(packageName: string): boolean {
    const regex = /^[a-zA-Z][a-zA-Z0-9_]*\.[a-zA-Z][a-zA-Z0-9_]*$/;
    return regex.test(packageName);
   }
  
  isValidSignatureHash(signatureHash: string): boolean {
    const regex = /^.{11}$/; 
    return regex.test(signatureHash);
  }
  
  isDuplicatePackageName(index: number): boolean {
    const currentPackageName = this.packageNameAndSignatureHashObj[index].packageName;
    return this.packageNameAndSignatureHashObj.some(
      (app, i) => i !== index && app.packageName === currentPackageName
    );
  }
  

  addApp(): void {
    if (this.packageNameAndSignatureHashObj.length < this.maxApps) {
      this.packageNameAndSignatureHashObj.push({
        packageName: '' ,signatureHash: '',
        init: function (_data?: any): void {
          throw new Error('Function not implemented.');
        },
        toJSON: function (data?: any) {
          throw new Error('Function not implemented.');
        }
      });
          this.validateAppInputs();
    }
  }
  
  removeApp(index: number): void {
    if (this.packageNameAndSignatureHashObj.length > 1) {
      this.packageNameAndSignatureHashObj.splice(index, 1);
    }
    this.validateAppInputs();
  }
  toggleMinutesInput() {
    if( this.componentFooter.code_expiration_minutes==null){
      // this.codeExpirationMinutes = `${this.componentFooter.code_expiration_minutes}`; 
      // this.codeExpirationMinutes=`${10}`; ;
      this.componentFooter.code_expiration_minutes=10;
      this.minutesError = false;    
    }
    if (this.addExpirationTime) {
        this.componentFooter.text = "This code expires in "+ this.componentFooter.code_expiration_minutes+" minutes.";
        this.codeExpirationMinutes = `${this.componentFooter.code_expiration_minutes}`; 
      } else {
        this.componentFooter.text = "";
        this.codeExpirationMinutes=null;
        this.componentFooter.code_expiration_minutes=null;
      }  
  }

  validateMinutes() {
    if (this.componentFooter.code_expiration_minutes >= 1 && this.componentFooter.code_expiration_minutes <= 90) {
      this.minutesError = false;
      this.checkErrorForSubmit();
    } else {
      this.minutesError = true;
      this.checkErrorForSubmit();
    }
  }

  onCheckboxChange(): void {

    if (this.componentBody.add_security_recommendation) {
        this.componentBody.text = "*{{1}}* is your verification code. For your security, do not share this code.";
    } else {
        this.componentBody.text = "{{1}} is your verification code.";
    }
}


onToggleValidity(isChecked: boolean): void {
  this.isCustomValidity = isChecked;

  if (isChecked && this.firstToggle) {
    this.template.message_send_ttl_seconds = 60; 
    this.firstToggle = false; }
  }

checkErrorForSubmit(){
  if(!this.minutesError&& !this.isInvalidAppPackageName&& !this.isSignatureInvalid && !this.isDuplicateError&&this.zeroTapAgreement){
    this.Submit=true;
  }
  else{
    this.Submit=false;
  }
  this.sendDataForSubmit();
}

checkErrorForChange(){
  this.checkErrorForSubmit();
}

}
  


