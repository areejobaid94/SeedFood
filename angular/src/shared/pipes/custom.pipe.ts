import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml, SafeStyle, SafeScript, SafeUrl, SafeResourceUrl } from '@angular/platform-browser';

@Pipe({
  name: 'custom'
})
export class CustomPipe implements PipeTransform {
  constructor(protected _sanitizer: DomSanitizer) {
  }
  transform(v: string): SafeHtml {
    
    return this._sanitizer.bypassSecurityTrustResourceUrl(v);
}

 }