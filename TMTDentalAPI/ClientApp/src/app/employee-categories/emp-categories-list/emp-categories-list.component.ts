import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { FormGroup, FormBuilder } from '@angular/forms';
import { map, distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DialogRef, DialogCloseResult, WindowService, DialogService, WindowRef, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { EmployeeCategoryPaged } from '../emp-category';
import { EmpCategoryService } from '../emp-category.service';
import { EmpCategoriesCreateUpdateComponent } from '../emp-categories-create-update/emp-categories-create-update.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-emp-categories-list',
  templateUrl: './emp-categories-list.component.html',
  styleUrls: ['./emp-categories-list.component.css']
})
export class EmpCategoriesListComponent implements OnInit {

  constructor(private fb: FormBuilder, private service: EmpCategoryService, private modalService: NgbModal, private dialogService: DialogService) { }

  loading = false;
  gridView: GridDataResult;
  windowOpened: boolean = false;
  skip = 0;
  pageSize = 20;

  formFilter: FormGroup;
  search: string;
  searchUpdate = new Subject<string>();

  ngOnInit() {
    this.formFilter = this.fb.group({
      search: null
    });

    this.getCategEmployeesList();
    this.searchChange();

  }

  getCategEmployeesList() {
    this.loading = true;
    var empPaged = new EmployeeCategoryPaged();
    empPaged = this.formFilter.value;
    empPaged.limit = this.pageSize;
    empPaged.offset = this.skip;

    this.service.getCategEmployeePaged(empPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.loading = false;
    }, er => {
      this.loading = true;
      console.log(er);
    }
    )
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getCategEmployeesList();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.getCategEmployeesList();
  }

  // openWindow(id) {
  //   const windowRef: WindowRef = this.windowService.open(
  //     {
  //       title: 'Nhóm nhân viên',
  //       content: EmpCategoriesCreateUpdateComponent,
  //       minWidth: 250,
  //     });
  //   this.windowOpened = true;
  //   const instance = windowRef.content.instance;
  //   instance.empCategId = id;

  //   windowRef.result.subscribe(
  //     (result) => {
  //       this.windowOpened = false;
  //       if (!(result instanceof WindowCloseResult)) {
  //         this.getCategEmployeesList();
  //       }
  //     }
  //   )
  // }
  openModal(id) {
    const modalRef = this.modalService.open(EmpCategoriesCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (id) {
      modalRef.componentInstance.empCategId = id;
    }
    modalRef.result.then(
      rs => {
        this.getCategEmployeesList();
      },
      er => { }
    )
  }

  deleteCategEmployee(id) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa nhóm nhân viên',
      content: 'Bạn chắc chắn muốn xóa nhóm nhân viên này ?',
      width: 450,
      height: 200,
      minWidth: 250,
      actions: [
        { text: 'Hủy', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ]
    });
    dialogRef.result.subscribe(
      rs => {
        if (!(rs instanceof DialogCloseResult)) {
          if (rs['value']) {
            this.service.deleteCategEmployee(id).subscribe(
              () => { this.getCategEmployeesList(); }
            );
          }
        }
      }
    )
  }

}
