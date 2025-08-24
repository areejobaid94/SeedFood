import {
    ChangeDetectorRef,
    Component,
    ElementRef,
    HostListener,
    Injector,
    OnInit,
    QueryList,
    Renderer2,
    ViewChild,
    ViewChildren,
    inject,
} from "@angular/core";

import { ActivatedRoute, Router } from "@angular/router";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { CreateDialogComponent } from "./create-dialog/create-dialog.component";
import { NgZone } from "@angular/core";
import {
    BotFlowServiceProxy,
    GetBotFlowForViewDto,
    GetBotModelFlowForViewDto,
    ContentInfo,
    Dtocontent,
    ImageFlowModel,
    RequestModel,
    ConditionalModel,
    ConditionList,
    AreasServiceProxy,
    UserServiceProxy,
    UserListDto,
    ParameterSet,
    ScheduleModel,
    MessageTemplateModel,
    WhatsAppMessageTemplateServiceProxy,
    GoogleSheetIntegrationModel,
    Cacatalog,
    CatalogTemplateDto,
    TemplateFooter,
    TemplateBody,
    TemplateHeader,
    ProductDto,
    TenantServiceProxy
} from "@shared/service-proxies/service-proxies";
import uniqid from "uniqid";
import moment from "moment";
import { TestBedStatic } from "@angular/core/testing";
import { parse } from "path";
import { Subject } from "rxjs";
import { DebouncerService } from "../debouncer.service";
import { AreaDto } from "../bot-builder-dialog.model";
import { debounceTime } from "rxjs/operators";
import { NodeSearchModalComponent } from "./node-search-modal/node-search-modal.component";
import { Catalog, CatalogService, Product } from "./catalog.service";
declare var $: any;

@Component({
    selector: "app-bot-builder-page",
    templateUrl: "./bot-builder-page.component.html",
    styleUrls: ["./bot-builder-page.component.scss"],
})
export class BotBuilderPageComponent extends AppComponentBase {
    // @ts-ignore
    @ViewChildren("nodeRef") nodesElements: QueryList<ElementRef>;
    // @ts-ignore
    @ViewChild("svgContainer") svgContainer: ElementRef;
    @ViewChild("createBotDialog", { static: true })
    createBotDialog: CreateDialogComponent;
    @ViewChild("NodeSearch", { static: true })
    NodeSearch: NodeSearchModalComponent;
    @ViewChild("testBot", { static: true })
    testBot: TestBedStatic;
    @ViewChild("dropdownMenu", { static: false }) dropdownMenu: ElementRef;
    @ViewChild("dropdownMenu1", { static: false }) dropdownMenu2: ElementRef;
    @ViewChild("builderRoot") zoomSection: ElementRef;
    @ViewChild("builderRoot") mainContent: ElementRef;
    @ViewChild("contentOverview") contentOverview: ElementRef;
    @ViewChild("viewport") viewport: ElementRef;
    private subject = new Subject<any>();
    private nodeCounter: { [key: string]: number };
    zoomLevel: number = 100;
    nodes: GetBotFlowForViewDto[] = [];
    bot = new GetBotModelFlowForViewDto();
    searchDialogTypes = "";
    saving = false;
    isDrawLines: boolean = true;
    catalog!: Catalog;
    selectedType: 'single' | 'multi' = 'single';
    selectedProduct?: Product;
    selectedProducts: Product[] = [];
    availableProducts:any;
    // availableProducts: ProductDto[] = [
    //     new ProductDto({
    //         productId: 'B001',
    //         name: 'Burger',
    //         description: 'Delicious beef burger with cheese',
    //         price: '5.99',
    //         currency: 'USD',
    //         imageUrl: 'assets/images/burger.jpg'
    //     }),
    //     new ProductDto({
    //         productId: 'S002',
    //         name: 'Shawarma',
    //         description: 'Middle Eastern grilled meat wrap',
    //         price: '6.49',
    //         currency: 'USD',
    //         imageUrl: 'assets/images/shawarma.jpg'
    //     }),
    //         new ProductDto({
    //         productId: 'P003',
    //         name: 'Shawarma',
    //         description: 'Middle Eastern grilled meat wrap',
    //         price: '6.49',
    //         currency: 'USD',
    //         imageUrl: 'assets/images/shawarma.jpg'
    //     }),
    //         new ProductDto({
    //         productId: 'T006',
    //         name: 'Shawarma',
    //         description: 'Middle Eastern grilled meat wrap',
    //         price: '6.49',
    //         currency: 'USD',
    //         imageUrl: 'assets/images/shawarma.jpg'
    //     }),
    //         new ProductDto({
    //         productId: 'IC09',
    //         name: 'Shawarma',
    //         description: 'Middle Eastern grilled meat wrap',
    //         price: '6.49',
    //         currency: 'USD',
    //         imageUrl: 'assets/images/shawarma.jpg'
    //     }),
    // ];
    
    dialogTypes = [
        {
            name: this.l("sendMessage"),
            id: "Send Message",
            icon: "bi bi-chat-left",
            color: "#34b8ca",
        },
        {
            name: this.l("branch"),
            id: "Branches",
            icon: "bi  bi-geo",
            color: "#0802A3",
        },
        {
            name: this.l("branchEN"),
            id: "BranchesEN",
            icon: "bi  bi-geo",
            color: "#0802A3",
        },
        // {
        //     name: this.l("language"),
        //     id: "Language",
        //     icon: "bi bi-translate",
        //     color: "#fc7c29",
        // },
        {
            name: this.l("replyButtons"),
            id: "Reply buttons",
            icon: "bi bi-reply",
            color: "#359cec",
        },
        {
            name: this.l("humanHandOver"),
            id: "Human handover",
            icon: "bi bi-headset",
            color: "#f8cc72",
        },
        // {
        //     name: this.l("collectInput"),
        //     id: "Collect input",
        //     icon: "bi bi-input-cursor",
        //     color: "#cb62e4",
        // },
        {
            name: this.l("jump"),
            id: "Jump",
            icon: "bi bi-shuffle",
            color: "#fc7c29",
        },
        {
            name: this.l("scheduleNode"),
            id: "ScheduleNode",
            icon: "bi bi-clock-history",
            color: "#b8a609",
        },
        {
            name: this.l("listOptions"),
            id: "List options",
            icon: "bi bi-card-checklist",
            color: "#09B253",
        },
        {
            name: this.l("setParameter"),
            id: "Set Parameter",
            icon: "bi bi-p-square",
            color: "#ef6c00",
        },
        // {
        //     name: this.l("interactiveTemplate"),
        //     id: "Interactive template",
        //     icon: "bi bi-chat-left-text",
        //     color: "#2ab9cd",
        // },
        {
            name: this.l("delay"),
            id: "Delay",
            icon: "bi bi-hourglass-split",
            color: "#cb62e4",
        },
        {
            name: this.l("httpRequest"),
            id: "Http request",
            icon: "bi bi-code-slash",
            color: "#fc7c29",
        },
        {
            name: this.l("condition"),
            id: "Condition",
            icon: "bi bi-funnel",
            color: "#2eb67d",
        },
        {
            name: this.l("End"),
            id: "End",
            icon: "bi bi-sign-stop",
            color: "#ff0000",
        },
        {
            name: this.l("Integration"),
            id: "Integration",
            icon: "bi bi-plug",
            color: "#9b59b6",
        },
        {
            name: this.l("catalogeSingleProduct"),
            id: "Cataloge Single Product",
            icon: "bi bi-bag",
            color: "#ff9800",
        },
        {
            name: this.l("catalogeMultityProduct"),
            id: "Cataloge multity Product",
            icon: "bi bi-basket",
            color: "#4caf50",
        },
    ];
    usersAssign: UserListDto[] = [];
    isDragging = false;
    isDraggingNode = false;
    private startX: number = 0;
    branches: AreaDto[] = [];
    branchesEN: AreaDto[] = [];



