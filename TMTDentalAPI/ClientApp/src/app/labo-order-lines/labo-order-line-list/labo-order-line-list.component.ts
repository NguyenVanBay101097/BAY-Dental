import { Component, ViewChild, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { LaboOrderLinePaged, LaboOrderLineService, LaboOrderLineDisplay } from '../labo-order-line.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LaboOrderLineCuDialogComponent } from '../labo-order-line-cu-dialog/labo-order-line-cu-dialog.component';
import { Subject } from 'rxjs';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-labo-order-line-list',
  templateUrl: './labo-order-line-list.component.html',
  styleUrls: ['./labo-order-line-list.component.css']
})
export class LaboOrderLineListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  searchCustomer: string;
  searchSupplier: string;
  searchProduct: string;
  sentDateFrom: Date;
  sentDateTo: Date;
  receivedDateFrom: Date;
  receivedDateTo: Date;
  searchUpdate = new Subject<string>();

  constructor(private laboOrderLineService: LaboOrderLineService,
    private windowService: WindowService, private dialogService: DialogService, private intlService: IntlService) {
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

  loadDataFromApi() {
    this.loading = true;
    var val = new LaboOrderLinePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.searchCustomer = this.searchCustomer || '';
    val.searchSupplier = this.searchSupplier || '';
    val.searchProduct = this.searchProduct || '';
    val.sentDateFrom = this.sentDateFrom ? this.intlService.formatDate(this.sentDateFrom, 'd', 'en-US') : '';
    val.sentDateTo = this.sentDateTo ? this.intlService.formatDate(this.sentDateTo, 'd', 'en-US') : '';
    val.receivedDateFrom = this.receivedDateFrom ? this.intlService.formatDate(this.receivedDateFrom, 'd', 'en-US') : '';
    val.receivedDateTo = this.receivedDateTo ? this.intlService.formatDate(this.receivedDateTo, 'd', 'en-US') : '';

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

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    const windowRef = this.windowService.open({
      title: 'Thêm labo',
      content: LaboOrderLineCuDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
    });
  }

  editItem(item: LaboOrderLineDisplay) {
    const windowRef = this.windowService.open({
      title: `Sửa labo`,
      content: LaboOrderLineCuDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.id = item.id;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
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
