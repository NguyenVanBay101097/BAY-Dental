import { Component, OnInit, ViewChild } from '@angular/core';
import { NgSelectComponent } from '@ng-select/ng-select';
import { Observable, of, Subject } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { SearchAllService } from '../search-all.service';

@Component({
  selector: 'app-search-all',
  templateUrl: './search-all.component.html',
  styleUrls: ['./search-all.component.css']
})
export class SearchAllComponent implements OnInit {
  selectedResult: any;
  resultSelection: string = 'all';
  search$: Observable<any[]>;
  searchLoading = false;
  searchInput$ = new Subject<string>();

  options: any[] = [
    { value: 'all', text: 'Tất cả' },
    { value: 'customer', text: 'Khách hàng' },
    { value: 'supplier', text: 'Nhà cung cấp' },
    { value: 'sale-order', text: 'Phiếu điều trị' },
  ];

  constructor(private searchAllService: SearchAllService) { }

  ngOnInit() {
    this.search$ = this.searchInput$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => this.searchLoading = true),
      switchMap(term => this.onSearch(term).pipe(
          catchError(() => of([])), // empty list on error
          tap(() => this.searchLoading = false)
      ))
    );
  }

  onChangeResultSelection(value) {
    this.resultSelection = value;
  }

  onSearchChange(item) {
    setTimeout(() => {
      this.selectedResult = null;
    });

    switch (item.type) {
      case "customer":
        window.open(`/partners/customer/${item.id}/overview`, '_blank');
        break;
      case "supplier":
        window.open(`/partners/supplier/${item.id}/info`, '_blank');
        break;
      case "sale-order":
        window.open(`/sale-orders/form?id=${item.id}`, '_blank');
        break;
      default:
        break;
    }
  }

  onSearch(q?: string) {
    var value = {
      limit: 20,
      search: q || '',
      resultSelection: this.resultSelection ? this.resultSelection : ''
    }
    return this.searchAllService.getAll(value);
  }

}
