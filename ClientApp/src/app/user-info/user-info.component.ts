import { Component, OnInit } from '@angular/core';
import { AutoToLoginComponent } from 'src/app/shared/ui-auto-to-login.component';
import { UserSessionResponse } from 'src/models/responses/UserSessionResponse';
import { FormErrorStateMatcher } from '../shared/form-error-state-matcher';
import { UserRequest } from 'src/models/requests/UserRequest';
import { Store } from 'projects/store';
import { FormControl, Validators } from '@angular/forms';
import {FileState, UploadFileElement} from 'src/common/components/files-upload/files-upload.component';
import { UserLoginResponse } from 'src/models/responses/UserLoginResponse';

@Component({
  selector: 'app-page-user-info',
  templateUrl: './user-info.component.html'
})
export class UserInfoComponent extends AutoToLoginComponent implements OnInit {
  instanz: UserInfoComponent = this;
  matcher: FormErrorStateMatcher = new FormErrorStateMatcher();
  response!: UserSessionResponse;
  userNameControl!: FormControl;
  phoneNumberControl!:FormControl;
  isMarried:boolean=false;
  isLinear: boolean = false;
  fileName:string='';
  selectedFile!:File;
  setting!:UserLoginResponse;

  ngOnInit() {
    this.setting=Store.get('user');
    this.response={userId:'',userName:'',maritalStatus:false,maxUploadFileSize:0,updateTs:0,birthDate:'',sessionUpdateTs:0,sessionId:'',email:'',roleId:'',phoneNumber:'',photoPath:''};
    this.loadResponse();
    this.userNameControl = new FormControl("", [Validators.required,
    Validators.minLength(2)]);
    this.phoneNumberControl=new FormControl("");
  }
  initValue() {
    this.reset('step1');
    this.reset('step2');
  }
  loadResponse() {
    let sessionId = Store.get('user').sessionId;
    this.get<UserSessionResponse>(`Users/${sessionId}`, (data) => {
      this.response = data;
      Store.action('user', 'updateSession')(data);
      this.initValue();
    });
  }
  reset(step: string) {
    if (step == 'step1') {
      this.userNameControl.setValue(this.response.userName);
    }
    if (step == 'step2') {
      this.isMarried=this.response.maritalStatus;
      this.phoneNumberControl.setValue(this.response.phoneNumber);
    }
  }
  save() {
    if (this.invalid())
      return;
    let request: UserRequest;;
  }
  invalid(): boolean {
    return this.userNameControl.invalid;
  }

  getErrorMessage(control: string): string {
    if (control == "userName") {
      if (this.userNameControl.hasError("required")) {
        return this.t('messages.required', { value: this.t('ui.userName') });
      }
      if (this.userNameControl.hasError("minlength"))
        return this.t('messages.minLength', { value: this.t('ui.userName') });
    }

    return "";
  }
  uploadFile(fileElement:UploadFileElement){
    let sessionId = Store.get('user').sessionId;
    this.fileUpload(sessionId,fileElement);
    
  }
}
