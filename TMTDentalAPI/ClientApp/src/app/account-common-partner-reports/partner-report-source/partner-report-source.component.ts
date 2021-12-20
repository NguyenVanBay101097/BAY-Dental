import { Component, OnInit } from '@angular/core';
import { EChartsOption } from 'echarts';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-report-source',
  templateUrl: './partner-report-source.component.html',
  styleUrls: ['./partner-report-source.component.css']
})
export class PartnerReportSourceComponent implements OnInit {
  chartOption: EChartsOption;
  dataSet: [];
  constructor(
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit(): void {
    this.loadReportSource();
  }

  loadReportSource() {
    let val = new AccountCommonPartnerReportOverviewFilter();
    this.accountCommonPartnerReportService.getPartnerReportSourceOverview(val).subscribe((res: any) => {
      // console.log(res);
      this.dataSet = res.map(val => {
        return { value: val.totalPartner, name: val.partnerSourceName }
      })
      // console.log(this.dataSet);
      this.loadChartOption();
    }, error => console.log(error));
  }

  loadChartOption() {
    this.chartOption = {
      tooltip: {
        trigger: 'item',
        formatter: function (params) {
          return `${params.name}: ${params.percent}%`;
        }
      },
      legend: {
        bottom: 10,
        left: 'center',
      },
      series: [
        {
          type: 'pie',
          radius: '50%',
          data: this.dataSet,
          label: {
            show: false,
          },
          emphasis: {
            itemStyle: {
              shadowBlur: 10,
              shadowOffsetX: 0,
              shadowColor: 'rgba(0, 0, 0, 0.5)'
            }
          }
        },
      ]
    }
  }
}
