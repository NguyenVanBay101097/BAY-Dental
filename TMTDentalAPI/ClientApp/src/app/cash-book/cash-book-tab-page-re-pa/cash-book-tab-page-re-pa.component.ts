import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PhieuThuChiService, PhieuThuChiPaged } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { CashBookCuDialogComponent } from '../cash-book-cu-dialog/cash-book-cu-dialog.component';
import { CashBookPaged } from '../cash-book.service';

@Component({
  selector: 'app-cash-book-tab-page-re-pa',
  templateUrl: './cash-book-tab-page-re-pa.component.html',
  styleUrls: ['./cash-book-tab-page-re-pa.component.css']
})
export class CashBookTabPageRePaComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  type: string;
  paged: CashBookPaged;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, private modalService: NgbModal, 
    private phieuThuChiService: PhieuThuChiService, private router: Router,
    private printService: PrintService) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.type = params.get('type');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  getType(type) {
    if (type == "inbound") {
      return "Phiếu thu";
    } else {
      return "Phiếu chi";
    }
  }

  getState(state) {
    if (state == "posted") {
      return "Đã xác nhận";
    } else {
      return "Nháp"
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PhieuThuChiPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.type;
    this.phieuThuChiService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.gridData = res;
      console.log(res);
      this.loading = false;
    }, (err) => {
      console.log(err);
      this.loading = false;
    })
  }

  createItem(type) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = type;
    modalRef.result.then((res) => {
      this.loadDataFromApi();
    }, (err) => { });
  }

  editItem(item) {
    const modalRef = this.modalService.open(CashBookCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.type = item.type;
    modalRef.componentInstance.itemId = item.resId;
    modalRef.result.then((res) => {
      this.loadDataFromApi();
    }, (err) => { });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = `Xóa ${this.getType(item.type).toLowerCase()}`;
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa ${this.getType(item.type).toLowerCase()}?`;

    modalRef.result.then((res) => {
      this.phieuThuChiService.delete(item.resId).subscribe(() => {
        this.loadDataFromApi();
      }, (res) => {
      });
    }, (err) => {
    });
  }
  
  printItem(id) {
    this.phieuThuChiService.getPrint(id).subscribe((data: any) => {
      this.printService.printHtml(data.html);
    });
  }
}
