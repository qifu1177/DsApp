import { Component, Input } from '@angular/core';
import {Store} from 'projects/store';
import { UserLoginResponse } from 'src/models/responses/UserLoginResponse';

export enum FileState {
    start,
    success,
    failed,
    run
}
export interface UploadFileElement {
    file: File;
    state: FileState;
}
@Component({
    selector: 'files-upload',
    templateUrl: './files-upload.component.html'
})

export class FilesUploadComponent {
    @Input('formInstanz') formInstanz: any = {};
    @Input('sendAction') sendAction: string = '';
    @Input('uploadFileTypes') uploadFileTypes:string[]=[];
    @Input('maxUploadFileSize') maxUploadFileSize:number=0;

    fileState=FileState;
    selectedFiles: Map<string, UploadFileElement> = new Map();
    setting:UserLoginResponse=Store.get('user');

    changFile(event: any) {
        let fileInput = event.target;
        for (let item of fileInput.files) {
            if (this.selectedFiles.has(item.name))
                continue;
            this.selectedFiles.set(item.name, { file: item, state: FileState.start });
        }
    }
    selectFile(event: any) {
        let currentElement = event.currentTarget || event.target;
        let next = currentElement.nextElementSibling;
        next.click();
    }
    drag(event: any) {
        event.preventDefault();
    }
    drop(event: any) {
        for (let item of event.dataTransfer.files) {
            if (this.selectedFiles.has(item.name))
                continue;
            this.selectedFiles.set(item.name, { file: item, state: FileState.start });
        }
        event.preventDefault();
    };
    deleteAll(event: any) {
        this.selectedFiles.clear();
    }
    delete(key: string) {
        this.selectedFiles.delete(key);
    }
    upload(item: UploadFileElement) {
        if(this.invalid(item))
            return;
        this.formInstanz[this.sendAction](item);
        this.maxUploadFileSize-=item.file.size;
    }
    uploadAll(event: any) {
        this.selectedFiles.forEach((value) => {
            this.upload(value);
        })
    }
    getFileType(fileName:string):string{
        let postion=fileName.lastIndexOf('.');
        let fileType= fileName.substr(postion+1);
        return fileType;
    }
    invalid(item: UploadFileElement):boolean{
        let validType=this.uploadFileTypes.includes(this.getFileType(item.file.name));
        return !( validType && this.maxUploadFileSize>item.file.size);
    }
    invalidAll(){
        let sumSize:number=0;
        this.selectedFiles.forEach((value)=>{
            if(this.invalid(value))
                return;
            sumSize+=value.file.size;            
        })
        return this.maxUploadFileSize<sumSize;
    }
    uploadParamenter():any{
        let uploadFileTypeStr=this.uploadFileTypes.join('|');
        return {"typeStr":uploadFileTypeStr, "maxSize":this.maxUploadFileSize};
    }
}