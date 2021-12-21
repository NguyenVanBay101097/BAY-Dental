import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { EChartsOption } from 'echarts';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';
import { partnerData } from './partner-report';
// type RawNode = {
//   [key: string]: RawNode;
// } & {
//   $count: number;
// };

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
  @Output() filterEmit = new EventEmitter<any>();
  data = {
    children: [] as TreeNode[]
  } as TreeNode;
  dataSet: any;
  codeFilter = Object.assign({});

  chartOption: EChartsOption = {}
  constructor(
    private accountCommonPartnerReportService: AccountCommonPartnerReportService
  ) { }

  ngOnInit(): void {
    // this.loadChartOption();
    this.loadReportArea();
  }

  loadReportArea() {
    // console.log('1')
    this.dataSet = {};
    let val = new AccountCommonPartnerReportOverviewFilter();
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
            // districtsData['$count'] = el2.count;
            districtsData['$code'] = el2.districtCode;
            districtsData['$type'] = 'district';
            cityData[el2.districtName] = districtsData;
          });
        }
        // cityData['$count'] = el1.count;
        cityData['$code'] = el1.cityCode;
        cityData['$type'] = 'city';
        this.dataSet[el1.cityName] = cityData;
      });
      // console.log(this.dataSet);
      this.loadChartOption();
    }, error => console.log(error));
  }

  loadChartOption() {
    this.chartOption = {
      tooltip: {},
      toolbox: {
        show: false,
        feature: {
          dataZoom: {
            yAxisIndex: "none",
            xAxisIndex: "none",
          },
        }
      },
      dataZoom: [{
        type: 'slider',//Telescopic strip below the chart
        show: false, //Whether it is displayed
        realtime: true, //When dragging, do you really update a series of views
        start: 0, //Telescope start position (1-100), can be changed at any time
        end: 100, //Telescopic balance (1-100), can be changed at any time
      }],
      series: [
        {
          name: 'Việt Nam',
          type: 'treemap',
          visibleMin: 300,
          data: this.data.children,
          leafDepth: 1,
          levels: [
            {
              itemStyle: {
                borderColor: '#555',
                borderWidth: 4,
                gapWidth: 4
              }
            },
            {
              colorSaturation: [0.3, 0.6],
              itemStyle: {
                borderColorSaturation: 0.7,
                gapWidth: 2,
                borderWidth: 2
              }
            },
            {
              colorSaturation: [0.3, 0.5],
              itemStyle: {
                borderColorSaturation: 0.6,
                gapWidth: 1
              }
            },
            {
              colorSaturation: [0.3, 0.5]
            }
          ]
        }
      ]
    }
    const rawData = this.dataSet;
    // const rawData = partnerData;
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
          // console.log(source[key]);
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
    // console.log(event);
    // const a = event?.data?.children.slice(-1).pop();
    // console.log(a);
    // console.log(a.type)
    // if (a) {
    //   switch (a.type) {
    //     case 'city':
    //       this.codeFilter['cityCode'] = a?.code;
    //       this.codeFilter['districtCode'] = null;
    //       this.codeFilter['wardCode'] = null;
    //       break;
    //     case 'district':
    //       // this.codeFilter['cityCode'] = a?.code;
    //       this.codeFilter['districtCode'] = a?.code;
    //       this.codeFilter['wardCode'] = null;
    //       break;
    //     case 'ward':
    //       // this.codeFilter['cityCode'] = a?.code;
    //       // this.codeFilter['districtCode'] = a?.code;
    //       this.codeFilter['wardCode'] = a?.code;
    //       break;
    //     default:
    //       this.codeFilter['cityCode'] = null;
    //       this.codeFilter['districtCode'] = null;
    //       this.codeFilter['wardCode'] = null;
    //       break;
    //   }
    // }
    // console.log(this.codeFilter);
  }
}
