import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AgentService } from 'src/app/agents/agent.service';
import { Commission, CommissionPaged, CommissionService } from 'src/app/commissions/commission.service';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { ResBankService, ResPartnerBankPaged } from 'src/app/res-banks/res-bank.service';
import { BankCuDialogComponent } from '../bank-cu-dialog/bank-cu-dialog.component';
import { CheckPermissionService } from '../check-permission.service';
import { NotifyService } from '../services/notify.service';

@Component({
  selector: 'app-agent-create-update-dialog',
  templateUrl: './agent-create-update-dialog.component.html',
  styleUrls: ['./agent-create-update-dialog.component.css']
})
export class AgentCreateUpdateDialogComponent implements OnInit, AfterViewInit {
  id: string;
  formGroup: FormGroup;
  submitted = false;
  isRoleRead = false;
  isRoleCreate = false;
  title: string;

  dayList: number[] = [];
  monthList: number[] = [];
  yearList: number[] = [];
  listAgentCommissions: Commission[] = [];
  listBank: any[] = [];
  customerSimpleFilter: PartnerSimple[] = [];
  employeeSimpleFilter: any[] = [];
  classify: string = 'partner';
  @ViewChild('agentCommissionCbx', { static: false }) agentCommissionCbx: ComboBoxComponent;
  @ViewChild('listBankCbx', { static: false }) listBankCbx: ComboBoxComponent;
  @ViewChild('customerCbx', { static: false }) customerCbx: ComboBoxComponent;
  @ViewChild('employeeCbx', { static: false }) employeeCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder,
    public agentService: AgentService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private intlService: IntlService,
    private checkPermissionService: CheckPermissionService,
    private commissionService: CommissionService,
    private partnerService: PartnerService,
    private notifyService: NotifyService,
    private employeeService: EmployeeService,
    private resBankService: ResBankService,
  ) { }


  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ["", Validators.required],
      gender: "male",
      birthDayStr: '',
      birthMonthStr: '',
      birthYearStr: '',
      email: null,
      phone: null,
      jobTitle: null,
      address: null,
      commission: [null, Validators.required],
      customer: null,
      employee: null,
      bank: null, // ngân hàng
      classify: 'partner', // phân loại khách hàng
      bankBranch: '', // chi nhánh ngân hàng
      accountNumber: '', // số tài khoản
      accountHolder: '', // chủ tài khoản
    });

    setTimeout(() => {
      this.checkRole();
      this.reload();
      this.dayList = _.range(1, 32);
      this.monthList = _.range(1, 13);
      this.yearList = _.range(new Date().getFullYear(), 1900, -1);
      this.loadListcommissionAgents();
      this.loadListBank();
      this.getCustomerList();
      this.getEmployeesList();
    });
  }

  ngAfterViewInit(): void {
    this.agentCommissionCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.agentCommissionCbx.loading = true)),
      switchMap(value => this.searchCommissionAgents(value))
    ).subscribe(result => {
      this.listAgentCommissions = result.items;
      this.agentCommissionCbx.loading = false;
    });

    this.listBankCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.listBankCbx.loading = true)),
      switchMap(value => this.searchBank(value))
    ).subscribe((result: any) => {
      this.listBank = result;
      this.listBankCbx.loading = false;
    });

    this.customerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.customerCbx.loading = true),
      switchMap(val => this.searchCustomers(val.toString().toLowerCase()))
    ).subscribe(
      (rs: any) => {
        this.customerSimpleFilter = rs;
        this.customerCbx.loading = false;
      }
    )
    
    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.employeeCbx.loading = true),
      switchMap(val => this.searchEmloyee(val.toString().toLowerCase()))
    ).subscribe(
      (rs: any) => {
        this.employeeSimpleFilter = rs;
        this.employeeCbx.loading = false;
      }
    )
  }

  loadListBank() {
    this.searchBank().subscribe((result: any) => {
      this.listBank = _.unionBy(this.listBank, result, 'id');
    });
  }

  searchBank(q?: string) {
    let val = new ResPartnerBankPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = q || '';
    return this.resBankService.getAutocomplete(val);
  }

  getCustomerList() {
    this.searchCustomers().subscribe((rs: any) => {
      this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, rs, 'id');
    });
  }

  searchCustomers(q?: string) {
    var val = new PartnerPaged();
    val.employee = false;
    val.customer = true;
    val.supplier = false;
    val.limit = 20;
    val.offset = 0;
    val.search = q || '';
    return this.partnerService.autocompletePartnerInfo(val);
  }

  getEmployeesList() {
    this.searchEmloyee().subscribe((rs: any) => {
      this.employeeSimpleFilter = rs;
    });
  }

  searchEmloyee(q?: string) {
    var empPn = new EmployeePaged();
    empPn.limit = 20;
    empPn.offset = 0;
    empPn.search = q || '';
    return this.employeeService.getAutocompleteInfos(empPn);
  }

  loadListcommissionAgents() {
    this.searchCommissionAgents().subscribe(result => {
      this.listAgentCommissions = _.unionBy(this.listAgentCommissions, result.items, 'id');
    });
  }

  searchCommissionAgents(q?: string) {
    var val = new CommissionPaged();
    val.search = q || '';
    val.type = 'agent';
    return this.commissionService.getPaged(val);
  }

  reload() {
    if (this.id) {
      this.agentService.get(this.id).subscribe((result: any) => {
        console.log(result);
        this.formGroup.patchValue(result);
        this.classify = result.classify ? result.classify : 'partner';
        if (result.birthYear) {
          this.formGroup.get("birthYearStr").setValue(result.birthYear + '');
        }

        if (result.birthMonth) {
          this.formGroup.get("birthMonthStr").setValue(result.birthMonth + '');
        }

        if (result.birthDay) {
          this.formGroup.get("birthDayStr").setValue(result.birthDay + '');
        }
      });
    }
  }

  get f() { return this.formGroup.controls; }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }
    var val = this.formGroup.value;
    val.birthDay = val.birthDayStr ? parseInt(val.birthDayStr) : null;
    val.birthMonth = val.birthMonthStr ? parseInt(val.birthMonthStr) : null;
    val.birthYear = val.birthYearStr ? parseInt(val.birthYearStr) : null;
    val.commissionId = val.commission ? val.commission.id : null;
    val.bankId = val.bank ? val.bank.id : null;
    val.customerId = val.customer ? val.customer.id : null;
    val.employeeId = val.employee ? val.employee.id : null;
    if (this.id) {
      this.agentService.update(this.id, val).subscribe(
        () => {
          this.activeModal.close(true);
        },
      );
    } else {
      this.agentService.create(val).subscribe(
        (result) => {
          this.activeModal.close(result);
        },
      );
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  checkRole() {
    this.isRoleRead = this.checkPermissionService.check(["Catalog.Agent.Read"]);
    this.isRoleCreate = this.checkPermissionService.check(["Catalog.Agent.Create"]);
  }

  createBank() {
    const modalRef = this.modalService.open(BankCuDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm ngân hàng';
    modalRef.result.then(result => {
      this.formGroup.patchValue({ bank: result });
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadListBank();
    })
  }

  onChangeType(e) {
    let val = e.target.value;
    this.classify = val;
    this.resetFormGroup();
    if ( val === 'partner' ) {
      this.f.name.setValidators(Validators.required);
      this.f.name.updateValueAndValidity();
      this.f.customer.clearValidators();
      this.f.customer.updateValueAndValidity();
      this.f.employee.clearValidators();
      this.f.employee.updateValueAndValidity();
      
    }
    else if ( val === 'customer' ) {
      this.f.customer.setValidators(Validators.required);
      this.f.customer.updateValueAndValidity();
      this.f.name.clearValidators();
      this.f.name.updateValueAndValidity();
      this.f.employee.clearValidators();
      this.f.employee.updateValueAndValidity();
    }
    else {
      this.f.employee.setValidators(Validators.required);
      this.f.employee.updateValueAndValidity();
      this.f.name.clearValidators();
      this.f.name.updateValueAndValidity();
      this.f.customer.clearValidators();
      this.f.customer.updateValueAndValidity();
    }
  }

  onChangeCustomer(e) {
    if (e) {
      this.formGroup.patchValue(e);
      if (e.birthYear) {
        this.formGroup.get("birthYearStr").setValue(e.birthYear);
      }

      if (e.birthMonth) {
        this.formGroup.get("birthMonthStr").setValue(e.birthMonth);
      }

      if (e.birthDay) {
        this.formGroup.get("birthDayStr").setValue(e.birthDay);
      }
    }
    else {
      this.resetFormGroup();
    }
  }

  onChangeEmployee(e) {
    if (e) {
      this.formGroup.patchValue(e);
      if (e.birthDay) {
        let DOB = new Date(e.birthDay);
        this.formGroup.get("birthYearStr").setValue(DOB.getFullYear());
        this.formGroup.get("birthMonthStr").setValue(DOB.getMonth());
        this.formGroup.get("birthDayStr").setValue(DOB.getDay());
      }
    }
    else {
      this.resetFormGroup();
    }
  }

  resetFormGroup() {
    let defaultFormGroupData = {
      name: '',
      gender: "male",
      birthDayStr: '',
      birthMonthStr: '',
      birthYearStr: '',
      email: null,
      phone: null,
      jobTitle: null,
      address: null,
      customer: null,
      employee: null,
      classify: this.classify
    }
    this.formGroup.patchValue(defaultFormGroupData);
  }
}
