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

  get getThuocArray() { return this.thuocsForm.get('thuocArray') as FormArray; }

  constructor(private fb: FormBuilder, 
    private productService: ProductService, 
    private modalService: NgbModal) { }

  ngOnInit() {
    this.thuocsForm = this.fb.group({
      thuocArray: this.fb.array([])
    });

    this.addThuocArray();

    setTimeout(() => {
      this.loadFilteredProducts();
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    this.addThuocArray();
  }

  setThuocItem(item, openInput, i): FormGroup {
    if (i) {
      this.getThuocArray.at(i).setValue(item);
    } else {
      if (item) {
        return this.fb.group({
          product: [item.product, Validators.required],
          productId: item.productId,
          numberOfTimes: item.numberOfTimes, 
          amountOfTimes: item.amountOfTimes, 
          quantity: item.quantity, 
          unit: "Viên", 
          numberOfDays: item.numberOfDays, 
          useAt: item.useAt,
          openInput: openInput,
          isEdit: true, // If openInput && !isEdit => Create // If openInput && isEdit => Edit
          submitted: false
        })
      } else {
        return this.fb.group({
          product: [null, Validators.required],
          productId: null,
          numberOfTimes: 1, 
          amountOfTimes: 1, 
          quantity: 1, 
          unit: "Viên", 
          numberOfDays: 1, 
          useAt: 'after_meal',
          openInput: openInput,
          isEdit: false,
          submitted: false
        })
      }
    }
  }

  deleteThuocItem(i) {
    this.getThuocArray.removeAt(i);
  }

  addThuocArray() {
    if (!this.dataThuocsReceive) 
      return;
    for (let i = 0; i < this.dataThuocsReceive.length; i++) {
      this.getThuocArray.push(this.setThuocItem(this.dataThuocsReceive[i], false, null));
      this.tempThuocArray.push(null);
    }
  }

  addThuocArray_DefaultItem() {
    this.getThuocArray.insert(0, this.setThuocItem(null, true, null));
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
      // console.log(result);
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

  saveLine(i) {
    // this.submitted = true;
    this.setValueThuocArray(i, 'submitted', true);

    if (!this.getThuocArray.at(i).valid) {
      return;
    }

    this.setValueThuocArray(i, 'openInput', false);

    this.dataThuocsSend.emit(this.getThuocArray.value); //
  }
  
  cancelLine(i) {
    this.setValueThuocArray(i, 'submitted', false);
    // this.submitted = false;
    console.log(this.getThuocArray.at(i).value);
    console.log(this.tempThuocArray[i]);
    if (this.getValueThuocArray(i, 'isEdit') == false) {
      this.deleteThuocItem(i);
      this.tempThuocArray.splice(i, 1);
    } else {
      this.setThuocItem(this.tempThuocArray[i], null, i);
      this.tempThuocArray[i] = null;
    }
    this.dataThuocsSend.emit(this.getThuocArray.value); //
  }

  editLine(i) {
    this.setValueThuocArray(i, 'openInput', true);
    this.setValueThuocArray(i, 'isEdit', true);
    // this.tempThuocArray[i] = this.getThuocArray.at(i).value; 
    console.log(this.getThuocArray.at(i).value);
    console.log(this.tempThuocArray[i]);
  }

  deleteLine(i) {
    // this.lines.splice(i, 1); 
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
