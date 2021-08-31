import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { WarrantyCuDidalogComponent } from '../warranty-cu-didalog/warranty-cu-didalog.component';

@Component({
  selector: 'app-labo-warranty-detail-list',
  templateUrl: './labo-warranty-detail-list.component.html',
  styleUrls: ['./labo-warranty-detail-list.component.css']
})
export class LaboWarrantyDetailListComponent implements OnInit {
  @Input() item: any;
  limit = 20;
  skip = 0;
  loading = false;
  gridData: GridDataResult;

  constructor(
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    console.log(this.item);

  }

  editItem() { }
  deleteItem() { }
  pageChange() { }
  createNewWarranty() {
    let val = {
      laboOrderId: this.item.id,
      laboOrderName: this.item.name,
      partnerName: this.item.partnerName,
      partnerRef: this.item.partnerRef,
      customerName: this.item.customerName,
      saleOrderId: this.item.saleOrderId,
      saleOrderLineName: this.item.saleOrderLineName,
      teeth: this.item.teeth
    }
    const modalRef = this.modalService.open(WarrantyCuDidalogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.infoLabo = val;

  }
}
