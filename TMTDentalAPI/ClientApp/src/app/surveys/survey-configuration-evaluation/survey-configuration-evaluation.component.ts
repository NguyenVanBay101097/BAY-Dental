import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SurveyConfigurationEvaluationDialogComponent } from '../survey-configuration-evaluation-dialog/survey-configuration-evaluation-dialog.component';
import { SurveyQuestionPaged, SurveyQuestionService } from '../survey-question.service';

@Component({
  selector: 'app-survey-configuration-evaluation',
  templateUrl: './survey-configuration-evaluation.component.html',
  styleUrls: ['./survey-configuration-evaluation.component.css']
})
export class SurveyConfigurationEvaluationComponent implements OnInit {
  gridData: GridDataResult;
  loading = false;
  limit: number = 20;
  offset: number = 0;
  title: string = "Cấu hình đánh giá"
  searchUpdate = new Subject<string>();
  search: string;

  constructor(
    private surveyQuestionService: SurveyQuestionService,
    private modalService: NgbModal,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new SurveyQuestionPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.search = this.search ? this.search : '';
    this.surveyQuestionService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  createQuestion() {
    let modalRef = this.modalService.open(SurveyConfigurationEvaluationDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm câu hỏi';
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

}
