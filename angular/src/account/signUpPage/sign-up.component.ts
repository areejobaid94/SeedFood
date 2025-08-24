import { MapsAPILoader } from "@agm/core";
import {
    Component,
    ElementRef,
    Injector,
    NgZone,
    OnInit,
    ViewChild,
} from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    BookingRequestModel,
    CampaignRequestModel,
    EnterpriseRequestModel,
    OnlineStoreRequestModel,
    SellingRequestDto,
    SellingRequestServiceProxy,
    SginUpRequestModel,
} from "@shared/service-proxies/service-proxies";
import Stepper from "bs-stepper";
import { FlatpickrOptions } from "ng2-flatpickr";
import { CountryISO, SearchCountryField } from "ngx-intl-tel-input";
import { FacebookService } from "./facebook.service";

@Component({
    selector: "app-sign-up",
    templateUrl: "./sign-up.component.html",
    styleUrls: ["./sign-up.component.css"],
})
export class SignUpComponent extends AppComponentBase implements OnInit {
    private horizontalWizardStepper: Stepper;
    CountryISO = CountryISO;
    phoneNumber: any;
    preferredCountries: CountryISO[] = [CountryISO.SaudiArabia];
    signUpModel: SellingRequestDto = undefined;
    signUpRequestModel: SginUpRequestModel = new SginUpRequestModel();

    //service Field
    service: any;
    //number Field
    dialCode: any;

    //map Field
    SearchCountryField = SearchCountryField;
    zoom: number;
    private geoCoder;

    //CountryField
    isUAE = false;
    isJordan = false;
    isKSA = false;
    isIraq = false;
    isBahrain = false;
    isQatar = false;
    isKuwait = false;

    //chatBot language
    isArabic = false;
    isEnglish = false;

    //options
    isDelivery = false;
    isTakeAway = false;
    isPreOrder = false;
    isInquiry = false;

    //pricing Method
    isPerKiloMeter = false;
    isPerZone = false;

    //whatsappCampaign
    isChatcustomer: any;

    //booking
    bookingInquiry: any;

    //interprise
    enterpriseIntegration: any;

    //flatPicker
    public dateTimeOptions: FlatpickrOptions = {
        altInput: true,
        enableTime: true,
        inline: true,
        wrap: false,
    };
    submitted = false;
    phoneForm = new FormGroup({
        phone: new FormControl(undefined, [Validators.required]),
    });

    //iframe Link
    source = "https://www.zoho.com/books/online-payments.html";

    @ViewChild("search") public searchElementRef: ElementRef;

    constructor(
        private facebookService: FacebookService,
        injector: Injector,
        private _accountService: SellingRequestServiceProxy,
        private mapsAPILoader: MapsAPILoader,
        private ngZone: NgZone
    ) {
        super(injector);
    }

