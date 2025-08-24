    declare var gapi: any;
    declare var google: any;
import {
    Component,
    ElementRef,
    EventEmitter,
    HostListener,
    Injector,
    Input,
    OnInit,
    Output,
    Renderer2,
    ViewChild,
    ViewEncapsulation,
} from "@angular/core";
import { FormArray, FormControl, FormGroup, ValidationErrors ,} from "@angular/forms";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ModalDirective } from "ngx-bootstrap/modal";
import moment, { Moment } from "moment";
import {
    AreasServiceProxy,
    BotFlowServiceProxy,
    BotParameterModel,
    ConditionList,
    ConditionalModel,
    ContentInfo,
    Dtocontent,
    GetBotFlowForViewDto,
    GoogleSheetIntegrationModel,
    ImageFlowModel,
    ItemsServiceProxy,
    MessageTemplateModel,
    PagedResultDtoOfBotParameterModel,
    ParameterSet,
    RType,
    RequestModel,
    ScheduleModel,
    TeamsDtoModel,
    TeamsServiceProxy,
    UserListDto,
    UserServiceProxy,
    WhatsAppComponentModel,
    WhatsAppMessageTemplateServiceProxy,
    TenantSettingsServiceProxy,
    Cacatalog,
    TemplateFooter,
    TemplateBody,
    TemplateHeader,
    CatalogTemplateDto,
    TenantServiceProxy,
    ProductDto,
    CatalogueDto
} from "@shared/service-proxies/service-proxies";
import { Validators } from "@angular/forms";
import { CreateParameterComponent } from "./create-parameter/create-parameter.component";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import {
    CdkDragDrop,
    moveItemInArray,
    transferArrayItem,
} from "@angular/cdk/drag-drop";
import uniqid from "uniqid";
import { AreaDto, DtoContent } from "../../bot-builder-dialog.model";
import { DropdownFilterOptions } from "primeng/dropdown";
import { TabView } from "primeng/tabview";
import { FlatpickrOptions } from "ng2-flatpickr";
import { SelectItem } from "@node_modules/primeng/api";
import { Catalog, CatalogService, Product } from "../catalog.service";
// import { NgbDropdown } from "@ng-bootstrap/ng-bootstrap";

function noWhitespaceValidator(control: FormControl): ValidationErrors | null {
    const isWhitespace = (control.value || "").trim().length === 0;
    const isValid = !isWhitespace;
    return isValid ? null : { whitespace: true };
}
@Component({
    selector: "createBotDialog",
    templateUrl: "./create-dialog.component.html",
    styleUrls: ["./create-dialog.component.scss"],
})
export class CreateDialogComponent extends AppComponentBase {
    @ViewChild("createBotDialog", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Output() modalClose: EventEmitter<any> = new EventEmitter<any>();
    @Input() branches: AreaDto[] = [];
    @Input() branchesEN: AreaDto[] = [];
    @Input() nodes: any;
    @ViewChild("createParameter", { static: true })
    listOfJumNodes = [];
    createParameter: CreateParameterComponent;
    hideEmoji = true;
    hideEmojiArabic = true;
    imageUrl = "";
    bodyLength = 1024;

    isData: boolean = true;
    isNow: boolean = false;
    numberButton: number = null;
    unavailableDate: string | undefined = undefined;
    startDate: moment.Moment | undefined = undefined;
    endDate: moment.Moment | undefined = undefined;

    date: Date | undefined;

    minDate: Date | undefined;

    maxDate: Date | undefined;

    selectedDates: Date[] = [];

    selectedFirstTime: Date = new Date();
    selectedEndTime: Date = new Date();
    activeTab: 'received' | 'product' = 'received';
    catalogMultiProductModel: any = {};
    catalogSingleProductModel: any = {};

    fakeCatalogs = [
    { id: 'CAT001', name: 'Food Catalog' }
    ];

        catalog: any[] = []; 


    // public endDateOptions = {
    //     altInput: true,
    //     mode: "single",
    //     altInputClass:
    //         "form-control flat-picker flatpickr-input invoice-edit-input",
    //     enableTime: false,
    //     onClose: (selectedDates: any) => {

    //         // this.getDate(selectedDates[0]);
    //     },
    // };

    // availableProducts = [
    // { productId: 'B001', name: 'Burger', description: '', price: '', currency: '', imageUrl: '' },
    // { productId: 'S002', name: 'Shawarma', description: '', price: '', currency: '', imageUrl: '' },
    // { productId: 'P003', name: 'Pizza', description: '', price: '', currency: '', imageUrl: '' },
    // { productId: 'T006', name: 'Pasta', description: '', price: '', currency: '', imageUrl: '' },
    // { productId: 'IC09', name: 'Salad', description: '', price: '', currency: '', imageUrl: '' },
    //     ];
    availableProducts: any[] = [];
        // catalog!: Catalog;
        selectedType: 'single' | 'multi' = 'single';
        selectedProduct?: Product;
        selectedProducts: Product[] = [];
        
    pickerOptions: FlatpickrOptions = {
        altInput: true,
        mode: "range",
        dateFormat: "Y-m-d",
        altInputClass:
            "form-control flat-picker flatpickr-input invoice-edit-input",
        onClose: (selectedDates: any) => {
            this.getDayesDate(selectedDates[0], selectedDates[1]);
        },
        minDate: new Date(),
        maxDate: null,
    };

    startOptions: FlatpickrOptions = {
        mode: "single",
        inline: true,

        enableTime: true,
        noCalendar: false,
        dateFormat: "H:i",
        time_24hr: true,
        minDate: new Date(Date.now()),
    };
    fb: any;

    onDateChange(event: any) {
        // Convert selected dates to Moment.js objects
        const momentDates = this.selectedDates.map((date: Date) =>
            moment(date)
        );

        // Format dates and join them into a string
        const formattedDates = momentDates.map((date) =>
            date.format("YYYY-MM-DD")
        );
        const concatenatedString = formattedDates.join(",");
    }

    dialogForm = new FormGroup({
        captionEn: new FormControl("", [
            Validators.required,
            noWhitespaceValidator,
        ]),
        captionAr: new FormControl(""),
        value: new FormControl("on"),
        // headersTextEn: new FormControl(""),
        // headersTextAr: new FormControl(""),
        footerTextEn: new FormControl(""),
        footerTextAr: new FormControl(""),
        top: new FormControl(0),
        left: new FormControl(0),
        bottom: new FormControl(0),
        right: new FormControl(0),
        parameter: new FormControl(""),
        type: new FormControl({ value: "", disabled: true }),
        childIndex: new FormControl(),
        parentIndex: new FormControl(),
        childId: new FormControl(),
        parentId: new FormControl(),
        id: new FormControl(),
        rangeDatesSchedual: new FormControl(),
        isRoot: new FormControl(false),
        urlImageArray: new FormControl([]),
        dilationTime: new FormControl<number>(0, { nonNullable: true }),
        listOfUsers: new FormControl(""),
        listOfTeams: new FormControl(""),
        parameterList: new FormControl([]),
        actionBlock: new FormControl(""),
        inputHint: new FormControl("", Validators.required),
        isOneTimeQuestion: new FormControl(false),
        isAdvance: new FormControl(false),
        hintForReplay: new FormControl(""),
        templateId: new FormControl(""),
        content: new FormControl(new ContentInfo()),
        urlImage: new FormControl(""),
        parameterType: new FormControl(""),
        title: new FormControl("", [
            Validators.required,
            noWhitespaceValidator,
        ]),
        jumpId: new FormControl<number>(0, { nonNullable: true }),
        request: new FormControl(),
        schedule: new FormControl(),
        condition: new FormControl(new ConditionalModel()),
        integrationType: new FormControl(""),
        googleSheetAction: new FormControl(""),
        spreadSheetName: new FormControl(""),
        spreadSheetId: new FormControl(""),
        workSheet: new FormControl(""),
        lookupColumn: new FormControl(""),
        lookupValue: new FormControl(""),
        parameters: new FormArray([]),
        worksheetColumns: new FormArray([]),
        
        headerTextEn: new FormControl(""),  
        headerTextAr: new FormControl(""),  
        // bodyTextEn: new FormControl('', Validators.required),
        bodyTextEn: new FormControl(""),
        bodyTextAr: new FormControl(""),    
        // footerTextEn: new FormControl(""),  
        // footerTextAr: new FormControl(""),  
        // catalogId: new FormControl('', Validators.required),
        catalogId: new FormControl(""),
        sectionTitle: new FormControl(""),
        // sectionTitle: new FormControl('', Validators.required),
        catalogName: new FormControl(""),
        products: new FormArray([]),
        // selectedProducts: new FormControl([], Validators.required),
        selectedProducts: new FormControl([]),

    });

    get parameters(): FormArray {
        return this.dialogForm.get("parameters") as FormArray;
    }
    get columns(): FormArray {
        return this.dialogForm.get("worksheetColumns") as FormArray;
    }

    listOfImages: any[] = [];
    type: any;
    listOfButtons: Dtocontent[] = [];
    index: any;
    showLoading = false;
    public selectMultiUsers: UserListDto[] = [];
    public selectMultiTeams: TeamsDtoModel[] = [];
    dropdownSettings = {};
    dropdownTeamsSettings = {};
    dropdownSettingsHuman = {};

    isUnavailbleDayActive: boolean = false;
    request = new RequestModel();
    isArabic = false;
    listOfOperations: any[] = [
        "Less tham",
        "Less or equal to",
        "Greater than",
        "Equal to",
        "Not equal to",
        "Contains",
        "Does not contain",
    ];
    listOfConditions: ConditionList[] = [];
    listOfParametr: ParameterSet[] = [];
    schedual!: ScheduleModel;
    orAndCondition = "Equal to";
    node = new GetBotFlowForViewDto();
    imagesList: any[] = [];
    condition: string = "AND";
    parentId!: number[] | undefined;
    parentIndex!: number[] | undefined;
    formattedStartDate!: Moment;
    formattedEndDate!: Moment;
    eventDate = null;
    stateOptions: any[] = [
        { label: "off", value: "off" },
        { label: "on", value: "on" },
    ];

    // value: string = "date";

    @ViewChild("emojiMart", { static: false }) emojiMart: ElementRef;

    @ViewChild("captionEn") myTextarea: ElementRef;
    @ViewChild("captionAr") myTextarea1: ElementRef;
    @ViewChild("filee", { static: false }) filee: ElementRef;
    @ViewChild("endDatePickerSelectDate") endDatePicker: ElementRef;
    listOfNodeWithVar: Map<number, { name: string; id: number }> = new Map<
        number,
        { name: string; id: number }
    >();

    // select icon
    public selectParameter: any[] = [];

    parameterForSetParameters: any[] = [];

    selectedTeamsIds: Array<any> = [];
    selectedUserIds: Array<any> = [];
    public selectedParametrs: Array<any> = [];

    //Template
    Templates: MessageTemplateModel[] = [new MessageTemplateModel()];
    selectedTemplate: MessageTemplateModel = new MessageTemplateModel();
    Component: WhatsAppComponentModel[] = [new WhatsAppComponentModel()];
    ComponentHeader: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentBody: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentFooter: WhatsAppComponentModel = new WhatsAppComponentModel();
    ComponentButton: WhatsAppComponentModel = new WhatsAppComponentModel();
    imageFlag: boolean = false;
    videoFlag: boolean = false;
    documentFlag: boolean = false;
    selectedTemplateId = 0;
    public safeUrl: SafeResourceUrl;
    rType: RType[];
    selectedVariable: string | undefined;
    // rangeDatesSchedual: Date[];
    filterValue: string | undefined = "";
    schedualTypevalue: string = "off";
    tabIndex: number = 0;
    diffInDays: number = 0;

    //google picker api
    readonly googleClientId = '492714879503-2ds4jcvd44ifrfnaicg4t7qodcek5rv2.apps.googleusercontent.com';
    readonly  googleApiKey  = 'AIzaSyAxQ3BbrWMwOhseHC30g8RwpixHc9elRCY';
    tokenClient: any;




    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private _itemsServiceProxy: ItemsServiceProxy,
        private renderer: Renderer2,
        // private dropdown: NgbDropdown,
        private el: ElementRef,
        private _BotFlowServiceProxy: BotFlowServiceProxy,
        private _userServiceProxy: UserServiceProxy,
        private _teamsService: TeamsServiceProxy,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        private sanitizer: DomSanitizer,
        private _areasServiceProxy: AreasServiceProxy,
        private _tenantSettingServiceProxy : TenantSettingsServiceProxy,
        private _tenantService: TenantServiceProxy ,
    ) {
        super(injector);

        // Initialize GIS token client here as you already do
        this.tokenClient = google.accounts.oauth2.initTokenClient({
            client_id: this.googleClientId,
            scope: 'https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/userinfo.email',
            callback: (tokenResponse: any) => {
                if (tokenResponse.error) {
                    console.error('Token error:', tokenResponse);
                    return;
                }
                this.loadPicker(tokenResponse.access_token);
            },
        });

        // Wait until gapi is loaded before loading client and picker APIs
        this.loadGapiApi().then(() => {
            gapi.load('picker', () => {
                console.log('Picker API loaded');
            });

        });
    }
    checkboxValues: string[] = [];

