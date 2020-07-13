import { Component, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { PartnerService, PartnerReportLocationDistrict, PartnerReportLocationWard } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-partner-report-location-chart-pie',
  templateUrl: './partner-report-location-chart-pie.component.html',
  styleUrls: ['./partner-report-location-chart-pie.component.css']
})
export class PartnerReportLocationChartPieComponent implements OnInit {
  public pieDataDistrict: any[] = [];
  public pieDataWard: any[] = [];
  dateFrom: Date;
  dateTo: Date;
  date: Date;
  districtCode: string;
  constructor(
    private intl: IntlService,
    private partnerService: PartnerService
  ) {
    this.getReportLocationCompanyDistrict();
    this.getReportLocationCompanyWard();
  }

  ngOnInit() {
    this.labelContent = this.labelContent.bind(this);
  }


  labelContent(args: LegendLabelsContentArgs): string {
    return `${args.dataItem.category}: ${args.dataItem.total} người - Chiếm ${this.intl.formatNumber(args.dataItem.value, 'p2')}`;
  }

  getReportLocationCompanyWard() {
    var value = {
      dateFrom: this.intl.formatDate(this.dateFrom, "yyyy-MM-ddTHH:mm"),
      dateTo: this.intl.formatDate(this.dateTo, "yyyy-MM-ddTHH:mm"),
      districtCode: this.districtCode ? this.districtCode : ''
    }
    this.partnerService.getReportLocationCompanyWard(value).subscribe(
      res => {
        this.pieDataWard = [];
        res.forEach(item => {
          var model = {
            category: '',
            value: 0,
            total: ''
          }
          if (item.wardCode && item.wardName) {
            model.total = item.total.toString();
            model.category = item.wardName;
          } else {
            model.category = "Khác";
            model.total = item.total.toString();
          }
          model.value = Number.parseFloat(item.percentage.toFixed(2)) / 100;

          this.pieDataWard.push(model);
        });
        console.log(this.pieDataWard);

      }
    )
  }

  // changeDistrict(value) {
  //   console.log('seriesClick', value);
  //   if (value && value.dataItem && value.dataItem.code)
  //     this.districtCode = value.dataItem.code;
  //   this.getReportLocationCompanyWard();
  // }

  getReportLocationCompanyDistrict() {
    if (this.dateFrom && this.dateTo)
      var val = {
        dateFrom: this.intl.formatDate(this.dateFrom, "yyyy-MM-ddTHH:mm"),
        dateTo: this.intl.formatDate(this.dateTo, "yyyy-MM-ddTHH:mm")
      }
    this.partnerService.getReportLocationCompanyDistrict(val).subscribe(
      res => {
        this.pieDataDistrict = [];
        res.forEach(item => {
          var model = {
            category: '',
            value: 0,
            total: ''
          }
          if (item.districtName && item.districtCode) {
            model.total = item.total.toString();
            model.category = item.districtName;
          } else {
            model.category = "Khác";
            model.total = item.total.toString();
          }
          model.value = Number.parseFloat(item.percentage.toFixed(2)) / 100;
          this.pieDataDistrict.push(model);
        });
        console.log(this.pieDataDistrict);
      }
    )
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.getReportLocationCompanyDistrict();
    this.getReportLocationCompanyWard();
    if (data) {
      this.districtCode == null;
    }
  }

}
