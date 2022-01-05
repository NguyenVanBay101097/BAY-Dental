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
  responseData: any = [];
  updateOptions: any;
  chartOption: EChartsOption = {
    textStyle: {
      fontFamily: 'sans-serif',
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      },
    },
    legend: {
      bottom: 10,
      left: 'center',
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
        data: []
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
        data: []
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
        data: []
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
        data: []
      }
    ]
  };

  constructor(
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit(): void {
    this.loadReportAgeGender();
  }

  loadReportAgeGender() {
    let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;
    this.accountCommonPartnerReportService.getPartnerReportGenderOverview(val).subscribe((res: any) => {
      this.responseData = res;
      this.updateOptions = {
        xAxis: {
          data: res.map(x => x.ageRangeName)
        },
        series: [
          {
            data: res.map(x => x.totalMale)
          },
          {
            data: res.map(x => x.totalFemale)
          },
          {
            data: res.map(x => x.totalOther)
          }
        ]
      };
    }, error => console.log(error));
  }

  getGender(val: string[]) {
    return val.map(el => {
      switch (el) {
        case 'male':
          return 'Nam';
        case 'female':
          return 'Nữ';
        default:
          return 'Khác';
      }
    })
  }
}
