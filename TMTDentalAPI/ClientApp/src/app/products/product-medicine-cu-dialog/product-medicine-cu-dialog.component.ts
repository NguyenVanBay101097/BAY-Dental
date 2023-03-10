import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ProductCategoryDialogComponent } from 'src/app/shared/product-category-dialog/product-category-dialog.component';
import { StockInventoryCriteriaPaged, StockInventoryCriteriaService } from 'src/app/stock-inventories/stock-inventory-criteria.service';
import { UoMBasic, UoMPaged, UomService } from 'src/app/uoms/uom.service';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-product-medicine-cu-dialog',
  templateUrl: './product-medicine-cu-dialog.component.html',
  styleUrls: ['./product-medicine-cu-dialog.component.css']
})

export class ProductMedicineCuDialogComponent implements OnInit {
  title: string;
  id: string;
  productForm: FormGroup;
  filterdCategories: ProductCategoryBasic[] = [];
  filterdUoMs: UoMBasic[] = [];
  filterdUoMPOs: UoMBasic[] = [];
  listProductCriteria = [];
  categoryIdSave: string;
  opened = false;
  submitted = false;
  showStandardPrice = false; // ẩn hiện giá vốn theo phân quyền

  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @ViewChild('uoMCbx', { static: true }) uoMCbx: ComboBoxComponent;
  @ViewChild('uoMPOCbx', { static: true }) uoMPOCbx: ComboBoxComponent;
  @ViewChild('criteriaMultiSelect', { static: true }) criteriaMultiSelect: MultiSelectComponent;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private uoMService: UomService,
    private productCriteriaService: StockInventoryCriteriaService, 
    private checkPermissionService: CheckPermissionService
  ) {
  }

  ngOnInit() {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      saleOK: false,
      purchaseOK: false,
      categ: [null, Validators.required],
      uom: null,
      uompo: null,
      type: 'consu',
      type2: 'medicine',
      listPrice: [1000, Validators.required],
      standardPrice: 0,
      productCriterias: null,
      companyId: null,
      defaultCode: '',
      keToaNote: null,
      keToaOK: true,
      isLabo: false,
      purchasePrice: 0,
      minInventory: 0,
      origin: null,
      expiry: null
    });

    setTimeout(() => {
      this.default();

      this.searchCategories().subscribe(result => {
        this.filterdCategories = _.unionBy(this.filterdCategories, result, 'id');
      });

      this.searchUoMs().subscribe((result: any) => {
        this.filterdUoMs = _.unionBy(this.filterdUoMs, result.items, 'id');
      });

      this.searchProductCriterias().subscribe((result: any) => {
        this.listProductCriteria = _.unionBy(this.listProductCriteria, result.items, 'id');
      });

      this.criteriaMultiSelect.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.criteriaMultiSelect.loading = true)),
        switchMap(value => this.searchProductCriterias(value))
      ).subscribe(result => {
        this.listProductCriteria = result.items;
        this.criteriaMultiSelect.loading = false;
      });

      this.checkPermission();
      this.loadProductCriteriaList();
      this.categCbxFilterChange();
      this.uoMCbxFilterChange();
      this.uoMPOCbxFilterChange();

    });
  }


  uoMPOCbxFilterChange() {
    this.uoMPOCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.uoMPOCbx.loading = true)),
      switchMap(value => this.searchUoMPOs(value))
    ).subscribe((result: any) => {
      console.log(result);
      this.filterdUoMPOs = result.items;
      this.uoMPOCbx.loading = false;
    });
  }


  searchUoMPOs(q?: string) {
    var uom = this.productForm.get('uom').value;
    var paged = new UoMPaged();
    paged.categoryId = uom ? uom.categoryId : null;
    paged.search = q || '';

    return this.uoMService.getPaged(paged);
  }

  default() {
    if (this.id) {
      this.productService.get(this.id).subscribe((result: any) => {
        this.productForm.patchValue(result);

        const inventory = result.minInventory ? result.minInventory : 0;
        this.productForm.get('minInventory').setValue(inventory);

        this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ], 'id');

        this.filterdUoMs = _.unionBy(this.filterdUoMs, [result.uom], 'id');

        if(result.stockInventoryCriterias.length > 0){      
          this.productForm.get('productCriterias').setValue(result.stockInventoryCriterias);
          this.listProductCriteria = _.unionBy(result.stockInventoryCriterias, this.listProductCriteria, 'id');
        }
  
        if (result.uompo) {
          this.uoMService.getPaged({ categoryId: result.uompo.categoryId }).subscribe((result2: any) => {
            this.filterdUoMPOs = result2.items;
            this.filterdUoMPOs = _.unionBy(this.filterdUoMPOs, [result.uompo], 'id');
          });
        }
      });
    } else {
      this.productService.getDefaultProducMedicine().subscribe((result: any) => {
        this.productForm.patchValue(result);

        if (result.categ) {
          this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
        }

        if (result.uom) {
          this.filterdUoMs = _.unionBy(this.filterdUoMs, [result.uom], 'id');
        }

        if (result.uompo) {
          this.uoMService.getPaged({ categoryId: result.uompo.categoryId }).subscribe((result2: any) => {
            this.filterdUoMPOs = result2.items;
            this.filterdUoMPOs = _.unionBy(this.filterdUoMPOs, [result.uompo], 'id');
          });
        }

        this.productForm.get('type').setValue(result.type);
        this.productForm.get('type2').setValue('medicine');
        this.productForm.get('saleOK').setValue(false);
        this.productForm.get('purchaseOK').setValue(false);
        this.productForm.get('keToaOK').setValue(true);
        this.productForm.get('minInventory').setValue(0);
      });
    }
  }

  loadProductCriteriaList() {
    this.searchProductCriterias().subscribe((result) => {
      this.listProductCriteria = _.unionBy(this.listProductCriteria, result.items, 'id');;
    });
  }

  searchProductCriterias(q?: string) {
    var val = new StockInventoryCriteriaPaged();
    val.limit = 0;
    val.offset = 0;
    val.search = q || '';
    return this.productCriteriaService.getPaged(val);
  }

  uoMCbxFilterChange() {
    this.uoMCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.uoMCbx.loading = true)),
      switchMap(value => this.searchUoMs(value))
    ).subscribe((result: any) => {
      this.filterdUoMs = result.items;
      this.uoMCbx.loading = false;
    });
  }

  // uoMPOCbxFilterChange() {
  //   this.uoMPOCbx.filterChange.asObservable().pipe(
  //     debounceTime(300),
  //     tap(() => (this.uoMPOCbx.loading = true)),
  //     switchMap(value => this.searchUoMs(value))
  //   ).subscribe(result => {
  //     this.filterdUoMPOs = result;
  //     this.uoMPOCbx.loading = false;
  //   });
  // }

  uoMChange(value) {
    if (value) {
      var uom = this.productForm.get('uom').value;
      var uom_po = this.productForm.get('uompo').value;
      var data = {
        uomId: uom != null ? uom.id : null,
        uomPOId: uom_po != null ? uom_po.id : null
      };

      this.productService.onChangeUOM(data).subscribe((result: any) => {
        this.productForm.patchValue(result);

        if (result.uom) {
          this.filterdUoMs = _.unionBy(this.filterdUoMs, [result.uom], 'id');
        }

        if (result.uompo) {
          this.uoMService.getPaged({ categoryId: result.uompo.categoryId }).subscribe((result2: any) => {
            this.filterdUoMPOs = result2.items;
            this.filterdUoMPOs = _.unionBy(this.filterdUoMPOs, [result.uompo], 'id');
          });
        }
      });
    }
  }

  searchUoMs(q?: string) {
    var val = new UoMPaged();
    val.search = q || '';
    return this.uoMService.getPaged(val);
  }

  categCbxFilterChange() {
    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchCategories(value))
    ).subscribe(result => {
      this.filterdCategories = result;
      this.categCbx.loading = false;
    });
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'medicine';
    return this.productCategoryService.autocomplete(val);
  }

  quickCreateCateg() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhóm thuốc';
    modalRef.componentInstance.type = 'medicine';
    modalRef.result.then(result => {
      this.filterdCategories.push(result as ProductCategoryBasic);
      this.productForm.patchValue({ categ: result });
    }, () => {
    });
  }

  onSave() {
    this.submitted = true;
    if (!this.productForm.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(result => {
      if (result) {
        this.activeModal.close(result);
      } else {
        this.activeModal.close(true);
      }
    }, err => {
      console.log(err);
    });
  }

  saveOrUpdate() {
    var data = this.getBodyData();
    if (this.id) {
      return this.productService.update(this.id, data);
    } else {
      return this.productService.create(data);
    }
  }

  getBodyData() {
    var data = this.productForm.value;
    data.categId = data.categ.id;
    data.uoMIds = [];
    data.uomId = data.uom.id;
    data.productCriteriaIds = data.productCriterias ? data.productCriterias.map(x => x.id) : [];
    // data.uompoId = data.uompo.id;
    // data.uoMIds.push(data.uompo.id);
    data.uompoId = data.uom.id;
    data.uoMIds.push(data.uom.id);
    return data;
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.dismiss();
  }

  get f() {
    return this.productForm.controls;
  }

  loadListProductCriteria() {
    var page = {
      limit: 0,
      offset: 0
    }
    this.productCriteriaService.getPaged(page).subscribe(
      (res: any) => {
        this.listProductCriteria = res.items;
      }
    );
  }

  checkPermission(){
    this.showStandardPrice = this.checkPermissionService.check(['Catalog.Products.StandardPrice']);
  }
}




