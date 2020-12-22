import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { LaboOrderExportDialogComponent } from '../labo-order-export-dialog/labo-order-export-dialog.component';
import { ExportLaboPaged, LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-export',
  templateUrl: './labo-order-export.component.html',
  styleUrls: ['./labo-order-export.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class LaboOrderExportComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  stateFilter: string;
  dateExportFrom: Date;
  dateExportTo: Date;

  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Đã xuất', value: 'daxuat' },
    { text: 'Chưa xuất', value: 'chuaxuat' }
  ];

  constructor(
    private laboOrderService: LaboOrderService, 
    private intlService: IntlService, 
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  onStateSelectChange(data: TmtOptionSelect) {
    this.stateFilter = data.value;
    this.loadDataFromApi();
  }

  onDateSearchChange(data) {
    this.dateExportFrom = data.dateFrom;
    this.dateExportTo = data.dateTo;
    this.loadDataFromApi();
  }

  getState(state) {
    switch (state) {
      case 'daxuat':
        return 'Đã xuất';
      case 'chuaxuat':
        return 'Chưa xuất';
      default:
        return 'Chưa xuất';
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ExportLaboPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = this.stateFilter || '';
    val.dateExport = this.intlService.formatDate(new Date(), 'd', 'en-US');
    if (this.dateExportFrom) {
      val.dateExportFrom = this.intlService.formatDate(this.dateExportFrom, 'd', 'en-US');
    }
    if (this.dateExportTo) {
      val.dateExportTo = this.intlService.formatDate(this.dateExportTo, 'd', 'en-US');
    }

    this.laboOrderService.getExportLabo(val).pipe(
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

  editItem(item) {
    const modalRef = this.modalService.open(LaboOrderExportDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.labo = item;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, er => { });
  }
}
