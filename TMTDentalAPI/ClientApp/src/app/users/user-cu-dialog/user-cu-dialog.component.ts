import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../user.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CompanyBasic, CompanySimple, CompanyService, CompanyPaged } from 'src/app/companies/company.service';
import * as _ from 'lodash';
import { ResGroupBasic, ResGroupService, ResGroupPaged } from 'src/app/res-groups/res-group.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

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
  listGroups: ResGroupBasic[] = [];

  title: string;
  constructor(private fb: FormBuilder, private userService: UserService, public activeModal: NgbActiveModal,
    private companyService: CompanyService, private resGroupService: ResGroupService,
    private showErrorService: AppSharedShowErrorService) {
  }

  ngOnInit() {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      userName: null,
      passWord: null,
      email: null,
      companyId: null,
      company: [null, Validators.required],
      companies: [[]],
      avatar: null,
      groups: [[]],
    });

    setTimeout(() => {
      if (this.id) {
        this.userService.get(this.id).subscribe(result => {
          this.userForm.patchValue(result);
          if (result.company) {
            this.listCompanies = _.unionBy(this.listCompanies, [result.company], 'id');
          }

          if (result.companies) {
            this.listCompanies = _.unionBy(this.listCompanies, result.companies, 'id');
          }

          if (result.groups) {
            this.listGroups = _.unionBy(this.listGroups, result.groups, 'id');
          }
        });
      } else {
        this.userService.defaultGet().subscribe(result => {
          this.userForm.patchValue(result);
          if (result.company) {
            this.listCompanies = _.unionBy(this.listCompanies, [result.company], 'id');
          }

          if (result.companies) {
            this.listCompanies = _.unionBy(this.listCompanies, result.companies, 'id');
          }
        });
      }

      this.loadListCompanies();
      this.loadListGroups();
    });
  }

  loadListCompanies() {
    var val = new CompanyPaged();
    this.companyService.getPaged(val).subscribe(result => {
      this.listCompanies = _.unionBy(this.listCompanies, result.items, 'id');
    });
  }

  loadListGroups() {
    var val = new ResGroupPaged();
    this.resGroupService.getPaged(val).subscribe(result => {
      this.listGroups = _.unionBy(this.listGroups, result.items, 'id');
    });
  }

  onSave() {
    if (!this.userForm.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(() => {
      this.activeModal.close(true);
    }, err => {
      this.showErrorService.show(err);
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
