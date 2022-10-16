import { Input, Component } from "@angular/core";
import { FormControl } from "@angular/forms";
import { ErrorStateMatcher } from '@angular/material/core';

@Component({
    selector: 'validation-input',
    templateUrl: './validation-input.component.html'
})

export class ValitionInput {
    @Input('formInstanz') formInstanz: any = {};
    @Input('translateLabel') translateLabel: string = '';
    @Input('placeholder') placeholder: string = '';
    @Input('inputType') inputType: string = "text";
    @Input('control') formControl: FormControl=new FormControl('');
    @Input('matcher') matcher!: ErrorStateMatcher;
    @Input('errorMessageCall') errorMessageCall: string = '';
    @Input('erterAction') enterAction: string = '';
    @Input('inputKey') inputKey: string = '';
    @Input('focus') focus:string='';
    getErrorMessage() {
        if (this.errorMessageCall != '' && this.inputKey != '')
            return this.formInstanz[this.errorMessageCall](this.inputKey);
        return '';
    }
    focusAction(event:any) { 
        if(this.focus!='')
            this.formInstanz[this.focus](event);
    }
}