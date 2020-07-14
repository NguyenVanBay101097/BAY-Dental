import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountAccountFormComponent } from '../account-account-form/account-account-form.component';
import { AccountAccountService, accountAccountPaged, accountAccountBasic } from '../account-account.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-account-account-list',
  templateUrl: './account-account-list.component.html',
  styleUrls: ['./account-account-list.component.css']
})
export class AccountAccountListComponent implements OnInit {
  loading = false;
  items: any[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  resultSelection: string;

  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, private modalService: NgbModal, 
    private accountAccountService: AccountAccountService) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.resultSelection = params.get('result_selection');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }
  
  loadDataFromApi() {
    this.loading = true;
    var val = new accountAccountPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.resultSelection;

    this.accountAccountService.getPaged(val).pipe(
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

  convertResultSelection() {
    switch (this.resultSelection) {
      case 'thu':
        return 'loại thu';
      case 'chi':
        return 'loại chi';
    }
  }

  createItem() {
    const modalRef = this.modalService.open(AccountAccountFormComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm ' + this.convertResultSelection();
    modalRef.componentInstance.type = this.resultSelection;
    modalRef.result.then(() => {
       this.loadDataFromApi();
    }, () => {

    });
  }

  editItem(item: accountAccountBasic) {
    const modalRef = this.modalService.open(AccountAccountFormComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa ' + item.name;
    modalRef.componentInstance.itemId = item.id;
    modalRef.result.then(() => {
       this.loadDataFromApi();
    }, () => {

    });
  }

  deleteItem(item: accountAccountBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + this.convertResultSelection();

    modalRef.result.then(() => {
      this.accountAccountService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }
}
