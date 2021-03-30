import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-tooth-diagnosis-list',
  templateUrl: './tooth-diagnosis-list.component.html',
  styleUrls: ['./tooth-diagnosis-list.component.css']
})
export class ToothDiagnosisListComponent implements OnInit {

  searchUpdate = new Subject<string>();

  constructor() { }

  ngOnInit() {
  }

  createItem(){

  }

  editItem(item:any){

  }

  deleteItem(item:any){
    
  }
}
