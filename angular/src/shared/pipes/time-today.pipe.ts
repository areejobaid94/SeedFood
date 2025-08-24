import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeToday'
})
export class TimeTodayPipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }

}
