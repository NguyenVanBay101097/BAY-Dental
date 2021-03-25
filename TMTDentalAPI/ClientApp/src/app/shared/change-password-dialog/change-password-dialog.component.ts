import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'src/app/auth/auth.service';
import { MustMatch } from '../must-match-validator';

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-password-dialog.component.html',
  styleUrls: ['./change-password-dialog.component.css']
})
export class ChangePasswordDialogComponent implements OnInit {
  resetPwdForm: FormGroup;
  error: string;
  submitted = false;
  constructor(public authService: AuthService, public router: Router, private fb: FormBuilder, private notificationService: NotificationService,
    private route: ActivatedRoute, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.resetPwdForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    },
    {validators: MustMatch('newPassword','confirmPassword')}
    );
  }

  get f(){
    return this.resetPwdForm.controls;
  }

  submit() {
    this.submitted = true;
    if (!this.resetPwdForm.valid) {
      return;
    }

    var val = this.resetPwdForm.value;
    this.authService.changePassword(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
        this.notificationService.show({
          content: 'Đổi mật khẩu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      } else {
        this.error = result.message;
      }
    }, error => {
      this.error = error.error.error;
    });
  }
}


