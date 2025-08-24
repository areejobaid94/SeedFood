import { DarkModeService } from "./../../services/dark-mode.service";
import * as JSZip from 'jszip'; // Import JSZip for file compression

import {
    ChangeDetectionStrategy,
    ChangeDetectorRef,
    Component,
    ElementRef,
    HostListener,
    Injector,
    IterableDiffers,
    OnDestroy,
    OnInit,
    ViewChild,
    inject,
} from "@angular/core";
import {
    FormControl,
    FormGroup,
    NgForm,
    UntypedFormControl,
    UntypedFormGroup,
    Validators,
} from "@angular/forms";
import { DomSanitizer } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { AssignToModalComponent } from "@app/main/teamInbox/assign-to-modal/assign-to-modal.component";
import { Channel } from "@app/main/teamInbox/channel";
import { ChannelMessage } from "@app/main/teamInbox/channelMessage";
import { CustomerListFilter } from "@app/main/teamInbox/customer-list-filter.model";
import { UpdateCustomerModel } from "@app/main/teamInbox/customer-update.model";
import { RecordRTCService } from "@app/main/teamInbox/record-rtc.service";
import { TeamInboxService } from "@app/main/teamInbox/teaminbox.service";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    ContactDto,
    ContactsServiceProxy,
    GroupDtoModel,
    GroupMembersDto,
    GroupServiceProxy,
    LiveChatServiceProxy,
    MembersDto,
    MessageTemplateModel,
    MoveMembersDto,
    ProfileServiceProxy,
    TeamInboxServiceProxy,
    TemplateMessagesServiceProxy,
    WhatsAppMessageTemplateServiceProxy,
    ItemsServiceProxy,
    CustomerChat

} from "@shared/service-proxies/service-proxies";
import { PermissionCheckerService } from "abp-ng2-module";
import * as FileSaver from "file-saver";
import { Subscription } from "rxjs";
import { SocketioService } from "@app/shared/socketio/socketioservice";
import { ThemeHelper } from "@app/shared/layout/themes/ThemeHelper";
import { ToastrService } from "ngx-toastr";
import { Howl } from "howler";
import * as moment from "moment";
import "moment/locale/ar-sa"; // Import the Arabic locale for Hijri dates
import "moment-hijri"; // Hijri extension for moment.js
const { toGregorian } = require("hijri-converter");
import * as rtlDetect from "rtl-detect";
import * as RecordRTC from "recordrtc";
import { TeaminboxTemplateModalComponent } from "./teaminbox-template-modal/teaminbox-template-modal.component";
import { debounceTime, switchMap } from "rxjs/operators";
import { SendMessageModalComponent } from "./send-message-modal/send-message-modal.component";
import { NewCustomerListFilter } from "../teamInbox/new-customer-list-filter.model";
import { AppSessionService } from "@shared/common/session/app-session.service";
import { NgbDropdownConfig } from "@ng-bootstrap/ng-bootstrap";
import { AssignUsersModalComponent } from "../liveChat/assign-users-modal/assign-users-modal.component";
import { AssignTicketModalComponent } from "./assign-ticket-modal/assign-ticket-modal.component";
import Swal from "sweetalert2";
import { HttpClient } from "@node_modules/@angular/common/http";
import { UserServiceService } from "@app/shared/layout/notifications/UserService.service";
import { NotificationsService } from "@app/services/notifications.service";
import { LocalStorageService } from "@shared/utils/local-storage.service";

declare var FFmpeg: any;

