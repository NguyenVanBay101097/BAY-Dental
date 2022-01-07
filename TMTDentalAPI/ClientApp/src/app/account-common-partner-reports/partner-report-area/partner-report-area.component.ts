import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EChartsOption } from 'echarts';
import { PartnerInfoFilter } from 'src/app/partners/partner.service';
import { AccountCommonPartnerReportService } from '../account-common-partner-report.service';
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
  @Output() filterEmit = new EventEmitter();
  data: any;
  dataSet: any;
  codeFilter = Object.assign({});
  chartOption: EChartsOption = {};
  echartsInstance: any[] = [];

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
    let val = Object.assign({}, this.filter) as PartnerInfoFilter;
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
                districtsData[el3.wardName || 'Chưa xác định'] = wardsData;
              });
            }
            districtsData['$code'] = el2.districtCode;
            districtsData['$type'] = 'district';
            cityData[el2.districtName || 'Chưa xác định'] = districtsData;
          });
        }
        cityData['$code'] = el1.cityCode;
        cityData['$type'] = 'city';
        this.dataSet[el1.cityName || 'Chưa xác định'] = cityData;
      });

      this.loadChartOption();
    }, error => console.log(error));
  }

  loadChartOption() {
    this.chartOption = {
      tooltip: {},
      series: [
        {
          name: 'Việt Nam',
          type: 'treemap',
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
      if (!key.match(/^\$/)) {
        target.children = target.children || [];
        const child = {
          name: key
        } as TreeNode;
        if (typeof source[key] === 'object') {
          child.code = source[key].$code;
          child.type = source[key].$type;
        }
        target.children.push(child);
        this.convert(source[key], child, key);
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
    let res;
    if (event.dataType && event.dataType == 'main') {
      // this.echartsInstance = this.echartsInstance.filter(val => !event.treePathInfo.map(s => s.dataIndex).includes(val.dataIndex));
      event.treePathInfo.forEach(element => {
        const isDuplicate = this.echartsInstance.map(s => s.dataIndex).includes(event.dataIndex);
        if (element.dataIndex === event.dataIndex && !isDuplicate) {
          const data = { ...element };
          data['code'] = event.data.code;
          data['type'] = event.data.type;
          this.echartsInstance.push(data);
        }
      });
      res = event.data;
    }

    if (event.selfType && event.selfType == 'breadcrumb') {
      // this.echartsInstance = this.echartsInstance.filter(val => !event.treePathInfo.map(s => s.dataIndex).includes(val.dataIndex));
      res = this.echartsInstance.find(s => s.dataIndex == event.nodeData.dataIndex);
    }

    let dataEmit;
    if (res) {
      dataEmit = {
        type: res ? res.type : '',
        code: res ? res.code : ''
      }
      this.filterEmit.emit(dataEmit);
    }
    else {
      this.filterEmit.emit(false);
    }
  }
}
