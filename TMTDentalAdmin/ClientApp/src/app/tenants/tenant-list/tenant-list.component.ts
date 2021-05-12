import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { TenantService, TenantPaged } from '../tenant.service';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TenantUpdateExpiredDialogComponent } from '../tenant-update-expired-dialog/tenant-update-expired-dialog.component';
import { Subject } from 'rxjs';
import { TenantUpdateInfoDialogComponent } from '../tenant-update-info-dialog/tenant-update-info-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';
import { TenantUpdateExpiredV2DialogComponent } from '../tenant-update-expired-v2-dialog/tenant-update-expired-v2-dialog.component';

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

  constructor(
    private tenantService: TenantService,
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
      console.log(res);
      
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
    let modalRef = this.modalService.open(TenantUpdateExpiredDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = dataItem.id;
    modalRef.componentInstance.title = `Gia hạn: ${dataItem.hostname}`;
    modalRef.componentInstance.expirationDate = dataItem.dateExpired ? new Date(dataItem.dateExpired) : null;
    modalRef.componentInstance.tenant = {
      dateExpired: dataItem.dateExpired ? new Date(dataItem.dateExpired) : null,
      activeCompaniesNbr: dataItem.activeCompaniesNbr
    };
    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Gia hạn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    });
  }

  quickUpdateExpired(dataItem) {
    let modalRef = this.modalService.open(TenantUpdateExpiredV2DialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.item = dataItem;
    modalRef.componentInstance.title = `Gia hạn nhanh: ${dataItem.hostname}`;
    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Gia hạn nhanh thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    });
  }

  editItem(dataItem) {
    let modalRef = this.modalService.open(TenantUpdateInfoDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = dataItem.id;
    modalRef.componentInstance.title = `Cập nhật thông tin: ${dataItem.hostname}`;

    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    });
  }
}
