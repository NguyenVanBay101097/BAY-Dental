import { NotifyService } from 'src/app/shared/services/notify.service';
import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LoaiThuChiService, loaiThuChiPaged, loaiThuChiBasic, loaiThuChi } from '../loai-thu-chi.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LoaiThuChiFormComponent } from 'src/app/shared/loai-thu-chi-form/loai-thu-chi-form.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-loai-thu-chi-list',
  templateUrl: './loai-thu-chi-list.component.html',
  styleUrls: ['./loai-thu-chi-list.component.css']
})
export class LoaiThuChiListComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  type: string;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, private modalService: NgbModal,
    private notifyService: NotifyService,
    private loaiThuChiService: LoaiThuChiService,
    private authService: AuthService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.type = data.type;
      this.skip = 0;
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new loaiThuChiPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.type;
    val.companyId = this.authService.userInfo.companyId;

    this.loaiThuChiService.getPaged(val).pipe(
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
    this.limit = event.take;
    this.loadDataFromApi();
  }

  converttype() {
    switch (this.type) {
      case 'thu':
        return 'loại thu';
      case 'chi':
        return 'loại chi';
    }
  }

  createItem() {
    const modalRef = this.modalService.open(LoaiThuChiFormComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm ' + this.converttype();
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => {

    });
  }

  editItem(item: loaiThuChi) {
    const modalRef = this.modalService.open(LoaiThuChiFormComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa ' + this.converttype();
    modalRef.componentInstance.itemId = item.id;
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => {

    });
  }

  deleteItem(item: loaiThuChiBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + this.converttype();

    modalRef.result.then(() => {
      this.loaiThuChiService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success', 'Xóa thành công');
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }
}
