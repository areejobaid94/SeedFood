import { EventEmitter, Injectable, Injector, NgZone } from "@angular/core";
//import * as signalR from "@microsoft/signalr";
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from "@shared/common/app-component-base";

@Injectable({
    providedIn: "root",
})
export class MaintenanceSignalRService extends AppComponentBase {
//     private hubConnection: signalR.HubConnection;

//     public agentOrder: EventEmitter<any> = new EventEmitter<any>();
//     public BotOrder: EventEmitter<any> = new EventEmitter<any>();

    constructor(
       injector: Injector,
     //  public _zone: NgZone
   ) {
      super(injector);   }

//   chatHub: signalR.HubConnection;
//   isChatConnected = false;


//   init(): void {
//     this._zone.runOutsideAngular(() => {
//         abp.signalr.connect();
//         abp.signalr.startConnection(AppConsts.remoteServiceBaseUrl +'/maintenances', connection => {
//             this.configureConnection(connection);
//         }).then(() => {
//             abp.event.trigger('app.chat.connected');
//             this.isChatConnected = true;
//         });
//     });
// }

//     public startConnection = () => {

//         this.hubConnection = new signalR.HubConnectionBuilder()
//                                 .withUrl( AppConsts.remoteServiceBaseUrl +'/maintenances')
//                                 .withAutomaticReconnect()
//                                 .build();



//         this.hubConnection
//           .start()
//           .then(() => console.log('Connection started with Maintenances SIGNAL R'))
//           .catch(err => console.log('Error while starting connection: ' + err))

//       }

//       public closeConnection = () => {

//           this.hubConnection.stop()
//           .then(() => console.log("Connection stoped with Maintenances SIGNAL R")
//           )
//       }


//     public addMaintenancesAgentOrderListener = () => {


//         this.hubConnection.on('MaintenancesAgentOrder', (data) => {
          
//             console.log(data);
//             this.agentOrder.emit(data);
//         });
//     }

//       public addMaintenancesBotOrderListener = () => {

//         this.hubConnection.on('MaintenancesBotOrder', (data) => {
         
//           console.log(data);
//           this.BotOrder.emit(data);
//         });
//       }


//       configureConnection(connection): void {
//         // Set the common hub
//         this.chatHub = connection;

//         // Reconnect loop
//         let reconnectTime = 5000;
//         let tries = 1;
//         let maxTries = 8;
//         function start() {
//             return new Promise(function (resolve, reject) {
//                 if (tries > maxTries) {
//                     reject();
//                 } else {
//                     connection.start()
//                         .then(resolve)
//                         .then(() => {
//                             reconnectTime = 5000;
//                             tries = 1;
//                         })
//                         .catch(() => {
//                             setTimeout(() => {
//                                 start().then(resolve);
//                             }, reconnectTime);
//                             reconnectTime *= 2;
//                             tries += 1;
//                         });
//                 }
//             });
//         }

//         // Reconnect if hub disconnects
//         connection.onclose(e => {
//             this.isChatConnected = false;

//             if (e) {
//                 // window.location.reload();
//                 abp.log.debug('Chat connection closed with error: ' + e);
//               //  this.configureConnection(connection)
//             } else {
//                 // window.location.reload();
//                 abp.log.debug('Chat disconnected');
//                // this.configureConnection(connection)
//             }

//             start().then(() => {
//                 this.isChatConnected = true;
//             });
//         });

//         // Register to get notifications
//       //  this.registerChatEvents(connection);
//     }

}
