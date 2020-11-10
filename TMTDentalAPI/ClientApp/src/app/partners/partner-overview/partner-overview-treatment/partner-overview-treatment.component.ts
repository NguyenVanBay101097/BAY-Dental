import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';

@Component({
  selector: 'app-partner-overview-treatment',
  templateUrl: './partner-overview-treatment.component.html',
  styleUrls: ['./partner-overview-treatment.component.css']
})
export class PartnerOverviewTreatmentComponent implements OnInit {

  thTable_saleOrders = [
    { name: 'Số phiếu' },
    { name: 'Ngày lập phiếu' },
    { name: 'Tổng tiền' },
    { name: 'Còn nợ' }
  ]
  @Input() partnerId: string;

  gridData: GridDataResult;
  limit = 1000;
  skip = 0;
  title = 'Đơn vị tính';
  loading = false;

  constructor(
    private router: Router,
    private saleOrderService: SaleOrderService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  chossesSaleOrder(id) {
    if (id) {
      this.router.navigateByUrl(`sale-orders/form?id=${id}`)
    } else {
      this.router.navigateByUrl(`sale-orders/form`)
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId;
    this.saleOrderService.getPaged(val).pipe(
      map((response: any) =>
        (<GridDataResult>{
          data: response.items,
          total: response.totalItems
        }))
    ).subscribe(res => {
      console.log(res);

      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }
}