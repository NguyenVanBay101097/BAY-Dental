import { HttpParams } from '@angular/common/http';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map } from 'rxjs/operators';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnersService } from 'src/app/shared/services/partners.service';

@Component({
  selector: 'app-partner-overview-treatment',
  templateUrl: './partner-overview-treatment.component.html',
  styleUrls: ['./partner-overview-treatment.component.css']
})
export class PartnerOverviewTreatmentComponent implements OnInit {
  // @Input() saleOrders: SaleOrderBasic[] = [];
  @Input() saleOrdersData: GridDataResult;
  @Input() limit: number;
  @Input() skip: number;
  @Output() deleteItemEvent = new EventEmitter<any>();
  @Output() pageChageEvent = new EventEmitter<any>();

  

  constructor(
    private router: Router,
    private saleOrderService: SaleOrderService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
  ) { }

  ngOnInit() {
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu điều trị';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa phiếu điều trị?';
    modalRef.result.then(() => {
      this.saleOrderService.unlink([item.id]).subscribe(() => {
        this.notify("success","Xóa thành công");
        this.deleteItemEvent.emit(null);
      });
    });
  }

  viewSaleOrder(id) {
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  pageChange(event: PageChangeEvent){
    this.skip = event.skip;
    this.pageChageEvent.emit(this.skip);
  }
}