    ngOnInit(): void {

        this.getCatalogueID();  
        this.getProductfromCatalogue();

        // this.catalogService.getCatalog().subscribe(c => this.catalog = c);
        this.nodes;
        this.getParameteres();
        this.getTenantUsers();
        this.getTenantTeams();
        this.getTemplates();
        this.checkSchdularType();
        this.listOfButtons;
        this.dropdownTeamsSettings = {
            singleSelection: false,
            idField: "id",
            textField: "teamsName",
            itemsShowLimit: 3,
            allowSearchFilter: false,
            maxHeight: 200,
            closeDropDownOnSelection: true,
        };
        this.dropdownSettings = {
            singleSelection: false,
            idField: "id",
            textField: "fullName",
            itemsShowLimit: 3,
            allowSearchFilter: false,
            maxHeight: 200,
            closeDropDownOnSelection: true,
        };
        this.minDate = new Date();
        this.dropdownSettingsHuman = {
            singleSelection: false,
            idField: "id",
            textField: "name",
            itemsShowLimit: 3,
            allowSearchFilter: true,
            maxHeight: 200,
            closeDropDownOnSelection: true,
        };

        this.dialogForm.get("isAdvance").valueChanges.subscribe((value) => {
            if (value === false) {
                if (this.type === "List options") {
                    this.imagesList = [];
                    this.listOfImages = [];
                    this.listOfButtons.splice(10);
                    for (
                        let index = 0;
                        index < this.listOfButtons.length;
                        index++
                    ) {
                        this.listOfButtons[index].valueEn = this.listOfButtons[
                            index
                        ].valueEn.substring(0, 20);
                        this.listOfButtons[index].valueAr = this.listOfButtons[
                            index
                        ].valueAr.substring(0, 20);
                    }
                }
                if (this.type === "Reply buttons") {
                    for (
                        let index = 0;
                        index < this.listOfButtons.length;
                        index++
                    ) {
                        this.listOfButtons[index].valueEn = this.listOfButtons[
                            index
                        ].valueEn.substring(0, 20);
                        this.listOfButtons[index].valueAr = this.listOfButtons[
                            index
                        ].valueAr.substring(0, 20);
                    }
                }
            }
        });

        //integration related
        this.dialogForm
            .get("googleSheetAction")
            ?.valueChanges.subscribe((value) => {
                this.onActionChange(value);
            });

        this.dialogForm.get("spreadSheetId")?.valueChanges.subscribe((value) => {
            this.onSpreadSheetChange(value);
        });

        this.dialogForm.get("workSheet")?.valueChanges.subscribe((value) => {
            this.onWorkSheetChange(value);
        });

    }


    @HostListener("mousedown", ["$event"])
    onMouseDown(event: MouseEvent): void {
        event.stopPropagation();
        this.renderer.setStyle(this.el.nativeElement, "cursor", "default");
    }

    @HostListener("mousemove", ["$event"])
    onMouseMove(event: MouseEvent): void {
        event.stopPropagation();
        this.renderer.setStyle(this.el.nativeElement, "cursor", "default");
    }

    @HostListener("mouseup", ["$event"])
    onMouseUp(event: MouseEvent): void {
        event.stopPropagation();
        this.renderer.setStyle(this.el.nativeElement, "cursor", "default");
    }

    @HostListener("mouseleave", ["$event"])
    onMouseLeave(event: MouseEvent): void {
        event.stopPropagation();
        this.renderer.setStyle(this.el.nativeElement, "cursor", "default");
    }

    checkSchdularType(event = null) {
        if (!event) this.tabIndex = 0;
    }

    handleChangeUnavailavleDate(event) {
        this.isUnavailbleDayActive = !this.isUnavailbleDayActive;
    }

    getDayesDate(startDate: Date, endDate: Date) {
        if (startDate != null && endDate != null) {
            this.diffInDays = moment(endDate).diff(startDate, "days");
            this.formattedStartDate = moment(startDate);
            this.formattedEndDate = moment(endDate);
        }
    }
    addBranch(branch: AreaDto) {
        let button = new Dtocontent({
            key: this.stringToNumber(uniqid("IB-")),
            parentIndex:
                this.listOfButtons.length !== 0
                    ? this.listOfButtons[0].parentIndex
                    : this.parentIndex,
            childIndex: null,
            parentId:
                this.listOfButtons.length !== 0
                    ? this.listOfButtons[0].parentId
                    : this.parentId,
            childId: null,
            valueEn: branch.areaNameEnglish,
            valueAr: branch.areaName,
            branchID: branch.id.toString(),
        });
        this.listOfButtons.push(button);
        debugger;
        for (let i = 0; i < this.branches.length; i++) {
            if (this.branches[i].id === branch.id) {
                this.branches[i].checked = !this.branches[i].checked;
                break;
            }
        }
    }
        addBranchEN(branchEN: AreaDto) {
        let button = new Dtocontent({
            key: this.stringToNumber(uniqid("IB-")),
            parentIndex:
                this.listOfButtons.length !== 0
                    ? this.listOfButtons[0].parentIndex
                    : this.parentIndex,
            childIndex: null,
            parentId:
                this.listOfButtons.length !== 0
                    ? this.listOfButtons[0].parentId
                    : this.parentId,
            childId: null,
            valueEn: branchEN.areaName,
            valueAr: branchEN.areaName,
            branchID: branchEN.id.toString(),
        });
        this.listOfButtons.push(button);
        debugger;
        for (let i = 0; i < this.branchesEN.length; i++) {
            if (this.branchesEN[i].id === branchEN.id) {
                this.branchesEN[i].checked = !this.branchesEN[i].checked;
                break;
            }
        }
    }
    removeBranch(branch: AreaDto) {
        const indexToRemove = this.listOfButtons.findIndex(
            (button) => button.branchID == branch.id.toString()
        );

        if (indexToRemove !== -1) {
            this.listOfButtons.splice(indexToRemove, 1);
        }
        debugger;
        for (let i = 0; i < this.branches.length; i++) {
            if (this.branches[i].id === branch.id) {
                this.branches[i].checked = !this.branches[i].checked;
                break;
            }
        }
    }
        removeBranchEN(branch: AreaDto) {
        const indexToRemove = this.listOfButtons.findIndex(
            (button) => button.branchID == branch.id.toString()
        );

        if (indexToRemove !== -1) {
            this.listOfButtons.splice(indexToRemove, 1);
        }
        debugger;
        for (let i = 0; i < this.branchesEN.length; i++) {
            if (this.branchesEN[i].id === branch.id) {
                this.branchesEN[i].checked = !this.branchesEN[i].checked;
                break;
            }
        }
    }
    toggleBranch(branch: AreaDto): void {
        debugger;
        if (branch.checked) {
            this.removeBranch(branch);
        } else {
            this.addBranch(branch);
        }
    }
        toggleBranchEN(branch: AreaDto): void {
        debugger;
        if (branch.checked) {
            this.removeBranchEN(branch);
        } else {
            this.addBranchEN(branch);
        }
    }

    resetFunction(options: DropdownFilterOptions) {
        options.reset();
        this.filterValue = "";
    }

