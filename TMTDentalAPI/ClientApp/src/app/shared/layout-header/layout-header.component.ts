import { Component, OnInit } from '@angular/core';
import { NavSidebarService } from '../nav-sidebar.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChangePasswordDialogComponent } from '../change-password-dialog/change-password-dialog.component';
import { AuthService } from 'src/app/auth/auth.service';
import { Router } from '@angular/router';
import { CompanyBasic } from 'src/app/companies/company.service';
import { UserChangeCurrentCompanyVM, UserService } from 'src/app/users/user.service';

@Component({
  selector: 'app-layout-header',
  templateUrl: './layout-header.component.html',
  styleUrls: ['./layout-header.component.css']
})
export class LayoutHeaderComponent implements OnInit {

  userChangeCurrentCompany: UserChangeCurrentCompanyVM;
  constructor(private sidebarService: NavSidebarService, private modalService: NgbModal,
    public authService: AuthService, private router: Router, private userService: UserService) { }

  ngOnInit() {
    this.authService.currentUser.subscribe(user => {
      if (user) {
        this.userService.getChangeCurrentCompany().subscribe(result => {
          this.userChangeCurrentCompany = result;
        });
      }
    });
  }

  switchCompany(companyId) {
    this.userService.switchCompany({ companyId: companyId }).subscribe(() => {
      this.authService.refresh().subscribe(result => {
        window.location.reload();
      });
    });
  }

  toggleSidebar() {
    this.sidebarService.toggle();
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
