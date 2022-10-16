import { Component,OnInit } from '@angular/core';
import { HttpBaseComponent } from '../shared/ui-base-http.component';

@Component({
  selector: 'app-page-home',
  templateUrl: './home.component.html'
})
export class HomeComponent extends HttpBaseComponent implements OnInit {
  csvs:string[]=["test"];
  ngOnInit(){
    
    this.loadCsvs();
  }
  loadCsvs(){
    this.get<string[]>("api/Data/csvs",(data)=>{
      this.csvs.splice(0,this.csvs.length);
      for(let d of data)
        this.csvs.push(d);
    });
  }
  openCsv(csv:string){

  }
}
