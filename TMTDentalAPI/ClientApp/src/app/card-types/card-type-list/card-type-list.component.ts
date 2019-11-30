import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { NgbDate, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { IntlService } from '@progress/kendo-angular-intl';
import { CardTypeService, CardTypePaged, CardTypeBasic } from '../card-type.service';

@Component({
  selector: 'app-card-type-list',
  templateUrl: './card-type-list.component.html',
  styleUrls: ['./card-type-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CardTypeListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  search: string;
  searchUpdate = new Subject<string>();

  constructor(private cardTypeService: CardTypeService,
    private router: Router,
    private modalService: NgbModal, private intlService: IntlService) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CardTypePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.cardTypeService.getPaged(val).pipe(
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
    this.router.navigate(['/card-types/create']);
  }

  editItem(item: CardTypeBasic) {
    this.router.navigate(['/card-types/edit/', item.id]);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa loại thẻ';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.cardTypeService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}



