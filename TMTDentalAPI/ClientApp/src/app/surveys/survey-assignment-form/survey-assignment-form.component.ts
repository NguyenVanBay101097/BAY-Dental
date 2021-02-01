import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { SurveyCallcontentService } from '../survey-callcontent.service';
import { SurveyAssignmentDisplay, SurveyService } from '../survey.service';

@Component({
  selector: 'app-survey-assignment-form',
  templateUrl: './survey-assignment-form.component.html',
  styleUrls: ['./survey-assignment-form.component.css']
})
export class SurveyAssignmentFormComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  searchUpdate = new Subject<string>();
  surveyAssignment: SurveyAssignmentDisplay = new SurveyAssignmentDisplay();
  loading = false;

  constructor(
    private intlService: IntlService,
    private modalService: NgbModal,
    private surveyService: SurveyService,
    private route: ActivatedRoute,
    private callContentService: SurveyCallcontentService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    
    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id'); 
    });

    // this.id = this.route.snapshot.paramMap.get('id');
    this.formGroup = this.fb.group({
      saleOrderId: null,
      saleOrder: null,
      surveyUserInput: null,
      callContents: this.fb.array([]),
      status: null
    });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    if (this.id) {
      this.surveyService.get(this.id).subscribe(result => {
        debugger
        this.surveyAssignment = result;
        console.log(this.surveyAssignment);
        this.formGroup.patchValue(this.surveyAssignment);
        // let dateOrder = new Date(result.dateOrder);
        // this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        let control = this.formGroup.get('callContents') as FormArray;
        control.clear();
        result.callContents.forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
      });
    }
  }

}
