import { Component, OnInit } from "@angular/core";
import { FormGroup, FormBuilder } from "@angular/forms";
import {
  accountAccountDefault,
  AccountAccountService,
} from "../account-account.service";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
  CompanyService,
  CompanyPaged,
} from "src/app/companies/company.service";
import { map } from "rxjs/operators";

@Component({
  selector: "app-account-account-form",
  templateUrl: "./account-account-form.component.html",
  styleUrls: ["./account-account-form.component.css"],
})
export class AccountAccountFormComponent implements OnInit {
  title: string;
  type: string;
  itemId: string;
  accountForm: FormGroup;

  companies: any[];

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private accountAccountService: AccountAccountService,
    private companyService: CompanyService
  ) {}

  ngOnInit() {
    this.accountForm = this.fb.group({
      active: null,
      code: null,
      company: null,
      companyId: null,
      internalType: null,
      name: null,
      note: null,
      reconcile: null,
      userType: null,
      userTypeId: null,
    });

    setTimeout(() => {
      if (!this.itemId) this.defaultGet();
      else this.get();
      this.loadCompanies();
    });
  }

  defaultGet() {
    var val = new accountAccountDefault();
    val.type = this.type;
    this.accountAccountService.defaultGet(val).subscribe(
      (result) => {
        this.accountForm.patchValue(result);
      },
      (err) => {
        console.log(err);
        this.activeModal.dismiss();
      }
    );
  }

  get() {
    this.accountAccountService.get(this.itemId).subscribe(
      (result) => {
        this.accountForm.patchValue(result);
      },
      (err) => {
        console.log(err);
        this.activeModal.dismiss();
      }
    );
  }

  loadCompanies() {
    var val = new CompanyPaged();
    this.companyService.getPaged(val).subscribe(
      (result) => {
        this.companies = result.items;
      },
      (err) => {
        console.log(err);
      }
    );
  }

  save() {
    var value = this.accountForm.value;
    value.companyId = value.company ? value.company.id : null;
    if (!this.itemId) {
      this.accountAccountService.create(value).subscribe(
        (result) => {
          this.activeModal.close(true);
        },
        (err) => {
          console.log(err);
        }
      );
    } else {
      this.accountAccountService.update(this.itemId, value).subscribe(
        (result) => {
          this.activeModal.close(result);
        },
        (err) => {
          console.log(err);
        }
      );
    }
  }

  cancel() {
    this.activeModal.dismiss();
  }
}
