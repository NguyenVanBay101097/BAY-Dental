import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { ToaThuocLineSave } from '../../toa-thuoc.service';

@Component({
  selector: '[app-toa-thuoc-line-save]',
  templateUrl: './toa-thuoc-line-save.component.html',
  styleUrls: ['./toa-thuoc-line-save.component.css']
})
export class ToaThuocLineSaveComponent implements OnInit, OnChanges {
  thuocForm: FormGroup;
  id: string;
  filteredProducts: ProductSimple[];
  valThuocForm: any;
  @Input() dataThuocReceive: any;
  @Input() createOrEdit: boolean;
  @Output() showCreateOrEdit = new EventEmitter<boolean>();
  @Output() dataThuocSend = new EventEmitter<any>();

  constructor(private fb: FormBuilder, private productService: ProductService) { }

  ngOnInit() {
    this.thuocForm = this.fb.group({
      product: [null, Validators.required],
      numberOfTimes: 1, 
      amountOfTimes: 1, 
      quantity: 1, 
      unit: "Viên", 
      numberOfDays: 1, 
      note: ""
    })

    if (this.valThuocForm) {
      this.thuocForm.patchValue(this.valThuocForm);
    }

    setTimeout(() => {
      this.loadFilteredProducts();
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.dataThuocReceive) {
      this.valThuocForm = changes.dataThuocReceive.currentValue;
    }
  }

  getValue(key: string) {
    return this.thuocForm.get(key).value;
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

  saveLine() {
    this.showCreateOrEdit.emit(false);
    this.dataThuocSend.emit(this.thuocForm.value);
  }
  
  cancelLine() {
    this.showCreateOrEdit.emit(false);
  }

  editLine(i) {
    this.showCreateOrEdit.emit(true);
    // this.indexLineEdit = i;
  }

  deleteLine(i) {
    // this.lines.splice(i, 1); 
  }
}
