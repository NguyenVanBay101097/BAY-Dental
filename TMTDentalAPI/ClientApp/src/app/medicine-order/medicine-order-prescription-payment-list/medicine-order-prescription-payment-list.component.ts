import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-medicine-order-prescription-payment-list',
  templateUrl: './medicine-order-prescription-payment-list.component.html',
  styleUrls: ['./medicine-order-prescription-payment-list.component.css']
})
export class MedicineOrderPrescriptionPaymentListComponent implements OnInit {
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  search: string;
  loading = false;
  limit = 20;
  offset = 0;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor() { }

  ngOnInit() {
  }

}
