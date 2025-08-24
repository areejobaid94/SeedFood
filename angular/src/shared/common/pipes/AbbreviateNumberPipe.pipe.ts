import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "abbreviateNumber",
})
export class AbbreviateNumberPipe implements PipeTransform {
    transform(value: number): string {
        if (value === null || value === undefined) {
            return "";
        }
        if (value < 1000) {
            return value.toString();
        } else if (value < 1000000) {
            return Math.floor(value / 1000) + "K";
        } else if (value < 1000000000) {
            return Math.floor(value / 1000000) + "M";
        } else {
            return Math.floor(value / 1000000000) + "B";
        }
    }
}
