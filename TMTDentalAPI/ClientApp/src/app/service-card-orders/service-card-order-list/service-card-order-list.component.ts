import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ServiceCardOrderPaged } from '../service-card-order-paged';
import { ServiceCardOrderService } from '../service-card-order.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-service-card-order-list',
  templateUrl: './service-card-order-list.component.html',
  styleUrls: ['./service-card-order-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class ServiceCardOrderListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  title = 'Đơn bán thẻ dịch vụ';

  constructor(private cardOrderService: ServiceCardOrderService,
    private modalService: NgbModal, private router: Router) { }

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
    var val = new ServiceCardOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.cardOrderService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
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
    this.router.navigate(['/service-card-orders/form']);
  }

  editItem(item: any) {
    this.router.navigate(['/service-card-orders/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.cardOrderService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}




