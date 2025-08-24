import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
    Input,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";
import {
    GetItemAdditionDetailsModel,
    MenuDto,
    MenusServiceProxy,
    RType,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "@app/services/dark-mode.service";

@Component({
    selector: "viewItemSpecificationsModal",
    templateUrl: "./view-ItemSpecifications-modal.component.html",
})
export class ViewItemSpecificationsModalComponent extends AppComponentBase {
    theme: string;
    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @Input() rtypeFromPerant: RType[]
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Output() returnRtype: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    rType: RType[];
    item: GetItemAdditionDetailsModel[];

    constructor(
        injector: Injector,
        private _menusServiceProxy: MenusServiceProxy,
        public darkModeService: DarkModeService
    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.theme = ThemeHelper.getTheme();
    }

    show(item: MenuDto): void {
        if(!this.rtypeFromPerant || this.rtypeFromPerant?.length <= 0){
            this._menusServiceProxy
            .getRType(this.appSession.tenantId)
            .subscribe((result) => {
                this.rType = result;
                this.returnRtype.emit(this.rType);
            });
        }
        else{
            this.rType = this.rtypeFromPerant;
        }
        this._menusServiceProxy
            .getItemSpecificationsListForView(item.restaurantsType)
            .subscribe((res) => {
                this.item = res;

                this.active = true;
                this.modal.show();
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    onchangeBranch(event: any): void {
        this._menusServiceProxy
            .getItemSpecificationsListForView(event.target.value)
            .subscribe((res) => {
                this.item = res;
            });
    }

    save(): void {
        this._menusServiceProxy
            .updateItemSpecificationsListForView(this.item)
            .subscribe((res) => {
                this.active = false;
                this.modal.hide();
            });
    }
}
