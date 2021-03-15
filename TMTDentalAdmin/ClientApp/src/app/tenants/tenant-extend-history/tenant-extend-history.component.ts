import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ConfirmDialogComponent } from '@shared/confirm-dialog/confirm-dialog.component';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { TenantExtendHistoryService } from '../tenant-extend-history.service';

@Component({
  selector: 'app-tenant-extend-history',
  templateUrl: './tenant-extend-history.component.html',
  styleUrls: ['./tenant-extend-history.component.css']
})
export class TenantExtendHistoryComponent implements OnInit {
  @Input() tenant: any;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  selectedIds: string[] = [];

  search: string;
  searchUpdate = new Subject<string>();

  constructor(
    private tenantExtendHistoryService: TenantExtendHistoryService,
    private notificationService: NotificationService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    if (this.tenant) {
      this.loadDataFromApi();
    }
  }

  loadDataFromApi() {
    this.loading = true;
    this.tenantExtendHistoryService.getAllByTenantId(this.tenant.id).pipe(
      map(response => (<GridDataResult>{
        data: response,
        total: response ? response.length : 0
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.body = "Bạn chắc chắn muốn xóa lịch sử gia hạn không";
    modalRef.componentInstance.title = `Xóa lịch sử gia hạn`;

    modalRef.result.then(() => {
      this.tenantExtendHistoryService.delete(item.id).subscribe(
        () => {
          this.loadDataFromApi();
          this.notificationService.show({
            content: 'Xóa thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }
      )
    });

  }

}
