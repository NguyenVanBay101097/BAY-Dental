import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { PartnerCategorySimple } from '../partner-simple';
import { PartnerCategoryService, PartnerCategoryPaged } from 'src/app/partner-categories/partner-category.service';
import { PartnerService } from '../partner.service';
import { WindowRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'app-partner-customer-cu-dialog',
  templateUrl: './partner-customer-cu-dialog.component.html',
  styleUrls: ['./partner-customer-cu-dialog.component.css']
})
export class PartnerCustomerCuDialogComponent implements OnInit {

  id: string;
  formGroup: FormGroup;
  isDisabledDistricts: boolean = true;
  isDisabledWards: boolean = true;

  dataSourceCities: Array<{ code: string, name: string }>;
  dataSourceDistricts: Array<{ code: string, name: string, cityCode: string, cityName: string }>;
  dataSourceWards: Array<{ code: string, name: string, districtCode: string, districtName: string, cityCode: string, cityName: string }>;

  dataResultCities: Array<{ code: string, name: string }>;
  dataResultDistricts: Array<{ code: string, name: string, cityCode: string, cityName: string }>;
  dataResultWards: Array<{ code: string, name: string, districtCode: string, districtName: string, cityCode: string, cityName: string }>;

  categoriesList: PartnerCategorySimple[] = [];

  dayList: number[] = [];
  monthList: number[] = [];
  yearList: number[] = [];

  constructor(private fb: FormBuilder, private http: HttpClient, private partnerCategoryService: PartnerCategoryService,
    private partnerService: PartnerService, private windowRef: WindowRef) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      gender: 'male',
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
      comment: null,
      jobTitle: null,
      customer: true,
    });

    if (this.id) {
      this.partnerService.getPartner(this.id).subscribe(result => {
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
      });
    }

    this.dayList = this.birthInit(1, 31);
    this.monthList = this.birthInit(1, 12);
    this.yearList = this.birthInit(1900, new Date().getFullYear());
    this.loadSourceCities();
    this.loadCategoriesList();
  }

  loadSourceCities() {
    this.http.post('https://aship.skyit.vn/api/ApiShippingCity/GetCities', {
      provider: 'Undefined'
    }).subscribe((result: any) => {
      this.dataSourceCities = result;
      this.dataResultCities = this.dataSourceCities.slice();
    });
  }

  loadSourceDistricts(cityCode: string) {
    this.http.post('https://aship.skyit.vn/api/ApiShippingDistrict/GetDistricts', {
      data: {
        code: cityCode
      },
      provider: 'Undefined'
    }).subscribe((result: any) => {
      this.dataSourceDistricts = result;
      this.dataResultDistricts = this.dataSourceDistricts.slice();
    });
  }

  loadSourceWards(districtCode: string) {
    this.http.post('https://aship.skyit.vn/api/ApiShippingWard/GetWards', {
      data: {
        code: districtCode
      },
      provider: 'Undefined'
    }).subscribe((result: any) => {
      this.dataSourceWards = result;
      this.dataResultWards = this.dataSourceWards.slice();
    });
  }

  handleCityFilter(value) {
    this.dataResultCities = this.dataSourceCities.filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  handleDistrictFilter(value) {
    this.dataResultDistricts = this.dataSourceDistricts.filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  handleWardFilter(value) {
    this.dataResultWards = this.dataSourceWards.filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  handleCityChange(value) {
    this.formGroup.get('city').setValue(value);
    this.formGroup.get('district').setValue(null);
    this.formGroup.get('ward').setValue(null);

    if (value == undefined) {
      this.isDisabledDistricts = true;
      this.dataResultDistricts = [];
    }
    else {
      this.isDisabledDistricts = false;
      this.loadSourceDistricts(value.code);
    }

    this.isDisabledWards = true;
    this.dataResultWards = [];

  }

  handleDistrictChange(value) {
    this.formGroup.get('district').setValue(value);
    this.formGroup.get('ward').setValue(null);

    if (value == undefined) {
      this.isDisabledWards = true;
      this.dataResultWards = [];
    } else {
      this.isDisabledWards = false;
      this.loadSourceWards(value.code);
    }
  }

  handleWardChange(value) {
    this.formGroup.get('ward').setValue(value);
  }

  loadCategoriesList() {
    this.searchCategories().subscribe(result => {
      this.categoriesList = result;
    });
  }

  searchCategories(q?: string) {
    var val = new PartnerCategoryPaged();
    val.search = q;
    return this.partnerCategoryService.autocomplete(val);
  }

  birthInit(begin: number, end: number) {
    var list = new Array();
    for (let i = begin; i <= end; i++) {
      list.push(i);
    }
    return list;
  }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    if (this.id) {
      var val = this.formGroup.value;
      this.partnerService.update(this.id, val).subscribe(() => {
        this.windowRef.close(true);
      });
    } else {
      var val = this.formGroup.value;
      this.partnerService.create(val).subscribe(result => {
        this.windowRef.close(result);
      });
    }
  }

  onCancel() {
    this.windowRef.close();
  }
}
