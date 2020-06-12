import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TCareRuleCondition } from 'src/app/tcare/tcare.service';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService, ProductPaged, ProductFilter } from 'src/app/products/product.service';

@Component({
  selector: 'app-audience-filter-service',
  templateUrl: './audience-filter-service.component.html',
  styleUrls: ['./audience-filter-service.component.css']
})
export class AudienceFilterServiceComponent implements OnInit {

  formGroup: FormGroup;
  searchUpdate = new Subject<string>();
  productList = [];
  search: string;
  submitted = false;
  type: string;
  name: string;
  @Output() saveClick = new EventEmitter<any>();
  data: any;

  constructor(private fb: FormBuilder,
    private notificationService: NotificationService,
    private productService: ProductService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      op: 'contains',
      product: [null, Validators.required]
    });

    setTimeout(() => {
      if (this.data) {
        this.formGroup.patchValue({
          op: this.data.op,
          product: {
            id: this.data.value,
            name: this.data.displayValue
          }
        });
      }

      this.loadProductList();

      this.searchUpdate.pipe(
        debounceTime(400),
        distinctUntilChanged())
        .subscribe(value => {
          this.loadProductList(value);
          this.formGroup.get('product').setValue(null);
          this.submitted = false;
        });
    });
  }

  get productControl() {
    return this.formGroup.get('product');
  }

  productClick(product) {
    this.formGroup.get('product').setValue(product);
  }

  get productSelected() {
    return this.formGroup.get('product').value;
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.saleOK = true;
    val.limit = 5;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  loadProductList(q?: string) {
    this.searchProducts(q).subscribe(result => {
      this.productList = result;
    });
  }

  getOpDisplay(op) {
    if (op == 'contains') {
      return 'Chứa'
    } else if (op == 'not_contains') {
      return 'Không chứa';
    } else {
      return '';
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    var res = {
      op: value.op,
      opDisplay: this.getOpDisplay(value.op),
      value: value.product.id,
      displayValue: value.product.name,
      type: this.type,
      name: this.name
    };

    this.saveClick.emit(res);
  }
}
