import { Component, ViewChild, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { LaboOrderLinePaged, LaboOrderLineService, LaboOrderLineDisplay } from '../labo-order-line.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LaboOrderLineCuDialogComponent } from '../labo-order-line-cu-dialog/labo-order-line-cu-dialog.component';
import { Subject } from 'rxjs';
import { IntlService } from '@progress/kendo-angular-intl';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-labo-order-line-list',
  templateUrl: './labo-order-line-list.component.html',
  styleUrls: ['./labo-order-line-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class LaboOrderLineListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  search: string;
  searchCustomer: string;
  searchSupplier: string;
  searchProduct: string;
  sentDateFrom: Date;
  sentDateTo: Date;
  receivedDateFrom: Date;
  receivedDateTo: Date;
  searchUpdate = new Subject<string>();

  constructor(private laboOrderLineService: LaboOrderLineService,
    private windowService: WindowService, private dialogService: DialogService, private intlService: IntlService,
    private modalService: NgbModal) {
  }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
  }

  onChangeDate(value: Date) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  stateGet(state) {
    switch (state) {
      case 'purchase':
        return 'Đơn hàng';
      case 'done':
        return 'Đã khóa';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Nháp';
    }
  }


  loadDataFromApi() {
    this.loading = true;
    var val = new LaboOrderLinePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    if (this.search) {
      val.search = this.search;
    }
    if (this.sentDateFrom) {
      val.sentDateFrom = this.intlService.formatDate(this.sentDateFrom, 'd', 'en-US');
    }
    if (this.sentDateTo) {
      val.sentDateTo = this.intlService.formatDate(this.sentDateTo, 'd', 'en-US');
    }
    if (this.receivedDateFrom) {
      val.receivedDateFrom = this.intlService.formatDate(this.receivedDateFrom, 'd', 'en-US');
    }
    if (this.receivedDateTo) {
      val.receivedDateTo = this.intlService.formatDate(this.receivedDateTo, 'd', 'en-US');
    }

    this.laboOrderLineService.getPaged(val).pipe(
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

  onAdvanceSearchChange(data) {
    this.sentDateFrom = data.sentDateFrom;
    this.sentDateTo = data.sentDateTo;
    this.receivedDateFrom = data.receivedDateFrom;
    this.receivedDateTo = data.receivedDateTo;

    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(LaboOrderLineCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'Tạo labo';

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: LaboOrderLineDisplay) {
    let modalRef = this.modalService.open(LaboOrderLineCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'Sửa labo';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa nhóm sản phẩm',
      content: 'Bạn có chắc chắn muốn xóa?',
      actions: [
        { text: 'Hủy bỏ', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ],
      width: 450,
      height: 200,
      minWidth: 250
    });

    dialog.result.subscribe((result) => {
      if (result instanceof DialogCloseResult) {
        console.log('close');
      } else {
        console.log('action', result);
        if (result['value']) {
          this.laboOrderLineService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }
}
