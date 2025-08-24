import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'CustomCurrencyPipe'
})
export class CustomCurrencyPipePipe implements PipeTransform {

  transform(value: number = 0): string {
    // Check if the decimal part is zero
    if (value % 1 === 0) {
      // If decimal part is zero, format without decimals
      return value.toFixed(0);
    } else {
      // If decimal part is non-zero, format with decimals
      return value.toFixed(2);
    }
  }
}