@Component({
    selector: "app-chat-them12",
    templateUrl: "./chat-them12.component.html",
    styleUrls: ["./chat-them12.component.scss"],
    providers: [NgbDropdownConfig],
})
export class ChatThem12Component
    extends AppComponentBase
    implements OnInit, OnDestroy
{

    profileOpend = false;
    theme: string;
    isUserSelectedChat = false;
    isOnline = false;
    showChatt = false;
    chatType: number = 1;
    selectedMessageId: string = null;
    groupID: number = null;
    groupName: string = null;
    public selectedIndex = null;
    tempButtonsList: string[];
    tempMessageText: string;
    colors = [];
    profilePicture =
        AppConsts.appBaseUrl +
        "/assets/common/images/default-profile-picture.png";
    @ViewChild("assignToModal2", { static: true })
    private assignToModalLocal: AssignToModalComponent;

    @ViewChild("assignusersmodal", { static: true })
    assignUsers: AssignUsersModalComponent;

    @ViewChild("scrollChat", { static: false }) private scrollChat: ElementRef;
    @ViewChild("scrollChatt", { static: false })
    private scrollChatt: ElementRef;
    @ViewChild("emojiMart", { static: false }) emojiMart: ElementRef;

    @ViewChild("appteaminboxtemplatemodal", { static: false })
    modal: TeaminboxTemplateModalComponent;

    @ViewChild("sendMessageTeamInbox3", { static: false })
    sendMessageModal: SendMessageModalComponent;
    @ViewChild(ChatThem12Component) them12Component!: ChatThem12Component;


    @ViewChild(AssignTicketModalComponent) assignTicketModal!: AssignTicketModalComponent;
    selectedUserId: string;

    @ViewChild("file")
    file: ElementRef;
    messageIndex: number = -1;
    contactIndex: number = -1;
isNoteMode: boolean ;


    newCustomerFilter: NewCustomerListFilter = {
        searchId: 3,
        chatFilterID: 0,
        searchTerm: "",
        pageNumber: 0,
        pageSize: 40,
    };
    customersFilter: CustomerListFilter = {
        pageNumber: 0,
        pageSize: 1000,
        searchTerm: null,
    };
    differ: any;
    color: string;
    postMessageObj;
    UsersChannels: Channel[];
    user: Channel;
    UserMessage: ChannelMessage[] = [];
    selectedUser: Channel = new Channel();
    templates: any[] =[];

    userUpdateModel: UpdateCustomerModel = {};
    pageSize = 10;
    pageNumber = 0;
    pageSizeC = 20;
    pageNumberC = 0;
    topnumber = 0;
    groups: GroupDtoModel[] = [];
    tempGroups: GroupDtoModel[] = [];
    group: GroupDtoModel = null;
    throttle = 500;
    scrollDistance = 1;
    scrollUpDistance = 2;

    PeforH = 0;
    PeforT = 0;
    visible: boolean = false;
    isBlock: boolean;
    chatForm: UntypedFormGroup;
    hideEmoji = true;
    hideUploadFile = true;
    disabledAnotherAgent: boolean;
    urlPattern = /https?:\/\//;

    selectMessageToReply = false;
    messageToReply: ChannelMessage;
    isArabic = false;
    image_url: string =
        "https://scontent.xx.fbcdn.net/v/t39.30808-6/448581240_384370784648767_5463070973090164859_n.jpg?stp=dst-jpg_s851x315&_nc_cat=104&ccb=1-7&_nc_sid=c9359e&_nc_ohc=4Tv4u0QzIbUQ7kNvgE_gwUZ&_nc_ad=z-m&_nc_cid=0&_nc_ht=scontent.xx&oh=00_AYCZlRjpe1h0FSUVW1hheA_DFI_GTei3C6yPbPYb7KKR4w&oe=66958BFD";
    image_body: string =
        "نقــدم لكم اشتراك أدوبي كرييتف كلاود\nاشتراك أصلي 100%  و بحسب الدولة التي تقيم فيها لا حاجة لاستخدام VPN 🤩 و سعر أقل من الموقع الرسمي \n\n سنة كاملة \n(للماك والآياد +الويندوز) بالذكــــــاء الصناعي \n\nيتيح لك الاشتراك في أدوبي كلاود الوصول إلى جميع برامج أدوبي التالية: كالفوتوشوب -الستريتور - افتر افيكت - ادوبي اكروبات -بريمير........الخ \n\n👌خصائص الاشتراك:\n✅نسخة قانونية 100% وتحديثات أوتوماتيكية\n✅اشتراك سنة كاملة دفعة واحدة ولا يتم تجديده شهريا\n✅سهولة التفعيل وارتباطه بإيميلك الشخصي و نقله من جهاز لآخر\n✅يدعم جميع أجهزة (ويندوز-ماك-أندرويد-ايباد)\n✅يقبل التحديثات حتى آخر اصدار لأنه اشتراك رسمي وعلى موقع أدوبي الرسمي\n✅ضمان لكامل المدة\n✅الاشتراك غير متشارك مع أحد ولايوجد انضمام لفريق أو مؤسسة وماشابه\n✅دعم فني خاص بك من قبل شركة أدوبي\n\n👈 اشتراك محلى ليس تركي او أرجنتيني او اي اشتراك خارجي ينقطع بعد فترة (ضمان شهر كامل)\n👈 لا حاجة لأستخدام vpn او تغيير الدولة\n👈طريقة الإشتراك شرعية 100% (حلال)\n 👈مساحة تخزينية 100 جيجا\n\n😊يتم تفعيل اشتراك ادوبي كريتيف كلاود خلال 3 ساعات بحد اقصى من استلام الطلب \n😊 خصم 70% (سعر أقل من الموقع الرسمي )\n\nللاستفسار\n0789131598";
    image_headline: string = "طريق ميديا";
    image_link: string = "https://fb.me/6EUbpv6GO";

    // filetype = '.pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .rtf, .html, .zip, .mp3, .mp4, .wma, .mpg, .flv, .avi, .jpg, .jpeg, .png, .gif, .avi,.wav';
    // maxFileSize = 10000000;
    filename: string;

    selectedFilter: any = null;
ListNote:CustomerChat[];


ListNote3= [
    { listName: 'Meeting Notes', text: 'Discuss project timeline and milestones.' },
    { listName: 'Shopping List', text: 'Milk, Bread, Eggs, Butter.' },
    { listName: 'Ideas', text: 'Build a note-taking app with Angular.' }
  ];

    remainingTime: any;
    showPageLoader = false;

    anotherAgentMessgeSub: Subscription;
    customerMessageSub: Subscription;

    isSupport: boolean;

    isOneTime: boolean;
    isLoading = false;
    currentPage = 0;
    nextPage = 0;
    itemsPerPage = 20;

    isBackToTicketActive: boolean = false;

    agentsList: any[] = [];
    assigneeAgentName: string;
    files: FormData[] = [];
    showInputBox = false;
    uplodeFileOnclick = false;
    oneClick: boolean;
    time: boolean;
    date: boolean;
    titleMessage: string;
    uploadButton: boolean = false;
    newMessage: string = "";
    firstRender = true;
    scroll: boolean = false;
    isMore = true;
    isMore2 = true;
    showMessageLoader = false;

    prvHeight: number;
    ifFirstTime = true;
    globalFilter: string = "0";
    sound: any;
    isHasPermissionTemplate: boolean;
    heiget = 0;
    searchInput = new FormControl("");
    currentPosition: any;
    isSelectedUser: boolean;
    openChat = true;
    contact: ContactDto = new ContactDto();
    selectedChatFilterType: number = 0;
    filters: {
        selectedCustomerID: string;
        selectedLiveChatID: number;
    } = <any>{};
    statusID: number | null = null;

    selectedNewGroup: GroupDtoModel | undefined;
    submitted = false;
    isLanguageArabic = false;
    template: MessageTemplateModel[] = [];
    isNicknameUpdated: boolean = false;
    showLoaderOnOpenClose = false;
    record;
    recording = false;

    note2=false;
    countdown: number;
    selectedChatType: string = "0";
    isSearch = false;
    previewData: any;
    disabledkeyboard:boolean=false;
    ticketType:string;
profileNote=false;



    constructor(
        differs: IterableDiffers,
        injector: Injector,
        public darkModeService: DarkModeService,
        private teamService: TeamInboxService,
        public sanitizer: DomSanitizer,
        private templateMessagesServiceProxy: TemplateMessagesServiceProxy,
        private teamInboxServiceProxy: TeamInboxServiceProxy,
        private socketioService: SocketioService,
        private _contactsServiceProxy: ContactsServiceProxy,
        private _router: Router,
        public _recordRTC: RecordRTCService,
        private _activatedRoute: ActivatedRoute,
        private cd: ChangeDetectorRef,
        private groupService: GroupServiceProxy,
        private _permissionCheckerService: PermissionCheckerService,
        private _profileServiceProxy: ProfileServiceProxy,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        config: NgbDropdownConfig,
        private userService: UserServiceService,
        private router: Router,
                private _itemsServiceProxy: ItemsServiceProxy,
        
        public notificationService: NotificationsService,

    ) {
        super(injector);
        config.autoClose = false;
        this.differ = differs.find([]).create(null);
    }

    onToggleNoteMode() {

    console.log("Note mode:", this.isNoteMode); // should reflect current state

    // if(this.isNoteMode){
    //     this.isNoteMode=false;
    // }else{
    //     this.isNoteMode=true;
    // }
     this.updatenone()
}

    ngOnDestroy(): void {
        this.isBackToTicketActive = false;
        // this.searchInput.enable();
        const storedData = localStorage.getItem('ticketData'); 
     
        if (storedData) {
          const parsedData = JSON.parse(storedData);
          parsedData.statusID = null;  // Set statusID to null in the object
    
          // Save the updated object back to localStorage
          localStorage.setItem('ticketData', JSON.stringify(parsedData));
        }  

        if(this.ticketType){
            localStorage.removeItem('ticketType');
        }  
    }
    
    
    async ngOnInit() {
        if(localStorage.getItem('ticketType')){
            this.ticketType=localStorage.getItem('ticketType')
        }


        var x=false;
    this._activatedRoute.queryParams.subscribe(params => {

        const customerID = params['CustomerID'];
        if (!customerID) {

            x=true;
            return; 
        }  
        this.selectedUser.nickName = "";

        this.remainingTime = { leftTime: 0 };
        this.theme = ThemeHelper.getTheme();
        this.ifFirstTime = true;
        this.isHasPermissionTemplate = this._permissionCheckerService.isGranted(
            "Pages.Administration.TemplateMessages"
        );
        this.setFiltersFromRoute();
        this.onSearchEnter(null);
        return
    });

        this.getStatusIDFromLocalStorage();

        this.selectedUser.nickName = "";

        // // this.isLanguageArabic = rtlDetect.isRtlLang(
        // //     abp.localization.currentLanguage.name
        // // );

        // console.log(this.appSession.user.userName)

        this.remainingTime = { leftTime: 0 };
        this.theme = ThemeHelper.getTheme();
        this.ifFirstTime = true;
        this.isHasPermissionTemplate = this._permissionCheckerService.isGranted(
            "Pages.Administration.TemplateMessages"
        );
        this.setFiltersFromRoute();


        if(x){
            this.loadUsers();

        }
        this.initForm();

        

        this.subscribeTenantMessage();
        this.subscribeUserMessage();

        await this.getIsAdmin();

        // done
        // this.getTemplates();
        // this.getUsers();

        //  deleted
        // this.getGroupAll();
        // // this.getChannels();
        // this.getProfilePicture();
        // this.getDataFromLocalStorage();

    }

    getStatusIDFromLocalStorage(): void {
        const storedData = localStorage.getItem('ticketData'); 
        if (storedData) {
          const parsedData = JSON.parse(storedData);
          this.statusID = parsedData.statusID || null;  // Assuming statusID is in the stored object
        }
    }

    // Close Emoji-Mart when clicking outside
    @HostListener("document:click", ["$event"])
    onClick(event: MouseEvent) {
        if (this.isUserSelectedChat) {
            const target = event.target as HTMLElement;

            if (
                !this.emojiMart.nativeElement.contains(target) &&
                !target.classList.contains("fa-smile")
            ) {
                this.hideEmoji = true; // Close Emoji-Mart
            }
        }
    }

    
    getMessageAndContactIndex() {
        this.contactIndex = this.UsersChannels.slice().findIndex(
            (item) => item.searchId == 1
        );
        this.messageIndex = this.UsersChannels.slice().findIndex(
            (item) => item.searchId == 2
        );
    }

    returnBackToTicket() {
        this._router.navigate(["/app/main/liveChat"]);
    }

    getGroupAll() {
        this.groupService
            .groupGetAll("", 0, 2147483640)
            .subscribe(({ groupDtoModel }) => {
                this.groups = groupDtoModel;
                this.tempGroups = groupDtoModel;
            });
    }

    handleSelectGroup() {
        this.visible = true;
        this.submitted = true;
        if (this.selectedUser.groupId) {
            // to be moved
            const moveMembersDto = new MoveMembersDto();
            const membersDto = new MembersDto();
            membersDto.id = Number(this.selectedUser.contactID);
            membersDto.displayName = this.selectedUser.displayName;
            membersDto.failedId = 0;
            membersDto.isFailed = false;
            membersDto.phoneNumber = this.selectedUser.phoneNumber;

            moveMembersDto.membersDto = [membersDto];
            moveMembersDto.oldGroupId = this.selectedUser.groupId;
            moveMembersDto.newGroupId = this.selectedNewGroup.id;

            this.groupService.movingGroup(moveMembersDto).subscribe(
                (res) => {
                    if (res.state === 1) {
                        this.message.error("", this.l("groupNameisUsed"));
                        return;
                    } else if (res.state === 4) {
                        this.message.error("", this.l("groupNamecantbeempty"));
                        return;
                    } else if (res.state === 3) {
                        this.message.error("", this.l("invalidTenant"));
                        return;
                    } else if (res.state === 2) {
                        this.selectedUser.groupId = this.selectedNewGroup.id;
                        this.selectedUser.groupName =
                            this.selectedNewGroup.groupName;
                        this.notify.success("success");
                    }
                },
                (error: any) => {
                    if (error) {
                        this.submitted = false;
                        this.notify.error(error.error.error.message);
                    }
                },
                () => {
                    this.visible = false;
                    this.submitted = false;
                    this.tempGroups = null;
                    this.selectedNewGroup = null;
                }
            );
        } else {
            const groupMembersDto = new GroupMembersDto();
            const groupDtoModel = new GroupDtoModel();
            const membersDto = new MembersDto();

            membersDto.id = Number(this.selectedUser.contactID);
            membersDto.displayName = this.selectedUser.displayName;
            membersDto.failedId = 0;
            membersDto.isFailed = false;
            membersDto.phoneNumber = this.selectedUser.phoneNumber;

            groupDtoModel.id = this.selectedNewGroup.id;
            groupDtoModel.groupName = this.selectedNewGroup.groupName;

            groupMembersDto.membersDto = [membersDto];
            groupMembersDto.totalCount = 0;
            groupMembersDto.groupDtoModel = groupDtoModel;

            this.groupService.groupUpdate(false, 2, groupMembersDto).subscribe(
                (res) => {
                    if (res.state === 1) {
                        this.message.error("", this.l("groupNameisUsed"));
                        return;
                    } else if (res.state === 4) {
                        this.message.error("", this.l("groupNamecantbeempty"));
                        return;
                    } else if (res.state === 3) {
                        this.message.error("", this.l("invalidTenant"));
                        return;
                    } else if (res.state === 2) {
                        this.selectedUser.groupId = this.selectedNewGroup.id;
                        this.selectedUser.groupName =
                            this.selectedNewGroup.groupName;
                        this.notify.success("success");
                    }
                },
                (error: any) => {
                    if (error) {
                        this.notify.error(error.error.error.message);
                    }
                },
                () => {
                    this.visible = false;
                    this.submitted = false;
                    this.selectedNewGroup = null;
                }
            );
        }
    }

    // handleSelectChange(selectedItmem: any) {
    //     debugger

    //     this.groupID = selectedItmem.id;
    //     this.groupName = selectedItmem.name;
    //     // this.getGroupById();
    // }

    handleChangeGroup(selectedCustomer: any = null) {
        this.showDialog();
    }

    get GroupheaderText() {
        return this.selectedUser.groupId
            ? "Move Member to other Group"
            : "Move Member to other Group";
    }

    handleClose() {
        this.visible = false;
        this.selectedNewGroup = null;
    }

    onSubmitChatFilter() {

        this.globalFilter = this.selectedChatType;

        this.newCustomerFilter.chatFilterID = Number(this.globalFilter);

        this.onSearchEnter(null);

        this.dropSetting.close();
    }

    getValueLocal(opt: number) {
        if (opt === 1) return this.l("optOut");
        if (opt === 2) return this.l("optIn");
        if (opt === 0) return this.l("netural");
    }

    toggleSetting() {
        this.selectedChatType = this.globalFilter;
    }

    closeSetting(event: any) {
        event.stopPropagation();
        this.selectedChatType = this.globalFilter;
        this.dropSetting.close();
    }

    //on textInput search

    onSearchEnter(noteID:any): void {
        var searchValue = this.searchInput.value?.trim();
        var userID="";
        this.newCustomerFilter.searchId=5;
        if (this.filters?.selectedCustomerID) {
            this.searchInput.setValue(this.filters.selectedCustomerID);
            this.newCustomerFilter.searchId=3;
            searchValue=this.filters?.selectedCustomerID;
            // valid and truthy value
          } else {
            // it's undefined, null, or empty
          }


          if(noteID!=null){

             this.newCustomerFilter.searchId=6;
             searchValue=noteID.text;
             userID=noteID.userId
          }



        if (searchValue && (searchValue.length > 3 || searchValue==='')) {

            if (searchValue !== null && searchValue !== undefined) {

              
                    this.isSearch = searchValue.length > 0;
                    this.currentPage = 0;
                    this.isLoading = true;
                
                    this.teamInboxServiceProxy
                      .customersGetAll(
                        searchValue,
                        this.newCustomerFilter.searchId,
                        this.newCustomerFilter.chatFilterID,
                        this.currentPage,
                        this.itemsPerPage,this.appSession.user.id,userID
                      )
                      .subscribe((result: any) => {
                        this.UsersChannels = result;
                        this.handleCustomerSearchResults(result);




                        // this.UsersChannels = this.UsersChannels.filter(
                        //     (v, i, a) => a.findIndex(t => t.phoneNumber === v.phoneNumber) === i
                        //   );
                        //   const grouped = this.UsersChannels.reduce((acc, user) => {
                        //     const key = user.phoneNumber;
                        //     if (!acc[key]) {
                        //       acc[key] = { ...user, conversationsCount: 1 };
                        //     } else {
                        //       acc[key].conversationsCount++;
                        //     }
                        //     return acc;
                        //   }, {} as { [key: string]: any });
                          
                        //   this.UsersChannels = Object.values(grouped);







                      });              
             
            }//end if 1

        }else{

            this.teamInboxServiceProxy
            .customersGetAll(
              searchValue,
              this.newCustomerFilter.searchId,
              this.newCustomerFilter.chatFilterID,
              this.currentPage,
              this.itemsPerPage,this.appSession.user.id,userID
            )
            .subscribe((result: any) => {
              this.UsersChannels = result;
              this.handleCustomerSearchResults(result);


            });              

        }
    
       
      }
      handleCustomerSearchResults(result: any): void {
        this.isLoading = false;
      
        const colors = ["#4972bc", "#b45f69", "#7dc7b9", "#ec7a1d", "#85b4b4"];
        const images = ["avatar3", "avatar3", "avatar3", "avatar3", "avatar3"];
      
        this.UsersChannels = result.map((item) => ({
          ...item,
          color: colors[Math.floor(Math.random() * colors.length)],
          avatarUrl: images[Math.floor(Math.random() * images.length)],
          fullCretaedDate: item.createDate,
        }));
      
        if (this.newCustomerFilter.searchId !== 3) {
          this.sortedArrayOfCustomerMessages();
        } else {
          this.getMessageAndContactIndex();
        }
      
        if (this.UsersChannels.length > 0) {
          this.disabledAnotherAgent =(
            this.selectedUser.isLockedByAgent &&
            this.appSession.user.id === this.selectedUser.agentId &&
            this.selectedUser.isOpen ) ;
      
          this.isMore = true;
          this.isMore2 = true;
          this.showPageLoader = false;
      
          this.getTime(this.UsersChannels);
          this.countDown(null);
        } else {
          this.showPageLoader = false;
        }
      
        if (this.filters.selectedCustomerID || this.filters.selectedLiveChatID) {
          this.isBackToTicketActive = true;
          this.selectUserToChat(result[0]);
          this.selectedIndex = 0;
        }
      }
    // onSearchEnter(): void {
    //     if (
    //         this.filters.selectedCustomerID == null ||
    //         this.filters.selectedCustomerID == ""
    //     ) {
    //         this.searchInput.valueChanges
    //             .pipe(
    //                 debounceTime(800),
    //                 switchMap((searchTerm) => {
    //                     this.currentPage = 0;
    //                     this.isLoading = true;
    //                     if (searchTerm.toString().length > 0) {
    //                         this.isSearch = true;
    //                     } else {
    //                         this.isSearch = false;
    //                     }
    //                     // this.newCustomerFilter.searchTerm = searchTerm.toString();
    //                     return this.teamInboxServiceProxy.customersGetAll(
    //                         searchTerm.toString(),
    //                         this.newCustomerFilter.searchId,
    //                         this.newCustomerFilter.chatFilterID,
    //                         this.currentPage,
    //                         this.itemsPerPage
    //                     );
    //                 })
    //             )
    //             .subscribe(
    //                 (result: any) => {
    //                     this.UsersChannels = result;

    //                     this.isLoading = false;

    //                     var colors = [
    //                         "#4972bc  ",
    //                         "#b45f69 ",
    //                         "#7dc7b9 ",
    //                         "#ec7a1d ",
    //                         "#85b4b4",
    //                     ];
    //                     var images = [
    //                         "avatar1",
    //                         "avatar2",
    //                         "avatar3",
    //                         "avatar4",
    //                         "avatar5",
    //                     ];
    //                     this.UsersChannels.forEach((item) => {
    //                         item.color =
    //                             colors[
    //                                 Math.floor(Math.random() * colors.length)
    //                             ];
    //                         item.avatarUrl =
    //                             images[
    //                                 Math.floor(Math.random() * images.length)
    //                             ];
    //                         item.fullCretaedDate = item.createDate;
    //                     });
    //                     if (this.newCustomerFilter.searchId != 3) {
    //                         this.sortedArrayOfCustomerMessages();
    //                     } else {
    //                         this.getMessageAndContactIndex();
    //                     }

    //                     if (this.UsersChannels.length > 0) {
    //                         if (
    //                             this.selectedUser.isLockedByAgent &&
    //                             this.appSession.user.id ===
    //                                 this.selectedUser.agentId &&
    //                             this.selectedUser.isOpen
    //                         ) {
    //                             this.disabledAnotherAgent = true;
    //                         } else {
    //                             this.disabledAnotherAgent = false;
    //                         }

    //                         this.isMore = true;
    //                         this.isMore2 = true;
    //                         this.showPageLoader = false;

    //                         this.getTime(this.UsersChannels);
    //                         this.countDown(null);
    //                     } else {
    //                         this.showPageLoader = false;
    //                     }

    //                     if (
    //                         this.filters.selectedCustomerID ||
    //                         this.filters.selectedLiveChatID
    //                     ) {
    //                         // this.searchInput.disable();
    //                         this.isBackToTicketActive = true;
    //                         this.selectUserToChat(result[0]);
    //                         this.selectedIndex = 0;
    //                     }
    //                 },
    //                 (error) => {
    //                     this.toggleLoading();
    //                     this.showPageLoader = false;
    //                     console.log(error);
    //                 }
    //             );
                
    //     } else {
    //         this.newCustomerFilter.searchId = 3;
    //         this.newCustomerFilter.searchTerm = this.filters.selectedCustomerID;
    //         this.searchInput.setValue(this.filters.selectedCustomerID);
    //     }
    // }

    handleChangeType() {

        this.globalFilter = this.selectedChatType;

        this.newCustomerFilter.chatFilterID = Number(this.globalFilter);

        this.handleChangeType();

        this.dropSetting.close();
    }

    @ViewChild("chatFilterForm", { static: true }) chatFilterForm: NgForm;
    @ViewChild("dropSetting", { static: true }) dropSetting: any;

    toggleLoading = () => (this.isLoading = !this.isLoading);

    loadUsers = () => {
        this.toggleLoading();

        this.UsersChannels = [];

        const { searchId, chatFilterID, searchTerm } = this.newCustomerFilter;

        this.teamInboxServiceProxy
            .customersGetAll(
                searchTerm,
                searchId,
                chatFilterID,
                this.currentPage,
                this.itemsPerPage,this.appSession.user.id,""
            )
            .subscribe(
                (result: any) => {
                    this.UsersChannels = result;
                    var colors = [
                        "#4972bc  ",
                        "#b45f69 ",
                        "#7dc7b9 ",
                        "#ec7a1d ",
                        "#85b4b4",
                    ];
                    var images = [
                        "avatar3",
                        "avatar3",
                        "avatar3",
                        "avatar3",
                        "avatar3",
                    ];
                    this.UsersChannels.forEach((item) => {
                        item.color =
                            colors[Math.floor(Math.random() * colors.length)];

                        //      if(item.channel =="facebook" && item.avatarUrl!=''){
                        //     //this.UsersChannels[index].avatarUrl = data.avatarUrl;
                        //     }else{
                        //       item.avatarUrl =
                       
                        // item.avatarUrl =
                        //     images[Math.floor(Math.random() * images.length)];

                        //     }

                        item.fullCretaedDate = item.createDate;
                    });
                    // this.sortedArrayOfCustomerMessages();

                    if (this.UsersChannels.length > 0) {
                        if (
                            this.selectedUser.isLockedByAgent &&
                            this.appSession.user.id ===
                                this.selectedUser.agentId &&
                            this.selectedUser.isOpen
                        ) {
                            this.disabledAnotherAgent = true;
                        } else {
                            this.disabledAnotherAgent = false;
                        }

                        this.isMore = true;
                        this.isMore2 = true;
                        this.showPageLoader = false;

                        this.getTime(this.UsersChannels);

                        this.countDown(null);
                    } else {
                        this.showPageLoader = false;
                    }

                    if (
                        this.filters.selectedCustomerID ||
                        this.filters.selectedLiveChatID
                    ) {
                        this.searchInput.disable();
                        this.isBackToTicketActive = true;
                        this.selectUserToChat(result[0]);
                        this.selectedIndex = 0;
                    }
                    this.toggleLoading();
                },
                (error) => {
                    this.toggleLoading();
                    this.showPageLoader = false;
                }
            );
    };

    onFocus() {
        // Ensures focus goes back to the textarea after it's re-enabled
        if (!this.disabledkeyboard) {
          setTimeout(() => {
            document.getElementById('ChatMessage')?.focus();
          }, 0);
        }
      }
      
    appendData = () => {
        var { searchId, chatFilterID, searchTerm } = this.newCustomerFilter;
        this.UsersChannels;

var searchValue = this.searchInput.value?.trim();
if (searchValue !== null && searchValue !== undefined) {


    searchTerm=searchValue;
}
        this.teamInboxServiceProxy
            .customersGetAll(
                searchTerm,
                searchId,
                chatFilterID,
                this.currentPage,
                this.itemsPerPage,this.appSession.user.id,""
            )
            .subscribe(
                (result: any) => {
                    result.forEach((element) => {
                        const isfoiu = this.UsersChannels.some(
                            (item) => item.phoneNumber === element.phoneNumber
                        );
                        var colors = [
                            "#4972bc  ",
                            "#b45f69 ",
                            "#7dc7b9 ",
                            "#ec7a1d ",
                            "#85b4b4",
                        ];
                        var images = [
                            "avatar3",
                            "avatar3",
                            "avatar3",
                            "avatar3",
                            "avatar3",
                        ];

                        element.color =
                            colors[Math.floor(Math.random() * colors.length)];

                            // if(element.channel =="facebook" && element.avatarUrl!=''){
                            // //this.UsersChannels[index].avatarUrl = data.avatarUrl;
                            // }else{
                            //   element.avatarUrl =
                            // images[Math.floor(Math.random() * images.length)];

                            // }
                     

                        element.fullCretaedDate = element.createDate;

                        if (!isfoiu) {
                            this.UsersChannels.push(element);
                        }

                    });
                    if (this.newCustomerFilter.searchId != 3) {
                        this.sortedArrayOfCustomerMessages();
                    } else {
                        this.getMessageAndContactIndex();
                    }

                    if (this.UsersChannels.length > 0) {
                        if (
                            this.selectedUser.isLockedByAgent &&
                            this.appSession.user.id ===
                                this.selectedUser.agentId &&
                            this.selectedUser.isOpen
                        ) {
                            this.disabledAnotherAgent = true;
                        } else {
                            this.disabledAnotherAgent = false;
                        }

                        this.getTime(this.UsersChannels);
                        this.countDown(null);
                    } else {
                        this.showPageLoader = false;
                    }

                    // this.isMore2 = result.result.length > 0;
                    // this.scrollChatt.nativeElement.scrollTop = this.scrollChatt.nativeElement.scrollHeight;//this.prvHeight;
                    this.isOneTime = true;
                },
                (error) => {
                    this.showPageLoader = false;
                }
            );
    };

    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }

    initiateRecording() {
        this.recording = true;
        let mediaConstraints = {
            video: false,
            audio: true,
        };
        navigator.mediaDevices
            .getUserMedia(mediaConstraints)
            .then(this.successCallback.bind(this));
    }

    successCallback(stream) {
        var options = {
            mimeType: "audio/wav",
            numberOfAudioChannels: 1,
            sampleRate: 16000,
        };
        var StereoAudioRecorder = RecordRTC.StereoAudioRecorder;
        this.record = new StereoAudioRecorder(stream, options);
        this.record.record();
    }

    stopRecording() {
        this.recording = false;
    }
    updateNickName() {
        this.isNicknameUpdated = true;
        this.teamInboxServiceProxy
            .nickNameUpdate(
                Number(this.selectedUser.contactID),
                this.selectedUser.nickName
            )
            .subscribe((response) => {
                this.isNicknameUpdated = false;
                if (response.state === 2) this.notify.success("Updated");
                else {
                    this.notify.error("Something went Wrong");
                }
            });
    }

    openSendMessageModal() {
        // this.modal.viewDetails(this.template || [] , selectedUser);
        //      this.template = [];
        // this._whatsAppMessageTemplateServiceProxy
        //     .getWhatsAppTemplateForCampaign(0, 50, this.appSession.tenantId)
        //     .subscribe((result) => {
        //         this.template = result.lstWhatsAppTemplateModel.filter(
        //             (element) =>
        //                 element.language == "ar" || element.language == "en"
        //         );
        //         this.primengTableHelper.hideLoadingIndicator();
        //     });
        this.sendMessageModal.show(this.template || []);
    }

    setFiltersFromRoute(): void {
        if (this._activatedRoute.snapshot.queryParams["CustomerID"] != null) {
            this.filters.selectedCustomerID =
                this._activatedRoute.snapshot.queryParams["CustomerID"];
        }
        if (this._activatedRoute.snapshot.queryParams["LiveChatId"] != null) {
            this.filters.selectedLiveChatID =
                this._activatedRoute.snapshot.queryParams["LiveChatId"];
            this.selectedUser.userId =
                this._activatedRoute.snapshot.queryParams["CustomerID"];

            // if (this.appSession.tenantId !== 59) {
            //     // if (
            //     //     this._activatedRoute.snapshot.queryParams["openChat"] ===
            //     //     "true"
            //     // ) {
            //     //     this.openChat = true;
            //     //     // this.open();
            //     // } else {
            //     //     this.openChat = false;
            //     // }
            // }

            //    this. _liveChatServiceProxy.updateConversationsCount(this.filters.selectedCustomerID).subscribe((result) => { });
        }
    }

    getDataFromLocalStorage() {
        // Retrieve data from local storage
        const jsonString = localStorage.getItem("ticketData");

        // Parse JSON string to JavaScript object
        if (jsonString) {
            this.isBackToTicketActive = true;
        } else {
            this.isBackToTicketActive = false;
        }
    }

    getProfilePicture(): void {
        this._profileServiceProxy.getProfilePicture().subscribe((result) => {
            if (result && result.profilePicture) {
                this.profilePicture =
                    "data:image/jpeg;base64," + result.profilePicture;
            }
        });
    }

    getTemplates() {
        this.template = [];
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppTemplateForCampaign(0, 50, this.appSession.tenantId)
            .subscribe((result) => {
                this.template = result.lstWhatsAppTemplateModel.filter(
                    (element) =>
                        element.language == "ar" || element.language == "en"
                );
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    subscribeTenantMessage = () => {
        //chat get // bot answer // open // close //  admin answer


        this.anotherAgentMessgeSub =
            this.socketioService.agentMessage.subscribe((data) => {
                if (this.appSession.tenantId === data.tenantId) {
                    const index = this.UsersChannels.findIndex(
                        (e) => e.userId === data.userId
                    );
                    this.showLoaderOnOpenClose = false;

                    if (index != -1) {
                        if (this.selectedUser?.userId === data.userId) {
                            if (data.customerChat != null) {
                                const indexChat = this.UserMessage.findIndex(
                                    (e) =>
                                        e.messageId ===
                                        data.customerChat.messageId
                                );

                                if (indexChat == -1) {
                                    this.UserMessage.push(data.customerChat);
                                } else {
                                    this.UserMessage[indexChat].status =
                                        data.customerChat.status;
                                    this.UserMessage.push(data.customerChat);
                                }
                            }
                            // else {
                            //     this.UserMessage.push(data.customerChat);
                            // }
                            this.UserMessage.forEach((message) => {
                                if (message.text != null) {
                                    if (message.text.includes("*")) {
                                        message.text = message.text.replace(
                                            /\*/g,
                                            ""
                                        );
                                    }
                                }
                            });

                            // this.UserMessage.forEach(message => {
                            //     this.hasArabicCodepoints(message);
                            // });
                            // this.UsersChannels[index].avatarUrl = "avatar2";
                            this.UsersChannels[index].color = "#4972bc";
                            this.UsersChannels[
                                index
                            ].lastConversationStartDateTime =
                                data.lastConversationStartDateTime;

                            this.UsersChannels[index].lastMessageData =
                                data.customerChat.createDate;

                            this.UsersChannels[index].lastMessageText =
                                data.customerChat.text;
                            this.UsersChannels[index].unreadMessagesCount =
                                data.unreadMessagesCount;
                            this.UsersChannels[index].conversationsCount =
                                data.conversationsCount;

                            this.UsersChannels[index].isSupport =
                                data.isSupport;
                            this.UsersChannels[index].isComplaint =
                                data.isComplaint;
                            this.UsersChannels[index].isOpen = data.isOpen;
                            this.UsersChannels[index].lockedByAgentName =
                                data.lockedByAgentName;
                            this.UsersChannels[index].isLockedByAgent =
                                data.isLockedByAgent;
                            this.UsersChannels[index].isBlockCustomer =
                                data.isBlockCustomer;
                            this.UsersChannels[index].isConversationExpired =
                                data.isConversationExpired;
                            this.UsersChannels[index].agentId = data.agentId;

                            this.UsersChannels[index].isBlock = data.isBlock;
                            this.UsersChannels[index].isBotCloseChat =
                                data.isBotCloseChat;

                            this.UsersChannels[index].isBotChat =
                                data.isBotChat;
                            this.UsersChannels[index].expiration_timestamp =
                                data.expiration_timestamp;
                            this.UsersChannels[index].creation_timestamp =
                                data.creation_timestamp;
                            this.UsersChannels[index].customerChat.status =
                                data.customerChat.status;



                                this.UsersChannels[index].channel=
                                data.channel;
                  this.UsersChannels[index].gender=
                                data.gender;
  this.UsersChannels[index].instagramUserInfoModel=
                                data.instagramUserInfoModel;

     this.UsersChannels[index].facebookUserInfoModel=
                                data.facebookUserInfoModel;                         // this.UserMessage.push(data.customerChat);
                            if (data.customerChat) {
                                this.getTime(this.UsersChannels);
                            }
                            // this.UserMessage.push(data.customerChat);
                            if (this.isUserSelectedChat) {
                                setTimeout(() => {
                                    this.scrollChat.nativeElement.scroll({
                                        top: this.scrollChat.nativeElement
                                            .scrollHeight,
                                        left: 0,
                                        behavior: "smooth",
                                    });
                                }, 100);
                            }

                            if (
                                data.isLockedByAgent &&
                                this.appSession.user.id === data.agentId &&
                                data.isOpen
                            ) {
                                this.disabledAnotherAgent = true;
                                //  this.selectedUser.isOpen=true;
                            } else {
                                this.disabledAnotherAgent = false;
                            }
                        } else {
                            // this.UsersChannels[index].avatarUrl = "avatar2";
                            this.UsersChannels[index].color = "#4972bc";
                            this.UsersChannels[
                                index
                            ].lastConversationStartDateTime =
                                data.lastConversationStartDateTime;

                            this.UsersChannels[index].lastMessageData =
                                data.customerChat.createDate;

                            this.UsersChannels[index].lastMessageText =
                                data.customerChat.text;
                            this.UsersChannels[index].unreadMessagesCount =
                                data.unreadMessagesCount;
                            this.UsersChannels[index].conversationsCount =
                                data.conversationsCount;
                            this.UsersChannels[index].isSupport =
                                data.isSupport;
                            this.UsersChannels[index].isComplaint =
                                data.isComplaint;
                            this.UsersChannels[index].isOpen = data.isOpen;
                            this.UsersChannels[index].lockedByAgentName =
                                data.lockedByAgentName;
                            this.UsersChannels[index].isLockedByAgent =
                                data.isLockedByAgent;
                            this.UsersChannels[index].isBlockCustomer =
                                data.isBlockCustomer;
                            this.UsersChannels[index].isConversationExpired =
                                data.isConversationExpired;
                            this.UsersChannels[index].agentId = data.agentId;

                            this.UsersChannels[index].isBlock = data.isBlock;
                            this.UsersChannels[index].isBotCloseChat =
                                data.isBotCloseChat;

                            this.UsersChannels[index].isBotChat =
                                data.isBotChat;
                            this.UsersChannels[index].expiration_timestamp =
                                data.expiration_timestamp;

                            this.UsersChannels[index].creation_timestamp =
                                data.creation_timestamp;
                            this.UsersChannels[index].customerChat.status =
                                data.customerChat.status;

                                this.UsersChannels[index].channel=
                                data.channel;
                              this.UsersChannels[index].gender=
                                data.gender;
                                  this.UsersChannels[index].instagramUserInfoModel=
                                data.instagramUserInfoModel;
                                   this.UsersChannels[index].facebookUserInfoModel=
                                data.facebookUserInfoModel;                               
                            if (data.customerChat) {
                                this.getTime(this.UsersChannels);
                            }
                            // if (data.isLockedByAgent && this.appSession.user.id === data.agentId) {

                            //     this.disabledAnotherAgent = true;
                            // } else {

                            //     this.disabledAnotherAgent = false;
                            // }
                        }
                    }
                }
            });
    };

    subscribeUserMessage = () => {
        //contact get // user answer        
        this.showPageLoader = true;
        this.customerMessageSub =
            this.socketioService.customerMessage.subscribe((data) => {
                this.countDown(null);
                if (!(this.UsersChannels.some(channel => channel.userId === data.userId)) && (this.selectedChatType=='1' ||this.selectedChatType=='2' ||this.selectedChatType=='3' ||this.selectedChatType=='4')) {
                    return;
                }

                if ( ( this.isBackToTicketActive &&data.userId != this.selectedUser.userId) ) {
                    return;
                }



                // if (
                //     (this.isBackToTicketActive &&
                //         data.userId != this.filters.selectedCustomerID) ||
                //     this.isSearch
                // ) {
                //     return;
                // }
                if (this.appSession.tenantId == data.tenantId) {
                    this.showLoaderOnOpenClose = false;
                    const oldontactndex = this.UsersChannels.findIndex(
                        (x) => x.phoneNumber == data.phoneNumber
                    );
                    console.log()
                    if (oldontactndex != -1) {
                        const index = this.UsersChannels.findIndex(
                            (e) => e.userId === data.userId
                        );

                        if (this.selectedUser.userId === data.userId) {
                            this.UserMessage.push(data.customerChat);
                            this.UserMessage.forEach((message) => {
                                if (message.text != null) {
                                    if (message.text.includes("*")) {
                                        message.text = message.text.replace(
                                            /\*/g,
                                            ""
                                        );
                                    }
                                }
                            });
                            // this.UserMessage.forEach(message => {
                            //     this.hasArabicCodepoints(message);
                            // });
                            this.goToLastMassage();

                            this.UsersChannels[
                                index
                            ].lastConversationStartDateTime =
                                data.lastConversationStartDateTime;
                            this.UsersChannels[index].createDate =
                                data.customerChat.createDate;

                            this.UsersChannels[index].createDate =
                                data.customerChat.createDate;

                            this.UsersChannels[index].lastMessageText =
                                data.customerChat.text;
                            this.UsersChannels[index].unreadMessagesCount =
                                data.unreadMessagesCount;
                            this.UsersChannels[index].conversationsCount =
                                data.conversationsCount;
                            this.UsersChannels[index].isSupport =
                                data.isSupport;
                            this.UsersChannels[index].isComplaint =
                                data.isComplaint;
                            this.UsersChannels[index].isOpen = data.isOpen;
                            this.UsersChannels[index].lockedByAgentName =
                                data.lockedByAgentName;
                            this.UsersChannels[index].isLockedByAgent =
                                data.isLockedByAgent;
                            this.UsersChannels[index].isBlockCustomer =
                                data.isBlockCustomer;
                            this.UsersChannels[index].isConversationExpired =
                                data.isConversationExpired;
                            this.UsersChannels[index].agentId = data.agentId;
                            this.UsersChannels[index].isBlock = data.isBlock;
                            this.UsersChannels[index].isBotCloseChat =
                                data.isBotCloseChat;
                            this.UsersChannels[index].isBotChat =
                                data.isBotChat;
                            // if(data.channel =="facebook" && data.avatarUrl!=''){
                            // this.UsersChannels[index].avatarUrl = data.avatarUrl;
                            // }else{
                            // this.UsersChannels[index].avatarUrl = "avatar3";

                            // }
                            this.UsersChannels[index].color = "#4972bc";
                            this.UsersChannels[index].expiration_timestamp =
                                data.expiration_timestamp;
                            this.UsersChannels[index].creation_timestamp =
                                data.creation_timestamp;
                            this.UsersChannels[index].lastMessageData =
                                data.lastMessageData;
                            this.UsersChannels[index].fullCretaedDate =
                                data.createDate;
                            this.UsersChannels[index].customerChat.status =
                                data.customerChat.status;

                                this.UsersChannels[index].channel=
                                data.channel;

                               this.UsersChannels[index].gender=
                                data.gender;
                                  this.UsersChannels[index].instagramUserInfoModel=
                                data.instagramUserInfoModel;
                                   this.UsersChannels[index].facebookUserInfoModel=
                                data.facebookUserInfoModel;                               
                            if (data.customerChat) {
                                this.getTime(this.UsersChannels);
                            }
                            this.selectedIndex = 0;
                        } else {
                            
                        //  if(data.channel =="facebook"){
                        //     //this.UsersChannels[index].avatarUrl = data.avatarUrl;
                        //     }else{
                        //           data.avatarUrl = "avatar3";
                        //     }
                            
                            data.color = "#4972bc";
                            data.fullCretaedDate = data.createDate;

                            this.UsersChannels[index].channel=
                            data.channel;
                           this.UsersChannels[index].gender=
                            data.gender;
                              this.UsersChannels[index].instagramUserInfoModel=
                                data.instagramUserInfoModel;
                               this.UsersChannels[index].facebookUserInfoModel=
                                data.facebookUserInfoModel;                               
                            this.UsersChannels.splice(index, 1);
                            this.UsersChannels.unshift(data);
                            this.selectedIndex = this.UsersChannels.findIndex(
                                (e) => e.userId === this.selectedUser.userId
                            );

                            if (
                                this.selectedUser.id != null ||
                                this.selectedUser.id != undefined
                            ) {
                                // this.selectedIndex = selectedUserIndex + 1;
                                if (
                                    this.selectedUser.isLockedByAgent &&
                                    this.appSession.user.id ===
                                        this.selectedUser.agentId &&
                                    this.selectedUser.isOpen
                                ) {
                                    this.disabledAnotherAgent = true;
                                } else {
                                    this.disabledAnotherAgent = false;
                                }
                            }
                        }
                    } else {
                        if (
                            this.customersFilter.searchTerm == "" ||
                            this.customersFilter.searchTerm == null
                        ) {

                        //  if(data.channel =="facebook" && data.avatarUrl!=''){
                        //     //this.UsersChannels[index].avatarUrl = data.avatarUrl;
                        //     }else{
                        //           data.avatarUrl = "avatar3";
                        //     }
                        
                            data.color = "#4972bc";
                            data.fullCretaedDate = data.createDate;
                            this.UsersChannels.push(data);
                            this.getTime(this.UsersChannels);
                            this.countDown(null);
                        }
                    }
                    this.UsersChannels = this.UsersChannels.filter(
                        (el, i, a) => i === a.indexOf(el)
                    );
                    this.getTime(this.UsersChannels);
                    this.sortedArrayOfCustomerMessages();
                    let ishere = window.location.href.includes("teamInbox12");
                    if (data.isOpen && ishere) {
                        if (this.sound != null) {
                            this.sound.stop();
                            this.sound.unload();
                            this.sound = null;
                        }

                        this.sound = new Howl({
                            src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/WhatsAppNotification.mp3",
                            html5: true,
                            volume: 1.0,
                        });
                        this.sound.play();
                    }
                }
            });
    };
    copyCode(inputTextValue) {
        const selectBox = document.createElement("textarea");
        selectBox.style.position = "fixed";
        selectBox.value = inputTextValue;
        document.body.appendChild(selectBox);
        selectBox.focus();
        selectBox.select();
        document.execCommand("copy");
        document.body.removeChild(selectBox);
        this.notify.success(this.l("succcessfullyCopied"));
    }

    sortedArrayOfCustomerMessages() {
        this.UsersChannels.sort(function (a, b) {
            // Turn your strings into dates, and then subtract them
            // to get a value that is either negative, positive, or zero.
            return (
                Number(new Date(b.lastMessageData)) -
                Number(new Date(a.lastMessageData))
            );
        });
    }

    initForm() {
        this.chatForm = new UntypedFormGroup({
            text: new UntypedFormControl("", [Validators.required]),
            formFile: new UntypedFormControl("", [Validators.required]),
        });
    }


    NoteSwitch(): void {
        
       
      }


    getAllTemplates(): void {
        
        if (!this.templates || this.templates.length === 0) {
          this.templateMessagesServiceProxy.getAllNoFilter().subscribe(
            (result) => {
              this.templates = result.items;
            },
            (error) => this.notify.error('Error Happened')
          );
        }
      }
    
    openModal(selectedUser: Channel) {
        this.modal.viewDetails(this.template || [], selectedUser);
    }

    getTemplateMessage(templateMessageId) {
        this.templateMessagesServiceProxy
            .getTemplateMessageForEdit(templateMessageId)
            .subscribe((result) => {
                this.chatForm
                    .get("text")
                    .setValue(result.templateMessage.messageText);
            });
    }

    getChannels() {
        this.isOneTime = true;
        this.isMore = true;
        this.isMore2 = true;
        this.pageNumber = 0;
        this.UsersChannels = [];

        if (!this.UsersChannels) {
            this.showPageLoader = true;
        }
        this.customersFilter.pageNumber = this.pageNumberC;
        this.customersFilter.pageSize = this.pageSizeC;

        if (
            this.filters.selectedCustomerID == null ||
            this.filters.selectedCustomerID == ""
        ) {
        } else {
            this.customersFilter.searchTerm = this.filters.selectedCustomerID;
        }

        const { searchId, chatFilterID, searchTerm, pageNumber, pageSize } =
            this.newCustomerFilter;

        this.teamInboxServiceProxy
            .customersGetAll(
                searchTerm,
                searchId,
                chatFilterID,
                pageNumber,
                pageSize,this.appSession.user.id,""
            )
            .subscribe(
                (result: any) => {
                    this.UsersChannels = result;
                    var colors = [
                        "#4972bc  ",
                        "#b45f69 ",
                        "#7dc7b9 ",
                        "#ec7a1d ",
                        "#85b4b4",
                    ];
                    var images = [
                        "avatar3",
                        "avatar3",
                        "avatar3",
                        "avatar3",
                        "avatar3",
                    ];
                    this.UsersChannels.forEach((item) => {
                        item.color =
                            colors[Math.floor(Math.random() * colors.length)];
                        // if(item.channel =="facebook" && item.avatarUrl!=''){
                        //     //this.UsersChannels[index].avatarUrl = data.avatarUrl;
                        //     }else{ 
                        //          item.avatarUrl =
                        //     images[Math.floor(Math.random() * images.length)];

                        //     }
                      
                        item.fullCretaedDate = item.createDate;
                    });
                    this.sortedArrayOfCustomerMessages();

                    if (this.UsersChannels.length > 0) {
                        if (
                            this.selectedUser.isLockedByAgent &&
                            this.appSession.user.id ===
                                this.selectedUser.agentId &&
                            this.selectedUser.isOpen
                        ) {
                            this.disabledAnotherAgent = true;
                        } else {
                            this.disabledAnotherAgent = false;
                        }

                        this.isMore = true;
                        this.isMore2 = true;
                        this.showPageLoader = false;

                        this.getTime(this.UsersChannels);
                        this.countDown(null);
                    } else {
                        this.showPageLoader = false;
                    }

                    if (
                        this.filters.selectedCustomerID ||
                        this.filters.selectedLiveChatID
                    ) {
                        this.searchInput.disable();
                        this.isBackToTicketActive = true;
                        this.selectUserToChat(result[0]);
                        this.selectedIndex = 0;
                    }
                },
                (error) => {
                    this.showPageLoader = false;
                }
            );

        // this.teamService.getCustomer(this.customersFilter).subscribe(
        //     (result: any) => {
        //         this.UsersChannels = result.result;
        //         var colors = [
        //             "#4972bc  ",
        //             "#b45f69 ",
        //             "#7dc7b9 ",
        //             "#ec7a1d ",
        //             "#85b4b4",
        //         ];
        //         var images = [
        //             "avatar1",
        //             "avatar2",
        //             "avatar3",
        //             "avatar4",
        //             "avatar5",
        //         ];
        //         this.UsersChannels.forEach((item) => {
        //             item.color =
        //                 colors[Math.floor(Math.random() * colors.length)];
        //             item.avatarUrl =
        //                 images[Math.floor(Math.random() * images.length)];
        //             item.fullCretaedDate = item.createDate;
        //         });
        //         this.sortedArrayOfCustomerMessages();

        //         if (this.UsersChannels.length > 0) {
        //             if (
        //                 this.selectedUser.isLockedByAgent &&
        //                 this.appSession.user.id === this.selectedUser.agentId &&
        //                 this.selectedUser.isOpen
        //             ) {
        //                 this.disabledAnotherAgent = true;
        //             } else {
        //                 this.disabledAnotherAgent = false;
        //             }

        //             this.isMore = true;
        //             this.isMore2 = true;
        //             this.showPageLoader = false;

        //             this.getTime(this.UsersChannels);
        //             this.countDown(null);
        //         } else {
        //             this.showPageLoader = false;
        //         }

        //         if (
        //             this.filters.selectedCustomerID ||
        //             this.filters.selectedLiveChatID
        //         ) {
        //             this.selectUserToChat(result.result[0]);
        //             this.selectedIndex = 0;
        //         }
        //     },
        //     (error) => {
        //         this.showPageLoader = false;
        //     }
        // );
    }
    convertHijriToGregorian(hijriDateTimeString) {
        const [hijriDateString, time] = hijriDateTimeString.split("T");
        const [year, month, day] = hijriDateString.split("-").map(Number);
        const gregorianDate = toGregorian(year, month, day);

        // Format the date to YYYY-MM-DD
        const formattedDate = [
            gregorianDate.gy,
            String(gregorianDate.gm).padStart(2, "0"),
            String(gregorianDate.gd).padStart(2, "0"),
        ].join("-");

        return formattedDate + "T" + time;
    }

    //     getCustomers() {

    //         this.customersFilter.pageNumber = 0;
    //         this.teamService.getCustomer(this.customersFilter).subscribe(
    //             (result: any) => {
    //                 this.UsersChannels = result.result;
    //                 var colors = ['#4972bc  ', '#b45f69 ', '#7dc7b9 ', '#ec7a1d ', '#85b4b4'];
    //                 var images = ['avatar1', 'avatar2', 'avatar3', 'avatar4', 'avatar5'];

    //                 this.UsersChannels.forEach(item => {
    //                     item.color = colors[Math.floor(Math.random() * colors.length)];
    //                     item.avatarUrl = images[Math.floor(Math.random() * images.length)];
    //                 });
    //                 if (this.UsersChannels.length > 0) {

    //                     this.UsersChannels[0].isSelected = true;
    //                     this.selectedUser = this.UsersChannels[0];
    //                     this.showPageLoader = false;
    //                     this.pageNumber = 0;
    //                     this.setUpdateModelFromOrignalData();
    //                     this.loadMessages();
    //                     this.getTime(this.UsersChannels);

    //                 }
    //             },
    //             (error) => {
    //                 this.showPageLoader = false;
    //             }
    //         );
    //     }

    // hasArabicCodepoints(message) {
    //     if (message.text != null) {
    //         let firstCharacter = message.text.charAt(0);
    //         if (/[\u0600-\u06FF]/.test(firstCharacter)) {
    //             message.isArabic = true;
    //         } else if ((/^[A-Za-z0-9]*$/.test(firstCharacter))) {
    //             message.isArabic = false;
    //         }
    //     }
    // }

    detectLanguage(event) {
        if (event.target.value != null) {
            if (/[\u0600-\u06FF]/.test(event.target.value.charAt(0))) {
                this.isArabic = true;
            } else if (/^[A-Za-z0-9]*$/.test(event.target.value.charAt(0))) {
                this.isArabic = false;
            }
        }
    }

    loadMessages() {
        this.showMessageLoader = true;

        this.teamService
            .getChannelMessage(
                this.selectedUser.userId,
                this.pageSize,
                this.pageNumber
            )
            .subscribe(
                (res: any) => {
                    this.firstRender = true;
                    this.UserMessage = res.result;
                    // this.searchMessages();
                    this.UserMessage.forEach((message) => {
                        if (message.text != null) {
                            if (message.text.includes("*")) {
                                message.text = message.text.replace(/\*/g, "");
                            }
                        }
                        if (this.isLanguageArabic) {
                            message.createDate = new Date(
                                this.convertHijriToGregorian(message.createDate)
                            );
                        }
                    });

                    if (this.selectedUser.searchId == 1) {
                        setTimeout(() => {
                            this.scrollChat.nativeElement.scroll({
                                top: this.scrollChat.nativeElement.scrollHeight,
                            });
                        }, 300);
                    }
                    this.showMessageLoader = false;

                    if (this.ifFirstTime) {
                        this.ifFirstTime = false;
                    }
                },
                (error) => {
                    this.showPageLoader = false;
                }
            );
    }

    onSelect(event) {
        if (this.uploadButton) {
            this.files = event.target.files;
        } else {
            this.files = event;
        }
        this.uploadFile();
        event.target.files = null;
        // this.getImgData(this.files[0]);
    }

    onRemove(index) {
        let data = [];
        for (let i = 0; i < this.files.length; i++) {
            if (i !== index) {
                data.push(this.files[i]);
            }
        }
        this.files = data;
        this.file.nativeElement.value = "";
        // this.files.splice(i, 1);
        this.uploadFile();
    }

    uploadFile() {
        this.uploadButton = false;

        // this.chatForm.reset();
        let files = this.files;
        for (let file of files) {
            this.chatForm.patchValue({
                formFile: files,
            });
            this.hideUploadFile = true;
            //const fillTextarea = document.getElementById('ChatMessage') as HTMLInputElement;
            // fillTextarea.value += '\n' + file.name;
        }
        // form.clear();
        // this.uplodeFileOnclick = false;
        this.chatForm.get("formFile").updateValueAndValidity();
    }

    loadMoreChat() {
        // this.showMessageLoader = true;
        this.pageNumber = this.pageNumber + 1;
        this.teamService
            .getChannelMessage(
                this.selectedUser.userId,
                this.pageSize,
                this.pageNumber
            )
            .subscribe(
                (res: any) => {
                    // this.showMessageLoader = false;
                    this.PeforH = this.scrollChat.nativeElement.scrollHeight;
                    this.PeforT = this.scrollChat.nativeElement.scrollTop;

                    this.UserMessage = res.result.concat(this.UserMessage);
                    this.UserMessage.forEach((message) => {
                        if (message.text != null) {
                            if (message.text.includes("*")) {
                                message.text = message.text.replace(/\*/g, "");
                            }
                        }
                    });
                    // this.UserMessage.forEach(message => {
                    //     this.hasArabicCodepoints(message);
                    // });

                    var h = this.scrollChat.nativeElement.scrollHeight;
                    var t = this.scrollChat.nativeElement.scrollTop;

                    this.scrollChat.nativeElement.scrollTop = 1;

                    this.isMore = res.result.length > 0;
                },
                (error) => {
                    // this.showMessageLoader = false;
                }
            );
    }
    loadMoreContact() {
        //this.showMessageLoader = true;
        this.customersFilter.pageNumber = this.pageNumberC;
        this.customersFilter.pageSize = this.pageSizeC;
        this.customersFilter.searchTerm = "";
        this.teamService.getCustomer(this.customersFilter).subscribe(
            (result: any) => {
                result.result.forEach((element) => {
                    const isfoiu = this.UsersChannels.some(
                        (item) => item.phoneNumber === element.phoneNumber
                    );
                    var colors = [
                        "#4972bc  ",
                        "#b45f69 ",
                        "#7dc7b9 ",
                        "#ec7a1d ",
                        "#85b4b4",
                    ];
                    var images = [
                        "avatar3",
                        "avatar3",
                        "avatar3",
                        "avatar3",
                        "avatar3",
                    ];

                    element.color =
                        colors[Math.floor(Math.random() * colors.length)];

                // if(element.channel =="facebook" && element.avatarUrl!=''){
                //             //this.UsersChannels[index].avatarUrl = data.avatarUrl;
                // }else{
                //          element.avatarUrl =images[Math.floor(Math.random() * images.length)];

                //     }
              

                    element.fullCretaedDate = element.createDate;

                    if (!isfoiu) {
                        this.UsersChannels.push(element);
                    }
                });
                this.sortedArrayOfCustomerMessages();
                if (this.UsersChannels.length > 0) {
                    if (
                        this.selectedUser.isLockedByAgent &&
                        this.appSession.user.id === this.selectedUser.agentId &&
                        this.selectedUser.isOpen
                    ) {
                        this.disabledAnotherAgent = true;
                    } else {
                        this.disabledAnotherAgent = false;
                    }

                    this.getTime(this.UsersChannels);
                    this.countDown(null);
                } else {
                    this.showPageLoader = false;
                }

                this.isMore2 = result.result.length > 0;
                // this.scrollChatt.nativeElement.scrollTop = this.scrollChatt.nativeElement.scrollHeight;//this.prvHeight;
                this.isOneTime = true;
            },
            (error) => {
                // this.showPageLoader = false;
            }
        );
    }


    async submitForm() { 
        this.selectMessageToReply = false;
        let formData: any = new FormData();
        this.showInputBox = false;
        this.uplodeFileOnclick = false;
        // this.chatForm.reset();
        this.oneClick = true;
    
        // Handling file and text input
        if (this._recordRTC.recordingFile) {
            this.chatForm.controls["formFile"].setValue(
                this._recordRTC.recordingFile
            );
            this.chatForm.get("formFile").updateValueAndValidity();
        }
    
        if (!this.chatForm.get("formFile").value) {
            const userId = this.selectedUser.userId;
            let text = this.newMessage;
            this.newMessage = "";
            // this.chatForm.reset();
            this.showPageLoader = false;
            if (text) {
                this.disabledkeyboard = true;
                this.postMessageObj = {};
                this.postMessageObj.userId = userId;
                this.postMessageObj.text = text;
                this.postMessageObj.agentName = this.appSession.user.userName;
                this.postMessageObj.agentId = this.appSession.userId.toString();

                let typeM="text";
                if(this.isNoteMode){
                  typeM="note";
                }else{
                   typeM="text";
                }
                this.postMessageObj.type = typeM;
                this.postMessageObj.to = this.selectedUser.phoneNumber;
                if (this.filters?.selectedLiveChatID) {
                    this.postMessageObj.selectedLiveChatID =
                        this.filters.selectedLiveChatID;
                } else {
                    this.postMessageObj.selectedLiveChatID = 0;
                }
                this.chatForm.reset();
                this.teamService.postMessageD360(this.postMessageObj).subscribe(
                    (response) => {

                        if(this.postMessageObj.type=="note"){
                           this.selectedUser.numberNote=5;
                        }
                        


                        this.showPageLoader = false;
                        this.chatForm.reset();
                        this.file.nativeElement.value = "";
                        this.files = [];
                        this.showInputBox = false;
                        this.selectedUser.lastMessageText = text;
                        this.oneClick = false;
                        this.newMessage = "";
                        text = "";
                        setTimeout(() => {
                            this.scrollChat.nativeElement.scroll({
                                top: this.scrollChat.nativeElement.scrollHeight,
                                left: 0,
                                behavior: "smooth",
                            });
                        }, 100);
                        this.disabledkeyboard = false;
                        this.onFocus();
                    },
                    (error) => {
                        this.showPageLoader = false;
                        this.disabledkeyboard = false;
                        this.onFocus();
                    }
                );
            }
        } else {
            this.disabledkeyboard = true;
            let FormFile = this.chatForm.get("formFile").value;
            this.files = [];
            if (this.uploadButton || this._recordRTC?.blobUrl) {
                FormFile = [FormFile];
            }

            for (let i = 0; i < FormFile.length; i++) {
                let formDataFile = new FormData();
                formDataFile.set("to", this.selectedUser.phoneNumber);
                formDataFile.set("agentName", this.appSession.user.userName);
                // @ts-ignore
                formDataFile.set("agentId", this.appSession.userId);
                formDataFile.set("Text", this.chatForm.get("text").value || "");
                formDataFile.set(
                    "altText",
                    this.chatForm.get("formFile").value.name || ""
                );
                 // @ts-ignore

                formDataFile.append("formFile", FormFile[i]);
    
                // Compress file if large
                // if (FormFile[i].size > 50 * 1024 * 1024) { // If file is larger than 50MB
                //     const zip = new JSZip();
                //     zip.file(FormFile[i].name, FormFile[i]);
    
                //     const compressedBlob = await zip.generateAsync({ type: 'blob', compression: 'DEFLATE' });
    
                //     formDataFile.set('formFile', compressedBlob, FormFile[i].name + '.zip');
                // }
    
                this.teamService.postD360Attachment(formDataFile).subscribe(
                    (response) => {
                        this.uploadButton = false;
                        this.pageNumber = 0;
                        this.chatForm.reset();
                        this._recordRTC.clearRecordedData();
                        this._recordRTC.recordingFile = "";
                        this.files = [];
                        this.cd.detectChanges();
                        this.newMessage = "";
                        if (document.getElementById("ChatMessage")) {
                            // @ts-ignore
                            document.getElementById("ChatMessage").value = "";
                            document.getElementById(
                                "ChatMessage"
                            ).style.display = "block";
                        }
                        setTimeout(() => {
                            this.scrollChat.nativeElement.scroll({
                                top: this.scrollChat.nativeElement.scrollHeight,
                                left: 0,
                                behavior: "smooth",
                            });
                        }, 100);
                        this.disabledkeyboard = false;
                        this.onFocus();
                        // this.loadMoreChat();

                    },
                    (error) => {
                        this._recordRTC.clearRecordedData();
                        this.disabledkeyboard = false;
                        this.onFocus();
                    }
                );
            }
            this.uploadButton = false;
            this.pageNumber = 0;
            this.files = [];
            this.chatForm.reset();
            this._recordRTC.clearRecordedData();
            this._recordRTC.recordingFile = "";
            this.cd.detectChanges();
            this.loadMessages();
            this.file.nativeElement.value = "";
        }
    }

    addEmoji(event) {
        this.newMessage += event.emoji.native;
        // this.hideEmoji = true;
    }

    showEmoji() {
        this.hideEmoji = !this.hideEmoji;
    }

    showFileUpload() {
        this.hideEmoji = true;
        const fileInput = document.getElementById("upload-file") as HTMLInputElement;
        fileInput.onchange = (event: Event) => {
            const input = event.target as HTMLInputElement;
    
            if (input?.files && input.files.length > 0) {
                const file = input.files[0];
    
                const fileSizeInMB = file.size / (1024 * 1024); 
                const fileType = file.type; 
    
                if (fileType.startsWith("image/") && fileSizeInMB > 5) {
                    Swal.fire({
                        icon: "error",
                        title: "Image Too Large",
                        text: "The uploaded image exceeds 5 MB. Please select a smaller image.",
                        confirmButtonText: "OK",
                    }).then(() => {
                        input.value = ""; 
                    });
                } else if (fileType.startsWith("video/") && fileSizeInMB > 16) {
                    Swal.fire({
                        icon: "error",
                        title: "Video Too Large",
                        text: "The uploaded video exceeds 16 MB. Please select a smaller video.",
                        confirmButtonText: "OK",
                    }).then(() => {
                        input.value = ""; 
                    });
                } else if (fileSizeInMB > 100) {
                    Swal.fire({
                        icon: "error",
                        title: "File Too Large",
                        text: "The uploaded document exceeds 100 MB. Please select a smaller file.",
                        confirmButtonText: "OK",
                    }).then(() => {
                        input.value = ""; 
                    });
                }
            }
        };
    
        fileInput.click(); 
        this.uploadButton = true;
        this.hideUploadFile = !this.hideUploadFile;
    }
    
    searchForCustomerClicked() {
        this.isMore = true;
        this.pageNumber = 0;
        if (this.customersFilter.searchTerm) {
            this.getChannels();
        } else {
            this.customersFilter.searchTerm = "";
            this.getChannels();
        }
    }

    searchMessages() {
        this.scrollToFirstMatchingMessage();
    }

    //   @ViewChild('scrollChat') chatContainer: ElementRef;

    scrollToFirstMatchingMessage() {
        const chatContainerElement = this.scrollChat?.nativeElement;
        const firstMatchingMessage: any = this.UserMessage.find(
            (message) => message.id == this.selectedMessageId
        );
        if (firstMatchingMessage) {
            const messageId = `message-${firstMatchingMessage.id}`;

            const messageElement = chatContainerElement.querySelector(
                `#${messageId}`
            );
            if (messageElement) {
                const topOffset = messageElement.offsetTop;
                chatContainerElement.scroll({
                    top: topOffset - 20,
                    behavior: "smooth",
                });
            }
        }
    }

    // selectUser(userId: string) {
    //     this.UsersChannels;
    //     setTimeout(() => {
    //         let filteredChannels = this.UsersChannels.filter((channel: Channel) => channel.userId === userId);
    //         if (filteredChannels.length > 0) { 
    //             this.selectUserToChat(filteredChannels[0]); 
    //         }
    //     },5000); 
    // }
    
    selectUserToChat(newSelected: Channel) {

        this.isNoteMode=false;
        this.newMessage="";
        newSelected;
        this.showLoaderOnOpenClose = false;
        this.selectMessageToReply = false;
        this.hideEmoji = true;
        this.isMore = true;
        this.pageNumber = 0;
        this.countdown = 0;
        this._recordRTC.clearRecordedData();
        this.files = [];
        this.selectedUser.id = newSelected?.id;
        this.chatType = newSelected?.searchId;
        if (newSelected?.searchId !== 1) {
            this.selectedMessageId = newSelected?.listCustomerChat[0].id;
        }
        // this.selectedUser.expiration_timestamp = newSelected.expiration_timestamp;
        //this.selectedUser.creation_timestamp = newSelected.creation_timestamp;
        this.isSelectedUser = true;
        this.isUserSelectedChat = true;
        setTimeout(() => {
            this.scrollChat.nativeElement.scroll({
                top: this.scrollChat.nativeElement.scrollHeight,
            });
        }, 300);
        let showChat = document.getElementById("showChat");
        showChat.classList.remove("show");

        this.UsersChannels.forEach((user) => {
            if (user.userId !== newSelected.userId) {
                user.isSelected = false;
                this.isSelectedUser = false;
                this.UserMessage = [];
            } else {
                user.isSelected = true;
                this.isSelectedUser = true;
            }
        });
        if (
            newSelected.isLockedByAgent &&
            this.appSession.user.id === newSelected.agentId &&
            newSelected.isOpen
        ) {
            // newSelected.isOpen = true;

            this.disabledAnotherAgent = true;
        } else {
            this.disabledAnotherAgent = false;
        }

        this.selectedUser = newSelected;
        let year = new Date(this.selectedUser.fullCretaedDate).getFullYear();
        if (year < 2000) {
            this.selectedUser.fullCretaedDate = new Date(
                this.convertHijriToGregorian(this.selectedUser.fullCretaedDate)
            );
        }
        this.pageNumber = 0;

        if (this.newCustomerFilter.searchId == 1) {
            this.loadMessages();
        } else {
            if (this.selectedUser.searchId == 2) this.loadAllMessages(10000, 0);
            else {
                this.loadMessages();
            }
        }

        this.countDown(null);
    }
selectUserToNote(newSelected: CustomerChat) {
    //  setTimeout(() => {
    //         this.scrollChat.nativeElement.scroll({
    //             top: this.scrollChat.nativeElement.scrollHeight,
    //         });
    //     }, 300);
        //        let showChat = document.getElementById("showChat");
        // showChat.classList.remove("show");



}
    loadAllMessages(pageSize, pageNumber) {
        this.showMessageLoader = true;

        this.teamService
            .getChannelMessage(this.selectedUser.userId, pageSize, pageNumber)
            .subscribe(
                (res: any) => {
                    this.firstRender = true;
                    this.UserMessage = res.result;
                    // this.searchMessages();
                    this.UserMessage.forEach((message) => {
                        if (message.text != null) {
                            if (message.text.includes("*")) {
                                message.text = message.text.replace(/\*/g, "");
                            }
                        }
                        if (this.isLanguageArabic) {
                            message.createDate = new Date(
                                this.convertHijriToGregorian(message.createDate)
                            );
                        }
                    });
                    if (this.isUserSelectedChat) {
                        setTimeout(() => {
                            this.searchMessages();
                        }, 300);
                    }
                    this.showMessageLoader = false;

                    if (this.ifFirstTime) {
                        this.ifFirstTime = false;
                    }
                },
                (error) => {
                    this.showPageLoader = false;
                }
            );
    }

    countDown(data) {
        // let diff;
        // if (data) {
        //     diff = Math.abs(
        //         new Date().getTime() -
        //         new Date(data.lastConversationStartDateTime).getTime()
        //     ) / 3600000;
        // } else {
        //     diff = Math.abs(
        //         new Date().getTime() -
        //         new Date(this.selectedUser.lastConversationStartDateTime).getTime()
        //     ) / 3600000;
        // }

        // diff = diff - 3600 / 259200000;
        this.countdown = 0;
        const current = new Date();
        const timestamp = current.getTime();
        if (data) {
            let alltime = data.expiration_timestamp - timestamp;
            let curTimeAndExpirTime = Math.ceil(
                data.expiration_timestamp - timestamp / 1000
            );
            // let remainingTime = alltime - curTimeAndExpirTime;

            if (curTimeAndExpirTime > 0) {
                if (alltime <= 86400) {
                    this.remainingTime = {
                        stopTime: data.expiration_timestamp,
                        leftTime: curTimeAndExpirTime,
                        format: "HH:mm:ss",
                    }; //86400 => 24 houer
                    this.countdown = curTimeAndExpirTime;
                } else {
                    this.remainingTime = {
                        stopTime: data.expiration_timestamp,
                        leftTime: curTimeAndExpirTime,
                        format: " ( d ) :HH:mm:ss",
                    }; //259200 => 72 houer
                    this.countdown = curTimeAndExpirTime;
                }
            } else {
                this.remainingTime = { leftTime: 0 };
                this.countdown = 0;
            }
        } else {
            let alltime = this.selectedUser.expiration_timestamp - timestamp;
            let curTimeAndExpirTime = Math.ceil(
                this.selectedUser.expiration_timestamp - timestamp / 1000
            );
            // let remainingTime = alltime - curTimeAndExpirTime;
            if (curTimeAndExpirTime > 0) {
                if (alltime <= 86500) {
                    this.remainingTime = {
                        stopTime: this.selectedUser.expiration_timestamp,
                        curTimeAndExpirTime,
                        format: "HH:mm:ss",
                    }; //86400 => 24 houer
                    this.countdown = curTimeAndExpirTime;
                } else {
                    this.remainingTime = {
                        stopTime: this.selectedUser.expiration_timestamp,
                        curTimeAndExpirTime,
                        format: " ( d ) :HH:mm:ss",
                    }; //259200 => 72 houer
                    const countdownDurationInHours = 72; // Change this to the desired duration in hours
                    const countdownDurationInMilliseconds =
                        countdownDurationInHours * 60 * 60 * 1000;
                    this.countdown = curTimeAndExpirTime;
                }
            } else {
                this.remainingTime = { leftTime: 0 };
                this.countdown = 0;
            }

            // var dateExpiration=  new Date(this.selectedUser.expiration_timestamp*1000);
            // var diff =(dateExpiration.getTime() - current.getTime()) / 1000;
            // var ho= Math.abs(Math.round(diff));
        }

        if (this.remainingTime.leftTime > 0) {
            this.isOnline = true;
        } else {
            this.isOnline = false;
        }
        //this.isOneTime=true;
    }

    ngAfterViewInit() {
        setInterval(() => {

            this.countDown(null);
        }, 1000);
        this.assignTicketModal.submitClicked.subscribe(() => {
            this.close();
          });
    }

    replyToMessage(message: ChannelMessage): void {
        this.selectMessageToReply = true;
        this.messageToReply = message;
    }

    closeReply() {
        this.selectMessageToReply = false;
    }

    open() {
        this.showLoaderOnOpenClose = true;
        if (this.openChat) {
        } else {
            this.filters.selectedLiveChatID = 0;
        }
        this.selectMessageToReply = false;
        if (
            this.filters.selectedLiveChatID == null ||
            this.filters.selectedLiveChatID == 0
        ) {
            this.filters.selectedLiveChatID = 0;
        }

        // if(this.appSession.tenantId==59){
        //     this.filters.selectedLiveChatID=0;
        // }
        this.selectedUser.isSupport = false;
        this.selectedUser.lockedByAgentName = this.appSession.user.userName;
        this.selectedUser.unreadMessagesCount = 0;
        this.teamService
            .lockCustomer(
                this.selectedUser.userId,
                this.appSession.user.userName,
                this.appSession.user.id,
                this.filters.selectedLiveChatID
            )
            .subscribe(
                (response) => {
                    if (response) {
                        this.selectedUser.isOpen = true;
                        this.selectedUser.isLockedByAgent = true;
                        this.showLoaderOnOpenClose = false;
                    }
                },
                (error: any) => {
                    if (error) {
                        this.showLoaderOnOpenClose = false;
                    }
                }
            );
    }

    updatenone() {

        this.teamService
            .updatenote(
                this.selectedUser.userId,
                this.appSession.user.userName,
                this.appSession.user.id,
                this.isNoteMode
            )
            .subscribe(
                (response) => {
          




                },
                (error: any) => {
                    if (error) {
                       
                    }
                }
            );
    }


    updateCustomer() {
        this.showPageLoader = true;

        if (
            this.userUpdateModel.displayName &&
            this.userUpdateModel.phoneNumber
        ) {
            this.teamService.updateCustomer(this.userUpdateModel).subscribe(
                (res) => {
                    this.showPageLoader = false;
                },
                (error) => {
                    this.showPageLoader = false;
                }
            );
        }
    }

    setUpdateModelFromOrignalData() {
        this.userUpdateModel.userId = this.selectedUser.userId;
        this.userUpdateModel.phoneNumber =
            this.selectedUser?.phoneNumber?.toString();
        this.userUpdateModel.displayName =
            this.selectedUser?.displayName?.toString();
        this.userUpdateModel.emailAddress =
            this.selectedUser?.emailAddress?.toString();
        this.userUpdateModel.website = this.selectedUser?.website?.toString();
        this.userUpdateModel.description =
            this.selectedUser?.description?.toString();
    }

    close() {
        this.showLoaderOnOpenClose = true;
        if (
            this.filters.selectedLiveChatID == null ||
            this.filters.selectedLiveChatID == 0
        ) {
            this.filters.selectedLiveChatID = 0;
        }
        this.selectedUser.lockedByAgentName = this.appSession.user.userName;
        this.teamService
            .updateCustomerStatus(
                this.selectedUser.userId,
                false,
                this.selectedUser.lockedByAgentName,
                this.filters.selectedLiveChatID
            )
            .subscribe((res) => {
                if (res) {
                    this.selectedUser.isOpen = false;
                    this.selectedUser.isLockedByAgent = false;
                    this.showLoaderOnOpenClose = false;
                }
            }),
            (error: any) => {
                if (error) {
                    this.showLoaderOnOpenClose = false;
                }
            };
    }

    array = [];
    sum = 100;
    throttle1 = 300;
    scrollDistance1 = 1;
    scrollUpDistance1 = 2;
    direction = "";
    modalOpen = false;

    showAssignToModal(): void {
        this.assignToModalLocal.show();
        this.close();
    }

    showAssignUserModal(): void {
        this.assignUsers.showInbox(this.filters.selectedLiveChatID);
        //this.close();
    }

    // getUsers() {
    //     this.teamService.getUsers().subscribe((result: any) => {
    //         this.agentsList = result.result.items;
    //     });
    // }

    // return here
    assigned(event?: any) {
        this.assigneeAgentName = event.userName + " " + event.surname;
        this.teamService
            .assignTo(
                this.selectedUser.userId,
                this.assigneeAgentName,
                event.id
            )
            .subscribe((res: any) => {
                // this.selectedUser.isOpen= res.result.isOpen
                this.assignToModalLocal.close();
            });
            
            // this.notificationService.loadNotifications();        


    }

    assignTicketTo(event?: any) {
        this.assigneeAgentName = event.userName + " " + event.surname;
        this.teamService
            .assignTo(
                this.selectedUser.userId,
                this.assigneeAgentName,
                event.id
            )
            .subscribe((res: any) => {
                // this.selectedUser.isOpen= res.result.isOpen
                this.assignToModalLocal.close();
            });
    }

    // startVoiceRecord(message) {

    //     this._recordRTC.toggleRecord(message);
    //     message.style.display = 'none';

    // }

    // clearRecord(message) {
    //     this._recordRTC.clearRecordedData();
    //     message.style.display = 'block';
    //     message.value = '';
    //     this._recordRTC.recordingFile = '';
    // }

    unblockUser(contactID: string) {
        this._contactsServiceProxy
            .blockContact(+contactID, false, this.appSession.user.userName)
            .subscribe((res) => {});
    }
    showDialog() {
        if (this.groups.length === 0) {
            this.message.warn(this.l("thereisNoGroups"), this.l("groupInfo"));
            return;
        }
        if (this.selectedUser.groupId) {
            this.tempGroups = this.groups.filter(
                (group) => group.id != this.selectedUser.groupId
            );
        }
        this.visible = true;
    }

    handleCloseDialog() {
        this.visible = false;
    }

    blockUser(user: Channel) {
        this.message.confirm(
            this.selectedUser.isBlock
                ? this.l("areyousureyouwanttounblock")
                : this.l("areyousureyouwanttoblock"),
            this.l("block"),
            (confirm) => {
                if (confirm) {
                    this._contactsServiceProxy
                        .blockContact(
                            +user.contactID,
                            !this.selectedUser.isBlock,
                            this.appSession.user.userName
                        )
                        .subscribe((res) => {
                            this.notify.success(this.l("savedSuccessfully"));
                        });

                    this.selectedUser.isBlock = !this.selectedUser.isBlock;
                }
            }
        );
    }

    updateCustomerInfo(selectedUser) {
        this._contactsServiceProxy
            .getContactbyId(Number(selectedUser.contactID))
            .subscribe((res) => {
                this.contact = res;
                (this.contact.displayName = selectedUser.displayName),
                    // this.contact.emailAddress= selectedUser.emailAddress,
                    (this.contact.description = selectedUser.description),
                    this._contactsServiceProxy
                        .createOrEdit(this.contact)
                        .subscribe(() => {
                            this.notify.success(this.l("savedSuccessfully"));
                        });
            });
    }

    getTime(users) {
        for (let user of users) {
            const today = new Date();
            const yesterday = new Date(today);
            yesterday.setDate(yesterday.getDate() - 1);
            if (this.isLanguageArabic) {
                let year = moment(user.lastMessageData).year();
                if (year < 2000) {
                    user.lastMessageData = this.convertHijriToGregorian(
                        user.lastMessageData
                    );
                }
            }
            if (
                new Date(user.lastMessageData).toDateString() ===
                today.toDateString()
            ) {
                const time = new Date(user.lastMessageData).toString();
                if (this.isLanguageArabic) {
                    // user.createDate = moment.utc(this.convertHijriToGregorian(user.createDate)).format('mm:hh a');
                }
                // else{
                user.createDate = moment
                    .utc(time)
                    .locale("en")
                    .format("hh:mm a"); //Today
                // }
            } else if (
                new Date(user.lastMessageData).toDateString() ===
                yesterday.toDateString()
            ) {
                if (this.isLanguageArabic) {
                    // user.createDate =this.convertHijriToGregorian(user.createDate);
                    user.createDate = "قبل يوم"; // Yesterday
                } else {
                    user.createDate = "Yesterday"; // Yesterday
                }
            } else {
                const date = new Date(user.lastMessageData).toString();
                user.createDate = moment(date)
                    .locale("en")
                    .format("DD/MM/YYYY"); //In the past
                // user.createDate =this.convertHijriToGregorian(user.createDate);
            }
        }
    }

    isHijriLeapYear(year: number): boolean {
        return (year * 11 + 14) % 30 < 11;
    }

    async downloadFromURL(url, filename) {
        FileSaver.saveAs(url, filename);
    }

    // chatBoxKeyup($event: KeyboardEvent) {
    //     if ($event.keyCode === 13) {
    //         $event.preventDefault();
    //         $event.stopPropagation();
    //         this.submitForm();
    //     }
    // }
    chatBoxKeyup($event: KeyboardEvent) {
        if ($event.key === 'Enter') {
            if ($event.shiftKey) {
                const textarea = $event.target as HTMLTextAreaElement;
                const cursorPosition = textarea.selectionStart;
                const textBefore = textarea.value.substring(0, cursorPosition);
                const textAfter = textarea.value.substring(cursorPosition);
                textarea.value = `${textBefore}\n${textAfter}`;
                textarea.selectionStart = textarea.selectionEnd = cursorPosition + 1; 
                $event.preventDefault(); 
            } else  if ($event.keyCode === 13) {
                $event.preventDefault();
                $event.stopPropagation();
                this.submitForm();
            }
        }
    }

    getTemplateMessageText(id) {
        this.titleMessage = "";
        this.templateMessagesServiceProxy
            .getTemplateMessageForEdit(id)
            .subscribe((res) => {
                this.titleMessage = res.templateMessage.messageText;
            });
    }

    async stopRecord() {
        this._recordRTC.clickStopRTC().then((res) => {});
    }
    onScrollDown() {
        //this.nextPage = this.currentPage * this.itemsPerPage ;
        this.currentPage++;
        this.appendData();
    }
    onUp() {}

    onScroll() {
        if (this.scrollChat.nativeElement.scrollTop === 0 && this.isMore) {
            // this.pageNumber =this.pageNumber+ 1;
            if (this.chatType == 1) this.loadMoreChat();
        }
    }

    onScroll2() {
        let scroll = window.pageYOffset;
        if (scroll > this.currentPosition) {
        } else {
        }
        this.currentPosition = scroll;

        // var toppp=this.scrollChatt.nativeElement.scrollTop;

        var d = document.getElementById("DivcontactList");
        // var b =  document.getElementById("contactList");

        // var offset = d.scrollTop + window.innerHeight;
        var height = d.offsetHeight;

        // var offsetb = b.scrollTop + window.innerHeight;
        // var heightb = b.offsetHeight;

        if (this.prvHeight > height) {
        } else {
            var h = this.scrollChatt.nativeElement.scrollHeight - 30; //-20;//-height;
            var t = this.scrollChatt.nativeElement.scrollTop + height;
            if (t >= h && this.isOneTime) {
                this.isOneTime = false;
                this.prvHeight = height;

                this.topnumber = this.scrollChatt.nativeElement.scrollTop;
                this.heiget = this.scrollChatt.nativeElement.scrollHeight;
                this.pageNumberC = this.pageNumberC + 1;
                this.loadMoreContact();
            }
        }
    }

    goToLastMassage() {
        setTimeout(() => {
            this.scrollChat.nativeElement.scroll({
                top: this.scrollChat.nativeElement.scrollHeight + 20,
                left: 0,
                behavior: "smooth",
            });
        });
    }

    updateIsComplaintSwitch(e): void {
        if (this.selectedUser.isComplaint) {
            this.selectedUser.isComplaint = false;
        } else {
            this.selectedUser.isComplaint = true;
        }
        this.teamInboxServiceProxy
            .updateComplaint(
                this.selectedUser.userId,
                this.selectedUser.isComplaint,
                this.appSession.user.userName
            )
            .subscribe((res) => {});
    }
openprofileNote() {

if(this.profileNote){
    this.profileNote=false;
}else{
    this.profileNote=true;
}

this.GetNoteChat();
}
    openProfile() {
            this.profileOpend = true;

    }

    closeProfile() {
        if(this.profileNote=true){
            this.profileNote=false;
            // this.onSearchEnter(null)
        }

        this.profileOpend = false;
        this.profileNote=false;

  
    }

    setIndex(index: number) {
        this.selectedIndex = index;
    }
    showChat() {
        let showChat = document.getElementById("showChat");

        if (this.showChatt) {
            this.showChatt = false;
            showChat.classList.remove("show");
        } else {
            this.showChatt = true;
            showChat.classList.add("show");
        }
    }
    trackByFunction(index, channel) {
        return channel.id; // or another unique property of your items
    }
    trackByFunctionMessages(index, message) {
        return message.messageId;
    }
    clearRecord(message) {
        this._recordRTC.clearRecordedData();
        message.style.display = "block";
        message.value = "";
        this._recordRTC.recordingFile = "";
    }
    startVoiceRecord(message) {
        this._recordRTC.toggleRecord(message);
        message.style.display = "none";
    }

    async convertToMp3(file: File): Promise<void> {
        try {
            const reader = new FileReader();
            reader.onload = async (e: any) => {
                const data = new Uint8Array(e.target.result);
                const ffmpeg = FFmpeg.createFFmpeg({ log: true });
                await ffmpeg.load();

                ffmpeg.FS("writeFile", file.name, data);

                await ffmpeg.run("-i", file.name, "-b:a", "192k", "output.mp3");

                const mp3Data = ffmpeg.FS("readFile", "output.mp3");
                const mp3Blob = new Blob([mp3Data.buffer], {
                    type: "audio/mpeg",
                });

                this.downloadBlob(mp3Blob, "converted.mp3");
            };
            reader.readAsArrayBuffer(file);
        } catch (e) {
            console.error("Error");
        }
    }

    downloadBlob(blob: Blob, filename: string) {
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = filename;
        a.click();
        URL.revokeObjectURL(url);
    }

    handleTemplateList(list): void {
        this.template = list;
        // console.log('List received from child:', list);
    }

    isUrl(input: string): boolean {
        return this.urlPattern.test(input);
    }
    isButtonNotBot(): boolean {
        if (
            !this.selectedUser.isConversationExpired &&
            this.selectedUser.isOpen &&
            !this.showLoaderOnOpenClose &&
            this.selectedUser.lockedByAgentName == this.appSession.user.userName
        ) {
            return false;
        } else if (this.selectedUser.isConversationExpired) {
            return false;
        } else if (
            !this.selectedUser.isConversationExpired &&
            !this.selectedUser.isOpen &&
            !this.showLoaderOnOpenClose
        ) {
            return false;
        } else {
            return true;
        }
    }

    messageButtons(inputString: string, type: string) {
        this.tempButtonsList = [];
        if (type == "text") {
            let tempMessageText = "";

            // Define the regex pattern to match "{number}- text" format
            const pattern = /^\d+-\s*(.+)$/;

            // Split the inputString by newline characters to handle each line separately
            const lines = inputString.split(/\r?\n/);

            // Iterate over each line
            for (const line of lines) {
                const match = line.match(pattern); // Use match instead of exec
                if (match) {
                    // If the line matches the pattern, add the captured text to tempButtonsList
                    this.tempButtonsList.push(match[1].trim());
                } else if (line.trim().length > 0) {
                    // If the line does not match the pattern, add it to tempMessageText
                    tempMessageText += (tempMessageText ? "\n" : "") + line;
                }
            }

            // Store tempMessageText in this class property or handle it as needed
            this.tempMessageText = tempMessageText;

            return this.tempButtonsList;
        } else {
            return [];
        }
    }

    getClassBasedOnMessageCount(): string {
        const count = this.tempButtonsList.length;
        if (count === 1) {
            return "single-message";
        } else if (count === 2) {
            return "two-messages";
        } else {
            return "more-than-two-messages";
        }
    }

    returnBackToNotifications(){
        this.router.navigate(
            ["/app/notifications"]);
    }



    GetNoteChat(){

        this.teamService.GetNoteChat(this.selectedUser.userId,10000000,0)
            .subscribe((result:any) => {
                  this.ListNote=result.result;;
             
                //    this.ListNote.forEach((element) => {
                                
                //          debugger

                //             });

             
               
        });  

    }

}
