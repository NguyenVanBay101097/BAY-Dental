import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { FacebookMassMessagingService, FacebookMassMessagingPaged } from '../facebook-mass-messaging.service';

@Component({
  selector: 'app-facebook-mass-messaging-list',
  templateUrl: './facebook-mass-messaging-list.component.html',
  styleUrls: ['./facebook-mass-messaging-list.component.css']
})
export class FacebookMassMessagingListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  constructor(private massMessageService: FacebookMassMessagingService,
    private router: Router, private modalService: NgbModal) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new FacebookMassMessagingPaged();
    val.limit = this.limit;
    val.offset = this.skip;

    this.massMessageService.getPaged(val).pipe(
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

  stateGet(state) {
    switch (state) {
      case 'done':
        return 'Đã gửi';
      case 'in_queue':
        return 'Chờ gửi';
      default:
        return 'Nháp';
    }
  }

  editItem(item: any) {
    this.router.navigate(['/facebook-management/mass-messagings/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa nhắn tin hàng hoạt';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.massMessageService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}

