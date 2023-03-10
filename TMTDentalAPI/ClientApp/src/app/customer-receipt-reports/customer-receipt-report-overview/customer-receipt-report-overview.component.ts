import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { ChartDataset, ChartOptions } from 'chart.js';
import * as pluginDataLabels from 'chartjs-plugin-datalabels';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { CustomerReceiptReportFilter, CustomerReceiptReportService } from '../customer-receipt-report.service';

@Component({
  selector: 'app-customer-receipt-report-overview',
  templateUrl: './customer-receipt-report-overview.component.html',
  styleUrls: ['./customer-receipt-report-overview.component.css']
})
export class CustomerReceiptReportOverviewComponent implements OnInit {
  loading = false;
  limit = 10;
  skip = 0;
  pagerSettings: any;
  total: number;
  gridData: GridDataResult;
  listCompany: CompanySimple[] = [];
  listEmployee: EmployeeSimple[] = [];
  searchUpdate = new Subject<string>();
  search: string;
  dateFrom: Date;
  dateTo: Date;
  state: string;
  public today: Date = new Date(new Date().toDateString());
  isExamination: string;
  isNotTreatment: string;
  companyId: string;
  employeeId: string;
  isAdvanced: boolean;
  public isCollapsed = false;
  public animateChart = true;

  // Pie
  public pieDataExamination: any[] = [];
  public pieDataNoTreatment: any[] = [];

