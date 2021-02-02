import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { WebService } from 'src/app/core/services/web.service';
import { SurveyQuestionService } from '../survey-question.service';
import { SurveyUserInputDisplay, SurveyUserinputService } from '../survey-userinput.service';

@Component({
  selector: 'app-survey-userinput-dialog',
  templateUrl: './survey-userinput-dialog.component.html',
  styleUrls: ['./survey-userinput-dialog.component.css']
})
export class SurveyUserinputDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;// có thể là input
  userinput: SurveyUserInputDisplay = new SurveyUserInputDisplay();



  constructor(private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private userInputService: SurveyUserinputService,
    private intlService: IntlService,
    private questionService: SurveyQuestionService,
    private webService: WebService
  ) { }

  ngOnInit() {
    if (this.id) {
      this.loadData();
    } else {
      this.loadDefault();
    }
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

}
