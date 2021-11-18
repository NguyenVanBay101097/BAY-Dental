import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { isThisSecond } from 'date-fns';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ServiceCardTypeService } from '../../service-card-type.service';

@Component({
  selector: 'app-service-card-type-apply-all-dialog',
  templateUrl: './service-card-type-apply-all-dialog.component.html',
  styleUrls: ['./service-card-type-apply-all-dialog.component.css']
})
export class ServiceCardTypeApplyAllDialogComponent implements OnInit {

  title: string = '';
  computePrice = 'percentage';
  percentPrice = 0;
  fixedAmountPrice = 0;
  cardTypeId:string;
  constructor(
    public activeModal: NgbActiveModal,
    private cardTypeService: ServiceCardTypeService,
    private notifyService: NotifyService,


  ) { }

  ngOnInit(): void {
  }

  onApply() {
    var res = {id:this.cardTypeId, percentPrice: this.percentPrice, fixedAmountPrice: this.fixedAmountPrice, computePrice: this.computePrice};
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
