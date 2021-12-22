import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { da } from 'date-fns/locale';
import * as _ from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { ProductCategory } from 'src/app/product-categories/product-category';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ProductCategoryDialogComponent } from 'src/app/shared/product-category-dialog/product-category-dialog.component';
import { UoMBasic, UoMPaged, UomService } from 'src/app/uoms/uom.service';
import { Product } from '../product';
import { ProductBomDisplay } from '../product-bom';
import { ProductProductCuDialogComponent } from '../product-product-cu-dialog/product-product-cu-dialog.component';
import { ProductStepDisplay } from '../product-step';
import { ProductFilter, ProductService } from '../product.service';

@Component({
  selector: 'app-product-service-cu-dialog',
  templateUrl: './product-service-cu-dialog.component.html',
  styleUrls: ['./product-service-cu-dialog.component.css'],
})

export class ProductServiceCuDialogComponent implements OnInit {
  title: string;
  id: string;
  productForm: FormGroup;
  item: Product;
  inserted: boolean;
  filterdCategories: ProductCategoryBasic[] = [];
  opened = false;
  submitted = false;

  stepNameEdit = true;
  categCbxLoading = false;
  productCbxLoading = false;
  uoM: any;
  searchCategory = new Subject<string>();

  stepTab: boolean;
  order: number;
  stepList: ProductStepDisplay[] = [];
  productBomList: ProductBomDisplay[] = [];
  stepListMore: ProductStepDisplay[] = [];
  lengthStepList: number = 0; // độ dài step list
  filteredProducts: any[] = [];
  isExist: boolean = false;
  showStandardPrice = false; // ẩn hiện giá vốn theo phân quyền
  filterdUoMs: UoMBasic[] = [];

  @Input() productDefaultVal: Product;
  @Input() name: string = '';

  @ViewChild('form', { static: true }) formView: any;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @ViewChild('uomCbx', { static: true }) uomCbx: ComboBoxComponent;


  constructor(private fb: FormBuilder, private productService: ProductService,
    private productCategoryService: ProductCategoryService, public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private authService: AuthService,
    private checkPermissionService: CheckPermissionService,
    private uomService: UomService) {
  }

  formStepEdit = this.fb.group({
    name: null
  })


