import { Component, OnInit, Input } from '@angular/core';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { PurchaseOrderPaged } from 'src/app/purchase-orders/purchase-order.service';
import { PartnerService } from '../partner.service';
import { map } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-purchase-order-refund',
  templateUrl: './purchase-order-refund.component.html',
  styleUrls: ['./purchase-order-refund.component.css']
})
export class PurchaseOrderRefundComponent implements OnInit {

  @Input() public id: string; // ID của NCC
  constructor(private service: PartnerService, private fb: FormBuilder) { }

  gridView: GridDataResult;
  loading = false;
  skip = 0;
  pageSize = 5;
  gridLoading = false;
  formFilter: FormGroup;

  ngOnInit() {
    this.formFilter = this.fb.group({
      isOrder: true,
      isRefund: true,
    })
    this.loadPurchaseOrder();
  }

  loadPurchaseOrder() {
    this.gridLoading = true;
    var poPaged = new PurchaseOrderPaged;
    poPaged.partnerId = this.id;
    poPaged.limit = this.pageSize;
    poPaged.offset = this.skip;

    var ar = [];
    if (this.getIsOrder)
      ar.push('order');
    if (this.getIsRefund)
      ar.push('refund');
    poPaged.type = ar.join(",");

    this.service.getPurchaseOrderByPartner(poPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.gridLoading = false;
    }, er => {
      this.gridLoading = true;
      console.log(er);
    }
    );
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
    this.loadPurchaseOrder();
  }

  typeGet(type: string) {
    switch (type.toLowerCase()) {
      case 'order':
        return 'Phiếu mua';
      case 'refund':
        return 'Phiếu trả';
    }
  }

  stateGet(state: string) {
    switch (state.toLowerCase()) {
      case 'purchase':
        return 'purchase';
      case 'refund':
        return 'Phiếu trả';
    }
  }

  get getIsOrder() {
    return this.formFilter.get('isOrder').value;
  }

  get getIsRefund() {
    return this.formFilter.get('isRefund').value;
  }
}
