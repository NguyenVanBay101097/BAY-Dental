import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { StockPickingMlDialogComponent } from '../stock-picking-ml-dialog/stock-picking-ml-dialog.component';
import { StockMoveDisplay, StockPickingService, StockPickingDefaultGet, StockPickingDisplay } from '../stock-picking.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { StockPickingTypeService, StockPickingTypeBasic } from 'src/app/stock-picking-types/stock-picking-type.service';
import { StockMoveService, StockMoveOnChangeProduct } from 'src/app/stock-moves/stock-move.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged, ProductBasic2, ProductService } from 'src/app/products/product.service';
import { TaiProductListSelectableComponent } from 'src/app/shared/tai-product-list-selectable/tai-product-list-selectable.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SelectUomProductDialogComponent } from 'src/app/shared/select-uom-product-dialog/select-uom-product-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';

declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-stock-picking-outgoing-create-update',
  templateUrl: './stock-picking-outgoing-create-update.component.html',
  styleUrls: ['./stock-picking-outgoing-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class StockPickingOutgoingCreateUpdateComponent implements OnInit {
  pickingForm: FormGroup;
  opened = false;
  picking: StockPickingDisplay = new StockPickingDisplay();
  id: string;
  filteredPartners: PartnerSimple[] = [];
  productSearch: string;
  productList: ProductBasic2[] = [];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild(TaiProductListSelectableComponent, { static: false }) productListSelectable: TaiProductListSelectableComponent;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private intlService: IntlService,
    private stockPickingService: StockPickingService,
    private router: Router,
    private partnerService: PartnerService,
    private notificationService: NotificationService,
    private pickingTypeService: StockPickingTypeService,
    private stockMoveService: StockMoveService,
    private productService: ProductService,
    private modalService: NgbModal,
    private printServie: PrintService
  ) { }

  ngOnInit() {
    this.pickingForm = this.fb.group({
      partner: null,
      dateObj: new Date(),
      note: null,
      moveLines: this.fb.array([]),
      companyId: null,
      locationId: null,
      locationDestId: null,
      pickingTypeId: null,
      state: null,
      name: null
    });

    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.loadRecord();
    } else {
      this.loadDefault();
    }

    this.loadProductList();
    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });

    this.loadFilteredPartners();
  }

  get partner() {return this.pickingForm.get('partner').value;}
  get note() {return this.pickingForm.get('note').value;}

  loadProductList() {
    var val = new ProductPaged();
    val.limit = 10;
    val.offset = 0;
    val.type = 'product,consu';
    val.search = this.productSearch || '';
    this.productService.getPaged(val).subscribe(res => {
      this.productList = res.items;
      this.productListSelectable.resetIndex();
    }, err => {
    });
  }

  upDownEnterChange(code) {
    if (code == 40) {
      this.productListSelectable.moveUp();
    } else if (code == 38) {
      this.productListSelectable.moveDown();
    } else if (code == 13) {
      this.productListSelectable.selectCurrent();
    }
  }

  onProductInputSearchChange(text) {
    this.productSearch = text;
    this.loadProductList();
  }

  loadFilteredPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = result;
    });
  }

  searchPartners(search?: string) {
    var val = new PartnerPaged();
    val.search = search;
    // val.customer = true;
    return this.partnerService.getAutocompleteSimple(val);
  }

  loadDefault() {
    this.stockPickingService.defaultGetOutgoing().subscribe(result => {
      this.pickingForm.patchValue(result);
      this.picking = result;
    });
  }

  loadRecord() {
    this.stockPickingService.get(this.id).subscribe(result => {
      this.picking = result;
      this.pickingForm.patchValue(result);
      this.moveLines.clear();
      result.moveLines.forEach(line => {
        this.moveLines.push(this.fb.group(line));
      });
    });
  }

  changeUoM(line: AbstractControl) {
    var product = line.get('product').value;
    let modalRef = this.modalService.open(SelectUomProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
    modalRef.componentInstance.title = 'Chọn đơn vị';
    modalRef.componentInstance.productId = product.id;
    modalRef.result.then((res: any) => {
      line.get('productUOM').setValue(res);
      line.get('productUOMId').setValue(res.id);
    }, () => {
    });
  }

  onChangeProduct(value: ProductBasic2) {
    var product = value;
    var index = _.findIndex(this.moveLines.controls, o => {
      return o.get('product').value.id == product.id && o.get('productUOMId').value == product.uomId;
    });

    if (index !== -1) {
      var control = this.moveLines.controls[index];
      control.patchValue({ productUOMQty: control.get('productUOMQty').value + 1 });
    } else {
      var val = new StockMoveOnChangeProduct();
      val.productId = product.id;

      var productSimple = new ProductSimple();
      productSimple.id = product.id;
      productSimple.name = product.name;

      this.stockMoveService.onChangeProduct(val).subscribe((result: any) => {
        var group = this.fb.group({
          name: result.name,
          productUOMId: result.productUOM.id,
          productUOM: result.productUOM,
          product: productSimple,
          productId: product.id,
          productUOMQty: 1
        });

        this.moveLines.push(group);
        this.focusLastRow();
      });
    }
  }

  focusLastRow() {
    setTimeout(() => {
      var $lastTr = $('tr:last', $('#table_details tbody'));
      $('input:first', $lastTr).focus();
    }, 70);
  }

  deleteLine(index: number) {
    this.moveLines.removeAt(index);
  }

  get pickingState() {
    return this.pickingForm.get('state').value;
  }

  get pickingName() {
    return this.pickingForm.get('name').value;
  }

  get moveLines() {
    return this.pickingForm.get('moveLines') as FormArray;
  }

  checkQtyPriceValid() {
    var index = _.findIndex(this.moveLines.controls, o => {
      return o.get('productUOMQty').value == null;
    });
    return index !== -1 ? false : true;
  }

  onSaveOrUpdate() {
    if (!this.checkQtyPriceValid()) {
      alert('Vui lòng nhập số lượng');
      return;
    }

    if (!this.pickingForm.valid) {
      return;
    }

    var val = this.pickingForm.value;
    val.partnerId = val.partner ? val.partner.id : null;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    if (this.id) {
      this.stockPickingService.update(this.id, val).subscribe(() => {
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
      this.stockPickingService.create(val).subscribe(result => {
        this.router.navigate(['stock/outgoing-pickings/edit/', result.id]);
      });
    }
  }

  actionDone() {
    if (this.id) {
      if (this.pickingForm.dirty) {
        //update and done

        if (!this.checkQtyPriceValid()) {
          alert('Vui lòng nhập số lượng');
          return;
        }

        if (!this.pickingForm.valid) {
          return;
        }

        var val = this.pickingForm.value;
        val.partnerId = val.partner ? val.partner.id : null;
        val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');

        this.stockPickingService.update(this.id, val).subscribe(() => {
          this.stockPickingService.actionDone([this.id]).subscribe(() => {
            this.loadRecord();
          });
        });
      } else {
        //only need done
        this.stockPickingService.actionDone([this.id]).subscribe(() => {
          this.loadRecord();
        });
      }
    } else {
      //save and done

      if (!this.checkQtyPriceValid()) {
        alert('Vui lòng nhập số lượng');
        return;
      }

      if (!this.pickingForm.valid) {
        return;
      }

      var val = this.pickingForm.value;
      val.partnerId = val.partner ? val.partner.id : null;
      val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');

      this.stockPickingService.create(val).subscribe(result => {
        this.stockPickingService.actionDone([result.id]).subscribe(() => {
          this.router.navigate(['stock/outgoing-pickings/edit/', result.id]);
        }, () => {
          this.router.navigate(['stock/outgoing-pickings/edit/', result.id]);
        });
      });
    }
  }

  onPrint() {
    this.stockPickingService.Print(this.id).subscribe((res:any) => {
      this.printServie.printHtml(res.html);
    });
  }
}

