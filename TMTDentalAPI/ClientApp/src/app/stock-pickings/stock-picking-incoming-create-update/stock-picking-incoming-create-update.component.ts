import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
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
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SelectUomProductDialogComponent } from 'src/app/shared/select-uom-product-dialog/select-uom-product-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { forkJoin } from 'rxjs';
import { unionBy } from 'lodash';
import { observe } from 'fast-json-patch';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
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
  sourceProductList = [];

  canActionDone = false;
  canCreateUpdate = false;
  canCreate = false;
  canPrint = false;

  filteredPartners: PartnerSimple[] = [];
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
    private printServie: PrintService,
    private checkPermissionService: CheckPermissionService
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
    this.checkRole();
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
    ).subscribe(results => {
      this.filteredPartners =_.unionBy(results[0], results[1],results[2], 'id');
      this.partnerCbx.loading = false;
    });

    this.loadFilteredPartners();
  }

  get state() {return this.pickingForm.get('state').value;}
  get partner() {return this.pickingForm.get('partner').value;}
  get note() {return this.pickingForm.get('note').value;}

  loadFilteredPartners() {
    this.searchPartners().subscribe(
      results => {
        this.filteredPartners =_.unionBy(results[0], results[1],results[2], 'id');
      }
    );
  }

  searchPartners(search?: string) {
    var val = new PartnerPaged();
    val.search = search;
    val.customer = true;
    val.limit = 10;
    val.active = true;
    var partner$ = this.partnerService.getAutocompleteSimple(val);

    var val2 = Object.assign({},val);
    delete val2.customer;
    val2.supplier = true;
    var supplier$ = this.partnerService.getAutocompleteSimple(val2);

    var val3 = Object.assign({},val);
    delete val3.customer;
    val3.employee = true;
    var employee$ = this.partnerService.getAutocompleteSimple(val3);

    return forkJoin([partner$, supplier$, employee$]);
  }

  loadProductList() {
    var val = new ProductPaged();
    val.limit = 0;
    val.offset = 0;
    val.type = 'product,consu';
    val.search = this.productSearch || '';
    this.productService.getPaged(val).subscribe(res => {
      this.productList = res.items;
      this.sourceProductList = res.items;
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

    if(!this.productSearch || this.productSearch.trim() == '')
     this.productList = this.sourceProductList;
     else {
       this.productSearch = this.productSearch.trim().toLocaleLowerCase();
      this.productList = this.sourceProductList.filter(x=> x.name.toLocaleLowerCase().indexOf(this.productSearch) >= 0
      || x.nameNoSign.toLocaleLowerCase().indexOf(this.productSearch) >= 0 
      || x.defaultCode.toLocaleLowerCase().indexOf(this.productSearch) >=0
      );
     }

     this.productListSelectable.resetIndex();
  }

  loadDefault() {
    this.stockPickingService.defaultGetIncoming().subscribe(result => {
      this.pickingForm.patchValue(result);
      this.picking = result;
    });
  }

  loadRecord() {
    this.stockPickingService.get(this.id).subscribe(result => {
      console.log(result);
      
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

  onSaveOrUpdate() {
    if (!this.pickingForm.valid) {
      return false;
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
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.router.navigate(['stock/incoming-pickings/edit/', result.id]);
      });
    }
  }

  actionDone() {
    if (this.id) {
      if (this.pickingForm.dirty) {
        if (!this.pickingForm.valid) {
          return;
        }

        var val = this.pickingForm.value;
        val.partnerId = val.partner ? val.partner.id : null;
        val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');

        this.stockPickingService.update(this.id, val).subscribe(() => {
          this.stockPickingService.actionDone([this.id]).subscribe(() => {
            this.notificationService.show({
              content: 'Xác nhận thành công',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'success', icon: true }
            });
            this.loadRecord();
          });
        });
      } else {
        //only need done
        this.stockPickingService.actionDone([this.id]).subscribe(() => {
          this.notificationService.show({
            content: 'Xác nhận thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.loadRecord();
        });
      }
    } else {
      //save and done
      if (!this.pickingForm.valid) {
        return;
      }

      var val = this.pickingForm.value;
      val.partnerId = val.partner ? val.partner.id : null;
      val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');

      this.stockPickingService.create(val).subscribe(result => {
        this.stockPickingService.actionDone([result.id]).subscribe(() => {
          this.notificationService.show({
            content: 'Xác nhận thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.router.navigate(['stock/incoming-pickings/edit/', result.id]);
        }, () => {
          this.router.navigate(['stock/incoming-pickings/edit/', result.id]);
        });
      });
    }
  }

  onPrint() {
    this.stockPickingService.Print(this.id).subscribe((res:any) => {     
      this.printServie.printHtml(res);
    });
  }

  checkRole(){
    this.canActionDone = this.checkPermissionService.check(["Stock.Picking.Update"]);
    this.canCreateUpdate = this.checkPermissionService.check(["Stock.Picking.Update","Stock.Picking.Create"]);
    this.canPrint = this.checkPermissionService.check(["Stock.Picking.Read"]);
    this.canCreate = this.checkPermissionService.check(["Stock.Picking.Create"]);
  }
}


