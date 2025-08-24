import {
    AfterViewInit,
    Component,
    EventEmitter,
    Injector,
    OnInit,
    Output,
    ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ModalDirective } from "ngx-bootstrap/modal";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import {
    ActionsModel,
    BotFlowServiceProxy,
    BotReservedWordsModel,
    GetBotFlowForViewDto,
    GetBotModelFlowForViewDto,
    KeyWordModel,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { DarkModeService } from "@app/services/dark-mode.service";


@Component({
    selector: "createReservedWordsModal",
    templateUrl: "./create-reserved-words.component.html",
    styleUrls: ["./create-reserved-words.component.css"],
})
export class CreateReservedWordsComponent extends AppComponentBase  {
    @ViewChild("createReservedWordsModal", { static: true })
    modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    theme: string;
    public keyWord = [];
    bots : GetBotModelFlowForViewDto[];
    nodes : GetBotFlowForViewDto[];
    isEdit = false;
    selectedBot : GetBotModelFlowForViewDto;
    selectedBotId : number = null;
    selectedNode : GetBotFlowForViewDto = null;
    selectedNodeId : number = null;
    keywordModel : KeyWordModel;
    selectedValueNode : string = "Select Node";
    text = "";
    submitted = false;
    submitted2 = false;
    isUpdate : boolean = false;


    saving = false;
    reservedWords: BotReservedWordsModel = new BotReservedWordsModel();
    botActions : ActionsModel[];
    
        isFuzzyMatch: boolean = false; 
    percentage: number = 2 ;


    KeyWordType:string="3";

    updatePercentage(value: string) {
        this.percentage = +value; 

    }

    getPercentageValue(): number {
        switch (this.percentage) {
            case 0:
                return 20;
            case 1:
                return 50;
            case 2:
                return 80;
            default:
                return 80;
        }
    }
    onSelectMatchType(matchType: string) {
       this.KeyWordType=matchType;
        if (matchType=="1") {
            this.isFuzzyMatch=true;
        }else{
            this.isFuzzyMatch=false;
        }
    }
    constructor(
        injector: Injector,
        private _whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy,
        private modalService: NgbModal,
        public darkModeService: DarkModeService,
        private _BotFlowServiceProxy: BotFlowServiceProxy,
    ) {
        super(injector);
        
    }

    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
        this.keywordModel = new KeyWordModel;
        this.getActions();
        this.getFlows()
    }


    selectBot(event :any ){
        let id = event.target.value;
        this.selectedBot = this.bots?.find(item => item.id == id);
        if(this.selectedBot){
            this.selectedBotId = this.selectedBot.id;
            this.nodes = this.selectedBot.getBotFlowForViewDto.filter(item => {
                return item.type !== 'Condition' && item.type !== 'Delay' && item.type !== 'Jump';
            });
        }
        this.keywordModel.action =  this.selectedBot.flowName;
        this.keywordModel.actionId =   this.selectedBot.id;
     
    }
    selectNode(event :any ){
        let id = event.target.value;
        this.selectedNode = this.selectedBot.getBotFlowForViewDto.find(item => item.id == id);
        this.selectedNodeId = this.selectedNode.id;
        this.keywordModel.triggersBot = this.selectedNode.captionEn;
        this.keywordModel.triggersBotId = this.selectedNode.id;
    }

    getFlows() {
        this._BotFlowServiceProxy
            .getAllBotFlows(
                this.appSession.tenantId,
               10000
                ,1
            )
            .subscribe((result) => {
                this.bots = result.items
            });
    }

    modalOpen(modalBasic) {
        this.text = "";
        this.modalService.open(modalBasic, {
            windowClass: "modal",
        });
    }
    show(id?) {
        this.keyWord = [];
        this.reservedWords = new BotReservedWordsModel();
        this.nodes =[];
        this.selectedBotId  = null;
        this.selectedBot  =new GetBotModelFlowForViewDto();
        // this.selectedNode = new GetBotFlowForViewDto();
        this.selectedNode = null;
        this.isEdit=false;
        if(id){
            this.isUpdate = true;
            this._whatsAppMessageTemplateServiceProxy
            .keyWordGetById(id)
            .subscribe(
                (result) => {
                   this.keywordModel = result;
                   this.selectedBot = this.bots?.find(item => item.id == result.actionId);
                   this.KeyWordType=result.keyWordType.toString();
                   this.percentage=result.fuzzyMatch;

                   if (this.percentage==1) {
                    this.isFuzzyMatch=true;
                   }else{
                    this.isFuzzyMatch=false;
                    }
                

                   if(this.selectedBot){
                    this.selectedBotId = this.selectedBot.id;
                    this.nodes = this.selectedBot.getBotFlowForViewDto.filter(item => {
                        return item.type !== 'Condition' && item.type !== 'Delay' && item.type !== 'Jump';
                    });
                    this.selectedNode = this.selectedBot.getBotFlowForViewDto.find(item => item.id == result.triggersBotId);
                    this.selectedNodeId =   this.selectedNode.id;
                }
                   this.keyWord = this.keywordModel.buttonText.split(',');
                   this.isEdit = true;
                   this.modal.show();
                });
        }else{
            this.isUpdate = false;
            this.modal.show();
            if(this.selectedBot){
                this.nodes = this.selectedBot.getBotFlowForViewDto.filter(item => {
                    return item.type !== 'Condition' && item.type !== 'Delay' && item.type !== 'Jump';
                });
            }
        }
     
      
      
    }
    saveKeyWord(item) {
        this.submitted2 = false;
        if(item === "" || item === undefined || item === null){
            this.submitted2 = true;
            return;
        }
        if(this.keyWord.includes(item)){
            this.notify.warn( this.l('alreadyExist'));
            return;
        }else{
             this.keyWord.push(item);
             this.modalService.dismissAll();
        }
      
    }
    close(): void {
        this.selectedBot  =new GetBotModelFlowForViewDto();
        this.selectedBotId  = null;
        this.selectedNodeId = null;
        this.selectedNode = new GetBotFlowForViewDto();
        this.modal.hide();
        this.modalSave.emit(null);
        this.isEdit=false;
    }
    addItem() {
        this.keyWord.push({
            text: "",
        });
    }

    /**
     * DeleteItem
     *
     * @param id
     */
    deleteItem(id) {
        for (let i = 0; i < this.keyWord.length; i++) {
            if (this.keyWord.indexOf(this.keyWord[i]) === id) {
                this.keyWord.splice(i, 1);
                break;
            }
        }
    }

    save(): void {
        // this.saving = true;
        this.submitted = true;
        if (!this.selectedBot) {
            this.notify.error("Please Select Bot");
            this.saving = false;
            return;
        }
     
        if (!this.selectedNode) {
            this.notify.error("Please Select Node");
            this.saving = false;
            return;
        }
        if (this.keyWord.length === 0) {
            this.notify.error("Please Add Keyword");
            this.saving = false;
            return;
        }

        
        this.keywordModel.buttonText = this.keyWord.join(",");
        this.keywordModel.tenantId = this.appSession.tenantId;


        this.keywordModel.keyWordType =parseInt(this.KeyWordType);
        this.keywordModel.fuzzyMatch = this.percentage


        this.saving = true;
        if(this.isEdit){
            this.saving = false;
            this._whatsAppMessageTemplateServiceProxy
            .keyWordUpdate(this.keywordModel)
            .subscribe(
                (result) => {
                    switch (result.state) {
                        case 2:
                            this.notify.success(this.l("savedSuccessfully"));
                            this.submitted = false;
                            this.saving = false;
                            this.close();
                            break;
                        case 1:
                            this.message.error('(' + result.message + ')' + ' ' +this.l( "alreadyExist"));
                            this.submitted = false;
                            this.saving = false;
                            break; 
                        case 3:
                            this.message.error('(' + result.message + ')' + ' ' +this.l( "buttonTextError"));
                            this.submitted = false;
                            this.saving = false;
                            break; 
                            
                        case 4:
                            this.message.error(this.l( "actionIdDoesnotExits"));
                            this.submitted = false;
                            this.saving = false;
                            break;  

                       
                        case -1:
                            this.message.error(result.message);
                            this.submitted = false;
                            this.saving = false;
                            break;  
                                
                    
                        default:
                            this.message.error(this.l( "errorFromServerKeyword"));
                            this.submitted = false;
                            this.saving = false;
                            break;
                    }
                 },
                (error: any) => {
                    if (error) {
                        this.saving = false;
                        this.submitted = false;
                    }
                }
            );
       
        }else{
            this.saving = false;
            this._whatsAppMessageTemplateServiceProxy
            .keyWordAdd(this.keywordModel)
            .subscribe(
                (result) => {
                    switch (result.state) {
                        case 2:
                            this.notify.success(this.l("savedSuccessfully"));
                            this.submitted = false;
                            this.saving = false;
                            this.close();
                            break;
                        case 1:
                            this.message.error('(' + result.message + ')' + ' ' +this.l( "alreadyExist"));
                            this.submitted = false;
                            this.saving = false;
                            break; 
                        case 3:
                            this.message.error('(' + result.message + ')' + ' ' +this.l( "buttonTextError"));
                            this.submitted = false;
                            this.saving = false;
                            break; 
                            
                        case 4:
                            this.message.error(this.l( "actionIdDoesnotExits"));
                            this.submitted = false;
                            this.saving = false;
                            break;                         
                        case -1:
                            this.message.error(result.message);
                            this.submitted = false;
                            this.saving = false;
                            break;                                  
                        default:
                            this.message.error(this.l( "errorFromServerKeyword"));
                            this.submitted = false;
                            this.saving = false;
                            break;
                    }
                 }
                ,
                (error: any) => {
                    if (error) {
                        this.saving = false;
                        this.submitted = false;
                    }
                }
            ); 
        }
      
    
    }

    getActions(){
        this._whatsAppMessageTemplateServiceProxy
        .getAllActions()
        .subscribe(
            (result) => {
               this.botActions = result;
            });
    }
    
}
