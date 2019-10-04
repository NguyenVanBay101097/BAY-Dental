import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { PartnerService } from '../partner.service';
import { WindowRef, WindowService } from '@progress/kendo-angular-dialog';
import { PartnerDisplay, PartnerCategorySimple, District, AshipRequest, City, Ward, AshipData } from '../partner-simple';
import { ActivatedRoute } from '@angular/router';
import { FileRestrictions, SelectEvent, RemoveEvent, UploadEvent, SuccessEvent, FileInfo } from '@progress/kendo-angular-upload';
import { EmployeeSimple, EmployeePaged } from 'src/app/employees/employee';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { switchMap, tap, debounceTime } from 'rxjs/operators';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';



@Component({
  selector: 'app-partner-create-update',
  templateUrl: './partner-create-update.component.html',
  styleUrls: ['./partner-create-update.component.css']
})


export class PartnerCreateUpdateComponent implements OnInit {

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

  districtsList: District[] = [];
  provincesList: City[] = [];
  wardsList: Ward[] = [];
  districtsFilter: District[] = [];
  provincesFilter: City[] = [];
  wardsFilter: Ward[] = [];
  employeeSimpleFilter: EmployeeSimple[] = [];

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
      gender: 'Other',
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
      note: null
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


    // this.filterChangeCombobox();

  }

  //Load lại dữ liệu khi đổi routing giữa Customer <=> Supplier
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

  //Tạo danh sách số ngày-tháng-năm sinh
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

  //Filter dữ liệu ngày tháng năm sinh
  handleFilterDay(value) {
    this.dayList = this.daySource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }
  handleFilterMonth(value) {
    this.monthList = this.monthSource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }
  handleFilterYear(value) {
    this.yearList = this.yearSource.filter((s) => s.toString().toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  //Tạo hoặc cập nhật KH
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
    console.log(value);

    this.isChange = true;
    this.service.createUpdateCustomer(value, this.cusId).subscribe(
      rs => {
        this.closeModal();
      },
      er => {
        console.log(er);
      }
    );
  }



  //Cho phép field phone chỉ nhập số
  onlyGetNumbers(formControlName) {
    this.formCreate.controls[formControlName].setValue(this.formCreate.get(formControlName).value.replace(/[^0-9.]/g, ''));
  }

  //Đóng dialog
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
  closeModal() {
    if (this.isChange) {
      this.activeModal.close(true);
    }
    else {
      this.activeModal.dismiss();
    }
  }

  //Lấy thông tin KH theo id load lên dialog form
  getCustomerInfo() {
    if (this.cusId != null) {
      this.service.getPartner(this.cusId).subscribe(
        rs => {
          this.formCreate.patchValue(rs);
          if (rs.city && rs.city.code) {
            this.formCreate.get('city').setValue(rs.city);
            this.getDistrictList(rs.city.code);
            this.formCreate.get('district').enable();
          } else {
            this.formCreate.get('district').disable();
          }
          if (rs.district && rs.district.code) {
            this.formCreate.get('district').setValue(rs.district);
            this.getWardList(rs.district.code);
            this.formCreate.get('ward').enable();
          } else {
            this.formCreate.get('ward').disable();
          }
          if (rs.ward && rs.ward.code) {
            this.getWardList(rs.district.code);
            this.formCreate.get('ward').setValue(rs.ward);
          }
          this.employeeSimpleFilter = _.unionBy(this.employeeSimpleFilter, [rs.employees], 'id');
        },
        er => {
          console.log(er);
        }
      )
    }
  }

  ////=====================Lấy nhóm KH==========================
  getPartnerCategories() {
    this.service.getPartnerCategories().subscribe(
      rs => {
        this.fullCategoriesList = rs['items'] as PartnerCategorySimple[];
      },
      er => console.log(er)
    )
  }

  ////=====================Sự kiện thay đổi tỉnh tp - quận huyện==========================
  changeProvince() {
    this.formCreate.get('district').setValue(null);
    this.formCreate.get('ward').setValue(null);
    this.formCreate.get('ward').disable();
    var cityValue = this.formCreate.get('city').value as City;
    if (cityValue != null) {
      this.getDistrictList(cityValue.code);
      this.formCreate.get('district').enable();
    }
    else {
      this.formCreate.get('district').disable();
    }
  }

  changeDistrict() {
    this.formCreate.get('ward').setValue(null);
    var districtValue = this.formCreate.get('district').value as District;
    if (districtValue != null) {
      this.getWardList(districtValue.code);
      this.formCreate.get('ward').enable();
    }
    else {
      this.formCreate.get('ward').disable();
    }
  }

  ////======================Lọc dữ liệu TP Tỉnh - Quận Huyện - Phường Xã=========================
  filterProvince(e: string) {
    this.provincesFilter = this.provincesList.filter((s) => s.name.toLowerCase().indexOf(e.toLowerCase()) !== -1);
  }
  filterDistrict(e: string) {
    this.districtsFilter = this.districtsList.filter((s) => s.name.toLowerCase().indexOf(e.toLowerCase()) !== -1);
  }
  filterWard(e: string) {
    this.wardsFilter = this.wardsList.filter((s) => s.name.toLowerCase().indexOf(e.toLowerCase()) !== -1);
  }

  //=======================Lấy TP Tỉnh - Quận Huyện - Phường Xã=============================
  getProvinceList() {
    if (this.cusId == null) {
      this.formCreate.get('district').disable();
      this.formCreate.get('ward').disable();
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
      return 'Thêm mới khách hàng';
    } else if (this.cusId && this.queryCustomer) {
      return 'Cập nhật thông tin khách hàng';
    } else if (!this.cusId && this.querySupplier) {
      return 'Thêm mới nhà cung cấp';
    } else if (this.cusId && this.querySupplier) {
      return 'Cập nhật thông tin nhà cung cấp';
    }
  }
}
