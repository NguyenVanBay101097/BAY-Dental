import { Component, OnInit, Inject, ViewChild, ElementRef, Input } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { ProductFilter, ProductService } from '../product.service';
import { Product } from '../product';
import { ProductCategoryService, ProductCategoryPaged, ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { ProductCategory } from 'src/app/product-categories/product-category';
import { debounceTime, switchMap, tap, map, distinctUntilChanged } from 'rxjs/operators';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Observable, Subject } from 'rxjs';
import * as _ from 'lodash';
import { ProductStepDisplay } from '../product-step';
import { or } from '@progress/kendo-angular-grid/dist/es2015/utils';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { ProductCategoryDialogComponent } from 'src/app/shared/product-category-dialog/product-category-dialog.component';
import { ProductProductCuDialogComponent } from '../product-product-cu-dialog/product-product-cu-dialog.component';
import { ProductBomDisplay } from '../product-bom';
import { result } from 'lodash';
import { Console } from 'console';
import { NotificationService } from '@progress/kendo-angular-notification';

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
  @Input() productDefaultVal: Product;

  @ViewChild('form', { static: true }) formView: any;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private productService: ProductService,
    private productCategoryService: ProductCategoryService, public activeModal: NgbActiveModal,
    private modalService: NgbModal, private showErrorService: AppSharedShowErrorService, private notificationService: NotificationService) {
  }

  formStepEdit = this.fb.group({
    name: null
  })


  ngOnInit() {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      saleOK: true,
      purchaseOK: false,
      categ: [null, Validators.required],
      uomId: null,
      uompoId: null,
      type: 'service',
      type2: 'service',
      listPrice: 1,
      standardPrice: 0,
      laboPrice: 0,
      companyId: null,
      defaultCode: '',
      keToaNote: null,
      keToaOK: false,
      isLabo: false,
      //Công đoạn
      stepName: null,
      note: null,
      default: true,
      order: this.order,
      purchasePrice: 0,
      firm: null,
      // Định mức vật tư
      boms: this.fb.array([]),
    });



    this.searchCategories('').subscribe(result => {
      this.filterdCategories = _.unionBy(this.filterdCategories, result, 'id');
    });

    this.searchProducts('').subscribe(result => {
      this.filteredProducts = result;
      console.log(result);
      
    });

    this.categCbxFilterChange();
    this.productCbxFilterChange();
    if (this.id == null) {
      this.onCreate();
    }

    if (this.id) {
      this.loadDataFromApi();
    } else {
      this.loadDefault();
    }

  }

  loadDataFromApi() {
    this.productService.get(this.id).subscribe(result => {
      this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
      this.productForm.patchValue(result);

      this.loadStepList();
      this.productForm.get('type').value == 'service' ? this.stepTab = true : this.stepTab = false;
      if (result.boms.length>0) {
        var array = this.productForm.get('boms') as FormArray
        result.boms.forEach(bom => {
          var index = this.filteredProducts.findIndex(x => x.id == bom.materialProduct.id);
          if (index < 0) {
            this.filteredProducts.push(bom.materialProduct);
          }
          array.push(this.fb.group(bom))
        });
      }
      else{
        this.onCreate();
      }
    });
  }

  loadDefault() {
    this.productService.defaultGet().subscribe(result => {
      if (result.categ) {
        this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
      }
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
      this.filterdCategories = result;
      this.categCbx.loading = false;
    });
  }

  productCbxFilterChange() {
    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });
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

  onKeyDownStepName(event) {
    if (event.key === "Enter") {
      this.createTempProductStepList();
    }
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'service';
    return this.productCategoryService.autocomplete(val);
  }

  displayFn(category: ProductCategory) {
    if (category) {
      return category.name;
    }
  }

  quickCreateCateg() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhóm dịch vụ';
    modalRef.componentInstance.type = 'service';
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

    this.saveOrUpdate();
  }

  get isLabo() {
    return this.productForm.get('isLabo').value;
  }

  saveOrUpdate() {
    var data = this.getBodyData();
    if (this.id) {
      this.productService.update(this.id, data).subscribe(
        result=>{
          this.notificationService.show({
            content: 'Cập nhật dịch vụ thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
      });
    } else {
      this.productService.create(data).subscribe(
        result=>{
          this.notificationService.show({
            content: 'Tạo dịch vụ thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
      });;
    }
    this.activeModal.close(result);
  }

  removeNullItem(){
    // xóa các phần tử null
    for (let i=0; i<this.boms.length;) {
      if(this.boms.at(i).get('materialProduct').value==null){
        this.boms.removeAt(i);
        this.boms.markAsDirty();
      }
      else{
        i++;
      }
    }
  }
  getBodyData() {
    debugger;
    this.removeNullItem();
    var data = this.productForm.value;
    data.categId = data.categ.id;
    data.stepList = this.stepList;
    if (data.boms.length>0) {
      data.boms.forEach(bom => {
        if(bom.materialProduct){
          bom.materialProductId = bom.materialProduct.id;
          bom.productUOMId = bom.productUOM.id;
        }
        else{
          return;
        }
        
      });
    }
    return data;
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

  createTempProductStepList() {
    if (this.productForm.get('stepName').value && this.productForm.get('stepName').value.trim()) {

      var prdStep = new ProductStepDisplay;
      prdStep.name = this.productForm.get('stepName').value;
      prdStep.order = this.stepList.length + 1;
      prdStep.default = true;
      // this.reorderStepList();

      this.stepList.push(prdStep);
      this.productForm.get('stepName').setValue(null);
    }
  }

  searchProducts(q?: string) {
    var filter = new ProductFilter();
    filter.search = q || '';
    filter.type = 'product';
    filter.type2 = 'product';
    filter.limit = 20;
    filter.offset = 0;
    return this.productService.autocomplete2(filter);
  }

  reorder() {
    for (var i = 0; i < this.stepList.length; i++) {
      this.stepList[i].order = i + 1;
    }
  }

  removeStepListItem(order) {
    this.stepList.splice(order - 1, 1);
    this.reorder();
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
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm: vật tư";
    modalRef.result.then(
      
      result => {
        console.log(result);
        this.filteredProducts.push(result);
        var value = {
          materialProduct: result,
          productUOM: { id: result.uomId, name: result.uomName },
          quantity: 1
        }
        this.onCreate(value)
        this.searchProducts('').subscribe(res => {
          this.filteredProducts = res;
        });
      },
      () => { }
    );
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
    this.onCreate();
  }

  onCreate(bom?: any) {
    var grs = this.boms.controls.filter(x => x.value.materialProduct == null);
    if (grs && grs.length > 0 && bom == null) {
      this.notificationService.show({
        content: 'Vui lòng chọn vật tư',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return;
    }
    
    if (bom) {
      if(grs.length == 0){
        this.boms.push(
          this.fb.group(bom)
        )
        
      }
      else{
        for (let i=0; i<this.boms.length; i++) {;
          if(this.boms.at(i).get('materialProduct').value==null){
            this.boms.removeAt(i);
            this.boms.markAsDirty();
            this.boms.push(
              this.fb.group(bom)
            )
          }
        }
      }
    }
    else {
      this.boms.push(
        this.fb.group({
          quantity: 1,
          materialProduct: null,
          productUOM: null
        })
      )
    }
  }

  deleteMaterialProduct(index) {
    if(this.boms.length==1 && this.boms.at(index).get('materialProduct').value == null){
      return;
    }
    else if(this.boms.length==1 && this.boms.at(index).get('materialProduct').value != null){
      var gr = this.boms.at(index).patchValue({ materialProduct: null, productUOM: null, quantity: 1 });
    }
    else{
      this.boms.removeAt(index);
      this.boms.markAsDirty();
    }
  }

  onValueChange(item,i) {
    this.isExist = false;
    // var prBoms = this.MaterialProductBoms;
    if (item) {
      var grs = this.boms.controls.filter(x => x.value.materialProduct.id == item.id);
      if (grs && grs.length > 1) {
        grs[1].patchValue({ materialProduct: null, productUOM: null, quantity: 1 });
        this.notificationService.show({
          content: 'Vật tư đã tồn tại',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
      this.boms.controls.forEach(group => {
        if (group.value && group.value.materialProduct.id == item.id) {
          group.patchValue({ materialProduct: item, productUOM: item.uom, quantity: 1 });
        }
      });
    }
    else {
      var gr = this.boms.at(i);
      if (gr) {
        gr.patchValue({ materialProduct: null, productUOM: null, quantity: 1 });
      }
    }
  }

}


