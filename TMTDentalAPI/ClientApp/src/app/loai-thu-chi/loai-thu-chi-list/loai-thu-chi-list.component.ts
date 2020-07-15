import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LoaiThuChiFormComponent } from '../loai-thu-chi-form/loai-thu-chi-form.component';
import { LoaiThuChiService, loaiThuChiPaged, loaiThuChiBasic } from '../loai-thu-chi.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-loai-thu-chi-list',
  templateUrl: './loai-thu-chi-list.component.html',
  styleUrls: ['./loai-thu-chi-list.component.css']
})
export class LoaiThuChiListComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  resultSelection: string;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, private modalService: NgbModal, 
    private loaiThuChiService: LoaiThuChiService) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.resultSelection = params.get('result_selection');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }
  
  loadDataFromApi() {
    this.loading = true;
    var val = new loaiThuChiPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.resultSelection;

    this.loaiThuChiService.getPaged(val).pipe(
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

  convertResultSelection() {
    switch (this.resultSelection) {
      case 'thu':
        return 'loại thu';
      case 'chi':
        return 'loại chi';
    }
  }

  createItem() {
    const modalRef = this.modalService.open(LoaiThuChiFormComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm ' + this.convertResultSelection();
    modalRef.componentInstance.type = this.resultSelection;
    modalRef.result.then(() => {
       this.loadDataFromApi();
    }, () => {

    });
  }

  editItem(item: loaiThuChiBasic) {
    const modalRef = this.modalService.open(LoaiThuChiFormComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa ' + item.name;
    modalRef.componentInstance.itemId = item.id;
    modalRef.result.then(() => {
       this.loadDataFromApi();
    }, () => {

    });
  }

  deleteItem(item: loaiThuChiBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + this.convertResultSelection();

    modalRef.result.then(() => {
      this.loaiThuChiService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }
}
