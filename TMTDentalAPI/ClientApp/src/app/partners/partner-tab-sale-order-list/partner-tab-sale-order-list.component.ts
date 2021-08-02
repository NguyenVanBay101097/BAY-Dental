import { Component, OnInit, Input } from '@angular/core';
import { PartnerBasic } from '../partner-simple';
import { SaleOrderService, SaleOrderPaged } from 'src/app/core/services/sale-order.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-partner-tab-sale-order-list',
  templateUrl: './partner-tab-sale-order-list.component.html',
  styleUrls: ['./partner-tab-sale-order-list.component.css']
})
export class PartnerTabSaleOrderListComponent implements OnInit {
  @Input() item: PartnerBasic;
  gridData: GridDataResult;
  limit = 5;
  skip = 0;
  loading = false;

  constructor(private saleOrderService: SaleOrderService, private router: Router,
    private modalService: NgbModal) { }

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
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: item.id } });
  }

  deleteBtnClick(item: SaleOrderBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.saleOrderService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }

  addSOBtnClick() {
    this.router.navigate(['/sale-orders/form'], { queryParams: { partner_id: this.item.id } });
  }
}
