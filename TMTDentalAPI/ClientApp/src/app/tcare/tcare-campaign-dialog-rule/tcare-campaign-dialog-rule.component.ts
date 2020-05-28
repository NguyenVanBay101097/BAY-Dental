import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
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

  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      beforeDays: [0],
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
