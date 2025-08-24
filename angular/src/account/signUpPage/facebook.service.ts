import { Injectable } from '@angular/core';
declare global {
  interface Window {
    fbAsyncInit: () => void;
    FB: any;
  }
}

@Injectable({
  providedIn: 'root'
})
export class FacebookService {
  private fbLoaded = false;

  constructor() {
    this.loadFbSdk();
  }

  private loadFbSdk(): void {
    if (this.fbLoaded) return;

    window.fbAsyncInit = () => {
      window.FB.init({
        appId: '885586280068397',
        autoLogAppEvents: true,
        xfbml: true,
        version: 'v20.0'
      });
      this.fbLoaded = true;
    };

    // Load the Facebook SDK script
    ((d, s, id) => {
      const element = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) { return; }
      const fjs = element;
      const js = d.createElement(s) as HTMLScriptElement;
      js.id = id;
      js.async = true;
      js.defer = true;
      js.crossOrigin = "anonymous";
      js.src = "https://connect.facebook.net/en_US/sdk.js";
      fjs.parentNode.insertBefore(js, fjs);
    })(document, 'script', 'facebook-jssdk');
  }

  public getFB(): any {
    return window.FB;
  }
}