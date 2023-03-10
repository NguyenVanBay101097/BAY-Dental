import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';
import { ServiceCardCardService } from 'src/app/service-card-cards/service-card-card.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { SaleOrderService } from '../../core/services/sale-order.service';

@Component({
  selector: 'app-sale-order-apply-service-cards-dialog',
  templateUrl: './sale-order-apply-service-cards-dialog.component.html',
  styleUrls: ['./sale-order-apply-service-cards-dialog.component.css']
})
export class SaleOrderApplyServiceCardsDialogComponent implements OnInit {
  amountTotal: number;
  orderId: string;

  cards: any[] = [];

  checkCodeFormGroup: FormGroup;
  applyFormGroup: FormGroup;
  @ViewChild('codeInput', { static: true }) codeInput: ElementRef;

  constructor(
    private saleOrderService: SaleOrderService, 
    public activeModal: NgbActiveModal,
    private errorService: AppSharedShowErrorService,
    private fb: FormBuilder,
    private cardService: ServiceCardCardService) { }

  ngOnInit() {
    this.checkCodeFormGroup = this.fb.group({
      code: [null, Validators.required]
    });

    this.applyFormGroup = this.fb.group({
      amount: [0, Validators.required]
    });
  }

  checkCode() {
    if (!this.checkCodeFormGroup.valid) {
      return false;
    }

    var value = this.checkCodeFormGroup.value;

    this.cardService.checkCode(value).subscribe((result: any) => {
      var index = _.findIndex(this.cards, x => x.id == result.id);
      if (index === -1) {
        this.cards.push(result);
      }

      this.checkCodeFormGroup.get('code').setValue('');
      this.codeInput.nativeElement.focus();

      this.suggestSetAmount();
    }, err => {
    });
  }

  removeCard(index) {
    this.cards.splice(index, 1);
    this.suggestSetAmount();
  }

  suggestSetAmount() {
    var total = _.sumBy(this.cards, x => x.residual);
    var amount = Math.min(total, this.amountTotal);
    this.applyFormGroup.get('amount').setValue(amount);
  }

  onSave() {
    if (!this.cards.length) {
      alert('Vui l??ng qu??t t???i thi???u 1 th???');
      return false;
    }

    if (!this.applyFormGroup.valid) {
      return false;
    }

    var val = {
      id: this.orderId,
      cardIds: this.cards.map(x => x.id),
      amount: this.applyFormGroup.get('amount').value
    };

    this.saleOrderService.applyServiceCards(val).subscribe(() => {
      this.activeModal.close(true);
    }, (error) => {
      this.errorService.show(error);
    });
  }
}

