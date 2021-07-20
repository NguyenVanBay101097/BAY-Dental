import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbDate, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PurchaseOrderService, PurchaseOrderPaged, PurchaseOrderBasic } from '../purchase-order.service';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { IntlService } from '@progress/kendo-angular-intl';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { PartnerBasic, PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-purchase-order-list',
  templateUrl: './purchase-order-list.component.html',
  styleUrls: ['./purchase-order-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PurchaseOrderListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  type: string;

  dateFrom: Date;
  dateTo: Date;
  stateFilter: string;
  supplierFilter: string;
  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Nháp', value: 'draft' },
    { text: 'Đơn hàng', value: 'purchase' },
    { text: 'Hoàn thành', value: 'done' }
  ];
  supplierData: PartnerSimple[] = [];
  canAdd = true;
  canUpdate = true;
  canDelete = true;
  @ViewChild('supplierCbx', { static: true }) supplierCbx: ComboBoxComponent;
  constructor(private purchaseOrderService: PurchaseOrderService,
    private router: Router,
    private modalService: NgbModal, private route: ActivatedRoute, private intlService: IntlService,
    private checkPermissionService: CheckPermissionService,
    private partnerService: PartnerService,
    private notifyService: NotifyService
  ) { }

  ngOnInit() {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.route.queryParamMap.subscribe(params => {
      this.type = params.get('type');
      this.skip = 0;
      this.loadDataFromApi();
    });
    this.checkRole();
    this.loadSupplier();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.suppliertCbxFilterChange();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  onStateSelectChange(e) {
    var value = e ? e.value : null;
    if (value) {
      this.stateFilter = value;
    } else {
      
      this.stateFilter = null;
    }

    this.skip = 0;
    this.loadDataFromApi();
  }

  

  handleFilter(event) {
    this.supplierFilter = event ? event.id : null;
    this.skip = 0;
    this.loadDataFromApi();
  }

  suppliertCbxFilterChange() {
    this.supplierCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.supplierCbx.loading = true)),
      switchMap(value => this.searchSuppliers(value))
    ).subscribe(result => {
      this.supplierData = result;
      this.supplierCbx.loading = false;
    });
  }

  stateGet(state) {
    switch (state) {
      case 'purchase':
        return 'Đơn hàng';
      case 'done':
        return 'Hoàn thành';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Nháp';
    }
  }

  getTitle() {
    switch (this.type) {
      case 'refund':
        return 'trả hàng';
      default:
        return 'mua hàng';
    }
  }

  unlink() {
    if (this.selectedIds.length == 0) {
      return false;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu ' + this.getTitle();
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa phiếu '+this.getTitle() + '?';
    modalRef.result.then(() => {
      this.purchaseOrderService.unlink(this.selectedIds).subscribe(() => {
        this.selectedIds = [];
        this.loadDataFromApi();
      });
    });
  }

  loadDataFromApi() {
    debugger
    this.loading = true;
    var val = new PurchaseOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.type || '';
    val.partnerId = this.supplierFilter || '';
    val.state = this.stateFilter || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
  

    this.purchaseOrderService.getPaged(val).pipe(
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

  loadSupplier() {
    this.searchSuppliers().subscribe(result => {
      this.supplierData = result;
    })
  }

  searchSuppliers(q?: string) {
    var filter = new PartnerPaged();
    filter.search = q || '';
    filter.supplier = true;
    filter.active = true;
    filter.offset = 0;
    return this.partnerService.getAutocompleteSimple(filter);
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/purchase/orders/create'], { queryParams: { type: this.type } });
  }

  editItem(item: PurchaseOrderBasic) {
    this.router.navigate(['/purchase/orders/edit/', item.id]);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', size: 'sm' });
    modalRef.componentInstance.title = 'Xóa phiếu mua hàng';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa phiếu mua hàng?';
    modalRef.result.then(() => {
      this.purchaseOrderService.unlink([item.id]).subscribe(() => {
        this.notifyService.notify("success", "Xóa thành công")
        this.loadDataFromApi();
      }, error => {
      });
    });
  }

  exportExcelFile() {
    var val = new PurchaseOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.type || '';
    val.partnerId = this.supplierFilter || '';
    val.state = this.stateFilter || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
  
    this.purchaseOrderService.exportExcelFile(val).subscribe((res) => {
      let filename = this.type == 'order' ? 'Mua-hang' : 'Tra-hang';

      let newBlob = new Blob([res], {
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

  checkRole() {
    this.canAdd = this.checkPermissionService.check(['Purchase.Order.Create']);
    this.canUpdate = this.checkPermissionService.check(['Purchase.Order.Update']);
    this.canDelete = this.checkPermissionService.check(['Purchase.Order.Delete']);
  }
}



