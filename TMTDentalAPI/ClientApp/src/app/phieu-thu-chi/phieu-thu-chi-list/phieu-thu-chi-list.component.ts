import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PhieuThuChiService, phieuThuChiPaged, phieuThuChi } from '../phieu-thu-chi.service';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PhieuThuChiFormComponent } from '../phieu-thu-chi-form/phieu-thu-chi-form.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { loaiThuChiBasic } from 'src/app/loai-thu-chi/loai-thu-chi.service';

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
    private phieuThuChiService: PhieuThuChiService, private router: Router) { }

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
    var val = new phieuThuChiPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.resultSelection;

    this.phieuThuChiService.getPaged(val).pipe(
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
        return 'phiếu thu';
      case 'chi':
        return 'phiếu chi';
    }
  }

  getTypePayerReceiver() {
    switch (this.resultSelection) {
      case 'thu':
        return 'Người nhận tiền';
      case 'chi':
        return 'Người nộp tiền';
    }
  }

  createItem() {
    this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { result_selection: this.resultSelection } });
  }

  editItem(item: any) {
    this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { id: item.id, result_selection: this.resultSelection } });
  }

  deleteItem(item: loaiThuChiBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + this.convertResultSelection();

    modalRef.result.then(() => {
      this.phieuThuChiService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }
}
