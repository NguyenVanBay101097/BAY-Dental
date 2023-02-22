import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-partner-report-location-filter',
  templateUrl: './partner-report-location-filter.component.html',
  styleUrls: ['./partner-report-location-filter.component.css']
})
export class PartnerReportLocationFilterComponent implements OnInit {
  formGroup: FormGroup;
  isDisabledDistricts: boolean = true;
  isDisabledWards: boolean = true;

  dataSourceCities: Array<{ code: string, name: string }>;
  dataSourceDistricts: Array<{ code: string, name: string, cityCode: string, cityName: string }>;
  dataSourceWards: Array<{ code: string, name: string, districtCode: string, districtName: string, cityCode: string, cityName: string }>;

  dataResultCities: Array<{ code: string, name: string }>;
  dataResultDistricts: Array<{ code: string, name: string, cityCode: string, cityName: string }>;
  dataResultWards: Array<{ code: string, name: string, districtCode: string, districtName: string, cityCode: string, cityName: string }>;

  @Output() filterChange = new EventEmitter<any>();

  constructor(private fb: FormBuilder, private http: HttpClient) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      city: null,
      district: null,
      ward: null,
    });

    this.loadSourceCities();
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

    this.emitFilterChange();
  }

  get city() {
    return this.formGroup.get('city');
  }

  get district() {
    return this.formGroup.get('district');
  }

  get ward() {
    return this.formGroup.get('ward');
  }

  emitFilterChange() {
    var city = this.city ? this.city.value : null;
    var district = this.district ? this.district.value : null;
    var ward = this.ward ? this.ward.value : null;
    this.filterChange.emit({ city, district, ward });
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

    this.emitFilterChange();
  }

  handleWardChange(value) {
    this.formGroup.get('ward').setValue(value);

    this.emitFilterChange();
  }
}
