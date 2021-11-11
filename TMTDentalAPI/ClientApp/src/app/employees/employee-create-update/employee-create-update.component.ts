import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { Commission, CommissionPaged, CommissionService } from 'src/app/commissions/commission.service';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { EmpCategoriesCreateUpdateComponent } from 'src/app/employee-categories/emp-categories-create-update/emp-categories-create-update.component';
import { EmployeeCategoryBasic, EmployeeCategoryDisplay, EmployeeCategoryPaged } from 'src/app/employee-categories/emp-category';
import { EmpCategoryService } from 'src/app/employee-categories/emp-category.service';
import { HrJobService, HrJobsPaged } from 'src/app/hr-jobs/hr-job.service';
import { HrPayrollStructureTypeSimple } from 'src/app/hrs/hr-payroll-structure-type.service';
import { ResGroupService } from 'src/app/res-groups/res-group.service';
import { ApplicationRolePaged, RoleService } from 'src/app/roles/role.service';
import { UserSimple } from 'src/app/users/user-simple';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { EmployeeService } from '../employee.service';
import { HrPayrollStructureTypePaged, HrPayrollStructureTypeService } from './../../hrs/hr-payroll-structure-type.service';

@Component({
  selector: 'app-employee-create-update',
  templateUrl: './employee-create-update.component.html',
  styleUrls: ['./employee-create-update.component.css'],
})
export class EmployeeCreateUpdateComponent implements OnInit, AfterViewInit {

  constructor(private fb: FormBuilder,
    private employeeService: EmployeeService,
    private structureTypeService: HrPayrollStructureTypeService,
    private empCategService: EmpCategoryService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal, private notificationService: NotificationService,
    private intlService: IntlService, private commissionService: CommissionService, private userService: UserService,
    private companyService: CompanyService,
    private resGroupService: ResGroupService,
    private roleService: RoleService,
    private hrJobService: HrJobService,
  ) { }
  empId: string;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('commissionCbx') commissionCbx: ComboBoxComponent;
  @ViewChild('assistantCommissionCbx') assistantCommissionCbx: ComboBoxComponent;
  @ViewChild('counselorCommissionCbx') counselorCommissionCbx: ComboBoxComponent;
  @ViewChild("name", { static: true }) private nameEL: ElementRef;
  @ViewChild('cbxHrJob', { static: true }) public cbxHrJob: ComboBoxComponent;


  isChange: boolean = false;
  formCreate: FormGroup;
  windowOpened: boolean = false;
  title = 'Nhân viên';
  isDoctor: boolean;
  isAssistant: boolean;
  isShowSalary = false;
  submitted = false;

  filteredstructureTypes: HrPayrollStructureTypeSimple[] = [];
  categoriesList: EmployeeCategoryBasic[] = [];
  categoriesList2: EmployeeCategoryDisplay[] = [];
  filteredUsers: UserSimple[] = [];
  listCommissions: Commission[] = [];
  listAssistantCommissions: Commission[] = [];
  listCounselorCommissions: Commission[] = [];
  listCompanies: CompanyBasic[] = [];
  groupSurvey: any[] = [];
  roles: any[] = [];
  cbxPopupSettings = {
    width: 'auto'
  };
  hrJobs: any[] = [];

