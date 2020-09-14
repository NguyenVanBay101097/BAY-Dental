import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleCouponProgramService, SaleCouponProgramPaged, SaleCouponProgramBasic } from '../sale-coupon-program.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-sale-coupon-program-list',
  templateUrl: './sale-coupon-program-list.component.html',
  styleUrls: ['./sale-coupon-program-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleCouponProgramListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  filterActive = true;

  constructor(private programService: SaleCouponProgramService, private route: ActivatedRoute,
    private router: Router, private modalService: NgbModal) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  stateGet(state) {
    switch (state) {
      case 'open':
        return 'Đã xác nhận';
      case 'paid':
        return 'Đã thanh toán';
      default:
        return 'Nháp';
    }
  }

  filterActiveChange(active) {
    this.filterActive = active;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SaleCouponProgramPaged();
    val.programType = 'coupon_program';
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.active = this.filterActive;

    this.programService.getPaged(val).pipe(
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
    this.router.navigate(['programs/coupon-programs/form']);
  }

  editItem(item: SaleCouponProgramBasic) {
    this.router.navigate(['programs/coupon-programs/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa chương trình coupon';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.programService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
        this.selectedIds = [];
      });
    });
  }

  unlink() {
    if (this.selectedIds.length == 0) {
      return false;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa chương trình coupon';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.programService.unlink(this.selectedIds).subscribe(() => {
        this.loadDataFromApi();
        this.selectedIds = [];
      });
    });
  }

  actionArchive() {
    if (this.selectedIds.length) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Đóng chương trình';
      modalRef.componentInstance.body = 'Bạn chắc chắn muốn đóng chương trình?';
      modalRef.result.then(() => {
        this.programService.actionArchive(this.selectedIds).subscribe(() => {
          this.loadDataFromApi();
          this.selectedIds = [];
        });
      });
    }
  }

  actionUnArchive() {
    if (this.selectedIds.length) {
      this.programService.actionUnArchive(this.selectedIds).subscribe(() => {
        this.loadDataFromApi();
        this.selectedIds = [];
      });
    }
  }
}


