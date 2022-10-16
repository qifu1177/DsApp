import { Directive } from '@angular/core';
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { HttpComponent } from "./http.component";
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ErrorDialog } from './error-dialog/error-dialog.component';
import {FileState, UploadFileElement} from 'src/common/components/files-upload/files-upload.component';
import {MessageResponse} from 'src/models/responses/MessageResponse';

export interface callBack<T>{(params:T):void};
@Directive()
export abstract class HttpBaseComponent extends HttpComponent {
    waiting:boolean=false;
    constructor(protected _http: HttpClient, protected _translate: TranslateService, protected _router: Router,protected _location:Location, public dialog: MatDialog) {
        super(_http);
    }
    t(key: string, value: any = null): string {
        if (value)
            return this._translate.instant(key, value);
        else
            return this._translate.instant(key);
    }
    showError(error: any, width: number = 360): void {
        if(error.error==null){
            error.error={};            
        }        
        const dialogRef = this.dialog.open(ErrorDialog, {
            width: `${width}px`,
            data: error
        });

        dialogRef.afterClosed().subscribe(result => {
        });
    }
    back() {
        this._location.back();
    }
    get<T>(url:string,callback:callBack<T>){
        this.waiting=true;
        this._http.get<T>(this.createUrl(url)).subscribe({
            next: data => {
                this.waiting=false;
                callback(data);
            },
            error: error => {
                this.waiting=false;
                this.showError(error);
            }
        });
    }
    post<T>(url:string,request:any,callback:callBack<T>){
        this.waiting=true;
        this._http.post<T>(this.createUrl(url), request).subscribe({
            next: data => {
                this.waiting=false;
                callback(data);
            },
            error: error => {
                this.waiting=false;
                this.showError(error);
            }
        });
    }
    put<T>(url:string,request:any,callback:callBack<T>){
        this.waiting=true;        
        this._http.put<T>(this.createUrl(url), request).subscribe({
            next: data => {
                this.waiting=false;
                callback(data);
            },
            error: error => {
                this.waiting=false;
                this.showError(error);
            }
        });
    }
    delete<T>(url:string,callback:callBack<T>){
        this.waiting=true;
        this._http.delete<T>(this.createUrl(url)).subscribe({
            next: data => {
                this.waiting=false;
                callback(data);
            },
            error: error => {
                this.waiting=false;
                this.showError(error);
            }
        });
    }
    fileUpload(sessionId:string,file:UploadFileElement){
        file.state=FileState.run;
        let formData:FormData = new FormData();
        formData.append('uploadFile', file.file, file.file.name);        
        let url:string=this.createUrl(`Files/upload/${sessionId}`);
        this._http.post<MessageResponse>(url, formData).subscribe({
            next: data => {               
                file.state=FileState.success;
            },
            error: error => {
                file.state=FileState.failed;
                this.showError(error);
            }
        });
    }
    getFileUrl(fileName:string,sessionId:string):string{
        return this.createUrl(`Files/${fileName}/${sessionId}`);
    }
    getIconUrl(fileName:string,sessionId:string):string{
        return this.createUrl(`Files/icon/${fileName}/${sessionId}`);
    }
    fileDelete(fileName:string,sessionId:string){
        this.waiting=true;
        let url:string=this.getFileUrl(fileName,sessionId);
        this._http.delete<MessageResponse>(this.createUrl(url)).subscribe({
            next: data => {               
                this.waiting=false;
            },
            error: error => {
                this.waiting=false;
                this.showError(error);
            }
        });
    }
    fileList(sessionId:string,fileList:string[]){
        this.waiting=true;
        let url:string=this.createUrl(`Files/${sessionId}`);
        this._http.get<string[]>(url).subscribe({
            next: data => {
                this.waiting=false;
                fileList.splice(0,fileList.length);
                for(let item of data)
                    fileList.push(item);                
            },
            error: error => {
                this.waiting=false;
                this.showError(error);
            }
        });
    }

}


