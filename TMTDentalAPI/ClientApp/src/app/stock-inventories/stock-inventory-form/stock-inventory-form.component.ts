import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { StockInventoryCriteriaBasic, StockInventoryCriteriaPaged, StockInventoryCriteriaService } from '../stock-inventory-criteria.service';
import { StockInventoryLineOnChangeCreateLine, StockInventoryLineService } from '../stock-inventory-line.service';
import { StockInventoryService } from '../stock-inventory.service';

@Component({
  selector: 'app-stock-inventory-form',
  templateUrl: './stock-inventory-form.component.html',
  styleUrls: ['./stock-inventory-form.component.css']
})
export class StockInventoryFormComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  id: string;
  state: string;
  valueSearch: string;
  limit = 20;
  skip = 0;
  loading = false;
  // stockInventory: any;
  filterdCategories: ProductCategoryBasic[] = [];
  filterdCriterias: StockInventoryCriteriaBasic[] = [];

  public minDateTime: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public maxDateTime: Date = new Date(new Date(this.monthEnd.setHours(23, 59, 59)).toString());
  submitted = false;

  canCreateUpdate = false;
  canCreate = false;
  canprepareInventory = false;
  canActionDone = false;
  canCancel = false;
  canPrint = false;

  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @ViewChild('criteriaCbx', { static: true }) criteriaCbx: ComboBoxComponent;

  filterInventories = [
    { name: 'T???t c??? s???n ph???m', value: 'none' },
    { name: 'Nh??m s???n ph???m', value: 'category' },
    { name: 'Ch???n s???n ph???m th??? c??ng', value: 'partial' },
    { name: 'Ti??u ch?? ki???m kho', value: 'criteria' },
  ];

  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private route: ActivatedRoute,
    private router: Router,
    private notificationService: NotificationService,
    private productCategoryService: ProductCategoryService,
    private stockInventorySevice: StockInventoryService,
    private intlService: IntlService,
    private criteriaService: StockInventoryCriteriaService,
    private printService: PrintService,
    private stockInventoryLineService: StockInventoryLineService,
    private checkPermissionRole: CheckPermissionService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      dateObj: [null, Validators.required],
      date: null,
      locationId: null,
      productId: null,
      categoryId: null,
      category: null,
      criteriaId: null,
      criteria: null,
      filter: null,
      note: null,
      exhausted: false,
      state: 'draft',
      companyId: null,
      lines: this.fb.array([]),
      moves: this.fb.array([])
    });

    this.checkRole();
    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id');
      this.loadDataFromApi();
    });

    setTimeout(() => {

      this.categCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.categCbx.loading = true)),
        switchMap(value => this.searchCategories(value))
      ).subscribe(result => {
        this.filterdCategories = result;
        this.categCbx.loading = false;
      });


      this.criteriaCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.criteriaCbx.loading = true)),
        switchMap(value => this.searchCriteria(value))
      ).subscribe(result => {
        this.filterdCriterias = result.items;
        this.criteriaCbx.loading = false;
      });

      this.loadCategories();
      this.loadCriteria();
    });



  }

  get f() { return this.formGroup.controls; }

  get stockInventory() { return this.formGroup.value; }

  get stateControl() {
    return this.formGroup.get('state').value;
  }

  get filterControl() {
    return this.formGroup.get('filter').value;
  }

  get lines() {
    return this.formGroup.get('lines') as FormArray;
  }

  get moves() {
    return this.formGroup.get('moves') as FormArray;
  }

  loadDataFromApi() {
    if (this.id) {
      this.stockInventorySevice.get(this.id).subscribe(result => {
        this.formGroup.patchValue(result);
        let date = new Date(result.date);
        this.formGroup.get('dateObj').patchValue(date);

        if (result.category) {
          this.filterdCategories = _.unionBy(this.filterdCategories, [result.category], "id");
        }

        let control = this.formGroup.get('lines') as FormArray;
        control.clear();
        result.lines.forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });

        let moveControl = this.formGroup.get('moves') as FormArray;
        moveControl.clear();
        result.moves.forEach(move => {
          var g = this.fb.group(move);
          moveControl.push(g);
        });
        this.onChangeFilter();
      });
    } else {
      // var companyId = this.authService.userInfo.companyId;
      this.stockInventorySevice.getDefault().subscribe((result: any) => {
        this.formGroup.patchValue(result);
        let date = new Date(result.date);
        this.formGroup.get('dateObj').patchValue(date);

        let control = this.formGroup.get('lines') as FormArray;
        control.clear();
        result.lines.forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
      });
    }
  }

  createNew() {
    this.router.navigate(['stock-inventories/form']);
  }

  loadCategories() {
    this.searchCategories().subscribe((result) => {
      this.filterdCategories = _.unionBy(this.filterdCategories, result, "id");
    });
  }

  loadCriteria() {
    this.searchCriteria().subscribe((result) => {
      this.filterdCriterias = _.unionBy(this.filterdCriterias, result.items, "id");
    });
  }

  onChangeFilter() {
    var res = this.formGroup.value;
    this.submitted = false;
    if (res.filter === 'category') {
      this.formGroup.get("category").setValidators([Validators.minLength(0), Validators.required]);
      this.formGroup.get("category").updateValueAndValidity();
      this.formGroup.get('criteria').clearValidators();
      this.formGroup.get('criteria').updateValueAndValidity();
    }
    else if (res.filter === 'criteria') {
      this.formGroup.get("criteria").setValidators([Validators.minLength(0), Validators.required]);
      this.formGroup.get("criteria").updateValueAndValidity();
      this.formGroup.get('category').clearValidators();
      this.formGroup.get('category').updateValueAndValidity();
    }
    else {
      res.category = null;
      res.criteria = null;
      this.formGroup.get('category').clearValidators();
      this.formGroup.get('category').updateValueAndValidity();
      this.formGroup.get('criteria').clearValidators();
      this.formGroup.get('criteria').updateValueAndValidity();
    }

    this.formGroup.patchValue(res);

  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'product,medicine';
    return this.productCategoryService.autocomplete(val);
  }

  searchCriteria(q?: string) {
    var val = new StockInventoryCriteriaPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = q || '';
    return this.criteriaService.getPaged(val);
  }

  computeForm(val) {
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm");
    val.productId = val.product ? val.product.id : null;
    val.categoryId = val.category ? val.category.id : null;
    val.criteriaId = val.criteria ? val.criteria.id : null;
    return val;
  }


  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) {
      return false;
    }

    var val = this.formGroup.value;
    val = this.computeForm(val);

    if (this.id) {
      this.stockInventorySevice.update(this.id, val).subscribe(
        () => {
          this.notificationService.show({
            content: 'C???p nh???t th??nh c??ng',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.submitted = false;
          this.loadDataFromApi();
        }
      );
    } else {
      this.stockInventorySevice.create(val).subscribe(
        result => {
          this.router.navigate(['/stock-inventories/form'], { queryParams: { id: result.id } });
          this.notificationService.show({
            content: 'L??u th??nh c??ng',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.submitted = false;
        }
      );
    }

  }

  addLine(val) {
    var res = new StockInventoryLineOnChangeCreateLine();
    res.productId = val.id;
    res.locationId = this.formGroup.get('locationId').value;
    this.stockInventoryLineService.onChangeCreateLine(res).subscribe((result: any) => {

      var invLine = this.fb.group(result);

      if (!this.lines.controls.some(x => x.value.productId === invLine.value.productId)) {
        this.lines.push(invLine);
      } else {
        var line = this.lines.controls.find(x => x.value.productId === invLine.value.productId);
        if (line) {
          line.value.productQty += 1;
          line.patchValue(line.value);
        }
      }
    });

  }

  deleteLine(index: number) {
    this.lines.removeAt(index);
  }

  onChangeSearch(value) {
    this.valueSearch = value;
  }

  prepareInventory() {
    if (this.id) {
      this.submitted = true;
      if (this.formGroup.invalid) {
        return false;
      }

      var val = this.formGroup.value;
      val = this.computeForm(val);
      this.stockInventorySevice.update(this.id, val).subscribe(
        () => {
          this.stockInventorySevice.prepareInventory([this.id]).subscribe(rs => {
            this.loadDataFromApi();
            this.submitted = false;
          })
        }
      );
    }
  }

  actionDone() {
    if (this.id) {
      this.submitted = true;
      if (this.formGroup.invalid) {
        return false;
      }

      var val = this.formGroup.value;
      val = this.computeForm(val);
      this.stockInventorySevice.update(this.id, val).subscribe(() => {
        this.stockInventorySevice.actionDone([this.id]).subscribe(() => {
          this.notificationService.show({
            content: 'X??c nh???n th??nh c??ng',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.submitted = false;
          this.loadDataFromApi();
        })
      })
    }
  }

  actionCancel() {
    if (this.id) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'H???y phi???u ki???m kho';
      modalRef.componentInstance.body = "B???n c?? ch???c ch???n h???y phi???u ki???m kho ?"
      modalRef.result.then(() => {
        var ids = [];
        ids.push(this.id);
        this.stockInventorySevice.actionCancel(ids).subscribe(
          () => {
            this.notificationService.show({
              content: 'H???y th??nh c??ng',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'success', icon: true }
            });
            this.loadDataFromApi();
          }
        )
      }, () => {
      });
    }
  }

  onPrintStockInventory() {
    if (!this.id) {
      return;
    }
    this.stockInventorySevice.getPrint(this.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  getFilter(filter) {
    switch (filter) {
      case 'none':
        return 'T???t c??? s???n ph???m';
      case 'category':
        return 'Nh??m s???n ph???m';
      case 'criteria':
        return 'Ti??u ch?? ki???m kho';
      case 'partial':
        return 'Ch???n s???n ph???m th??? c??ng';
    }
  }

  checkRole() {
    this.canCreate = this.checkPermissionRole.check(["Stock.Inventory.Create"]);
    this.canCreateUpdate = this.checkPermissionRole.check(["Stock.Inventory.Create", "Stock.Inventory.Update"]);
    this.canPrint = this.checkPermissionRole.check(["Stock.Inventory.Read"]);
    this.canprepareInventory = this.checkPermissionRole.check(["Stock.Inventory.Update"]);
    this.canCancel = this.checkPermissionRole.check(["Stock.Inventory.Update"]);
    this.canActionDone = this.checkPermissionRole.check(["Stock.Inventory.Update"]);
  }

}
