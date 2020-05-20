import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PartnerCategoryService } from '../partner-category.service';

@Component({
  selector: 'app-partner-category-import',
  templateUrl: './partner-category-import.component.html',
  styleUrls: ['./partner-category-import.component.css']
})
export class PartnerCategoryImportComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder,
    private partnerCategoryService: PartnerCategoryService) { }

  formGroup: FormGroup;
  title = 'Import';
  errors: any = [];

  ngOnInit() {
    this.formGroup = this.fb.group({
      fileBase64: [null, Validators.required],
    });
  }

  onFileChange(data) {
    this.formGroup.get('fileBase64').patchValue(data);
  }

  import() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    this.partnerCategoryService.actionImport(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    });
  }
}