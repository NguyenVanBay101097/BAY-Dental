import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { EmployeePaged, EmployeeBasic } from '../employee';
import { FormGroup, FormBuilder } from '@angular/forms';
import { EmployeeService } from '../employee.service';
import { map, distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { EmployeeCreateUpdateComponent } from '../employee-create-update/employee-create-update.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class EmployeeListComponent implements OnInit {

  constructor(private fb: FormBuilder, private service: EmployeeService,
    private activeroute: ActivatedRoute, private modalService: NgbModal) { }

  loading = false;
  gridView: GridDataResult;
  windowOpened: boolean = false;
  skip = 0;
  pageSize = 20;

  search: string;
  searchUpdate = new Subject<string>();

  isDoctor: boolean;
  isAssistant: boolean;
  isOther: boolean;

  btnDropdown: any[] = [{ text: 'Bác sĩ' }, { text: 'Phụ tá' }, { text: 'Nhân viên khác' }];

  ngOnInit() {
    this.getEmployeesList();
    this.searchChange();
  }

  getEmployeesList() {
    var positionList = new Array<string>();
    this.loading = true;
    var empPaged = new EmployeePaged();
    empPaged.limit = this.pageSize;
    empPaged.offset = this.skip;
    if (this.search) {
      empPaged.search = this.search;
    }
    if (this.isDoctor) {
      empPaged.isDoctor = this.isDoctor;
    }
    if (this.isAssistant) {
      empPaged.isAssistant = this.isAssistant;
    }

    this.service.getEmployeePaged(empPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.loading = false;
    }, er => {
      this.loading = false;
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

  onAdvanceSearchChange(filter) {
    this.isDoctor = filter.isDoctor;
    this.isAssistant = filter.isAssistant;
    this.getEmployeesList();
  }

  createDoctor() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm bác sĩ';
    modalRef.componentInstance.isDoctor = true;
    modalRef.result.then(() => {
      this.getEmployeesList();
    });
  }

  createAssistant() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm phụ tá';
    modalRef.componentInstance.isAssistant = true;
    modalRef.result.then(() => {
      this.getEmployeesList();
    });
  }

  createEmployee() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhân viên';
    modalRef.result.then(() => {
      this.getEmployeesList();
    });
  }

  editEmployee(item: EmployeeBasic) {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa nhân viên';
    modalRef.componentInstance.empId = item.id;
    modalRef.result.then(() => {
      this.getEmployeesList();
    });
  }

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
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa nhân viên';
    modalRef.result.then(() => {
      this.service.deleteEmployee(id).subscribe(
        () => { this.getEmployeesList(); }
      );
    }, () => {
    });
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
