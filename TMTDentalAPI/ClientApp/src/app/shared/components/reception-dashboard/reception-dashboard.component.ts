import { Component, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin } from 'rxjs';
import { AppointmentPaged } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { LaboOrderReportInput, LaboOrderReportOutput, LaboOrderService } from 'src/app/labo-orders/labo-order.service';
import { SaleReportItem, SaleReportSearch, SaleReportService } from 'src/app/sale-report/sale-report.service';

@Component({
  selector: 'app-reception-dashboard',
  templateUrl: './reception-dashboard.component.html',
  styleUrls: ['./reception-dashboard.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ReceptionDashboardComponent implements OnInit {

  saleReport: SaleReportItem;
  appointmentStateCount = {};
  laboOrderReport: LaboOrderReportOutput;

  constructor(private intlService: IntlService, 
    private appointmentService: AppointmentService, 
    private saleReportService: SaleReportService, 
    private laboOrderService: LaboOrderService) { }

  ngOnInit() {
    this.loadSaleReport();
    this.loadAppoiment();
    this.loadLaboOrderReport();
  }

  loadSaleReport() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    this.saleReportService.getReport(val).subscribe(
      result => {
        if (result.length) {
          this.saleReport = result[0];
        } else {
          this.saleReport = null;
        }
      }, 
      error => {

      }
    );
  }

  loadAppoiment() {
    var states = ["confirmed", "done", "cancel"];

    var obs = states.map(state => {
      var val = new AppointmentPaged();
      val.dateTimeFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
      val.dateTimeTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
      val.state = state;
      return this.appointmentService.getPaged(val);
    });

    forkJoin(obs).subscribe((result: any) => {
      result.forEach(item => {
        if (item.items.length) {
          var state = item.items[0].state;
          if (state == "done") {
            this.appointmentStateCount[state] = item.totalItems;
          }
          if (this.appointmentStateCount['all']) {
            this.appointmentStateCount['all'] += item.totalItems;
          } else {
            this.appointmentStateCount['all'] = item.totalItems;
          }
        }
      });
    });
  }

  loadLaboOrderReport() {
    var val = new LaboOrderReportInput();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    console.log(val);
    this.laboOrderService.getLaboOrderReport(val).subscribe(
      result => {
        this.laboOrderReport = result;
      },
      error => {

      }
    );
  }
}
