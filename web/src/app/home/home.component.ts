import { Component, ViewChild,OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { HttpBaseComponent } from '../shared/ui-base-http.component';
import { FileState, UploadFileElement } from 'src/common/components/files-upload/files-upload.component';
import { ResultResponse } from 'src/models/responses/ResultResponse';
import { StringArrayResultResponse } from 'src/models/responses/StringArrayResultResponse';
import { DtValue } from 'src/models/datas/DtValue';

import { Chart, ChartConfiguration, ChartEvent, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { default as Annotation } from 'chartjs-plugin-annotation';

@Component({
  selector: 'app-page-home',
  templateUrl: './home.component.html'
})
export class HomeComponent extends HttpBaseComponent implements OnInit {
  csvs: string[] = [];
  instanz: HomeComponent = this;
  uploadFileTypes: string[] = ["csv"];
  maxUploadFileSize: number = 1000000000;
  rawValue: DtValue[] = [];
  indexActivities: DtValue[] = [];
  currentFile: string = "";
  currentSdt: Date = new Date(0);
  currentEdt: Date = new Date();
  deltaSecond: number = 5;
  minVal: number = 0;
  maxVal: number = 10000000;
  //for chart
  public lineChartType: ChartType = 'line';
  public lineChartOptions: ChartConfiguration['options'];
  public lineChartData!: ChartConfiguration['data'];
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;
  constructor(http: HttpClient, router: Router, location: Location, dialog: MatDialog) {
    super(http, router, location, dialog);
    Chart.register(Annotation);
  }
  ngOnInit() {
    this.initChart();
    this.loadCsvs();
  }
  initChart() {
    this.lineChartOptions = {
      elements: {
        line: {
          tension: 0.5
        }
      },
      scales: {
        // We use this empty structure as a placeholder for dynamic theming.
        x: { 
          time:{
            unit:'second'
          },
          title: {
            display: true,
            text: 'Zeit'
          }
        },
        'y-axis-0': {
          position:'left',
          grid:{
            color:'rgba(255,0,0,0.3)'
          },
          ticks:{
            color:'red',
          },
          title: {
            display: true,
            text: '[kW]'
          }
        },
        'y-axis-1': {
          position:'right',
          grid:{
            color:'rgba(255,0,0,0.3)'
          },
          ticks:{
            color:'blue',
          },
          title: {
            display: true,
            text: 'Index Activity'
          }
        }
      }
    };
    this.lineChartData = {
      labels: [],
      datasets: [{
        label: 'Wirkleistung',
        backgroundColor: 'rgba(255,255,255,0)',
        borderColor: 'rgba(36,117,78,1)',
        pointBackgroundColor: 'rgba(148,159,177,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(148,159,177,0.8)',
        pointRadius:0,
        fill: 'origin',
        data: []
      },
      {
        label: 'Index Activity',
        backgroundColor: 'rgba(255,255,255,0)',
        borderColor: 'rgba(102, 102, 51,1)',
        pointBackgroundColor: 'rgba(148,159,177,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(148,159,177,0.8)',
        pointRadius:0,
        fill: 'origin',
        data: []
      }
    ]
    };
  }
  private updateChartDatas() {
    this.lineChartData.labels?.splice(0, this.lineChartData.labels.length);
    this.lineChartData.datasets[0].data.splice(0, this.lineChartData.datasets[0].data.length);
    for (let item of this.rawValue) {
      let dtstr=`${item.dt.getDate()}.${item.dt.getMonth()}.${item.dt.getFullYear()} ${item.dt.getHours()}:${item.dt.getMinutes()}:${item.dt.getSeconds()}`;
      this.lineChartData.labels?.push(dtstr);
      this.lineChartData.datasets[0].data.push(item.value);
    }
    this.chart?.update();
  }
  private updateChartIndexs() {
    this.lineChartData.datasets[1].data.splice(0, this.lineChartData.datasets[1].data.length);
    for (let item of this.indexActivities) {
      this.lineChartData.datasets[1].data.push(item.value);
    }
    this.chart?.update();
  }
  // events
  public chartClicked({ event, active }: { event?: ChartEvent, active?: {}[] }): void {
    console.log(event, active);
  }

  public chartHovered({ event, active }: { event?: ChartEvent, active?: {}[] }): void {
    console.log(event, active);
  }
  loadCsvs() {
    this.get<StringArrayResultResponse>("api/Data/csvs", (data) => {
      this.csvs.splice(0, this.csvs.length);
      for (let d of data.value)
        this.csvs.push(d);
    });
  }
  openCsv(csv: string) {
    this.currentFile = csv;
    this.currentSdt = new Date(0);
    this.currentEdt = new Date();
    this.loadRawDatas(this.currentSdt.getTime(), this.currentEdt.getTime());
  }
  loadRawDatas(sdt: number, edt: number) {
    this.get<ResultResponse>(`api/Data/data/${this.currentFile}/${sdt}/${edt}`, (data) => {
      this.rawValue.splice(0, this.rawValue.length);
      let vals = data.value.split(';');
      for (let val of vals) {
        let strs = val.split(':');
        if (strs.length == 2) {
          let dtval: DtValue = { dt: new Date(Number(strs[0])), value: Number(strs[1]) };
          this.rawValue.push(dtval);
        }
      }
      let count = this.rawValue.length;
      if (this.currentSdt.getTime() == 0 && count > 0) {
        this.currentSdt = this.rawValue[0].dt;
        this.currentEdt = this.rawValue[count - 1].dt;
      }
      if (count > 0) {
        this.loadIndexActivity(sdt, edt);
        this.updateChartDatas();
      }
    });
  }
  loadIndexActivity(sdt: number, edt: number) {
    this.get<ResultResponse>(`api/Data/indexActivity/${this.currentFile}/${sdt}/${edt}/${this.deltaSecond}/${this.minVal}/${this.maxVal}`, (data) => {
      this.indexActivities.splice(0, this.indexActivities.length);
      let vals = data.value.split(';');
      for (let val of vals) {
        let strs = val.split(':');
        if (strs.length == 2) {
          let dtval: DtValue = { dt: new Date(Number(strs[0])), value: Number(strs[1]) };
          this.indexActivities.push(dtval);
        }
      }
      this.updateChartIndexs();
    });
  }
  uploadFile(fileElement: UploadFileElement) {
    this.fileUpload(fileElement, (message) => {
      this.loadCsvs();
    });
  }

}
