import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
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
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-stock-picking-incoming-create-update',
  templateUrl: './stock-picking-incoming-create-update.component.html',
  styleUrls: ['./stock-picking-incoming-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class StockPickingIncomingCreateUpdateComponent implements OnInit {
  pickingForm: FormGroup;
  opened = false;
  picking: StockPickingDisplay = new StockPickingDisplay();
  id: string;

  productSearch: string;
  productList: ProductBasic2[] = [];

  filteredPartners: PartnerSimple[] = [];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;

  @ViewChild(TaiProductListSelectableComponent, { static: false }) productListSelectable: TaiProductListSelectableComponent;

  constructor(private fb: FormBuilder, private route: ActivatedRoute,
    private windowService: WindowService, private intlService: IntlService,
    private stockPickingService: StockPickingService, private router: Router, private partnerService: PartnerService,
    private notificationService: NotificationService, private pickingTypeService: StockPickingTypeService,
    private stockMoveService: StockMoveService, private productService: ProductService) { }

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

  loadFilteredPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = result;
    });
  }

  searchPartners(search?: string) {
    var val = new PartnerPaged();
    val.search = search;
    val.supplier = true;
    return this.partnerService.getAutocompleteSimple(val);
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
    this.stockPickingService.defaultGetIncoming().subscribe(result => {
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

  onChangeProduct(value: ProductBasic2) {
    var item = new StockMoveDisplay();
    item.product = new ProductSimple();
    item.product.id = value.id;
    item.productId = value.id;
    item.product.name = value.name;
    item.name = value.name;
    item.productUOMQty = 1;
    item.priceUnit = 0;
    let flag = true;
    this.moveLines.controls.forEach(line => {
      if (line.get('productId').value == item.productId) {
        line.get('productUOMQty').setValue(line.get('productUOMQty').value + 1);
        flag = false;
        return;
      }
    })

    if (flag) {
      this.moveLines.push(this.fb.group(item));
      this.focusLastRow();
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
      return o.get('productUOMQty').value == null || o.get('priceUnit').value == null
    });
    return index !== -1 ? false : true;
  }

  onSaveOrUpdate() {
    if (!this.checkQtyPriceValid()) {
      alert('Vui lòng nhập số lượng và đơn giá');
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
        this.router.navigate(['incoming-pickings/edit/', result.id]);
      });
    }
  }

  actionDone() {
    if (this.id) {
      if (this.pickingForm.dirty) {
        //update and done

        if (!this.checkQtyPriceValid()) {
          alert('Vui lòng nhập số lượng và đơn giá');
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
      if (!this.checkQtyPriceValid()) {
        alert('Vui lòng nhập số lượng và đơn giá');
        return;
      }

      //save and done
      if (!this.pickingForm.valid) {
        return;
      }

      var val = this.pickingForm.value;
      val.partnerId = val.partner ? val.partner.id : null;
      val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');

      this.stockPickingService.create(val).subscribe(result => {
        this.stockPickingService.actionDone([result.id]).subscribe(() => {
          this.router.navigate(['incoming-pickings/edit/', result.id]);
        }, () => {
          this.router.navigate(['incoming-pickings/edit/', result.id]);
        });
      });
    }
  }
}


