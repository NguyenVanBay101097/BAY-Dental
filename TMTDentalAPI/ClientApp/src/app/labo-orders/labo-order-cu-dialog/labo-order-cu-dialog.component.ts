import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { debounceTime, switchMap, tap, map } from 'rxjs/operators';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { UserService, UserPaged } from 'src/app/users/user.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { LaboOrderDisplay, LaboOrderService, LaboOrderDefaultGet } from '../labo-order.service';
import { LaboOrderCuLineDialogComponent } from '../labo-order-cu-line-dialog/labo-order-cu-line-dialog.component';
import { PartnerSupplierCuDialogComponent } from 'src/app/partners/partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { SaleOrderPaged, SaleOrderService } from 'src/app/sale-orders/sale-order.service';
declare var $: any;


@Component({
  selector: 'app-labo-order-cu-dialog',
  templateUrl: './labo-order-cu-dialog.component.html',
  styleUrls: ['./labo-order-cu-dialog.component.css']
})
export class LaboOrderCuDialogComponent implements OnInit {

  formGroup: FormGroup;
  id: string;
  dotKhamId: string;
  saleOrderId: string;
  filteredPartners: PartnerSimple[];
  filteredSaleOrders: SaleOrderBasic[];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('saleOrderCbx', { static: true }) saleOrderCbx: ComboBoxComponent;
  laboOrder: LaboOrderDisplay = new LaboOrderDisplay();
  laboOrderPrint: any;
  title: string;

