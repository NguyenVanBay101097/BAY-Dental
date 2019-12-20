import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { GridDataResult, PageChangeEvent, SelectionEvent } from '@progress/kendo-angular-grid';
import { PartnerBasic } from 'src/app/partners/partner-simple';


@Component({
  selector: 'app-shared-partner-grid',
  templateUrl: './shared-partner-grid.component.html',
  styleUrls: ['./shared-partner-grid.component.css']
})
export class SharedPartnerGridComponent implements OnInit {
  @Input() gridData: GridDataResult;
  @Input() limit;
  @Input() skip;
  @Input() loading;

  @Output() pageChange = new EventEmitter<PageChangeEvent>();
  @Output() selectionChange = new EventEmitter<SelectionEvent>();

  constructor() { }
  ngOnInit() {
  }

  onPageChange(event: PageChangeEvent): void {
    this.pageChange.emit(event);
  }

  onSelectionChange(event: SelectionEvent) {
    this.selectionChange.emit(event);
  }
}

