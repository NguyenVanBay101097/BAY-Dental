import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { IntlService } from '@progress/kendo-angular-intl';
import { map } from 'rxjs/operators';
import { CompanyService, CompanyPaged, CompanyBasic } from '../company.service';
import { CompanyCuDialogComponent } from '../company-cu-dialog/company-cu-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CompanyListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  opened = false;
  loading = false;

  constructor(private companyService: CompanyService, private modalService: NgbModal, public intl: IntlService,
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
    var val = new CompanyPaged();
    val.offset = this.skip;
    val.limit = this.limit;

    this.companyService.getPaged(val).pipe(
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
    let modalRef = this.modalService.open(CompanyCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm chi nhánh';

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: CompanyBasic) {
    let modalRef = this.modalService.open(CompanyCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa chi nhánh';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item: CompanyBasic) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa chi nhánh',
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
          this.companyService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }

}

