import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ServiceCardTypeCuDialogComponent } from 'src/app/service-card-types/service-card-type-cu-dialog/service-card-type-cu-dialog.component';
import { ServiceCardTypePaged } from 'src/app/service-card-types/service-card-type-paged';
import { ServiceCardTypeService } from 'src/app/service-card-types/service-card-type.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-service-card-order-type-list',
  templateUrl: './service-card-order-type-list.component.html',
  styleUrls: ['./service-card-order-type-list.component.css']
})
export class ServiceCardOrderTypeListComponent implements OnInit {
  @Output() typeCard = new EventEmitter<any>();
  gridData: GridDataResult;
  page: number = 1;
  pageSize: number = 10;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  typeCardList: any[] = [];
  total: number;

  constructor(private cardTypeService: ServiceCardTypeService,
    private modalService: NgbModal) { }

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
    var val = new ServiceCardTypePaged();
    val.limit = 100;
    val.search = this.search || '';

    this.cardTypeService.getPaged(val).subscribe((result: any) => {
      this.typeCardList = result.items;
      this.total = result.totalItems;
    });
  }

  onClickData(val: any){
    this.typeCard.emit(val);
  }

  pageChange(page): void {
    this.page = page;
    this.loadDataFromApi();
  }

}