  ngOnInit() {
    this.productForm = this.fb.group({
      name: [this.name, Validators.required],
      categ: [null, Validators.required],
      uom: [null, Validators.required],
      listPrice: 1,
      standardPrice: 0,
      laboPrice: 0,
      defaultCode: null,
      isLabo: false,
      firm: null,
      steps: this.fb.array([]),
      // Định mức vật tư
      boms: this.fb.array([]),
    });


    this.searchCategories('').subscribe(result => {
      this.filterdCategories = _.unionBy(this.filterdCategories, result, 'id');
    });

    this.searchProducts().subscribe(result => {
      this.filteredProducts = result;
    });

    this.uomCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.uomCbx.loading = true)),
      switchMap(value => this.searchUoms(value))
    ).subscribe((result: any) => {
      this.filterdUoMs = result.items;
      this.uomCbx.loading = false;
    });

    if (this.id) {
      this.loadDataFromApi();
    }

    this.checkPermission();
    this.categCbxFilterChange();
    this.productCbxFilterChange();
    this.loadUoms();

  }

  loadDataFromApi() {
    this.productService.getService(this.id).subscribe((result: any) => {
      console.log(result);
      this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
      this.filterdUoMs = _.unionBy(this.filterdUoMs, [result.uom as UoMBasic], 'id');

      this.productForm.patchValue(result);

      result.steps.forEach(step => {
        this.stepsFA.push(this.fb.group({
          id: step.id,
          name: [step.name, Validators.required],
        }))
      });

      result.boms.forEach(bom => {
        this.boms.push(this.fb.group({
          id: bom.id,
          materialProduct: [bom.materialProduct, Validators.required],
          productUOM: bom.productUOM,
          quantity: [bom.quantity, Validators.required]
        }))
      });
    });
  }

  loadDefault() {
    this.productService.defaultGet().subscribe(result => {
      if (result.categ) {
        this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
      }
      result.name = this.name;
      this.productForm.patchValue(result);
      this.productForm.get('type').setValue('service');
      this.productForm.get('type2').setValue('service');
      this.productForm.get('purchaseOK').setValue(false);
    });

  }

  get saleOK() {
    return this.productForm.get('saleOK').value;
  }

  get purchaseOK() {
    return this.productForm.get('purchaseOK').value;
  }

  categCbxFilterChange() {
    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchCategories(value))
    ).subscribe(result => {
      console.log('go ogogo');
      this.filterdCategories = result;
      this.categCbx.loading = false;
    });
  }

  productCbxFilterChange() {

  }

  categCbxFilterChange2(e: string) {
    debounceTime(300);
    this.searchCategories(e.toLowerCase()).subscribe(result => {
      this.filterdCategories = result;
      this.categCbxLoading = false;
    });
  }

  productCbxFilterChange2(e: string) {
    debounceTime(300);
    this.searchProducts(e.toLowerCase()).subscribe(result => {
      this.filteredProducts = result;
      this.productCbxLoading = false;
    });
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'service';
    return this.productCategoryService.autocomplete(val);
  }

  searchUoms(q?: string) {
    return this.uomService.listServiceUoMs({ search: q || '' });
  }

  loadUoms() {
    this.searchUoms().subscribe((result: any) => {
      this.filterdUoMs = _.unionBy(this.filterdUoMs, result.uoms, 'id');
    });
  }

  displayFn(category: ProductCategory) {
    if (category) {
      return category.name;
    }
  }

  quickCreateCateg() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhóm dịch vụ';
    modalRef.componentInstance.type = 'service';
    modalRef.result.then(result => {
      this.filterdCategories.push(result as ProductCategoryBasic);
      this.productForm.patchValue({ categ: result });
    }, () => {
    });
  }

  get isLabo() {
    return this.productForm.get('isLabo').value;
  }

  saveOrUpdate() {
    this.submitted = true;
    if (!this.productForm.valid) {
      return;
    }

    var data = this.getBodyData();
    if (this.id) {
      data.id = this.id;
      this.productService.updateService(data).subscribe(
        result => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.activeModal.close(result);
        });
    } else {
      this.productService.createService(data).subscribe(
        (result: any) => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.activeModal.close(result);
        });
    }
  }

  getBodyData(): any {
    var data = this.productForm.value;
    return {
      name: data.name,
      categId: data.categ.id,
      uomId: data.uom.id,
      defaultCode: data.defaultCode,
      standardPrice: data.standardPrice,
      laboPrice: data.laboPrice,
      listPrice: data.listPrice,
      isLabo: data.isLabo,
      firm: data.firm,
      steps: data.steps.map(x => {
        return {
          id: x.id,
          name: x.name
        }
      }),
      boms: data.boms.map(x => {
        return {
          id: x.id,
          materialProductId: x.materialProduct.id,
          productUomId: x.productUOM.id,
          quantity: x.quantity,
        }
      })
    };
  }

  onSaveAndNew() {
    if (!this.productForm.valid) {
      return;
    }

    if (!this.id) {
      var data = this.getBodyData();
      this.productService.create(data).subscribe(() => {
        this.inserted = true;
        this.formView.reset();
        this.productService.defaultGet().subscribe(result => {
          this.productForm.patchValue(result);
          this.nameInput.nativeElement.focus();
        });
      });
    }
  }

  onCancel() {
    this.submitted = false;
    if (this.inserted) {
      this.activeModal.close(true);
    } else {
      this.activeModal.dismiss();
    }
  }

  changeType() {
    this.productForm.get('type').value == 'service' ? this.stepTab = true : this.stepTab = false;
  }

  loadStepList() {
    this.productService.getStepByProductId(this.id).subscribe(
      rs => {
        this.stepList = rs;
      }
    )
  }

  get stepsFA() {
    return this.productForm.get('steps') as FormArray;
  }

  createTempProductStepList(stepName) {
    if (stepName) {
      this.stepsFA.push(this.fb.group({
        name: stepName
      }))
    }
  }

  searchProducts(q?: string) {
    var filter = new ProductFilter();
    filter.search = q || '';
    filter.type2 = 'product';
    filter.limit = 1000;
    filter.offset = 0;
    return this.productService.autocomplete2(filter);
  }

  reorder() {
    for (var i = 0; i < this.stepList.length; i++) {
      this.stepList[i].order = i + 1;
    }
  }

  removeStepListItem(i) {
    this.stepsFA.removeAt(i);
  }

  createProductStep(productId) {
    var prdStep = new ProductStepDisplay;
    prdStep.name = this.productForm.get('stepName').value;
    prdStep.productId = productId;
    prdStep.order = this.stepList.length + 1;
    prdStep.default = true;
    // this.productService.createStep(prdStep).subscribe(
    //   rs => {
    //     this.loadStepList();
    //     console.log(rs);
    //   },
    //   er => {
    //     console.log(er);
    //   }
    // )
  }

  changeCheckbox(order) {
    this.stepList[order - 1].default = !this.stepList[order - 1].default;
    // this.productService.updateDefault(pds, id).subscribe(
    //   rs => {
    //     this.loadStepList();
    //   }
    // )
  }

  //Submit khi edit tên công đoạn
  submitStepName(order) {
    var index = order - 1;
    this.stepList[index].name = this.formStepEdit.get('name').value;
    this.inputOrder = 0;
    // this.productService.updateName(pds, id).subscribe(
    //   rs => {
    //     this.loadStepList();
    //     this.inputOrder = 0;
    //   }
    // );
  }

  inputOrder: number = 0;
  editStepOrCancel(order, name: string) {
    if (this.inputOrder == 0) {
      this.inputOrder = order;
      this.formStepEdit.get('name').setValue(name);
    } else {
      this.inputOrder = 0;
    }
  }

  showInput(order) {
    if (order != this.inputOrder) {
      return true;
    } else {
      return false;
    }
  }

  disableButton(order, btnEdit: boolean) {
    if ((this.showInput(order) || !btnEdit) && this.inputOrder != 0) {
      return true;
    } else {
      return false;
    }
  }


  drop(event: CdkDragDrop<string[]>) {
    if (this.inputOrder == 0) {
      moveItemInArray(this.stepList, event.previousIndex, event.currentIndex);
      this.reorder();
    }
  }

  get f() {
    return this.productForm.controls;
  }

  createMaterialProduct() {
    let modalRef = this.modalService.open(ProductProductCuDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm: vật tư";
    modalRef.result.then(
      result => {
        this.loadFilteredProducts();
      },
      () => { }
    );
  }

  loadFilteredProducts() {
    this.searchProducts().subscribe(res => {
      this.filteredProducts = res;
    });
  }

  get boms() {
    return this.productForm.get('boms') as FormArray;
  }

  getListProductBom() {
    this.productBomList.forEach((element, index) => {
      if (element.materialProductId == null) {
        this.productBomList.splice(index, 1);
      }
    });
  }

  addMaterialProduct() {
    var line = this.fb.group({
      materialProduct: [null, Validators.required],
      productUOM: [null, Validators.required],
      quantity: [0, Validators.required]
    });

    this.boms.push(line);

  }

  deleteMaterialProduct(index) {
    this.boms.removeAt(index);
    this.boms.markAsDirty();
  }

  onValueChange(item, i) {
    if (item) {
      var temp = this.boms.value.filter(x => x.materialProduct ? x.materialProduct.id == item.id : false);
      if (temp.length > 1) {
        this.notificationService.show({
          content: 'Vật tư đã tồn tại',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });

        this.boms.at(i).get('materialProduct').setValue(null);
      }
      else {
        this.boms.at(i).get('productUOM').setValue(item.uom);
      }
    }
  }

  checkPermission() {
    this.showStandardPrice = this.checkPermissionService.check(['Catalog.Products.StandardPrice']);
  }
}




