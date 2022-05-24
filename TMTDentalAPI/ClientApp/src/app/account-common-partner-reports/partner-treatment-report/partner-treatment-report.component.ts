import { KeyValue } from '@angular/common';
import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { forkJoin, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { GetSaleOrderPagedReq, PartnerOldNewReportReq, PartnerOldNewReportRes, PartnerOldNewReportService } from 'src/app/sale-report/partner-old-new-report.service';
import { AddressDialogComponent } from 'src/app/shared/address-dialog/address-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';

@Component({
  selector: 'app-partner-treatment-report',
  templateUrl: './partner-treatment-report.component.html',
  styleUrls: ['./partner-treatment-report.component.css']
})
export class PartnerTreatmentReportComponent implements OnInit {

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
  rangeDays: any = [
    { text: '> 1 tháng', intervalNbr: 1, interval: 'month' },
    { text: '> 3 tháng', intervalNbr: 3, interval: 'month' },
    { text: '> 6 tháng', intervalNbr: 6, interval: 'month' },
    { text: '> 12 tháng', intervalNbr: 12, interval: 'month' },
  ];
  dataFilterObj = Object.create({});
  advancedFiltering: any;
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
  pagerSettings: any;

  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  constructor(
    private companyService: CompanyService,
    private partnerOldNewRpService: PartnerOldNewReportService,
    private printService: PrintService,
    private modalService: NgbModal,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

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
    console.log(val)
    this.loading = true;
    this.partnerOldNewRpService.getReport(val).subscribe(res => {
      this.loading = false;
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      };
      console.log(this.gridData);
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

  pageChange(e) {
    this.filter.offset = e.skip;
    this.filter.limit = e.take;
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

  handleFilterDate($event) {
    this.filter.overInterval = $event?.interval || '';
    this.filter.overIntervalNbr = $event?.intervalNbr || 0;
    this.onFilterChange();
  }

  onChange($event) {
    this.advancedFiltering = { ...$event };
    this.filter.typeReport = $event.typeReport?.value || '';
    this.filter.categIds = $event.categs?.map(x => x.id) || [];
    this.filter.sourceIds = $event.partnerSources?.map(x => x.id) || [];
    this.filter.gender = $event?.gender?.value || '';
    this.filter.overInterval =  $event.overIntervalData?.interval || ''; 
    this.filter.overIntervalNbr =  $event.overIntervalData?.intervalNbr || 0; 
    this.onFilterChange();
    this.showFilterInfo($event);
  }

  showFilterInfo(dataFilter) {
    this.dataFilterObj = {};
    if (dataFilter) {
      if (dataFilter.overIntervalData) {
        this.dataFilterObj['overIntervalData'] = dataFilter.overIntervalData.text;
      }

      if (dataFilter.categs && dataFilter.categs.length > 0) {
        this.dataFilterObj['categs'] = dataFilter.categs.map(el => el.name).join(', ');
      }

      if (dataFilter.partnerSources && dataFilter.partnerSources.length > 0) {
        this.dataFilterObj['partnerSources'] = dataFilter.partnerSources.map(el => el.name).join(', ');
      }

      if (dataFilter.gender) {
        this.dataFilterObj['gender'] = dataFilter.gender.text;
      }
    }
  }

  reverseKey = (a: KeyValue<number, string>, b: KeyValue<number, string>): number => {
    return a.key > b.key ? -1 : (b.key > a.key ? 1 : 0);
  }

  getTitleDisplay(key: string) {
    switch (key) {
      case 'typeReport':
        return 'Loại khách hàng';
      case 'categs':
        return 'Nhãn khách hàng';
      case 'partnerSources':
        return 'Nguồn khách hàng';
      case 'gender':
        return 'Giới tính';
        case 'overIntervalData':
          return 'Ngày điều trị gần nhất';
      default:
        return '';
    }
  }

  onRemoveFilter(key: string) {
    switch (key) {
      case 'typeReport':
        this.filter.typeReport = '';
        break;
      case 'categs':
        this.filter.categIds = [];
        break;
      case 'partnerSources':
        this.filter.sourceIds = [];
        break;
      case 'gender':
        this.filter.gender = '';
        break;
        case 'overIntervalData':
          this.filter.overInterval = '';
          this.filter.overIntervalNbr = 0;
          break;
      default:
        this.filter[key] = null;
        break;
    }
    this.advancedFiltering[key] = null;
    delete this.dataFilterObj[key];
    this.onFilterChange();
  }

  onSelectTypeReport(e){
    this.filter.typeReport = e ? e.value : '';
    this.onFilterChange();
  }
}
