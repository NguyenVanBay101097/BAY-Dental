import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import {ProductPricelistItems, ServiceCardTypeObj, ServiceCardTypeService } from '../../service-card-type.service';
import { ServiceCardTypeApplyDialogComponent } from '../service-card-type-apply-dialog/service-card-type-apply-dialog.component';

@Component({
  selector: 'app-preferential-card-create-update',
  templateUrl: './preferential-card-create-update.component.html',
  styleUrls: ['./preferential-card-create-update.component.css']
})
export class PreferentialCardCreateUpdateComponent implements OnInit {
  cardTypeId: string;
  cardTypeName: string = '';
  periodList = [1,2,3,4,5,6,7,8,9,11,12];
  products: any[] = []; // danh sách dịch vụ
  categories: any[] = []; // nhóm dịch vụ
  productExists: string[] = [];
  cardTypeObj: ServiceCardTypeObj;
  objCategories = Object.create(null);
  submitted = false;
  companyId: string;
  constructor(
    private modalService: NgbModal,
    private cardService: ServiceCardTypeService,
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
    this.cardTypeObj  = {
      name: '',
      period: 'year',
      nbrPeriod: 1,
      companyId: this.companyId,
      productPricelistItems: []
    };  
    this.cardTypeId = this.route.snapshot.queryParamMap.get('id');
    if (this.cardTypeId){
      this.getCardTypeById();
    }
  }

  getCardTypeById(){
    this.cardService.get(this.cardTypeId).subscribe(result => {
      this.cardTypeName = result.name;
      this.cardTypeObj.name = result.name;
      this.cardTypeObj.period = result.period;
      this.cardTypeObj.nbrPeriod = result.nbrPeriod;
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
          categId: item.product?.categId
        });
      });
      this.categories = Object.keys(this.objCategories).map((key)=> [this.objCategories[key]]); 
    })
  }

  addLine(event){
    let find = this.productExists.find(x => x == event.id);
    if (find)
      return;
    this.productExists.push(event.id);
    this.objCategories[event.categId] = this.objCategories[event.categId] || {categName:'',categId:'',products:[]};
    this.objCategories[event.categId].categName = event.categName;
    this.objCategories[event.categId].categId = event.categId;
    event.fixedAmountPrice = null;
    event.percentPrice = null;
    event.computePrice = 'percentage';
    event.price = null;
    event.productId = event.id;
    event.defaultCode = event.defaultCode;
    event.id = '';
    this.objCategories[event.categId].products.push(event);
    this.categories = Object.keys(this.objCategories).map((key)=> [this.objCategories[key]]); 
  }
  onSave(){
    if (this.cardTypeObj.name == '')
      return;
  
    let productItems = this.categories.reduce((r,a) => {
      return r.concat(a[0].products);
    },[]);
    this.cardTypeObj.productPricelistItems = productItems;

    if (this.cardTypeId){
      this.cardService.update(this.cardTypeId,this.cardTypeObj).subscribe(() => {
        this.notify('Lưu thành công','success');
      })
    }
    else {
      this.cardService.create(this.cardTypeObj).subscribe(result=>{
        this.router.navigateByUrl('/', {skipLocationChange: true}).then(() => {
          this.router.navigate(['card-types/preferential-cards/form'], { queryParams: { id: result.id } });
      });
        this.notify('Lưu thành công','success');
      })
    }
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

  onApplyAll(){
    let productItems = this.getAllServices();
    let prices = productItems.map(x => x.listPrice);
    let min = Math.min(...prices);
    let modalRef = this.modalService.open(ServiceCardTypeApplyDialogComponent, 
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Áp dụng ưu đãi cho tất cả dịch vụ';
      modalRef.componentInstance.priceMin = min;
      modalRef.result.then(res => {
        if (res.computePrice == 'percentage' && res.price > 100){
          this.notify('Ưu đãi vượt quá giá bán','error');
          return;
        }
        if (res.computePrice == 'fixed_amount' && res.price > min){
          this.notify('Ưu đãi vượt quá giá bán','error');
          return;
        }
        productItems.map(x => {
          x.computePrice = res.computePrice;
          x.computePrice == 'percentage' ? (x.percentPrice = res.price, x.fixedAmountPrice = null) :
          (x.fixedAmountPrice = res.price, x.percentPrice = null);
        });
        this.notify('Áp dụng thành công','success');
      }, () => {
    });
  }

  onApplyCateg(categId){
    let productItems = this.getAllServices();
    productItems = productItems.filter(x => x.categId == categId);
    let prices = productItems.map(x => x.listPrice);
    let min = Math.min(...prices);
    let modalRef = this.modalService.open(ServiceCardTypeApplyDialogComponent, 
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Áp dụng ưu đãi cho nhóm dịch vụ';
      modalRef.componentInstance.priceMin = min;
      modalRef.result.then(res => {
        if (res.computePrice == 'percentage' && res.price > 100){
          this.notify('Ưu đãi vượt quá giá bán','error');
          return;
        }
        if (res.computePrice == 'fixed_amount' && res.price > min){
          this.notify('Ưu đãi vượt quá giá bán','error');
          return;
        }
        productItems.map(x => {
          x.computePrice = res.computePrice;
          x.computePrice == 'percentage' ? (x.percentPrice = res.price, x.fixedAmountPrice = null) :
          (x.fixedAmountPrice = res.price, x.percentPrice = null);
        });
        this.notify('Áp dụng thành công','success');
      }, () => {
      });
  }

  getAllServices(){
    let productItems = this.categories.reduce((r,a) => {
      return r.concat(a[0].products);
    },[]);
    return productItems;
  }

  resetForm(){
    this.cardTypeObj  = {
      name: '',
      period: 'year',
      nbrPeriod: 1,
      productPricelistItems: [],
      companyId: this.companyId
    };
    this.categories = [];
  }

  changePeriod(product){
    product.percentPrice = null;
    product.fixedAmountPrice = null;
  }

}
