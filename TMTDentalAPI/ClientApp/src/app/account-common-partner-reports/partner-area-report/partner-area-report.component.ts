import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ChartOptions, ChartType } from 'chart.js';
import { Label, SingleDataSet } from 'ng2-charts';

@Component({
  selector: 'app-partner-area-report',
  templateUrl: './partner-area-report.component.html',
  styleUrls: ['./partner-area-report.component.css']
})
export class PartnerAreaReportComponent implements OnInit {
  @ViewChild('companyCbx',{static:true}) companyCbx: ComboBoxComponent;
  @ViewChild('cityCbx',{static:true}) cityCbx: ComboBoxComponent;
  companies: any[] = [{id:'01',name:'Chi nhánh 01'},{id:'02',name:'Chi nhánh 02'}]
  cities: any[] = [{id:'01',name:'TP.Hồ Chí Minh'}];
  dateFrom: any;
  dateTo: any;
  pieObjData: any[] = [
    {name: 'Quận 1',total: 100,percent: '20%', color: '#4271C9'},
    {name: 'Quận 2',total: 200,percent: '30%', color: '#F57A27'},
    {name: 'Quận 3',total: 300,percent: '40%', color: '#A8A8A8'},
    {name: 'Quận 4',total: 400,percent: '20%', color: '#F5C000'},
    {name: 'Quận 5',total: 500,percent: '30%', color: '#4C93D4'},
    {name: 'Quận 6',total: 600,percent: '40%', color: '#6FB342'},
    {name: 'Quận 7',total: 700,percent: '20%', color: '#22427D'},
    {name: 'Quận 8',total: 800,percent: '30%', color: '#A64D15'},
    {name: 'Quận 9',total: 900,percent: '40%', color: '#A5ABB8'},
    {name: 'Quận 10',total: 950,percent: '20%', color: '#A17702'},
    {name: 'Khác',total: 330,percent: '30%', color: '#000000'},
  ]
  pieChartLabels: Label[] = ['Quận 1',  'Quận 2' , 'Quận 3'];
  pieChartData: SingleDataSet = [];
  pieChartType: ChartType = 'pie';
  pieChartLegend = false;
  pieChartPlugins = [this.pieChartLabels];
  pieChartColors = [
    {
      backgroundColor: this.pieObjData.map(x => x.color),
    },
  ];
  pieChartOptions: ChartOptions = {
    responsive: true,
    tooltips: {
      enabled: false
    },
  };

  barChartData = {
    labels: ['Hòa thạnh',  'Tân thạnh','Hòa thạnh',  'Tân thạnh','Hòa thạnh',  'Tân thạnh'],
    datasets: [
      {
        label: 'Khách mới',
        data: [100,200,450,350,900,120],
        backgroundColor: '#2395FF',
        hoverBackgroundColor: 'rgba(35,149,255,0.8)'
      },
      {
        label: 'Khách quay lại',
        data: [100,700,850,550,700,220],
        backgroundColor: '#28A745',
        hoverBackgroundColor: 'rgba(40,167,69,0.8)'
      }
    ]
  }
  barChartType: ChartType = 'bar';
  barChartOptions = {
    responsive: true,
    indexAxis: 'y',
    title: {
      position: 'top',
      display: true,
      text:'Số lượng khách mới - quay lại'
    },
    legend: {
      position: 'bottom',
      display: true
    }
  }
  
  constructor() { }

  ngOnInit() {
    this.initFilterData();
    this.loadPieChart();
  }

  loadPieChart(){
    this.pieChartData = this.pieObjData.map(x => x.total);
  }

  initFilterData() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.dateFrom = this.dateFrom || new Date(y, m, 1);
    this.dateTo = this.dateTo || new Date(y, m + 1, 0);
  }

  onSelectCompany(event){

  }

  onSelectCity(event){

  }

  onSearchDateChange(event){

  }
}
