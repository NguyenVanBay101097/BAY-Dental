import { Component, OnInit } from '@angular/core';
import { EChartsOption } from 'echarts';
import { partnerData } from './partner-report';
// type RawNode = {
//   [key: string]: RawNode;
// } & {
//   $count: number;
// };

interface TreeNode {
  name: string;
  value: number;
  children?: TreeNode[];
}

@Component({
  selector: 'app-partner-report-area',
  templateUrl: './partner-report-area.component.html',
  styleUrls: ['./partner-report-area.component.css']
})
export class PartnerReportAreaComponent implements OnInit {
  data = {
    children: [] as TreeNode[]
  } as TreeNode;

  uploadedDataURL = 'https://echarts.apache.org/examples/data/asset/data/ec-option-doc-statistics-201604.json'
  chartOption: EChartsOption = {}

  constructor() { }

  ngOnInit(): void {
    this.chartOption = {
      title: {
        text: 'ECharts Options',
        subtext: '2016/04',
        left: 'leafDepth'
      },
      tooltip: {},
      series: [
        {
          name: 'option',
          type: 'treemap',
          visibleMin: 300,
          data: this.data.children,
          leafDepth: 2,
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
    const rawData = partnerData;
    if (rawData) {
      this.convert(rawData, this.data, '');

    }
  }

  convert(source: any, target: TreeNode, basePath: string) {
    for (let key in source) {
      let path = basePath ? basePath + '.' + key : key;
      if (!key.match(/^\$/)) {
        target.children = target.children || [];
        const child = {
          name: path
        } as TreeNode;
        target.children.push(child);
        this.convert(source[key], child, path);
      }
    }

    if (!target.children) {
      target.value = source.$count || 1;
    } else {
      target.children.push({
        name: basePath,
        value: source.$count
      });
    }
  }
}
