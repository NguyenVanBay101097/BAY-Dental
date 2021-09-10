import { HrPayslipBasic } from './../hr-payslip.service';
import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HrPaysliprunService, HrPayslipRunPaged, HrPayslipRunBasic } from '../hr-paysliprun.service';
import { Router } from '@angular/router';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-hr-payslip-run-list',
  templateUrl: './hr-payslip-run-list.component.html',
  styleUrls: ['./hr-payslip-run-list.component.css']
})
export class HrPayslipRunListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  title = "Bảng lương";
  loading = false;
  searchUpdate = new Subject<string>();
  search: string;
  toTalAmount: number;

  payslipGridData: GridDataResult;
  payslipData: HrPayslipBasic[];

  constructor(private modalService: NgbModal, 
    private hrPaysliprunService: HrPaysliprunService,
    private router: Router,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }
  
  loadDataFromApi() {
    this.loading = true;
    var val = new HrPayslipRunPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    
    this.hrPaysliprunService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
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
    this.limit = event.take;
    this.loadDataFromApi();
  }

  stateGet(state) {
    switch (state) {
      case 'confirm':
        return 'Chờ xác nhận';
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Nháp';
    }
  }

  createItem() {
    this.router.navigateByUrl("hr/payslip-run/form");
  }

  editItem(item: any) {
    this.router.navigateByUrl("hr/payslip-run/form?id=" + item.id);
  }

  deleteItem(item: HrPayslipRunBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + this.title + ': ' + item.name;

    modalRef.result.then(() => {
      this.hrPaysliprunService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }



  
}
