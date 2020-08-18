import { Component, OnInit, ViewChild } from '@angular/core';
import { WindowService, WindowRef, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PartnerService, ImportExcelDirect } from '../partner.service';
import { PartnerBasic, PartnerPaged, PartnerDisplay } from '../partner-simple';
import { PartnerCreateUpdateComponent } from '../partner-create-update/partner-create-update.component';
import { GridDataResult, PageChangeEvent, GridComponent, SelectionEvent, CellClickEvent } from '@progress/kendo-angular-grid';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject, Observable } from 'rxjs';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { PartnerImportComponent } from '../partner-import/partner-import.component';
import { saveAs } from '@progress/kendo-drawing/pdf';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/shared/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';


@Component({
  selector: 'app-partner-list',
  templateUrl: './partner-list.component.html',
  styleUrls: ['./partner-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerListComponent implements OnInit {

  @ViewChild('grid', { static: true }) grid: GridComponent;
  expandKeys: string[] = [];
  // gridData: PartnerBasic[];
  loading = false;
  gridView: GridDataResult;
  windowOpened: boolean;
  skip = 0;
  pageSize = 20;

  queryCustomer: boolean = false;
  querySupplier: boolean = false;

  searchNamePhoneRef: string;
  searchNamePhoneRefUpdate = new Subject<string>();


  sort: SortDescriptor[] = [{
    field: 'name',
    dir: 'asc'
  }];

  // formFilter = new FormGroup({
  //   searchNamePhoneRef: new FormControl(),
  //   customer: new FormControl(),
  //   employee: new FormControl()
  // })
  closeResult: string;

  constructor(
    private partnerService: PartnerService, private windowService: WindowService,
    private activeRoute: ActivatedRoute, private dialogService: DialogService, private modalService: NgbModal,
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.windowOpened = false;
    this.routingChange();
    this.searchChange();
  }


  routingChange() {
    this.activeRoute.queryParamMap.subscribe(
      params => {
        if (params['params']['customer'] == 'true') {
          this.queryCustomer = true;
          this.querySupplier = false;
        }
        if (params['params']['supplier'] == 'true') {
          this.querySupplier = true;
          this.queryCustomer = false;
        }
        this.getPartnersList();
      },
      er => {
        console.log(er);
      }
    );
  }


  getPartnersList() {
    this.loading = true;
    var pnPaged = new PartnerPaged();
    pnPaged.limit = this.pageSize;
    pnPaged.offset = this.skip;
    pnPaged.search = this.searchNamePhoneRef || '';
    pnPaged.customer = this.queryCustomer;
    pnPaged.supplier = this.querySupplier;
    this.partnerService.getPartnerPaged(pnPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.loading = false;

      console.log(rs2);
    }, er => {
      this.loading = true;
      console.log(er);
    }
    )
  }

  searchChange() {
    this.searchNamePhoneRefUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.getPartnersList();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
    this.getPartnersList();
  }

  sortChange(sort: SortDescriptor[]): void {
    this.sort = sort;
    this.getPartnersList();
  }

  updateCustomersZaloId() {
    this.partnerService.updateCustomersZaloId().subscribe(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    });
  }

  // getQuery() {
  //   this.param = new HttpParams()
  //     .set('employee', this.formFilter.get('employee').value == true ? 'true' : 'false')
  //     .set('customer', this.formFilter.get('customer').value == true ? 'true' : 'false')
  //     .set('searchNameRef', this.formFilter.get('searchNameRef').value)
  //     .set('searchPhone', this.formFilter.get('searchPhone').value);

  //   if (this.formFilter.get('searchPhone').value == null) {
  //     this.param = this.param.delete('searchPhone');
  //   };
  //   if (this.formFilter.get('searchNameRef').value == null) {
  //     this.param = this.param.delete('searchNameRef');
  //   };
  //   this.partnerService.getPartnerList(this.param).subscribe(
  //     rs => {
  //       this.gridView = {
  //         data: orderBy((rs['items'] as PartnerDisplay[]), this.sort).slice(this.skip, this.skip + this.pageSize),
  //         total: rs['items'].length
  //       };
  //     }
  //   )
  // }

  openWindow(id) {
    const windowRef: WindowRef = this.windowService.open(
      {
        title: this.windowTitle(id),
        content: PartnerCreateUpdateComponent,
        minWidth: 250,
        resizable: false
        // width: 920
      });
    this.windowOpened = true;
    const instance = windowRef.content.instance;
    instance.cusId = id;

    windowRef.result.subscribe(
      (result) => {
        this.windowOpened = false;
        console.log(result);
        if (!(result instanceof WindowCloseResult)) {
          this.getPartnersList();
        }
      }
    )
  }

  openModal(id) {
    const modalRef = this.modalService.open(PartnerCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.cusId = id;
    modalRef.result.then(
      rs => {
        this.getPartnersList();
      },
      er => { }
    )
    // const windowRef: WindowRef = this.windowService.open(
    //   {
    //     title: this.windowTitle(id),
    //     content: PartnerCreateUpdateComponent,
    //     minWidth: 250,
    //     // width: 920
    //   });
    // this.windowOpened = true;
    // const instance = windowRef.content.instance;
    // instance.cusId = id;

    // windowRef.result.subscribe(
    //   (result) => {
    //     this.windowOpened = false;
    //     console.log(result);
    //     if (!(result instanceof WindowCloseResult)) {
    //       this.getPartnersList();
    //     }
    //   }
    // )
  }

  windowTitle(id) {
    if (!id && this.queryCustomer) {
      return 'Thêm mới khách hàng';
    } else if (id && this.queryCustomer) {
      return 'Cập nhật thông tin khách hàng';
    } else if (!id && this.querySupplier) {
      return 'Thêm mới nhà cung cấp';
    } else if (id && this.querySupplier) {
      return 'Cập nhật thông tin nhà cung cấp';
    }
  }

  deleteCustomer(id, event) {
    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + (this.queryCustomer ? 'khách hàng' : 'nhà cung cấp');

    modalRef.result.then(() => {
      this.partnerService.deleteCustomer(id).subscribe(() => {
        this.routingChange();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }

  getGender(g: string) {
    switch (g.toLowerCase()) {
      case "male":
        return "Nam";
      case "female":
        return "Nữ";
      case "other":
        return "Khác";
    }
  }

  onUpdateChange() {
    this.getPartnersList();
  }

  getAge(y: number) {
    var today = new Date();
    return today.getFullYear() - y;
  }

  rowSelectionChange(e: SelectionEvent) {
    console.log(e.selectedRows[0]);
    console.log(e);
    var selected = e.selectedRows[0];
    if (e.selectedRows.indexOf(selected) == -1) {
      console.log(1);
      this.grid.collapseRow(selected.index);
    } else if (e.selectedRows.indexOf(selected) > -1) {
      console.log(2);
      this.grid.expandRow(selected.index);
    }
    // this.getInvoiceDetail(e.selectedRows[0].dataItem.id);
  }

  cellClick(e: CellClickEvent) {
    console.log(e);
    this.grid.expandRow(e.rowIndex);
    var index = this.expandKeys.indexOf(e.dataItem.id);
    if (index == -1) {
      this.expandKeys.push(e.dataItem.id)
      this.grid.expandRow(e.rowIndex);
    } else if (index > -1) {
      this.expandKeys.splice(index, 1);
      this.grid.collapseRow(e.rowIndex);
    }
    // if(this.grid.row)
    // this.grid.selectionChange.toPromise().then(
    //   rs => {
    //     var item = { dataItem: e.dataItem, index: e.rowIndex };
    //     console.log(rs.selectedRows.indexOf(item));
    //     if (rs.selectedRows.indexOf(item) == -1) {
    //       rs.selectedRows.push(item);
    //       this.grid.expandRow(e.rowIndex);
    //     } else {
    //       rs.selectedRows.splice(0, 1);
    //       this.grid.collapseRow(e.rowIndex);
    //     }

    //   }
    // )
  }

  importFromExcel(isCreateNew: boolean) {
    const modalRef = this.modalService.open(PartnerImportComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.isCreateNew = isCreateNew;
    modalRef.result.then(
      rs => {
        this.getPartnersList();
      },
      er => { }
    )
  }

  //=================== SERVER Excel export ============================
  exportExcelFile() {
    var paged = new PartnerPaged();
    paged.search = this.searchNamePhoneRef || '';
    paged.customer = this.queryCustomer;
    paged.supplier = this.querySupplier;
    this.partnerService.excelServerExport(paged).subscribe(
      rs => {
        let filename = 'ExportedExcelFile';
        let newBlob = new Blob([rs], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        console.log(rs);

        let data = window.URL.createObjectURL(newBlob);
        let link = document.createElement('a');
        link.href = data;
        link.download = filename;
        link.click();
        setTimeout(() => {
          // For Firefox it is necessary to delay revoking the ObjectURL
          window.URL.revokeObjectURL(data);
        }, 100);
      }
    );
  }

  registerPayment(id: string) {
    this.partnerService.getDefaultRegisterPayment(id).subscribe(result => {
      let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thanh toán';
      modalRef.componentInstance.defaultVal = result;
      modalRef.result.then(() => {
        this.notificationService.show({
          content: 'Thanh toán thành công',
          hideAfter: 3000,
          position: { horizontal: 'right', vertical: 'bottom' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, () => {
      });
    });
  }

  getAddress(rs: PartnerDisplay) {
    var addArray = new Array<string>();
    if (rs.street) {
      addArray.push(rs.street);
    }
    if (rs.wardName) {
      addArray.push(rs.wardName);
    }
    if (rs.districtName) {
      addArray.push(rs.districtName);
    }
    if (rs.cityName) {
      addArray.push(rs.cityName);
    }
    return addArray.join(', ');
  }

  getHistories(partner: PartnerDisplay) {
    if (partner.histories || partner.medicalHistory) {
      var arr = new Array<string>();

      if (partner.medicalHistory) {
        arr.push(partner.medicalHistory);
      }

      partner.histories.forEach(e => {
        arr.push(e.name);
      });
      return arr.join(', ');
    }
  }

  getBirth(partner: PartnerDisplay) {
    var day = partner.birthDay ? partner.birthDay : '';
    var month = partner.birthMonth ? partner.birthMonth : '';
    var year = partner.birthYear ? partner.birthYear : '';

    return day + '/' + month + '/' + year;
  }
}
