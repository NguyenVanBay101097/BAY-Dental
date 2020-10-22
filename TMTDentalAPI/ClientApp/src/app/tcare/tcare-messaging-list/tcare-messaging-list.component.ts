import { TCareMessagingPaged, TcareMessagingService } from './../tcare-messaging.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { TCareScenarioBasic, TCareScenarioPaged, TCareScenarioService } from '../tcare-scenario.service';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-tcare-messaging-list',
  templateUrl: './tcare-messaging-list.component.html',
  styleUrls: ['./tcare-messaging-list.component.css']
})
export class TcareMessagingListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  title = 'Kịch bản';
  loading = false;
  opened = false;
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  //searchPartner: PartnerSimple;
  searchCateg: TCareScenarioBasic;
  filteredCategs: TCareScenarioBasic[];
  @ViewChild("categCbx", { static: true }) categCbx: ComboBoxComponent;

  constructor(private tcareScenarioService: TCareScenarioService, private tcareMessagingService: TcareMessagingService, private intlService: IntlService) { }

  ngOnInit() {
    // var today = new Date(new Date().toDateString());
    // this.dateFrom = today;
    // this.dateTo = today;

    this.loadDataFromApi();

    this.categCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.categCbx.loading = true)),
        switchMap((value) => this.searchScenario(value))
      )
      .subscribe((result) => {
        this.filteredCategs = result;
        this.categCbx.loading = false;
      });

    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadFilteredCategs();
  }

  onChangeDate(value: any) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  searchScenario(search?: string) {
    var val = new TCareScenarioPaged();
    val.search = search;
    return this.tcareScenarioService.autocomplete(val);
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();

  }

  loadDataFromApi() {
    this.loading = true;
    var val = new TCareMessagingPaged();
    if (this.dateFrom) {
      val.dateFrom = this.intlService.formatDate(this.dateFrom, "d", "en-US");
    }
    if (this.dateTo) {
      val.dateTo = this.intlService.formatDate(this.dateTo, "d", "en-US");
    }
    val.tCareScenarioId = this.searchCateg ? this.searchCateg.id : "";

    this.tcareMessagingService.getPaged(val).pipe(
      map((response: any) =>
        (<GridDataResult>{
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

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
  }

  loadFilteredCategs() {
    this.searchScenario().subscribe(
      (result) => (this.filteredCategs = result)
    );
  }

  onCategChange(value) {
    this.searchCateg = value;
    this.loadDataFromApi();
  }

  getState(state: string) {
    switch (state) {
      case 'in_queue':
        return 'Chờ gửi';
      case 'sending':
        return 'Đang gửi';
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Mới';
    }
  }



}
