import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ServiceCardCardPaged } from '../service-card-card-paged';
import { ServiceCardCardService } from '../service-card-card.service';
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
  today: Date = new Date();
  activatedDate: any;
  expiredDateFrom: any;
  expiredDateTo: any;
  state: string;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  filterState = [
    { name: 'Chưa kích hoạt', value: 'draft' },
    { name: 'Đã kích hoạt', value: 'in_use' },
    { name: 'Tạm dừng', value: 'locked' },
    { name: 'Hết hạn', value: 'cancelled' },
  ]
  constructor(
    private modalService: NgbModal,
    private serviceCardsService: ServiceCardCardService,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    this.expiredDateFrom = this.monthStart;
    this.expiredDateTo = this.monthEnd;
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
    let val = new ServiceCardCardPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search ? this.search : '';
    val.state = this.state ? this.state : '';
    // val.activatedDate = this.activatedDate ? moment(this.activatedDate).format('YYYY-MM-DD') : '';
    // val.expiredDateFrom = this.expiredDateFrom ? moment(this.expiredDateFrom).format('YYYY-MM-DD') : '';
    // val.expiredDateTo = this.expiredDateTo ? moment(this.expiredDateTo).format('YYYY-MM-DD') : '';
    
    this.serviceCardsService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.gridData = res;

    });
  }

  onPageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    const modalRef = this.modalService.open(ServiceCardCardsPreferentialCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Tạo thẻ ưu đãi dịch vụ";
    modalRef.result.then(result => {
      
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => { });
  }

  editItem(item) {
    const modalRef = this.modalService.open(ServiceCardCardsPreferentialCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Chỉnh sửa thẻ " + item.barcode;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(result => {
      console.log(result);

      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, () => { });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa thẻ ưu đãi dịch vụ';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa thẻ ưu đãi dịch vụ này?';

    modalRef.result.then(() => {
      this.serviceCardsService.delete(item.id).subscribe((res: any) => {
        this.notifyService.notify('success', 'Xóa thành công');
        this.loadDataFromApi();
      });
    }, (error) => {
      console.log(error);
    });
  }

  onSearchChange(data) {
    this.expiredDateFrom = data.dateFrom;
    this.expiredDateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  exportExcelFile() {
    let val = new ServiceCardCardPaged();
    val.limit = 20;
    val.offset = 0;

    let todayStr = moment(new Date()).format("YYYYMMDD");
    this.serviceCardsService.exportExcel(val).subscribe((res) => {
      let filename = `The_uu_dai_dich_vu_${todayStr}`;

      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  importExcelFile() {
    const modalRef = this.modalService.open(ServiceCardCardsPreferentialImportDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'Import excel';
    modalRef.result.then((result) => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  onChangeState(e) {
    this.state = e ? e.value : '';
    this.skip = 0;
    this.loadDataFromApi();
  }

  actionLock() {
    if (this.selectedIds.length) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Tạm dừng thẻ ưu đãi dịch vụ';
      modalRef.componentInstance.body = 'Bạn có chắc chắn muốn dừng thẻ này?';
      modalRef.componentInstance.body2 = 'Lưu ý: Chỉ tạm dừng các thẻ ưu đãi dịch vụ đã kích hoạt';
      modalRef.result.then(() => {
        this.serviceCardsService.buttonLock(this.selectedIds).subscribe((res: any) => {
          this.notifyService.notify('success', 'Tạm dừng thành công');
          this.loadDataFromApi();
        });
      }, (error) => { console.log(error) });
    }
  }

  actionActive() {
    if (this.selectedIds.length) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Kích hoạt thẻ ưu đãi dịch vụ';
      modalRef.componentInstance.body = 'Bạn có chắc chắn muốn kích hoạt thẻ ưu đãi dịch vụ này?';
      modalRef.result.then(() => {
        this.serviceCardsService.buttonActive(this.selectedIds).subscribe((res: any) => {
          this.notifyService.notify('success', 'Kích hoạt thành công');
          this.loadDataFromApi();
        });
      }, (error) => { console.log(error) });
    }
  }

  actionCancel() {
    if (this.selectedIds.length) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Hủy thẻ ưu đãi dịch vụ';
      modalRef.componentInstance.body = 'Bạn có chắc chắn muốn hủy thẻ ưu đãi dịch vụ này?';
      modalRef.componentInstance.body2 = 'Lưu ý: Các thẻ sau khi hủy sẽ không thể kích hoạt để sử dụng lại';
      modalRef.result.then(() => {
        this.serviceCardsService.buttonCancel(this.selectedIds).subscribe((res: any) => {
          this.notifyService.notify('success', 'Hủy thành công');
          this.loadDataFromApi();
        });
      }, (error) => { console.log(error) });
    }
  }

  onChangeActivatedDate(e) {
    this.activatedDate = e;
    this.skip = 0;
    this.loadDataFromApi();
  }
}
