import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import * as moment from 'moment';
import { Subject } from 'rxjs/internal/Subject';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { SaleOrderLinePaged } from 'src/app/partners/partner.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { DateRangePickerFilterComponent } from 'src/app/shared/date-range-picker-filter/date-range-picker-filter.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';


@Component({
  selector: 'app-service-sale-report',
  templateUrl: './service-sale-report.component.html',
  styleUrls: ['./service-sale-report.component.css']
})
export class ServiceSaleReportComponent implements OnInit {
  filter = new SaleOrderLinePaged();
  companies: CompanySimple[] = [];
  employees: EmployeeSimple[] = [];
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  searchUpdate = new Subject<string>();
  filterMonth: any = "";
  search: string;
  dateFrom: Date;
  dateTo: Date;
  state: string;
  companyId: string;
  employeeId: string;
  stateDisplay= {
    sale:"Đang điều trị",
    done: "Hoàn thành"
  }

  ranges: any = {
    'Tháng này': [moment().startOf('month'), moment().endOf('month')],
    '1-3 tháng trước': [moment().subtract(3, 'months'), moment().subtract(1, 'months')],
    '3-6 tháng trước': [moment().subtract(6, 'months'), moment().subtract(3, 'months')],
    '6-12 tháng trước': [moment().subtract(12, 'months'),  moment().subtract(6, 'months')],
  }

  isDisabledDoctors = true;
  filteredServices: ProductSimple[] = [];
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("empCbx", { static: true }) empVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  @ViewChild(DateRangePickerFilterComponent, {static: true}) dateRangeComp: DateRangePickerFilterComponent;
  @ViewChild('serviceMultiSelect', { static: true }) serviceMultiSelect: MultiSelectComponent
  constructor(
    private saleOrderLineService: SaleOrderLineService,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
    private intlService: IntlService,
    private printService:PrintService,
    private productService: ProductService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.initFilter();
    this.initFilterData();
    this.loadCompanies();
    this.loadEmployees();
    this.FilterCombobox();
    this.loadAllData();
    this.filterChangeMultiselect();
    this.loadService();
  }

  initFilter() {
    this.filter.limit = 20;
    this.filter.offset = 0;

    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateOrderFrom = this.dateFrom || new Date(y, m, 1);
    this.filter.dateOrderTo = this.dateTo || new Date(y, m + 1, 0);
  }
  
  loadAllData() {
    var val = this.getDataApiParam();
    this.loading = true;
    this.saleOrderLineService.getPaged(val).subscribe(res => {
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      }
      this.loading = false;
    },
      err => {
        this.loading = false;
      });
  }

  getDataApiParam() {
    var val = new SaleOrderLinePaged();
    val.limit = this.filter.limit;
    val.offset = this.filter.offset;
    val.search = this.filter.search || '';
    val.partnerName = this.filter.partnerName || '';
    val.companyId = this.companyId || '';
    val.employeeId = this.employeeId || '';
    val.dateOrderFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateOrderTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    val.state = 'sale';
    val.serviceIds = this.filter.serviceIds || [];
    return val;
  }

  FilterCombobox() {
    this.companyVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyVC.loading = true)),
        switchMap((value) => this.searchCompany$(value)
        )
      )
      .subscribe((x: any) => {
        this.companies = x.items;
        this.companyVC.loading = false;
      });

      this.empVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.empVC.loading = true)),
        switchMap((value) => this.searchEmployee$(value)
        )
      )
      .subscribe((x: any) => {
        this.employees = x;
        this.empVC.loading = false;
      });
  }

  initFilterData() {
    this.searchUpdate.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(r=> {
      this.filter.offset = 0;
      this.loadAllData();
    })
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

  searchEmployee$(q?: string) {
    var val = new EmployeePaged();
    val.search = q;
    val.isDoctor = true;
    val.companyId = this.filter.companyId || '';
    return this.employeeService.getEmployeeSimpleList(val);
  }

  loadEmployees() {
    this.searchEmployee$().subscribe((res: any) => {
      this.employees = res;
    });
  }

  onSearchDateChange(e) {
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    this.filter.offset = 0;
    this.loadAllData();
  }

  onSelectEmployee(event) {
    this.skip = 0;
    this.loadAllData();
  }

  onSelectCompany(company) {
    this.filter.employeeId = null;
    if (company) {
      this.isDisabledDoctors = false;
      this.loadEmployees();
    } else {
      this.isDisabledDoctors = true;
      this.employees = [];
    }

    this.skip = 0;
    this.loadAllData();
  }

  pageChange(event: PageChangeEvent): void {
    this.filter.limit = event.take;
    this.filter.offset = event.skip;
    this.loadAllData();
  }

  showTeethList(toothType, teeth) {
    //dựa vào this.line
    switch (toothType) {
      case 'whole_jaw':
        return 'Nguyên hàm';
      case 'upper_jaw':
        return 'Hàm trên';
      case 'lower_jaw':
        return 'Hàm dưới';
      default:
        return teeth.map(x => x.name).join(', ');
    }
  }

  onExportExcel() {
    var val = this.getDataApiParam();
    this.loading = true;
    this.saleOrderLineService.getSaleReportExportExcel(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaoDichVu_DangDieuTri";

      let newBlob = new Blob([res], {
        type:
        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
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

  onExportPDF() {
    var val = this.getDataApiParam();
    this.loading = true;
    this.saleOrderLineService.getSaleReportExportPdf(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaoDichVu_DangDieuTri";

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


  onPrint(){
    var val = this.getDataApiParam();
    this.loading = true;
      this.saleOrderLineService.SaleReportPrint(val).subscribe((result: any) => {
        this.loading = false;
        this.printService.printHtml(result);
      });
  }

  filterChangeMultiselect() {
    this.serviceMultiSelect.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.serviceMultiSelect.loading = true)),
        switchMap((value) => this.searchService(value))
      )
      .subscribe((result) => {
        this.filteredServices = result;
        this.serviceMultiSelect.loading = false;
      });
  }
  
  searchService(q?: string) {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0
    val.search = q || '';
    val.type = "service";
    val.type2 = "service";
    return this.productService.autocomplete2(val);
  }

  loadService() {
    this.searchService().subscribe(
      result => {
        this.filteredServices = result;
      }
    )
  }

  onChangeServiceSelected(val){
    this.skip = 0;
    this.loadAllData();
  }

}
