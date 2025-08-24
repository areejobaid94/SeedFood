import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "numberAbbreviation",
})
export class NumberAbbreviationPipe implements PipeTransform {
    transform(value: number = 0): string {
        
        if (value >= 1e9) {
            return (value / 1e9).toFixed(2).replace(/\.00/, '') + 'B'; // Billions
        } else if (value >= 1e6) {
            return (value / 1e6).toFixed(2).replace(/\.00/, '') + 'M'; // Millions
        } else if (value >= 1e3) {
            return (value / 1e3).toFixed(2).replace(/\.00/, '') + 'k'; // Thousands
        } else {
            return value.toString(); // Less than 1k
        }
    }
}
