import {
    Component,
    EventEmitter,
    Injector,
    OnInit,
    Output,
    ViewChild,
} from "@angular/core";
import {
    FormGroup,
    UntypedFormArray,
    UntypedFormBuilder,
    UntypedFormControl,
} from "@angular/forms";
import { DomSanitizer } from "@angular/platform-browser";
import { FeatureTreeComponent } from "@app/admin/shared/feature-tree.component";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    BotTemplatesModel,
    CommonLookupServiceProxy,
    EntityDto,
    GeneralServiceProxy,
    ItemsServiceProxy,
    SettingsTenantHostModel,
    SubscribableEditionComboboxItemDto,
    TenantEditDto,
    TenantServiceModalDto,
    TenantServiceProxy,
    TenantServicesServiceProxy,
    UpdateTenantFeaturesInput,
} from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { ToastrService } from "ngx-toastr";
import { forkJoin, Observable, of } from "rxjs";
import { catchError, filter, finalize, map, tap } from "rxjs/operators";

@Component({
    selector: "app-all-settings-modal",
    templateUrl: "./all-settings-modal.component.html",
    styleUrls: ["./all-settings-modal.component.css"],
})
export class AllSettingsModalComponent
    extends AppComponentBase
    implements OnInit
{
    @ViewChild("AllSettingsModal", { static: true }) modal: ModalDirective;
    @ViewChild("featureTree") featureTree: FeatureTreeComponent;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    form: FormGroup;
    saving = false;
    isButtonDisabled = true;
    tabIndex = 0;
    tenantForSettings: TenantEditDto;
    isViewValuesEdited = false;
    uploadedViewFileUrl;
    isPDFView = false;
    Integration:boolean=false;

    ViewValues = {
        tenantId: 0,
        customerName: "",
        editCustomer: "",
        phoneNumber: "",
        email: "",
        address: "",
        tenantwebSite: "",
        isContractUpload: false,
        uploadedFileUrl: "",
    };

    SettingsValues = {
        timeZone: "",
        currency: "",
        currencyList: [],
        clientIpAddress: "",
        zohoCustomerId: "",
        cautionDays: 0,
        warningDays: 0,
        totalCustomerWallet: 0,
        isPreOrder: false,
        isPickup: false,
        isDelivery: false,
        isModified: false,
        isLoaded: false,
    };

    EditValues = {
        botTemplates: [] as BotTemplatesModel[],
        editions: [] as SubscribableEditionComboboxItemDto[],
        tenant: new TenantEditDto() as TenantEditDto,
        isUnlimited: false,
        isLoaded: false,
        isModified: false,
    };

    FeatureValues = {
        editData: null,
        resettingFeatures: false,
        isLoaded: false,
    };

    ServiceValues = {
        services: [] as TenantServiceModalDto[],
        activeCheckboxFormArray: null,
        editTenantServicesForm: null,
        isLoaded: false,
        isModified: false,
    };

    errors = {
        currencyError: false,
        editError: false,
        editErrorCN: false,
        editErrorPI: false,
        editErrorTW: false,
        editErrorLI: false,
        editErrorPN: false,
        editErrorA: false,
        editErrorTT: false,
        editErrorTN: false,
        editErrorTL: false,
        editErrorWBL: false,
        editErrorWAI: false,
        editErrorBDL: false,
        editErrorBI: false,
        editErrorIN: false,
        editErrorAT: false,
        editErrorDLS: false,
        editErrorMI: false,

        editErrorFP: false,
        editErrorII: false,
        editErrorFAT: false,
        editErrorIAT: false,
    };

    constructor(
        injector: Injector,
        private _tenantService: TenantServiceProxy,
        private _itemsServiceProxy: ItemsServiceProxy,
        private sanitizer: DomSanitizer,
        private toastr: ToastrService,
        private _tenantDashboardServiceProxy: TenantServiceProxy,
        private _commonLookupService: CommonLookupServiceProxy,
        private _generalServiceProxy: GeneralServiceProxy,
        private _tenantServiceService: TenantServicesServiceProxy,
        private fb: UntypedFormBuilder
    ) {
        super(injector);
    }

    ngOnInit(): void {}

    onSubmit() {
        if (this.errors.currencyError) {
            return;
        }
        if (this.checkEditErrors()) {
            return;
        }

        this.awaitedSubmit().subscribe({
            next: (result) => {
                console.log(result);
                this.modalSave.emit();
                this.modal.hide();
            },
            error: (err) => {
                console.error("Error during submission:", err);
                // Handle the error accordingly
            },
        });
    }

    awaitedSubmit(): Observable<any> {
        const saveOperations: Observable<any>[] = [];

        if (this.ViewValues.isContractUpload) {
            saveOperations.push(this.ViewSave());
        }

        if (this.SettingsValues.isModified) {
            saveOperations.push(this.SettingsSave());
        }

        if (this.EditValues.isModified) {
            saveOperations.push(this.saveEdit());
        }

        if (this.featureTree?.isEdited) {
            saveOperations.push(this.FeatureSave());
        }

        if (this.ServiceValues.isModified) {
            saveOperations.push(this.saveServices());
        }

        // Return a combined observable or just of(true) if there are no operations
        return saveOperations.length > 0
            ? forkJoin(saveOperations).pipe(
                  // You can handle the results here if needed
                  filter((results) =>
                      results.every((result) => result === true)
                  ) // Example filter
              )
            : of(true);
    }

    show(tenantId: number) {
        // ViewValues on ShowF
        this.tabIndex = 0;
        this._tenantService
            .getTenantForEdit(tenantId)
            .subscribe((tenantResult) => {
                this.tenantForSettings = tenantResult;
                this.ViewValues.tenantId = tenantResult.id;
                this.ViewValues.address = tenantResult.address;
                this.ViewValues.customerName = tenantResult.name;
                this.ViewValues.editCustomer = tenantResult.tenancyName;
                this.ViewValues.email = tenantResult.email;
                this.ViewValues.phoneNumber = tenantResult.phoneNumber;
                this.ViewValues.tenantwebSite = tenantResult.website;
                this.SettingsValues.isLoaded = false;
                this.EditValues.isLoaded = false;
                this.FeatureValues.isLoaded = false;
                this.ServiceValues.isLoaded = false;
            });

        this.errors = {
            currencyError: false,
            editError: false,
            editErrorCN: false,
            editErrorPI: false,
            editErrorTW: false,
            editErrorLI: false,
            editErrorPN: false,
            editErrorA: false,
            editErrorTT: false,
            editErrorTN: false,
            editErrorTL: false,
            editErrorWBL: false,
            editErrorWAI: false,
            editErrorBDL: false,
            editErrorBI: false,
            editErrorIN: false,
            editErrorAT: false,
            editErrorDLS: false,
            editErrorMI: false,
            

            editErrorFP: false,
            editErrorII: false,
            editErrorFAT: false,
            editErrorIAT: false,
        };

        this.modal.show();
    }

    close() {
        this.modal.hide();
    }

    switchHeaders(tab: any) {
        this.tabIndex = tab.index;

        // Settings Tab
        if (this.tabIndex == 1) {
            if (!this.SettingsValues.isLoaded) {
                this._tenantDashboardServiceProxy
                    .getSettingsTenantHost(this.tenantForSettings.id)
                    .subscribe((tenantResult: SettingsTenantHostModel) => {
                        this.SettingsValues.timeZone = tenantResult.timeZone;
                        this.SettingsValues.currency = tenantResult.currency;
                        this.SettingsValues.clientIpAddress =
                            tenantResult.clientIpAddress;
                        this.SettingsValues.zohoCustomerId =
                            tenantResult.zohoCustomerId;
                        this.SettingsValues.cautionDays =
                            tenantResult.cautionDays;
                        this.SettingsValues.warningDays =
                            tenantResult.warningDays;
                        this.SettingsValues.totalCustomerWallet =
                            tenantResult.totalCustomerWallet;
                        this.SettingsValues.isPreOrder =
                            tenantResult.isPreOrder;
                        this.SettingsValues.isPickup = tenantResult.isPickup;
                        this.SettingsValues.isDelivery =
                            tenantResult.isDelivery;
                        this.SettingsValues.currencyList =
                            tenantResult.currencyList;
                        this.SettingsValues.isLoaded = true;

                        if (
                            this.SettingsValues.currency == null ||
                            this.SettingsValues.currency == ""
                        ) {
                            this.errors.currencyError = true;
                        }
                    });
            } else {
                if (
                    this.SettingsValues.currency == null ||
                    this.SettingsValues.currency == ""
                ) {
                    this.errors.currencyError = true;
                }
            }
        } else if (this.tabIndex == 2) {
            if (!this.EditValues.isLoaded) {
                this._generalServiceProxy
                    .getBotTemplates()
                    .subscribe((result) => {
                        this.EditValues.botTemplates = result;

                        this._commonLookupService
                            .getEditionsForCombobox(false)
                            .subscribe((editionsResult) => {
                                this.EditValues.editions = editionsResult.items;
                                let notSelectedEdition =
                                    new SubscribableEditionComboboxItemDto();
                                notSelectedEdition.displayText =
                                    this.l("NotAssigned");
                                notSelectedEdition.value = "";
                                this.EditValues.editions.unshift(
                                    notSelectedEdition
                                );
                                this.getDataForEdit(this.ViewValues.tenantId);
                                this.EditValues.isLoaded = true;
                            });
                    });
            }
        } else if (this.tabIndex == 3) {
            if (!this.FeatureValues.isLoaded) {
                this.loadFeatures();
                this.FeatureValues.isLoaded = true;
            } else {
                setTimeout(() => {
                    this.featureTree.editData = this.FeatureValues.editData;
                }, 300);
            }
        } else if (this.tabIndex == 4) {
            if (!this.ServiceValues.isLoaded) {
                this.ServiceValues.editTenantServicesForm = this.fb.group({
                    activeCheckbox: this.fb.array([]),
                });
                this.getTenatServices(this.ViewValues.tenantId);
            }
        }
    }

    onFileChange2(event) {
        const fileInput = event.target;
        const files = fileInput.files;

        if (files && files.length) {
            const [file] = files;

            // Check if the file is a PDF or an image
            const validImageTypes = ["image/jpeg", "image/png"];
            if (
                file.type === "application/pdf" ||
                validImageTypes.includes(file.type)
            ) {
                if (file.type === "application/pdf") {
                    this.isPDFView = true;
                } else {
                    console.log(this.isPDFView);
                    this.isPDFView = false;
                }
                // Proceed with handling the file
                const reader = new FileReader();
                reader.readAsDataURL(file);

                let form = new FormData();
                form.append("FormFile", file);
                this._itemsServiceProxy.getFileUrl(form).subscribe((res) => {
                    this.ViewValues.uploadedFileUrl = res["result"];
                    this.ViewValues.isContractUpload = true;
                    if (this.ViewValues?.uploadedFileUrl?.length > 0) {
                        this.uploadedViewFileUrl =
                            this.sanitizer.bypassSecurityTrustResourceUrl(
                                this.ViewValues.uploadedFileUrl
                            );
                    }
                });
            } else {
                // Handle the case when the file is not a PDF or an image
                alert("Please upload a valid PDF or image file.");
            }
        }
    }

    ViewSave(): Observable<boolean> {
        return this._tenantService
            .updateTenantFile(
                this.ViewValues.uploadedFileUrl,
                this.ViewValues.tenantId
            )
            .pipe(
                tap(() => {
                    this.toastr.info(
                        "Contract Saved Successfully 🚀",
                        "Contract Save Status",
                        {
                            positionClass: "toast-bottom-right",
                            timeOut: 3000,
                            progressBar: true,
                        }
                    );
                }),
                map(() => true), // Return true on success
                catchError((error) => {
                    console.error("Error saving contract:", error);
                    return of(false); // Return false on error
                })
            );
    }

    handleTextInputSettings(valueName: string, event: Event) {
        // Cast the event target to HTMLInputElement to access value
        const inputElement = event.target as HTMLInputElement;
        const inputValue = inputElement.value;

        this.SettingsValues.isModified = true;

        if (valueName == "TZ") {
            this.SettingsValues.timeZone = inputValue;
        } else if (valueName === "TCW") {
            const parsedValue = parseFloat(inputValue); // Use parseFloat to allow decimal values
            if (!isNaN(parsedValue)) { // Check if parsedValue is a valid number
                this.SettingsValues.totalCustomerWallet = parsedValue;
            }  else {
                inputElement.value = "";
            }
        } else if (valueName == "ZCI") {
            this.SettingsValues.zohoCustomerId = inputValue;
        } else if (valueName == "CD") {
            const parsedValue = Number(inputValue);
            if (Number.isInteger(parsedValue)) {
                this.SettingsValues.cautionDays = parsedValue;
            } else {
                inputElement.value = "";
            }
        } else if (valueName == "WD") {
            const parsedValue = Number(inputValue);
            if (Number.isInteger(parsedValue)) {
                this.SettingsValues.warningDays = parsedValue;
            } else {
                inputElement.value = "";
            }
        } else if (valueName == "CIA") {
            this.SettingsValues.clientIpAddress = inputValue;
        }
    }

    handleChangeInputSettings(valueName: string, event: Event) {
        if (valueName == "C") {
            this.SettingsValues.isModified = true;
            if (
                this.SettingsValues.currency != null ||
                this.SettingsValues.currency != ""
            ) {
                this.errors.currencyError = false;
            }
            return;
        }
        const inputElement = event.target as HTMLInputElement;
        const isChecked = inputElement.checked;
        this.SettingsValues.isModified = true;

        if (valueName == "IPO") {
            this.SettingsValues.isPreOrder = isChecked;
        } else if (valueName == "IP") {
            this.SettingsValues.isPickup = isChecked;
        } else if (valueName == "ID") {
            this.SettingsValues.isDelivery = isChecked;
        }
    }

    SettingsSave(): Observable<boolean> {
        const settings: SettingsTenantHostModel = new SettingsTenantHostModel();
        settings.timeZone = this.SettingsValues.timeZone;
        settings.currency = this.SettingsValues.currency;
        settings.totalCustomerWallet = this.SettingsValues.totalCustomerWallet;
        settings.zohoCustomerId = this.SettingsValues.zohoCustomerId;
        settings.cautionDays = this.SettingsValues.cautionDays;
        settings.warningDays = this.SettingsValues.warningDays;
        settings.clientIpAddress = this.SettingsValues.clientIpAddress;
        settings.isPreOrder = this.SettingsValues.isPreOrder;
        settings.isPickup = this.SettingsValues.isPickup;
        settings.isDelivery = this.SettingsValues.isDelivery;
        settings.tenantId = this.ViewValues.tenantId;

        return this._tenantDashboardServiceProxy
            .updateSettingsTenantHost(settings)
            .pipe(
                tap(() => {
                    this.toastr.info(
                        "Tenant Settings Saved Successfully 🚀",
                        "Tenant Settings Save Status",
                        {
                            positionClass: "toast-bottom-right",
                            timeOut: 3000,
                            progressBar: true,
                        }
                    );
                }),
                map(() => true), // Return true on success
                catchError((error) => {
                    console.error("Error saving tenant settings:", error);
                    return of(false); // Return false on error
                })
            );
    }

    getDataForEdit(tenantId) {
        this._tenantService
            .getTenantForEdit(tenantId)
            .subscribe((tenantResult) => {
                this.EditValues.tenant = tenantResult;
                this.EditValues.isUnlimited =
                    !this.EditValues.tenant.subscriptionEndDateUtc;
                if (
                    this.EditValues.tenant.name == null ||
                    this.EditValues.tenant.name.length <= 0
                ) {
                    this.errors.editErrorCN = true;
                }

                if (
                    this.EditValues.tenant.legalID == null ||
                    this.EditValues.tenant.legalID.length <= 0
                ) {
                    this.errors.editErrorLI = true;
                }

                if (
                    this.EditValues.tenant.website == null ||
                    this.EditValues.tenant.website.length <= 0
                ) {
                    this.errors.editErrorTW = true;
                }

                if (
                    this.EditValues.tenant.phoneNumber == null ||
                    this.EditValues.tenant.phoneNumber.length <= 0
                ) {
                    this.errors.editErrorPN = true;
                } else {
                    this.errors.editErrorPN = false;
                }

                if (
                    this.EditValues.tenant.address == null ||
                    this.EditValues.tenant.address.length <= 0
                ) {
                    this.errors.editErrorA = true;
                } else {
                    this.errors.editErrorA = false;
                }

                if (this.EditValues.tenant.tenantType == null) {
                    this.errors.editErrorTT = true;
                } else {
                    this.errors.editErrorTT = false;
                }

                if (this.EditValues.tenant.botTemplateId == null) {
                    this.errors.editErrorTN = true;
                } else {
                    this.errors.editErrorTN = false;
                }

                if (this.EditValues.tenant.botLocal == null) {
                    this.errors.editErrorTL = true;
                } else {
                    this.errors.editErrorTL = false;
                }

                if (
                    this.EditValues.tenant.whatsAppAccountID == null ||
                    this.EditValues.tenant.whatsAppAccountID.length <= 0
                ) {
                    this.errors.editErrorWBL = true;
                } else {
                    this.errors.editErrorWBL = false;
                }

                if (
                    this.EditValues.tenant.whatsAppAppID == null ||
                    this.EditValues.tenant.whatsAppAppID.length <= 0
                ) {
                    this.errors.editErrorWAI = true;
                } else {
                    this.errors.editErrorWAI = false;
                }

                if (this.EditValues.tenant.biDailyLimit == null) {
                    this.errors.editErrorBDL = true;
                } else {
                    this.errors.editErrorBDL = false;
                }

                if (
                    this.EditValues.tenant.d360Key == null ||
                    this.EditValues.tenant.d360Key.length <= 0
                ) {
                    this.errors.editErrorPI = true;
                } else {
                    this.errors.editErrorPI = false;
                }

                if (
                    this.EditValues.tenant.botId == null ||
                    this.EditValues.tenant.botId.length <= 0
                ) {
                    this.errors.editErrorBI = true;
                } else {
                    this.errors.editErrorBI = false;
                }

                if (this.EditValues.tenant.dueDay == null) {
                    this.errors.editErrorIN = true;
                } else {
                    this.errors.editErrorIN = false;
                }

                if (
                    this.EditValues.tenant.accessToken == null ||
                    this.EditValues.tenant.accessToken.length <= 0
                ) {
                    this.errors.editErrorAT = true;
                } else {
                    this.errors.editErrorAT = false;
                }

                if (
                    this.EditValues.tenant.directLineSecret == null ||
                    this.EditValues.tenant.directLineSecret.length <= 0
                ) {
                    this.errors.editErrorDLS = true;
                } else {
                    this.errors.editErrorDLS = false;
                }


                if (
                    this.EditValues.tenant.merchantID == null ||
                    this.EditValues.tenant.merchantID.length <= 0
                ) {
                    this.errors.editErrorMI = false;
                } else {
                    this.errors.editErrorMI = false;
                }



                
            });
    }

    saveEdit(): Observable<boolean> {

        // Prepare tenant data
        if (this.EditValues.tenant.editionId === 0) {
            this.EditValues.tenant.editionId = null;
        }

        if (this.EditValues.isUnlimited) {
            this.EditValues.tenant.isInTrialPeriod = false;
        }

        if (this.EditValues.isUnlimited || !this.EditValues.tenant.editionId) {
            this.EditValues.tenant.subscriptionEndDateUtc = null;
        }

        return this._tenantService.updateTenant(this.EditValues.tenant).pipe(
            tap(() => {
                this.toastr.info(
                    "Edit Save Successfully 🚀",
                    "Edit Save Status",
                    {
                        positionClass: "toast-bottom-right",
                        timeOut: 3000,
                        progressBar: true,
                    }
                );
            }),
            map(() => true), // Return true on success
            catchError((error) => {
                console.error("Error saving edit:", error);
                return of(false); // Return false on error
            })
        );
    }

    handleModificationEdit(Iname: string, event: Event) {
        if (Iname == "CN") {
            if (
                this.EditValues.tenant.name == null ||
                this.EditValues.tenant.name.length <= 0
            ) {
                this.errors.editErrorCN = true;
            }
        } else if (Iname == "LI") {
            if (
                this.EditValues.tenant.legalID == null ||
                this.EditValues.tenant.legalID.length <= 0
            ) {
                this.errors.editErrorLI = true;
            }
        } else if (Iname == "TW") {
            if (
                this.EditValues.tenant.website == null ||
                this.EditValues.tenant.website.length <= 0
            ) {
                this.errors.editErrorTW = true;
            }
        } else if (Iname == "PN") {
            if (
                this.EditValues.tenant.phoneNumber == null ||
                this.EditValues.tenant.phoneNumber.length <= 0
            ) {
                this.errors.editErrorPN = true;
            } else {
                this.errors.editErrorPN = false;
            }
        } else if (Iname == "A") {
            if (
                this.EditValues.tenant.address == null ||
                this.EditValues.tenant.address.length <= 0
            ) {
                this.errors.editErrorA = true;
            } else {
                this.errors.editErrorA = false;
            }
        } else if (Iname == "TT") {
            if (this.EditValues.tenant.tenantType == null) {
                this.errors.editErrorTT = true;
            } else {
                this.errors.editErrorTT = false;
            }
        } else if (Iname == "TN") {
            if (this.EditValues.tenant.botTemplateId == null) {
                this.errors.editErrorTN = true;
            } else {
                this.errors.editErrorTN = false;
            }
        } else if (Iname == "TL") {
            if (this.EditValues.tenant.botLocal == null) {
                this.errors.editErrorTL = true;
            } else {
                this.errors.editErrorTL = false;
            }
        } else if (Iname == "WBL") {
            if (
                this.EditValues.tenant.whatsAppAccountID == null ||
                this.EditValues.tenant.whatsAppAccountID.length <= 0
            ) {
                this.errors.editErrorWBL = true;
            } else {
                this.errors.editErrorWBL = false;
            }
        } else if (Iname == "WAI") {
            if (
                this.EditValues.tenant.whatsAppAppID == null ||
                this.EditValues.tenant.whatsAppAppID.length <= 0
            ) {
                this.errors.editErrorWAI = true;
            } else {
                this.errors.editErrorWAI = false;
            }
        } else if (Iname == "BDL") {
            if (this.EditValues.tenant.biDailyLimit == null) {
                this.errors.editErrorBDL = true;
            } else {
                this.errors.editErrorBDL = false;
            }
        } else if (Iname == "PI") {
            if (
                this.EditValues.tenant.d360Key == null ||
                this.EditValues.tenant.d360Key.length <= 0
            ) {
                this.errors.editErrorPI = true;
            } else {
                this.errors.editErrorPI = false;
            }
        } else if (Iname == "BI") {
            if (
                this.EditValues.tenant.botId == null ||
                this.EditValues.tenant.botId.length <= 0
            ) {
                this.errors.editErrorBI = true;
            } else {
                this.errors.editErrorBI = false;
            }
        } else if (Iname == "IN") {
            if (this.EditValues.tenant.dueDay == null) {
                this.errors.editErrorIN = true;
            } else {
                this.errors.editErrorIN = false;
            }
        } else if (Iname == "AT") {
            if (
                this.EditValues.tenant.accessToken == null ||
                this.EditValues.tenant.accessToken.length <= 0
            ) {
                this.errors.editErrorAT = true;
            } else {
                this.errors.editErrorAT = false;
            }
        } else if (Iname == "DLS") {
            if (
                this.EditValues.tenant.directLineSecret == null ||
                this.EditValues.tenant.directLineSecret.length <= 0
            ) {
                this.errors.editErrorDLS = true;
            } else {
                this.errors.editErrorDLS = false;
            }
        }else if (Iname == "MI") {
            if (
                this.EditValues.tenant.merchantID == null ||
                this.EditValues.tenant.merchantID.length <= 0
            ) {
                this.errors.editErrorMI = false;
            } else {
                this.errors.editErrorMI = false;
            }
        }
        else if(Iname == "BI"){}
        else if(Iname == "CL"){}
        else if(Iname == "CAT"){}
        this.EditValues.isModified = true;
    }

    loadFeatures(): void {
        this._tenantService
            .getTenantFeaturesForEdit(this.ViewValues.tenantId)
            .subscribe((result) => {
                this.FeatureValues.editData = result;
                this.featureTree.editData = result;
            });
    }

    resetFeatures(): void {
        const input = new EntityDto();
        input.id = this.ViewValues.tenantId;

        this.FeatureValues.resettingFeatures = true;
        this._tenantService
            .resetTenantSpecificFeatures(input)
            .pipe(
                finalize(() => (this.FeatureValues.resettingFeatures = false))
            )
            .subscribe(() => {
                this.toastr.info(
                    "Feature Reset Successfully 🚀",
                    "Feature Reset Status",
                    {
                        positionClass: "toast-bottom-right",
                        timeOut: 3000,
                        progressBar: true,
                    }
                );
                this.loadFeatures();
            });
    }

    FeatureSave(): Observable<boolean> {
        if (!this.featureTree.areAllValuesValid()) {
            this.message.warn(this.l("InvalidFeaturesWarning"));
            return of(false); // Return false if values are invalid
        }

        const input = new UpdateTenantFeaturesInput();
        input.id = this.ViewValues.tenantId;
        input.featureValues = this.featureTree.getGrantedFeatures();

        return this._tenantService.updateTenantFeatures(input).pipe(
            tap(() => {
                this.toastr.info(
                    "Feature Saved Successfully 🚀",
                    "Feature Save Status",
                    {
                        positionClass: "toast-bottom-right",
                        timeOut: 3000,
                        progressBar: true,
                    }
                );
            }),
            map(() => true), // Return true on success
            catchError((error) => {
                console.error("Error saving features:", error);
                return of(false); // Return false on error
            })
        );
    }

    getTenatServices(tenantId): void {
        this.ServiceValues.activeCheckboxFormArray = <UntypedFormArray>(
            this.ServiceValues.editTenantServicesForm.controls.activeCheckbox
        );

        this._tenantServiceService
            .getTenatServices(tenantId)
            .subscribe((result) => {
                this.ServiceValues.services = result;

                this.ServiceValues.services.forEach((element) => {
                    if (element.isSelected) {
                        this.ServiceValues.activeCheckboxFormArray.push(
                            new UntypedFormControl(element)
                        );
                    } else {
                        let index =
                            this.ServiceValues.activeCheckboxFormArray.controls.findIndex(
                                (x) => x.value == element
                            );
                        if (index >= 0)
                            this.ServiceValues.activeCheckboxFormArray.removeAt(
                                index
                            );
                    }
                });
            });
    }

    saveServices(): Observable<boolean> {
        return this._tenantServiceService
            .updateTenantService(this.ServiceValues.services)
            .pipe(
                tap(() => {
                    this.toastr.info(
                        "Service Saved Successfully 🚀",
                        "Service Save Status",
                        {
                            positionClass: "toast-bottom-right",
                            timeOut: 3000,
                            progressBar: true,
                        }
                    );
                }),
                map(() => true), // If the service call is successful, map to true
                catchError((error) => {
                    console.error("Error saving services:", error);
                    return of(false); // Return false on error
                })
            );
    }

    handleServiceIsModified(event: Event) {
        this.ServiceValues.isModified = true;
    }

    checkEditErrors(): boolean {
        if (this.errors.editErrorA) {
            return true;
        }
        if (this.errors.editErrorAT) {
            return true;
        }
        if (this.errors.editErrorBDL) {
            return true;
        }
        if (this.errors.editErrorBI) {
            return true;
        }
        if (this.errors.editErrorCN) {
            return true;
        }
        if (this.errors.editErrorDLS) {
            return true;
        }
        if (this.errors.editErrorIN) {
            return true;
        }
        if (this.errors.editErrorLI) {
            return true;
        }
        if (this.errors.editErrorPI) {
            return true;
        }
        if (this.errors.editErrorPN) {
            return true;
        }
        if (this.errors.editErrorTL) {
            return true;
        }
        if (this.errors.editErrorTN) {
            return true;
        }
        if (this.errors.editErrorTT) {
            return true;
        }
        if (this.errors.editErrorTW) {
            return true;
        }
        if (this.errors.editErrorWAI) {
            return true;
        }
        if (this.errors.editErrorWBL) {
            return true;
        }
        if (this.errors.editErrorMI) {
            return true;
        }
        return false;
    }
}
