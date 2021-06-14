import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { UserService } from 'src/app/users/user.service';
import { AgentService } from '../agent.service';

@Component({
  selector: 'app-agent-create-update-dialog',
  templateUrl: './agent-create-update-dialog.component.html',
  styleUrls: ['./agent-create-update-dialog.component.css']
})
export class AgentCreateUpdateDialogComponent implements OnInit {
  id: string;
  formGroup: FormGroup;
  submitted = false;
  isRoleRead = false;
  isRoleCreate = false;
  title: string;

  dayList: number[] = [];
  monthList: number[] = [];
  yearList: number[] = [];

  constructor(private fb: FormBuilder,
    public agentService: AgentService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private intlService: IntlService,
    private userService: UserService,
    private checkPermissionService: CheckPermissionService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ["", Validators.required],
      gender: "male",
      birthDayStr: '',
      birthMonthStr: '',
      birthYearStr: '',
      email: null,
      phone: null,
      jobTitle: null,
      address: null
    });

    setTimeout(() => {
      this.checkRole();
      this.reload();
      this.dayList = _.range(1, 32);
      this.monthList = _.range(1, 13);
      this.yearList = _.range(new Date().getFullYear(), 1900, -1);
    })
  }

  reload() {
    if (this.id) {
      this.agentService.get(this.id).subscribe((result) => {
        this.formGroup.patchValue(result);

        if (result.birthYear) {
          this.formGroup.get("birthYearStr").setValue(result.birthYear + '');
        }

        if (result.birthMonth) {
          this.formGroup.get("birthMonthStr").setValue(result.birthMonth + '');
        }

        if (result.birthDay) {
          this.formGroup.get("birthDayStr").setValue(result.birthDay + '');
        }

      });
    }
  }

  get f() { return this.formGroup.controls; }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.birthDay = val.birthDayStr ? parseInt(val.birthDayStr) : null;
    val.birthMonth = val.birthMonthStr ? parseInt(val.birthMonthStr) : null;
    val.birthYear = val.birthYearStr ? parseInt(val.birthYearStr) : null;

    if (this.id) {
      this.agentService.update(this.id, val).subscribe(
        () => {
          this.activeModal.close(true);
        },
      );
    } else {
      this.agentService.create(val).subscribe(
        (result) => {
          this.activeModal.close(result);
        },
      );
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  checkRole() {
    this.isRoleRead = this.checkPermissionService.check(["Catalog.Agent.Read"]);
    this.isRoleCreate = this.checkPermissionService.check(["Catalog.Agent.Create"]);
  }
}
