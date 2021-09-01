import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { saveAs } from "@progress/kendo-file-saver";
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { WarrantyCuDidalogComponent } from 'src/app/shared/warranty-cu-didalog/warranty-cu-didalog.component';
import { LaboOrderService } from '../../labo-order.service';
import { LaboWarrantyPaged, LaboWarrantyService } from '../../labo-warranty.service';

@Component({
  selector: 'app-labo-order-warranty-list',
  templateUrl: './labo-order-warranty-list.component.html',
  styleUrls: ['./labo-order-warranty-list.component.css']
})
export class LaboOrderWarrantyListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string = '';
  state: string = '';
  searchUpdate = new Subject<string>();
  laboOrder: any;
  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Mới', value: 'new' },
    { text: 'Đã gửi', value: 'sent' },
    { text: 'Đã nhận', value: 'received' },
    { text: 'Đã Lắp', value: 'assembled' }
  ];
  constructor(
    private laboWarrantyService: LaboWarrantyService,
    private modalService: NgbModal,
    private laboOrderService: LaboOrderService,

  ) { }

  ngOnInit() {
    this.loadDataFromApi()
  }

  loadLaboOrder(laboOrderId) {
    this.laboOrderService.get(laboOrderId).subscribe(result => {
      this.laboOrder = result;
      console.log(this.laboOrder);
      
      // this.patchValue(result);
      // this.processTeeth(result.saleOrderLine.teeth);
    });
  }

  loadDataFromApi() {
    let val = new LaboWarrantyPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.supplierId = '';
    val.state = this.state || '';
    val.laboOrderId = '';
    val.dateReceiptFrom = '';
    val.dateReceiptTo = '';
    this.laboWarrantyService.getPaged(val).pipe(
      map(res => {
        return <DataResult>{
          data: res.items,
          total: res.totalItems
        }
      })
    ).subscribe(res => {
      this.gridData = res;
      console.log(this.gridData);

      this.loading = false;
    }, err => { this.loading = false; });
  }

  editItem() {

  }

  editWarranty(item) {
    const modalRef = this.modalService.open(WarrantyCuDidalogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    // modalRef.componentInstance.laboId = item.id;
    modalRef.componentInstance.laboWarrantyId = item.id;
    // modalRef.componentInstance.laboTeeth = item.teeth;

    modalRef.result.then((res) => {
      this.loadDataFromApi()
    }, (err) => { console.log(err) });
  }

  editLabo(item) {
    console.log(item);
    this.loadLaboOrder(item.laboOrderId)
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật phiếu labo';
    modalRef.componentInstance.id = item.laboOrderId;
    // modalRef.componentInstance.saleOrderLineId = item.saleOrderLineId;

    modalRef.result.then(res => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  exportExcelFile() {

  }

  public onExcelExport(args: any): void {
    // Prevent automatically saving the file. We will save it manually after we fetch and add the details
    const workbook = args.workbook;
    var sheet = workbook.sheets[0];
    var rows = sheet.rows;
    var columns = sheet.columns;
    columns.splice(9,1,{});
    
    sheet.name = 'QuanLyBaoHanh';
    sheet.rows.splice(0, 0, { cells: [{
      value:"QUẢN LÝ BẢO HÀNH",
      textAlign: "center"
    }], type: 'header' });
    sheet.rows.splice(1, 0, { cells: [{
     // value: `Từ ngày ${this.filter.dateFrom ? this.intlService.formatDate(this.filter.dateFrom, 'dd/MM/yyyy') : '...'} đến ngày ${this.filter.dateTo ? this.intlService.formatDate(this.filter.dateTo, 'dd/MM/yyyy') : '...'}`,
      textAlign: "center"
    }], type: 'header' });
   sheet.mergedCells = ["A2:C2"];
    sheet.frozenRows = 3;
   rows.forEach((row, index) => {
    if (row.type === "header" && index == 2) {
      row.cells.forEach(cell => {
        cell.background = "#fff";
        cell.color = "#000";
        cell.bold = true;
        cell.borderTop = { color: "black", size: 1 };
        cell.borderRight = { color: "black", size: 1 };
        cell.borderBottom = { color: "black", size: 1 };
        cell.borderLeft = { color: "black", size: 1 };
      });
    }  
    if (row.type === "data") {
          row.cells[0].value = moment(row.cells[0].value).format('DD/MM/YYYY');
          row.cells.forEach(cell => {
            cell.borderTop = { color: "black", size: 1 };
            cell.borderRight = { color: "black", size: 1 };
            cell.borderBottom = { color: "black", size: 1 };
            cell.borderLeft = { color: "black", size: 1 };
          });
          row.cells[8].value = this.showState(row.cells[8].value);
    }
  });


    args.preventDefault();
    this.loading = true;
    new Workbook(workbook).toDataURL().then((dataUrl: string) => {
      // https://www.telerik.com/kendo-angular-ui/components/filesaver/
      saveAs(dataUrl, "QuanLyBaoHanh.xlsx");
      this.loading = false;
    });
  }

  exportExcel(grid: GridComponent) {
    grid.saveAsExcel();
  }

  showState(state) {
    switch (state) {
      case 'new':
        return 'Mới'
      case 'sent':
        return 'Đã gửi'
      case 'received':
        return 'Đã nhận'
      case 'assembled':
        return 'Đã lắp'
      default:
        return 'Nháp'
    }
  }
}
