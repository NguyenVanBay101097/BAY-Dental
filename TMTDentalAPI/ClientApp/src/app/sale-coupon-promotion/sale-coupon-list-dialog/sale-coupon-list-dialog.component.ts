import { Component, Inject, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { SaleCouponPaged, SaleCouponService } from '../sale-coupon.service';

@Component({
  selector: 'app-sale-coupon-list-dialog',
  templateUrl: './sale-coupon-list-dialog.component.html',
  styleUrls: ['./sale-coupon-list-dialog.component.css']
})
export class SaleCouponListDialogComponent implements OnInit {
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  programId: string;
  title = 'Coupon';

  constructor(private couponService: SaleCouponService, public activeModal: NgbActiveModal,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettingsPopup }

  ngOnInit() {

    setTimeout(() => {
      this.loadDataFromApi();
    });


    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  stateGet(state) {
    switch (state) {
      case 'used':
        return 'Đã sử dụng';
      case 'expired':
        return 'Đã hết hạn';
      case 'reserved':
        return 'Để dành riêng';
      default:
        return 'Có giá trị';
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SaleCouponPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    if (this.programId) {
      val.programId = this.programId;
    }

    this.couponService.getPaged(val).pipe(
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

  exportFile() {
    var val = new SaleCouponPaged();
    val.search = this.search || '';
    if (this.programId) {
      val.programId = this.programId;
    }
    this.couponService.exportFile(val).subscribe(result => {
      let filename = 'danh_sach_coupon';
      let newBlob = new Blob([result], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement('a');
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }
}




