// import { Directive } from '@angular/core';
// import {
//     ChangeDetectorRef,
//     Component,
//     ElementRef,
//     EventEmitter,
//     Injector,
//     Input,
//     IterableDiffers,
//     OnDestroy,
//     OnInit,
//     ViewChild
// } from '@angular/core';
// import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
// import { DomSanitizer } from '@angular/platform-browser';
// import { ActivatedRoute } from '@angular/router';
// import { SocketioService } from '@app/shared/socketio/socketioservice';
// import { appModuleAnimation } from '@shared/animations/routerTransition';
// import { AppComponentBase } from '@shared/common/app-component-base';
// import {
//     TeamInboxServiceProxy,
//     TemplateMessagesServiceProxy
// } from '@shared/service-proxies/service-proxies';
// import { PermissionCheckerService } from 'abp-ng2-module';
// import * as FileSaver from 'file-saver';
// import * as moment from 'moment';
// import {  Subscription } from 'rxjs';

// import { AssignToModalComponent } from './assign-to-modal/assign-to-modal.component';
// import { Channel } from './channel';
// import { ChannelMessage } from './channelMessage';
// import { CustomerListFilter } from './customer-list-filter.model';
// import { UpdateCustomerModel } from './customer-update.model';
// import { RecordRTCService } from './record-rtc.service';
// import { TeamInboxService } from './teaminbox.service';

// @Component({
//     // selector: '[appAutofocus]',
//     templateUrl: './teaminbox.component.html',
//     styleUrls: ['./teaminbox.component.less'],
//     animations: [appModuleAnimation()]
// })

// export class TeamInboxComponent extends AppComponentBase
//     implements OnInit, OnDestroy {

    

//     @ViewChild('assignToModal', {static: true}) assignToModal: AssignToModalComponent;
//     @ViewChild('scrollChat', {static: false}) private scrollChat: ElementRef;
//     @ViewChild('scrollChatt', {static: false}) private scrollChatt: ElementRef;

//     public timer: EventEmitter<any> = new EventEmitter<any>();
//     // customersFilter: SearchCustomerModel;
//     customersFilter: CustomerListFilter = {
//         pageNumber: 0,
//         pageSize: 1000,
//         searchTerm: null,
//     };
//     @Input() datasource: Array<any> = [];

//     differ: any;

//     postMessageObj;
//     UsersChannels: Channel[];
//     user: Channel;
//     UserMessage: ChannelMessage[] = [];
//     selectedUser: Channel = new Channel();
//     templates: any[] = [];
//     userUpdateModel: UpdateCustomerModel = {};
//     pageSize = 10;
//     pageNumber = 0;

//     pageSizeC =20;
//     pageNumberC = 0;
//     topnumber=0;

//     PeforH=0;
//     PeforT=0;

//     isBlock: boolean;
//     chatForm: UntypedFormGroup;
//     hideEmoji = true;
//     hideUploadFile = true;
//     loading = false;
//     disabledAnotherAgent: boolean;

//     SendButton = false;
//     filetype = '.pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .rtf, .html, .zip, .mp3, .mp4, .wma, .mpg, .flv, .avi, .jpg, .jpeg, .png, .gif, .avi,.wav';
//     maxFileSize = 10000000;
//     contents: any = null;
//     filename: string;

//     remainingTime: any;
//     showPageLoader = false;

//     anotherAgentMessgeSub: Subscription;
//     customerMessageSub: Subscription;

//     isSupport:boolean;

//     isOneTime:boolean;

//     emitted = true;
//     agentsList: any[] = [];
//     assigneeAgentName: string;
//     files: FormData[] = [];
//     showInputBox = false;
//     uplodeFileOnclick = false;
//     oneClick: boolean;
//     time: boolean;
//     date: boolean;
//     yesterday: boolean;
//     titleMessage: string;
//     uploadButton: boolean = false;
//     newMessage: string = '';
//     firstRender = true;
//     scroll: boolean = false;
//     timeout: any;
//     isMore = true;
//     isMore2 = true;
//     showMessageLoader = false;
//     change:any;
//     prvHeight:number;
//     ifFirstTime=true;
//     isHasPermissionTemplate:boolean;
//     one=true;
//     heiget=0;
//     currentPosition:any;
//     isSelectedUser:boolean;
//     filters: {
//         selectedCustomerID: string,
//         selectedLiveChatID: number,
//     } = <any>{};


//     constructor(
//        // private host: ElementRef,
//         differs: IterableDiffers,
//         injector: Injector,
//         private teamService: TeamInboxService,
//         public sanitizer: DomSanitizer,
//         private templateMessagesServiceProxy: TemplateMessagesServiceProxy,
//         private teamInboxServiceProxy:  TeamInboxServiceProxy,
//         //private teamInboxSignalR: TeamInboxSignalRService,
//         private socketioService: SocketioService,
//         public _recordRTC: RecordRTCService,
//         private _activatedRoute: ActivatedRoute,
//         private cd: ChangeDetectorRef,
//         private _permissionCheckerService: PermissionCheckerService,
//     ) {
//         super(injector);
//         this.differ = differs.find([]).create(null);
//     }
//     setFiltersFromRoute(): void {

