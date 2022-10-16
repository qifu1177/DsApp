import { Component, OnInit } from '@angular/core';
import { UserLoginRequest } from 'src/models/requests/UserLoginRequest';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormErrorStateMatcher } from '../shared/form-error-state-matcher';
import { HttpBaseComponent } from '../shared/ui-base-http.component';
import { UserLoginResponse } from 'src/models/responses/UserLoginResponse';
import { Store } from "projects/store";
import { Md5 } from 'ts-md5';

@Component({
  selector: 'app-page-user-login',
  templateUrl: './user-login.component.html'
})

export class UserLoginComponent extends HttpBaseComponent implements OnInit {
  request!: UserLoginRequest;
  matcher: FormErrorStateMatcher = new FormErrorStateMatcher();
  instanz:UserLoginComponent=this;
  //response!: UserLoginResponse;
  loginNameFormControl!: FormControl;
  passwordFormControl!: FormControl;
 
 
  ngOnInit() {
    this.request = { loginName: "", password: "" };
    this.loginNameFormControl = new FormControl(this.request.loginName, [Validators.required, Validators.email]);
    this.passwordFormControl = new FormControl(this.request.password, [Validators.required]);
  }
  login() {
    if(this.invalid())
      return;
   
    let password = Md5.hashStr(this.passwordFormControl.value);
    let loginRequset: UserLoginRequest = { loginName: this.loginNameFormControl.value, password: `${password}` };
    this.post<UserLoginResponse>('Users/login', loginRequset, (data) => {
      Store.action("user", "login")(data);
      this.back();
    });    
  }
  register() {
    this._router.navigate(["user-register"]);
  }
  forgetPassword(){
    this._router.navigate(['forget-password']);
  }
  invalid(): boolean {
    return this.loginNameFormControl.invalid || this.passwordFormControl.invalid;
  }
  
  getErrorMessage(control: string): string {
    if (control == "loginName") {
      if (this.loginNameFormControl.hasError("required")) {
        return this.t('messages.required', { value: this.t('ui.email') });
      }
      if (this.loginNameFormControl.hasError("email"))
        return this.t('messages.emailInvalid');
    }
    if (control == "password") {
      if (this.passwordFormControl.hasError("required")) {
        return this.t('messages.required', { value: this.t('ui.password') });
      }
      
    }
    return "";
  }
}
