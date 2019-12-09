import { Component, OnInit } from '@angular/core';
import { ResPartnerBankService } from '../res-partner-bank.service';
import { DialogService, DialogCloseResult, DialogRef } from '@progress/kendo-angular-dialog';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { SortDescriptor } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { ResPartnerBankCreateUpdateComponent } from '../res-partner-bank-create-update/res-partner-bank-create-update.component';
import { ResPartnerBankPaged } from '../res-partner-bank';
import { AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-res-partner-bank-list',
  templateUrl: './res-partner-bank-list.component.html',
  styleUrls: ['./res-partner-bank-list.component.css']
})
export class ResPartnerBankListComponent implements OnInit {

  loading = false;
  gridView: GridDataResult;
  skip = 0;
  pageSize = 20;

  searchName: string;
  searchNameUpdate = new Subject<string>();
  type = ["bank", "cash"];

  sort: SortDescriptor[] = [{
    field: 'name',
    dir: 'asc'
  }];


  constructor(private service: ResPartnerBankService, private modalService: NgbModal, private dialogService: DialogService,
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.getJournals();
    this.searchChange();
  }

  getJournals() {
    this.loading = true;
    var filter = new AccountJournalFilter();
    filter.limit = this.pageSize;
    filter.offset = this.skip;
    filter.search = this.searchName || '';
    filter.type = this.type.join(',');

    this.service.getPaged(filter).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.loading = false;
    }, er => {
      this.loading = true;
      console.log(er);
    }
    )
  }

  searchChange() {
    this.searchNameUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getJournals();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
    this.getJournals();
  }

  sortChange(sort: SortDescriptor[]): void {
    this.sort = sort;
    this.getJournals();
  }


  openModal(id) {
    const modalRef = this.modalService.open(ResPartnerBankCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(
      rs => {
        this.getJournals();
      },
      er => { }
    )
  }

  // deleteAccount(id, event) {
  //   event.stopPropagation();
  //   const dialogRef: DialogRef = this.dialogService.open({
  //     title: 'Xóa tài khoản ngân hàng',
  //     content: 'Bạn chắc chắn muốn xóa?',
  //     width: 450,
  //     height: 200,
  //     minWidth: 250,
  //     actions: [
  //       { text: 'Hủy', value: false },
  //       { text: 'Đồng ý', primary: true, value: true }
  //     ]
  //   });
  //   dialogRef.result.subscribe(
  //     rs => {
  //       if (!(rs instanceof DialogCloseResult)) {
  //         if (rs['value']) {
  //           this.service.deletePartnerBank(id).subscribe(
  //             () => { this.getJournals(); }
  //           );
  //         }
  //       }
  //     }
  //   )
  // }

  deactive(id) {
    event.stopPropagation();
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa tài khoản',
      content: 'Bạn chắc chắn muốn xóa?',
      width: 450,
      height: 200,
      minWidth: 250,
      actions: [
        { text: 'Hủy', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ]
    });
    dialogRef.result.subscribe(
      rs => {
        if (!(rs instanceof DialogCloseResult)) {
          if (rs['value']) {
            this.service.deactive([id]).subscribe(
              rs => {
                this.getJournals();
                this.notificationService.show({
                  content: 'Đã xóa',
                  hideAfter: 3000,
                  position: { horizontal: 'right', vertical: 'bottom' },
                  animation: { type: 'fade', duration: 400 },
                  type: { style: 'warning', icon: true }
                });
              }
            )
          }
        }
      }
    )
  }


}
