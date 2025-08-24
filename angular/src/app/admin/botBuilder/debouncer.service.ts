import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DebouncerService {
  private subject = new Subject<any>();

  constructor() {
    this.subject.pipe(debounceTime(200)).subscribe((func) => func());
  }

  debounce(func: Function): void {
    this.subject.next(func);
  }
}
