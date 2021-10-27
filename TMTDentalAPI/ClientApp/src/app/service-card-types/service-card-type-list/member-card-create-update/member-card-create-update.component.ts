import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

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

  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.cardForm = this.fb.group({
      name: ['',Validators.required],
      basicPoint: ['',Validators.required],
      color: null,
      productPricelistItems: null
    });
  }

  onSave(){
    let productItems = this.categories.reduce((r,a) => {
      return r.concat(a[0].products);
    },[]);
    console.log(productItems);
    
  }

  clickColor(i){
    this.colorSelected = i;
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

  onApplyAll(){

  }

  onApplyCateg(categId){

  }

  changeComputePrice(event,product){
    console.log(event);
    console.log(product);
    
    
  }

}
