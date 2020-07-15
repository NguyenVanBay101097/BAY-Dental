import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-account-payment-list-t',
  templateUrl: './account-payment-list-t.component.html',
  styleUrls: ['./account-payment-list-t.component.css']
})
export class AccountPaymentListTComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  resultSelection: string;

  search: string;
  searchUpdate = new Subject<string>();
  
  constructor(private route: ActivatedRoute, private modalService: NgbModal, ) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.resultSelection = params.get('result_selection');
      // this.loadDataFromApi();
    });

  }

}