//         if (this._activatedRoute.snapshot.queryParams['CustomerID'] != null) {
//             this.filters.selectedCustomerID = this._activatedRoute.snapshot.queryParams['CustomerID'];
//         }

//         if (this._activatedRoute.snapshot.queryParams['LiveChatId'] != null) {
//             this.filters.selectedLiveChatID = this._activatedRoute.snapshot.queryParams['LiveChatId'];
//         }
//     }

//     ngOnInit(): void {



//        // this.socketioService.setupSocketConnection();

//         this.remainingTime = {leftTime: 0};
//         window.blur();

//         document.getElementById("bodyID").style.overflow = "hidden";

//         this.ifFirstTime=true;
//         this.isHasPermissionTemplate=this._permissionCheckerService.isGranted("Pages.Administration.TemplateMessages");
//         this.setFiltersFromRoute();

//         this.subscribeAnotherAgentMessages();
//         this.subscribeCustomertMessages();

//         this.initForm();
//         this.getChannels();
//         if(this.isHasPermissionTemplate){

//             this.getAllTemplates();
//         }
        
//         this.getUsers();


//     }





//     ngOnDestroy(): void {

//     }

//     subscribeAnotherAgentMessages = () => {


//         this.anotherAgentMessgeSub = this.socketioService.agentMessage.subscribe((data: Channel) => {


//             const index = this.UsersChannels.findIndex(e => e.userId === data.userId)
//             if (this.selectedUser?.userId === data.userId) {

//                 this.UserMessage.push(data.customerChat);

//                 this.UsersChannels[index].lastConversationStartDateTime=data.lastConversationStartDateTime;
//                 this.UsersChannels[index].lastMessageData = data.customerChat.createDate;
//                 this.UsersChannels[index].lastMessageText = data.customerChat.text;
//                 this.UsersChannels[index].unreadMessagesCount = data.unreadMessagesCount;

//                 this.UsersChannels[index].isSupport=data.isSupport;
//                 this.UsersChannels[index].isComplaint = data.isComplaint;
//                 this.UsersChannels[index].isOpen = data.isOpen;
//                 this.UsersChannels[index].lockedByAgentName = data.lockedByAgentName;
//                 this.UsersChannels[index].isLockedByAgent = data.isLockedByAgent;
//                 this.UsersChannels[index].isBlockCustomer = data.isBlockCustomer;
//                 this.UsersChannels[index].isConversationExpired = data.isConversationExpired;
//                 this.UsersChannels[index].agentId = data.agentId;

//                 this.UsersChannels[index].isBlock = data.isBlock;
//                 this.UsersChannels[index].isBotCloseChat=data.isBotCloseChat;

//                 this.UsersChannels[index].isBotChat=data.isBotChat;

//                 if (data.customerChat) {
//                     this.getTime(this.UsersChannels);
//                 }


//                 //this.selectedUser.description=data.description;

//                 if (data.customerChat) {

//                    // this.UserMessage.push(data.customerChat);
//                     setTimeout(() => {
//                         this.scrollChat.nativeElement.scroll({
//                             top: this.scrollChat.nativeElement.scrollHeight,
//                             left: 0,
//                             behavior: 'smooth'
//                         });
//                     }, 100);
//                 }



//             if (data.isLockedByAgent && this.appSession.user.id === data.agentId) {
                                 
//                 this.disabledAnotherAgent = true;
//             } else {
                                 
//                 this.disabledAnotherAgent = false;
//             }


//             }






//         });
//     };

//     subscribeCustomertMessages = () => {

//         this.showPageLoader = true;
//         this.customerMessageSub = this.socketioService.customerMessage.subscribe((data: Channel) => {
                             
//              if(this.appSession.tenantId==data.tenantId){
                                 

//                 const isfoiu=this.UsersChannels.some(item => item.phoneNumber === data.phoneNumber)
                                 
//                 if( isfoiu){

//                     const index = this.UsersChannels.findIndex(e => e.userId === data.userId)

//                     if (this.selectedUser?.userId === data.userId) {

                                         
//                         // this.UsersChannels.splice(index, 1);
//                         // this.selectedUser=this.UsersChannels[0]


//                         // this.UsersChannels.push(data);

//                             this.UserMessage.push(data.customerChat);
//                             this.goToLastMassage();
//                             //this.countDown(data);

                                             
//                            // this.UsersChannels[index].customerChat=data.customerChat;
//                            this.UsersChannels[index].lastConversationStartDateTime=data.lastConversationStartDateTime;
//                             this.UsersChannels[index].createDate = data.customerChat.createDate;
//                             this.UsersChannels[index].lastMessageText = data.customerChat.text;
//                             this.UsersChannels[index].unreadMessagesCount = data.unreadMessagesCount;

