import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { EmployeePaged } from '../employee';
import { FormGroup, FormBuilder } from '@angular/forms';
import { EmployeeService } from '../employee.service';
import { map, distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DialogRef, DialogCloseResult, WindowService, DialogService, WindowRef, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { EmployeeCreateUpdateComponent } from '../employee-create-update/employee-create-update.component';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit {

  constructor(private fb: FormBuilder, private service: EmployeeService, private windowService: WindowService, private dialogService: DialogService) { }

  loading = false;
  gridView: GridDataResult;
  windowOpened: boolean = false;
  skip = 0;
  pageSize = 20;

  formFilter: FormGroup;
  search: string;
  searchUpdate = new Subject<string>();

  isDoctor: boolean;
  isAssistant: boolean;
  isOther: boolean;

  ngOnInit() {
    this.formFilter = this.fb.group({
      search: null,
      isDoctor: null,
      isAssistant: null,
      isOther: null
    });

    this.getEmployeesList();
    this.searchChange();

  }

  getEmployeesList() {
    var positionList = new Array<string>();
    this.loading = true;
    var empPaged = new EmployeePaged();
    empPaged = this.formFilter.value;
    empPaged.limit = this.pageSize;
    empPaged.offset = this.skip;
    if (this.formFilter.get('isDoctor').value) {
      positionList.push('doctor');
    }
    if (this.formFilter.get('isAssistant').value) {
      positionList.push('assistant');
    }
    if (this.formFilter.get('isOther').value) {
      positionList.push('other');
    }
    empPaged.position = positionList.join(',');
    // if (this.isDoctor === true) {
    //   empPaged.isDoctor = this.isDoctor;
    // }
    // if (this.isAssistant === true) {
    //   empPaged.isAssistant = this.isAssistant;
    // }
    // if (this.isOther === true) {
    //   empPaged.isDoctor = false;
    //   empPaged.isAssistant = false;
    // }

    this.service.getEmployeePaged(empPaged).pipe(
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
        this.getEmployeesList();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.getEmployeesList();
  }

  openWindow(id) {
    const windowRef: WindowRef = this.windowService.open(
      {
        title: 'Thông tin nhân viên',
        content: EmployeeCreateUpdateComponent,
        minWidth: 250,
      });
    this.windowOpened = true;
    const instance = windowRef.content.instance;
    instance.empId = id;

    windowRef.result.subscribe(
      (result) => {
        this.windowOpened = false;
        if (!(result instanceof WindowCloseResult)) {
          this.getEmployeesList();
        }
      }
    )
  }

  deleteEmployee(id) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa nhân viên',
      content: 'Bạn chắc chắn muốn xóa nhân viên này ?',
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
            this.service.deleteEmployee(id).subscribe(
              () => { this.getEmployeesList(); }
            );
          }
        }
      }
    )
  }

}
