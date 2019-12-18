import { Component, OnInit } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { WindowRef, WindowCloseResult, WindowService } from '@progress/kendo-angular-dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EmployeeDisplay } from '../employee';
import { EmpCategoriesCreateUpdateComponent } from 'src/app/employee-categories/emp-categories-create-update/emp-categories-create-update.component';
import { EmpCategoryService } from 'src/app/employee-categories/emp-category.service';
import { EmployeeCategoryPaged, EmployeeCategoryBasic, EmployeeCategoryDisplay } from 'src/app/employee-categories/emp-category';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-employee-create-update',
  templateUrl: './employee-create-update.component.html',
  styleUrls: ['./employee-create-update.component.css']
})
export class EmployeeCreateUpdateComponent implements OnInit {

  constructor(private fb: FormBuilder, private service: EmployeeService,
    private empCategService: EmpCategoryService, public activeModal: NgbActiveModal, private modalService: NgbModal, private intlService: IntlService) { }
  empId: string;

  isChange: boolean = false;
  formCreate: FormGroup;
  windowOpened: boolean = false;
  title = 'Nhân viên';

  categoriesList: EmployeeCategoryBasic[] = [];
  categoriesList2: EmployeeCategoryDisplay[] = [];

  ngOnInit() {
    this.formCreate = this.fb.group({
      name: [null, Validators.required],
      phone: null,
      address: null,
      ref: null,
      identityCard: null,
      email: null,
      birthDay: null,
      birthDayObj: null,
      category: [null],
      isDoctor: false,
      isAssistant: false
    });
    this.loadAutocompleteTypes(null);
    this.getEmployeeInfo();
  }

  getEmployeeInfo() {
    if (this.empId != null) {
      this.service.getEmployee(this.empId).subscribe(
        rs => {
          this.formCreate.patchValue(rs);
          let birthDay = this.intlService.parseDate(rs.birthDay);
          this.formCreate.get('birthDayObj').patchValue(birthDay);
        },
        er => {
          console.log(er);
        }
      )
    }
  }

  //Tạo hoặc cập nhật NV
  createUpdateEmployee() {
    //this.assignValue();
    var value = this.formCreate.value;
    value.categoryId = value.category ? value.category.id : null;
    value.birthDay = this.intlService.formatDate(value.birthDayObj, 'd', 'en-US');
    this.isChange = true;
    this.service.createUpdateEmployee(value, this.empId).subscribe(
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

  // quickCreateCategory() {
  //   const windowRef: WindowRef = this.windowService.open(
  //     {
  //       title: 'Tạo nhóm mới',
  //       content: EmpCategoriesCreateUpdateComponent,
  //       minWidth: 250
  //     });
  //   this.windowOpened = true;

  //   windowRef.result.subscribe(
  //     (result) => {
  //       this.windowOpened = false;
  //       // console.log(result instanceof WindowCloseResult);
  //       if (!(result instanceof WindowCloseResult)) {
  //         console.log(result);
  //         this.categoriesList.push(result);
  //         this.formCreate.get('category').setValue(result);
  //       }
  //     }
  //   )
  // }

  quickCreateCategory() {
    const modalRef = this.modalService.open(EmpCategoriesCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });

    modalRef.result.then(
      result => {
        this.windowOpened = false;

        if (result && result.name) {
          console.log(result);
          this.categoriesList.push(result);
          this.formCreate.get('category').setValue(result);
        }
      },
      er => { }
    )
  }

  loadAutocompleteTypes(searchKw: string) {
    var empCategPaged = new EmployeeCategoryPaged;
    empCategPaged.limit = 20;
    empCategPaged.offset = 0;
    if (searchKw) {
      empCategPaged.search = searchKw.toLowerCase();
    }
    this.empCategService.autocompleteCategoryTypes(empCategPaged).subscribe(
      rs => {
        this.categoriesList = rs;
      },
      er => {
        console.log(er);
      }
    )
  }
}
