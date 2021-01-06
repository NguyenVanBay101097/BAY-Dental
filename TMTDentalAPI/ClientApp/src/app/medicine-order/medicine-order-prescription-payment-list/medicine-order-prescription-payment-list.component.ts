import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { MedicineOrderService, PrecscriptionPaymentPaged } from '../medicine-order.service';

@Component({
  selector: 'app-medicine-order-prescription-payment-list',
  templateUrl: './medicine-order-prescription-payment-list.component.html',
  styleUrls: ['./medicine-order-prescription-payment-list.component.css']
})
export class MedicineOrderPrescriptionPaymentListComponent implements OnInit {
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  search: string;
  dateFrom: Date;
  dateTo: Date;
  loading = false;
  state: string = '';
  limit = 20;
  offset = 0;
  states = [
    { value: "draft", name: "Chưa thanh toán" },
    { value: "confirmed", name: "Đã thanh toán" },
    { value: "cancel", name: "Hủy thanh toán" }
  ]
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(
    private intlService: IntlService,
    private medicineOrderSerive: MedicineOrderService
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }


  loadDataFromApi() {
    this.loading = true;
    var paged = new PrecscriptionPaymentPaged();
    paged.limit = this.limit;
    paged.offset = this.offset;
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
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  stateChange(item) {
    this.state = item ? item.value : '';
    this.loadDataFromApi();
  }


}
