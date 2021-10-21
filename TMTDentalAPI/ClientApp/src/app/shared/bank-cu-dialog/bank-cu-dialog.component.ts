import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ResBankService } from 'src/app/res-banks/res-bank.service';

@Component({
  selector: 'app-bank-cu-dialog',
  templateUrl: './bank-cu-dialog.component.html',
  styleUrls: ['./bank-cu-dialog.component.css']
})
export class BankCuDialogComponent implements OnInit {
  @Input() title: string;
  formGroup: FormGroup;
  id: string;
  submitted: boolean = false;

  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private resBankService: ResBankService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required]
    })
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    this.resBankService.createUpdate(val, this.id).subscribe(res => {
      this.activeModal.close(res);
    })

  }
}
