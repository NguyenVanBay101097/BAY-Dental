import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { PartnerPaged, PartnerBasic } from '../partner-simple';
import { PartnerService } from '../partner.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { PartnerCustomerCuDialogComponent } from '../partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnerSupplierCuDialogComponent } from '../partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';

@Component({
  selector: 'app-partner-supplier-list',
  templateUrl: './partner-supplier-list.component.html',
  styleUrls: ['./partner-supplier-list.component.css']
})
export class PartnerSupplierListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;

  searchNameRef: string;
  searchPhone: string;
  searchUpdate = new Subject<string>();
  constructor(private partnerService: PartnerService, private windowService: WindowService,
    private dialogService: DialogService) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var params = new HttpParams().set('limit', this.limit.toString())
      .set('offset', this.skip.toString())
      .set('supplier', 'true');
    if (this.searchNameRef) {
      params = params.append('searchNameRef', this.searchNameRef);
    }
    if (this.searchPhone) {
      params = params.append('searchPhone', this.searchPhone);
    }

    this.partnerService.getPaged(params).pipe(
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
      title: 'Thêm nhà cung cấp',
      content: PartnerSupplierCuDialogComponent,
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

  editItem(item: PartnerBasic) {
    const windowRef = this.windowService.open({
      title: `Sửa nhà cung cấp`,
      content: PartnerSupplierCuDialogComponent,
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

  deleteItem(item: PartnerBasic) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa nhà cung cấp',
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
      } else {
        if (result['value']) {
          this.partnerService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
          });
        }
      }
    });
  }
}
