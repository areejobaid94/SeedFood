// src/app/facebook.service.ts

import { Injectable, OnDestroy } from "@angular/core";
import {
    AccountServiceProxy,
    LoginFacebookModel,
    TenantDataFacebook,
} from "@shared/service-proxies/service-proxies";
import { BehaviorSubject } from "rxjs";
import { EmbededSignInModalComponent } from "../embeded-sign-in-modal/embeded-sign-in-modal.component";

declare const FB: any;

@Injectable({
    providedIn: "root",
})
export class SignInFacebookService implements OnDestroy {
    private fbScriptLoaded = new BehaviorSubject<boolean>(false);
    phone_number_id: string;
    waba_id: string;
    code: string;
    AccessToken: string;
    tenantDataAfterFacebook: TenantDataFacebook;
    private signInModal: EmbededSignInModalComponent | null = null;
    isLoading: boolean = false;

    constructor(private _accountService: AccountServiceProxy) {
        this.loadFbScript();
        this.initFbSdk();
        this.listenToMessages();
    }

    private loadFbScript() {
        const script = document.createElement("script");
        script.src = "https://connect.facebook.net/en_US/sdk.js";
        script.async = true;
        script.defer = true;
        script.crossOrigin = "anonymous";
        script.onload = () => this.fbScriptLoaded.next(true);
        document.body.appendChild(script);
    }

    private initFbSdk() {
        window["fbAsyncInit"] = () => {
            FB.init({
                appId: "885586280068397",
                autoLogAppEvents: true,
                xfbml: true,
                version: "v20.0",
            });
            this.fbScriptLoaded.next(true);
        };
    }

    private listenToMessages() {
        window.addEventListener("message", (event) => {
            if (
                event.origin !== "https://www.facebook.com" &&
                event.origin !== "https://web.facebook.com"
            ) {
                return;
            }
            try {
                const data = JSON.parse(event.data);
                if (data.type === "WA_EMBEDDED_SIGNUP") {
                    switch (data.event) {
                        case "FINISH":
                            const { phone_number_id, waba_id } = data.data;
                            // console.log(
                            //     "Phone number ID ",
                            //     phone_number_id,
                            //     " WhatsApp business account ID ",
                            //     waba_id
                            // );
                            this.phone_number_id = phone_number_id;
                            this.waba_id = waba_id;
                            break;
                        case "CANCEL":
                            const { current_step } = data.data;
                            // console.warn("Cancel at ", current_step);
                            break;
                        case "ERROR":
                            const { error_message } = data.data;
                            // console.error("Error ", error_message);
                            break;
                    }
                }
                document.getElementById("session-info-response")!.textContent =
                    JSON.stringify(data, null, 2);
            } catch {
                // console.log("Non JSON Responses", event.data);
            }
        });
    }

    fbLoginCallback(response: any) {
        if (response.authResponse) {
            this.isLoading = true;
            const code = response.authResponse.code;
            this.code = code;
            // console.log("getting facebook access token");
            let loginFacebookModel = new LoginFacebookModel();

            loginFacebookModel.code = code;
            loginFacebookModel.phone_number_id = this.phone_number_id;
            loginFacebookModel.waba_id = this.waba_id;

            this._accountService
                .faceBookAccessToken(loginFacebookModel)
                .subscribe((result) => {
                    this.tenantDataAfterFacebook = result;
                    this.signInModal.show(this.tenantDataAfterFacebook);
                    // this.AccessToken = result.access_token;
                });
        }
        document.getElementById("sdk-response")!.textContent = JSON.stringify(
            response,
            null,
            2
        );
    }

    launchWhatsAppSignup() {
        if (this.fbScriptLoaded.getValue()) {
            FB.login(this.fbLoginCallback.bind(this), {
                config_id: "1070595861148790",
                response_type: "code",
                override_default_response_type: true,
                extras: {
                    setup: {
                        // business: {
                        //     id: null,
                        //     name: "Infoseed",
                        //     email: "support@info-seed.com",
                        //     phone: {},
                        //     website: "https://www.info-seed.com",
                        //     address: { state: null, country: "SA" },
                        //     timezone: null,
                        // },
                        // phone: { category: null, description: "" },
                    },
                    featureType: "",
                    sessionInfoVersion: "2",
                },
            });
        }
    }

    ngOnDestroy() {
        window.removeEventListener("message", this.listenToMessages);
    }

    setSignInModal(modal: EmbededSignInModalComponent) {
        this.signInModal = modal;
    }
}
