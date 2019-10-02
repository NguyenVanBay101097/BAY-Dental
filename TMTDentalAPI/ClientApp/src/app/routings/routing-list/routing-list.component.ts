import { Component, OnInit, Inject, ViewChild, ElementRef } from '@angular/core';
import { debounceTime, switchMap, tap, map, distinctUntilChanged } from 'rxjs/operators';
import { WindowRef, WindowService, WindowCloseResult, DialogService, DialogRef, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { RoutingPaged, RoutingService, RoutingBasic } from '../routing.service';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-routing-list',
  templateUrl: './routing-list.component.html',
  styleUrls: ['./routing-list.component.css']
})
export class RoutingListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  search: string;
  searchUpdate = new Subject<string>();

  constructor(private routingService: RoutingService, private windowService: WindowService,
    private dialogService: DialogService, public intl: IntlService, private router: Router) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new RoutingPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.routingService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      console.log(res);
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  createItem() {
    this.router.navigate(['routings/create']);
  }

  editItem(item: RoutingBasic) {
    this.router.navigate(['routings/edit/', item.id]);
  }

  deleteItem(item) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'Xóa sản phẩm',
      content: 'Bạn có chắc chắn muốn xóa?',
      actions: [
        { text: 'Hủy bỏ', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ],
      width: 450,
      height: 200,
      minWidth: 250
    });

    dialog.result.subscribe((result) => {
      if (result instanceof DialogCloseResult) {
        console.log('close');
      } else {
        console.log('action', result);
        if (result['value']) {
          this.routingService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }

}
