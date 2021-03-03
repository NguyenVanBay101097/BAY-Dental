import { Component, HostListener, OnInit } from '@angular/core';
import { NavSidebarService } from '../nav-sidebar.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChangePasswordDialogComponent } from '../change-password-dialog/change-password-dialog.component';
import { AuthService } from 'src/app/auth/auth.service';
import { Router } from '@angular/router';
import { CompanyBasic } from 'src/app/companies/company.service';
import { UserChangeCurrentCompanyVM, UserService } from 'src/app/users/user.service';
import { environment } from 'src/environments/environment';
import { UserProfileEditComponent } from '../user-profile-edit/user-profile-edit.component';
import { WebService } from 'src/app/core/services/web.service';
import { IrConfigParameterService } from 'src/app/core/services/ir-config-parameter.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SearchAllService } from '../search-all.service';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-layout-header',
  templateUrl: './layout-header.component.html',
  styleUrls: ['./layout-header.component.css']
})
export class LayoutHeaderComponent implements OnInit {
  searchResults: any[] = [];
  isSearch = false;
  searchInput: string;
  arrowkeyLocation = 0;
  formSelect: FormGroup;
  resultSelection: string;
  searchUpdate = new Subject<string>();
  userChangeCurrentCompany: UserChangeCurrentCompanyVM;
  expire = '';
  constructor(
    private sidebarService: NavSidebarService,
    private modalService: NgbModal,
    public authService: AuthService,
    private router: Router,
    private userService: UserService,
    private webService: WebService,
    private fb: FormBuilder,
    private searchAllService: SearchAllService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formSelect = this.fb.group({
      type: 'all',
      searchValue: ''
    })
    if (localStorage.getItem('user_change_company_vm')) {
      this.userChangeCurrentCompany = JSON.parse(localStorage.getItem('user_change_company_vm'));
    }
    this.authService.currentUser.subscribe(result => {
      if (result) {
        this.loadChangeCurrentCompany();
        this.loadExpire();
      }
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.fetchAll();
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
    let modalRef = this.modalService.open(ChangePasswordDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Đổi mật khẩu';

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
    modalRef.componentInstance.title = 'Xóa dữ liệu mẫu';
    modalRef.componentInstance.body = 'Hệ thống sẽ xóa toàn bộ dữ liệu về trạng thái ban đầu, bạn có chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.webService.removeSampleData().subscribe(
        () => {
          this.notificationService.show({
            content: 'Xóa dữ liệu mẫu thành công',
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

  fetchAll() {
    this.resultSelection = this.formSelect.get('type').value;
    this.isSearch = true;
    var value = {
      limit: 20,
      search: this.searchInput ? this.searchInput : '',
      resultSelection: this.resultSelection ? this.resultSelection : ''
    }

    if (!value || !value.search || value.search === '') {
      this.searchResults = [];
      this.isSearch = false;
    } else {
      this.searchAllService.getAll(value).subscribe(
        result => {
          console.log(result);

          this.searchResults = result ? result : [];
        }
      )
    }

  }

  clickOut() {
    this.isSearch = false;
    this.arrowkeyLocation = 0;
  }

  clickItem(item) {
    this.arrowkeyLocation = 0;
    this.isSearch = false;
    switch (item.type) {
      case "customer":
        window.open(`/partners/customer/${item.id}/overview`, '_blank');
        break;
      case "supplier":
        window.open(`/partners/supplier/${item.id}/info`, '_blank');
        break;
      case "sale-order":
        window.open(`/sale-orders/form?id=${item.id}`, '_blank');
        break;
      default:
        break;
    }
  }

  @HostListener('document:keydown', ['$event']) onKeydownHandler(event: KeyboardEvent) {
    switch (event.keyCode) {
      case 38: // this is the ascii of arrow up
        if (this.isSearch && this.arrowkeyLocation > 0) {
          this.arrowkeyLocation--;
        } else if (this.isSearch && this.searchResults) {
          this.arrowkeyLocation = this.searchResults.length - 1;
        }
        break;
      case 40: // this is the ascii of arrow down
        if (this.isSearch && this.searchResults && ((this.searchResults.length - 1) > this.arrowkeyLocation)) {
          this.arrowkeyLocation++;
        } else if (this.isSearch && this.searchResults) {
          this.arrowkeyLocation = 0;
        }
        break;
      case 13: // this is the ascii of enter
        if (this.isSearch && this.searchResults && this.searchResults.length > 0) {
          var item = this.searchResults[this.arrowkeyLocation];
          if (item) {
            this.clickItem(item);
          }
        }
        break;
    }
  }


  onSearchClick() {
    this.isSearch = this.searchResults.length > 0 ? true : false;
  }
}
