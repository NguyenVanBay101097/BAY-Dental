import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
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
import { SharedDemoDataDialogComponent } from 'src/app/shared/shared-demo-data-dialog/shared-demo-data-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SelectUomProductDialogComponent } from 'src/app/shared/select-uom-product-dialog/select-uom-product-dialog.component';
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

  productSearch: string;
  productList: ProductBasic2[] = [];
  @ViewChild(TaiProductListSelectableComponent, { static: false }) productListSelectable: TaiProductListSelectableComponent;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private windowService: WindowService,
    private intlService: IntlService,
    private stockPickingService: StockPickingService,
    private router: Router,
    private partnerService: PartnerService,
    private notificationService: NotificationService,
    private pickingTypeService: StockPickingTypeService,
    private stockMoveService: StockMoveService,
    private productService: ProductService,
    private modalService: NgbModal
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
  }

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


  onChangeUoMProduct(productId, line: AbstractControl) {
    if (this.picking.state != "done") {
      let modalRef = this.modalService.open(SharedDemoDataDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
      modalRef.componentInstance.title = 'Chọn đơn vị';
      modalRef.componentInstance.productId = productId;
      modalRef.result.then(
        res => {
          if (res) {
            line.get('productUOM').patchValue(res);
            line.get('productUOMId').patchValue(res.id);
          }
        }, () => {
        });
    }

  }

  onChangeProduct(value: ProductBasic2) {
    var product = value;
    var index = _.findIndex(this.moveLines.controls, o => {
      return o.get('product').value.id == product.id && o.get('productUOMId').value == product.uomId;
    });

    if (index !== -1) {
      var control = this.moveLines.controls[index];
      control.patchValue({ productQty: control.get('productQty').value + 1 });
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

  showAddLineModal() {
    const windowRef = this.windowService.open({
      title: 'Thêm chi tiết',
      content: StockPickingMlDialogComponent,
      resizable: false,
    });

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        var line = result as StockMoveDisplay;
        var lines = this.pickingForm.get('moveLines') as FormArray;
        lines.push(this.fb.group(line));
      }
    });
  }

  editLine(line: FormGroup) {
    const windowRef = this.windowService.open({
      title: 'Sửa chi tiết',
      content: StockPickingMlDialogComponent,
      resizable: false,
    });

    const instance = windowRef.content.instance;
    instance.line = line.value;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        var a = result as StockMoveDisplay;
        line.patchValue(result);
      }
    });
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
        this.router.navigate(['outgoing-pickings/edit/', result.id]);
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
          this.router.navigate(['outgoing-pickings/edit/', result.id]);
        }, () => {
          this.router.navigate(['outgoing-pickings/edit/', result.id]);
        });
      });
    }
  }
}

