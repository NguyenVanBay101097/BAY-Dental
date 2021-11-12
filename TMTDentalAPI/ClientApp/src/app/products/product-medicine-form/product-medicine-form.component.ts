import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime } from 'rxjs/operators';
import { ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { Product } from '../product';

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
  constructor() { }

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

