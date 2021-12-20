import { Component, OnInit } from '@angular/core';
import { EChartsOption } from 'echarts';

@Component({
  selector: 'app-partner-report-age-gender',
  templateUrl: './partner-report-age-gender.component.html',
  styleUrls: ['./partner-report-age-gender.component.css']
})
export class PartnerReportAgeGenderComponent implements OnInit {

  chartOption: EChartsOption = {
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
        data: ['0-12', '13-17', '18-24', '25-34', '35-44', '45-64', '65+', 'Không xác định']
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
        data: [10, 12, 14, 7, 4, 10, 12, 14]
      },
      {
        name: 'Nữ',
        type: 'bar',
        emphasis: {
          focus: 'series'
        },
        data: [27, 34, 12, 65, 34, 10, 12, 14]
      },
      {
        name: 'Khác',
        type: 'bar',
        emphasis: {
          focus: 'series'
        },
        data: [22, 53, 12, 76, 34, 10, 12, 14]
      }
    ]
  }
  constructor() { }

  ngOnInit(): void {
  }

}