    ngOnInit(): void {

    // Example usage of the Facebook SDK
    const FB = this.facebookService.getFB();
    if (FB) {
      FB.getLoginStatus(response => {
        console.log(response);
      });
    }



        this.horizontalWizardStepper = new Stepper(
            document.querySelector("#stepper1"),
            {}
        );
        this.signUpModel = new SellingRequestDto();
        this.signUpModel.sginUpRequest = new SginUpRequestModel();
        // nested Models
        this.signUpModel.sginUpRequest.onlineStoreRequestModel =
            new OnlineStoreRequestModel();
        this.signUpModel.sginUpRequest.campaignRequestModel =
            new CampaignRequestModel();
        this.signUpModel.sginUpRequest.bookingRequestModel =
            new BookingRequestModel();
        this.signUpModel.sginUpRequest.enterpriseRequestModel =
            new EnterpriseRequestModel();
        //
        this.service = "0";
        this.mapsAPILoader.load().then(() => {
            this.setCurrentLocation();
            this.geoCoder = new google.maps.Geocoder();
        });
    }
    private setCurrentLocation() {
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition((position) => {
                this.signUpModel.sginUpRequest.latitude =
                    position.coords.latitude;
                this.signUpModel.sginUpRequest.longitude =
                    position.coords.longitude;
                this.zoom = 15;
            });
        }
    }

    getCountryCode(event: any) {
        this.dialCode = event.dialCode;
    }

    markerDragEnd(event) {
        // var marker = new google.maps.Marker({
        //     position: event.latLng,
        //     map: this.googleMap
        //   });

        this.signUpModel.sginUpRequest.latitude = event.latLng.lat();
        this.signUpModel.sginUpRequest.longitude = event.latLng.lng();
    }

    horizontalWizardStepperNext() {
        this.submitted = false;
        if (
            this.service === null ||
            this.service == undefined ||
            this.service === "" ||
            this.phoneNumber === null ||
            this.phoneNumber == undefined ||
            this.phoneNumber == "" ||
            this.signUpModel.sginUpRequest.latitude === null ||
            this.signUpModel.sginUpRequest.latitude == undefined ||
            this.signUpModel.sginUpRequest.longitude === null ||
            this.signUpModel.sginUpRequest.longitude == undefined
        ) {
            this.submitted = true;
            return;
        }

        if (
            !this.isBahrain &&
            !this.isIraq &&
            !this.isJordan &&
            !this.isKSA &&
            !this.isKuwait &&
            !this.isQatar &&
            !this.isUAE
        ) {
            this.submitted = true;
            return;
        }

        if (this.service == "0") {
            if (this.isArabic === false && this.isEnglish === false) {
                this.submitted = true;
                return;
            }
            if (
                this.isDelivery === false &&
                this.isTakeAway === false &&
                this.isPreOrder === false &&
                this.isInquiry === false
            ) {
                this.submitted = true;
                return;
            }
            if (this.isPerKiloMeter === false && this.isPerZone === false) {
                this.submitted = true;
                return;
            }
            if (
                this.signUpModel.sginUpRequest.onlineStoreRequestModel
                    .numberOfBranches === null ||
                this.signUpModel.sginUpRequest.onlineStoreRequestModel
                    .numberOfBranches === undefined
            ) {
                this.submitted = true;
                return;
            }
        } else if (this.service == "1") {
            if (
                this.isChatcustomer === null ||
                this.isChatcustomer === undefined
            ) {
                this.submitted = true;
                return;
            }
        } else if (this.service == "2") {
            if (
                this.bookingInquiry === null ||
                this.bookingInquiry === undefined ||
                this.signUpModel.sginUpRequest.bookingRequestModel
                    .numberOfBranches === null ||
                this.signUpModel.sginUpRequest.bookingRequestModel
                    .numberOfBranches === undefined ||
                this.signUpModel.sginUpRequest.bookingRequestModel
                    .totalNumberOfDepartmentsOrDoctor === null ||
                this.signUpModel.sginUpRequest.bookingRequestModel
                    .totalNumberOfDepartmentsOrDoctor === undefined
            ) {
                this.submitted = true;
                return;
            }
        } else if (this.service == "3") {
            if (
                this.enterpriseIntegration === null ||
                this.enterpriseIntegration === undefined ||
                this.signUpModel.sginUpRequest.enterpriseRequestModel
                    .bookDemo === null ||
                this.signUpModel.sginUpRequest.enterpriseRequestModel
                    .bookDemo === undefined
            ) {
                this.submitted = true;
                return;
            }
        }
        this.horizontalWizardStepper.next();
    }
    /**
     * Horizontal Wizard Stepper Previous
     */
    horizontalWizardStepperPrevious() {
        this.horizontalWizardStepper.previous();
    }
    onSubmit() {
        alert("Submitted!!");
        return false;
    }

    changeService(event) {
        this.service = event.target.value;
        this.signUpModel.sginUpRequest.serviceTypeId = Number(this.service);
        if (this.service == "0") {
            this.signUpModel.sginUpRequest.onlineStoreRequestModel =
                new OnlineStoreRequestModel();
            this.signUpModel.sginUpRequest.campaignRequestModel =
                new CampaignRequestModel();
            this.signUpModel.sginUpRequest.bookingRequestModel =
                new BookingRequestModel();
            this.signUpModel.sginUpRequest.enterpriseRequestModel =
                new EnterpriseRequestModel();
            this.isArabic = false;
            this.isEnglish = false;
            this.isPerKiloMeter = false;
            this.isPerZone = false;
            this.isDelivery = false;
            this.isTakeAway = false;
            this.isPreOrder = false;
            this.isInquiry = false;
            this.isChatcustomer = null;
            this.bookingInquiry = null;
            this.enterpriseIntegration = null;
        } else if (this.service == "1") {
            this.signUpModel.sginUpRequest.onlineStoreRequestModel =
                new OnlineStoreRequestModel();
            this.signUpModel.sginUpRequest.campaignRequestModel =
                new CampaignRequestModel();
            this.signUpModel.sginUpRequest.bookingRequestModel =
                new BookingRequestModel();
            this.signUpModel.sginUpRequest.enterpriseRequestModel =
                new EnterpriseRequestModel();
            this.isArabic = false;
            this.isEnglish = false;
            this.isPerKiloMeter = false;
            this.isPerZone = false;
            this.isDelivery = false;
            this.isTakeAway = false;
            this.isPreOrder = false;
            this.isInquiry = false;
            this.isChatcustomer = null;
            this.bookingInquiry = null;
            this.enterpriseIntegration = null;
        } else if (this.service == "2") {
            this.signUpModel.sginUpRequest.onlineStoreRequestModel =
                new OnlineStoreRequestModel();
            this.signUpModel.sginUpRequest.campaignRequestModel =
                new CampaignRequestModel();
            this.signUpModel.sginUpRequest.bookingRequestModel =
                new BookingRequestModel();
            this.signUpModel.sginUpRequest.enterpriseRequestModel =
                new EnterpriseRequestModel();
            this.isArabic = false;
            this.isEnglish = false;
            this.isPerKiloMeter = false;
            this.isPerZone = false;
            this.isDelivery = false;
            this.isTakeAway = false;
            this.isPreOrder = false;
            this.isInquiry = false;
            this.isChatcustomer = null;
            this.bookingInquiry = null;
            this.enterpriseIntegration = null;
        } else if (this.service == "3") {
            this.signUpModel.sginUpRequest.onlineStoreRequestModel =
                new OnlineStoreRequestModel();
            this.signUpModel.sginUpRequest.campaignRequestModel =
                new CampaignRequestModel();
            this.signUpModel.sginUpRequest.bookingRequestModel =
                new BookingRequestModel();
            this.signUpModel.sginUpRequest.enterpriseRequestModel =
                new EnterpriseRequestModel();
            this.isArabic = false;
            this.isEnglish = false;
            this.isPerKiloMeter = false;
            this.isPerZone = false;
            this.isDelivery = false;
            this.isTakeAway = false;
            this.isPreOrder = false;
            this.isInquiry = false;
            this.isChatcustomer = null;
            this.bookingInquiry = null;
            this.enterpriseIntegration = null;
        }
    }

    save(): void {
        this.signUpModel.sginUpRequest.serviceTypeId = Number(this.service);
        let countryArray = [];
        if (this.isUAE) {
            countryArray.push("UAE");
        }
        if (this.isJordan) {
            countryArray.push("Jordan");
        }
        if (this.isKSA) {
            countryArray.push("KSA");
        }
        if (this.isIraq) {
            countryArray.push("Iraq");
        }
        if (this.isBahrain) {
            countryArray.push("Bahrain");
        }
        if (this.isQatar) {
            countryArray.push("Qatar");
        }
        if (this.isKuwait) {
            countryArray.push("Kuwait");
        }
        this.signUpModel.sginUpRequest.country = countryArray.join(",");
        this.signUpModel.sginUpRequest.contactNumber =
            this.dialCode + this.phoneNumber.number;

        if (this.signUpModel.sginUpRequest.serviceTypeId == 0) {
            this.signUpModel.sginUpRequest.onlineStoreRequestModel.deliveryPricingMethodId =
                this.isPerKiloMeter ? 0 : 1;
            let languageArray = [];
            if (this.isArabic) {
                languageArray.push(0);
            }
            if (this.isEnglish) {
                languageArray.push(1);
            }

            this.signUpModel.sginUpRequest.onlineStoreRequestModel.chatBotLanguage =
                languageArray.join(",");

            let optionsArray = [];
            if (this.isDelivery) {
                optionsArray.push(0);
            }

            if (this.isTakeAway) {
                optionsArray.push(1);
            }
            if (this.isPreOrder) {
                optionsArray.push(2);
            }
            if (this.isInquiry) {
                optionsArray.push(3);
            }

            this.signUpModel.sginUpRequest.onlineStoreRequestModel.optionIncluded =
                optionsArray.join(",");
        } else if (this.signUpModel.sginUpRequest.serviceTypeId == 1) {
            this.signUpModel.sginUpRequest.campaignRequestModel.isChatCustomer =
                this.isChatcustomer ? true : false;
        } else if (this.signUpModel.sginUpRequest.serviceTypeId == 2) {
            this.signUpModel.sginUpRequest.bookingRequestModel.isIncludedCustomerInquiry =
                this.bookingInquiry ? true : false;
        } else if (this.signUpModel.sginUpRequest.serviceTypeId == 3) {
            this.signUpModel.sginUpRequest.enterpriseRequestModel.isIntegrationWithSystem =
                this.enterpriseIntegration ? true : false;
        }

        // this._accountService.addSginUpRequest(this.signUpModel).subscribe(() => {

        // }
        // );
    }
}
