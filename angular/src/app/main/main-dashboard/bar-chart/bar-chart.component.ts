import {
    Component,
    Injector,
    Input,
    OnChanges,
    OnInit,
    SimpleChanges,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";

@Component({
    selector: "app-bar-chart",
    templateUrl: "./bar-chart.component.html",
    styleUrls: ["./bar-chart.component.css"],
})
export class BarChartComponent
    extends AppComponentBase
    implements OnInit, OnChanges
{
    @Input() DataFromParent: number[];
    locWData = [
        { name: "Pending", value: 0 },
        { name: "Opened", value: 0 },
        { name: "Closed", value: 0 },
    ];

    verticalBarOptions = {
        showXAxis: true,
        showYAxis: true,
        gradient: false,
        showLegend: false,
        showGridLines: true,
        barPadding: 100,
        showXAxisLabel: true,
        xAxisLabel: "Tickets",
        showYAxisLabel: true,
        yAxisLabel: "Number",
    };

    isBrowser: boolean;

    colorScheme = {
        domain: ["#EA5455", "#00CFE8", "#28C76E"],
    };

    constructor(injector: Injector) {
        super(injector);
    }
    ngOnChanges(changes: SimpleChanges): void {
        if (changes["DataFromParent"] && this.DataFromParent) {
            var tempData = [
                { name: "Pending", value: this.DataFromParent[0] },
                { name: "Opened", value: this.DataFromParent[1] },
                { name: "Closed", value: this.DataFromParent[2] },
            ];
            this.locWData = [...tempData];
        }
    }

    ngOnInit(): void {}
}
