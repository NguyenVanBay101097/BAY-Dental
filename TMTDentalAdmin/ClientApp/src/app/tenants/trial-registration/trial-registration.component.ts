import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { MustMatch } from '../../shared/must-match-validator';
import { TenantService } from '../tenant.service';
import { Router } from '@angular/router';
import { NullTemplateVisitor } from '@angular/compiler';
import { EmployeeAdminPaged, EmployeeAdminService } from 'app/employee-admins/employee-admin.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-trial-registration',
  templateUrl: './trial-registration.component.html',
  styleUrls: ['./trial-registration.component.css']
})
export class TrialRegistrationComponent implements OnInit {
  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;
  formGroup: FormGroup;
  registerSuccess = false;
  registerResult: any;
  filterdEmployees: any[];
  constructor(private fb: FormBuilder, private tenantService: TenantService, private employeeAdminService: EmployeeAdminService,
    private router: Router) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      email: [null, [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      customerSource: null,
      employeeAdmin: null,
      companyName: ['', Validators.required],
      hostName: ['', [Validators.required, Validators.pattern('^[a-z0-9]*$'), Validators.minLength(4)]],
      userName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: null,
      address: null
    }, {
      validators: MustMatch('password', 'confirmPassword')
    });

    this.empCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.empCbx.loading = true)),
      switchMap(value => this.searchEmployee(value))
    ).subscribe(result => {
      this.filterdEmployees = result.items;
      this.empCbx.loading = false;
    });

    this.loadEmployee();
  }

  get email() { return this.formGroup.get('email'); }

  get phone() { return this.formGroup.get('phone'); }

  get hostName() { return this.formGroup.get('hostName'); }

  get password() { return this.formGroup.get('password'); }

  get confirmPassword() { return this.formGroup.get('confirmPassword'); }

  getHostName(host) {
    return environment.catalogScheme + '://' + (host || '') + '.' + environment.catalogHost;
  }

  onSubmit() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    value.employeeId = value.employeeAdmin ? value.employeeAdmin.id : null;
    this.tenantService.register(value).subscribe((result: any) => {
      this.registerSuccess = true;
      this.registerResult = result;
    }, (err) => {
      console.log(err);
    });
  }

  createNew() {
    this.registerSuccess = false;
    this.formGroup.reset();
  }

  loadEmployee() {
    this.searchEmployee().subscribe(
      result => {
        this.filterdEmployees = result.items;
      }
    )
  }

  searchEmployee(q?: string) {
    var val = new EmployeeAdminPaged();
    val.search = q || '';
    val.limit = 20;
    val.offset = 0;
    return this.employeeAdminService.getPaged(val);
  }
}
