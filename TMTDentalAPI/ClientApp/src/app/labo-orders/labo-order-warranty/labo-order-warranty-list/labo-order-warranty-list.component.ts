import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import * as moment from 'moment';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { saveAs } from "@progress/kendo-file-saver";
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { WarrantyCuDidalogComponent } from 'src/app/shared/warranty-cu-didalog/warranty-cu-didalog.component';
import { LaboOrderService } from '../../labo-order.service';
import { LaboWarrantyPaged, LaboWarrantyService } from '../../labo-warranty.service';
import { LaboOrderWarrantyConfirmDialogComponent } from '../labo-order-warranty-confirm-dialog/labo-order-warranty-confirm-dialog.component';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import * as _ from 'lodash';
import { IntlService } from '@progress/kendo-angular-intl';

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
  supplierData: any[] = [];
  supplierId: string;
  dateFrom: Date;
  dateTo: Date;
  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Mới', value: 'new' },
    { text: 'Đã gửi', value: 'sent' },
    { text: 'Đã nhận', value: 'received' },
    { text: 'Đã lắp', value: 'assembled' }
  ];
  constructor(
    private laboWarrantyService: LaboWarrantyService,
    private modalService: NgbModal,
    private laboOrderService: LaboOrderService,
    private partnerService: PartnerService,
    private intlService: IntlService,

  ) { }

  ngOnInit() {
    this.loadSupplier();
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadLaboOrder(laboOrderId) {
    this.laboOrderService.get(laboOrderId).subscribe(result => {
      this.laboOrder = result;
    });
  }

  loadSupplier() {
    this.searchSupplier().subscribe(result => {
      this.supplierData = _.unionBy(this.supplierData, result, 'id');
    });
  }

  searchSupplier(search?: string) {
    var val = new PartnerPaged();
    val.offset = 0;
    this.limit = 1000;
    val.search = search || '';
    val.supplier = true;
    val.active = true;
    return this.partnerService.getAutocompleteSimple(val);
  }

  loadDataFromApi() {
    let val = new LaboWarrantyPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.supplierId = this.supplierId || '';
    val.states = this.state || '';
    val.laboOrderId = '';
    val.notDraft = true;
    val.dateReceiptFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateReceiptTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
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

  editItem(item) {
    const modalRef = this.modalService.open(LaboOrderWarrantyConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.state = item.state;
    modalRef.componentInstance.laboWarrantyId = item.id;
    modalRef.componentInstance.dateSendWarranty = item.dateSendWarranty;
    modalRef.componentInstance.dateReceiptInspection = item.dateReceiptInspection;
    modalRef.componentInstance.dateAssemblyWarranty = item.dateAssemblyWarranty;

    modalRef.result.then((res) => {
      this.loadDataFromApi()
    }, (err) => { console.log(err) });
  }

  editWarranty(item) {
    const modalRef = this.modalService.open(WarrantyCuDidalogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.laboOrderId = item.laboOrderId;
    modalRef.componentInstance.laboWarrantyId = item.id;

    modalRef.result.then((res) => {
      this.loadDataFromApi()
    }, (err) => { console.log(err) });
  }

  editLabo(item) {
    this.loadLaboOrder(item.laboOrderId)
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật phiếu labo';
    modalRef.componentInstance.id = item.laboOrderId;

    modalRef.result.then(res => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onChangeState(event) {
    this.state = event ? event.value : null;
    this.skip = 0
    this.loadDataFromApi();
  }

  supplierChange(event) {
    this.supplierId = event;
    this.skip = 0;
    this.loadDataFromApi();
  }

  onSearchChange(data){
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  public onExcelExport(args: any): void {
    // Prevent automatically saving the file. We will save it manually after we fetch and add the details
    const workbook = args.workbook;
    var sheet = workbook.sheets[0];
    var rows = sheet.rows;
    var columns = sheet.columns;
    columns.splice(9, 1, {});

    sheet.name = 'QuanLyBaoHanh';
    sheet.rows.splice(0, 0, {
      cells: [{
        value: "QUẢN LÝ BẢO HÀNH",
        textAlign: "center"
      }], type: 'header'
    });
    sheet.rows.splice(1, 0, {
      cells: [{
        value: `Từ ngày ${this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'dd/MM/yyyy') : '...'} đến ngày ${this.dateTo ? this.intlService.formatDate(this.dateTo, 'dd/MM/yyyy') : '...'}`,
        textAlign: "center"
      }], type: 'header'
    });
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
