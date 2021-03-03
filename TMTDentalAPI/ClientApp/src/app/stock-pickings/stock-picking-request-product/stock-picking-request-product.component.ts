import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-stock-picking-request-product',
  templateUrl: './stock-picking-request-product.component.html',
  styleUrls: ['./stock-picking-request-product.component.css']
})
export class StockPickingRequestProductComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;

  constructor(
    private modalService: NgbModal,
    private intlService: IntlService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
  }

}
