import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  private groupNameSource = new BehaviorSubject<string>('');

  currentGroupName = this.groupNameSource.asObservable();


constructor() { }

changeGroupName(groupName : string){
  this.groupNameSource.next(groupName)
}

}
