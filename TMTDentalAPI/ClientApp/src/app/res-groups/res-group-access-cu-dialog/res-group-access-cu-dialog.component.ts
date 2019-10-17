import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IRModelBasic } from 'src/app/ir-models/ir-model';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { IRModelService } from 'src/app/ir-models/ir-model.service';
import { HttpParams } from '@angular/common/http';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { IRModelAccessDisplay } from '../res-group.service';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-res-group-access-cu-dialog',
  templateUrl: './res-group-access-cu-dialog.component.html',
  styleUrls: ['./res-group-access-cu-dialog.component.css']
})
export class ResGroupAccessCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  listModels: IRModelBasic[];
  item: IRModelAccessDisplay;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('modelCbx', { static: true }) modelCbx: ComboBoxComponent;
  constructor(private fb: FormBuilder, private modelService: IRModelService, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      model: [null, Validators.required],
      permRead: true,
      permCreate: true,
      permWrite: true,
      permUnlink: true
    });

    if (this.item) {
      if (this.item.model) {
        this.listModels = _.unionBy(this.listModels, [this.item.model], 'id');
      }
      this.formGroup.patchValue(this.item);
    }

    this.modelCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.modelCbx.loading = true)),
      switchMap(value => this.searchModels(value))
    ).subscribe(result => {
      this.listModels = result.items;
      this.modelCbx.loading = false;
    });


    setTimeout(() => {
      this.nameInput.nativeElement.focus();
    }, 200);
  }

  searchModels(q?: string) {
    return this.modelService.getPaged(new HttpParams().set('filter', q));
  }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    var value = this.formGroup.value;
    value.modelId = value.model.id;
    this.activeModal.close(value);
  }
}
