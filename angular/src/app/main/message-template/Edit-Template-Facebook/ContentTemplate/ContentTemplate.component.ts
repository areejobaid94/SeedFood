import { id } from '@swimlane/ngx-charts';
import { template } from '@node_modules/@types/lodash';
import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, SimpleChanges, Type } from '@angular/core';
import { SafeHtml } from '@node_modules/@angular/platform-browser';
import {    WhatsAppHeaderUrl,
  WhatsAppButtonModel, FacebookTemplateDto,MessageTemplateModel, TemplateContent, WhatsAppComponentModel, WhatsAppExampleModel, WhatsAppMessageTemplateServiceProxy } from '@shared/service-proxies/service-proxies';
import { TemplateService } from '../../Facebook-Template/Template.service';
// import { HttpClient } from '@node_modules/@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { MessageService } from '@node_modules/abp-ng2-module/public-api';
import { FastClick } from '@metronic/themes/theme12/vendors/js/forms/toggle/switchery';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-ContentTemplate',
  templateUrl: './ContentTemplate.component.html',
  styleUrls: ['./ContentTemplate.component.css']
})
export class ContentTemplateComponent implements OnInit {

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
    isArabicFooter: boolean = false;
    isArabic: boolean = false;
    isArabicHeader: boolean = false;

  @Output() errorSampleBodyChange = new EventEmitter<boolean>();
  @Output() errorsampleHeaderChange = new EventEmitter<boolean>();  
  @Output() errorHeaderChange = new EventEmitter<boolean>();  
  @Output() errorStatusChanged = new EventEmitter<boolean>();
  @Output() errorBodyTextIsEmpty = new EventEmitter<boolean>();

  errorBodyIsEmpty :boolean=false;
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
  @Input() template: any ;

  whatsAppHeaderHandle: WhatsAppHeaderUrl = new WhatsAppHeaderUrl();

  bodyTextContent: string | undefined;

  
  @Input() componentBody:WhatsAppComponentModel
  @Input() componentFooter:WhatsAppComponentModel
  @Input() componentHeader: WhatsAppComponentModel;
  @Output() componentHeaderChange = new EventEmitter<WhatsAppComponentModel>();
  @Output() componentBoodyChange = new EventEmitter<WhatsAppComponentModel>();
  @Output() componentFooterChange = new EventEmitter<WhatsAppComponentModel>();

  @Output() componentTextChange = new EventEmitter<WhatsAppComponentModel>();
  @Output() whatsAppHeaderChanged = new EventEmitter<WhatsAppHeaderUrl>();

  // updateErrorStates() {
  //   this.errorSampleBodyChange.emit(this.errorAddSampleBody);
  //   this.errorsampleHeaderChange.emit(this.errorAddSampleHeader);
  //   this.errorHeaderChange.emit(this.errorHeadertext);
  // }

      mediaUrl: WhatsAppExampleModel = new WhatsAppExampleModel();
      message: MessageService;
  
  onHeaderTextChange(value: string) {
    this.componentHeader.text = value;
    if (!this.componentHeader?.text || this.componentHeader.text.trim() === '') {
        this.errorHeadertext=true;
        this.errorAddMedia=false;
    }
    else{
        this.errorHeadertext=false;
        this.errorAddMedia=false;
    }
      this.componentHeaderChange.emit(this.componentHeader);
      this.errorHeaderChange.emit(this.errorHeadertext);
      this.errorStatusChanged.emit(this.errorAddMedia);
  }

  onBodyTextChange(value: string) {
    this.componentBody.text = value;
    if(this.componentBody.text.length==0){
      this.errorBodyIsEmpty=true;
      this.errorBodyTextIsEmpty.emit(this.errorBodyIsEmpty);
    }
    else if(this.errorBodyIsEmpty==true && this.componentBody.text.length!=0){
      this.errorBodyIsEmpty=false;
      this.errorBodyTextIsEmpty.emit(this.errorBodyIsEmpty);

    }
    this.componentBoodyChange.emit(this.componentBody);  
    this.parseDynamicText(value);     
  }

