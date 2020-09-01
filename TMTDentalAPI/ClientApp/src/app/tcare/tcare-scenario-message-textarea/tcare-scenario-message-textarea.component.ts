import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tcare-scenario-message-textarea',
  templateUrl: './tcare-scenario-message-textarea.component.html',
  styleUrls: ['./tcare-scenario-message-textarea.component.css']
})
export class TcareScenarioMessageTextareaComponent implements OnInit {

  @ViewChild('popOver', { static: true }) popOver: NgbPopover;
  @Input() num_CharLeft: string;
  @Output() content_send = new EventEmitter<string>();

  tabs = [
    { name: 'Tên khách hàng', value: '{{ten_khach_hang}}' },
    { name: 'Họ và tên khách hàng', value: '{{fullname_khach_hang}}' },
    { name: 'Tên trang', value: '{{ten_page}}' },
    { name: 'Danh xưng khách hàng', value: '{{danh_xung_khach_hang}}' },
  ];

  constructor() { }

  ngOnInit() {

  }



  clickTab(value) {
    this.content_send.emit(value);
    this.popOver.close();
  }

  toggleWithGreeting(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      popover.open();
    }
  }

}
