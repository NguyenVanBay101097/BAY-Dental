import { Component, OnInit, Input } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CardCardCuDialogComponent } from 'src/app/card-cards/card-card-cu-dialog/card-card-cu-dialog.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { CardCardPaged, CardCardService } from 'src/app/card-cards/card-card.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { Subject } from 'rxjs';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-card-card-list',
  templateUrl: './card-card-list.component.html',
  styleUrls: ['./card-card-list.component.css']
})
export class CardCardListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  stateFilter: string = '';

  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Nháp', value: 'draft' },
    { text: 'Chờ cấp thẻ', value: 'confirmed' },
    { text: 'Đang sử dụng', value: 'in_use' },
    { text: 'Đã khóa', value: 'locked' },
    { text: 'Đã hủy', value: 'cancelled' }
  ];

  constructor(private modalService: NgbModal, private cardCardService: CardCardService) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  onStateSelectChange(data: TmtOptionSelect) {
    this.stateFilter = data.value;
    this.loadDataFromApi();
  }

  addNewCard() {
    let modalRef = this.modalService.open(CardCardCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CardCardPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    if (this.stateFilter) {
      val.state = this.stateFilter;
    }

    this.cardCardService.getPaged(val).pipe(
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

  onEditClick(item) {
    let modalRef = this.modalService.open(CardCardCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  onDeleteClick(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa thẻ thành viên';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.cardCardService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }

  exportExcelFile() {
    var paged = new CardCardPaged();
    paged.search = this.search || '';
    paged.state = this.stateFilter;
    this.cardCardService.excelServerExport(paged).subscribe(
      rs => {
        let filename = 'ExportedExcelFile';
        let newBlob = new Blob([rs], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        console.log(rs);

        let data = window.URL.createObjectURL(newBlob);
        let link = document.createElement('a');
        link.href = data;
        link.download = filename;
        link.click();
        setTimeout(() => {
          // For Firefox it is necessary to delay revoking the ObjectURL
          window.URL.revokeObjectURL(data);
        }, 100);
      }
    );
  }
}