    customFilterFunction(event: KeyboardEvent, options: DropdownFilterOptions) {
        options.filter(event);
    }
    closeDropDown() { }
    add_removeBR(id) {
        for (let i = 0; i < this.listOfButtons.length; i++) {
            if (this.listOfButtons.indexOf(this.listOfButtons[i]) === id) {
                this.listOfButtons.splice(i, 1);
                break;
            }
        }
    }
    findObjectsNotInArray2(branches: AreaDto[], buttons: Dtocontent[]): any[] {
        debugger
        return buttons.filter(
            (button) =>
                !branches.some(
                    (branch) => branch.id.toString() == button.branchID
                )
        );
    }
    //     findObjectsNotInArray2EN(branchesEN: AreaDto[], buttons: Dtocontent[]): any[] {
    //     debugger
    //     return buttons.filter(
    //         (button) =>
    //             !branchesEN.some(
    //                 (branch) => branch.id.toString() == button.branchID
    //             )
    //     );
    // }
    rangeDates: Date[] | undefined;
    show(node) {
        this.listOfButtons = [];
        this.listOfImages = [];
        this.imagesList = [];
        this.listOfConditions = [];
        this.listOfParametr = [];
        this.schedual = null;

        this.showLoading = false;
        this.eventDate = null;
        this.showNode = true;

        this.request = new RequestModel();
        this.node = node;
        if (node.urlImageArray != null || node.urlImageArray != undefined) {
            if (node.urlImageArray.length > 0) {
                node.urlImageArray.forEach((element) => {
                    if (element.name === null) {
                        node.urlImageArray = null;
                    } else if (
                        element.url === null ||
                        element.url === undefined
                    ) {
                        node.urlImageArray = null;
                    }
                });
            }
        }

        this.selectedTemplate = new MessageTemplateModel();
        this.selectedTemplateId = 0;
        this.dialogForm.reset({
            captionEn: "",
            captionAr: "",
            footerTextEn: "",
            // headerTextEn:"",
            // headerTextAr:"",
            inputHint: "",
            isOneTimeQuestion: false,
            isAdvance: false,
            footerTextAr: "",
            top: 0,
            left: 0,
            bottom: 0,
            right: 0,
            parameter: "",
            type: "",
            childIndex: null,
            parentIndex: null,
            id: null,
            isRoot: false,
            content: new ContentInfo(),
            urlImage: "",
        });
        this.dialogForm.get("parameter").setValidators([Validators.required]);
        this.dialogForm.patchValue(node);
        this.dialogForm.get("actionBlock").clearValidators();
        this.dialogForm.get("actionBlock").updateValueAndValidity();
        this.dialogForm.get("listOfUsers").clearValidators();
        this.dialogForm.get("listOfUsers").updateValueAndValidity();

        this.dialogForm.get("listOfTeams").clearValidators();
        this.dialogForm.get("listOfTeams").updateValueAndValidity();

        this.dialogForm.get("templateId").clearValidators();
        this.dialogForm.get("templateId").updateValueAndValidity();
        this.dialogForm.get("jumpId").clearValidators();
        this.dialogForm.get("jumpId").updateValueAndValidity();
        this.dialogForm.get("jumpId").clearValidators();
        this.dialogForm.get("jumpId").updateValueAndValidity();
        this.dialogForm.controls["condition"].setValue(node.conditional);
        this.dialogForm.controls["parameterList"].setValue(node.parameterList);
        this.dialogForm.value.condition.conditionList.forEach((element) => {
            this.listOfConditions.push(element);
        });
        this.dialogForm.value.parameterList?.forEach((element) => {
            this.listOfParametr.push(element);
        });
        this.type = node.type;
        if (this.type === "Language") {
            this.dialogForm.controls["parameter"].disable();
            this.dialogForm.get("parameter").updateValueAndValidity();
        } else {
            this.dialogForm.controls["parameter"].enable();
        }
        if (
            this.type == "Reply buttons" ||
            this.type === "List options" ||
            this.type === "Branches" ||
            this.type === "BranchesEN" ||
            this.type === "Language"
        ) {
            debugger
            this.dialogForm.controls["content"].setValue(
                node.content.dtoContent
            );
            this.imageUrl = node.urlImage;
            this.dialogForm
                .get("parameter")
                .setValidators([Validators.required]);
            this.dialogForm.get("parameter").updateValueAndValidity();
            this.dialogForm.controls["content"].setValue(node.content);
            this.dialogForm.value.content.dtoContent.forEach((element) => {
                this.listOfButtons.push(element);
            });
            if (this.type === "Branches") {
                debugger
                this.parentId =
                    this.dialogForm.value.content.dtoContent[0].parentId || [];
                this.parentIndex =
                    this.dialogForm.value.content.dtoContent[0].parentIndex ||
                    [];
                this.branches.forEach((branch) => {
                    debugger;
                    branch.checked = this.listOfButtons.some(
                        (button) => branch.id.toString() == button.branchID
                    );
                });
            }
             if (this.type === "BranchesEN") {
                debugger;
                this.parentId =
                    this.dialogForm.value.content.dtoContent[0].parentId || [];
                this.parentIndex =
                    this.dialogForm.value.content.dtoContent[0].parentIndex ||
                    [];
                this.branchesEN.forEach((branch) => {
                    debugger;
                    branch.checked = this.listOfButtons.some(
                        (button) => branch.id.toString() == button.branchID
                    );
                });
            }
        }
        if (this.type === "Send Message") {
            this.dialogForm.get("parameter").clearValidators();
            this.dialogForm.get("parameter").updateValueAndValidity();
            this.bodyLength = 4000;
        } else {
            this.bodyLength = 1024;
        }
        if (this.type === "Collect input") {
            this.dialogForm
                .get("parameter")
                .setValidators([Validators.required]);
            this.dialogForm.get("parameter").updateValueAndValidity();
        }
        if (this.type == "Set Parameter") {
            this.dialogForm.get("parameter").clearValidators();
            this.dialogForm.get("parameter").updateValueAndValidity();
        }
        if (this.type == "ScheduleNode") {
            this.tabIndex = 0;
            // this.dialogForm.get("parameter").clearValidators();
            // this.dialogForm.get("parameter").updateValueAndValidity();
            if (node.schedule) {
                this.isData = node.schedule.isData;
                this.tabIndex = node.schedule.isData ? 0 : 1;
                this.isNow = node.schedule.isNow;
                if (node.schedule.unavailableDate) {
                    const dates = node.schedule.unavailableDate
                        .split(",")
                        .map((dateString) => new Date(dateString));

                    this.selectedDates = dates;
                    this.isUnavailbleDayActive = false;
                    this.isUnavailbleDayActive = true;
                } else {
                    this.selectedDates = [];
                    this.isUnavailbleDayActive = false;
                }

                if (!node.schedule.isNow) {
                    if (node.schedule?.isData) {
                        this.eventDate = [
                            node.schedule.startDate?.toDate(),
                            node.schedule.endDate?.toDate(),
                        ];
                        this.formattedStartDate = moment(
                            node.schedule.startDate?.toDate()
                        );
                        this.formattedEndDate = moment(
                            node.schedule.endDate?.toDate()
                        );
                    } else {
                        this.selectedFirstTime =
                            node.schedule.startDate?.toDate();
                        this.selectedEndTime = node.schedule.endDate?.toDate();
                    }
                } else {
                    this.numberButton = node.schedule.numberButton;
                }
            } else {
                this.isNow = false;
                this.numberButton = null;
                this.eventDate = [null, null];
                this.formattedStartDate = null;
                this.formattedEndDate = null;

                this.selectedFirstTime = new Date(Date.now());
                this.selectedEndTime = new Date(Date.now());
                this.isData = true;
                this.selectedDates = [];
                this.isUnavailbleDayActive = false;
            }
        }
        if (this.type === "Human handover") {
            this.dialogForm
                .get("actionBlock")
                .setValidators([Validators.required]);
            this.dialogForm.get("actionBlock").updateValueAndValidity();
            this.dialogForm
                .get("listOfUsers")
                .setValidators([Validators.required]);
            this.dialogForm.get("listOfUsers").updateValueAndValidity();

            this.dialogForm.get("listOfTeams").setValidators(null);
            this.dialogForm.get("listOfTeams").updateValueAndValidity();

            this.dialogForm.get("parameter").clearValidators();
            this.dialogForm.get("parameter").updateValueAndValidity();

            this.selectedUserIds = [];
            if (
                this.dialogForm.value.listOfUsers != undefined &&
                this.dialogForm.value.listOfUsers != null &&
                this.dialogForm.value.listOfUsers != ""
            ) {
                var array = this.dialogForm.value.listOfUsers.split(",");
                array.forEach((element) => {
                    var user = this.selectMultiUsers.find(
                        (x) => x.id == parseInt(element)
                    );
                    if (user) {
                        this.selectedUserIds.push(user);
                    }
                });
            }

            this.selectedTeamsIds = [];
            if (
                this.dialogForm.value.listOfTeams != undefined &&
                this.dialogForm.value.listOfTeams != null &&
                this.dialogForm.value.listOfTeams != ""
            ) {
                var array = this.dialogForm.value.listOfTeams.split(",");
                array.forEach((element) => {
                    var user = this.selectMultiTeams.find(
                        (x) => x.id == parseInt(element)
                    );
                    if (user) {
                        this.selectedTeamsIds.push(user);
                    }
                });
            }

            this.selectedParametrs = [];
            if (
                this.dialogForm.value.parameter != undefined &&
                this.dialogForm.value.parameter != null &&
                this.dialogForm.value.parameter != ""
            ) {
                var array2 = this.dialogForm.value.parameter.split(",");
                array2.forEach((element) => {
                    var parameter = array2.find((x) => x == element);
                    if (parameter != undefined) {
                        this.selectedParametrs.push(parameter);
                    }
                });

            }
        }
        if (this.type === "whatsApp template") {
            this.dialogForm.get("parameter").clearValidators();
            this.dialogForm
                .get("templateId")
                .setValidators([Validators.required]);
            this.dialogForm.get("parameter").updateValueAndValidity();
            this.dialogForm.get("templateId").updateValueAndValidity();

            if (
                this.dialogForm.value.templateId != null &&
                this.dialogForm.value.templateId != undefined &&
                this.dialogForm.value.templateId != "" &&
                this.dialogForm.value.templateId != "0"
            ) {
                this.selectedTemplateId = Number(
                    this.dialogForm.value.templateId
                );
                this.changeTemplate(this.selectedTemplateId);
            }
        }
        if (this.type === "Http request") {
            this.dialogForm
                .get("parameter")
                .setValidators([Validators.required]);
            this.dialogForm.get("parameter").updateValueAndValidity();
            this.dialogForm.get("captionEn").clearValidators();
            this.dialogForm.get("captionEn").updateValueAndValidity();
            //validattion for request

            if (
                this.dialogForm.value.request != null &&
                this.dialogForm.value.request != undefined
            ) {
                this.request = this.dialogForm.value.request;
            }
        }
        if (this.type === "Jump") {
            this.dialogForm.get("parameter").clearValidators();
            this.dialogForm.get("parameter").updateValueAndValidity();
            this.dialogForm.get("captionEn").clearValidators();
            this.dialogForm.get("captionEn").updateValueAndValidity();
            this.dialogForm.get("jumpId").setValidators([Validators.required]);
            this.dialogForm.get("jumpId").updateValueAndValidity();
        }
        if (
            this.dialogForm.value.urlImageArray != null &&
            this.dialogForm.value.urlImageArray != undefined
        ) {
            let images = this.dialogForm.value.urlImageArray;
            images.forEach((element) => {
                let image = {
                    name: element.name,
                    url: element.url,
                    show: false,
                    showLoading: false,
                };
                this.listOfImages.push(image);
                this.imagesList.push(element);
            });
        }
        if (this.type === "Condition") {
            this.dialogForm.get("parameter").clearValidators();
            this.dialogForm.get("parameter").updateValueAndValidity();
            this.dialogForm.get("captionEn").clearValidators();
            this.dialogForm.get("captionEn").updateValueAndValidity();
            this.dialogForm.controls["content"].setValue(
                node.content.dtoContent
            );
            this.dialogForm.controls["content"].setValue(node.content);
            this.dialogForm.value.content.dtoContent.forEach((element) => {
                this.listOfButtons.push(element);
            });
            this.condition = this.dialogForm.value.condition.orAnd;
        }
   
        if (this.type === "Cataloge Single Product") {
            debugger;
            const catalogTemplate = this.node?.catalogTemplateDto ?? new CatalogTemplateDto();
            const catalog = catalogTemplate.catalog ?? new Cacatalog();
        
            // this.dialogForm.get("headerTextEn")?.setValue(catalogTemplate.header?.text ?? '');
            this.dialogForm.get("bodyTextEn")?.setValue(catalogTemplate.body?.text ?? '');
            this.dialogForm.get("footerTextEn")?.setValue(catalogTemplate.footer?.text ?? '');
            this.dialogForm.get("catalogId")?.setValue(catalog.catalogId ?? '');
            this.dialogForm.get("catalogName")?.setValue(catalog.catalogName ?? '');

            const productsFormArray = this.dialogForm.get("products") as FormArray;
            productsFormArray.clear();

            if (catalog.products && catalog.products.length > 0) {
                catalog.products.forEach(product => {
                    productsFormArray.push(new FormGroup({
                        productId: new FormControl(product.productId ?? ''),
                        name: new FormControl(product.name ?? ''),
                        description: new FormControl(product.description ?? ''),
                        price: new FormControl(product.price ?? ''),
                        currency: new FormControl(product.currency ?? ''),
                        imageUrl: new FormControl(product.imageUrl ?? '')
                    }));
                });

                const selectedProductIds = catalog.products.map(p => p.retailer_Id);
                const selectedIds = catalog.products?.map(p => String(p.retailer_Id)) ?? [];

            if (this.type === "Cataloge Single Product") {
                const firstProductId = catalog.products?.[0]?.retailer_Id ?? null;
                this.dialogForm.get("selectedProducts")?.setValue(firstProductId ? [firstProductId] : []);
            }
              
            // this.dialogForm.get("selectedProducts")?.setValue(selectedIds);
            } else {
                this.dialogForm.get("selectedProducts")?.setValue([]);
            }

                        
        }
        if (this.type === "Cataloge multity Product") {
            debugger;
            const catalogTemplate = this.node?.catalogTemplateDto ?? new CatalogTemplateDto();
            const catalog = catalogTemplate.catalog ?? new Cacatalog();
        
            this.dialogForm.get("headerTextEn")?.setValue(catalogTemplate.header?.text ?? '');
            this.dialogForm.get("bodyTextEn")?.setValue(catalogTemplate.body?.text ?? '');
            this.dialogForm.get("footerTextEn")?.setValue(catalogTemplate.footer?.text ?? '');
            this.dialogForm.get("catalogId")?.setValue(catalog.catalogId ?? '');
            this.dialogForm.get("catalogName")?.setValue(catalog.catalogName ?? '');
            this.dialogForm.get("sectionTitle")?.setValue(catalog.sectionTitle ?? '');

                    const productsFormArray = this.dialogForm.get("products") as FormArray;
                    productsFormArray.clear();

                    if (catalog.products && catalog.products.length > 0) {
                        catalog.products.forEach(product => {
                            productsFormArray.push(new FormGroup({
                                productId: new FormControl(product.productId ?? ''),
                                name: new FormControl(product.name ?? ''),
                                description: new FormControl(product.description ?? ''),
                                price: new FormControl(product.price ?? ''),
                                currency: new FormControl(product.currency ?? ''),
                                imageUrl: new FormControl(product.imageUrl ?? '')
                            }));
                        });

                        // For ng-select, we need to set the selected product IDs
                        const selectedProductIds = catalog.products.map(p => p.productId);
                        const selectedIds = catalog.products?.map(p => String(p.productId)) ?? [];

                        this.dialogForm.get("selectedProducts")?.setValue(selectedProductIds);
                        this.dialogForm.get("selectedProducts")?.setValue(selectedIds);

                    } else {
                        this.dialogForm.get("selectedProducts")?.setValue([]);
                    }
        }


        if (this.type == "Integration") {
            node.googleSheetIntegration != undefined
                ? node.googleSheetIntegration
                : new GoogleSheetIntegrationModel();
            this.dialogForm
                .get("integrationType")
                .setValue(
                    node.googleSheetIntegration != undefined
                        ? node.googleSheetIntegration.integrationType
                        : null
                );
            this.dialogForm
                .get("googleSheetAction")
                .setValue(
                    node.googleSheetIntegration != undefined
                        ? node.googleSheetIntegration.googleSheetAction
                        : null
                );
            this.dialogForm
                .get("spreadSheetName")
                .setValue(
                    node.googleSheetIntegration != undefined
                        ? node.googleSheetIntegration.spreadSheetName
                        : null
                );

            this.dialogForm
                .get("spreadSheetId")
                .setValue(
                    node.googleSheetIntegration != undefined
                        ? node.googleSheetIntegration.spreadSheetId
                        : null
                );
            this.dialogForm
                .get("workSheet")
                .setValue(
                    node.googleSheetIntegration != undefined
                        ? node.googleSheetIntegration.workSheet
                        : null
                );
            this.dialogForm
                .get("lookupColumn")
                .setValue(
                    node.googleSheetIntegration != undefined
                        ? node.googleSheetIntegration.lookupColumn
                        : null
                );
            this.dialogForm
                .get("lookupValue")
                .setValue(
                    node.googleSheetIntegration != undefined
                        ? node.googleSheetIntegration.lookupValue
                        : null
                );
        }

        this.modal.show();
        this.modal.onHide.subscribe((reason: string | any) => {
            if (
                reason.dismissReason === "backdrop-click" ||
                reason.dismissReason === null ||
                reason.dismissReason === "esc"
            ) {
                this.modalClose.emit();
            }
        });
    }

