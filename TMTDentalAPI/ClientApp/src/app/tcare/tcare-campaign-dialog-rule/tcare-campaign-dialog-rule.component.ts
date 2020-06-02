import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService } from '../tcare.service';

@Component({
  selector: 'app-tcare-campaign-dialog-rule',
  templateUrl: './tcare-campaign-dialog-rule.component.html',
  styleUrls: ['./tcare-campaign-dialog-rule.component.css']
})
export class TcareCampaignDialogRuleComponent implements OnInit {

  title: string;
  cell: any;
  formGroup: FormGroup;
  conditions = [
    { name: 'birthday', text: 'Trước ngày sinh nhật' },
    { name: 'treatment', text: "Sau ngày điều trị cuối" },
    { name: 'customerGroup', text: 'Nhóm khách hàng' },
    { name: 'service', text: 'Dịch vụ' },
    { name: 'serviceGroup', text: 'Nhóm dịch vụ' }
  ];

  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      typeCondition: ['', Validators.required],
      valueCondition: ['', Validators.required],
      flagCondition: [false, Validators.required]
    });

    if (this.cell)
      this.loadFormApi();
  }

  loadFormApi() {
    this.formGroup.patchValue(this.cell);
  }

  onSave() {
    this.cell.beforeDays = this.formGroup.get('beforeDays').value;
    this.activeModal.close(this.cell);
  }

}
