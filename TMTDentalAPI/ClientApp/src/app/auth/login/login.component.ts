import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  submitted: boolean = false;
  constructor(public authService: AuthService, public router: Router, private fb: FormBuilder) { }

  ngOnInit() {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      remember_me: false
    });
  }

  login() {
    this.submitted = true;

    if (!this.loginForm.valid) {
      return;
    }

    this.authService.login(this.loginForm.value).subscribe(data => {
      if (data.succeeded) {
        this.router.navigateByUrl('/');
      } else {
        // alert("Tài khoản hoặc mật khẩu không đúng");
        alert(data.message);
      }
      this.submitted = false;
    }, error => {
      console.log('error login', error);
      this.submitted = false;
    });
  }

  logout() {
    this.authService.logout();
  }

  get f() {
    return this.loginForm.controls;
  }
}
