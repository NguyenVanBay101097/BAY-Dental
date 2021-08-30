import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { LaboOrderExportDialogComponent } from '../labo-order-export-dialog/labo-order-export-dialog.component';
import { ExportLaboPaged, LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-export-not-export',
  templateUrl: './labo-order-export-not-export.component.html',
  styleUrls: ['./labo-order-export-not-export.component.css']
})
export class LaboOrderExportNotExportComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateExportFrom: Date;
  dateExportTo: Date;
  supplierData: any[]=[];
  partnerId: string = '';
  canUpdate: boolean = false;
  constructor(
    private laboOrderService: LaboOrderService, 
    private intlService: IntlService, 
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private checkPermissionService: CheckPermissionService,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
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
    this.canUpdate = this.checkPermissionService.check(['Labo.LaboOrder.Update']);
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ExportLaboPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = 'chuaxuat';
    val.partnerId = this.partnerId ? this.partnerId : '';
    if (this.dateExportFrom) {
      val.dateExportFrom = this.intlService.formatDate(this.dateExportFrom, 'd', 'en-US');
    }
    if (this.dateExportTo) {
      val.dateExportTo = this.intlService.formatDate(this.dateExportTo, 'd', 'en-US');
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
    this.limit = 1000;
    val.search = search || '';
    val.supplier = true;
    val.active = true;
    return this.partnerService.getAutocompleteSimple(val);
  }

  dateExportChange(data) {
    this.dateExportFrom = data.dateFrom;
    this.dateExportTo = data.dateTo;
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
    this.loadDataFromApi();
  }

  exportPartner(item){
    const modalRef = this.modalService.open(LaboOrderExportDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.labo = item;

    modalRef.result.then(
      result => {        
        if (result) {
          this.skip = 0;
          this.loadDataFromApi();
        }
      },
      er => { }
    )
  }

  editItem(item) {    
    console.log(item);
    
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật phiếu labo';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.saleOrderLineId = item.saleOrderLineId;
   // modalRef.componentInstance.saleOrderLineLabo = this.item;

    modalRef.result.then(res => {
      this.loadDataFromApi();
      //this.reload.next(true);
    }, () => {
    });
  }
}
