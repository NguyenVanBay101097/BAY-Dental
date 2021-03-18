import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { LaboOrderReceiptDialogComponent } from '../labo-order-receipt-dialog/labo-order-receipt-dialog.component';
import { LaboOrderBasic, LaboOrderService, OrderLaboPaged } from '../labo-order.service';

@Component({
  selector: 'app-order-labo-list',
  templateUrl: './order-labo-list.component.html',
  styleUrls: ['./order-labo-list.component.css']
})
export class OrderLaboListComponent implements OnInit {
  skip = 0;
  limit = 20;
  gridData: GridDataResult;
  details: LaboOrderBasic[];
  search: string;
  searchUpdate = new Subject<string>();
  loading = false;
  stateFilter: string;

  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Trễ hạn', value: 'trehan' },
    { text: 'Chờ nhận', value: 'chonhan' },
    { text: 'Tới hạn', value: 'toihan' }
  ];

  filterLaboStatus = [
    {name:'Trễ hạn',value:'trehan'},
    {name:'Chờ nhận',value:'chonhan'},
    {name:'Tới hạn',value:'toihan'},
  ];
  
  constructor(private laboOrderService: LaboOrderService,private modalService: NgbModal,private intlService: IntlService,private notificationService: NotificationService,) { }

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
    var val = new OrderLaboPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = this.stateFilter || '';
    this.laboOrderService.getOrderLabo(val).pipe(
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

  getState(val) {
    var today = new Date();
    var now = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    var datePlanned = new Date(val.datePlanned);
    if (val.datePlanned != null && now > datePlanned) {
      return "Trễ hạn";
    } else if (val.datePlanned != null && now.getDate() == datePlanned.getDate() && now.getMonth() == datePlanned.getMonth() && now.getFullYear() == datePlanned.getFullYear()) {
      return "Tới hạn";
    } else  {
      return "Chờ nhận";
    }
  }

  getTextColor(val){
    var today = new Date();
    var now = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    var datePlanned = new Date(val.datePlanned);
    if (val.datePlanned != null && now > datePlanned) {
      return {'text-danger': true};
    } else if (val.datePlanned != null && now.getDate() == datePlanned.getDate() && now.getMonth() == datePlanned.getMonth() && now.getFullYear() == datePlanned.getFullYear()) {
      return {'text-success':true};
    } else {
      return;
    }
  }

  onStateSelectChange(data: TmtOptionSelect) {
    this.stateFilter = data.value;
    this.loadDataFromApi();
  }

  onChangeLaboState(e) {
    this.stateFilter = e? e.value : null;
    this.loadDataFromApi();
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  editItem(item) {
    const modalRef = this.modalService.open(LaboOrderReceiptDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.labo = item;
    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    }, er => { });
  }

}
