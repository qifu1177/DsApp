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
import {EnterDirective} from 'src/common/directives/enter-directive';
import {ValitionInput} from 'src/common/components/validation-input/validation-input.component';
import {FilesUploadComponent} from 'src/common/components/files-upload/files-upload.component';

const routes: Routes = [
  { path: 'page-not-fount', component: PageNotFoundComponent },
  { path: 'home', component: HomeComponent },  
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }

];

export function HttpLoaderFactory(http: HttpClient){
  
}

@NgModule({
  declarations: [
    PageNotFoundComponent,
    HomeComponent,
    ErrorDialog,
    EnterDirective,
    ValitionInput,
    FilesUploadComponent    
  ],
  imports: [
    RouterModule.forRoot(routes),
    ConfigLoaderModule.forRoot(),
    
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
  ]
})
export class AppRoutingModule { }
