import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, NgZone, OnInit, Renderer2 } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { closest } from '@ng-bootstrap/ng-bootstrap/util/util';
import { GridDataResult, PageChangeEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { fromEvent, Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, tap } from 'rxjs/operators';
import { SurveyConfigurationEvaluationDialogComponent } from '../survey-configuration-evaluation-dialog/survey-configuration-evaluation-dialog.component';
import { SurveyQuestionBasic, SurveyQuestionDisplay, SurveyQuestionPaged, SurveyQuestionService } from '../survey-question.service';

@Component({
  selector: 'app-survey-configuration-evaluation',
  templateUrl: './survey-configuration-evaluation.component.html',
  styleUrls: ['./survey-configuration-evaluation.component.css']
})
export class SurveyConfigurationEvaluationComponent implements OnInit {
  gridData: GridDataResult;
  loading = false;
  questions: SurveyQuestionBasic[] = [];
  limit: number = 20;
  offset: number = 0;
  title: string = "Cấu hình đánh giá"
  searchUpdate = new Subject<string>();
  search: string;

  constructor(
    private surveyQuestionService: SurveyQuestionService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
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
    this.surveyQuestionService.getPaged(val).subscribe(res => {
      this.questions = res.items;
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

  pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }


  onDrop(event: CdkDragDrop<SurveyQuestionBasic[]>) {
    moveItemInArray(this.questions, event.previousIndex, event.currentIndex);
    this.questions.forEach((item, idx) => {
      item.sequence = idx + 1;
    });
  }

}
