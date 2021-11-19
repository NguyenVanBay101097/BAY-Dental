import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { ResInsuranceCuDialogComponent } from 'src/app/shared/res-insurance-cu-dialog/res-insurance-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { InsuranceIsActivePatch, ResInsurancePaged } from '../res-insurance.model';
import { ResInsuranceService } from '../res-insurance.service';

@Component({
  selector: 'app-res-insurance-list',
  templateUrl: './res-insurance-list.component.html',
  styleUrls: ['./res-insurance-list.component.css']
})
export class ResInsuranceListComponent implements OnInit {
  gridData: GridDataResult;
  limit: number = 20;
  skip: number = 0;
  pagerSettings: any;
  search: string;
  isActive: string;
  isDebt: string;
  searchUpdate = new Subject<string>();

  statesFilter: { text: string, value: string }[] = [
    { text: 'Đang liên kết', value: 'true' },
    { text: 'Ngưng liên kết', value: 'false' }
  ];

  debitsFilter: { text: string, value: string }[] = [
    { text: 'Có công nợ', value: 'true' },
    { text: 'Không có công nợ', value: 'false' }
  ];

  constructor(
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private resInsuranceService: ResInsuranceService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    this.loadDataFromApi();
    this.onSearchUpdate();
  }

  onSearchUpdate(): void {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi(): void {
    let val = new ResInsurancePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.isActive = this.isActive || '';
    val.isDebt = this.isDebt || '';
    this.resInsuranceService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
    }, err => {
      console.log(err);
    })
  }

  createItem(): void {
    const modalRef = this.modalService.open(ResInsuranceCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Thêm công ty bảo hiểm';
    modalRef.result.then(() => {
      this.notifyService.notify("success", "Lưu thành công")
      this.loadDataFromApi();
    }, () => { });
  }

  editItem(item: any): void {
    const modalRef = this.modalService.open(ResInsuranceCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Sửa công ty bảo hiểm';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify("success", "Lưu thành công")
      this.loadDataFromApi();
    }, () => { });
  }

  deleteItem(item) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', size: 'sm' });
    modalRef.componentInstance.title = 'Xóa công ty bảo hiểm';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa Công ty bảo hiểm?';
    modalRef.result.then(() => {
      this.resInsuranceService.remove(item.id).subscribe((res: any) => {
        this.notifyService.notify("success", "Xóa thành công")
        this.loadDataFromApi();
      })
    }, () => { });
  }

  activeInsurance(item: any): void {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', size: 'sm' });
    modalRef.componentInstance.title = `Liên kết với công ty bảo hiểm ${item.name}`;
    modalRef.componentInstance.body = `Bạn chắc chắn muốn liên kết với công ty bảo hiểm ${item.name}?`;
    modalRef.result.then(() => {
      let val = new InsuranceIsActivePatch();
      val.isActive = 'true';
      this.resInsuranceService.patchIsActive(item.id, val).subscribe((res: any) => {
        this.notifyService.notify("success", "Liên kết thành công")
        this.loadDataFromApi();
      })
    }, () => { });
  }

  inactiveInsurance(item: any): void {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', size: 'sm' });
    modalRef.componentInstance.title = `Ngưng liên kết với công ty bảo hiểm ${item.name}`;
    modalRef.componentInstance.body = `Bạn chắc chắn muốn ngưng liên kết với công ty bảo hiểm ${item.name}?`;
    modalRef.result.then(() => {
      let val = new InsuranceIsActivePatch();
      val.isActive = 'false';
      this.resInsuranceService.patchIsActive(item.id, val).subscribe((res: any) => {
        this.notifyService.notify("success", "Ngưng liên kết thành công")
        this.loadDataFromApi();
      })
    }, () => { });
  }

  onStateChange(data: any): void {
    this.isActive = data ? data.value : '';
    this.loadDataFromApi();
  }
  
  onDebtChange(data: any): void {
    this.isDebt = data ? data.value : '';
    this.loadDataFromApi();
  }

  onPageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }
}
