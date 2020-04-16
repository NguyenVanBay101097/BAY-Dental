import { Component, OnInit } from '@angular/core';
import { NavSidebarService } from '../nav-sidebar.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChangePasswordDialogComponent } from '../change-password-dialog/change-password-dialog.component';
import { AuthService } from 'src/app/auth/auth.service';
import { Router } from '@angular/router';
import { CompanyBasic } from 'src/app/companies/company.service';
import { UserChangeCurrentCompanyVM, UserService } from 'src/app/users/user.service';
import { environment } from 'src/environments/environment';
import { UserProfileEditComponent } from '../user-profile-edit/user-profile-edit.component';

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
    if (localStorage.getItem('user_change_company_vm')) {
      this.userChangeCurrentCompany = JSON.parse(localStorage.getItem('user_change_company_vm'));
    }

    this.authService.currentUser.subscribe(result => {
      if (result) {
        this.loadChangeCurrentCompany();
      }
    });
  }

  loadChangeCurrentCompany() {
    this.userService.getChangeCurrentCompany().subscribe(result => {
      this.userChangeCurrentCompany = result;
      localStorage.setItem('user_change_company_vm', JSON.stringify(result));
    });
  }

  switchCompany(companyId) {
    this.userService.switchCompany({ companyId: companyId }).subscribe(() => {
      this.authService.refresh().subscribe(result => {
        this.userService.getChangeCurrentCompany().subscribe(result => {
          localStorage.setItem('user_change_company_vm', JSON.stringify(result));
          window.location.reload();
        });
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

  getAvatarImgSource(obj: string) {
    if (obj)
      return environment.uploadDomain + 'api/Web/Image/' + obj;
    else
      return '/assets/images/user_avatar.png';
  }

  editProfile(item) {
    let modalRef = this.modalService.open(UserProfileEditComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa thông tin';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.authService.getUserInfo(item.id).subscribe(
        rs => {
          var user_info = JSON.parse(localStorage.getItem('user_info'));
          user_info.name = rs.name;
          user_info.avatar = rs.avatar;
          localStorage.setItem('user_info', JSON.stringify(user_info));
        }
      )
    }, () => {
    });
  }
}
