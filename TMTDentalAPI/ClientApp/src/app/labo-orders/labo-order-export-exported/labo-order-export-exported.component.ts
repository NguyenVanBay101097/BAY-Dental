import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SessionInfoStorageService } from 'src/app/core/services/session-info-storage.service';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { ExportLaboPaged, LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-export-exported',
  templateUrl: './labo-order-export-exported.component.html',
  styleUrls: ['./labo-order-export-exported.component.css']
})
export class LaboOrderExportExportedComponent implements OnInit {
  
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  pagerSettings: any;
  search: string;
  searchUpdate = new Subject<string>();
  dateExportFrom: Date;
  dateExportTo: Date;
  dateReceiptFrom: Date;
  dateReceiptTo: Date;
  supplierData: any[]=[];
  partnerId: string = '';
  canUpdate: boolean = false;
  canUpdateSaleOrder: boolean = false;
  canReadLaboWarranty: boolean = false;
  constructor(
    private laboOrderService: LaboOrderService, 
    private intlService: IntlService, 
    private modalService: NgbModal,
    private checkPermissionService: CheckPermissionService,
    private partnerService: PartnerService,
    private sessionInfoStorageService: SessionInfoStorageService,
    private authService: AuthService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.checkRole();
    setTimeout(() => {
      this.loadDataFromApi();
      this.loadSupplier();
      this.searchUpdate.pipe(
        debounceTime(400),
        distinctUntilChanged())
        .subscribe(() => {
          this.skip = 0;
          this.loadDataFromApi();
        });
    },);
    
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ExportLaboPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = 'daxuat';
    val.partnerId = this.partnerId ? this.partnerId : '';
    if (this.dateExportFrom) {
      val.dateExportFrom = this.intlService.formatDate(this.dateExportFrom, 'd', 'en-US');
    }
    if (this.dateExportTo) {
      val.dateExportTo = this.intlService.formatDate(this.dateExportTo, 'd', 'en-US');
    }

    if (this.dateReceiptFrom) {
      val.dateReceiptFrom = this.intlService.formatDate(this.dateReceiptFrom, 'd', 'en-US');
    }
    if (this.dateReceiptTo) {
      val.dateReceiptTo = this.intlService.formatDate(this.dateReceiptTo, 'd', 'en-US');
    }

    this.laboOrderService.getExportLabo(val).pipe(
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

  loadSupplier(){
    this.searchSupplier().subscribe(result => {
      this.supplierData = _.unionBy(this.supplierData, result, 'id');
    });
  }

  searchSupplier(search?: string){
    var val = new PartnerPaged();
    val.offset = 0;
    val.limit = 1000;
    val.search = search || '';
    val.supplier = true;
    val.active = true;
    if (this.sessionInfoStorageService.getSessionInfo().settings && !this.sessionInfoStorageService.getSessionInfo().settings.companySharePartner) {
      val.companyId = this.authService.userInfo.companyId;
    }
    return this.partnerService.getAutocompleteSimple(val);
  }

  dateExportChange(data) {
    this.dateExportFrom = data.dateFrom;
    this.dateExportTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  dateReceiptChange(data){
    this.dateReceiptFrom = data.dateFrom;
    this.dateReceiptTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  supplierChange(event){
    this.partnerId = event;
    this.skip = 0;
    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  editItem(item){
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'C???p nh???t phi???u Labo';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.saleOrderLineId = item.saleOrderLineId;
   // modalRef.componentInstance.saleOrderLineLabo = this.item;

    modalRef.result.then(res => {
      this.loadDataFromApi();
      //this.reload.next(true);
    }, () => {
    });
  }

  checkRole(){
    this.canUpdate = this.checkPermissionService.check(['Labo.LaboOrder.Update']);
    this.canUpdateSaleOrder = this.checkPermissionService.check(['Basic.SaleOrder.Update']);
    this.canReadLaboWarranty = this.checkPermissionService.check(['Labo.LaboWarranty.Read']);
  }

  get showOnlyBeveragesDetails() {
    var isShow = this.checkPermissionService.check(['Labo.LaboWarranty.Read']);
    return isShow === true;
  }
}
