import { KeyValue } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, PopupSettings } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CardTypeBasic, CardTypePaged, CardTypeService } from 'src/app/card-types/card-type.service';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerSourcePaged, PartnerSourceService } from 'src/app/partner-sources/partner-source.service';
import { PartnerSourceSimple } from 'src/app/partners/partner-simple';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';
import { PartnerReportAgeGenderComponent } from '../partner-report-age-gender/partner-report-age-gender.component';
import { PartnerReportAreaComponent } from '../partner-report-area/partner-report-area.component';
import { PartnerReportSourceComponent } from '../partner-report-source/partner-report-source.component';

@Component({
  selector: 'app-partner-report-overview',
  templateUrl: './partner-report-overview.component.html',
  styleUrls: ['./partner-report-overview.component.css']
})
export class PartnerReportOverviewComponent implements OnInit, AfterViewInit {
  @ViewChild("companyCbx", { static: false }) companyCbx: ComboBoxComponent;
  @ViewChild("reportAreaComp", { static: false }) reportAreaComp: PartnerReportAreaComponent;
  @ViewChild('reportSourceComp', { static: false }) reportSourceComp: PartnerReportSourceComponent;
  @ViewChild("reportAgeGenderComp", { static: false }) reportAgeGenderComp: PartnerReportAgeGenderComponent;
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;

  formGroup: FormGroup

  revenueExpect: { text: string, value: boolean }[] = [
    { text: 'Có dự kiến thu', value: true },
    { text: 'Không có dự kiến thu', value: false }
  ];

  totalDebits: { text: string, value: boolean }[] = [
    { text: 'Có công nợ', value: true },
    { text: 'Không có công nợ', value: false }
  ];

  orderStates: { text: string, value: string }[] = [
    { text: 'Chưa phát sinh', value: 'draft' },
    { text: 'Đang điều trị', value: 'sale' },
    { text: 'Hoàn thành', value: 'done' }
  ];

  reportSumary: any;
  reportSource: any;
  companies: CompanySimple[] = [];
  filter = new AccountCommonPartnerReportOverviewFilter();

  listCardType: CardTypeBasic[] = [];
  listPartnerSource: PartnerSourceSimple[] = [];
  listPartnerCategory: PartnerCategoryBasic[] = [];

