import { Component, OnInit, Input } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { NgbDate, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-card-card-grid',
  templateUrl: './card-card-grid.component.html',
  styleUrls: ['./card-card-grid.component.css']
})

export class CardCardGridComponent implements OnInit {
  @Input() gridData: GridDataResult;
  @Input() limit = 20;
  @Input() skip = 0;
  @Input() loading = false;

  constructor() { }

  ngOnInit() {
  }
}




