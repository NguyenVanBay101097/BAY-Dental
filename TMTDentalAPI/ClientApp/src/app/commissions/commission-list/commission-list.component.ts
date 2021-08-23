import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { CommissionService, CommissionPaged, Commission } from '../commission.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-commission-list',
  templateUrl: './commission-list.component.html',
  styleUrls: ['./commission-list.component.css']
})
export class CommissionListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pageSizes = [20, 50, 100, 200];
  loading = false;

  search: string;
  searchUpdate = new Subject<string>();

  title = 'Hoa hồng';

  constructor(private commissionService: CommissionService,
    private modalService: NgbModal, private router: Router) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CommissionPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.commissionService.getPaged(val).pipe(
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

  onPageSizeChange(value: number): void {
    this.skip = 0;
    this.limit = value;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/commissions/form']);
  }

  editItem(item: Commission) {
    this.router.navigate(['/commissions/form'], {
      queryParams: {
        id: item.id
      },
    });
  }

  deleteItem(item: Commission) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;
    modalRef.result.then(() => {
      this.commissionService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, err => {
        console.log(err);
      });
    }, () => {
    });
  }
}
