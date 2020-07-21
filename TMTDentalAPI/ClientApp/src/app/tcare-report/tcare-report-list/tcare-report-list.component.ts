import { Component, OnInit, ViewChild } from "@angular/core";
import { GridDataResult, PageChangeEvent } from "@progress/kendo-angular-grid";
import {
  ReportTCareItem,
  TcareReportService,
  ReportTCareSearch,
} from "../tcare-report.service";
import { IntlService } from "@progress/kendo-angular-intl";
import { TCareScenarioService, TCareScenarioBasic } from "src/app/tcare/tcare-scenario.service";
import { ActivatedRoute } from "@angular/router";
import { TCareScenarioPaged } from "src/app/tcare/tcare.service";
import { aggregateBy } from "@progress/kendo-data-query";
import { Subject } from "rxjs";
import { debounceTime, distinctUntilChanged, tap, switchMap } from "rxjs/operators";
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';

@Component({
  selector: "app-tcare-report-list",
  templateUrl: "./tcare-report-list.component.html",
  styleUrls: ["./tcare-report-list.component.css"],
})
export class TCareReportListComponent implements OnInit {
  loading = false;
  items: ReportTCareItem[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  //searchPartner: PartnerSimple;
  searchCateg: TCareScenarioBasic;
  filteredCategs: TCareScenarioBasic[];
  @ViewChild("categCbx", { static: true }) categCbx: ComboBoxComponent;
  
  searchUpdate = new Subject<string>();
  public monthStart: Date = new Date( new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(),new Date().getMonth() + 1,0).getDate())).toDateString());

  public total: any;
  public aggregates: any[] = [{ field: "end", aggregate: "sum" }];
  constructor(
    private reportService: TcareReportService,
    private intlService: IntlService,
    private tcareScenarioService: TCareScenarioService,
  ) {}

  ngOnInit() {
    var date = new Date();
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

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
    var val = new ReportTCareSearch();
    if (this.dateFrom) {
      val.dateFrom = this.intlService.formatDate(this.dateFrom, "d", "en-US");
    }
    if (this.dateTo) {
      val.dateTo = this.intlService.formatDate(this.dateTo, "d", "en-US");
    }
    val.tCareScenarioId = this.searchCateg ? this.searchCateg.id : "";

    this.reportService.getReport(val).subscribe(
      (res) => {
        this.items = res;
        this.total = aggregateBy(this.items, this.aggregates);
        this.loadItems();
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
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

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length,
    };
  }
}
