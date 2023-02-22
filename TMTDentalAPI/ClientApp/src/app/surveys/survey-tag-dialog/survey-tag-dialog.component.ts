import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { SurveyTagService } from '../survey-tag.service';

@Component({
  selector: 'app-survey-tag-dialog',
  templateUrl: './survey-tag-dialog.component.html',
  styleUrls: ['./survey-tag-dialog.component.css']
})
export class SurveyTagDialogComponent implements OnInit {
  formGroup: FormGroup;
  @ViewChild('form', { static: true }) formView: any;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  @Input() public id: string;
  title: string;
  submitted = false;
  colorSelected = 0;

  constructor(private fb: FormBuilder, private surveyTagService: SurveyTagService,
    public activeModal: NgbActiveModal) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required]
    });

    if (this.id) {
      setTimeout(() => {
        this.surveyTagService.get(this.id).subscribe((rs: any) => {
          this.formGroup.patchValue(rs);
          this.colorSelected = parseInt(rs.color) || 0;
        });
      });
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(result => {
      if (result) {
        this.activeModal.close(result);
      } else {
        this.activeModal.close(true);
      }
    }, err => {
      console.log(err);
    });
  }

  saveOrUpdate() {
    var val = this.formGroup.value;
    val.color = this.colorSelected;
    if (!this.id) {
      return this.surveyTagService.create(val);
    } else {
      return this.surveyTagService.update(this.id, val);
    }
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.close();
  }

  get f() {
    return this.formGroup.controls;
  }

  clickColor(i) {
    this.colorSelected = i;
  }

}
