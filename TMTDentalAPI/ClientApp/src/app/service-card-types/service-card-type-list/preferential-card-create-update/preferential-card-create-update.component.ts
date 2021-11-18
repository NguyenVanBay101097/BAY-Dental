import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductPriceListItemsService } from '../../product-price-list-items.service';
import { ServiceCardTypeService } from '../../service-card-type.service';
import { ServiceCardTypeApplyAllDialogComponent } from '../card-type-apply-all-dialog/service-card-type-apply-all-dialog.component';
import { ServiceCardTypeApplyCateDialogComponent } from '../card-type-apply-cate-dialog/service-card-type-apply-cate-dialog.component';

@Component({
  selector: 'app-preferential-card-create-update',
  templateUrl: './preferential-card-create-update.component.html',
  styleUrls: ['./preferential-card-create-update.component.css'],
})
export class PreferentialCardCreateUpdateComponent implements OnInit {
  cardTypeId: string;
  cardTypeName: string = '';
  productExists: any[] = [];
  submitted = false;
  companyId: string;
  cardForm: FormGroup;
  cateDict: { [id: string]: any; } = {};

  constructor(
    private modalService: NgbModal,
    private cardTypeService: ServiceCardTypeService,
    private notificationService: NotificationService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private productPriceListItemService: ProductPriceListItemsService

  ) { }

  ngOnInit(): void {
    var user_info = localStorage.getItem('user_info');
    if (user_info) {
      var userInfo = JSON.parse(user_info);
      this.companyId = userInfo.companyId;
    }
    this.cardForm = this.fb.group({
      name: ['', Validators.required],
      period: 'year',
      nbrPeriod: [1, Validators.required],
      companyId: this.companyId,
      productPricelistItems: this.fb.array([])
    });
    this.cardTypeId = this.route.snapshot.queryParamMap.get('id');
    if (this.cardTypeId) {
      this.getCardTypeById();
    }
  }

  getCardTypeById() {
    this.cardTypeService.get(this.cardTypeId).subscribe(result => {
      this.cardForm.patchValue(result);
      this.cardTypeName = result.name;
      this.productPricelistItems.clear();

      result.productPricelistItems.forEach((item, index) => {
        this.productExists.push(item.productId);
        var productItem = {
          id: item.id,
          product: item.product,
          computePrice: [item.computePrice,Validators.required],
          percentPrice: [item.percentPrice,Validators.required],
          fixedAmountPrice: [(item.fixedAmountPrice ?? 0), Validators.required],
        };
        
        var product = item.product;
        var i = this.productPricelistItems.controls.findIndex(x => x.get('id').value == product.categId);
        if (i !== -1) {
          var productFormArray = this.productPricelistItems.at(i).get('products') as FormArray;
          productFormArray.push(this.fb.group(item));
        } else {
          var group = this.fb.group({
            id: product.categId,
            name: product.categ.name,
            products: this.fb.array([])
          });
    
          var productFormArray = group.get('products') as FormArray;
          productFormArray.push(this.fb.group(productItem));
          this.productPricelistItems.push(group);
        }
      });
    })
  }

  get productExistValue() {
    return this.cardForm.value.productPricelistItems.flatMap(x => x.products) as any[];
  }

  onAddLine(product){
    let index = this.productExistValue.findIndex(x => x.product.id == product.id);
    if (index >= 0)
      return;
    var item = {
      id: '',
      product: product,
      computePrice: 'percentage',
      percentPrice: [0, Validators.required],
      fixedAmountPrice: [0, Validators.required],
    };
  
    var val = {
      id: this.cardTypeId,
      ProductIds: [product.id]
    }
    this.cardTypeService.addProductPricelistItem(this.cardTypeId,val).subscribe((res: any) => {
      item.id = res[0].id;
      this.pushProduct(product,item);
    })
  }
    addLine(product) {
      this.submitted = true;
      if (this.cardForm.invalid)
        return;
      var val = this.getFormValueSave();
  
      if (!this.cardTypeId) {
        this.cardTypeService.create(val).subscribe(result => {
          this.cardTypeId = result.id;
          this.router.navigate([], { queryParams: { id: result.id }, relativeTo: this.route });
          this.onAddLine(product);
        })
      } else {
        this.onAddLine(product);
      }
    }

