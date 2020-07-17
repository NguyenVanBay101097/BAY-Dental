import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ReportCashBankGeneralLedger, AccountReportGeneralLedgerService } from '../account-report-general-ledger.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PermissionService } from 'src/app/shared/permission.service';

@Component({
  selector: 'app-account-report-general-ledger-cash-bank',
  templateUrl: './account-report-general-ledger-cash-bank.component.html',
  styleUrls: ['./account-report-general-ledger-cash-bank.component.css']
})
export class AccountReportGeneralLedgerCashBankComponent implements OnInit {

  formGroup: FormGroup;
  reportValues: any;
  loading = false;

  @ViewChild('companyCbx', { static: true }) companyCbx: ComboBoxComponent;
  filteredCompanies: CompanyBasic[] = [];

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(private fb: FormBuilder, private intlService: IntlService,
     private reportGeneralLedgerService: AccountReportGeneralLedgerService,
     private companyService: CompanyService, public permissionService: PermissionService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateFrom: this.monthStart,
      dateTo: this.monthEnd,
      company: null
    });

    this.loadDataFromApi();
    this.loadFilteredCompanies();

    this.companyCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.companyCbx.loading = true),
      switchMap(val => this.searchCompanies(val))
    ).subscribe(
      rs => {
        this.filteredCompanies = rs.items;
        this.companyCbx.loading = false;
      });
  }

  loadDataFromApi() {
    var formValue = this.formGroup.value;
    var val = new ReportCashBankGeneralLedger();
    val.dateFrom = formValue.dateFrom ? this.intlService.formatDate(formValue.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = formValue.dateTo ? this.intlService.formatDate(formValue.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.companyId = formValue.company ? formValue.company.id : null;

    this.loading = true;
    this.reportGeneralLedgerService.getCashBankReport(val).subscribe(result => {
      this.loading = false;
      this.reportValues = result;
    }, () => {
      this.loading = false;
    });
  }

  loadFilteredCompanies() {
    this.searchCompanies().subscribe(
      result => {
        this.filteredCompanies = result.items;
      }
    )
  }

  searchCompanies(search?: string) {
    var params = new CompanyPaged();
    params.search = search || '';
    return this.companyService.getPaged(params);
  }
}
