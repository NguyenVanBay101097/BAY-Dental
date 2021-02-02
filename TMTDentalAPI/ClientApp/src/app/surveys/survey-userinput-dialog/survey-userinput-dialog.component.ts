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

  // loadData() {
  //   this.user.get(this.id).subscribe(result => {
  //     this.laboOrder = result;
  //     this.patchValue(result);
  //   });
  // }

  // loadDefault() {
  //   var df = new LaboOrderDefaultGet();
  //   df.saleOrderLineId = this.saleOrderLineId;
  //   this.laboOrderService.defaultGet(df).subscribe(result => {
  //     this.laboOrder = result;
  //     result.quantity = 1;
  //     this.patchValue(result);
  //   (result.saleOrderLine && result.saleOrderLine.product )? this.priceUnitFC.patchValue(result.saleOrderLine.product.laboPrice) : '';
  //   });
  // }

}
