import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import * as Chart from 'chart.js';
import { ChartOptions, ChartType } from 'chart.js';
import { Label, SingleDataSet } from 'ng2-charts';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountInvoiceReportService, RevenueDistrictAreaPar } from 'src/app/account-invoice-reports/account-invoice-report.service';
import { CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { PartnerOldNewReportByWardReq, PartnerOldNewReportService } from 'src/app/sale-report/partner-old-new-report.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-partner-area-report',
  templateUrl: './partner-area-report.component.html',
  styleUrls: ['./partner-area-report.component.css']
})
export class PartnerAreaReportComponent implements AfterViewInit, OnInit {
  @ViewChild('companyCbx',{static:true}) companyCbx: ComboBoxComponent;
  @ViewChild('cityCbx',{static:true}) cityCbx: ComboBoxComponent;
  @ViewChild('pieCanvas', {static: true}) pieCanvas: ElementRef;
  pieChart: any;
  companies: any[] = [];
  cities: any[] = [{id:'01',name:'TP.Hồ Chí Minh'}];
  dateFrom: any;
  dateTo: any;
  currentCompany: any = {};
  currentCity: any;
  currentDistrictCode: any;
  dataSourceCities: Array<{ code: string; name: string }>;
  dataResultCities: Array<{ code: string; name: string }>;
  cityName: string = '';
  // pieObjData: any[] = [
  //   {name: 'Quận 1',total: 100,percent: '20%', color: '#4271C9'},
  //   {name: 'Quận 2',total: 200,percent: '30%', color: '#F57A27'},
  //   {name: 'Quận 3',total: 300,percent: '40%', color: '#A8A8A8'},
  //   {name: 'Quận 4',total: 400,percent: '20%', color: '#F5C000'},
  //   {name: 'Quận 5',total: 500,percent: '30%', color: '#4C93D4'},
  //   {name: 'Quận 6',total: 600,percent: '40%', color: '#6FB342'},
  //   {name: 'Quận 7',total: 700,percent: '20%', color: '#22427D'},
  //   {name: 'Quận 8',total: 800,percent: '30%', color: '#A64D15'},
  //   {name: 'Quận 9',total: 900,percent: '40%', color: '#A5ABB8'},
  //   {name: 'Quận 10',total: 950,percent: '20%', color: '#A17702'},
  //   {name: 'Khác',total: 330,percent: '30%', color: '#000000'},
  // ]
  pieLabels: any[] = []
  pieOptions: any= {};
  pieDatasets: any[] = [];
  pieObjData: any[] = []
  pieChartData: any[] = [];
  pieChartColors = ['#4271C9','#F57A27','#A8A8A8','#F5C000','#4C93D4','#6FB342','#22427D','#A64D15','#A5ABB8','#A17702','#000000'];
  colorArr: string[] = [];

