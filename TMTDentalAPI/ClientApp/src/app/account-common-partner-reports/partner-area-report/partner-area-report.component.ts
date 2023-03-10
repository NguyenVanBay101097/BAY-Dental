import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { ChartDataset, ChartOptions } from 'chart.js';
import * as pluginDataLabels from 'chartjs-plugin-datalabels';
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
  @ViewChild('companyCbx', { static: true }) companyCbx: ComboBoxComponent;
  @ViewChild('cityCbx', { static: true }) cityCbx: ComboBoxComponent;
  @ViewChild('pieCanvas', { static: true }) pieCanvas: ElementRef;
  @ViewChild('contPieChart', { static: true }) contPieChart: ElementRef;
  pieChart: any;
  companies: any[] = [];
  dateFrom: any;
  dateTo: any;
  currentCompany: string;
  currentCity: any;
  currentDistrictCode: any;
  dataSourceCities: Array<{ code: string; name: string }>;
  dataResultCities: Array<{ code: string; name: string }>;
  pieLabels: any[] = []
  pieOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'right',
      }
    }
  }
  pieChartPlugins = [pluginDataLabels];
  pieDatasets: any[] = [];
  pieObjData: any[] = []
  pieChartData: any[] = [];
  pieChartColors = ['#4271C9', '#F57A27', '#A8A8A8', '#F5C000', '#4C93D4', '#6FB342', '#22427D', '#A64D15', '#A5ABB8', '#A17702', '#000000'];
  colorArr: string[] = [];

  barChartOptions_PartnerCount: ChartOptions = {
    responsive: true,
    indexAxis: 'y',
    plugins: {
      title: {
        position: 'top',
        display: true,
        text: 'S??? l?????ng kh??ch m???i - quay l???i'
      },
      legend: {
        position: 'bottom',
        display: true
      },
      tooltip: {
        mode: 'index'
      }
    },
    scales: {
      x: {
        ticks: {
          stepSize: 1
        },
        beginAtZero: true
      },
    }
  }

  barChartLabels_PartnerCount: string[] = [];
  barChartData_PartnerCount: ChartDataset[] = [
    {
      label: 'Kh??ch m???i',
      data: [],
      backgroundColor: '#2395FF',
      hoverBackgroundColor: 'rgba(35,149,255,0.8)'
    },
    {
      label: 'Kh??ch quay l???i',
      data: [],
      backgroundColor: '#28A745',
      hoverBackgroundColor: 'rgba(40,167,69,0.8)'
    }
  ]

  barChartOptions_PartnerRevenue: ChartOptions = {
    responsive: true,
    indexAxis: 'y',
    plugins: {
      title: {
        position: 'top',
        display: true,
        text: 'Doanh thu ??i???u tr??? kh??ch m???i - quay l???i'
      },
      legend: {
        position: 'bottom',
        display: true
      },
      tooltip: {
        mode: 'index'
      }
      // tooltip: {
      //   enabled: true,
      //   callbacks: {
      //     label: function (tooltipItem: any, data: ChartData) {
      //       return data.datasets[tooltipItem.datasetIndex].label + ': ' + tooltipItem.xLabel.toLocaleString();
      //     },
      //   }
      // }
    },
    scales: {
      x: {
        ticks: {
          callback: function (value: number, index, values) {
            return Intl.NumberFormat().format(value);
          }
        }
      }
    },

  }

  barChartLabels_PartnerRevenue: string[] = [];
  barChartData_PartnerRevenue: ChartDataset[] = [
    {
      label: 'Kh??ch m???i',
      data: [],
      backgroundColor: '#2395FF',
      hoverBackgroundColor: 'rgba(35,149,255,0.8)'
    },
    {
      label: 'Kh??ch quay l???i',
      data: [],
      backgroundColor: '#28A745',
      hoverBackgroundColor: 'rgba(40,167,69,0.8)'
    }
  ]

  constructor(private http: HttpClient,
    private companyService: CompanyService,
    private accountInvoiceReportService: AccountInvoiceReportService,
    private intlService: IntlService,
    private partnerOldNewRpService: PartnerOldNewReportService
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadSourceCities();
    this.loadCompanies();
    this.loadCurrentCompany();
  }

  ngAfterViewInit() {
  }

  loadDataFromApi() {
    var val = new RevenueDistrictAreaPar();
    var colors = this.pieChartColors;
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.cityCode = this.currentCity ? this.currentCity.code : null;
    val.companyId = this.currentCompany || null;
    this.accountInvoiceReportService.getRevenueDistrictArea(val).subscribe(result => {
      var top10Data = result.slice(0, 10);
      var otherData = result.slice(10);
      
      this.pieLabels = top10Data.map(x => {
        if (x.districtName) {
          return x.districtName;
        }
        return 'Ch??a x??c ?????nh';
      });

      var revenueData = top10Data.map(x => x.revenue);
      if (otherData.length) {
        this.pieLabels.push('Kh??c');
        var otherTotalRevenue = otherData.map(x => x.revenue).reduce((a, b) => a + b);
        revenueData.push(otherTotalRevenue);
      }

      this.pieDatasets = [{
        // backgroundColor: this.pieChartColors.slice(0, result.length),
        data: revenueData
      }];

      this.pieObjData = result;
      this.pieObjData.forEach(function (item, index) {
        item.color = colors[index];
      });
      this.pieChartData = this.pieObjData.map(x => x.revenue);
      this.loadPieChart();

      if (this.pieObjData && this.pieObjData.length > 0) {
        this.currentDistrictCode = this.pieObjData[0];
        this.loadDataReportByWard(this.pieObjData[0]);
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

  loadPieChart() {
    
    // var ulHtml = '';
    // var leftLegendHtml = [];
    // var rightLegendHtml = [];
    // if (pieData.length == 0)
    //   ulHtml = '';
    // else if (pieData.length > 6) {
    //   var leftData = pieData.slice(0, 6);
    //   leftData.forEach(item => {
    //     var districtName = item.districtName ? item.districtName : 'Kh??ng x??c ?????nh';
    //     var html = '<li style="display:flex; padding-top: 3px; padding-bottom:3px"><span style="flex:1; min-width: 115px"><i class="fas fa-circle mr-2" style="color:' + item.color + '"></i>' + districtName + '</span>' +
    //       '<span style="padding-left:3px">(' + item.percent + ')</span>';
    //     leftLegendHtml.push(html);
    //   });
    //   var rightData = pieData.slice(6, 11);
    //   rightData.forEach(item => {
    //     var districtName = item.districtName ? item.districtName : 'Kh??ng x??c ?????nh';
    //     var html = '<li style="display:flex; padding-top: 3px; padding-bottom:3px"><span style="flex:1; min-width: 115px"><i class="fas fa-circle mr-2" style="color:' + item.color + '"></i>' + districtName + '</span>' +
    //       '<span style="padding-left:3px">(' + item.percent + ')</span>';
    //     rightLegendHtml.push(html);
    //   });
    //   ulHtml = '<ul style="list-style-type: none; margin-left: auto; margin-right:auto; padding:0">'
    //     + leftLegendHtml.join('') + '</ul>' +
    //     '<ul style="list-style-type: none; margin-left: auto; margin-right:auto; padding:0">'
    //     + rightLegendHtml.join('') + '</ul>';
    // }
    // else {
    //   pieData.forEach(item => {
    //     var districtName = item.districtName ? item.districtName : 'Kh??ng x??c ?????nh';
    //     var html = '<li style="display:flex; padding-top: 3px; padding-bottom:3px"><span style="flex:1; min-width: 115px"><i class="fas fa-circle mr-2" style="color:' + item.color + '"></i>' + districtName + '</span>' +
    //       '<span style="padding-left:3px">(' + item.percent + ')</span>';
    //     leftLegendHtml.push(html);
    //   });
    //   ulHtml = '<ul style="list-style-type: none; margin-left: auto; margin-right:auto; padding:0">' + leftLegendHtml.join('') +
    //     '</ul>';
    // }

    // let legend = this._elementRef.nativeElement.querySelector(`#pie-chart-legend`);
    // legend.innerHTML = ulHtml;
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

  loadCurrentCompany() {
    var companyId = JSON.parse(localStorage.getItem('user_info')).companyId;
    this.currentCompany = companyId;
    this.companyService.get(companyId).subscribe(result => {
      if (result.city) {
        this.currentCity = result.city;
        this.loadDataFromApi();
      }
    });
  }

  filterCombobox() {
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

  onSelectCompany(event) {
    this.currentDistrictCode = null;
    if (event) {
      let companyId = event;
      this.companyService.get(companyId).subscribe(result => {
        this.currentCity = { code: result.city.code, name: result.city.name };
        this.loadDataFromApi();
      });

    }
    else {
      this.currentCity = null;
      this.loadDataFromApi();
    }
  }

  onSelectCity(event) {
    this.currentDistrictCode = null;
    this.loadDataFromApi();
  }

  onSelectDistrict(event) {
    this.currentDistrictCode = event;
    this.loadDataReportByWard(event);
  }

  onSearchDateChange(event) {
    this.currentDistrictCode = null;
    this.dateFrom = event.dateFrom;
    this.dateTo = event.dateTo;
    this.loadDataFromApi();
  }

  loadDataReportByWard(districtData) {
    var val = new PartnerOldNewReportByWardReq();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    val.cityCode = districtData.cityCode;
    val.districtCode = districtData.districtCode;
    val.companyId = this.currentCompany || '';

    this.partnerOldNewRpService.reportByWard(val).subscribe((res: any) => {
      this.barChartLabels_PartnerCount = res.map(x => {
        if (x.wardName)
          return x.wardName;
        return 'Ch??a x??c ?????nh';
      });
      this.barChartData_PartnerCount[0].data = res.map(x => x.partnerNewCount);
      this.barChartData_PartnerCount[1].data = res.map(x => x.partnerOldCount);
      this.barChartLabels_PartnerRevenue = res.map(x => {
        if (x.wardName)
          return x.wardName;
        return 'Ch??a x??c ?????nh';
      });
      this.barChartData_PartnerRevenue[0].data = res.map(x => x.partnerNewRevenue);
      this.barChartData_PartnerRevenue[1].data = res.map(x => x.partnerOldRevenue);
    }, err => {
      console.log(err);
    })
  }

  getMaxHeightPieChart() {
    return this.contPieChart.nativeElement.offsetHeight;
  }
}