  ngOnInit() {
    this.formCreate = this.fb.group({
      name: [null, Validators.required, Validators.minLength(6)],
      phone: null,
      address: null,
      ref: null,
      identityCard: null,
      email: null,
      birthDay: null,
      category: [null],
      isDoctor: this.isDoctor || false,
      isAssistant: this.isAssistant || false,
      commissionId: null,
      commission: null,
      assistantCommissionId: null,
      assistantCommission: null,
      counselorCommissionId: null,
      counselorCommission: null,
      userId: null,
      user: null,
      structureTypeId: null,
      structureType: null,
      wage: 0,
      hourlyWage: 0,
      startWorkDate: null,
      enrollNumber: null,
      leavePerMonth: 0,
      regularHour: 8,
      overtimeRate: 150,
      restDayRate: 100,
      allowance: 0,
      isUser: false,
      userCompany: null,
      userCompanies: [[]],
      userName: null,
      userPassword: [null],
      createChangePassword: true,
      avatar: null,
      userAvatar: null,
      groupId: null,
      isAllowSurvey: false,
      roles: null,
      hrJob: null,
    });

    setTimeout(() => {
      this.loadListCommissions();
      this.loadListcommissionAssistants();
      this.loadListcommissionCounselors();
      this.getEmployeeInfo();
      // this.loadUsers();
      // this.loadstructureTypes();
      this.loadListCompanies();
      this.loadHrJobs();

      // this.userCbx.filterChange.asObservable().pipe(
      //   debounceTime(300),
      //   tap(() => (this.userCbx.loading = true)),
      //   switchMap(value => this.searchUsers(value))
      // ).subscribe(result => {
      //   this.filteredUsers = result;
      //   this.userCbx.loading = false;
      // });
      this.loadGroupSurvey();

      this.loadRoles();
      this.cbxHrJob.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.cbxHrJob.loading = true)),
        switchMap(value => this.searchHrJob(value))
      ).subscribe((res : any) => {
        this.hrJobs = res.items;
        this.cbxHrJob.loading = false;
      });
    });
    document.getElementById('name').focus();
  }

  ngAfterViewInit(): void {
    this.commissionCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.commissionCbx.loading = true)),
      switchMap(value => this.searchCommissions(value))
    ).subscribe(result => {
      this.listCommissions = result.items;
      this.commissionCbx.loading = false;
    });

    this.assistantCommissionCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.assistantCommissionCbx.loading = true)),
      switchMap(value => this.searchCommissionAssistants(value))
    ).subscribe(result => {
      this.listAssistantCommissions = result.items;
      this.assistantCommissionCbx.loading = false;
    });

    this.counselorCommissionCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.counselorCommissionCbx.loading = true)),
      switchMap(value => this.searchCommissionCounselors(value))
    ).subscribe(result => {
      this.listCounselorCommissions = result.items;
      this.counselorCommissionCbx.loading = false;
    });

  }

  get isUser() {
    return this.formCreate.get('isUser').value;
  }

  get createChangePassword() {
    return this.formCreate.get('createChangePassword').value;
  }

  get avatar() {
    return this.formCreate.get('avatar').value;
  }

  get f() {
    return this.formCreate.controls;
  }

  getValueForm(key) {
    return this.formCreate.get(key).value;
  }

  getControlForm(key) {
    return this.formCreate.get(key);
  }

  loadHrJobs() {
    this.searchHrJob().subscribe((res:any) => {
      this.hrJobs = res.items;
    })
  }
  
  searchHrJob(s?) {
    var val = new HrJobsPaged();
    val.offset = 0;
    val.limit = 20;
    val.search = s || '';
    return this.hrJobService.getPaged(val);
  }

  loadUsers() {
    this.searchUsers().subscribe(result => {
      this.filteredUsers = _.unionBy(this.filteredUsers, result, 'id');
    });
  }

  loadListCompanies() {
    var val = new CompanyPaged();
    val.active = true;
    this.companyService.getPaged(val).subscribe(result => {
      this.listCompanies = _.unionBy(this.listCompanies, result.items, 'id');
    });
  }

  onChangeCreateChangePassword(e) {
    if (this.createChangePassword) {
      this.formCreate.get('userPassword').setValidators([Validators.required]);
      this.formCreate.get('userPassword').updateValueAndValidity();
      this.f.userPassword.markAsDirty();
    } else {
      this.formCreate.get('userPassword').setValidators([]);
      this.formCreate.get('userPassword').updateValueAndValidity();
    }
  }

  updateValidation() {
    if (this.isUser) {
      this.formCreate.get('userName').setValidators([Validators.required]);
      this.formCreate.get('userName').updateValueAndValidity();

      this.formCreate.get('userCompany').setValidators([Validators.required]);
      this.formCreate.get('userCompany').updateValueAndValidity();
    } else {
      this.formCreate.get('userName').setValidators([]);
      this.formCreate.get('userName').updateValueAndValidity();

      this.formCreate.get('userCompany').setValidators([]);
      this.formCreate.get('userCompany').updateValueAndValidity();
    }
  }

  onChangeIsUser(e) {
    if (this.isUser) {
      setTimeout(() => {
        this.nameEL.nativeElement.focus();
      }, 300);

      this.formCreate.get('userName').setValidators([Validators.required]);
      this.formCreate.get('userName').updateValueAndValidity();

      this.formCreate.get('userPassword').setValidators([Validators.required]);
      this.formCreate.get('userPassword').updateValueAndValidity();
      this.formCreate.get('createChangePassword').setValue(true);

      this.formCreate.get('userCompany').setValidators([Validators.required]);
      this.formCreate.get('userCompany').updateValueAndValidity();

      this.userService.defaultGet().subscribe((result) => {
        if (result.company) {
          this.formCreate.get('userCompany').setValue(result.company);
          this.listCompanies = _.unionBy(this.listCompanies, [result.company], 'id');
        }

        if (result.companies) {
          this.formCreate.get('userCompanies').setValue(result.companies);
          this.listCompanies = _.unionBy(this.listCompanies, result.companies, 'id');
        }
      });
    } else {
      this.formCreate.get('userName').setValidators([]);
      this.formCreate.get('userName').updateValueAndValidity();

      this.formCreate.get('userPassword').setValidators([]);
      this.formCreate.get('userPassword').updateValueAndValidity();
      this.formCreate.get('createChangePassword').setValue(false);

      this.formCreate.get('userCompany').setValidators([]);
      this.formCreate.get('userCompany').updateValueAndValidity();
    }
  }


  searchUsers(filter?: string) {
    var val = new UserPaged();
    val.search = filter || '';
    return this.userService.autocompleteSimple(val);
  }


  loadListCommissions() {
    this.searchCommissions().subscribe(result => {
      this.listCommissions = _.unionBy(this.listCommissions, result.items, 'id');
    });
  }

  searchCommissions(q?: string) {
    var val = new CommissionPaged();
    val.search = q || '';
    val.type = 'doctor';
    return this.commissionService.getPaged(val);
  }

  loadListcommissionCounselors() {
    this.searchCommissionCounselors().subscribe(result => {
      this.listCounselorCommissions = _.unionBy(this.listCounselorCommissions, result.items, 'id');
    });
  }

  searchCommissionCounselors(q?: string) {
    var val = new CommissionPaged();
    val.search = q || '';
    val.type = 'counselor';
    return this.commissionService.getPaged(val);
  }

  loadListcommissionAssistants() {
    this.searchCommissionAssistants().subscribe(result => {
      this.listAssistantCommissions = _.unionBy(this.listAssistantCommissions, result.items, 'id');
    });
  }

  searchCommissionAssistants(q?: string) {
    var val = new CommissionPaged();
    val.search = q || '';
    val.type = 'assistant';
    return this.commissionService.getPaged(val);
  }

  getEmployeeInfo() {
    if (this.empId != null) {
      this.employeeService.getEmployee(this.empId).subscribe(
        (rs: any) => {
          rs.birthDay = rs.birthDay ? new Date(rs.birthDay) : null;
          rs.startWorkDate = rs.startWorkDate ? new Date(rs.startWorkDate) : null;
          if (rs.commission) {
            this.listCommissions = _.unionBy(this.listCommissions, [rs.commission], 'id');
          }

          if (rs.assistantCommission) {
            this.listAssistantCommissions = _.unionBy(this.listAssistantCommissions, [rs.assistantCommission], 'id');
          }
      
          if (rs.counselorCommission) {
            this.listCounselorCommissions = _.unionBy(this.listCounselorCommissions, [rs.counselorCommission], 'id');
          }
          
          if (rs.hrJob) {
            this.hrJobs = _.unionBy(this.hrJobs, [rs.hrJob], 'id');
          }
      
          this.formCreate.get('createChangePassword').setValue(false);
          this.onChangeCreateChangePassword(null);
          this.formCreate.patchValue(rs);

          this.updateValidation();
        },
        er => {
          console.log(er);
        }
      );
    }
  }

  onAvatarUploaded(data) {
    var fileUrl = data ? data.fileUrl : null;
    this.formCreate.get('avatar').setValue(fileUrl);
  }

  //Tạo hoặc cập nhật NV
  createUpdateEmployee() {
    this.submitted = true;

    if (!this.formCreate.valid) {
      return false;
    }

    var value = this.formCreate.value;
    value.birthDay = value.birthDay ? this.intlService.formatDate(value.birthDay, 'yyyy-MM-dd') : null;
    value.startWorkDate = value.startWorkDate ? this.intlService.formatDate(value.startWorkDate, 'yyyy-MM-dd') : null;
    if (value.isUser) {
      value.userCompanyId = value.userCompany.id;
      value.userCompanyIds = value.userCompanies.map(x => x.id);
      value.roleIds = value.roles && value.roles.length > 0 ? value.roles.map(x => x.id) : [];
    }
    
    value.commissionId = value.commission ? value.commission.id : null;
    value.counselorCommissionId = value.counselorCommission ? value.counselorCommission.id : null;
    value.assistantCommissionId = value.assistantCommission ? value.assistantCommission.id : null;
    value.hrJobId = value.hrJob ? value.hrJob.id : null;
    this.isChange = true;

    this.employeeService.createUpdateEmployee(value, this.empId).subscribe(
      rs => {
        this.notify('success', 'Lưu thành công');
        if (this.empId) {
          this.activeModal.close(true);
        } else {
          this.activeModal.close(rs);
        }
        //nếu có đổi group survey thì reload groups
        // this.authService.getGroups().subscribe((result: any) => {
        //   this.permissionService.define(result);
        // });
      },
      er => {
        console.log(er);
        this.submitted = false;
      }
    );
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  get structureTypeValue() {
    return this.formCreate.get('structureType').value ? this.formCreate.get('structureType').value : null;
  }

  //Cho phép field phone chỉ nhập số
  onlyGetNumbers(formControlName) {
    this.formCreate.controls[formControlName].setValue(this.formCreate.get(formControlName).value.replace(/[^0-9.]/g, ''));
  }

  //Đóng dialog
  // closeWindow(result: any) {
  //   if (this.isChange) {
  //     if (result == null) {
  //       this.window.close(true);
  //     }
  //     else {
  //       this.window.close(result);
  //     }
  //   } else {
  //     this.window.close(false);
  //   }
  // }

  closeModal() {
    this.submitted = false;
    if (this.isChange) {
      this.activeModal.close(true);
    }
    else {
      this.activeModal.dismiss();
    }
  }

  // quickCreateCategory() {
  //   const windowRef: WindowRef = this.windowService.open(
  //     {
  //       title: 'Tạo nhóm mới',
  //       content: EmpCategoriesCreateUpdateComponent,
  //       minWidth: 250
  //     });
  //   this.windowOpened = true;

  //   windowRef.result.subscribe(
  //     (result) => {
  //       this.windowOpened = false;
  //       // console.log(result instanceof WindowCloseResult);
  //       if (!(result instanceof WindowCloseResult)) {
  //         console.log(result);
  //         this.categoriesList.push(result);
  //         this.formCreate.get('category').setValue(result);
  //       }
  //     }
  //   )
  // }

  quickCreateCategory() {
    const modalRef = this.modalService.open(EmpCategoriesCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });

    modalRef.result.then(
      result => {
        this.windowOpened = false;

        if (result && result.name) {
          console.log(result);
          this.categoriesList.push(result);
          this.formCreate.get('category').setValue(result);
        }
      },
      er => { }
    )
  }

  handleSourceFilter(value) {
    this.filteredstructureTypes = this.filteredstructureTypes.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  searchstructureTypes(q?: string) {
    var val = new HrPayrollStructureTypePaged();
    val.search = q;
    return this.structureTypeService.autocomplete(val);
  }

  loadstructureTypes() {
    this.searchstructureTypes().subscribe((result) => {
      this.filteredstructureTypes = _.unionBy(this.filteredstructureTypes, result, 'id');
    });
  }

  loadAutocompleteTypes(searchKw: string) {
    var empCategPaged = new EmployeeCategoryPaged;
    empCategPaged.limit = 20;
    empCategPaged.offset = 0;
    if (searchKw) {
      empCategPaged.search = searchKw.toLowerCase();
    }
    this.empCategService.autocompleteCategoryTypes(empCategPaged).subscribe(
      rs => {
        this.categoriesList = rs;
      },
      er => {
        console.log(er);
      }
    )
  }

  toggleVisibility(e) {
    this.isShowSalary = e.target.checked;
  }

  loadGroupSurvey() {
    this.resGroupService.getListForSurvey()
      .subscribe(
        (res: any) => {
          this.groupSurvey = res;
        }
      );
  }

  onChangeIsAllowSurvey(e) {
    var value = e.target.checked;
    if (value == false) {
      this.getControlForm('groupId').setValue(null);
    } else {
      if (this.groupSurvey.length) {
        this.getControlForm('groupId').setValue(this.groupSurvey[0].id);
      }
    }
  }

  loadRoles() {
    var page = new ApplicationRolePaged();
    page.limit = 100;
    page.offset = 0;
    this.roleService.getPaged(page).subscribe((res: any) => {
      this.roles = res.items;
    });
  }
}
