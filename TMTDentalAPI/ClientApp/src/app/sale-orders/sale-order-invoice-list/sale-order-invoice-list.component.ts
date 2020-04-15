import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { SaleOrderService } from '../sale-order.service';

@Component({
  selector: 'app-sale-order-invoice-list',
  templateUrl: './sale-order-invoice-list.component.html',
  styleUrls: ['./sale-order-invoice-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleOrderInvoiceListComponent implements OnInit {
  gridData: any;
  loading = false;
  id: string;
  saleOrder: any;

  constructor(private saleOrderService: SaleOrderService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.loadDataFromApi();
  }

  stateGet(state) {
    switch (state) {
      case 'posted':
        return 'Đã vào sổ';
      default:
        return 'Nháp';
    }
  }

  loadDataFromApi() {
    this.loading = true;

    this.saleOrderService.getInvoices(this.id).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }
}


