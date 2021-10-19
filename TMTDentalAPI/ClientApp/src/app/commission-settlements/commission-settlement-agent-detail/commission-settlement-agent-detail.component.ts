import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-commission-settlement-agent-detail',
  templateUrl: './commission-settlement-agent-detail.component.html',
  styleUrls: ['./commission-settlement-agent-detail.component.css']
})
export class CommissionSettlementAgentDetailComponent implements OnInit {
  dateFrom: Date;
  dateTo: Date;
  search: string = '';
  searchUpdate = new Subject<string>();
  agentTypes : any[] = [
    {id:'doi_tac',name:'Đối tác'},
    {id: 'khach_hang', name: 'Khách hàng'}
  ]
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor() { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  onSearchDateChange(e){
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    this.loadDataFromApi();
  }

  loadDataFromApi(){

  }

}
