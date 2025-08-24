import { DarkModeService } from "./../../../../services/dark-mode.service";
import { PermissionCheckerService } from "abp-ng2-module";
import {
    Injector,
    ElementRef,
    Component,
    ViewEncapsulation,
    Renderer2,
    ViewChild,
    Input,
    HostListener,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { AppMenu } from "../app-menu";
import { AppNavigationService } from "../app-navigation.service";
import { NavigationEnd, Router } from "@angular/router";
import { take, takeUntil, filter } from "rxjs/operators";
import { MenuOptions } from "@metronic/app/core/_base/layout/directives/menu.directive";
import { FormattedStringValueExtracter } from "@shared/helpers/FormattedStringValueExtracter";
import * as objectPath from "object-path";
import { AppConsts } from "@shared/AppConsts";
import {
    CustomerLiveChatModel,
    GetOrderForViewDto,
    InvoicesModel,
    ProfileServiceProxy,
    SellingRequestDto,
} from "@shared/service-proxies/service-proxies";
import { Subject, Subscription } from "rxjs";
import { Howl } from "howler";
import { TeamInboxService } from "@app/main/teamInbox/teaminbox.service";
import { PerfectScrollbarDirective } from "ngx-perfect-scrollbar";
import { SocketioService } from "@app/shared/socketio/socketioservice";
import { result } from "lodash";
import { NotificationsService } from "../../../../services/notifications.service";
import { ToastrService } from "ngx-toastr";
import { WebNotificationsService } from "../../../../services/web-notifications.service";
declare function MenuTest2(): any;

@Component({
    selector: "sidebarthem12",
    templateUrl: "./sidebarthem12.component.html",
    styleUrls: ["./sidebarthem12.component.css"],
    encapsulation: ViewEncapsulation.None,
})
export class Sidebarthem12Component extends AppComponentBase {
    @Input() isDark: boolean;
    subNavOpened = false;
    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;
    profilePicture =
        AppConsts.appBaseUrl +
        "/assets/common/images/default-profile-picture.png";
    tenancyName = "";
    coreConfig: any;
    isCollapsed: boolean;
    isScrolled: boolean = false;
    expanded: boolean = true;
    element: any;
    menu: AppMenu = null;
    audio: any;
    currentRouteUrl = "";
    insideTm: any;
    outsideTm: any;
    hideSideNav: boolean = false;
    OrderCount = 0;
    EvaluationCount = 0;
    MaintenancesCount = 0;

    LiveChatCount = 0;
    SellingRequestCount = 0;
    bookingCount = 0;
    menuOptions: MenuOptions = {
        submenu: {
            desktop: {
                default: "dropdown",
            },
            tablet: "accordion",
            mobile: "accordion",
        },

        accordion: {
            expandAll: false,
        },
    };
    botOrderSub: Subscription;
    botevaluationSub: Subscription;
    bookingSub: Subscription;

    sound: any; // Private
    private _unsubscribeAll: Subject<any>;

    /**
     * Constructor
     *
     * @param {CoreMenuService} _coreMenuService
     * @param {CoreSidebarService} _coreSidebarService
     * @param {Router} _router
     * */
    constructor(
        private _router: Router,
        public darkModeService: DarkModeService,
        injector: Injector,
        private _profileServiceProxy: ProfileServiceProxy,
        private teamService: TeamInboxService,
        private el: ElementRef,
        private router: Router,
        //  public permission: PermissionCheckerService,
        private _appNavigationService: AppNavigationService,
        private render: Renderer2,
        private socketioService: SocketioService,
        private toastr: ToastrService,
        private _permissionCheckerService: PermissionCheckerService,
        public notificationsService: NotificationsService,
        public webNotifications: WebNotificationsService
    ) {
        super(injector);
        this._unsubscribeAll = new Subject();
        this.webNotifications.requestPermission();
        // Howler.mute(true);
        // this.webNotifications.receiveMessages();
    }
    @ViewChild(PerfectScrollbarDirective, { static: false })
    directiveRef?: PerfectScrollbarDirective;
    ngDoCheck() {
        try {
            this.element = document.getElementById("chatPage");
            if (this.element) {
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                    this.SellingRequestCount = this.SellingRequestCount - 1;
                }
            }
        } catch {}
        //    try {
        //     this.element = document.getElementById("SellingRequestDetails");
        //     if (this.element.ariaHidden == "false") {
        //         if (this.sound != null) {
        //             this.sound.stop();
        //             this.sound.unload();
        //             this.sound = null;
        //             this.SellingRequestCount = this.SellingRequestCount - 1;
        //         }
        //     }
        // } catch {}
        // try {
        //     this.element = document.getElementById("ViewRequestDetails");
        //     if (this.element) {
        //         if (this.sound != null) {
        //             this.sound.stop();
        //             this.sound.unload();
        //             this.sound = null;
        //             this.SellingRequestCount = this.SellingRequestCount - 1;
        //         }
        //     }
        // } catch {}
        try {
            this.element = document.getElementById("ViewRequestDetails");

            if (this.element.ariaHidden == "false") {
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                    this.LiveChatCount = this.LiveChatCount - 1;
                }
            }
        } catch {}

        try {
            this.element = document.getElementById("clickedLiveChat");
            this.element.addEventListener("click", (event: Event) => {
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                    this.LiveChatCount = this.LiveChatCount - 1;
                }
            });
        } catch {}
        try {
            this.element = document.getElementById("openLiveChat");
            this.element.addEventListener("click", (event: Event) => {
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                    this.LiveChatCount = this.LiveChatCount - 1;
                }
            });
        } catch {}
        try {
            this.element = document.getElementById("OrderModel");

            if (this.element.ariaHidden == "false") {
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                    this.OrderCount = this.OrderCount - 1;
                }
            }
        } catch {}
        try {
            this.element = document.getElementById("BookingModal");
            let className = this.element.className;
            let index = className.indexOf("show");
            if (index != -1) {
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                    this.bookingCount = this.bookingCount - 1;
                }
            }
        } catch {}
    }
    async ngOnInit() {
        this.StopSound();
        this.socketioService.setupSocketConnection();
        this.darkModeService.checkMode();
        await this.getIsAdmin();
        this.tenancyName = this.appSession.tenancyName;
        MenuTest2();
        window.focus();
        this.getProfilePicture();
        this.subscribeBotOrder();
        this.subscribeBotEvaluation();
        this.MaintenancesBotEvaluation();
        this.subscribeLiveChat();
        this.subscribeSellingRequest();
        this.subscribeBooking();
        this.subscribeInvoicesRequest();
        this.checkPayment();

        this.menu = this._appNavigationService.getMenu();
        this.currentRouteUrl = this.router.url.split(/[?#]/)[0];
        this.router.events
            .pipe(filter((event) => event instanceof NavigationEnd))
            .subscribe((event) => {
                this.menu.items.forEach((element) => {
                    this.currentRouteUrl = this.router.url.split(/[?#]/)[0];

                    if (this.currentRouteUrl.includes("teamInbox")) {
                        document.getElementById("bodyID").style.overflow =
                            "hidden";
                    } else {
                        document.getElementById("bodyID").style.overflow = "";
                    }

                    if (this.currentRouteUrl.includes("orders")) {
                        this.OrderCount = 0;
                        // element.isNewItem = false;
                    }

                    if (this.currentRouteUrl.includes("evaluation")) {
                        this.EvaluationCount = 0;
                        // element.isNewItem = false;
                    }

                    if (this.currentRouteUrl.includes("liveChat")) {
                        this.LiveChatCount = 0;
                        // element.isNewItem = false;
                    }
                    if (this.currentRouteUrl.includes("sellingRequest")) {
                        this.SellingRequestCount = 0;
                        // element.isNewItem = false;
                    }

                    if (this.currentRouteUrl.includes("calendar")) {
                        this.bookingCount = 0;
                        // element.isNewItem = false;
                    }
                });
            });
        // Close the menu on router NavigationEnd (Required for small screen to close the menu on select)
        this._router.events
            .pipe(
                filter((event) => event instanceof NavigationEnd),
                takeUntil(this._unsubscribeAll)
            )
            .subscribe();
    }
    subscribeLiveChat = () => {

        this.socketioService.liveChat.subscribe(
            (data: CustomerLiveChatModel) => {
                if (data.liveChatStatus === 1 || data.liveChatStatus === 6) {
                    if (data.tenantId == this.appSession.tenant.id) {
                        // let ishasPermissionToDepartment =
                        //     this._permissionCheckerService.isGranted(
                        //         "Pages.Department"
                        //     );

                        if (this.appSession.tenant.isBellOn) {
                            if (
                                data.userIds != null &&
                                data.userIds != undefined &&
                                data.userIds != ""
                            ) {
                                if (
                                    data.userIds.includes(
                                        this.appSession.user.id.toString()
                                    )
                                ) {
                                    if (data.liveChatStatus === 1) {
                                        this.webNotifications.showNotification(
                                            "New Ticket Arrivrd! 🚀",
                                            "You Have " +
                                                "" +
                                                data.liveChatStatusName +
                                                "Ticket Arrived From " +
                                                data.displayName,
                                            "open_url"
                                        );
                                    } else {
                                        this.webNotifications.showNotification(
                                            "New Ticket Has Been Assigned To You! 🚀",
                                            "You Have " +
                                                "" +
                                                "Assigned " +
                                                "Ticket Arrived From " +
                                                data.displayName,
                                            "open_url"
                                        );
                                        this.toastr.warning(
                                            "Ticket With Id: " + data.idLiveChat + " Has Been " + "" + "Assigned " + "To You" ,
                                            "New Ticket Has Been Assigned To You! 🚀",
                                            {
                                                positionClass: "toast-bottom-right",
                                                closeButton: true,
                                                tapToDismiss: false,
                                                timeOut: 0,
                                                extendedTimeOut: 0,
                                            }
                                        );
                                    }
                                    if (
                                        this.appSession.tenant.isBellContinues
                                    ) {
                                        if (this.sound) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }
                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            loop: true,
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    } else {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    }

                                    this.LiveChatCount = this.LiveChatCount + 1;
                                }
                            } else if (
                                data.departmentUserIds != null ||
                                data.departmentUserIds != undefined
                            ) {
                                if (
                                    data.departmentUserIds.includes(
                                        this.appSession.user.id.toString()
                                    )
                                ) {
                                    if (data.liveChatStatus === 1) {
                                        this.webNotifications.showNotification(
                                            "New Ticket Arrivrd! 🚀",
                                            "You Have " +
                                                "" +
                                                data.liveChatStatusName +
                                                "Ticket Arrived From " +
                                                data.displayName,
                                            "open_url"
                                        );
                                    }
                                    if (
                                        this.appSession.tenant.isBellContinues
                                    ) {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            loop: true,
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    } else {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    }
                                    this.LiveChatCount = this.LiveChatCount + 1;
                                }
                            } else {
                                // data.userIds === null
                                if (data.liveChatStatus === 1) {
                                    this.webNotifications.showNotification(
                                        "New Ticket Arrivrd! 🚀",
                                        "You Have " +
                                            "" +
                                            data.liveChatStatusName +
                                            "Ticket Arrived From " +
                                            data.displayName,
                                        "open_url"
                                    );
                                }
                                if (this.appSession.tenant.isBellContinues) {
                                    if (this.sound) {
                                        this.sound.stop();
                                        this.sound.unload();
                                        this.sound = null;
                                    }
                                    this.sound = new Howl({
                                        src: [this.appSession.tenant.bellSrc],
                                        loop: true,
                                        html5: true,
                                        volume: 1.0,
                                    });

                                    this.sound.play();
                                } else {
                                    if (this.sound != null) {
                                        this.sound.stop();
                                        this.sound.unload();
                                        this.sound = null;
                                    }

                                    this.sound = new Howl({
                                        src: [this.appSession.tenant.bellSrc],
                                        html5: true,
                                        volume: 1.0,
                                    });

                                    this.sound.play();
                                }

                                this.LiveChatCount = this.LiveChatCount + 1;
                            }
                        } else {
                            if (
                                data.userIds != null &&
                                data.userIds != undefined &&
                                data.userIds != ""
                            ) {
                                if (
                                    data.userIds.includes(
                                        this.appSession.user.id.toString()
                                    )
                                ) {
                                    if (data.liveChatStatus === 1) {
                                        this.webNotifications.showNotification(
                                            "New Ticket Arrivrd! 🚀",
                                            "You Have " +
                                                "" +
                                                data.liveChatStatusName +
                                                "Ticket Arrived From " +
                                                data.displayName,
                                            "open_url"
                                        );
                                    } else {
                                        this.webNotifications.showNotification(
                                            "New Ticket Has Been Assigned To You! 🚀",
                                            "You Have " +
                                                "" +
                                                "Assigned" +
                                                "Ticket Arrived From " +
                                                data.displayName,
                                            "open_url"
                                        );
                                        this.toastr.warning(
                                            "Ticket With Id: " + data.idLiveChat + " Has Been " + "" + "Assigned " + "To You" ,
                                            "New Ticket Has Been Assigned To You! 🚀",
                                            {
                                                positionClass: "toast-bottom-right",
                                                closeButton: true,
                                                tapToDismiss: false,
                                                timeOut: 0,
                                                extendedTimeOut: 0,
                                            }
                                        );
                                    }

                                    this.LiveChatCount = this.LiveChatCount + 1;
                                }
                            } else if (
                                data.departmentUserIds != null ||
                                data.departmentUserIds != undefined
                            ) {
                                if (
                                    data.departmentUserIds.includes(
                                        this.appSession.user.id.toString()
                                    )
                                ) {
                                    this.LiveChatCount = this.LiveChatCount + 1;
                                    if (data.liveChatStatus === 1) {
                                        this.webNotifications.showNotification(
                                            "New Ticket Arrivrd! 🚀",
                                            "You Have " +
                                                "" +
                                                data.liveChatStatusName +
                                                "Ticket Arrived From " +
                                                data.displayName,
                                            "open_url"
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        );
    };
    subscribeSellingRequest = () => {
        this.socketioService.sellingRequest.subscribe(
            (data: SellingRequestDto) => {
                if (data.sellingRequestStatus == 1) {
                    if (data.tenantId == this.appSession.tenant.id) {
                        if (this.appSession.tenant.isBellOn) {
                            if (
                                data.departmentUserIds != null ||
                                data.departmentUserIds != undefined
                            ) {
                                if (
                                    data.departmentUserIds.includes(
                                        this.appSession.user.id.toString()
                                    )
                                ) {
                                    if (
                                        this.appSession.tenant.isBellContinues
                                    ) {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            loop: true,
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    } else {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    }
                                    this.webNotifications.showNotification(
                                        "New Request Arrivrd! 🚀",
                                        "You Have " +
                                            "" +
                                            data.sellingRequestStatus +
                                            "Request Arrived From " +
                                            data.phoneNumber,
                                        "open_url"
                                    );
                                    this.SellingRequestCount =
                                        this.SellingRequestCount + 1;

                                    // }
                                }
                            } else if (
                                data.userIds != null &&
                                data.userIds != undefined &&
                                data.userIds != ""
                            ) {
                                if (
                                    data.userIds.includes(
                                        this.appSession.user.id.toString()
                                    )
                                ) {
                                    if (
                                        this.appSession.tenant.isBellContinues
                                    ) {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            loop: true,
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    } else {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }
                                        // Howler.mute(false);

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    }

                                    this.webNotifications.showNotification(
                                        "New Request Arrivrd! 🚀",
                                        "You Have " +
                                            "" +
                                            data.sellingRequestStatus +
                                            "Request Arrived From " +
                                            data.phoneNumber,
                                        "open_url"
                                    );
                                    this.SellingRequestCount =
                                        this.SellingRequestCount + 1;
                                }
                            }
                        } else {
                            if (
                                data.departmentUserIds != null ||
                                data.departmentUserIds != undefined
                            ) {
                                if (
                                    data.departmentUserIds.includes(
                                        this.appSession.user.id.toString()
                                    )
                                ) {
                                    this.SellingRequestCount =
                                        this.SellingRequestCount + 1;
                                    this.webNotifications.showNotification(
                                        "New Request Arrivrd! 🚀",
                                        "You Have " +
                                            "" +
                                            data.sellingRequestStatus +
                                            "Request Arrived From " +
                                            data.phoneNumber,
                                        "open_url"
                                    );

                                    // }
                                }
                            } else if (
                                data.userIds != null &&
                                data.userIds != undefined &&
                                data.userIds != ""
                            ) {
                                if (
                                    data.userIds.includes(
                                        this.appSession.user.id.toString()
                                    )
                                ) {
                                    this.webNotifications.showNotification(
                                        "New Request Arrivrd! 🚀",
                                        "You Have " +
                                            "" +
                                            data.sellingRequestStatus +
                                            "Request Arrived From " +
                                            data.phoneNumber,
                                        "open_url"
                                    );
                                    this.SellingRequestCount =
                                        this.SellingRequestCount + 1;
                                }
                            }
                        }
                    }
                }
            }
        );
    };
    subscribeBotOrder = () => {
        this.botOrderSub = this.socketioService.BotOrder.subscribe(
            (data: GetOrderForViewDto) => {
                if (this.appSession.tenantId === data.tenantId) {
                    if (
                        (data.orderStatusName == "Pending" ||
                            data.orderStatusName == "Pre_Order" ||
                            data.orderStatusName == "Canceled") &&
                        data.order.orderRemarks != "CancelByAgent"
                    ) {
                        if (this.appSession.tenantId == data.tenantId) {
                            let isher = window.location.href.includes("orders");
                            let ishasPermissionToOrder =
                                this._permissionCheckerService.isGranted(
                                    "Pages.Orders"
                                );
                            if (!data.isLockedByAgent) {
                                if (ishasPermissionToOrder) {
                                    if (data.isAssginToAllUser) {
                                        if (
                                            data.order.agentIds.includes(
                                                this.appSession.user.id.toString()
                                            )
                                        ) {
                                            this.webNotifications.showNotification(
                                                "New Order Arrived! 🚀",
                                                "You Have" +
                                                    "" +
                                                    data.orderStatusName +
                                                    "Order Arrived From " +
                                                    data.customerCustomerName,
                                                "open_url"
                                            );
                                            if (
                                                this.appSession.tenant.isBellOn
                                            ) {
                                                if (
                                                    this.appSession.tenant
                                                        .isBellContinues
                                                ) {
                                                    if (
                                                        data.order
                                                            .orderStatus == 6
                                                    ) {
                                                        if (
                                                            this.sound != null
                                                        ) {
                                                            this.sound.stop();
                                                            this.sound.unload();
                                                            this.sound = null;
                                                        }

                                                        this.sound = new Howl({
                                                            src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/cancel.mp3",

                                                            loop: true,
                                                            html5: true,
                                                            volume: 1.0,
                                                        });

                                                        this.sound.play();
                                                    } else {
                                                        if (
                                                            this.sound != null
                                                        ) {
                                                            this.sound.stop();
                                                            this.sound.unload();
                                                            this.sound = null;
                                                        }

                                                        this.sound = new Howl({
                                                            src: [
                                                                this.appSession
                                                                    .tenant
                                                                    .bellSrc,
                                                            ],
                                                            loop: true,
                                                            html5: true,
                                                            volume: 1.0,
                                                        });

                                                        this.sound.play();
                                                    }
                                                } else {
                                                    if (
                                                        data.order
                                                            .orderStatus == 6
                                                    ) {
                                                        if (
                                                            this.sound != null
                                                        ) {
                                                            this.sound.stop();
                                                            this.sound.unload();
                                                            this.sound = null;
                                                        }

                                                        this.sound = new Howl({
                                                            src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/cancel.mp3",
                                                            html5: true,
                                                            volume: 1.0,
                                                        });

                                                        this.sound.play();
                                                    } else {
                                                        if (
                                                            this.sound != null
                                                        ) {
                                                            this.sound.stop();
                                                            this.sound.unload();
                                                            this.sound = null;
                                                        }

                                                        this.sound = new Howl({
                                                            src: [
                                                                this.appSession
                                                                    .tenant
                                                                    .bellSrc,
                                                            ],
                                                            html5: true,
                                                            volume: 1.0,
                                                        });

                                                        this.sound.play();
                                                    }
                                                }
                                            }
                                            if (!isher) {
                                                this.OrderCount =
                                                    this.OrderCount + 1;
                                                // element.isNewItem = true;
                                            }
                                        }

                                        // if (!isher) {
                                        //     this.OrderCount = this.OrderCount + 1;
                                        //     // element.isNewItem = true;
                                        // }
                                    } else {
                                        let isher =
                                            window.location.href.includes(
                                                "orders"
                                            );

                                        //f(data.order.agentId!=0 && this.appSession.user.id==data.order.agentId)
                                        if (
                                            data.order.agentIds != null &&
                                            data.order.agentIds.includes(
                                                this.appSession.user.id.toString()
                                            )
                                        ) {
                                            this.webNotifications.showNotification(
                                                "New Order Arrived! 🚀",
                                                "You Have" +
                                                    data.orderStatusName +
                                                    "Order Arrived From " +
                                                    data.customerCustomerName,
                                                "open_url"
                                            );
                                            if (
                                                this.appSession.tenant.isBellOn
                                            ) {
                                                if (
                                                    this.appSession.tenant
                                                        .isBellContinues
                                                ) {
                                                    if (
                                                        data.order
                                                            .orderStatus == 6
                                                    ) {
                                                        if (
                                                            this.sound != null
                                                        ) {
                                                            this.sound.stop();
                                                            this.sound.unload();
                                                            this.sound = null;
                                                        }

                                                        this.sound = new Howl({
                                                            src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/cancel.mp3",
                                                            loop: true,
                                                            html5: true,
                                                            volume: 1.0,
                                                        });

                                                        this.sound.play();
                                                    } else {
                                                        if (
                                                            this.sound != null
                                                        ) {
                                                            this.sound.stop();
                                                            this.sound.unload();
                                                            this.sound = null;
                                                        }

                                                        this.sound = new Howl({
                                                            src: [
                                                                this.appSession
                                                                    .tenant
                                                                    .bellSrc,
                                                            ],
                                                            loop: true,
                                                            html5: true,
                                                            volume: 1.0,
                                                        });

                                                        this.sound.play();
                                                    }
                                                } else {
                                                    this.webNotifications.showNotification(
                                                        "New Order Arrived! 🚀",
                                                        "You Have" +
                                                            "" +
                                                            data.orderStatusName +
                                                            "Order Arrived From " +
                                                            data.customerCustomerName,
                                                        "open_url"
                                                    );
                                                    if (
                                                        data.order
                                                            .orderStatus == 6
                                                    ) {
                                                        if (
                                                            this.sound != null
                                                        ) {
                                                            this.sound.stop();
                                                            this.sound.unload();
                                                            this.sound = null;
                                                        }

                                                        this.sound = new Howl({
                                                            src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/cancel.mp3",
                                                            html5: true,
                                                            volume: 1.0,
                                                        });

                                                        this.sound.play();
                                                    } else {
                                                        if (
                                                            this.sound != null
                                                        ) {
                                                            this.sound.stop();
                                                            this.sound.unload();
                                                            this.sound = null;
                                                        }

                                                        this.sound = new Howl({
                                                            src: [
                                                                this.appSession
                                                                    .tenant
                                                                    .bellSrc,
                                                            ],

                                                            html5: true,
                                                            volume: 1.0,
                                                        });

                                                        this.sound.play();
                                                    }
                                                }
                                            }

                                            if (!isher) {
                                                this.OrderCount =
                                                    this.OrderCount + 1;
                                                // element.isNewItem = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        );
    };
    subscribeBotEvaluation = () => {
        
        
        this.botevaluationSub = this.socketioService.evaluationBot.subscribe(
            (data: any) => {
                if (data.tenantId === this.appSession.tenantId) {
                    let ishasPermissionToEvaluation =
                        this._permissionCheckerService.isGranted(
                            "Pages.Evaluation"
                        );
                    if (ishasPermissionToEvaluation) {
                        this.webNotifications.showNotification(
                            "New Evaluation Arrivrd! 🚀",
                            "You Have New Evaluation From" + data.contactName,
                            "open_url"
                        );
                        if (this.appSession.tenant.isBellOn) {
                            if (this.appSession.tenant.isBellContinues) {
                                if (this.sound != null) {
                                    this.sound.stop();
                                    this.sound.unload();
                                    this.sound = null;
                                }

                                this.sound = new Howl({
                                    src: [this.appSession.tenant.bellSrc],
                                    loop: true,
                                    html5: true,
                                    volume: 1.0,
                                });

                                this.sound.play();
                            } else {
                                if (this.sound != null) {
                                    this.sound.stop();
                                    this.sound.unload();
                                    this.sound = null;
                                }

                                this.sound = new Howl({
                                    src: [this.appSession.tenant.bellSrc],
                                    html5: true,
                                    volume: 1.0,
                                });

                                this.sound.play();
                            }
                            this.EvaluationCount = this.EvaluationCount + 1;
                        } else {
                            this.EvaluationCount = this.EvaluationCount + 1;
                            this.webNotifications.showNotification(
                                "New Evaluation Arrivrd! 🚀",
                                "You Have New Evaluation From" +
                                    data.contactName,
                                "open_url"
                            );
                        }
                    }
                }
            }
        );
    };

    subscribeBooking = () => {
        this.bookingSub = this.socketioService.Booking.subscribe(
            (data: any) => {
                if (data.tenantId == this.appSession.tenant.id) {
                    let ishasPermissionToBooking =
                        this._permissionCheckerService.isGranted(
                            "Pages.Booking"
                        );

                    if (ishasPermissionToBooking) {
                        if (this.isAdmin) {
                            if (
                                data.statusId === 1 ||
                                data.statusId === 4 ||
                                data.statusId === 3
                            ) {
                                if (this.appSession.tenant.isBellOn) {
                                    if (
                                        this.appSession.tenant.isBellContinues
                                    ) {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            loop: true,
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    } else {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: [
                                                this.appSession.tenant.bellSrc,
                                            ],
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    }
                                    this.bookingCount = this.bookingCount + 1;
                                } else {
                                    this.bookingCount = this.bookingCount + 1;
                                }
                                this.notificationsService.loadNotifications();
                            }
                        } else {
                            if (data.userId === this.appSession.userId) {
                                if (
                                    data.statusId === 1 ||
                                    data.statusId === 4 ||
                                    data.statusId === 3
                                ) {
                                    if (this.appSession.tenant.isBellOn) {
                                        if (
                                            this.appSession.tenant
                                                .isBellContinues
                                        ) {
                                            if (this.sound != null) {
                                                this.sound.stop();
                                                this.sound.unload();
                                                this.sound = null;
                                            }

                                            this.sound = new Howl({
                                                src: [
                                                    this.appSession.tenant
                                                        .bellSrc,
                                                ],
                                                loop: true,
                                                html5: true,
                                                volume: 1.0,
                                            });

                                            this.sound.play();
                                        } else {
                                            if (this.sound != null) {
                                                this.sound.stop();
                                                this.sound.unload();
                                                this.sound = null;
                                            }

                                            this.sound = new Howl({
                                                src: [
                                                    this.appSession.tenant
                                                        .bellSrc,
                                                ],
                                                html5: true,
                                                volume: 1.0,
                                            });

                                            this.sound.play();
                                        }
                                        this.bookingCount =
                                            this.bookingCount + 1;
                                    } else {
                                        this.bookingCount =
                                            this.bookingCount + 1;
                                    }
                                }
                                this.notificationsService.loadNotifications();
                            }
                        }
                    }
                }
            }
        );
    };

    MaintenancesBotEvaluation = () => {
        this.botevaluationSub = this.socketioService.maintenance.subscribe(
            (data: any) => {
                this.menu.items.forEach((element) => {
                    if (
                        element.name == "Maintenances" &&
                        data.tenantID === this.appSession.tenantId
                    ) {
                        let isher =
                            window.location.href.includes("Maintenances");
                        if (!isher) {
                            this.MaintenancesCount = this.MaintenancesCount + 1;
                            element.isNewItem = true;
                        }
                    }
                });
            }
        );
    };

    subscribeInvoicesRequest = () => {
        this.socketioService.Invoices.subscribe((data: InvoicesModel) => {
            if (data.tenantId == this.appSession.tenant.id) {
                this.appSession.tenant.isPaidInvoice = data.isPaidInvoice;
                this.appSession.tenant.isCaution = data.isCaution;
                this.checkPayment();
            }
        });
    };

    checkPayment() {
        if (this.appSession.tenant != undefined) {
            if (!this.appSession.tenant.isPaidInvoice) {
                this.toastr
                    .error(
                        "You have an unpaid bill, as a result you are out of the service, PAY IT NOW",
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
    }
    goToBillings() {
        this.router.navigate(["/app/main/billings/billings"]);
    }

    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }

    ngAfterViewInit(): void {
        this.scrollToCurrentMenuElement();
    }

    showMenuItem(menuItem): boolean {
        return this._appNavigationService.showMenuItem(menuItem);
    }

    isMenuItemIsActive(item): boolean {
        if (item.items.length) {
            return this.isMenuRootItemIsActive(item);
        }

        if (!item.route) {
            return false;
        }

        let urlTree = this.router.parseUrl(
            this.currentRouteUrl.replace(/\/$/, "")
        );
        let urlString =
            "/" +
            urlTree.root.children.primary.segments
                .map((segment) => segment.path)
                .join("/");
        let exactMatch = urlString === item.route.replace(/\/$/, "");
        if (!exactMatch && item.routeTemplates) {
            for (let i = 0; i < item.routeTemplates.length; i++) {
                let result = new FormattedStringValueExtracter().Extract(
                    urlString,
                    item.routeTemplates[i]
                );
                if (result.IsMatch) {
                    return true;
                }
            }
        }
        return exactMatch;
    }
    isMenuRootItemIsActive(item): boolean {
        let result = false;

        for (const subItem of item.items) {
            result = this.isMenuItemIsActive(subItem);
            if (result) {
                item.isNewItem = true;
                return true;
            }
        }

        return false;
    }

    /**
     * Use for fixed left aside menu, to show menu on mouseenter event.
     * @param e Event
     */
    mouseEnter(e: Event) {
        // check if the left aside menu is fixed
        if (document.body.classList.contains("aside-fixed")) {
            if (this.outsideTm) {
                clearTimeout(this.outsideTm);
                this.outsideTm = null;
            }

            this.insideTm = setTimeout(() => {
                // if the left aside menu is minimized
                if (
                    document.body.classList.contains("aside-minimize") &&
                    KTUtil.isInResponsiveRange("desktop")
                ) {
                    // show the left aside menu
                    this.render.removeClass(document.body, "aside-minimize");
                    this.render.addClass(document.body, "aside-minimize-hover");
                }
            }, 50);
        }
    }

    /**
     * Use for fixed left aside menu, to show menu on mouseenter event.
     * @param e Event
     */
    mouseLeave(e: Event) {
        if (document.body.classList.contains("aside-fixed")) {
            if (this.insideTm) {
                clearTimeout(this.insideTm);
                this.insideTm = null;
            }

            this.outsideTm = setTimeout(() => {
                // if the left aside menu is expand
                if (
                    document.body.classList.contains("aside-minimize-hover") &&
                    KTUtil.isInResponsiveRange("desktop")
                ) {
                    // hide back the left aside menu
                    this.render.removeClass(
                        document.body,
                        "aside-minimize-hover"
                    );
                    this.render.addClass(document.body, "aside-minimize");
                }
            }, 100);
        }
    }

    scrollToCurrentMenuElement(): void {
        const path = location.pathname;
        const menuItem = document.querySelector("a[href='" + path + "']");
        if (menuItem) {
            menuItem.scrollIntoView({ block: "center" });
        }
    }

    getItemAttrSubmenuToggle(item) {
        let toggle = "hover";
        if (objectPath.get(item, "toggle") === "click") {
            toggle = "click";
        } else if (objectPath.get(item, "submenu.type") === "tabs") {
            toggle = "tabs";
        } else {
            // submenu toggle default to 'hover'
        }

        return toggle;
    }

    getItemCssClasses(item) {
        let classes = "menu-item";

        if (objectPath.get(item, "submenu")) {
            classes += " menu-item-submenu";
        }

        if (!item.items && this.isMenuItemIsActive(item)) {
            classes += " menu-item-active menu-item-here";
        }

        if (item.items && this.isMenuItemIsActive(item)) {
            classes += " menu-item-open menu-item-here";
        }

        // custom class for menu item
        const customClass = objectPath.get(item, "custom-class");
        if (customClass) {
            classes += " " + customClass;
        }

        if (objectPath.get(item, "icon-only")) {
            classes += " menu-item-icon-only";
        }

        return classes;
    }

    StopSound(): void {
        if (this.sound != null) {
            this.sound.stop();
            this.sound.unload();
            this.sound = null;
        }
    }

    onSidebarScroll(): void {
        // if (this.directiveRef.position(true).y > 3) {
        //     this.isScrolled = true;
        // } else {
        //     this.isScrolled = false;
        // }
    }
    getProfilePicture(): void {
        this._profileServiceProxy.getProfilePicture().subscribe((result) => {
            if (result && result.profilePicture) {
                this.profilePicture =
                    "data:image/jpeg;base64," + result.profilePicture;
            }
        });
    }

    toggleSideNav(): void {
        this.darkModeService.navMenuToggled();
    }
    openSubLi(item) {
        this.menu.items.forEach((items) => {
            if (items.name != item.name) {
                items.isNewItem = false;
            } else {
                if (item.isNewItem) {
                    item.isNewItem = false;
                } else {
                    item.isNewItem = true;
                }
            }
        });
    }
    closeAll() {
        this.menu.items.forEach((element) => {
            element.isNewItem = false;
        });
    }

    /**
     * Toggle sidebar expanded status
     */

    /**
     * Toggle sidebar collapsed status
     */
}
