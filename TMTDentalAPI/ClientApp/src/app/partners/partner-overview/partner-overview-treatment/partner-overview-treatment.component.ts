import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-partner-overview-treatment',
  templateUrl: './partner-overview-treatment.component.html',
  styleUrls: ['./partner-overview-treatment.component.css']
})
export class PartnerOverviewTreatmentComponent implements OnInit, OnChanges {

  thTable_saleOrders = [
    { name: 'Số phiếu' },
    { name: 'Ngày lập phiếu' },
    { name: 'Tổng tiền' },
    { name: 'Còn nợ' }
  ]
  @Input() saleOrders: SaleOrderBasic[];
  @Input() partnerId: string;

  gridData: GridDataResult;
  limit = 1000;
  skip = 0;
  title = 'Đơn vị tính';
  loading = false;

  constructor(
    private router: Router,
    private saleOrderService: SaleOrderService,
    private modalService: NgbModal
  ) { }
  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
  }

  ngOnInit() {
    setTimeout(() => {
      if (this.saleOrders.length > 0) {
        this.loadData();
      }
    }, 300);
    // this.loadDataFromApi();
  }

  chossesSaleOrder(id) {
    if (id) {
      this.router.navigateByUrl(`sale-orders/form?id=${id}`)
    } else {
      this.router.navigate(['/sale-orders/form'], { queryParams: { partner_id: this.partnerId } });
    }
  }

  loadData() {
    this.gridData = {
      data: this.saleOrders,
      total: this.saleOrders && this.saleOrders.length ? this.saleOrders.length : 0
    };
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    // val.partnerId = this.partnerId;
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

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.saleOrderService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}