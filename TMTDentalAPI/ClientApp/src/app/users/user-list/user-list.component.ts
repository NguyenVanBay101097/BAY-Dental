import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { UserService, UserPaged, UserBasic } from '../user.service';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { IntlService } from '@progress/kendo-angular-intl';
import { map } from 'rxjs/operators';
import { UserCuDialogComponent } from '../user-cu-dialog/user-cu-dialog.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  opened = false;
  loading = false;

  constructor(private userService: UserService, private windowService: WindowService, public intl: IntlService,
    private dialogService: DialogService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new UserPaged();
    val.offset = this.skip;
    val.limit = this.limit;

    this.userService.getPaged(val).pipe(
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

  createItem() {
    const windowRef = this.windowService.open({
      title: 'Thêm người dùng',
      content: UserCuDialogComponent,
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

  editItem(item: UserBasic) {
    const windowRef = this.windowService.open({
      title: `Sửa người dùng`,
      content: UserCuDialogComponent,
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

  deleteItem(item: UserBasic) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa người dùng',
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
          this.userService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }

}
