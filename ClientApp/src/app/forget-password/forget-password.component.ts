import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { FormErrorStateMatcher } from '../shared/form-error-state-matcher';
import { HttpBaseComponent } from 'src/app/shared/ui-base-http.component';
import { MessageResponse } from 'src/models/responses/MessageResponse';

@Component({
  selector: 'app-page-forget-password',
  templateUrl: './forget-password.component.html'
})
export class ForgetPasswordComponent extends HttpBaseComponent implements OnInit {
  instanz: ForgetPasswordComponent = this;
  emailControl!: FormControl;
  matcher: FormErrorStateMatcher = new FormErrorStateMatcher();
  ngOnInit() {
    this.emailControl = new FormControl('', [Validators.required, Validators.email]);
  }
  forgetPassword() {
    if (this.invalid())
      return;
    this.put<MessageResponse>(`Users/resetpassword/${this.emailControl.value}`, {}, (data) => {
      this.back();
    });
  }
  invalid(): boolean {
    return this.emailControl.invalid;
  }

  getErrorMessage(control: string): string {
    if (control == "email") {
      if (this.emailControl.hasError("required")) {
        return this.t('messages.required', { value: this.t('ui.email') });
      }
      if (this.emailControl.hasError("email"))
        return this.t('messages.emailInvalid');
    }

    return "";
  }
}
