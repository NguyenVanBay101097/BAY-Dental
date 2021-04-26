import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SmsSmsPaged, SmsSmsService } from '../sms-sms.service';

@Component({
  selector: 'app-sms-statisticaddd',
  templateUrl: './sms-statistic.component.html',
  styleUrls: ['./sms-statistic.component.css'],
})
export class SmsStatisticComponent implements OnInit {
  gridData: GridDataResult;

  filteredState: any[] = [
    { name: 'Gửi thất bại', value: 'fails' },
    { name: 'Gửi Thành công', value: 'success' },
    { name: 'Đang gửi', value: 'sending' }
  ]

  state: string;
  limit: number = 20;
  skip: number = 0;
  search: string;
  searchUpdate = new Subject<string>();

  constructor(
    private smsSmsService: SmsSmsService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    var val = new SmsSmsPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = this.state || '';

    this.smsSmsService.getPaged(val)
      .pipe(map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))).subscribe(res => {
        this.gridData = res;
      })
  }

  onChangeState(state) {
    if (state) {
      this.state = state.value;
      this.skip = 0;
      this.loadDataFromApi();
    }
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

}
