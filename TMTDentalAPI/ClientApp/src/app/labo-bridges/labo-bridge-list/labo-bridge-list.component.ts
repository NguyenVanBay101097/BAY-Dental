import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { LaboFinnishLineImportComponent } from 'src/app/labo-finish-lines/labo-finnish-line-import/labo-finnish-line-import.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboBridgeCuDialogComponent } from '../labo-bridge-cu-dialog/labo-bridge-cu-dialog.component';
import { LaboBridgeBasic, LaboBridgePaged, LaboBridgeService } from '../labo-bridge.service';

@Component({
  selector: 'app-labo-bridge-list',
  templateUrl: './labo-bridge-list.component.html',
  styleUrls: ['./labo-bridge-list.component.css']
})
export class LaboBridgeListComponent implements OnInit {
  constructor(
    private laboBridgeService: LaboBridgeService,
    private modalService: NgbModal
  ) { }

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  search: string;
  searchCategId: string;
  searchUpdate = new Subject<string>();

  opened = false;

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new LaboBridgePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.laboBridgeService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
      console.log(this.gridData);
      
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  onPageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(LaboBridgeCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm kiểu nhịp Labo';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: LaboBridgeBasic) {
    let modalRef = this.modalService.open(LaboBridgeCuDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa kiểu nhịp Labo';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item: LaboBridgeBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa kiểu nhịp Labo';
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa kiểu nhịp Labo ${item.name}?`;
    modalRef.result.then(() => {
      this.laboBridgeService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    }, () => {
    });
  }

  onImport() {
    let modalRef = this.modalService.open(LaboFinnishLineImportComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.componentInstance.type = 'labo_bridge';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }
}
