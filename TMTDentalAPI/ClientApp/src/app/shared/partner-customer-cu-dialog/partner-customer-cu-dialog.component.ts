import { HttpClient } from "@angular/common/http";
import { Component, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { NavigationStart, Router, RouterEvent } from "@angular/router";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { AutoCompleteComponent, ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { IntlService } from "@progress/kendo-angular-intl";
import * as _ from "lodash";
import { Observable, of, OperatorFunction, Subject } from "rxjs";
import { catchError, debounceTime, delay, distinctUntilChanged, filter, map, mergeMap, switchMap, take, tap } from 'rxjs/operators';
import { HistorySimple } from "src/app/history/history";
import { PartnerCategoryCuDialogComponent } from "src/app/partner-categories/partner-category-cu-dialog/partner-category-cu-dialog.component";
import {
  PartnerCategoryPaged, PartnerCategoryService
} from "src/app/partner-categories/partner-category.service";
import { PartnerSourceCreateUpdateDialogComponent } from "src/app/partner-sources/partner-source-create-update-dialog/partner-source-create-update-dialog.component";
import { PartnerSourcePaged, PartnerSourceService } from "src/app/partner-sources/partner-source.service";
import { PartnerTitle, PartnerTitlePaged, PartnerTitleService } from 'src/app/partner-titles/partner-title.service';
import { PartnerExistListDialogComponent } from "src/app/partners/partner-exist-list-dialog/partner-exist-list-dialog.component";
import { City, District, PartnerCategorySimple, PartnerSimple, PartnerSourceSimple, Ward } from 'src/app/partners/partner-simple';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { AddressCheckApi } from 'src/app/price-list/price-list';
import { UserSimple } from "src/app/users/user-simple";
import { UserPaged, UserService } from "src/app/users/user.service";
import { environment } from 'src/environments/environment';
import { AgentCreateUpdateDialogComponent } from '../agent-create-update-dialog/agent-create-update-dialog.component';
import { CheckPermissionService } from '../check-permission.service';
import { PartnerTitleCuDialogComponent } from '../partner-title-cu-dialog/partner-title-cu-dialog.component';
import { NotifyService } from "../services/notify.service";
import { AgentBasic, AgentPaged, AgentService } from './../../agents/agent.service';

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
  @ViewChild("autocomplete", { static: true }) partnerAutocomplete: AutoCompleteComponent;

  id: string;
  formGroup: FormGroup;
  isDisabledDistricts: boolean = true;
  isDisabledWards: boolean = true;
  title: string = 'Kh??ch h??ng';
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
  filterPartners: PartnerSimple[] = [];
  phoneExistPartners = [];
  partner: any;

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

  showAgent = false;
  showPartnerCategories = false;
  showInfo = false;
  canCreateTitle = false;


  get f() { return this.formGroup.controls; }

  search: OperatorFunction<string, any[]>;
  formatter = (x: any) => x.name || x;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private agentService: AgentService,
    private partnerCategoryService: PartnerCategoryService,
    private partnerSourceService: PartnerSourceService,
    private partnerService: PartnerService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private intlService: IntlService,
    private userService: UserService,
    private partnerTitleService: PartnerTitleService,
    private checkPermissionService: CheckPermissionService,
    private notifyService: NotifyService,
    private router: Router
  ) {
    // Close any opened dialog when route changes
    router.events.pipe(
      filter((event: RouterEvent) => event instanceof NavigationStart),
      tap(() => this.activeModal.dismiss()),
      take(1),
    ).subscribe(rs => { });
  }

  ngAfterViewInit(): void {
    //Called after ngAfterContentInit when the component's view has been initialized. Applies to components only.
    //Add 'implements AfterViewInit' to the class.
    // this.partnerAutocomplete.focus();
  }

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
          this.partner = result;
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
      this.loadHistoriesList();
      this.loadSourceList();
      this.loadTitleList();
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
        this.filteredAgents = result.items;
        this.agentCbx.loading = false;
      });

    this.search = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      switchMap(term => this.autoComplete$(term))
    )
  }

  get sourceValue() {
    return this.formGroup.get('source').value;
  }

  autoComplete$(s?) {
    var val = new PartnerFilter();
    val.limit = 20;
    val.search = s ?? '';
    val.customer = true;
    return this.partnerService.autocomplete2(val)
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
    modalRef.componentInstance.title = 'Th??m danh x??ng';
    modalRef.result.then(result => {
      this.filteredTitles.push(result as PartnerTitle);
      this.formGroup.patchValue({ title: result });
    }, () => {
    });
  }

  quickCreateAgent() {
    let modalRef = this.modalService.open(AgentCreateUpdateDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m ng?????i gi???i thi???u';
    modalRef.result.then(result => {
      this.filteredAgents.push(result as AgentBasic);
      this.formGroup.patchValue({ agent: result });
    }, () => {
    });
  }

  quickCreateSource() {
    let modalRef = this.modalService.open(PartnerSourceCreateUpdateDialogComponent, { size: 'lg', windowClass: "o_technical_modal", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.title = "Th??m ngu???n kh??ch h??ng";
    modalRef.result.then(result => {
      this.notifyService.notify("success", "L??u th??nh c??ng");
      this.filteredSources.push(result as PartnerSourceSimple);
      this.formGroup.patchValue({ source: result });
    }, () => { }
    );
  }

  loadSourceCities() {
    this.http
      .post(environment.ashipApi + "api/ApiShippingCity/GetCities", {
        provider: "Undefined",
      })
      .subscribe((result: any) => {
        this.dataSourceCities = result;
        this.dataResultCities = this.dataSourceCities.slice();
      });
  }

  loadSourceDistricts(cityCode: string) {
    this.http
      .post(environment.ashipApi + "api/ApiShippingDistrict/GetDistricts", {
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
      .post(environment.ashipApi + "api/ApiShippingWard/GetWards", {
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
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Th??m nh??m kh??ch h??ng";

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

  preparePostData(value): any {
    return {
      name: value.name,
      phone: value.phone,
      ref: value.ref,
      birthDay: value.birthDayStr ? parseInt(value.birthDayStr) : null,
      birthMonth: value.birthMonthStr ? parseInt(value.birthMonthStr) : null,
      birthYear: value.birthYearStr ? parseInt(value.birthYearStr) : null,
      gender: value.gender,
      titleId: value.title != null ? value.title.id : null,
      email: value.email,
      jobTitle: value.jobTitle,
      agentId: value.agent != null ? value.agent.id : null,
      date: value.dateObj ? this.intlService.formatDate(value.dateObj, "yyyy-MM-dd") : null,
      street: value.street,
      city: value.city,
      district: value.district,
      ward: value.ward,
      sourceId: value.source != null ? value.source.id : null,
      comment: value.comment,
      avatar: value.avatar,
      medicalHistory: value.medicalHistory,
      historyIds: value.histories.map(x => x.id)
    };
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    var postVal = this.preparePostData(val);

    if (this.id) {
      postVal.id = this.id;
      this.partnerService.updateCustomer(postVal).subscribe(
        () => {
          this.notifyService.notify('success', 'L??u th??nh c??ng');
          this.activeModal.close(true);
        },
      );
    } else {
      this.partnerService.createCustomer(postVal).pipe(
        mergeMap((rs: any) => {
          this.notifyService.notify('success', 'L??u th??nh c??ng');
          return this.partnerService.getCustomerInfo(rs.id);
        })).subscribe(
          (result: PartnerSimple) => {
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

  onAgeEmit(data): void {
    const currentYear = (new Date()).getFullYear();
    if (data && currentYear >= data) {
      const year = currentYear - (+data);
      this.formGroup.get("birthYearStr").setValue(year);
    } else {
      this.formGroup.get("birthYearStr").setValue('');
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  onAvatarUploaded(data) {
    this.f.avatar.setValue(data ? data.fileUrl : null);
  }

  checkRole() {
    this.canCreateTitle = this.checkPermissionService.check(["Catalog.PartnerTitle.Create"]);
    this.showInfo = this.checkPermissionService.check(["Basic.Partner.ContactInfo"]);
  }

  onBlurPhone(e) {
    var value = e.target.value as string;
    if (!value || (this.partner && this.partner.phone == value)) {
      this.phoneExistPartners = [];
      return;
    }
    this.partnerService.getExist({ phone: value, customer: true }).subscribe(res => {
      this.phoneExistPartners = res;
    })
  }

  onDetailExistPartner() {
    let modalRef = this.modalService.open(PartnerExistListDialogComponent, { size: 'lg', windowClass: "o_technical_modal", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.data = this.phoneExistPartners;
    modalRef.result.then(result => {
    }, () => { }
    );
  }

  onSelectPartnerValue(event, input) {
    this.formGroup.get('name').setValue(event.item.name);
    event.preventDefault();
  }
}
