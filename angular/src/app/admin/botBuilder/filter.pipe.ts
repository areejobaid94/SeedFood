import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filter'
})
export class FilterPipe implements PipeTransform {
    transform(value: any[], searchTerm: string, property: string): any[] {
        if (!searchTerm) {
          return value;
        }
        return value.filter(option => option[property].toLowerCase().includes(searchTerm.toLowerCase()));
      }
}