  onfooterTextChange(value: string) {
    this.componentFooter.text = value;
    this.componentFooterChange.emit(this.componentFooter);
  }

  footer: string = "";
  Template: MessageTemplateModel = new MessageTemplateModel();
  TemplateId: string;
  // template: MessageTemplateModel;
  bodyText: string;
  imageFlag: boolean = false;
  videoFlag: boolean = false;
  documentFlag: boolean = false;

  image:string;
  element: ElementRef;

  private http: HttpClient;

  constructor(private templateService:TemplateService ,injector: Injector, http: HttpClient ) {
    this.http = http;

    // this.combineAndFilterButtons();
  }
  initializeTemplate(): void {
    this.template = new MessageTemplateModel();
    this.componentHeader = new WhatsAppComponentModel();
    this.componentBody = new WhatsAppComponentModel();
    this.componentFooter = new WhatsAppComponentModel();
  
  }
  
  updateBodyTextContent(newText: string): void {
    const bodyComponent = this.facebookTemplateDto.components.find(c => c.type === 'BODY');
    if (bodyComponent) {
        bodyComponent.text = newText;
        this.parseDynamicText(newText);
    }
}
  ngOnChanges(changes: SimpleChanges) {
    if (changes.template && !changes.template.firstChange) {
    }
  }
// ngOnChanges(changes: SimpleChanges): void {
//   if (changes['facebookTemplateDto']) {
//     console.log('Received facebookTemplateDto:', changes['facebookTemplateDto'].currentValue);
//   }
// }

  ngOnInit() {
    if(!this.template.id){
      this.template;
       this.componentHeader = this.template?.components?.find(
        (i: { type: string }) => i.type === "HEADER"
      ) || null;

    if (this.componentHeader) {
          if (!this.componentHeader.example) {
            this.componentHeader.example = new WhatsAppExampleModel();
          }

          this.componentHeader.example.header_text = this.componentHeader.example.header_text || [""];
          this.componentHeader.example.header_handle = this.componentHeader.example.header_handle || [""];

          this.componentHeader.example.header_text[0] = "";
          this.componentHeader.example.header_handle[0] = "";
        }
      


      this.componentBody = this.template?.components?.find(
        (i: { type: string }) => i.type === "BODY"
      ) || null;

      this.componentBody.add_security_recommendation=null;
      this.componentBody.text="hello"
      this.componentFooter = this.template?.components?.find(
        (i: { type: string }) => i.type === "FOOTER"
      ) || null;

      if (this.componentBody && this.componentBody.example && Array.isArray(this.componentBody.example.body_text)) {
        this.dynamicVariables = this.componentBody.example.body_text;
      } else {
        this.dynamicVariables = [];
      }
    }
  else{
      this.componentHeader = this.template?.components?.find(
        (i: { type: string }) => i.type === "HEADER"
      ) || null;
      if(this.componentHeader){
        
        if(this.componentHeader?.format=='IMAGE'||this.componentHeader?.format=='VIDEO'||this.componentHeader?.format=='DOCUMENT'){
          this.errorAddMedia=false;
          if(!this.componentHeader?.example||this.componentHeader?.example==null||this.componentHeader?.example==undefined){
            const example = new WhatsAppExampleModel();
            // example.header_text = [""];
            example.header_handle = [""];
            this.componentHeader.example=example;   
          }
        }
        else if(this.componentHeader?.format=='TEXT'){
          if(!this.componentHeader?.example||this.componentHeader?.example==null||this.componentHeader?.example==undefined){
          const example = new WhatsAppExampleModel();
          // example.header_text = [""];
          example.header_handle = [""];
          this.componentHeader.example=example;
          }
        }
      }
  
      this.componentBody = this.template?.components?.find(
        (i: { type: string }) => i.type === "BODY"
      ) || null;
      
      this.componentFooter = this.template?.components?.find(
        (i: { type: string }) => i.type === "FOOTER"
      ) || null;

      if (this.componentBody && this.componentBody.example && Array.isArray(this.componentBody.example.body_text)) {
        this.dynamicVariables = this.componentBody.example.body_text;
      } else {
        this.dynamicVariables = [];
      }
 }



 }
  
