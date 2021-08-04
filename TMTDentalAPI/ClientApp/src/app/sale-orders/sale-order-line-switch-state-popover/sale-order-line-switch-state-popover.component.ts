import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { SaleOrderLineDisplay } from '../sale-order-line-display';

@Component({
  selector: 'app-sale-order-line-switch-state-popover',
  templateUrl: './sale-order-line-switch-state-popover.component.html',
  styleUrls: ['./sale-order-line-switch-state-popover.component.css']
})
export class SaleOrderLineSwitchStatePopoverComponent implements OnInit {
  public listState = [
    {text:'Đang điều trị', value:'sale'},
    {text:'Hoàn thành', value:'done'},
    {text:'Ngừng điều trị', value:'cancel'},
   ];
   @ViewChild('popOver', { static: true }) public popover: any;
   @Input() popOverPlace = 'right';
   @Input() lineInput = new SaleOrderLineDisplay();
   @Output() onSaveEvent = new EventEmitter<any>();
   line: any;
  constructor() { }

  ngOnInit() {
    this.line = Object.assign({}, this.lineInput);
  }

  toggleWithTags(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      popover.open();
    }
  }

  onSave() {
    this.popover.close();
    this.onSaveEvent.emit(this.line);
  }

}
