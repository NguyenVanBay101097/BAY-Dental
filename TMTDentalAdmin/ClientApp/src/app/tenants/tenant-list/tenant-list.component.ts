import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { TenantService, TenantPaged } from '../tenant.service';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TenantUpdateExpiredDialogComponent } from '../tenant-update-expired-dialog/tenant-update-expired-dialog.component';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-tenant-list',
  templateUrl: './tenant-list.component.html',
  styleUrls: ['./tenant-list.component.css']
})

export class TenantListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  selectedIds: string[] = [];

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private tenantService: TenantService, private router: Router, private modalService: NgbModal) { }

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
    var val = new TenantPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.tenantService.getPaged(val).pipe(
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

  updateExpired(dataItem) {
    let modalRef = this.modalService.open(TenantUpdateExpiredDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.id = dataItem.id;
    modalRef.componentInstance.title = `Gia hạn: ${dataItem.hostname}`;
    modalRef.componentInstance.dateExpired = dataItem.dateExpired ? new Date(dataItem.dateExpired) : null;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    });
  }
}
