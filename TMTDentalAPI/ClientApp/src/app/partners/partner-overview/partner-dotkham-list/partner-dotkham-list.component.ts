import { Component, Input, OnInit } from '@angular/core';
import { DotKhamBasic } from 'src/app/dot-khams/dot-khams';

@Component({
  selector: 'app-partner-dotkham-list',
  templateUrl: './partner-dotkham-list.component.html',
  styleUrls: ['./partner-dotkham-list.component.css']
})
export class PartnerDotkhamListComponent implements OnInit {

  constructor() { }
  
  @Input() dotkhams: DotKhamBasic[] = [];

  ngOnInit() {
  }

}
