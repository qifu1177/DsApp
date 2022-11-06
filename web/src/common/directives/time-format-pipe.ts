import { Pipe, PipeTransform } from "@angular/core";
@Pipe({ name: 'toTimeFormat' })
export class ToTimeFormatPipe implements PipeTransform {
    transform(value: number, unit: string = 'h') {
        if (unit == 's')
            return Math.round(value / 1000);
        else if (unit == 'm') {
            let seconds = value / 1000;
            let minunts = Math.floor(seconds / 60);
            seconds = Math.round(seconds - minunts * 60);
            return `${minunts}:${seconds}`;
        }
        else {
            let seconds = value / 1000;
            let hours = Math.floor(seconds / 3600);
            seconds = seconds - hours * 3600;
            let minunts = Math.floor(seconds / 60);
            seconds = Math.round(seconds - minunts * 60);
            return `${hours}:${minunts}:${seconds}`;
        }
    }
}