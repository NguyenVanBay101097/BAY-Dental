import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { debounce } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { ServiceCardTypeService } from '../../service-card-type.service';

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
    private router: Router,
    private cardService: ServiceCardTypeService,
  ) {this.pagerSettings = config.pagerSettings; }

  ngOnInit(): void {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => this.loadDataFromApi());
    this.loadDataFromApi();
  }

  loadDataFromApi(){
    var val = {search: this.search, offset: this.skip, limit: this.limit};
    this.cardService.getPreferentialCards(val).subscribe(result => {
      this.gridData = {
        data: result.items,
        total: result.totalItems
      }
    })
  }

  createCardLevel(){
    this.router.navigate(['card-types/preferential-cards/form']);
  }

  editItem(item){
    this.router.navigate(['card-types/preferential-cards/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item){

  }

  pageChange(event: PageChangeEvent){
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  getCardLevel(item){
    let period = item.period ? (item.period == 'year' ? 'Năm' : 'Tháng') : '';
    let nbrPeriod = item.nbrPeriod || '';
    return period + ' ' + nbrPeriod;
  }
}
