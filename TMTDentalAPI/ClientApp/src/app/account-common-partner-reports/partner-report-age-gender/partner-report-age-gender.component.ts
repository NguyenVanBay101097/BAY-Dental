import { Component, OnInit } from '@angular/core';
import { EChartsOption } from 'echarts';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-report-age-gender',
  templateUrl: './partner-report-age-gender.component.html',
  styleUrls: ['./partner-report-age-gender.component.css']
})
export class PartnerReportAgeGenderComponent implements OnInit {
  // chartOption: EChartsOption;
  dataSet: any;
  chartOption: EChartsOption;
  constructor(
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit(): void {
    this.loadReportAgeGender();
    // this.loadChartOption();
  }

  loadReportAgeGender() {
    console.log('3')
    let val = new AccountCommonPartnerReportOverviewFilter();
    this.accountCommonPartnerReportService.getPartnerReportGenderOverview(val).subscribe((res: any) => {
      this.dataSet = res;
      this.loadChartOption();
      // console.log(this.dataSet);
    }, error => console.log(error));
  }

  loadChartOption() {
    this.chartOption = {
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'shadow'
        }
      },
      legend: {
        bottom: 10,
        left: 'center',
        data: ['Nam', 'Nữ', 'Khác']
      },
      toolbox: {
        show: true,
        orient: 'vertical',
        left: 'right',
        top: 'center',

      },
      xAxis: [
        {
          type: 'category',
          axisLabel: {
            interval: 0
          },
          // axisTick: { show: false },
          data: this.dataSet?.legendChart
        }
      ],
      yAxis: [
        {
          type: "value",
          axisLabel: {
            formatter: "{value} %"
          }
        }
      ],
      series: [
        {
          name: 'Nam',
          type: 'bar',
          barGap: 0,
          emphasis: {
            focus: 'series'
          },
          itemStyle: {
            color: '#007BFF'
          },
          data: this.dataSet?.partnerGenderItems[0]?.percent
        },
        {
          name: 'Nữ',
          type: 'bar',
          emphasis: {
            focus: 'series'
          },
          itemStyle: {
            color: '#EB3B5B'
          },
          data: this.dataSet?.partnerGenderItems[1]?.percent
        },
        {
          name: 'Khác',
          type: 'bar',
          emphasis: {
            focus: 'series'
          },
          itemStyle: {
            color: '#28A745'
          },
          data: this.dataSet?.partnerGenderItems[2]?.percent
        }
      ]
    }
  }
}
