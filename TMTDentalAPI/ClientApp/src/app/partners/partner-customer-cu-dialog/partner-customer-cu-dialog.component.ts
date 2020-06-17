import { PartnerSourceSimple } from "./../partner-simple";
import {
  PartnerSourceService,
  PartnerSourcePaged,
} from "./../../partner-sources/partner-source.service";
import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { FormBuilder, FormGroup, Validators, FormArray } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { PartnerCategorySimple } from "../partner-simple";
import {
  PartnerCategoryService,
  PartnerCategoryPaged,
} from "src/app/partner-categories/partner-category.service";
import { PartnerService } from "../partner.service";
import { WindowRef } from "@progress/kendo-angular-dialog";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { HistorySimple } from "src/app/history/history";
import { PartnerCategoryCuDialogComponent } from "src/app/partner-categories/partner-category-cu-dialog/partner-category-cu-dialog.component";
import * as _ from "lodash";
import { AppSharedShowErrorService } from "src/app/shared/shared-show-error.service";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { debounceTime, tap, switchMap } from "rxjs/operators";
import { UserPaged, UserService } from "src/app/users/user.service";
import { UserSimple } from "src/app/users/user-simple";

@Component({
  selector: "app-partner-customer-cu-dialog",
  templateUrl: "./partner-customer-cu-dialog.component.html",
  styleUrls: ["./partner-customer-cu-dialog.component.css"],
})
export class PartnerCustomerCuDialogComponent implements OnInit {
  @ViewChild("sourceCbx", { static: true }) sourceCbx: ComboBoxComponent;
  @ViewChild("userCbx", { static: true }) userCbx: ComboBoxComponent;

  id: string;
  formGroup: FormGroup;
  submitted = false;
  isDisabledDistricts: boolean = true;
  isDisabledWards: boolean = true;
  idShowRef = false;
  title: string;
  filteredSources: PartnerSourceSimple[] = [];
  filteredReferralUsers: UserSimple[] = [];
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

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private partnerCategoryService: PartnerCategoryService,
    private partnerSourceService: PartnerSourceService,
    private partnerService: PartnerService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private showErrorService: AppSharedShowErrorService,
    private userService: UserService
  ) {}

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ["", Validators.required],
      gender: "male",
      ref: null,
      medicalHistory: null,
      birthDay: null,
      birthMonth: null,
      birthYear: null,
      street: null,
      city: null,
      district: null,
      ward: null,
      email: null,
      phone: null,
      categories: null,
      sourceId: null,
      source: null,
      referralUserId: null,
      referralUser: null,
      comment: null,
      jobTitle: null,
      customer: true,
      histories: this.fb.array([]),
      companyId: null,
    });

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

    setTimeout(() => {
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
          if (result.source) {
           this.handleSourceChange(result.source);
          }
          if (result.referralUser) {
            this.handleReferralChange(result.referralUser);
          }

          if (result.histories.length) {
            result.histories.forEach((history) => {
              var histories = this.formGroup.get("histories") as FormArray;
              histories.push(this.fb.group(history));
            });
          }
        });
      }

      this.dayList = _.range(1, 32);
      this.monthList = _.range(1, 13);
      this.yearList = _.range(new Date().getFullYear(), 1900, -1);
      this.loadSourceCities();
      this.loadCategoriesList();
      this.loadSourceList();
      this.loadHistoriesList();
      this.loadReferralUserList();
    });
  }

  get f() {
    return this.formGroup.controls;
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
    this.filteredReferralUsers = this.filteredReferralUsers.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
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

  handleSourceChange(value) {
    if (value) {
      this.formGroup.get("source").setValue(value);
      if (value.type == "referral") {
        this.idShowRef = true;
        setTimeout(() => {
          this.userCbx.filterChange
            .asObservable()
            .pipe(
              debounceTime(300),
              tap(() => (this.userCbx.loading = true)),
              switchMap((value) => this.searchReferralUsers(value))
            )
            .subscribe((result) => {
              this.filteredReferralUsers = result;
              this.userCbx.loading = false;
            });
        }, 100);
      } else {
        this.idShowRef = false;
      }
    } else {
      this.idShowRef = false;
    }
  }

  handleReferralChange(value) {
    this.formGroup.get("referralUser").setValue(value);
  }

  loadCategoriesList() {
    this.searchCategories().subscribe((result) => {
      this.categoriesList = result;
    });
  }

  loadSourceList() {
    this.searchSources().subscribe((result) => {
      this.filteredSources = result;
    });
  }

  loadReferralUserList() {
    this.searchReferralUsers().subscribe((result) => {
      this.filteredReferralUsers = result;
    });
  }

  loadSourcesList() {
    this.searchSources().subscribe((result) => {
      this.filteredSources = result;
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
      () => {}
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
      return;
    }

    if (this.id) {
      var val = this.formGroup.value;
      val.sourceId = val.source ? val.source.id : null;
      val.referralUserId = val.referralUser ? val.referralUser.id : null;
      this.partnerService.update(this.id, val).subscribe(
        () => {
          this.activeModal.close(true);
        },
        (err) => this.showErrorService.show(err)
      );
    } else {
      var val = this.formGroup.value;
      val.sourceId = val.source ? val.source.id : null;
      val.referralUserId = val.referralUser ? val.referralUser.id : null;
      this.partnerService.create(val).subscribe(
        (result) => {
          this.activeModal.close(result);
        },
        (err) => this.showErrorService.show(err)
      );
    }
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.dismiss();
  }
}
