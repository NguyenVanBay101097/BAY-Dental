import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DialogCloseResult, DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { LaboOrderLineCuDialogComponent } from '../labo-order-line-cu-dialog/labo-order-line-cu-dialog.component';
import { LaboOrderLineDisplay, LaboOrderLinePaged, LaboOrderLineService } from '../labo-order-line.service';

@Component({
  selector: 'app-labo-order-line-list',
  templateUrl: './labo-order-line-list.component.html',
  styleUrls: ['./labo-order-line-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class LaboOrderLineListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  opened = false;
  search: string;
  searchCustomer: string;
  searchSupplier: string;
  searchProduct: string;
  sentDateFrom: Date;
  sentDateTo: Date;
  receivedDateFrom: Date;
  receivedDateTo: Date;
  searchUpdate = new Subject<string>();

  constructor(
    private laboOrderLineService: LaboOrderLineService,
    private dialogService: DialogService,
    private intlService: IntlService,
    private modalService: NgbModal,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
  }

  onChangeDate(value: Date) {
    setTimeout(() => {
      this.loadDataFromApi();
    }, 200);
  }

  stateGet(state) {
    switch (state) {
      case 'purchase':
        return '????n h??ng';
      case 'done':
        return '???? kh??a';
      case 'cancel':
        return '???? h???y';
      default:
        return 'Nh??p';
    }
  }


  loadDataFromApi() {
    this.loading = true;
    var val = new LaboOrderLinePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    if (this.search) {
      val.search = this.search;
    }
    if (this.sentDateFrom) {
      val.sentDateFrom = this.intlService.formatDate(this.sentDateFrom, 'd', 'en-US');
    }
    if (this.sentDateTo) {
      val.sentDateTo = this.intlService.formatDate(this.sentDateTo, 'd', 'en-US');
    }
    if (this.receivedDateFrom) {
      val.receivedDateFrom = this.intlService.formatDate(this.receivedDateFrom, 'd', 'en-US');
    }
    if (this.receivedDateTo) {
      val.receivedDateTo = this.intlService.formatDate(this.receivedDateTo, 'd', 'en-US');
    }

    this.laboOrderLineService.getPaged(val).pipe(
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

  onAdvanceSearchChange(data) {
    this.sentDateFrom = data.sentDateFrom;
    this.sentDateTo = data.sentDateTo;
    this.receivedDateFrom = data.receivedDateFrom;
    this.receivedDateTo = data.receivedDateTo;

    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(LaboOrderLineCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'T???o labo';

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: LaboOrderLineDisplay) {
    let modalRef = this.modalService.open(LaboOrderLineCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.title = 'S???a labo';
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    const dialog: DialogRef = this.dialogService.open({
      title: 'X??a nh??m s???n ph???m',
      content: 'B???n c?? ch???c ch???n mu???n x??a?',
      actions: [
        { text: 'H???y b???', value: false },
        { text: '?????ng ??', primary: true, value: true }
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
          this.laboOrderLineService.delete(item.id).subscribe(() => {
            this.loadDataFromApi();
          }, err => {
            console.log(err);
          });
        }
      }
    });
  }
}
