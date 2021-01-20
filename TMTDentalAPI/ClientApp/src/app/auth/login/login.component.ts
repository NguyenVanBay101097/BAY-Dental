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
  constructor(public authService: AuthService, public router: Router, private fb: FormBuilder) { }

  ngOnInit() {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      remember_me: false
    });
  }

  login() {
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
    }, error => {
      console.log('error login', error);
    });
  }

  logout() {
    this.authService.logout();
  }
}
