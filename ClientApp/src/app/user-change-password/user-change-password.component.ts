import { Component, OnInit } from '@angular/core';
import { PasswordRequest } from 'src/models/requests/PasswordRequest';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormErrorStateMatcher } from '../shared/form-error-state-matcher';
import { HttpBaseComponent } from '../shared/ui-base-http.component';
import { MessageSessionResponse } from 'src/models/responses/MessageSessionResponse';
import { Md5 } from 'ts-md5';
import { Store } from "projects/store";
import { RegexStrings } from "src/common/constents/RegexStrings";

@Component({
    selector: 'app-page-user-change-password',
    templateUrl: './user-change-password.component.html'
})
export class UserChangePasswordComponent extends HttpBaseComponent implements OnInit {
    instanz:UserChangePasswordComponent=this;
    waiting:boolean=false;
    
    oldPswFormControl!: FormControl;
    newPswFormControl!: FormControl;
    newPswConfirmFormControl!: FormControl;
    matcher: FormErrorStateMatcher = new FormErrorStateMatcher();
    ngOnInit() {
        this.oldPswFormControl = new FormControl("", [Validators.required]);
        this.newPswFormControl = new FormControl("", [Validators.required,
        Validators.minLength(8),
        Validators.pattern(new RegExp(RegexStrings.PasswordDigit)),
        Validators.pattern(new RegExp(RegexStrings.PasswordCharLower)),
        Validators.pattern(new RegExp(RegexStrings.PasswordCharUpper)),
        Validators.pattern(new RegExp(RegexStrings.PasswordSymbol))]);
        this.newPswConfirmFormControl = new FormControl("", [Validators.required]);
    }
    onFocus(){
        this.newPswConfirmFormControl.clearValidators();
        this.newPswConfirmFormControl.addValidators([Validators.required,
            Validators.pattern(`^${this.newPswFormControl.value}$`)]);
    }
    changPassword() {
        if(this.invalid())
            return;
        this.waiting=true;
        let oldPsw = Md5.hashStr(this.oldPswFormControl.value);
        let newPsw = Md5.hashStr(this.newPswFormControl.value);
        let request: PasswordRequest = { oldPassword: oldPsw, newPassword: newPsw };
        this._http.put<MessageSessionResponse>(this.createUrl('Users/changepassword'), request).subscribe({
            next: data => {
                this.waiting=false;
                Store.action("user", "updateSession")(data);
                this._router.navigate([".."]);
            },
            error: error => {
                this.waiting=false;
                this.showError(error);
            }
        });
    }
    
    invalid(): boolean {
        return this.oldPswFormControl.invalid || this.newPswFormControl.invalid || this.newPswConfirmFormControl.invalid;
    }
    getErrorMessage(control: string): string {
        if (control == "oldPsw") {
            if (this.oldPswFormControl.hasError("required")) {
                return this.t('messages.required', { value: this.t('ui.password') });
            }
            if (this.oldPswFormControl.hasError("minLength"))
                return this.t('messages.minLength');
        }
        if (control == "newPsw") {
            if (this.newPswFormControl.hasError("required")) {
                return this.t('messages.required', { value: this.t('ui.email') });
            }
            if (this.newPswFormControl.hasError("minLength"))
                return this.t('messages.minLength');
            if (this.newPswFormControl.hasError("pattern"))
                return this.t('messages.passwordFormat');
        }
        if (control == "newPswConfirm") {
            if (this.newPswConfirmFormControl.hasError("required")) {
                return this.t('messages.required', { value: this.t('ui.email') });
            }
            if (this.newPswConfirmFormControl.hasError("pattern"))
                return this.t('messages.passwordConfirmNoSame');
        }
        return "";
    }
}
