import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { EmployeeAdminService } from '../employee-admin.service';

@Component({
  selector: 'app-employee-admin-update',
  templateUrl: './employee-admin-update.component.html',
  styleUrls: ['./employee-admin-update.component.css']
})
export class EmployeeAdminUpdateComponent implements OnInit {

  updateForm: FormGroup;
  loading = false;
  id: string;
  title: string;
  employeeAdminDisplay: any;
  constructor(public employeeAdminService: EmployeeAdminService,
    public router: Router,
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private activeRoute: ActivatedRoute,) { }

  ngOnInit() {
    this.activeRoute.params
      .subscribe(
        (params: Params) => {
          this.id = params['id'];
          console.log(this.id);
        }
      );
    this.updateForm = this.fb.group({
      name: ['', Validators.required],
    });
    this.loadEmployeeInfo();
  }

  onSubmit() {
    if (!this.updateForm.valid) {
      return;
    }
    this.loading = true;
    this.employeeAdminService.update(this.id, this.updateForm.value).subscribe(() => {
      this.loading = false;
      this.notificationService.show({
        content: 'Cập nhật thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.router.navigate(['/employee-admins']);
    }, error => {
      this.loading = false;
      this.notificationService.show({
        content: 'Cập nhật không thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      console.log('error', error);
    });
  }

  loadEmployeeInfo() {
    this.employeeAdminService.get(this.id).subscribe(
      rs => {
        this.employeeAdminDisplay = rs;
        this.loadValueForm();
      });
  }

  loadValueForm() {
    this.updateForm.patchValue({
      name: this.employeeAdminDisplay.name,
    });
  }
  get name() { return this.updateForm.get('name'); }
}
 