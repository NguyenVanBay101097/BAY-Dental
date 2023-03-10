import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { SaleCouponProgramBasic, SaleCouponProgramPaged, SaleCouponProgramService } from '../sale-coupon-program.service';

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
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  filterActive = true;

  // permission
  canSaleCouponProgramCreate = this.checkPermissionService.check(["SaleCoupon.SaleCouponProgram.Create"]);
  canSaleCouponProgramUpdate = this.checkPermissionService.check(["SaleCoupon.SaleCouponProgram.Update"]);
  canSaleCouponProgramDelete = this.checkPermissionService.check(["SaleCoupon.SaleCouponProgram.Delete"]);

  constructor(private programService: SaleCouponProgramService,
    private router: Router, private modalService: NgbModal, 
    private checkPermissionService: CheckPermissionService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

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
        return '???? x??c nh???n';
      case 'paid':
        return '???? thanh to??n';
      default:
        return 'Nh??p';
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
    this.limit = event.take;
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
    modalRef.componentInstance.title = 'X??a ch????ng tr??nh coupon';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a?';
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
    modalRef.componentInstance.title = 'X??a ch????ng tr??nh coupon';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a?';
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
      modalRef.componentInstance.title = '????ng ch????ng tr??nh';
      modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n ????ng ch????ng tr??nh?';
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


