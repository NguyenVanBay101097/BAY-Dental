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
  constructor() { }

  ngOnInit(): void {
  }

  addLine(event){
    console.log(event);
    
  }

}
