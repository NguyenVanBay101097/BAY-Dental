import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { GridComponent, GridDataResult } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { CompanyPaged, CompanyService, CompanySimple } from 'src/app/companies/company.service';
import { MemberLevelAutoCompleteReq, MemberLevelService } from 'src/app/member-level/member-level.service';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerSourcePaged, PartnerSourceService } from 'src/app/partner-sources/partner-source.service';
import { PartnerSourceSimple } from 'src/app/partners/partner-simple';
import { PartnerOldNewReportReq, PartnerOldNewReportService, PartnerOldNewReportSumReq } from 'src/app/sale-report/partner-old-new-report.service';
import { PrintService } from 'src/app/shared/services/print.service';

@Component({
  selector: 'app-partner-report-overview',
  templateUrl: './partner-report-overview.component.html',
  styleUrls: ['./partner-report-overview.component.css']
})
export class PartnerReportOverviewComponent implements OnInit {

  orderStateDisplay = {
    'sale':'Đang điều trị',
    'done':'Hoàn thành',
    'draft':'Chưa phát sinh'
  };
  cbxPopupSettings = {
    width: 'auto'
  };
  filter = new PartnerOldNewReportReq();
  companies: CompanySimple[] = [];
  partnerTypes = [
    {text: 'Khách mới', value:'new'},
    {text: 'Khách quay lại', value:'old'}
  ];
  partnerGenders = [
    {text: 'Nam', value:'male'},
    {text: 'Nữ', value:'female'},
    {text: 'Khác', value:'other'}
  ];
  partnerSources: PartnerSourceSimple[] = [];
  memberLevels = [];
  filteredCategs: PartnerCategoryBasic[];
  allDataGrid: any = [];
  // allDataExport: any;
  gridData: GridDataResult;
  loading = false;
  skip = 0;
  limit = 20;
  searchUpdate = new Subject<string>();
  sumOld = 0;
  sumNew = 0;
  sumOldNew = 0;
  isFilterAdvance = false;
  
  @ViewChild("companyCbx", { static: true }) companyVC: ComboBoxComponent;
  @ViewChild("pnSourceCbx", { static: true }) pnSourceVC: ComboBoxComponent;
  @ViewChild("memberLevelCbx", { static: true }) memberLevelVC: ComboBoxComponent;
  @ViewChild(GridComponent, { static: true }) public grid: GridComponent;
  @ViewChild("categMst", { static: true }) categMst: MultiSelectComponent;
  constructor(
    private companyService: CompanyService,
    private pnSourceService: PartnerSourceService,
    private partnerOldNewRpService: PartnerOldNewReportService,
    private partnerCategoryService: PartnerCategoryService,
    private printService: PrintService,
    private memberLevelService: MemberLevelService
  ) { }

  ngOnInit() {
    this.initFilterData();
    this.FilterCombobox();
    this.loadAllData();
    this.loadSummary();
  }

  get sumRevenue(){
    return (this.allDataGrid as []).reduce((total, cur:any) => {
      return total + cur.revenue;
    }, 0);
  }

  loadAllData() {
    var val = Object.assign({}, this.filter) as PartnerOldNewReportReq;
    
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.partnerOldNewRpService.getReport(val).subscribe(res => {
      this.allDataGrid = res;
      this.loading = false;
      this.loadReport();
    },
      err => {
        this.loading = false;
      });
  }

  loadSummary() {
    var obs = [];
    var val = new PartnerOldNewReportSumReq();
    val.dateFrom = this.filter.dateFrom ? moment(this.filter.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = this.filter.dateTo ? moment(this.filter.dateTo).format('YYYY/MM/DD') : '';
    val.companyId = this.filter.companyId || '';
    // val.typeReport = this.filter.typeReport;
    obs.push(this.partnerOldNewRpService.sumReport(val));
    this.partnerTypes.forEach(el => {
      var valNew = Object.assign({}, val);
      valNew.typeReport = el.value;
    obs.push(this.partnerOldNewRpService.sumReport(valNew));
    });

    forkJoin(obs).subscribe((res: any[]) => {
      this.sumOldNew = res[0] || 0;
      this.sumNew = res[1] || 0;
      this.sumOld = res[2] || 0;
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
          this.companyVC.loading = false;
        });

        this.loadMemberLevel();
        this.memberLevelVC.filterChange.asObservable().pipe(
          debounceTime(300),
          tap(() => (this.memberLevelVC.loading = true)),
          switchMap(value => this.searchMemberLevel(value))
        ).subscribe(result => {
          this.memberLevels = result.items;
          this.memberLevelVC.loading = false;
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
    ).subscribe(r=> {
      this.skip = 0;
      this.loadAllData();
    })

    // var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    // this.filter.dateFrom = this.filter.dateFrom || new Date(y, m, 1);
    // this.filter.dateTo = this.filter.dateTo || new Date(y, m + 1, 0);
    this.skip = 0;
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
  
  searchMemberLevel(s?) {
    var val = new MemberLevelAutoCompleteReq();
    val.offset = 0;
    val.limit = 20;
    val.search = s || '';
    return this.memberLevelService.autoComplete(val);
  }

  loadMemberLevel(){
    this.searchMemberLevel().subscribe(res => {
      this.memberLevels = res.items;
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

  loadReport() {
    this.gridData = <GridDataResult>{
      total: this.allDataGrid.length,
      data: this.allDataGrid.slice(this.skip, this.skip + this.limit)
    };
  }

  onSearchDateChange(e) {
    this.filter.dateFrom = e.dateFrom;
    this.filter.dateTo = e.dateTo;
    this.skip = 0;
    this.loadAllData();
    this.loadSummary();
  }

  onSelectCompany(e) {
    this.filter.companyId = e ? e.id : null;
    this.skip = 0;
    this.loadAllData();
    this.loadSummary();
  }

  onSelectPartnerType(e) {
    this.filter.typeReport = e ? e.value : '';
    this.skip = 0;
    this.loadAllData(); 
  }

  onSelectPartnerSource(e) {
    this.filter.sourceId = e ? e.id : null;
    this.skip = 0;
    this.loadAllData();
  }

  onMemberLevelSelect(e)
  {
    this.filter.memberLevelId = e? e.id : '';
    this.skip = 0;
    this.loadAllData();
  }

  onSelectPartnerGender(e) {
    this.filter.gender = e ? e.value : '';
    this.skip = 0;
    this.loadAllData(); 
  }

  onCategChange(value) {
    this.filter.cateIds = value.map(x=> x.id);
     this.skip = 0;
     this.loadAllData();
   }

  pageChange(e) {
    this.skip = e.skip;
    this.loadReport();
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

  toggleFilterAdvance() {
    this.isFilterAdvance = !this.isFilterAdvance;
  }

  onExportPDF() {
    var val = Object.assign({}, this.filter) as PartnerOldNewReportReq;
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
    this.partnerOldNewRpService.getReportPdf(val).subscribe(res => {
      this.loading = false;
      let filename ="BaoCaoTheoDichVu";

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


  onPrint(){
    var val = Object.assign({}, this.filter) as PartnerOldNewReportReq;
    
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    this.loading = true;
      this.partnerOldNewRpService.getReportPrint(val).subscribe((result: any) => {
        this.loading = false;
        this.printService.printHtml(result);
      });
  }
}