  pushProduct(event, item) {
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

  getFormValueSave() {
    var formValue = this.cardForm.value;
    var val = {
      name: formValue.name,
      period: formValue.period,
      nbrPeriod: formValue.nbrPeriod,
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
    return val;
  }

  onSave() {
    this.submitted = true;
    if (this.cardForm.invalid)
      return;
    
    var val = this.getFormValueSave();

    if (this.cardTypeId) {
      this.cardTypeService.updateCardType(this.cardTypeId, val).subscribe(() => {
        this.notify('Lưu thành công', 'success');
        this.cardTypeName = val.name;
        this.getCardTypeById();
      })
    }
    else {
      this.cardTypeService.create(val).subscribe(result => {
        this.notify('Lưu thành công', 'success');
        this.cardTypeId = result.id;
        this.router.navigate([], { queryParams: { id: result.id }, relativeTo: this.route });
        this.getCardTypeById();
      })
    }
  }

  deleteItem(cateIndex, proIndex) {
    var proFA = ((this.productPricelistItems.controls[cateIndex] as FormGroup).controls.products as FormArray);
    var id = proFA.controls[proIndex].value.id;
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa dịch vụ';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.productPriceListItemService.delete(id).subscribe(() => {
        this.notify('Xóa thành công','success');
        proFA.removeAt(proIndex);
        if (!proFA.length) {
          this.productPricelistItems.removeAt(cateIndex);
        }
      })
    });
  }

  apply(event,categIdex,proIndex) {
    var proFA = ((this.productPricelistItems.controls[categIdex] as FormGroup).controls.products as FormArray);
    var pro = proFA.controls[proIndex];
    var proId = pro.value.product.id;
    var proObj = {
      id: pro.value.id,
      productId: proId,
      computePrice: pro.value.computePrice,
      fixedAmountPrice: pro.value.fixedAmountPrice,
      percentPrice: pro.value.percentPrice
    };
    
    var onApply = ()=> {
      var val = {
        id: this.cardTypeId,
        ProductListItems : [proObj]
      };
      this.cardTypeService.updateProductPricelistItem(this.cardTypeId,val).subscribe(() => {
        this.notify('Lưu thành công','success');
        pro.get('computePrice').setValue(event.computePrice);
        pro.get('fixedAmountPrice').setValue(event.fixedAmountPrice);
        pro.get('percentPrice').setValue(event.percentPrice);
      })
    }
   
    if (!this.cardTypeId) {
      var formValue = this.cardForm.value;
      this.cardTypeService.create(formValue).subscribe(result => {
        this.cardTypeId = result.id;
        onApply();
      })
    }
    else {
      onApply();
    }
  }

  getPriceObj(group) {
    var obj = {
      productId: group.get('product').value.id,
      computePrice: group.get('computePrice').value,
      fixedAmountPrice: group.get('fixedAmountPrice').value,
      percentPrice: group.get('percentPrice').value,
    }
    return obj;
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

  onOpenApplyAll(){
    let modalRef = this.modalService.open(ServiceCardTypeApplyAllDialogComponent,
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Áp dụng ưu đãi cho tất cả dịch vụ';
    modalRef.componentInstance.cardTypeId = this.cardTypeId;
    modalRef.result.then(res => {
      this.getCardTypeById();
    }, () => {
    });
  }

  onOpenApplyCateg(){
    let modalRef = this.modalService.open(ServiceCardTypeApplyCateDialogComponent,
      { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Áp dụng ưu đãi cho nhóm dịch vụ';
    modalRef.componentInstance.cardTypeId = this.cardTypeId;
    modalRef.result.then(res => {
      this.getCardTypeById();
    }, () => {
    });
  }

  onApplyAll() {
    this.submitted = true;
    if (this.cardForm.invalid)
      return;
    var val = this.getFormValueSave();
    if (!this.cardTypeId) {
      this.cardTypeService.create(val).subscribe(result => {
        this.cardTypeId = result.id;
        this.cardTypeName = val.name;
        this.router.navigate([], { queryParams: { id: result.id }, relativeTo: this.route });
        this.onOpenApplyAll();
      })
    } else {
      this.onOpenApplyAll();
    }
  }

  onApplyCateg() {
    this.submitted = true;
    if (this.cardForm.invalid)
      return;
    var val = this.getFormValueSave();

    if (!this.cardTypeId) {
      this.cardTypeService.create(val).subscribe(result => {
        this.cardTypeId = result.id;
        this.cardTypeName = val.name;
        this.router.navigate([], { queryParams: { id: result.id }, relativeTo: this.route });
        this.onOpenApplyCateg();
      })
    } else {
      this.onOpenApplyCateg();
    }
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
    (<FormArray>this.cardForm.get('productPricelistItems')).controls.forEach((proControl,index)=> {
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
