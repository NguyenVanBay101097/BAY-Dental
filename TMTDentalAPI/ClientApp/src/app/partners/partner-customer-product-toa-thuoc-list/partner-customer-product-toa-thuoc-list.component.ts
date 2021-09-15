import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ToaThuocPaged, ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { ToaThuocPrintComponent } from 'src/app/shared/toa-thuoc-print/toa-thuoc-print.component';
import { ToaThuocCuDialogSaveComponent } from 'src/app/shared/toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-partner-customer-product-toa-thuoc-list',
  templateUrl: './partner-customer-product-toa-thuoc-list.component.html',
  styleUrls: ['./partner-customer-product-toa-thuoc-list.component.css']
})
export class PartnerCustomerProductToaThuocListComponent implements OnInit {

  id: string;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchCateg: ProductCategoryBasic;

  constructor(
    private activeRoute: ActivatedRoute,
    private toaThuocService: ToaThuocService,
    private modalService: NgbModal,
    private printService: PrintService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.loadData();
  }

  loadData() {
    this.loading = true;
    var val = new ToaThuocPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.partnerId = this.id;

    this.toaThuocService.getPaged(val).pipe(
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

  printToaThuoc(item) {
    this.toaThuocService.getPrint(item.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  createProductToaThuoc() {
    let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Đơn Thuốc';
    modalRef.componentInstance.defaultVal = { partnerId: this.id };
    modalRef.result.then((result: any) => {
      this.loadData();
      if (result.print) {
        this.printToaThuoc(result.item);
      }
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadData();
  }

  editProductToaThuoc(item: any) {
    let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: Đơn Thuốc';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.partnerId = this.id;
    modalRef.result.then((result) => {
      this.loadData();
      if (result.print) {
        this.printToaThuoc(item);
      }
    }, () => {
    });
  }

  deleteProductToaThuoc(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: Đơn Thuốc';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa đơn thuốc này?';
    modalRef.result.then(() => {
      this.toaThuocService.delete(item.id).subscribe(() => {
        this.loadData();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

}
