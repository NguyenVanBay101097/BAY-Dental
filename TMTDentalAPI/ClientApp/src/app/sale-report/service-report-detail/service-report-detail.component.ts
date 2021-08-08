import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { DataResult } from '@progress/kendo-data-query';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { ToothBasic } from 'src/app/teeth/tooth.service';
import { SaleReportService, ServiceReportDetailReq, ServiceReportDetailRes } from '../sale-report.service';

@Component({
  selector: 'app-service-report-detail',
  templateUrl: './service-report-detail.component.html',
  styleUrls: ['./service-report-detail.component.css']
})
export class ServiceReportDetailComponent implements OnInit {


  @Input() parentFilter;
  filter: ServiceReportDetailReq = new ServiceReportDetailReq();
  @Input() parent: any;
  gridData: GridDataResult;
  loading = false;
  stateDisplay= {
    sale:"Đang điều trị",
    done: "Hoàn thành"
  }

  constructor(
    private saleReportService: SaleReportService,
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.loadReport();
  }

  initFilterData() {
    this.filter = Object.assign({}, this.parentFilter);
    this.filter.limit = 20;
    this.filter.offset = 0;
    this.filter.productId = this.parent.productId || '';
    this.filter.companyId = this.filter.companyId || '';
    this.filter.dateFrom = this.parent.date? moment(this.parent.date).format('YYYY/MM/DD')
                          : ( this.filter.dateFrom ? moment(this.filter.dateFrom).format('YYYY/MM/DD') : '');
    this.filter.dateTo = this.parent.date? moment(this.parent.date).format('YYYY/MM/DD')
                          : ( this.filter.dateTo ? moment(this.filter.dateTo).format('YYYY/MM/DD') : '');
  }

  loadReport() {
    var val = Object.assign({}, this.filter);
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    val.employeeId = val.employeeId || '';
    (val.active as any) = (val.active && val.active !== null)? val.active : '';
    this.loading = true;
    this.saleReportService.getServiceReportDetailPaged(val).pipe(
      map(res => {
        return <DataResult>{
          data: res.items,
          total: res.totalItems
        }
      })
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    },
      err => {
        this.loading = false;
      });
  }

  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadReport();
  }

  getTeethDisplay(teeth: ToothBasic[]){
    return teeth.map(x=> x.name).join(' ')
  }
}
