import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, NgZone, OnInit, Renderer2 } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { closest } from '@ng-bootstrap/ng-bootstrap/util/util';
import { GridDataResult, PageChangeEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { fromEvent, Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, tap } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
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

  editQuestion(item) {
    let modalRef = this.modalService.open(SurveyConfigurationEvaluationDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa câu hỏi';
    modalRef.componentInstance.id = item.id
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  doubleQuestion(item) {
    this.surveyQuestionService.doublicateQuestion(item.id).subscribe(
      () => {
        this.loadDataFromApi();
        this.notificationService.show({
          content: 'Sao chép thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }
    )
  }

  removeQuestion(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa câu hỏi';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa câu hỏi ?';

    modalRef.result.then(() => {
      this.surveyQuestionService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      })
    })
  }

  onDrop(event: CdkDragDrop<any[]>) {
    moveItemInArray(this.questions, event.previousIndex, event.currentIndex);
    this.questions.forEach((item, idx) => {
      item.sequence = idx + 1;
    });
    this.surveyQuestionService.updateListSequence(this.questions).subscribe(
      () => {
        this.notificationService.show({
          content: 'Sắp xếp thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }
    )

  }

}
