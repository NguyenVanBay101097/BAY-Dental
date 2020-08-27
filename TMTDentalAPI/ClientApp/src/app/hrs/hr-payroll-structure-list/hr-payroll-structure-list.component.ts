import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HrPayrollStructureService, HrPayrollStructurePaged } from '../hr-payroll-structure.service';
import { Router } from '@angular/router';
import { map } from 'rxjs/internal/operators/map';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-payroll-structure-list',
  templateUrl: './hr-payroll-structure-list.component.html',
  styleUrls: ['./hr-payroll-structure-list.component.css']
})
export class HrPayrollStructureListComponent implements OnInit {

  gridData: GridDataResult = {
    data: [],
    total: 0
  };
  pageSize = 20;
  page = 1;
  loading = false;
  collectionSize = 0;
  search: string;

  constructor(
    private HrPayrollStructureService: HrPayrollStructureService,
    private modalService: NgbModal,
    private router: Router) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    const val = new HrPayrollStructurePaged();
    val.limit = this.pageSize;
    val.offset = (this.page - 1) * this.pageSize;
    if (this.search) { val.filter = this.search; }

    this.HrPayrollStructureService.getPaged(val).pipe(
      map((res: any) => ({
        data: res.items,
        total: res.totalItems,
      } as GridDataResult))
    )
      .subscribe(res => {
        this.gridData = res;
        this.collectionSize = this.gridData.total;
        this.loading = false;
      }, err => {
        console.log(err);
        this.loading = false;
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/hr/payroll-structures/create']);
  }

  editItem(dataitem) {
    this.router.navigate(['/hr/payroll-structures/edit/' + dataitem.id]);
  }

  deleteItem(dataitem) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa mẫu lương';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.HrPayrollStructureService.delete(dataitem.id).subscribe(res => {
        this.loadDataFromApi();
      });
    });
  }
}
