import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { debounce } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-preferential-card-list',
  templateUrl: './preferential-card-list.component.html',
  styleUrls: ['./preferential-card-list.component.css']
})
export class PreferentialCardListComponent implements OnInit {
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;
  pagerSettings: any;
  limit = 20;
  skip = 0;
  loading = false;
  search: string = '';
  constructor(
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
    private router: Router
  ) {this.pagerSettings = config.pagerSettings; }

  ngOnInit(): void {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => this.loadDataFromApi());
  }

  loadDataFromApi(){
    var val = {search: this.search, offset: this.skip, limit: this.limit};
  }

  createCardLevel(){
    this.router.navigate(['card-types/preferential-cards/form']);
  }

  editItem(item){

  }

  deleteItem(item){

  }

  pageChange(event: PageChangeEvent){
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  getPeriod(item){
    let period = item ? item.period : '';
    let nbrPeriod = item ? item.nbrPeriod : '';
    return period + ' ' + nbrPeriod;
  }
}
