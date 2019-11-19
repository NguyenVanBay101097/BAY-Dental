import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { MustMatch } from '../shared/must-match-validator';
import { TenantService } from '../tenants/tenant.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-trial-registration',
  templateUrl: './trial-registration.component.html',
  styleUrls: ['./trial-registration.component.css']
})
export class TrialRegistrationComponent implements OnInit {
  formGroup: FormGroup;
  constructor(private fb: FormBuilder, private tenantService: TenantService,
    private router: Router) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      email: [null, [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      companyName: ['', Validators.required],
      hostName: ['', [Validators.required, Validators.pattern('^[a-z0-9]*$'), Validators.minLength(6)]],
      userName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: null,
    }, {
      validators: MustMatch('password', 'confirmPassword')
    });
  }

  get email() { return this.formGroup.get('email'); }

  get phone() { return this.formGroup.get('phone'); }

  get hostName() { return this.formGroup.get('hostName'); }

  get password() { return this.formGroup.get('password'); }

  get confirmPassword() { return this.formGroup.get('confirmPassword'); }

  getHostName() {
    return environment.catalogScheme + '://' + this.hostName.value + '.' + environment.catalogHost;
  }

  onSubmit() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    this.tenantService.register(value).subscribe(() => {
      value.hostName = this.getHostName();
      this.router.navigate(['/register-success'], { state: value });
    }, () => {
      alert('Register fail.');
    });
  }
}
