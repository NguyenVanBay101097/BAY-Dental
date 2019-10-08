import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { EmployeePaged } from '../employee';
import { FormGroup, FormBuilder } from '@angular/forms';
import { EmployeeService } from '../employee.service';
import { map, distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DialogRef, DialogCloseResult, WindowService, DialogService, WindowRef, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { EmployeeCreateUpdateComponent } from '../employee-create-update/employee-create-update.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit {

  constructor(private fb: FormBuilder, private service: EmployeeService,
    private dialogService: DialogService, private modalService: NgbModal) { }

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

  btnDropdown: any[] = [{ text: 'Bác sĩ' }, { text: 'Phụ tá' }, { text: 'Nhân viên khác' }];

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

  // openWindow(id) {
  //   const windowRef: WindowRef = this.windowService.open(
  //     {
  //       title: id ? 'Cập nhật nhân viên' : 'Tạo nhân viên',
  //       content: EmployeeCreateUpdateComponent,
  //       minWidth: 250,
  //     });
  //   this.windowOpened = true;
  //   const instance = windowRef.content.instance;
  //   if (id) {
  //     instance.empId = id;
  //   }

  //   windowRef.result.subscribe(
  //     (result) => {
  //       this.windowOpened = false;
  //       if (!(result instanceof WindowCloseResult)) {
  //         this.getEmployeesList();
  //       }
  //     }
  //   )
  // }

  openModal(id, isDoctor, isAssistant) {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (id) {
      modalRef.componentInstance.empId = id;
    }
    modalRef.componentInstance.isDoctor = isDoctor;
    modalRef.componentInstance.isAssistant = isAssistant;
    modalRef.result.then(
      rs => {
        this.getEmployeesList();
      },
      er => { }
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

  btnDropdownItemClick(e) {
    console.log(e);
    switch (e.text.toString()) {
      case "Bác sĩ":
        console.log(1);
        this.openModal(null, true, false);
        break;
      case "Phụ tá":
        console.log(2);
        this.openModal(null, false, true);
        break;
      case "Nhân viên khác":
        console.log(3);
        this.openModal(null, false, false);
        break;
    }
  }

}
