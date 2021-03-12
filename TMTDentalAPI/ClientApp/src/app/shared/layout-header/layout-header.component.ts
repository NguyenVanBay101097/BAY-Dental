import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
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
import { Observable, of, Subject } from 'rxjs';
import { catchError, concat, count, debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { SearchAllService } from '../search-all.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgSelectComponent } from '@ng-select/ng-select';

@Component({
  selector: 'app-layout-header',
  templateUrl: './layout-header.component.html',
  styleUrls: ['./layout-header.component.css']
})
export class LayoutHeaderComponent implements OnInit {
  searchResults: any[] = [];
  selectedResult: any;
  arrowkeyLocation = 0;
  resultSelection: string = 'all';
  userChangeCurrentCompany: UserChangeCurrentCompanyVM;
  searchString: string = '';
  searchUpdate = new Subject<string>();
  expire = '';

  search$: Observable<any[]>;
  searchLoading = false;
  searchInput$ = new Subject<string>();
  @ViewChild('searchAllSelect', {static: true}) searchAllSelect: NgSelectComponent;
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
    if (localStorage.getItem('user_change_company_vm')) {
      this.userChangeCurrentCompany = JSON.parse(localStorage.getItem('user_change_company_vm'));
    }
    this.authService.currentUser.subscribe(result => {
      if (result) {
        this.loadChangeCurrentCompany();
        this.loadExpire();
      }
    });

    this.search$ = this.searchInput$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => this.searchLoading = true),
      switchMap(term => this.onSearch(term).pipe(
          catchError(() => of([])), // empty list on error
          tap(() => this.searchLoading = false)
      ))
  );
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

  onSearchChange(item) {
    setTimeout(() => {
      this.selectedResult = null;
    });

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

  onSearch(q?: string) {
    var value = {
      limit: 20,
      search: q || '',
      resultSelection: this.resultSelection ? this.resultSelection : ''
    }
    return this.searchAllService.getAll(value);
  }
}
