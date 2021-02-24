import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { EmployeeAdminRegisterComponent } from '../employee-admin-register/employee-admin-register.component';
import { EmployeeAdminPaged, EmployeeAdminService } from '../employee-admin.service';

@Component({
  selector: 'app-employee-admin-list',
  templateUrl: './employee-admin-list.component.html',
  styleUrls: ['./employee-admin-list.component.css']
})
export class EmployeeAdminListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  selectedIds: string[] = [];

  search: string;
  searchUpdate = new Subject<string>();

  constructor(
    private employeeAdminService: EmployeeAdminService,
    private notificationService: NotificationService,
    private modalService: NgbModal) { }

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
    var val = new EmployeeAdminPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.employeeAdminService.getPaged(val).pipe(
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

  editItem(dataItem) {
    let modalRef = this.modalService.open(EmployeeAdminRegisterComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.id = dataItem.id;
    modalRef.componentInstance.title = 'Cập nhật nhân viên';

    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    });
  }

}
