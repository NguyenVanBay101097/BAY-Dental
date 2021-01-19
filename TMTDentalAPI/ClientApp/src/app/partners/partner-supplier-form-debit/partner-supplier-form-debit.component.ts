import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AccountCommonPartnerReportItemDetail, AccountCommonPartnerReportSearch, AccountCommonPartnerReportService, AccountMoveBasic } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PartnerSupplierFormDebitPaymentDialogComponent } from '../partner-supplier-form-debit-payment-dialog/partner-supplier-form-debit-payment-dialog.component';

@Component({
  selector: 'app-partner-supplier-form-debit',
  templateUrl: './partner-supplier-form-debit.component.html',
  styleUrls: ['./partner-supplier-form-debit.component.css']
})
export class PartnerSupplierFormDebitComponent implements OnInit {
  id: string;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: AccountMoveBasic[];
  loading = false;
  selectableSettings = { model: 'multiple' }
  rowsSelected: any[] = [];
  search: string;
  searchUpdate = new Subject<string>();
  constructor(
    private reportService: AccountCommonPartnerReportService,
    private activeRoute: ActivatedRoute,
    private authService: AuthService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    if (this.id) {
      this.loadDataFromApi();

      this.searchUpdate.pipe(
        debounceTime(400),
        distinctUntilChanged())
        .subscribe(() => {
          this.loadDataFromApi();
        });

    }
  }

  loadDataFromApi() {
    var val = new AccountCommonPartnerReportSearch();
    val.partnerId = this.id;
    val.search = this.search ? this.search : '';
    val.companyId = this.authService.userInfo.companyId;
    val.resultSelection = "supplier";
    this.reportService.getListReportPartner(val).subscribe(
      res => {
        this.details = res;
        this.loadItems();
        this.loading = false;
      }
    )
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.details.slice(this.skip, this.skip + this.limit),
      total: this.details.length
    };
  }

  onPayment() {
    let modalRef = this.modalService.open(PartnerSupplierFormDebitPaymentDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.rowsSelected = this.rowsSelected;
    modalRef.componentInstance.partnerId = this.id;
    modalRef.componentInstance.partnerType = "supplier";
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  chooseRows(events: any[]) {
    this.rowsSelected = [];
    if (events && events.length > 0) {
      events.forEach(element => {
        var model = this.details.find(x => x.id == element);
        if (model) {
          this.rowsSelected.push(model);
        }
      });
    }
    console.log(this.rowsSelected);
  }

}
