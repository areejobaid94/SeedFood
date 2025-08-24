import {
  Component,
  EventEmitter,
  Injector,
  OnInit,
  Output,
  ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ModalDirective } from "ngx-bootstrap/modal";
import { DarkModeService } from "@app/services/dark-mode.service";
import { BotFlowServiceProxy, GetBotModelFlowForViewDto } from "@shared/service-proxies/service-proxies";

@Component({
  selector: "createBotFlow",
  templateUrl: "./create-flow.component.html",
  styleUrls: ["./create-flow.component.css"],
})
export class CreateFlowComponent extends AppComponentBase {
  @ViewChild("createBotFlow", { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  sumbitted = false;
  botFlow = new GetBotModelFlowForViewDto();
  submitted = false;
  saving = false;

  constructor(
    injector: Injector,
    public darkModeService: DarkModeService,
    private _BotFlowServiceProxy: BotFlowServiceProxy,
 ) {
    super(injector);

  }

  ngOnInit(): void { }
  show() {
    this.botFlow = new GetBotModelFlowForViewDto();
    this.modal.show();
  }

  close() {
    this.modal.hide();
    this.modalSave.emit(null);
  }

  onlySpaces(string) {
    return /^\s*$/.test(string);
}

  save(){
    if (this.onlySpaces(this.botFlow.flowName)) {

      this.message.error("", this.l("canotHaveOnlySpace"));
      return;
    }
    this.saving=true;
    this.botFlow.tenantId = this.appSession.tenant.id;
    this.botFlow.createdUserId = this.appSession.userId;
    this.botFlow.createdUserName =this.appSession.user.userName;
    if(this.botFlow.flowName === null || this.botFlow.flowName === undefined || this.botFlow.flowName === "" ||
        this.botFlow.statusId === null || this.botFlow.statusId === undefined ){
        this.submitted = true;
        this.saving=false;
      return
    }
    this._BotFlowServiceProxy
    .createBotFlows(this.botFlow)
    .subscribe(
        (res) => {
            if (res > 0) {
                this.notify.success(this.l("SavedSuccessfully"));
                this.close();
                this.submitted = false;
                this.saving=false;
            }else{
              this.notify.error(this.l("Failed"));
              this.close();
              this.submitted = false;      
              this.saving=false;
            }
            
        },
        (error: any) => {
            if (error) {
              this.submitted = false;
              this.saving = false;
            }
        }
      );
    }

}
