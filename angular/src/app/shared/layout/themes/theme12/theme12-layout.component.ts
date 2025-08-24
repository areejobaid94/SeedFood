import { ThemeServicesService } from './../../../../../shared/shared-services/theme-services.service';
import {
    Injector,
    Component,
    OnInit,
    AfterViewInit,
    Input,
    NgZone,
    HostListener,
    Inject,
} from "@angular/core";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { ThemesLayoutBaseComponent } from "@app/shared/layout/themes/themes-layout-base.component";
import { UrlHelper } from "@shared/helpers/UrlHelper";
import { AppConsts } from "@shared/AppConsts";
import { ToggleOptions } from "@metronic/app/core/_base/layout/directives/toggle.directive";
import { OffcanvasOptions } from "@metronic/app/core/_base/layout/directives/offcanvas.directive";
import { AppSessionService } from "@shared/common/session/app-session.service";
import {
    LinkedUserDto,
    ProfileServiceProxy,
    UserLinkServiceProxy,
} from "@shared/service-proxies/service-proxies";

import * as Feather from "feather-icons";
import { ImpersonationService } from "@app/admin/users/impersonation.service";
import { AppAuthService } from "@app/shared/common/auth/app-auth.service";
import { AbpMultiTenancyService, AbpSessionService, PermissionCheckerService } from "abp-ng2-module";
import { LinkedAccountService } from "../../linked-account.service";
import { ChangeUserLanguageDto } from "@shared/service-proxies/service-proxies";
import { DarkModeService } from "../../../../services/dark-mode.service";
import * as _ from "lodash";
import {
    NotificationServiceProxy,
    UserNotification,
} from "@shared/service-proxies/service-proxies";
import {
    IFormattedUserNotification,
    UserNotificationHelper,
} from "../../notifications/UserNotificationHelper";

