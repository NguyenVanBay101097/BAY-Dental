import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { TenantService } from '../tenant.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { EmployeeAdminPaged, EmployeeAdminService } from 'app/employee-admins/employee-admin.service';

@Component({
  selector: 'app-tenant-update-info-dialog',
  templateUrl: './tenant-update-info-dialog.component.html',
  styleUrls: ['./tenant-update-info-dialog.component.css']
})

export class TenantUpdateInfoDialogComponent implements OnInit {
  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;
  filterdEmployees: any[];
  title = 'Cập nhật thông tin';
  formGroup: FormGroup;
  id: string;

  constructor(
    private fb: FormBuilder,
    private intlService: IntlService,
    private tenantService: TenantService,
    public activeModal: NgbActiveModal,
    private employeeAdminService: EmployeeAdminService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      email: [null, [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      customerSource: null,
      employeeAdmin: null,
      companyName: ['', Validators.required],
      address: null
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
    this.loadRecord();
  }

  get email() { return this.formGroup.get('email'); }

  get phone() { return this.formGroup.get('phone'); }

  loadRecord() {
    this.tenantService.get(this.id).subscribe((result: any) => {
      console.log(result);
      
      this.formGroup.patchValue(result);
    });
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

  onSave() {
    var val = this.formGroup.value;
    val.employeeId = val.employeeAdmin ? val.employeeAdmin.id : '';
    this.tenantService.updateInfo(this.id, val).subscribe(() => {
      this.activeModal.close(true);
    }, (err) => {
      if (err.message) {
        alert(err.message);
      }
    });
  }
}
