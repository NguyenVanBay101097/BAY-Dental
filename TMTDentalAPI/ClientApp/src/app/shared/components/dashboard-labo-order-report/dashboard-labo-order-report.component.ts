import { Component, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { LaboOrderReportInput, LaboOrderService } from 'src/app/labo-orders/labo-order.service';

@Component({
  selector: 'app-dashboard-labo-order-report',
  templateUrl: './dashboard-labo-order-report.component.html',
  styleUrls: ['./dashboard-labo-order-report.component.css']
})
export class DashboardLaboOrderReportComponent implements OnInit {

  laboOrderStateCount: any = {};
  laboOrderStates: any[] = [
    { value: 'danhan', text: 'LABO ĐÃ NHẬN'},
    { value: 'toihan', text: 'LABO TỚI HẸN'},
  ]

  constructor(private intlService: IntlService, 
    private authService: AuthService, 
    private laboOrderService: LaboOrderService, 
  ) { }

  ngOnInit() {
    this.loadLaboOrderStateCount();
  }

  loadLaboOrderStateCount() {
    forkJoin(this.laboOrderStates.map(x => {
      var val = new LaboOrderReportInput();
      val.state = x.value;
      val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
      val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
      return this.laboOrderService.getCountLaboOrder(val).pipe(
        switchMap(count => of({state: x.value, count: count}))
      );
    })).subscribe((result) => {
      result.forEach(item => {
        this.laboOrderStateCount[item.state] = item.count;
      });
    });
  }
}
