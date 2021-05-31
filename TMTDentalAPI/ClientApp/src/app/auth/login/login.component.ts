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

    this.authService.login(this.loginForm.value).subscribe((data: any) => {
      if (data.succeeded == false) {
        alert(data.message);
      } else {
        localStorage.setItem('user_permission', JSON.stringify(data));
        this.router.navigateByUrl('/app/dashboard');
      }
    }, error => {
      console.log('error login', error);
    });
  }

  logout() {
    this.authService.logout();
  }

  get f() {
    return this.loginForm.controls;
  }
}
