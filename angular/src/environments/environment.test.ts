// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `angular-cli.json`.

// "Production" enabled environment
  // ng build --configuration test --aot
export const environment = {
    production: false,
    test:true,
    hmr: false,
    appConfig: 'appconfig.staging.json',

   // socketIoUrl:"https://infoseedsocketioserver-prod.azurewebsites.net",
   //  socketIoToken:"2bb33e3ae845db0b32dd1c5efdd9f35c",
   socketIoUrl:"https://infoseedsocketioserverstg.azurewebsites.net",
    socketIoToken:"313cb6e6-caad-4457-a0e4-669415d93250",
  //  socketIoUrl:"https://infoseedsocketioserverqa.azurewebsites.net",
  //  socketIoToken:"a3901a01-e947-46d3-b347-b081a2fd1230",
  
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

