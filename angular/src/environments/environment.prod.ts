// "Production" enabled environment

// ng build --configuration production --aot

export const environment = {
    production: true,
    hmr: false,
    staging:false,
    appConfig: 'appconfig.production.json',
  socketIoUrl:"https://infoseedsocketioserver-prod.azurewebsites.net",
  socketIoToken:"2bb33e3ae845db0b32dd1c5efdd9f35c",
  firebase: {
    apiKey: "AIzaSyB96mmrshd56OaGPZNPvi6l1NN2yUa8ua8",
    authDomain: "info-seed-prod.firebaseapp.com",
    projectId: "info-seed-prod",
    storageBucket: "info-seed-prod.appspot.com",
    messagingSenderId: "174110793202",
    appId: "1:174110793202:web:87412ae4ece27e21d09f6b",
    measurementId: "G-C1ETYG243Y"
  },
 

  
};
