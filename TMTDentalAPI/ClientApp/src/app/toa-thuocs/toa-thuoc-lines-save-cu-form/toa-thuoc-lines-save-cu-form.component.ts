import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { ToaThuocLineSave } from '../toa-thuoc.service';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-toa-thuoc-lines-save-cu-form',
  templateUrl: './toa-thuoc-lines-save-cu-form.component.html',
  styleUrls: ['./toa-thuoc-lines-save-cu-form.component.css']
})
export class ToaThuocLinesSaveCuFormComponent implements OnInit, OnChanges {
  thuocsForm: FormGroup;
  thuocArray: FormArray;
  filteredProducts: ProductSimple[];
  submitted = false;
  tempThuocArray: any[] = [];
  @Input() dataThuocsReceive: any[];
  @Output() dataThuocsSend = new EventEmitter<any[]>();
  @Input() eventSaveReceive: any;

  get getThuocArray() { return this.thuocsForm.get('thuocArray') as FormArray; }

  constructor(private fb: FormBuilder, 
    private productService: ProductService, 
    private modalService: NgbModal) { }

  ngOnInit() {
    this.thuocsForm = this.fb.group({
      thuocArray: this.fb.array([])
    });

    setTimeout(() => {
      this.loadFilteredProducts();
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.dataThuocsReceive) {
      this.addThuocArray();
      this.dataThuocsSend.emit(this.getThuocArray.value); //
    }
    if (changes.eventSaveReceive) {
      if (changes.eventSaveReceive.currentValue == true) {
        for (let i = 0; i < this.getThuocArray.length; i++) {
          this.setValueThuocArray(i, 'submitted', true);
        }
      } else {
        for (let i = 0; i < this.getThuocArray.length; i++) {
          this.setValueThuocArray(i, 'submitted', false);
        }
      }
    }
  }

  setThuocItem(item): FormGroup {
    if (item) {
      return this.fb.group({
        product: [item.product, Validators.required],
        productId: item.productId,
        numberOfTimes: item.numberOfTimes, 
        amountOfTimes: item.amountOfTimes, 
        quantity: item.quantity,  
        numberOfDays: item.numberOfDays, 
        useAt: item.useAt,
        submitted: false
      })
    } else {
      return this.fb.group({
        product: [null, Validators.required],
        productId: null,
        numberOfTimes: 1, 
        amountOfTimes: 1, 
        quantity: 1, 
        numberOfDays: 1, 
        useAt: 'after_meal',
        submitted: false
      })
    }
  }

  deleteThuocItem(i) {
    this.getThuocArray.removeAt(i);
  }

  addThuocArray() {
    if (!this.dataThuocsReceive) 
      return;
    for (let i = 0; i < this.dataThuocsReceive.length; i++) {
      this.getThuocArray.push(this.setThuocItem(this.dataThuocsReceive[i]));
      this.tempThuocArray.push(null);
    }
  }

  addThuocArray_DefaultItem() {
    // this.getThuocArray.insert(0, this.setThuocItem(null));
    this.getThuocArray.push(this.setThuocItem(null));
    this.tempThuocArray.splice(0, 0, null);
  }

  getControlsThuocArray(i) { 
    return this.getThuocArray.at(i)['controls']; 
  }

  getValueThuocsForm(key: string) {
    return this.thuocsForm.get(key).value;
  }

  getValueThuocArray(i, key: string) {
    return this.getThuocArray.at(i).get(key).value;
  }

  setValueThuocArray(i, key: string, value) {
    this.getThuocArray.at(i).get(key).setValue(value);
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

  getUsedAt(useAt) {
    switch (useAt) {
      case 'before_meal':
        return 'trước khi ăn';
      case 'after_meal':
        return 'sau khi ăn';
      case 'in_meal':
        return 'trong khi ăn';
      case 'after_wakeup':
        return 'sau khi thức dậy';
      case 'before_sleep':
        return 'trước khi đi ngủ';
      default:
        return 'sau khi ăn';
    }
  }

  onCreate() {
    this.addThuocArray_DefaultItem();
    this.dataThuocsSend.emit(this.getThuocArray.value); //
  }

  onChange(event) {
    this.dataThuocsSend.emit(this.getThuocArray.value); //
  }

  deleteLine(i) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa Thuốc';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa Thuốc này?';

    modalRef.result.then(() => {
      this.deleteThuocItem(i);
      this.dataThuocsSend.emit(this.getThuocArray.value); //
    }, () => {
    });
  }
}
