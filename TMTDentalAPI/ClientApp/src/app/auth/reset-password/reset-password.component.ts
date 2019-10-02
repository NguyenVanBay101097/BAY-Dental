import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  resetPwdForm: FormGroup;
  constructor(public authService: AuthService, public router: Router, private fb: FormBuilder, private notificationService: NotificationService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.resetPwdForm = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
    });
  }

  submit() {
    if (!this.resetPwdForm.valid) {
      return;
    }

    var val = this.resetPwdForm.value;
    val.code = this.route.snapshot.queryParams['code'];

    this.authService.resetPassword(val).subscribe(() => {
      this.notificationService.show({
        content: 'Reset mật khẩu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    }, error => {
      console.log('error login', error);
    });
  }
}


