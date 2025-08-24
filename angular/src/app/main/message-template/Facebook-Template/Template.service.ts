import { Injectable } from '@angular/core';
import { Router } from '@node_modules/@angular/router';
import { BehaviorSubject } from '@node_modules/rxjs';
import { MessageTemplateModel } from '@shared/service-proxies/service-proxies';

@Injectable({
  providedIn: 'root'
})
export class TemplateService {
  error(arg0: string, arg1: any) {
    throw new Error('Method not implemented.');
  }
  private template: MessageTemplateModel = new MessageTemplateModel();
  setTemplate(template: MessageTemplateModel): void {
    this.template = template;
  }

  getTemplate(): MessageTemplateModel {
    return this.template;
  }
constructor(private router: Router) { } 
private selectedCategorySubject = new BehaviorSubject<string>('');
private selectedMessageTypeSubject = new BehaviorSubject<string>('');
 // Set selected category - only if not already set
 setSelectedCategory(category: string): void {
  if (!this.selectedCategorySubject.value) {
    this.selectedCategorySubject.next(category);
  }
}

// Get selected category as Observable
getSelectedCategory() {
  return this.selectedCategorySubject.asObservable();
}

// Set selected message type - only if not already set
setSelectedMessageType(messageType: string): void {
  if (!this.selectedMessageTypeSubject.value) {
    this.selectedMessageTypeSubject.next(messageType);
  }
}

// Get selected message type as Observable
getSelectedMessageType() {
  return this.selectedMessageTypeSubject.asObservable();
}

// Navigate to the desired route
next(): void {
  this.router.navigate(['/app/main/EditTemplateFacebook']);
}
}