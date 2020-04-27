import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ServiceCardCardService } from '../service-card-card.service';
import { ServiceCardCardPaged } from '../service-card-card-paged';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-service-card-card-list',
  templateUrl: './service-card-card-list.component.html',
  styleUrls: ['./service-card-card-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ServiceCardCardListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  title = 'Thẻ dịch vụ';
  orderId: string;

  constructor(private cardCardService: ServiceCardCardService,
    private modalService: NgbModal, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe((param: ParamMap) => {
      this.orderId = param.get('order_id');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ServiceCardCardPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    if (this.orderId) {
      val.orderId = this.orderId;
    }

    this.cardCardService.getPaged(val).pipe(
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

  stateGet(state) {
    switch (state) {
      case 'in_use':
        return 'Đang sử dụng';
      default:
        return 'Nháp';
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {

  }

  editItem(item: any) {

  }

  deleteItem(item) {
  }
}





