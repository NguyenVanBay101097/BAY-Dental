import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { GetRevenueSumTotalReq, SaleOrderReportRevenuePaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { saveAs } from '@progress/kendo-file-saver';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PrintService } from 'src/app/shared/services/print.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

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
  pagerSettings: any;

  sumRevenue = {
    amountTotal : 0,
    amountPaid : 0,
    residual : 0
  };
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;

  constructor(
    private companyService: CompanyService,
    private saleOrderService: SaleOrderService,
    private printService: PrintService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.initFilterData();

    this.loadCompanies();
    this.loadReport();
    this.searchChange();

    this.getRevenueSumTotal();
    this.loadCompanies();
    this.FilterCombobox();
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
      .subscribe((x) => {
        this.companies = x.items;
        this.companyVC.loading = false;
      });
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
   return  this.companyService.getPaged(val);
  } 

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }
  
  onSelectCompany(e){
    this.filter.companyId = e? e.id : null;
    this.loadReport();
    this.getRevenueSumTotal();
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


  initFilterData() {
    this.filter.companyId = '';
    this.filter.limit = 20;
    this.filter.offset = 0;
  }

  loadReport() {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId || val.companyId;
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
    this.filter.limit = e.take;
    this.loadReport();
  }

  public allData = (): any => {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId || val.companyId;
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

  exportExcel() {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId || val.companyId;
    this.saleOrderService.exportRevenueReportExcel(val).subscribe((rs) => {
      let filename = "BaoCaoDuKienThu";
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

  getRevenueSumTotal() {
    var val = new GetRevenueSumTotalReq();
    val.companyId = this.filter.companyId || '';
    this.saleOrderService.getRevenueSumTotal(val).subscribe((res:any) => {
      this.sumRevenue = res;
    }
    );
  }

  public onExcelExport(args: any): void {
    // Prevent automatically saving the file. We will save it manually after we fetch and add the details
    const workbook = args.workbook;
    var sheet = workbook.sheets[0];
    var rows = sheet.rows;
    sheet.name = 'BaoCaoDuKienThu';
    sheet.rows.splice(0, 0, { cells: [{
      value:"BÁO CÁO DỰ KIẾN THU",
      textAlign: "center"
    }], type: 'header' });
    sheet.mergedCells = ["A1:E1"];
    sheet.frozenRows = 2;
    
    args.preventDefault();
    this.loading = true;
    

    rows.forEach((row, index) => {
      //làm màu
      if (row.type != "header") {
        row.cells[2].textAlign = 'right';
        row.cells[3].textAlign = 'right';
        row.cells[4].textAlign = 'right';
        row.cells[2].format = '#,##0';
        row.cells[3].format = '#,##0';
        row.cells[4].format = '#,##0';
      }
      else {
        if (index != 0){
          row.cells.forEach((cell,index) => {
            cell.background = "#aabbcc";
            cell.color = "#000000";
          });
        }
        if (index == 1){
          row.cells[2].textAlign = 'right';
          row.cells[3].textAlign = 'right';
          row.cells[4].textAlign = 'right';
        }
       
      }
    });

    new Workbook(workbook).toDataURL().then((dataUrl: string) => {
      // https://www.telerik.com/kendo-angular-ui/components/filesaver/
      saveAs(dataUrl, 'Dự kiến doanh thu.xlsx');
      this.loading = false;
    });

  }

  printReport(){
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId || val.companyId;
    this.saleOrderService.getPrintRevenueReport(val).subscribe(result => {
      this.printService.printHtml(result);
    })
    
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter);
    val.companyId = val.companyId || val.companyId;
    this.loading = true;
    this.saleOrderService.getRevenueReportPdf(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaoDuKienDoanhThu";

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
