import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-preferential-card-create-update',
  templateUrl: './preferential-card-create-update.component.html',
  styleUrls: ['./preferential-card-create-update.component.css']
})
export class PreferentialCardCreateUpdateComponent implements OnInit {
  cardId: string;
  cardName: string = '';
  periodList = [1,2,3,4,5,6,7,8,9,11,12];
  search: string = '';
  searchUpdate = new Subject<string>();
  saleOffType: string = 'percentage';
  products: any[] = [];
  categories: any[] = [];
  constructor() { }

  ngOnInit(): void {
  }

  addLine(event){
    console.log(event);
    this.products.push(event);
    this.groupbyProduct();
    console.log(this.categories);
    
  }

  groupbyProduct() {
    let data = null;
    data = this.products.reduce((r, a) => {
      const categId = a.categId;
      r[categId] = r[categId] || {categName:'',products:[]};
      // r[categId].push(a);
      r[categId].categName = a.categName;
      r[categId].products.push({id: a.id, name: a.name, listPrice: a.listPrice})
      return r;
    }, Object.create(null));  
    // return data;
    // var result = Object.keys(obj).map((key) => [Number(key), obj[key]]);
    this.categories = Object.keys(data).map((key)=> [data[key]]);    
  }

}