//                             this.UsersChannels[index].isSupport=data.isSupport;
//                             this.UsersChannels[index].isComplaint = data.isComplaint;
//                             this.UsersChannels[index].isOpen = data.isOpen;
//                             this.UsersChannels[index].lockedByAgentName = data.lockedByAgentName;
//                             this.UsersChannels[index].isLockedByAgent = data.isLockedByAgent;
//                             this.UsersChannels[index].isBlockCustomer = data.isBlockCustomer;
//                             this.UsersChannels[index].isConversationExpired = data.isConversationExpired;
//                             this.UsersChannels[index].agentId = data.agentId;
//                             this.UsersChannels[index].isBlock = data.isBlock;
//                             this.UsersChannels[index].isBotCloseChat=data.isBotCloseChat;
//                             this.UsersChannels[index].isBotChat=data.isBotChat;

//                            // this.UsersChannels[index].customerChat=data.customerChat;


//                            this.UsersChannels[index].lastMessageData=data.lastMessageData;
//                             if (data.customerChat) {
//                                 this.getTime(this.UsersChannels);
//                             }


//                     }else{
                                           
//                           this.UsersChannels.splice(index, 1);
//                           this.UsersChannels.push(data);
//                        // this.UsersChannels.splice(0,index,this.UsersChannels[index]);
//                     }





//                 }else{
                                     
//                     if(this.customersFilter.searchTerm =="" ||this.customersFilter.searchTerm ==null ){
//                         this.UsersChannels.push(data);
//                         this.getTime(this.UsersChannels);
//                         this.countDown(null);

//                     }


//                 }
//                 this.UsersChannels = this.UsersChannels.filter((el, i, a) => i === a.indexOf(el))
//                 this.getTime(this.UsersChannels);

//                 this.sortedArrayOfCustomerMessages();

//              }

//         });

//     };

//     sortedArrayOfCustomerMessages() {
//         this.UsersChannels.sort(function (a, b) {
//             // Turn your strings into dates, and then subtract them
//             // to get a value that is either negative, positive, or zero.
//             return Number(new Date(b.lastMessageData)) - Number(new Date(a.lastMessageData));
//         });
//     }

//     initForm() {
//         this.chatForm = new UntypedFormGroup({
//             text: new UntypedFormControl('', [Validators.required]),
//             formFile: new UntypedFormControl('', [Validators.required]),
//         });
//     }


//     getAllTemplates() {

//         this.templateMessagesServiceProxy
//             .getAllNoFilter()
//             .subscribe((result) => {
//                 this.templates = result.items;
//             });
//     }

//     getTemplateMessage(templateMessageId) {


//         this.templateMessagesServiceProxy
//             .getTemplateMessageForEdit(templateMessageId)
//             .subscribe((result) => {


//                 this.chatForm
//                     .get('text')
//                     .setValue(result.templateMessage.messageText);
//             });
//     }

//     getChannels() {

//         this.isOneTime=true;

//         this.isMore=true;
//         this.isMore2=true;
//         this.pageNumber=0;

//         if (!this.UsersChannels) {
//             this.showPageLoader = true;
//         }
//          this.customersFilter.pageNumber =this.pageNumberC;
//          this.customersFilter.pageSize = this.pageSizeC
                          

//         //  if(this.customersFilter.searchTerm==""){

//         //     this.filters.selectedCustomerID="";
//         //  }

//         if( this.filters.selectedCustomerID ==null ||  this.filters.selectedCustomerID ==""){

//         }else{
//             this.customersFilter.searchTerm = this.filters.selectedCustomerID;
//         }

//           this.teamService.getCustomer(this.customersFilter).subscribe(
//             (result: any) => {



//                 this.UsersChannels = result.result;
//                 this.sortedArrayOfCustomerMessages();




//                 if (this.UsersChannels.length > 0) {


//                        //select first user
//                         this.UsersChannels[0].isSelected = true;
//                         this.selectedUser = this.UsersChannels[0];



//                     if (this.selectedUser.isLockedByAgent && this.appSession.user.id === this.selectedUser.agentId) {
                                         
//                         this.disabledAnotherAgent = true;
//                     } else {
                                         
//                         this.disabledAnotherAgent = false;
//                     }

//                     this.isMore=true;
//                     this.isMore2=true;
//                     this.showPageLoader = false;
//                     this.setUpdateModelFromOrignalData();
//                     this.loadMessages();

//                     this.getTime(this.UsersChannels);
//                     this.countDown(null);

//                 } else {
//                     this.showPageLoader = false;
//                 }


//                 this.filters.selectedCustomerID=null;
//             },
//             (error) => {
//                 this.showPageLoader = false;
//             }
//         );
//     }

//     getCustomers() {
//         this.customersFilter.pageNumber = 0;
//         this.teamService.getCustomer(this.customersFilter).subscribe(
//             (result: any) => {

//                 this.UsersChannels = result.result;
//                 if (this.UsersChannels.length > 0) {



//                     // if(this.filters.selectedCustomerID!=null){

//                     //     for (let user of this.UsersChannels) {

