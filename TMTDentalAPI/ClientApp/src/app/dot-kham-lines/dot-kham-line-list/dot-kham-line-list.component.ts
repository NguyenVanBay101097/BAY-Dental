import { Component, OnInit } from "@angular/core";
import { GridDataResult, PageChangeEvent } from "@progress/kendo-angular-grid";
import { IntlService } from "@progress/kendo-angular-intl";
import { take } from "lodash";
import { Subject } from "rxjs";
import { debounceTime, distinctUntilChanged, map } from "rxjs/operators";
import {
  DotKhamLinePagedResult,
  DotKhamLinePaging,
  DotKhamLineService,
} from "src/app/dot-kham-lines/dot-kham-line.service";

@Component({
  selector: "app-dot-kham-line-list",
  templateUrl: "./dot-kham-line-list.component.html",
  styleUrls: ["./dot-kham-line-list.component.css"],
})
export class DotKhamLineListComponent implements OnInit {
  dateFrom: Date = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
  dateTo: Date = new Date(
    new Date().getFullYear(),
    new Date().getMonth() + 1,
    0
  );
  skip = 0;
  pageSize = 20;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;

  constructor(
    private lineService: DotKhamLineService,
    private intlService: IntlService
  ) {}

  ngOnInit() {
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((val) => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var page = new DotKhamLinePaging();
    page.limit = this.pageSize;
    page.offset = this.skip;
    page.search = this.search ? this.search : "";
    page.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd")
    page.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59")

    this.loading = true;
    this.lineService
      .getPaged(page)
      .pipe(
        map(
          (res) =>
            <GridDataResult>{
              data: res.items,
              total: res.totalItems,
            }
        )
      )
      .subscribe(
        (res) => {
          this.gridData = res;
        },
        () => {},
        () => {
          this.loading = false;
        }
      );
  }

  onDateSearchChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  pageChange(e: PageChangeEvent) {
    this.skip = e.skip;
    this.loadDataFromApi();
  }

  getTeethDisplay(teeth) {
    if(teeth.length > 0)
    return teeth.map(x=> x.name).join(',') + ';';

    return;
  }
}
