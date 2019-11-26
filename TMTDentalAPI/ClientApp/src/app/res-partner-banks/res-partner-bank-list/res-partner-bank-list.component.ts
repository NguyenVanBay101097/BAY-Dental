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

  searchNameBIC: string;
  searchNameBICUpdate = new Subject<string>();


  sort: SortDescriptor[] = [{
    field: 'name',
    dir: 'asc'
  }];

  constructor(private service: ResPartnerBankService, private modalService: NgbModal, private dialogService: DialogService) { }

  ngOnInit() {
    this.getBankList();
    this.searchChange();
  }

  getBankList() {
    this.loading = true;
    var rbPaged = new ResPartnerBankPaged();
    rbPaged.limit = this.pageSize;
    rbPaged.offset = this.skip;
    rbPaged.search = this.searchNameBIC || '';

    this.service.getPaged(rbPaged).pipe(
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
    this.searchNameBICUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getBankList();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
    this.getBankList();
  }

  sortChange(sort: SortDescriptor[]): void {
    this.sort = sort;
    this.getBankList();
  }


  openModal(id) {
    const modalRef = this.modalService.open(ResPartnerBankCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(
      rs => {
        this.getBankList();
      },
      er => { }
    )
  }

  deleteBank(id, event) {
    event.stopPropagation();
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa đối tác',
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
            this.service.deletePartnerBank(id).subscribe(
              () => { this.getBankList(); }
            );
          }
        }
      }
    )
  }

}
