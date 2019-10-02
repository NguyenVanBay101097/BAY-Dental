import { Component, ViewChild, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PartnerCategoryService, PartnerCategoryPaged, PartnerCategoryBasic } from '../partner-category.service';
import { PartnerCategoryCuDialogComponent } from '../partner-category-cu-dialog/partner-category-cu-dialog.component';

@Component({
  selector: 'app-partner-category-list',
  templateUrl: './partner-category-list.component.html',
  styleUrls: ['./partner-category-list.component.css']
})

export class PartnerCategoryListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private partnerCategoryService: PartnerCategoryService,
    private windowService: WindowService, private dialogService: DialogService) {
  }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PartnerCategoryPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.partnerCategoryService.getPaged(val).pipe(
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
      title: 'Thêm nhóm đối tác',
      content: PartnerCategoryCuDialogComponent,
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

  editItem(item: PartnerCategoryBasic) {
    const windowRef = this.windowService.open({
      title: `Sửa nhóm sản phẩm`,
      content: PartnerCategoryCuDialogComponent,
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

  deleteItem(item: PartnerCategoryBasic) {
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
          this.partnerCategoryService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }
}
