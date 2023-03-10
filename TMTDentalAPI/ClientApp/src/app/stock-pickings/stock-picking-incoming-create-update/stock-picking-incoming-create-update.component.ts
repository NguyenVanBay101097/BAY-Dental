import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { forkJoin } from 'rxjs';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { EmployeeCreateUpdateComponent } from 'src/app/employees/employee-create-update/employee-create-update.component';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductBasic2, ProductPaged, ProductService } from 'src/app/products/product.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnerSupplierCuDialogComponent } from 'src/app/shared/partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';
import { PermissionService } from 'src/app/shared/permission.service';
import { SelectUomProductDialogComponent } from 'src/app/shared/select-uom-product-dialog/select-uom-product-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { TaiProductListSelectableComponent } from 'src/app/shared/tai-product-list-selectable/tai-product-list-selectable.component';
import { StockMoveOnChangeProduct, StockMoveService } from 'src/app/stock-moves/stock-move.service';
import {
  StockPickingDisplay,
  StockPickingService
} from '../stock-picking.service';
// declare var jquery: any;
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
  type2: string = 'medicine';
  createdByName: string = '';
  submitted = false;
  productSearch: string;
  productList: ProductBasic2[] = [];
  sourceProductList = [];
  listProducts: ProductSimple[] = [];
  listType: string = 'medicine,product';
  listTypePartner = [
    { text: "Nh?? cung c???p", value: 'supplier' },
    { text: "Kh??ch h??ng", value: 'customer' },
    { text: "Nh??n vi??n", value: 'employee' }
  ]
  canActionDone = false;
  canCreateUpdate = false;
  canCreate = false;
  canPrint = false;
  hasDefined = false;
  filteredPartners: PartnerSimple[] = [];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;

  @ViewChild(TaiProductListSelectableComponent) productListSelectable: TaiProductListSelectableComponent;

  get f() { return this.pickingForm.controls; }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private intlService: IntlService,
    private stockPickingService: StockPickingService,
    private router: Router,
    private partnerService: PartnerService,
    private notificationService: NotificationService,
    private stockMoveService: StockMoveService,
    private productService: ProductService,
    private modalService: NgbModal,
    private printServie: PrintService,
    private checkPermissionService: CheckPermissionService,
    public authService: AuthService,
    private permissionService: PermissionService
  ) { }

  ngOnInit() {
    this.pickingForm = this.fb.group({
      partner: [null],
      dateObj: [new Date(), Validators.required],
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
      this.filteredPartners = _.unionBy(results[0], results[1], results[2], 'id');
      this.partnerCbx.loading = false;
    });

    this.loadFilteredPartners();
    this.authService.getGroups().subscribe((result: any) => {
      this.permissionService.define(result);
      this.hasDefined = this.permissionService.hasOneDefined(['product.group_uom']);
    });
  }

  get state() { return this.pickingForm.get('state').value; }
  get partner() { return this.pickingForm.get('partner').value; }
  get note() { return this.pickingForm.get('note').value; }

  loadFilteredPartners() {
    this.searchPartners().subscribe(
      results => {
        this.filteredPartners = _.unionBy(this.filteredPartners, results[0], results[1], results[2], 'id');
      }
    );
  }

  searchPartners(search?: string) {
    var val = new PartnerPaged();
    val.search = search;
    val.customer = true;
    val.limit = 10;
    val.active = true;
    val.companyId = this.authService.userInfo.companyId;
    var partner$ = this.partnerService.getAutocompleteSimple(val);

    var val2 = Object.assign({}, val);
    delete val2.customer;
    val2.supplier = true;
    var supplier$ = this.partnerService.getAutocompleteSimple(val2);

    var val3 = Object.assign({}, val);
    delete val3.customer;
    val3.employee = true;
    var employee$ = this.partnerService.getAutocompleteSimple(val3);

    return forkJoin([partner$, supplier$, employee$]);
  }

  loadProductList() {
    // var val = new ProductPaged();
    // val.limit = 0;
    // val.offset = 0;
    // val.type = 'product,consu';
    // val.search = this.productSearch || '';
    // val.type2 = this.type2;
    // this.productService.getPaged(val).subscribe(res => {
    //   this.productList = res.items;
    //   this.sourceProductList = res.items;
    //   this.productListSelectable.resetIndex();
    // }, err => {
    // });
    var val = new ProductPaged();
    val.limit = 0;
    val.offset = 0;
    val.type = 'product,consu';
    this.productService
      .autocomplete2(val).subscribe(
        (res) => {
          this.listProducts = res;
        },
        (err) => {
          console.log(err);
        }
      );
  }

  // upDownEnterChange(code) {
  //   if (code == 40) {
  //     this.productListSelectable.moveUp();
  //   } else if (code == 38) {
  //     this.productListSelectable.moveDown();
  //   } else if (code == 13) {
  //     this.productListSelectable.selectCurrent();
  //   }
  // }

  // onProductInputSearchChange(text) {
  //   this.productSearch = text;

  //   if (!this.productSearch || this.productSearch.trim() == '')
  //     this.productList = this.sourceProductList;
  //   else {
  //     this.productSearch = this.productSearch.trim().toLocaleLowerCase();
  //     this.productList = this.sourceProductList.filter(x => x.name.toLocaleLowerCase().indexOf(this.productSearch) >= 0
  //       || x.nameNoSign.toLocaleLowerCase().indexOf(this.productSearch) >= 0
  //       || x.defaultCode.toLocaleLowerCase().indexOf(this.productSearch) >= 0
  //     );
  //   }

  //   this.productListSelectable.resetIndex();
  // }

  loadDefault() {
    this.stockPickingService.defaultGetIncoming().subscribe(result => {
      this.createdByName = this.authService.userInfo.name;
      this.pickingForm.patchValue(result);
      this.picking = result;
    });
  }

  loadRecord() {
    this.stockPickingService.get(this.id).subscribe((result: any) => {
      this.filteredPartners = _.unionBy(this.filteredPartners,[result.partner], 'id');
      this.createdByName = result.createdByName;
      this.picking = result;
      this.pickingForm.patchValue(result);
      this.moveLines.clear();
      result.moveLines.forEach(line => {
        const rs = this.fb.group(line);
        rs.controls.productUOMQty.setValidators(Validators.required);
        rs.controls.productUOMQty.updateValueAndValidity();
        this.moveLines.push(rs);
      });
    });
  }

  changeUoM(line: AbstractControl) {
    var product = line.get('product').value;
    let modalRef = this.modalService.open(SelectUomProductDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
    modalRef.componentInstance.title = 'Ch???n ????n v???';
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
      productSimple.defaultCode = product.defaultCode;
      productSimple.type2 = product.type2

      this.stockMoveService.onChangeProduct(val).subscribe((result: any) => {
        var group = this.fb.group({
          name: result.name,
          productUOMId: result.productUOM.id,
          productUOM: result.productUOM,
          product: productSimple,
          productId: product.id,
          productUOMQty: [1, Validators.required]
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
    this.submitted = true;
    if (!this.pickingForm.valid) {
      return false;
    }

    var val = this.pickingForm.value;
    val.partnerId = val.partner ? val.partner.id : null;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    val.createdByName = this.createdByName;
    if (this.id) {
      this.stockPickingService.update(this.id, val).subscribe(() => {
        this.notificationService.show({
          content: 'L??u th??nh c??ng',
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
          content: 'L??u th??nh c??ng',
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
    this.submitted = true;
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
              content: 'C???p nh???t th??nh c??ng',
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
            content: 'C???p nh???t th??nh c??ng',
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
            content: 'X??c nh???n th??nh c??ng',
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
    this.stockPickingService.Print(this.id).subscribe((res: any) => {
      this.printServie.printHtml(res.html);
    });
  }

  checkRole() {
    this.canActionDone = this.checkPermissionService.check(["Stock.Picking.Update"]);
    this.canCreateUpdate = this.checkPermissionService.check(["Stock.Picking.Update", "Stock.Picking.Create"]);
    this.canPrint = this.checkPermissionService.check(["Stock.Picking.Read"]);
    this.canCreate = this.checkPermissionService.check(["Stock.Picking.Create"]);
  }

  onChangeType(value) {
    this.type2 = value;
    this.loadProductList();
  }

  selectProduct(product) {
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
      productSimple.defaultCode = product.defaultCode;
      productSimple.type2 = product.type2

      this.stockMoveService.onChangeProduct(val).subscribe((result: any) => {
        var group = this.fb.group({
          name: result.name,
          productUOMId: result.productUOM.id,
          productUOM: result.productUOM,
          product: productSimple,
          productId: product.id,
          productUOMQty: [1, Validators.required]
        });

        this.moveLines.push(group);
        this.focusLastRow();
      });
    }
  }


  onCreatePartner(type) {
    var onModal = (comp, title) => {
      let modalRef = this.modalService.open(comp, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = title;

      modalRef.result.then((res: any) => {
        var resPartner = type == "employee"? res.partner: res;
          this.pickingForm.get("partner").patchValue(resPartner);
          this.filteredPartners = _.unionBy(this.filteredPartners, [resPartner], "id");
      }, () => {
      });
    }
    switch (type) {
      case "customer":
        onModal(PartnerCustomerCuDialogComponent, 'Th??m kh??ch h??ng');
        break;
      case "supplier":
        onModal(PartnerSupplierCuDialogComponent, 'Th??m nh?? cung c???p');
        break;
      case "employee":
        onModal(EmployeeCreateUpdateComponent, 'Th??m nh??n vi??n');
        break;
      default:
        break;
    }

  }
}


