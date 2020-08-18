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
    return `${args.dataItem.name || 'Chưa xác định'}: ${args.dataItem.total} (${this.intl.formatNumber(args.dataItem.percentage, 'n2')}%)`;
  }

  getReportLocationCompanyWard() {
    var value = {
      dateFrom: this.dateFrom ? this.intl.formatDate(this.dateFrom, "yyyy-MM-ddTHH:mm") : null,
      dateTo: this.dateTo ? this.intl.formatDate(this.dateTo, "yyyy-MM-ddTHH:mm") : null,
    }

    this.partnerService.getReportLocationCompanyWard(value).subscribe(
      res => {
        this.pieDataWard = res;
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
    var val = {
      dateFrom: this.dateFrom ? this.intl.formatDate(this.dateFrom, "yyyy-MM-ddTHH:mm") : null,
      dateTo: this.dateTo ? this.intl.formatDate(this.dateTo, "yyyy-MM-ddTHH:mm") : null
    };
  
    this.partnerService.getReportLocationCompanyDistrict(val).subscribe(
      res => {
        this.pieDataDistrict = res;
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
