import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { ServiceCardTypeService } from '../../service-card-type.service';

@Component({
  selector: 'app-card-type-apply-all-dialog',
  templateUrl: './card-type-apply-all-dialog.component.html',
  styleUrls: ['./card-type-apply-all-dialog.component.css']
})
export class CardTypeApplyAllDialogComponent implements OnInit {

  title: string = '';
  computePrice = 'percentage';
  percentPrice = 0;
  fixedAmountPrice = 0;
  cardTypeId:string;
  constructor(
    public activeModal: NgbActiveModal,
    private cardTypeService: ServiceCardTypeService,
  ) { }

  ngOnInit(): void {
  }

  onApply() {
    var res = {percentPrice: this.percentPrice, fixedAmountPrice: this.fixedAmountPrice, computePrice: this.computePrice};
    this.cardTypeService.onApplyAll(this.cardTypeId,res).subscribe(() => {
    this.activeModal.close();
    })
  }

  changePrice() {
    this.percentPrice = 0;
    this.fixedAmountPrice = 0;
  }

}
