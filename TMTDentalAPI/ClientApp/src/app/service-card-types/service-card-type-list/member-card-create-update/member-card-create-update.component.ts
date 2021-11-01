import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { result } from 'lodash';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { ServiceCardTypeService } from '../../service-card-type.service';
import { ServiceCardTypeApplyDialogComponent } from '../service-card-type-apply-dialog/service-card-type-apply-dialog.component';

@Component({
  selector: 'app-member-card-create-update',
  templateUrl: './member-card-create-update.component.html',
  styleUrls: ['./member-card-create-update.component.css']
})
export class MemberCardCreateUpdateComponent implements OnInit {
  cardTypeId: string;
  cardTypeName: string = '';
  colorSelected: number = 0;
  cardForm: FormGroup;
  productExists: any[] = [];
  objCategories = Object.create(null);
  categories: any[] = []; // nhóm dịch vụ
  submitted = false;
  categIndex = 0;
  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private cardTypeService: CardTypeService,
    private notificationService: NotificationService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.cardForm = this.fb.group({
      name: ['',Validators.required],
      basicPoint: ['',Validators.required],
      color: null,
      productPricelistItems: this.fb.array([])
    });
    this.cardTypeId = this.route.snapshot.queryParamMap.get('id');
    if (this.cardTypeId){
      this.getCardTypeById();
    }
  }

  onSave(){
    this.submitted = true;
    if (this.cardForm.invalid)
      return;
    var val = this.cardForm.value;
    val.color = this.colorSelected;
    if (this.cardTypeId){
      this.cardTypeService.update(this.cardTypeId,val).subscribe(() => {
        this.notify('Lưu thành công', 'success');
        this.cardTypeName = val.name;
      })
    }
    else {
      this.cardTypeService.createCardType(val).subscribe(result => {
        this.notify('Lưu thành công', 'success');
        this.router.navigateByUrl('/', {skipLocationChange: true}).then(() => {
          this.router.navigate(['card-types/member-cards/form'], { queryParams: { id: result.id } });
      });
      })
    }
  }

  clickColor(i){
    this.colorSelected = i;
  }

  addLine(event){
    let find = this.productExists.find(x => x == event.id);
    if (find)
      return;
    this.productExists.push(event.id);
    if (this.objCategories[event.categId]){
      this.objCategories[event.categId] = this.objCategories[event.categId]
    }
    else {
      this.objCategories[event.categId] = {categName:'',categId:'',products:[], index: this.categIndex};
    }
    this.objCategories[event.categId].categName = event.categName;
    this.objCategories[event.categId].categId = event.categId;
    event.fixedAmountPrice = null;
    event.percentPrice = null;
    event.computePrice = 'percentage';
    event.price = null;
    event.productId = event.id;
    event.defaultCode = event.defaultCode;
    event.id = '';
    event.productIndex = this.productExists.length == 0 ? 0 : this.productExists.length - 1 ;
    this.objCategories[event.categId].products.push(event);
    this.productPricelistItems.push(this.fb.group({
      productId:event.productId,
      percentPrice: null,
      fixedAmountPrice: null,
      computePrice: 'percentage',
      listPrice: event.listPrice,
      categId: event.categId
    }));
    console.log(this.cardForm);
    
    this.categories = Object.keys(this.objCategories).map((key)=> [this.objCategories[key]]); 
  }

  getCardTypeById(){
    this.cardTypeService.get(this.cardTypeId).subscribe(result => {
      this.cardForm.patchValue(result);
      this.colorSelected = result.color;
      this.cardTypeName = result.name;
      result.productPricelistItems.forEach(item => {
        this.productExists.push(item.productId);
        this.objCategories[item.product?.categId] = this.objCategories[item.product?.categId] || {categName:'',categId:'',products:[]};
        this.objCategories[item.product?.categId].categName = item.product?.categ.name;
        this.objCategories[item.product?.categId].categId = item.product?.categId;
        this.objCategories[item.product?.categId].products.push({
          name: item.product?.name,
          defaultCode: item.product?.defaultCode,
          listPrice: item.product?.listPrice,
          computePrice: item.computePrice,
          percentPrice: item.percentPrice,
          fixedAmountPrice: item.fixedAmountPrice,
          categId: item.product?.categId,
          productIndex: this.productExists.length == 0 ? 0 : this.productExists.length - 1
        });
        this.productPricelistItems.push(this.fb.group({
          productId:item.product?.id,
          computePrice: item.computePrice,
          percentPrice: item.percentPrice,
          fixedAmountPrice: item.fixedAmountPrice,
          listPrice: item.product?.listPrice,
          categId: item.product?.categId
        }));
      });
      this.categories = Object.keys(this.objCategories).map((key)=> [this.objCategories[key]]); 
    })
  }

  onApplyAll(){
    let productItems = this.getAllServices();
    let prices = productItems.map(x => x.listPrice);
    let min = Math.min(...prices);
    let modalRef = this.modalService.open(ServiceCardTypeApplyDialogComponent, 
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Áp dụng ưu đãi cho tất cả dịch vụ';
      modalRef.componentInstance.priceMin = min;
      modalRef.result.then(res => {
        if (res.computePrice == 'percentage' && res.price <= 100 || res.computePrice == 'fixed_amount' && res.price <= min){
          this.notify('Áp dụng thành công','success');
        }
        productItems.map(x => {
          x.computePrice = res.computePrice;
          x.computePrice == 'percentage' ? (x.percentPrice = res.price, x.fixedAmountPrice = null) :
          (x.fixedAmountPrice = res.price, x.percentPrice = null);
        });
        this.f.productPricelistItems.patchValue(productItems);
        this.touchedFixedAmount();
      }, () => {
    });
  }

  onApplyCateg(categId){
    let productItems = this.getAllServices();
    let productItemsFiltered = productItems.filter(x => x.categId == categId);
    let prices = productItemsFiltered.map(x => x.listPrice);
    let min = Math.min(...prices);
    let modalRef = this.modalService.open(ServiceCardTypeApplyDialogComponent, 
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Áp dụng ưu đãi cho nhóm dịch vụ';
      modalRef.componentInstance.priceMin = min;
      modalRef.result.then(res => {
        if (res.computePrice == 'percentage' && res.price <= 100 || res.computePrice == 'fixed_amount' && res.price <= min){
          this.notify('Áp dụng thành công','success');
        }
        this.productPricelistItems.controls.filter(x => x.value.categId === categId ).map(x => {
          x.get('computePrice').patchValue(res.computePrice);
          if(res.computePrice == 'percentage'){
            x.get('percentPrice').patchValue(res.price);
            x.get('fixedAmountPrice').patchValue(null);
          }else{
            x.get('percentPrice').patchValue(null);
            x.get('fixedAmountPrice').patchValue(res.price);
          }       
        });
        this.touchedFixedAmount();
      }, () => {
      });
  }

  getAllServices(){
    let productItems = this.productPricelistItems.value;
    return productItems;
  }

  notify(content: string, style){
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  getComputePrice(index){
    return this.productPricelistItems.controls[index].value.computePrice;
  }

  touchedFixedAmount() {
    (<FormArray>this.cardForm.get('productPricelistItems')).controls.forEach((group: FormGroup) => {
      (<any>Object).values(group.controls).forEach((control: FormControl) => { 
          control.markAsTouched();
      }) 
    });
    return;
  }

  get f(){
    return this.cardForm.controls;
  }

  get productPricelistItems() {
    return this.cardForm.get('productPricelistItems') as FormArray;
  }

}
