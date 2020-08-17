import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-account-report-general-ledger-move-line',
  templateUrl: './account-report-general-ledger-move-line.component.html',
  styleUrls: ['./account-report-general-ledger-move-line.component.css']
})
export class AccountReportGeneralLedgerMoveLineComponent implements OnInit {
  @Input() items: any = [];
  constructor() { }

  ngOnInit() {
  }

}
