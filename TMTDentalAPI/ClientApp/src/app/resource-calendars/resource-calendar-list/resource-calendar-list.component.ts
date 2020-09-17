import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ResourceCalendarService, ResourceCalendarPaged } from '../resource-calendar.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-resource-calendar-list',
  templateUrl: './resource-calendar-list.component.html',
  styleUrls: ['./resource-calendar-list.component.css']
})
export class ResourceCalendarListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  title = 'Lịch làm việc';
  loading = false;
  opened = false;
  searchUpdate = new Subject<string>();
  search: string;


  constructor(
    private modalService: NgbModal,
    private resourceCalendarService: ResourceCalendarService,
    private router: Router
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ResourceCalendarPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.resourceCalendarService.getPage(val).pipe(
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

  createItem() {
    this.router.navigateByUrl("resource-calendars/form");
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item) {
    this.router.navigateByUrl("resource-calendars/form?id=" + item.id)
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Xóa: " + this.title;
    modalRef.result.then(
      () => {
        this.resourceCalendarService.delete(item.id).subscribe(
          () => {
            this.loadDataFromApi();
          },
          (err) => {
            console.log(err);
          }
        );
      },
      () => { }
    );
  }

}
