import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { EmpCategoryService } from '../emp-category.service';
import { EmployeeCategoryDisplay } from '../emp-category';

@Component({
  selector: 'app-emp-categories-create-update',
  templateUrl: './emp-categories-create-update.component.html',
  styleUrls: ['./emp-categories-create-update.component.css']
})
export class EmpCategoriesCreateUpdateComponent implements OnInit {

  constructor(private fb: FormBuilder, public window: WindowRef, private service: EmpCategoryService) { }
  formCreate: FormGroup;
  empCategId: string;

  typeList = new Array<{ type: string; id: string }>();
  ngOnInit() {
    this.formCreate = this.fb.group({
      name: [null, Validators.required],
      typeObj: [null, Validators.required],
    });

    this.getEmployeeCategoryInfo();

    this.typeList.push({ type: "Bác sĩ", id: "doctor" });
    this.typeList.push({ type: "Phụ tá", id: "assistant" });
    this.typeList.push({ type: "Khác", id: "other" });
    this.formCreate.get('typeObj').setValue(this.typeList.find(x => x.type == 'Bác sĩ'));

  }

  closeWindow(result: any) {
    if (result) {
      this.window.close(result);
    } else {
      this.window.close();
    }
  }

  getEmployeeCategoryInfo() {
    if (this.empCategId != null) {
      this.service.getCategEmployee(this.empCategId).subscribe(
        rs => {
          this.formCreate.patchValue(rs);
          var typeObj = this.typeList.find(x => x.id == rs.type);
          this.formCreate.get('typeObj').setValue(typeObj);
        },
        er => {
          console.log(er);
        }
      )
    }
  }

  //Tạo hoặc cập nhật nhóm NV
  createUpdateEmployee() {
    var value = new EmployeeCategoryDisplay;
    value = this.formCreate.value;
    console.log(this.formCreate.get('typeObj').value.type);
    value.type = this.getEmployeeType(this.formCreate.get('typeObj').value.type);
    this.service.createUpdateEmployeeCategory(value, this.empCategId).subscribe(
      rs => {
        this.closeWindow(rs);
      },
      er => {
        console.log(er);
      }
    );
  }

  getEmployeeType(type: string) {
    switch (type) {
      case "Bác sĩ":
        return "doctor";
      case "Phụ tá":
        return "assistant";
      case "Khác":
        return "other";
      case "doctor":
        return "Bác sĩ";
      case "assistant":
        return "Trợ lý";
      case "other":
        return "Khác";
      default:
        break;
    }
  }

}
