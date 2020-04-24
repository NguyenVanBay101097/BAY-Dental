import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SaleOrderService } from '../sale-order.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { PartnerService } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-sale-order-apply-service-cards-dialog',
  templateUrl: './sale-order-apply-service-cards-dialog.component.html',
  styleUrls: ['./sale-order-apply-service-cards-dialog.component.css']
})
export class SaleOrderApplyServiceCardsDialogComponent implements OnInit {
  orderId: string;
  partnerId: string;

  cards: any[] = [];
  selectedCards: any[] = [];
  constructor(private saleOrderService: SaleOrderService, public activeModal: NgbActiveModal,
    private errorService: AppSharedShowErrorService,
    private partnerService: PartnerService) { }

  ngOnInit() {
    this.loadValidCards();
  }

  loadValidCards() {
    this.partnerService.getValidServiceCards(this.partnerId).subscribe((result: any) => {
      this.cards = result;
      this.cards.forEach(card => {
        this.onCardClick(card);
      });
    });
  }

  onCardClick(card) {
    var index = this.selectedCards.indexOf(card);
    if (index != -1) {
      this.selectedCards.splice(index, 1);
    } else {
      this.selectedCards.push(card);
    }
  }

  onSave() {
    if (!this.selectedCards.length) {
      alert('Vui lòng chọn tối thiểu 1 thẻ');
      return false;
    }

    var val = {
      id: this.orderId,
      cardIds: this.selectedCards.map(x => x.id)
    };

    this.saleOrderService.applyServiceCards(val).subscribe(() => {
      this.activeModal.close(true);
    }, (error) => {
      this.errorService.show(error);
    });
  }
}

