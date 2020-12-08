import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {
  forgotPwdForm: FormGroup;
  submitted = false;
  constructor(public authService: AuthService, public router: Router, private fb: FormBuilder, private notificationService: NotificationService) { }

  ngOnInit() {
    this.forgotPwdForm = this.fb.group({
      email: ['', Validators.required],
    });
  }

  get emailControl() {
    return this.forgotPwdForm.get('email');
  }

  submit() {
    debugger;
    this.submitted = true;
    if (!this.forgotPwdForm.valid) {
      return;
    }

    this.authService.forgotPassword(this.forgotPwdForm.value).subscribe(data => {
      if (data.success) {
        this.notificationService.show({
          content: data.message,
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      } else {
        this.notificationService.show({
          content: data.message,
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
    }, error => {
      console.log('error login', error);
    });
  }
}

