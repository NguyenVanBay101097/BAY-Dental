import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { ServiceCardTypeApplyDialogComponent } from '../service-card-type-apply-dialog/service-card-type-apply-dialog.component';

@Component({
  selector: 'app-member-card-create-update',
  templateUrl: './member-card-create-update.component.html',
  styleUrls: ['./member-card-create-update.component.css']
})
export class MemberCardCreateUpdateComponent implements OnInit {
  cardTypeId: string;
  cardTypeName: string = '';
  productExists: any[] = [];
  submitted = false;
  companyId: string;
  cardForm: FormGroup;
  cateDict: { [id: string]: any; } = {};
  colorSelected = 0;

  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private cardTypeService: CardTypeService,
    private notificationService: NotificationService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit(): void {
    var user_info = localStorage.getItem('user_info');
    if (user_info) {
      var userInfo = JSON.parse(user_info);
      this.companyId = userInfo.companyId;
    }

    this.cardForm = this.fb.group({
      name: ['', Validators.required],
      basicPoint: ['', Validators.required],
      color: null,
      companyId: this.companyId,
      productPricelistItems: this.fb.array([])
    });
    this.cardTypeId = this.route.snapshot.queryParamMap.get('id');
    if (this.cardTypeId) {
      this.getCardTypeById();
    }
  }

  onSave() {
    this.submitted = true;
    if (this.cardForm.invalid)
      return;
    var formValue = this.cardForm.value;
    var val = {
      name: formValue.name,
      basicPoint: formValue.basicPoint,
      color: this.colorSelected,
      companyId: this.companyId,
      productPricelistItems: formValue.productPricelistItems.flatMap(x => x.products).map(x => {
        return {
          id: x.id,
          productId: x.product.id,
          computePrice: x.computePrice,
          percentPrice: x.percentPrice,
          fixedAmountPrice: x.fixedAmountPrice,
        }
      })
    };

    if (this.cardTypeId) {
      this.cardTypeService.update(this.cardTypeId, val).subscribe(() => {
        this.notify('Lưu thành công', 'success');
        this.cardTypeName = val.name;
        this.getCardTypeById();
      })
    }
    else {
      this.cardTypeService.createCardType(val).subscribe(result => {
        this.notify('Lưu thành công', 'success');
        this.cardTypeId = result.id;
        this.router.navigate([], { queryParams: { id: result.id }, relativeTo: this.route });
        this.getCardTypeById();
      })
    }
  }

  deleteItem(cateIndex, proIndex) {
    var proFA = ((this.productPricelistItems.controls[cateIndex] as FormGroup).controls.products as FormArray);
    proFA.removeAt(proIndex);
    if (!proFA.length) {
      this.productPricelistItems.removeAt(cateIndex);
    }
  }

  clickColor(i) {
    this.colorSelected = i;
  }

  get productExistValue() {
    return this.cardForm.value.productPricelistItems.flatMap(x => x.products) as any[];
  }

  addLine(event) {
    let index = this.productExistValue.findIndex(x => x.product.id == event.id);
    if (index >= 0)
      return;

    var item = {
      product: event,
      computePrice: 'percentage',
      percentPrice: [0, Validators.required],
      fixedAmountPrice: [0, Validators.required],
    };

    var i = this.productPricelistItems.controls.findIndex(x => x.get('id').value == event.categId);

    if (i !== -1) {
      var productFormArray = this.productPricelistItems.at(i).get('products') as FormArray;
      productFormArray.push(this.fb.group(item));
    } else {
      var group = this.fb.group({
        id: event.categId,
        name: event.categName,
        products: this.fb.array([])
      });

      var productFormArray = group.get('products') as FormArray;
      productFormArray.push(this.fb.group(item));
      this.productPricelistItems.push(group);
    }
  }

