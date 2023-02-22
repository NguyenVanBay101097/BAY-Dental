import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app/auth/auth.service';

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-password-dialog.component.html',
  styleUrls: ['./change-password-dialog.component.css']
})
export class ChangePasswordDialogComponent implements OnInit {
  resetPwdForm: FormGroup;
  error: string;
  constructor(public authService: AuthService, public router: Router, private fb: FormBuilder,
    private route: ActivatedRoute, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.resetPwdForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
      confirmPassword: ['', Validators.required],
    });
  }

  submit() {
    if (!this.resetPwdForm.valid) {
      return;
    }

    var val = this.resetPwdForm.value;

    this.authService.changePassword(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
        this.authService.logout();
        this.router.navigate(['login']);
      } else {
        this.error = result.message;
      }
    }, error => {
      this.error = error.error.error;
    });
  }
}


