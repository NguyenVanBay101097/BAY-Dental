import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { EmployeeService } from '../employee.service';

@Component({
  selector: 'app-emp-categories-create-update',
  templateUrl: './emp-categories-create-update.component.html',
  styleUrls: ['./emp-categories-create-update.component.css']
})
export class EmpCategoriesCreateUpdateComponent implements OnInit {

  constructor(private fb: FormBuilder, public window: WindowRef, private service: EmployeeService) { }
  formCreate: FormGroup;
  typeList = new Array<{ type: string; id: number }>();
  ngOnInit() {
    this.formCreate = this.fb.group({
      name: [null, Validators.required],
      type: [null, Validators.required],
    });

    this.typeList.push({ type: "Bác sĩ", id: 1 });
    this.typeList.push({ type: "Phụ tá", id: 2 });
    this.typeList.push({ type: "Khác", id: 3 });

  }

  closeWindow(result: any) {
    if (result) {
      this.window.close(result);
    } else {
      this.window.close();
    }
  }

  createEmpCategory() {
    var categ = this.formCreate.value;
    categ.type = categ.type.type;
    this.service.createEmployeeCategory(categ).subscribe(
      rs => {
        this.closeWindow(categ);
      },
      er => {
        console.log(er);
      }
    )
  }

}
