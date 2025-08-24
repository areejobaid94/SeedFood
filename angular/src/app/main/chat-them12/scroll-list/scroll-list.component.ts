import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ContactsServiceProxy, TeamInboxServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-scroll-list',
  templateUrl: './scroll-list.component.html',
  styleUrls: ['./scroll-list.component.css']
})
export class ScrollListComponent implements OnInit {

  @Output() childEvent: EventEmitter<any> = new EventEmitter<any>();

  dataFrom: string;

  items:any[]=[];
   isLoading=false;
   currentPage=0;
   itemsPerPage=10;

   toggleLoading = ()=>this.isLoading=!this.isLoading;

   loadDataifEmpty(){
    if(this.dataFrom.length === 0){
      this.loadData();
    }
   }

   onUp(){
   }

   triggerParentFunction(user): void {
    // Perform some logic in the child component
    const selectedContact =user;
    
    // Emit an event to notify the parent component
    this.childEvent.emit(selectedContact);
  }

   loadData= ()=>{
    this.currentPage=0;
     this.toggleLoading();
     this.teamInboxServiceProxy.customersGetAll(this.dataFrom,1,0, this.currentPage,this.itemsPerPage,0,""//this.appSession.user.id
      
     ).subscribe({

      next:result=>this.items = result,
      error:err=>console.log(err),
      complete:()=>this.toggleLoading()
     })
   }
   
   appendData= ()=>{
    this.toggleLoading();
    this.teamInboxServiceProxy.customersGetAll(this.dataFrom,1,0, this.currentPage,this.itemsPerPage,0,""//this.appSession.user.id

    ).subscribe({
     next:response=>this.items = [...this.items,...response],
     error:err=>console.log(err),
     complete:()=>this.toggleLoading()
    })
  }

   onScroll= ()=>{
    this.currentPage++;
    this.appendData();
   }

   
   constructor( private teamInboxServiceProxy: TeamInboxServiceProxy) {
   }

  ngOnInit(): void {
    this.loadData();
  }

}
