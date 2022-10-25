import { Component, ViewChild, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { HttpBaseComponent } from '../shared/ui-base-http.component';
import { FileState, UploadFileElement } from 'src/common/components/files-upload/files-upload.component';
import { ResultResponse } from 'src/models/responses/ResultResponse';
import { StringArrayResultResponse } from 'src/models/responses/StringArrayResultResponse';
import { StatusResponse } from 'src/models/responses/StatusResponse';
import { DtValue } from 'src/models/datas/DtValue';
import { Status } from 'src/models/datas/Status';
import { ChartData } from 'src/models/datas/ChartData';
import { Chart, ChartConfiguration, ChartEvent, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { default as Annotation } from 'chartjs-plugin-annotation';
import { DxChartModule, DxRangeSelectorModule } from 'devextreme-angular';
@Component({
  selector: 'app-page-home',
  templateUrl: './home.component.html'
})
export class HomeComponent extends HttpBaseComponent implements OnInit {
  now: Date = new Date();
  panelOpenState: boolean = false;
  fileUploadPaneState: boolean = false;
  csvs: string[] = [];
  instanz: HomeComponent = this;
  uploadFileTypes: string[] = ["csv"];
  maxUploadFileSize: number = 1000000000;
  chartData: ChartData[] = [];
  rawValue: DtValue[] = [];
  indexActivities: DtValue[] = [];
  status: Status[] = [];
  currentFile: string = "";
  currentSdt: Date = new Date(0);
  currentEdt!: Date;
  selectedSdt!: Date;
  selectedEdt!: Date;
  visualRange: any = { startValue: this.currentSdt, endValue: this.currentEdt };
  //for index activity
  deltaSecond: number = 5;
  minVal: number = 0;
  maxVal: number = 10000000;
  //for status
  standbyLimit: number = 0.5;
  minDuration: number = 30;
  //for chart
  public lineChartType: ChartType = 'line';
  public lineChartOptions: ChartConfiguration['options'];
  public lineChartData!: ChartConfiguration['data'];
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;
  constructor(http: HttpClient, router: Router, location: Location, dialog: MatDialog) {
    super(http, router, location, dialog);
    this.currentEdt = this.now;
    this.selectedSdt = this.currentSdt;
    this.selectedEdt = this.currentEdt;
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
          time: {
            unit: 'second'
          },
          title: {
            display: true,
            text: 'Zeit'
          }
        },
        'y-axis-0': {
          position: 'left',
          grid: {
            color: 'rgba(255,0,0,0.3)'
          },
          ticks: {
            color: 'red',
          },
          title: {
            display: true,
            text: '[kW]'
          }
        },
        'y-axis-1': {
          position: 'right',
          grid: {
            color: 'rgba(255,0,0,0.3)'
          },
          ticks: {
            color: 'blue',
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
        pointRadius: 0,
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
        pointRadius: 0,
        fill: 'origin',
        data: []
      }
      ]
    };
  }
  format(dt: Date): string {
    return `${dt.getDate()}.${dt.getMonth()}.${dt.getFullYear()} ${dt.getHours()}:${dt.getMinutes()}:${dt.getSeconds()}`;
  }
  wirklesitungCustomizeText(o: any) {
    return `kW`;
  }
  customizeTooltip = (info: any) => ({
    html: `<div><div class='tooltip-header'>${
      this.format(info.argument)}</div>`
                + '<div class=\'tooltip-body\'><div class=\'series-name\'>'
                + `<span class='top-series-name'>${info.points[0].seriesName}</span>`
                + ': '
                + `<span class='top-series-value'>${info.points[0].valueText}</span>`
                + '</div><div class=\'series-name\'>'
                + `<span class='bottom-series-name'>${info.points[1].seriesName}</span>`
                + ': '
                + `<span class='bottom-series-value'>${info.points[1].valueText}</span>`
                + '</div></div></div>',
  });
  indexActivityCustomizeText(o: any) {
    return ``;
  }
  private updateChartDatas() {

    this.chartData.splice(0, this.chartData.length);
    for (let item of this.rawValue) {
      let dv: ChartData = { dt: item.dt, rawValue: item.value, indexActivity: null };
      this.chartData.push(dv);
    }
  }
  private updateChartIndexs() {

    this.chartData.splice(0, this.chartData.length);
    let i: number = 0;
    for (let item of this.rawValue) {
      let dv: ChartData = { dt: item.dt, rawValue: item.value, indexActivity: null };
      if (i < this.indexActivities.length) {
        if (this.indexActivities[i].dt.getTime() == item.dt.getTime()) {
          dv.indexActivity = this.indexActivities[i].value;
          i++;
        }
      }
      this.chartData.push(dv);
    }


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
        /* this.updateChartDatas(); */
      }
    });
  }
  reloadIndexActivity() {
    this.loadIndexActivity(this.selectedSdt.getTime(), this.selectedEdt.getTime());
  }
  calculateStatus() {
    this.loadStatus(this.selectedSdt.getTime(), this.selectedEdt.getTime());
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
  loadStatus(sdt: number, edt: number) {
    this.get<StatusResponse>(`api/Data/status/${this.currentFile}/${sdt}/${edt}/${this.standbyLimit}/${this.minDuration}`, (data) => {
      this.status.splice(0, this.status.length);
      let vals = data.value;
      for (let val of vals) {
        let v:Status={Sdt:new Date(val.sdt),Edt:new Date(val.sdt),Value:val.status};
        this.status.push(v);
      }
      //this.updateChartIndexs();
    });
  }
  zoomChanged(o: any) {
    this.selectedSdt = o.value[0];
    this.selectedEdt = o.value[1];
    if (this.selectedSdt == this.currentSdt && (this.selectedEdt == this.currentEdt || this.selectedEdt == this.now))
      return;
    this.loadRawDatas(this.selectedSdt.getTime(), this.selectedEdt.getTime());
  }
  uploadFile(fileElement: UploadFileElement) {
    this.fileUpload(fileElement, (message) => {
      this.loadCsvs();
    });
  }

}