  barChartOptions_PartnerCount = {
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

  barChartData_PartnerCount = {
    labels: [],
    datasets: [
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

  barChartOptions_PartnerRevenue = {
    responsive: true,
    indexAxis: 'y',
    title: {
      position: 'top',
      display: true,
      text:'Doanh thu khách mới - quay lại'
    },
    legend: {
      position: 'bottom',
      display: true
    }
  }

  barChartData_PartnerRevenue = {
    labels: [],
    datasets: [
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
  
  constructor (private http: HttpClient,
    private companyService: CompanyService,
    private accountInvoiceReportService: AccountInvoiceReportService,
    private intlService: IntlService,
    private _elementRef : ElementRef,
    private partnerOldNewRpService: PartnerOldNewReportService
  ) { }

  ngOnInit() {
    this.currentCompany.id = JSON.parse(localStorage.getItem('user_info')).companyId;
    this.initFilterData();
    this.loadSourceCities();
    this.loadCompanies();
    this.loadCurrentCompany();
    this.loadDataFromApi();
    
  }

  ngAfterViewInit(){
  }

  loadDataFromApi(){
    var val = new RevenueDistrictAreaPar();
    var colors = this.pieChartColors;
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.cityCode = this.currentCity ? this.currentCity.code : null;
    val.companyId = this.currentCompany ? this.currentCompany.id : null;
    this.accountInvoiceReportService.getRevenueDistrictArea(val).subscribe(result => {
      this.pieLabels = result.map(x => x.districtName);
      this.pieDatasets = [{
        backgroundColor: this.pieChartColors.slice(0,result.length),
        data: result.map(x => x.revenue)
      }];
      this.pieObjData = result;
      this.pieObjData.forEach(function(item,index){
        item.color = colors[index];
      });
      this.pieChartData = this.pieObjData.map(x => x.revenue);
      this.loadPieChart();

      if (this.pieObjData && this.pieObjData.length > 0) {
        this.currentDistrictCode = this.pieObjData[0].districtCode;
        this.loadDataReportByWard();
      }
    });
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
    var pieData = this.pieObjData;
    var itemId = 0;
    this.pieOptions = {
      responsive: true,
      legend: {
        display: false,
      },
     
      tooltips: {
              enabled: true,
              callbacks: {
                title: function(tooltipItem, data) {
                   let id = tooltipItem[0] ? tooltipItem[0].index : 1; 
                   itemId = id
                  return data.labels[id].toString();
                },
                label: function(tooltipItem, data) {
                  let labels =  "Tổng doanh thu "+ Intl.NumberFormat().format(pieData[itemId].revenue); 
                  return labels;
                },
                afterLabel: function(tooltipItem, data) {
                  // let id = tooltipItem[0] ? tooltipItem[0].index : 1;
                  let percent = "Chiếm tỷ lệ "+ pieData[itemId].percent; 
                  return percent;
                }
              },
              backgroundColor: '#FFF',
              titleFontSize: 16,
              titleFontColor: '#0066ff',
              bodyFontColor: '#000',
              bodyFontSize: 14,
              displayColors: false
      }
    }
    var ulHtml = '';
              var leftLegendHtml = [];
              var rightLegendHtml = [];
              if (pieData.length == 0)
                ulHtml = '';
              else if (pieData.length > 6){
                var leftData = pieData.slice(0,6);
                leftData.forEach(item => {
                  var html = '<li style="display:flex; padding-top: 3px; padding-bottom:3px"><span style="flex:1; min-width: 115px"><i class="fas fa-circle mr-2" style="color:'+item.color+'"></i>'+item.name+'</span>'+
                  '<span style="padding-left:3px">('+item.percent+')</span>';
                  leftLegendHtml.push(html);
                });
                var rightData = pieData.slice(6,11);
                rightData.forEach(item => {
                  var html = '<li style="display:flex; padding-top: 3px; padding-bottom:3px"><span style="flex:1; min-width: 115px"><i class="fas fa-circle mr-2" style="color:'+item.color+'"></i>'+item.districtName+'</span>'+
                  '<span style="padding-left:3px">('+item.percent+')</span>';
                  rightLegendHtml.push(html);
                });
                ulHtml = '<ul style="list-style-type: none; margin-left: auto; margin-right:auto; padding:0">'
                +leftLegendHtml.join('')+'</ul>'+
                '<ul style="list-style-type: none; margin-left: auto; margin-right:auto; padding:0">'
                +rightLegendHtml.join('')+'</ul>';
              }
              else {
                pieData.forEach(item => {
                  var html = '<li style="display:flex; padding-top: 3px; padding-bottom:3px"><span style="flex:1; min-width: 115px"><i class="fas fa-circle mr-2" style="color:'+item.color+'"></i>'+item.districtName+'</span>'+
                  '<span style="padding-left:3px">('+item.percent+')</span>';
                  leftLegendHtml.push(html);
                });
                ulHtml ='<ul style="list-style-type: none; margin-left: auto; margin-right:auto; padding:0">' + leftLegendHtml.join('')+
                '</ul>';
              }

    let legend = this._elementRef.nativeElement.querySelector(`#pie-chart-legend`);
    legend.innerHTML = ulHtml;
    // Chart.helpers.each(Chart.instances, function(instance){
    //   if (instance.config.type == "pie"){
    //     console.log(instance);
        
    //     legend.innerHTML = ulHtml;
    //   }
    // });
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
      // this.loadCurrentCompany();
    });
  }

  loadCurrentCompany(){
    var companyId = JSON.parse(localStorage.getItem('user_info')).companyId;
    this.companyService.get(companyId).subscribe(result => {
      this.currentCompany = {id: result.id, name: result.name};
      this.currentCity = {code: result.city.code, name: result.city.name};
      this.cityName = result.city.name;
      this.loadDataFromApi();
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
    if (event){
      let companyId = event.id;
      this.currentCompany.id = companyId;
      this.currentCity.code = '';
      this.loadDataFromApi();
      this.companyService.get(companyId).subscribe(result => {
        this.currentCity = {code: result.city.code, name: result.city.name};
        this.cityName = result.city.name;
      });
      
    }
    else {
      this.currentCity.code = '79';
      this.currentCompany.id = '';
      this.cityName = this.currentCity ? this.currentCity.name : '';
      this.loadDataFromApi();
    }

  }

  onSelectCity(event){
    if (event){
      this.cityName = event.name;
      this.currentCompany.id = '';
      this.currentCity.code = event.code;
      this.loadDataFromApi();
      
    }
    else {
      this.cityName = '';
      this.currentCompany.id = '';
      this.currentCity.code = '';
      this.loadDataFromApi();
    }
  }

  onSelectDistrict(event) {
    if (event && event.districtCode != this.currentDistrictCode) {
      this.currentDistrictCode = event.districtCode;
      this.loadDataReportByWard();
    }
  }

  onSearchDateChange(event) {
    this.dateFrom = event.dateFrom;
    this.dateTo = event.dateTo;
    this.loadDataReportByWard(); 
    this.loadDataFromApi();
  }

  loadDataReportByWard() {
    var val = new PartnerOldNewReportByWardReq();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    val.cityCode = this.currentCity ? this.currentCity.code || '' : '';
    val.districtCode = this.currentDistrictCode || '';
    val.companyId = this.currentCompany ? this.currentCompany.id || '' : '';
    
    this.partnerOldNewRpService.reportByWard(val).subscribe((res: any) => {
      if (res && res.length > 0) {
        this.barChartData_PartnerCount.labels = res.map(x => x.wardName);
        this.barChartData_PartnerCount.datasets[0].data = res.map(x => x.partnerNewCount);
        this.barChartData_PartnerCount.datasets[1].data = res.map(x => x.partnerOldCount);
        this.barChartData_PartnerRevenue.labels = res.map(x => x.wardName);
        this.barChartData_PartnerRevenue.datasets[0].data = res.map(x => x.partnerNewRevenue);
        this.barChartData_PartnerRevenue.datasets[1].data = res.map(x => x.partnerOldRevenue);
      } else {
        this.barChartData_PartnerCount.labels = [];
        this.barChartData_PartnerCount.datasets[0].data = [];
        this.barChartData_PartnerCount.datasets[1].data = [];
        this.barChartData_PartnerRevenue.labels = [];
        this.barChartData_PartnerRevenue.datasets[0].data = [];
        this.barChartData_PartnerRevenue.datasets[1].data = [];
      }
    }, err => {
      console.log(err);
    })
  }
}

