import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-res-insurance-histories-detail',
  templateUrl: './res-insurance-histories-detail.component.html',
  styleUrls: ['./res-insurance-histories-detail.component.css']
})
export class ResInsuranceHistoriesDetailComponent implements OnInit {
  @Input() paymentId: string;
  gridData: GridDataResult;

  constructor() { }

  ngOnInit(): void {
    console.log(this.paymentId);
  }

}
