import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UomCategoryService, UoMCategoryBasic } from '../uom-category.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-uom-category-cr-up',
  templateUrl: './uom-category-cr-up.component.html',
  styleUrls: ['./uom-category-cr-up.component.css']
})
export class UomCategoryCrUpComponent implements OnInit {

  id: string;
  formGroup: FormGroup;
  title: string;
  uomCategory: UoMCategoryBasic;
  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private uoMCategoryService: UomCategoryService,
  ) { }

  ngOnInit() {
    this.uomCategory = new UoMCategoryBasic();
    this.formGroup = this.fb.group({
      name: ['', Validators.required]
    });
    if (this.id) {
      this.loadFormApi();
    }
  }

  loadFormApi() {
    this.uoMCategoryService.get(this.id).subscribe(
      result => {
        this.formGroup.patchValue(result);
      }
    )
  }

  onCancel() {
    this.activeModal.close();
  }

  onSave() {
    if (this.formGroup.invalid)
      return false;

    var value = this.formGroup.value;
    value.measureType = "none";
    if (this.id) {
      this.uoMCategoryService.update(this.id, value).subscribe(
        () => {
          value.id = this.id;
          this.activeModal.close(value);
        }
      )
    } else {
      this.uoMCategoryService.create(value).subscribe(
        result => {
          console.log(result);
          this.activeModal.close(result);
        }
      )
    }
  }

}
