import { Component, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { GridDataResult, PageChangeEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { PartnerPaged, PartnerBasic } from '../partner-simple';
import { map, debounceTime, distinctUntilChanged, tap, switchMap, subscribeOn } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NgbModal, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnerImportComponent } from '../partner-import/partner-import.component';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnerInfoPaged, PartnerService } from '../partner.service';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { PartnerCategoryPopoverComponent } from './partner-category-popover/partner-category-popover.component';
import { PartnersBindingDirective } from 'src/app/shared/directives/partners-binding.directive';
import { PartnerCustomerAutoGenerateCodeDialogComponent } from '../partner-customer-auto-generate-code-dialog/partner-customer-auto-generate-code-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { MemberLevelAutoCompleteReq, MemberLevelService } from 'src/app/member-level/member-level.service';
import { values } from 'lodash';

@Component({
  selector: 'app-partner-customer-list',
  templateUrl: './partner-customer-list.component.html',
  styleUrls: ['./partner-customer-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerCustomerListComponent implements OnInit {

  gridData: GridDataResult;
  filter = new PartnerInfoPaged();
  loading = false;
  opened = false;

  filteredCategs: PartnerCategoryBasic[];
  searchUpdate = new Subject<string>();

  @ViewChild("categMst", { static: true }) categMst: MultiSelectComponent;
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;
  @ViewChild('cbxLevel', { static: true }) public cbxLevel: ComboBoxComponent;

  canExport = false;
  canAdd = false;
  canImport = false;
  canFilterPartnerCategory = false;
  canUpdateExcel = false;
  
  OrderResiduals: { text: string, value: number }[] = [
    { text: 'Có dự kiến thu', value: 1 },
    { text: 'Không có dự kiến thu', value: 0}
  ];

  totalDebits: { text: string, value: number }[] = [
    { text: 'Có công nợ', value: 1 },
    { text: 'Không có công nợ', value: 0 }
  ];
  memberLevels = [];
  orderStateDisplay = {
    'sale':'Đang điều trị',
    'done':'Hoàn thành',
    'draft':'Chưa phát sinh'
  };

  cbxPopupSettings = {
    width: 'auto'
  };

  color='red';

  constructor(private partnerService: PartnerService, private modalService: NgbModal,
    private partnerCategoryService: PartnerCategoryService, private notificationService: NotificationService, 
    private checkPermissionService: CheckPermissionService,
    private memberLevelService: MemberLevelService
    
    ) { }

  ngOnInit() {
    this.initFilter();
    this.refreshData();
    this.checkRole();
    this.loadMemberLevel();
    this.cbxLevel.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.cbxLevel.loading = true)),
      switchMap(value => this.searchMemberLevel(value))
    ).subscribe(result => {
      this.memberLevels = result.items;
      this.cbxLevel.loading = false;
    });
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.refreshData();
        this.filter.offset = 0;
      });

    if (this.canFilterPartnerCategory && this.categMst) {
      this.categMst.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.categMst.loading = true)),
        switchMap((value) => this.searchCategories(value))
      )
      .subscribe((result) => {
        this.filteredCategs = result;
        this.categMst.loading = false;
      });
    }

    if(this.canFilterPartnerCategory){
      this.loadFilteredCategs();
    }
  }

  initFilter() {
    this.filter.limit = 1;
    this.filter.offset = 20;
    this.filter.hasOrderResidual = -1;
    this.filter.hasTotalDebit = -1;
    this.filter.orderState='';
  }

  refreshData() {
    var val = Object.assign({}, this.filter);
    this.loading = true;
   this.partnerService.getPartnerInfoPaged2(val).subscribe(res => {
     this.gridData = <GridDataResult> {
      data : res.items,
      total : res.totalItems
     };
   },()=> {},
   ()=>{
     this.loading = false;
   }
   );
  }

  toggleTagsPopOver(popover: any, dataItem: any) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      popover.open({ dataItem });
    }
  }

  searchMemberLevel(s?) {
    var val = new MemberLevelAutoCompleteReq();
    val.offset = 0;
    val.limit = 20;
    val.search = s || '';
    return this.memberLevelService.autoComplete(val);
  }

  loadMemberLevel(){
    this.searchMemberLevel().subscribe(res => {
      this.memberLevels = res.items;
    });
  }

  importFromExcel() {
    const modalRef = this.modalService.open(PartnerImportComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.type = 'customer';
    modalRef.componentInstance.title = 'Import Excel'
    modalRef.result.then(() => {
      this.refreshData();
    }, () => {
    });
  }

  updateFromExcel() {
    const modalRef = this.modalService.open(PartnerImportComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.type = 'customer';
    modalRef.componentInstance.title = 'Cập nhật Excel';
    modalRef.componentInstance.isUpdate = true;
    modalRef.result.then(() => {
      this.refreshData();
    }, () => {
    });
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(
      (result) => (this.filteredCategs = result)
    );
  }

  onCategChange(value) {
   this.filter.categIds = value.map(x=> x.id);
    this.filter.offset = 0;
    this.refreshData();
  }

  searchCategories(search?: string) {
    var val = new PartnerCategoryPaged();
    val.search = search;
    return this.partnerCategoryService.autocomplete(val);
  }

  exportPartnerExcelFile() {
    var val = Object.assign({}, this.filter);
    this.partnerService.exportPartnerExcelFile(val).subscribe((rs) => {
      let filename = "danh_sach_khach_hang";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  createItem() {
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khách hàng';
    modalRef.result.then(() => {
      this.refreshData();
    }, er => { })
  }

  editItem(item: any) {
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa khách hàng';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.refreshData();
    }, () => {
    })
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa khách hàng';

    modalRef.result.then(() => {
      this.partnerService.delete(item.Id).subscribe(() => {
        this.refreshData();
      }, () => {
      });
    }, () => {
    });
  }

  setupAutoCodeCustomer() {
    let modalRef = this.modalService.open(PartnerCustomerAutoGenerateCodeDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.code = 'customer';

    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    }, () => {
    });
  }

  checkRole(){
    this.canExport = this.checkPermissionService.check(['Basic.Partner.Export']);
    this.canAdd = this.checkPermissionService.check(['Basic.Partner.Create']);
    this.canImport = this.checkPermissionService.check(['Basic.Partner.Create']);
    this.canFilterPartnerCategory = this.checkPermissionService.check(["Catalog.PartnerCategory.Read"])
    this.canUpdateExcel = this.checkPermissionService.check(["Basic.Partner.Update"]);
  }

  onResidualSelect(e)
  {
    this.filter.hasOrderResidual = e? e.value : '';
    this.filter.offset = 0;
    this.refreshData();
  }

  onDebitSelect(e)
  {
    this.filter.hasTotalDebit = e? e.value : '';
    this.filter.offset = 0;
    this.refreshData();
  }

  onMemberLevelSelect(e)
  {
    this.filter.memberLevelId = e? e.id : '';
    this.filter.offset = 0;
    this.refreshData();
  }

  public onPageChange(event: PageChangeEvent): void {
    this.filter.offset = event.skip;;
    this.refreshData();
  }
}
