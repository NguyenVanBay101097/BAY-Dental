import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EChartsOption } from 'echarts';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-report-area',
  templateUrl: './partner-report-area.component.html',
  styleUrls: ['./partner-report-area.component.css']
})
export class PartnerReportAreaComponent implements OnInit {
  @Input() filter: any;
  @Output() filterEmit = new EventEmitter();
  data: any = [];
  dataSet: any;
  codeFilter = Object.assign({});
  chartOption: EChartsOption = {
    tooltip: {},
    series: [
      {
        name: 'Việt Nam',
        type: 'treemap',
        roam: false,
        data: this.data,
        leafDepth: 1,
      }
    ]
  };

  updateOptions: any;

  echartsInstance: any[] = [];
  chartInstance: any;

  clickTracking: any[] = [];

  constructor(
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit(): void {
    this.loadReportArea();
  }

  loadReportArea() {
    let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;
    this.accountCommonPartnerReportService.getPartnerReportTreeMapOverview(val).subscribe((res: any) => {
      this.data = res.map(x => {
        return {
          name: x.cityName || 'Chưa xác định',
          value: x.count,
          code: x.cityCode,
          type: 'city',
          children: x.districts.map(s => {
            return {
              name: s.districtName || 'Chưa xác định',
              value: s.count,
              code: s.districtCode,
              type: 'district',
              children: s.wards.map(m => {
                return {
                  name: m.wardName || 'Chưa xác định',
                  value: m.count,
                  code: m.wardCode,
                  type: 'ward',
                }
              })
            }
          })
        }
      });

      this.updateOptions = {
        series: [{
          data: this.data
        }]
      };
    }, error => console.log(error));
  }

  onChartInit(e: any) {
    this.chartInstance = e;
    console.log('on chart init:', e);
  }

  onChartClick(event) {
    if (event.dataType && event.dataType == 'main') {
      var item = <any>{
        dataIndex: event.dataIndex,
        data: event.data
      };

      this.clickTracking.push(item);
      this.filterEmit.emit(item);
    }

    if (event.selfType && event.selfType == 'breadcrumb') {
      var item = this.clickTracking.find(x => x.dataIndex == event.dataIndex);
      this.filterEmit.emit(item);
    }
  }
}
