import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { HrPayrollStructureTypePaged, HrPayrollStructureTypeService } from '../hr-payroll-structure-type.service';
import { HrPayrollStructureTypeCreateComponent } from '../hr-payroll-structure-type-create/hr-payroll-structure-type-create.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-hr-payroll-structure-type-list',
  templateUrl: './hr-payroll-structure-type-list.component.html',
  styleUrls: ['./hr-payroll-structure-type-list.component.css']
})
export class HrPayrollStructureTypeListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  title = 'Loại cấu trúc lương';
  loading = false;
  opened = false;
  searchUpdate = new Subject<string>();
  search: string;
  constructor(
    private modalService: NgbModal,
    private hrPayrollStructureTypeService: HrPayrollStructureTypeService,
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new HrPayrollStructureTypePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search ? this.search : '';
    this.hrPayrollStructureTypeService.getPaged(val).pipe(
      map((response: any) =>
        (<GridDataResult>{
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

  createItem() {
    let modalRef = this.modalService.open(HrPayrollStructureTypeCreateComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: ' + this.title;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item) {
    let modalRef = this.modalService.open(HrPayrollStructureTypeCreateComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: ' + this.title;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;

    modalRef.result.then(() => {
      this.hrPayrollStructureTypeService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

}
