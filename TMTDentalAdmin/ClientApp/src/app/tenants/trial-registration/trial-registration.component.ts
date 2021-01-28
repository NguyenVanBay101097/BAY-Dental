import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { MustMatch } from '../../shared/must-match-validator';
import { TenantService } from '../tenant.service';
import { Router } from '@angular/router';
import { NullTemplateVisitor } from '@angular/compiler';

@Component({
  selector: 'app-trial-registration',
  templateUrl: './trial-registration.component.html',
  styleUrls: ['./trial-registration.component.css']
})
export class TrialRegistrationComponent implements OnInit {
  formGroup: FormGroup;
  registerSuccess = false;
  registerResult: any;
  constructor(private fb: FormBuilder, private tenantService: TenantService,
    private router: Router) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      email: [null, [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      customerSource: null,
      supporterName: null,
      companyName: ['', Validators.required],
      hostName: ['', [Validators.required, Validators.pattern('^[a-z0-9]*$'), Validators.minLength(4)]],
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

  getHostName(host) {
    return environment.catalogScheme + '://' + host + '.' + environment.catalogHost;
  }

  onSubmit() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    this.tenantService.register(value).subscribe((result: any) => {
      console.log(result);
      this.registerSuccess = true;
      this.registerResult = result;
    }, (err) => {
      console.log(err);
    });
  }
}
