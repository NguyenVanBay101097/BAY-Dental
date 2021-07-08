import { EmployeeService } from 'src/app/employees/employee.service';
import { EmployeeSimple } from 'src/app/employees/employee';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { AppointmentGetCountVM } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { CustomerReceiptPaged, CustomerReceiptService } from 'src/app/customer-receipt/customer-receipt.service';
import { CustomerReceipCreateUpdateComponent } from '../../customer-receip-create-update/customer-receip-create-update.component';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { EmployeePaged } from 'src/app/employees/employee';

@Component({
  selector: 'app-dashboard-customer-receipt-list-today',
  templateUrl: './dashboard-customer-receipt-list-today.component.html',
  styleUrls: ['./dashboard-customer-receipt-list-today.component.css']
})
export class DashboardCustomerReceiptListTodayComponent implements OnInit {
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;

  gridData: GridDataResult;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  limit = 1000;
  skip = 0;
  total: number;
  listEmployees: EmployeeSimple[] = [];
  stateFilter: string = '';
  stateCount: any = {};
  states: any[] = [
    { value: '', text: 'Tất cả'},
    { value: 'waiting', text: 'Chờ khám'},
    { value: 'examination', text: 'Đang khám'},
    { value: 'done', text: 'Hoàn thành'},
  ]

  public today: Date = new Date(new Date().toDateString());

  constructor(private intlService: IntlService,private customerReceiptService: CustomerReceiptService,
    private employeeService: EmployeeService,
    private notifyService : NotifyService,
    private modalService: NgbModal) { }

  ngOnInit() {
    this.loadDataFromApi();
    this.loadStateCount();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadListEmployees();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CustomerReceiptPaged();
    val.limit = 0;
    val.offset = this.skip;
    val.search = this.search || "";
    val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.state = this.stateFilter == '' ? '' : this.stateFilter;

    this.customerReceiptService.getPaged(val).pipe(
      map((response) => <GridDataResult>{
        data: response.items,
        total: response.totalItems,
      }
      )
    ).subscribe(
      (res) => {
        this.gridData = res;
        this.total = res.total;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadStateCount() {
    forkJoin(this.states.map(x => {
      var val = new AppointmentGetCountVM();
      val.state = x.value;
      val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
      val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
      return this.customerReceiptService.getCount(val).pipe(
        switchMap(count => of({state: x.value, count: count}))
      );
    })).subscribe((result) => {
      result.forEach(item => {
        this.stateCount[item.state] = item.count;
      });
    });
  }

  setStateFilter(state: any) {
    this.stateFilter = state;  
  }

  loadListEmployees() {
    this.searchEmployees().subscribe(
      rs => {
        this.listEmployees = rs.items;
      });
  }

  onChangeDoctor(val){
    debugger
  }

  searchEmployees(search?: string) {
    var paged = new EmployeePaged();
    paged.search = search || '';
    paged.isDoctor = true;
    return this.employeeService.getEmployeePaged(paged);
  }

  createItem() {
    let modalRef = this.modalService.open(CustomerReceipCreateUpdateComponent, { size: "lg", windowClass: "o_technical_modal modal-appointment", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.title = "Tiếp nhận";
    modalRef.result.then(() => {
      this.notifyService.notify('success','Lưu thành công');
      this.loadDataFromApi();
      this.loadStateCount();
    }, () => { }
    );
  }

  editItem(item) {
    const modalRef = this.modalService.open(CustomerReceipCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Cập nhật tiếp nhận";
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify('success','Lưu thành công');
      this.loadDataFromApi();
      this.loadStateCount();
    }, () => {
    });
  }

  getTime(item){
    var state = item.state;
    switch (state) {
      case 'waiting':
        return item.dateWaiting;
      case 'examination':
        return item.dateExamination;
      case 'done':
        return item.dateDone;    
    }
  }

  getState(state) {
    switch (state) {
      case 'waiting':
        return 'Chờ khám';
      case 'examination':
        return 'Đang khám';
      case 'done':
        return 'Hoàn thánh';    
    }
  }

  getColorState(state) {
    switch (state) {
      case 'waiting':
        return 'badge-outline-warning';
      case 'examination':
        return 'badge-outline-info';
      case 'done':
        return 'badge-outline-success';
    }
  }


}
