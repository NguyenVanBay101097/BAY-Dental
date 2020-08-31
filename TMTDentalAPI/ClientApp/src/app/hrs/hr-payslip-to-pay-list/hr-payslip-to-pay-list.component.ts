import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { HrPayslipService, HrPayslipPaged } from '../hr-payslip.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { map } from 'rxjs/internal/operators/map';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ValueAxisLabelsComponent } from '@progress/kendo-angular-charts';

@Component({
  selector: 'app-hr-payslip-to-pay-list',
  templateUrl: './hr-payslip-to-pay-list.component.html',
  styleUrls: ['./hr-payslip-to-pay-list.component.css']
})
export class HrPayslipToPayListComponent implements OnInit {
  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;

  gridData: GridDataResult = {
    data: [],
    total: 0
  };
  limit = 20;
  skip = 0;
  loading = false;
  collectionSize = 0;
  StateFilters = [
    { text: 'tất cả', value: '' },
    { text: 'bản nháp', value: 'draft' },
    { text: 'đang xử lý', value: 'process' },
    { text: 'hoàn thành', value: 'done' }
  ];
  searchUpdate = new Subject<string>();
  listEmployees: any;
  searchForm: FormGroup;

  constructor(
    private hrPayslipService: HrPayslipService,
    private employeeService: EmployeeService,
    private modalService: NgbModal, private intlService: IntlService,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit() {
    this.searchForm = this.fb.group({
      employee: null,
      search: null,
      dateFrom: null,
      dateTo: null,
      state: null,
    });

    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadEmployeePaged();
    this.empCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.empCbx.loading = true)),
      switchMap(value => this.searchEmployee(value))
    ).subscribe(result => {
      this.listEmployees = result.items;
      this.empCbx.loading = false;
    });
  }

  get datefromControl() { return this.searchForm.get('dateFrom'); }
  get dateToControl() { return this.searchForm.get('dateTo'); }
  get statecontrol() { return this.searchForm.get('state'); }
  get searchcontrol() { return this.searchForm.get('search'); }
  get employeecontrol() { return this.searchForm.get('employee'); }

  loadDataFromApi() {
    this.loading = true;
    const val = new HrPayslipPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchcontrol.value || '';
    val.state = this.statecontrol.value || '';
    val.dateFrom = this.datefromControl.value ? this.intlService.formatDate(this.datefromControl.value, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.dateTo = this.datefromControl.value ? this.intlService.formatDate(this.dateToControl.value, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.employeeId = this.employeecontrol.value ? this.employeecontrol.value.id : '';

    this.hrPayslipService.getPaged(val).pipe(
      map((res: any) => ({
        data: res.items,
        total: res.totalItems,
      } as GridDataResult))
    )
      .subscribe(res => {
        this.gridData = res;
        this.loading = false;
      }, err => {
        console.log(err);
        this.loading = false;
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadEmployeePaged() {
    this.searchEmployee().subscribe((res) => {
      this.listEmployees = res.items;
    });
  }

  onEmployeeSelectChange(e) {
    this.employeecontrol.setValue(e);
    this.loadDataFromApi();
  }

  searchEmployee(search?: string) {
    const val = new EmployeePaged();
    val.search = search ? search : '';
    return this.employeeService.getEmployeePaged(val);
  }

  createItem() {
    this.router.navigate(['/hr/payslips/create']);
  }

  editItem(dataitem) {
    this.router.navigate(['/hr/payslips/edit/' + dataitem.id]);
  }

  deleteItem(dataitem) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu lương';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.hrPayslipService.delete(dataitem.id).subscribe(res => {
        this.loadDataFromApi();
      });
    });
  }

  onSelect(state) {
    this.searchForm.get('state').setValue(state);
    this.loadDataFromApi();
  }

  OnDateFilterChange() {
    if (this.searchForm.get('dateFrom').value && this.searchForm.get('dateTo').value) {
      this.loadDataFromApi();
    }
  }

  GetStateFilter() {
    switch (this.searchForm.get('state').value) {
      case 'draft':
        return 'Bản nháp';
      case 'verify':
        return 'Đang xử lý';
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Tất cả trạng thái';
    }
  }

  onDateSearchChange(e) {
    this.searchForm.get('dateFrom').setValue(e.dateFrom);
    this.searchForm.get('dateTo').setValue(e.dateTo);
    this.loadDataFromApi();
  }

}
