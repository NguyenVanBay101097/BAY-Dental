import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PromotionProgramService, PromotionProgramPaged, PromotionProgramBasic } from '../promotion-program.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-promotion-program-list',
  templateUrl: './promotion-program-list.component.html',
  styleUrls: ['./promotion-program-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PromotionProgramListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];

  constructor(private programService: PromotionProgramService, private route: ActivatedRoute,
    private router: Router, private modalService: NgbModal) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  stateGet(state) {
    switch (state) {
      case 'open':
        return 'Đã xác nhận';
      case 'paid':
        return 'Đã thanh toán';
      default:
        return 'Nháp';
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PromotionProgramPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.programService.getPaged(val).pipe(
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

  createItem() {
    this.router.navigate(['/promotion-programs/form']);
  }

  editItem(item: PromotionProgramBasic) {
    this.router.navigate(['/promotion-programs/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa chương trình khuyến mãi';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.programService.toggleActive([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}



