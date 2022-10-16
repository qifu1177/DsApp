import { Directive, Input, HostListener, ElementRef } from "@angular/core";

@Directive({
    selector: '[enter]'
  })

export class EnterDirective {
    @Input('enter') enter:any={};
    @Input('action') action:string='';
    @HostListener("keyup", ['$event']) onKeyUp(event: KeyboardEvent) {
        if (event.key == "Enter" && this.action!='') {
            this.enter[this.action]();
        }
    }
}