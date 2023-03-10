import { Component, OnInit, Input, Inject } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AccountInvoicePaged, AccountInvoiceService } from 'src/app/account-invoices/account-invoice.service';
import { map } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-partner-customer-invoices',
  templateUrl: './partner-customer-invoices.component.html',
  styleUrls: ['./partner-customer-invoices.component.css']
})
export class PartnerCustomerInvoicesComponent implements OnInit {
  @Input() partnerId: string;
  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  pagerSettings: any;
  loading = false;

  constructor(
    private accountInvoiceService: AccountInvoiceService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettingsPopup }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AccountInvoicePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId;

    this.accountInvoiceService.getPaged(val).pipe(
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
    this.limit = event.take;
    this.loadDataFromApi();
  }
}
