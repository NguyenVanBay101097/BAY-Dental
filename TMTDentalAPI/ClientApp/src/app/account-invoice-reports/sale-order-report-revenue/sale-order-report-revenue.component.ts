import { Component, OnInit } from '@angular/core';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { SaleOrderReportRevenuePaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { saveAs } from '@progress/kendo-file-saver';

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

  amountTotal = 0;
   residual= 0;
  amountPaid = 0;

  constructor(
    private companyService: CompanyService,
    private saleOrderService: SaleOrderService
  ) { }

  ngOnInit() {
    this.initFilterData();

    this.loadCompanies();
    this.loadReport();
    this.searchChange();

    this.sumTotalAmount();
    this.sumTotalPaid();
    this.sumTotalResidual();
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
          acc.amountTotal = (acc.amountTotal || 0).toLocaleString('vi') as any;
          acc.totalPaid = (acc.totalPaid || 0).toLocaleString('vi') as any;
          acc.residual = (acc.residual || 0).toLocaleString('vi') as any;
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
    this.saleOrderService.getSumTotal({ column: 'AmountTotal' }).subscribe((res:any) => {
      this.amountTotal = res;
    }
    );
  }

  sumTotalPaid() {
    this.saleOrderService.getSumTotal({ column: 'TotalPaid' }).subscribe((res:any) => {
      this.amountPaid = res;
    }
    );
  }

  sumTotalResidual() {
    this.saleOrderService.getSumTotal({ column: 'Residual' }).subscribe((res:any) => {
      this.residual = res;
    }
    );
  }

  public onExcelExport(args: any): void {
    // Prevent automatically saving the file. We will save it manually after we fetch and add the details
    args.preventDefault();
    this.loading = true;
    const workbook = args.workbook;

    const rows = workbook.sheets[0].rows;

    rows.forEach((row, index) => {
      //làm màu
      if (row.type != "header") {
        row.cells[2].textAlign = 'right';
        row.cells[3].textAlign = 'right';
        row.cells[4].textAlign = 'right';
      }
    });

    new Workbook(workbook).toDataURL().then((dataUrl: string) => {
      // https://www.telerik.com/kendo-angular-ui/components/filesaver/
      saveAs(dataUrl, 'baocaodukienthu.xlsx');
      this.loading = false;
    });

  }

}
