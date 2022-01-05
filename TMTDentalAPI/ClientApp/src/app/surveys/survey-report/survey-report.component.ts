import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { EChartsOption } from 'echarts';
import * as moment from 'moment';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { GetReportAssignmentQueryRequest, SurveReportService } from '../survey-report.service';

@Component({
  selector: 'app-survey-report',
  templateUrl: './survey-report.component.html',
  styleUrls: ['./survey-report.component.css']
})
export class SurveyReportComponent implements OnInit {

  filter = new GetReportAssignmentQueryRequest();
  companies: CompanySimple[] = [];
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  empChartOption: EChartsOption;
  scoreChartOption: EChartsOption;
  questionChartOption: EChartsOption;
  dataScroreReport: any;
  dataEmpReport: any = [];
  dataQuestionReport: any = [];
  constructor(
    private companyService: CompanyService,
    private surveReportService: SurveReportService
  ) { }

  ngOnInit(): void {

    this.initFilter();
    this.loadCompanies();
    this.companyVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyVC.loading = true)),
        switchMap((value) => this.searchCompany$(value)
        )
      )
      .subscribe((x: any) => {
        this.companies = x.items;
        this.companyVC.loading = false;
      });

    this.loadDataFromApi();
  }

  initFilter() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    this.filter.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
  }

  loadDataFromApi() {
    this.loadReportNumberOfAssigmentByEmployee();
    this.loadReportRatingScroreRate();
    this.loadReportSatifyScoreRatingByQuestion();
  }

  getApiReq() {
    var val = Object.assign({}, this.filter);
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.companyId = val.companyId ?? '';
    return val;
  }

  loadReportNumberOfAssigmentByEmployee() {
    var val = this.getApiReq();
    val.status = "done,contact";
    this.surveReportService.reportNumberOfAssigmentByEmployee(val).subscribe((res: any[]) => {
      this.dataEmpReport = res;
      let xAxisData = [];
      let data1 = [];
      let data2 = [];
      var emphasisStyle = {
        itemStyle: {
          shadowBlur: 10,
          shadowColor: 'rgba(0,0,0,0.3)'
        }
      };

      res.forEach(e => {
        xAxisData.push(e.employeeName);
        data1.push(e.doneCount);
        data2.push(e.contactCount);
      });
      this.empChartOption =
      {
        legend: {
          data: ['Khảo sát hoàn thành', 'Khảo sát đang theo dõi'],
          bottom: 0
        },
        tooltip: {
          trigger: "axis",
          textStyle: {
            fontSize: 'unset',
            fontFamily:"unset"
          },
          axisPointer: {
            type: "line",
          },
          // formatter: (params) => {
          //   return `
          //       ${params[0].seriesName}: ${params[0].value}<br />
          //       ${params[1].seriesName}: ${params[1].value}<br />
          //       Nhân viên: ${params[0].name}
          //       `;
          // },
        },
        xAxis: {
          data: xAxisData,
          axisLine: {
            onZero: true,
            lineStyle: {
              type: 'solid'
            },
          },
          axisTick: {
            alignWithLabel: true
          }
        },
        yAxis: {
          axisLine: {
            onZero: true,
            lineStyle: {
              type: 'dashed'
            },
          },
          axisTick: {
            alignWithLabel: true
          }
        },
        grid: {
          top: 30,
          bottom: 60
        },
        series: [
          {
            name: 'Khảo sát hoàn thành',
            type: 'bar',
            stack: 'one',
            emphasis: emphasisStyle,
            data: data1,
            color: '#007BFF',
          },
          {
            name: 'Khảo sát đang theo dõi',
            type: 'bar',
            stack: 'one',
            emphasis: emphasisStyle,
            data: data2,
            color: '#28A745'
          }
        ]
      };

    })
  }

  loadReportRatingScroreRate() {
    var val = this.getApiReq();
    val.status = "done";
    this.surveReportService.reportRatingScroreRate(val).subscribe((res: any) => {
      this.dataScroreReport = res;
      var colors = ['#007BFF', '#FFC107', '#28A645', '#939EAB', '#EB3B5B'];
      this.scoreChartOption = {
        tooltip: {
          trigger: 'item',
          textStyle: {
            fontSize: 'unset',
            fontFamily:"unset"
          },
        },
        legend: {
          right: 'center',
          bottom: 0
        },
        grid: {
          top: 1,
          bottom: 60
        },
        series: [
          {
            radius: '70%',
            label: {
              show: false,
            },
            type: 'pie',
            data: res.lines,
            color: colors.slice(0, res.lines.length),
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: 'rgba(0, 0, 0, 0.5)'
              }
            }
          }
        ]
      };

    })
  }

  get isShowQuestionReport() {
    return this.dataQuestionReport?.questionNames?.length;
  }

  loadReportSatifyScoreRatingByQuestion() {
    var val = this.getApiReq();
    val.status = "done";
    this.surveReportService.reportSatifyScoreRatingByQuestion(val).subscribe((res: any) => {
      this.dataQuestionReport = res;
      var colors = ['#939EAB', '#EB3B5B', '#FFC107', '#28A645', '#007BFF'];
      var seriesData = [];
      var yAxisData = [];
      res.data.forEach(e => {
        yAxisData = res.questionNames;
        seriesData.push({
          name: `${e.score} điểm`,
          type: 'bar',
          stack: 'total',
          label: {
            show: true
          },
          emphasis: {
            focus: 'series'
          },
          data: e.data.map((x, i) => {
            var total = (res.data.map(z => z.data[i]) as number[]).reduce((pre, cur) => pre + cur);
            return { value: Math.round((x * 100 / total)) == 0 ? null : Math.round((x * 100 / total)), valueCount: x };
          })
        })
      });
      this.questionChartOption = {
        tooltip: {
          trigger: 'axis',
          textStyle: {
            fontSize: 'unset',
            fontFamily:"unset"
          },
          axisPointer: {
            type: 'line' // 'shadow' as default; can also be 'line' or 'shadow'
          },
          formatter: (params) => {
            // create new marker
            var fmText = `${params[0].name} </br> `;
            params.forEach(p => {
              var icon0 = `<span style="background-color:${p.color};height: 4px;width: 16px;height: 16px"></span>`;
              fmText += `<span class="d-flex">${icon0}&nbsp ${p.seriesName}: <span class="font-weight-600"> &nbsp ${p.data.valueCount} (${p.data.value ?? 0}%) </span> </span>`;
            });
            return fmText;
          },
        },
        legend: {
          bottom: 0
        },
        grid: {
          left: '3%',
          right: '4%',
          top: 20,
          bottom: 40,
          containLabel: true
        },
        xAxis: {
          type: 'value',
          axisLabel: {
            formatter: value => {
              return `${value} %`;
            }
          }
        },
        yAxis: {
          type: 'category',
          data: yAxisData,
          axisLine: {
            onZero: true,
            lineStyle: {
              type: 'solid'
            },
          },
          axisTick: {
            alignWithLabel: true
          }
        },
        series: seriesData,
        color: colors.slice(0, seriesData.length)
      };

    })
  }

  searchChangeDate(value: any) {
    this.filter.dateFrom = value.dateFrom;
    this.filter.dateTo = value.dateTo;
    this.loadDataFromApi();
  }

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : '';
    this.loadDataFromApi();
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

}
