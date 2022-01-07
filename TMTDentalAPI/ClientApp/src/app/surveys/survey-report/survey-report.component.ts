import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { EChartsOption } from 'echarts';
import * as moment from 'moment';
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';
import { GetReportAssignmentQueryRequest, SurveReportService } from '../survey-report.service';
import { SurveyTagPaged, SurveyTagService } from '../survey-tag.service';
import { SurveyAssignmentPaged, SurveyAssignmentService } from '../survey.service';

@Component({
  selector: 'app-survey-report',
  templateUrl: './survey-report.component.html',
  styleUrls: ['./survey-report.component.css']
})
export class SurveyReportComponent implements OnInit {

  filter = new GetReportAssignmentQueryRequest();
  companies: CompanySimple[] = [];
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("surveyMst", { static: true }) surveyMst: MultiSelectComponent;

  empChartOption: EChartsOption;
  scoreChartOption: EChartsOption;
  questionChartOption: EChartsOption;
  dataScroreReport: any;
  dataEmpReport: any = [];
  dataQuestionReport: any = [];
  employeesData: EmployeeSimple[] = [];
  gridData: GridDataResult;
  loading = false;
  limit = 20;
  offset = 0;
  employeeId = '';
  pagerSettings: any;
  filteredSurveyTags: any[] = [];
  surveyTagId: '';
  constructor(
    private companyService: CompanyService,
    private surveReportService: SurveReportService,
    private employeeService: EmployeeService,
    private intlService: IntlService,
    private surveyAssignmentService: SurveyAssignmentService,
    private surveyTagService: SurveyTagService,
    private printService: PrintService,
    private router: Router,


    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) {this.pagerSettings = config.pagerSettings }

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

      this.surveyMst.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.surveyMst.loading = true)),
        switchMap((value) => this.searchServeyTag(value))
      )
      .subscribe((result: any) => {
        this.filteredSurveyTags = result;
        this.surveyMst.loading = false;
      });

    this.loadDataFromApi();
    this.getSurveyEmployees();
    this.getServeyTagsList();
    this.getDoneServeyAssignments();
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
          axisLabel: {
            interval: 0,
            // rotate: 25 //If the label names are too long you can manage this by rotating the label.
            
          },
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
      var totalPercent = 0;
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
            var percentValue = Math.round(((x * 100 / total) + Number.EPSILON) * 100) / 100;
            totalPercent += percentValue;
            return { value: i == e.data.length? (totalPercent == 100 ? null : 100 - totalPercent) : (percentValue  == 0 ? null : percentValue), valueCount: x };
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
    this.getDoneServeyAssignments();
  }

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : '';
    this.loadDataFromApi();
    this.getDoneServeyAssignments();

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

  getSurveyEmployees() {
    this.employeeService.getAllowSurveyList().subscribe((result: EmployeeSimple[]) => {
      this.employeesData = result
    })
  }

  onSelectEmployee(e){
    this.employeeId = e ? e.id : null;
    this.offset = 0;
    this.getDoneServeyAssignments();
  }

  getDoneServeyAssignments() {
    var paged = this.getDoneSurveyAssignmentPagedFilter();
    paged.limit = this.limit;
    paged.offset = this.offset;
    this.loading = true;
    this.surveyAssignmentService.getPaged(paged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      this.loading = false;
    })
  }

  pageChange(e) {
    this.offset = e.skip;
    this.limit = e.take;
    this.getDoneServeyAssignments();
  }

  searchServeyTag(q? : string) {
    var val = new SurveyTagPaged();
    val.search = q || '';
    val.limit = 0;
    val.offset = 0;
    return this.surveyTagService.getPaged(val);
  }

  getServeyTagsList() {
    this.searchServeyTag().subscribe((result: any) => {
      this.filteredSurveyTags = result.items;
    })
  }

  onSurveyChange(value) {
    var surveyTagId = value.map(x => x.id);
    this.surveyTagId = surveyTagId;
    this.offset = 0;
    this.getDoneServeyAssignments();
  }

  exportDoneSurveyListExcel() {
    var paged = this.getDoneSurveyAssignmentPagedFilter();

    this.surveyAssignmentService.exportDoneSurveyAssignmentExcel(paged).subscribe((rs) => {
      let filename = "DanhSachKhaoSatHoanThanh";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  printDoneSurveyList() {
    var paged = this.getDoneSurveyAssignmentPagedFilter();

    this.surveyAssignmentService.printDoneSurvey(paged).subscribe(result => {
      this.printService.printHtml(result);
    })
  }

  getDoneSurveyAssignmentPagedFilter() {
    var paged = new SurveyAssignmentPaged();
    paged.limit = 0;
    paged.offset = 0;
    paged.employeeId = this.employeeId ? this.employeeId : '';
    paged.dateFrom = this.filter.dateFrom ? moment(this.filter.dateFrom).format('YYYY/MM/DD') : '';
    paged.dateTo = this.filter.dateTo ? moment(this.filter.dateTo).format('YYYY/MM/DD') : '';
    paged.status = 'done';
    paged.surveyTagId = this.surveyTagId ? this.surveyTagId : '';
    paged.companyId = this.filter.companyId ? this.filter.companyId : '';

    return paged;
  }

  clickItem(item) {
    var id = item.dataItem.id;
    this.router.navigate(['/surveys/form'], { queryParams: { id: id } });
  }
}
