import { PermissionCheckerService } from 'abp-ng2-module';
import {
    Injector,
    ElementRef,
    Component,
    OnInit,
    ViewEncapsulation,
    Renderer2,
    AfterViewInit,
} from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppMenu } from './app-menu';
import { AppNavigationService } from './app-navigation.service';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { MenuOptions } from '@metronic/app/core/_base/layout/directives/menu.directive';
import { FormattedStringValueExtracter } from '@shared/helpers/FormattedStringValueExtracter';
import * as objectPath from 'object-path';
import { CustomerLiveChatModel, CustomerModel, GetOrderForViewDto, InvoicesModel, SellingRequestDto } from '@shared/service-proxies/service-proxies';
import { Subject, Subscription } from 'rxjs';

import { Howl } from 'howler';
import { TeamInboxService } from '@app/main/teamInbox/teaminbox.service';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { SocketioService } from '@app/shared/socketio/socketioservice';
import { ToastrService } from 'ngx-toastr';
import { NotificationsService } from '@app/services/notifications.service';


@Component({
    templateUrl: './side-bar-menu.component.html',
    selector: 'side-bar-menu',
    encapsulation: ViewEncapsulation.None,
})
export class SideBarMenuComponent extends AppComponentBase
    implements OnInit, AfterViewInit {
    element: any;
    menu: AppMenu = null;
    theme12 = true;
    audio: any;
    currentRouteUrl = '';
    insideTm: any;
    outsideTm: any;

    OrderCount = 0;
    EvaluationCount = 0;
    MaintenancesCount = 0;
    bookingCount = 0;

    LiveChatCount = 0;
    SellingRequestCount = 0;
    menuOptions: MenuOptions = {
        submenu: {
            desktop: {
                default: 'dropdown',
            },
            tablet: 'accordion',
            mobile: 'accordion',
        },

        accordion: {
            expandAll: false,
        }
    };
    botOrderSub: Subscription;
    botevaluationSub: Subscription;
    bookingSub: Subscription;

    sound: any;

    //theme 12 coreConfig: any;
    isCollapsed: boolean;
    isScrolled: boolean = false;

    // Private
    private _unsubscribeAll: Subject<any>;
    constructor(
        injector: Injector,
        private teamService: TeamInboxService,
        private el: ElementRef,
        private router: Router,
        public permission: PermissionCheckerService,
        private _appNavigationService: AppNavigationService,
        private render: Renderer2,
        private toastr: ToastrService,
        private socketioService: SocketioService,
        private _permissionCheckerService: PermissionCheckerService,
        public notificationsService: NotificationsService

    ) {
        super(injector);

    }
    ngDoCheck() {
        try {
            this.element = document.getElementById("SellingRequestDetails");
            if (this.element.ariaHidden == "false") {
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
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
                }
            })
        }catch {}
        try {
            this.element = document.getElementById("openLiveChat");
            this.element.addEventListener("click", (event: Event) => { 
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                }
            })
          
        }  
        catch {}
        try {
            this.element = document.getElementById("OrderModel");
            if (this.element.ariaHidden == "false") {
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                }
            }
        } catch {}
        try {
            this.element = document.getElementById("BookingModal");
              let className=this.element.className;
              let index = className.indexOf("show")
            if (
              index != -1
            ) {
              
                if (this.sound != null) {
                    this.sound.stop();
                    this.sound.unload();
                    this.sound = null;
                }
            }
        } catch {}
    }
    async ngOnInit() {



        try{

            this.sound.stop();
            this.sound.unload();
            this.sound = null;
        }catch{

        }
        debugger
        this.socketioService.setupSocketConnection();
        await this.getIsAdmin()
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
            .subscribe(
                (event) => {


                    this.menu.items.forEach(element => {
                        (this.currentRouteUrl = this.router.url.split(/[?#]/)[0]);


                        if (this.currentRouteUrl.includes('teamInbox')) {
                            document.getElementById("bodyID").style.overflow = "hidden";
                        } else {
                            document.getElementById("bodyID").style.overflow = "";
                        }

                        if (this.currentRouteUrl.includes('orders')) {

                            this.OrderCount = 0
                            element.isNewItem = false;
                        }

                        if (this.currentRouteUrl.includes('evaluation')) {

                            this.EvaluationCount = 0
                            element.isNewItem = false;
                        }


                        if (this.currentRouteUrl.includes('liveChat')) {
                            this.LiveChatCount = 0
                            element.isNewItem = false;
                        }
                        if (this.currentRouteUrl.includes('sellingRequest')) {
                            this.SellingRequestCount = 0
                            element.isNewItem = false;
                        }
                        if (this.currentRouteUrl.includes("calendar")) {
                            this.bookingCount = 0;
                            // element.isNewItem = false;
                        }
                    });
                }
            );
    }
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
        if(this.appSession.tenant != undefined){
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

    subscribeLiveChat = () => {

        
        this.socketioService.liveChat.subscribe(
            (data: CustomerLiveChatModel) => { 
                if (data.liveChatStatus === 1) {
                    if (data.tenantId == this.appSession.tenant.id) {
                        if (this.appSession.tenant.isBellOn) {
                            let ishasPermissionToDepartment =
                                this._permissionCheckerService.isGranted(
                                    "Pages.Department"
                                );
                            if (ishasPermissionToDepartment) {
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
                                            this.appSession.tenant
                                                .isBellContinues
                                        ) {
                                            if (this.sound != null) {
                                                this.sound.stop();
                                                this.sound.unload();
                                                this.sound = null;
                                            }

                                            this.sound = new Howl({
                                                src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                                src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
                                                html5: true,
                                                volume: 1.0,
                                            });

                                            this.sound.play();
                                        }
                                        this.LiveChatCount =
                                            this.LiveChatCount + 1;
                                    }
                                }
                            } else {
                                
                                if (this.appSession.tenant.isBellContinues) {
                                    if (this.sound != null) {
                                        this.sound.stop();
                                        this.sound.unload();
                                        this.sound = null;
                                    }

                                    this.sound = new Howl({
                                        src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                        src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
                                        html5: true,
                                        volume: 1.0,
                                    });

                                    this.sound.play();
                                }
                                this.LiveChatCount = this.LiveChatCount + 1;
                            }
                        } else {
                            this.LiveChatCount = this.LiveChatCount + 1;
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
                            let ishasPermissionToDepartment =
                                this._permissionCheckerService.isGranted(
                                    "Pages.Department"
                                );
                            if (ishasPermissionToDepartment) {
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
                                            this.appSession.tenant
                                                .isBellContinues
                                        ) {
                                            if (this.sound != null) {
                                                this.sound.stop();
                                                this.sound.unload();
                                                this.sound = null;
                                            }

                                            this.sound = new Howl({
                                                src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                                src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
                                                html5: true,
                                                volume: 1.0,
                                            });

                                            this.sound.play();
                                        }

                                        this.SellingRequestCount =
                                            this.SellingRequestCount + 1;

                                        // }
                                    }
                                } else {
                                    if (
                                        this.appSession.tenant.isBellContinues
                                    ) {
                                        if (this.sound != null) {
                                            this.sound.stop();
                                            this.sound.unload();
                                            this.sound = null;
                                        }

                                        this.sound = new Howl({
                                            src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                            src:"https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
                                            html5: true,
                                            volume: 1.0,
                                        });

                                        this.sound.play();
                                    }

                                    this.SellingRequestCount =
                                        this.SellingRequestCount + 1;
                                }
                            } else {
                                if (this.appSession.tenant.isBellContinues) {
                                    if (this.sound != null) {
                                        this.sound.stop();
                                        this.sound.unload();
                                        this.sound = null;
                                    }

                                    this.sound = new Howl({
                                        src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                        src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
                                        html5: true,
                                        volume: 1.0,
                                    });

                                    this.sound.play();
                                }

                                this.SellingRequestCount =
                                    this.SellingRequestCount + 1;
                            }
                        } else {
                            this.SellingRequestCount =
                                this.SellingRequestCount + 1;
                        }
                    }
                }
            }
        );
    };
    subscribeBotOrder = () => {
        this.botOrderSub = this.socketioService.BotOrder.subscribe(
            (data: GetOrderForViewDto) => {
                if(this.appSession.tenantId === data.tenantId){
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
                                        if (this.appSession.tenant.isBellOn) {
                                            if (
                                                this.appSession.tenant
                                                    .isBellContinues
                                            ) {
                                                if (
                                                    data.order.orderStatus == 6
                                                ) {
                                                    if (this.sound != null) {
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
                                                    if (this.sound != null) {
                                                        this.sound.stop();
                                                        this.sound.unload();
                                                        this.sound = null;
                                                    }

                                                    this.sound = new Howl({
                                                        src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
                                                        loop: true,
                                                        html5: true,
                                                        volume: 1.0,
                                                    });

                                                    this.sound.play();
                                                }
                                            } else {
                                                if (
                                                    data.order.orderStatus == 6
                                                ) {
                                                    if (this.sound != null) {
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
                                                    if (this.sound != null) {
                                                        this.sound.stop();
                                                        this.sound.unload();
                                                        this.sound = null;
                                                    }

                                                    this.sound = new Howl({
                                                        src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                        window.location.href.includes("orders");

                                    //f(data.order.agentId!=0 && this.appSession.user.id==data.order.agentId)
                                    if (
                                        data.order.agentIds != null &&
                                        data.order.agentIds.includes(
                                            this.appSession.user.id.toString()
                                        )
                                    ) {
                                        if (this.appSession.tenant.isBellOn) {
                                            if (
                                                this.appSession.tenant
                                                    .isBellContinues
                                            ) {
                                                if (
                                                    data.order.orderStatus == 6
                                                ) {
                                                    if (this.sound != null) {
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
                                                    if (this.sound != null) {
                                                        this.sound.stop();
                                                        this.sound.unload();
                                                        this.sound = null;
                                                    }

                                                    this.sound = new Howl({
                                                        src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
                                                        loop: true,
                                                        html5: true,
                                                        volume: 1.0,
                                                    });

                                                    this.sound.play();
                                                }
                                            } else {
                                                if (
                                                    data.order.orderStatus == 6
                                                ) {
                                                    if (this.sound != null) {
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
                                                    if (this.sound != null) {
                                                        this.sound.stop();
                                                        this.sound.unload();
                                                        this.sound = null;
                                                    }

                                                    this.sound = new Howl({
                                                        src:"https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",

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
                this.menu.items.forEach((element) => {
                    if (
                        element.name == "Orders Maintenances" &&
                        data.tenantId === this.appSession.tenantId
                    ) {
                        let isher =
                            window.location.href.includes("maintenances");
                        if (!isher) {
                            this.EvaluationCount = this.EvaluationCount + 1;
                            element.isNewItem = true;
                        }
                    }
                });
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
                                            src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                            src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                                src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
                                                src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3",
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
            this.currentRouteUrl.replace(/\/$/, '')
        );
        let urlString =
            '/' +
            urlTree.root.children.primary.segments
                .map((segment) => segment.path)
                .join('/');
        let exactMatch = urlString === item.route.replace(/\/$/, '');
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
        if (document.body.classList.contains('aside-fixed')) {
            if (this.outsideTm) {
                clearTimeout(this.outsideTm);
                this.outsideTm = null;
            }

            this.insideTm = setTimeout(() => {
                // if the left aside menu is minimized
                if (document.body.classList.contains('aside-minimize') && KTUtil.isInResponsiveRange('desktop')) {
                    // show the left aside menu
                    this.render.removeClass(document.body, 'aside-minimize');
                    this.render.addClass(document.body, 'aside-minimize-hover');
                }
            }, 50);
        }
    }

    /**
     * Use for fixed left aside menu, to show menu on mouseenter event.
     * @param e Event
     */
    mouseLeave(e: Event) {
        if (document.body.classList.contains('aside-fixed')) {
            if (this.insideTm) {
                clearTimeout(this.insideTm);
                this.insideTm = null;
            }

            this.outsideTm = setTimeout(() => {
                // if the left aside menu is expand
                if (document.body.classList.contains('aside-minimize-hover') && KTUtil.isInResponsiveRange('desktop')) {
                    // hide back the left aside menu
                    this.render.removeClass(document.body, 'aside-minimize-hover');
                    this.render.addClass(document.body, 'aside-minimize');
                }
            }, 100);
        }
    }

    scrollToCurrentMenuElement(): void {
        const path = location.pathname;
        const menuItem = document.querySelector('a[href=\'' + path + '\']');
        if (menuItem) {
            menuItem.scrollIntoView({ block: 'center' });
        }
    }

    getItemAttrSubmenuToggle(item) {
        let toggle = 'hover';
        if (objectPath.get(item, 'toggle') === 'click') {
            toggle = 'click';
        } else if (objectPath.get(item, 'submenu.type') === 'tabs') {
            toggle = 'tabs';
        } else {
            // submenu toggle default to 'hover'
        }

        return toggle;
    }

    getItemCssClasses(item) {
        let classes = 'menu-item';

        if (objectPath.get(item, 'submenu')) {
            classes += ' menu-item-submenu';
        }

        if (!item.items && this.isMenuItemIsActive(item)) {
            classes += ' menu-item-active menu-item-here';
        }

        if (item.items && this.isMenuItemIsActive(item)) {
            classes += ' menu-item-open menu-item-here';
        }

        // custom class for menu item
        const customClass = objectPath.get(item, 'custom-class');
        if (customClass) {
            classes += ' ' + customClass;
        }

        if (objectPath.get(item, 'icon-only')) {
            classes += ' menu-item-icon-only';
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

    
    ngOnDestroy() {

        
        this.StopSound();
      }

}
