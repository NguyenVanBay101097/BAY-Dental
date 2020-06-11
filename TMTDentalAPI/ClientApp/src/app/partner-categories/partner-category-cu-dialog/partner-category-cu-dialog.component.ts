import { Component, OnInit, Inject, ViewChild, ElementRef, Input } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Observable } from 'rxjs';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PartnerCategoryService, PartnerCategoryBasic, PartnerCategoryPaged } from '../partner-category.service';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-partner-category-cu-dialog',
  templateUrl: './partner-category-cu-dialog.component.html',
  styleUrls: ['./partner-category-cu-dialog.component.css']
})

export class PartnerCategoryCuDialogComponent implements OnInit {
  myform: FormGroup;
  filterdCategories: PartnerCategoryBasic[];
  @ViewChild('form', { static: true }) formView: any;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  @Input() public id: string;
  title: string;
  submitted = false;


  constructor(private fb: FormBuilder, private partnerCategoryService: PartnerCategoryService,
    public activeModal: NgbActiveModal) {
  }

  ngOnInit() {
    this.myform = this.fb.group({
      name: ['', Validators.required],
    });

    if (this.id) {
      setTimeout(() => {
        this.partnerCategoryService.get(this.id).subscribe((result) => {
          this.myform.patchValue(result);
        });
      });
    }

    // this.categCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.categCbx.loading = true)),
    //   switchMap(value => this.searchCategories(value))
    // ).subscribe(result => {
    //   this.filterdCategories = result;
    //   this.categCbx.loading = false;
    // });
  }

  searchCategories(q?: string): Observable<PartnerCategoryBasic[]> {
    var val = new PartnerCategoryPaged();
    val.search = q;
    return this.partnerCategoryService.autocomplete(val);
  }

  onSave() {
    this.submitted = true;

    if (!this.myform.valid) {
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
    var val = this.myform.value;
    val.parentId = val.parent ? val.parent.id : null;
    if (!this.id) {
      return this.partnerCategoryService.create(val);
    } else {
      return this.partnerCategoryService.update(this.id, val);
    }
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.close();
  }

  get f() {
    return this.myform.controls;
  }
}


