import { fakeBackendProvider } from './../../../auth/helpers/fake-backend';
import { TemplateService } from './Template.service';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router, Routes } from '@node_modules/@angular/router';
import { EditTemplateFacebookComponent } from '../Edit-Template-Facebook/Edit-Template-Facebook.component';
import { FacebookTemplateServiceProxy, MessageTemplateModel } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-Facebook-Template',
  templateUrl: './Facebook-Template.component.html',
  styleUrls: ['./Facebook-Template.component.css']
})
export class FacebookTemplateComponent implements OnInit {
  // selectedCategory: string ;
  // selectedMessageType: string ;
  showChildComponent = false;

  selectedCategory: string = 'MARKETING';
  selectedMessageType: string = 'CUSTOM'
  templateId:string;
  template:MessageTemplateModel;


  circleStates = [false, false, false];



  constructor(private templateService:TemplateService,private route: ActivatedRoute,private router: Router){

  }
  nextCircle() {
    for (let i = 0; i < this.circleStates.length; i++) {
      if (!this.circleStates[i]) {
        this.circleStates[i] = true; 
        break; 
      }
    }
  }

  resetCircles() {
    for (let i = this.circleStates.length - 1; i >= 0; i--) {
      if (this.circleStates[i]) {
        this.circleStates[i] = false; 
        break; 
      }
    }
  }


  previewImageSrc: string = 'assets/common/images/imageFacebookTemplate/Marketing';
  imagePaths = {
    MARKETING: 'assets/common/images/imageFacebookTemplate/Marketing.png',
    UTILITY: 'assets/common/images/imageFacebookTemplate/Utility.png',
    AUTHENTICATION: 'assets/common/images/imageFacebookTemplate/Authentication.png',
    catalog: 'assets/common/images/imageFacebookTemplate/Catalog.gif', 
  };
  edit(model :any): void {
        
  }
  ngOnInit() {
    debugger;
    this.template = new MessageTemplateModel();
      this.template.category = JSON.parse(localStorage.getItem('category') || 'null');
      this.template.sub_category  = JSON.parse(localStorage.getItem('sub_category') || 'null'); 
    
    if(this.template.category==null){
      this.template.category="MARKETING";
      this.template.sub_category = this.getDefaultMessageType();
    }
    this.template;
    this.updateImage();
    localStorage.removeItem('category');
    localStorage.removeItem('sub_category');
  }

  selectCategory(category: string) {
    this.template.category = category;
    this.template.sub_category = this.getDefaultMessageType(); 
    this.updateImage();
  }

  updateImage() {
    this.previewImageSrc = this.imagePaths[this.template.category];
  }

  selectCatolog()
  {
    this.previewImageSrc ="assets/common/images/imageFacebookTemplate/Marketing.png";
  }

  getDefaultMessageType(): string {
    switch (this.template.category ) {
      case 'MARKETING':
        return 'CUSTOM';
      case 'UTILITY':
        return 'CUSTOM';
      case 'AUTHENTICATION':
        return 'OneTimePasscode';
      default:
        return '';
    }
  }

  next() {
    this.templateService.setTemplate(this.template); 
        this.router.navigate(['/app/main/EditTemplateFacebook'], {
      queryParams: {
        selectedCategory: this.template.category,
        selectedMessageType:this.template.sub_category,
      },
    });
  }

  Discard(){
    this.router.navigate(['/app/main/messageTemplate']);
  }

  onDataChanged(data: { category: string; messageType: string }) {
    this.selectedCategory = data.category;
    this.selectedMessageType = data.messageType;
  }
  
}
