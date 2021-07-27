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
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { LaboOrderDisplay, LaboOrderService, LaboOrderDefaultGet } from 'src/app/labo-orders/labo-order.service';
import { LaboOrderCuLineDialogComponent } from '../labo-order-cu-line-dialog/labo-order-cu-line-dialog.component';
import { PartnerSupplierCuDialogComponent } from 'src/app/shared/partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';
import { ProductSimple } from 'src/app/products/product-simple';
import { ToothBasic, ToothDisplay } from 'src/app/teeth/tooth.service';
declare var $: any;


@Component({
  selector: 'app-labo-order-cu-dialog',
  templateUrl: './labo-order-cu-dialog.component.html',
  styleUrls: ['./labo-order-cu-dialog.component.css']
})
export class  LaboOrderCuDialogComponent implements OnInit {

  saleOrderLineId: string;

  formGroup: FormGroup;
  id: string;
  dotKhamId: string;
  saleOrderId: string;
  filteredPartners: PartnerSimple[];
  filteredSaleOrders: SaleOrderBasic[];
  filteredLabos: any[];
  @ViewChild('laboCbx', { static: true }) laboCbx: ComboBoxComponent;
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('saleOrderCbx', { static: true }) saleOrderCbx: ComboBoxComponent;
  laboOrder: LaboOrderDisplay = new LaboOrderDisplay();
  laboOrderPrint: any;
  title: string;
  isChange = false;
  teethSelected = [];

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
      dateOrderObj: [null, Validators.required],
      datePlannedObj: null,
      product: [null, Validators.required],
      color: null,
      note: null,
      quantity: [0, Validators.required],
      priceUnit: [0, Validators.required],
      saleOrderLineId: [this.saleOrderLineId, Validators.required],
      warrantyCode: null,
      warrantyPeriodObj: null,
    });

    setTimeout(() => {
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

      this.laboCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.laboCbx.loading = true)),
        switchMap(value => this.searchLabos(value))
      ).subscribe(result => {
        this.filteredLabos = result;
        this.laboCbx.loading = false;
      });
      this.loadLabos();
    });
  }

  get partner() {
    return this.formGroup.get('partner').value;
  }

  get labo() {
    return this.formGroup.get('product').value;
  }

  get saleOrderLine() {return this.laboOrder.saleOrderLine;}

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

      if (result.warrantyPeriod) {
        let warrantyPeriod = this.intlService.parseDate(result.warrantyPeriod);
        this.formGroup.get('warrantyPeriodObj').patchValue(warrantyPeriod);
      }

      if (result.product) {
        this.filteredLabos = _.unionBy(this.filteredLabos, [result.product], 'id');
      }
    });

  }

  loadDefault() {
    var df = new LaboOrderDefaultGet();
    df.saleOrderLineId = this.saleOrderLineId;
    this.laboOrderService.defaultGet(df).subscribe(result => {
      this.laboOrder = result;
      this.formGroup.patchValue(result);

      let dateOrder = new Date(result.dateOrder);
      this.formGroup.get('dateOrderObj').patchValue(dateOrder);

      if (result.datePlanned) {
        let datePlanned = this.intlService.parseDate(result.datePlanned);
        this.formGroup.get('datePlannedObj').patchValue(datePlanned);
      }

      if (result.warrantyPeriod) {
        let warrantyPeriod = this.intlService.parseDate(result.warrantyPeriod);
        this.formGroup.get('warrantyPeriodObj').patchValue(warrantyPeriod);
      }
    });
  }

  loadLabos() {
    this.searchLabos().subscribe(result => {
      this.filteredLabos = _.unionBy(this.filteredLabos, result, 'id');
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

  searchLabos(filter?: string) {
    var val = new ProductFilter();
    val.type2 = 'labo';
    val.search = filter;
    return this.productService.autocomplete2(val);
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

  updateSupplierModal() {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: Nhà cung cấp';
    modalRef.componentInstance.id = this.partner.id;

    modalRef.result.then(() => {
    }, () => {
    });
  }

  quickCreateSupplier() {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Nhà cung cấp';

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
    val.warrantyPeriod = this.intlService.formatDate(val.warrantyPeriodObj, 'yyyy-MM-ddTHH:mm:ss');
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
        this.isChange = true;
        this.loadRecord();
      });
    }
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.datePlanned = val.datePlannedObj ? this.intlService.formatDate(val.datePlannedObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.warrantyPeriod = val.warrantyPeriodObj ? this.intlService.formatDate(val.warrantyPeriodObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.partnerId = val.partner.id;
    val.teeth = this.teethSelected;
    val.productId = val.product.id;
   
    if (this.id) {
      this.laboOrderService.update(this.id, val).subscribe(() => {
        this.notify('success', 'Lưu thành công');
        this.activeModal.close(true);
      });
    } else {
      this.laboOrderService.create(val).subscribe(result => {
        this.notify('success', 'Tạo thành công');
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

      });
    }
  }

  buttonCancel() {
    if (this.id) {
      this.laboOrderService.buttonCancel([this.id]).subscribe(() => {
        this.isChange = true;
        this.loadRecord();
      });
    }
  }

  get getPriceUnit() { return this.formGroup.get('priceUnit').value; }
  get getQuantity() { return this.formGroup.get('quantity').value; }

  getPriceSubTotal() {
    return this.getPriceUnit * this.getQuantity;
  }

  showAddLineModal() {
    let modalRef = this.modalService.open(LaboOrderCuLineDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm chi tiết';
    modalRef.componentInstance.saleOrderLineId = this.saleOrderLineId;

    modalRef.result.then(result => {
      let line = result as any;
      line.teeth = this.fb.array(line.teeth);
      line.teethListVirtual = this.fb.array(line.teethListVirtual);
      this.computeAmountTotal();
    }, () => {
    });
  }

  editLine(line: FormGroup) {
    let modalRef = this.modalService.open(LaboOrderCuLineDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa chi tiết';
    modalRef.componentInstance.line = line.value;
    modalRef.componentInstance.saleOrderLineId = this.saleOrderLineId;

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
    this.computeAmountTotal();
  }

  get getAmountTotal() {
    return this.formGroup.get('amountTotal').value;
  }

  computeAmountTotal() {
    let total = 0;
    this.laboOrder.amountTotal = total;
    // this.formGroup.get('amountTotal').patchValue(total);
  }

  onCancel() {
    if (this.isChange) {
      this.activeModal.close(true);
    } else {
      this.activeModal.dismiss();
    }
  }
  
  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
  }
  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.teethSelected.indexOf(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
    }

    //update quantity combobox
    if (this.teethSelected.length > 0) {
      this.formGroup.get("quantity").setValue(this.teethSelected.length);
    }
  }

  onLaboChange(e) {
    this.formGroup.get('priceUnit').setValue(e.priceUnit);
  }
}
