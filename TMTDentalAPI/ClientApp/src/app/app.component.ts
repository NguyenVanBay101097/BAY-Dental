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

  }

  logout() {
    this.authService.logout();
    this.router.navigate(['login']);
  }

  onPrintInvoice() {
    const invoiceIds = ['101', '102'];
    this.printService
      .printDocument('invoice', invoiceIds);
  }
}
