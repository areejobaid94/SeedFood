import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { FacebookPagesModel, FacebookServiceProxy } from '@shared/service-proxies/service-proxies';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-facebook-connect',
  templateUrl: './facebook-connect.component.html',
})
export class FacebookConnectComponent implements OnInit {

  private appId = '885586280068397';
  private appSecret = '6bb8de39780aefe14f7a49a4acbf0c98';
  private redirectUri = AppConsts.appBaseUrl+'/app/admin/facebook-connect'; // Update for prod "https://developers.facebook.com/tools/explorer/callback";//
//https://www.facebook.com/v19.0/dialog/oauth?client_id=885586280068397&redirect_uri=https://developers.facebook.com/tools/explorer/callback&scope=pages_messaging,pages_manage_metadata,pages_show_list,pages_read_engagement&response_type=code&state=infoseed

  userToken: string;
  pages: any[] = [];
  isfirst=true;

  constructor(private route: ActivatedRoute, private http: HttpClient, private router: Router, private _facebookServiceProxy: FacebookServiceProxy) {}

  ngOnInit(): void {
    debugger
    const code = this.route.snapshot.queryParamMap.get('code');
    console.log('Code :'+code);

    if (code) {
      this.exchangeCodeForToken(code);
    }else{
      this.getUserPages();
      //this.exchangeCodeForToken('fgdfgdfgd');
     // this.userToken='EAAMlb5ZBkNS0BO4v0vwP6RZBxUotdySHhkFkSTeRApnPZBOw4ttr6ZAnZAkd929TXXwCSLrB45p8UgKlV2ULji951hTyMEP7SUpnkSfBQgUq25NGrFdEdZCAMI9HLYxRDHO1MgekUSfKysKtnyqgGGSbpkInjJaJxjDGFz7rz3jGdLH3UBeoCPmZCO6X69ZAfqWXyNV8qDkYmPGVg5UiVR2omkDrokRB0dMcZBerYMXj6KVDwduoZAmzhL';
     // this.getUserPages();

    }
  }

  loginWithFacebook() {
    const appId = '885586280068397';
    const redirectUri = AppConsts.appBaseUrl + '/app/admin/facebook-connect';
    const scopes = 'pages_messaging,pages_manage_metadata,pages_show_list,pages_read_engagement';
  
    const authUrl = `https://www.facebook.com/v19.0/dialog/oauth` +
      `?client_id=${appId}` +
      `&redirect_uri=${encodeURIComponent(redirectUri)}` +
      `&scope=${scopes}` +
      `&response_type=code` +
      `&state=infoseed`;
  
    // Redirect in the same tab
    window.location.href = authUrl;
  }

  exchangeCodeForToken(code: string) {
debugger
        this._facebookServiceProxy
            .getPageAccessToken( code ).subscribe(
                (result) => {
                  debugger
                  console.log('access_token  :'+result);  
                  this.userToken = result;
                  this.getUserPages();
                           
                }  );

  }

  getUserPages() {
    this._facebookServiceProxy
    .getFacebookPages(this.userToken).subscribe(
        (result) => {

debugger

if (!result || !result.data) {
  this.isfirst = true;
  this.pages = [];
} else {
  this.isfirst = false;
  this.pages = result.data;
}

         
    } );


  }

  subscribeAppToPage(pageId: string, pageAccessToken: string, isSubscribe: any) {

    const targetPage = this.pages.find(p => p.id === pageId);

    if (targetPage) {
      targetPage.isSubscribe = isSubscribe;
    }

    if(isSubscribe){

      this. isfirst=false;
    }else{

      this. isfirst=false;
    }
  
    this._facebookServiceProxy
      .subscribePage(pageId, pageAccessToken, isSubscribe)
      .subscribe((result) => {
        if (result) {



           window.location.reload();
          Swal.fire({
            icon: 'success',
            title: 'Success',
            text: isSubscribe ? 'Page successfully subscribed!' : 'Page unsubscribed.',
            timer: 2000,
            showConfirmButton: false,
            toast: true,
            position: 'top-end'
          });
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Something went wrong. Please try again.',
            timer: 2500,
            showConfirmButton: false,
            toast: true,
            position: 'top-end'
          });
        }
      });
  }

  async toggleSubscription(page: any) {
    // Step 1: Unsubscribe all other pages first
    for (const p of this.pages) {
      if (p.id !== page.id && p.isSubscribe) {
        await this.unsubscribePageAsync(p.id, p.access_token);
        p.isSubscribe = false;
      }
    }
  
    // Step 2: Toggle current page subscription
    if (!page.isSubscribe) {
      await this.subscribePageAsync(page.id, page.access_token);
      page.isSubscribe = true;
    } else {
      await this.unsubscribePageAsync(page.id, page.access_token);
      page.isSubscribe = false;

      
    }
  }
  
  subscribePageAsync(pageId: string, token: string): Promise<void> {
    return new Promise((resolve) => {
      this._facebookServiceProxy.subscribePage(pageId, token, true).subscribe(() => resolve());
    });
  }
  
  unsubscribePageAsync(pageId: string, token: string): Promise<void> {
debugger
    this.isfirst=true;
    return new Promise((resolve) => {
      this._facebookServiceProxy.subscribePage(pageId, token, false).subscribe(() => resolve());
    });
  }
}