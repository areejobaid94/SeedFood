import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appNoSpecialCharecter]'
})
export class NoSpecialCharecterDirective {

  constructor(private el : ElementRef) { }
  @HostListener('keypress',['$event'])
    onKeyPress(e:KeyboardEvent){
      const input = e.key;
      if (!/^[a-zA-Z0-9]*$/.test(input)) {
        e.preventDefault();
      }

    }

}
