import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Subject } from 'rxjs';
import { IntlService } from '@progress/kendo-angular-intl';
import { CommissionReportsService, CommissionReport, ReportFilterCommission } from '../commission-reports.service';
import { map, debounceTime, tap, switchMap } from 'rxjs/operators';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { CompanyBasic, CompanyService, CompanyPaged } from 'src/app/companies/company.service';

@Component({
  selector: 'app-commission-report-list',
  templateUrl: './commission-report-list.component.html',
  styleUrls: ['./commission-report-list.component.css']
})
export class CommissionReportListComponent implements OnInit {
  loading = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  gridData: GridDataResult;
  reportResults: CommissionReport[] = [];
  limit = 20;
  skip = 0;
  dateFrom: Date;
  formGroup: FormGroup;
  dateTo: Date;
  searchUpdate = new Subject<string>();
  
  @ViewChild('companyCbx', { static: true }) companyCbx: ComboBoxComponent;
  filteredCompanies: CompanyBasic[] = [];
  
  constructor(private commissionReportService: CommissionReportsService,
    private fb: FormBuilder,
    private intl: IntlService,private companyService: CompanyService,) { }

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
    var val = new ReportFilterCommission();
    val.dateFrom = formValue.dateFrom ? this.intl.formatDate(formValue.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = formValue.dateTo ? this.intl.formatDate(formValue.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.companyId = formValue.company ? formValue.company.id : null;
    
    this.loading = true;
    this.commissionReportService.getReport(val).subscribe(result => {
      this.reportResults = result;       
      this.loading = false;
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

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
    
  }
  
}
