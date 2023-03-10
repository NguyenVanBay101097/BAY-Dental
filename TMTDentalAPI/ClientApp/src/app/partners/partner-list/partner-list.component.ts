import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { WindowCloseResult, WindowRef, WindowService } from '@progress/kendo-angular-dialog';
import { CellClickEvent, GridComponent, GridDataResult, PageChangeEvent, SelectionEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { SortDescriptor } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/shared/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PartnerCreateUpdateComponent } from '../partner-create-update/partner-create-update.component';
import { PartnerImportComponent } from '../partner-import/partner-import.component';
import { PartnerDisplay, PartnerPaged } from '../partner-simple';
import { PartnerService } from '../partner.service';


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
  pagerSettings: any;

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
    private activeRoute: ActivatedRoute, private modalService: NgbModal,
    private notificationService: NotificationService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

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
        content: 'C???p nh???t th??nh c??ng',
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
      return 'Th??m m???i kh??ch h??ng';
    } else if (id && this.queryCustomer) {
      return 'C???p nh???t th??ng tin kh??ch h??ng';
    } else if (!id && this.querySupplier) {
      return 'Th??m m???i nh?? cung c???p';
    } else if (id && this.querySupplier) {
      return 'C???p nh???t th??ng tin nh?? cung c???p';
    }
  }

  deleteCustomer(id, event) {
    event.stopPropagation();
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a ' + (this.queryCustomer ? 'kh??ch h??ng' : 'nh?? cung c???p');

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
        return "N???";
      case "other":
        return "Kh??c";
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
    const modalRef = this.modalService.open(PartnerImportComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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
      let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thanh to??n';
      modalRef.componentInstance.defaultVal = result;
      modalRef.result.then(() => {
        this.notificationService.show({
          content: 'Thanh to??n th??nh c??ng',
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
