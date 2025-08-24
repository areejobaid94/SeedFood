import { Language } from './../../../../../shared/service-proxies/service-proxies';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { template } from '@node_modules/@types/lodash';
import { FacebookTemplateDto,WhatsAppMessageTemplateServiceProxy, MessageTemplateModel, TemplateNameLanguageFacebook } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-TemplateNameAndLanguage',
  templateUrl: './TemplateNameAndLanguage.component.html',
  styleUrls: ['./TemplateNameAndLanguage.component.css']
})
export class TemplateNameAndLanguageComponent implements OnInit {
  tempName:TemplateNameLanguageFacebook;
  isUsed: boolean=false;
  isValidName = true;
  isNameUsed: boolean = false;
  isRecentlyDeleted: boolean = false;
  regexp = new RegExp("^[a-zA-Z0-9_.-]*$");
  submitted = false;
  checkTemplate: MessageTemplateModel[] = [new MessageTemplateModel()];
  // template: MessageTemplateModel = new MessageTemplateModel();
  IsStep2: boolean = false;

  @Input() template: any = {};
  @Input() templateId :string;
  @Output() languageChanged = new EventEmitter<string>();

  @Output() isUsedChange = new EventEmitter<boolean>(); 

  templateNameDisbelid:boolean=false;

  onLanguageChange() {
    this.languageChanged.emit(this.template.language); 
  }
  constructor(private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy) { }

  ChecktemplateId(){
    if(this.templateId!=null){
      this.templateNameDisbelid=true;
    }
  }
  ngOnInit() {
    this.ChecktemplateId();
    this.getMessageTemplate();
    this.template.language = 'en';
  }

 checkTemplateName(name: string): boolean {
    this.isUsed = false;
    this.checkTemplate.forEach((element) => {
      if (element.name == name.toLowerCase().split(" ").join("_")) {
        
        this.isUsed = true;
       // this.message.error("", this.l("thisNameAlreadyUsed"));
       this.isUsedChange.emit(this.isUsed);
       return;
      }
    });
  this.isUsedChange.emit(this.isUsed);
  return this.isUsed;
  }

getMessageTemplate() {
  this._whatsAppMessageTemplateServiceProxy
      .getWhatsAppMessageTemplate()
      .subscribe((result) => {

          this.checkTemplate = result.data;
      });
}
onTemplateNameChange(event: Event) {
  const input = event.target as HTMLInputElement;
  let updatedValue = input.value;
  updatedValue = updatedValue.toLowerCase();
  updatedValue = updatedValue.replace(/\s+/g, '_');
  updatedValue = updatedValue.replace(/[^\w]/g, '');
  this.template.name = updatedValue;
  input.value = updatedValue;
  this.isUsed = this.checkTemplate.some(
    (element) => element.name === updatedValue
  );
  this.isUsedChange.emit(this.isUsed || updatedValue.length === 0);
  this.IsStep2 = updatedValue.length > 0;
}

preventInvalidChars(event: KeyboardEvent) {

  const allowedChars = /^[a-zA-Z0-9_ ]$/;
  
  const specialKeys = [
    'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 
    'Tab', 'Home', 'End'
  ];

  if (specialKeys.includes(event.key) || allowedChars.test(event.key)) {
    return;
  }
  
  event.preventDefault();
}


replaceSpaceWithUnderscore(event: KeyboardEvent) {
  if (event.code === 'Space') {
    event.preventDefault(); 
    const input = event.target as HTMLInputElement;

    const start = input.selectionStart || 0;
    const end = input.selectionEnd || 0;

    const value = input.value;
    const newValue = value.substring(0, start) + '_' + value.substring(end);

    input.value = newValue;
    this.template.name = newValue;

    setTimeout(() => {
      input.setSelectionRange(start + 1, start + 1);
    });

    this.onTemplateNameChange({ target: input } as any);
  }
}



}