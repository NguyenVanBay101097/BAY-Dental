import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult, WindowRef } from '@progress/kendo-angular-dialog';
import { IntlService } from '@progress/kendo-angular-intl';
import { DotKhamService } from '../dot-kham.service';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { DotKhamCreateUpdateComponent } from '../dot-kham-create-update/dot-kham-create-update.component';
import { ProductDialogComponent } from 'src/app/products/product-dialog/product-dialog.component';
import { DotKhamPaged, DotKhamBasic } from '../dot-khams';

@Component({
  selector: 'app-dot-kham-list',
  templateUrl: './dot-kham-list.component.html',
  styleUrls: ['./dot-kham-list.component.css']
})
export class DotKhamListComponent implements OnInit {
  columns = [{ field: "id" }, { field: "name" }];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  windowOpened = false;
  search: string;
  searchUpdate = new Subject<string>();

  constructor(private dotKhamService: DotKhamService, private windowService: WindowService,
    private dialogService: DialogService, public intl: IntlService, private router: Router) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new DotKhamPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.dotKhamService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(res);
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

  openWindow(id) {
    const windowRef: WindowRef = this.windowService.open(
      {
        title: 'Đợt khám',
        content: DotKhamCreateUpdateComponent,
        minWidth: 250,
        width: 920
      });
    this.windowOpened = true;
    const instance = windowRef.content.instance;
    instance.id = id;
    console.log(this.windowOpened);
    windowRef.result.subscribe(
      (result) => {
        this.windowOpened = false;
        console.log(this.windowOpened);
        if (!(result instanceof WindowCloseResult)) {
        }
      }
    )
  }

  createItem() {
    // const windowRef = this.windowService.open({
    //   title: 'Thêm sản phẩm',
    //   content: ProductDialogComponent,
    //   resizable: false,
    //   autoFocusedElement: '[name="name"]',
    // });

    // this.opened = true;

    // windowRef.result.subscribe((result) => {
    //   this.opened = false;
    //   if (!(result instanceof WindowCloseResult)) {
    //     this.loadDataFromApi();
    //   }
    // });
  }

  editItem(item: DotKhamBasic) {
    this.router.navigate(['/dot-khams/edit/', item.id]);
    const windowRef = this.windowService.open({
      title: `Sửa sản phẩm`,
      content: ProductDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.id = item.id;

    this.windowOpened = true;

    windowRef.result.subscribe((result) => {
      this.windowOpened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.loadDataFromApi();
      }
    });
  }

  deleteItem(item) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa sản phẩm',
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
          this.dotKhamService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }
}
