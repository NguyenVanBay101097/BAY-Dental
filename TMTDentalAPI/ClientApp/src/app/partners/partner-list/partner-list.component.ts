import { Component, OnInit } from '@angular/core';
import { WindowService, WindowRef, WindowCloseResult, DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PartnerService } from '../partner.service';
import { PartnerBasic, PartnerPaged, PartnerDisplay } from '../partner-simple';
import { PartnerCreateUpdateComponent } from '../partner-create-update/partner-create-update.component';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'app-partner-list',
  templateUrl: './partner-list.component.html',
  styleUrls: ['./partner-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerListComponent implements OnInit {

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
    private service: PartnerService, private windowService: WindowService,
    private activeRoute: ActivatedRoute, private dialogService: DialogService, private modalService: NgbModal) { }

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
    pnPaged.searchNamePhoneRef = this.searchNamePhoneRef || '';
    pnPaged.customer = this.queryCustomer;
    pnPaged.supplier = this.querySupplier;
    this.service.getPartnerPaged(pnPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.loading = false;
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
  //   this.service.getPartnerList(this.param).subscribe(
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

  deleteCustomer(id) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa khách hàng',
      content: 'Bạn chắc chắn muốn xóa khách hàng này ?',
      width: 450,
      height: 200,
      minWidth: 250,
      actions: [
        { text: 'Hủy', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ]
    });
    dialogRef.result.subscribe(
      rs => {
        if (!(rs instanceof DialogCloseResult)) {
          if (rs['value']) {
            this.service.deleteCustomer(id).subscribe(
              () => { this.getPartnersList(); }
            );
          }
        }
      }
    )
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

  getAge(y: number) {
    var today = new Date();
    return today.getFullYear() - y;
  }


}
