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
  limit: number = 0;
  offset: number = 0;
  title: string = "Câu hỏi khảo sát"
  searchUpdate = new Subject<string>();
  search: string;
  active: boolean = true;
  listFilter: Array<any> = [
    {name: 'Câu hỏi hiện', value: true},
    {name: 'Câu hỏi ẩn', value: false},
  ];
  defaultFilter = this.listFilter[0];

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
    val.active = (this.active || this.active == false) ? this.active : '';
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
    this.surveyQuestionService.duplicate(item.id).subscribe(
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
    modalRef.componentInstance.body = `Bạn có chắc chắn muốn xóa câu hỏi "${item.name}"?`;

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
    if (event.previousIndex == event.currentIndex) {
      return false;
    }

    moveItemInArray(this.questions, event.previousIndex, event.currentIndex);
    var offset = Math.min(event.previousIndex, event.currentIndex);
    var ids = this.questions.slice(offset, Math.max(event.previousIndex, event.currentIndex) + 1).map(x => x.id);
 
    this.surveyQuestionService.resequence({ offset: offset, ids: ids }).subscribe(
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

  actionActive(item: SurveyQuestionBasic) {
    var val = {
      id: item.id,
      active: !item.active
    }
    console.log(val);
    

    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = item.active?'Ẩn câu hỏi':'Hiện câu hỏi';
    modalRef.componentInstance.body = `Bạn có chắc chắn muốn ${item.active? 'ẩn câu hỏi':'hiện câu hỏi'}?`;

    modalRef.result.then(() => {
      this.surveyQuestionService.actionActive(val).subscribe(
        (res:any) => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
  
          this.loadDataFromApi();
        }
      );
    })
   
  }

  onFilterChange(e){
    this.active = e ? e.value : null;
    this.loadDataFromApi();

  }

}
