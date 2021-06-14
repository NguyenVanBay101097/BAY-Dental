import { AgentPaged, AgentService, AgentBasic } from './../../agents/agent.service';
import { GenderPartner } from './../../partners/partner.service';
import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { FormBuilder, FormGroup, Validators, FormArray } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import {
  PartnerCategoryService,
  PartnerCategoryPaged,
} from "src/app/partner-categories/partner-category.service";
import { NgbActiveModal, NgbModal, NgbPopover } from "@ng-bootstrap/ng-bootstrap";
import { HistorySimple } from "src/app/history/history";
import { PartnerCategoryCuDialogComponent } from "src/app/partner-categories/partner-category-cu-dialog/partner-category-cu-dialog.component";
import * as _ from "lodash";
import { AppSharedShowErrorService } from "src/app/shared/shared-show-error.service";
import { IntlService } from "@progress/kendo-angular-intl";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { UserSimple } from "src/app/users/user-simple";
import { PartnerSourceService, PartnerSourcePaged } from "src/app/partner-sources/partner-source.service";
import { UserService, UserPaged } from "src/app/users/user.service";
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { AddressCheckApi } from 'src/app/price-list/price-list';
import { PartnerSourceSimple, District, City, Ward, PartnerCategorySimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { PartnerTitle, PartnerTitlePaged, PartnerTitleService } from 'src/app/partner-titles/partner-title.service';
import { PartnerTitleCuDialogComponent } from '../partner-title-cu-dialog/partner-title-cu-dialog.component';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PermissionService } from '../permission.service';
import { CheckPermissionService } from '../check-permission.service';
import { AgentCreateUpdateDialogComponent } from '../agent-create-update-dialog/agent-create-update-dialog.component';

@Component({
  selector: "app-partner-customer-cu-dialog",
  templateUrl: "./partner-customer-cu-dialog.component.html",
  styleUrls: ["./partner-customer-cu-dialog.component.css"],
})
export class PartnerCustomerCuDialogComponent implements OnInit {
  @ViewChild("sourceCbx", { static: true }) sourceCbx: ComboBoxComponent;
  @ViewChild("userCbx", { static: true }) userCbx: ComboBoxComponent;
  @ViewChild("titleCbx", { static: true }) titleCbx: ComboBoxComponent;
  @ViewChild("agentCbx", { static: true }) agentCbx: ComboBoxComponent;

  id: string;
  formGroup: FormGroup;
  isDisabledDistricts: boolean = true;
  isDisabledWards: boolean = true;
  title: string;
  addressCheck: AddressCheckApi[] = [];
  filteredSources: PartnerSourceSimple[] = [];
  filteredReferralUsers: UserSimple[] = [];
  districtsList: District[] = [];
  provincesList: City[] = [];
  wardsList: Ward[] = [];
  districtsFilter: District[] = [];
  provincesFilter: City[] = [];
  wardsFilter: Ward[] = [];
  filteredTitles: PartnerTitle[] = [];
  filteredAgents: AgentBasic[] = [];

  dataSourceCities: Array<{ code: string; name: string }>;
  dataSourceDistricts: Array<{
    code: string;
    name: string;
    cityCode: string;
    cityName: string;
  }>;
  dataSourceWards: Array<{
    code: string;
    name: string;
    districtCode: string;
    districtName: string;
    cityCode: string;
    cityName: string;
  }>;

  dataResultCities: Array<{ code: string; name: string }>;
  dataResultDistricts: Array<{
    code: string;
    name: string;
    cityCode: string;
    cityName: string;
  }>;
  dataResultWards: Array<{
    code: string;
    name: string;
    districtCode: string;
    districtName: string;
    cityCode: string;
    cityName: string;
  }>;

  categoriesList: PartnerCategorySimple[] = [];

  dayList: number[] = [];
  monthList: number[] = [];
  yearList: number[] = [];

  historiesList: HistorySimple[] = [];

  addtionalData: any;

  submitted = false;

