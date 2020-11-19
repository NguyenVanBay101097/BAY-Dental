import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SalaryPaymentService } from 'src/app/shared/services/salary-payment.service';
import { SalaryPaymentBindingDirective } from 'src/app/shared/directives/salary-payment-binding.directive';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';

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

  @ViewChild(SalaryPaymentBindingDirective, { static: true }) dataBinding: SalaryPaymentBindingDirective;

  gridFilter: CompositeFilterDescriptor = {
    logic: "and",
    filters: []
  };
  gridSort = [];
  advanceFilter: any = {
    params: {}
  };

  constructor(private route: ActivatedRoute, private modalService: NgbModal, 
    private salaryPaymentService: SalaryPaymentService, private router: Router) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.dataBinding.filter = this.generateFilter();
        this.refreshData();
      });
  }

  updateFilter() {
    this.gridFilter = this.generateFilter();
  }

  refreshData() {
    this.dataBinding.rebind();
  }

  generateFilter() {
    var filter: CompositeFilterDescriptor = {
      logic: "and",
      filters: []
    };

    if (this.search) {
      filter.filters.push({
        logic: "or",
        filters: [
          { field: "Name", operator: "contains", value: this.search }
        ]
      });
    }

    return filter;

  }
}
