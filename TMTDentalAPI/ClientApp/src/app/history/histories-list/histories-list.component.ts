import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HistoryService } from '../history.service';
import { DialogService, DialogCloseResult, DialogRef } from '@progress/kendo-angular-dialog';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { HistoriesCreateUpdateComponent } from '../histories-create-update/histories-create-update.component';
import { HistoryPaged } from '../history';

@Component({
  selector: 'app-histories-list',
  templateUrl: './histories-list.component.html',
  styleUrls: ['./histories-list.component.css']
})
export class HistoriesListComponent implements OnInit {

  constructor(private fb: FormBuilder, private service: HistoryService,
    private dialogService: DialogService, private modalService: NgbModal) { }

  loading = false;
  gridView: GridDataResult;
  windowOpened: boolean = false;
  skip = 0;
  pageSize = 20;

  formFilter: FormGroup;
  search: string;
  searchUpdate = new Subject<string>();

  ngOnInit() {
    this.formFilter = this.fb.group({
      search: null
    });
    this.getList();
    this.searchChange();
  }

  getList() {
    this.loading = true;
    var paged = new HistoryPaged();
    paged = this.formFilter.value;
    paged.limit = this.pageSize;
    paged.offset = this.skip;

    this.service.getList(paged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.loading = false;
    }, er => {
      this.loading = false;
      console.log(er);
    }
    )
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getList();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
    this.getList();
  }

  openModal(id) {
    const modalRef = this.modalService.open(HistoriesCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (id) {
      modalRef.componentInstance.id = id;
    }
    modalRef.result.then(
      rs => {
        this.getList();
      },
      er => { }
    )
  }

  delete(id) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa tiểu sử',
      content: 'Bạn chắc chắn muốn xóa tiểu sử này ?',
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
            this.service.delete(id).subscribe(
              () => { this.getList(); }
            );
          }
        }
      }
    )
  }

}