  public popupSettings: PopupSettings = {
    appendTo: "component",
  };
  dataFilterObj = Object.create({});
  constructor(
    private companyService: CompanyService,
    private accountCommonPartnerReportService: AccountCommonPartnerReportService,
    private cardTypeService: CardTypeService,
    private partnerSourceService: PartnerSourceService,
    private partnerCategoryService: PartnerCategoryService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      categs: [null],
      partnerSources: [null],
      cardTypes: [null],
      ageFrom: null,
      ageTo: null,
      revenueFrom: null,
      revenueTo: null,
      revenueExpectFrom: null,
      revenueExpectTo: null,
      debtFrom: null,
      debtTo: null,
    });

    this.loadCompanies();
    this.loadReportSumary();
    this.loadListCardType();
    this.loadSourceList();
    this.loadPartnerCategoryList();
  }

  ngAfterViewInit(): void {
    this.filterCompanyCbx();
  }

  filterCompanyCbx() {
    this.companyCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyCbx.loading = true)),
        switchMap((value) => this.searchCompany$(value)
        )
      )
      .subscribe((x) => {
        this.companies = x.items;
        this.companyCbx.loading = false;
      });
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : null;
    this.loadAllData();
  }

  loadListCardType() {
    this.searchCardTypes().subscribe(result => {
      this.listCardType = _.unionBy(this.listCardType, result, 'id');
    })
  }

  searchCardTypes(search?: string) {
    var val = new CardTypePaged();
    val.search = search || '';
    return this.cardTypeService.autoComplete(val);
  }

  loadSourceList() {
    this.searchSources().subscribe((result) => {
      this.listPartnerSource = _.unionBy(this.listPartnerSource, result, 'id');
    });
  }

  searchSources(q?: string) {
    var val = new PartnerSourcePaged();
    val.search = q;
    return this.partnerSourceService.autocomplete(val);
  }

  loadPartnerCategoryList() {
    this.searchPartnerCategory().subscribe((result) => {
      this.listPartnerCategory = _.unionBy(this.listPartnerCategory, result, 'id');
    });
  }

  searchPartnerCategory(q?: string) {
    var val = new PartnerCategoryPaged();
    val.search = q || '';
    return this.partnerCategoryService.autocomplete(val);
  }

  loadReportSumary() {
    let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;
    this.accountCommonPartnerReportService.getPartnerReportSumaryOverview(val).subscribe((res: any) => {
      this.reportSumary = res;
    }, error => console.log(error))
  }

  loadAllData() {
    setTimeout(() => {
      this.reportAreaComp?.loadReportArea();
      this.reportSourceComp?.loadReportSource();
      this.reportAgeGenderComp?.loadReportAgeGender();
      this.loadReportSumary();
    }, 0);
  }

  onSelectOrderStates(e) {
    this.filter.orderState = e ? e.value : null;
    this.loadAllData();
  }

  onSelectTotalDebits(e) {
    this.filter.isDebt = e ? e.value : null;
    this.loadAllData();
  }

  onSelectOrderResiduals(e) {
    this.filter.isRevenueExpect = e ? e.value : null;
    this.loadAllData();
  }

  exportExcelFile() {

  }

  onApply() {
    this.onFilterAdvance();
    this.myDrop.toggle();
  }

  onFilterAdvance() {
    const formValue = this.formGroup.value;
    const categIds = (formValue && formValue.categs != null) ? formValue.categs.map(val => val.id) : [];
    const partnerSourceIds = (formValue && formValue.partnerSources != null) ? formValue.partnerSources.map(val => val.id) : [];
    const cardTypeIds = (formValue && formValue.cardTypes != null) ? formValue.cardTypes.map(val => val.id) : [];
    this.filter.ageFrom = formValue.ageFrom;
    this.filter.ageTo = formValue.ageTo;
    this.filter.revenueFrom = formValue.revenueFrom;
    this.filter.revenueTo = formValue.revenueTo;
    this.filter.revenueExpectFrom = formValue.revenueExpectFrom;
    this.filter.revenueExpectTo = formValue.revenueExpectTo;
    this.filter.debtFrom = formValue.debtFrom;
    this.filter.debtTo = formValue.debtTo;
    this.filter.categIds = categIds;
    this.filter.partnerSourceIds = partnerSourceIds;
    this.filter.cardTypeIds = cardTypeIds;
    this.loadAllData();
    this.showFilterInfo();
  }

  showFilterInfo() {
    const dataFilter = { ...this.formGroup.value };
    this.dataFilterObj = {};
    if (dataFilter) {
      if (dataFilter.categs) {
        this.dataFilterObj['categs'] = dataFilter.categs.map(el => el.name).join(', ');
      }

      if (dataFilter.partnerSources) {
        this.dataFilterObj['partnerSources'] = dataFilter.partnerSources.map(el => el.name).join(', ');
      }

      if (dataFilter.cardTypes) {
        this.dataFilterObj['cardTypes'] = dataFilter.cardTypes.map(el => el.name).join(', ');
      }

      if (dataFilter.ageFrom || dataFilter.ageTo) {
        this.dataFilterObj['age'] = `Từ ${dataFilter.ageFrom && dataFilter.ageFrom >= 0 ? dataFilter.ageFrom : '--'} 
                                    đến ${dataFilter.ageTo && dataFilter.ageTo >= 0 ? dataFilter.ageTo : '--'}`;
      }

      if (dataFilter.revenueFrom || dataFilter.revenueTo) {
        this.dataFilterObj['revenue'] = `Từ ${dataFilter.revenueFrom && dataFilter.revenueFrom >= 0 ? dataFilter.revenueFrom : '--'} 
                                        đến ${dataFilter.revenueTo && dataFilter.revenueTo >= 0 ? dataFilter.revenueTo : '--'}`;
      }

      if (dataFilter.revenueExpectFrom || dataFilter.revenueExpectTo) {
        this.dataFilterObj['revenueExpect'] = `Từ ${dataFilter.revenueExpectFrom && dataFilter.revenueExpectFrom >= 0 ? dataFilter.revenueExpectFrom : '--'} 
                                              đến ${dataFilter.revenueExpectTo && dataFilter.revenueExpectTo >= 0 ? dataFilter.revenueExpectTo : '--'}`;
      }

      if (dataFilter.debtFrom || dataFilter.debtTo) {
        this.dataFilterObj['debt'] = `Từ ${dataFilter.debtFrom && dataFilter.debtFrom >= 0 ? dataFilter.debtFrom : '--'} 
                                      đến ${dataFilter.debtTo && dataFilter.debtTo >= 0 ? dataFilter.debtTo : '--'}`;
      }
    }

  }

  getTitleDisplay(key: string) {
    switch (key) {
      case 'categs':
        return 'Nhãn khách hàng';
      case 'partnerSources':
        return 'Nguồn khách hàng';
      case 'cardTypes':
        return 'Thẻ thành viên';
      case 'age':
        return 'Độ tuổi';
      case 'revenue':
        return 'Doanh thu';
      case 'revenueExpect':
        return 'Dự kiến thu';
      case 'debt':
        return 'Công nợ';
      default:
        return '';
    }
  }

  reverseKey = (a: KeyValue<number, string>, b: KeyValue<number, string>): number => {
    return a.key > b.key ? -1 : (b.key > a.key ? 1 : 0);
  }

  onRemoveFilter(key: string) {
    if (key === 'age') {
      this.formGroup.get('ageFrom').setValue(null);
      this.formGroup.get('ageTo').setValue(null);
    }
    else if (key === 'revenue') {
      this.formGroup.get('revenueFrom').setValue(null);
      this.formGroup.get('revenueTo').setValue(null);
    }
    else if (key === 'revenueExpect') {
      this.formGroup.get('revenueExpectFrom').setValue(null);
      this.formGroup.get('revenueExpectTo').setValue(null);
    }
    else if (key === 'debt') {
      this.formGroup.get('debtFrom').setValue(null);
      this.formGroup.get('debtTo').setValue(null);
    } else {
      this.formGroup.get(key).setValue(null);
    }
    this.onFilterAdvance();
  }

  onRemoveAllFilters() {
    this.formGroup.reset();
  }
}
