import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { SurveyCallContentPaged, SurveyCallcontentService } from '../survey-callcontent.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerService } from 'src/app/partners/partner.service';

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
  edit = false;
  loading = false;
  @Input() id: string;
  @Input() surveyStatus: string;
  editedRowIndex: number;
  
  constructor(
    private callcontentService: SurveyCallcontentService,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private fb: FormBuilder
  ) {

  }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new SurveyCallContentPaged();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.assignmentId = this.id ? this.id : '';
    this.callcontentService.getPaged(paged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(this.gridData);
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  // click in cell to edit
  editHandler(rowIndex, dataItem) {
    // this.closeEditor(rowIndex);
    this.formGroup = this.fb.group({
      id: dataItem ? dataItem.id : null,
      name: dataItem ? dataItem.name : '',
      assignmentId: dataItem ? dataItem.assignmentId : null
    });
    this.editedRowIndex = rowIndex;
    // sender.editRow(rowIndex, this.formGroup);
    this.edit = true;
  }

  public addHandler() {
    if (!this.edit) {
      this.formGroup = this.fb.group({
        name: "",
        date: new Date(),
      });
      this.gridData.data.push(this.formGroup.value);
      this.gridData.total += 1;

      this.editedRowIndex = this.gridData.total - 1;
      // setTimeout(() => {
      //   document.getElementById('editundefined').click();
      // }, 50);
      this.edit = true;
      this.editedRowIndex = this.gridData.data.length - 1;
    }
  }

  private closeEditor(rowIndex) {
    // grid.closeRow(rowIndex);
    this.editedRowIndex = undefined;
    this.formGroup = undefined;
  }

  public cancelHandler(i) {
    if(this.edit == true && i == this.editedRowIndex)
     {
      if(!this.formGroup.value.hasOwnProperty('id')) this.gridData.data.pop();

      this.closeEditor(i);
      this.edit = false;
    }
      else {
      return;
    }
  }

  public saveHandler(): void {
    var survey = this.formGroup.value;
    survey.name = survey.name;
    survey.assignmentId = this.id ? this.id : null;
    if (survey.id) {
      this.callcontentService.update(survey.id, survey).subscribe(
        () => {
          this.notificationService.show({
            content: 'Cập nhật thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.loadDataFromApi();
        }
      )
    } else {
      survey.date = this.intlService.formatDate(new Date(), 'yyyy-MM-ddTHH:mm:ss');
      this.callcontentService.create(survey).subscribe(
        () => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.loadDataFromApi();
        }
      )
    }
    this.edit = false;
    // sender.closeRow(rowIndex);
  }



}
