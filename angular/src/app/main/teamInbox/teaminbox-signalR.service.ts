import { AgmScaleControl } from "@agm/core";
import { EventEmitter, Injectable, Injector, NgZone } from "@angular/core";
//import * as signalR from "@microsoft/signalr";
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from "@shared/common/app-component-base";
@Injectable(
//     {
//     providedIn: "root",

// }
)
export class TeamInboxSignalRService extends AppComponentBase {
    //private hubConnection: signalR.HubConnection;
    public agentMessage: EventEmitter<any> = new EventEmitter<any>();
    public customerMessage: EventEmitter<any> = new EventEmitter<any>();
    //socket;
    constructor(
    
      injector: Injector,
      //public _zone: NgZone,
     // private socket: Socket

  ) {
      super(injector);
  }


  //chatHub: signalR.HubConnection;
  //isChatConnected = false;


  init(): void {
    
  
     
    // this._zone.runOutsideAngular(() => {
    //     abp.signalr.connect();
    //     abp.signalr.startConnection(AppConsts.remoteServiceBaseUrl +'/teaminbox', connection => {
            
    //         this.configureConnection(connection);
    //     }).then(() => {
            
    //         abp.event.trigger('app.chat.connected');
    //         this.isChatConnected = true;
    //     });
    // });
}

//public sockectIo = () => {

  // this.socket = io("http://localhost:3009/", {

  //   query: {

  //     token: "2bb33e3ae845db0b32dd1c5efdd9f35c"

  //   }

  // });

  // this.socket.emit('connected', 27);



    
//   socket.on('live-chat-get', (data) => print(data));
//   socket.on('order-get', (data) => print(data));
//  socket.on('change-order-status', (data) => print(data));

    
//}



    // public startConnection = () => {

    //     this.hubConnection = new signalR.HubConnectionBuilder()
    //                             .withUrl( AppConsts.remoteServiceBaseUrl +'/teaminbox')
    //                             .withAutomaticReconnect()
    //                             .build();

    //     this.hubConnection
    //       .start()
    //       .then(() => console.log('Connection started with TEAM INBOX SIGNAL R'))
    //       .catch(err => console.log('Error while starting connection: ' + err))
    //       this.isChatConnected = true;

    //   }

    //   public closeConnection = () => {

    //       this.hubConnection.stop()
    //       .then(() => console.log("Connection stoped with TEAM INBOX SIGNAL R")
    //       )
    //       this.isChatConnected = false;
    //   }


   // public addBroadcastEndUserMessagesListener = () => {

    
        //  this.socket.on('contact-get', (data) => {
           
         
        //     this.customerMessage.emit(data);
        // });
    
 //   }

      //public addBroadcastAgentMessagesListener = () => {
           
      
      //   this.socket.on('chat-get', (data) => {
           
      //     console.log(data);
      //     this.agentMessage.emit(data);
      // });
        // this.hubConnection.on('brodCastAgentMessage', (data) => {
            
        //   console.log(data);
        //   this.agentMessage.emit(data);
        // });
     // }
    //   configureConnection(connection): void {
    //     //   console.log('Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop ');
    //     // abp.log.debug('Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop Reconnect loop ');
    //     // Set the common hub
    //     this.chatHub = connection;

    //     // Reconnect loop
    //     let reconnectTime = 5000;
    //     let tries = 1;
    //     let maxTries = 8;
    //     function start() {
    //         return new Promise(function (resolve, reject) {
    //             if (tries > maxTries) {
    //                 reject();
    //             } else {
    //                 connection.start()
    //                     .then(resolve)
    //                     .then(() => {
    //                         reconnectTime = 5000;
    //                         tries = 1;
    //                     })
    //                     .catch(() => {
    //                         setTimeout(() => {
    //                             start().then(resolve);
    //                         }, reconnectTime);
    //                         reconnectTime *= 2;
    //                         tries += 1;
    //                     });
    //             }
    //         });
    //     }

    //     // Reconnect if hub disconnects
    //     connection.onclose(e => {
    //         this.isChatConnected = false;

    //         if (e) {
    //             abp.log.debug('Chat connection closed with error: ' + e);
    //             //this.configureConnection(connection)
    //             //window.location.reload();
                
    //         } else {
    //             abp.log.debug('Chat disconnected');
    //             //this.configureConnection(connection)
    //             // window.location.reload();
                
    //         }

    //         start().then(() => {
    //             this.isChatConnected = true;
    //         });
    //     });

    //     // Register to get notifications
    //   //  this.registerChatEvents(connection);
    // }
}
