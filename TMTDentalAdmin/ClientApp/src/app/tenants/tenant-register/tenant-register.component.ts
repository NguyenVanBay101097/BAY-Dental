

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TenantService } from '../tenant.service';

@Component({
  selector: 'app-tenant-register',
  templateUrl: './tenant-register.component.html',
  styleUrls: ['./tenant-register.component.css']
})
export class TenantRegisterComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;
  constructor(public tenantService: TenantService, public router: Router, private fb: FormBuilder) { }

  ngOnInit() {
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
      phone: ['', Validators.required],
      email: null,
      companyName: ['', Validators.required],
      hostName: ['', Validators.required],
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onSubmit() {
    if (!this.registerForm.valid) {
      return;
    }
    this.loading = true;
    this.tenantService.register(this.registerForm.value).subscribe(() => {
      this.loading = false;
      // this.router.navigate(['/tenants']);
    }, error => {
      this.loading = false;
      console.log('error', error);
    });
  }
}
