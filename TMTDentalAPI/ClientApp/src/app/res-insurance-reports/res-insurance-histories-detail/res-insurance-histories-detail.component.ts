import { Component, Inject, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { InsuranceHistoryInComeDetailFilter, InsuranceHistoryInComeDetailItem } from '../res-insurance-report.model';
import { ResInsuranceReportService } from '../res-insurance-report.service';

@Component({
  selector: 'app-res-insurance-histories-detail',
  templateUrl: './res-insurance-histories-detail.component.html',
  styleUrls: ['./res-insurance-histories-detail.component.css']
})
export class ResInsuranceHistoriesDetailComponent implements OnInit {
  paymentId: string;
  title: string;
  gridData: GridDataResult;
  limit: number = 20;
  skip: number = 0;
  itemDetail: any;
  pagerSettings: any;

  constructor(
    private resInsuranceReportService: ResInsuranceReportService,
    private activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private router: Router,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) {
    this.pagerSettings = config.pagerSettings
  }

  ngOnInit(): void {
    this.loadDataFromApi();
  }

  loadDataFromApi(): void {
    let val = new InsuranceHistoryInComeDetailFilter();
    val.paymentId = this.paymentId || '';
    this.resInsuranceReportService.getHistoryInComeDetail(val).subscribe((res: any) => {
      this.itemDetail = res;
      this.loadItems();
    }, (error) => console.log(error));
  }

  loadItems(): void {
    this.gridData = {
      data: this.itemDetail.lines.slice(this.skip, this.skip + this.limit),
      total: this.itemDetail.lines.length
    };
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems();
  }

  clickUrl(item) {
    this.router.navigateByUrl('/partners/customer/' + item.partnerId);
    this.activeModal.dismiss();
  }


  public getState(state) {
    switch (state) {
      case 'posted':
        return 'Đã xác nhận';
      case 'cancel':
        return 'Đã hủy';
    }
  }

  onCancel(): void {
    this.activeModal.dismiss();
  }
}
