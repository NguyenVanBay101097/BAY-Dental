import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-sms-care-after-order-form-automatic',
  templateUrl: './sms-care-after-order-form-automatic.component.html',
  styleUrls: ['./sms-care-after-order-form-automatic.component.css']
})
export class SmsCareAfterOrderFormAutomaticComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  offset = 0;
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  state: string;
  filterStatus = [
    { name: "Kích hoạt", value: "active" },
    { name: "Không kích hoạt", value: "inactive" }
  ];

  constructor() { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offset = 0;
        this.loadDataFromApi();
      });
  }

  onStatusChange(event) {
    this.state = event ? event.value : '';
    this.offset = 0;
    this.loadDataFromApi();
  }

  createItem() {

  }

  editItem() {

  }

  deleteItem() {

  }

  loadDataFromApi() {

  }

  pageChange(event): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }
}
