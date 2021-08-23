import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { Workbook, WorkbookSheetColumn, WorkbookSheetRow, WorkbookSheetRowCell } from '@progress/kendo-angular-excel-export';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { forkJoin, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerSourcePaged, PartnerSourceService } from 'src/app/partner-sources/partner-source.service';
import { PartnerSourceSimple } from 'src/app/partners/partner-simple';
import { GetSaleOrderPagedReq, PartnerOldNewReportReq, PartnerOldNewReportRes, PartnerOldNewReportService, PartnerOldNewReportSumReq } from 'src/app/sale-report/partner-old-new-report.service';
import { AddressDialogComponent } from 'src/app/shared/address-dialog/address-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { saveAs } from '@progress/kendo-file-saver';

@Component({
  selector: 'app-partner-report-overview',
  templateUrl: './partner-report-overview.component.html',
  styleUrls: ['./partner-report-overview.component.css']
})
export class PartnerReportOverviewComponent implements OnInit {

  orderStateDisplay = {
    'sale': 'Đang điều trị',
    'done': 'Hoàn thành',
    'draft': 'Chưa phát sinh'
  };
  cbxPopupSettings = {
    width: 'auto'
  };
  filter = new PartnerOldNewReportReq();
  companies: CompanySimple[] = [];
  partnerTypes = [
    { text: 'Khách mới', value: 'new' },
    { text: 'Khách quay lại', value: 'old' }
  ];
  partnerGenders = [
    { text: 'Nam', value: 'male' },
    { text: 'Nữ', value: 'female' },
    { text: 'Khác', value: 'other' }
  ];
  partnerSources: PartnerSourceSimple[] = [];
  filteredCategs: PartnerCategoryBasic[];
  gridData: GridDataResult;
  loading = false;
  searchUpdate = new Subject<string>();
  sumOld = 0;
  sumNew = 0;
  sumOldNew = 0;
  revenueOld = 0;
  revenueNew = 0;
  isFilterAdvance = false;
  addressFilter = null;
  pageSizes = [20, 50, 100, 200];

  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("pnSourceCbx", { static: true }) pnSourceVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  @ViewChild("categMst", { static: true }) categMst: MultiSelectComponent;
  constructor(
    private companyService: CompanyService,
    private pnSourceService: PartnerSourceService,
    private partnerOldNewRpService: PartnerOldNewReportService,
    private partnerCategoryService: PartnerCategoryService,
    private printService: PrintService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.FilterCombobox();
    this.loadData();
    this.loadSummary();
  }

