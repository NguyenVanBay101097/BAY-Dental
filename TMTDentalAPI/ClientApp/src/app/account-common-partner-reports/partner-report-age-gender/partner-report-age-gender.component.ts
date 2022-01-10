import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { EChartsOption } from 'echarts';

@Component({
  selector: 'app-partner-report-age-gender',
  templateUrl: './partner-report-age-gender.component.html',
  styleUrls: ['./partner-report-age-gender.component.css']
})
export class PartnerReportAgeGenderComponent implements OnInit, OnChanges {
  @Input() dataReportAgeGender: any;
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

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.dataReportAgeGender.length) {
      this.responseData = this.dataReportAgeGender;
      this.updateDataOptions();
    }
  }

  ngOnInit(): void {

  }

  updateDataOptions() {
    this.updateOptions = {
      xAxis: {
        data: this.dataReportAgeGender.map(x => x.ageRangeName)
      },
      series: [
        {
          data: this.dataReportAgeGender.map(x => x.totalMale)
        },
        {
          data: this.dataReportAgeGender.map(x => x.totalFemale)
        },
        {
          data: this.dataReportAgeGender.map(x => x.totalOther)
        }
      ]
    };
  }
}
