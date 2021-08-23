import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleCouponProgramService, SaleCouponProgramBasic, SaleCouponProgramGetListPagedRequest } from '../sale-coupon-program.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-sale-promotion-program-list',
  templateUrl: './sale-promotion-program-list.component.html',
  styleUrls: ['./sale-promotion-program-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SalePromotionProgramListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pageSizes = [20, 50, 100, 200];
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  filterActive = true;
  listStatus = [
    { text: 'Chưa chạy', value: 'waiting' }, 
    { text: 'Đang chạy', value: 'running' }, 
    { text: 'Tạm dừng', value: 'paused' }, 
    { text: 'Hết hạn', value: 'expired' },
  ]
  listFilterStatus = this.listStatus;
  selectedStatus = null;

  ruleDateFromBegin: Date;
  ruleDateFromEnd: Date;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  // permission
  canSaleCouponProgramCreate = this.checkPermissionService.check(["SaleCoupon.SaleCouponProgram.Create"]);
  canSaleCouponProgramUpdate = this.checkPermissionService.check(["SaleCoupon.SaleCouponProgram.Update"]);
  canSaleCouponProgramDelete = this.checkPermissionService.check(["SaleCoupon.SaleCouponProgram.Delete"]);

  constructor(private programService: SaleCouponProgramService, private route: ActivatedRoute, 
    private router: Router, private modalService: NgbModal, 
    private checkPermissionService: CheckPermissionService,
    private notificationService: NotificationService,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.ruleDateFromBegin = this.monthStart;
    this.ruleDateFromEnd = this.monthEnd;

    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  onDateSearchChange(data) {
    this.ruleDateFromBegin = data.dateFrom;
    this.ruleDateFromEnd = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  filterChangeStatus(search: string) {
    this.listFilterStatus = this.listStatus.filter(x => x.text.includes(search));
  }

  changeFilterActive() {
    this.skip = 0;
    this.loadDataFromApi();
  }

  getStatus(status) {
    switch (status) {
      case "waiting":
        return "Chưa chạy";
      case "running":
        return "Đang chạy";
      case "paused":
        return "Tạm dừng";
      case "expired":
        return "Hết hạn";
      default:
        return "";
    }
  }

  getColorStatus(status) {
    switch (status) {
      case "Chưa chạy":
        return "text-dark";
      case "Đang chạy":
        return "text-success";
      case "Tạm ngừng":
        return "text-warning";
      case "Hết hạn":
        return "text-danger";
      default:
        return "text-dark";
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SaleCouponProgramGetListPagedRequest();
    val.programType = 'promotion_program';
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.active = this.filterActive;
    val.ruleDateFromBegin = this.ruleDateFromBegin ? this.intlService.formatDate(this.ruleDateFromBegin, 'yyyy-MM-dd') : '';
    val.ruleDateFromEnd = this.ruleDateFromEnd ? this.intlService.formatDate(this.ruleDateFromEnd, 'yyyy-MM-dd') : '';

    if (this.selectedStatus && this.selectedStatus.value) {
      val.status = this.selectedStatus.value;
    }

    this.programService.getListPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
      console.log(this.gridData);
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onPageSizeChange(value: number): void {
    this.skip = 0;
    this.limit = value;
    this.loadDataFromApi();
  }

  notify(type, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type, icon: true }
    });
  }

  createItem() {
    this.router.navigate(['programs/promotion-programs/form']);
  }

  editItem(item: SaleCouponProgramBasic) {
    this.router.navigate(['programs/promotion-programs/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa chương trình khuyến mãi';
    modalRef.componentInstance.body = 'Bạn có muốn xóa chương trình khuyến mãi không?';
    modalRef.result.then(() => {
      this.programService.unlink([item.id]).subscribe(() => {
        this.notify('success', 'Xóa CTKM thành công');
        this.loadDataFromApi();
        this.selectedIds = [];
      });
    });
  }

  actionArchive() {
    if (this.selectedIds.length) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Tạm ngừng chương trình khuyến mãi';
      modalRef.componentInstance.body = 'Bạn có muốn ngừng chạy chương trình khuyến mãi không?';
      modalRef.componentInstance.body2 = 'Lưu ý: Chỉ tạm ngừng các khuyến mãi đã kích hoạt đang chạy hoặc chưa chạy.';
      modalRef.result.then(() => {
        this.programService.actionArchive(this.selectedIds).subscribe(() => {
          this.notify('success', 'Tạm ngừng CTKM thành công');
          this.loadDataFromApi();
          this.selectedIds = [];
        });
      });
    }
  }

  actionUnArchive() {
    if (this.selectedIds.length) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Kích hoạt chương trình khuyến mãi';
      modalRef.componentInstance.body = 'Bạn có muốn kích hoạt chương trình khuyến mãi?';
      modalRef.result.then(() => {
        this.programService.actionUnArchive(this.selectedIds).subscribe(() => {
          this.notify('success', 'Kích hoạt CTKM thành công');
          this.loadDataFromApi();
          this.selectedIds = [];
        });
      });
    }
  }

  valueChangeStatus(value) {
    this.selectedStatus = value;
    this.skip = 0;
    this.loadDataFromApi();
  }
}



