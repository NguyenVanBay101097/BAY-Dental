import {
  Component, OnInit, ViewChild,
} from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { PartnerService } from '../partner.service';
import { PartnerDisplay, PartnerCategorySimple, District, AshipRequest, City, Ward, AshipData } from '../partner-simple';
import { ActivatedRoute } from '@angular/router';
import { EmployeeSimple, EmployeePaged } from 'src/app/employees/employee';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HistorySimple } from 'src/app/history/history';
import { AddressCheckApi } from 'src/app/price-list/price-list';



@Component({
  selector: 'app-partner-create-update',
  templateUrl: './partner-create-update.component.html',
  styleUrls: ['./partner-create-update.component.css']
})


export class PartnerCreateUpdateComponent implements OnInit {
  @ViewChild('grid', { static: true }) grid: any;


  constructor(private fb: FormBuilder, private service: PartnerService, private activeRoute: ActivatedRoute, public activeModal: NgbActiveModal) { }

  // @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;

  newCust: PartnerDisplay;
  isChange: boolean = false;
  cusId: string;

  dayList: number[] = [];
  monthList: number[] = [];
  yearList: number[] = [];
  daySource: number[] = [];
  monthSource: number[] = [];
  yearSource: number[] = [];

  categoriesList: PartnerCategorySimple[] = [];
  fullCategoriesList: PartnerCategorySimple[] = [];
  historiesList: HistorySimple[] = [];
  checkedHistoriesList: HistorySimple[] = [];

  districtsList: District[] = [];
  provincesList: City[] = [];
  wardsList: Ward[] = [];
  districtsFilter: District[] = [];
  provincesFilter: City[] = [];
  wardsFilter: Ward[] = [];
  employeeSimpleFilter: EmployeeSimple[] = [];
  addressCheck: AddressCheckApi[] = [];
  checkedText: string;

  queryCustomer: boolean = false;
  querySupplier: boolean = false;
  header = ''

  formCreate: FormGroup;
  formImage: FormGroup;

  skip: number = 0;
  limit: number = 20;
  ngOnInit() {
    this.formCreate = this.fb.group({
      name: [null, Validators.required],
      phone: [null, Validators.minLength(9)],
      email: [null, Validators.email],
      barcode: null,
      ref: null,
      fax: null,

      customer: null,
      supplier: null,

      street: null,
      comment: null,
      gender: 'male',
      histories: this.fb.array([]),
      medicalHistory: null,

      city: null,
      district: null,
      ward: null,

      birthDay: null,
      birthMonth: null,
      birthYear: null,

      categories: null,
      company: null,
      jobTitle: null,
      employees: null,
      source: null,
      note: null,
      addressCheckDetail: 0
    })

    this.formImage = this.fb.group({
      uploadImage: null
    })

    this.getEmployeesList();
    this.getCustomerInfo();
    this.dobPrepare();
    this.getPartnerCategories();
    this.routingChange();
    this.formCreate.get('customer').setValue(this.queryCustomer ? true : false);
    this.formCreate.get('supplier').setValue(this.querySupplier ? true : false);
    this.getProDisWrd();
    this.getHistories();

    // this.filterChangeCombobox();

  }

  //Load l???i d??? li???u khi ?????i routing gi???a Customer <=> Supplier
  routingChange() {
    this.activeRoute.queryParamMap.subscribe(
      params => {
        if (params['params']['customer'] == 'true') {
          this.queryCustomer = true;
          this.querySupplier = false;
        }
        if (params['params']['supplier'] == 'true') {
          this.querySupplier = true;
          this.queryCustomer = false;
        }
      },
      er => {
        console.log(er);
      }
    );
  }

  //T???o danh s??ch s??? ng??y-th??ng-n??m sinh
  birthInit(begin: number, end: number) {
    var a = new Array();
    if (begin < end) {
      for (let i = begin; i <= end; i++) {
        a.push(i);
      }
    } else if (begin > end) {
      for (let i = begin; i >= end; i--) {
        a.push(i);
      }
    }

    return a;
  }

  dobPrepare() {
    this.daySource = this.birthInit(1, 31);
    this.dayList = this.daySource;
    this.monthSource = this.birthInit(1, 12);
    this.monthList = this.monthSource;
    this.yearSource = this.birthInit(new Date().getFullYear(), 1900);
    this.yearList = this.yearSource;
  }

