import { Component, OnInit, Input } from '@angular/core';
import { PartnerBasic } from '../partner-simple';
import { SaleOrderService, SaleOrderPaged } from 'src/app/sale-orders/sale-order.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { Router } from '@angular/router';

@Component({
  selector: 'app-partner-tab-sale-order-list',
  templateUrl: './partner-tab-sale-order-list.component.html',
  styleUrls: ['./partner-tab-sale-order-list.component.css']
})
export class PartnerTabSaleOrderListComponent implements OnInit {
  @Input() item: PartnerBasic;
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  loading = false;

  constructor(private saleOrderService: SaleOrderService, private router: Router) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.item.id;

    this.saleOrderService.getPaged(val).pipe(
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

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editBtnClick(item: SaleOrderBasic) {
    window.open(`/sale-orders/form?id=${item.id}`, '_blank');
  }

  deleteBtnClick(item: SaleOrderBasic) {
  }

  addSOBtnClick() {
    this.router.navigate(['/sale-orders/form'], { queryParams: { partner_id: this.item.id } });
  }
}
