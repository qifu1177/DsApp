import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ConfigLoaderModule } from 'projects/config-loader/';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { AppMaterialModule } from './material-module';

import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { UserLoginComponent } from './user-login/user-login.component';
import { UserRegisterComponent } from './user-register/user-register.component';
import { HomeComponent } from './home/home.component';
import { UserInfoComponent } from './user-info/user-info.component';
import { UserChangePasswordComponent } from './user-change-password/user-change-password.component';
import { ForgetPasswordComponent } from './forget-password/forget-password.component';

import { ErrorDialog } from './shared/error-dialog/error-dialog.component';
import {EnterDirective} from 'src/common/directives/enter-directive';
import {ValitionInput} from 'src/common/components/validation-input/validation-input.component';
import {FilesUploadComponent} from 'src/common/components/files-upload/files-upload.component';

const routes: Routes = [
  { path: 'page-not-fount', component: PageNotFoundComponent },
  { path: 'home', component: HomeComponent },
  { path: 'user-login', component: UserLoginComponent },
  { path: 'user-register', component: UserRegisterComponent },
  { path: 'user-info', component: UserInfoComponent },
  { path: 'change-password', component: UserChangePasswordComponent },
  { path: 'forget-password', component: ForgetPasswordComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }

];

export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
    PageNotFoundComponent,
    HomeComponent,
    UserLoginComponent,
    UserRegisterComponent,
    UserInfoComponent,
    UserChangePasswordComponent,
    ForgetPasswordComponent,
    ErrorDialog,
    EnterDirective,
    ValitionInput,
    FilesUploadComponent    
  ],
  imports: [
    RouterModule.forRoot(routes),
    ConfigLoaderModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    FormsModule,
    ReactiveFormsModule,
    MatNativeDateModule,
    AppMaterialModule,
    CommonModule,
    HttpClientModule],
  exports: [
    RouterModule,
    ConfigLoaderModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    MatNativeDateModule,
    AppMaterialModule,
    CommonModule,
    HttpClientModule
  ]
})
export class AppRoutingModule { }
