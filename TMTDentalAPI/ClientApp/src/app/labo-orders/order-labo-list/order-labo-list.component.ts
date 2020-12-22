import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
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
    { text: 'trễ hạn', value: 'trehan' },
    { text: 'Chờ nhận', value: 'chonhan' },
    { text: 'Tới hạn', value: 'toihan' }
  ];
  
  constructor(private laboOrderService: LaboOrderService,private modalService: NgbModal) { }

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

  GetState(val){
    var now = new Date();
    if(now > val.datePlanned ){
      return "Trễ hạn";
    }else if(now == val.datePlanned){
      return "Tới hạn";
    }else{
      return "Chờ nhận";
    }
  }

  onStateSelectChange(data: TmtOptionSelect) {
    this.stateFilter = data.value;
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
      this.loadDataFromApi();
    }, er => { });
  }

}
