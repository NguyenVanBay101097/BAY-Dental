import { Component, Input, OnInit } from '@angular/core';
import { PromotionProgramBasic } from 'src/app/promotion-programs/promotion-program.service';

@Component({
  selector: 'app-partner-overview-promotion',
  templateUrl: './partner-overview-promotion.component.html',
  styleUrls: ['./partner-overview-promotion.component.css']
})
export class PartnerOverviewPromotionComponent implements OnInit {

  @Input() promotions: PromotionProgramBasic;
  constructor() { }

  ngOnInit() {
  }

}
