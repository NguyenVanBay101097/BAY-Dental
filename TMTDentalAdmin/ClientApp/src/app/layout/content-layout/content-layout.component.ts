import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChangePasswordDialogComponent } from '@shared/change-password-dialog/change-password-dialog.component';
import { AuthService } from 'app/auth/auth.service';

@Component({
  selector: 'app-content-layout',
  templateUrl: './content-layout.component.html',
  styleUrls: ['./content-layout.component.css']
})
export class ContentLayoutComponent implements OnInit {

  public navItems: any[] = [
    { name: 'Danh sach dang ky', url: '/tenants', icon: 'fas fa-users' },
    { name: 'Nhan vien', url: '/employee-admins', icon: 'fas fa-users' }
  ]

  constructor(
    private modalService: NgbModal,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
  }

  changePassword() {
    let modalRef = this.modalService.open(ChangePasswordDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Đổi mật khẩu';

    modalRef.result.then(() => {
    }, () => {
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['login']);
  }

}
