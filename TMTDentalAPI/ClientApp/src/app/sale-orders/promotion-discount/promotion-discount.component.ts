import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-promotion-discount',
  templateUrl: './promotion-discount.component.html',
  styleUrls: ['./promotion-discount.component.css']
})
export class PromotionDiscountComponent implements OnInit {

  form = {
    discountFixed: 0,
    discountPercent: 0,
    discountType: "percentage", //percentage
    // code: null,
  };
    
  discountTypeDict = {
    VNĐ: "fixed",
    "%": "percentage",
  };

  @Output() applyEvent = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  onChangeDiscount(val) {
    if (!val.target.value || val.target.value == "") {
      this.form.discountFixed = 0;
      this.form.discountPercent = 0;
    }
  }

  onChangeDiscountType() {
    this.form.discountFixed = 0;
    this.form.discountPercent = 0;
  }

  applyDiscount() {
this.applyEvent.emit(this.form);
  }

}
