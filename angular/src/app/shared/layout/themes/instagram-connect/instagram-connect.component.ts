import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@node_modules/@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { FacebookServiceProxy } from '@shared/service-proxies/service-proxies';

import confetti from 'canvas-confetti';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-instagram-connect',
  templateUrl: './instagram-connect.component.html',
  styleUrls: ['./instagram-connect.component.css']
})
export class InstagramConnectComponent implements OnInit {

  triggerConfetti(): void {
    var duration = 15 * 1000;
    var animationEnd = Date.now() + duration;
    var defaults = { startVelocity: 30, spread: 360, ticks: 60, zIndex: 0 };
    
    function randomInRange(min, max) {
      return Math.random() * (max - min) + min;
    }
    
    var interval = setInterval(function() {
      var timeLeft = animationEnd - Date.now();
    
      if (timeLeft <= 0) {
        return clearInterval(interval);
      }
    
      var particleCount = 50 * (timeLeft / duration);
      // since particles fall down, start a bit higher than random
      confetti({ ...defaults, particleCount, origin: { x: randomInRange(0.1, 0.3), y: Math.random() - 0.2 } });
      confetti({ ...defaults, particleCount, origin: { x: randomInRange(0.7, 0.9), y: Math.random() - 0.2 } });
    }, 250);
  }

  username: string = '';
  profileUrl: string = '';

  isConnected = false;
  showSuccessMessage = false; // 👈 new flag

  isConcet = true; // 👈 new flag
  instagramAccountName: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private _facebookServiceProxy: FacebookServiceProxy
  ) {}

  ngOnInit(): void {
    const code = this.route.snapshot.queryParamMap.get('code');
    debugger
    if (code) {
      debugger;
      this._facebookServiceProxy.checkInstagram().subscribe((result) => {
     
        if(result==''){ 
            this._facebookServiceProxy.getInstagramToken(code, '', '').subscribe((result) => {
              const parts = result.split(',');
              this.username = parts[1]; // 'infoseed_mena'
              this.profileUrl = parts[2]; // the image URL
              // Simulate success
              this.isConnected = true;
              this.showSuccessMessage = true; // 👈 show congrats
               this.triggerConfetti();
      
      
            });
        
        }else{ 
          this.showSuccessMessage=false;
          this.isConnected = false; 
        }
      });

    }else{

      this._facebookServiceProxy.checkInstagram().subscribe((result) => {
     
        if(result==''){ 
            
          this.showSuccessMessage=false;
          this.isConnected = false; 

        }else{ 

          const parts = result.split(',');
          this.username = parts[1]; // 'infoseed_mena'
          this.profileUrl = parts[2]; // the image URL

          this.showSuccessMessage=false;
          this.isConnected = true; 
        }
      });


    }



    


    
  }

  connectInstagram(): void {

    debugger;
    const appId = '907410431131999';
    const redirectUri = AppConsts.appBaseUrl + '/app/admin/instagram-connect';
    const url = `https://www.instagram.com/oauth/authorize?enable_fb_login=0&force_authentication=1&client_id=${appId}&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=code&scope=instagram_business_basic%2Cinstagram_business_manage_messages%2Cinstagram_business_manage_comments%2Cinstagram_business_content_publish%2Cinstagram_business_manage_insights`;
    window.location.href = url;
  }


  continueAfterSuccess(): void {
    this.showSuccessMessage = false;
  }

  confirmRemoveAccount(): void {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Removing this account will delete all Instagram subscribers from Infoseed.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, remove it',
      cancelButtonText: 'No, keep it',
      confirmButtonColor: '#dc3545',
      cancelButtonColor: '#6c757d'
    }).then((result) => {
      if (result.isConfirmed) {
        // 👇 Put your actual remove logic here
        this.removeInstagramAccount();
      }
    });
  }
  removeInstagramAccount(): void {

    this._facebookServiceProxy.deleteInstagram().subscribe((result) => {
      this.isConnected=false;
      console.log('Instagram account removed'); // Replace with your real logic

    });
  }
}