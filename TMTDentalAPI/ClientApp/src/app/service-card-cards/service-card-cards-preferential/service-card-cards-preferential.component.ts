import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { ServiceCardCardsPreferentialCuDialogComponent } from '../service-card-cards-preferential-cu-dialog/service-card-cards-preferential-cu-dialog.component';
import { ServiceCardCardsPreferentialImportDialogComponent } from '../service-card-cards-preferential-import-dialog/service-card-cards-preferential-import-dialog.component';

@Component({
  selector: 'app-service-card-cards-preferential',
  templateUrl: './service-card-cards-preferential.component.html',
  styleUrls: ['./service-card-cards-preferential.component.css']
})
export class ServiceCardCardsPreferentialComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  dateFrom: Date;
  dateTo: Date;
  filterState = [
    {name:'Chưa kích hoạt', value: ''},
    {name:'Đã kích hoạt', value: ''},
    {name:'Tạm dừng', value: ''},
    {name:'Hết hạn', value: ''},
  ]
  constructor(
    private modalService: NgbModal,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    this.loadDataFromApi();

  }

  loadDataFromApi() {

  }

  onPageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    // ServiceCardCardsPreferentialCuDialogComponent
    const modalRef = this.modalService.open(ServiceCardCardsPreferentialCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Tạo thẻ ưu đãi dịch vụ";
    modalRef.result.then(result => {

    }, () => { });
  }

  onSearchChange(e) {

  }
  exportExcelFile() {

  }

  importExcelFile() {
    const modalRef = this.modalService.open(ServiceCardCardsPreferentialImportDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    // modalRef.componentInstance.type = 'supplier';
    modalRef.componentInstance.title = 'Import excel';
    modalRef.result.then(() => {
      
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item) {

  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa thẻ ưu đãi dịch vụ' ;

    modalRef.result.then(() => {
      
    }, (error) => {
      console.log(error);
    });
  }

  onChangeState(val){
    console.log(val);
    this
  }

}
