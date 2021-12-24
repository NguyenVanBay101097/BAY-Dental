import { Component, Input, OnInit } from '@angular/core';
import { EChartsOption } from 'echarts';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-report-age-gender',
  templateUrl: './partner-report-age-gender.component.html',
  styleUrls: ['./partner-report-age-gender.component.css']
})
export class PartnerReportAgeGenderComponent implements OnInit {
  @Input() filter: any;
  dataSet: any;
  chartOption: EChartsOption;

  constructor(
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit(): void {
    this.loadReportAgeGender();
  }

  loadReportAgeGender() {
    let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;
    this.accountCommonPartnerReportService.getPartnerReportGenderOverview(val).subscribe((res: any) => {
      this.dataSet = res;
      this.loadChartOption();
    }, error => console.log(error));
  }

  loadChartOption() {
    this.chartOption = {
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'shadow'
        },
        formatter: (params)=>{
          console.log(params);
          return '';
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
          data: this.dataSet?.legendChart
        }
      ],
      yAxis: [
        {
          type: "value",
          axisLabel: {
            formatter: "{value}"
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
          data: this.dataSet?.partnerGenderItems[0]?.count
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
          data: this.dataSet?.partnerGenderItems[1]?.count
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
          data: this.dataSet?.partnerGenderItems[2]?.count
        }
      ]
    }
  }
}
