import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { CommissionDialogComponent } from '../commission-dialog/commission-dialog.component';
import { CommissionPaged, CommissionService } from '../commission.service';

@Component({
  selector: 'app-commission-list-v2',
  templateUrl: './commission-list-v2.component.html',
  styleUrls: ['./commission-list-v2.component.css'],
  host: {'class': 'h-100'}
})
export class CommissionListV2Component implements OnInit {
  
  searchCommission: string;
  sourceCommissions: any[];
  commissions: any[];
  commission: any;
  checkCreate: boolean = false;

  constructor(
    private commissionService: CommissionService, 
    private modalService: NgbModal,
    private notifyService: NotifyService
    ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  onSearchChange(val: string) {
    this.sourceCommissions = this.commissions.filter(x => x.name.toLowerCase().includes(val.toLowerCase()));
  }

  loadDataFromApi() {
    var val = new CommissionPaged();
    // val.limit = 0;
    val.offset = 0;
    val.search = '';
    this.commissionService.getPaged(val).subscribe(res => {
      this.sourceCommissions = res.items;
      if (!this.searchCommission) {
        this.commissions = this.sourceCommissions;
      }
      if (this.checkCreate) {
        if (this.commissions.length > 0) {
          this.commission = this.commissions[0];
        }
        this.checkCreate = false;
      }
    }, err => {
      console.log(err);
    })
  }

  onSelectCommission(item: any) {
    if (this.commission === item) {
      this.commission = null;
    } else {
      this.commission = item;
    }
  }

  createCommission() {
    let modalRef = this.modalService.open(CommissionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm bảng hoa hồng';
    modalRef.result.then(result => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.checkCreate = true;
      this.loadDataFromApi();
    }, () => {
    });
  }

  editCommission(item: any, i) {
    let modalRef = this.modalService.open(CommissionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa bảng hoa hồng';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(result => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteCommission(item: any, i) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa bảng hoa hồng';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa Bảng hoa hồng?';
    modalRef.result.then(() => {
      this.commissionService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success', 'Xóa thành công');
        this.loadDataFromApi();
        if (item.id == this.commission.id) {
          this.commission = null;
        }
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

}
