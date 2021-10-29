import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-partner-overview-member-cards',
  templateUrl: './partner-overview-member-cards.component.html',
  styleUrls: ['./partner-overview-member-cards.component.css']
})
export class PartnerOverviewMemberCardsComponent implements OnInit {
  @Input() memberCard: any;
  @Input() partnerId: string;
  constructor() { }

  ngOnInit(): void {
    console.log(this.memberCard);
    
  }

  createCard(){

  }

  getState(state){
    switch (state){
      case 'draft':
        return 'Nháp';
      case 'in_use':
        return 'Đã kích hoạt';
      default:
        return '';   
    }
  }

  getStateClass(state){
    switch (state){
      case 'draft':
        return 'badge badge-secondary';
      case 'in_use':
        return 'badge badge-primary';
      default:
        return '';   
    }
  }

}
