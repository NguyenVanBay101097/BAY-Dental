import { Component, OnInit, ViewChild } from '@angular/core';
import { UomService, UoMDisplay } from '../uom.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { UoMCategoryBasic, UoMCategoryPaged, UomCategoryService } from 'src/app/uom-categories/uom-category.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-uom-cr-up',
  templateUrl: './uom-cr-up.component.html',
  styleUrls: ['./uom-cr-up.component.css']
})
export class UomCrUpComponent implements OnInit {

  @ViewChild('cateCbx', { static: true }) cateCbx: ComboBoxComponent;

  id: string;
  formGroup: FormGroup;
  title: string;
  filterdCategories: UoMCategoryBasic[] = [];
  uom: UoMDisplay;

  constructor(
    private uoMService: UomService,
    private activeModal: NgbActiveModal,
    private uomCategoryService: UomCategoryService,
    private fb: FormBuilder, private showErrorService: AppSharedShowErrorService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      categoryId: ['', Validators.required],
      uomType: ['reference', Validators.required],
      active: true,
      factor: [1, Validators.required],
      factorInv: 0,
      rounding: [0.0100]
    });

    if (this.id) {
      setTimeout(() => {
        this.loadFormApi();
      });
    }

    this.cateCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.cateCbx.loading = true)),
      switchMap(value => this.searchUoMCategories(value))
    ).subscribe(result => {
      this.filterdCategories = result;
      this.cateCbx.loading = false;
    });

    setTimeout(() => {
      this.loadUoMCategory();
    });
  }

  get uomTypeValue() {
    return this.formGroup.get('uomType').value;
  }

  onChangeUoMType(value) {
    if (value == 'reference') {
      this.formGroup.get('factor').setValue(1);
    }
  }

  loadFormApi() {
    this.uoMService.get(this.id).subscribe(
      result => {
        this.formGroup.patchValue(result);
      }
    )
  }

  loadUoMCategory() {
    this.searchUoMCategories().subscribe(
      result => {
        this.filterdCategories = result;
      }
    )
  }

  searchUoMCategories(q?: string) {
    var val = new UoMCategoryPaged();
    val.search = q || '';
    val.limit = 20;
    val.offset = 0;
    return this.uomCategoryService.autocomplete(val);
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  onSave() {
    if (this.formGroup.invalid) {
      return false;
    }

    var value = this.formGroup.value;

    if (this.id) {
      this.uoMService.update(this.id, value).subscribe(() => {
        value.id = this.id;
        this.activeModal.close(value);
      }, err => {
        this.showErrorService.show(err);
      });
    } else {
      this.uoMService.create(value).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        this.showErrorService.show(err);
      });
    }
  }

}
