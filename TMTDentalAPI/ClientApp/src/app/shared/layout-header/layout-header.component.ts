import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectComponent } from '@ng-select/ng-select';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { AuthService } from 'src/app/auth/auth.service';
import { WebService } from 'src/app/core/services/web.service';
import { UserChangeCurrentCompanyVM, UserService } from 'src/app/users/user.service';
import { ChangePasswordDialogComponent } from '../change-password-dialog/change-password-dialog.component';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { NavSidebarService } from '../nav-sidebar.service';
import { UserProfileEditComponent } from '../user-profile-edit/user-profile-edit.component';

@Component({
  selector: 'app-layout-header',
  templateUrl: './layout-header.component.html',
  styleUrls: ['./layout-header.component.css']
})
export class LayoutHeaderComponent implements OnInit {
  userChangeCurrentCompany: UserChangeCurrentCompanyVM;
  searchString: string = '';
  searchUpdate = new Subject<string>();
  expire = '';
  @ViewChild('searchAllSelect', {static: true}) searchAllSelect: NgSelectComponent;
  constructor(
    private sidebarService: NavSidebarService,
    private modalService: NgbModal,
    public authService: AuthService,
    private router: Router,
    private userService: UserService,
    private webService: WebService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    if (localStorage.getItem('user_change_company_vm')) {
      this.userChangeCurrentCompany = JSON.parse(localStorage.getItem('user_change_company_vm'));
    }
    this.authService.currentUser.subscribe(result => {
      if (result) {
        this.loadChangeCurrentCompany();
        this.loadExpire();
      }
    });
  }

  loadExpire() {
    this.webService.getExpire().subscribe((res: any) => {
      if (res && res.expireText) {
        this.expire = res.expireText;
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
      this.authService.refresh().subscribe(() => {
        this.userService.getChangeCurrentCompany().subscribe(result => {
          localStorage.setItem('user_change_company_vm', JSON.stringify(result));
          var userInfo = JSON.parse(localStorage.getItem("user_info"));
          localStorage.removeItem('user_info');
          userInfo.companyId = result.currentCompany.id;
          localStorage.setItem('user_info', JSON.stringify(userInfo));
          window.location.reload();
        });
      });
    });
  }

  toggleSidebar() {
    this.sidebarService.toggle();
  }

  changePassword() {
    let modalRef = this.modalService.open(ChangePasswordDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = '?????i m???t kh???u';

    modalRef.result.then(() => {
    }, () => {
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  removeSampleData() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'X??a d??? li???u m???u';
    modalRef.componentInstance.body = 'H??? th???ng s??? x??a to??n b??? d??? li???u v??? tr???ng th??i ban ?????u, b???n c?? ch???c ch???n mu???n x??a?';
    modalRef.result.then(() => {
      this.webService.removeSampleData().subscribe(
        () => {
          this.notificationService.show({
            content: 'X??a d??? li???u m???u th??nh c??ng',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          window.location.reload();
        }
      )
    }, () => { });

  }

  getAvatarImgSource(obj: string) {
    if (obj) {
      return obj;
    } else {
      return '/assets/images/user_avatar.png';
    }
  }

  editProfile(item) {
    let modalRef = this.modalService.open(UserProfileEditComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a th??ng tin';
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
