import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-shared-layout-dynamic-form',
  templateUrl: './shared-layout-dynamic-form.component.html',
  styleUrls: ['./shared-layout-dynamic-form.component.css']
})
export class SharedLayoutDynamicFormComponent implements OnInit {

  item = [
    { name: 'birthday', text: 'Trước ngày sinh nhật' },
    { name: 'treatment', text: "Sau ngày điều trị cuối" },
    { name: 'customerGroup', text: 'Nhóm khách hàng' },
    { name: 'service', text: 'Dịch vụ' },
    { name: 'serviceGroup', text: 'Nhóm dịch vụ' }
  ];

  constructor() { }

  ngOnInit() {

  }

}
