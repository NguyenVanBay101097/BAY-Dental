import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CardCardService } from 'src/app/card-cards/card-card.service';
import { CardCardsMemberCreateDialogComponent } from 'src/app/service-card-cards/card-cards-member-create-dialog/card-cards-member-create-dialog.component';

@Component({
  selector: 'app-partner-overview-member-cards',
  templateUrl: './partner-overview-member-cards.component.html',
  styleUrls: ['./partner-overview-member-cards.component.css']
})
export class PartnerOverviewMemberCardsComponent implements OnInit {
  @Input() memberCard: any;
  @Input() partnerId: string;
  constructor(
    private modalService: NgbModal,
    private cardService: CardCardService

  ) { }

  ngOnInit(): void {
  }

  createCard(){
    const modalRef = this.modalService.open(CardCardsMemberCreateDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Tạo thẻ thành viên ";
    modalRef.componentInstance.partnerId = this.partnerId;

    modalRef.result.then(result => {
      this.getNextMemberCard();
    }, () => { });
  }

  getNextMemberCard(){
    let val = {partnerId: this.partnerId};
    this.cardService.getCardCards(val).subscribe((res: any[]) => {
      this.memberCard = res[0];
    })
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
