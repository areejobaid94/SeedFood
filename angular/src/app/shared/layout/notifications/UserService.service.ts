import { Injectable } from '@angular/core';
import { Subject } from '@node_modules/rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserServiceService {

  private selectUserSource = new Subject<string>();
  selectUser$ = this.selectUserSource.asObservable();
constructor() { }

selectUser(userId: string) {
  debugger
  this.selectUserSource.next(userId);
}
}
