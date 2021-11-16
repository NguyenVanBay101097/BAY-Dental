import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-service-card-type-apply-all',
  templateUrl: './service-card-type-apply-all.component.html',
  styleUrls: ['./service-card-type-apply-all.component.css']
})
export class ServiceCardTypeApplyAllComponent implements OnInit {
  title: string = '';
  computePrice = 'percentage';
  percentPrice = 0;
  fixedAmountPrice = 0;
  constructor(
    public activeModal: NgbActiveModal,

  ) { }

  ngOnInit(): void {
  }

  onApply() {
    this.activeModal.close({percentPrice: this.percentPrice, fixedAmountPrice: this.fixedAmountPrice, computePrice: this.computePrice});
  }

  changePrice() {
    this.percentPrice = 0;
    this.fixedAmountPrice = 0;

  }

}
