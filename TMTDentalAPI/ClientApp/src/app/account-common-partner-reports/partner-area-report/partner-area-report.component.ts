import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ChartOptions, ChartType } from 'chart.js';
import { Label, SingleDataSet } from 'ng2-charts';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { PartnerOldNewReportService } from 'src/app/sale-report/partner-old-new-report.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-partner-area-report',
  templateUrl: './partner-area-report.component.html',
  styleUrls: ['./partner-area-report.component.css']
})
export class PartnerAreaReportComponent implements OnInit {
  @ViewChild('companyCbx',{static:true}) companyCbx: ComboBoxComponent;
  @ViewChild('cityCbx',{static:true}) cityCbx: ComboBoxComponent;
  @ViewChild('pieChart', {static: true}) pieChart: any;
  companies: any[] = [{id:'01',name:'Chi nhánh 01'},{id:'02',name:'Chi nhánh 02'}]
  cities: any[] = [{id:'01',name:'TP.Hồ Chí Minh'}];
  dateFrom: any;
  dateTo: any;
  currentCompany: any;
  currentCity: any;
  dataSourceCities: Array<{ code: string; name: string }>;
  dataResultCities: Array<{ code: string; name: string }>;
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
  pieChartLabels: Label[] = this.pieObjData.map(x => x.name);
  pieChartData: SingleDataSet = [];
  pieChartType: ChartType = 'pie';
  pieChartLegend = true;
  pieChartPlugins = [this.pieChartLabels];
  pieChartColors = [
    {
      backgroundColor: this.pieObjData.map(x => x.color),
    },
  ];
  pieChartOptions = {
    responsive: true,
    tooltips: {
      enabled: true
    },
    legend: {
      position: 'bottom',
      display: true,
      legendCallback: function() {
        console.log('legendCallback');
        
      }
    },
    title: {
      display: true,
      text: 'Chart.js Pie Chart'
    }
  };

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

  barChartData = {
    labels: ['Hòa thạnh',  'Tân thạnh','Hòa thạnh',  'Tân thạnh','Hòa thạnh',  'Tân thạnh'],
    partnerOldNewCount_datasets: [
      {
        label: 'Khách mới',
        data: [],
        backgroundColor: '#2395FF',
        hoverBackgroundColor: 'rgba(35,149,255,0.8)'
      },
      {
        label: 'Khách quay lại',
        data: [],
        backgroundColor: '#28A745',
        hoverBackgroundColor: 'rgba(40,167,69,0.8)'
      }
    ], 
    partnerOldNewRevenue_datasets: [
      {
        label: 'Khách mới',
        data: [],
        backgroundColor: '#2395FF',
        hoverBackgroundColor: 'rgba(35,149,255,0.8)'
      },
      {
        label: 'Khách quay lại',
        data: [],
        backgroundColor: '#28A745',
        hoverBackgroundColor: 'rgba(40,167,69,0.8)'
      }
    ]
  }
  
  constructor(private http: HttpClient,
    private companyService: CompanyService, 
    private partnerOldNewRpService: PartnerOldNewReportService
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadPieChart();
    this.loadSourceCities();
    this.loadCompanies();
    this.loadCurrentCompany();
    console.log(this.pieChart);
    
    
  }

  loadSourceCities() {
    this.http
      .post(environment.ashipApi + "api/ApiShippingCity/GetCities", {
        provider: "Undefined",
      })
      .subscribe((result: any) => {
        this.dataSourceCities = result;
        this.dataResultCities = this.dataSourceCities.slice();
      });
  }

  loadPieChart(){
    this.pieChartData = this.pieObjData.map(x => x.total);
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  loadCurrentCompany(){
    var companyId = JSON.parse(localStorage.getItem('user_info')).companyId;
    this.companyService.get(companyId).subscribe(result => {
      this.currentCompany = {id: result.id, name: result.name};
      this.currentCity = {code: result.city.code, name: result.city.name};
    });
  }

  filterCombobox(){
    this.companyCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyCbx.loading = true)),
        switchMap((value) => this.searchCompany$(value)
        )
      )
      .subscribe((x: any) => {
        this.companies = x.items;
        this.companyCbx.loading = false;
      });
  }

  handleCityFilter(value) {
    this.dataResultCities = this.dataSourceCities.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  initFilterData() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.dateFrom = this.dateFrom || new Date(y, m, 1);
    this.dateTo = this.dateTo || new Date(y, m + 1, 0);
  }

  onSelectCompany(event){
    console.log(event);
    if (event){
      let companyId = event.id;
      this.companyService.get(companyId).subscribe(result => {
      this.currentCity = {code: result.city.code, name: result.city.name};
      });
    }
    else {
      this.currentCity = {code: '79'};
    }
  }

  onSelectCity(event){
    console.log(event);
    if (event){
      var val = new CompanyPaged();
      val.cityCode = event.code;
      this.companyService.getPaged(val).subscribe(result => {
      this.companies = result.items;
      this.currentCompany = this.companies ? this.companies[0] : {};
      });
    }
    else {
      this.loadCompanies();
      // this.currentCompany = this.companies ? this.companies[0] : {};
    }
  }

  onSearchDateChange(event) {
    this.dateFrom = event.dateFrom;
    this.dateTo = event.dateTo;
  }
}
