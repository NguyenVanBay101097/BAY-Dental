import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EChartsOption } from 'echarts';
import { PartnerInfoFilter } from 'src/app/partners/partner.service';
import { AccountCommonPartnerReportService } from '../account-common-partner-report.service';
interface TreeNode {
  name: string;
  value: number;
  cityCode: string;
  districtCode: string;
  wardCode: string;
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
    // debugger;
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
                districtsData['$cityCode'] = el2.cityCode;
                districtsData['$districtCode'] = el2.districtCode;
                wardsData['$wardCode'] = el3.wardCode;
                wardsData['$type'] = 'ward';
                districtsData[el3.wardName || 'Chưa xác định'] = wardsData;
              });
            }
            districtsData['$cityCode'] = el2.cityCode;
            districtsData['$districtCode'] = el2.districtCode;
            districtsData['$type'] = 'district';
            districtsData['$count'] = el2.count;
            cityData[el2.districtName || 'Chưa xác định'] = districtsData;
          });
        }

        cityData['$cityCode'] = el1.cityCode;
        cityData['$type'] = 'city';
        cityData['$count'] = el1.count;
        this.dataSet[el1.cityName || 'Chưa xác định'] = cityData;
      });
      this.loadChartOption();
    }, error => console.log(error));
  }

  loadChartOption() {
    this.chartOption = {
      textStyle: {
        fontFamily: 'sans-serif',
      },
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
          child.cityCode = source[key].$cityCode;
          child.districtCode = source[key].$districtCode;
          child.wardCode = source[key].$wardCode;
          child.type = source[key].$type;
          child.value = source[key].$count;
        }
        target.children.push(child);
        this.convert(source[key], child, key);
      }
    }
  }

  onChartClick(event) {
    let res;
    if (event.dataType && event.dataType == 'main') {
      event.treePathInfo.forEach(element => {
        const isDuplicate = this.echartsInstance.map(s => s.element.dataIndex).includes(event.dataIndex);
        if (element.dataIndex === event.dataIndex && !isDuplicate) {
          const data = {
            element: element,
            data: event.data
          };

          this.echartsInstance.push(data);
        }
      });
      res = event.data;
    }

    if (event.selfType && event.selfType == 'breadcrumb') {
      var item = this.echartsInstance.find(s => s.element.dataIndex == event.nodeData.dataIndex);
      if (item) {
        res = item.data;
      }
    }

    if (res) {
      this.filterEmit.emit(res);
    }
    else {
      this.filterEmit.emit(false);
    }
  }
}
