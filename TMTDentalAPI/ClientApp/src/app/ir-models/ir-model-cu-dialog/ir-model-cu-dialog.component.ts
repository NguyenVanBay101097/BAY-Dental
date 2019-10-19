import { Component, OnInit, Inject, ViewChild, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IRModelService } from '../ir-model.service';

@Component({
  selector: 'app-ir-model-cu-dialog',
  templateUrl: './ir-model-cu-dialog.component.html',
  styleUrls: ['./ir-model-cu-dialog.component.css']
})

export class IrModelCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  @Input() public id: string;

  constructor(private fb: FormBuilder, private modelService: IRModelService,
    public activeModal: NgbActiveModal) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      model: ['', Validators.required],
    });

    if (this.id) {
      this.modelService.get(this.id).subscribe((result) => {
        this.formGroup.patchValue(result);
      });
    }
  }

  onSave() {
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
    val.parentId = val.parent ? val.parent.id : null;
    if (!this.id) {
      return this.modelService.create(val);
    } else {
      return this.modelService.update(this.id, val);
    }
  }
}


