import { Injectable } from '@angular/core';
import { AngularFireMessaging } from '@angular/fire/compat/messaging';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { getMessaging, getToken, onMessage } from "firebase/messaging";
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class WebNotificationsService {
  currentMessage = new BehaviorSubject(null);
  message:any = null;
  constructor(private angularFireMessaging: AngularFireMessaging,private router: Router
    ) {
    // this.angularFireMessaging.messages.subscribe(
    //   (message) => {
    //     console.log('Received FCM message:', message);
    //     this.currentMessage.next(message);
    //   }
    // );
   }

  requestPermission() {
    this.angularFireMessaging.requestToken.subscribe(
      (token) => {
        
        // console.log('Permission granted! Save to the server!', token);
        // Send the token to your server for further processing, if required.
      },
      (error) => {

        // console.error('Unable to get permission to notify.', error);
      }
    );
  }

  receiveMessages() {
  //   this.angularFireMessaging.messages.subscribe(
  //     (payload) => {
  //       console.log('New message received. ', payload);
  //       this.currentMessage.next(payload);
  //     }
  //   );
  }

  showNotification(title,message: string,headerTab) {

    if (Notification.permission === 'granted') {
      const options = {
        body: message,
        click_action: "open_url"
      };
      const notification = new Notification(title, options);
    
    }
  }
  

  
  
  listen() {
    const messaging = getMessaging();
    onMessage(messaging, (payload) => {
      this.message=payload;

    });
  }
}
