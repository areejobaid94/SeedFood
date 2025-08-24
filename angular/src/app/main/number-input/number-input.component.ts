import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-number-input',
  templateUrl: './number-input.component.html',
  styleUrls: ['./number-input.component.css']
})
export class numberinput {
  numberValue: string = '';
  isValidNumber: boolean = true;

  @Input() InnerClass: string;
  @Input() pHolder :string;
  @Output() valueChange = new EventEmitter<string>();

  onInput(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    const value = inputElement.value;
    // Regix to make sure only numbers allowed
    const numericPattern = /^[0-9]*\.?[0-9]*$/;;
    
    

    // Check if the input Match the Regix
    if (numericPattern.test(value)) {
      this.numberValue = value;
      this.isValidNumber = true;
    } else {
      // Remove invalid characters from input
      this.numberValue = this.numberValue.replace(/[^\d.]/g, '');
      this.isValidNumber = false;
    }
    
    // Ensure the input field displays only numeric characters and emit this.numberValue to Parent
    inputElement.value = this.numberValue;
    this.valueChange.emit(this.numberValue);
  }
}
