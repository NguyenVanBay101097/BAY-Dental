import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EChartsOption } from 'echarts';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';
interface TreeNode {
  name: string;
  value: number;
  code: string;
  type: string;
  children?: TreeNode[];
}

@Component({
  selector: 'app-partner-report-area',
  templateUrl: './partner-report-area.component.html',
  styleUrls: ['./partner-report-area.component.css']
})
export class PartnerReportAreaComponent implements OnInit {
  @Input() filter: any;
  @Output() filterEmit = new EventEmitter<any>();
  data: any;
  dataSet: any;
  codeFilter = Object.assign({});
  chartOption: EChartsOption = {};

  constructor(
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit(): void {
    this.loadReportArea();
  }

  loadReportArea() {
    this.dataSet = {};
    this.data = {
      children: [] as TreeNode[]
    } as TreeNode;
    let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;
    this.accountCommonPartnerReportService.getPartnerReportTreeMapOverview(val).subscribe((res: any) => {
      res.forEach(el1 => {
        let cityData = Object.assign({});
        if (el1.districts) {
          el1.districts.forEach(el2 => {
            let districtsData = Object.assign({});
            if (el2.wards) {
              el2.wards.forEach(el3 => {
                let wardsData = Object.assign({});
                wardsData['$count'] = el3.count;
                wardsData['$code'] = el3.wardCode;
                wardsData['$type'] = 'ward';
                districtsData[el3.wardName] = wardsData;
              });
            }
            districtsData['$code'] = el2.districtCode;
            districtsData['$type'] = 'district';
            cityData[el2.districtName] = districtsData;
          });
        }
        cityData['$code'] = el1.cityCode;
        cityData['$type'] = 'city';
        this.dataSet[el1.cityName] = cityData;
      });
      this.loadChartOption();
    }, error => console.log(error));
  }

  loadChartOption() {
    this.chartOption = {
      series: [
        {
          name: 'Việt Nam',
          type: 'treemap',
          visibleMin: 300,
          roam: false,
          data: this.data.children,
          leafDepth: 1,
        }
      ]
    }

    const rawData = this.dataSet;
    if (rawData) {
      this.convert(rawData, this.data, '');
    }
  }

  convert(source: any, target: TreeNode, basePath: string) {
    for (let key in source) {
      let path = key;
      if (!key.match(/^\$/)) {
        target.children = target.children || [];
        const child = {
          name: path
        } as TreeNode;
        if (typeof source[key] === 'object') {
          child.code = source[key].$code;
        }
        target.children.push(child);
        this.convert(source[key], child, path);
      }
    }

    if (!target.children) {
      target.value = source.$count || 1;
      target.code = source.$code || '';
      target.type = source.$type || '';
    } else {
      target.children.push({
        name: basePath,
        value: source.$count,
        code: source.$code,
        type: source.$type,
      });
    }
  }

  onChartClick(event) {
    console.log(event);
  }
}
