import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ServiceCardTypeService } from '../../service-card-type.service';

@Component({
  selector: 'app-member-card-type-apply-all',
  templateUrl: './member-card-type-apply-all.component.html',
  styleUrls: ['./member-card-type-apply-all.component.css']
})
export class MemberCardTypeApplyAllComponent implements OnInit {
  title: string = '';
  computePrice = 'percentage';
  percentPrice = 0;
  fixedAmountPrice = 0;
  cardTypeId:string;
  constructor(
    public activeModal: NgbActiveModal,
    private cardTypeService: CardTypeService,
    private notifyService: NotifyService
  ) { }

  ngOnInit(): void {
  }

  onApply(form) {
    if (form.invalid)
      return;
    var res = {percentPrice: this.percentPrice, fixedAmountPrice: this.fixedAmountPrice, computePrice: this.computePrice};
    this.cardTypeService.onApplyAll(this.cardTypeId,res).subscribe(() => {
    this.activeModal.close();
    this.notifyService.notify('success','Lưu thành công');
    })
  }

  changePrice() {
    this.percentPrice = 0;
    this.fixedAmountPrice = 0;

  }

}