  constructor(
    private fb: FormBuilder,
    private partnerService: PartnerService,
    private userService: UserService,
    private route: ActivatedRoute,
    private laboOrderService: LaboOrderService,
    private productService: ProductService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private router: Router,
    private notificationService: NotificationService,
    private saleOrderService: SaleOrderService,
    public activeModal: NgbActiveModal
  ) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      saleOrder: null,
      dateOrderObj: [null, Validators.required],
      datePlannedObj: null,
      orderLines: this.fb.array([]),
      dotKhamId: null
    });

    if (this.id) {
      this.loadData();
    } else {
      this.loadDefault();
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
    this.laboOrderService.get(this.id).subscribe(result => {
      this.laboOrder = result;
      this.formGroup.patchValue(result);

      let dateOrder = new Date(result.dateOrder);
      this.formGroup.get('dateOrderObj').patchValue(dateOrder);

      if (result.datePlanned) {
        let datePlanned = this.intlService.parseDate(result.datePlanned);
        this.formGroup.get('datePlannedObj').patchValue(datePlanned);
      }

      if (result.saleOrder) {
        this.filteredSaleOrders = _.unionBy(this.filteredSaleOrders, [result.saleOrder], 'id');
      }

      const control = this.formGroup.get('orderLines') as FormArray;
      control.clear();
      result.orderLines.forEach(line => {
        var g = this.fb.group(line);
        g.setControl('teeth', this.fb.array(line.teeth));
        control.push(g);
      });
    });

  }

  loadDefault() {
    var df = new LaboOrderDefaultGet();
    this.laboOrderService.defaultGet(df).subscribe(result => {
      this.laboOrder = result;
      this.formGroup.patchValue(result);

      let dateOrder = new Date(result.dateOrder);
      this.formGroup.get('dateOrderObj').patchValue(dateOrder);

      if (result.datePlanned) {
        let datePlanned = this.intlService.parseDate(result.datePlanned);
        this.formGroup.get('datePlannedObj').patchValue(datePlanned);
      }

      if (result.saleOrder) {
        this.filteredSaleOrders = _.unionBy(this.filteredSaleOrders, [result.saleOrder], 'id');
      }

      const control = this.formGroup.get('orderLines') as FormArray;
      control.clear();
      result.orderLines.forEach(line => {
        var g = this.fb.group(line);
        g.setControl('teeth', this.fb.array(line.teeth));
        control.push(g);
      });
    });
  }

  loadPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
    });
  }

  loadSaleOrders() {
    this.searchSaleOrders().subscribe(result => {
      this.filteredSaleOrders = _.unionBy(this.filteredSaleOrders, result.items, 'id');
    });
  }

  printLaboOrder() {
    if (this.id) {
      this.laboOrderService.getPrint(this.id).subscribe((result: any) => {
        this.laboOrderPrint = result;
        setTimeout(() => {
          var printContents = document.getElementById('printLaboOrderDiv').innerHTML;
          var popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
          popupWin.document.open();
          popupWin.document.write(`
              <html>
                <head>
                  <title>Print tab</title>
                  <link rel="stylesheet" type="text/css" href="/assets/css/bootstrap.min.css" />
                  <link rel="stylesheet" type="text/css" href="/assets/css/print.css" />
                </head>
            <body onload="window.print();window.close()">${printContents}</body>
              </html>`
          );
          popupWin.document.close();
          this.laboOrderPrint = null;
          // var html = document.getElementById('printLaboOrderDiv').innerHTML;
          // var hiddenFrame = $('<iframe style="visibility: hidden"></iframe>').appendTo('body')[0];
          // hiddenFrame.contentWindow.printAndRemove = function () {
          //   hiddenFrame.contentWindow.print();
          //   $(hiddenFrame).remove();
          // };
          // var htmlContent = "<!doctype html>" +
          //   "<html>" +
          //   "<head>" +
          //   '<link rel="stylesheet" type="text/css" href="/assets/css/bootstrap.min.css" />' +
          //   '<link rel="stylesheet" type="text/css" href="/assets/css/print.css" />' +
          //   "<head>" +
          //   '<body onload="printAndRemove();">' +
          //   html +
          //   '</body>' +
          //   "</html>";
          // var doc = hiddenFrame.contentWindow.document.open("text/html", "replace");
          // doc.write(htmlContent);
          // doc.close();
        }, 200);
      });
    }
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.supplier = true;
    val.search = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  searchSaleOrders(filter?: string) {
    var val = new SaleOrderPaged();
    val.search = filter || '';
    val.isQuotation = false;
    return this.saleOrderService.getPaged(val);
  }

  createNew() {
    this.router.navigate(['/labo-orders/form']);
  }



  get partner() {
    return this.formGroup.get('partner').value;
  }

  updateSupplierModal() {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: Labo';
    modalRef.componentInstance.id = this.partner.id;

    modalRef.result.then(() => {
    }, () => {
    });
  }

  quickCreateSupplier() {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Labo';

    modalRef.result.then(result => {
      var p = new PartnerSimple();
      p.id = result.id;
      p.name = result.name;
      p.displayName = result.displayName;
      this.formGroup.get('partner').patchValue(p);
      this.filteredPartners = _.unionBy(this.filteredPartners, [p], 'id');
    }, () => {
    });
  }

  onSaveConfirm() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.datePlanned = val.datePlannedObj ? this.intlService.formatDate(val.datePlannedObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.partnerId = val.partner.id;
    val.saleOrderId = val.saleOrder ? val.saleOrder.id : null;
    if (this.saleOrderId)
      val.saleOrderId = this.saleOrderId;
    this.laboOrderService.create(val).subscribe(result => {
      this.laboOrderService.buttonConfirm([result.id]).subscribe(() => {
        this.activeModal.close(result);
      });
    });
  }

  buttonConfirm() {
    if (this.id) {
      this.laboOrderService.buttonConfirm([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.datePlanned = val.datePlannedObj ? this.intlService.formatDate(val.datePlannedObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.partnerId = val.partner.id;
    val.saleOrderId = val.saleOrder ? val.saleOrder.id : null;

    if (this.saleOrderId)
      val.saleOrderId = this.saleOrderId;

    if (this.id) {
      this.laboOrderService.update(this.id, val).subscribe(() => {
        if (this.saleOrderId) {
          val.id = this.id;
          this.activeModal.close(val);
        }
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      });
    } else {

      this.laboOrderService.create(val).subscribe(result => {
        this.activeModal.close(result);
      });
    }
  }

  loadRecord() {
    if (this.id) {
      this.laboOrderService.get(this.id).subscribe(result => {
        this.laboOrder = result;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.dateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        if (result.datePlanned) {
          let datePlanned = this.intlService.parseDate(result.datePlanned);
          this.formGroup.get('datePlannedObj').patchValue(datePlanned);
        }

        if (result.partner) {
          this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
        }

        if (result.saleOrder) {
          this.filteredSaleOrders = _.unionBy(this.filteredSaleOrders, [result.saleOrder], 'id');
        }

        let control = this.formGroup.get('orderLines') as FormArray;
        control.clear();
        result.orderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('teeth', this.fb.array(line.teeth));
          control.push(g);
        });
      });
    }
  }

  buttonCancel() {
    if (this.id) {
      this.laboOrderService.buttonCancel([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  get orderLines() {
    return this.formGroup.get('orderLines') as FormArray;
  }

  showAddLineModal() {
    let modalRef = this.modalService.open(LaboOrderCuLineDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm chi tiết';

    modalRef.result.then(result => {
      let line = result as any;
      line.teeth = this.fb.array(line.teeth);
      this.orderLines.push(this.fb.group(line));
      this.computeAmountTotal();
    }, () => {
    });
  }

  editLine(line: FormGroup) {
    let modalRef = this.modalService.open(LaboOrderCuLineDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa chi tiết';
    modalRef.componentInstance.line = line.value;

    modalRef.result.then(result => {
      var a = result as any;
      line.patchValue(result);
      line.setControl('teeth', this.fb.array(a.teeth || []));
      this.computeAmountTotal();
    }, () => {
    });
  }

  lineTeeth(line: FormGroup) {
    var teeth = line.get('teeth').value as any[];
    return teeth.map(x => x.name).join(',');
  }

  deleteLine(index: number) {
    this.orderLines.removeAt(index);
    this.computeAmountTotal();
  }

  get getAmountTotal() {
    return this.formGroup.get('amountTotal').value;
  }

  computeAmountTotal() {
    let total = 0;
    this.orderLines.controls.forEach(line => {
      total += line.get('priceSubtotal').value;
    });
    this.laboOrder.amountTotal = total;
    // this.formGroup.get('amountTotal').patchValue(total);
  }
}
