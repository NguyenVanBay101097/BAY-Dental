import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ToothDiagnosisCreateUpdateDialogComponent } from '../tooth-diagnosis-create-update-dialog/tooth-diagnosis-create-update-dialog.component';
import { ToothDiagnosisPaged, ToothDiagnosisService } from '../tooth-diagnosis.service';

@Component({
  selector: 'app-tooth-diagnosis-list',
  templateUrl: './tooth-diagnosis-list.component.html',
  styleUrls: ['./tooth-diagnosis-list.component.css']
})
export class ToothDiagnosisListComponent implements OnInit {

  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  
  search: string;
  searchUpdate = new Subject<string>();
  constructor(
    private modalService: NgbModal,
    private toothDiagnosisService: ToothDiagnosisService,
    private notificationService: NotificationService,
    private authService : AuthService
  ) { }

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
    var val = new ToothDiagnosisPaged();
    val.offset = this.skip;
    val.limit = this.limit;
    val.search = this.search || '';
  

    this.toothDiagnosisService.getPaged(val).pipe(
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

  createItem(){
    const modalRef = this.modalService.open(ToothDiagnosisCreateUpdateDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm chẩn đoán răng';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {

    });
  }

  editItem(item:any){
    const modalRef = this.modalService.open(ToothDiagnosisCreateUpdateDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa chẩn đoán răng';
    modalRef.componentInstance.itemId = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {

    });
  }

  deleteItem(item:any){
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', });
    modalRef.componentInstance.title = 'Xóa chẩn đoán răng';
    modalRef.componentInstance.body = "Bạn có chắc chắn muốn xóa chẩn đoán răng?"
    modalRef.result.then(() => {
      this.toothDiagnosisService.delete(item.id).subscribe(() => {
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent){
    this.skip = event.skip;
    this.loadDataFromApi();
  }
}
