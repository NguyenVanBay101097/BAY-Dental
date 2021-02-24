import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeAdminDisplay, EmployeeAdminService } from '../employee-admin.service';

@Component({
  selector: 'app-employee-admin-update',
  templateUrl: './employee-admin-update.component.html',
  styleUrls: ['./employee-admin-update.component.css']
})
export class EmployeeAdminUpdateComponent implements OnInit {

  updateForm: FormGroup;
  loading = false;
  id : string;
  title : string;
  employeeAdminDisplay : any; 
  constructor(public employeeAdminService: EmployeeAdminService, 
    public router: Router, 
    private fb: FormBuilder,
    private activeRoute: ActivatedRoute,) { }

  ngOnInit() {
    this.updateForm = this.fb.group({
      name: ['', Validators.required],
    });

    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.loadEmployeeInfo();
  }

  onSubmit() {
    if (!this.updateForm.valid) {
      return;
    }
    // this.loading = true;
    // this.employeeAdminService.update(this.updateForm.value).subscribe(() => {
    //   this.loading = false;
    //   this.router.navigate(['/employee-admins']);
    // }, error => {
    //   this.loading = false;
    //   console.log('error', error);
    // });
  }

  loadEmployeeInfo(){
    this.employeeAdminService.get(this.id).subscribe(
      rs => {
        this.employeeAdminDisplay = rs;
      });
  }

}
