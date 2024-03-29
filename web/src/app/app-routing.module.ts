import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ConfigLoaderModule } from 'projects/config-loader/';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { AppMaterialModule } from './material-module';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { HomeComponent } from './home/home.component';
import { ErrorDialog } from './shared/error-dialog/error-dialog.component';
import { EnterDirective } from 'src/common/directives/enter-directive';
import { ValitionInput } from 'src/common/components/validation-input/validation-input.component';
import { FilesUploadComponent } from 'src/common/components/files-upload/files-upload.component';
import { NgChartsModule, NgChartsConfiguration } from 'ng2-charts';
import { DxChartModule, DxRangeSelectorModule } from 'devextreme-angular';
import { IndexTabelleComponent } from './index-tabelle/index-tabelle.component';
import { ToTimeFormatPipe } from 'src/common/directives/time-format-pipe';
import { locale, loadMessages } from "devextreme/localization";
const routes: Routes = [
  { path: 'page-not-fount', component: PageNotFoundComponent },
  { path: 'home', component: HomeComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }

];

export function HttpLoaderFactory(http: HttpClient) {

}

@NgModule({
  declarations: [
    PageNotFoundComponent,
    HomeComponent,
    ErrorDialog,
    EnterDirective,
    ToTimeFormatPipe,
    ValitionInput,
    FilesUploadComponent,
    IndexTabelleComponent,
  ],
  imports: [
    RouterModule.forRoot(routes),
    ConfigLoaderModule.forRoot(),
    NgChartsModule,
    DxChartModule,
    DxRangeSelectorModule,
    FormsModule,
    ReactiveFormsModule,
    MatNativeDateModule,
    AppMaterialModule,
    CommonModule,
    HttpClientModule],
  exports: [
    RouterModule,
    ConfigLoaderModule,
    FormsModule,
    ReactiveFormsModule,
    MatNativeDateModule,
    AppMaterialModule,
    CommonModule,
    HttpClientModule
  ],
  providers: [
    { provide: NgChartsConfiguration, useValue: { generateColors: false } }
  ]
})
export class AppRoutingModule {
  constructor() {
    locale(navigator.language);
  }
}
