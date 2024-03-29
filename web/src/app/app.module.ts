import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {StoreConfig,StoreModule} from 'projects/store';
import {UserState} from 'src/common/stores/user.state';
import { UserLoginResponse } from 'src/models/responses/UserLoginResponse';

import { registerLocaleData } from '@angular/common';
import localeDe from '@angular/common/locales/de';
import localeDeExtra from '@angular/common/locales/extra/de';

registerLocaleData(localeDe, 'de-DE', localeDeExtra);
@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,    
    StoreModule.forRoot<UserLoginResponse>({key:"user", state:new UserState()})
  ],
  providers: [
  ],
  entryComponents:[AppComponent],
  bootstrap: [AppComponent]
})
export class AppModule {
}
