import { KeyValue } from '@angular/common';
import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, PopupSettings } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CardTypeBasic } from 'src/app/card-types/card-type.service';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PartnerCategoryBasic } from 'src/app/partner-categories/partner-category.service';
import { PartnerSourceSimple } from 'src/app/partners/partner-simple';
import { PartnerInfoPaged, PartnerService } from 'src/app/partners/partner.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AccountCommonPartnerReportOverviewFilter, AccountCommonPartnerReportService } from '../account-common-partner-report.service';
import { PartnerReportAreaComponent } from '../partner-report-area/partner-report-area.component';
import { PartnerReportFilterPopupComponent } from '../partner-report-filter-popup/partner-report-filter-popup.component';

@Component({
  selector: 'app-partner-report-overview',
  templateUrl: './partner-report-overview.component.html',
  styleUrls: ['./partner-report-overview.component.css']
})
export class PartnerReportOverviewComponent implements OnInit, AfterViewInit {
  @ViewChild("companyCbx", { static: false }) companyCbx: ComboBoxComponent;
  @ViewChild("reportAreaComp", { static: false }) reportAreaComp: PartnerReportAreaComponent;
  @ViewChild("reportFilterPopup", { static: false }) reportFilterPopup: PartnerReportFilterPopupComponent;
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;

  formGroup: FormGroup
  gridData: GridDataResult;
  limit: number = 20;
  offset: number = 0;
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
  dateFrom: Date;
  dateTo: Date;
  dataReportSource: any[] = [];
  dataReportAgeGender: any[] = [];
  pagerSettings: any;

  constructor(
    private companyService: CompanyService,
    private accountCommonPartnerReportService: AccountCommonPartnerReportService,
    private intlService: IntlService,
    private partnerService: PartnerService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadReportSource();
    this.loadReportAgeGender();
    this.loadCompanies();
    this.loadListPartner();
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
    this.resetFilterCode();
    this.loadAllData();
  }

  // loadReportSumary() {
  //   let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;
  //   this.accountCommonPartnerReportService.getPartnerReportSumaryOverview(val).subscribe((res: any) => {
  //     this.reportSumary = res;
  //   }, error => console.log(error))
  // }

  loadReportSource() {
    let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;

    this.accountCommonPartnerReportService.getPartnerReportSourceOverview(val).subscribe((res: any) => {
      this.dataReportSource = res;
    }, error => console.log(error));
  }

  loadReportAgeGender() {
    let val = Object.assign({}, this.filter) as AccountCommonPartnerReportOverviewFilter;
    this.accountCommonPartnerReportService.getPartnerReportGenderOverview(val).subscribe((res: any) => {
      this.dataReportAgeGender = res;
    }, error => {
      console.log(error)
    });
  }

