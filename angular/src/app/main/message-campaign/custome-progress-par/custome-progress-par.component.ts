import {
    Component,
    EventEmitter,
    Injector,
    Input,
    OnInit,
    Output,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { CampaignStatisticsDto } from "@shared/service-proxies/service-proxies";

@Component({
    selector: "app-custome-progress-par",
    templateUrl: "./custome-progress-par.component.html",
    styleUrls: ["./custome-progress-par.component.css"],
})
export class CustomeProgressParComponent
    extends AppComponentBase
    implements OnInit
{
    @Output()
    closeModel: EventEmitter<void> = new EventEmitter<void>();

    @Input()
    details!: CampaignStatisticsDto;

    progressValue = 89;

    dynamicColorClass = "bg-success";

    close() {
        console.log(this.details);
        this.closeModel.emit();
    }
    constructor(injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        console.log(this.details);
    }

    floorIt(value) {
        return Math.floor(value);
    }
}
