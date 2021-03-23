import { Component, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
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
import { PartnerService } from '../partner.service';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { PartnerCategoryPopoverComponent } from './partner-category-popover/partner-category-popover.component';
import { PartnersBindingDirective } from 'src/app/shared/directives/partners-binding.directive';
import { PartnerCustomerAutoGenerateCodeDialogComponent } from '../partner-customer-auto-generate-code-dialog/partner-customer-auto-generate-code-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';

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
  limit = 10;
  skip = 0;
  loading = false;
  opened = false;

  search: string;
  searchCategs: PartnerCategoryBasic[];
  filteredCategs: PartnerCategoryBasic[];
  searchUpdate = new Subject<string>();

  @ViewChild("categMst", { static: true }) categMst: MultiSelectComponent;
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;
  @ViewChild(PartnersBindingDirective, { static: true }) dataBinding: PartnersBindingDirective;

  gridFilter: CompositeFilterDescriptor = {
    logic: "and",
    filters: [
      { field: "Customer", operator: "eq", value: true },
    ]
  };

  gridSort = [{ field: 'DateCreated', dir: 'desc' }];
  advanceFilter: any = {
    // expand: 'Tags,Source',
    params: {}
  };

  constructor(private partnerService: PartnerService, private modalService: NgbModal,
    private partnerCategoryService: PartnerCategoryService, private notificationService: NotificationService) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.dataBinding.filter = this.generateFilter();
        this.dataBinding.skip = 0;
        this.refreshData();
        this.skip = 0;
      });

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

    this.loadFilteredCategs();
  }

  updateFilter() {
    this.gridFilter = this.generateFilter();
  }

  refreshData() {
    this.dataBinding.rebind();
  }

  generateFilter() {
    var filter: CompositeFilterDescriptor = {
      logic: "and",
      filters: [
        { field: "Customer", operator: "eq", value: true },
      ]
    };

    if (this.search) {
      filter.filters.push({
        logic: "or",
        filters: [
          { field: "Name", operator: "contains", value: this.search },
          { field: "NameNoSign", operator: "contains", value: this.search },
          { field: "Ref", operator: "contains", value: this.search },
          { field: "Phone", operator: "contains", value: this.search }
        ]
      });
    }

    return filter;

  }

  toggleTagsPopOver(popover: any, dataItem: any) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      popover.open({ dataItem });
    }
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
    this.searchCategs = value;
    var tagIds = this.searchCategs.map(x => x.id);
    if (tagIds.length) {
      this.advanceFilter.params.tagIds = tagIds;
    } else {
      delete this.advanceFilter.params.tagIds;
    }

    this.dataBinding.skip = 0;
    this.refreshData();
  }

  searchCategories(search?: string) {
    var val = new PartnerCategoryPaged();
    val.search = search;
    return this.partnerCategoryService.autocomplete(val);
  }

  exportPartnerExcelFile() {
    var paged = new PartnerPaged();
    paged.customer = true;
    paged.search = this.search || "";

    var categs = this.searchCategs || [];
    paged.tagIds = categs.map(x => x.id);
    // paged.categoryId = this.searchCateg ? this.searchCateg.id : null;
    this.partnerService.exportPartnerExcelFile(paged).subscribe((rs) => {
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
    modalRef.componentInstance.id = item.Id;
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
}
