import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-partner-customer-treatment-history',
  templateUrl: './partner-customer-treatment-history.component.html',
  styleUrls: ['./partner-customer-treatment-history.component.css']
})
export class PartnerCustomerTreatmentHistoryComponent implements OnInit {
  partnerId: string;

  thTable_saleOrders = [
    { name: 'Số phiếu' },
    { name: 'Ngày lập phiếu' },
    { name: 'Tổng tiền' },
    { name: 'Còn nợ' }
  ]
  saleOrders: any[] = [
    {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }, {
      id: "this is id",
      date: "this is date",
      money: "this is money",
      owe: "this is owe",
    }
  ];

  thTable_services = [
    { name: 'Dịch vụ' },
    { name: 'Đơn giá' }
  ]
  services: any[] = [
    {
      service: "this is service name",
      money: "this is money",
    }, {
      service: "this is service name aaaaaaaaaaaaaa",
      money: "this is money",
    }
  ];

  date: Date = new Date();

  thTable_payments = [
    { name: 'Dịch vụ', width: '200px' },
    { name: 'Răng' },
    { name: 'Chuẩn đoán' },
    { name: 'Số lượng' },
    { name: 'Đơn giá' },
    { name: 'Giảm' },
    { name: 'Thành tiền' },
    { name: 'Thanh toán' },
    { name: 'Còn nợ' },
  ]
  payments: any[] = [
    {
      service: "this is service name",
      teeth: "this is teeth",
      chuandoan: "this is chuandoan",
      count: "this is count",
      money: "this is money",
      discount: "this is discount",
      tmoney: "this is tmoney",
      pay: "this is pay",
      owe: "this is owe",
    }, {
      service: "this is service name aaaaaaa aaaaaaa bbbbb nnnnn",
      teeth: "this is teeth",
      chuandoan: "this is chuandoan",
      count: "this is count",
      money: "this is money",
      discount: "this is discount",
      tmoney: "this is tmoney",
      pay: "this is pay",
      owe: "this is owe",
    }
  ];

  constructor(
    private activeRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');
  }

}