  //Filter d??? li???u ng??y th??ng n??m sinh
  handleFilterDay(value) {
    this.dayList = this.daySource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }
  handleFilterMonth(value) {
    this.monthList = this.monthSource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }
  handleFilterYear(value) {
    this.yearList = this.yearSource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  //T???o ho???c c???p nh???t KH
  createNewCustomer() {
    //this.assignValue();
    var value = this.formCreate.value;
    if (value.source == 'employee') {
      value.employeeId = value.employees ? value.employees.id : '';
    }
    else {
      value.employeeId = '';
    }

    if (value.source != 'other') {
      value.note = '';
    }
    if (value.birthDay == 'null') {
      value.birthDay = '';
    }
    if (value.birthMonth == 'null') {
      value.birthMonth = '';
    }
    if (value.birthYear == 'null') {
      value.birthYear = '';
    }

    this.isChange = true;
    this.service.createUpdateCustomer(value, this.cusId).subscribe(
      rs => {
        this.closeModal(rs);
      },
      er => {
        //Load l???i grid view tr??n component cha

        console.log(er);
      }
    );
  }


  //Cho ph??p field phone ch??? nh???p s???
  onlyGetNumbers(formControlName) {
    this.formCreate.controls[formControlName].setValue(this.formCreate.get(formControlName).value.replace(/[^0-9.]/g, ''));
  }

  //????ng dialog
  // closeWindow(result: any) {
  //   if (this.isChange) {
  //     if (result == null) {
  //       this.window.close(true);
  //     }
  //     else {
  //       this.window.close(result);
  //     }
  //   } else {
  //     this.window.close(false);
  //   }
  // }
  closeModal(rs) {
    if (this.isChange) {
      if (rs) {
        this.activeModal.close(rs);
      } else {
        this.activeModal.close(true);
      }
    }
    else {
      this.activeModal.dismiss();
    }
  }

  //L???y th??ng tin KH theo id load l??n dialog form
  getCustomerInfo() {
    if (this.cusId != null) {
      this.service.getPartner(this.cusId).subscribe(
        rs => {
          this.formCreate.patchValue(rs);
          this.checkedHistoriesList = rs.histories;
          rs.histories.forEach(e => {
            (this.formCreate.get('histories') as FormArray).push(new FormControl(e));
          });
          if (rs.city && rs.city.code) {
            this.getCity.setValue(rs.city);
            this.getDistrictList(rs.city.code);
            this.getDistrict.enable();
          } else {
            this.getDistrict.disable();
          }
          if (rs.district && rs.district.code) {
            this.getDistrict.setValue(rs.district);
            this.getWardList(rs.district.code);
            this.getWard.enable();
          } else {
            this.getWard.disable();
          }
          if (rs.ward && rs.ward.code) {
            this.getWardList(rs.district.code);
            this.getWard.setValue(rs.ward);
          }
          this.employeeSimpleFilter = _.unionBy(this.employeeSimpleFilter, [rs.employees], 'id');

        },
        er => {
          console.log(er);
        }
      )
    }
  }
  ////=====================L???y ds b???nh==========================
  getHistories() {
    this.service.getHistories().subscribe(
      rs => {
        this.historiesList = rs;
      },
      er => console.log(er)
    )
  }

  checkboxChange(hist: HistorySimple, isCheck: boolean) {
    const histFormArr = <FormArray>this.formCreate.get('histories');

    if (isCheck) {
      histFormArr.push(new FormControl(hist));
    } else {
      let index = histFormArr.controls.findIndex(x => x.value.id == hist.id);
      histFormArr.removeAt(index);
    }
  }

  checked(item: HistorySimple) {
    if (this.checkedHistoriesList.findIndex(x => x.id == item.id) >= 0) {
      return true;
    } else {
      return false;
    }
  }
  ////=====================L???y nh??m KH==========================
  getPartnerCategories() {
    this.service.getPartnerCategories().subscribe(
      rs => {
        this.fullCategoriesList = rs['items'] as PartnerCategorySimple[];
      },
      er => console.log(er)
    )
  }

  ////=====================S??? ki???n thay ?????i t???nh tp - qu???n huy???n==========================
  changeProvince() {
    this.getDistrict.setValue(null);
    this.getWard.setValue(null);
    this.getWard.disable();
    var cityValue = this.getCity.value as City;
    if (cityValue != null) {
      this.getDistrictList(cityValue.code);
      this.getDistrict.enable();
    }
    else {
      this.getDistrict.disable();
    }
  }

  changeDistrict() {
    this.getWard.setValue(null);
    var districtValue = this.getDistrict.value as District;
    if (districtValue != null) {
      this.getWardList(districtValue.code);
      this.getWard.enable();
    }
    else {
      this.getWard.disable();
    }
  }

  ////======================L???c d??? li???u TP T???nh - Qu???n Huy???n - Ph?????ng X??=========================
  filterProvince(e: string) {
    this.provincesFilter = this.provincesList.filter((s) => s.name.toLowerCase().indexOf(e.toLowerCase()) !== -1);
  }
  filterDistrict(e: string) {
    this.districtsFilter = this.districtsList.filter((s) => s.name.toLowerCase().indexOf(e.toLowerCase()) !== -1);
  }
  filterWard(e: string) {
    this.wardsFilter = this.wardsList.filter((s) => s.name.toLowerCase().indexOf(e.toLowerCase()) !== -1);
  }

  //=======================L???y TP T???nh - Qu???n Huy???n - Ph?????ng X??=============================
  getProvinceList() {
    if (this.cusId == null) {
      this.getDistrict.disable();
      this.getWard.disable();
    }
    var request = new AshipRequest();
    this.service.getProvinceAship(request).subscribe(
      rs => {
        this.provincesList = rs;
        this.provincesFilter = rs;
      },
      er => {
        console.log(er);
      }
    )
  }

  getDistrictList(parentCode) {
    var request = new AshipRequest();
    request.data = new AshipData();
    request.data.code = parentCode;
    this.service.getDistrictAship(request).subscribe(
      rs => {
        this.districtsList = rs;
        this.districtsFilter = rs;
      },
      er => {
        console.log(er);
      }
    )
  }

  getWardList(parentCode) {

    var request = new AshipRequest();
    request.data = new AshipData();
    request.data.code = parentCode;
    this.service.getWardAship(request).subscribe(
      rs => {
        this.wardsList = rs;
        this.wardsFilter = rs;
      },
      er => {
        console.log(er);
      }
    )
  }

  getProDisWrd() {
    this.getProvinceList();
  }

  getEmployeesList() {
    var empPn = new EmployeePaged;
    empPn.limit = this.limit;
    empPn.offset = this.skip;
    this.service.getEmployeeSimpleList(empPn).subscribe(
      rs => {
        this.employeeSimpleFilter = rs;
      });
  }

  // filterChangeCombobox() {
  //   this.employeeCbx.filterChange.asObservable().pipe(
  //     debounceTime(300),
  //     tap(() => this.employeeCbx.loading = true),
  //     switchMap(val => this.searchEmployees(val.toString().toLowerCase()))
  //   ).subscribe(
  //     rs => {
  //       this.employeeSimpleFilter = rs;
  //       this.employeeCbx.loading = false;
  //     }
  //   )
  // }

  searchEmployees(search) {
    var empPaged = new EmployeePaged();
    empPaged.position = "doctor";
    empPaged.limit = this.limit;
    empPaged.offset = this.skip;
    if (search != null) {
      empPaged.search = search.toLowerCase();
    }
    return this.service.getEmployeeSimpleList(empPaged);
  }

  windowTitle() {
    if (!this.cusId && this.queryCustomer) {
      return 'Th??m m???i kh??ch h??ng';
    } else if (this.cusId && this.queryCustomer) {
      return 'C???p nh???t th??ng tin kh??ch h??ng';
    } else if (!this.cusId && this.querySupplier) {
      return 'Th??m m???i nh?? cung c???p';
    } else if (this.cusId && this.querySupplier) {
      return 'C???p nh???t th??ng tin nh?? cung c???p';
    }
  }

  checkAddressApi() {
    var address = this.getStreet.value;
    if (!address) {
      return false;
    }
    this.service.checkAddressApi(address).subscribe(
      rs => {
        this.checkedText = this.getStreet.value;
        this.addressCheck = rs['result'];
        this.addressCheck = this.addressCheck.slice(0, 5);
        console.log(rs);
        this.chooseAddress(rs['result'][0], 0);
      }
    )
  }

  chooseAddress(adr: AddressCheckApi, e: number) {
    this.getAddressCheckDetail.setValue(e);
    var city = { code: adr.cityCode, name: adr.cityName };
    var district = { code: adr.districtCode, name: adr.districtName };
    var ward = { code: adr.wardCode, name: adr.wardName };
    this.getStreet.setValue(adr.shortAddress);
    if (city.code) {
      this.getCity.setValue(city);
      this.changeProvince();
    } else {
      this.getCity.setValue(null);
    }
    if (district.code) {
      this.getDistrict.enable();
      this.getDistrict.setValue(district);
      this.changeDistrict();
    } else {
      this.getDistrict.setValue(null);
    }
    if (ward.code) {
      this.getWard.enable();
      this.getWard.setValue(ward);
    } else {
      this.getWard.setValue(null);
    }
    // this.addressCheck = [];
  }

  get getStreet() {
    return this.formCreate.get('street');
  }

  get getCity() {
    return this.formCreate.get('city');
  }

  get getDistrict() {
    return this.formCreate.get('district');
  }

  get getWard() {
    return this.formCreate.get('ward');
  }

  get getAddressCheckDetail() {
    return this.formCreate.get('addressCheckDetail');
  }
}
