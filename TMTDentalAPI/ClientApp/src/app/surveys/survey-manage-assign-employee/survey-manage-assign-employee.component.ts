import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SurveyService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-assign-employee',
  templateUrl: './survey-manage-assign-employee.component.html',
  styleUrls: ['./survey-manage-assign-employee.component.css']
})
export class SurveyManageAssignEmployeeComponent implements OnInit {
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  search: string;
  limit = 10;
  offset = 0;
  dateFrom: Date;
  dateTo: Date;
  loading = false;
  states = [
    { value: "draft", name: "Chưa gọi" },
    { value: "done", name: "Hoàn thành" },
    { value: "contact", name: "Đang liên hệ" }
  ]

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private intlService: IntlService,
    private modalService: NgbModal,
    private surveyService: SurveyService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {

  }

}
