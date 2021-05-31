import { Component, OnInit } from '@angular/core';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { SaleOrderReportRevenuePaged, SaleOrderService } from 'src/app/core/services/sale-order.service';

@Component({
  selector: 'app-sale-order-report-revenue',
  templateUrl: './sale-order-report-revenue.component.html',
  styleUrls: ['./sale-order-report-revenue.component.css']
})
export class SaleOrderReportRevenueComponent implements OnInit {

  loading = false;
  filter = new SaleOrderReportRevenuePaged();
  gridData: GridDataResult;
  companies: CompanySimple[] = [];
  allDataReport: any;
  searchUpdate = new Subject<string>();

  constructor(
    private companyService: CompanyService,
    private saleOrderService: SaleOrderService
  ) { }

  ngOnInit() {
    this.initFilterData();

    this.loadCompanies();
    this.loadReport();
    this.searchChange();
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.filter.offset = 0;
        this.loadReport();
      });
  }


  loadCompanies() {
    var val = new CompanyPaged();
    val.active = true;
    this.companyService.getPaged(val).subscribe(res => {
      this.companies = res.items;
    });
  }

  initFilterData() {
    this.filter.companyId = 'all';
    this.filter.limit = 20;
    this.filter.offset = 0;
  }

  loadReport() {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    this.loading = true;
    this.saleOrderService.getRevenueReport(val).pipe(
      map(res => {
        return <DataResult>{
          data: res.items,
          total: res.totalItems
        }
      })
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    },
      err => {
        this.loading = false;
      });
  }

  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadReport();
  }

  public allData = (): any => {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId == 'all' ? '' : val.companyId;
    val.limit = 0;
    val.search = '';

    const observable = this.saleOrderService.getRevenueReport(val).pipe(
      map(res => {
        res.items.forEach((acc: any) => {
          acc.amountTotal = acc.amountTotal? acc.amountTotal.toLocaleString('vi') as any : 0;
          acc.totalPaid =  acc.totalPaid? acc.totalPaid.toLocaleString('vi') as any : 0;
          acc.residual = acc.residual? acc.residual.toLocaleString('vi') as any : 0;
        });
        return {
          data: res.items,
          total: res.totalItems
        }
      })
    );

    observable.pipe(
    ).subscribe((result) => {
      this.allDataReport = result;
    });

    return observable;

  }

  exportExcel(grid: GridComponent) {
    grid.saveAsExcel();
  }

  sumTotalAmount() {
    if(!this.gridData)  return 0;
    return this.gridData.data.reduce((total,el)=>{
      return total + (el.amountTotal || 0);
    }, 0);
  }

  sumTotalPaid(){
    if(!this.gridData)  return 0;
    return this.gridData.data.reduce((total,el)=>{
      return total + (el.totalPaid || 0);
    }, 0);
  }

  sumTotalResidual(){
    if(!this.gridData)  return 0;
    return this.gridData.data.reduce((total,el)=>{
      return total + (el.residual || 0);
    }, 0);
  }

}
