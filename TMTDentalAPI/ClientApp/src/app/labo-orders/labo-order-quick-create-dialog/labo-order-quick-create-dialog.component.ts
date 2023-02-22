import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import * as _ from 'lodash';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { LaboOrderLineDisplay } from 'src/app/labo-order-lines/labo-order-line.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-quick-create-dialog',
  templateUrl: './labo-order-quick-create-dialog.component.html',
  styleUrls: ['./labo-order-quick-create-dialog.component.css']
})
export class LaboOrderQuickCreateDialogComponent implements OnInit {

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;

  filteredPartners: PartnerSimple[];
  formGroup: FormGroup;
  listSaleOrderLine: SaleOrderLineDisplay[] = [];
  listLaboOrderLine: LaboOrderLineDisplay[] = [];
  saleOrderId: string;
  title: string;
  
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private laboOrderService: LaboOrderService,
    private partnerService: PartnerService,
    private saleOrderServive: SaleOrderService,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      dateOrderObj: [null, Validators.required],
      datePlannedObj: null,
    });
    if (this.saleOrderId) {
      this.loadData();
    }

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });
    this.loadPartners();
  }

  loadData() {
    let dateOrder = new Date();
    this.formGroup.get('dateOrderObj').patchValue(dateOrder);
    this.saleOrderServive.getServiceBySaleOrderId(this.saleOrderId).subscribe(
      result => {
        this.listSaleOrderLine = result;
        console.log(this.listSaleOrderLine);
      }
    )
  }

  chooseLaboOrderLine(line) {
    var index = this.listLaboOrderLine.findIndex(x => x.id == line.id);
    if (index < 0) {
      this.listLaboOrderLine.push(line);
    } else {
      this.listLaboOrderLine.splice(index, 1);
    }
  }

  loadPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.supplier = true;
    val.search = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  onSave() {
    if (this.formGroup.invalid)
      return false

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.datePlanned = val.datePlannedObj ? this.intlService.formatDate(val.datePlannedObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.partnerId = val.partner.id;
    val.saleOrderId = this.saleOrderId;
    val.orderLines = this.listLaboOrderLine;
    this.laboOrderService.create(val).subscribe(
      result => {
        this.activeModal.close();
        console.log(result);
      }
    )
  }

}
