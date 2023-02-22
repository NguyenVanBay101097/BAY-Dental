import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-sms-personalized-tabs',
  templateUrl: './sms-personalized-tabs.component.html',
  styleUrls: ['./sms-personalized-tabs.component.css']
})
export class SmsPersonalizedTabsComponent implements OnInit {
  @Input() type: string;
  @Output() contentEmit = new EventEmitter();
  // listContentTabs: string[] = [];
  tabs: Array<{ name: string, value: string }> = [];
  listTabs = {
    "partner": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên', value: '{ten}' },
      { name: 'Ngày sinh', value: '{ngay_sinh}' }
    ],
    "appointment": [
      { name: 'Danh xưng khách hàng', value: '{danh_xung_khach_hang}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Giờ hẹn', value: '{gio_hen}' },
      { name: 'Ngày hẹn', value: '{ngay_hen}' },
      { name: 'Tên bác sĩ', value: '{ten_bac_si}' }
    ],
    "saleOrderLine": [
      { name: 'Danh xưng khách hàng', value: '{danh_xung_khach_hang}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Số phiếu điều trị', value: '{so_phieu_dieu_tri}' },
      { name: 'Tên dịch vụ', value: '{ten_dich_vu}' },
      { name: 'Tên bác sĩ', value: '{ten_bac_si}' }
    ],
    "saleOrder": [
      { name: 'Danh xưng khách hàng', value: '{danh_xung_khach_hang}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
    ],
  }
  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    this.ngOnInit();
  }

  ngOnInit() {
    if (this.listTabs[this.type]) {
      this.tabs = this.listTabs[this.type];
    }
  }


  addToContent(tabValue) {
    // this.listContentTabs.push(tabValue);
    this.contentEmit.emit(tabValue);
  }

}