  loadListPartner() {
    let val = new PartnerInfoPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    this.partnerService.getPartnerInfoPaged2(val).subscribe(res => {
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      };
    }, (error) => {
      console.log(error);
    });
  }

  loadAllData() {
    this.loadReportSource();
    this.loadReportAgeGender();
    // this.loadListPartner();
    setTimeout(() => {
      this.reportAreaComp?.loadReportArea();
    }, 0);
  }

  exportExcelFile() {
    var val = new PartnerInfoPaged();
    this.partnerService.exportPartnerExcelFile(val).subscribe((rs) => {
      let filename = "DanhSachKhachang";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  onApplyEmit(event) {
    this.resetFilterCode();
    this.onFilterAdvance(event);
    this.myDrop.toggle();
  }

  onFilterAdvance(data) {
    const categIds = (data && data.categs != null) ? data.categs.map(val => val.id) : [];
    const partnerSourceIds = (data && data.partnerSources != null) ? data.partnerSources.map(val => val.id) : [];
    const cardTypeIds = (data && data.cardTypes != null) ? data.cardTypes.map(val => val.id) : [];
    this.filter.ageFrom = data.ageFrom;
    this.filter.ageTo = data.ageTo;
    this.filter.revenueFrom = data.revenueFrom;
    this.filter.revenueTo = data.revenueTo;
    this.filter.priceSubTotalFrom = data.priceSubTotalFrom;
    this.filter.priceSubTotalTo = data.priceSubTotalTo;
    this.filter.gender = data.gender;
    this.filter.categIds = categIds;
    this.filter.partnerSourceIds = partnerSourceIds;
    this.filter.cardTypeIds = cardTypeIds;
    this.loadAllData();
    this.showFilterInfo(data);
  }

  showFilterInfo(data) {
    const dataFilter = data;
    this.dataFilterObj = {};
    if (dataFilter) {
      if (dataFilter.categs && dataFilter.categs.length > 0) {
        this.dataFilterObj['categs'] = dataFilter.categs.map(el => el.name).join(', ');
      }

      if (dataFilter.partnerSources && dataFilter.partnerSources.length > 0) {
        this.dataFilterObj['partnerSources'] = dataFilter.partnerSources.map(el => el.name).join(', ');
      }

      if (dataFilter.cardTypes && dataFilter.cardTypes.length > 0) {
        this.dataFilterObj['cardTypes'] = dataFilter.cardTypes.map(el => el.name).join(', ');
      }

      if (dataFilter.ageFrom || dataFilter.ageTo) {
        this.dataFilterObj['age'] = `Từ ${typeof dataFilter.ageFrom === 'number' && dataFilter.ageFrom >= 0 ? dataFilter.ageFrom : '--'} 
                                    đến ${typeof dataFilter.ageTo === 'number' && dataFilter.ageTo >= 0 ? dataFilter.ageTo : '--'}`;
      }

      if (dataFilter.revenueFrom || dataFilter.revenueTo) {
        this.dataFilterObj['revenue'] = `Từ ${typeof dataFilter.revenueFrom === 'number' && dataFilter.revenueFrom >= 0 ? this.intlService.formatNumber(dataFilter.revenueFrom, { style: 'decimal' }) : '--'} 
                                        đến ${typeof dataFilter.revenueTo === 'number' && dataFilter.revenueTo >= 0 ? this.intlService.formatNumber(dataFilter.revenueTo, { style: 'decimal' }) : '--'}`;
      }

      if (dataFilter.priceSubTotalFrom || dataFilter.priceSubTotalTo) {
        this.dataFilterObj['priceSubTotal'] = `Từ ${typeof dataFilter.priceSubTotalFrom === 'number' && dataFilter.priceSubTotalFrom >= 0 ? this.intlService.formatNumber(dataFilter.priceSubTotalFrom, { style: 'decimal' }) : '--'} 
                                        đến ${typeof dataFilter.priceSubTotalTo === 'number' && dataFilter.priceSubTotalTo >= 0 ? this.intlService.formatNumber(dataFilter.priceSubTotalTo, { style: 'decimal' }) : '--'}`;
      }

      if (dataFilter.gender) {
        this.dataFilterObj['gender'] = `${this.getGender(dataFilter.gender)}`;
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
      case 'priceSubTotal':
        return 'Tổng tiền điều trị';
      case 'gender':
        return 'Giới tính';
      default:
        return '';
    }
  }

  getGender(key: string) {
    switch (key) {
      case 'male':
        return 'Nam';
      case 'female':
        return 'Nữ';
      default:
        return 'Khác';
    }
  }

  reverseKey = (a: KeyValue<number, string>, b: KeyValue<number, string>): number => {
    return a.key > b.key ? -1 : (b.key > a.key ? 1 : 0);
  }

  onRemoveFilter(key: string) {
    this.reportFilterPopup?.onRemoveFilter(key);
    this.myDrop.toggle();
  }

  onCloseEmit() {
    this.myDrop.toggle();
  }

  onToggleDropdown(event) {
    this.reportFilterPopup.onUpdateFormValue(event);
  }

  filterEmit(val) {
    if (val) {
      if (val.type === 'city') {
        this.filter.cityCode = val.code;
        this.filter.districtCode = null;
        this.filter.wardCode = null;
      }
      else if (val.type === 'district') {
        this.filter.cityCode = null;
        this.filter.districtCode = val.code;
        this.filter.wardCode = null;
      }
      else if (val.type === 'ward') {
        this.filter.cityCode = null;
        this.filter.districtCode = null;
        this.filter.wardCode = val.code;
      }
      else {
        this.filter.cityCode = null;
        this.filter.districtCode = null;
        this.filter.wardCode = null;
      }
    }
    else {
      this.filter.cityCode = null;
      this.filter.districtCode = null;
      this.filter.wardCode = null;
    }
    this.loadReportSource();
    this.loadReportAgeGender();
  }

  resetFilterCode() {
    this.filter.cityCode = null;
    this.filter.districtCode = null;
    this.filter.wardCode = null;
  }

  onSearchChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
  }

  onPageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.limit = event.take;
    this.loadListPartner();
  }
}