//                     //         if (user.userId === this.filters.selectedCustomerID) {

//                     //             user[0].isSelected = true;
//                     //             this.selectedUser = user;
//                     //             this.showPageLoader = false;
//                     //             this.pageNumber = 0;
//                     //             this.setUpdateModelFromOrignalData();
//                     //             this.loadMessages();
//                     //             this.getTime(this.UsersChannels);

//                     //             return ;
//                     //         }
//                     //     }
//                     // }else{

//                         this.UsersChannels[0].isSelected = true;
//                         this.selectedUser = this.UsersChannels[0];
//                         this.showPageLoader = false;
//                         this.pageNumber = 0;
//                         this.setUpdateModelFromOrignalData();
//                         this.loadMessages();
//                         this.getTime(this.UsersChannels);

//                   //  }





//                 }
//             },
//             (error) => {
//                 this.showPageLoader = false;
//             }
//         );
//     }

//     getSanitizedURL(data) {
//         const blob = new Blob([data.mediaUrl], {type: data.type});
//         let url = window.URL.createObjectURL(blob);
//         return this.sanitizer.bypassSecurityTrustUrl(url);
//     }




//     loadMessages() {
//         this.showPageLoader = true;

//         this.teamService
//             .getChannelMessage(
//                 this.selectedUser.userId,
//                 this.pageSize,
//                 this.pageNumber
//             )
//             .subscribe(
//                 (res: any) => {
//                     this.showPageLoader = false;
//                     this.firstRender = true;
//                     this.UserMessage = res.result;

                                     
//                     setTimeout(() => {
                                         
//                         this.scrollChat.nativeElement.scroll({
//                             top: this.scrollChat.nativeElement.scrollHeight,
//                             left: 0,
//                             behavior: 'smooth'
//                         });
//                     }, 500);

//                     if(this.ifFirstTime){
//                         this.ifFirstTime=false;
                  
//                        // this.teamInboxSignalR.startConnection();
//                        // this.socketioService.addBroadcastAgentMessagesListener();
//                        // this.socketioService.addBroadcastEndUserMessagesListener();
//                     }

//                 },
//                 (error) => {
//                     this.showPageLoader = false;
//                 }
//             );
//     }

//     onSelect(event) {
//         if (this.uploadButton) {
//             this.files = event.target.files;
//         } else {
//             this.files = event;
//         }
//         this.uploadFile();
//         // this.getImgData(this.files[0]);
//     }

//     onRemove(index) {
//         let data = [];
//         for (let i = 0; i < this.files.length; i++) {
//             if (i !== index) {
//                 data.push(this.files[i]);
//             }
//         }
//         this.files = data;
//         // this.files.splice(i, 1);
//         this.uploadFile();
//     }


//     uploadFile() {
//         this.uploadButton = false;

//         // this.chatForm.reset();
//         let files = this.files;
//         for (let file of files) {
//             this.chatForm.patchValue({
//                 formFile: files,

//             });
//             this.hideUploadFile = true;
//             const fillTextarea = document.getElementById('ChatMessage') as HTMLInputElement;
//             // fillTextarea.value += '\n' + file.name;
//         }
//         // form.clear();
//         // this.uplodeFileOnclick = false;
//         this.chatForm.get('formFile').updateValueAndValidity();
//     }

//     loadMoreChat() {
//                          ;
//         this.showMessageLoader = true;
//         this.pageNumber = this.pageNumber + 1;
//         this.teamService
//             .getChannelMessage(

//                 this.selectedUser.userId,
//                 this.pageSize,
//                 this.pageNumber
//             )
//             .subscribe(
//                 (res: any) => {
//                                      ;
//                     this.showMessageLoader = false;


//                     this.PeforH=this.scrollChat.nativeElement.scrollHeight;
//                     this.PeforT=this.scrollChat.nativeElement.scrollTop;

//                     this.UserMessage = res.result.concat(this.UserMessage);

//                     var h=this.scrollChat.nativeElement.scrollHeight;
//                     var t=this.scrollChat.nativeElement.scrollTop ;

                                     
//                     this.scrollChat.nativeElement.scrollTop=1;

//                     this.isMore = res.result.length > 0;
//                 },
//                 (error) => {
//                     this.showMessageLoader = false;
//                 }
//             );
//     }
//     loadMoreContact() {

//         //this.showMessageLoader = true;
//         this.customersFilter.pageNumber = this.pageNumberC ;
//         this.customersFilter.pageSize = this.pageSizeC;
//         this.customersFilter.searchTerm = "";
//         this.teamService.getCustomer(this.customersFilter).subscribe(
//             (result: any) => {

//                                  ;

//                 result.result.forEach(element => {

//                     const isfoiu=this.UsersChannels.some(item => item.phoneNumber === element.phoneNumber)

//                     if(!isfoiu){
//                         this.UsersChannels.push(element);

//                     }

//                 });
//                                  ;
//                 this.sortedArrayOfCustomerMessages();
//                 if (this.UsersChannels.length > 0) {

//                     // if(this.filters.selectedCustomerID!=null){

