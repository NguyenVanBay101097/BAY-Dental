import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { DataBindingDirective, DataStateChangeEvent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { process, State } from '@progress/kendo-data-query';
import { map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleReportSearch, SaleReportService } from 'src/app/sale-report/sale-report.service';

@Component({
  selector: 'app-dashboard-service-today-report',
  templateUrl: './dashboard-service-today-report.component.html',
  styleUrls: ['./dashboard-service-today-report.component.css']
})
export class DashboardServiceTodayReportComponent implements OnInit {

  @ViewChild(DataBindingDirective, { static: true }) dataBinding: DataBindingDirective;
  
  public gridData: any[];
  public gridView: any[];

  search: string = '';
  totalService: number;
  limit = 20;
  offset = 0;
  state: State = {
    skip: this.offset,
    take: this.limit,
    // Initial filter descriptor
    filter: {
      logic: 'and',
      filters: [{ field: 'name', operator: 'contains', value: '' }]
    }
  };

  constructor(private intlService: IntlService, 
    private authService: AuthService, 
    private saleReportService: SaleReportService, 
    private router: Router
  ) { }

  ngOnInit() {
    this.gridView = this.gridData;
    this.loadService();
  }

  loadService() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    val.companyId = this.authService.userInfo.companyId;
    val.search = this.search;
    val.state = 'draft';
    this.saleReportService.getReportService(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridView = res.data;
      this.gridData = res.data;
      this.totalService = res.total;
    }, err => {
      console.log(err);
    })
  }

  exportServiceExcelFile() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    val.companyId = this.authService.userInfo.companyId;
    val.search = this.search;
    val.state = 'draft';
    // paged.categoryId = this.searchCateg ? this.searchCateg.id : null;
    this.saleReportService.exportServiceReportExcelFile(val).subscribe((rs) => {
      let filename = "danh_sach_dich_vu_trong_ngay";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
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

  dataStateChange(state: DataStateChangeEvent): void {
    this.state = state;
    this.loadService();
  }

  onFilter(inputValue: string): void {
    this.gridView = process(this.gridData, {
      filter: {
        logic: "or",
        filters: [
          {
            field: 'order.name',
            operator: 'contains',
            value: inputValue
          },
          {
            field: 'orderPartner.name',
            operator: 'contains',
            value: inputValue
          },
          {
            field: 'employee.name',
            operator: 'contains',
            value: inputValue
          },
          {
            field: 'name',
            operator: 'contains',
            value: inputValue
          }
        ],
      }
    }).data;

    this.dataBinding.skip = 0;
  }

  pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.loadService();
  }

  redirectSaleOrder(item) {
    if (item) {
      this.router.navigateByUrl(`sale-orders/form?id=${item.id}`)
    }
  }

  getTeethDisplay(teeth) {
    return teeth.map(x => x.name).join(',');
  }

  getStateDisplay(state) {
    switch (state) {
      case 'sale':
        return 'Đang điều trị';
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Nháp';
    }
  }
}
