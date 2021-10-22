import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import {ProductPricelistItems, ServiceCardTypeObj, ServiceCardTypeService } from '../../service-card-type.service';
import { ServiceCardTypeApplyDialogComponent } from '../service-card-type-apply-dialog/service-card-type-apply-dialog.component';

@Component({
  selector: 'app-preferential-card-create-update',
  templateUrl: './preferential-card-create-update.component.html',
  styleUrls: ['./preferential-card-create-update.component.css']
})
export class PreferentialCardCreateUpdateComponent implements OnInit {
  cardId: string;
  cardName: string = '';
  periodList = [1,2,3,4,5,6,7,8,9,11,12];
  products: any[] = []; // danh sách dịch vụ
  categories: any[] = []; // nhóm dịch vụ
  productExists: string[] = [];
  cardTypeObj: ServiceCardTypeObj;
  objCategories = Object.create(null);
  submitted = false;
  constructor(
    private modalService: NgbModal,
    private cardService: ServiceCardTypeService,
    private notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    this.cardTypeObj  = {
      name: '',
      period: 'year',
      nbrPeriod: 1,
      productPricelistItems: []
    };
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
    this.cardService.create(this.cardTypeObj).subscribe(()=>{
      this.notificationService.show({
        content: 'Lưu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    })
  }
  onApplyAll(){
    let result: any;
    let productItems = this.categories.reduce((r,a) => {
      return r.concat(a[0].products);
    },[]);
    let modalRef = this.modalService.open(ServiceCardTypeApplyDialogComponent, 
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Áp dụng ưu đãi cho tất cả dịch vụ';
      modalRef.result.then(res => {
        productItems.map(x => {
          x.computePrice = res.computePrice;
          x.computePrice == 'percentage' ? x.percentPrice = res.price : x.fixedAmountPrice = res.price;
        })
      }, () => {
    });
  }

  onApplyCateg(categId){
    let modalRef = this.modalService.open(ServiceCardTypeApplyDialogComponent, 
      { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Áp dụng ưu đãi cho nhóm dịch vụ';
      modalRef.result.then(res => {
        this.cardTypeObj.productPricelistItems.filter(x => x.categId == categId).map(x => {
          x.computePrice = res.computePrice;
          x.computePrice == 'percentage' ? x.percentPrice = res.price : x.fixedAmountPrice = res.price;
        })
      }, () => {
      });

      
  }

  applyAll(res){
    this.cardTypeObj.productPricelistItems.forEach(x => {
      console.log(x);
      
      x.computePrice = res.computePrice;
      x.computePrice == 'percentage' ? x.percentPrice = res.price : x.fixedAmountPrice = res.price;
    })
  }

}
