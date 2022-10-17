import { Component,OnInit } from '@angular/core';
import { HttpBaseComponent } from '../shared/ui-base-http.component';
import {FileState, UploadFileElement} from 'src/common/components/files-upload/files-upload.component';
import {ResultResponse} from 'src/models/responses/ResultResponse';
import {StringArrayResultResponse} from 'src/models/responses/StringArrayResultResponse';
import {DtValue} from 'src/models/datas/DtValue';
@Component({
  selector: 'app-page-home',
  templateUrl: './home.component.html'
})
export class HomeComponent extends HttpBaseComponent implements OnInit {
  csvs:string[]=[];
  instanz:HomeComponent=this;
  uploadFileTypes:string[]=["csv"];
  maxUploadFileSize:number=1000000000;
  rawValue:DtValue[]=[];
  ngOnInit(){
    
    this.loadCsvs();
  }
  loadCsvs(){
    this.get<StringArrayResultResponse>("api/Data/csvs",(data)=>{
      this.csvs.splice(0,this.csvs.length);
      for(let d of data.value)
        this.csvs.push(d);
    });
  }
  openCsv(csv:string){
    let now=new Date();
    this.get<ResultResponse>(`api/Data/data/${csv}/0/${now.getTime()}`,(data)=>{
      this.rawValue.splice(0,this.rawValue.length);
      let vals=data.value.split(';');
      for(let val of vals)
      {
        let strs=val.split(':');
        if(strs.length==2){
          let dtval:DtValue={dt:new Date(Number(strs[0])), value:Number(strs[1]) };
          this.rawValue.push(dtval);
        }
      }
    });
  }
  uploadFile(fileElement:UploadFileElement){    
    this.fileUpload(fileElement,(message)=>{
      this.loadCsvs();
    });    
  }
  
}
