import { Component, OnInit, Inject, ViewChild, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IRRuleService } from '../ir-rule.service';
import { IRModelBasic } from 'src/app/ir-models/ir-model';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { IRModelService } from 'src/app/ir-models/ir-model.service';
import { HttpParams } from '@angular/common/http';
import * as _ from 'lodash';

@Component({
  selector: 'app-ir-rule-cu-dialog',
  templateUrl: './ir-rule-cu-dialog.component.html',
  styleUrls: ['./ir-rule-cu-dialog.component.css']
})

export class IrRuleCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  @Input() public id: string;

  listModels: IRModelBasic[];
  @ViewChild('modelCbx', { static: true }) modelCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private ruleService: IRRuleService,
    public activeModal: NgbActiveModal, private modelService: IRModelService) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      code: null,
      active: true,
      global: true,
      permRead: true,
      permCreate: true,
      permUnlink: true,
      permWrite: true,
      modelId: null
    });

    if (this.id) {
      this.ruleService.get(this.id).subscribe((result: any) => {
        this.formGroup.patchValue(result);
        if (result.model) {
          this.listModels = _.unionBy(this.listModels, [result.model as IRModelBasic], 'id');
        }
      });
    }

    this.modelCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.modelCbx.loading = true)),
      switchMap(value => this.searchModels(value))
    ).subscribe(result => {
      this.listModels = result.items;
      this.modelCbx.loading = false;
    });

  }

  searchModels(q?: string) {
    return this.modelService.getPaged(new HttpParams().set('filter', q || ''));
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
      return this.ruleService.create(val);
    } else {
      return this.ruleService.update(this.id, val);
    }
  }
}



