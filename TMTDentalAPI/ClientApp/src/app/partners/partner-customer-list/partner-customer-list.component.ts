import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { PartnerPaged, PartnerBasic } from '../partner-simple';
import { PartnerService } from '../partner.service';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { WindowService, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { PartnerCustomerCuDialogComponent } from '../partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnerImportComponent } from '../partner-import/partner-import.component';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerPrintComponent } from 'src/app/shared/partner-print/partner-print.component';

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

  search: string;
  searchCateg: PartnerCategoryBasic;
  filteredCategs: PartnerCategoryBasic[];
  searchUpdate = new Subject<string>();
  @ViewChild("categCbx", { static: true }) categCbx: ComboBoxComponent;
  @ViewChild(PartnerPrintComponent, {static: true}) partnerPrintComponent: PartnerPrintComponent;

  constructor(private partnerService: PartnerService, private modalService: NgbModal, 
    private partnerCategoryService: PartnerCategoryService) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
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
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  importFromExcel() {
    const modalRef = this.modalService.open(PartnerImportComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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

  printItem(item: PartnerBasic) {
    this.partnerService.getPrint(item.id).subscribe(result => {
      this.partnerPrintComponent.print(result);
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
}