    close() {
        this.modal.hide();
        this.showNode = false;
    }

@HostListener("document:click", ["$event"])
onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;

    if (target.classList.contains("modal")) {
        this.modal.hide();
    }
    else if (this.emojiMart !== undefined) {
        if (
            !this.emojiMart.nativeElement.contains(target) &&
            !target.classList.contains("bi-emoji-smile")
        ) {
            this.hideEmoji = true;
            this.hideEmojiArabic = true;
        }
    }
}

    hasOnlySpaces(inputString: string): boolean {
        return /^\s*$/.test(inputString);
    }

        validateCatalogFields() {
            let isValid = true;

            // Validate catalogId
            const catalogId = this.dialogForm.get("catalogId").value;
            if (!catalogId) {
                this.dialogForm.get("catalogId").setErrors({ required: true });
                isValid = false;
            }

            // Validate selectedProducts
            const products = this.dialogForm.get("selectedProducts").value;
            if (!products || products.length === 0) {
                this.dialogForm.get("selectedProducts").setErrors({ required: true });
                isValid = false;
            }

            return isValid;
        }
    
    save() {
        this.selectedTeamsIds;
        // this.dialogForm.controls['captionEn'].setErrors({ 'required': true });
        debugger;
        console.log(this.dialogForm);
        this.dialogForm;

        if (this.type !== "List options" && this.type !== "Branches" ||(this.type !== "List options" && this.type !== "BranchesEN")) {
            debugger;
            if (this.dialogForm.get("inputHint").value === "") {
                this.dialogForm.get("inputHint").setValue(" ");
            }
        }
        // if (this.type !== "List options" && this.type !== "BranchesEN") {
        //     if (this.dialogForm.get("inputHint").value === "") {
        //         this.dialogForm.get("inputHint").setValue(" ");
        //     }
        // }

        let content = new ContentInfo();
        content.dtoContent = [];
        this.dialogForm.controls["content"].setValue(content);
        this.imagesList = [];

        if (this.listOfImages.length > 0) {
            for (let i = 0; i < this.listOfImages.length; i++) {
                let image = new ImageFlowModel();
                image.url = this.listOfImages[i].url;
                image.name = this.listOfImages[i].name;
                this.imagesList.push(image);
                if (
                    this.listOfImages[i].name === null ||
                    this.listOfImages[i].name === undefined ||
                    this.listOfImages[i].name === "" ||
                    this.listOfImages[i].url === null ||
                    this.listOfImages[i].url === undefined ||
                    this.listOfImages[i].url === ""
                ) {
                    return;
                }
            }
        }
        this.dialogForm.controls["urlImageArray"].setValue(this.imagesList);
        if (this.type === "whatsApp template") {
            if (this.selectedTemplateId === 0) {
                return;
            } else {
                this.dialogForm.controls["templateId"].setValue(
                    this.selectedTemplateId.toString()
                );
                this.dialogForm.controls["captionEn"].setValue(
                    this.ComponentBody.text
                );
                if (this.ComponentFooter) {
                    if (this.ComponentFooter.text != undefined) {
                        this.dialogForm.controls["footerTextEn"].setValue(
                            this.ComponentFooter.text
                        );
                    }
                }
                if (this.ComponentButton) {
                    if (this.ComponentButton.buttons != undefined) {
                        if (this.ComponentButton.buttons.length > 0) {
                            this.dialogForm.controls["childId"].setValue(null);
                            this.dialogForm.controls["childIndex"].setValue(
                                null
                            );
                            let content = new ContentInfo();
                            content.txt = "";
                            this.ComponentButton.buttons.forEach(
                                (element, index) => {
                                    let button = new Dtocontent();
                                    button.valueEn = element.text;
                                    button.valueAr = element.text;
                                    button.parentId = [this.node.id];
                                    button.parentIndex = [1];
                                    button.childId = null;
                                    button.childIndex = null;
                                    button.key = this.stringToNumber(
                                        uniqid("IB-")
                                    );
                                    this.listOfButtons.push(button);
                                }
                            );
                            this.dialogForm.controls["content"].setValue(
                                content
                            );
                        }
                    } else {
                        this.listOfButtons = [];
                    }
                }
            }
        }
        if (
            this.type === "Reply buttons" ||
            this.type === "List options" ||
            this.type === "Branches"    ||
            this.type === "BranchesEN"
        ) {
            debugger;
            for (let i = 0; i < this.listOfButtons.length; i++) {
                // if(this.type!="BranchesEN" ){
                    if (
                        this.checkIfButtonValueExist(
                            this.listOfButtons[i].valueEn,
                            i
                        ) 
                    )
                        return;
                    if (
                        this.listOfButtons[i].valueEn === null ||
                        this.listOfButtons[i].valueEn === undefined ||
                        this.listOfButtons[i].valueEn === "" ||
                        /^ *$/.test(this.listOfButtons[i].valueEn)
                    ) {
                        return;
                    }

                // }

            }
        } 
        else if (this.type === "Set Parameter") {
            for (let i = 0; i < this.listOfParametr.length; i++) {
                if (this.hasOnlySpaces(this.listOfParametr[i].val)) {
                    this.notify.error(this.l("canNotHaveOnlySpace"));
                    return;
                }
                if (
                    this.listOfParametr[i].par === null ||
                    this.listOfParametr[i].par === undefined ||
                    this.listOfParametr[i].par === "" ||
                    this.listOfParametr[i].val === null ||
                    this.listOfParametr[i].val === undefined ||
                    this.listOfParametr[i].val === ""
                ) {
                    return;
                }
                for (let j = i + 1; j < this.listOfParametr.length; j++) {
                    if (
                        this.listOfParametr[i].par ===
                        this.listOfParametr[j].par
                    ) {
                        this.notify.error(this.l("canduplicate"));
                        return;
                    }
                }
            }

            this.dialogForm.controls["parameterList"].setValue(
                this.listOfParametr
            );
        } else if (this.type === "ScheduleNode") {
            this.isData = this.tabIndex === 0 ? true : false;
            if (
                (this.isData && !this.isNow && !this.formattedStartDate) ||
                (this.isData && this.isNow && !this.numberButton)
            ) {
                return;
            }
            const scheduleModel = new ScheduleModel();
            scheduleModel.isNow = this.isNow;
            scheduleModel.isData = this.tabIndex === 0 ? true : false;
            if (scheduleModel.isData) {
                scheduleModel.startDate = this.isNow
                    ? null
                    : this.formattedStartDate;
                scheduleModel.endDate = this.isNow
                    ? null
                    : this.formattedEndDate;
                scheduleModel.numberButton = !this.isNow
                    ? moment(this.formattedEndDate).diff(
                        this.formattedStartDate,
                        "days"
                    )
                    : this.numberButton;
                this.numberButton = scheduleModel.numberButton;
                if (this.numberButton > 10 || this.numberButton <= 0) return;
                if (this.selectedDates) {
                    const dateStrings = this.selectedDates.map((date) =>
                        date.toLocaleDateString()
                    );
                    const dateStringsWithCommas = dateStrings.join(",");
                    scheduleModel.unavailableDate = dateStringsWithCommas;
                }

                if (this.dialogForm.controls.inputHint.value.length <= 1)
                    return;
            } else {
                scheduleModel.startDate = this.isNow
                    ? null
                    : moment(this.selectedFirstTime);
                scheduleModel.endDate = this.isNow
                    ? null
                    : moment(this.selectedEndTime);
                scheduleModel.numberButton = !this.isNow
                    ? this.selectedEndTime.getHours() -
                    this.selectedFirstTime.getHours()
                    : this.numberButton;

                this.numberButton = scheduleModel.numberButton;
                if (this.numberButton > 10 || this.numberButton <= 0) return;
                scheduleModel.unavailableDate = "";
                this.selectedDates = [];
                this.isUnavailbleDayActive = false;
            }

            this.dialogForm.controls["schedule"].setValue(scheduleModel);
        } else if (this.type === "Condition") {
            for (let i = 0; i < this.listOfConditions.length; i++) {
                if (
                    this.listOfConditions[i].op1 === null ||
                    this.listOfConditions[i].op1 === undefined ||
                    this.listOfConditions[i].op1 === "" ||
                    this.listOfConditions[i].op2 === null ||
                    this.listOfConditions[i].op2 === undefined ||
                    this.listOfConditions[i].op2 === "" ||
                    this.listOfConditions[i].operation === null ||
                    this.listOfConditions[i].operation === undefined ||
                    this.listOfConditions[i].operation === ""
                ) {
                    return;
                }
            }
        } else if (this.type === "Human handover") {

            if (this.selectedUserIds.length === 0) {
                return;
            } else {
                let handOverUsers = this.selectedUserIds
                    .filter((f) => f.id > 0)
                    .map(({ id }) => id)
                    .toString();
                this.dialogForm.controls["listOfUsers"].setValue(handOverUsers);
            }

            if (this.selectedTeamsIds.length === 0) {
                this.dialogForm.controls["listOfTeams"].setValue("");
                //return;
            } else {
                let handOverUsers = this.selectedTeamsIds
                    .filter((f) => f.id > 0)
                    .map(({ id }) => id)
                    .toString();
                this.dialogForm.controls["listOfTeams"].setValue(handOverUsers);
            }

            if (this.selectedParametrs.length === 0) {
                return;
            } else {
                let selectedParameters = this.selectedParametrs
                    .filter((item) => item.name)
                    .map((item) => item.name)
                    .join(",");
                if (selectedParameters == "") {
                    selectedParameters = this.selectedParametrs
                        .filter((item) => item)
                        .map((item) => item)
                        .join(",");
                }
                this.dialogForm.controls["parameter"].setValue(
                    selectedParameters
                );
            }

            // else {
            //     let selectedParameters = "";
            //     this.dialogForm.controls["parameter"].setValue(
            //         selectedParameters
            //     );
            // }
        } else if (this.type === "Http request") {
            if (
                this.request.httpMethod === null ||
                this.request.httpMethod === undefined ||
                this.request.httpMethod === "" ||
                this.request.url === null ||
                this.request.url === undefined ||
                this.request.url === "" ||
                this.request.contentType === null ||
                this.request.contentType === undefined ||
                this.request.contentType === "" ||
                this.request.resposeType === null ||
                this.request.resposeType === undefined ||
                this.request.resposeType === ""
            ) {
                return;
            }

            if (this.request.httpMethod == "POST") {
                if (
                    this.request.body === null ||
                    this.request.body === undefined ||
                    this.request.body === ""
                ) {
                    return;
                }
            }

            this.dialogForm.controls["request"].setValue(this.request);
        } else if (this.type === "Send Message") {
            if (this.imagesList.length > 0) {
                this.dialogForm.controls["captionEn"].setErrors(null);
            } else {
                this.dialogForm.controls["captionEn"].setValidators([
                    Validators.required,
                    noWhitespaceValidator,
                ]);

                // Trigger validation to apply the validators
                this.dialogForm.controls["captionEn"].updateValueAndValidity();
            }

            if (this.imagesList.length > 0 && this.dialogForm.value.captionEn) {
                this.notify.error("You can't add image and text together");
                return;
            }
        }




        else if (this.type == "Integration") {
            let content = new ContentInfo();
            content.dtoContent = [];
            this.dialogForm.controls["content"].setValue(content);
            content.dtoContent.push();
        }

        if (this.listOfButtons.length > 0) {
            let content = new ContentInfo();
            content.txt = "";
            content.dtoContent = this.listOfButtons;
            this.dialogForm.controls["content"].setValue(content);
        }
        if (this.listOfConditions.length > 0) {
            let condition = new ConditionalModel();
            condition.orAnd = this.condition;
            condition.conditionList = this.listOfConditions;
            this.dialogForm.controls["condition"].setValue(condition);
        }
            if (this.type === "Cataloge multity Product" || this.type === "Cataloge Single Product") {
        // Initialize template structure if node exists
        if (this.node) {
            if (!this.node.catalogTemplateDto) {
                this.node.catalogTemplateDto = new CatalogTemplateDto();
            }
            
            const template = this.node.catalogTemplateDto;
            
            if (!template.body) template.body = new TemplateBody();
            if (!template.footer) template.footer = new TemplateFooter();
            if (!template.catalog) template.catalog = new Cacatalog();
            
            if (this.type === "Cataloge multity Product" && !template.header) {
                template.header = new TemplateHeader();
            }
            
            if (this.type === "Cataloge multity Product") {
                template.header.text = this.dialogForm.get('headerTextEn')?.value;
            }
            
            template.body.text = this.dialogForm.get('bodyTextEn')?.value;
            template.footer.text = this.dialogForm.get('footerTextEn')?.value;
            
            template.catalog.catalogId = this.dialogForm.get('catalogId')?.value;
            template.catalog.catalogName = this.dialogForm.get('catalogName')?.value;
            
            if (this.type === "Cataloge multity Product") {
                template.catalog.sectionTitle = this.dialogForm.get('sectionTitle')?.value;
            }
            
            const selectedProductIds = this.dialogForm.get('selectedProducts')?.value || [];

            template.catalog.products = this.availableProducts
                .filter(product => selectedProductIds.includes(product.productId))
                .map(product => new ProductDto({
                    productId: product.productId,
                    name: product.name,
                    retailer_Id: product.retailer_Id,
                    description: product.description || '',
                    availability: product.availability || '', 
                    price: product.price || '',
                    currency: product.currency || '',
                    imageUrl: product.imageUrl || ''
                }));

            template.catalog.products = this.availableProducts
                .filter(product => selectedProductIds.includes(product.retailer_Id))
                .map(product => new ProductDto({
                    productId: product.retailer_Id,
                    name: product.name,
                    retailer_Id: product.retailer_Id,
                    description: product.description || '',
                    availability: product.availability || '', 
                    price: product.price || '',
                    currency: product.currency || '',
                    imageUrl: product.imageUrl || ''
                }));

            template.catalog.products;
            template.catalog.products;
            if (template.catalog.products.length > 1 && this.type == 'Cataloge Single Product') {
                const errorMessage = "You must select only one product for this catalog type.";
                this.notify.error(errorMessage);
                return;
            }
        }
        
        this.setCatalogValidators();
        if (this.dialogForm.invalid) {
            const errorMessage = this.type === "Cataloge Single Product" 
                ? "Please fill all required fields for Catalog Multi Product"
                : "Please fill all required fields for Catalog Single Product";
            this.notify.error(errorMessage);
            return;
        }
        if (this.dialogForm.invalid) {
            const errorMessage = this.type === "Cataloge multity Product" 
                ? "Please fill all required fields for Catalog Multi Product"
                : "Please fill all required fields for Catalog Single Product";
            this.notify.error(errorMessage);
            return;
        }
        
        const content = new ContentInfo();
            this.dialogForm.get('sectionTitle')?.setValidators(Validators.required);
            this.dialogForm.get('selectedProducts')?.setValidators(Validators.required);
            this.dialogForm.get('catalogId')?.setValidators(Validators.required);
            this.dialogForm.get('headerTextEn')?.setValidators(Validators.required);
            this.dialogForm.get('bodyTextEn')?.setValidators(Validators.required);
            this.dialogForm.get('footerTextEn')?.setValidators(null); 

            this.dialogForm.get('sectionTitle')?.updateValueAndValidity();
            this.dialogForm.get('selectedProducts')?.updateValueAndValidity();

                var control = this.dialogForm.get('selectedProducts');
                var value = control?.value; 
                var selectedProducts = this.dialogForm.get('selectedProducts')?.value;
                var selectedProducts: any[] = this.dialogForm.get('selectedProducts')?.value || [];
                
            this.dialogForm.get('catalogId')?.updateValueAndValidity();
            this.dialogForm.get('headerTextEn')?.updateValueAndValidity();
            this.dialogForm.get('bodyTextEn')?.updateValueAndValidity();
            this.dialogForm.get('footerTextEn')?.updateValueAndValidity();

            // this.dialogForm.controls["content"].setValue(content);
            // content.dtoContent.push();
            // this.dialogForm.controls["content"].setValue(content);
            // content.dtoContent.push();
        
            const formValues = this.dialogForm.getRawValue();
            this.modalSave.emit(formValues);
            this.close();
    }
        this.dialogForm;
        if (this.type === 'Cataloge Single Product' || 
                this.type === 'Cataloge multity Product'
            ) {
                const fields = ['catalogId', 'headerTextEn', 'bodyTextEn', 'sectionTitle', 'selectedProducts'];
            
                fields.forEach(field => {
                    this.dialogForm.get(field)?.clearValidators();
                    this.dialogForm.get(field)?.updateValueAndValidity();
                });
            }
        if (this.dialogForm.valid) {
            debugger;
            const formvalues = this.dialogForm.getRawValue();
            this.modalSave.emit(formvalues);
            this.close();
        }
    // this.close();

    }


    onChanges() {
        this.dialogForm.valueChanges.subscribe((x) => {
            const formvalues = this.dialogForm.getRawValue();
            this.modalSave.emit(formvalues);
        });
    }

    deleteItem(id) {
        for (let i = 0; i < this.listOfButtons.length; i++) {
            if (this.listOfButtons.indexOf(this.listOfButtons[i]) === id) {
                this.listOfButtons.splice(i, 1);
                break;
            }
        }
    }
    deleteCondition(index) {
        for (let i = 0; i < this.listOfConditions.length; i++) {
            if (
                this.listOfConditions.indexOf(this.listOfConditions[i]) ===
                index
            ) {
                this.listOfConditions.splice(i, 1);
                break;
            }
        }
    }

    deleteParameter(index) {
        for (let i = 0; i < this.listOfParametr.length; i++) {
            if (this.listOfParametr.indexOf(this.listOfParametr[i]) === index) {
                this.listOfParametr.splice(i, 1);
                break;
            }
        }
    }

    addVariable() {
        if (this.type === "Set Parameter") {
            if (this.selectParameter.length === this.listOfParametr.length)
                return;
            let parameter = new ParameterSet();

            this.listOfParametr.push(parameter);
        }
    }

    addButtonOrOption() {
        if (this.type == "Reply buttons" || this.type === "Language") {
            if (this.listOfButtons.length < 3) {
                let button = new Dtocontent({
                    key: this.stringToNumber(uniqid("IB-")),
                    parentIndex: this.listOfButtons[0].parentIndex,
                    childIndex: null,
                    parentId: this.listOfButtons[0].parentId,
                    childId: null,
                    valueEn: "Button" + " " + (this.listOfButtons.length + 1),
                    valueAr: "زر" + " " + (this.listOfButtons.length + 1),
                    branchID: "-1",
                });
                this.listOfButtons.push(button);
            } else {
                this.notify.error("You can't add more than 3 buttons");
            }

        }

        //else if (this.type === "Cataloge multity Product") {
        //    let product = new Dtocontent({
        //        key: this.stringToNumber(uniqid("IB-")),
        //        parentIndex: this.listOfButtons[0]?.parentIndex || [this.index],
        //        childIndex: null,
        //        parentId: this.listOfButtons[0]?.parentId || [this.node.id],
        //        childId: null,
        //        valueEn: "Product " + (this.listOfButtons.length + 1),
        //        valueAr: "منتج " + (this.listOfButtons.length + 1),
        //        branchID: "-1",
        //    });
        //    this.listOfButtons.push(product);
        //}
        else if (this.type === "List options") {
            if (
                this.dialogForm.controls["isAdvance"].value ||
                (!this.dialogForm.controls["isAdvance"].value &&
                    this.listOfButtons.length < 10)
            ) {
                let button = new Dtocontent({
                    key: this.stringToNumber(uniqid("IB-")),
                    parentIndex: this.listOfButtons[0].parentIndex,
                    childIndex: null,
                    parentId: this.listOfButtons[0].parentId,
                    childId: null,
                    valueEn: "Option" + " " + (this.listOfButtons.length + 1),
                    valueAr: "خيار" + " " + (this.listOfButtons.length + 1),
                    branchID: "-1",
                });
                this.listOfButtons.push(button);
            } else {
                this.notify.error("You can't add more than 10 options");
            }
        }
    }
    addConditioin() {
        if (this.type === "Condition") {
            let condition = new ConditionList();
            condition.op1 = "";
            condition.operation = "Equal to";
            condition.op2 = "";
            this.listOfConditions.push(condition);
        }
    }
    private stringToNumber(value: string): number {
        let hash = 0;
        for (let i = 0; i < value.length; i++) {
            const char = value.charCodeAt(i);
            hash = (hash << 5) - hash + char;
            hash = hash & hash;
        }
        return hash;
    }

    addImage() {
        debugger;
        if (this.type === "Reply buttons" && this.listOfImages.length > 0) {
            this.notify.error("You can't add more than 1 image");
            return;
        } else if (
            this.type === "List options" &&
            this.listOfImages.length > 0
        ) {
            this.notify.error("You can't add more than 1 image");
            return;
        } else if (
            this.type === "Send Message" &&
            this.listOfImages.length >= 9
        ) {
            this.notify.error("You can't add more than 9 images");
            return;
        }

        let image = {
            name: "",
            url: "",
            show: true,
            showLoading: false,
        };
        this.listOfImages.push(image);
    }
    boldText() {
        if (this.dialogForm.controls["captionEn"].value) {
            const textarea = document.getElementById(
                "captionEn"
            ) as HTMLTextAreaElement;
            const startPosition = textarea.selectionStart;
            const endPosition = textarea.selectionEnd;
            const textBefore = textarea.value.substring(0, startPosition);
            const selectedText = textarea.value.substring(
                startPosition,
                endPosition
            );
            const textAfter = textarea.value.substring(
                endPosition,
                textarea.value.length
            );
            const modifiedText = `${textBefore}*${selectedText}*${textAfter}`;
            this.dialogForm.controls["captionEn"].setValue(modifiedText);
        } else {
            const textarea = this.myTextarea.nativeElement;
            const stars = "**";
            const cursorPosition = textarea.selectionStart || 0;
            const text = textarea.value;
            const newText =
                text.slice(0, cursorPosition) +
                stars +
                text.slice(cursorPosition);
            textarea.value = newText;
            textarea.setSelectionRange(cursorPosition + 1, cursorPosition + 1);
            textarea.focus();
        }
    }

    boldTextArabic() {
        if (this.dialogForm.controls["captionAr"].value) {
            const textarea = document.getElementById(
                "captionAr"
            ) as HTMLTextAreaElement;
            const startPosition = textarea.selectionStart;
            const endPosition = textarea.selectionEnd;
            const textBefore = textarea.value.substring(0, startPosition);
            const selectedText = textarea.value.substring(
                startPosition,
                endPosition
            );
            const textAfter = textarea.value.substring(
                endPosition,
                textarea.value.length
            );
            const modifiedText = `${textBefore}*${selectedText}*${textAfter}`;
            this.dialogForm.controls["captionAr"].setValue(modifiedText);
        } else {
            const textarea = this.myTextarea.nativeElement;
            const stars = "**";
            const cursorPosition = textarea.selectionStart || 0;
            const text = textarea.value;
            const newText =
                text.slice(0, cursorPosition) +
                stars +
                text.slice(cursorPosition);
            textarea.value = newText;
            textarea.setSelectionRange(cursorPosition + 1, cursorPosition + 1);
            textarea.focus();
        }
    }

    italicText() {
        if (this.dialogForm.controls["captionEn"].value) {
            const textarea = document.getElementById(
                "captionEn"
            ) as HTMLTextAreaElement;
            const startPosition = textarea.selectionStart;
            const endPosition = textarea.selectionEnd;
            const textBefore = textarea.value.substring(0, startPosition);
            const selectedText = textarea.value.substring(
                startPosition,
                endPosition
            );
            const textAfter = textarea.value.substring(
                endPosition,
                textarea.value.length
            );
            const modifiedText = `${textBefore}_${selectedText}_${textAfter}`;
            this.dialogForm.controls["captionEn"].setValue(modifiedText);
        } else {
            const textarea = this.myTextarea.nativeElement;
            const stars = "__";
            const cursorPosition = textarea.selectionStart || 0;
            const text = textarea.value;
            const newText =
                text.slice(0, cursorPosition) +
                stars +
                text.slice(cursorPosition);
            textarea.value = newText;
            textarea.setSelectionRange(cursorPosition + 1, cursorPosition + 1);
            textarea.focus();
            this.dialogForm.controls["captionEn"].setValue(newText);
        }
    }
    italicTextArabic() {
        if (this.dialogForm.controls["captionAr"].value) {
            const textarea = document.getElementById(
                "captionAr"
            ) as HTMLTextAreaElement;
            const startPosition = textarea.selectionStart;
            const endPosition = textarea.selectionEnd;
            const textBefore = textarea.value.substring(0, startPosition);
            const selectedText = textarea.value.substring(
                startPosition,
                endPosition
            );
            const textAfter = textarea.value.substring(
                endPosition,
                textarea.value.length
            );
            const modifiedText = `${textBefore}_${selectedText}_${textAfter}`;
            this.dialogForm.controls["captionAr"].setValue(modifiedText);
        } else {
            const textarea = this.myTextarea.nativeElement;
            const stars = "__";
            const cursorPosition = textarea.selectionStart || 0;
            const text = textarea.value;
            const newText =
                text.slice(0, cursorPosition) +
                stars +
                text.slice(cursorPosition);
            textarea.value = newText;
            textarea.setSelectionRange(cursorPosition + 1, cursorPosition + 1);
            textarea.focus();
            this.dialogForm.controls["captionAr"].setValue(newText);
        }
    }

    addParameter(parameterName) {
        if (this.dialogForm.controls["captionEn"].value) {
            const textarea = document.getElementById(
                "captionEn"
            ) as HTMLTextAreaElement;
            const startPosition = textarea.selectionStart;
            const endPosition = textarea.selectionEnd;
            const textBefore = textarea.value.substring(0, startPosition);
            const selectedText = textarea.value.substring(
                startPosition,
                endPosition
            );
            const textAfter = textarea.value.substring(
                endPosition,
                textarea.value.length
            );
            const modifiedText = `${textBefore}$${parameterName} ${selectedText}${textAfter}`;
            textarea.setSelectionRange(
                parameterName.length + 1,
                parameterName.length + 1
            );
            textarea.focus();
            this.dialogForm.controls["captionEn"].setValue(modifiedText);
            const dropdownElement =
                document.getElementById("dropdownMenuButton");
            if (dropdownElement) {
                dropdownElement.click();
            }
        } else {
            const textarea = this.myTextarea.nativeElement;
            const stars = "$";
            const cursorPosition = textarea.selectionStart || 0;
            const text = textarea.value;
            const newText =
                text.slice(0, cursorPosition) +
                stars +
                text.slice(cursorPosition) +
                parameterName;
            textarea.value = newText;
            textarea.setSelectionRange(newText.length + 1, newText.length + 1);
            textarea.focus();
            this.dialogForm.controls["captionEn"].setValue(newText);
            const dropdownElement =
                document.getElementById("dropdownMenuButton");
            if (dropdownElement) {
                dropdownElement.click();
            }
        }
    }
    addParameterArabic(parameterName) {
        if (this.dialogForm.controls["captionAr"].value) {
            const textarea = document.getElementById(
                "captionAr"
            ) as HTMLTextAreaElement;
            const startPosition = textarea.selectionStart;
            const endPosition = textarea.selectionEnd;
            const textBefore = textarea.value.substring(0, startPosition);
            const selectedText = textarea.value.substring(
                startPosition,
                endPosition
            );
            const textAfter = textarea.value.substring(
                endPosition,
                textarea.value.length
            );
            const modifiedText = `${textBefore}$${parameterName} ${selectedText}${textAfter}`;
            textarea.setSelectionRange(
                parameterName.length + 1,
                parameterName.length + 1
            );
            textarea.focus();
            this.dialogForm.controls["captionAr"].setValue(modifiedText);
            const dropdownElement = document.getElementById(
                "dropdownMenuButton1"
            );
            if (dropdownElement) {
                dropdownElement.click();
            }
        } else {
            const textarea = this.myTextarea.nativeElement;
            const stars = "$";
            const cursorPosition = textarea.selectionStart || 0;
            const text = textarea.value;
            const newText =
                text.slice(0, cursorPosition) +
                stars +
                text.slice(cursorPosition) +
                parameterName;
            textarea.value = newText;
            textarea.setSelectionRange(newText.length + 1, newText.length + 1);
            textarea.focus();
            this.dialogForm.controls["captionAr"].setValue(newText);
            const dropdownElement = document.getElementById(
                "dropdownMenuButton1"
            );
            if (dropdownElement) {
                dropdownElement.click();
            }
        }
    }
    addEmoji(event) {
        this.dialogForm.controls["captionEn"].setValue(
            this.dialogForm.value.captionEn + event.emoji.native
        );
    }
    addEmojiArabic(event) {
        this.dialogForm.controls["captionAr"].setValue(
            this.dialogForm.value.captionAr + event.emoji.native
        );
    }

    showEmoji() {
        this.hideEmoji = !this.hideEmoji;
    }
    showEmojiArabic() {
        this.hideEmojiArabic = !this.hideEmojiArabic;
    }
    onFileChange(event, index) {
        this.showLoading = true;
        this.listOfImages[index].showLoading = true;
        let form = new FormData();
        form.append("FormFile", event.target.files[0]);
        if (this.node.type === "Reply buttons") {
            if (event.target.files[0]) {
                if (
                    event.target.files[0].type === "application/doc" ||
                    event.target.files[0].type === "application/pdf"
                ) {
                    this.message.error(
                        "",
                        this.l("You cant upload Document or PDF file")
                    );
                    this.filee.nativeElement.value = "";
                    this.listOfImages[index].show = false;
                    this.listOfImages[index].showLoading = false;
                    this.showLoading = false;
                    this.listOfImages = [];
                    return;
                }
            }
        }
        if (this.node.type === "List options") {
            if (event.target.files[0]) {
                if (!event.target.files[0].type.startsWith("image/")) {
                    this.message.error(
                        "",
                        this.l("You Can upload only Image Type")
                    );
                    this.filee.nativeElement.value = "";
                    this.listOfImages[index].show = false;
                    this.listOfImages[index].showLoading = false;
                    this.showLoading = false;
                    this.listOfImages = [];
                    return;
                }
            }
        }
        let image = form;
        if (event.target.files[0]) {
            if (
                event.target.files[0].type === "image/jpeg" ||
                event.target.files[0].type === "image/jpg" ||
                event.target.files[0].type === "image/Jif" ||
                event.target.files[0].type === "image/jpg" ||
                event.target.files[0].type === "application/doc" ||
                event.target.files[0].type === "video/mp4" ||
                event.target.files[0].type === "application/pdf"
            ) {
                if (event.target.files[0].size <= 134217728) {
                    this._itemsServiceProxy
                        .getImageUrl(image)
                        .subscribe((res) => {
                            // this.imageUrl = res["result"];
                            this.listOfImages[index].url = res["result"];
                            // this.dialogForm.controls["urlImage"].setValue(res["result"]);
                            let image = new ImageFlowModel();
                            image.url = this.listOfImages[index].url;
                            image.name = event.target.files[0].name;
                            this.imagesList.push(image);
                            this.dialogForm.controls["urlImageArray"].setValue(
                                this.imagesList
                            );
                            this.showLoading = false;
                            this.listOfImages[index].name =
                                event.target.files[0].name;
                            this.listOfImages[index].show = false;
                            this.listOfImages[index].showLoading = false;
                            if (
                                this.type === "Send Message" &&
                                this.listOfImages.length > 0
                            ) {
                                this.dialogForm
                                    .get("captionEn")
                                    .clearValidators();
                                this.dialogForm
                                    .get("captionEn")
                                    .updateValueAndValidity();
                            }
                        });
                } else {
                    this.message.error("File should Be less than 16MB");
                }
            } else {
                this.message.error("", this.l("youCantUploadThisFile"));
                this.imagesList = [];
                this.listOfImages = [];
                this.filee.nativeElement.value = "";
                this.listOfImages[index].show = false;
                this.listOfImages[index].showLoading = false;
                this.showLoading = false;
            }
        } else {
            this.showLoading = false;
        }
    }
    deleteImage(index) {
        this.listOfImages.splice(index, 1);
        this.imagesList.splice(index, 1);
        let imageString = this.imagesList.join(",");
        // this.dialogForm.controls["urlImageArray"].setValue(imageString);
        if (this.type === "Send Message" && this.listOfImages.length === 0) {
            this.dialogForm
                .get("captionEn")
                .setValidators([Validators.required]);
            this.dialogForm.get("captionEn").updateValueAndValidity();
        }
    }
    styleAvatar(type) {
        if (type == "Send Message") {
            return "#34b8ca";
        } else if (type == "Language") {
            return "#fc7c29";
        } else if (type == "Reply buttons") {
            return "#359cec";
        } else if (type == "Human handove") {
            return "#f8cc72";
        } else if (type == "Collect input") {
            return "#cb62e4";
        } else if (type == "Jump") {
            return "#fc7c29";
        } else if (type == "List options") {
            return "#359cec";
        } else if (type == "Interactive template") {
            return "#2ab9cd";
        } else if (type == "Delay") {
            return "#cb62e4";
        } else if (type == "Http request") {
            return "#fc7c29";
        } else if (type == "Condition") {
            return "#2eb67d";
        } else if (type == "whatsApp template") {
            return "#33b349";
        } else if (type == "Branches") {
            return "#0802a3";
        }  else if (type == "BranchesEN") {
            return "#0802a3";
        } else if (type == "Set Parameter") {
            return "#fc7c29";
        } else if (type == "ScheduleNode") {
            return "#b8a609";
        } else if (type === "Integration") {
            return "#9b59b6";
        }else if (type === "Cataloge Single Product") {
            return "#ff9800"; 
        } else if (type === "Cataloge multity Product") {
            return "#4caf50";
        }
        return {};
    }

    onChangeVariable({ value }, parameterIndex: number) {
        this.listOfParametr[parameterIndex].par = value.name;
    }

    getParameteres() {
        debugger;
        this._BotFlowServiceProxy
            .botParameterGetAll(this.appSession.tenantId)
            .subscribe((result) => {
                this.selectParameter = result.items;
                this.parameterForSetParameters = result.items.map(
                    (item) => item.name
                );
            });
    }

    deleteVariable(id) {
        this._BotFlowServiceProxy.botParameterDeleteById(id).subscribe(
            (result) => {
                if (result) {
                    this.notify.success(this.l("SuccessfullyDeleted"));
                    this.getParameteres();
                } else {
                    this.notify.error(this.l("Failed"));
                }
            },
            (error: any) => {
                if (error) {
                    this.notify.error(this.l("Failed"));
                }
            }
        );
    }

    getTenantUsers() {
        this._userServiceProxy
            .getUsers(null, null, null, false, null, 1000, 0)
            .subscribe((result: any) => {
                this.selectMultiUsers = result.items;
            });
    }
    getTenantTeams() {
        this._teamsService.teamsGetAll(null, 0, 100000).subscribe((result) => {
            this.selectMultiTeams = result.teamsDtoModel;
        });
    }

    getTemplates() {
        this._whatsAppMessageTemplateServiceProxy
            .getWhatsAppTemplateForCampaign(0, 1000, this.appSession.tenantId)
            .subscribe((result) => {
                this.Templates = result.lstWhatsAppTemplateModel.filter(
                    (element) =>
                        element.language == "ar" || element.language == "en"
                );
            });
    }

    changeTemplate(selectedTemplate) {
        let template = new MessageTemplateModel();
        this.selectedTemplate = new MessageTemplateModel();
        this.Component = [new WhatsAppComponentModel()];
        this.ComponentHeader = new WhatsAppComponentModel();
        this.ComponentBody = new WhatsAppComponentModel();
        this.ComponentFooter = new WhatsAppComponentModel();
        this.ComponentButton = new WhatsAppComponentModel();
        template = this.Templates.find((x) => x.id == selectedTemplate);
        if (!template) {
            this.selectedTemplate = new MessageTemplateModel();
            this.selectedTemplateId = 0;
            return null;
        }
        this._whatsAppMessageTemplateServiceProxy
            .getTemplateByWhatsAppId(template.id)
            .subscribe((result) => {
                this.selectedTemplate = result;
                this.selectedTemplate.status = template.status;
                if (this.selectedTemplate.mediaType == "image") {
                    this.imageFlag = true;
                } else {
                    this.imageFlag = false;
                }
                if (this.selectedTemplate.mediaType == "video") {
                    this.videoFlag = true;
                } else {
                    this.videoFlag = false;
                }
                if (this.selectedTemplate.mediaType == "document") {
                    this.documentFlag = true;
                } else {
                    this.documentFlag = false;
                }
                this.Component = this.selectedTemplate.components;
                this.Component.forEach((e) => {
                    if (e.type == "HEADER") {
                        this.ComponentHeader = e;
                    }
                    if (e.type == "BODY") {
                        this.ComponentBody = e;
                    }
                    if (e.type == "FOOTER") {
                        this.ComponentFooter = e;
                    }
                    if (e.type == "BUTTONS") {
                        this.ComponentButton = e;
                    }
                });
            });
    }

    maketheUrlSafe(url) {
        return (this.safeUrl =
            this.sanitizer.bypassSecurityTrustResourceUrl(url));
    }
    drop(event: CdkDragDrop<string[]>) {
        moveItemInArray(
            this.listOfImages,
            event.previousIndex,
            event.currentIndex
        );
    }

    dropButton(event: CdkDragDrop<string[]>) {
        if (event.previousContainer === event.container) {
            moveItemInArray(
                event.container.data,
                event.previousIndex,
                event.currentIndex
            );
        } else {
            transferArrayItem(
                event.previousContainer.data,
                event.container.data,
                event.previousIndex,
                event.currentIndex
            );
        }

        if (this.type === "Branches") {
            debugger;
            let branch: AreaDto = event.container.data[0] as any;
            if (!branch.checked) {
                this.toggleBranch(branch);
            }
        }
        if (this.type === "BranchesEN") {
            debugger;
            let branchEN: AreaDto = event.container.data[0] as any;
            if (!branchEN.checked) {
                this.toggleBranchEN(branchEN);
            }
        }
    }
    trackByFunction(index: number, item: any): any {
        return item.id;
    }
    updateParameterType(event, node) {
        if (event) {
            const selectedParameter = this.selectParameter.find(
                (param) => param.name === event.name
            );
            this.listOfNodeWithVar.set(selectedParameter.id, {
                name: selectedParameter.name,
                id: node.id,
            });

            if (selectedParameter) {
                this.dialogForm.controls["parameterType"].setValue(
                    selectedParameter.format
                );
            }
        } else {
            this.dialogForm.controls["parameterType"].setValue("");
        }
    }

    TeamsCheck() {
    }

    searchNode(term: string, item) {
        return item.title.toLowerCase().startsWith(term.toLowerCase());
    }

    checkIfbuttononlyHaveSpace(value) {
        const trimmedValue = value.trim().toLowerCase();
        return /^ *$/.test(trimmedValue);
    }

    checkIfButtonValueExist(value, currentIndex) {
        const trimmedValue = value.trim().toLowerCase();
        return this.listOfButtons.some(
            (button, index) =>
                button.valueEn.trim().toLowerCase() === trimmedValue &&
                index !== currentIndex
        );
    }

    detectLanguage(event) {
        if (event.target.value != null) {
            if (/[\u0600-\u06FF]/.test(event.target.value.charAt(0))) {
                this.isArabic = true;
            } else if (/^[A-Za-z0-9]*$/.test(event.target.value.charAt(0))) {
                this.isArabic = false;
            }
        }
    }
    setCondition(newCondition: string) {
        this.condition = newCondition;
    }
    addVariableToConditionOp1(condition, index, variable) {
        condition.op1 = "$" + variable;
        // this.dropdown.close()
    }

    addVariableToConditionOp2(condition, index, variable) {
        condition.op2 = "$" + variable;
        // this.dropdown.close()
    }

    // integration region
    integrationsList: any[] = [
        { label: "Google Sheets", value: "googleSheets" },
        { label: "Salla (coming soon)", value: "salla" },
        { label: "ZID (coming soon)", value: "zid" },
        { label: "Oddo (coming soon)", value: "oddo" },
    ];
    googleSheetActions = [
        { label: "Insert row", value: "insertRow" },
        { label: "Get row by value", value: "getRowByValue" },
    ];
    workSheetsList: any[] = [];
    worksheetColumns: any[] = [];
    showSpreadsheetDropdown: boolean = false;
    showWorksheetDropdown: boolean = false;
    spreadSheetId: string = "";
    spreadSheetName : string = "";
    selectedAction: string = "";
    rowDict = new Map<string, string>();
    sheetName: string = "";
    lookupCol: string = "";
    lookupVal: string = "";
    headers: any[];
    values: any[];
    showNode: boolean = false;

    onIntegrationChange(event: any) { }

    onActionChange(event: any) {
        const action =
            event != undefined
                ? typeof event === "string"
                    ? event
                    : event?.value
                : this.dialogForm.controls.googleSheetAction.value;

        if (action == "insertRow") {
            this.showSpreadsheetDropdown = true;
            this.selectedAction = "insertRow";
            this.resetFields();
        } else if (action == "getRowByValue") {
            this.showSpreadsheetDropdown = true;
            this.selectedAction = "getRowByValue";
            this.resetFields();
        } else {
            this.showSpreadsheetDropdown = false;
            this.selectedAction = "";
            this.resetFields();
        }
    }

    resetFields() {
        this.dialogForm.controls.spreadSheetName.setValue(null);
        this.workSheetsList = [];
        this.dialogForm.controls.workSheet.setValue(null);
        this.worksheetColumns = [];
        this.resetParameter();
        this.showWorksheetDropdown = false;
    }


    onSpreadSheetChange(event: any) {
        if (event) {
            this.spreadSheetId =
                typeof event === "string" ? event : event?.value;
            this.showWorksheetDropdown = true;
            this.getWorkSheetList(this.spreadSheetId);
            this.resetParameter();
            this.dialogForm.controls.workSheet.setValue(null);
            this.dialogForm.get("lookupColumn").setValue(null);
            this.dialogForm.get("lookupValue").setValue(null);
            this.worksheetColumns = [];
        } else {
            this.showWorksheetDropdown = false;
            this.workSheetsList = [];
            this.dialogForm.controls.workSheet.setValue(null);
            this.worksheetColumns = [];
            this.resetParameter();
        }
    }

    getWorkSheetList(spreadSheetId: string) {
        let tenantId = this.appSession.tenantId;
        this._BotFlowServiceProxy
            .getWorkSheets(spreadSheetId, tenantId)
            .subscribe((response) => {
                if (response.length > 0) {
                    this.workSheetsList = response;
                }
            });
    }

    onWorkSheetChange(event: any) {
        if (event) {
            this.sheetName = event;
            this.getWorkSheetHeaders(this.sheetName);
        } else {
            this.worksheetColumns = [];
            this.resetParameter();
            this.dialogForm.get("lookupColumn").setValue(null);
            this.dialogForm.get("lookupValue").setValue(null);
        }
    }

    getWorkSheetHeaders(worksheet: string) {
        let tenantId = this.appSession.tenantId;
        this._BotFlowServiceProxy
            .getLookupHeaders(this.spreadSheetId, worksheet, tenantId)
            .subscribe((response) => {
                let tempCols = response;
                if (tempCols != null && tempCols.length == 1 &&
                    tempCols[0].includes("Invalid sheet structure")) {
                    this.message.error("", this.l(tempCols[0].toString()));
                    return;
                }
                else if (tempCols != null && tempCols.length == 1 && tempCols[0].includes("Failed")) {
                    // this.message.error("", this.l(tempCols[0].toString()));
                    return;
                }
                else {
                    this.worksheetColumns = response;
                    this.resetParameter();
                    this.parameters.clear();
                    this.worksheetColumns.forEach(() => {
                        this.parameters.push(new FormControl(null));
                    });

                    this.columns.clear();
                    this.worksheetColumns.forEach((col, index) => {
                        this.columns.push(new FormControl(null));
                        this.columns.at(index).setValue(col);
                    });

                    if (
                        this.node.googleSheetIntegration != undefined && this.showNode) {
                        const params = this.node.googleSheetIntegration.parameters;

                        if (params.length < this.worksheetColumns.length) {
                            const diff = this.worksheetColumns.length - params.length;
                            for (let i = 0; i < diff; i++) {
                                params.push(null);
                            }
                        }
                        const formArrayLength = this.parameters.length;

                        if (params.length > formArrayLength) {
                            params.splice(formArrayLength);
                        }

                        params.forEach((param, index) => {
                            this.parameters.at(index).setValue(param);
                        });
                    }
                }

            });
    }

    onLookupColumnChange(event: any) {
        if (event != null && event != undefined) {
            this.lookupCol = event;
        }
    }


    resetParameter() {
        this.dialogForm.get("parameter").clearValidators();
        this.dialogForm.get("parameter").setValue(null);
        this.dialogForm.get("parameter").updateValueAndValidity();
    }

    onParameterChange(selectedParameter: any, index: number) {
        if (this.selectedAction === "insertRow") {
            this.parameters
                .at(index)
                .setValue(
                    selectedParameter != undefined
                        ? selectedParameter.name
                        : null
                );
        }
        if (this.selectedAction === "getRowByValue") {
            this.parameters
                .at(index)
                .setValue(
                    selectedParameter != undefined
                        ? selectedParameter.name
                        : null
                );
        }
    }

    onLookupValueChange(event: any) { }


    requestAccessToken() {
        //check if there is a google account connected
        this._tenantSettingServiceProxy.googleSheetConfigGet(this.appSession.tenantId).subscribe(response => {
            debugger;
            if (response.accessToken != null) {

                this.tokenClient.requestAccessToken();
                this.modal.hide();
            }
            else {
                this.message.error("", "You have disconnected your Google account. Please reconnect to continue using the integration.");
            }
        })

    }

  loadPicker(accessToken: string) {

        const view = new google.picker.DocsView(google.picker.ViewId.SPREADSHEETS).setIncludeFolders(true);

        const picker = new google.picker.PickerBuilder()
            .setOAuthToken(accessToken)
            .setDeveloperKey(this.googleApiKey)
            .setOrigin(window.location.origin)
            .setAppId('492714879503')
            .addView(view)
            .setCallback((data: any) => this.pickerCallback(data))
            .build();

        picker.setVisible(true);

        // setTimeout(() => {
        //     const blocker = document.querySelector('.picker.shr-bb-shr-cb-shr-ae.picker-dialog-content') as HTMLElement;

        //     if (blocker) {
        //         // Step 1: Remove class
        //         blocker.className = '';

        //         //Step 2: Apply manual centering style
        //         blocker.style.position = 'relative';  // relative positioning
        //         blocker.style.top = '0';              // reset top-left to normal flow
        //         blocker.style.left = '0';
        //         blocker.style.margin = '0 auto';     // center horizontally via margin auto
        //         blocker.style.transform = 'none';

        //         blocker.style.zIndex = '99999';
        //         blocker.style.background = 'none';
        //         blocker.style.boxShadow = '0 0 20px rgba(0,0,0,0.2)';
        //         blocker.style.border = 'none';
        //         blocker.style.padding = '0';
                
        //     }

        //     // Optional: ensure iframe inside is visible
        //     const iframe = document.querySelector('iframe.picker-dialog-frame') as HTMLElement;
        //     if (iframe) {
        //         iframe.style.display = 'block';
        //         iframe.style.opacity = '1';
        //         iframe.style.zIndex = '99999';
        //         iframe.style.border = 'none';
        //     }

        // }, 1000);


    }  
 
    pickerCallback(data: any) {
        if (data.action === google.picker.Action.PICKED) {
            this.modal.show();
            const doc = data.docs[0];
            this.spreadSheetId = doc.id;
            this.spreadSheetName = doc.name;
            this.dialogForm.patchValue({
                spreadSheetId: doc.id,
                spreadSheetName: doc.name
            });
            this.onSpreadSheetChange(this.spreadSheetId);
        }

    }


    loadGapiApi(): Promise<void> {
        return new Promise((resolve, reject) => {
            if (window['gapi'] && gapi.load) {
                resolve();
            } else {
                const script = document.createElement('script');
                script.src = 'https://apis.google.com/js/api.js';
                script.onload = () => resolve();
                script.onerror = () => reject('Failed to load gapi script');
                document.body.appendChild(script);
            }
        });
    }



