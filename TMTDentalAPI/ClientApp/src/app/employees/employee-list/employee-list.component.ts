import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { EmployeePaged, EmployeeBasic } from '../employee';
import { FormGroup, FormBuilder } from '@angular/forms';
import { EmployeeService } from '../employee.service';
import { map, distinctUntilChanged, debounceTime, tap, switchMap } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { EmployeeCreateUpdateComponent } from '../employee-create-update/employee-create-update.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ActivatedRoute } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AuthService } from 'src/app/auth/auth.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { HrJobService, HrJobsPaged } from 'src/app/hr-jobs/hr-job.service';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class EmployeeListComponent implements OnInit {
  filterLaboStatus = [
    { name: 'Đang làm việc', value: true },
    { name: 'Ngưng làm việc', value: false }
  ];
  defaultFilter: any = this.filterLaboStatus[0];
  @ViewChild('cbxHrJob', { static: true }) public cbxHrJob: ComboBoxComponent;
  cbxPopupSettings = {
    width: 'auto'
  };
  hrJobs: any[] = [];
  constructor(private fb: FormBuilder, private service: EmployeeService,
    private notificationService: NotificationService,
    private activeroute: ActivatedRoute, private modalService: NgbModal,
    private authService: AuthService,
    private hrJobService: HrJobService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  loading = false;
  gridView: GridDataResult;
  windowOpened: boolean = false;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  active: boolean = true;

  search: string;
  searchUpdate = new Subject<string>();

  isDoctor: boolean;
  isAssistant: boolean;
  isOther: boolean;
  hrJobId: string;
  btnDropdown: any[] = [{ text: 'Bác sĩ' }, { text: 'Phụ tá' }, { text: 'Nhân viên khác' }];

  ngOnInit() {
    this.getEmployeesList();
    this.searchChange();
    this.loadHrJobs();
    this.cbxHrJob.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.cbxHrJob.loading = true)),
      switchMap(value => this.searchHrJob(value))
    ).subscribe((res : any) => {
      this.hrJobs = res.items;
      this.cbxHrJob.loading = false;
    });
  }

  loadHrJobs() {
    this.searchHrJob().subscribe((res:any) => {
      this.hrJobs = res.items;
    })
  }

  getEmployeesList() {
    this.loading = true;
    var empPaged = new EmployeePaged();
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    empPaged.companyId = this.authService.userInfo.companyId;
    empPaged.active = (this.active || this.active == false) ? this.active : '';
    empPaged.hrJobId = this.hrJobId || '';
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
        this.skip = 0;
        this.getEmployeesList();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.getEmployeesList();
  }

  onAdvanceSearchChange(filter) {
    this.isDoctor = filter.isDoctor;
    this.isAssistant = filter.isAssistant;
    this.getEmployeesList();
  }

  createDoctor() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm bác sĩ';
    modalRef.componentInstance.isDoctor = true;
    modalRef.result.then(() => {
      this.getEmployeesList();
    }, () => { });
  }

  createAssistant() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm phụ tá';
    modalRef.componentInstance.isAssistant = true;
    modalRef.result.then(() => {
      this.getEmployeesList();
    });
  }

  createEmployee() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhân viên';
    modalRef.result.then(() => {
      this.getEmployeesList();
    }, () => { });
  }

  editEmployee(item: EmployeeBasic) {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa nhân viên';
    modalRef.componentInstance.empId = item.id;
    modalRef.result.then(() => {
      this.getEmployeesList();
    }, () => { });
  }

  openModal(id, isDoctor, isAssistant) {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa nhân viên';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa nhân viên?';
    modalRef.result.then(() => {
      this.service.deleteEmployee(id).subscribe(
        () => {
          this.notify('success', 'Xóa thành công');
          this.getEmployeesList();
        }
      );
    }, () => {
    });
  }

  btnDropdownItemClick(e) {
    // console.log(e);
    switch (e.text.toString()) {
      case "Bác sĩ":
        // console.log(1);
        this.openModal(null, true, false);
        break;
      case "Phụ tá":
        // console.log(2);
        this.openModal(null, false, true);
        break;
      case "Nhân viên khác":
        // console.log(3);
        this.openModal(null, false, false);
        break;
    }
  }

  actionActive(emp: any, active: boolean) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = (active ? 'Hiện nhân viên ' : 'Ẩn nhân viên ') + emp.name;
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn ' + ((active ? 'hiện nhân viên ' : 'ẩn nhân viên ') + emp.name);
    modalRef.result.then(() => {
      this.service.actionActive(emp.id, active).subscribe(() => {
        if (active) {
          this.notify('success', 'Hiện nhân viên thành công');
        }
        else {
          this.notify('success', 'Ẩn nhân viên thành công');
        }
        this.getEmployeesList();
      });
    }, () => {
    });

  }

  onStateSelectChange(e) {
    this.active = e ? e.value : null;
    this.skip = 0;
    this.getEmployeesList();
  }

  onHrJobSelect(e){
    this.skip = 0;
    this.hrJobId = e?e.id:'';
    this.getEmployeesList();
  }

  searchHrJob(s?) {
    var val = new HrJobsPaged();
    val.offset = 0;
    val.limit = 20;
    val.search = s || '';
    return this.hrJobService.autoComplete(val);
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

}
