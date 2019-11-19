import { Component, OnInit, Inject, ViewChild, ElementRef, Input } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ProductService } from '../product.service';
import { Product } from '../product';
import { ProductCategoryService, ProductCategoryPaged, ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { ProductCategory } from 'src/app/product-categories/product-category';
import { debounceTime, switchMap, tap, map, distinctUntilChanged } from 'rxjs/operators';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Observable, Subject } from 'rxjs';
import { ProductCategoryDialogComponent } from 'src/app/product-categories/product-category-dialog/product-category-dialog.component';
import * as _ from 'lodash';
import { ProductStepDisplay } from '../product-step';
import { or } from '@progress/kendo-angular-grid/dist/es2015/utils';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-product-dialog',
  templateUrl: './product-dialog.component.html',
  styleUrls: ['./product-dialog.component.css'],
  providers: [ProductService],
})

export class ProductDialogComponent implements OnInit {
  title: string;
  id: string;
  productForm: FormGroup;
  item: Product;
  inserted: boolean;
  filterdCategories: ProductCategoryBasic[] = [];
  opened = false;

  stepNameEdit = true;
  categCbxLoading = false;
  searchCategory = new Subject<string>();

  stepTab: boolean;
  order: number;
  stepList: ProductStepDisplay[] = [];
  stepListMore: ProductStepDisplay[] = [];
  lengthStepList: number = 0; // độ dài step list

  @Input() productDefaultVal: Product;

  @ViewChild('form', { static: true }) formView: any;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  type: string;

  constructor(private fb: FormBuilder, private productService: ProductService,
    private productCategoryService: ProductCategoryService, public activeModal: NgbActiveModal,
    private modalService: NgbModal) {
  }

  formStepEdit = this.fb.group({
    name: null
  })


  ngOnInit() {

    if (this.productDefaultVal) {
      this.productDefaultVal.type == 'service' ? this.stepTab = true : this.stepTab = false;
    }

    this.productForm = this.fb.group({
      name: ['', Validators.required],
      saleOK: false,
      purchaseOK: false,
      categ: [null, Validators.required],
      uomId: null,
      uompoId: null,
      type: 'consu',
      listPrice: 1,
      standardPrice: 0,
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
    });

    this.default();

    this.searchCategories('').subscribe(result => {
      this.filterdCategories = _.unionBy(this.filterdCategories, result, 'id');
    });

    this.categCbxFilterChange();
  }

  default() {
    if (this.id) {
      this.productService.get(this.id).subscribe(result => {
        this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
        this.productForm.patchValue(result);
        this.loadStepList();
        this.productForm.get('type').value == 'service' ? this.stepTab = true : this.stepTab = false;
      });
    } else {
      this.productService.defaultGet().subscribe(result => {
        if (result.categ) {
          this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
        }
        this.productForm.patchValue(result);
        if (this.productDefaultVal) {
          this.productForm.patchValue(this.productDefaultVal);
        }
      });
    }
  }

  get saleOK() {
    return this.productForm.get('saleOK').value;
  }

  get purchaseOK() {
    return this.productForm.get('purchaseOK').value;
  }

  getLabelTitle() {
    switch (this.type) {
      case 'service':
        return 'dịch vụ';
      case 'product':
        return 'vật tư';
      case 'medicine':
        return 'thuốc';
      default:
        return 'sản phẩm';
    }
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

  categCbxFilterChange2(e: string) {
    debounceTime(300);
    this.searchCategories(e.toLowerCase()).subscribe(result => {
      this.filterdCategories = result;
      this.categCbxLoading = false;
    });
  }

  onKeyDownStepName(event) {
    if (event.key === "Enter") {
      this.createTempProductStepList();
    }
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q;
    val.type = this.type;
    return this.productCategoryService.autocomplete(val);
  }

  displayFn(category: ProductCategory) {
    if (category) {
      return category.name;
    }
  }

  quickCreateCateg() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhóm ' + this.getLabelTitle();
    modalRef.componentInstance.type = this.type;
    modalRef.result.then(result => {
      this.filterdCategories.push(result as ProductCategoryBasic);
      this.productForm.patchValue({ categ: result });
    }, () => {
    });
  }

  onSave() {
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
      return this.productService.updateWithSteps(this.id, data);
    } else {
      // return this.productService.create(data);
      return this.productService.createWithSteps(data);
    }
  }

  getBodyData() {
    var data = this.productForm.value;
    data.categId = data.categ.id;
    data.stepList = this.stepList;
    data.type2 = this.type;
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


  // moveUp(order) {
  //   var index = order - 1;

  //   var temp = this.stepList[index];
  //   this.stepList[index] = this.stepList[index - 1];
  //   this.stepList[index - 1] = temp;

  //   this.reorder();
  //   console.log(this.stepList);
  // }

  // moveDown(order) {
  //   var index = order - 1;
  //   var temp = this.stepList[index];
  //   this.stepList[index] = this.stepList[index + 1];
  //   this.stepList[index + 1] = temp;

  //   this.reorder();
  //   console.log(this.stepList);
  // }

  //=============Sắp xếp lại step list
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

}

