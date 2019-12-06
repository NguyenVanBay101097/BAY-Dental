import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleCouponService, SaleCouponPaged, SaleCouponBasic } from '../sale-coupon.service';

@Component({
  selector: 'app-sale-coupon-list',
  templateUrl: './sale-coupon-list.component.html',
  styleUrls: ['./sale-coupon-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleCouponListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  programId: string;

  constructor(private couponService: SaleCouponService, private route: ActivatedRoute,
    private router: Router, private modalService: NgbModal) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.programId = params.get('program_id');
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
    this.loadDataFromApi();
  }

  createItem() {
    var queryParams = this.route.snapshot.queryParams;
    this.router.navigate(['/coupon-programs/form'], { queryParams: queryParams });
  }

  editItem(item: SaleCouponBasic) {
    var queryParams = this.route.snapshot.queryParams;
    queryParams.id = item.id;
    this.router.navigate(['/coupon-programs/form'], { queryParams: queryParams });
  }

  deleteItem(item) {
  }
}



