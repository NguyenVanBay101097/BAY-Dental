import { KeyValue } from '@angular/common';
import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, PopupSettings } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import * as moment from 'moment';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { CardTypeBasic } from 'src/app/card-types/card-type.service';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PartnerCategoryBasic } from 'src/app/partner-categories/partner-category.service';
import { PartnerSourceSimple } from 'src/app/partners/partner-simple';
import { PartnerInfoFilter, PartnerInfoPaged, PartnerService } from 'src/app/partners/partner.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AccountCommonPartnerReportService } from '../account-common-partner-report.service';
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

  formGroup: FormGroup
  gridData: GridDataResult;
  limit: number = 5;
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
  filter = new PartnerInfoFilter();

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
    this.filter.limit = this.limit;
    this.filter.offset = this.offset;
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
    let val = Object.assign({}, this.filter) as PartnerInfoFilter;
    this.accountCommonPartnerReportService.getPartnerReportSourceOverview(val).subscribe((res: any) => {
      this.dataReportSource = res;
    }, error => console.log(error));
  }

  loadReportAgeGender() {
    let val = Object.assign({}, this.filter) as PartnerInfoFilter;
    this.accountCommonPartnerReportService.getPartnerReportGenderOverview(val).subscribe((res: any) => {
      this.dataReportAgeGender = res;
    }, error => {
      console.log(error)
    });
  }

  loadListPartner() {
    let val = {};
    for (const key in this.filter) {
      const element = this.filter[key];
      if (element) {
        val[key] = element;
      }
    }
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
    this.loadListPartner();
    setTimeout(() => {
      this.reportAreaComp?.loadReportArea();
    }, 0);
  }

  exportExcelFile() {
    var val = Object.assign({}, this.filter) as PartnerInfoFilter;
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

  onApplyEmit(data) {
    this.resetFilterCode();
    this.filter = Object.assign(this.filter, data);
    const categIds = (data && data.categs != null) ? data.categs.map(val => val.id) : [];
    const partnerSourceIds = (data && data.partnerSources != null) ? data.partnerSources.map(val => val.id) : [];
    const cardTypeIds = (data && data.cardTypes != null) ? data.cardTypes.map(val => val.id) : [];
    this.filter.categIds = categIds;
    this.filter.partnerSourceIds = partnerSourceIds;
    this.filter.cardTypeIds = cardTypeIds;
    this.filter = { ... this.filter };
    this.loadAllData();
    this.showFilterInfo(this.filter);
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

      if (dataFilter.amountTotalFrom || dataFilter.amountTotalTo) {
        this.dataFilterObj['amountTotal'] = `Từ ${typeof dataFilter.amountTotalFrom === 'number' && dataFilter.amountTotalFrom >= 0 ? this.intlService.formatNumber(dataFilter.amountTotalFrom, { style: 'decimal' }) : '--'} 
                                        đến ${typeof dataFilter.amountTotalTo === 'number' && dataFilter.amountTotalTo >= 0 ? this.intlService.formatNumber(dataFilter.amountTotalTo, { style: 'decimal' }) : '--'}`;
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
      case 'amountTotal':
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
    if (key === 'age') {
      this.filter.ageFrom = null;
      this.filter.ageTo = null;
    }
    else if (key === 'revenue') {
      this.filter.revenueFrom = null;
      this.filter.revenueTo = null;
    }
    else if (key === 'amountTotal') {
      this.filter.amountTotalFrom = null;
      this.filter.amountTotalTo = null;
    }
    else if (key === 'categs') {
      this.filter.categIds = [];
    }
    else if (key === 'partnerSources') {
      this.filter.partnerSourceIds = [];
    }
    else if (key === 'cardTypes') {
      this.filter.cardTypeIds = [];
    }

    this.filter[key] = null;
    this.filter.cityCode = null;
    this.filter.districtCode = null;
    this.filter.wardCode = null;
    this.filter = { ... this.filter };
    this.showFilterInfo(this.filter);
    this.loadAllData();
  }

  filterEmit(val) {
    if (val) {
      if (val.type === 'city') {
        if (val.cityCode) {
          delete this.filter['cityCodeIsNull'];
          this.filter.cityCode = val.cityCode;
        } else {
          delete this.filter['cityCode'];
          this.filter.cityCodeIsNull = true;
        }

        this.filter.districtCode = null;
        this.filter.wardCode = null;
        delete this.filter['districtCodeIsNull'];
        delete this.filter['wardCodeIsNull'];
      }
      else if (val.type === 'district') {
        this.filter.cityCode = val.cityCode;

        if (val.districtCode) {
          delete this.filter['districtCodeIsNull'];
          this.filter.districtCode = val.districtCode;
        } else {
          delete this.filter['districtCode'];
          this.filter.districtCodeIsNull = true;
        }

        this.filter.wardCode = null;
        delete this.filter['wardCodeIsNull'];
      }
      else if (val.type === 'ward') {
        this.filter.cityCode = val.cityCode;
        this.filter.districtCode = val.districtCode;
        if (val.wardCode) {
          delete this.filter['wardCodeIsNull'];
          this.filter.wardCode = val.wardCode;
        } else {
          delete this.filter['wardCode'];
          this.filter.wardCodeIsNull = true;
        }
      }
      else {
        this.filter.cityCode = null;
        this.filter.districtCode = null;
        this.filter.wardCode = null;
        delete this.filter['cityCodeIsNull'];
        delete this.filter['districtCodeIsNull'];
        delete this.filter['wardCodeIsNull'];
      }
    }
    else {
      this.filter.cityCode = null;
      this.filter.districtCode = null;
      this.filter.wardCode = null;
      delete this.filter['cityCodeIsNull'];
      delete this.filter['districtCodeIsNull'];
      delete this.filter['wardCodeIsNull'];
    }

    this.loadReportSource();
    this.loadReportAgeGender();
    this.loadListPartner();
  }

  resetFilterCode() {
    this.filter.cityCode = null;
    this.filter.districtCode = null;
    this.filter.wardCode = null;
  }

  onSearchChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.filter.dateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY-MM-DD') : '';
    this.filter.dateTo = this.dateTo ? moment(this.dateTo).format('YYYY-MM-DD') : '';
    this.resetFilterCode();
    this.loadAllData();
  }

  onPageChange(event: PageChangeEvent): void {
    this.filter.offset = event.skip;
    this.loadListPartner();
  }
}
