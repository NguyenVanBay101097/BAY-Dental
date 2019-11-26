import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../user.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CompanyBasic, CompanySimple, CompanyService, CompanyPaged } from 'src/app/companies/company.service';

@Component({
  selector: 'app-user-cu-dialog',
  templateUrl: './user-cu-dialog.component.html',
  styleUrls: ['./user-cu-dialog.component.css']
})
export class UserCuDialogComponent implements OnInit {
  id: string;
  userForm: FormGroup;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;

  listCompanies: CompanyBasic[] = [];

  title: string;
  constructor(private fb: FormBuilder, private userService: UserService, public activeModal: NgbActiveModal,
    private companyService: CompanyService) {
  }

  ngOnInit() {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      userName: null,
      passWord: null,
      email: null,
      companyId: null,
      company: [null, Validators.required],
      companies: [[]]
    });

    if (this.id) {
      this.userService.get(this.id).subscribe(result => this.userForm.patchValue(result));
    } else {
      this.userService.defaultGet().subscribe(result => this.userForm.patchValue(result));
    }

    this.loadListCompanies();
  }

  loadListCompanies() {
    var val = new CompanyPaged();
    this.companyService.getPaged(val).subscribe(result => {
      this.listCompanies = result.items;
    });
  }

  onSave() {
    if (!this.userForm.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(() => {
      this.activeModal.close(true);
    }, err => {
      console.log(err);
    });
  }

  saveOrUpdate() {
    var data = this.getBodyData();
    if (this.id) {
      return this.userService.update(this.id, data);
    } else {
      return this.userService.create(data);
    }
  }

  getBodyData() {
    var data = this.userForm.value;
    data.companyId = data.company.id;
    return data;
  }

  onCancel() {
    this.activeModal.close();
  }
}
