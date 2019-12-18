import { Component } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Router } from '@angular/router';
import { PrintService } from './print.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'tmt-dental';
  constructor(public authService: AuthService, private router: Router, public printService: PrintService) {
    this.loadGroups();
  }

  loadGroups() {
    var groups = localStorage.getItem('groups');
    if (!groups && this.authService.isAuthenticated()) {
      this.authService.getGroups().subscribe(result => {
        localStorage.setItem('groups', JSON.stringify(result));
      });
    }
  }
}
