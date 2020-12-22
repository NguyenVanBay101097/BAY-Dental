import { Component, OnInit } from '@angular/core';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';

@Component({
  selector: 'app-labo-order-export',
  templateUrl: './labo-order-export.component.html',
  styleUrls: ['./labo-order-export.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class LaboOrderExportComponent implements OnInit {

  selectedIds: string[] = [];

  stateFilter: string;
  dateOrderFrom: Date;
  dateOrderTo: Date;

  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Đơn hàng', value: 'confirm' },
    { text: 'Nháp', value: 'draft' }
  ];

  constructor() { }

  ngOnInit() {
  }

  onStateSelectChange(data: TmtOptionSelect) {
    this.stateFilter = data.value;
    this.loadDataFromApi();
  }

  onDateSearchChange(data) {
    this.dateOrderFrom = data.dateFrom;
    this.dateOrderTo = data.dateTo;
    this.loadDataFromApi();
  }

  loadDataFromApi() {

  }
}
