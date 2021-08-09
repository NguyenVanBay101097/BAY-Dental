import { Component, Input, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { CompanyService } from 'src/app/companies/company.service';
import { CustomerReceiptReportFilter, CustomerReceiptReportService } from 'src/app/customer-receipt-reports/customer-receipt-report.service';

@Component({
  selector: 'app-sale-dashboard-ap-cr-chartt',
  templateUrl: './sale-dashboard-ap-cr-chartt.component.html',
  styleUrls: ['./sale-dashboard-ap-cr-chartt.component.css']
})
export class SaleDashboardApCrCharttComponent implements OnInit {
  @Input() companyId: string;
  @Input() dateFrom: Date;
  @Input() dateTo: Date;

  loading = false;
  limit = 20;
  skip = 0;
  total: number;
  state: string;
  public today: Date = new Date(new Date().toDateString());
  isExamination: string;
  isNotTreatment: string;
 
  employeeId: string;
  isAdvanced: boolean;
  public isCollapsed = false;
  public animateChart = true;

  // Pie
  public pieDataCustomer: any[] = [];
  public pieDataNoTreatment: any[] = [];

  constructor(
    private customerReceiptReportService: CustomerReceiptReportService,
    private intlService: IntlService,
    private companyService: CompanyService,
  ) { }

  ngOnInit() {
  }

  loadDataNotreatment() {
    this.loading = true;
    var val = new CustomerReceiptReportFilter();
    val.limit = 0;
    val.offset = this.skip;
    val.companyId = this.companyId || '';
    val.state = 'done';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    this.customerReceiptReportService.getCountCustomerReceiptNoTreatment(val).subscribe(
      (res: any[]) => {
        this.pieDataNoTreatment = res;
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  loadNoTreatmentItems(items: any[]): void {
    for (let i = 0; i < items.length; i++) {
      this.pieDataNoTreatment.push({ category: items[i].name, value: items[i].countCustomerReceipt})
    };
  }

}
