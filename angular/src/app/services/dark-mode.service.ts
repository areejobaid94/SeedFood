import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DarkModeService {
  currentSkin: string = 'default';
  isDark = false;
  smallMenu = false;
  toglleMenu= 'open';
  hideSideNav = false;
  navBarColor = 'navbar-light';
  navBarStyle: string = 'floating-nav';
  remainingFreeConversation = null;


  constructor() { }


  toggleDarkSkin() {
    
    this.currentSkin = localStorage.getItem('mode');
    if (this.currentSkin == 'default') {
      this.currentSkin = 'dark';
    } else {
      this.currentSkin = 'default';
    }
    localStorage.setItem('mode', this.currentSkin)
  }


  checkMode() {
    localStorage.getItem('mode');
    this.currentSkin = localStorage.getItem('mode');
  }


  navMenuToggled() {

    if (this.smallMenu) {
      this.smallMenu = false;

    } else {
      this.smallMenu = true;
    }
    if (this.hideSideNav) {
      this.hideSideNav = false;

    } else {
      this.hideSideNav = true;
     
    }
  }

  toggleNavMenu() {

    if (this.toglleMenu == undefined) {
      this.toglleMenu = 'open';
      return;
    }
    if (this.toglleMenu === 'open') {
      this.toglleMenu = 'close';
    }
    else {
      this.toglleMenu = 'open';
    }
  }

  // Nav bar Customization
  navbarColor(color: string){
    this.navBarColor= color;
    localStorage.setItem('navColor', this.navBarColor)

  }
  
  checkNavColor() {
    
    localStorage.getItem('navColor');
    this.navBarColor = localStorage.getItem('navColor');
   
  }

  saveRemainingFreeConversation(remainingFreeConversation){
    if(remainingFreeConversation != null){
      this.remainingFreeConversation= remainingFreeConversation;
    }
  }

  getNavColor(key: string) {
    return localStorage.getItem(key);
  }


  //navBarStyle
  navbarStyle(style: string){
    
    this.navBarStyle= style;
    localStorage.setItem('navStyle', this.navBarStyle)

  }
  
  checkNavStyle() {
        localStorage.getItem('navStyle');
    this.navBarStyle = localStorage.getItem('navStyle');
    if(this.navBarStyle === null){
      this.navBarStyle='floating-nav';
    }  }

  getNavstyle(key: string) {
    return localStorage.getItem(key);
  }
}

