import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, QueryList, SimpleChanges, ViewChildren } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { SurveyCallContentPaged, SurveyCallcontentService } from '../survey-callcontent.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerService } from 'src/app/partners/partner.service';
import { SurveyAssignmentDisplayCallContent } from '../survey.service';

@Component({
  selector: 'app-survey-call-content-list',
  templateUrl: './survey-call-content-list.component.html',
  styleUrls: ['./survey-call-content-list.component.css']
})
export class SurveyCallContentListComponent implements OnInit {
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  formGroup: FormGroup;
  search: string;
  limit = 0;
  offset = 0;
  @Input() view = false;
  isEditing = false;
  loading = false;
  @Input() id: string;
  @Input() surveyStatus: string;
  editedRowIndex: number;
  @Output() addBtnEvent = new EventEmitter<any>();
  @Output() cancelBtnEvent = new EventEmitter<any>();
  @Output() saveBtnEvent = new EventEmitter<any>();
  @Output() removeBtnEvent = new EventEmitter<any>();

  @Input() data: SurveyAssignmentDisplayCallContent[] = [];
  @ViewChildren('nameInput') nameInputs: QueryList<ElementRef>;
  
  constructor(
    private callcontentService: SurveyCallcontentService,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private fb: FormBuilder
  ) {

  }

  ngOnInit() {
  }

  // click in cell to edit
  editHandler(rowIndex, dataItem) {
    if (this.isEditing) {
      return false;
    }

    this.formGroup = this.fb.group({
      name: dataItem.name,
    });

    this.focusEditingInput();

    this.editedRowIndex = rowIndex;
    this.isEditing = true;
  }

  focusEditingInput() {
    setTimeout(() => {
      this.nameInputs.first.nativeElement.focus();
    }, 200);
  }

  public addHandler() {
    if (!this.isEditing) {
      this.isEditing = true;
      this.editedRowIndex = this.data.length;
      this.formGroup = this.fb.group({
        name: [null, Validators.required]
      });

      this.addBtnEvent.emit(null);
      this.focusEditingInput();
    }
  }

  private closeEditor() {
    this.isEditing = false;
    this.editedRowIndex = undefined;
    this.formGroup = undefined;
  }

  public cancelHandler(item, index) {
    if (item.id) {
      this.closeEditor();
    } else {
      this.cancelBtnEvent.emit({index: index});
      this.closeEditor();
    }
  }

  removeHandler(index, item) {
    this.removeBtnEvent.emit(index);
  }

  public saveHandler(index) {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    this.closeEditor();
    this.saveBtnEvent.emit({
      data: value,
      index: index
    });
  }
}
