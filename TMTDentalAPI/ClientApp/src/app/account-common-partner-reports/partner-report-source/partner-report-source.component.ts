import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { EChartsOption } from 'echarts';

@Component({
  selector: 'app-partner-report-source',
  templateUrl: './partner-report-source.component.html',
  styleUrls: ['./partner-report-source.component.css']
})
export class PartnerReportSourceComponent implements OnInit, OnChanges {
  @Input() dataReportSource: any;

  updateOptions: any;
  responseData: any[] = [];
  chartOption: EChartsOption = {
    textStyle: {
      fontFamily: 'sans-serif',
    },
    tooltip: {
      trigger: 'item',
    },
    legend: {
      bottom: 10,
      left: 'center',
    },
    series: [
      {
        type: 'pie',
        radius: '50%',
        data: [],
        label: {
          show: true,
          position: 'inside',
          formatter: function (params) {
            return `${params.percent}%`;
          },
          color: '#fff'
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
  };

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.dataReportSource) {
      this.responseData = this.dataReportSource;
      this.processData();
    }
  }

  ngOnInit(): void {

  }

  processData() {
    const dataSet = this.dataReportSource.map(val => {
      return { value: val.totalPartner, name: val.partnerSourceName }
    });

    this.updateOptions = {
      series: [{
        data: dataSet
      }]
    };
  }
}