  public pieChartLabels: string[] = [];
  public pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'right'
      },
      datalabels: {
        display: true,
        anchor: 'end',
        align: 'start',
        color: '#fff'
      },
    }
  };

  pieChartPlugins = [pluginDataLabels];

  public pieChartData: ChartDataset[] = [
    { 
      data: [],
      backgroundColor: [ 'rgb(26,109,227)', 'rgb(149,200,255)' ],
      hoverBackgroundColor: [ 'rgb(26,109,227)', 'rgb(149,200,255)' ],
      hoverBorderColor: [ 'rgb(26,109,227)', 'rgb(149,200,255)' ],
    }
  ];

  noTreatmentChartLabels: string[] = [];
  noTreatmentChartData: ChartDataset[] = [
    { 
      data: [],
      backgroundColor: [ 'rgb(26,109,227)', 'rgb(149,200,255)' ],
      hoverBackgroundColor: [ 'rgb(26,109,227)', 'rgb(149,200,255)' ],
      hoverBorderColor: [ 'rgb(26,109,227)', 'rgb(149,200,255)' ],
    }
  ];

  @ViewChild("companyCbx", { static: true }) companyCbx: ComboBoxComponent;
  @ViewChild("employeeCbx", { static: true }) employeeCbx: ComboBoxComponent;

  filterExamination: any[] = [
    { value: 'true', text: 'T??i kh??m' },
    { value: 'false', text: 'Kh??m m???i' },
  ];

  filterNoTreatment: any[] = [
    { value: 'false', text: 'C?? ??i???u tr???' },
    { value: 'true', text: 'Kh??ng ??i???u tr???' },
  ];

  filterState: any[] = [
    { value: 'waiting', text: 'Ch??? kh??m' },
    { value: 'examination', text: '??ang kh??m' },
    { value: 'done', text: 'Ho??n th??nh' },
  ];

  constructor(
    private customerReceiptReportService: CustomerReceiptReportService,
    private intlService: IntlService,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.dateFrom = this.today;
    this.dateTo = this.today;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataApi();
      });

    this.loadCompanies();
    this.loadEmployees();

    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.companyCbx.loading = true)),
      switchMap((value) => this.searchCompany(value)
      )
    )
      .subscribe((x) => {
        this.listCompany = x.items;
        this.companyCbx.loading = false;
      });

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap((value) => this.searchEmployee(value)
      )
    )
      .subscribe((rs: any[]) => {
        this.listEmployee = rs;
        this.employeeCbx.loading = false;
      });


    this.loadDataExamination();
    this.loadDataNotreatment();
    this.loadDataApi();

  }

  getDataApiParam() {
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isRepeatCustomer = this.isExamination || '';
    val.isNoTreatment = this.isNotTreatment || '';
    val.companyId = this.companyId || '';
    val.doctorId = this.employeeId || '';
    val.state = this.state || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    return val;
  }
  loadDataApi() {
    this.loading = true;
    var val = this.getDataApiParam();
    this.customerReceiptReportService.getPaged(val).pipe(
      map((response) => <GridDataResult>{
        data: response.items,
        total: response.totalItems,
      }
      )
    ).subscribe(
      (res) => {
        this.gridData = res;
        this.total = res.total;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadDataExamination() {
    this.loading = true;
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isRepeatCustomer = this.isExamination || '';
    val.isNoTreatment = this.isNotTreatment || '';
    val.companyId = this.companyId || '';
    val.doctorId = this.employeeId || '';
    val.state = this.state || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.getCountCustomerReceipt(val).subscribe(
      (res: any[]) => {
        this.pieDataExamination = res;
        this.pieChartData[0].data = res.map(x => x.countCustomerReceipt);
        this.pieChartLabels = res.map(x => x.name);
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadDataNotreatment() {
    this.loading = true;
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isRepeatCustomer = this.isExamination || '';
    val.isNoTreatment = this.isNotTreatment || '';
    val.companyId = this.companyId || '';
    val.doctorId = this.employeeId || '';
    val.state = 'done';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.getCountCustomerReceiptNoTreatment(val).subscribe(
      (res: any[]) => {
        this.pieDataNoTreatment = res;
        this.noTreatmentChartData[0].data = res.map(x => x.countCustomerReceipt);
        this.noTreatmentChartLabels = res.map(x => x.name);
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadExaminationItems(items: any[]): void {
    for (let i = 0; i < items.length; i++) {
      this.pieDataExamination.push({ category: items[i].name, value: items[i].countCustomerReceipt, percentage: (items[i].countCustomerReceipt / items[i].totalCustomerReceipt * 100).toFixed(2) })
    };
  }

  loadNoTreatmentItems(items: any[]): void {
    for (let i = 0; i < items.length; i++) {
      this.pieDataNoTreatment.push({ category: items[i].name, value: items[i].countCustomerReceipt, percentage: (items[i].countCustomerReceipt / items[i].totalCustomerReceipt * 100).toFixed(2) })
    };
  }

  public labelContent(e: any): string {
    return e.percentage;
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataApi();
    this.loadDataExamination();
    this.loadDataNotreatment();
  }

  onChangeExamination(e) {
    var value = e ? e.value : null;
    if (value) {
      this.isExamination = value;
    } else {
      this.isExamination = null;
    }
    this.skip = 0;
    this.loadDataApi();
  }

  onChangeNotTreatment(e) {
    var value = e ? e.value : null;
    if (value) {
      this.isNotTreatment = value;
    } else {
      this.isNotTreatment = null;
    }
    this.skip = 0;
    this.loadDataApi();
  }

  onChangeState(e) {
    var value = e ? e.value : null;
    if (value) {
      this.state = value;
    } else {
      this.state = null;
    }
    this.skip = 0;
    this.loadDataApi();
    this.loadDataExamination();
    this.loadDataNotreatment();
  }

  onSelectCompany(e) {
    this.companyId = e ? e.id : null;
    this.skip = 0;
    this.loadDataApi();
    this.loadDataExamination();
    this.loadDataNotreatment();
  }

  onSelectEmployee(e) {
    this.employeeId = e ? e.id : null;
    this.skip = 0;
    this.loadDataApi();
    this.loadDataExamination();
    this.loadDataNotreatment();
  }

  loadEmployees() {
    this.searchEmployee().subscribe(res => {
      this.listEmployee = res;
    });
  }

  searchEmployee(search?) {
    var val = new EmployeePaged();
    val.search = search || '';
    val.active = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  searchCompany(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany().subscribe(res => {
      this.listCompany = res.items;
    });
  }


  onExcelExport() {
    var val = new CustomerReceiptReportFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isRepeatCustomer = this.isExamination || '';
    val.isNoTreatment = this.isNotTreatment || '';
    val.companyId = this.companyId || '';
    val.doctorId = this.employeeId || '';
    val.state = this.state || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.exportExcelReportOverview(val).subscribe((res: any) => {
      let filename = "BaoCaoTiepNhan";
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }

  getInfo(item) {
    return `${item.name}: ${item.countCustomerReceipt} (${(item.countCustomerReceipt / item.totalCustomerReceipt * 100).toFixed(2)}%)`;
  }



  getExamination(value) {
    switch (value) {
      case true:
        return 'T??i kh??m';
      case false:
        return 'Kh??m m???i';
    }
  }

  getMinute(value) {
    if(value == null || value == undefined){
      return '';
    }

    return `${value} ph??t`;
  }

  getState(value) {
    switch (value) {
      case 'examination':
        return '??ang kh??m';
      case 'done':
        return 'Ho??n th??nh';
      default:
        return 'Ch??? kh??m';
    }
  }

  getTreatment(value) {
    switch (value) {
      case true:
        return 'Kh??ng ??i???u tr???';
      case false:
        return 'C?? ??i???u tr???';
    }
  }

  onExportPDF() {
    var val = this.getDataApiParam();
    this.loading = true;
    this.customerReceiptReportService.reportPagedPdf(val).subscribe(res => {
      this.loading = false;
      let filename = "TiepNhanTongQuan";

      let newBlob = new Blob([res], {
        type:
          "application/pdf",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }


}
