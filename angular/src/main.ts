import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import 'moment-timezone';
import 'moment/min/locales.min';
import { environment } from './environments/environment';
import { hmrBootstrap } from './hmr';
import './polyfills.ts';
import { RootModule } from './root.module';
import * as Sentry from "@sentry/angular-ivy";

Sentry.init({
    dsn:"" ,//"https://d186fd09c4029fccef92aa140b1133e4@o4506030303674368.ingest.sentry.io/450605925262950",
    integrations: [
      new Sentry.BrowserTracing({
        // Set 'tracePropagationTargets' to control for which URLs distributed tracing should be enabled
        tracePropagationTargets: ["localhost", /^https:\/\/yourserver\.io\/api/],
        routingInstrumentation: Sentry.routingInstrumentation,
      }),
      new Sentry.Replay(),
    ],
    // Performance Monitoring
    tracesSampleRate: 1.0, // Capture 100% of the transactions
    // Session Replay
    replaysSessionSampleRate: 0.1, // This sets the sample rate at 10%. You may want to change it to 100% while in development and then sample at a lower rate in production.
    replaysOnErrorSampleRate: 1.0, // If you're not already sampling the entire session, change the sample rate to 100% when sampling sessions where errors occur.
  });
if (environment.production) {
    enableProdMode();
}

const bootstrap = () => {
    return platformBrowserDynamic().bootstrapModule(RootModule);
};

/* "Hot Module Replacement" is enabled as described on
 * https://medium.com/@beeman/tutorial-enable-hrm-in-angular-cli-apps-1b0d13b80130#.sa87zkloh
 */
   
//Audai Jamaeen:  Aya should check it
// if (environment.hmr) {
//     if (module['hot']) {
//         hmrBootstrap(module, bootstrap); //HMR enabled bootstrap
//     } 
//     else {
//     }
// } else {
    bootstrap(); //Regular bootstrap
//}
  