import { Component, OnInit, ViewChild } from '@angular/core';
import { ProductService } from '../product.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-product-labo-cu-dialog',
  templateUrl: './product-labo-cu-dialog.component.html',
  styleUrls: ['./product-labo-cu-dialog.component.css']
})
export class ProductLaboCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  title: string;

  constructor(private productService: ProductService, public activeModal: NgbActiveModal, private fb: FormBuilder) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      purchasePrice: 0,

    });

    if (this.id) {
      setTimeout(() => {
        this.productService.getLabo(this.id).subscribe(result => {
          this.formGroup.patchValue(result);
        });
      });
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    if (this.id) {
      this.productService.updateLabo(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      });
    } else {
      return this.productService.createLabo(val).subscribe(result => {
        this.activeModal.close(result);
      });;
    }
  }
}

