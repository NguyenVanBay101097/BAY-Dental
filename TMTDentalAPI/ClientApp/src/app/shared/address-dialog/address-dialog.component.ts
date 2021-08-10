import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-address-dialog',
  templateUrl: './address-dialog.component.html',
  styleUrls: ['./address-dialog.component.css']
})
export class AddressDialogComponent implements OnInit {
  dataSourceCities: Array<{ code: string; name: string }> = [];
  dataResultCities: Array<{ code: string; name: string }> = [];
  dataSourceDistricts: Array<{ code: string; name: string }> = [];
  dataResultDistricts: Array<{ code: string; name: string }> = [];
  dataSourceWards: Array<{ code: string; name: string }> = [];
  dataResultWards: Array<{ code: string; name: string }> = [];
title= "Chọn khu vực";
activeTab = "city";
addresObject = {
  city:null,
  district: null,
  ward: null
};

searchCity;
searchCityUpdate = new Subject<string>();

searchDistrict;
searchDistrictUpdate = new Subject<string>();

searchWard;
searchWardUpdate = new Subject<string>();
  constructor(
    private http: HttpClient,
    public activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
    this.loadSourceCities();
    this.loadInitAddress();

    this.searchCityUpdate.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(r=> {
     this.dataResultCities = this.dataSourceCities.filter(x=> x.name.toLowerCase().includes(this.searchCity.toLowerCase()));
    })
    this.searchDistrictUpdate.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(r=> {
      this.dataResultDistricts = this.dataSourceDistricts.filter(x=> x.name.toLowerCase().includes(this.searchDistrict.toLowerCase()));
    })
    this.searchWardUpdate.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(r=> {
      this.dataResultWards = this.dataSourceWards.filter(x=> x.name.toLowerCase().includes(this.searchWard.toLowerCase()));
    })
  }

  loadInitAddress() {
    if(this.addresObject.city) {
      this.loadSourceDistricts(this.addresObject.city.code);
      this.activeTab = 'district';
    }
    if(this.addresObject.district) {
      this.loadSourceWards(this.addresObject.district.code);
      this.activeTab = 'ward';
    }
  }

  resetSearch() {
    this.searchCity = '';
    this.searchDistrict = '';
    this.searchWard = '';
    this.dataResultCities = this.dataSourceCities.filter(x=> x.name.toLowerCase().includes(this.searchCity.toLowerCase()));
    this.dataResultWards = this.dataSourceWards.filter(x=> x.name.toLowerCase().includes(this.searchWard.toLowerCase()));
    this.dataResultWards = this.dataSourceWards.filter(x=> x.name.toLowerCase().includes(this.searchWard.toLowerCase()));
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

  handleCityChange(value) {
    this.resetSearch();
    this.activeTab = 'district';
    this.addresObject.city = value;
      this.loadSourceDistricts(value.code);
      this.addresObject.district = null;
      this.addresObject.ward = null;
  }

  handleDistrictChange(value) {
    this.resetSearch();
    this.activeTab = 'ward';
    this.addresObject.district = value;
      this.loadSourceWards(value.code);
      this.addresObject.ward = null;
  }

  handleWardChange(value) {
    this.resetSearch();
    this.addresObject.ward = value;
    this.activeModal.close(this.addresObject);
  }

  onAllCity() {
    this.addresObject.city = null;
    this.addresObject.district = null;
    this.addresObject.ward = null;
    this.activeModal.close(this.addresObject);
  }

  onAllDistrict() {
    this.addresObject.district = null;
    this.addresObject.ward = null;
    this.activeModal.close(this.addresObject);
  }

  onAllWard() {
    this.addresObject.ward = null;
    this.activeModal.close(this.addresObject);
  }

  toggleTab(val) {
    this.resetSearch();
this.activeTab = val;
  }
  onDismiss(){
    this.activeModal.dismiss();
  }
}