  getapi(){
    this.componentHeader = this.template?.components?.find(
      (i: { type: string }) => i.type === "HEADER"
    ) || { format: 'None', text: '', example: null }; 
    
    this.componentBody = this.template?.components?.find(
      (i: { type: string }) => i.type === "BODY"
    ) || { text: '' };
    
    this.componentFooter = this.template?.components?.find(
      (i: { type: string }) => i.type === "FOOTER"
    ) || { text: '' }; 
    
    if (this.componentBody && this.componentBody.example && Array.isArray(this.componentBody.example.body_text)) {
      this.dynamicVariables = this.componentBody.example.body_text;
    } else {
      this.dynamicVariables = [];
    }
    
  }

  isValidText(text) {
    const regex = /^(?=(?:\S*\s\S*\s\S*\s*)|$)(?!\s*$).*$/;
  
    const trimmedString = text?.replace(/\s+$/, "");
  
    return regex.test(trimmedString);
  }
  
  onHeaderChange(event: any): void {
    this.componentHeader.format= event.target.value;
    if(this.componentHeader.format=='None'){
      this.componentHeader.example=null;
    }
   else if(this.componentHeader.format=="TEXT"){
      this.componentHeader.text="";
    if(!this.componentHeader.example||this.componentHeader.example==undefined){
      const example = new WhatsAppExampleModel();
      example.header_text = [""];
      this.componentHeader.example = example;
     }
    if(this.componentHeader?.text=='' ){
       this.errorHeadertext=true;
       this.errorAddMedia=false;
     }
     else if(this.headervariable==1){
      this.errorAddSampleHeader=true;
     }
    else{
       this.errorHeadertext=false;
       this.errorAddMedia=false;
      }
    this.componentHeaderChange.emit(this.componentHeader);
    this.errorHeaderChange.emit(this.errorHeadertext);
    this.errorStatusChanged.emit(this.errorAddMedia);
    this.errorsampleHeaderChange.emit(this.errorAddSampleHeader);
    }
    else if(this.componentHeader.format=='IMAGE'||this.componentHeader.format=='VIDEO'||this.componentHeader.format=='DOCUMENT'){
      this.componentHeader.text='';
      this.errorAddMedia=true;
      this.errorStatusChanged.emit(this.errorAddMedia);
    }
    this.clearUploadedFile(); 
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

  addVariableHeader() {
    if (this.componentHeader.text === null|| !this.componentHeader.text ) {
      this.componentHeader.text= '';  
      this.componentHeader.text +='{{1}}'
    }
    else {
      this.componentHeader.text +='{{1}}'
    }
    if (!this.componentHeader.example) {
      this.componentHeader.example = new WhatsAppExampleModel();
  }

  if (!this.componentHeader.example.header_text) {
      this.componentHeader.example.header_text = [];
  }
    this.headervariable+=1;
    this.errorAddSampleHeader=true;

    this.errorsampleHeaderChange.emit(this.errorAddSampleHeader);
  }

  removeVariableHeader() {
    if (this.componentHeader.text) {

      this.componentHeader.text = this.componentHeader.text.trim().replace('{{1}}', '');
      this.componentHeader.example.header_text[0] ='';
    }
    this.headervariable -= 1;
    if(this.componentHeader.text !=''){
    this.errorAddSampleHeader=false;
    }
    this.errorsampleHeaderChange.emit(this.errorAddSampleHeader);
  }
  
  
  // uploadFile(event: Event, type: string): void {
  //   const fileInput = event.target as HTMLInputElement;
  //   const file = fileInput.files?.[0];
  
  //   if (file) {
  //     console.log(`Uploading ${type}:`, file);
  //     this.fileToUpload = file; // Store the file for later use
  //     this.extFile = this.fileToUpload.name.split('.').pop() || ''; // Get the file extension
  
  //     const formData = new FormData();
  //     formData.append('formFile', this.fileToUpload);
  
  //     switch (type) {
  //       case 'image':
  //         this.uploadedVideoUrl = null; // Clear other file type URLs
  //         this.uploadedDocumentUrl = null;
  //         this.handleImageUpload(formData);
  //         break;
  //       case 'video':
  //         this.uploadedImageUrl = null; // Clear other file type URLs
  //         this.uploadedDocumentUrl = null;
  //         this.handleVideoUpload(formData);
  //         break;
  //       case 'document':
  //         this.uploadedImageUrl = null; // Clear other file type URLs
  //         this.uploadedVideoUrl = null;
  //         this.handleDocumentUpload(formData);
  //         break;
  //     }
  
  //     this.mediaErorrmethod(); // Check if there's no file uploaded
  //   } else {
  //     this.mediaErorrmethod(); // Handle the case where no file is uploaded
  //   }
  // }

  uploadFile(event: Event, type: string): void {
    const fileInput = event.target as HTMLInputElement;
    const file = fileInput.files?.[0];
  
    if (file) {
      console.log(`Uploading ${type}:`, file);
      this.fileToUpload = file; 
      this.extFile = this.fileToUpload.name.split('.').pop() || ''; 
  
      const formData = new FormData();
      formData.append('formFile', this.fileToUpload);
  
      switch (type) {
        case 'image':
          this.uploadedVideoUrl = null; 
          this.uploadedDocumentUrl = null;
          this.handleImageUpload(formData);
          break;
        case 'video':
          this.uploadedImageUrl = null;
          this.uploadedDocumentUrl = null;
          this.handleVideoUpload(formData);
          break;
        case 'document':
          this.uploadedImageUrl = null; 
          this.uploadedVideoUrl = null;
          this.handleDocumentUpload(formData);
          break;
      }
  
      this.mediaErorrmethod(); 
    } else {
      this.mediaErorrmethod(); 
    }
  }
  
  
  

  handleImageUpload(formData: FormData): void {
    if (this.isImageValid()) {
        const example = new WhatsAppExampleModel(); 
        this.uploadImage(this.fileToUpload, example); 
    }
}

  handleVideoUpload(formData: FormData): void {
    if (this.isVideoValid()) {
      const example = new WhatsAppExampleModel(); 
      this.uploadImage(this.fileToUpload, example); 
    }
  }

  handleDocumentUpload(formData: FormData): void {
    if (this.isDocumentValid()) {
      console.log('Document uploaded successfully:', formData);
      this.uploadDocument(this.fileToUpload); 
    }
  }

  isImageValid(): boolean {
    const validExtensions = ['jpg', 'jpeg', 'png'];
    if (!validExtensions.includes(this.extFile.toLowerCase())) {
      console.error('Invalid image format. Please choose a jpg, jpeg, or png image.');
      return false;
    }
    return true;
  }

  isVideoValid(): boolean {
    if (this.extFile.toLowerCase() !== 'mp4') {
      console.error('Invalid video format. Please choose an mp4 video.');
      return false;
    }
    if (this.fileToUpload && this.fileToUpload.size > 30000000) {
      console.error('Video is too large. Maximum size is 30MB.');
      return false;
    }
    return true;
  }

  isDocumentValid(): boolean {
    const validExtensions = ['pdf'];
    if (!validExtensions.includes(this.extFile.toLowerCase())) {
      console.error('Invalid document format. Please choose a pdf document.');
      return false;
    }
    return true;
  }

  uploadImage(file: File, example: WhatsAppExampleModel): void {
    const reader = new FileReader();
  
    reader.onload = (event) => {
      const uploadedImageUrl = event.target?.result as string;
  
      this.componentHeader = this.template.components.find(
        (component: { type: string }) => component.type === "HEADER"
      );
  
      if (!this.componentHeader) {
        console.error("HEADER component not found. Creating new component...");
        
        const newHeaderComponent = new WhatsAppComponentModel();
        newHeaderComponent.type = "HEADER";
        newHeaderComponent.format = "IMAGE"; 
        const example = new WhatsAppExampleModel();
        example.header_handle = [uploadedImageUrl];
  
        this.template.components.push(newHeaderComponent);
        this.componentHeader = newHeaderComponent; 
      } else {
        if (!this.componentHeader.example) {
          this.componentHeader.example = new WhatsAppExampleModel(); 
        }
  
        if (!this.componentHeader.example.header_handle) {
          this.componentHeader.example.header_handle = []; 
        }
  
        this.componentHeader.example.header_handle[0] = uploadedImageUrl;
      }
    };
    reader.readAsDataURL(file);
    this.image = this.template.components.find(
      (component: { format: string }) => component.format === "IMAGE"
    );

  }
  
uploadVideo(file: File, example: WhatsAppExampleModel): void {
  const reader = new FileReader();
  reader.onload = (event) => {
      const uploadedVideoUrl = event.target?.result as string; 
      
      this.componentHeader = this.template.components.find(
          (i: { type: string }) => i.type === "HEADER"
      );

      if (this.componentHeader) {
          if (!this.componentHeader.example) {
              this.componentHeader.example = new WhatsAppExampleModel();
          }

          if (!this.componentHeader.example.header_handle) {
              this.componentHeader.example.header_handle = []; 
          }

          this.componentHeader.example.header_handle[0] = uploadedVideoUrl;
      } else {
          const newHeaderComponent = {
              type: "HEADER",
              format: "VIDEO",
              example: {
                  header_handle: [uploadedVideoUrl],
              },
          };

          this.template.components.push(newHeaderComponent);
      }
  };
  reader.readAsDataURL(file); 
}

  uploadDocument(file: File): void {
    const reader = new FileReader();
    reader.onload = (event) => {
      this.uploadedDocumentUrl = event.target?.result; 
    };
    reader.readAsDataURL(file);

  }

  mediaErorrmethod(){
    if(this.fileToUpload ==null && this.uploadedImageUrl==null &&this.uploadedVideoUrl==null &&this.uploadedDocumentUrl==null){
      this.mediaErorr= false;
    }
    this.mediaErorr= true;
  }

  parseDynamicText( text: string): { type: "text" | "input"; content: string; index?: number }[] {
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
    // this.formatText();
    return parts;
  }


checkVariable(parts: any) {
  const regex = /\{\{(\d*)\}\}/g;

  let holderText = this.componentBody.text;
  let matches = this.componentBody.text.match(regex);
  if (!matches) return;


  matches.forEach((match, index) => {
      const randomNumber = Math.floor(Math.random() * (1000 - 5 + 1)) + 5;
      holderText = holderText.replace(match, `{{${randomNumber}}}`);
  });

  matches = holderText.match(regex);

  matches.forEach((match, index) => {
      if (index > 100) {
          holderText = holderText.replace(match, "");
      } else {
          holderText = holderText.replace(match, `{{${++index}}}`);
      }
  });
  this.dynamicVariables.length;
  this.dynamicVariables.length = holderText.match(regex).length;
  this.componentBody.text = holderText;
  this.componentBody.example.body_text = this.dynamicVariables;

}
// formatText() {
  
//   // Step 1: Normalize bold markersthis.facebookTemplateDto.components
//   if (this.facebookTemplateDto.content?.body) {
//       let textWithBold = this.componentBody.text.replace(
//           /\*{1,}(.*?)\*{1,}/g,
//           "<strong>$1</strong>"
//       );

//       // Step 2: Normalize underline markers
//       let textWithUnderline = textWithBold.replace(
//           /_{1,}(.*?)_{1,}/g,
//           "<u>$1</u>"
//       );

//       // Step 3: Apply the formatting to the text
//       this.formattedText = textWithUnderline;
//   } else {
//       this.formattedText = "";
//   }

//   this.formattedTextBody = this.sanitizer.bypassSecurityTrustHtml(
//       this.formattedText
//   );
// }
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

addEmoji(event: any): void {
  this.componentBody.text += event.emoji.native;
}



onBodyInput() {
  this.checkTemplateVariables();

}

checkTemplateVariables(): void {
  const variableRegex = /{{\d+}}/g;
  const matches = this.body.match(variableRegex); 

  const numVariables = matches ? matches.length : 0;

  let numWords = 0;
  let inWord = false; 

  for (let i = 0; i < this.body.length; i++) {
    const char = this.body[i];

    if (char === ' ' || char === '\n' || char === '\t') {
      inWord = false;
    } else if (!inWord) {
      numWords++;
      inWord = true; 
    }
  }

  if (numWords - this.dynamicVariables.length < this.dynamicVariables.length * 2) {
    this.showWarningMessage = true;
  } else {
    this.showWarningMessage = false; 
  }
}

// updateVariablesInBody() {
//   let updatedBody = this.body;

//   // Extract all variables from the body using regex (e.g., {{1}}, {{2}})
//   const variableMatches = Array.from(updatedBody.matchAll(/{{(\d+)}}/g));

//   // Extract variable numbers from the matched regex
//   const extractedNumbers = variableMatches.map(match => parseInt(match[1], 10));

//   // Set the list of variables based on the extracted numbers
//   this.variables = extractedNumbers.map(num => ({ number: num }));

//   // Update the body content by ensuring the correct variable format
//   this.body = this.variables.reduce((newBody, variable) => {
//     const regex = new RegExp(`{{${variable.number}}}`, 'g');
//     return newBody.replace(regex, `{{${variable.number}}}`);
//   }, updatedBody);
// }

updateValidationMessage() {
  this.totalCharacters = this.body.length;
  if (this.totalCharacters > this.maxCharacters) {
    this.validationMessage = `Message exceeds ${this.maxCharacters} characters limit!`;
  } else {
    this.validationMessage = '';
  }
}


applyFormat(startTag: string, endTag: string): void {
  const textarea: HTMLTextAreaElement = document.getElementById('bodyText') as HTMLTextAreaElement;
  const selectionStart = textarea.selectionStart;
  const selectionEnd = textarea.selectionEnd;

  const selectedText = this.componentBody.text.slice(selectionStart, selectionEnd);
  
  this.componentBody.text = 
    this.componentBody.text.slice(0, selectionStart) +
    startTag + selectedText + endTag +
    this.componentBody.text.slice(selectionEnd);

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
 

addVariable() {
  if (this.totalCharacters < this.maxCharacters) {
    const newNumber = this.variables.length + 1;
    this.variables.push({ number: newNumber });
    this.sampleContents[newNumber] = ''; 
    this.variableAdded = true;
    this.insertVariableInBody(newNumber);
    this.checkAllSampleInputs(); 
  } else {
    this.validationMessage = `You can't add more variables. Total character count exceeds ${this.maxCharacters}.`;
  }
}

insertVariableInBody(variableNumber: number) {
  const variableText = `{{${variableNumber}}}`;
  this.body += ` ${variableText}`; 
}


checkAllSampleInputs() {
  this.isSampleErorr = false; 
  for (let variable of this.listOfVariables) {
    if (!this.sampleContents[variable.number] || this.sampleContents[variable.number].trim() === '') {
      this.isSampleErorr = true; 
      break;
    }
  }
}

updateSampleContent(variableNumber: number, event: any) {
  this.sampleContents[variableNumber] = event.target.value;
  this.checkAllSampleInputs();
}


deleteVariable(index: number) {
  const variableToDelete = this.variables[index];

  this.body = this.body.replace(`{{${variableToDelete.number}}}`, '');

  delete this.sampleContents[variableToDelete.number];

  this.variables.splice(index, 1);

  this.reorderVariables();

  this.checkTemplateVariables();
}

reorderVariables() {
  this.variables.sort((a, b) => a.number - b.number);

  this.variables.forEach((variable, index) => {
    variable.number = index + 1;
  });
}

updateBodyWithVariables() {
  let updatedBody = this.body;

  this.variables.forEach(variable => {
    const regex = new RegExp(`{{${variable.number + 1}}}`, 'g'); 
    updatedBody = updatedBody.replace(regex, `{{${variable.number}}}`); 
  });

  this.body = updatedBody;
}

jsdn(){
  this.dynamicVariables = this.componentBody.example.body_text;

}
onInputChange(event: Event, groupIndex: number, index: number): void {
  debugger
  const input = (event.target as HTMLInputElement).value;
  this.dynamicVariables[groupIndex][index] = input;
  this.checkInputsForEmpty();
  debugger
  this.componentBody.example.body_text;
}

checkInputsForEmpty() {
  debugger;
  this.errorAddSampleBody = this.dynamicVariables[0].some(value => value.trim() === '');
  this.errorSampleBodyChange.emit(this.errorAddSampleBody);
}

trackByGroup(index: number, group: any): number {
  return index;
}

trackByInput(index: number, input: any): number {
  return index;
}

getTextDirection(text: string): 'rtl' | 'ltr' {
  const arabicRegex = /[\u0600-\u06FF]/;
  return arabicRegex.test(text || '') ? 'rtl' : 'ltr';
}

getMaxLength(): number {
  if (this.template?.category === 'MARKETING') {
    if (this.template?.sub_category === 'CUSTOM' ) {
      return 1024;
    }
    if (this.template?.sub_category === 'carousel') {
      return 160;
    }
  }
  return 1024;
}
getTextDirection2(text: string, field: 'header' | 'footer'): 'rtl' | 'ltr' {
  const arabicRegex = /[\u0600-\u06FF]/;
  const isArabic = arabicRegex.test(text || '');

  if (field === 'header') {
    this.isArabicHeader = isArabic;
  } else if (field === 'footer') {
    this.isArabicFooter = isArabic;
  }

  return isArabic ? 'rtl' : 'ltr';
}
addVariableOne() {
  if (this.componentBody.text) {
    if (this.dynamicVariables.length <15 &&  this.dynamicVariables.length < 1024 - this.componentBody.text.length ) {
        this.dynamicVariables.length += 1;
        this.componentBody.text += " {{" + this.dynamicVariables.length + "}}  . .";

        if (!this.componentBody.example) {
            this.componentBody.example = new WhatsAppExampleModel();
        }

        if (!this.componentBody.example.body_text) {
            this.componentBody.example.body_text = [];
        }

        if (!Array.isArray(this.dynamicVariables[0])) {
            this.dynamicVariables[0] = [];
        }
        this.dynamicVariables[0].push('');
        if(!this.template.variableCount){
          this.template.variableCount=1;
        }else{
        this.template.variableCount+=1;
        }
        this.checkInputsForEmpty();

        this.parseDynamicText(this.componentBody.text);
    }
  }

}
test(){
  debugger;
  this.template;
}
deleteVaribale() {
  if (this.componentBody.text) {
    if (this.dynamicVariables.length > 0) {
      const variableNumber = this.dynamicVariables.length;

      const pattern = new RegExp(`\\{\\{${variableNumber}\\}} \\. \\.`, "g");

      const isPatternPresent = pattern.test(this.componentBody.text);

      if (isPatternPresent) {
        this.componentBody.text = this.componentBody.text.replace(pattern, "");
      } else {
        this.componentBody.text = this.componentBody.text.replace(`{{${variableNumber}}}`, "");
      }       
      
      debugger


      if (this.dynamicVariables[0] && this.dynamicVariables[0].length > 0) {
        this.dynamicVariables[0].pop(); 
                this.template.variableCount-=1;
      }

      this.componentBody.example.body_text = this.dynamicVariables;

      this.parseDynamicText(this.componentBody.text);
      this.checkInputsForEmpty() 

    } else {
      this.checkInputsForEmpty() 

      return;
    }
  }
}



incrementedIndex(index: number): number {
  return index + 1;
}



handleFileInput(event: { target: { files: any[] } }) {
  debugger
  this.fileToUpload = event.target.files[0];
  let formDataFile = new FormData();

  this.extFile = this.fileToUpload.name.substring(
    this.fileToUpload.name.lastIndexOf(".") + 1
  );

  if (this.componentHeader.format == "IMAGE") {
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
        this.GetHeaderHandle(formDataFile);
        this.template.mediaType = "image";
      }
    } else {
      this.element.nativeElement.value = "";
      event.target.files[0] = null;
      this.fileToUpload = null;
    }
  }

  if (this.componentHeader.format == "VIDEO") {
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
        this.GetHeaderHandle(formDataFile);
        this.template.mediaLink = "";
        this.videoFlag = false;
      }
    } else {
      this.element.nativeElement.value = "";
      event.target.files[0] = null;
      this.fileToUpload = null;
    }
  }

  if (this.componentHeader.format == "DOCUMENT") {
    if (this.extFile == "pdf") {
      if (this.fileToUpload.size > 100 * 1024 * 1024) {
        Swal.fire({
          icon: 'error',
          title: 'Oops...',
          text: 'The uploaded document exceeds 100 MB. Please select a smaller file.',
        });
        this.element.nativeElement.value = "";
        event.target.files[0] = null;
        this.fileToUpload = null;
      } else {
        formDataFile.append("formFile", this.fileToUpload);
        this.GetHeaderHandle(formDataFile);
        this.template.mediaLink = "";
      }
    } else {
      this.element.nativeElement.value = "";
      event.target.files[0] = null;
      this.fileToUpload = null;
      this.documentFlag = false;
    }
  }
}

