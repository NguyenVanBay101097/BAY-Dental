import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { FormBuilder, FormGroup, Validators, FormArray } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import {
  PartnerCategoryService,
  PartnerCategoryPaged,
} from "src/app/partner-categories/partner-category.service";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { AppSharedShowErrorService } from "src/app/shared/shared-show-error.service";
import { PartnerService } from 'src/app/partners/partner.service';
import { NotificationService } from "@progress/kendo-angular-notification";
import { IntlService } from "@progress/kendo-angular-intl";

@Component({
  selector: "app-partner-supplier-cu-dialog",
  templateUrl: "./partner-supplier-cu-dialog.component.html",
  styleUrls: ["./partner-supplier-cu-dialog.component.css"],
})
export class PartnerSupplierCuDialogComponent implements OnInit {
  id: string;
  formGroup: FormGroup;
  submitted = false;
  isDisabledDistricts: boolean = true;
  isDisabledWards: boolean = true;
  title: string;

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

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private partnerCategoryService: PartnerCategoryService,
    private partnerService: PartnerService,
    public activeModal: NgbActiveModal,
    private notificationService: NotificationService,
    private showErrorService: AppSharedShowErrorService,
    private intlService: IntlService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ["", Validators.required],
      street: null,
      fax: '',
      dateObj: null,
      city: null,
      district: null,
      ward: null,
      email: null,
      phone: null,
      comment: null,
      dateCreated: null,
      supplier: true,
      customer: false,
      ref: null,
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
          if (result.date) {
            var date = new Date(result.date);
            this.formGroup.get("dateObj").setValue(date);
          }
        });
      }
      else{
        this.formGroup.get("dateObj").setValue(new Date());
      }

      this.loadSourceCities();
    });
  }

  get f() {
    return this.formGroup.controls;
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

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    if (this.id) {
      var val = this.formGroup.value;
      val.date = val.dateObj ? this.intlService.formatDate(val.dateObj, "yyyy-MM-dd") : null;
      this.partnerService.update(this.id, val).subscribe(
        () => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.activeModal.close(true);
        },
      );
    } else {
      var val = this.formGroup.value;
      val.date = val.dateObj ? this.intlService.formatDate(val.dateObj, "yyyy-MM-dd") : null;
      this.partnerService.create(val).subscribe(
        (result) => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.activeModal.close(result);
        },
      );
    }
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.dismiss();
  }
}
