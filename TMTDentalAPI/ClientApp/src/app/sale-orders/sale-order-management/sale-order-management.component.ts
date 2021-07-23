import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import { saveAs } from '@progress/kendo-file-saver';
import * as moment from 'moment';
import { Subject, Observable, zip, of, forkJoin } from 'rxjs';
import { debounceTime, delay, distinctUntilChanged, map } from 'rxjs/operators';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderLinePaged } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-sale-order-management',
  templateUrl: './sale-order-management.component.html',
  styleUrls: ['./sale-order-management.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleOrderManagementComponent implements OnInit {

  search: string;
  searchUpdate = new Subject<string>();
  company: CompanyBasic;
  ranges: any = [
    { text: '> 1 tháng', intervalNbr: 1, interval: 'month' },
    { text: '> 3 tháng', intervalNbr: 3, interval: 'month' },
    { text: '> 6 tháng', intervalNbr: 6, interval: 'month' },
    { text: '> 12 tháng', intervalNbr: 12, interval: 'month' },
  ];
  dateOrderTo: any = this.ranges[0];
  companies: CompanyBasic[] = [];
  saleOrdersData: GridDataResult;
  loading = false;
  limit = 20;
  skip = 0;

  saleOrdersAllData: GridDataResult;

  constructor(
    private companyService: CompanyService,
    private saleOrderService: SaleOrderService,
    private router: Router,
    private saleOrderLineService: SaleOrderLineService
  ) { }

  ngOnInit() {
    this.loadCompanies();
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadCompanies() {
    var val = new CompanyPaged();
    val.active = true;
    val.limit = 1000;
    this.companyService.getPaged(val).subscribe(result => {
      this.companies = result.items;
    })
  }

  loadDataFromApi() {
    var val = new SaleOrderPaged();
    val.search = this.search ? this.search : '';
    val.companyId = this.company ? this.company.id : "";
    val.state = "sale";
    val.limit = this.limit;
    val.offset = this.skip;
    if (this.dateOrderTo) {
      val.overInterval = this.dateOrderTo.interval;
      val.overIntervalNbr = this.dateOrderTo.intervalNbr;
    }

    this.saleOrderService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.saleOrdersData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  handleFilterDateOrder() {
    this.skip = 0;
    this.loadDataFromApi();
  }

  handleFilterCompany() {
    this.skip = 0;
    this.loadDataFromApi();
  }

  getFormSaleOrder(id) {
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  public allData = (): any => {
    var val = new SaleOrderPaged();
    val.search = this.search ? this.search : '';
    val.companyId = this.company ? this.company.id : "";
    val.state = "sale";
    if (this.dateOrderTo) {
      val.overInterval = this.dateOrderTo.interval;
      val.overIntervalNbr = this.dateOrderTo.intervalNbr;
    }

    var observable = this.saleOrderService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    );

    observable.subscribe(res => {
      this.saleOrdersAllData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })

    return observable;
  };

  public onExcelExport(args: any): void {
    // Prevent automatically saving the file. We will save it manually after we fetch and add the details
    args.preventDefault();

    this.loading = true;

    const observables = [];
    const workbook = args.workbook;
    const rows = workbook.sheets[0].rows;

    for (var rowIndex = 1; rowIndex < rows.length; rowIndex++) {
      var row = rows[rowIndex];
      for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex ++) {
        if (cellIndex == 0) {
          row.cells[cellIndex].format = "dd/MM/yyyy HH:mm";
        } else if (cellIndex == 3 || cellIndex == 4 || cellIndex == 5) {
          row.cells[cellIndex].format = "#,##0";
        }
      }
    }

    // Get the default header styles.
    // Aternatively set custom styles for the details
    // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/WorkbookSheetRowCell/
    const headerOptions = rows[0].cells[0];
    const data = this.saleOrdersAllData.data;

    // Fetch the data for all details
    for (let idx = 0; idx < data.length; idx++) {
      var item = data[idx];
      var linePaged = new SaleOrderLinePaged();
      linePaged.orderId = item.id;
      observables.push(this.saleOrderLineService.getPaged(linePaged).pipe(
        map((response: any) => (<GridDataResult>{
          data: response.items,
          total: response.totalItems
        }))
      ));
    }

    forkJoin(observables).subscribe((result: any) => {
      // add the detail data to the generated master sheet rows
      // loop backwards in order to avoid changing the rows index
      for (let idx = result.length - 1; idx >= 0; idx--) {
        const products = (<any>result[idx]).data;

        // add the detail data
        for (
          let productIdx = products.length - 1;
          productIdx >= 0;
          productIdx--
        ) {
          const product = products[productIdx];
          rows.splice(idx + 2, 0, {
            cells: [
              {},
              { value: product.name },
              { value: product.productUOMQty, format: '#,##0' },
              { value: product.priceTotal, format: '#,##0' },
              { value: product.amountPaid, format: '#,##0' },
              { value: product.amountResidual, format: '#,##0' },
            ],
          });
        }

        // add the detail header
        rows.splice(idx + 2, 0, {
          cells: [
            {},
            Object.assign({}, headerOptions, { value: "Dịch vụ" }),
            Object.assign({}, headerOptions, { value: "Số lượng" }),
            Object.assign({}, headerOptions, { value: "Thành tiền" }),
            Object.assign({}, headerOptions, { value: "Thanh toán" }),
            Object.assign({}, headerOptions, { value: "Còn lại" }),
          ],
        });
      }

      // create a Workbook and save the generated data URL
      // https://www.telerik.com/kendo-angular-ui/components/excelexport/api/Workbook/
      new Workbook(workbook).toDataURL().then((dataUrl: string) => {
        // https://www.telerik.com/kendo-angular-ui/components/filesaver/
        saveAs(dataUrl, "dieu_tri_chua_hoan_thanh.xlsx");
        this.loading = false;
      });
    });
  }

  exportExcelFile(grid: GridComponent) {
    grid.saveAsExcel();
    // var paged = new SaleOrderPaged();
    // paged.search = this.search || '';
    // paged.companyId = this.company ? this.company.id : "";
    // paged.state = "sale";
    // if (this.dateOrderTo) {
    //   paged.overInterval = this.dateOrderTo.interval;
    //   paged.overIntervalNbr = this.dateOrderTo.intervalNbr;
    // }

    // this.saleOrderService.exportExcelFile(paged).subscribe((res) => {
    //   let filename = "Quản lý điều trị chưa hoàn thành";

    //   let newBlob = new Blob([res], {
    //     type:
    //       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //   });

    //   let data = window.URL.createObjectURL(newBlob);
    //   let link = document.createElement("a");
    //   link.href = data;
    //   link.download = filename;
    //   link.click();
    //   setTimeout(() => {
    //     // For Firefox it is necessary to delay revoking the ObjectURL
    //     window.URL.revokeObjectURL(data);
    //   }, 100);
    // });
  }

}
