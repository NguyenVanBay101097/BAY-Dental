import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PhieuThuChiService } from '../phieu-thu-chi.service';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PhieuThuChiFormComponent } from '../phieu-thu-chi-form/phieu-thu-chi-form.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-phieu-thu-chi-list',
  templateUrl: './phieu-thu-chi-list.component.html',
  styleUrls: ['./phieu-thu-chi-list.component.css']
})
export class PhieuThuChiListComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  resultSelection: string;

  search: string;
  searchUpdate = new Subject<string>();
  
  constructor(private route: ActivatedRoute, private modalService: NgbModal, 
    private phieuThuChiService: PhieuThuChiService) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.resultSelection = params.get('result_selection');
      // this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        // this.loadDataFromApi();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    // this.loadDataFromApi();
  }

  convertResultSelection() {
    switch (this.resultSelection) {
      case 'thu':
        return 'phiếu thu';
      case 'chi':
        return 'phiếu chi';
    }
  }

  createItem() {
    const modalRef = this.modalService.open(PhieuThuChiFormComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm ' + this.convertResultSelection();
    modalRef.componentInstance.type = this.resultSelection;
    modalRef.result.then(() => {
      //  this.loadDataFromApi();
    }, () => {

    });
  }

  editItem(item: any /*phieuThuChi*/) {
    const modalRef = this.modalService.open(PhieuThuChiFormComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa ' + item.name;
    modalRef.componentInstance.itemId = item.id;
    modalRef.componentInstance.type = this.resultSelection;
    modalRef.result.then(() => {
      //  this.loadDataFromApi();
    }, () => {

    });
  }

  deleteItem(item: any /*loaiThuChiBasic*/) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + this.convertResultSelection();

    modalRef.result.then(() => {
      /*
      this.phieuThuChiService.delete(item.id).subscribe(() => {
        // this.loadDataFromApi();
      }, () => {
      });
      */
    }, () => {
    });
  }
}
