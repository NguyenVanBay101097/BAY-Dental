import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { TenantSaleOrderProcessUpdateService, TenantSaleOrderProcessUpdatePaged } from '../tenant-sale-order-process-update.service';

@Component({
  selector: 'app-tenant-sale-order-process-update-list',
  templateUrl: './tenant-sale-order-process-update-list.component.html',
  styleUrls: ['./tenant-sale-order-process-update-list.component.css']
})

export class TenantSaleOrderProcessUpdateListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  selectedIds: string[] = [];

  search: string;
  searchUpdate = new Subject<string>();

  constructor(
    private processUpdateService: TenantSaleOrderProcessUpdateService,
    private notificationService: NotificationService,
    private modalService: NgbModal) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new TenantSaleOrderProcessUpdatePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.processUpdateService.getPaged(val).pipe(
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

  loadAll() {
    this.processUpdateService.loadAll().subscribe(() => {
      this.loadDataFromApi();
    })
  }

  processAll() {
    this.processUpdateService.processAll().subscribe(() => {
      this.loadDataFromApi();
    })
  }
}

