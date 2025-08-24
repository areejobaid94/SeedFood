// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `angular-cli.json`.

export const environment = {
    production: false,
    hmr: false,
    test:false,
    

    
    appConfig: "appconfig.json",
    teamInboxUrl: "https://localhost:44301",
    socketIoUrl:"https://infoseedsocketioserverstg.azurewebsites.net",
    socketIoToken:"313cb6e6-caad-4457-a0e4-669415d93250",
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
