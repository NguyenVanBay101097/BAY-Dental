import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridComponent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { matches } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { MedicineOrderCreateDialogComponent } from '../medicine-order-create-dialog/medicine-order-create-dialog.component';
import { MedicineOrderService, PrecscriptionPaymentPaged, PrecscriptionPaymentReport } from '../medicine-order.service';

@Component({
  selector: 'app-medicine-order-prescription-payment-list',
  templateUrl: './medicine-order-prescription-payment-list.component.html',
  styleUrls: ['./medicine-order-prescription-payment-list.component.css']
})
export class MedicineOrderPrescriptionPaymentListComponent implements OnInit {
  @ViewChild(GridComponent, { static: true }) private grid: GridComponent;
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  search: string;
  dateFrom: Date;
  dateTo: Date;
  loading = false;
  precscriptionPaymentReport: PrecscriptionPaymentReport = new PrecscriptionPaymentReport();
  state: string = '';
  limit = 20;
  skip = 0;
  states = [
    // { value: "draft", name: "Chưa thanh toán" },
    { value: "confirmed", name: "Đã thanh toán" },
    { value: "cancel", name: "Hủy thanh toán" }
  ]
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(
    private intlService: IntlService,
    private medicineOrderSerive: MedicineOrderService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
    this.getReport();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new PrecscriptionPaymentPaged();
    paged.limit = this.limit;
    paged.offset = this.skip;
    paged.search = this.search ? this.search : '';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:50");
    paged.state = this.state;
    this.medicineOrderSerive.getPaged(paged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(res);

      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  clickItem(item) {
    if (item && item.dataItem) {
      var id = item.dataItem.id
      const modalRef = this.modalService.open(MedicineOrderCreateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thanh toán hóa đơn thuốc';
      modalRef.componentInstance.id = id;
      modalRef.result.then(res => {
        this.loadDataFromApi();
        this.getReport();
      }, () => {
      });
    }
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
    this.getReport();
  }

  getReport() {
    var val = {
      dateTo: this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59"),
      dateFrom: this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd")
    }
    this.medicineOrderSerive.getReport(val).subscribe(
      result => {
        if (result) {
          this.precscriptionPaymentReport = result;
        }
      }
    )
  }

  stateChange(item) {
    this.state = item ? item.value : '';
    this.skip = 0;
    this.loadDataFromApi();
  }


}