//                     //     for (let user of this.UsersChannels) {

//                     //         if (user.userId === this.filters.selectedCustomerID) {

//                     //             user.isSelected = true;
//                     //            this.selectedUser = user;
//                     //         }
//                     //     }
//                     // }else{

//                     //     // this.UsersChannels[0].isSelected = true;
//                     //     // this.selectedUser = this.UsersChannels[0];
//                     // }




//                     if (this.selectedUser.isLockedByAgent && this.appSession.user.id === this.selectedUser.agentId) {
                                         
//                         this.disabledAnotherAgent = true;
//                     } else {
                                         
//                         this.disabledAnotherAgent = false;
//                     }

//                     // this.isMore=true;
//                     // this.isMore2=true;
//                     // this.showPageLoader = false;
//                     // this.setUpdateModelFromOrignalData();
//                     // this.loadMessages();

//                     this.getTime(this.UsersChannels);
//                     this.countDown(null);


//                 } else {
//                     this.showPageLoader = false;
//                 }
//                // this.showMessageLoader = false;
//                // this.UsersChannels = result.result.concat(this.UsersChannels);

//                this.isMore2 = result.result.length > 0;
//                this.scrollChatt.nativeElement.scrollTop=this.scrollChatt.nativeElement.scrollHeight;//this.prvHeight;
//                this.isOneTime=true;
//             },
//             (error) => {
//                // this.showPageLoader = false;
//             }
//         );
//     }

//     ScrollChat(){

                        
//         setTimeout(() => {
                             
//             this.scrollChat.nativeElement.scroll({
//                 top: this.scrollChat.nativeElement.scrollHeight,
//                 left: 0,
//                 behavior: 'smooth'
//             });
//         }, 100);

//     }

//     download(data) {
//         const blob = new Blob([data.mediaUrl], {type: data.type});
//         let url = URL.createObjectURL(blob);
//         return this.sanitizer.bypassSecurityTrustUrl(url);
//     }

//     async submitForm() {


//         let formData: any = new FormData();
//         this.showInputBox = false;
//         this.uplodeFileOnclick = false;
//         // this.chatForm.reset();
//         this.oneClick = true;

//         if (this._recordRTC.recordingFile) {
//             this.chatForm.controls['formFile'].setValue(this._recordRTC.recordingFile);
//             this.chatForm.get('formFile').updateValueAndValidity();
//         }

//         if (!this.chatForm.get('formFile').value) {
//             const userId = this.selectedUser.userId;
//             let text = this.newMessage;
//             this.newMessage = '';
//             // this.chatForm.reset();
//             this.showPageLoader = false;

//             if (text) {
//                 this.postMessageObj = {};
//                 this.postMessageObj.userId = userId;
//                 this.postMessageObj.text = text;
//                 this.postMessageObj.agentName = this.appSession.user.userName;
//                 this.postMessageObj.agentId = this.appSession.userId.toString();
//                 this.postMessageObj.type="text";

//                 this.postMessageObj.to=this.selectedUser.phoneNumber;




//                 this.chatForm.reset();
//                 this.teamService.postMessageD360(this.postMessageObj).subscribe(
//                     (response) => {

//                         this.showPageLoader = false;
//                         this.chatForm.reset();
//                         this.files = [];
//                         this.showInputBox = false;
//                         this.selectedUser.lastMessageText = text;
//                         this.oneClick = false;
//                         this.newMessage = '';
//                         text = '';
//                         setTimeout(() => {
//                             this.scrollChat.nativeElement.scroll({
//                                 top: this.scrollChat.nativeElement.scrollHeight,
//                                 left: 0,
//                                 behavior: 'smooth'
//                             });
//                         }, 100);
//                     },
//                     (error) => {
//                         this.showPageLoader = false;
//                     }
//                 );
//             }

//         } else {

//             let FormFile = this.chatForm.get('formFile').value;
//             this.files = [];
//             if (this.uploadButton || this._recordRTC?.blobUrl) {
//                 FormFile = [FormFile];
//             }

//             for (let i = 0; i < FormFile.length; i++) {
                                 
//                 let formDataFile = new FormData();
//                 formDataFile.append('to', this.selectedUser.phoneNumber);
//                 formDataFile.append('agentName', this.appSession.user.userName);
//                 // @ts-ignore
//                 formDataFile.append('agentId', this.appSession.userId);
//                 formDataFile.append('Text', this.chatForm.get('text').value || '');
//                 formDataFile.append('altText', this.chatForm.get('formFile').value.name || '');
//                 // @ts-ignore
//                 formDataFile.append('formFile', FormFile[i]);
//                 this.teamService.postD360Attachment(formDataFile).subscribe(
//                     (response) => {
                                         
//                         this.uploadButton = false;
//                         this.pageNumber = 0;
//                         this.chatForm.reset();
//                         this._recordRTC.clearRecordedData();
//                         this._recordRTC.recordingFile = '';
//                         this.files = [];
//                         this.cd.detectChanges();
//                         this.newMessage = '';
//                         if (document.getElementById('ChatMessage')) {
//                             // @ts-ignore
//                             document.getElementById('ChatMessage').value = '';
//                             document.getElementById('ChatMessage').style.display = 'block';

