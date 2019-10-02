import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, NgForm } from '@angular/forms';
import { Product } from '../product';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-product-medicine-form',
  templateUrl: './product-medicine-form.component.html',
  styleUrls: ['./product-medicine-form.component.css']
})

export class ProductMedicineFormComponent implements OnInit {
  @Input() formGroup: FormGroup;
  @Input() filteredCategories: ProductCategoryBasic[] = [];
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @Input() product: Product;
  @Output() validate = new EventEmitter<boolean>();
  @Output() btnCreateCategClick = new EventEmitter<any>();
  @Output() filterChangeCateg = new EventEmitter<string>();
  constructor(private fb: FormBuilder, private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300)
    ).subscribe(result => {
      console.log(result);
      this.filterChangeCateg.emit(result);
    });
  }

  quickCreateCateg() {
    this.btnCreateCategClick.emit(null);
  }
}