  showPartnerSource = false;
  showPartnerHistories = false;
  showPartnerTitles = false;
  showAgent = false;
  showPartnerCategories = false;
  canCreateTitle = false;

  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private agentService: AgentService,
    private partnerCategoryService: PartnerCategoryService,
    private partnerSourceService: PartnerSourceService,
    private partnerService: PartnerService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private showErrorService: AppSharedShowErrorService,
    private intlService: IntlService,
    private userService: UserService,
    private partnerTitleService: PartnerTitleService,
    private employeeService: EmployeeService,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ["", Validators.required],
      gender: "male",
      ref: null,
      medicalHistory: null,
      birthDayStr: '',
      birthMonthStr: '',
      birthYearStr: '',
      street: null,
      city: null,
      district: null,
      ward: null,
      email: null,
      phone: null,
      categories: null,
      source: null,
      referralUserId: null,
      referralUser: null,
      comment: null,
      jobTitle: null,
      customer: true,
      histories: this.fb.array([]),
      companyId: null,
      dateObj: null,
      addressCheckDetail: 0,
      title: null,
      agent: null,
      avatar: null
    });

    setTimeout(() => {
      this.checkRole();
      if (this.id) {
        this.partnerService.getPartner(this.id).subscribe((result) => {
          this.formGroup.patchValue(result);
          if (result.city && result.city.code) {
            this.handleCityChange(result.city);
          }
          if (result.district && result.district.code) {
            this.handleDistrictChange(result.district);
          }
          if (result.ward && result.ward.code) {
            this.handleWardChange(result.ward);
          }

          if (result.histories.length) {
            result.histories.forEach((history) => {
              var histories = this.formGroup.get("histories") as FormArray;
              histories.push(this.fb.group(history));
            });
          }

          if (result.date) {
            var date = new Date(result.date);
            this.formGroup.get("dateObj").setValue(date);
          }

          if (result.birthYear) {
            this.formGroup.get("birthYearStr").setValue(result.birthYear + '');
          }

          if (result.birthMonth) {
            this.formGroup.get("birthMonthStr").setValue(result.birthMonth + '');
          }

          if (result.birthDay) {
            this.formGroup.get("birthDayStr").setValue(result.birthDay + '');
          }

          if (result.source) {
            this.filteredSources = _.unionBy(this.filteredSources, [result.source], 'id');
          }

          if (result.referralUser) {
            this.filteredReferralUsers = _.unionBy(this.filteredReferralUsers, [result.referralUser], 'id');
          }

          if (result.title) {

            this.filteredTitles = _.unionBy(this.filteredTitles, [result.title], 'id');
          }
          
          if (result.agent) {
            this.filteredAgents = _.unionBy(this.filteredAgents, [result.agent], 'id');
          }
        });
      } else {
        this.partnerService.defaultGet().subscribe((result) => {
          this.formGroup.patchValue(result);
          this.formGroup.get("dateObj").setValue(new Date());

          if (this.addtionalData) {
            this.formGroup.patchValue(this.addtionalData);
          }
        });
      }

      this.dayList = _.range(1, 32);
      this.monthList = _.range(1, 13);
      this.yearList = _.range(new Date().getFullYear(), 1900, -1);
      this.loadSourceCities();
      if(this.showPartnerCategories){
        this.loadCategoriesList();
      }
      if(this.showPartnerHistories){
        this.loadHistoriesList();
      }
      if(this.showPartnerSource){
        this.loadSourceList();
      }
      // this.loadReferralUserList();
      if(this.showPartnerTitles){
        this.loadTitleList();
      }
        this.loadAgentList();


      this.sourceCbx.filterChange
        .asObservable()
        .pipe(
          debounceTime(300),
          tap(() => (this.sourceCbx.loading = true)),
          switchMap((value) => this.searchSources(value))
        )
        .subscribe((result) => {
          this.filteredSources = result;
          this.sourceCbx.loading = false;
        });
    });

    this.titleCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.titleCbx.loading = true)),
        switchMap((value) => this.searchTitles(value))
      )
      .subscribe((result) => {     
        this.filteredTitles = result;
        this.titleCbx.loading = false;
      });

    this.agentCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.agentCbx.loading = true)),
        switchMap((value) => this.searchAgents(value))
      )
      .subscribe((result) => {
        console.log(result);
        this.filteredAgents = result.items;
        this.agentCbx.loading = false;
      });
  }

  get sourceValue() {
    return this.formGroup.get('source').value;
  }

  loadHistoriesList() {
    this.partnerService.getHistories().subscribe((result) => {
      this.historiesList = result;
    });
  }

  checked(item: HistorySimple) {
    var histories = this.formGroup.get("histories") as FormArray;
    for (var i = 0; i < histories.controls.length; i++) {
      var control = histories.controls[i];
      if (control.value.id == item.id) {
        return true;
      }
    }

    return false;
  }

  checkboxChange(hist: HistorySimple, isCheck: boolean) {
    var histories = this.formGroup.get("histories") as FormArray;

    if (isCheck) {
      let index = histories.controls.findIndex((x) => x.value.id == hist.id);
      if (index == -1) {
        histories.push(this.fb.group(hist));
      }
    } else {
      let index = histories.controls.findIndex((x) => x.value.id == hist.id);
      if (index != -1) {
        histories.removeAt(index);
      }
    }
  }

  handleAddress(adr: AddressCheckApi) {
    var city = { code: adr.cityCode, name: adr.cityName };
    var district = { code: adr.districtCode, name: adr.districtName };
    var ward = { code: adr.wardCode, name: adr.wardName };
    this.getStreet.setValue(adr.shortAddress);
    if (city && city.code) {
      this.handleCityChange(city);
    }
    if (district && district.code) {
      this.handleDistrictChange(district);
    }
    if (ward && ward.code) {
      this.handleWardChange(ward);
    }
  }

  get getStreet() {
    return this.formGroup.get('street');
  }

  quickCreateTitle() {
    let modalRef = this.modalService.open(PartnerTitleCuDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm danh xưng';
    modalRef.result.then(result => {
      this.filteredTitles.push(result as PartnerTitle);
      this.formGroup.patchValue({ title: result });
    }, () => {
    });
  }

  quickCreateAgent() {
    let modalRef = this.modalService.open(AgentCreateUpdateDialogComponent,  { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm người giới thiệu';
    modalRef.result.then(result => {
      this.filteredAgents.push(result as AgentBasic);
      this.formGroup.patchValue({ agent: result });
    }, () => {
    });
  }


  loadSourceCities() {
    this.http
      .post("https://aship.skyit.vn/api/ApiShippingCity/GetCities", {
        provider: "Undefined",
      })
      .subscribe((result: any) => {
        this.dataSourceCities = result;
        this.dataResultCities = this.dataSourceCities.slice();
      });
  }

  loadSourceDistricts(cityCode: string) {
    this.http
      .post("https://aship.skyit.vn/api/ApiShippingDistrict/GetDistricts", {
        data: {
          code: cityCode,
        },
        provider: "Undefined",
      })
      .subscribe((result: any) => {
        this.dataSourceDistricts = result;
        this.dataResultDistricts = this.dataSourceDistricts.slice();
      });
  }

  loadSourceWards(districtCode: string) {
    this.http
      .post("https://aship.skyit.vn/api/ApiShippingWard/GetWards", {
        data: {
          code: districtCode,
        },
        provider: "Undefined",
      })
      .subscribe((result: any) => {
        this.dataSourceWards = result;
        this.dataResultWards = this.dataSourceWards.slice();
      });
  }

  handleCityFilter(value) {
    this.dataResultCities = this.dataSourceCities.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  handleDistrictFilter(value) {
    this.dataResultDistricts = this.dataSourceDistricts.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  handleWardFilter(value) {
    this.dataResultWards = this.dataSourceWards.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  handleSourceFilter(value) {
    this.filteredSources = this.filteredSources.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  handleReferralUserFilter(value) {
    this.searchReferralUsers(value).subscribe((result) => {
      this.filteredReferralUsers = result;
    });
  }

  handleCityChange(value) {
    this.formGroup.get("city").setValue(value);
    this.formGroup.get("district").setValue(null);
    this.formGroup.get("ward").setValue(null);

    if (value == undefined) {
      this.isDisabledDistricts = true;
      this.dataResultDistricts = [];
    } else {
      this.isDisabledDistricts = false;
      this.loadSourceDistricts(value.code);
    }

    this.isDisabledWards = true;
    this.dataResultWards = [];
  }

  handleDistrictChange(value) {
    this.formGroup.get("district").setValue(value);
    this.formGroup.get("ward").setValue(null);

    if (value == undefined) {
      this.isDisabledWards = true;
      this.dataResultWards = [];
    } else {
      this.isDisabledWards = false;
      this.loadSourceWards(value.code);
    }
  }

  handleWardChange(value) {
    this.formGroup.get("ward").setValue(value);
  }

  loadCategoriesList() {
    this.searchCategories().subscribe((result) => {
      this.categoriesList = result;
    });
  }

  loadSourceList() {
    this.searchSources().subscribe((result) => {
      this.filteredSources = _.unionBy(this.filteredSources, result, 'id');
    });
  }

  loadReferralUserList() {
    this.searchReferralUsers().subscribe((result) => {
      this.filteredReferralUsers = _.unionBy(this.filteredReferralUsers, result, 'id');
    });
  }

  loadTitleList() {
    this.searchTitles().subscribe((result) => {
      this.filteredTitles = _.unionBy(this.filteredTitles, result, 'id');
    });
  }

  loadAgentList() {
    this.searchAgents().subscribe((result) => {
      this.filteredAgents = _.unionBy(this.filteredAgents, result.items, 'id');
    });
  }

  quickCreatePartnerCategory() {
    let modalRef = this.modalService.open(PartnerCategoryCuDialogComponent, {
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm nhóm khách hàng";

    modalRef.result.then(
      () => {
        this.loadCategoriesList();
      },
      () => { }
    );
  }

  searchCategories(q?: string) {
    var val = new PartnerCategoryPaged();
    val.search = q;
    return this.partnerCategoryService.autocomplete(val);
  }

  searchSources(q?: string) {
    var val = new PartnerSourcePaged();
    val.search = q;
    return this.partnerSourceService.autocomplete(val);
  }

  searchReferralUsers(q?: string) {
    var val = new UserPaged();
    val.search = q;
    return this.userService.autocompleteSimple(val);
  }

  searchTitles(q?: string) {
    var val = new PartnerTitlePaged();
    val.search = q;
    return this.partnerTitleService.autocomplete(val);
  }

  searchAgents(q?: string) {
    var val = new AgentPaged();
    val.search = q || '';
    return this.agentService.getPaged(val);
  }

  birthInit(begin: number, end: number) {
    var list = new Array();
    for (let i = begin; i <= end; i++) {
      list.push(i);
    }
    return list;
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.sourceId = val.source ? val.source.id : null;
    val.referralUserId = val.referralUser ? val.referralUser.id : null;
    val.titleId = val.title ? val.title.id : null;
    val.date = val.dateObj ? this.intlService.formatDate(val.dateObj, "yyyy-MM-dd") : null;
    val.birthDay = val.birthDayStr ? parseInt(val.birthDayStr) : null;
    val.birthMonth = val.birthMonthStr ? parseInt(val.birthMonthStr) : null;
    val.birthYear = val.birthYearStr ? parseInt(val.birthYearStr) : null;
    val.agentId = val.agent ? val.agent.id : null;

    if (this.id) {
      this.partnerService.update(this.id, val).subscribe(
        () => {
          this.activeModal.close(true);
        },
      );
    } else {
      this.partnerService.create(val).subscribe(
        (result) => {
          this.activeModal.close(result);
        },
      );
    }
  }

  onChangeGender(gender) {
    this.partnerService.getDefaultTitle({ gender: gender }).subscribe((result: any) => {
      this.formGroup.get('title').setValue(result);
    });
  }


  onCancel() {
    this.activeModal.dismiss();
  }

  onAvatarUploaded(data) {
    this.f.avatar.setValue(data ? data.fileUrl : null);
  }

  checkRole(){
    this.showPartnerHistories = this.checkPermissionService.check(["Catalog.History.Read"]);
    this.canCreateTitle = this.checkPermissionService.check(["Catalog.PartnerTitle.Create"]);
  }
}