getCatalogueID() {
    this._tenantService.getCatalogue(this.appSession.tenantId).subscribe((result: CatalogueDto) => {
        this.catalog = [];

        if (result.data && result.data.length > 0) {
            this.catalog = result.data.map(item => ({
                id: item.id,
                name: item.name || 'Catalog_products'
            }));
        }

        console.log('Updated catalog:', this.catalog);
    });
}

    getProductfromCatalogue() {
        this._tenantService.getCatalogueItems(this.appSession.tenantId).subscribe((result: any) => {
            if (result?.data && Array.isArray(result.data)) {
                this.availableProducts = result.data.map((product: any) => ({
                    productId: product.id,
                    name: product.name,
                    retailer_Id: product.retailer_Id ,
                    description: product.description || '',
                    price: product.price ? product.price.replace('JOD', '') : '',
                    currency: product.currency || 'JOD',
                    imageUrl: product.image_Url || product.url || ''
                }));
            } else {
                console.warn('Unexpected API response structure', result);
                this.availableProducts = []; 
            }
        }, (error) => {
            console.error('Error fetching products:', error);
            this.availableProducts = []; 
        });
    }
test(){
    debugger;
     this.availableProducts ;
    const selectedProducts = this.dialogForm.get('selectedProducts')?.value;
    console.log('Selected Products:', selectedProducts);
    console.log('Available Products:', this.availableProducts);
    console.log('Catalog:', this.catalog);

    this.availableProducts;
    this.catalog;
}
private setCatalogValidators() {
    // Common validators for both types
    this.dialogForm.get('selectedProducts')?.setValidators(Validators.required);
    this.dialogForm.get('catalogId')?.setValidators(Validators.required);
    this.dialogForm.get('bodyTextEn')?.setValidators(Validators.required);
    this.dialogForm.get('footerTextEn')?.setValidators(null);

    // Type-specific validators
    if (this.type === "Cataloge multity Product") {
        this.dialogForm.get('sectionTitle')?.setValidators(Validators.required);
        this.dialogForm.get('headerTextEn')?.setValidators(Validators.required);
    } else {
        this.dialogForm.get('sectionTitle')?.clearValidators();
        this.dialogForm.get('headerTextEn')?.clearValidators();
    }

    // Update validity for all fields
    [
        'selectedProducts', 'catalogId', 'bodyTextEn', 'footerTextEn',
        'sectionTitle', 'headerTextEn'
    ].forEach(field => {
        this.dialogForm.get(field)?.updateValueAndValidity();
    });
}
}
