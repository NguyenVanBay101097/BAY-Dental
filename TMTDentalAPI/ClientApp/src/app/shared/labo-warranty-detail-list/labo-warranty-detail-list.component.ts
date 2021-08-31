import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { WarrantyCuDidalogComponent } from '../warranty-cu-didalog/warranty-cu-didalog.component';

@Component({
  selector: 'app-labo-warranty-detail-list',
  templateUrl: './labo-warranty-detail-list.component.html',
  styleUrls: ['./labo-warranty-detail-list.component.css']
})
export class LaboWarrantyDetailListComponent implements OnInit {
  limit = 20;
  skip = 0;
  loading = false;
  gridData: GridDataResult;

  constructor(
    private modalService: NgbModal
  ) { }

  ngOnInit() {
  }

  editItem() { }
  deleteItem() { }
  pageChange() { }
  createNewWarranty(){
    const modalRef = this.modalService.open(WarrantyCuDidalogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    
  }
}
