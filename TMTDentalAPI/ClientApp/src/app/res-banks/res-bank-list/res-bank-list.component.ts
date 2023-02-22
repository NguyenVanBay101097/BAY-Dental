import { Component, OnInit } from '@angular/core';
import { ResBankService } from '../res-bank.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { SortDescriptor } from '@progress/kendo-data-query';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ResBankPaged } from '../res-bank';
import { DialogCloseResult, DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ResBankCreateUpdateComponent } from '../res-bank-create-update/res-bank-create-update.component';

@Component({
  selector: 'app-res-bank-list',
  templateUrl: './res-bank-list.component.html',
  styleUrls: ['./res-bank-list.component.css']
})
export class ResBankListComponent implements OnInit {

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

  constructor(private service: ResBankService, private modalService: NgbModal, private dialogService: DialogService) { }

  ngOnInit() {
    this.getBankList();
    this.searchChange();
  }

  getBankList() {
    this.loading = true;
    var rbPaged = new ResBankPaged();
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
    const modalRef = this.modalService.open(ResBankCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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
      title: 'Xóa ngân hàng',
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
            this.service.deleteBank(id).subscribe(
              () => { this.getBankList(); }
            );
          }
        }
      }
    )
  }
}
