import { EventEmitter, Injectable, Injector, inject } from "@angular/core";
import { AppSessionService } from "@shared/common/session/app-session.service";
//import { io } from 'socket.io-client';
import { Socket } from "ngx-socket-io";
import { CookieService } from "ngx-cookie-service";

@Injectable({
    providedIn: "root",
})
export class SocketioService {
    private socketConnected = false;  

    appSession: AppSessionService;
    public agentMessage: EventEmitter<any> = new EventEmitter<any>();
    public customerMessage: EventEmitter<any> = new EventEmitter<any>();

    public agentOrder: EventEmitter<any> = new EventEmitter<any>();
    public BotOrder: EventEmitter<any> = new EventEmitter<any>();

    public evaluationBot: EventEmitter<any> = new EventEmitter<any>();

    public sellingRequest: EventEmitter<any> = new EventEmitter<any>();
    public liveChat: EventEmitter<any> = new EventEmitter<any>();
    public maintenance: EventEmitter<any> = new EventEmitter<any>();
    public Booking: EventEmitter<any> = new EventEmitter<any>();
    public Invoices: EventEmitter<any> = new EventEmitter<any>();
    public Account: EventEmitter<any> = new EventEmitter<any>();
    cookieService = inject(CookieService);

    constructor(private socket: Socket, injector: Injector) {
        this.appSession = injector.get(AppSessionService);
        this.socket.emit("connected", this.appSession.tenantId);
    }

    ngOnInit(): void {
        // console.log('setupSocketConnection')
        //this.setupSocketConnection();
    }
    setupSocketConnection() {


        
        if (this.socketConnected) {
            return;
        }
        //   console.log('setupSocketConnection2')
        //  if (!this.socket){
        //   console.log('setupSocketConnection3')
        //     //var url='https://infoseedsocketioserver.azurewebsites.net/'
        //     var url='https://infoseedsocketioserverstg.azurewebsites.net/'
        //    // var url='http://localhost:3009/'
        //    //const token = "2bb33e3ae845db0b32dd1c5efdd9f35c"; // Prod
        //    const token = "313cb6e6-caad-4457-a0e4-669415d93250";// STG
        //   this.socket = io(url, {
        //       query: {

        //         token: token, // prod

        //       },

        //     });

        //  this.socket.emit('connected', this.appSession.tenantId);

        //     this.socket.on('connected', (data) => {
        //       console.log('connected= '+data)
        //       //this.socket.disconnect(console.log('disconnect'))

        //     });

        this.socket.on("chat-get", (data) => {
            //this.socket.disconnect(console.log('disconnect'))
            this.socket.emit('connected', this.appSession.tenantId);
            this.agentMessage.emit(data);
        });

        this.socket.on("booking-get", (data) => {
            this.Booking.emit(data);
        });

        this.socket.on("contact-get", (data) => {
            this.customerMessage.emit(data);
        });

        this.socket.on("order-get", (data) => {
            // console.log(data)
            this.BotOrder.emit(data);
        });

        this.socket.on("evaluation-get", (data) => {
            this.evaluationBot.emit(data);
        });


        this.socket.on("live-chat-get", (data) => {
            this.liveChat.emit(data);
        });
        this.socket.on("selling-request-get", (data) => {
            this.sellingRequest.emit(data);
        });
        this.socket.on("maintenance", (data) => {
            this.maintenance.emit(data);
        });

        this.socket.on("invoices-get", (data) => {
            this.Invoices.emit(data);
        });
        this.socket.on("account-get", (data) => {
            this.Account.emit(data);
            if (data.userId === this.appSession.userId) {
                this.cookieService.delete("Abp.AuthToken", " / ");
              
                if (window.confirm("The remaining session will get logged out soon, Please login again!")) {
                    window.location.reload();
                } else {
                    window.location.reload();
                }
            }
        });
        this.socketConnected = true;
    }
}

// public addBroadcastAgentMessagesListener = () => {
//     this.socket.on('chat-get', (data) => {
//         this.agentMessage.emit(data);
//     });
// }

// public addBroadcastEndUserMessagesListener = () => {
//      this.socket.on('contact-get', (data) => {
//         this.customerMessage.emit(data);
//     });
// }
// disconnect() {
//   if (this.socket) {
//     this.socket.disconnect();
//   }
// }
//}
