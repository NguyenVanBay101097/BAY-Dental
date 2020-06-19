import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';

@Component({
  selector: '[app-toa-thuoc-line-save]',
  templateUrl: './toa-thuoc-line-save.component.html',
  styleUrls: ['./toa-thuoc-line-save.component.css']
})
export class ToaThuocLineSaveComponent implements OnInit {
  thuocForm: FormGroup;
  id: string;
  filteredProducts: ProductSimple[];

  constructor(private fb: FormBuilder, private productService: ProductService) { }

  ngOnInit() {
    this.thuocForm = this.fb.group({
      product: [null, Validators.required],
      numberOfTimes: 1, 
      amountOfTimes: 1, 
      quantity: 1, 
      unit: "Viên", 
      numberOfDays: 1, 
    })

    setTimeout(() => {
      this.loadFilteredProducts();
    });
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.keToaOK = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  loadFilteredProducts() {
    return this.searchProducts().subscribe(result => {
      this.filteredProducts = result;
    });
  }
}
