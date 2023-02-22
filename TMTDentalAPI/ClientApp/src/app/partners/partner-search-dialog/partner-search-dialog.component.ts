import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent, SelectionEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-search-dialog',
  templateUrl: './partner-search-dialog.component.html',
  styleUrls: ['./partner-search-dialog.component.css']
})
export class PartnerSearchDialogComponent implements OnInit {
  domain: any;
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  title: string;

  constructor(private partnerService: PartnerService, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadDataFromApi();

      this.searchUpdate.pipe(
        debounceTime(400),
        distinctUntilChanged())
        .subscribe(value => {
          this.loadDataFromApi();
        });
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = Object.assign({}, this.domain);
    val.limit = this.limit;
    val.offset = this.skip;
    val.searchNamePhoneRef = this.search || '';

    this.partnerService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
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

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  selectionChange(event: SelectionEvent): void {
    this.activeModal.close(event.selectedRows);
  }

  searchEnter() {
    if (this.gridData.data.length == 1) {
      this.activeModal.close([{ dataItem: this.gridData.data[0] }]);
    }
  }
}

