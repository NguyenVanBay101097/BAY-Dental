import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeAdminService } from '../employee-admin.service';

@Component({
  selector: 'app-employee-admin-register',
  templateUrl: './employee-admin-register.component.html',
  styleUrls: ['./employee-admin-register.component.css']
})
export class EmployeeAdminRegisterComponent implements OnInit {

  registerForm: FormGroup;
  loading = false;
  id : string;
  title : string;
  constructor(public employeeAdminService: EmployeeAdminService, public router: Router, private fb: FormBuilder) { }

  ngOnInit() {
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
    });
  }

  onSubmit() {
    if (!this.registerForm.valid) {
      return;
    }
    this.loading = true;
    this.employeeAdminService.create(this.registerForm.value).subscribe(() => {
      this.loading = false;
      this.router.navigate(['/employee-admins']);
    }, error => {
      this.loading = false;
      console.log('error', error);
    });
  }

}
