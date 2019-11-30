import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-shared-card-card-grid',
  templateUrl: './shared-card-card-grid.component.html',
  styleUrls: ['./shared-card-card-grid.component.css']
})

export class SharedCardCardGridComponent implements OnInit {
  @Input() gridData: GridDataResult;
  @Input() limit = 20;
  @Input() skip = 0;
  @Input() loading = false;
  @Output() editClick = new EventEmitter<any>();
  @Output() deleteClick = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  showState(state: string) {
    switch (state) {
      case 'confirmed':
        return 'Chờ cấp thẻ';
      case 'in_use':
        return 'Đang sử dụng';
      case 'cancelled':
        return 'Đã hủy';
      case 'locked':
        return 'Đã khóa';
      default:
        return 'Nháp';
    }
  }

  editItem(item) {
    this.editClick.emit(item);
  }

  deleteItem(item) {
    this.deleteClick.emit(item);
  }
}




