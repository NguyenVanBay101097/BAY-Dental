import { Component, OnInit, Output, EventEmitter, ViewChild, Input } from '@angular/core';

@Component({
  selector: 'app-sale-coupon-program-filter-active',
  templateUrl: './sale-coupon-program-filter-active.component.html',
  styleUrls: ['./sale-coupon-program-filter-active.component.css']
})
export class SaleCouponProgramFilterActiveComponent implements OnInit {
  @Input() value: boolean;
  items: { text: string, value: boolean }[] = [
    { text: 'Đang hoạt động', value: true },
    { text: 'Đã đóng', value: false },
  ];

  @Output() valueChange = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  onSelect(value) {
    this.value = value;
    this.valueChange.emit(value);
  }

  getResult() {
    switch (this.value) {
      case false:
        return 'Đang đóng';
      default:
        return 'Đang hoạt động';
    }
  }
}




