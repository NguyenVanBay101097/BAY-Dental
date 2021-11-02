
import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { environment } from 'src/environments/environment';
import { CompanyService } from '../company.service';

@Component({
  selector: 'app-company-cu-dialog',
  templateUrl: './company-cu-dialog.component.html',
  styleUrls: ['./company-cu-dialog.component.css']
})

export class CompanyCuDialogComponent implements OnInit {
  id: string;
  companyForm: FormGroup;
  filterdCategories: ProductCategoryBasic[] = [];
  opened = false;
  isDisabledDistricts: boolean = true;
  isDisabledWards: boolean = true;
  title: string;
  submitted = false;
  dataSourceCities: Array<{ code: string, name: string }>;
  dataSourceDistricts: Array<{ code: string, name: string, cityCode: string, cityName: string }>;
  dataSourceWards: Array<{ code: string, name: string, districtCode: string, districtName: string, cityCode: string, cityName: string }>;

  dataResultCities: Array<{ code: string, name: string }>;
  dataResultDistricts: Array<{ code: string, name: string, cityCode: string, cityName: string }>;
  dataResultWards: Array<{ code: string, name: string, districtCode: string, districtName: string, cityCode: string, cityName: string }>;


  @ViewChild('form', { static: true }) formView: any;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private companyService: CompanyService, public activeModal: NgbActiveModal, private http: HttpClient,
    ) {
  }

  ngOnInit() {
    this.companyForm = this.fb.group({
      name: ['', Validators.required],
      street: null,
      city: null,
      district: null,
      ward: null,
      email: [null],
      phone: [null],
      logo: null
    });

  

    if (this.id) {
      setTimeout(() => {
        this.companyService.get(this.id).subscribe(result => {
          this.companyForm.patchValue(result);
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
      });
    }

    setTimeout(() => {
      this.loadSourceCities();
    });

  }

  get f(){
    return this.companyForm.controls;
  }

  onAvatarUploaded(data: any) {
    var fileUrl = data ? data.fileUrl : null;
    this.companyForm.get('logo').setValue(fileUrl);
  }

  loadSourceCities() {
    this.http.post(environment.ashipApi + "api/ApiShippingCity/GetCities", {
      provider: 'Undefined'
    }).subscribe((result: any) => {
      this.dataSourceCities = result;
      this.dataResultCities = this.dataSourceCities.slice();
    });
  }

  loadSourceDistricts(cityCode: string) {
    this.http.post(environment.ashipApi + "api/ApiShippingDistrict/GetDistricts", {
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
    this.http.post(environment.ashipApi + "api/ApiShippingWard/GetWards", {
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
    this.companyForm.get('city').setValue(value);
    this.companyForm.get('district').setValue(null);
    this.companyForm.get('ward').setValue(null);

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
    this.companyForm.get('district').setValue(value);
    this.companyForm.get('ward').setValue(null);

    if (value == undefined) {
      this.isDisabledWards = true;
      this.dataResultWards = [];
    } else {
      this.isDisabledWards = false;
      this.loadSourceWards(value.code);
    }
  }

  handleWardChange(value) {
    this.companyForm.get('ward').setValue(value);
  }

  onSave() {
    this.submitted = true;
    if (!this.companyForm.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(() => {
      this.activeModal.close(true);
    }, err => {
    });
  }

  saveOrUpdate() {
    var data = this.getBodyData();
    if (this.id) {
      return this.companyService.update(this.id, data);
    } else {
      return this.companyService.create(data);
    }
  }

  getBodyData() {
    var data = this.companyForm.value;
    return data;
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}


