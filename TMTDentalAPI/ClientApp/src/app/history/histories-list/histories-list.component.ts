import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { HistoriesCreateUpdateComponent } from '../histories-create-update/histories-create-update.component';
import { HistoryPaged } from '../history';
import { HistoryImportExcelDialogComponent } from '../history-import-excel-dialog/history-import-excel-dialog.component';
import { HistoryService } from '../history.service';

@Component({
  selector: 'app-histories-list',
  templateUrl: './histories-list.component.html',
  styleUrls: ['./histories-list.component.css']
})
export class HistoriesListComponent implements OnInit {

  constructor(
    private service: HistoryService,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  loading = false;
  gridView: GridDataResult;
  windowOpened: boolean = false;
  skip = 0;
  pageSize = 20;
  pagerSettings: any;

  search: string;
  searchUpdate = new Subject<string>();

  ngOnInit() {
    this.getList();
    this.searchChange();
  }

  getList() {
    this.loading = true;
    var paged = new HistoryPaged();
    paged.limit = this.pageSize;
    paged.offset = this.skip;
    paged.search = this.search || '';

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
        this.skip = 0;
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
        this.notifyService.notify("success","L??u th??nh c??ng");
        this.getList();
      },
      er => { }
    )
  }

  importFromExcel() {
    let modalRef = this.modalService.open(HistoryImportExcelDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static',scrollable:true });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.result.then(() => {
      this.getList();
    }, () => {
    });
  }

  delete(id) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a ti???u s??? b???nh';
    modalRef.result.then(() => {
      this.notifyService.notify("success","X??a th??nh c??ng");
      this.service.delete(id).subscribe(
        () => { this.getList(); }
      );
    }, () => {
    });
  }

}
