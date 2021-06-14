import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';
import { update } from 'lodash';
import { forkJoin } from 'rxjs';
import { PartnerService } from 'src/app/partners/partner.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { MemberLevelService } from '../member-level.service';

@Component({
  selector: 'app-member-level-create-update',
  templateUrl: './member-level-create-update.component.html',
  styleUrls: ['./member-level-create-update.component.css']
})
export class MemberLevelCreateUpdateComponent implements OnInit {
  memberLevels: any[] = [];
  memberColors = [
    { id: '0', value: "#6C757D", checked: true },
    { id: '1', value: "#F06050", checked: false },
    { id: '2', value: "#F4A460", checked: false },
    { id: '3', value: "#F7CD1F", checked: false },
    { id: '4', value: "#6CC1ED", checked: false },
    { id: '5', value: "#814968", checked: false },
    { id: '6', value: "#EB7E7F", checked: false },
    { id: '7', value: "#2C8397", checked: false },
    { id: '8', value: "#475577", checked: false },
    { id: '9', value: "#D6145F", checked: false },
    { id: '10', value: "#30C381", checked: false },
    { id: '11', value: "#9365B8", checked: false },
  ]
  formGroup: FormGroup;
  submitted: boolean = false;

  get f() {
    return this.memberArray.controls;
  }

  get memberArray() {
    return this.formGroup.controls.memberArrayForm as FormArray;
  }
  constructor(
    private memberLevelService: MemberLevelService,
    private notifyService: NotifyService,
    private modalService: NgbModal,
    private router: Router,
    private partnerService: PartnerService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      memberArrayForm: this.fb.array([], this.customDuplicateValidation)
    })
    this.loadDataFromApi();
  }

  onAddMemberLevel() {
    this.memberArray.push(
      this.fb.group({
        name: ['', Validators.required],
        point: [null, Validators.required],
        color: '0',
      })
    )
  }

  loadDataFromApi() {
    this.memberLevelService.get().subscribe((result) => {
      this.memberArray.clear();
      result.forEach((member) => {
        this.memberArray.push(this.fb.group({
          name: [member.name, Validators.required],
          point: [member.point, Validators.required],
          color: member.color,
        }))
      })
      if (result.length == 0) {
        this.onAddMemberLevel();
      }
    })
  }

  onDeleteMemberLevel(index) {
    this.memberArray.removeAt(index);
  }

  onSave() {
    this.submitted = true;

    if (this.formGroup.invalid) {
      return;
    }

    var vals = this.formGroup.get('memberArrayForm').value;

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Hạng thành viên';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn lưu thiết lập hạng thành viên?';
    modalRef.componentInstance.body2 = 'Lưu ý: Hạng thành viên của khách hàng sẽ được cập nhật ngay sau khi xác nhận';
    modalRef.result.then(() => {
      this.memberLevelService.updateMember(vals).subscribe((res) => {
        this.partnerService.checkUpdateLevel().subscribe(() => {
          this.notifyService.notify("success", "Lưu hạng thành viên thành công");
          vals.length > 0 ?
            this.router.navigate(['member-level/list']) :
            this.router.navigate(['member-level/management']);
        });
      })
    });
  }

  customDuplicateValidation(formArray: FormArray) {
    const listName = _.groupBy(formArray.controls, c => c.get('name').value);
    const listPoint = _.groupBy(formArray.controls, c => c.get('point').value);

    for (let prop in listName) {
      if (prop) {
        if (listName[prop].length > 1) {
          _.forEach(listName[prop], function (item) {
            item.setErrors({ 'duplicateName': true })
          })
        }
        else {
          _.forEach(listName[prop], function (item) {
            item.setErrors(null)

          })
        }
      }
    }

    for (let prop in listPoint) {
        if (listPoint[prop].length > 1) {
          _.forEach(listPoint[prop], function (item) {
            if (item.value.point != null && item.value.point >=0) {
              if (item.hasError('duplicateName')) {
                item.setErrors({ 'duplicateName': true, 'duplicatePoint': true })
              } else {
                item.setErrors({ 'duplicatePoint': true });
              }
            }
          })
        }
        else {
          _.forEach(listPoint[prop], function (item) {
            if (item.hasError('duplicateName')) {
              item.setErrors({ 'duplicateName': true, 'duplicatePoint': false })
            } else {
              item.setErrors(null);
            }

          })
        }
    }

    return null;
  }

}
