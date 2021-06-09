import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
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
      memberArrayForm: this.fb.array([])
    })
    this.loadDataFromApi();
  }

  onAddMemberLevel() {
    this.memberArray.push(
      this.fb.group({
        name: ['',
          {
            validators: [Validators.required, this.isNameDup()]
          }
        ],
        point: ['',
          {
            validators: [Validators.required, this.isPointDup()]
          }
        ],
        color: '0',
      })
    )
  }

  loadDataFromApi() {
    this.memberLevelService.get().subscribe((result) => {
      this.memberArray.clear();
      result.forEach((member) => {
        this.memberArray.push(this.fb.group({
          name: [member.name,
          {
            validators: [Validators.required, this.isNameDup()]
          }
          ],
          point: [member.point,
          {
            validators: [Validators.required, this.isPointDup()]
          }
          ],
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
    console.log(this.formGroup);
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

  isNameDup() {
    const validator: ValidatorFn = (formArray: FormArray) => {
      const names = this.f.map(value => value.get('name').value);
      const hasDuplicate = names.some(
        (name, index) => names.indexOf(name, index + 1) != -1
      );
      return hasDuplicate ? { duplicate: true } : null;
    };
    return validator;
  }

  isPointDup() {
    const validator: ValidatorFn = (formArray: FormArray) => {
      const names = this.f.map(value => value.get('point').value);
      const hasDuplicate = names.some(
        (name, index) => names.indexOf(name, index + 1) != -1
      );
      return hasDuplicate ? { duplicate: true } : null;
    };
    return validator;
  }
}
