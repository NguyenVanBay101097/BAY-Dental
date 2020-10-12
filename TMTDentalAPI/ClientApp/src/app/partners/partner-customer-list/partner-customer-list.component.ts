import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { PartnerPaged, PartnerBasic } from '../partner-simple';
import { PartnerAddRemoveTags, PartnerService } from '../partner.service';
import { map, debounceTime, distinctUntilChanged, tap, switchMap, subscribeOn } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NgbModal, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnerImportComponent } from '../partner-import/partner-import.component';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';

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
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  rowPartnerId: any;
  search_partnerCategory: string;

  search: string;
  searchCateg: PartnerCategoryBasic;
  filteredCategs: PartnerCategoryBasic[];
  searchUpdate = new Subject<string>();
  searchUpdatePopOver = new Subject<string>();

  value_partnerCategoryPopOver : any;
  partnerCategoriesPopover: any;

  @ViewChild("categCbx", { static: true }) categCbx: ComboBoxComponent;
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;

  constructor(private partnerService: PartnerService, private modalService: NgbModal,
    private partnerCategoryService: PartnerCategoryService) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.searchUpdatePopOver.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadPartnerCategoryPopOver();
      });

    this.categCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.categCbx.loading = true)),
        switchMap((value) => this.searchCategories(value))
      )
      .subscribe((result) => {
        this.filteredCategs = result;
        this.categCbx.loading = false;
      });

    this.loadDataFromApi();
    this.loadFilteredCategs();
    this.loadPartnerCategoryPopOver();

  }

  loadPartnerCategoryPopOver() {
    const val = new PartnerCategoryPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = this.search_partnerCategory || '';

    this.partnerCategoryService.getPaged(val).subscribe(res => {
      this.partnerCategoriesPopover = res.items;
      // if (this.popover && this.popover.isOpen()) {
      //   console.log(this.popover);
      //   this.popover.open({ partnerCategoriesPopOver: this.partnerCategoriesPopover });
      // } else if (this.popover) {
      //   this.popover.open({ partnerCategoriesPopOver: this.partnerCategoriesPopover });
      // }
    }, err => {
      console.log(err);
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  importFromExcel() {
    const modalRef = this.modalService.open(PartnerImportComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.type = 'customer';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }


  loadDataFromApi() {
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.customer = true;
    val.search = this.search || '';
    val.categoryId = this.searchCateg ? this.searchCateg.id : "";

    this.loading = true;
    this.partnerService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(
      (result) => (this.filteredCategs = result)
    );
  }

  onCategChange(value) {
    this.searchCateg = value;
    this.loadDataFromApi();
  }

  searchCategories(search?: string) {
    var val = new PartnerCategoryPaged();
    val.search = search;
    return this.partnerCategoryService.autocomplete(val);
  }

  onPaymentChange() {
    this.loadDataFromApi();
  }

  exportPartnerExcelFile() {
    var paged = new PartnerPaged();
    paged.customer = true;
    paged.search = this.search || "";
    paged.categoryId = this.searchCateg ? this.searchCateg.id : null;
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
      this.loadDataFromApi();
    }, er => { })
  }

  editItem(item: PartnerBasic) {
    const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa khách hàng';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    })
  }

  deleteItem(item: PartnerBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa khách hàng';

    modalRef.result.then(() => {
      this.partnerService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

  onToggleCategory(popOver, id) {
    if (popOver.isOpen()) {
      popOver.close();
    } else {
      this.rowPartnerId = id;
      const val = new PartnerCategoryPaged();
      val.limit = 20;
      val.offset = 0;
      val.partnerId = id;
      this.partnerCategoryService.getPaged(val).subscribe((res) => {
          this.value_partnerCategoryPopOver = res.items;
      });
      popOver.open({ partnerCategoriesPopOver: this.partnerCategoriesPopover });
      this.popover = popOver;
    }
  }

  handleFilterCategoryPopOver(value) {
    this.search_partnerCategory = value;
  }

  SavePartnerCategories() {
    console.log(this.value_partnerCategoryPopOver);
    const val = new PartnerAddRemoveTags();
    val.id = this.rowPartnerId;
    val.tagIds = [];
    this.value_partnerCategoryPopOver.forEach(element => {
      val.tagIds.push(element.id);
    });
    this.partnerService.updateTags(val).subscribe(() => {
      this.loadDataFromApi();
      this.value_partnerCategoryPopOver = [];
    });
  }

  ClosePopover() {
    this.popover.close();
  }
}