//                         }

//                         setTimeout(() => {
//                             this.scrollChat.nativeElement.scroll({
//                                 top: this.scrollChat.nativeElement.scrollHeight,
//                                 left: 0,
//                                 behavior: 'smooth'
//                             });
//                         }, 100);
//                         // this.loadMoreChat();
//                     },
//                     (error) => console.log(error)
//                 );
//             }
//             this.uploadButton = false;
//             this.pageNumber = 0;
//             this.files = [];
//             this.chatForm.reset();
//             this._recordRTC.clearRecordedData();
//             this._recordRTC.recordingFile = '';
//             this.cd.detectChanges();
//             this.loadMessages();
//         }
//     }

//     addEmoji(event) {
//         this.newMessage += event.emoji.native;
//         this.hideEmoji = true;
//     }

//     showEmoji() {
//         this.hideEmoji = !this.hideEmoji;
//     }

//     showFileUpload() {
//         document.getElementById('upload-file').click();
//         this.uploadButton = true;
//         this.hideUploadFile = !this.hideUploadFile;
//     }

//     searchForCustomerClicked() {
                         
//         this.isMore=true;
//         this.pageNumber=0;
//         if (this.customersFilter.searchTerm) {
//             this.getChannels();
//         } else {
//             this.customersFilter.searchTerm = '';
//             this.getChannels();
//         }
//     }

//     selectUserToChat(newSelected: Channel) {

                         
//         this.isMore=true;
//         this.pageNumber=0;
//         if (newSelected.id === this.selectedUser.id) {
//             this.isSelectedUser=true;
//             return;
//         }

//         this.UsersChannels.forEach((user) => {
//             if (user.userId !== newSelected.userId) {
//                 user.isSelected = false;
//                 this.isSelectedUser=false;
//                 this.UserMessage = [];
//             } else {

//                 user.isSelected = true;
//                 this.isSelectedUser=true;
//             }
//         });

//         if (newSelected.isLockedByAgent && this.appSession.user.id === newSelected.agentId) {
//             // newSelected.isOpen = false;

//             this.disabledAnotherAgent = true;
//         } else {
//             this.disabledAnotherAgent = false;
//         }


//         this.selectedUser = newSelected;


//         this.pageNumber = 0;
//         this.loadMessages();
//         this.countDown(null);
//     }

//     countDown(data) {
//         // let diff;
//         // if (data) {
//         //     diff = Math.abs(
//         //         new Date().getTime() -
//         //         new Date(data.lastConversationStartDateTime).getTime()
//         //     ) / 3600000;
//         // } else {
//         //     diff = Math.abs(
//         //         new Date().getTime() -
//         //         new Date(this.selectedUser.lastConversationStartDateTime).getTime()
//         //     ) / 3600000;
//         // }

//         // diff= diff-3600/ 3600000;
//         // this.remainingTime = {leftTime: 86400 - (diff * 3600)};
//         //this.isOneTime=true; 83414




//         const current = new Date();
//         const timestamp = current.getTime();

//         if (data) {
//             let alltime=data.expiration_timestamp-data.creation_timestamp 
//             let curTimeAndExpirTime=Math.ceil(data.expiration_timestamp- (timestamp / 1000))
//             let remainingTime=alltime-curTimeAndExpirTime;
              
//             if(curTimeAndExpirTime>0){

//                 if(alltime<=86400){

//                     this.remainingTime = {stopTime: data.expiration_timestamp, leftTime: 86400- remainingTime, format:'HH:mm:ss'}; //86400 => 24 houer
//                 }else{

//                     this.remainingTime = {stopTime: data.expiration_timestamp, leftTime: 259200 - remainingTime, format:' ( d ) :HH:mm:ss'}; //86400 => 24 houer

//                 }

                
//             }else{

//                 this.remainingTime = {leftTime: 0};
//             }
           
//         } else {
//             let alltime=(this.selectedUser.expiration_timestamp-this.selectedUser.creation_timestamp)
//             let curTimeAndExpirTime=Math.ceil(this.selectedUser.expiration_timestamp- (timestamp / 1000))
//             let remainingTime=alltime-curTimeAndExpirTime;
//             if(curTimeAndExpirTime>0){
//                 if(alltime<=86500){

//                     this.remainingTime = {stopTime: this.selectedUser.expiration_timestamp, leftTime: 86400- remainingTime, format:'HH:mm:ss'}; //86400 => 24 houer
//                 }else{

//                     this.remainingTime = {stopTime: this.selectedUser.expiration_timestamp, leftTime: 259200 - remainingTime, format:' ( d ) :HH:mm:ss'}; //86400 => 24 houer

//                 }
              
//             }else{

