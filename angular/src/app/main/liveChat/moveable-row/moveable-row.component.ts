import { Component, Injector, Input, OnInit } from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppComponentBase } from "@shared/common/app-component-base";

interface TimeDuration {
    days?: number;
    hours?: number;
    minutes: number;
}

@Component({
    selector: "app-moveable-row",
    templateUrl: "./moveable-row.component.html",
    styleUrls: ["./moveable-row.component.css"],
})
export class MoveableRowComponent extends AppComponentBase implements OnInit {
    @Input() name: string="";
    @Input() record;
    // @Input() l: (arg) => string;

    constructor(injector: Injector,public darkModeService: DarkModeService) {
        super(injector);
    }

    ngOnInit(): void {

     
        this.record;
    }
    
    convertMinutesToTimeDuration(minutes: number): string {
        if (minutes < 0) {
            throw new Error("Input must be a non-negative number of minutes.");
        }

        const hoursInDay = 24;
        const minutesInHour = 60;

        const days = Math.floor(minutes / (hoursInDay * minutesInHour));
        const remainingHours = Math.floor(
            (minutes % (hoursInDay * minutesInHour)) / minutesInHour
        );
        const remainingMinutes = minutes % minutesInHour;
        let resolution = "";

        const result: TimeDuration = {
            minutes: remainingMinutes,
        };

        if (days > 0) {
            result.days = days;
            resolution += days + "D:";
        }

        if (remainingHours > 0) {
            result.hours = remainingHours;
            resolution += remainingHours + "H:";
        }
        resolution += remainingMinutes + "min";

        return resolution;
    }
}
