import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { map } from 'rxjs/operators';
import { CompanyService, CompanyPaged, CompanyBasic } from '../company.service';
import { CompanyCuDialogComponent } from '../company-cu-dialog/company-cu-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CompanyListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  active = true;
  opened = false;
  loading = false;

  constructor(private companyService: CompanyService, private modalService: NgbModal, public intl: IntlService, 
    private notificationService: NotificationService) { }

  filterCompanyStatus = [
    { name: 'Đang hoạt động', value: true },
    { name: 'Ngưng hoạt động', value: false }
  ];
  defaultFilter: any = this.filterCompanyStatus[0];

  ngOnInit() {
    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CompanyPaged();
    val.offset = this.skip;
    val.limit = this.limit;
    val.active = this.active;
    this.companyService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(res);
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  createItem() {
    let modalRef = this.modalService.open(CompanyCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm chi nhánh';

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: CompanyBasic) {
    let modalRef = this.modalService.open(CompanyCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa chi nhánh';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item: CompanyBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa chi nhánh';

    modalRef.result.then(() => {
      this.companyService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

  actionActive(item, active) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    if (active) {
      modalRef.componentInstance.title = "Hiện chi nhánh " + item.name;
      modalRef.componentInstance.body = "Bạn có chắc chắn muốn hiện chi nhánh " + item.name;
      modalRef.result.then(() => {
        this.companyService.actionUnArchive([item.id]).subscribe((res) => {
          this.loadDataFromApi();
        }, (err) => {
    
        })
      }, () => {
      });
    } else {
      modalRef.componentInstance.title = "Ẩn chi nhánh " + item.name;
      modalRef.componentInstance.body = "Bạn có chắc chắn muốn ẩn chi nhánh " + item.name;
      modalRef.result.then(() => {
        this.companyService.actionArchive([item.id]).subscribe((res) => {
          this.loadDataFromApi();
        }, (err) => {
    
        })
      }, () => {
      });
    }

  }

  onStateSelectChange(event){
    this.active = event != null ? event.value : '';
    this.loadDataFromApi();
  }

}