//                 this.remainingTime = {leftTime: 0};
//             }
          
        
//             // var dateExpiration=  new Date(this.selectedUser.expiration_timestamp*1000);         
//             // var diff =(dateExpiration.getTime() - current.getTime()) / 1000;
//             // var ho= Math.abs(Math.round(diff));
//         }

//     }

//     onSelectNewStatus(event) {
//     }

//     lockUser() {
//         if( this.filters.selectedLiveChatID ==null ||  this.filters.selectedLiveChatID ==0){
//             this.filters.selectedLiveChatID=0;
//         }
//         this.selectedUser.isSupport=false;
//         this.selectedUser.isOpen = true;
//         this.selectedUser.isLockedByAgent = true;
//         this.selectedUser.lockedByAgentName = this.appSession.user.userName;
//         this.selectedUser.unreadMessagesCount = 0;
//         this.teamService
//             .lockCustomer(
//                 this.selectedUser.userId,
//                 this.appSession.user.userName,
//                 this.appSession.user.id,
//                 this.filters.selectedLiveChatID
//             )
//             .subscribe((response) => {
//             });
//     }

//     unlockUser() {
//         this.selectedUser.isOpen = true;
//         this.showInputBox = false;
//         this.selectedUser.isLockedByAgent = false;
//         this.selectedUser.lockedByAgentName = this.appSession.user.userName;
//         this.teamService
//             .unlockCustomer(
//                 this.selectedUser.userId,
//                 this.appSession.user.userName,
//                 this.appSession.user.id
//             )
//             .subscribe((response) => {
//             });
//     }




//     updateCustomer() {
//         this.showPageLoader = true;

//         if (this.userUpdateModel.displayName && this.userUpdateModel.phoneNumber) {
//             this.teamService.updateCustomer(this.userUpdateModel).subscribe(
//                 (res) => {
//                     this.showPageLoader = false;
//                 },
//                 (error) => {
//                     this.showPageLoader = false;
//                 }
//             );
//         }
//     }

//     setUpdateModelFromOrignalData() {
//         this.userUpdateModel.userId = this.selectedUser.userId;
//         this.userUpdateModel.phoneNumber = this.selectedUser?.phoneNumber?.toString();
//         this.userUpdateModel.displayName = this.selectedUser?.displayName?.toString();
//         this.userUpdateModel.emailAddress = this.selectedUser?.emailAddress?.toString();
//         this.userUpdateModel.website = this.selectedUser?.website?.toString();
//         this.userUpdateModel.description = this.selectedUser?.description?.toString();
//     }

//     onBlur() {
//     }

//     close() {

//         if( this.filters.selectedLiveChatID ==null ||  this.filters.selectedLiveChatID ==0){
//             this.filters.selectedLiveChatID=0;
//         }

//         this.selectedUser.isOpen = false;
//         this.selectedUser.lockedByAgentName = this.appSession.user.userName;
//         this.teamService.updateCustomerStatus(this.selectedUser.userId, this.selectedUser.isOpen, this.selectedUser.lockedByAgentName,this.filters.selectedLiveChatID).subscribe(
//             (res) => {
//                 this.selectedUser.isLockedByAgent = false;
//             }
//         );
//     }

//     showAssignToModal(): void {
//         this.assignToModal.show();

//     }

//     getUsers() {
//         this.teamService.getUsers().subscribe((result: any) => {
//             this.agentsList = result.result.items;
//         });
//     }

//     assigned(event?: any) {
//         this.assigneeAgentName = event.userName + ' ' + event.surname;
//         this.teamService.assignTo(this.selectedUser.userId, this.assigneeAgentName, event.id).subscribe(res => {
//             this.assignToModal.close();
//         });
//     }

//     startVoiceRecord(message) {

//         this._recordRTC.toggleRecord(message);
//         message.style.display = 'none';

//     }

//     clearRecord(message) {
//         this._recordRTC.clearRecordedData();
//         message.style.display = 'block';
//         message.value = '';
//         this._recordRTC.recordingFile = '';
//     }

//     getLastMessage() {
//         this.teamService.getLastMessage(this.selectedUser.id).subscribe();
//     }

//     updateCustomerInfo(selectedUser, displayName,phoneNumber, email, location) {
//         const customerInfo = {
//             userId: selectedUser.userId,
//             displayName: selectedUser.displayName,
//             phoneNumber: selectedUser.phoneNumber,
//             website: selectedUser.website,
//             emailAddress: selectedUser.emailAddress,
//             description: selectedUser.description,
//             isConversationExpired: selectedUser.isConversationExpired,
//             isOpen: selectedUser.isOpen
//         };
//         this.teamService.updateCustomer(customerInfo).subscribe(() => {
//            this.notify.success(this.l('successfullyUpdated'));
//             // dropdownMenu.style.display = 'none'
//         });
//     }

//     getTime(users) {

//         for (let user of users) {
//             const diff = new Date().getDate() - new Date(user.lastMessageData).getDate();

