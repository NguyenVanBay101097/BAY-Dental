import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerTitleService } from 'src/app/partner-titles/partner-title.service';

@Component({
  selector: 'app-partner-title-cu-dialog',
  templateUrl: './partner-title-cu-dialog.component.html',
  styleUrls: ['./partner-title-cu-dialog.component.css']
})
export class PartnerTitleCuDialogComponent implements OnInit {
  title: string;
  itemId: string;
  myForm: FormGroup;
  submitted = false;
  
  constructor(private fb: FormBuilder, 
    public activeModal: NgbActiveModal, 
    private partnerTitleService: PartnerTitleService) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      name: [null, Validators.required]
    });

    if (this.itemId) {
      setTimeout(() => {
        this.partnerTitleService.get(this.itemId).subscribe((result) => {
          this.myForm.patchValue(result);
        }, err => {
          console.log(err);
          this.activeModal.dismiss();
        });
      });
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.myForm.valid) {
      return false;
    }

    var value = this.myForm.value;
    if (!this.itemId) {
      this.partnerTitleService.create(value).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        console.log(err);
      })
    } else {
      this.partnerTitleService.update(this.itemId, value).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        console.log(err);
      })
    }
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.close();
  }

  get f() {
    return this.myForm.controls;
  }
}
