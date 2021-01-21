import { Component, OnInit, ViewChild } from '@angular/core';
import { RealRevenueReportSearch, RealRevenueReportService, RealRevenueReportItem, RealRevenueReportResult } from '../real-revenue-report.service';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-real-revenue-report-overview',
  templateUrl: './real-revenue-report-overview.component.html',
  styleUrls: ['./real-revenue-report-overview.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class RealRevenueReportOverviewComponent implements OnInit {
  reportResult: RealRevenueReportResult;
  loading = false;
  formGroup: FormGroup;

  @ViewChild('companyCbx', { static: true }) companyCbx: ComboBoxComponent;
  filteredCompanies: CompanyBasic[] = [];

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(private intlService: IntlService, private realRevenueReportService: RealRevenueReportService,
    private fb: FormBuilder, private companyService: CompanyService) {
  }

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
    var val = new RealRevenueReportSearch();
    val.dateFrom = formValue.dateFrom ? this.intlService.formatDate(formValue.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = formValue.dateTo ? this.intlService.formatDate(formValue.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.companyId = formValue.company ? formValue.company.id : null;

    this.loading = true;
    this.realRevenueReportService.getReport(val).subscribe(result => {
      this.loading = false;
      this.reportResult = result;
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
    params.active = true;
    return this.companyService.getPaged(params);
  }
}