//             if (diff === 1) {
//                 user.createDate = 'Yesterday';
//             } else if (diff === 0) {
//                 const time = new Date(user.lastMessageData).toString();
//                 user.createDate = moment.utc(time).format('hh:mm a');
//             } else {
//                 const date = new Date(user.lastMessageData).toString();
//                 user.createDate = moment.utc(date).format('DD/MM/YYYY');
//             }
//         }
//     }

//     downloadImageFromURL(url) {

//         let xhr = new XMLHttpRequest();
//         xhr.open('GET', url, true);
//         xhr.responseType = 'blob';
//         xhr.onload = function () {
//             let urlCreator = window.URL || window.webkitURL;
//             let imageUrl = urlCreator.createObjectURL(this.response);
//             let tag = document.createElement('a');
//             tag.href = imageUrl;
//             tag.download = 'down.jpg';
//             document.body.appendChild(tag);
//             tag.click();
//             document.body.removeChild(tag);
//         };
//         xhr.send();
//     }

//     async downloadFromURL(url, filename) {
//         FileSaver.saveAs(url, filename);
//     }

//     chatBoxKeyup($event: KeyboardEvent) {
//         if ($event.keyCode === 13) {
//             $event.preventDefault();
//             $event.stopPropagation();
//             this.submitForm();
//         }
//     }

//     searchTerm($event: KeyboardEvent) {

//         this.isMore=true;
//         this.pageNumber=0;
//         $event.preventDefault();
//         $event.stopPropagation();
//         this.getChannels();
//     }

//     dropEventContainsFiles(event) {
//         // if (event.dataTransfer.types) {
//         //     this.showInputBox = event.dataTransfer.types[0] ? event.dataTransfer.types[0] === 'Files' : false;
//         // } else {
//         //     this.showInputBox = false;
//         // }
//     }

//     getImgData(file) {
//         // file.forEach(element => {
//         let imgPreview = document.getElementById('imgPreview');
//         if (file && imgPreview) {
//             const fileReader = new FileReader();
//             fileReader.readAsDataURL(file);
//             fileReader.addEventListener('load', function () {
//                 imgPreview.style.display = 'block';
//                 imgPreview.innerHTML = '<img src="' + this.result + '" />';
//             });
//         }
//         // });

//     }

//     getTemplateMessageText(id) {
//         this.titleMessage = '';
//         this.templateMessagesServiceProxy
//             .getTemplateMessageForEdit(id).subscribe(res => {
//             this.titleMessage = res.templateMessage.messageText;
//         });
//     }

//     blockContact(selectedUser, flag) {
//         this.teamService.blockContact(selectedUser.userId, selectedUser.agentId, selectedUser.lockedByAgentName, flag).subscribe();
//     }

//     async stopRecord() {
//         this._recordRTC.clickStopRTC().then((res => {
//         }));
//     }

//     onScroll() {
      
//         if (this.scrollChat.nativeElement.scrollTop === 0 && this.isMore) {
                            
//             // this.pageNumber =this.pageNumber+ 1;
//             this.loadMoreChat();
//         }
//     }

//     onScroll2() {

        

//             let scroll = window.pageYOffset;
//             if (scroll > this.currentPosition) {
//             } else {
//             }
//             this.currentPosition = scroll;

//         // var toppp=this.scrollChatt.nativeElement.scrollTop;

//         var d =  document.getElementById("DivcontactList");
//         // var b =  document.getElementById("contactList");

//         // var offset = d.scrollTop + window.innerHeight;
//         var height = d.offsetHeight;

//         // var offsetb = b.scrollTop + window.innerHeight;
//         // var heightb = b.offsetHeight;
                         

//         if(this.prvHeight>height){
                             

//         }else{
//             var h=this.scrollChatt.nativeElement.scrollHeight-30;//-20;//-height;
//             var t=this.scrollChatt.nativeElement.scrollTop + height;
//             if (t>= h  && this.isOneTime) {
                                

//                this.isOneTime=false;
//                this.prvHeight=height;

//                this.topnumber=this.scrollChatt.nativeElement.scrollTop;
//                this.heiget=this.scrollChatt.nativeElement.scrollHeight;
//                  this.pageNumberC =this.pageNumberC+ 1;
//                 this.loadMoreContact();
//             }

//         }

//     }


//     goToLastMassage() {
                         
//         setTimeout(() => {
                             
//             this.scrollChat.nativeElement.scroll({
//                 top: this.scrollChat.nativeElement.scrollHeight + 20,
//                 left: 0,
//                 behavior: 'smooth'
//             });
//         });
//     }


//     updateIsComplaintSwitch(e): void {



//         if (e.target.checked) {

//             this.selectedUser.isComplaint = e.target.checked;
//             this.teamInboxServiceProxy.updateComplaint(this.selectedUser.userId,this.selectedUser.isComplaint).subscribe(
//                 (res) => {

//                 }
//             );
//         } else {

//             this.selectedUser.isComplaint = e.target.checked;
//             this.teamInboxServiceProxy.updateComplaint(this.selectedUser.userId,this.selectedUser.isComplaint).subscribe(
//                 (res) => {

//                 }
//             );
//         }


//     }
// }