  getCardTypeById() {
    this.cardTypeService.get(this.cardTypeId).subscribe(result => {
      this.cardForm.patchValue(result);
      this.colorSelected = result.color;
      this.cardTypeName = result.name;
      this.productPricelistItems.clear();

      result.productPricelistItems.forEach((item, index) => {
        this.productExists.push(item.productId);
        var productItem = {
          id: item.id,
          product: item.product,
          computePrice: [item.computePrice, Validators.required],
          percentPrice: [item.percentPrice, Validators.required],
          fixedAmountPrice: [item.fixedAmountPrice, Validators.required],
        };

        var product = item.product;
        var i = this.productPricelistItems.controls.findIndex(x => x.get('id').value == product.categId);
        if (i !== -1) {
          var productFormArray = this.productPricelistItems.at(i).get('products') as FormArray;
          productFormArray.push(this.fb.group(item));
        } else {
          var group = this.fb.group({
            id: product.categId,
            name: product.categName,
            products: this.fb.array([])
          });

          var productFormArray = group.get('products') as FormArray;
          productFormArray.push(this.fb.group(productItem));
          this.productPricelistItems.push(group);
        }
      });
    })
  }

  onApplyAll() {
    let modalRef = this.modalService.open(ServiceCardTypeApplyDialogComponent,
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Áp dụng ưu đãi cho tất cả dịch vụ';
    modalRef.result.then(res => {
      this.productPricelistItems.controls.forEach((productControl, index) => {
        var productsFormArray = this.productPricelistItems.at(index).get('products') as FormArray;
        productsFormArray.controls.forEach(control => {
          control.get('computePrice').setValue(res.computePrice);
          if (res.computePrice == 'percentage') {
            control.get('percentPrice').setValue(res.price);
            control.get('fixedAmountPrice').setValue(0);
          } else {
            control.get('percentPrice').setValue(0);
            control.get('fixedAmountPrice').setValue(res.price);
          }
        });
        this.touchedFixedAmount();

      })
    }, () => {
    });
  }

  onApplyCateg(index) {
    let modalRef = this.modalService.open(ServiceCardTypeApplyDialogComponent,
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Áp dụng ưu đãi cho nhóm dịch vụ';
    modalRef.result.then(res => {
      var productsFormArray = this.productPricelistItems.at(index).get('products') as FormArray;
      productsFormArray.controls.forEach(control => {
        control.get('computePrice').setValue(res.computePrice);
        if (res.computePrice == 'percentage') {
          control.get('percentPrice').setValue(res.price);
          control.get('fixedAmountPrice').setValue(0);
        } else {
          control.get('percentPrice').setValue(0);
          control.get('fixedAmountPrice').setValue(res.price);
        }
      });
      this.touchedFixedAmount();
    }, () => {
    });
  }

  getAllServices() {
    let productItems = this.productPricelistItems.value;
    return productItems;
  }

  notify(content: string, style) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  getComputePrice(index) {
    return this.productPricelistItems.controls[index].value.computePrice;
  }

  changePeriod(categIndex, productIndex) {
    var productFormArray = this.productPricelistItems.at(categIndex).get('products') as FormArray;
    if (productFormArray.at(productIndex).get('computePrice').value === 'fixed_amount') {
      productFormArray.at(productIndex).get('percentPrice').setValue(0);
      productFormArray.at(productIndex).get('fixedAmountPrice').setValidators([Validators.required]);
      productFormArray.at(productIndex).get('fixedAmountPrice').updateValueAndValidity();
    } else {
      productFormArray.at(productIndex).get('fixedAmountPrice').setValue(0);
      productFormArray.at(productIndex).get('percentPrice').setValidators([Validators.required]);
      productFormArray.at(productIndex).get('percentPrice').updateValueAndValidity();
    }
  }

  touchedFixedAmount() {
    (<FormArray>this.cardForm.get('productPricelistItems')).controls.forEach((proControl, index) => {
      var productsFormArray = this.productPricelistItems.at(index).get('products') as FormArray;
      productsFormArray.controls.forEach((group: FormGroup) => {
        (<any>Object).values(group.controls).forEach((control: FormControl) => {
          control.markAsTouched();
        })
      })
    });
    return;
  }

  get f() {
    return this.cardForm.controls;
  }

  get productPricelistItems() {
    return this.cardForm.get('productPricelistItems') as FormArray;
  }

}