    private startY: number = 0;
    private initialX: number = 0; // starting position of central div
    private initialY: number = 0; // starting position of central div
    node: any;
    index: any;
    id = null;
    hasError = false;
    selectedNode: any;
    selectedItem: any;
    nodesWithErrors = [];
    showMiniMap = false;
    dragOccurred = false;
    IsCalculation = false;
    isMouseDown = false;
    private lastExecutionTime = 0;
    private throttleDelay = 32; // roughly 30 FPS
    private centerDiv: HTMLElement;
    isDrown = false;
    Templates: MessageTemplateModel[] = [new MessageTemplateModel()];

    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private el: ElementRef,
        private renderer: Renderer2,
        private cdr: ChangeDetectorRef,
        private router: Router,
        private ngZone: NgZone,
        private _Activatedroute: ActivatedRoute,
        private _BotFlowServiceProxy: BotFlowServiceProxy,
        private debouncerService: DebouncerService,
        private elementRef: ElementRef,
        private _areasServiceProxy: AreasServiceProxy,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        // private catalogService: CatalogService,
        private _tenantService: TenantServiceProxy ,
        
    ) {
        super(injector);
    }

    _userServiceProxy: UserServiceProxy = inject(UserServiceProxy);

    onDragStateChange(isDragging: boolean): void {
        debugger;
        this.isDragging = false;
        this.isDraggingNode = isDragging;
    }
    getBranch() {
        this.branches = [];
        this._areasServiceProxy
            .getAvailableAreas(this.appSession.tenantId)
            .subscribe((result) => {
                this.branches = result.map((bracnh) => ({
                    ...bracnh,
                    checked: false,
                }));
            });
    }
        getBranchEN() {
        this.branchesEN = [];
        this._areasServiceProxy
            .getAvailableAreas(this.appSession.tenantId)
            .subscribe((result) => {
                this.branchesEN = result.map((bracnh) => ({
                    ...bracnh,
                    checked: false,
                }));
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

    updateViewport() {
        const main = this.mainContent.nativeElement;
        const viewport = this.viewport.nativeElement;
        const ratio =
            this.contentOverview.nativeElement.clientHeight / main.scrollHeight;

        viewport.style.height = main.clientHeight * ratio + "px";
        viewport.style.top = main.scrollTop * ratio + "px";
    }
    hasOnlySpaces(inputString: string): boolean {
        return /^\s*$/.test(inputString);
    }

    ngOnInit(): void {

        this.getProductfromCatalogue()
        this.id = this._Activatedroute.snapshot.paramMap.get("id");
        this.getbotById(this.id);
        this.getBranchEN();
        this.getBranch();

        this.getTenantUsers();
        this.getTemplates();

        //  this.Templates ;
        this.centerDiv = this.el.nativeElement.querySelector(".center-div");

    }

    @HostListener("mousedown", ["$event"])
    onMouseDown(event: MouseEvent): void {
        if (this.IsCalculation) return;
        this.isMouseDown = true;
        this.renderer.setStyle(document.body, "user-select", "none"); // Prevent text selection
        this.isDragging = true;
        this.startX = event.clientX;
        this.startY = event.clientY;
        this.dragOccurred = false;
        this.stopPropagation(event);
        this.renderer.setStyle(this.el.nativeElement, "cursor", "grabbing");
    }

    @HostListener("mouseup", ["$event"])
    onMouseUp(event: MouseEvent): void {
        console.log("i am called from perant");
        if (this.dragOccurred) {
            // Calculate final position
            const dx = event.clientX - this.startX;
            const dy = event.clientY - this.startY;
            this.initialX += dx;
            this.initialY += dy;

            this.renderer.setStyle(
                this.centerDiv,
                "transform",
                `translate(${this.initialX}px, ${this.initialY}px)`
            );
            // translate(${5390 + 400}px, ${-530 + 400}px)`
            // console.log(this.initialX);
            // console.log(this.initialY);
        }
        this.isDragging = false;
        this.dragOccurred = false;
        this.stopPropagation(event);
        this.renderer.setStyle(this.el.nativeElement, "cursor", "grab");
        this.renderer.removeStyle(document.body, "user-select"); // Restore text selection
        this.IsCalculation = false;
        this.isMouseDown = false;
        if (this.isDrawLines) {
            this.drawConnections();
        }
    }

    @HostListener("mousemove", ["$event"])
    onMouseMove(event: MouseEvent): void {
        if (!this.isDragging) return;

        const currentTime = Date.now();
        if (currentTime - this.lastExecutionTime < this.throttleDelay) {
            return; // Throttle the execution
        }

        this.lastExecutionTime = currentTime; // Update last execution time

        event.preventDefault(); // Prevent default actions during dragging
        this.stopPropagation(event);

        const dx = event.clientX - this.startX;
        const dy = event.clientY - this.startY;
        const newTransform = `translate(${this.initialX + dx}px, ${
            this.initialY + dy
        }px)`;

        this.updateTransform(newTransform);
        this.dragOccurred = true;
    }

    @HostListener("mouseleave", ["$event"])
    onMouseLeave(event: MouseEvent): void {
        this.isDragging = false;
        this.stopPropagation(event);
        this.renderer.setStyle(this.el.nativeElement, "cursor", "grab");
    }

    @HostListener("window:resize", ["$event"])
    onResize(event: Event): void {
        if (this.IsCalculation) return;
        this.stopPropagation(event);
        this.undrawLins();
        //   this.drawConnections();
    }
    drawLinesToggle() {
        this.isDrawLines = !this.isDrawLines;
        if (this.isDrawLines) {
            this.undrawLins();
            this.drawConnections();
        }
    }
    moveToNode(data: number[]) {
        this.undrawLins();
        this.initialX = data[0] * -1 + 200;
        this.initialY = data[1] * -1 + 300;
        this.renderer.setStyle(
            this.centerDiv,
            "transform",
            `translate(${this.initialX}px, ${this.initialY}px)`
        );
    }

    openSearch() {
        this.NodeSearch.open();
    }

    private updateTransform(newTransform: string): void {
        this.renderer.setStyle(this.centerDiv, "transform", newTransform);
        this.undrawLins(); // Ensure any additional logic here is optimized
        this.drawConnections();
    }

    checkIfItold() {}

    getbotById(id) {
        this.saving = false;
        this._BotFlowServiceProxy.getByIdBotFlows(id).subscribe((result) => {
            this.nodesWithErrors = [];
            this.bot = result;
            this.nodes = result.getBotFlowForViewDto;
            let content = new ContentInfo();
            content.dtoContent = [];
            let firstNode = {
                id: this.stringToNumber(uniqid("IB-")),
                captionAr: "Bot is triggered if..",
                captionEn: "Bot is triggered if..",
                footerTextAr: "",
                footerTextEn: "",
                parentIndex: [],
                childIndex: 1,
                parentId: [],
                childId: 1,
                top: 30,
                bottom: 0,
                left: 50,
                inputHint: "",
                isOneTimeQuestion: false,
                isAdvance: false,
                rigth: 0,
                urlImage: "",
                isRoot: true,
                isNodeRoot: false,
                urlImageArray: [],
                dilationTime: 0,
                listOfUsers: "",
                listOfTeams: "",
                actionBlock: "",
                templateId: "",
                content: content,
                parameter: "",
                parameterType: "",
                title: "Bot is triggered if..",
                jumpId: null,
                request: new RequestModel(),
                type: "Bot is triggered if..",
                conditional: new ConditionalModel(),
            };
            //OutBound WhatsApp Template
            if (this.bot.statusId === 4 && this.nodes === undefined) {
                this.nodes = [];
                let content = new ContentInfo();
                content.dtoContent = [];
                let condition = new ConditionalModel();
                condition.orAnd = "";
                condition.conditionList = [];
                this.nodes.push({
                    id: this.stringToNumber(uniqid("IB-")),
                    captionAr: "Bot is triggered if..",
                    captionEn: "Bot is triggered if..",
                    footerTextAr: "",
                    footerTextEn: "",
                    parentIndex: [],
                    childIndex: null,
                    parameterList: [],
                    parentId: [],
                    childId: null,
                    top: 30,
                    bottom: 0,
                    isNodeRoot: false,
                    left: 50,
                    rigth: 0,
                    urlImage: new ImageFlowModel(),
                    content: content,
                    isAdvance: false,
                    parameter: "",
                    schedule: null,
                    isRoot: true,
                    urlImageArray: [],
                    isOneTimeQuestion: false,
                    dilationTime: 0,
                    listOfUsers: "",
                    listOfTeams: "",
                    actionBlock: "",
                    templateId: "",
                    parameterType: "",
                    inputHint: "",
                    counterNode: 0,
                    title: "Bot is triggered if..",
                    jumpId: null,
                    googleSheetIntegration : new GoogleSheetIntegrationModel(),
                    catalogTemplateDto : new CatalogTemplateDto(),
                    request: new RequestModel(),
                    conditional: condition,
                    type: "Bot is triggered if..",

                    init: function (_data?: any): void {
                        throw new Error("Function not implemented.");
                    },
                    toJSON: function (data?: any) {
                        throw new Error("Function not implemented.");
                    },
                });
                this.nodes.push({
                    id: this.stringToNumber(uniqid("IB-")),
                    captionAr: "WhatsApp template",
                    captionEn: "WhatsApp template",
                    footerTextAr: "",
                    footerTextEn: "",
                    isOneTimeQuestion: false,
                    isAdvance: false,
                    parentIndex: [0],
                    childIndex: null,
                    parentId: [this.nodes[0].id],
                    childId: null,
                    top: this.nodes[0].top + 150,
                    bottom: 0,
                    left: this.nodes[0].left,
                    isNodeRoot: true,
                    rigth: 0,
                    urlImage: new ImageFlowModel(),
                    content: content,
                    parameter: "",
                    parameterList: [],
                    isRoot: false,
                    urlImageArray: [],
                    schedule: null,
                    dilationTime: 0,
                    inputHint: " ",
                    counterNode: 0,
                    listOfUsers: "",
                    listOfTeams: "",
                    actionBlock: "",
                    templateId: "",
                    parameterType: "",
                    title: "Bot is triggered if..",
                    jumpId: null,
                    googleSheetIntegration : new GoogleSheetIntegrationModel(),
                    catalogTemplateDto : new CatalogTemplateDto(),
                    request: new RequestModel(),
                    type: "whatsApp template",
                    conditional: condition,
                    init: function (_data?: any): void {
                        throw new Error("Function not implemented.");
                    },
                    toJSON: function (data?: any) {
                        throw new Error("Function not implemented.");
                    },
                });
                this.nodes[0].childIndex = 1;
                this.nodes[0].childId = this.nodes[1].id;
                this.drawConnections();
                this.repopulateNodeCounter();
                return;
            } else if (this.bot.statusId === 4 && this.nodes != undefined) {
                let findFirstNode = this.nodes.find((node) => node.isNodeRoot);
                if (!findFirstNode) {
                    findFirstNode = this.nodes.find((x) =>
                        x.parentIndex.includes(0)
                    );
                }
                firstNode.childId = findFirstNode.id;
                firstNode.id = findFirstNode.parentId[0];
                firstNode.top = findFirstNode.top - 150;
                firstNode.left = findFirstNode.left;
                this.nodes.unshift(firstNode as any);
                this.nodes.forEach((node, index) => {
                    if (node.type === "text") {
                        node.type = "Send Message";
                    } else if (node.type === "button") {
                        node.type = "Reply buttons";
                    }
                    if (node.childIndex === -1) {
                        node.childIndex = null;
                    }
                    if (node.content.dtoContent != undefined) {

                        node.content.dtoContent.forEach((content) => {
                            if (content.childIndex === -1) {
                                content.childIndex = null;
                            }
                        });
                    }
                    if (
                        node.title === undefined ||
                        node.title === null ||
                        node.title === ""
                    ) {
                        node.title = node.type;
                    }
                    if (node.conditional === undefined) {
                        node.conditional = new ConditionalModel();
                        node.conditional.orAnd = "";
                        node.conditional.conditionList = [];
                    }
                });
                this.drawConnections();
                this.repopulateNodeCounter();
                return;
            }
            if (this.nodes === undefined || this.nodes.length === 0) {
                this.nodes = [];
                this.nodes.push({
                    id: this.stringToNumber(uniqid("IB-")),
                    captionAr: "Bot is triggered if..",
                    captionEn: "Bot is triggered if..",
                    footerTextAr: "",
                    footerTextEn: "",
                    parentIndex: [],
                    childIndex: null,
                    parentId: [],
                    childId: null,
                    parameterList: [],
                    isAdvance: false,
                    top: 30,
                    bottom: 0,
                    inputHint: "",
                    schedule: null,
                    isOneTimeQuestion: false,
                    left: 50,
                    rigth: 0,
                    counterNode: 0,
                    urlImage: new ImageFlowModel(),
                    content: new ContentInfo(),
                    parameter: "",
                    isRoot: true,
                    isNodeRoot: false,
                    urlImageArray: [],
                    dilationTime: 0,
                    listOfUsers: "",
                    listOfTeams: "",
                    actionBlock: "",
                    templateId: "",
                    parameterType: "",
                    title: "Bot is triggered if..",
                    jumpId: null,
                    googleSheetIntegration : new GoogleSheetIntegrationModel(),
                    catalogTemplateDto : new CatalogTemplateDto(),

                    request: new RequestModel(),
                    type: "Bot is triggered if..",
                    conditional: new ConditionalModel(),
                    init: function (_data?: any): void {
                        throw new Error("Function not implemented.");
                    },
                    toJSON: function (data?: any) {
                        throw new Error("Function not implemented.");
                    },
                });
            } else {
                // firstNode.childId = this.nodes[0].id;
                // firstNode.id = this.nodes[0].parentId[0];

                let findFirstNode = this.nodes.find((node) => node.isNodeRoot);
                if (!findFirstNode) {
                    findFirstNode = this.nodes.find((x) =>
                        x.parentIndex.includes(0)
                    );
                }

                firstNode.childId = findFirstNode?.id;
                firstNode.id = findFirstNode.parentId[0];
                firstNode.top = findFirstNode.top - 150;
                firstNode.left = findFirstNode.left;
                this.nodes.unshift(firstNode as any);
                this.nodes.forEach((node, index) => {
                    if (node.type === "text") {
                        node.type = "Send Message";
                    } else if (node.type === "button") {
                        node.type = "Reply buttons";
                    }
                    if (node.childIndex === -1) {
                        node.childIndex = null;
                    }
                    if (
                        node.title === undefined ||
                        node.title === null ||
                        node.title === ""
                    ) {
                        node.title = node.type;
                    }
                    if (node.content.dtoContent != undefined) {
                        node.content.dtoContent.forEach((content) => {
                            if (content.childIndex === -1) {
                                content.childIndex = null;
                            }
                        });
                    }
                    if (node.conditional === undefined) {
                        node.conditional = new ConditionalModel();
                        node.conditional.orAnd = "";
                        node.conditional.conditionList = [];
                    }
                });
                this.drawConnections();
            }

            this.repopulateNodeCounter();
        });
    }



        fillBranches(index: number) {
        return this.branches.map(
            (area) =>
                new Dtocontent({
                    parentIndex: [index],
                    childIndex: null,
                    parentId: [this.nodes[index].id],
                    childId: null,
                    valueAr: area.areaName,
                    valueEn: area.areaNameEnglish,
                    key: this.stringToNumber(uniqid("IB-")),
                    branchID: area.id.toString(),
                })
        );
    }
        fillBranchesEN(index: number) {
        return this.branchesEN.map(
            (area) =>
                new Dtocontent({
                    parentIndex: [index],
                    childIndex: null,
                    parentId: [this.nodes[index].id],
                    childId: null,
                    valueAr: area.areaName,
                    valueEn: area.areaNameEnglish,
                    key: this.stringToNumber(uniqid("IB-")),
                    branchID: area.id.toString(),
                })
        );
    }

    addNode(
        index: number,
        event: any,
        type,
        isRoot: boolean = false,
        isFromButton?,
        buttonIndex?,
        buttonId?
    ) {
        debugger;

        if (!this.nodeCounter[type]) {
            this.nodeCounter[type] = 1;
        } else {
            this.nodeCounter[type]++;
        }
        if (event.target.type !== "text") {
            event.stopPropagation();
        }
        const dropdown = this.dropdownMenu.nativeElement;
        this.renderer.removeClass(dropdown, "show");
        const dropdown2 = this.dropdownMenu2.nativeElement;
        this.renderer.removeClass(dropdown2, "show");
        this.node = new GetBotFlowForViewDto();
        let node = new GetBotFlowForViewDto();
        node.id = this.stringToNumber(uniqid("IB-"));
        node.captionAr = "مرحباً، اسمي ...";
        node.captionEn = "Hi There, My Name Is ... ";
        node.footerTextAr = "";
        node.footerTextEn = "";
        (node.inputHint = ""),
            (node.isOneTimeQuestion = false),
            (node.top = this.nodes[index].top + 150);
        node.rigth = 0;
        node.bottom = 0;
        node.isAdvance = false;
        node.left = this.nodes[index].left;
        node.content = new ContentInfo();
        node.content.txt = "";
        node.content.dtoContent = [];
        node.parameter = "";
        node.urlImage = new ImageFlowModel();
        node.urlImageArray = [];
        node.parameterList = [];
        node.childIndex = null;
        node.parentIndex = [index];
        node.childId = null;
        node.parentId = [this.nodes[index].id];
        node.type = type;
        node.isRoot = false;
        node.isNodeRoot = isRoot;
        node.title = `${type} ${this.nodeCounter[type]}`;
        node.jumpId = null;
        node.request = new RequestModel();
        node.parameterType = "";
        node.schedule = null;
        node.conditional = new ConditionalModel();
        node.conditional.orAnd = "";
        node.conditional.conditionList = [];
        if (isRoot) {
            let rootedNodeIndex = this.nodes.findIndex(
                (node) => node.isNodeRoot
            );
            if (rootedNodeIndex > 0) {
                this.nodes[rootedNodeIndex].isNodeRoot = false;
            }
            node.isNodeRoot = true;
        }
        if (this.nodes[index].type === "Delay") {
            node.top = this.nodes[index].top + 200;
        }
        if (isFromButton) {
            this.nodes[index].content.dtoContent[buttonIndex].childIndex =
                this.nodes.length;
            node.top = this.nodes[index].top + 200;
            let buttonCount = 5;
            if (
                this.nodes[index].content.dtoContent.length > buttonCount + 1 &&
                buttonIndex > 2
            ) {
                node.left =
                    this.nodes[index].left - 200 + buttonIndex * 100 - 200;
            } else if (
                this.nodes[index].content.dtoContent.length > buttonCount + 1 &&
                buttonIndex <= 2
            ) {
                node.left =
                    this.nodes[index].left - 200 + buttonIndex * 100 - 200;
            } else if (
                this.nodes[index].content.dtoContent.length === buttonCount ||
                this.nodes[index].content.dtoContent.length === 4
            ) {
                node.left =
                    this.nodes[index].left - 50 + buttonIndex * 100 - 100;
            } else if (this.nodes[index].content.dtoContent.length === 6) {
                node.left =
                    this.nodes[index].left - 50 + buttonIndex * 100 - 100;
            } else {
                node.left = this.nodes[index].left - 50 + buttonIndex * 100;
            }
            this.nodes[index].content.dtoContent[buttonIndex].childId = node.id;
            node.parentId = [
                this.nodes[index].content.dtoContent[buttonIndex].key,
            ];
            node.parentIndex = [buttonIndex];
        } else {
            this.nodes[index].childIndex = this.nodes.length;
            this.nodes[index].childId = node.id;
        }
        if (type == "Send Message" || type == "Collect input") {
            this.nodes.push(node);
        } else if (type == "Reply buttons") {
            node.captionAr = "ماذا تريد ان تختار؟";
            node.captionEn = "What would you like to choose?";
            node.content = new ContentInfo();
            node.content.txt = "";
            node.content.dtoContent = [
                new Dtocontent({
                    parentIndex: [index],
                    childIndex: null,
                    parentId: [this.nodes[index].id],
                    childId: null,
                    valueAr: "زر 1",
                    valueEn: "Button 1",
                    key: this.stringToNumber(uniqid("IB-")),
                    branchID: "-1",
                }),
            ];
            this.nodes.push(node);
        } else if (type == "Cataloge Single Product") {
                node.captionAr = "ماذا تريد ان تختار؟";
                node.captionEn = "What would you like to choose?";
                node.catalogTemplateDto = new CatalogTemplateDto();

                const template = node.catalogTemplateDto; 
                if (!template.header) template.header = new TemplateHeader();
                if (!template.body) template.body = new TemplateBody();
                if (!template.footer) template.footer = new TemplateFooter();
                if (!template.catalog) template.catalog = new Cacatalog();

                // template.header.text = "";
                template.body.text = "";
                template.footer.text = "";

                template.catalog.catalogId = "";
                template.catalog.catalogName = "";
                template.catalog.products = []; 
                this.nodes.push(node);

        }else if (type == "Cataloge multity Product") {
            debugger
                node.captionAr = "ماذا تريد ان تختار؟";
                node.captionEn = "What would you like to choose?";
                node.catalogTemplateDto = new CatalogTemplateDto();

                const template = node.catalogTemplateDto; 
                if (!template.header) template.header = new TemplateHeader();
                if (!template.body) template.body = new TemplateBody();
                if (!template.footer) template.footer = new TemplateFooter();
                if (!template.catalog) template.catalog = new Cacatalog();

                template.header.text = "";
                template.body.text = "";
                template.footer.text = "";

                template.catalog.catalogId = "";
                template.catalog.catalogName = "";
                template.catalog.sectionTitle = "";
                template.catalog.products = []; 
                this.nodes.push(node);
        } 
        else if (type === "Condition") {
            node.captionAr = " ";
            node.captionEn = "This is condition content ";
            node.content = new ContentInfo();
            node.content.txt = "";
            node.conditional.orAnd = "AND";
            node.conditional.conditionList = [
                new ConditionList({
                    op1: null,
                    operation: "Equal to",
                    op2: null,
                }),
            ];
            node.content.dtoContent = [
                new Dtocontent({
                    parentIndex: [index],
                    childIndex: null,
                    parentId: [this.nodes[index].id],
                    childId: null,
                    valueAr: "نعم",
                    valueEn: "Yes",
                    key: this.stringToNumber(uniqid("IB-")),
                    branchID: "-1",
                }),
                new Dtocontent({
                    parentIndex: [index],
                    childIndex: null,
                    parentId: [this.nodes[index].id],
                    childId: null,
                    valueAr: "لا",
                    valueEn: "No",
                    key: this.stringToNumber(uniqid("IB-")),
                    branchID: "-1",
                }),
            ];
            this.nodes.push(node);
        }

        // else if (type == "Language") {
        //     node.captionAr = "من فضلك اختر اللغة؟";
        //     node.captionEn = "Plaese choose Language?";
        //     node.content = new ContentInfo();
        //     node.content.txt = '';
        //     node.parameter = "Language";
        //     node.content.dtoContent = [new Dtocontent({
        //         parentIndex: index,
        //         childIndex: null,
        //         parentId: this.nodes[index].id,
        //         childId: null,
        //         valueAr: "English",
        //         valueEn: "English",
        //         key: this.stringToNumber(uniqid('IB-')),
        //     },
        //     ),
        //     new Dtocontent({
        //         parentIndex: index,
        //         childIndex: null,
        //         parentId: this.nodes[index].id,
        //         childId: null,
        //         valueAr: "العربية",
        //         valueEn: "العربية",
        //         key: this.stringToNumber(uniqid('IB-')),
        //     })]
        //     this.nodes.push(node);
        // }
        else if (type == "List options") {
            node.captionAr = "اختر من الخيارات التالية";
            node.captionEn = "Choose From The Below Options";

            node.content = new ContentInfo();
            node.content.txt = "";
            node.content.dtoContent = [
                new Dtocontent({
                    parentIndex: [index],
                    childIndex: null,
                    parentId: [this.nodes[index].id],
                    childId: null,
                    valueAr: "الخيار 1",
                    valueEn: "Option 1",
                    key: this.stringToNumber(uniqid("IB-")),
                    branchID: "-1",
                }),
            ];
            this.nodes.push(node);
        } 
        else if (type == "Branches") {          
            node.captionAr = "اختر الفرع";
            node.captionEn = "Choose a Branch";
            node.content = new ContentInfo();
            node.content.txt = "";
            let branches: Dtocontent[] = this.fillBranches(index);
            debugger
            node.content.dtoContent = branches;
            this.nodes.push(node);
        }
        else if (type == "BranchesEN") {
            node.captionAr = "اختر الفرع";
            node.captionEn = "Choose a Branch";
            node.content = new ContentInfo();
            node.content.txt = "";
            let branchesEN: Dtocontent[] = this.fillBranchesEN(index);
            node.content.dtoContent = branchesEN;
            this.nodes.push(node);
        }  
        
        else if (type === "Delay") {
            node.captionAr =
                "Adds an extra delay on top of what the system current adds between each block";
            node.captionEn =
                "Adds an extra delay on top of what the system current adds between each block";
            node.dilationTime = 1;
            this.nodes.push(node);
        } else if (type === "End") {
            node.captionAr = "Ends the flow.";
            node.captionEn = "Ends the flow.";
            this.nodes.push(node);
        } else if (type === "Human handover") {
            node.captionAr =
                "Forwarding of a conversation from a human to a real person. Chatbot to a real human being";
            node.captionEn =
                "Forwarding of a conversation from a human to a real person. Chatbot to a real human being";
            this.nodes.push(node);
        } else if (type === "Http request") {
            node.captionAr = "";
            node.captionEn = "";
            this.nodes.push(node);
        } else if (type === "Jump") {
            node.captionEn = "";
            this.nodes.push(node);
        } else if (type === "Set Parameter") {
            node.captionAr = "نعيين متغيرات";
            node.captionEn = "Set Parameter";

            let parameter = new ParameterSet();
            node.parameterList = [parameter];

            this.nodes.push(node);
        } else if (type === "ScheduleNode") {
            node.captionAr = "جدوله";
            node.captionEn = "Schedule";
            node.schedule = null;
            this.nodes.push(node);
        } 
        else if (type === "Integration") {
            node.captionAr = "تكامل مع خدمة خارجية";
            node.captionEn = "Integration with external service";
            //node.content = new ContentInfo();
            //node.content.txt = "";
            //node.parameter = ""; 
            this.nodes.push(node);
        }

        setTimeout(() => {
            if (isFromButton) {
                let nodeElement = this.nodesElements.toArray()[index];
                let node = this.nodes[index];
                const svg = this.svgContainer.nativeElement;
                this.drawDTOContentPaths(
                    nodeElement,
                    node.content.dtoContent,
                    index,
                    svg,
                    node
                );
            } else {
                let nodeElement = this.nodesElements.toArray()[index];
                const svg = this.svgContainer.nativeElement;
                this.drawPath(nodeElement, index, svg);
            }
        }, 100);

        // this.drawConnections();
    }
    stopPropagation(event: Event): void {
        event.stopPropagation();
    }

    repopulateNodeCounter() {
        this.nodeCounter = {};
        let max: { [key: string]: any } = {};
        this.nodes.forEach((node) => {
            if (node.type) {
                if (!this.nodeCounter[node.type]) {
                    this.nodeCounter[node.type] = 0;
                    max[node.type] = 0;
                } else {
                    let counter = Number(node.title.split(" ").pop());
                    max[node.type] = Math.max(counter, max[node.type]);
                    max[node.type] -= 1;
                }
                this.nodeCounter[node.type] = max[node.type];
                this.nodeCounter[node.type]++;
            }
        });
    }

    recenterElements(): void {
        this.initialX = 0;
        this.initialY = 0;
        this.renderer.setStyle(
            this.centerDiv,
            "transform",
            "translate(0px, 0px)"
        );
        this.undrawLins();
        if (this.isDrawLines) {
            this.drawConnections();
        }
    }

    onNodeDrag(event, node, index) {
        let target: HTMLElement = event.mouseEvent.target as HTMLElement;
        let string = "undraggble-hidden-elements";
        let indexOf = target.classList.value.indexOf(string);
        if (indexOf === -1) {
            this.isDragging = false;
            this.isDraggingNode = true;
            this.removeLinesForNode(node);
            this.redrawLinesForNode(node, index);
        } else {
            this.isDragging = false;
            this.isDraggingNode = false;
            return;
        }
    }
    private redrawLinesForNode(node, index) {
        this.debouncerService.debounce(() => {
            this.ngZone.runOutsideAngular(() => {
                // this.removeLinesForNode(node);
                const svg = this.svgContainer.nativeElement;
                if (index <= this.nodes.length - 1) {
                    // Redraw lines after this node
                    const nodeElementAfter = this.nodesElements.find(
                        (element, index) => this.nodes[index] === node
                    );
                    if (!nodeElementAfter) return;
                    if (node.childIndex !== null) {
                        this.drawPath(
                            nodeElementAfter,
                            this.nodes.indexOf(node),
                            svg
                        );
                    } else if (
                        node.childIndex === null &&
                        node.content.hasOwnProperty("dtoContent")
                    ) {
                        if (
                            node.content.dtoContent.length !== 0 &&
                            node.content.dtoContent !== undefined
                        ) {
                            this.drawDTOContentPaths(
                                nodeElementAfter,
                                node.content.dtoContent,
                                this.nodes.indexOf(node),
                                svg,
                                node
                            );
                        }
                    }
                }
                const drawLinesForParent = (parentNode) => {
                    if (parentNode.childIndex !== null) {
                        const nodeElementBefore = this.nodesElements.find(
                            (element, idx) => this.nodes[idx] === parentNode
                        );
                        if (nodeElementBefore) {
                            this.drawPath(
                                nodeElementBefore,
                                this.nodes.indexOf(parentNode),
                                svg
                            );
                        }
                    } else if (
                        parentNode.childIndex === null &&
                        parentNode.content.hasOwnProperty("dtoContent")
                    ) {
                        if (
                            parentNode.content.dtoContent.length !== 0 &&
                            parentNode.content.dtoContent !== undefined
                        ) {
                            const nodeElementBefore = this.nodesElements.find(
                                (element, idx) => this.nodes[idx] === parentNode
                            );
                            if (nodeElementBefore) {
                                this.drawDTOContentPaths(
                                    nodeElementBefore,
                                    parentNode.content.dtoContent,
                                    this.nodes.indexOf(parentNode),
                                    svg,
                                    parentNode
                                );
                            }
                        }
                    }
                };
                if (index > 0) {
                    //Redraw Lines before this node
                    for (const parentId of node.parentId) {
                        const parentNode = this.nodes.find(
                            (x) => x.id == parentId
                        );
                        if (parentNode) {
                            drawLinesForParent(parentNode);
                        } else {
                            this.nodes.forEach((nodee) => {
                                if (
                                    nodee.content.hasOwnProperty("dtoContent")
                                ) {
                                    nodee.content.dtoContent.forEach(
                                        (button) => {
                                            if (button.key == parentId) {
                                                drawLinesForParent(nodee);
                                            }
                                        }
                                    );
                                }
                            });
                        }
                    }
                }
            });
        });
        this.isDrown = true;
    }

    private removeLinesForNode(node) {
        this.ngZone.runOutsideAngular(() => {
            const svg = this.svgContainer.nativeElement;
            const svgElementsToRemove = [];
            for (const svgElement of svg.children) {
                const svgElementNodeId =
                    svgElement.getAttribute("data-node-id");
                const splitIds = svgElementNodeId.split("_");

                if (
                    svgElementNodeId !== null &&
                    (splitIds[0] === node.id.toString() ||
                        splitIds[1] === node.id.toString())
                ) {
                    svgElementsToRemove.push(svgElement);
                }
            }
            svgElementsToRemove.forEach((element) => svg.removeChild(element));
        });
        this.isDrown = false;
    }

    private isUndraggble(event: MouseEvent): boolean {
        // Check if the click event target is inside the undraggble element
        const undraggble = this.el.nativeElement.querySelector(".undraggble");
        return undraggble && undraggble.contains(event.target);
    }

    onNodeStretch(stretchResult: {
        parent: any;
        parentButton: any;
        dropNodeId: any;
    }) {
        let dropNodeId = Number(stretchResult.dropNodeId);
        let childNodeIndex = this.nodes.findIndex((x) => x.id === dropNodeId);
        if (stretchResult.parent.isRoot) {
            let rootedNodeIndex = this.nodes.findIndex(
                (node) => node.isNodeRoot
            );
            if (rootedNodeIndex > 0) {
                this.nodes[rootedNodeIndex].isNodeRoot = false;
            }
            this.nodes[childNodeIndex].isNodeRoot = true;
        }
        if (stretchResult.parentButton) {
            let parentNode = stretchResult.parentButton;
            let parentNodeIndex;
            this.nodes.forEach((node, index) => {
                //check if the node has dtocontent and find the index of the parent node
                if (node.content.hasOwnProperty("dtoContent")) {
                    node.content.dtoContent.forEach((button, index) => {
                        if (button.key == stretchResult.parentButton.key) {
                            parentNodeIndex = index;
                        }
                    });
                }
            });
            let childNode = this.nodes.find((x) => x.id === dropNodeId);
            let childNodeIndex = this.nodes.findIndex(
                (x) => x.id === dropNodeId
            );
            if (childNode && parentNode) {
                parentNode.childIndex = childNodeIndex;
                parentNode.childId = childNode.id;
                //push the parent node index to my child node
                childNode.parentIndex = [
                    ...childNode.parentIndex,
                    parentNodeIndex,
                ];
                childNode.parentId = [...childNode.parentId, parentNode.key];
                if (childNode.content.dtoContent.length > 0) {
                    childNode.content.dtoContent.forEach((button) => {
                        button.parentIndex.push(parentNodeIndex);
                        button.parentId.push(parentNode.key);
                    });
                }
            }
            this.removeLinesForNode(parentNode);
            this.redrawLinesForNode(parentNode, parentNodeIndex);
        } else {
            let parentNode = stretchResult.parent;
            let childNode = this.nodes.find((x) => x.id === dropNodeId);
            let parentNodeIndex = this.nodes.findIndex(
                (x) => x.id == stretchResult.parent.id
            );
            let childNodeIndex = this.nodes.findIndex(
                (x) => x.id === dropNodeId
            );
            if (parentNode && childNode) {
                parentNode.childIndex = childNodeIndex;
                parentNode.childId = childNode.id;
                childNode.parentIndex = [
                    ...childNode.parentIndex,
                    parentNodeIndex,
                ];
                childNode.parentId = [...childNode.parentId, parentNode.id];
                if (childNode.content.dtoContent.length > 0) {
                    childNode.content.dtoContent.forEach((button) => {
                        button.parentIndex.push(parentNodeIndex);
                        button.parentId.push(parentNode.id);
                    });
                }
            }
            this.removeLinesForNode(parentNode);
            this.redrawLinesForNode(parentNode, parentNodeIndex);
        }
    }

    undrawLins() {
        this.ngZone.runOutsideAngular(() => {
            const svg = this.svgContainer.nativeElement;
            while (svg.firstChild) {
                svg.removeChild(svg.firstChild);
            }
        });
        this.isDrown = false;
    }

    drawConnections() {
        if (!this.isDrown) {
            if (this.isMouseDown || this.isDragging) return;
            this.debouncerService.debounce(() => {
                this.ngZone.runOutsideAngular(() => {
                    const svg = this.svgContainer.nativeElement;

                    this.nodesElements.forEach((nodeElement, index) => {
                        const node = this.nodes[index];

                        if (node.childIndex !== null) {
                            this.drawPath(nodeElement, index, svg);
                        } else if (
                            node.childIndex === null &&
                            node.content.hasOwnProperty("dtoContent")
                        ) {
                            if (
                                node.content.dtoContent.length !== 0 &&
                                node.content.dtoContent !== undefined
                            ) {
                                this.drawDTOContentPaths(
                                    nodeElement,
                                    node.content.dtoContent,
                                    index,
                                    svg,
                                    node
                                );
                            }
                        }
                    });
                });
            });
            this.isDrown = true;
        }
    }

    isLineInViewport(node1, node2) {
        const viewportWidth =
            window.innerWidth || document?.documentElement.clientWidth;
        const viewportHeight =
            window.innerHeight || document?.documentElement.clientHeight;
        let isInline =
            (node1?.left + node1?.width <= viewportWidth &&
                node2?.right - node2?.width >= viewportWidth) ||
            (node2?.left + node2?.width <= viewportWidth &&
                node1?.right - node1?.width >= viewportWidth) ||
            (node1?.top <= viewportHeight && node2?.bottom >= viewportHeight) ||
            (node2?.top <= viewportHeight && node1?.bottom >= viewportHeight);

        return isInline;
        // return (
        //     (node1.left+node1.width <= viewportWidth && node2.right-node2.width >= viewportWidth)
        //     ||
        //     (node1.top <= viewportHeight && node2.bottom >= viewportHeight)
        //   );
    }

    private isElementInViewport(rect: DOMRect): boolean {
        return (
            rect?.top >= 0 &&
            rect?.left + rect?.width >= 0 &&
            rect?.bottom <= window.innerHeight &&
            rect?.right - rect?.width <= window.innerWidth
        );
    }

    private drawPath(nodeElement: any, index: number, svg) {
        this.ngZone.runOutsideAngular(() => {
            const svg = this.svgContainer.nativeElement;
            const node = this.nodes[index];

            const parentBox =
                nodeElement.nativeElement?.getBoundingClientRect();
            if (!parentBox) return;
            const child = document.getElementById(`${node.childId}`);
            const childBox = child?.getBoundingClientRect();

            if (
                !this.isElementInViewport(parentBox) &&
                !this.isElementInViewport(childBox)
            ) {
                if (!this.isLineInViewport(parentBox, childBox)) {
                    return;
                } else {
                }
            }

            const commonCoords = this.calculateCommonCoords(
                parentBox,
                childBox
            );

            const path = this.createPath(commonCoords);
            path.style.pointerEvents = "auto";
            path.setAttribute("data-node-id", `${node.id}_${node.childId}`);
            const button = this.createButton(commonCoords);
            this.addDeleteButtonEvents(button, path, node);
            svg.appendChild(path);
            document.body.appendChild(button);
        });
    }

    private drawDTOContentPaths(
        nodeElement: any,
        dtoContents: any[],
        index: number,
        svg,
        node
    ) {
        this.ngZone.runOutsideAngular(() => {
            const fragment = document.createDocumentFragment();
            const initialParent = nodeElement.nativeElement;
            const listOfElements = initialParent.querySelectorAll(".ibbat");

            dtoContents.forEach((item, indaxaye) => {
                if (item.childIndex == null) return;

                const parentBox =
                    listOfElements[indaxaye].getBoundingClientRect();
                const child = document.getElementById(`${item.childId}`);
                const childBox = child?.getBoundingClientRect();

                if (
                    !this.isElementInViewport(parentBox) &&
                    !this.isElementInViewport(childBox)
                ) {
                    if (!this.isLineInViewport(parentBox, childBox)) {
                        return;
                    }
                }

                const commonX = parentBox.left + parentBox?.width / 2;
                const commonY = (parentBox.bottom + childBox?.top) / 2;

                const path = document.createElementNS(
                    "http://www.w3.org/2000/svg",
                    "path"
                );
                path.setAttribute(
                    "d",
                    `M ${commonX} ${
                        parentBox.bottom
                    } C ${commonX} ${commonY}, ${
                        childBox?.left + childBox?.width / 2
                    } ${commonY}, ${childBox?.left + childBox?.width / 2} ${
                        childBox?.top
                    }`
                );
                path.setAttribute("stroke", "#b6b9bb");
                path.setAttribute("fill", "none");
                path.setAttribute("stroke-width", "3");
                path.setAttribute("data-node-id", `${node.id}_${item.childId}`);
                path.style.cursor = "pointer";
                path.style.pointerEvents = "auto";

                const button = document.createElement("button");
                button.style.left = `${
                    (commonX + childBox?.left + childBox?.width / 2) / 2
                }px`;
                button.style.top = `${commonY}px`;

                // Adding delete button events
                this.addDeleteButtonEvents(button, path, null, item);

                fragment.appendChild(path);
                document.body.appendChild(button);
            });

            svg.appendChild(fragment);
        });
    }
    private calculateCommonCoords(parentBox, childBox) {
        const startX = parentBox?.left + parentBox?.width / 2;
        const startY = parentBox?.bottom;
        const endX = childBox?.left + childBox?.width / 2;
        const endY = childBox?.top;
        const commonY = (startY + endY) / 2;
        return { startX, startY, endX, endY, commonY };
    }

    private createPath(commonCoords) {
        const { startX, startY, endX, endY, commonY } = commonCoords;
        const path = document.createElementNS(
            "http://www.w3.org/2000/svg",
            "path"
        );
        path.setAttribute(
            "d",
            `M ${startX} ${startY} C ${startX} ${commonY}, ${endX} ${commonY}, ${endX} ${endY}`
        );
        path.setAttribute("stroke", "#b6b9bb");
        path.setAttribute("fill", "none");
        path.setAttribute("stroke-width", "3");
        path.style.cursor = "pointer";
        return path;
    }

    private createButton(commonCoords) {
        const { startX, startY, endX, endY } = commonCoords;
        const button = document.createElement("button");
        button.setAttribute("class", "btn btn-default bi bi-trash3");
        button.style.position = "absolute";
        button.style.left = `${(startX + endX) / 2}px`;
        button.style.top = `${(startY + endY) / 2}px`;
        button.style.display = "none";
        button.style.zIndex = "999";
        return button;
    }
    private addDeleteButtonEvents(button, path, node, item = null) {
        button.setAttribute("class", "btn btn-default bi bi-trash3");
        button.style.position = "absolute";
        button.style.display = "none";
        button.style.zIndex = "999";

        let timeoutId;

        path.addEventListener("mouseover", () => {
            clearTimeout(timeoutId);
            path.setAttribute("stroke", "red");
            button.style.display = "block";
        });

        path.addEventListener("mouseout", () => {
            path.setAttribute("stroke", "#b6b9bb");
            timeoutId = setTimeout(() => {
                button.style.display = "none";
            }, 1000);
        });

        button.onclick = () => {
            button.style.display = "none";
            path.parentNode.removeChild(path);
            button.parentNode.removeChild(button);
            clearTimeout(timeoutId);
            if (item) {
                this.handleDTOContentDelete(item);
            } else {
                this.handleNodeDelete(node);
            }
            // this.drawConnections();
        };
    }

    getTenantUsers() {
        this._userServiceProxy
            .getUsers(null, null, null, false, null, 1000, 0)
            .subscribe((result: any) => {
                this.usersAssign = result.items;
            });
    }



    private handleNodeDelete(node) {
        debugger;
        let childNode = this.nodes.find((x) => x.id == node.childId);
        if (childNode && childNode.parentId.length > 0) {
            childNode.parentId.forEach((id, index) => {
                if (node.id === id) {
                    childNode.parentIndex.splice(index, 1);
                    childNode.parentId.splice(index, 1);
                }
            });
        }
        node.childIndex = null;
        node.childId = null;
    }

    private handleDTOContentDelete(item) {
        debugger;
        const childNode = this.nodes.find((x) => x.id == item.childId);
        if (childNode && childNode.parentId.length > 0) {
            childNode.parentId.forEach((id, index) => {
                if (item.key === id) {
                    childNode.parentIndex.splice(index, 1);
                    childNode.parentId.splice(index, 1);
                }
            });
        }
        item.childIndex = null;
        item.childId = null;
    }

    editDialog(node, index: number) {
        this.nodesWithErrors = [];
        if (!node.isRoot) {
            if (
                !this.isDraggingNode &&
                node.type !== "Delay" &&
                node.type !== "End"
            ) {
                if (node != undefined || !node.isRoot) {
                    this.selectedNode = index;
                    const filteredNodes = this.nodes.filter(
                        (n) =>
                            n.type !== "Bot is triggered if.." &&
                            n.type !== "Jump"
                    );
                    this.createBotDialog.nodes = filteredNodes;
                    this.createBotDialog.show(node);
                    this.createBotDialog.modalClose.subscribe((result: any) => {
                        this.selectedNode = "";
                    });
                    this.createBotDialog.modalSave.subscribe((result: any) => {
                        const indexOfNode = this.nodes.findIndex(
                            (x) => x.id == result.id
                        );
                        if (indexOfNode != -1) {
                            this.selectedNode = "";
                            let node = new GetBotFlowForViewDto();
                            node.id = result.id;
                            node.type = result.type;
                            node.captionAr = result.captionAr;
                            node.captionEn = result.captionEn;
                            node.footerTextAr = result.footerTextAr;
                            node.inputHint = result.inputHint;
                            node.isOneTimeQuestion = result.isOneTimeQuestion;
                            node.isAdvance = result.isAdvance;
                            node.footerTextEn = result.footerTextEn;
                            node.childIndex = result.childIndex;
                            node.parentIndex = result.parentIndex;
                            node.childId = result.childId;
                            node.parentId = result.parentId;
                            node.isRoot = result.isRoot;
                            node.isNodeRoot =
                            this.nodes[indexOfNode].isNodeRoot;
                            node.top = result.top;
                            node.rigth = result.right;
                            node.urlImage = result.urlImage;
                            node.bottom = result.bottom;
                            node.left = result.left;
                            node.urlImageArray = result.urlImageArray;
                            node.dilationTime = result.dilationTime;
                            node.listOfUsers = result.listOfUsers;
                            node.listOfTeams = result.listOfTeams;
                            node.parameterList = result.parameterList;
                            node.actionBlock = result.actionBlock;
                            node.templateId = result.templateId;
                            node.title = result.title;
                            node.parameterType = result.parameterType;
                            node.schedule = result.schedule;
                            node.request = result.request;
                            node.jumpId = result.jumpId;
                            node.conditional = new ConditionalModel({
                                orAnd: result.condition.orAnd,
                                conditionList: result.condition.conditionList,
                            });
                            node.content = new ContentInfo({
                                txt: "",
                                dtoContent: result.content.dtoContent || [],
                            });
                            node.parameter = result.parameter;
                            node.urlImage = result.urlImage;
                            this.nodes[indexOfNode] = node;
                            node.googleSheetIntegration = new GoogleSheetIntegrationModel();
                            node.googleSheetIntegration.integrationType = result.integrationType;
                            node.googleSheetIntegration.googleSheetAction = result.googleSheetAction;
                            node.googleSheetIntegration.spreadSheetId = result.spreadSheetId;
                            node.googleSheetIntegration.spreadSheetName = result.spreadSheetName;
                            node.googleSheetIntegration.workSheet = result.workSheet;
                            node.googleSheetIntegration.lookupColumn = result.lookupColumn;
                            node.googleSheetIntegration.lookupValue = result.lookupValue;
                            node.googleSheetIntegration.parameters = result.parameters;
                            node.googleSheetIntegration.worksheetColumns = result.worksheetColumns;
                            node.googleSheetIntegration.tenantId = this.appSession.tenantId;
                            node.catalogTemplateDto = new CatalogTemplateDto();
                            node.catalogTemplateDto.header = new TemplateHeader();
                            node.catalogTemplateDto.body = new TemplateBody();
                            node.catalogTemplateDto.footer = new TemplateHeader();
                            node.catalogTemplateDto.catalog= new Cacatalog();

                            node.catalogTemplateDto.header.text=result.headerTextEn
                            node.catalogTemplateDto.body.text=result.bodyTextEn
                            node.catalogTemplateDto.footer.text=result.footerTextEn
                            debugger;
                            node.catalogTemplateDto.catalog.catalogId=result.catalogId
                            node.catalogTemplateDto.catalog.catalogName=result.catalogName
                            node.catalogTemplateDto.catalog.sectionTitle=result.sectionTitle
                          const selectedIds = Array.isArray(result.selectedProducts)
                                ? result.selectedProducts
                                : result.selectedProducts
                                    ? [result.selectedProducts]
                                    : [];

                            node.catalogTemplateDto.catalog.products = selectedIds.map(id =>
                                new ProductDto({
                                    productId: id,
                                    name: '',
                                    retailer_Id: id,
                                    description: '',
                                    availability: '',
                                    price: '',
                                    currency: '',
                                    imageUrl: ''
                                })
                            );
                            node.catalogTemplateDto.catalog.products;

                            node.catalogTemplateDto.catalog.products ;
                        }
                        this.drawConnections();
                    });
                }
            }
        }
        this.isDraggingNode = false;
    }

    deleteNode(index, nodeId, event: MouseEvent) {
        debugger;
        event.stopPropagation();

        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            debugger;
            if (isConfirmed) {
                if (this.nodes.length - 1 === index)
                    this.nodeCounter[this.nodes[index].type]--;
                this.nodesWithErrors = [];
                let parentNode;
                let childNode;
                parentNode = this.nodes.find(
                    (x) => x.id == this.nodes[index].parentId[0]
                );
                childNode = this.nodes.find(
                    (x) => x.id == this.nodes[index].childId
                );
                if (parentNode === undefined) {
                    this.nodes.forEach((node, ix) => {
                        if (node.content.hasOwnProperty("dtoContent")) {
                            node.content.dtoContent.forEach((item) => {
                                if (this.nodes[index].parentId.length > 0) {
                                    this.nodes[index].parentId.forEach(
                                        (id, index) => {
                                            if (item.key === id) {
                                                item.childIndex = null;
                                                item.childId = null;
                                            }
                                        }
                                    );
                                }
                            });
                        }
                    });
                } else {
                    this.nodes[index].parentId.forEach((id) => {
                        parentNode = this.nodes.find((x) => x.id == id);
                        parentNode.childIndex = null;
                        parentNode.childId = null;
                    });
                }
                if (childNode) {
                    childNode.parentIndex = [];
                    childNode.parentId = [];
                }
                this.removeLinesForNode(this.nodes[index]);
                this.nodes.splice(index, 1);
                // this.drawConnections();
            }
        });
    }

    back() {
        this.router.navigate(["/app/admin/botFlows"]);
    }

    checkValidteUser(node: any) {
        let flag = true;
        let nodeUsers = node.listOfUsers.split(",");

        for (let index = 0; index < nodeUsers.length; index++) {
            const isFined = this.usersAssign.find(
                (user) => +user.id === +nodeUsers[index]
            );
            if (!isFined) {
                flag = false;
                break;
            }
        }

        return flag;
    }
    diffInDays;
    // checkSchedaulValid(){
    //     (startDate:Date , endDate : Date) {
    //         if (startDate != null && endDate != null ) {

    //                 this.diffInDays = moment(endDate).diff(startDate, 'days');
    //         }
    //     }
    // }

    checkNodeExists(nodeID: number) {
        let node = this.nodes.find((node) => node.id === nodeID);
        if (node) return true;
        else return false;
    }

    saveBot() {
        if (this.hasOnlySpaces(this.bot.flowName)) {
            this.message.error("", this.l("canotHaveOnlySpace"));
            return;
        }
        this.saving = true;
        this.hasError = false;
        this.nodesWithErrors = [];
        let firstNode = this.nodes[0];
        this.nodes.shift();
        if (this.bot.statusId === 4) {
            if (this.nodes[0].type != "whatsApp template") {
                this.hasError = true;
            }
        }
        this.nodes.forEach((node) => {
            if (!node.isNodeRoot === true) node.isNodeRoot = false;

            if (node.parentIndex.length != 0) {
                if (node.childIndex === null) {
                    node.childIndex = -1;
                }
                if (node.jumpId === null || node.jumpId === undefined) {
                    node.jumpId = 0;
                }
                if (
                    node.dilationTime === null ||
                    node.dilationTime === undefined
                ) {
                    node.dilationTime = 0;
                }
                if (node.content.hasOwnProperty("dtoContent")) {
                    if (node.content.dtoContent.length > 0) {
                        node.content.dtoContent.forEach((content) => {
                            if (content.childIndex === null) {
                                content.childIndex = -1;
                            }
                            if (
                                content.valueEn === null ||
                                content.valueEn === undefined ||
                                content.valueEn === ""
                            ) {
                                this.hasError = true;
                                return;
                            }
                        });
                    }
                }
                if (
                    this.bot.statusId === 4 &&
                    node.type === "whatsApp template"
                ) {
                    if (
                        node.templateId === null ||
                        node.templateId === undefined ||
                        node.templateId === ""
                    ) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                    var template = this.Templates.find(
                        (x) => x.id == node.templateId
                    );
                    if (!template) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                }
                if (
                    node.type === "List options" ||
                    node.type === "Reply buttons" ||
                    node.type === "Cataloge Single Product" ||
                    node.type === "Cataloge multity Product" ||
                    node.type === "Branches" ||
                    node.type === "BranchesEN" ||
                    node.type === "ScheduleNode"
                ) {
                    if (
                        node.parameter === "" ||
                        node.parameter === undefined ||
                        node.parameter === null ||
                        node.inputHint === "" ||
                        node.inputHint === undefined ||
                        node.inputHint === null
                    ) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                }
                if (node.type === "Condition") {
                    node.conditional.conditionList.forEach((condition) => {
                        if (
                            condition.op1 === null ||
                            condition.op1 === undefined ||
                            condition.op1 === "" ||
                            condition.op2 === null ||
                            condition.op2 === undefined ||
                            condition.op2 === ""
                        ) {
                            this.hasError = true;
                            this.nodesWithErrors.push(node);
                            return;
                        }
                    });
                }
                if (node.type === "Set Parameter") {
                    node.parameterList.forEach((param) => {
                        if (
                            param.par === null ||
                            param.par === undefined ||
                            param.par === "" ||
                            param.val === null ||
                            param.val === undefined ||
                            param.val === ""
                        ) {
                            this.hasError = true;
                            this.nodesWithErrors.push(node);
                            return;
                        }
                    });
                }
                if (node.type === "ScheduleNode") {
                    if (
                        node.schedule === null ||
                        node.schedule === undefined ||
                        (node.schedule.isData &&
                            !node.schedule.isNow &&
                            !node.schedule.startDate) ||
                        (node.schedule.isData &&
                            node.schedule.isNow &&
                            !node.schedule.numberButton) ||
                        (!node.schedule.isData &&
                            node.schedule.isNow &&
                            !node.schedule.numberButton) ||
                        (!node.schedule.isData &&
                            !node.schedule.isNow &&
                            !node.schedule.startDate) ||
                        node.schedule.numberButton >= 10 ||
                        node.schedule.numberButton <= 0
                    ) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                }
                if (node.type === "Human handover") {
                    if (
                        node.actionBlock === "" ||
                        node.actionBlock === undefined ||
                        node.actionBlock === null ||
                        node.captionEn === null ||
                        node.captionEn === undefined ||
                        node.captionEn === "" ||

                        node.listOfUsers === null ||
                        node.listOfUsers === undefined ||
                        node.listOfUsers === ""
                        // node.listOfTeams === null ||
                        // node.listOfTeams === undefined ||
                        // node.listOfTeams === ""
                    ) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                    let isValid = this.checkValidteUser(node);
                    if (!isValid) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                }
                if (node.type === "Collect input") {
                    if (
                        node.parameter === "" ||
                        node.parameter === undefined ||
                        node.parameter === null
                    ) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                }
                if (node.type === "Jump") {
                    if (
                        node.jumpId === null ||
                        node.jumpId === undefined ||
                        node.jumpId === 0
                    ) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                    if (!this.checkNodeExists(node.jumpId)) {
                        this.hasError = true;
                        this.nodesWithErrors.push(node);
                        return;
                    }
                }
            } else {
                this.hasError = true;
                this.nodesWithErrors.push(node);
                return;
            }
        });
        if (!this.hasError) {
            if (
                this.bot.modifiedDate == null ||
                this.bot.modifiedDate == undefined
            ) {
                this.bot.modifiedDate = moment();
            }
            debugger;
            this.bot.getBotFlowForViewDto = this.nodes;
            this.bot.getBotFlowForViewDto = this.setEmptyStrings(
                this.bot.getBotFlowForViewDto
            ) as any;
            this._BotFlowServiceProxy
                .updateBotFlowsJsonModel(this.id, this.bot)
                .subscribe((result: any) => {
                    this.notify.success(this.l("SavedSuccessfully"));
                    this.nodes.unshift(firstNode);
                    this.hasError = false;
                    this.nodes.forEach((node, index) => {
                        if (node.childIndex === -1) {
                            node.childIndex = null;
                        }

                        if (node.content.hasOwnProperty("dtoContent")) {
                            node.content.dtoContent.forEach((content) => {
                                if (content.childIndex === -1) {
                                    content.childIndex = null;
                                }
                            });
                        }
                    });
                    this.nodesWithErrors = [];
                    this.saving = false;
                });
        } else {
            this.nodes.unshift(firstNode);
            this.nodes.forEach((node, index) => {
                if (node.childIndex === -1) {
                    node.childIndex = null;
                }
                if (node.content.dtoContent != undefined) {
                    node.content.dtoContent.forEach((content) => {
                        if (content.childIndex === -1) {
                            content.childIndex = null;
                        }
                    });
                }
            });
            this.saving = false;
        }
    }

    askForDeploy() {
        let isBotPublished = this.bot.isPublished;
        isBotPublished = !isBotPublished;
        if (this.bot.getBotFlowForViewDto != undefined) {
            //Check if the flow is outbound
            if (this.bot.statusId == 4) {
                if (this.bot.isPublished) {
                    this.message.confirm(
                        "Are You Sure You Want To Deactivate This Flow?",
                        this.l("Important Notice"),
                        (isConfirmed) => {
                            if (isConfirmed) {
                                this.bot.isPublished = isBotPublished;
                                this.deploy();
                            }
                        }
                    );
                } else {
                    if (this.bot.getBotFlowForViewDto.length < 2) {
                        this.notify.error(this.l("PleaseAddNodes"));
                        return;
                    }
                    this.message.confirm(
                        "When the flow is deployed, it CANNOT be deactivated or terminated After 24 HOURS. Please proceed with deployment carefully.",
                        this.l("Important Notice"),
                        (isConfirmed) => {
                            if (isConfirmed) {
                                this.bot.isPublished = isBotPublished;
                                this.deploy();
                            }
                        }
                    );
                }
            } else {
                if (this.bot.isPublished) {
                    this.message.confirm(
                        this.l(
                            "Are You Sure You Want To Deactivate This Flow?"
                        ),
                        this.l("Important Notice"),
                        (isConfirmed) => {
                            if (isConfirmed) {
                                this.bot.isPublished = isBotPublished;
                                this.deploy();
                            }
                        }
                    );
                } else {
                    if (this.bot.getBotFlowForViewDto.length < 2) {
                        this.notify.error(this.l("PleaseAddNodes"));
                        return;
                    }
                    this.message.confirm(
                        "Please Note: when you deploy this flow it will effect on the bot directly, please proceed with deployment carefully!",
                        this.l("Important Notice"),
                        (isConfirmed) => {
                            if (isConfirmed) {
                                this.bot.isPublished = isBotPublished;
                                this.deploy();
                            }
                        }
                    );
                }
            }
        } else {
            this.notify.error(this.l("PleaseAddNodes"));
        }
    }

    deploy() {
        this._BotFlowServiceProxy
            .updateBotFlowsIsPublished(
                this.bot.id,
                this.appSession.userId,
                this.appSession.user.name,
                this.bot.isPublished,
                this.bot.botChannel
            )
            .subscribe((result: any) => {
                if (result > 0) {
                    this.notify.success(this.l("savedSuccessfully"));
                }
            });
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

    private setEmptyStrings(obj) {
        for (const key in obj) {
            if (obj[key] === undefined || obj[key] === null) {
                obj[key] = "";
            }
            if (typeof obj[key] === "object" && !Array.isArray(obj[key])) {
                this.setEmptyStrings(obj[key]);
            }
        }
        return obj;
    }

    styleNode(node) {
        if (node.top == 0 && node.left == 0) {
            return {
                top: "30%",
                left: "50%",
            };
        } else {
            return {
                top: node.top + "%",
                left: node.left + "%",
            };
        }
    }
    styleAvatar(type) {
        if (type == "Send Message") {
            return "#34b8ca";
        }
        // else if (type == "Language") {[]
        //     return '#fc7c29'
        // }
        else if (type == "Reply buttons") {
            return "#359cec";
        } else if (type == "Human handover") {
            return "#f8cc72";
        } else if (type == "Collect input") {
            return "#cb62e4";
        } else if (type == "Jump") {
            return "#fc7c29";
        } else if (type == "List options") {
            return "#09B253";
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
        } else if (type == "whatsApp template") {
            return "#33b349";
        } else if (type == "Set Parameter") {
            return "#fc7c29";
        } else if (type == "Branches") {
            return "#0802a3";
        } else if (type == "BranchesEN") {
            return "#0802a3";
        }else if (type == "ScheduleNode") {
            return "#b8a609";
        } else if (type === "End") {
            return "#ff0000";
        }
        else if (type === 'Integration'){
            return '#9b59b6';
        }else if (type === "Cataloge Single Product") {
            return "#ff9800"; 
        } else if (type === "Cataloge multity Product") {
            return "#4caf50"; 
        }

        return {};
    }

    zoomIn() {
        this.zoomLevel += 10;
        if (this.zoomLevel >= 150) {
            return;
        }
        (
            this.zoomSection.nativeElement.style as any
        ).zoom = `${this.zoomLevel}%`;
        this.drawConnections();
    }

    zoomOut() {
        this.zoomLevel -= 10;
        if (this.zoomLevel <= 50) {
            return;
        }
        (
            this.zoomSection.nativeElement.style as any
        ).zoom = `${this.zoomLevel}%`;
        this.drawConnections();
    }
    zoomReset(): void {
        this.zoomLevel = 100;
        (
            this.zoomSection.nativeElement.style as any
        ).zoom = `${this.zoomLevel}%`;
        this.drawConnections();
    }
    selectDuration(duration, node) {
        node.dilationTime = duration;
    }
    copyDialog(node: GetBotFlowForViewDto, event: MouseEvent) {
        if (!this.nodeCounter[node.type]) {
            this.nodeCounter[node.type] = 1;
        } else {
            this.nodeCounter[node.type]++;
        }
        event.stopPropagation();
        let newNode = new GetBotFlowForViewDto();
        newNode.captionAr = node.captionAr;
        newNode.captionEn = node.captionEn;
        newNode.content = new ContentInfo();
        newNode.content.txt = node.content.txt;
        newNode.title = `${node.type} ${this.nodeCounter[node.type]}`;
        newNode.content.dtoContent = [];
        if (node.content.hasOwnProperty("dtoContent")) {
            if (node.content.dtoContent.length > 0) {
                node.content.dtoContent.forEach((item) => {
                    let button = new Dtocontent();
                    button.key = this.stringToNumber(uniqid("IB-"));
                    button.childId = null;
                    button.childIndex = null;
                    button.parentId = [];
                    button.parentIndex = [];
                    button.valueEn = item.valueEn;
                    button.valueAr = item.valueAr;
                    newNode.content.dtoContent.push(button);
                });
            }
        }
        newNode.conditional = new ConditionalModel();
        newNode.conditional.orAnd = node.conditional.orAnd;
        newNode.conditional.conditionList = [];
        if (node.conditional.conditionList.length > 0) {
            node.conditional.conditionList.forEach((item) => {
                let condition = new ConditionList();
                condition.op1 = item.op1;
                condition.op2 = item.op2;
                condition.operation = item.operation;
                newNode.conditional.conditionList.push(condition);
            });
        }
        if (node.type === "Set Parameter") {
            newNode.parameterList = [new ParameterSet()];
        }

        newNode.bottom = node.bottom;
        newNode.top = node.top;
        newNode.rigth = node.rigth;
        newNode.footerTextEn = node.footerTextEn;
        newNode.footerTextAr = node.footerTextAr;
        newNode.inputHint = node.inputHint;
        newNode.isOneTimeQuestion = node.isOneTimeQuestion;
        newNode.isAdvance = node.isAdvance;
        newNode.dilationTime = node.dilationTime;
        newNode.listOfUsers = node.listOfUsers;
        newNode.listOfTeams = node.listOfTeams;
        newNode.actionBlock = node.actionBlock;
        newNode.templateId = node.templateId;
        newNode.parameterType = "";
        newNode.request = node.request;
        newNode.jumpId = node.jumpId;
        newNode.parameter = "";
        newNode.urlImage = node.urlImage;
        newNode.urlImageArray = node.urlImageArray;
        newNode.type = node.type;
        newNode.isRoot = node.isRoot;
        newNode.id = this.stringToNumber(uniqid("IB-"));
        newNode.childId = null;
        newNode.childIndex = null;
        newNode.parentId = [];
        newNode.parentIndex = [];
        newNode.left = node.left + 250;
        if (newNode.content.hasOwnProperty("dtoContent")) {
            if (newNode.content.dtoContent.length > 0) {
                newNode.content.dtoContent.forEach((item) => {
                    item.key = this.stringToNumber(uniqid("IB-"));
                    item.childId = null;
                    item.childIndex = null;
                    item.parentId = [];
                    item.parentIndex = [];
                });
            }
        }
        this.nodes.push(newNode);
        // this.drawConnections();
    }
    ngAfterViewInit(): void {
        // this.initMinimap();
    }

    initMinimap(): void {
        let $viewport = $('<div class="viewport"></div>');
        $("#minimap").append($viewport);
        let $contentClone = $("body").clone();
        $contentClone.find("#minimap").remove();
        $("#minimap").append($contentClone);
        $("#minimap").css("transform", "scale(0.2)");

        function updateViewport() {
            let scrollTop = $(window).scrollTop();
            let scrollLeft = $(window).scrollLeft();
            $viewport.css({
                top: scrollTop * 0.2,
                left: scrollLeft * 0.2,
                width: $(window).width() * 0.2,
                height: $(window).height() * 0.2,
            });
        }

        $(window).on("scroll resize", updateViewport);
        updateViewport();

        $viewport.draggable({
            drag: function (event, ui) {
                $(window).scrollTop(ui.position.top / 0.2);
                $(window).scrollLeft(ui.position.left / 0.2);
            },
        });
    }
    isNodeInError(node): boolean {
        return this.nodesWithErrors.some(
            (errorNode) => errorNode.id === node.id
        );
    }
    trackByFunction(index, node) {
        return node.id;
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
}