import { Howl } from "howler";
import { Router, RouterOutlet } from "@angular/router";
import { FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { ToastrService } from "ngx-toastr";
import { NotificationsService } from "../../../../services/notifications.service";
import * as rtlDetect from "rtl-detect";
import { SharedService } from "@shared/shared-services/shared.service";
import { UserServiceService } from '../../notifications/UserService.service';
import { Subscription } from '@node_modules/rxjs';
import { SocketioService } from '@app/shared/socketio/socketioservice';
declare const FB: any;
declare function MenuTest2(): any;
@Component({
    templateUrl: "./theme12-layout.component.html",
    selector: "theme12-layout",
    styleUrls: ['./theme12-layout.component.css']

})
export class Theme12LayoutComponent
    extends ThemesLayoutBaseComponent
    implements OnInit, AfterViewInit
{
    form: FormGroup;
    searchCtrl = new FormControl();
    searchOpened = false;
    windowScrolled: boolean = false;
    topOffset: number = 150; // Top offset to display scroll to top button
    userMenuToggleOptions: ToggleOptions = {
        target: "kt_aside",
        targetState: "aside-on",
        toggleState: "active",
    };
    anotherAgentMessgeSub: Subscription;
    public currentSkin: string = "default";
    menuOpened = false;
    currentYear: number;
    collapsedMenu = false;
    isDark = false;
    @Input() iconOnly = false;

    @Input() togglerCssClass =
        "btn btn-icon w-auto btn-clean d-flex align-items-center btn-lg px-2";
    @Input() textCssClass =
        "text-dark-50 font-weight-bolder font-size-base d-none d-md-inline mr-3";
    @Input() symbolCssClass = "symbol symbol-35 symbol-light-success";
    @Input() symbolTextCssClass = "symbol-label font-size-h5 font-weight-bold";

    usernameFirstLetter = "";
    public animate;

    profilePicture =
        AppConsts.appBaseUrl +
        "/assets/common/images/default-profile-picture.png";
    shownLoginName = "";
    tenancyName = "";
    userName = "";
    phoneNumber: string;
    languages: abp.localization.ILanguageInfo[];
    currentLanguage: abp.localization.ILanguageInfo;
    appSession: AppSessionService;
    recentlyLinkedUsers: LinkedUserDto[];
    isImpersonatedLogin = false;
    isMultiTenancyEnabled = false;
    customized_side_bar_opened = false;
    notifications: IFormattedUserNotification[] = [];
    unreadNotificationCount = 0;
    @Input() isDropup = false;
    @Input() customStyle = "btn btn-icon btn-dropdown btn-clean btn-lg mr-1";
    ishasPermissionToBooking = false;
    public showFacebookModal = false;
    public facebookAuthUrl = '';
    
    toastButtons = [
        {
            id: "1",
            title: "Pay Now",
        },
        {
            id: "2",
            title: "Cancel",
        },
    ];
    offcanvasOptions: OffcanvasOptions = {
        overlay: true,
        baseClass: "offcanvas",
        placement: "right",
        closeBy: "kt_demo_panel_close",
        toggleBy: "kt_quick_user_toggle",
    };
    isArabic = false;
    isInBotBuilderPage = false;

    // private sharedService : SharedService =  Inject(SharedService);


    public constructor(
        injector: Injector,
        private _linkedAccountService: LinkedAccountService,
        private themeServicesService : ThemeServicesService,
        public darkModeService: DarkModeService,
        private _abpMultiTenancyService: AbpMultiTenancyService,
        private _profileServiceProxy: ProfileServiceProxy,
        private _userLinkServiceProxy: UserLinkServiceProxy,
        private _authService: AppAuthService,
        private router: Router,
        private _impersonationService: ImpersonationService,
        private _abpSessionService: AbpSessionService,
        private _notificationService: NotificationServiceProxy,
        private _userNotificationHelper: UserNotificationHelper,
        private _formBuilder: FormBuilder,
        private toastr: ToastrService,
        public notificationService: NotificationsService,
        private _permissionCheckerService: PermissionCheckerService,
        private userService: UserServiceService,
           private socketioService: SocketioService,
        public _zone: NgZone
    ) {
        super(injector);
        this.appSession = injector.get(AppSessionService);
    }

    ngDoCheck() {
      
        try {
            this.isInBotBuilderPage= window.location.href.includes("botBuilder");
        } catch {}
    }

    @HostListener("window:scroll", [])
    onWindowScroll() {
        if (
            window.pageYOffset > this.topOffset ||
            document.documentElement.scrollTop > this.topOffset ||
            document.body.scrollTop > this.topOffset
        ) {
            this.windowScrolled = true;
        } else if (
            (this.windowScrolled && window.pageYOffset) ||
            document.documentElement.scrollTop ||
            document.body.scrollTop < 10
        ) {
            this.windowScrolled = false;
        }
    }

    ngOnInit(): void {
        this.loadFacebookChatPlugin(); // auto-load if needed
        this.subscribeTenantMessage();
        // this.checkPayment();
        this.onWindowScroll();
        this.isArabic = rtlDetect.isRtlLang(
            abp.localization.currentLanguage.name
        );
        this.ishasPermissionToBooking = this._permissionCheckerService.isGranted("Pages.Booking");
        this.darkModeService.checkNavColor();
        this.darkModeService.checkNavStyle();
        this.currentYear = new Date().getFullYear();

        this.isImpersonatedLogin =
            this._abpSessionService.impersonatorUserId > 0;
        this.isMultiTenancyEnabled = this._abpMultiTenancyService.isEnabled;
        this.setCurrentLoginInformations();
        this.getProfilePicture();
        this.getRecentlyLinkedUsers();
        this.registerToEvents();
        this.phoneNumber = this.appSession.tenant.phoneNumber;
        this.usernameFirstLetter = this.appSession.user.userName
            .substring(0, 1)
            .toUpperCase();
        this.languages = _.filter(
            this.localization.languages,
            (l) => l.isDisabled === false
        );

        this.currentLanguage = this.localization.currentLanguage;
     


        this.notificationService.loadNotifications();
        this.registerToEvents();
        this.form = this._formBuilder.group({
            app: this._formBuilder.group({
                appName: new FormControl(),
                appTitle: new FormControl(),
                appLogoImage: new FormControl(),
                appLanguage: new FormControl(),
            }),
            layout: this._formBuilder.group({
                skin: new FormControl(),
                type: new FormControl(),
                animation: new FormControl(),
                menu: this._formBuilder.group({
                    hidden: new FormControl(),
                    collapsed: new FormControl(),
                }),
                navbar: this._formBuilder.group({
                    hidden: new FormControl(),
                    type: new FormControl(),
                    background: new FormControl(),
                    customBackgroundColor: new FormControl(),
                    backgroundColor: new FormControl(),
                }),
                footer: this._formBuilder.group({
                    hidden: new FormControl(),
                    type: new FormControl(),
                    background: new FormControl(),
                    customBackgroundColor: new FormControl(),
                    backgroundColor: new FormControl(),
                }),
                enableLocalStorage: new FormControl(),
                customizer: new FormControl(),
                scrollTop: new FormControl(),
                buyNow: new FormControl(),
            }),
        });
      
    }
    subscribeTenantMessage = () => {
        //chat get // bot answer // open // close //  admin answer
        this.anotherAgentMessgeSub =
            this.socketioService.agentMessage.subscribe((data) => {
                if(this.appSession.user.id.toString()==data.agentId && data.customerChat.notificationsText.includes("Assign To")){
                    this. notificationService.unreadNotificationCount++;
                    this. notificationService.loadNotifications();
                }
            });
    };

    checkPayment() {
        if (!this.appSession.tenant.isPaidInvoice) {
            this.toastr
                .error(
                    "you have an unpaid bill, pay it now to avoid disconnecting the service",
                    "WARNING!",
                    {
                        tapToDismiss: false,
                        positionClass: "toast-top-center",
                        timeOut: 0,
                        extendedTimeOut: 0,
                    }
                )
                .onTap.subscribe(() => this.goToBillings());
        } else if (this.appSession.tenant.isCaution) {
            this.toastr
                .warning(
                    "You have an unpaid bill, pay it before disconnecting the service.",
                    "CAUTION!",
                    {
                        tapToDismiss: false,
                        positionClass: "toast-top-center",
                        timeOut: 0,
                        extendedTimeOut: 0,
                        // @ts-ignore
                        onclick: function () {
                            this.goToBillings();
                        },
                    }
                )
                .onTap.subscribe(() => this.goToBillings());
        }
    }

    goToBillings() {
        this.router.navigate(["/app/main/billings/billings"]);
    }

    prepareRoute(outlet: RouterOutlet) {
        return outlet.activatedRouteData.animation;
    }
    setCurrentLoginInformations(): void {
        this.shownLoginName = this.appSession.getShownLoginName();
        this.tenancyName = this.appSession.tenancyName;
        this.userName = this.appSession.user.userName;
    }

    getShownUserName(linkedUser: LinkedUserDto): string {
        if (!this._abpMultiTenancyService.isEnabled) {
            return linkedUser.username;
        }

        return (
            (linkedUser.tenantId ? linkedUser.tenancyName : ".") +
            "\\" +
            linkedUser.username
        );
    }

    getProfilePicture(): void {
        this._profileServiceProxy.getProfilePicture().subscribe((result) => {
            if (result && result.profilePicture) {
                this.profilePicture =
                    "data:image/jpeg;base64," + result.profilePicture;
            }
        });
    }

    getRecentlyLinkedUsers(): void {
        this._userLinkServiceProxy
            .getRecentlyUsedLinkedUsers()
            .subscribe((result) => {
                this.recentlyLinkedUsers = result.items;
            });
    }

    showLoginAttempts(): void {
        abp.event.trigger("app.show.loginAttemptsModal");
    }

    showLinkedAccounts(): void {
        abp.event.trigger("app.show.linkedAccountsModal");
    }

    showUserDelegations(): void {
        abp.event.trigger("app.show.userDelegationsModal");
    }

    changePassword(): void {
        abp.event.trigger("app.show.changePasswordModal");
    }

    changeProfilePicture(): void {
        abp.event.trigger("app.show.changeProfilePictureModal");
    }

    changeMySettings(): void {
        abp.event.trigger("app.show.mySettingsModal");
    }

    logout(): void {

        localStorage.removeItem('MessagingPortal/abpzerotemplate_local_storage/enc_auth_token');
        const keysToKeep = ['savedPassword', 'savedUsername', 'tenancyName'];

      Object.keys(localStorage).forEach((key) => {
        if (!keysToKeep.includes(key)) {
            localStorage.removeItem(key);
              }
         });

      //  localStorage.clear();
        this._authService.logout();
    }

    onMySettingsModalSaved(): void {
        this.shownLoginName = this.appSession.getShownLoginName();
    }

    backToMyAccount(): void {
        this._impersonationService.backToImpersonator();
    }

    switchToLinkedUser(linkedUser: LinkedUserDto): void {
        this._linkedAccountService.switchToAccount(
            linkedUser.id,
            linkedUser.tenantId
        );
    }

    downloadCollectedData(): void {
        this._profileServiceProxy.prepareCollectedData().subscribe(() => {
            this.message.success(this.l("GdprDataPrepareStartedNotification"));
        });
    }

    ngAfterViewInit(): void {
        Feather.replace();
    }
    changeLanguage(languageName: string): void {
        const input = new ChangeUserLanguageDto();
        input.languageName = languageName;
        this._profileServiceProxy.changeLanguage(input).subscribe(() => {
            abp.utils.setCookieValue(
                "Abp.Localization.CultureName",
                languageName,
                new Date(new Date().getTime() + 5 * 365 * 86400000), //5 year
                abp.appPath
            );
            // MenuTest2();
            window.location.reload();
        });
    }



    // loadNotifications(): void {
    //     if (UrlHelper.isInstallUrl(location.href)) {
    //         return;
    //     }

    //     this._notificationService
    //         .getUserNotifications(undefined, undefined, undefined, 3, 0)
    //         .subscribe((result) => {
    //             this.unreadNotificationCount = result.unreadCount;
    //             this.notifications = [];
    //             _.forEach(result.items, (item: UserNotification) => {
    //                 //
    //                 //  item.notification.creationTime.hour()-7;
    //                 //
    //                 this.notifications.push(
    //                     this._userNotificationHelper.format(<any>item)
    //                 );
    //             });
    //         });
    // }

    registerToEvents() {
        let self = this;

        function onNotificationReceived(userNotification) {
            self._userNotificationHelper.show(userNotification);
            self.notificationService.loadNotifications();
        }

        abp.event.on("abp.notifications.received", (userNotification) => {
            if (this.appSession.user.id == userNotification.userId) {
                var sound = new Howl({
                    src: ["../assets/common/sound/Bell.mp3"],
                });

                sound.play();
            }

            self._zone.run(() => {
                onNotificationReceived(userNotification);
            });
        });

        function onNotificationsRefresh() {
            self.notificationService.loadNotifications();
        }

        abp.event.on("app.notifications.refresh", () => {
            self._zone.run(() => {
                onNotificationsRefresh();
            });
        });

        function onNotificationsRead(userNotificationId) {
            for (let i = 0; i < self.notifications.length; i++) {
                if (
                    self.notifications[i].userNotificationId ===
                    userNotificationId
                ) {
                    self.notifications[i].state = "READ";
                }
            }

            self.unreadNotificationCount -= 1;
        }

        abp.event.on("app.notifications.read", (userNotificationId) => {
            self._zone.run(() => {
                onNotificationsRead(userNotificationId);
            });
        });
    }

    setAllNotificationsAsRead(): void {
        this._userNotificationHelper.setAllAsRead();
    }

    openNotificationSettingsModal(): void {
        this._userNotificationHelper.openSettingsModal();
    }

    setNotificationAsRead(userNotification: IFormattedUserNotification): void {
        this._userNotificationHelper.setAsRead(
            userNotification.userNotificationId
        );
    }
    isRead(record: any): boolean {
        return record.state === 'READ';
      
    }
    gotoUrl(url,userId): void {
        if (url) {
            location.href = url;
        }
        setTimeout(() => {
            this.notificationService.loadNotifications(); 
        }, 100);
        if(userId){
            const CustomerID=userId;
            this.router.navigate(
                ["/app/main/teamInbox/teamInbox12"],
                {
                    queryParams: { CustomerID },
                }
            );
            // this.userService.selectUser(userId);
            // this.router.navigate(['app/main/teamInbox/teamInbox12']);
        }
        
    }
        getNumberAfterComma(text: string): string {
            if (!text.includes(',')) return ''; 
            return text.split(',')[1].trim(); 
        }
        
    scrollToTop() {
        window.scroll({
            top: 0,
            left: 0,
            behavior: "smooth",
        });
    }

    toggleDarkSkin() {
        this.darkModeService.toggleDarkSkin();
        const mode  = (localStorage.getItem('mode'));
        if(mode === 'default'){
            this.themeServicesService.switchTheme('lara-light-blue');
        }
        if(mode === 'dark'){
            this.themeServicesService.switchTheme('lara-dark-blue');
        }
    }

    toggleSideBar() {
        if (!this.customized_side_bar_opened) {
            const customizer = document.getElementById("customizer-toggle");
            customizer.classList.add("open");
            this.customized_side_bar_opened = true;
            this.collapsedMenu = this.darkModeService.hideSideNav;
        } else {
            const customizer = document.getElementById("customizer-toggle");
            customizer.classList.remove("open");
            this.customized_side_bar_opened = false;
        }
    }
    toggleSideNav(): void {
        this.darkModeService.navMenuToggled();
        this.collapsedMenu = this.darkModeService.hideSideNav;
    }
    closeMenu() {
        this.darkModeService.toglleMenu = "open";
        this.darkModeService.toggleNavMenu();
    }
    save() {}
    openSearch() {
        if (this.searchOpened) {
            this.searchOpened = false;
            this.searchCtrl = null;
        } else {
            this.searchOpened = true;
        }
    }

    search() {
        const value = this.searchCtrl.value as string;
        if (value) {
            if (
                value.toLowerCase().indexOf("messages") > -1 ||
                value.toLowerCase().indexOf("teaminbox") > -1 ||
                value.toLowerCase().indexOf("chat") > -1 ||
                value.toLowerCase().indexOf("الرسائل") > -1 ||
                value.toLowerCase().indexOf("شات") > -1
            ) {
                this.router.navigate(["/app/main/teamInbox/teamInbox12"]);
            } else if (
                value.toLowerCase().indexOf("dashboard") > -1 ||
                value.toLowerCase().indexOf("analysis") > -1 ||
                value.toLowerCase().indexOf("operations") > -1
            ) {
                this.router.navigate(["/app/main/dashboard"]);
            } else if (value.toLowerCase().indexOf("livechat") > -1) {
                this.router.navigate(["/app/main/liveChat"]);
            } else if (value.toLowerCase().indexOf("contact") > -1) {
                this.router.navigate(["/app/main/contacts/contacts"]);
            } else if (value.toLowerCase().indexOf("external") > -1) {
                this.router.navigate(["/app/main/externalContacts"]);
            } else if (value.toLowerCase().indexOf("order") > -1) {
                this.router.navigate(["/app/main/orders/orders"]);
            } else if (value.toLowerCase().indexOf("archived") > -1) {
                this.router.navigate(["/app/main/orders/ordersArchived"]);
            } else if (
                value.toLowerCase().indexOf("request") > -1 ||
                value.toLowerCase().indexOf("selling") > -1
            ) {
                this.router.navigate(["/app/main/sellingRequest"]);
            } else if (
                value.toLowerCase().indexOf("delivery") > -1 ||
                value.toLowerCase().indexOf("cost") > -1
            ) {
                this.router.navigate(["/app/main/deliveryCost"]);
            } else if (value.toLowerCase().indexOf("assets") > -1) {
                this.router.navigate(["/app/main/assets"]);
            } else if (value.toLowerCase().indexOf("template") > -1) {
                this.router.navigate(["/app/main/messageTemplate"]);
            } else if (value.toLowerCase().indexOf("campaign") > -1) {
                this.router.navigate(["/app/main/messageCampaign"]);
            } else if (value.toLowerCase().indexOf("location") > -1) {
                this.router.navigate(["/app/main/location"]);
            } else if (
                value.toLowerCase().indexOf("menu") > -1 ||
                value.toLowerCase().indexOf("item") > -1 ||
                value.toLowerCase().indexOf("category") > -1 ||
                value.toLowerCase().indexOf("subcategory") > -1
            ) {
                this.router.navigate(["/app/main/menus/menus"]);
            } else if (value.toLowerCase().indexOf("evaluation") > -1) {
                this.router.navigate(["/app/main/evaluation"]);
            } else if (value.toLowerCase().indexOf("branch") > -1) {
                this.router.navigate(["/app/main/areaLocation/areaLocation"]);
            } else if (
                value.toLowerCase().indexOf("user") > -1 ||
                value.toLowerCase().indexOf("permission") > -1
            ) {
                this.router.navigate(["/app/admin/users"]);
            } else if (value.toLowerCase().indexOf("setting") > -1) {
                this.router.navigate(["/app/admin/tenantSettings"]);
            } else if (
                value.toLowerCase().indexOf("visual") > -1 ||
                value.toLowerCase().indexOf("setting") > -1
            ) {
                this.router.navigate(["/app/admin/ui-customization"]);
            } else {
                this.message.warn("No Result Found");
            }
        }
    }

    copyPhoneNumber() {
        const textToCopy = this.phoneNumber;
    
        if (navigator.clipboard) {
          navigator.clipboard.writeText(textToCopy).then(() => {
          }).catch(err => {
          });
        } else {
          const textarea = document.createElement('textarea');
          textarea.value = textToCopy;
          document.body.appendChild(textarea);
          textarea.select();
          try {
            document.execCommand('copy');
            alert('Phone number copied: ' + textToCopy);
          } catch (err) {
          }
          document.body.removeChild(textarea);
        }
      }

      loadFacebookChatPlugin() {
        if ((window as any).FB) {
          (window as any).FB.XFBML.parse();
          // Optionally open the chat immediately:
          (window as any).FB.CustomerChat.showDialog?.();
          return;
        }
      
        // Define fbAsyncInit BEFORE loading the SDK
        (window as any).fbAsyncInit = () => {
          FB.init({
            xfbml: true,
            version: 'v18.0'
          });
      
          // Optional: open the chat automatically after loading
          FB.CustomerChat.showDialog?.();
        };
      
        // Inject fb-root if not present
        if (!document.getElementById('fb-root')) {
          const fbRoot = document.createElement('div');
          fbRoot.id = 'fb-root';
          document.body.appendChild(fbRoot);
        }
      
        // Inject fb-customer-chat if not present
        if (!document.getElementById('fb-customer-chat')) {
          const chatDiv = document.createElement('div');
          chatDiv.className = 'fb-customerchat';
          chatDiv.id = 'fb-customer-chat';
          chatDiv.setAttribute('page_id', '885586280068397'); // ✅ YOUR page ID
          chatDiv.setAttribute('attribution', 'biz_inbox');
          document.body.appendChild(chatDiv);
        }
      
        // Load Facebook SDK
        if (!document.getElementById('facebook-jssdk')) {
          const script = document.createElement('script');
          script.id = 'facebook-jssdk';
          script.src = 'https://connect.facebook.net/en_US/sdk/xfbml.customerchat.js';
          document.body.appendChild(script);
        }
      }


      connectFacebookPopup() {
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
    //   connectInstagram() {
    //     const appId = '907410431131999';
    //     const redirectUri = AppConsts.appBaseUrl + '/app/admin/instagram-connect'; // example: https://teaminbox-stg.azurewebsites.net/app/admin/instagram-connect
    //     const scope = [
    //       'instagram_business_basic',
    //       'instagram_business_manage_messages',
    //       'instagram_business_manage_comments',
    //       'instagram_business_content_publish',
    //       'instagram_business_manage_insights'
    //     ].join(',');
      
    //     const url = `https://www.instagram.com/oauth/authorize?enable_fb_login=0&force_authentication=1&client_id=${appId}&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=code&scope=${encodeURIComponent(scope)}`;
      
    //     window.location.href = url;
    //   }

     connectInstagram() {

        const appId = '907410431131999';
        const redirectUri = AppConsts.appBaseUrl + '/app/admin/instagram-connect'; // example: https://teaminbox-stg.azurewebsites.net/app/admin/instagram-connect
        const url = `https://www.instagram.com/consent/?flow=ig_biz_login_oauth&params_json=%7B%22client_id%22%3A%22907410431131999%22%2C%22redirect_uri%22%3A%22${encodeURIComponent(redirectUri)}%22%2C%22response_type%22%3A%22code%22%2C%22state%22%3A%22%7B%5C%22f3_request_id%5C%22%3A%5C%2288a03917-a06d-48ba-92ea-2876c79e6417%5C%22%2C%5C%22ig_app_id%5C%22%3A%5C%22907410431131999%5C%22%2C%5C%22nonce%5C%22%3A%5C%22mwLjTA6sES9snubu%5C%22%7D%22%2C%22scope%22%3A%22business_basic-business_manage_messages%22%2C%22logger_id%22%3A%2288a03917-a06d-48ba-92ea-2876c79e6417%22%2C%22app_id%22%3A%22907410431131999%22%2C%22platform_app_id%22%3A%22907410431131999%22%7D&source=oauth_permissions_page_www`;
        const url2 =`https://www.instagram.com/oauth/authorize?enable_fb_login=0&force_authentication=1&client_id=907410431131999&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=code&scope=instagram_business_basic%2Cinstagram_business_manage_messages%2Cinstagram_business_manage_comments%2Cinstagram_business_content_publish%2Cinstagram_business_manage_insights`
       debugger
        window.location.href = url2;
      }

      
      generateUUID(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            const r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
          });
      }
}
