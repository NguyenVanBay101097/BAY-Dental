import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintPaperSizeCreateUpdateDialogComponent } from '../print-paper-size-create-update-dialog/print-paper-size-create-update-dialog.component';
import { PrintPaperSizePaged, PrintPaperSizeService } from '../print-paper-size.service';

@Component({
  selector: 'app-print-paper-size-list',
  templateUrl: './print-paper-size-list.component.html',
  styleUrls: ['./print-paper-size-list.component.css']
})
export class PrintPaperSizeListComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  
  search: string;
  searchUpdate = new Subject<string>();
  
  constructor(private route: ActivatedRoute, 
    private modalService: NgbModal, 
    private printPaperSizeService: PrintPaperSizeService) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PrintPaperSizePaged();
    val.offset = this.skip;
    val.limit = this.limit;
    val.search = this.search || '';

    this.printPaperSizeService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
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

  createItem() {
    const modalRef = this.modalService.open(PrintPaperSizeCreateUpdateDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khổ giấy in';
    modalRef.result.then(() => {
       this.loadDataFromApi();
    }, () => {

    });
  }

  editItem(item) {
    const modalRef = this.modalService.open(PrintPaperSizeCreateUpdateDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa khổ giấy in';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
       this.loadDataFromApi();
    }, () => {

    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa khổ giấy in';

    modalRef.result.then(() => {
      this.printPaperSizeService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

}
