import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ActivatedRoute } from '@angular/router';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { GridDataResult, PageChangeEvent, SelectionEvent } from '@progress/kendo-angular-grid';
import { EmployeeCreateUpdateComponent } from 'src/app/employees/employee-create-update/employee-create-update.component';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { EmployeePaged, EmployeeBasic } from 'src/app/employees/employee';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-select-employee-dialog',
  templateUrl: './select-employee-dialog.component.html',
  styleUrls: ['./select-employee-dialog.component.css']
})
export class SelectEmployeeDialogComponent implements OnInit {
  @Output() selectionChange = new EventEmitter<SelectionEvent>();
  title: string;
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
  private rowsSelected: any;
  

  btnDropdown: any[] = [{ text: 'Bác sĩ' }, { text: 'Phụ tá' }, { text: 'Nhân viên khác' }];
  
  constructor(private service: EmployeeService,
    private activeroute: ActivatedRoute, private modalService: NgbModal, public activeModal: NgbActiveModal) { }

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

  selectedKeysChange(rows : any) {    
    this.rowsSelected = rows;
  }

  onselectedEmployees(){
    this.activeModal.close(this.rowsSelected);  
  } 

  onAdvanceSearchChange(filter) {
    this.isDoctor = filter.isDoctor;
    this.isAssistant = filter.isAssistant;
    this.getEmployeesList();
  }

  createDoctor() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm bác sĩ';
    modalRef.componentInstance.isDoctor = true;
    modalRef.result.then(() => {
      this.getEmployeesList();
    });
  }

  createAssistant() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm phụ tá';
    modalRef.componentInstance.isAssistant = true;
    modalRef.result.then(() => {
      this.getEmployeesList();
    });
  }

  createEmployee() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhân viên';
    modalRef.result.then(() => {
      this.getEmployeesList();
    });
  }

  editEmployee(item: EmployeeBasic) {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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