GetHeaderHandle(file: FormData) {  
  debugger;
  const example = new WhatsAppExampleModel();
  // example.header_text = [""];
  example.header_handle = [""];
  this.componentHeader.example=example;

  this.http
      .post<WhatsAppHeaderUrl>(
          AppConsts.remoteServiceBaseUrl +
              "/api/services/app/WhatsAppMessageTemplate/GetWhatsAppMediaLink",
          file
      )
      .subscribe((result) => {
        debugger
          this.whatsAppHeaderHandle = new WhatsAppHeaderUrl();
          this.componentHeader.example.header_handle[0] = result.infoSeedUrl;
          this.template.mediaLink = result.infoSeedUrl;
          this.whatsAppHeaderHandle = result;
          this.whatsAppHeaderChanged.emit(this.whatsAppHeaderHandle);
      });

      this.componentHeader.type="HEADER";
      this.template.mediaLink;
      this.errorAddMedia=false;
      this.errorStatusChanged.emit(this.errorAddMedia);
      this.componentHeaderChange.emit(this.componentHeader);
    }

    clearUploadedFile() {
      this.fileToUpload = null;
      this.template.mediaLink = "";
      if(this.componentHeader.format!='None'&&this.componentHeader.format!='TEXT'){
        if (this.componentHeader.example && this.componentHeader.example.header_handle) {
          this.componentHeader.example.header_handle[0] = ""; 
        }
      
        this.errorAddMedia = true;
      
        this.errorStatusChanged.emit(this.errorAddMedia);
        this.componentHeaderChange.emit(this.componentHeader);
  
        const fileInputElement = document.getElementById("headerImage") as HTMLInputElement;
        if (fileInputElement) {
          fileInputElement.value = "";
        }
      }
      else if(this.componentHeader.format=='None'){
        this.errorHeadertext=false;
        this.errorAddMedia=false;
        this.errorHeaderChange.emit(this.errorHeadertext);
        this.errorStatusChanged.emit(this.errorAddMedia);
        this.errorsampleHeaderChange.emit(false);
      }

    }
    
    

}



