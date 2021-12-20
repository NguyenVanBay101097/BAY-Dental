import { Component, OnInit } from '@angular/core';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-report-overview',
  templateUrl: './partner-report-overview.component.html',
  styleUrls: ['./partner-report-overview.component.css']
})
export class PartnerReportOverviewComponent implements OnInit {
  orderResiduals: { text: string, value: number }[] = [
    { text: 'Có dự kiến thu', value: 1 },
    { text: 'Không có dự kiến thu', value: 0 }
  ];

  totalDebits: { text: string, value: number }[] = [
    { text: 'Có công nợ', value: 1 },
    { text: 'Không có công nợ', value: 0 }
  ];

  orderStates: { text: string, value: string }[] = [
    { text: 'Chưa phát sinh', value: 'draft' },
    { text: 'Đang điều trị', value: 'sale' },
    { text: 'Hoàn thành', value: 'done' }
  ];

  reportSumary: any;
  reportSource: any;

  constructor(
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    let val = new AccountCommonPartnerReportOverviewFilter();
    this.accountCommonPartnerReportService.getPartnerReportSumaryOverview(val).subscribe((res: any) => {
      this.reportSumary = res;
    }, error => console.log(error))
  }

}
