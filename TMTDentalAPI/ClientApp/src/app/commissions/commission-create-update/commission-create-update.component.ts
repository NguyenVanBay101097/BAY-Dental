import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { CommissionCreateUpdateDialogComponent } from '../commission-create-update-dialog/commission-create-update-dialog.component';
import { CommissionService } from '../commission.service';

@Component({
  selector: 'app-commission-create-update',
  templateUrl: './commission-create-update.component.html',
  styleUrls: ['./commission-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CommissionCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  filteredProductCategories: ProductCategoryBasic[];
  selectedProductCategory: ProductCategoryBasic;
  min: number = 0;
  max: number = 100;

  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;

  id: string;
  submitted = false;
  temp_name: string;

  constructor(private fb: FormBuilder, 
    private route: ActivatedRoute, 
    private router: Router, 
    private commissionService: CommissionService, 
    private modalService: NgbModal, 
    private productCategoryService: ProductCategoryService,
    private productService: ProductService, 
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      commissionProductRules: this.fb.array([]),
    });

    this.routeActive();

    setTimeout(() => {
      this.loadProductCategories();
    });
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  getValueLineForm(line: FormGroup, key) {
    if (key == "name") {
      var appliedOn = line.get('appliedOn').value;
      switch (appliedOn) {
        case "3_global":
          return null;
        case "2_product_category":
          return line.get('categ').value['name'];
        case "0_product_variant":
          return line.get('product').value['name'];
        default:
          return null;
      }
    }
    return line.get(key) ? line.get(key).value : null;
  }

  getAppliedOn(line: FormGroup, key) {
    var appliedOn = line.get('appliedOn').value;
    switch (appliedOn) {
      case "3_global":
        return "Tất cả dịch vụ";
      case "2_product_category":
        return "Nhóm dịch vụ";
      case "0_product_variant":
        return "Dịch vụ";
      default:
        return null;
    }
  }

  get f() {
    return this.formGroup.controls;
  }

  get commissionProductRules() {
    return this.formGroup.get('commissionProductRules') as FormArray;
  }

  routeActive() {
    this.route.queryParams.subscribe(params => {
      this.id = params['id'];
    });

    if (this.id) {
      this.commissionService.get(this.id).subscribe(result => {
        this.formGroup.patchValue(result);
        this.temp_name = this.getValueForm("name");

        const control = this.formGroup.get('commissionProductRules') as FormArray;
        control.clear();
        result['commissionProductRules'].forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
        this.formGroup.markAsPristine();
      }, err => {
        console.log(err);
      });
    }
  }

  addLine() {
    let modalRef = this.modalService.open(CommissionCreateUpdateDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.result.then(result => {
      console.log(result);
      let line = result as any;
      this.commissionProductRules.push(this.fb.group(line));
      this.commissionProductRules.markAsDirty();
    }, () => {
    });
  }

  editLine(line: FormGroup) {
    let modalRef = this.modalService.open(CommissionCreateUpdateDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.line = line.value;
    modalRef.result.then(result => {
      line.patchValue(result);
      this.commissionProductRules.markAsDirty();
    }, () => {
    });
  }

  deleteLine(i) {
    this.commissionProductRules.removeAt(i);
    this.commissionProductRules.markAsDirty();
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;

    if (this.id) {
      this.commissionService.update(this.id, val)
      .subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.temp_name = this.getValueForm("name");
      }, err => {
        console.log(err);
      });
    } else {
      this.commissionService.create(val)
      .subscribe(result => {
        this.router.navigate(['/commissions/form'], {
          queryParams: {
            id: result['id']
          },
        });
        this.temp_name = this.getValueForm("name");
      }, err => {
        console.log(err);
      });
    }
  }

  createNew() {
    this.router.navigate(['/commissions/form']);
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      commissionProductRules: this.fb.array([]),
    });
  }

  loadProductCategories() {
    this.searchProductCategories('').subscribe(result => {
      this.filteredProductCategories = _.unionBy(this.filteredProductCategories, result, 'id');
    });
  }

  searchProductCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'service';
    return this.productCategoryService.autocomplete(val);
  }

  filterChangeProductCategories(value) {
    this.searchProductCategories(value).subscribe(result => {
      this.filteredProductCategories = result;
    });
  }

  selectionChangeProductCategories(value) {
    this.selectedProductCategory = value;
  }

  addProductsFromProductCategory() {
    if (this.selectedProductCategory == null)
      return;
    var val = new ProductPaged();
    val.categId = this.selectedProductCategory.id;
    val.type2 = "service";
    this.productService.getPaged(val).subscribe(
      result => {
        result.items.forEach(element => {
          this.commissionProductRules.push(this.fb.group({
            appliedOn: "0_product_variant",
            categ: null,
            categId: null,
            percentFixed: 0,
            product: { 
              id: element.id,
              name: element.name
            },
            productId: element.id
          }));
          this.commissionProductRules.markAsDirty();
        });
      }, error => {
        console.log(error);
      }
    );
    this.selectedProductCategory = null;
  }
}