  loadData() {
    var val = Object.assign({}, this.filter) as PartnerOldNewReportReq;

    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.partnerOldNewRpService.getReport(val).subscribe(res => {
      this.loading = false;
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      };
    },
      err => {
        this.loading = false;
      });
  }

  loadSummary() {
    var obs = [];
    var val = Object.assign({}, this.filter) as PartnerOldNewReportReq;

    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    obs.push(this.partnerOldNewRpService.sumReport(val));
    if (val.typeReport) {
      if (val.typeReport == 'new') {
        obs.push(this.partnerOldNewRpService.sumReport(val));
        obs.push(this.partnerOldNewRpService.sumReVenue(val));
        obs.push(of(0));
        obs.push(of(0));
      } else {
        obs.push(of(0));
        obs.push(of(0));
        obs.push(this.partnerOldNewRpService.sumReport(val));
        obs.push(this.partnerOldNewRpService.sumReVenue(val));
      }
      obs.push(this.partnerOldNewRpService.sumReVenue(val));
    } else {
      this.partnerTypes.forEach(el => {
        var valNew = Object.assign({}, val);
        valNew.typeReport = el.value;
        obs.push(this.partnerOldNewRpService.sumReport(valNew));
        obs.push(this.partnerOldNewRpService.sumReVenue(valNew));
      });
    }

    forkJoin(obs).subscribe((res: any[]) => {
      this.sumOldNew = res[0] || 0;
      this.sumNew = res[1] || 0;
      this.revenueNew = res[2] || 0;
      this.sumOld = res[3] || 0;
      this.revenueOld = res[4] || 0;
    });
  }

  FilterCombobox() {
    this.loadCompanies();
    this.companyVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.companyVC.loading = true)),
        switchMap((value) => this.searchCompany$(value)
        )
      )
      .subscribe((x: any) => {
        this.companies = x.items;
        this.companyVC.loading = false;
      });

    this.loadPnSources();
    this.pnSourceVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.pnSourceVC.loading = true)),
        switchMap((value) => this.searchPartnerSource$(value)
        )
      )
      .subscribe((x: any) => {
        this.partnerSources = x;
        this.pnSourceVC.loading = false;
      });

    this.loadFilteredCategs();
    this.categMst.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.categMst.loading = true)),
        switchMap((value) => this.searchCategories(value))
      )
      .subscribe((result) => {
        this.filteredCategs = result;
        this.categMst.loading = false;
      });
  }

  initFilterData() {
    this.searchUpdate.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(r => {
      this.onFilterChange();
    })

    // var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    // this.filter.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    // this.filter.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
    this.filter.offset = 0;
    this.filter.limit = 20;
  }

  searchCompany$(search?) {
    var val = new CompanyPaged();
    val.active = true;
    val.search = search || '';
    return this.companyService.getPaged(val);
  }

  loadCompanies() {
    this.searchCompany$().subscribe(res => {
      this.companies = res.items;
    });
  }

  searchPartnerSource$(search?) {
    var val = new PartnerSourcePaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search || '';
    return this.pnSourceService.autocomplete(val);
  }

  loadPnSources() {
    this.searchPartnerSource$().subscribe(res => {
      this.partnerSources = res;
    });
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(
      (result) => (this.filteredCategs = result)
    );
  }
  searchCategories(search?: string) {
    var val = new PartnerCategoryPaged();
    val.search = search;
    return this.partnerCategoryService.autocomplete(val);
  }

  onFilterChange() {
    this.filter.offset = 0;
    this.loadData();
    this.loadSummary();
  }

  onSearchDateChange(e) {
    this.filter.dateFrom = e.dateFrom;
    this.filter.dateTo = e.dateTo;
    this.onFilterChange();
  }

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : '';
    this.onFilterChange();
  }

  onSelectPartnerType(e) {
    this.filter.typeReport = e ? e.value : '';
    this.onFilterChange();
  }

  onSelectPartnerSource(e) {
    this.filter.sourceId = e ? e.id : '';
    this.onFilterChange();
  }

  onSelectPartnerGender(e) {
    this.filter.gender = e ? e.value : '';
    this.onFilterChange();
  }

  onCategChange(value) {
    this.filter.categIds = value.map(x => x.id);
    this.onFilterChange();
  }

  pageChange(e) {
    this.filter.offset = e.skip;
    this.loadData();
  }

  onPageSizeChange(value: number): void {
    this.filter.offset = 0;
    this.filter.limit = value;
    this.loadData();
  }

  getGenderDisplay(e) {
    switch (e) {
      case 'male':
        return 'Nam';
      case 'female':
        return 'Nữ';
      default:
        return "Khác";
    }
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as PartnerOldNewReportReq;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.partnerOldNewRpService.getReportPdf(val).subscribe(res => {
      this.loading = false;
      let filename = "BaoCaoTongQuanKhachHang";

      let newBlob = new Blob([res], {
        type:
          "application/pdf",
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


  onPrint() {
    var val = Object.assign({}, this.filter) as PartnerOldNewReportReq;

    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.partnerOldNewRpService.getReportPrint(val).subscribe((result: any) => {
      this.loading = false;
      this.printService.printHtml(result);
    });
  }

  openAddressDialog() {
    let modalRef = this.modalService.open(AddressDialogComponent, { size: 'md', scrollable: true, windowClass: 'o_technical_modal', keyboard: true, backdrop: 'static' });
    if (this.addressFilter) {
      modalRef.componentInstance.addresObject = Object.assign({}, this.addressFilter);
    }
    modalRef.result.then((res) => {
      this.addressFilter = res;
      this.filter.cityCode = res.city ? res.city.code : '';
      this.filter.districtCode = res.district ? res.district.code : '';
      this.filter.wardCode = res.ward ? res.ward.code : '';
      this.onFilterChange();
    }, () => { });

  }

  getAddressFilterDisplay() {
    if (!this.addressFilter)
      return 'Tất cả khu vực';
    var names = [];
    if (this.addressFilter.city)
      names.push(this.addressFilter.city.name);
    if (this.addressFilter.district)
      names.push(this.addressFilter.district.name);
    if (this.addressFilter.ward)
      names.push(this.addressFilter.ward.name);
    return (names as []).join(', ') || 'Tất cả khu vực';
  }

  exportExcel() {
    var val = Object.assign({}, this.filter) as PartnerOldNewReportReq;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.partnerOldNewRpService.getReportExcel(val).subscribe((res: any) => {
      let filename = "ThongKeKhachHangDieuTri";
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }

  getFilterDetail(row: PartnerOldNewReportRes) {
    var val = <GetSaleOrderPagedReq>{
      companyId: this.filter.companyId || '',
      dateFrom: this.filter.dateFrom || '',
      dateTo: this.filter.dateTo || '',
      partnerId: row.id,
      typeReport: this.filter.typeReport || ''
    };
    return val;
  }
}
