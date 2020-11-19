import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SalaryPaymentPaged, SalaryPaymentService } from '../salary-payment.service';

@Component({
  selector: 'app-salary-payment-list',
  templateUrl: './salary-payment-list.component.html',
  styleUrls: ['./salary-payment-list.component.css']
})
export class SalaryPaymentListComponent implements OnInit {
  type: string;
  search: string;
  searchUpdate = new Subject<string>();
  loading = false;
  limit = 20;
  skip = 0;
  gridData: GridDataResult;

  constructor(private route: ActivatedRoute, private modalService: NgbModal, 
    private salaryPaymentService: SalaryPaymentService, private router: Router) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.type = params.get('type');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new SalaryPaymentPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.type;
    this.salaryPaymentService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }
}
