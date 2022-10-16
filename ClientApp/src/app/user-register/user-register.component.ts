import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormErrorStateMatcher } from '../shared/form-error-state-matcher';
import { HttpBaseComponent } from '../shared/ui-base-http.component';
import { MessageResponse } from 'src/models/responses/MessageResponse';
import { Md5 } from 'ts-md5';
import { RegexStrings } from "src/common/constents/RegexStrings";
import { UserRegisterRequest } from 'src/models/requests/UserRegisterRequest';

@Component({
    selector: 'app-page-user-register',
    templateUrl: './user-register.component.html'
})
export class UserRegisterComponent extends HttpBaseComponent {
    instanz: UserRegisterComponent = this;
    userNameControl!: FormControl;
    emailControl!: FormControl;
    pswControl!: FormControl;
    passwordConfirmControl!: FormControl;
    matcher: FormErrorStateMatcher = new FormErrorStateMatcher();
    ngOnInit() {
        this.userNameControl = new FormControl("", [Validators.required,
        Validators.minLength(2)]);
        this.emailControl = new FormControl("", [Validators.required,
        Validators.email]);
        this.pswControl = new FormControl("", [Validators.required,
        Validators.minLength(8),
        Validators.pattern(new RegExp(RegexStrings.PasswordDigit)),
        Validators.pattern(new RegExp(RegexStrings.PasswordCharLower)),
        Validators.pattern(new RegExp(RegexStrings.PasswordCharUpper)),
        Validators.pattern(new RegExp(RegexStrings.PasswordSymbol))]);
        this.passwordConfirmControl = new FormControl("", [Validators.required]);
        
    }
    onFocus(){
        this.passwordConfirmControl.clearValidators();
        this.passwordConfirmControl.addValidators([Validators.required,
            Validators.pattern(`^${this.pswControl.value}$`)]);
    }
    register() {
        if(this.invalid())
            return;       
        let password = Md5.hashStr(this.pswControl.value);
        let request: UserRegisterRequest = { userName: this.userNameControl.value, email: this.emailControl.value, password: password };
        this.post<MessageResponse>('Users/register', request, (data) => {           
            this.back();
          });          
    }
    
    invalid(): boolean {
        return this.userNameControl.invalid || this.emailControl.invalid || this.pswControl.invalid || this.passwordConfirmControl.invalid;
    }
    getErrorMessage(control: string): string {
        if (control == "userName") {
            if (this.userNameControl.hasError("required")) {
                return this.t('messages.required', { value: this.t('ui.userName') });
            }
            if (this.userNameControl.hasError("minlength"))
                return this.t('messages.minLength', { value: this.t('ui.userName') });
        }
        if (control == "email") {
            if (this.emailControl.hasError("required")) {
                return this.t('messages.required', { value: this.t('ui.email') });
            }
            if (this.emailControl.hasError("email"))
                return this.t('messages.emailInvalid');

        }
        if (control == "password") {
            if (this.pswControl.hasError("required")) {
                return this.t('messages.required', { value: this.t('ui.password') });
            }
            if (this.pswControl.hasError("minlength")) {
                return this.t('messages.minLength', { value: this.t('ui.password') });
            }
            if (this.pswControl.hasError("pattern"))
                return this.t('messages.passwordFormat');
        }
        if (control == "passwordConfirm") {
            if (this.passwordConfirmControl.hasError("required")) {
                return this.t('messages.required', { value: this.t('ui.passwordConfirm') });
            }
            if (this.passwordConfirmControl.hasError("pattern"))
                return this.t('messages.passwordConfirm');
        }
        return "";
    }
}
